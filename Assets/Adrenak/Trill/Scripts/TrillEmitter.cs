using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Adrenak.Trill {
	[RequireComponent(typeof(AudioSource))]
	public class TrillEmitter : MonoBehaviour {
		public bool IsEmitting { get; private set; }
		public float Frequency { get; private set; }

		int m_TimeIndex = 0;
		int m_SampleRate;

		// Prevent "new" construction of MonoBehaviour
		TrillEmitter() { }

		public static TrillEmitter New() {
			var go = new GameObject("Trill Emitter");
			var e = go.AddComponent<TrillEmitter>();
			e.m_SampleRate = AudioSettings.outputSampleRate;
			go.hideFlags = HideFlags.HideAndDontSave;
			var audioSource = go.AddComponent<AudioSource>();
			audioSource.playOnAwake = true;
			audioSource.spatialBlend = 0;
			audioSource.Play();
			return e;
		}

		public void EmitString(char[] chars) {
			EmitString(new string(chars));
		}

		public void EmitString(string str) {
			EmitBytes(TrillUtils.StringToBytes(str));
		}

		public void EmitBytes(byte[] bytes) {
			List<byte> payload = new List<byte>();
			TrillK.StartPattern.ToList().ForEach(x => payload.Add(x));
			bytes.ToList().ForEach(x => payload.Add(x));
			TrillK.EndPattern.ToList().ForEach(x => payload.Add(x));

			payload.ForEach(x => Debug.Log(x));
			StartCoroutine(EmitCo(payload.Select(x => TrillUtils.ByteToFrequency(x)).ToArray()));
		}
		
		IEnumerator EmitCo(int[] sequence) {
			IsEmitting = true;
			yield return null;
			for (int i = 0; i < sequence.Length; i++) {
				Frequency = sequence[i];
				yield return new WaitForSeconds(TrillK.PulseDuration);
			}
			IsEmitting = false;
		}

		void OnAudioFilterRead(float[] data, int channels) {
			if (!IsEmitting) return;
			for (int i = 0; i < data.Length; i += channels) {
				data[i] = CreateSine(Frequency);
				m_TimeIndex++;
			}
		}

		public float CreateSine(float frequency) {
			return Mathf.Sin(2 * Mathf.PI * m_TimeIndex * frequency / m_SampleRate);
		}
	}
}
