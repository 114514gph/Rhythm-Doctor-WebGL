using System;
using System.IO;

namespace MP3Sharp.Support
{
	internal class SupportClass
	{
		public static int URShift(int number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2 << ~bits);
		}

		public static int URShift(int number, long bits)
		{
			return URShift(number, (int)bits);
		}

		public static long URShift(long number, int bits)
		{
			if (number >= 0)
			{
				return number >> bits;
			}
			return (number >> bits) + (2L << ~bits);
		}

		public static long URShift(long number, long bits)
		{
			return URShift(number, (int)bits);
		}

		public static void WriteStackTrace(Exception throwable, TextWriter stream)
		{
			stream.Write(throwable.StackTrace);
			stream.Flush();
		}

		public static long Identity(long literal)
		{
			return literal;
		}

		public static ulong Identity(ulong literal)
		{
			return literal;
		}

		public static float Identity(float literal)
		{
			return literal;
		}

		public static double Identity(double literal)
		{
			return literal;
		}

		public static int ReadInput(Stream sourceStream, ref sbyte[] target, int start, int count)
		{
			byte[] array = new byte[target.Length];
			int num = sourceStream.Read(array, start, count);
			for (int i = start; i < start + num; i++)
			{
				target[i] = (sbyte)array[i];
			}
			return num;
		}

		public static byte[] ToByteArray(sbyte[] sbyteArray)
		{
			byte[] array = new byte[sbyteArray.Length];
			for (int i = 0; i < sbyteArray.Length; i++)
			{
				array[i] = (byte)sbyteArray[i];
			}
			return array;
		}

		public static byte[] ToByteArray(string sourceString)
		{
			byte[] array = new byte[sourceString.Length];
			for (int i = 0; i < sourceString.Length; i++)
			{
				array[i] = (byte)sourceString[i];
			}
			return array;
		}

		public static void GetSBytesFromString(string sourceString, int sourceStart, int sourceEnd, ref sbyte[] destinationArray, int destinationStart)
		{
			int num = sourceStart;
			int num2 = destinationStart;
			while (num < sourceEnd)
			{
				destinationArray[num2] = (sbyte)sourceString[num];
				num++;
				num2++;
			}
		}
	}
}
