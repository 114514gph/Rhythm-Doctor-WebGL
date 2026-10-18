using System.Runtime.InteropServices;

namespace MP3Sharp.Decoding
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct DecoderErrors
	{
		public static readonly int UNKNOWN_ERROR = BitstreamErrors.DECODER_ERROR;

		public static readonly int UNSUPPORTED_LAYER = BitstreamErrors.DECODER_ERROR + 1;
	}
}
