using System;

namespace OggVorbisEncoder.Setup
{
	public class CodeBook
	{
		public int Dimensions { get; }

		public int Entries { get; }

		public int UsedEntries { get; }

		public IStaticCodeBook StaticBook { get; }

		public float[] ValueList { get; }

		public uint[] CodeList { get; }

		public int[] DecIndex { get; }

		public byte[] DecCodeLengths { get; }

		public uint[] DecFirstTable { get; }

		public int DecFirstTableN { get; }

		public int DecMaxLength { get; }

		public int QuantValues { get; }

		public int MinVal { get; }

		public int Delta { get; }

		public CodeBook(int dimensions, int entries, int usedEntries, IStaticCodeBook staticBook, float[] valueList, uint[] codeList, int[] decIndex, byte[] decCodeLengths, uint[] decFirstTable, int decFirstTableN, int decMaxLength, int quantValues, int minVal, int delta)
		{
			Dimensions = dimensions;
			Entries = entries;
			UsedEntries = usedEntries;
			StaticBook = staticBook;
			ValueList = valueList;
			CodeList = codeList;
			DecIndex = decIndex;
			DecCodeLengths = decCodeLengths;
			DecFirstTable = decFirstTable;
			DecFirstTableN = decFirstTableN;
			DecMaxLength = decMaxLength;
			QuantValues = quantValues;
			MinVal = minVal;
			Delta = delta;
		}

		public static CodeBook InitEncode(IStaticCodeBook source)
		{
			return new CodeBook(source.Dimensions, source.LengthList.Length, source.LengthList.Length, source, null, Encoding.MakeWords(source.LengthList, 0), null, null, null, 0, 0, source.GetQuantVals(), (int)Math.Round(Encoding.UnpackFloat(source.QuantMin)), (int)Math.Round(Encoding.UnpackFloat(source.QuantDelta)));
		}
	}
}
