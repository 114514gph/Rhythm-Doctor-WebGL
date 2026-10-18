using MP3Sharp.Decoding.Decoders.LayerI;

namespace MP3Sharp.Decoding.Decoders
{
	internal class LayerIDecoder : IFrameDecoder
	{
		protected internal ABuffer buffer;

		protected internal Crc16 crc;

		protected internal SynthesisFilter filter1;

		protected internal SynthesisFilter filter2;

		protected internal Header header;

		protected internal int mode;

		protected internal int num_subbands;

		protected internal Bitstream stream;

		protected internal ASubband[] subbands;

		protected internal int which_channels;

		public LayerIDecoder()
		{
			crc = new Crc16();
		}

		public virtual void DecodeFrame()
		{
			num_subbands = header.number_of_subbands();
			subbands = new ASubband[32];
			mode = header.mode();
			CreateSubbands();
			ReadAllocation();
			ReadScaleFactorSelection();
			if (crc != null || header.IsChecksumOK())
			{
				ReadScaleFactors();
				ReadSampleData();
			}
		}

		public virtual void Create(Bitstream stream0, Header header0, SynthesisFilter filtera, SynthesisFilter filterb, ABuffer buffer0, int whichCh0)
		{
			stream = stream0;
			header = header0;
			filter1 = filtera;
			filter2 = filterb;
			buffer = buffer0;
			which_channels = whichCh0;
		}

		protected internal virtual void CreateSubbands()
		{
			if (mode == 3)
			{
				for (int i = 0; i < num_subbands; i++)
				{
					subbands[i] = new SubbandLayer1(i);
				}
			}
			else if (mode == 1)
			{
				int i;
				for (i = 0; i < header.intensity_stereo_bound(); i++)
				{
					subbands[i] = new SubbandLayer1Stereo(i);
				}
				for (; i < num_subbands; i++)
				{
					subbands[i] = new SubbandLayer1IntensityStereo(i);
				}
			}
			else
			{
				for (int i = 0; i < num_subbands; i++)
				{
					subbands[i] = new SubbandLayer1Stereo(i);
				}
			}
		}

		protected internal virtual void ReadAllocation()
		{
			for (int i = 0; i < num_subbands; i++)
			{
				subbands[i].ReadBitAllocation(stream, header, crc);
			}
		}

		protected internal virtual void ReadScaleFactorSelection()
		{
		}

		protected internal virtual void ReadScaleFactors()
		{
			for (int i = 0; i < num_subbands; i++)
			{
				subbands[i].ReadScaleFactor(stream, header);
			}
		}

		protected internal virtual void ReadSampleData()
		{
			bool flag = false;
			bool flag2 = false;
			int num = header.mode();
			do
			{
				for (int i = 0; i < num_subbands; i++)
				{
					flag = subbands[i].ReadSampleData(stream);
				}
				do
				{
					for (int i = 0; i < num_subbands; i++)
					{
						flag2 = subbands[i].PutNextSample(which_channels, filter1, filter2);
					}
					filter1.calculate_pcm_samples(buffer);
					if (which_channels == OutputChannels.BOTH_CHANNELS && num != 3)
					{
						filter2.calculate_pcm_samples(buffer);
					}
				}
				while (!flag2);
			}
			while (!flag);
		}
	}
}
