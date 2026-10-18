namespace MP3Sharp.Decoding.Decoders
{
	internal abstract class ASubband
	{
		public static readonly float[] ScaleFactors = new float[64]
		{
			2f,
			1.587401f,
			1.2599211f,
			1f,
			0.7937005f,
			0.62996054f,
			0.5f,
			0.39685026f,
			0.31498027f,
			0.25f,
			0.19842513f,
			0.15749013f,
			0.125f,
			0.099212565f,
			0.07874507f,
			0.0625f,
			0.049606282f,
			0.039372534f,
			1f / 32f,
			0.024803141f,
			0.019686267f,
			1f / 64f,
			0.012401571f,
			0.009843133f,
			1f / 128f,
			0.0062007853f,
			0.0049215667f,
			1f / 256f,
			0.0031003926f,
			0.0024607833f,
			2f / 1024f,
			0.0015501963f,
			0.0012303917f,
			1f / 1024f,
			0.00077509816f,
			0.00061519584f,
			2f / 4096f,
			0.00038754908f,
			0.00030759792f,
			1f / 4096f,
			0.00019377454f,
			0.00015379896f,
			1f / 8192f,
			9.688727E-05f,
			7.689948E-05f,
			1f / 16384f,
			4.8443635E-05f,
			3.844974E-05f,
			1f / 32768f,
			2.4221818E-05f,
			1.922487E-05f,
			1f / 65536f,
			1.2110909E-05f,
			9.612435E-06f,
			8f / 1048576f,
			6.0554544E-06f,
			4.8062175E-06f,
			4f / 1048576f,
			3.0277272E-06f,
			2.4031087E-06f,
			2f / 1048576f,
			1.5138636E-06f,
			1.2015544E-06f,
			0f
		};

		public abstract void ReadBitAllocation(Bitstream stream, Header header, Crc16 crc);

		public abstract void ReadScaleFactor(Bitstream stream, Header header);

		public abstract bool ReadSampleData(Bitstream stream);

		public abstract bool PutNextSample(int channels, SynthesisFilter filter1, SynthesisFilter filter2);
	}
}
