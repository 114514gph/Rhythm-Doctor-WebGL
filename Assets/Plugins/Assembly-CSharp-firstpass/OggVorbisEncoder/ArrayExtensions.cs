using System;
using System.Collections.Generic;
using System.Linq;

namespace OggVorbisEncoder
{
	internal static class ArrayExtensions
	{
		public static TElement[] ToFixedLength<TElement>(this TElement[] input, int fixedLength)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (input.Length == fixedLength)
			{
				return input;
			}
			if (input.Length > fixedLength)
			{
				throw new IndexOutOfRangeException(string.Format("{0} of size [{1}] is greater than {2} of [{3}]", "input", input.Length, "fixedLength", fixedLength));
			}
			int count = fixedLength - input.Length;
			IEnumerable<TElement> second = Enumerable.Range(0, count).Select((Func<int, TElement>)((int s) => default));
			return input.Concat(second).ToArray();
		}
	}
}
