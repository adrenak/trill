using System;
using System.Collections.Generic;

namespace Adrenak.Trill {
	public static class TrillExtensions {
		public static int MaxIndex<T>(this IEnumerable<T> sequence)	where T : IComparable<T> {
			int maxIndex = -1;
			T maxValue = default(T);

			int index = 0;
			foreach (T value in sequence) {
				if (value.CompareTo(maxValue) > 0 || maxIndex == -1) {
					maxIndex = index;
					maxValue = value;
				}
				index++;
			}
			return maxIndex;
		}
	}
}
