using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Adrenak.Trill {
	public class TrillDetector : MonoBehaviour {
		public enum DetectorState {
			WAITING,
			READING
		}

		public event Action<byte[]> OnGetMessage;
		public DetectorState State { get; private set; }
		public bool DebugMode { get; set; }

		TrillListener m_Listener;
		List<byte> m_ByteHistory = new List<byte>();

		// Prevent "new" construction of MonoBehaviour
		TrillDetector() { }

		public static TrillDetector New() {
			var go = new GameObject("Trill Detector");
			DontDestroyOnLoad(go);
			var instance = go.AddComponent<TrillDetector>();

			instance.State = DetectorState.WAITING;
			instance.m_Listener = TrillListener.New();
			instance.m_Listener.OnGetByte += instance.HandleGetByte;

			return instance;
		}

		public void StartDetecting() {
			m_Listener.StartListening();
		}

		public void StopDetecting() {
			m_Listener.StopListening();
		}

		void HandleGetByte(byte b) {
			Debug.Log(b);
			m_ByteHistory.Add(b);

			// Clip the history to a very large count
			if (m_ByteHistory.Count > 1024)
				m_ByteHistory.RemoveAt(0);

			switch (State) {
				case DetectorState.WAITING:
					if (m_ByteHistory.Count < TrillK.StartPattern.Length) return;

					bool started = true;
					var head = m_ByteHistory.GetRange(m_ByteHistory.Count - TrillK.StartPattern.Length, TrillK.StartPattern.Length);

					for (int i = 0; i < head.Count; i++) {
						if (head[i] != TrillK.StartPattern[i]) {
							started = false;
							break;
						}
					}

					if (started) {
						m_ByteHistory.Clear();
						TrillK.StartPattern.ToList().ForEach(x => m_ByteHistory.Add(x));
						State = DetectorState.READING;
					}
					break;
				case DetectorState.READING:
					if (m_ByteHistory.Count < TrillK.StartPattern.Length + TrillK.EndPattern.Length)
						return;

					var tail = m_ByteHistory.GetRange(m_ByteHistory.Count - TrillK.EndPattern.Length, TrillK.EndPattern.Length);

					bool ended = true;
					for (int i = 0; i < tail.Count; i++) {
						if (tail[i] != TrillK.EndPattern[i]) {
							ended = false;
							break;
						}
					}

					if (!ended) return;

					while (TrillK.StartPattern.Contains(m_ByteHistory[0])) 
						m_ByteHistory.RemoveAt(0);

					m_ByteHistory.RemoveRange(m_ByteHistory.Count - TrillK.EndPattern.Length, TrillK.EndPattern.Length);
					var msg = m_ByteHistory.ToArray();
					if (msg.Length == 0) return;

					if (OnGetMessage != null) OnGetMessage(msg);

					m_ByteHistory.Clear();
					State = DetectorState.WAITING;
					break;
			}
		}
	}
}
