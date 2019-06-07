using System;
using System.Linq;
using UnityEngine;
using Adrenak.UniMic;
using System.Collections.Generic;

namespace Adrenak.Trill {
	public class TrillListener : MonoBehaviour {
		class FrequencyEntry {
			public int frequency;
			public float time;
		}

		public event Action<byte> OnGetByte;
		public bool IsRunning { get; private set; }

		List<FrequencyEntry> m_History = new List<FrequencyEntry>();
		Mic mic;
		float last;

		// Prevent "new" construction of MonoBehaviour
		TrillListener() { }

		public static TrillListener New() {
			var go = new GameObject("Trill Listener");
			DontDestroyOnLoad(go);
			var instance = go.AddComponent<TrillListener>();
			instance.mic = Mic.Instance;
			instance.mic.StartRecording(AudioSettings.outputSampleRate, 10);
			return instance;
		}

		public void StartListening() {
			IsRunning = true;
			last = Time.unscaledTime;
		}

		public void StopListening() {
			IsRunning = false;
		}

		void Update() {
			if (!IsRunning) return;

			var spectrum = mic.GetSpectrumData(FFTWindow.BlackmanHarris, TrillK.FFTSamples);
			var freqIndex = spectrum.MaxIndex();
			var frequency = (int)(freqIndex * AudioSettings.outputSampleRate / 2f / TrillK.FFTSamples);
			m_History.Add(new FrequencyEntry() {
				frequency = frequency,
				time = Time.unscaledTime
			});

			if (m_History.Count > 1024)
				m_History.RemoveAt(0);

			CheckBuffer();
		}

		void CheckBuffer() {
			// If the history is not older than duration, we wait
			if (m_History[0].time > Time.unscaledTime - TrillK.PulseDuration) return;

			// We collect all history values that have come after the last capture
			var frame = m_History.Where(x => x.time > last).ToList()
				// Are within the frequency range
				.Where(x => x.frequency > TrillK.BottomFrequency && x.frequency < TrillK.TopFrequency)
				// And are within the last Pulse Duration 
				.Where(x => Time.unscaledTime - x.time < TrillK.PulseDuration).ToList();

			if (frame.Count() == 0) return;

			var mostOccuringBand = frame.GroupBy(x => TrillUtils.FrequencyToByte(x.frequency))
				.OrderByDescending(x => x.Count())
				.Select(x => x.Key).ToList()[0];

			// store all the history entries that are in the most occuring frequency band
			var entriesOfMOF = frame.Where(x => TrillUtils.FrequencyToByte(x.frequency) == mostOccuringBand);

			if (
				entriesOfMOF.Count() > frame.Count * .5f 
				&&
				Time.unscaledTime > last + TrillK.PulseDuration
			) {
				var band = mostOccuringBand;
				last = Time.unscaledTime;
				if (OnGetByte != null) OnGetByte(band);
			}
		}
	}
}
