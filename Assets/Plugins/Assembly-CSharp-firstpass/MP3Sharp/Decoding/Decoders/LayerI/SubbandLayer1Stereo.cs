namespace MP3Sharp.Decoding.Decoders.LayerI
{
	internal class SubbandLayer1Stereo : SubbandLayer1
	{
		protected internal int channel2_allocation;

		protected internal float channel2_factor;

		protected internal float channel2_offset;

		protected internal float channel2_sample;

		protected internal int channel2_samplelength;

		protected internal float channel2_scalefactor;

		public SubbandLayer1Stereo(int subbandnumber)
			: base(subbandnumber)
		{
		}

		public override void ReadBitAllocation(Bitstream stream, Header header, Crc16 crc)
		{
			allocation = stream.GetBitsFromBuffer(4);
			channel2_allocation = stream.GetBitsFromBuffer(4);
			if (crc != null)
			{
				crc.add_bits(allocation, 4);
				crc.add_bits(channel2_allocation, 4);
			}
			if (allocation != 0)
			{
				samplelength = allocation + 1;
				if (SubbandLayer1.TableFactor.Length > allocation)
				{
					factor = SubbandLayer1.TableFactor[allocation];
				}
				if (SubbandLayer1.TableOffset.Length > allocation)
				{
					offset = SubbandLayer1.TableOffset[allocation];
				}
			}
			if (channel2_allocation != 0)
			{
				channel2_samplelength = channel2_allocation + 1;
				if (SubbandLayer1.TableFactor.Length > channel2_allocation && SubbandLayer1.TableOffset.Length > channel2_allocation)
				{
					channel2_factor = SubbandLayer1.TableFactor[channel2_allocation];
					channel2_offset = SubbandLayer1.TableOffset[channel2_allocation];
				}
			}
		}

		public override void ReadScaleFactor(Bitstream stream, Header header)
		{
			if (allocation != 0)
			{
				scalefactor = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
			}
			if (channel2_allocation != 0)
			{
				channel2_scalefactor = ASubband.ScaleFactors[stream.GetBitsFromBuffer(6)];
			}
		}

		public override bool ReadSampleData(Bitstream stream)
		{
			bool result = base.ReadSampleData(stream);
			if (channel2_allocation != 0)
			{
				channel2_sample = stream.GetBitsFromBuffer(channel2_samplelength);
			}
			return result;
		}

		public override bool PutNextSample(int channels, SynthesisFilter filter1, SynthesisFilter filter2)
		{
			base.PutNextSample(channels, filter1, filter2);
			if (channel2_allocation != 0 && channels != OutputChannels.LEFT_CHANNEL)
			{
				float num = (channel2_sample * channel2_factor + channel2_offset) * channel2_scalefactor;
				if (channels == OutputChannels.BOTH_CHANNELS)
				{
					filter2.WriteSample(num, subbandnumber);
				}
				else
				{
					filter1.WriteSample(num, subbandnumber);
				}
			}
			return true;
		}
	}
}
