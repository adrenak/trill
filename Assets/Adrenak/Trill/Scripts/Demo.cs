using UnityEngine;
using UnityEngine.UI;

namespace Adrenak.Trill {
	public class Demo : MonoBehaviour {
		[SerializeField] InputField m_Input;
		[SerializeField] Text m_Message;

		TrillEmitter m_Emitter;

		void Start () {
			Application.runInBackground = true;
			m_Emitter = TrillEmitter.New();
		}

		public void Emit() {
			var input = m_Input.text;
			m_Emitter.EmitString(input);
		}

		public void Listen() {
			TrillDetector d = TrillDetector.New();
			d.StartDetecting();
			d.OnGetMessage += bytes => {
				m_Message.text = TrillUtils.BytesToString(bytes);
			};
		}
	}
}
