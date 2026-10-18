using MP3Sharp.Decoding.Decoders.LayerII;

namespace MP3Sharp.Decoding.Decoders
{
	internal class LayerIIDecoder : LayerIDecoder, IFrameDecoder
	{
		protected internal override void CreateSubbands()
		{
			if (mode == 3)
			{
				for (int i = 0; i < num_subbands; i++)
				{
					subbands[i] = new SubbandLayer2(i);
				}
			}
			else if (mode == 1)
			{
				int i;
				for (i = 0; i < header.intensity_stereo_bound(); i++)
				{
					subbands[i] = new SubbandLayer2Stereo(i);
				}
				for (; i < num_subbands; i++)
				{
					subbands[i] = new SubbandLayer2IntensityStereo(i);
				}
			}
			else
			{
				for (int i = 0; i < num_subbands; i++)
				{
					subbands[i] = new SubbandLayer2Stereo(i);
				}
			}
		}

		protected internal override void ReadScaleFactorSelection()
		{
			for (int i = 0; i < num_subbands; i++)
			{
				((SubbandLayer2)subbands[i]).read_scalefactor_selection(stream, crc);
			}
		}
	}
}
