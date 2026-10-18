using System;
using MP3Sharp.Decoding.Decoders.LayerIII;
using MP3Sharp.Support;

namespace MP3Sharp.Decoding.Decoders
{
	internal sealed class LayerIIIDecoder : IFrameDecoder
	{
		private const int SSLIMIT = 18;

		private const int SBLIMIT = 32;

		private static readonly int[][] slen;

		public static readonly int[] pretab;

		public static readonly float[] two_to_negative_half_pow;

		public static readonly float[] t_43;

		public static readonly float[][] io;

		public static readonly float[] TAN12;

		private static int[][] reorder_table;

		private static readonly float[] cs;

		private static readonly float[] ca;

		public static readonly float[][] win;

		public static readonly int[][][] nr_of_sfb_block;

		private readonly ABuffer buffer;

		private readonly int channels;

		private readonly SynthesisFilter filter1;

		private readonly SynthesisFilter filter2;

		private readonly int first_channel;

		private readonly Header header;

		private readonly ScaleFactorData[] III_scalefac_t;

		private readonly int[] is_1d;

		private readonly float[][] k;

		private readonly int last_channel;

		private readonly float[][][] lr;

		private readonly int max_gr;

		private readonly int[] nonzero;

		private readonly float[] out_1d;

		private readonly float[][] prevblck;

		private readonly float[][][] ro;

		private readonly ScaleFactorData[] scalefac;

		private readonly SBI[] sfBandIndex;

		private readonly int sfreq;

		private readonly Layer3SideInfo m_SideInfo;

		private readonly Bitstream stream;

		private readonly int which_channels;

		private BitReserve m_BitReserve;

		private int CheckSumHuff;

		private int counter;

		private int frame_start;

		internal int[] is_pos;

		internal float[] is_ratio;

		private int[] new_slen;

		private int part2_start;

		internal float[] rawout;

		private float[] samples1;

		private float[] samples2;

		public int[] scalefac_buffer;

		public ScaleFactorTable sftable;

		internal float[] tsOutCopy;

		internal int[] v = new int[1];

		internal int[] w = new int[1];

		internal int[] x = new int[1];

		internal int[] y = new int[1];

		static LayerIIIDecoder()
		{
			slen = new int[2][]
			{
				new int[16]
				{
					0, 0, 0, 0, 3, 1, 1, 1, 2, 2,
					2, 3, 3, 3, 4, 4
				},
				new int[16]
				{
					0, 1, 2, 3, 0, 1, 2, 3, 1, 2,
					3, 1, 2, 3, 2, 3
				}
			};
			pretab = new int[22]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 1, 1, 1, 1, 2, 2, 3, 3, 3,
				2, 0
			};
			two_to_negative_half_pow = new float[64]
			{
				1f,
				0.70710677f,
				0.5f,
				0.35355338f,
				0.25f,
				0.17677669f,
				0.125f,
				0.088388346f,
				0.0625f,
				0.044194173f,
				1f / 32f,
				0.022097087f,
				1f / 64f,
				0.011048543f,
				1f / 128f,
				0.0055242716f,
				1f / 256f,
				0.0027621358f,
				2f / 1024f,
				0.0013810679f,
				1f / 1024f,
				0.00069053395f,
				2f / 4096f,
				0.00034526698f,
				1f / 4096f,
				0.00017263349f,
				1f / 8192f,
				8.6316744E-05f,
				1f / 16384f,
				4.3158372E-05f,
				1f / 32768f,
				2.1579186E-05f,
				1f / 65536f,
				1.0789593E-05f,
				8f / 1048576f,
				5.3947965E-06f,
				4f / 1048576f,
				2.6973983E-06f,
				2f / 1048576f,
				1.3486991E-06f,
				1f / 1048576f,
				6.7434956E-07f,
				4.7683716E-07f,
				3.3717478E-07f,
				2.3841858E-07f,
				1.6858739E-07f,
				1.1920929E-07f,
				8.4293696E-08f,
				5.9604645E-08f,
				4.2146848E-08f,
				2.9802322E-08f,
				2.1073424E-08f,
				1.4901161E-08f,
				1.0536712E-08f,
				7.450581E-09f,
				5.268356E-09f,
				3.7252903E-09f,
				2.634178E-09f,
				1.8626451E-09f,
				1.317089E-09f,
				9.313226E-10f,
				6.585445E-10f,
				4.656613E-10f,
				3.2927225E-10f
			};
			io = new float[2][]
			{
				new float[32]
				{
					1f,
					0.8408964f,
					0.70710677f,
					0.59460354f,
					0.5f,
					0.4204482f,
					0.35355338f,
					0.29730177f,
					0.25f,
					0.2102241f,
					0.17677669f,
					0.14865088f,
					0.125f,
					0.10511205f,
					0.088388346f,
					0.07432544f,
					0.0625f,
					0.052556027f,
					0.044194173f,
					0.03716272f,
					1f / 32f,
					0.026278013f,
					0.022097087f,
					0.01858136f,
					1f / 64f,
					0.013139007f,
					0.011048543f,
					0.00929068f,
					1f / 128f,
					0.0065695033f,
					0.0055242716f,
					0.00464534f
				},
				new float[32]
				{
					1f,
					0.70710677f,
					0.5f,
					0.35355338f,
					0.25f,
					0.17677669f,
					0.125f,
					0.088388346f,
					0.0625f,
					0.044194173f,
					1f / 32f,
					0.022097087f,
					1f / 64f,
					0.011048543f,
					1f / 128f,
					0.0055242716f,
					1f / 256f,
					0.0027621358f,
					2f / 1024f,
					0.0013810679f,
					1f / 1024f,
					0.00069053395f,
					2f / 4096f,
					0.00034526698f,
					1f / 4096f,
					0.00017263349f,
					1f / 8192f,
					8.6316744E-05f,
					1f / 16384f,
					4.3158372E-05f,
					1f / 32768f,
					2.1579186E-05f
				}
			};
			TAN12 = new float[16]
			{
				0f, 0.2679492f, 0.57735026f, 1f, 1.7320508f, 3.732051f, 1E+11f, -3.732051f, -1.7320508f, -1f,
				-0.57735026f, -0.2679492f, 0f, 0.2679492f, 0.57735026f, 1f
			};
			cs = new float[8] { 0.8574929f, 0.881742f, 0.94962865f, 0.9833146f, 0.9955178f, 0.9991606f, 0.9998992f, 0.99999315f };
			ca = new float[8] { -0.51449573f, -0.47173196f, -0.31337744f, -0.1819132f, -0.09457419f, -0.040965583f, -0.014198569f, -0.0036999746f };
			win = new float[4][]
			{
				new float[36]
				{
					-0.016141215f, -0.05360318f, -0.100707136f, -0.16280818f, -0.5f, -0.38388735f, -0.6206114f, -1.1659756f, -3.8720753f, -4.225629f,
					-1.519529f, -0.97416484f, -0.73744076f, -1.2071068f, -0.5163616f, -0.45426053f, -0.40715656f, -0.3696946f, -0.3387627f, -0.31242222f,
					-0.28939587f, -0.26880082f, -0.5f, -0.23251417f, -0.21596715f, -0.20004979f, -0.18449493f, -0.16905846f, -0.15350361f, -0.13758625f,
					-0.12103922f, -0.20710678f, -0.084752575f, -0.06415752f, -0.041131172f, -0.014790705f
				},
				new float[36]
				{
					-0.016141215f, -0.05360318f, -0.100707136f, -0.16280818f, -0.5f, -0.38388735f, -0.6206114f, -1.1659756f, -3.8720753f, -4.225629f,
					-1.519529f, -0.97416484f, -0.73744076f, -1.2071068f, -0.5163616f, -0.45426053f, -0.40715656f, -0.3696946f, -0.33908543f, -0.3151181f,
					-0.29642227f, -0.28184548f, -0.5411961f, -0.2621323f, -0.25387916f, -0.2329629f, -0.19852729f, -0.15233535f, -0.0964964f, -0.03342383f,
					0f, 0f, 0f, 0f, 0f, 0f
				},
				new float[36]
				{
					-0.0483008f, -0.15715657f, -0.28325045f, -0.42953748f, -1.2071068f, -0.8242648f, -1.1451749f, -1.769529f, -4.5470223f, -3.489053f,
					-0.7329629f, -0.15076515f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
					0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f,
					0f, 0f, 0f, 0f, 0f, 0f
				},
				new float[36]
				{
					0f, 0f, 0f, 0f, 0f, 0f, -0.15076514f, -0.7329629f, -3.489053f, -4.5470223f,
					-1.769529f, -1.1451749f, -0.8313774f, -1.306563f, -0.54142016f, -0.46528974f, -0.4106699f, -0.3700468f, -0.3387627f, -0.31242222f,
					-0.28939587f, -0.26880082f, -0.5f, -0.23251417f, -0.21596715f, -0.20004979f, -0.18449493f, -0.16905846f, -0.15350361f, -0.13758625f,
					-0.12103922f, -0.20710678f, -0.084752575f, -0.06415752f, -0.041131172f, -0.014790705f
				}
			};
			nr_of_sfb_block = new int[6][][]
			{
				new int[3][]
				{
					new int[4] { 6, 5, 5, 5 },
					new int[4] { 9, 9, 9, 9 },
					new int[4] { 6, 9, 9, 9 }
				},
				new int[3][]
				{
					new int[4] { 6, 5, 7, 3 },
					new int[4] { 9, 9, 12, 6 },
					new int[4] { 6, 9, 12, 6 }
				},
				new int[3][]
				{
					new int[4] { 11, 10, 0, 0 },
					new int[4] { 18, 18, 0, 0 },
					new int[4] { 15, 18, 0, 0 }
				},
				new int[3][]
				{
					new int[4] { 7, 7, 7, 0 },
					new int[4] { 12, 12, 12, 0 },
					new int[4] { 6, 15, 12, 0 }
				},
				new int[3][]
				{
					new int[4] { 6, 6, 6, 3 },
					new int[4] { 12, 9, 9, 6 },
					new int[4] { 6, 12, 9, 6 }
				},
				new int[3][]
				{
					new int[4] { 8, 8, 5, 0 },
					new int[4] { 15, 12, 9, 0 },
					new int[4] { 6, 18, 9, 0 }
				}
			};
			t_43 = create_t_43();
		}

		public LayerIIIDecoder(Bitstream stream0, Header header0, SynthesisFilter filtera, SynthesisFilter filterb, ABuffer buffer0, int whichCh0)
		{
			Huffman.Initialize();
			InitBlock();
			is_1d = new int[580];
			ro = new float[2][][];
			for (int i = 0; i < 2; i++)
			{
				ro[i] = new float[32][];
				for (int j = 0; j < 32; j++)
				{
					ro[i][j] = new float[18];
				}
			}
			lr = new float[2][][];
			for (int k = 0; k < 2; k++)
			{
				lr[k] = new float[32][];
				for (int l = 0; l < 32; l++)
				{
					lr[k][l] = new float[18];
				}
			}
			out_1d = new float[576];
			prevblck = new float[2][];
			for (int m = 0; m < 2; m++)
			{
				prevblck[m] = new float[576];
			}
			this.k = new float[2][];
			for (int n = 0; n < 2; n++)
			{
				this.k[n] = new float[576];
			}
			nonzero = new int[2];
			III_scalefac_t = new ScaleFactorData[2];
			III_scalefac_t[0] = new ScaleFactorData();
			III_scalefac_t[1] = new ScaleFactorData();
			scalefac = III_scalefac_t;
			sfBandIndex = new SBI[9];
			int[] thel = new int[23]
			{
				0, 6, 12, 18, 24, 30, 36, 44, 54, 66,
				80, 96, 116, 140, 168, 200, 238, 284, 336, 396,
				464, 522, 576
			};
			int[] thes = new int[14]
			{
				0, 4, 8, 12, 18, 24, 32, 42, 56, 74,
				100, 132, 174, 192
			};
			int[] thel2 = new int[23]
			{
				0, 6, 12, 18, 24, 30, 36, 44, 54, 66,
				80, 96, 114, 136, 162, 194, 232, 278, 330, 394,
				464, 540, 576
			};
			int[] thes2 = new int[14]
			{
				0, 4, 8, 12, 18, 26, 36, 48, 62, 80,
				104, 136, 180, 192
			};
			int[] thel3 = new int[23]
			{
				0, 6, 12, 18, 24, 30, 36, 44, 54, 66,
				80, 96, 116, 140, 168, 200, 238, 284, 336, 396,
				464, 522, 576
			};
			int[] thes3 = new int[14]
			{
				0, 4, 8, 12, 18, 26, 36, 48, 62, 80,
				104, 134, 174, 192
			};
			int[] thel4 = new int[23]
			{
				0, 4, 8, 12, 16, 20, 24, 30, 36, 44,
				52, 62, 74, 90, 110, 134, 162, 196, 238, 288,
				342, 418, 576
			};
			int[] thes4 = new int[14]
			{
				0, 4, 8, 12, 16, 22, 30, 40, 52, 66,
				84, 106, 136, 192
			};
			int[] thel5 = new int[23]
			{
				0, 4, 8, 12, 16, 20, 24, 30, 36, 42,
				50, 60, 72, 88, 106, 128, 156, 190, 230, 276,
				330, 384, 576
			};
			int[] thes5 = new int[14]
			{
				0, 4, 8, 12, 16, 22, 28, 38, 50, 64,
				80, 100, 126, 192
			};
			int[] thel6 = new int[23]
			{
				0, 4, 8, 12, 16, 20, 24, 30, 36, 44,
				54, 66, 82, 102, 126, 156, 194, 240, 296, 364,
				448, 550, 576
			};
			int[] thes6 = new int[14]
			{
				0, 4, 8, 12, 16, 22, 30, 42, 58, 78,
				104, 138, 180, 192
			};
			int[] thel7 = new int[23]
			{
				0, 6, 12, 18, 24, 30, 36, 44, 54, 66,
				80, 96, 116, 140, 168, 200, 238, 284, 336, 396,
				464, 522, 576
			};
			int[] thes7 = new int[14]
			{
				0, 4, 8, 12, 18, 26, 36, 48, 62, 80,
				104, 134, 174, 192
			};
			int[] thel8 = new int[23]
			{
				0, 6, 12, 18, 24, 30, 36, 44, 54, 66,
				80, 96, 116, 140, 168, 200, 238, 284, 336, 396,
				464, 522, 576
			};
			int[] thes8 = new int[14]
			{
				0, 4, 8, 12, 18, 26, 36, 48, 62, 80,
				104, 134, 174, 192
			};
			int[] thel9 = new int[23]
			{
				0, 12, 24, 36, 48, 60, 72, 88, 108, 132,
				160, 192, 232, 280, 336, 400, 476, 566, 568, 570,
				572, 574, 576
			};
			int[] thes9 = new int[14]
			{
				0, 8, 16, 24, 36, 52, 72, 96, 124, 160,
				162, 164, 166, 192
			};
			sfBandIndex[0] = new SBI(thel, thes);
			sfBandIndex[1] = new SBI(thel2, thes2);
			sfBandIndex[2] = new SBI(thel3, thes3);
			sfBandIndex[3] = new SBI(thel4, thes4);
			sfBandIndex[4] = new SBI(thel5, thes5);
			sfBandIndex[5] = new SBI(thel6, thes6);
			sfBandIndex[6] = new SBI(thel7, thes7);
			sfBandIndex[7] = new SBI(thel8, thes8);
			sfBandIndex[8] = new SBI(thel9, thes9);
			if (reorder_table == null)
			{
				reorder_table = new int[9][];
				for (int num = 0; num < 9; num++)
				{
					reorder_table[num] = Reorder(sfBandIndex[num].s);
				}
			}
			int[] thel10 = new int[5] { 0, 6, 11, 16, 21 };
			int[] thes10 = new int[3] { 0, 6, 12 };
			sftable = new ScaleFactorTable(this, thel10, thes10);
			scalefac_buffer = new int[54];
			stream = stream0;
			header = header0;
			filter1 = filtera;
			filter2 = filterb;
			buffer = buffer0;
			which_channels = whichCh0;
			frame_start = 0;
			channels = ((header.mode() == 3) ? 1 : 2);
			max_gr = ((header.version() != 1) ? 1 : 2);
			sfreq = header.sample_frequency() + ((header.version() == 1) ? 3 : ((header.version() == 2) ? 6 : 0));
			if (channels == 2)
			{
				switch (which_channels)
				{
				case 1:
				case 3:
					first_channel = (last_channel = 0);
					break;
				case 2:
					first_channel = (last_channel = 1);
					break;
				default:
					first_channel = 0;
					last_channel = 1;
					break;
				}
			}
			else
			{
				first_channel = (last_channel = 0);
			}
			for (int num2 = 0; num2 < 2; num2++)
			{
				for (int num3 = 0; num3 < 576; num3++)
				{
					prevblck[num2][num3] = 0f;
				}
			}
			nonzero[0] = (nonzero[1] = 576);
			m_BitReserve = new BitReserve();
			m_SideInfo = new Layer3SideInfo();
		}

		public void DecodeFrame()
		{
			Decode();
		}

		private void InitBlock()
		{
			rawout = new float[36];
			tsOutCopy = new float[18];
			is_ratio = new float[576];
			is_pos = new int[576];
			new_slen = new int[4];
			samples2 = new float[32];
			samples1 = new float[32];
		}

		public void seek_notify()
		{
			frame_start = 0;
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 576; j++)
				{
					prevblck[i][j] = 0f;
				}
			}
			m_BitReserve = new BitReserve();
		}

		public void Decode()
		{
			int num = header.slots();
			ReadSideInfo();
			for (int i = 0; i < num; i++)
			{
				m_BitReserve.hputbuf(stream.GetBitsFromBuffer(8));
			}
			int num2 = SupportClass.URShift(m_BitReserve.hsstell(), 3);
			int num3;
			if ((num3 = m_BitReserve.hsstell() & 7) != 0)
			{
				m_BitReserve.ReadBits(8 - num3);
				num2++;
			}
			int num4 = frame_start - num2 - m_SideInfo.MainDataBegin;
			frame_start += num;
			if (num4 < 0)
			{
				return;
			}
			if (num2 > 4096)
			{
				frame_start -= 4096;
				m_BitReserve.RewindStreamBytes(4096);
			}
			while (num4 > 0)
			{
				m_BitReserve.ReadBits(8);
				num4--;
			}
			for (int j = 0; j < max_gr; j++)
			{
				for (int k = 0; k < channels; k++)
				{
					part2_start = m_BitReserve.hsstell();
					if (header.version() == 1)
					{
						ReadScaleFactors(k, j);
					}
					else
					{
						get_LSF_scale_factors(k, j);
					}
					HuffmanDecode(k, j);
					dequantize_sample(ro[k], k, j);
				}
				stereo(j);
				if (which_channels == OutputChannels.DOWNMIX_CHANNELS && channels > 1)
				{
					doDownMix();
				}
				for (int k = first_channel; k <= last_channel; k++)
				{
					Reorder(lr[k], k, j);
					Antialias(k, j);
					Hybrid(k, j);
					for (int l = 18; l < 576; l += 36)
					{
						for (int m = 1; m < 18; m += 2)
						{
							out_1d[l + m] = 0f - out_1d[l + m];
						}
					}
					if (k == 0 || which_channels == OutputChannels.RIGHT_CHANNEL)
					{
						for (int m = 0; m < 18; m++)
						{
							int num5 = 0;
							for (int l = 0; l < 576; l += 18)
							{
								samples1[num5] = out_1d[l + m];
								num5++;
							}
							filter1.WriteAllSamples(samples1);
							filter1.calculate_pcm_samples(buffer);
						}
						continue;
					}
					for (int m = 0; m < 18; m++)
					{
						int num5 = 0;
						for (int l = 0; l < 576; l += 18)
						{
							samples2[num5] = out_1d[l + m];
							num5++;
						}
						filter2.WriteAllSamples(samples2);
						filter2.calculate_pcm_samples(buffer);
					}
				}
			}
			counter++;
			buffer.WriteBuffer(1);
		}

		private bool ReadSideInfo()
		{
			if (header.version() == 1)
			{
				m_SideInfo.MainDataBegin = stream.GetBitsFromBuffer(9);
				if (channels == 1)
				{
					m_SideInfo.PrivateBits = stream.GetBitsFromBuffer(5);
				}
				else
				{
					m_SideInfo.PrivateBits = stream.GetBitsFromBuffer(3);
				}
				for (int i = 0; i < channels; i++)
				{
					m_SideInfo.Channels[i].ScaleFactorBits[0] = stream.GetBitsFromBuffer(1);
					m_SideInfo.Channels[i].ScaleFactorBits[1] = stream.GetBitsFromBuffer(1);
					m_SideInfo.Channels[i].ScaleFactorBits[2] = stream.GetBitsFromBuffer(1);
					m_SideInfo.Channels[i].ScaleFactorBits[3] = stream.GetBitsFromBuffer(1);
				}
				for (int j = 0; j < 2; j++)
				{
					for (int i = 0; i < channels; i++)
					{
						m_SideInfo.Channels[i].Granules[j].Part23Length = stream.GetBitsFromBuffer(12);
						m_SideInfo.Channels[i].Granules[j].BigValues = stream.GetBitsFromBuffer(9);
						m_SideInfo.Channels[i].Granules[j].GlobalGain = stream.GetBitsFromBuffer(8);
						m_SideInfo.Channels[i].Granules[j].ScaleFacCompress = stream.GetBitsFromBuffer(4);
						m_SideInfo.Channels[i].Granules[j].WindowSwitchingFlag = stream.GetBitsFromBuffer(1);
						if (m_SideInfo.Channels[i].Granules[j].WindowSwitchingFlag != 0)
						{
							m_SideInfo.Channels[i].Granules[j].BlockType = stream.GetBitsFromBuffer(2);
							m_SideInfo.Channels[i].Granules[j].MixedBlockFlag = stream.GetBitsFromBuffer(1);
							m_SideInfo.Channels[i].Granules[j].TableSelect[0] = stream.GetBitsFromBuffer(5);
							m_SideInfo.Channels[i].Granules[j].TableSelect[1] = stream.GetBitsFromBuffer(5);
							m_SideInfo.Channels[i].Granules[j].SubblockGain[0] = stream.GetBitsFromBuffer(3);
							m_SideInfo.Channels[i].Granules[j].SubblockGain[1] = stream.GetBitsFromBuffer(3);
							m_SideInfo.Channels[i].Granules[j].SubblockGain[2] = stream.GetBitsFromBuffer(3);
							if (m_SideInfo.Channels[i].Granules[j].BlockType == 0)
							{
								return false;
							}
							if (m_SideInfo.Channels[i].Granules[j].BlockType == 2 && m_SideInfo.Channels[i].Granules[j].MixedBlockFlag == 0)
							{
								m_SideInfo.Channels[i].Granules[j].Region0Count = 8;
							}
							else
							{
								m_SideInfo.Channels[i].Granules[j].Region0Count = 7;
							}
							m_SideInfo.Channels[i].Granules[j].Region1Count = 20 - m_SideInfo.Channels[i].Granules[j].Region0Count;
						}
						else
						{
							m_SideInfo.Channels[i].Granules[j].TableSelect[0] = stream.GetBitsFromBuffer(5);
							m_SideInfo.Channels[i].Granules[j].TableSelect[1] = stream.GetBitsFromBuffer(5);
							m_SideInfo.Channels[i].Granules[j].TableSelect[2] = stream.GetBitsFromBuffer(5);
							m_SideInfo.Channels[i].Granules[j].Region0Count = stream.GetBitsFromBuffer(4);
							m_SideInfo.Channels[i].Granules[j].Region1Count = stream.GetBitsFromBuffer(3);
							m_SideInfo.Channels[i].Granules[j].BlockType = 0;
						}
						m_SideInfo.Channels[i].Granules[j].Preflag = stream.GetBitsFromBuffer(1);
						m_SideInfo.Channels[i].Granules[j].ScaleFacScale = stream.GetBitsFromBuffer(1);
						m_SideInfo.Channels[i].Granules[j].Count1TableSelect = stream.GetBitsFromBuffer(1);
					}
				}
			}
			else
			{
				m_SideInfo.MainDataBegin = stream.GetBitsFromBuffer(8);
				if (channels == 1)
				{
					m_SideInfo.PrivateBits = stream.GetBitsFromBuffer(1);
				}
				else
				{
					m_SideInfo.PrivateBits = stream.GetBitsFromBuffer(2);
				}
				for (int i = 0; i < channels; i++)
				{
					m_SideInfo.Channels[i].Granules[0].Part23Length = stream.GetBitsFromBuffer(12);
					m_SideInfo.Channels[i].Granules[0].BigValues = stream.GetBitsFromBuffer(9);
					m_SideInfo.Channels[i].Granules[0].GlobalGain = stream.GetBitsFromBuffer(8);
					m_SideInfo.Channels[i].Granules[0].ScaleFacCompress = stream.GetBitsFromBuffer(9);
					m_SideInfo.Channels[i].Granules[0].WindowSwitchingFlag = stream.GetBitsFromBuffer(1);
					if (m_SideInfo.Channels[i].Granules[0].WindowSwitchingFlag != 0)
					{
						m_SideInfo.Channels[i].Granules[0].BlockType = stream.GetBitsFromBuffer(2);
						m_SideInfo.Channels[i].Granules[0].MixedBlockFlag = stream.GetBitsFromBuffer(1);
						m_SideInfo.Channels[i].Granules[0].TableSelect[0] = stream.GetBitsFromBuffer(5);
						m_SideInfo.Channels[i].Granules[0].TableSelect[1] = stream.GetBitsFromBuffer(5);
						m_SideInfo.Channels[i].Granules[0].SubblockGain[0] = stream.GetBitsFromBuffer(3);
						m_SideInfo.Channels[i].Granules[0].SubblockGain[1] = stream.GetBitsFromBuffer(3);
						m_SideInfo.Channels[i].Granules[0].SubblockGain[2] = stream.GetBitsFromBuffer(3);
						if (m_SideInfo.Channels[i].Granules[0].BlockType == 0)
						{
							return false;
						}
						if (m_SideInfo.Channels[i].Granules[0].BlockType == 2 && m_SideInfo.Channels[i].Granules[0].MixedBlockFlag == 0)
						{
							m_SideInfo.Channels[i].Granules[0].Region0Count = 8;
						}
						else
						{
							m_SideInfo.Channels[i].Granules[0].Region0Count = 7;
							m_SideInfo.Channels[i].Granules[0].Region1Count = 20 - m_SideInfo.Channels[i].Granules[0].Region0Count;
						}
					}
					else
					{
						m_SideInfo.Channels[i].Granules[0].TableSelect[0] = stream.GetBitsFromBuffer(5);
						m_SideInfo.Channels[i].Granules[0].TableSelect[1] = stream.GetBitsFromBuffer(5);
						m_SideInfo.Channels[i].Granules[0].TableSelect[2] = stream.GetBitsFromBuffer(5);
						m_SideInfo.Channels[i].Granules[0].Region0Count = stream.GetBitsFromBuffer(4);
						m_SideInfo.Channels[i].Granules[0].Region1Count = stream.GetBitsFromBuffer(3);
						m_SideInfo.Channels[i].Granules[0].BlockType = 0;
					}
					m_SideInfo.Channels[i].Granules[0].ScaleFacScale = stream.GetBitsFromBuffer(1);
					m_SideInfo.Channels[i].Granules[0].Count1TableSelect = stream.GetBitsFromBuffer(1);
				}
			}
			return true;
		}

		private void ReadScaleFactors(int ch, int gr)
		{
			GranuleInfo granuleInfo = m_SideInfo.Channels[ch].Granules[gr];
			int scaleFacCompress = granuleInfo.ScaleFacCompress;
			int n = slen[0][scaleFacCompress];
			int n2 = slen[1][scaleFacCompress];
			if (granuleInfo.WindowSwitchingFlag != 0 && granuleInfo.BlockType == 2)
			{
				if (granuleInfo.MixedBlockFlag != 0)
				{
					int i;
					for (i = 0; i < 8; i++)
					{
						scalefac[ch].l[i] = m_BitReserve.ReadBits(slen[0][granuleInfo.ScaleFacCompress]);
					}
					for (i = 3; i < 6; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							scalefac[ch].s[j][i] = m_BitReserve.ReadBits(slen[0][granuleInfo.ScaleFacCompress]);
						}
					}
					for (i = 6; i < 12; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							scalefac[ch].s[j][i] = m_BitReserve.ReadBits(slen[1][granuleInfo.ScaleFacCompress]);
						}
					}
					i = 12;
					for (int j = 0; j < 3; j++)
					{
						scalefac[ch].s[j][i] = 0;
					}
					return;
				}
				scalefac[ch].s[0][0] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[1][0] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[2][0] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[0][1] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[1][1] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[2][1] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[0][2] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[1][2] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[2][2] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[0][3] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[1][3] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[2][3] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[0][4] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[1][4] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[2][4] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[0][5] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[1][5] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[2][5] = m_BitReserve.ReadBits(n);
				scalefac[ch].s[0][6] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[1][6] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[2][6] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[0][7] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[1][7] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[2][7] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[0][8] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[1][8] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[2][8] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[0][9] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[1][9] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[2][9] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[0][10] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[1][10] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[2][10] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[0][11] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[1][11] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[2][11] = m_BitReserve.ReadBits(n2);
				scalefac[ch].s[0][12] = 0;
				scalefac[ch].s[1][12] = 0;
				scalefac[ch].s[2][12] = 0;
			}
			else
			{
				if (m_SideInfo.Channels[ch].ScaleFactorBits[0] == 0 || gr == 0)
				{
					scalefac[ch].l[0] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[1] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[2] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[3] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[4] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[5] = m_BitReserve.ReadBits(n);
				}
				if (m_SideInfo.Channels[ch].ScaleFactorBits[1] == 0 || gr == 0)
				{
					scalefac[ch].l[6] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[7] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[8] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[9] = m_BitReserve.ReadBits(n);
					scalefac[ch].l[10] = m_BitReserve.ReadBits(n);
				}
				if (m_SideInfo.Channels[ch].ScaleFactorBits[2] == 0 || gr == 0)
				{
					scalefac[ch].l[11] = m_BitReserve.ReadBits(n2);
					scalefac[ch].l[12] = m_BitReserve.ReadBits(n2);
					scalefac[ch].l[13] = m_BitReserve.ReadBits(n2);
					scalefac[ch].l[14] = m_BitReserve.ReadBits(n2);
					scalefac[ch].l[15] = m_BitReserve.ReadBits(n2);
				}
				if (m_SideInfo.Channels[ch].ScaleFactorBits[3] == 0 || gr == 0)
				{
					scalefac[ch].l[16] = m_BitReserve.ReadBits(n2);
					scalefac[ch].l[17] = m_BitReserve.ReadBits(n2);
					scalefac[ch].l[18] = m_BitReserve.ReadBits(n2);
					scalefac[ch].l[19] = m_BitReserve.ReadBits(n2);
					scalefac[ch].l[20] = m_BitReserve.ReadBits(n2);
				}
				scalefac[ch].l[21] = 0;
				scalefac[ch].l[22] = 0;
			}
		}

		private void get_LSF_scale_data(int ch, int gr)
		{
			int num = header.mode_extension();
			int num2 = 0;
			GranuleInfo granuleInfo = m_SideInfo.Channels[ch].Granules[gr];
			int scaleFacCompress = granuleInfo.ScaleFacCompress;
			int num3 = ((granuleInfo.BlockType == 2) ? ((granuleInfo.MixedBlockFlag == 0) ? 1 : ((granuleInfo.MixedBlockFlag == 1) ? 2 : 0)) : 0);
			if ((num != 1 && num != 3) || ch != 1)
			{
				if (scaleFacCompress < 400)
				{
					new_slen[0] = SupportClass.URShift(scaleFacCompress, 4) / 5;
					new_slen[1] = SupportClass.URShift(scaleFacCompress, 4) % 5;
					new_slen[2] = SupportClass.URShift(scaleFacCompress & 0xF, 2);
					new_slen[3] = scaleFacCompress & 3;
					m_SideInfo.Channels[ch].Granules[gr].Preflag = 0;
					num2 = 0;
				}
				else if (scaleFacCompress < 500)
				{
					new_slen[0] = SupportClass.URShift(scaleFacCompress - 400, 2) / 5;
					new_slen[1] = SupportClass.URShift(scaleFacCompress - 400, 2) % 5;
					new_slen[2] = (scaleFacCompress - 400) & 3;
					new_slen[3] = 0;
					m_SideInfo.Channels[ch].Granules[gr].Preflag = 0;
					num2 = 1;
				}
				else if (scaleFacCompress < 512)
				{
					new_slen[0] = (scaleFacCompress - 500) / 3;
					new_slen[1] = (scaleFacCompress - 500) % 3;
					new_slen[2] = 0;
					new_slen[3] = 0;
					m_SideInfo.Channels[ch].Granules[gr].Preflag = 1;
					num2 = 2;
				}
			}
			if ((num == 1 || num == 3) && ch == 1)
			{
				int num4 = SupportClass.URShift(scaleFacCompress, 1);
				if (num4 < 180)
				{
					new_slen[0] = num4 / 36;
					new_slen[1] = num4 % 36 / 6;
					new_slen[2] = num4 % 36 % 6;
					new_slen[3] = 0;
					m_SideInfo.Channels[ch].Granules[gr].Preflag = 0;
					num2 = 3;
				}
				else if (num4 < 244)
				{
					new_slen[0] = SupportClass.URShift((num4 - 180) & 0x3F, 4);
					new_slen[1] = SupportClass.URShift((num4 - 180) & 0xF, 2);
					new_slen[2] = (num4 - 180) & 3;
					new_slen[3] = 0;
					m_SideInfo.Channels[ch].Granules[gr].Preflag = 0;
					num2 = 4;
				}
				else if (num4 < 255)
				{
					new_slen[0] = (num4 - 244) / 3;
					new_slen[1] = (num4 - 244) % 3;
					new_slen[2] = 0;
					new_slen[3] = 0;
					m_SideInfo.Channels[ch].Granules[gr].Preflag = 0;
					num2 = 5;
				}
			}
			for (int i = 0; i < 45; i++)
			{
				scalefac_buffer[i] = 0;
			}
			int num5 = 0;
			for (int j = 0; j < 4; j++)
			{
				for (int k = 0; k < nr_of_sfb_block[num2][num3][j]; k++)
				{
					scalefac_buffer[num5] = ((new_slen[j] != 0) ? m_BitReserve.ReadBits(new_slen[j]) : 0);
					num5++;
				}
			}
		}

		private void get_LSF_scale_factors(int ch, int gr)
		{
			int num = 0;
			GranuleInfo granuleInfo = m_SideInfo.Channels[ch].Granules[gr];
			get_LSF_scale_data(ch, gr);
			if (granuleInfo.WindowSwitchingFlag != 0 && granuleInfo.BlockType == 2)
			{
				if (granuleInfo.MixedBlockFlag != 0)
				{
					for (int i = 0; i < 8; i++)
					{
						scalefac[ch].l[i] = scalefac_buffer[num];
						num++;
					}
					for (int i = 3; i < 12; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							scalefac[ch].s[j][i] = scalefac_buffer[num];
							num++;
						}
					}
					for (int j = 0; j < 3; j++)
					{
						scalefac[ch].s[j][12] = 0;
					}
					return;
				}
				for (int i = 0; i < 12; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						scalefac[ch].s[j][i] = scalefac_buffer[num];
						num++;
					}
				}
				for (int j = 0; j < 3; j++)
				{
					scalefac[ch].s[j][12] = 0;
				}
			}
			else
			{
				for (int i = 0; i < 21; i++)
				{
					scalefac[ch].l[i] = scalefac_buffer[num];
					num++;
				}
				scalefac[ch].l[21] = 0;
				scalefac[ch].l[22] = 0;
			}
		}

		private void HuffmanDecode(int ch, int gr)
		{
			x[0] = 0;
			y[0] = 0;
			v[0] = 0;
			w[0] = 0;
			int num = part2_start + m_SideInfo.Channels[ch].Granules[gr].Part23Length;
			int num2;
			int num3;
			if (m_SideInfo.Channels[ch].Granules[gr].WindowSwitchingFlag != 0 && m_SideInfo.Channels[ch].Granules[gr].BlockType == 2)
			{
				num2 = ((sfreq == 8) ? 72 : 36);
				num3 = 576;
			}
			else
			{
				int num4 = m_SideInfo.Channels[ch].Granules[gr].Region0Count + 1;
				int num5 = num4 + m_SideInfo.Channels[ch].Granules[gr].Region1Count + 1;
				if (num5 > sfBandIndex[sfreq].l.Length - 1)
				{
					num5 = sfBandIndex[sfreq].l.Length - 1;
				}
				num2 = sfBandIndex[sfreq].l[num4];
				num3 = sfBandIndex[sfreq].l[num5];
			}
			int i = 0;
			Huffman h;
			for (int j = 0; j < m_SideInfo.Channels[ch].Granules[gr].BigValues << 1; j += 2)
			{
				h = ((j >= num2) ? ((j >= num3) ? Huffman.ht[m_SideInfo.Channels[ch].Granules[gr].TableSelect[2]] : Huffman.ht[m_SideInfo.Channels[ch].Granules[gr].TableSelect[1]]) : Huffman.ht[m_SideInfo.Channels[ch].Granules[gr].TableSelect[0]]);
				Huffman.Decode(h, x, y, v, w, m_BitReserve);
				is_1d[i++] = x[0];
				is_1d[i++] = y[0];
				CheckSumHuff = CheckSumHuff + x[0] + y[0];
			}
			h = Huffman.ht[m_SideInfo.Channels[ch].Granules[gr].Count1TableSelect + 32];
			int num6 = m_BitReserve.hsstell();
			while (num6 < num && i < 576)
			{
				Huffman.Decode(h, x, y, v, w, m_BitReserve);
				is_1d[i++] = v[0];
				is_1d[i++] = w[0];
				is_1d[i++] = x[0];
				is_1d[i++] = y[0];
				CheckSumHuff = CheckSumHuff + v[0] + w[0] + x[0] + y[0];
				num6 = m_BitReserve.hsstell();
			}
			if (num6 > num)
			{
				m_BitReserve.RewindStreamBits(num6 - num);
				i -= 4;
			}
			num6 = m_BitReserve.hsstell();
			if (num6 < num)
			{
				m_BitReserve.ReadBits(num - num6);
			}
			if (i < 576)
			{
				nonzero[ch] = i;
			}
			else
			{
				nonzero[ch] = 576;
			}
			if (i < 0)
			{
				i = 0;
			}
			for (; i < 576; i++)
			{
				is_1d[i] = 0;
			}
		}

		private void i_stereo_k_values(int is_pos, int io_type, int i)
		{
			if (is_pos == 0)
			{
				k[0][i] = 1f;
				k[1][i] = 1f;
			}
			else if ((is_pos & 1) != 0)
			{
				k[0][i] = io[io_type][SupportClass.URShift(is_pos + 1, 1)];
				k[1][i] = 1f;
			}
			else
			{
				k[0][i] = 1f;
				k[1][i] = io[io_type][SupportClass.URShift(is_pos, 1)];
			}
		}

		private void dequantize_sample(float[][] xr, int ch, int gr)
		{
			GranuleInfo granuleInfo = m_SideInfo.Channels[ch].Granules[gr];
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5;
			if (granuleInfo.WindowSwitchingFlag != 0 && granuleInfo.BlockType == 2)
			{
				if (granuleInfo.MixedBlockFlag != 0)
				{
					num5 = sfBandIndex[sfreq].l[1];
				}
				else
				{
					num3 = sfBandIndex[sfreq].s[1];
					num5 = (num3 << 2) - num3;
					num2 = 0;
				}
			}
			else
			{
				num5 = sfBandIndex[sfreq].l[1];
			}
			float num6 = (float)Math.Pow(2.0, 0.25 * ((double)granuleInfo.GlobalGain - 210.0));
			for (int i = 0; i < nonzero[ch]; i++)
			{
				int num7 = i % 18;
				int num8 = (i - num7) / 18;
				if (is_1d[i] == 0)
				{
					xr[num8][num7] = 0f;
					continue;
				}
				int num9 = is_1d[i];
				float num10 = num6 * (float)((num9 > 0) ? 1 : (-1));
				num9 *= ((num9 >= 0) ? 1 : (-1));
				if (num9 < t_43.Length)
				{
					xr[num8][num7] = num10 * t_43[num9];
				}
				else
				{
					xr[num8][num7] = 0f;
				}
			}
			for (int i = 0; i < nonzero[ch]; i++)
			{
				int num11 = i % 18;
				int num12 = (i - num11) / 18;
				if (num4 == num5)
				{
					if (granuleInfo.WindowSwitchingFlag != 0 && granuleInfo.BlockType == 2)
					{
						if (granuleInfo.MixedBlockFlag != 0)
						{
							if (num4 == sfBandIndex[sfreq].l[8])
							{
								num5 = sfBandIndex[sfreq].s[4];
								num5 = (num5 << 2) - num5;
								num = 3;
								num3 = sfBandIndex[sfreq].s[4] - sfBandIndex[sfreq].s[3];
								num2 = sfBandIndex[sfreq].s[3];
								num2 = (num2 << 2) - num2;
							}
							else if (num4 < sfBandIndex[sfreq].l[8])
							{
								num5 = sfBandIndex[sfreq].l[++num + 1];
							}
							else
							{
								num5 = sfBandIndex[sfreq].s[++num + 1];
								num5 = (num5 << 2) - num5;
								num2 = sfBandIndex[sfreq].s[num];
								num3 = sfBandIndex[sfreq].s[num + 1] - num2;
								num2 = (num2 << 2) - num2;
							}
						}
						else
						{
							num5 = sfBandIndex[sfreq].s[++num + 1];
							num5 = (num5 << 2) - num5;
							num2 = sfBandIndex[sfreq].s[num];
							num3 = sfBandIndex[sfreq].s[num + 1] - num2;
							num2 = (num2 << 2) - num2;
						}
					}
					else
					{
						num5 = sfBandIndex[sfreq].l[++num + 1];
					}
				}
				if (granuleInfo.WindowSwitchingFlag != 0 && ((granuleInfo.BlockType == 2 && granuleInfo.MixedBlockFlag == 0) || (granuleInfo.BlockType == 2 && granuleInfo.MixedBlockFlag != 0 && i >= 36)))
				{
					int num13 = (num4 - num2) / num3;
					int num14 = scalefac[ch].s[num13][num] << granuleInfo.ScaleFacScale;
					num14 += granuleInfo.SubblockGain[num13] << 2;
					xr[num12][num11] *= two_to_negative_half_pow[num14];
				}
				else
				{
					int num15 = scalefac[ch].l[num];
					if (granuleInfo.Preflag != 0)
					{
						num15 += pretab[num];
					}
					num15 <<= granuleInfo.ScaleFacScale;
					xr[num12][num11] *= two_to_negative_half_pow[num15];
				}
				num4++;
			}
			for (int i = nonzero[ch]; i < 576; i++)
			{
				int num16 = i % 18;
				int num17 = (i - num16) / 18;
				if (num16 < 0)
				{
					num16 = 0;
				}
				if (num17 < 0)
				{
					num17 = 0;
				}
				xr[num17][num16] = 0f;
			}
		}

		private void Reorder(float[][] xr, int ch, int gr)
		{
			GranuleInfo granuleInfo = m_SideInfo.Channels[ch].Granules[gr];
			if (granuleInfo.WindowSwitchingFlag != 0 && granuleInfo.BlockType == 2)
			{
				for (int i = 0; i < 576; i++)
				{
					out_1d[i] = 0f;
				}
				if (granuleInfo.MixedBlockFlag != 0)
				{
					for (int i = 0; i < 36; i++)
					{
						int num = i % 18;
						int num2 = (i - num) / 18;
						out_1d[i] = xr[num2][num];
					}
					int num3 = 3;
					int num4 = sfBandIndex[sfreq].s[3];
					int num5 = sfBandIndex[sfreq].s[4] - num4;
					while (num3 < 13)
					{
						int num6 = (num4 << 2) - num4;
						int num7 = 0;
						int num8 = 0;
						while (num7 < num5)
						{
							int num9 = num6 + num7;
							int num10 = num6 + num8;
							int num11 = num9 % 18;
							int num12 = (num9 - num11) / 18;
							out_1d[num10] = xr[num12][num11];
							num9 += num5;
							num10++;
							num11 = num9 % 18;
							num12 = (num9 - num11) / 18;
							out_1d[num10] = xr[num12][num11];
							num9 += num5;
							num10++;
							num11 = num9 % 18;
							num12 = (num9 - num11) / 18;
							out_1d[num10] = xr[num12][num11];
							num7++;
							num8 += 3;
						}
						num3++;
						num4 = sfBandIndex[sfreq].s[num3];
						num5 = sfBandIndex[sfreq].s[num3 + 1] - num4;
					}
				}
				else
				{
					for (int i = 0; i < 576; i++)
					{
						int num13 = reorder_table[sfreq][i];
						int num14 = num13 % 18;
						int num15 = (num13 - num14) / 18;
						out_1d[i] = xr[num15][num14];
					}
				}
			}
			else
			{
				for (int i = 0; i < 576; i++)
				{
					int num16 = i % 18;
					int num17 = (i - num16) / 18;
					out_1d[i] = xr[num17][num16];
				}
			}
		}

		private void stereo(int gr)
		{
			if (channels == 1)
			{
				for (int i = 0; i < 32; i++)
				{
					for (int j = 0; j < 18; j += 3)
					{
						lr[0][i][j] = ro[0][i][j];
						lr[0][i][j + 1] = ro[0][i][j + 1];
						lr[0][i][j + 2] = ro[0][i][j + 2];
					}
				}
				return;
			}
			GranuleInfo granuleInfo = m_SideInfo.Channels[0].Granules[gr];
			int num = header.mode_extension();
			bool flag = header.mode() == 1 && (num & 2) != 0;
			bool flag2 = header.mode() == 1 && (num & 1) != 0;
			bool flag3 = header.version() == 0 || header.version() == 2;
			int io_type = granuleInfo.ScaleFacCompress & 1;
			int k;
			for (k = 0; k < 576; k++)
			{
				is_pos[k] = 7;
				is_ratio[k] = 0f;
			}
			if (flag2)
			{
				if (granuleInfo.WindowSwitchingFlag != 0 && granuleInfo.BlockType == 2)
				{
					if (granuleInfo.MixedBlockFlag != 0)
					{
						int num2 = 0;
						for (int l = 0; l < 3; l++)
						{
							int num3 = 2;
							int num4;
							for (num4 = 12; num4 >= 3; num4--)
							{
								k = sfBandIndex[sfreq].s[num4];
								int num5 = sfBandIndex[sfreq].s[num4 + 1] - k;
								k = (k << 2) - k + (l + 1) * num5 - 1;
								while (num5 > 0)
								{
									if (ro[1][k / 18][k % 18] != 0f)
									{
										num3 = num4;
										num4 = -10;
										num5 = -10;
									}
									num5--;
									k--;
								}
							}
							num4 = num3 + 1;
							if (num4 > num2)
							{
								num2 = num4;
							}
							int num6;
							int i;
							for (; num4 < 12; num4++)
							{
								num6 = sfBandIndex[sfreq].s[num4];
								i = sfBandIndex[sfreq].s[num4 + 1] - num6;
								k = (num6 << 2) - num6 + l * i;
								while (i > 0)
								{
									is_pos[k] = scalefac[1].s[l][num4];
									if (is_pos[k] != 7)
									{
										if (flag3)
										{
											i_stereo_k_values(is_pos[k], io_type, k);
										}
										else
										{
											is_ratio[k] = TAN12[is_pos[k]];
										}
									}
									k++;
									i--;
								}
							}
							num4 = sfBandIndex[sfreq].s[10];
							i = sfBandIndex[sfreq].s[11] - num4;
							num4 = (num4 << 2) - num4 + l * i;
							num6 = sfBandIndex[sfreq].s[11];
							i = sfBandIndex[sfreq].s[12] - num6;
							k = (num6 << 2) - num6 + l * i;
							while (i > 0)
							{
								is_pos[k] = is_pos[num4];
								if (flag3)
								{
									this.k[0][k] = this.k[0][num4];
									this.k[1][k] = this.k[1][num4];
								}
								else
								{
									is_ratio[k] = is_ratio[num4];
								}
								k++;
								i--;
							}
						}
						if (num2 <= 3)
						{
							k = 2;
							int j = 17;
							int i = -1;
							while (k >= 0)
							{
								if (ro[1][k][j] != 0f)
								{
									i = (k << 4) + (k << 1) + j;
									k = -1;
									continue;
								}
								j--;
								if (j < 0)
								{
									k--;
									j = 17;
								}
							}
							for (k = 0; sfBandIndex[sfreq].l[k] <= i; k++)
							{
							}
							int num4 = k;
							k = sfBandIndex[sfreq].l[k];
							for (; num4 < 8; num4++)
							{
								for (i = sfBandIndex[sfreq].l[num4 + 1] - sfBandIndex[sfreq].l[num4]; i > 0; i--)
								{
									is_pos[k] = scalefac[1].l[num4];
									if (is_pos[k] != 7)
									{
										if (flag3)
										{
											i_stereo_k_values(is_pos[k], io_type, k);
										}
										else
										{
											is_ratio[k] = TAN12[is_pos[k]];
										}
									}
									k++;
								}
							}
						}
					}
					else
					{
						for (int m = 0; m < 3; m++)
						{
							int num7 = -1;
							int num6;
							int num4;
							for (num4 = 12; num4 >= 0; num4--)
							{
								num6 = sfBandIndex[sfreq].s[num4];
								int num5 = sfBandIndex[sfreq].s[num4 + 1] - num6;
								k = (num6 << 2) - num6 + (m + 1) * num5 - 1;
								while (num5 > 0)
								{
									if (ro[1][k / 18][k % 18] != 0f)
									{
										num7 = num4;
										num4 = -10;
										num5 = -10;
									}
									num5--;
									k--;
								}
							}
							int i;
							for (num4 = num7 + 1; num4 < 12; num4++)
							{
								num6 = sfBandIndex[sfreq].s[num4];
								i = sfBandIndex[sfreq].s[num4 + 1] - num6;
								k = (num6 << 2) - num6 + m * i;
								while (i > 0)
								{
									is_pos[k] = scalefac[1].s[m][num4];
									if (is_pos[k] != 7)
									{
										if (flag3)
										{
											i_stereo_k_values(is_pos[k], io_type, k);
										}
										else
										{
											is_ratio[k] = TAN12[is_pos[k]];
										}
									}
									k++;
									i--;
								}
							}
							num6 = sfBandIndex[sfreq].s[10];
							int num8 = sfBandIndex[sfreq].s[11];
							i = num8 - num6;
							num4 = (num6 << 2) - num6 + m * i;
							i = sfBandIndex[sfreq].s[12] - num8;
							k = (num8 << 2) - num8 + m * i;
							while (i > 0)
							{
								is_pos[k] = is_pos[num4];
								if (flag3)
								{
									this.k[0][k] = this.k[0][num4];
									this.k[1][k] = this.k[1][num4];
								}
								else
								{
									is_ratio[k] = is_ratio[num4];
								}
								k++;
								i--;
							}
						}
					}
				}
				else
				{
					k = 31;
					int j = 17;
					int i = 0;
					while (k >= 0)
					{
						if (ro[1][k][j] != 0f)
						{
							i = (k << 4) + (k << 1) + j;
							k = -1;
							continue;
						}
						j--;
						if (j < 0)
						{
							k--;
							j = 17;
						}
					}
					for (k = 0; sfBandIndex[sfreq].l[k] <= i; k++)
					{
					}
					int num4 = k;
					k = sfBandIndex[sfreq].l[k];
					for (; num4 < 21; num4++)
					{
						for (i = sfBandIndex[sfreq].l[num4 + 1] - sfBandIndex[sfreq].l[num4]; i > 0; i--)
						{
							is_pos[k] = scalefac[1].l[num4];
							if (is_pos[k] != 7)
							{
								if (flag3)
								{
									i_stereo_k_values(is_pos[k], io_type, k);
								}
								else
								{
									is_ratio[k] = TAN12[is_pos[k]];
								}
							}
							k++;
						}
					}
					num4 = sfBandIndex[sfreq].l[20];
					i = 576 - sfBandIndex[sfreq].l[21];
					while (i > 0 && k < 576)
					{
						is_pos[k] = is_pos[num4];
						if (flag3)
						{
							this.k[0][k] = this.k[0][num4];
							this.k[1][k] = this.k[1][num4];
						}
						else
						{
							is_ratio[k] = is_ratio[num4];
						}
						k++;
						i--;
					}
				}
			}
			k = 0;
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 18; j++)
				{
					if (is_pos[k] == 7)
					{
						if (flag)
						{
							lr[0][i][j] = (ro[0][i][j] + ro[1][i][j]) * 0.70710677f;
							lr[1][i][j] = (ro[0][i][j] - ro[1][i][j]) * 0.70710677f;
						}
						else
						{
							lr[0][i][j] = ro[0][i][j];
							lr[1][i][j] = ro[1][i][j];
						}
					}
					else if (flag2)
					{
						if (flag3)
						{
							lr[0][i][j] = ro[0][i][j] * this.k[0][k];
							lr[1][i][j] = ro[0][i][j] * this.k[1][k];
						}
						else
						{
							lr[1][i][j] = ro[0][i][j] / (1f + is_ratio[k]);
							lr[0][i][j] = lr[1][i][j] * is_ratio[k];
						}
					}
					k++;
				}
			}
		}

		private void Antialias(int ch, int gr)
		{
			GranuleInfo granuleInfo = m_SideInfo.Channels[ch].Granules[gr];
			if (granuleInfo.WindowSwitchingFlag != 0 && granuleInfo.BlockType == 2 && granuleInfo.MixedBlockFlag == 0)
			{
				return;
			}
			int num = ((granuleInfo.WindowSwitchingFlag == 0 || granuleInfo.MixedBlockFlag == 0 || granuleInfo.BlockType != 2) ? 558 : 18);
			for (int i = 0; i < num; i += 18)
			{
				for (int j = 0; j < 8; j++)
				{
					int num2 = i + 17 - j;
					int num3 = i + 18 + j;
					float num4 = out_1d[num2];
					float num5 = out_1d[num3];
					out_1d[num2] = num4 * cs[j] - num5 * ca[j];
					out_1d[num3] = num5 * cs[j] + num4 * ca[j];
				}
			}
		}

		private void Hybrid(int ch, int gr)
		{
			GranuleInfo granuleInfo = m_SideInfo.Channels[ch].Granules[gr];
			for (int i = 0; i < 576; i += 18)
			{
				int blockType = ((granuleInfo.WindowSwitchingFlag == 0 || granuleInfo.MixedBlockFlag == 0 || i >= 36) ? granuleInfo.BlockType : 0);
				float[] array = out_1d;
				for (int j = 0; j < 18; j++)
				{
					tsOutCopy[j] = array[j + i];
				}
				InverseMDCT(tsOutCopy, rawout, blockType);
				for (int k = 0; k < 18; k++)
				{
					array[k + i] = tsOutCopy[k];
				}
				float[][] array2 = prevblck;
				array[i] = rawout[0] + array2[ch][i];
				array2[ch][i] = rawout[18];
				array[1 + i] = rawout[1] + array2[ch][i + 1];
				array2[ch][i + 1] = rawout[19];
				array[2 + i] = rawout[2] + array2[ch][i + 2];
				array2[ch][i + 2] = rawout[20];
				array[3 + i] = rawout[3] + array2[ch][i + 3];
				array2[ch][i + 3] = rawout[21];
				array[4 + i] = rawout[4] + array2[ch][i + 4];
				array2[ch][i + 4] = rawout[22];
				array[5 + i] = rawout[5] + array2[ch][i + 5];
				array2[ch][i + 5] = rawout[23];
				array[6 + i] = rawout[6] + array2[ch][i + 6];
				array2[ch][i + 6] = rawout[24];
				array[7 + i] = rawout[7] + array2[ch][i + 7];
				array2[ch][i + 7] = rawout[25];
				array[8 + i] = rawout[8] + array2[ch][i + 8];
				array2[ch][i + 8] = rawout[26];
				array[9 + i] = rawout[9] + array2[ch][i + 9];
				array2[ch][i + 9] = rawout[27];
				array[10 + i] = rawout[10] + array2[ch][i + 10];
				array2[ch][i + 10] = rawout[28];
				array[11 + i] = rawout[11] + array2[ch][i + 11];
				array2[ch][i + 11] = rawout[29];
				array[12 + i] = rawout[12] + array2[ch][i + 12];
				array2[ch][i + 12] = rawout[30];
				array[13 + i] = rawout[13] + array2[ch][i + 13];
				array2[ch][i + 13] = rawout[31];
				array[14 + i] = rawout[14] + array2[ch][i + 14];
				array2[ch][i + 14] = rawout[32];
				array[15 + i] = rawout[15] + array2[ch][i + 15];
				array2[ch][i + 15] = rawout[33];
				array[16 + i] = rawout[16] + array2[ch][i + 16];
				array2[ch][i + 16] = rawout[34];
				array[17 + i] = rawout[17] + array2[ch][i + 17];
				array2[ch][i + 17] = rawout[35];
			}
		}

		private void doDownMix()
		{
			for (int i = 0; i < 18; i++)
			{
				for (int j = 0; j < 18; j += 3)
				{
					lr[0][i][j] = (lr[0][i][j] + lr[1][i][j]) * 0.5f;
					lr[0][i][j + 1] = (lr[0][i][j + 1] + lr[1][i][j + 1]) * 0.5f;
					lr[0][i][j + 2] = (lr[0][i][j + 2] + lr[1][i][j + 2]) * 0.5f;
				}
			}
		}

		public void InverseMDCT(float[] inValues, float[] outValues, int blockType)
		{
			float num17;
			float num16;
			float num2;
			float num15;
			float num3;
			float num14;
			float num4;
			float num13;
			float num5;
			float num12;
			float num6;
			float num11;
			float num7;
			float num10;
			float num8;
			float num9;
			float num = (num2 = (num3 = (num4 = (num5 = (num6 = (num7 = (num8 = (num9 = (num10 = (num11 = (num12 = (num13 = (num14 = (num15 = (num16 = (num17 = 0f))))))))))))))));
			if (blockType == 2)
			{
				outValues[0] = 0f;
				outValues[1] = 0f;
				outValues[2] = 0f;
				outValues[3] = 0f;
				outValues[4] = 0f;
				outValues[5] = 0f;
				outValues[6] = 0f;
				outValues[7] = 0f;
				outValues[8] = 0f;
				outValues[9] = 0f;
				outValues[10] = 0f;
				outValues[11] = 0f;
				outValues[12] = 0f;
				outValues[13] = 0f;
				outValues[14] = 0f;
				outValues[15] = 0f;
				outValues[16] = 0f;
				outValues[17] = 0f;
				outValues[18] = 0f;
				outValues[19] = 0f;
				outValues[20] = 0f;
				outValues[21] = 0f;
				outValues[22] = 0f;
				outValues[23] = 0f;
				outValues[24] = 0f;
				outValues[25] = 0f;
				outValues[26] = 0f;
				outValues[27] = 0f;
				outValues[28] = 0f;
				outValues[29] = 0f;
				outValues[30] = 0f;
				outValues[31] = 0f;
				outValues[32] = 0f;
				outValues[33] = 0f;
				outValues[34] = 0f;
				outValues[35] = 0f;
				int num18 = 0;
				for (int i = 0; i < 3; i++)
				{
					inValues[15 + i] += inValues[12 + i];
					inValues[12 + i] += inValues[9 + i];
					inValues[9 + i] += inValues[6 + i];
					inValues[6 + i] += inValues[3 + i];
					inValues[3 + i] += inValues[i];
					inValues[15 + i] += inValues[9 + i];
					inValues[9 + i] += inValues[3 + i];
					float num19 = inValues[12 + i] * 0.5f;
					float num20 = inValues[6 + i] * 0.8660254f;
					float num21 = inValues[i] + num19;
					num = inValues[i] - inValues[12 + i];
					float num22 = num21 + num20;
					num2 = num21 - num20;
					num19 = inValues[15 + i] * 0.5f;
					num20 = inValues[9 + i] * 0.8660254f;
					float num23 = inValues[3 + i] + num19;
					num4 = inValues[3 + i] - inValues[15 + i];
					num5 = num23 + num20;
					num3 = num23 - num20;
					num3 *= 1.9318516f;
					num4 *= 0.70710677f;
					num5 *= 0.5176381f;
					float num24 = num22;
					num22 += num5;
					num5 = num24 - num5;
					float num25 = num;
					num += num4;
					num4 = num25 - num4;
					float num26 = num2;
					num2 += num3;
					num3 = num26 - num3;
					num22 *= 0.5043145f;
					num *= 0.5411961f;
					num2 *= 0.6302362f;
					num3 *= 0.8213398f;
					num4 *= 1.306563f;
					num5 *= 3.830649f;
					num8 = (0f - num22) * 0.7933533f;
					num9 = (0f - num22) * 0.6087614f;
					num7 = (0f - num) * 0.9238795f;
					num10 = (0f - num) * 0.38268343f;
					num6 = (0f - num2) * 0.9914449f;
					num11 = (0f - num2) * 0.13052619f;
					num22 = num3;
					num = num4 * 0.38268343f;
					num2 = num5 * 0.6087614f;
					num3 = (0f - num5) * 0.7933533f;
					num4 = (0f - num4) * 0.9238795f;
					num5 = (0f - num22) * 0.9914449f;
					num22 *= 0.13052619f;
					outValues[num18 + 6] += num22;
					outValues[num18 + 7] += num;
					outValues[num18 + 8] += num2;
					outValues[num18 + 9] += num3;
					outValues[num18 + 10] += num4;
					outValues[num18 + 11] += num5;
					outValues[num18 + 12] += num6;
					outValues[num18 + 13] += num7;
					outValues[num18 + 14] += num8;
					outValues[num18 + 15] += num9;
					outValues[num18 + 16] += num10;
					outValues[num18 + 17] += num11;
					num18 += 6;
				}
			}
			else
			{
				inValues[17] += inValues[16];
				inValues[16] += inValues[15];
				inValues[15] += inValues[14];
				inValues[14] += inValues[13];
				inValues[13] += inValues[12];
				inValues[12] += inValues[11];
				inValues[11] += inValues[10];
				inValues[10] += inValues[9];
				inValues[9] += inValues[8];
				inValues[8] += inValues[7];
				inValues[7] += inValues[6];
				inValues[6] += inValues[5];
				inValues[5] += inValues[4];
				inValues[4] += inValues[3];
				inValues[3] += inValues[2];
				inValues[2] += inValues[1];
				inValues[1] += inValues[0];
				inValues[17] += inValues[15];
				inValues[15] += inValues[13];
				inValues[13] += inValues[11];
				inValues[11] += inValues[9];
				inValues[9] += inValues[7];
				inValues[7] += inValues[5];
				inValues[5] += inValues[3];
				inValues[3] += inValues[1];
				float num27 = inValues[0] + inValues[0];
				float num28 = num27 + inValues[12];
				float num29 = num28 + inValues[4] * 1.8793852f + inValues[8] * 1.5320889f + inValues[16] * 0.34729636f;
				float num30 = num27 + inValues[4] - inValues[8] - inValues[12] - inValues[12] - inValues[16];
				float num31 = num28 - inValues[4] * 0.34729636f - inValues[8] * 1.8793852f + inValues[16] * 1.5320889f;
				float num32 = num28 - inValues[4] * 1.5320889f + inValues[8] * 0.34729636f - inValues[16] * 1.8793852f;
				float num33 = inValues[0] - inValues[4] + inValues[8] - inValues[12] + inValues[16];
				float num34 = inValues[6] * 1.7320508f;
				float num35 = inValues[2] * 1.9696155f + num34 + inValues[10] * 1.2855753f + inValues[14] * 0.6840403f;
				float num36 = (inValues[2] - inValues[10] - inValues[14]) * 1.7320508f;
				float num37 = inValues[2] * 1.2855753f - num34 - inValues[10] * 0.6840403f + inValues[14] * 1.9696155f;
				float num38 = inValues[2] * 0.6840403f - num34 + inValues[10] * 1.9696155f - inValues[14] * 1.2855753f;
				float num39 = inValues[1] + inValues[1];
				float num40 = num39 + inValues[13];
				float num41 = num40 + inValues[5] * 1.8793852f + inValues[9] * 1.5320889f + inValues[17] * 0.34729636f;
				float num42 = num39 + inValues[5] - inValues[9] - inValues[13] - inValues[13] - inValues[17];
				float num43 = num40 - inValues[5] * 0.34729636f - inValues[9] * 1.8793852f + inValues[17] * 1.5320889f;
				float num44 = num40 - inValues[5] * 1.5320889f + inValues[9] * 0.34729636f - inValues[17] * 1.8793852f;
				float num45 = (inValues[1] - inValues[5] + inValues[9] - inValues[13] + inValues[17]) * 0.70710677f;
				float num46 = inValues[7] * 1.7320508f;
				float num47 = inValues[3] * 1.9696155f + num46 + inValues[11] * 1.2855753f + inValues[15] * 0.6840403f;
				float num48 = (inValues[3] - inValues[11] - inValues[15]) * 1.7320508f;
				float num49 = inValues[3] * 1.2855753f - num46 - inValues[11] * 0.6840403f + inValues[15] * 1.9696155f;
				float num50 = inValues[3] * 0.6840403f - num46 + inValues[11] * 1.9696155f - inValues[15] * 1.2855753f;
				float num51 = num29 + num35;
				float num52 = (num41 + num47) * 0.5019099f;
				float num22 = num51 + num52;
				num17 = num51 - num52;
				float num53 = num30 + num36;
				num52 = (num42 + num48) * 0.5176381f;
				num = num53 + num52;
				num16 = num53 - num52;
				float num54 = num31 + num37;
				num52 = (num43 + num49) * 0.55168897f;
				num2 = num54 + num52;
				num15 = num54 - num52;
				float num55 = num32 + num38;
				num52 = (num44 + num50) * 0.61038727f;
				num3 = num55 + num52;
				num14 = num55 - num52;
				num4 = num33 + num45;
				num13 = num33 - num45;
				float num56 = num32 - num38;
				num52 = (num44 - num50) * 0.8717234f;
				num5 = num56 + num52;
				num12 = num56 - num52;
				float num57 = num31 - num37;
				num52 = (num43 - num49) * 1.1831008f;
				num6 = num57 + num52;
				num11 = num57 - num52;
				float num58 = num30 - num36;
				num52 = (num42 - num48) * 1.9318516f;
				num7 = num58 + num52;
				num10 = num58 - num52;
				float num59 = num29 - num35;
				num52 = (num41 - num47) * 5.7368565f;
				num8 = num59 + num52;
				num9 = num59 - num52;
				float[] array = win[blockType];
				outValues[0] = (0f - num9) * array[0];
				outValues[1] = (0f - num10) * array[1];
				outValues[2] = (0f - num11) * array[2];
				outValues[3] = (0f - num12) * array[3];
				outValues[4] = (0f - num13) * array[4];
				outValues[5] = (0f - num14) * array[5];
				outValues[6] = (0f - num15) * array[6];
				outValues[7] = (0f - num16) * array[7];
				outValues[8] = (0f - num17) * array[8];
				outValues[9] = num17 * array[9];
				outValues[10] = num16 * array[10];
				outValues[11] = num15 * array[11];
				outValues[12] = num14 * array[12];
				outValues[13] = num13 * array[13];
				outValues[14] = num12 * array[14];
				outValues[15] = num11 * array[15];
				outValues[16] = num10 * array[16];
				outValues[17] = num9 * array[17];
				outValues[18] = num8 * array[18];
				outValues[19] = num7 * array[19];
				outValues[20] = num6 * array[20];
				outValues[21] = num5 * array[21];
				outValues[22] = num4 * array[22];
				outValues[23] = num3 * array[23];
				outValues[24] = num2 * array[24];
				outValues[25] = num * array[25];
				outValues[26] = num22 * array[26];
				outValues[27] = num22 * array[27];
				outValues[28] = num * array[28];
				outValues[29] = num2 * array[29];
				outValues[30] = num3 * array[30];
				outValues[31] = num4 * array[31];
				outValues[32] = num5 * array[32];
				outValues[33] = num6 * array[33];
				outValues[34] = num7 * array[34];
				outValues[35] = num8 * array[35];
			}
		}

		private static float[] create_t_43()
		{
			float[] array = new float[8192];
			double num = 1.3333333333333333;
			for (int i = 0; i < 8192; i++)
			{
				array[i] = (float)Math.Pow(i, num);
			}
			return array;
		}

		internal static int[] Reorder(int[] scalefac_band)
		{
			int num = 0;
			int[] array = new int[576];
			for (int i = 0; i < 13; i++)
			{
				int num2 = scalefac_band[i];
				int num3 = scalefac_band[i + 1];
				for (int j = 0; j < 3; j++)
				{
					for (int k = num2; k < num3; k++)
					{
						array[3 * k + j] = num++;
					}
				}
			}
			return array;
		}
	}
}
