using System.Text;
using MP3Sharp.Support;

namespace MP3Sharp.Decoding
{
	internal class Header
	{
		public const int MPEG2_LSF = 0;

		public const int MPEG25_LSF = 2;

		public const int MPEG1 = 1;

		public const int STEREO = 0;

		public const int JOINT_STEREO = 1;

		public const int DUAL_CHANNEL = 2;

		public const int SINGLE_CHANNEL = 3;

		public const int FOURTYFOUR_POINT_ONE = 0;

		public const int FOURTYEIGHT = 1;

		public const int THIRTYTWO = 2;

		public static readonly int[][] frequencies = new int[3][]
		{
			new int[4] { 22050, 24000, 16000, 1 },
			new int[4] { 44100, 48000, 32000, 1 },
			new int[4] { 11025, 12000, 8000, 1 }
		};

		public static readonly int[][][] bitrates = new int[3][][]
		{
			new int[3][]
			{
				new int[16]
				{
					0, 32000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000,
					160000, 176000, 192000, 224000, 256000, 0
				},
				new int[16]
				{
					0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000,
					96000, 112000, 128000, 144000, 160000, 0
				},
				new int[16]
				{
					0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000,
					96000, 112000, 128000, 144000, 160000, 0
				}
			},
			new int[3][]
			{
				new int[16]
				{
					0, 32000, 64000, 96000, 128000, 160000, 192000, 224000, 256000, 288000,
					320000, 352000, 384000, 416000, 448000, 0
				},
				new int[16]
				{
					0, 32000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 160000,
					192000, 224000, 256000, 320000, 384000, 0
				},
				new int[16]
				{
					0, 32000, 40000, 48000, 56000, 64000, 80000, 96000, 112000, 128000,
					160000, 192000, 224000, 256000, 320000, 0
				}
			},
			new int[3][]
			{
				new int[16]
				{
					0, 32000, 48000, 56000, 64000, 80000, 96000, 112000, 128000, 144000,
					160000, 176000, 192000, 224000, 256000, 0
				},
				new int[16]
				{
					0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000,
					96000, 112000, 128000, 144000, 160000, 0
				},
				new int[16]
				{
					0, 8000, 16000, 24000, 32000, 40000, 48000, 56000, 64000, 80000,
					96000, 112000, 128000, 144000, 160000, 0
				}
			}
		};

		public static readonly string[][][] bitrate_str = new string[3][][]
		{
			new string[3][]
			{
				new string[16]
				{
					"free format", "32 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s",
					"160 kbit/s", "176 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "forbidden"
				},
				new string[16]
				{
					"free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s",
					"96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "forbidden"
				},
				new string[16]
				{
					"free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s",
					"96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "forbidden"
				}
			},
			new string[3][]
			{
				new string[16]
				{
					"free format", "32 kbit/s", "64 kbit/s", "96 kbit/s", "128 kbit/s", "160 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "288 kbit/s",
					"320 kbit/s", "352 kbit/s", "384 kbit/s", "416 kbit/s", "448 kbit/s", "forbidden"
				},
				new string[16]
				{
					"free format", "32 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "160 kbit/s",
					"192 kbit/s", "224 kbit/s", "256 kbit/s", "320 kbit/s", "384 kbit/s", "forbidden"
				},
				new string[16]
				{
					"free format", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s",
					"160 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "320 kbit/s", "forbidden"
				}
			},
			new string[3][]
			{
				new string[16]
				{
					"free format", "32 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s", "96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s",
					"160 kbit/s", "176 kbit/s", "192 kbit/s", "224 kbit/s", "256 kbit/s", "forbidden"
				},
				new string[16]
				{
					"free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s",
					"96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "forbidden"
				},
				new string[16]
				{
					"free format", "8 kbit/s", "16 kbit/s", "24 kbit/s", "32 kbit/s", "40 kbit/s", "48 kbit/s", "56 kbit/s", "64 kbit/s", "80 kbit/s",
					"96 kbit/s", "112 kbit/s", "128 kbit/s", "144 kbit/s", "160 kbit/s", "forbidden"
				}
			}
		};

		private int _headerstring = -1;

		public short checksum;

		private Crc16 crc;

		public int framesize;

		private bool h_copyright;

		private bool h_original;

		private int h_layer;

		private int h_protection_bit;

		private int h_bitrate_index;

		private int h_padding_bit;

		private int h_mode_extension;

		private int h_mode;

		private int h_number_of_subbands;

		private int h_intensity_stereo_bound;

		private int h_sample_frequency;

		private int h_version;

		public int nSlots;

		private sbyte syncmode;

		public virtual int SyncHeader => _headerstring;

		internal Header()
		{
			InitBlock();
		}

		private void InitBlock()
		{
			syncmode = Bitstream.INITIAL_SYNC;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(200);
			stringBuilder.Append("Layer ");
			stringBuilder.Append(layer_string());
			stringBuilder.Append(" frame ");
			stringBuilder.Append(mode_string());
			stringBuilder.Append(' ');
			stringBuilder.Append(version_string());
			if (!IsProtection())
			{
				stringBuilder.Append(" no");
			}
			stringBuilder.Append(" checksums");
			stringBuilder.Append(' ');
			stringBuilder.Append(sample_frequency_string());
			stringBuilder.Append(',');
			stringBuilder.Append(' ');
			stringBuilder.Append(bitrate_string());
			return stringBuilder.ToString();
		}

		internal void read_header(Bitstream stream, Crc16[] crcp)
		{
			bool flag = false;
			int num;
			do
			{
				num = (_headerstring = stream.syncHeader(syncmode));
				if (syncmode == Bitstream.INITIAL_SYNC)
				{
					h_version = SupportClass.URShift(num, 19) & 1;
					if ((SupportClass.URShift(num, 20) & 1) == 0)
					{
						if (h_version != 0)
						{
							throw stream.newBitstreamException(BitstreamErrors.UNKNOWN_ERROR);
						}
						h_version = 2;
					}
					if ((h_sample_frequency = SupportClass.URShift(num, 10) & 3) == 3)
					{
						throw stream.newBitstreamException(BitstreamErrors.UNKNOWN_ERROR);
					}
				}
				h_layer = (4 - SupportClass.URShift(num, 17)) & 3;
				h_protection_bit = SupportClass.URShift(num, 16) & 1;
				h_bitrate_index = SupportClass.URShift(num, 12) & 0xF;
				h_padding_bit = SupportClass.URShift(num, 9) & 1;
				h_mode = SupportClass.URShift(num, 6) & 3;
				h_mode_extension = SupportClass.URShift(num, 4) & 3;
				if (h_mode == 1)
				{
					h_intensity_stereo_bound = (h_mode_extension << 2) + 4;
				}
				else
				{
					h_intensity_stereo_bound = 0;
				}
				if ((SupportClass.URShift(num, 3) & 1) == 1)
				{
					h_copyright = true;
				}
				if ((SupportClass.URShift(num, 2) & 1) == 1)
				{
					h_original = true;
				}
				if (h_layer == 1)
				{
					h_number_of_subbands = 32;
				}
				else
				{
					int num2 = h_bitrate_index;
					if (h_mode != 3)
					{
						num2 = ((num2 == 4) ? 1 : (num2 - 4));
					}
					if (num2 == 1 || num2 == 2)
					{
						if (h_sample_frequency == 2)
						{
							h_number_of_subbands = 12;
						}
						else
						{
							h_number_of_subbands = 8;
						}
					}
					else if (h_sample_frequency == 1 || (num2 >= 3 && num2 <= 5))
					{
						h_number_of_subbands = 27;
					}
					else
					{
						h_number_of_subbands = 30;
					}
				}
				if (h_intensity_stereo_bound > h_number_of_subbands)
				{
					h_intensity_stereo_bound = h_number_of_subbands;
				}
				calculate_framesize();
				stream.read_frame_data(framesize);
				if (stream.IsSyncCurrentPosition(syncmode))
				{
					if (syncmode == Bitstream.INITIAL_SYNC)
					{
						syncmode = Bitstream.STRICT_SYNC;
						stream.SetSyncWord(num & -521024);
					}
					flag = true;
				}
				else
				{
					stream.unreadFrame();
				}
			}
			while (!flag);
			stream.ParseFrame();
			if (h_protection_bit == 0)
			{
				checksum = (short)stream.GetBitsFromBuffer(16);
				if (crc == null)
				{
					crc = new Crc16();
				}
				crc.add_bits(num, 16);
				crcp[0] = crc;
			}
			else
			{
				crcp[0] = null;
			}
			_ = h_sample_frequency;
		}

		public int version()
		{
			return h_version;
		}

		public int layer()
		{
			return h_layer;
		}

		public int bitrate_index()
		{
			return h_bitrate_index;
		}

		public int sample_frequency()
		{
			return h_sample_frequency;
		}

		public int frequency()
		{
			return frequencies[h_version][h_sample_frequency];
		}

		public int mode()
		{
			return h_mode;
		}

		public bool IsProtection()
		{
			if (h_protection_bit == 0)
			{
				return true;
			}
			return false;
		}

		public bool IsCopyright()
		{
			return h_copyright;
		}

		public bool IsOriginal()
		{
			return h_original;
		}

		public bool IsChecksumOK()
		{
			return checksum == crc.Checksum();
		}

		public bool IsPadding()
		{
			if (h_padding_bit == 0)
			{
				return false;
			}
			return true;
		}

		public int slots()
		{
			return nSlots;
		}

		public int mode_extension()
		{
			return h_mode_extension;
		}

		public int calculate_framesize()
		{
			if (h_layer == 1)
			{
				framesize = 12 * bitrates[h_version][0][h_bitrate_index] / frequencies[h_version][h_sample_frequency];
				if (h_padding_bit != 0)
				{
					framesize++;
				}
				framesize <<= 2;
				nSlots = 0;
			}
			else
			{
				framesize = 144 * bitrates[h_version][h_layer - 1][h_bitrate_index] / frequencies[h_version][h_sample_frequency];
				if (h_version == 0 || h_version == 2)
				{
					framesize >>= 1;
				}
				if (h_padding_bit != 0)
				{
					framesize++;
				}
				if (h_layer == 3)
				{
					if (h_version == 1)
					{
						nSlots = framesize - ((h_mode == 3) ? 17 : 32) - ((h_protection_bit == 0) ? 2 : 0) - 4;
					}
					else
					{
						nSlots = framesize - ((h_mode == 3) ? 9 : 17) - ((h_protection_bit == 0) ? 2 : 0) - 4;
					}
				}
				else
				{
					nSlots = 0;
				}
			}
			framesize -= 4;
			return framesize;
		}

		public int max_number_of_frames(int streamsize)
		{
			if (framesize + 4 - h_padding_bit == 0)
			{
				return 0;
			}
			return streamsize / (framesize + 4 - h_padding_bit);
		}

		public int min_number_of_frames(int streamsize)
		{
			if (framesize + 5 - h_padding_bit == 0)
			{
				return 0;
			}
			return streamsize / (framesize + 5 - h_padding_bit);
		}

		public float ms_per_frame()
		{
			return (new float[3][]
			{
				new float[3] { 8.707483f, 8f, 12f },
				new float[3] { 26.12245f, 24f, 36f },
				new float[3] { 26.12245f, 24f, 36f }
			})[h_layer - 1][h_sample_frequency];
		}

		public float total_ms(int streamsize)
		{
			return (float)max_number_of_frames(streamsize) * ms_per_frame();
		}

		public string layer_string()
		{
			return h_layer switch
			{
				1 => "I", 
				2 => "II", 
				3 => "III", 
				_ => null, 
			};
		}

		public string bitrate_string()
		{
			return bitrate_str[h_version][h_layer - 1][h_bitrate_index];
		}

		public string sample_frequency_string()
		{
			switch (h_sample_frequency)
			{
			case 2:
				if (h_version == 1)
				{
					return "32 kHz";
				}
				if (h_version == 0)
				{
					return "16 kHz";
				}
				return "8 kHz";
			case 0:
				if (h_version == 1)
				{
					return "44.1 kHz";
				}
				if (h_version == 0)
				{
					return "22.05 kHz";
				}
				return "11.025 kHz";
			case 1:
				if (h_version == 1)
				{
					return "48 kHz";
				}
				if (h_version == 0)
				{
					return "24 kHz";
				}
				return "12 kHz";
			default:
				return null;
			}
		}

		public string mode_string()
		{
			return h_mode switch
			{
				0 => "Stereo", 
				1 => "Joint stereo", 
				2 => "Dual channel", 
				3 => "Single channel", 
				_ => null, 
			};
		}

		public string version_string()
		{
			return h_version switch
			{
				1 => "MPEG-1", 
				0 => "MPEG-2 LSF", 
				2 => "MPEG-2.5 LSF", 
				_ => null, 
			};
		}

		public int number_of_subbands()
		{
			return h_number_of_subbands;
		}

		public int intensity_stereo_bound()
		{
			return h_intensity_stereo_bound;
		}
	}
}
