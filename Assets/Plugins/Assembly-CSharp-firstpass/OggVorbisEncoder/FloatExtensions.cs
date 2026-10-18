using System;

namespace OggVorbisEncoder
{
	public static class FloatExtensions
	{
		public static float ToDecibel(this float x)
		{
			return (float)(BitConverter.ToUInt32(BitConverter.GetBytes(x), 0) & 0x7FFFFFFF) * 7.1771143E-07f - 764.6162f;
		}
	}
}
