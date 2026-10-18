using System;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder
{
	public static class StaticCodeBookExtensions
	{
		public static int GetQuantVals(this IStaticCodeBook book)
		{
			int num = (int)Math.Floor(Math.Pow(book.LengthList.Length, 1f / (float)book.Dimensions));
			while (true)
			{
				int num2 = 1;
				int num3 = 1;
				for (int i = 0; i < book.Dimensions; i++)
				{
					num2 *= num;
					num3 *= num + 1;
				}
				if (num2 <= book.LengthList.Length && num3 > book.LengthList.Length)
				{
					break;
				}
				num = ((num2 <= book.LengthList.Length) ? (num + 1) : (num - 1));
			}
			return num;
		}
	}
}
