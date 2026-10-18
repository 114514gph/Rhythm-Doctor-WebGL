using System.Runtime.InteropServices;

namespace MP3Sharp.Decoding
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct BitstreamErrors
	{
		public static readonly int UNKNOWN_ERROR = BITSTREAM_ERROR;

		public static readonly int UNKNOWN_SAMPLE_RATE = BITSTREAM_ERROR + 1;

		public static readonly int STREAM_ERROR = BITSTREAM_ERROR + 2;

		public static readonly int UNEXPECTED_EOF = BITSTREAM_ERROR + 3;

		public static readonly int STREAM_EOF = BITSTREAM_ERROR + 4;

		public static readonly int BITSTREAM_LAST = 511;

		public static readonly int BITSTREAM_ERROR = 256;

		public static readonly int DECODER_ERROR = 512;
	}
}
