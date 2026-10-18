namespace MP3Sharp.Decoding.Decoders.LayerII
{
	internal class SubbandLayer2IntensityStereo : SubbandLayer2
	{
		protected internal float channel2_scalefactor1;

		protected internal float channel2_scalefactor2;

		protected internal float channel2_scalefactor3;

		protected internal int channel2_scfsi;

		public SubbandLayer2IntensityStereo(int subbandnumber)
			: base(subbandnumber)
		{
		}

		public override void ReadBitAllocation(Bitstream stream, Header header, Crc16 crc)
		{
			base.ReadBitAllocation(stream, header, crc);
		}

		public override void read_scalefactor_selection(Bitstream stream, Crc16 crc)
		{
			if (allocation != 0)
			{
				scfsi = stream.GetBitsFromBuffer(2);
				channel2_scfsi = stream.GetBitsFromBuffer(2);
				if (crc != null)
				{
					crc.add_bits(scfsi, 2);
					crc.add_bits(channel2_scfsi, 2);
				}
			}
		}

		public override void ReadScaleFactor(Bitstream stream, Header header)
		{
			if (allocation != 0)
			{
				base.ReadScaleFactor(stream, header);
				switch (channel2_scfsi)
				{
				case 0:
					channel2_scalefactor1 = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
					channel2_scalefactor2 = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
					channel2_scalefactor3 = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
					break;
				case 1:
					channel2_scalefactor1 = (channel2_scalefactor2 = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)]);
					channel2_scalefactor3 = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
					break;
				case 2:
					channel2_scalefactor1 = (channel2_scalefactor2 = (channel2_scalefactor3 = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)]));
					break;
				case 3:
					channel2_scalefactor1 = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
					channel2_scalefactor2 = (channel2_scalefactor3 = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)]);
					break;
				}
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
				float num = samples[samplenumber];
				if (groupingtable[0] == null)
				{
					num = (num + d[0]) * c[0];
				}
				if (channels == OutputChannels.BOTH_CHANNELS)
				{
					float num2 = num;
					if (groupnumber <= 4)
					{
						num *= scalefactor1;
						num2 *= channel2_scalefactor1;
					}
					else if (groupnumber <= 8)
					{
						num *= scalefactor2;
						num2 *= channel2_scalefactor2;
					}
					else
					{
						num *= scalefactor3;
						num2 *= channel2_scalefactor3;
					}
					filter1.WriteSample(num, subbandnumber);
					filter2.WriteSample(num2, subbandnumber);
				}
				else if (channels == OutputChannels.LEFT_CHANNEL)
				{
					num = ((groupnumber <= 4) ? (num * scalefactor1) : ((groupnumber > 8) ? (num * scalefactor3) : (num * scalefactor2)));
					filter1.WriteSample(num, subbandnumber);
				}
				else
				{
					num = ((groupnumber <= 4) ? (num * channel2_scalefactor1) : ((groupnumber > 8) ? (num * channel2_scalefactor3) : (num * channel2_scalefactor2)));
					filter1.WriteSample(num, subbandnumber);
				}
			}
			if (++samplenumber == 3)
			{
				return true;
			}
			return false;
		}
	}
}
