using System.Text;

namespace Adrenak.Trill {
	public class TrillUtils {
		public static int ByteToFrequency(byte b) {
			return TrillK.BottomFrequency + TrillK.Bandwidth * b + TrillK.Bandwidth / 2;
		}

		public static byte FrequencyToByte(int frequency) {
			if (frequency < TrillK.BottomFrequency) return 0;
			if (frequency > TrillK.BottomFrequency + TrillK.Bandwidth * 256) return 255;

			return (byte)((frequency - TrillK.BottomFrequency) / TrillK.Bandwidth);
		}

		public static byte[] StringToBytes(string str) {
			return Encoding.UTF8.GetBytes(str);
		}

		public static string BytesToString(byte[] bytes) {
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
