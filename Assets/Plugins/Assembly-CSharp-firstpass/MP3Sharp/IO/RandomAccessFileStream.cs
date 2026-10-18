using System;
using System.IO;

namespace MP3Sharp.IO
{
	internal class RandomAccessFileStream
	{
		public static FileStream CreateRandomAccessFile(string fileName, string mode)
		{
			FileStream fileStream = null;
			if (mode.CompareTo("rw") == 0)
			{
				return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			}
			if (mode.CompareTo("r") == 0)
			{
				return new FileStream(fileName, FileMode.Open, FileAccess.Read);
			}
			throw new ArgumentException();
		}
	}
}
