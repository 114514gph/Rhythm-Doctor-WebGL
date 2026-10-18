namespace MP3Sharp.Decoding.Decoders.LayerI
{
	internal class SubbandLayer1IntensityStereo : SubbandLayer1
	{
		protected internal float channel2_scalefactor;

		public SubbandLayer1IntensityStereo(int subbandnumber)
			: base(subbandnumber)
		{
		}

		public override void ReadBitAllocation(Bitstream stream, Header header, Crc16 crc)
		{
			base.ReadBitAllocation(stream, header, crc);
		}

		public override void ReadScaleFactor(Bitstream stream, Header header)
		{
			if (allocation != 0)
			{
				scalefactor = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
				channel2_scalefactor = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
			}
		}

		public override bool ReadSampleData(Bitstream stream)
		{
			return base.ReadSampleData(stream);
		}

		public override bool PutNextSample(int channels, SynthesisFilter filter1, SynthesisFilter filter2)
		{
			if (allocation != 0)
			{
				sample = sample * factor + offset;
				if (channels == OutputChannels.BOTH_CHANNELS)
				{
					float num = sample * scalefactor;
					float num2 = sample * channel2_scalefactor;
					filter1.WriteSample(num, subbandnumber);
					filter2.WriteSample(num2, subbandnumber);
				}
				else if (channels == OutputChannels.LEFT_CHANNEL)
				{
					float num3 = sample * scalefactor;
					filter1.WriteSample(num3, subbandnumber);
				}
				else
				{
					float num4 = sample * channel2_scalefactor;
					filter1.WriteSample(num4, subbandnumber);
				}
			}
			return true;
		}
	}
}
