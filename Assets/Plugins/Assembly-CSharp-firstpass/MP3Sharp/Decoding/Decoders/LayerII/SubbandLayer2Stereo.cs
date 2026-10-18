namespace MP3Sharp.Decoding.Decoders.LayerII
{
	internal class SubbandLayer2Stereo : SubbandLayer2
	{
		protected internal int channel2_allocation;

		protected internal float[] channel2_c = new float[1];

		protected internal int[] channel2_codelength = new int[1];

		protected internal float[] channel2_d = new float[1];

		protected internal float[] channel2_factor = new float[1];

		protected internal float[] channel2_samples;

		protected internal float channel2_scalefactor1;

		protected internal float channel2_scalefactor2;

		protected internal float channel2_scalefactor3;

		protected internal int channel2_scfsi;

		public SubbandLayer2Stereo(int subbandnumber)
			: base(subbandnumber)
		{
			channel2_samples = new float[3];
		}

		public override void ReadBitAllocation(Bitstream stream, Header header, Crc16 crc)
		{
			int num = get_allocationlength(header);
			allocation = stream.GetBitsFromBuffer(num);
			channel2_allocation = stream.GetBitsFromBuffer(num);
			if (crc != null)
			{
				crc.add_bits(allocation, num);
				crc.add_bits(channel2_allocation, num);
			}
		}

		public override void read_scalefactor_selection(Bitstream stream, Crc16 crc)
		{
			if (allocation != 0)
			{
				scfsi = stream.GetBitsFromBuffer(2);
				crc?.add_bits(scfsi, 2);
			}
			if (channel2_allocation != 0)
			{
				channel2_scfsi = stream.GetBitsFromBuffer(2);
				crc?.add_bits(channel2_scfsi, 2);
			}
		}

		public override void ReadScaleFactor(Bitstream stream, Header header)
		{
			base.ReadScaleFactor(stream, header);
			if (channel2_allocation != 0)
			{
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
				prepare_sample_reading(header, channel2_allocation, 1, channel2_factor, channel2_codelength, channel2_c, channel2_d);
			}
		}

		public override bool ReadSampleData(Bitstream stream)
		{
			bool result = base.ReadSampleData(stream);
			if (channel2_allocation != 0)
			{
				if (groupingtable[1] != null)
				{
					int bitsFromBuffer = stream.GetBitsFromBuffer(channel2_codelength[0]);
					bitsFromBuffer += bitsFromBuffer << 1;
					float[] array = channel2_samples;
					float[] array2 = groupingtable[1];
					int num = 0;
					int num2 = bitsFromBuffer;
					array[num] = array2[num2];
					num2++;
					num++;
					array[num] = array2[num2];
					num2++;
					num++;
					array[num] = array2[num2];
					return result;
				}
				channel2_samples[0] = (float)((double)((float)stream.GetBitsFromBuffer(channel2_codelength[0]) * channel2_factor[0]) - 1.0);
				channel2_samples[1] = (float)((double)((float)stream.GetBitsFromBuffer(channel2_codelength[0]) * channel2_factor[0]) - 1.0);
				channel2_samples[2] = (float)((double)((float)stream.GetBitsFromBuffer(channel2_codelength[0]) * channel2_factor[0]) - 1.0);
			}
			return result;
		}

		public override bool PutNextSample(int channels, SynthesisFilter filter1, SynthesisFilter filter2)
		{
			bool result = base.PutNextSample(channels, filter1, filter2);
			if (channel2_allocation != 0 && channels != OutputChannels.LEFT_CHANNEL)
			{
				float num = channel2_samples[samplenumber - 1];
				if (groupingtable[1] == null)
				{
					num = (num + channel2_d[0]) * channel2_c[0];
				}
				num = ((groupnumber <= 4) ? (num * channel2_scalefactor1) : ((groupnumber > 8) ? (num * channel2_scalefactor3) : (num * channel2_scalefactor2)));
				if (channels == OutputChannels.BOTH_CHANNELS)
				{
					filter2.WriteSample(num, subbandnumber);
					return result;
				}
				filter1.WriteSample(num, subbandnumber);
			}
			return result;
		}
	}
}
