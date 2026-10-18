namespace MP3Sharp.Decoding.Decoders.LayerI
{
	internal class SubbandLayer1 : ASubband
	{
		public static readonly float[] TableFactor = new float[15]
		{
			0f,
			2f / 3f,
			0.2857143f,
			2f / 15f,
			66f / 1023f,
			2f / 63f,
			2f / 127f,
			2f / 255f,
			0.0039138943f,
			2f / 1023f,
			0.0009770396f,
			2f / 4095f,
			0.00024417043f,
			0.00012207776f,
			2f / 32767f
		};

		public static readonly float[] TableOffset = new float[15]
		{
			0f,
			-2f / 3f,
			-0.8571429f,
			-0.9333334f,
			-990f / 1023f,
			-0.98412704f,
			-126f / 127f,
			-0.9960785f,
			-0.99804306f,
			-0.9990225f,
			-0.9995115f,
			-0.99975586f,
			-0.9998779f,
			-0.99993896f,
			-0.9999695f
		};

		protected int allocation;

		protected float factor;

		protected float offset;

		protected float sample;

		protected int samplelength;

		protected int samplenumber;

		protected float scalefactor;

		protected int subbandnumber;

		public SubbandLayer1(int subbandnumber)
		{
			this.subbandnumber = subbandnumber;
			samplenumber = 0;
		}

		public override void ReadBitAllocation(Bitstream stream, Header header, Crc16 crc)
		{
			int num = (allocation = stream.GetBitsFromBuffer(4));
			_ = 15;
			crc?.add_bits(allocation, 4);
			if (allocation != 0 && allocation < TableFactor.Length && allocation < TableOffset.Length)
			{
				samplelength = allocation + 1;
				factor = TableFactor[allocation];
				offset = TableOffset[allocation];
			}
		}

		public override void ReadScaleFactor(Bitstream stream, Header header)
		{
			if (allocation != 0)
			{
				scalefactor = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
			}
		}

		public override bool ReadSampleData(Bitstream stream)
		{
			if (allocation != 0)
			{
				sample = stream.GetBitsFromBuffer(samplelength);
			}
			if (++samplenumber == 12)
			{
				samplenumber = 0;
				return true;
			}
			return false;
		}

		public override bool PutNextSample(int channels, SynthesisFilter filter1, SynthesisFilter filter2)
		{
			if (allocation != 0 && channels != OutputChannels.RIGHT_CHANNEL)
			{
				float num = (sample * factor + offset) * scalefactor;
				filter1.WriteSample(num, subbandnumber);
			}
			return true;
		}
	}
}
