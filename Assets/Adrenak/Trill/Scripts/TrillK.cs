namespace Adrenak.Trill {
	public class TrillK {
		/// <summary>
		/// A prefix when any data transfer starts to mark the beginning of the data
		/// </summary>
		public static readonly byte[] StartPattern = new byte[] { 134, 255 };

		/// <summary>
		/// A suffix when any data transfer end to mark the end of the data
		/// </summary>
		public static readonly byte[] EndPattern = new byte[] { 218, 222 };

		/// <summary>
		/// The lowest frequency sound that will be used. 2000 is recommended
		/// </summary>
		public static readonly int BottomFrequency = 2000;

		/// <summary>
		/// The top frequency sound that will be used.
		/// </summary>
		public static int TopFrequency {
			get { return BottomFrequency + 256 * Bandwidth; }
		}

		/// <summary>
		/// The frequency range that will be used per byte (0 to 255)
		/// Low values lead to less accuracy but carry more data (and vice-versa)
		/// </summary>
		public static readonly int Bandwidth = 16;

		/// <summary>
		/// Duration of a single frequency. High values lead to more accuracy and vice-versa
		/// </summary>
		public const float PulseDuration = .5f;

		/// <summary>
		/// The number of samples returned when performing FFT. Higher values increases quality, 
		/// but it quickly ends up in diminishing returns
		/// </summary>
		public const int FFTSamples = 2048;
	}
}
