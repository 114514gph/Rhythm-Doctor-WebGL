using System;

namespace MP3Sharp.Decoding
{
	internal class SynthesisFilter
	{
		private const double MY_PI = Math.PI;

		private static readonly float cos1_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI / 64.0)));

		private static readonly float cos3_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 3.0 / 64.0)));

		private static readonly float cos5_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 5.0 / 64.0)));

		private static readonly float cos7_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 7.0 / 64.0)));

		private static readonly float cos9_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 9.0 / 64.0)));

		private static readonly float cos11_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 11.0 / 64.0)));

		private static readonly float cos13_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 13.0 / 64.0)));

		private static readonly float cos15_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 15.0 / 64.0)));

		private static readonly float cos17_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 17.0 / 64.0)));

		private static readonly float cos19_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 19.0 / 64.0)));

		private static readonly float cos21_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 21.0 / 64.0)));

		private static readonly float cos23_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 23.0 / 64.0)));

		private static readonly float cos25_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 25.0 / 64.0)));

		private static readonly float cos27_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 27.0 / 64.0)));

		private static readonly float cos29_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 29.0 / 64.0)));

		private static readonly float cos31_64 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 31.0 / 64.0)));

		private static readonly float cos1_32 = (float)(1.0 / (2.0 * Math.Cos(Math.PI / 32.0)));

		private static readonly float cos3_32 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 3.0 / 32.0)));

		private static readonly float cos5_32 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 5.0 / 32.0)));

		private static readonly float cos7_32 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 7.0 / 32.0)));

		private static readonly float cos9_32 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 9.0 / 32.0)));

		private static readonly float cos11_32 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 11.0 / 32.0)));

		private static readonly float cos13_32 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 13.0 / 32.0)));

		private static readonly float cos15_32 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 15.0 / 32.0)));

		private static readonly float cos1_16 = (float)(1.0 / (2.0 * Math.Cos(Math.PI / 16.0)));

		private static readonly float cos3_16 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 3.0 / 16.0)));

		private static readonly float cos5_16 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 5.0 / 16.0)));

		private static readonly float cos7_16 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 7.0 / 16.0)));

		private static readonly float cos1_8 = (float)(1.0 / (2.0 * Math.Cos(Math.PI / 8.0)));

		private static readonly float cos3_8 = (float)(1.0 / (2.0 * Math.Cos(Math.PI * 3.0 / 8.0)));

		private static readonly float cos1_4 = (float)(1.0 / (2.0 * Math.Cos(Math.PI / 4.0)));

		private static float[] d;

		private static float[][] d16;

		private static readonly float[] d_data = new float[512]
		{
			0f,
			-0.000442505f,
			213f / 65536f,
			-459f / 65536f,
			0.031082153f,
			-0.07862854f,
			0.10031128f,
			-0.57203674f,
			1.144989f,
			0.57203674f,
			0.10031128f,
			0.07862854f,
			0.031082153f,
			459f / 65536f,
			213f / 65536f,
			0.000442505f,
			-1.5259E-05f,
			-0.000473022f,
			109f / 32768f,
			-0.007919312f,
			125f / 4096f,
			-0.08418274f,
			0.090927124f,
			-0.6002197f,
			1.1442871f,
			0.54382324f,
			0.1088562f,
			0.07305908f,
			0.03147888f,
			0.006118774f,
			0.003173828f,
			0.000396729f,
			-1.5259E-05f,
			-0.000534058f,
			0.003387451f,
			-581f / 65536f,
			122f / 4096f,
			-0.08970642f,
			661f / 8192f,
			-0.6282959f,
			1.1422119f,
			0.51560974f,
			955f / 8192f,
			0.06752014f,
			130f / 4096f,
			0.0052948f,
			0.003082275f,
			0.000366211f,
			-1.5259E-05f,
			-19f / 32768f,
			0.003433228f,
			-645f / 65536f,
			0.028884888f,
			-0.09516907f,
			0.06959534f,
			-0.6562195f,
			1.1387634f,
			0.48747253f,
			0.12347412f,
			0.06199646f,
			0.031845093f,
			147f / 32768f,
			0.002990723f,
			0.000320435f,
			-1.5259E-05f,
			-0.00062561f,
			0.003463745f,
			-711f / 65536f,
			911f / 32768f,
			-0.10054016f,
			59f / 1024f,
			-0.6839142f,
			1.1339264f,
			0.45947266f,
			0.12957764f,
			0.056533813f,
			0.031814575f,
			0.003723145f,
			95f / 32768f,
			19f / 65536f,
			-1.5259E-05f,
			-0.000686646f,
			57f / 16384f,
			-779f / 65536f,
			0.026535034f,
			-0.1058197f,
			0.044784546f,
			-0.71131897f,
			1.1277466f,
			0.43165588f,
			0.1348877f,
			0.051132202f,
			0.031661987f,
			0.003005981f,
			0.002792358f,
			0.000259399f,
			-1.5259E-05f,
			-0.000747681f,
			57f / 16384f,
			-53f / 4096f,
			0.02508545f,
			-0.110946655f,
			0.031082153f,
			-0.7383728f,
			1.120224f,
			0.40408325f,
			0.13945007f,
			751f / 16384f,
			0.03138733f,
			0.002334595f,
			0.002685547f,
			0.000244141f,
			-3.0518E-05f,
			-0.000808716f,
			0.003463745f,
			-919f / 65536f,
			0.023422241f,
			-0.11592102f,
			0.01651001f,
			-0.7650299f,
			1.1113739f,
			0.37680054f,
			0.14326477f,
			0.040634155f,
			127f / 4096f,
			0.001693726f,
			0.002578735f,
			0.000213623f,
			-3.0518E-05f,
			-0.00088501f,
			0.003417969f,
			-0.01512146f,
			707f / 32768f,
			-0.12069702f,
			0.001068115f,
			-0.791214f,
			1.1012115f,
			0.34986877f,
			0.1463623f,
			0.03555298f,
			0.030532837f,
			0.001098633f,
			161f / 65536f,
			0.000198364f,
			-3.0518E-05f,
			-0.000961304f,
			0.003372192f,
			-133f / 8192f,
			5f / 256f,
			-0.1252594f,
			-0.015228271f,
			-0.816864f,
			1.0897827f,
			0.32331848f,
			0.1487732f,
			0.03060913f,
			981f / 32768f,
			0.000549316f,
			0.002349854f,
			0.000167847f,
			-3.0518E-05f,
			-0.001037598f,
			0.00328064f,
			-0.017349243f,
			0.01725769f,
			-0.12956238f,
			-0.03237915f,
			-0.84194946f,
			1.0771179f,
			0.2972107f,
			0.15049744f,
			423f / 16384f,
			0.029281616f,
			3.0518E-05f,
			147f / 65536f,
			0.000152588f,
			-4.5776E-05f,
			-0.001113892f,
			0.003173828f,
			-605f / 32768f,
			485f / 32768f,
			-0.1335907f,
			-825f / 16384f,
			-0.8663635f,
			1.0632172f,
			0.2715912f,
			0.15159607f,
			0.0211792f,
			935f / 32768f,
			-0.000442505f,
			0.002120972f,
			0.000137329f,
			-4.5776E-05f,
			-0.001205444f,
			0.003051758f,
			-0.019577026f,
			0.012115479f,
			-0.13729858f,
			-0.06916809f,
			-0.89009094f,
			1.0481567f,
			0.24650574f,
			0.15206909f,
			0.016708374f,
			0.02772522f,
			-57f / 65536f,
			0.00201416f,
			0.00012207f,
			-6.1035E-05f,
			-0.001296997f,
			0.002883911f,
			-339f / 16384f,
			605f / 65536f,
			-0.14067078f,
			-0.088775635f,
			-0.9130554f,
			1.0319366f,
			0.22198486f,
			0.15196228f,
			407f / 32768f,
			0.02684021f,
			-0.001266479f,
			0.001907349f,
			0.000106812f,
			-6.1035E-05f,
			-0.00138855f,
			0.002700806f,
			-0.02178955f,
			201f / 32768f,
			-0.14367676f,
			-0.10916138f,
			-0.9351959f,
			1.0146179f,
			0.19805908f,
			0.15130615f,
			0.00831604f,
			849f / 32768f,
			-0.001617432f,
			0.001785278f,
			0.000106812f,
			-7.6294E-05f,
			-0.001480103f,
			0.002487183f,
			-749f / 32768f,
			185f / 65536f,
			-0.1462555f,
			-0.13031006f,
			-0.95648193f,
			0.99624634f,
			0.17478943f,
			0.15011597f,
			0.004394531f,
			817f / 32768f,
			-0.001937866f,
			0.001693726f,
			9.1553E-05f,
			-7.6294E-05f,
			-0.001586914f,
			0.002227783f,
			-0.023910522f,
			-0.000686646f,
			-0.14842224f,
			-0.15220642f,
			-0.9768524f,
			0.9768524f,
			0.15220642f,
			0.14842224f,
			0.000686646f,
			0.023910522f,
			-0.002227783f,
			0.001586914f,
			7.6294E-05f,
			-9.1553E-05f,
			-0.001693726f,
			0.001937866f,
			-817f / 32768f,
			-0.004394531f,
			-0.15011597f,
			-0.17478943f,
			-0.99624634f,
			0.95648193f,
			0.13031006f,
			0.1462555f,
			-185f / 65536f,
			749f / 32768f,
			-0.002487183f,
			0.001480103f,
			7.6294E-05f,
			-0.000106812f,
			-0.001785278f,
			0.001617432f,
			-849f / 32768f,
			-0.00831604f,
			-0.15130615f,
			-0.19805908f,
			-1.0146179f,
			0.9351959f,
			0.10916138f,
			0.14367676f,
			-201f / 32768f,
			0.02178955f,
			-0.002700806f,
			0.00138855f,
			6.1035E-05f,
			-0.000106812f,
			-0.001907349f,
			0.001266479f,
			-0.02684021f,
			-407f / 32768f,
			-0.15196228f,
			-0.22198486f,
			-1.0319366f,
			0.9130554f,
			0.088775635f,
			0.14067078f,
			-605f / 65536f,
			339f / 16384f,
			-0.002883911f,
			0.001296997f,
			6.1035E-05f,
			-0.00012207f,
			-0.00201416f,
			57f / 65536f,
			-0.02772522f,
			-0.016708374f,
			-0.15206909f,
			-0.24650574f,
			-1.0481567f,
			0.89009094f,
			0.06916809f,
			0.13729858f,
			-0.012115479f,
			0.019577026f,
			-0.003051758f,
			0.001205444f,
			4.5776E-05f,
			-0.000137329f,
			-0.002120972f,
			0.000442505f,
			-935f / 32768f,
			-0.0211792f,
			-0.15159607f,
			-0.2715912f,
			-1.0632172f,
			0.8663635f,
			825f / 16384f,
			0.1335907f,
			-485f / 32768f,
			605f / 32768f,
			-0.003173828f,
			0.001113892f,
			4.5776E-05f,
			-0.000152588f,
			-147f / 65536f,
			-3.0518E-05f,
			-0.029281616f,
			-423f / 16384f,
			-0.15049744f,
			-0.2972107f,
			-1.0771179f,
			0.84194946f,
			0.03237915f,
			0.12956238f,
			-0.01725769f,
			0.017349243f,
			-0.00328064f,
			0.001037598f,
			3.0518E-05f,
			-0.000167847f,
			-0.002349854f,
			-0.000549316f,
			-981f / 32768f,
			-0.03060913f,
			-0.1487732f,
			-0.32331848f,
			-1.0897827f,
			0.816864f,
			0.015228271f,
			0.1252594f,
			-5f / 256f,
			133f / 8192f,
			-0.003372192f,
			0.000961304f,
			3.0518E-05f,
			-0.000198364f,
			-161f / 65536f,
			-0.001098633f,
			-0.030532837f,
			-0.03555298f,
			-0.1463623f,
			-0.34986877f,
			-1.1012115f,
			0.791214f,
			-0.001068115f,
			0.12069702f,
			-707f / 32768f,
			0.01512146f,
			-0.003417969f,
			0.00088501f,
			3.0518E-05f,
			-0.000213623f,
			-0.002578735f,
			-0.001693726f,
			-127f / 4096f,
			-0.040634155f,
			-0.14326477f,
			-0.37680054f,
			-1.1113739f,
			0.7650299f,
			-0.01651001f,
			0.11592102f,
			-0.023422241f,
			919f / 65536f,
			-0.003463745f,
			0.000808716f,
			3.0518E-05f,
			-0.000244141f,
			-0.002685547f,
			-0.002334595f,
			-0.03138733f,
			-751f / 16384f,
			-0.13945007f,
			-0.40408325f,
			-1.120224f,
			0.7383728f,
			-0.031082153f,
			0.110946655f,
			-0.02508545f,
			53f / 4096f,
			-57f / 16384f,
			0.000747681f,
			1.5259E-05f,
			-0.000259399f,
			-0.002792358f,
			-0.003005981f,
			-0.031661987f,
			-0.051132202f,
			-0.1348877f,
			-0.43165588f,
			-1.1277466f,
			0.71131897f,
			-0.044784546f,
			0.1058197f,
			-0.026535034f,
			779f / 65536f,
			-57f / 16384f,
			0.000686646f,
			1.5259E-05f,
			-19f / 65536f,
			-95f / 32768f,
			-0.003723145f,
			-0.031814575f,
			-0.056533813f,
			-0.12957764f,
			-0.45947266f,
			-1.1339264f,
			0.6839142f,
			-59f / 1024f,
			0.10054016f,
			-911f / 32768f,
			711f / 65536f,
			-0.003463745f,
			0.00062561f,
			1.5259E-05f,
			-0.000320435f,
			-0.002990723f,
			-147f / 32768f,
			-0.031845093f,
			-0.06199646f,
			-0.12347412f,
			-0.48747253f,
			-1.1387634f,
			0.6562195f,
			-0.06959534f,
			0.09516907f,
			-0.028884888f,
			645f / 65536f,
			-0.003433228f,
			19f / 32768f,
			1.5259E-05f,
			-0.000366211f,
			-0.003082275f,
			-0.0052948f,
			-130f / 4096f,
			-0.06752014f,
			-955f / 8192f,
			-0.51560974f,
			-1.1422119f,
			0.6282959f,
			-661f / 8192f,
			0.08970642f,
			-122f / 4096f,
			581f / 65536f,
			-0.003387451f,
			0.000534058f,
			1.5259E-05f,
			-0.000396729f,
			-0.003173828f,
			-0.006118774f,
			-0.03147888f,
			-0.07305908f,
			-0.1088562f,
			-0.54382324f,
			-1.1442871f,
			0.6002197f,
			-0.090927124f,
			0.08418274f,
			-125f / 4096f,
			0.007919312f,
			-109f / 32768f,
			0.000473022f,
			1.5259E-05f
		};

		private readonly int m_ChannelIndex;

		private readonly float[] m_SubbandSamples;

		private readonly float scalefactor;

		private readonly float[] v1;

		private readonly float[] v2;

		private float[] _tmpOut;

		private float[] actual_v;

		private int actual_write_pos;

		private float[] eq;

		public float[] EQ
		{
			set
			{
				eq = value;
				if (eq == null)
				{
					eq = new float[32];
					for (int i = 0; i < 32; i++)
					{
						eq[i] = 1f;
					}
				}
				if (eq.Length < 32)
				{
					throw new ArgumentException("eq0");
				}
			}
		}

		public SynthesisFilter(int channelIndex, float factor, float[] eq0)
		{
			InitBlock();
			if (d == null)
			{
				d = d_data;
				d16 = splitArray(d, 16);
			}
			v1 = new float[512];
			v2 = new float[512];
			m_SubbandSamples = new float[32];
			m_ChannelIndex = channelIndex;
			scalefactor = factor;
			EQ = eq;
			reset();
		}

		private void InitBlock()
		{
			_tmpOut = new float[32];
		}

		public void reset()
		{
			for (int i = 0; i < 512; i++)
			{
				v1[i] = (v2[i] = 0f);
			}
			for (int j = 0; j < 32; j++)
			{
				m_SubbandSamples[j] = 0f;
			}
			actual_v = v1;
			actual_write_pos = 15;
		}

		public void WriteSample(float sample, int subbandIndex)
		{
			m_SubbandSamples[subbandIndex] = eq[subbandIndex] * sample;
		}

		public void WriteAllSamples(float[] s)
		{
			for (int num = 31; num >= 0; num--)
			{
				m_SubbandSamples[num] = s[num] * eq[num];
			}
		}

		private void compute_new_v()
		{
			float[] subbandSamples = m_SubbandSamples;
			float num = subbandSamples[0];
			float num2 = subbandSamples[1];
			float num3 = subbandSamples[2];
			float num4 = subbandSamples[3];
			float num5 = subbandSamples[4];
			float num6 = subbandSamples[5];
			float num7 = subbandSamples[6];
			float num8 = subbandSamples[7];
			float num9 = subbandSamples[8];
			float num10 = subbandSamples[9];
			float num11 = subbandSamples[10];
			float num12 = subbandSamples[11];
			float num13 = subbandSamples[12];
			float num14 = subbandSamples[13];
			float num15 = subbandSamples[14];
			float num16 = subbandSamples[15];
			float num17 = subbandSamples[16];
			float num18 = subbandSamples[17];
			float num19 = subbandSamples[18];
			float num20 = subbandSamples[19];
			float num21 = subbandSamples[20];
			float num22 = subbandSamples[21];
			float num23 = subbandSamples[22];
			float num24 = subbandSamples[23];
			float num25 = subbandSamples[24];
			float num26 = subbandSamples[25];
			float num27 = subbandSamples[26];
			float num28 = subbandSamples[27];
			float num29 = subbandSamples[28];
			float num30 = subbandSamples[29];
			float num31 = subbandSamples[30];
			float num32 = subbandSamples[31];
			float num33 = num + num32;
			float num34 = num2 + num31;
			float num35 = num3 + num30;
			float num36 = num4 + num29;
			float num37 = num5 + num28;
			float num38 = num6 + num27;
			float num39 = num7 + num26;
			float num40 = num8 + num25;
			float num41 = num9 + num24;
			float num42 = num10 + num23;
			float num43 = num11 + num22;
			float num44 = num12 + num21;
			float num45 = num13 + num20;
			float num46 = num14 + num19;
			float num47 = num15 + num18;
			float num48 = num16 + num17;
			float num49 = num33 + num48;
			float num50 = num34 + num47;
			float num51 = num35 + num46;
			float num52 = num36 + num45;
			float num53 = num37 + num44;
			float num54 = num38 + num43;
			float num55 = num39 + num42;
			float num56 = num40 + num41;
			float num57 = (num33 - num48) * cos1_32;
			float num58 = (num34 - num47) * cos3_32;
			float num59 = (num35 - num46) * cos5_32;
			float num60 = (num36 - num45) * cos7_32;
			float num61 = (num37 - num44) * cos9_32;
			float num62 = (num38 - num43) * cos11_32;
			float num63 = (num39 - num42) * cos13_32;
			float num64 = (num40 - num41) * cos15_32;
			num33 = num49 + num56;
			num34 = num50 + num55;
			num35 = num51 + num54;
			num36 = num52 + num53;
			num37 = (num49 - num56) * cos1_16;
			num38 = (num50 - num55) * cos3_16;
			num39 = (num51 - num54) * cos5_16;
			num40 = (num52 - num53) * cos7_16;
			num41 = num57 + num64;
			num42 = num58 + num63;
			num43 = num59 + num62;
			num44 = num60 + num61;
			num45 = (num57 - num64) * cos1_16;
			num46 = (num58 - num63) * cos3_16;
			num47 = (num59 - num62) * cos5_16;
			num48 = (num60 - num61) * cos7_16;
			float num65 = num33 + num36;
			num50 = num34 + num35;
			num51 = (num33 - num36) * cos1_8;
			num52 = (num34 - num35) * cos3_8;
			num53 = num37 + num40;
			num54 = num38 + num39;
			num55 = (num37 - num40) * cos1_8;
			num56 = (num38 - num39) * cos3_8;
			num57 = num41 + num44;
			num58 = num42 + num43;
			num59 = (num41 - num44) * cos1_8;
			num60 = (num42 - num43) * cos3_8;
			num61 = num45 + num48;
			num62 = num46 + num47;
			num63 = (num45 - num48) * cos1_8;
			num64 = (num46 - num47) * cos3_8;
			num33 = num65 + num50;
			num34 = (num65 - num50) * cos1_4;
			num35 = num51 + num52;
			num36 = (num51 - num52) * cos1_4;
			num37 = num53 + num54;
			num38 = (num53 - num54) * cos1_4;
			num39 = num55 + num56;
			num40 = (num55 - num56) * cos1_4;
			num41 = num57 + num58;
			num42 = (num57 - num58) * cos1_4;
			num43 = num59 + num60;
			num44 = (num59 - num60) * cos1_4;
			num45 = num61 + num62;
			num46 = (num61 - num62) * cos1_4;
			num47 = num63 + num64;
			num48 = (num63 - num64) * cos1_4;
			float num67;
			float num68;
			float num66 = 0f - (num67 = (num68 = num40) + num38) - num39;
			float num69 = 0f - num39 - num40 - num37;
			float num71;
			float num72;
			float num70 = (num71 = (num72 = num48) + num44) + num46;
			float num74;
			float num73 = 0f - (num74 = num48 + num46 + num42) - num47;
			float num75 = 0f - num47 - num48 - num43 - num44;
			float num76 = num75 - num46;
			float num77 = 0f - num47 - num48 - num45 - num41;
			float num78 = num75 - num45;
			float num79 = 0f - num33;
			float num80 = num34;
			float num82;
			float num81 = 0f - (num82 = num36) - num35;
			num33 = (num - num32) * cos1_64;
			num34 = (num2 - num31) * cos3_64;
			num35 = (num3 - num30) * cos5_64;
			num36 = (num4 - num29) * cos7_64;
			num37 = (num5 - num28) * cos9_64;
			num38 = (num6 - num27) * cos11_64;
			num39 = (num7 - num26) * cos13_64;
			num40 = (num8 - num25) * cos15_64;
			num41 = (num9 - num24) * cos17_64;
			num42 = (num10 - num23) * cos19_64;
			num43 = (num11 - num22) * cos21_64;
			num44 = (num12 - num21) * cos23_64;
			num45 = (num13 - num20) * cos25_64;
			num46 = (num14 - num19) * cos27_64;
			num47 = (num15 - num18) * cos29_64;
			num48 = (num16 - num17) * cos31_64;
			float num83 = num33 + num48;
			num50 = num34 + num47;
			num51 = num35 + num46;
			num52 = num36 + num45;
			num53 = num37 + num44;
			num54 = num38 + num43;
			num55 = num39 + num42;
			num56 = num40 + num41;
			num57 = (num33 - num48) * cos1_32;
			num58 = (num34 - num47) * cos3_32;
			num59 = (num35 - num46) * cos5_32;
			num60 = (num36 - num45) * cos7_32;
			num61 = (num37 - num44) * cos9_32;
			num62 = (num38 - num43) * cos11_32;
			num63 = (num39 - num42) * cos13_32;
			num64 = (num40 - num41) * cos15_32;
			num33 = num83 + num56;
			num34 = num50 + num55;
			num35 = num51 + num54;
			num36 = num52 + num53;
			num37 = (num83 - num56) * cos1_16;
			num38 = (num50 - num55) * cos3_16;
			num39 = (num51 - num54) * cos5_16;
			num40 = (num52 - num53) * cos7_16;
			num41 = num57 + num64;
			num42 = num58 + num63;
			num43 = num59 + num62;
			num44 = num60 + num61;
			num45 = (num57 - num64) * cos1_16;
			num46 = (num58 - num63) * cos3_16;
			num47 = (num59 - num62) * cos5_16;
			num48 = (num60 - num61) * cos7_16;
			float num84 = num33 + num36;
			num50 = num34 + num35;
			num51 = (num33 - num36) * cos1_8;
			num52 = (num34 - num35) * cos3_8;
			num53 = num37 + num40;
			num54 = num38 + num39;
			num55 = (num37 - num40) * cos1_8;
			num56 = (num38 - num39) * cos3_8;
			num57 = num41 + num44;
			num58 = num42 + num43;
			num59 = (num41 - num44) * cos1_8;
			num60 = (num42 - num43) * cos3_8;
			num61 = num45 + num48;
			num62 = num46 + num47;
			num63 = (num45 - num48) * cos1_8;
			num64 = (num46 - num47) * cos3_8;
			num33 = num84 + num50;
			num34 = (num84 - num50) * cos1_4;
			num35 = num51 + num52;
			num36 = (num51 - num52) * cos1_4;
			num37 = num53 + num54;
			num38 = (num53 - num54) * cos1_4;
			num39 = num55 + num56;
			num40 = (num55 - num56) * cos1_4;
			num41 = num57 + num58;
			num42 = (num57 - num58) * cos1_4;
			num43 = num59 + num60;
			num44 = (num59 - num60) * cos1_4;
			num45 = num61 + num62;
			num46 = (num61 - num62) * cos1_4;
			num47 = num63 + num64;
			num48 = (num63 - num64) * cos1_4;
			float num86;
			float num87;
			float num88;
			float num85 = (num86 = (num87 = (num88 = num48) + num40) + num44) + num38 + num46;
			float num90;
			float num89 = (num90 = num48 + num44 + num36) + num46;
			float num91 = num46 + num48 + num42;
			float num93;
			float num92 = 0f - (num93 = num91 + num34) - num47;
			float num95;
			float num94 = 0f - (num95 = num91 + num38 + num40) - num39 - num47;
			float num96 = 0f - num43 - num44 - num47 - num48;
			float num97 = num96 - num46 - num35 - num36;
			float num98 = num96 - num46 - num38 - num39 - num40;
			float num99 = num96 - num45 - num35 - num36;
			float num101;
			float num100 = num96 - num45 - (num101 = num37 + num39 + num40);
			float num102 = 0f - num41 - num45 - num47 - num48;
			float num103 = num102 - num33;
			float num104 = num102 - num101;
			float[] array = actual_v;
			int num105 = actual_write_pos;
			array[num105] = num80;
			array[16 + num105] = num93;
			array[32 + num105] = num74;
			array[48 + num105] = num95;
			array[64 + num105] = num67;
			array[80 + num105] = num85;
			array[96 + num105] = num70;
			array[112 + num105] = num89;
			array[128 + num105] = num82;
			array[144 + num105] = num90;
			array[160 + num105] = num71;
			array[176 + num105] = num86;
			array[192 + num105] = num68;
			array[208 + num105] = num87;
			array[224 + num105] = num72;
			array[240 + num105] = num88;
			array[256 + num105] = 0f;
			array[272 + num105] = 0f - num88;
			array[288 + num105] = 0f - num72;
			array[304 + num105] = 0f - num87;
			array[320 + num105] = 0f - num68;
			array[336 + num105] = 0f - num86;
			array[352 + num105] = 0f - num71;
			array[368 + num105] = 0f - num90;
			array[384 + num105] = 0f - num82;
			array[400 + num105] = 0f - num89;
			array[416 + num105] = 0f - num70;
			array[432 + num105] = 0f - num85;
			array[448 + num105] = 0f - num67;
			array[464 + num105] = 0f - num95;
			array[480 + num105] = 0f - num74;
			array[496 + num105] = 0f - num93;
			float[] obj = ((actual_v == v1) ? v2 : v1);
			obj[num105] = 0f - num80;
			obj[16 + num105] = num92;
			obj[32 + num105] = num73;
			obj[48 + num105] = num94;
			obj[64 + num105] = num66;
			obj[80 + num105] = num98;
			obj[96 + num105] = num76;
			obj[112 + num105] = num97;
			obj[128 + num105] = num81;
			obj[144 + num105] = num99;
			obj[160 + num105] = num78;
			obj[176 + num105] = num100;
			obj[192 + num105] = num69;
			obj[208 + num105] = num104;
			obj[224 + num105] = num77;
			obj[240 + num105] = num103;
			obj[256 + num105] = num79;
			obj[272 + num105] = num103;
			obj[288 + num105] = num77;
			obj[304 + num105] = num104;
			obj[320 + num105] = num69;
			obj[336 + num105] = num100;
			obj[352 + num105] = num78;
			obj[368 + num105] = num99;
			obj[384 + num105] = num81;
			obj[400 + num105] = num97;
			obj[416 + num105] = num76;
			obj[432 + num105] = num98;
			obj[448 + num105] = num66;
			obj[464 + num105] = num94;
			obj[480 + num105] = num73;
			obj[496 + num105] = num92;
		}

		private void compute_new_v_old()
		{
			float[] array = new float[32];
			float[] array2 = new float[16];
			float[] array3 = new float[16];
			for (int num = 31; num >= 0; num--)
			{
				array[num] = 0f;
			}
			float[] subbandSamples = m_SubbandSamples;
			array2[0] = subbandSamples[0] + subbandSamples[31];
			array2[1] = subbandSamples[1] + subbandSamples[30];
			array2[2] = subbandSamples[2] + subbandSamples[29];
			array2[3] = subbandSamples[3] + subbandSamples[28];
			array2[4] = subbandSamples[4] + subbandSamples[27];
			array2[5] = subbandSamples[5] + subbandSamples[26];
			array2[6] = subbandSamples[6] + subbandSamples[25];
			array2[7] = subbandSamples[7] + subbandSamples[24];
			array2[8] = subbandSamples[8] + subbandSamples[23];
			array2[9] = subbandSamples[9] + subbandSamples[22];
			array2[10] = subbandSamples[10] + subbandSamples[21];
			array2[11] = subbandSamples[11] + subbandSamples[20];
			array2[12] = subbandSamples[12] + subbandSamples[19];
			array2[13] = subbandSamples[13] + subbandSamples[18];
			array2[14] = subbandSamples[14] + subbandSamples[17];
			array2[15] = subbandSamples[15] + subbandSamples[16];
			array3[0] = array2[0] + array2[15];
			array3[1] = array2[1] + array2[14];
			array3[2] = array2[2] + array2[13];
			array3[3] = array2[3] + array2[12];
			array3[4] = array2[4] + array2[11];
			array3[5] = array2[5] + array2[10];
			array3[6] = array2[6] + array2[9];
			array3[7] = array2[7] + array2[8];
			array3[8] = (array2[0] - array2[15]) * cos1_32;
			array3[9] = (array2[1] - array2[14]) * cos3_32;
			array3[10] = (array2[2] - array2[13]) * cos5_32;
			array3[11] = (array2[3] - array2[12]) * cos7_32;
			array3[12] = (array2[4] - array2[11]) * cos9_32;
			array3[13] = (array2[5] - array2[10]) * cos11_32;
			array3[14] = (array2[6] - array2[9]) * cos13_32;
			array3[15] = (array2[7] - array2[8]) * cos15_32;
			array2[0] = array3[0] + array3[7];
			array2[1] = array3[1] + array3[6];
			array2[2] = array3[2] + array3[5];
			array2[3] = array3[3] + array3[4];
			array2[4] = (array3[0] - array3[7]) * cos1_16;
			array2[5] = (array3[1] - array3[6]) * cos3_16;
			array2[6] = (array3[2] - array3[5]) * cos5_16;
			array2[7] = (array3[3] - array3[4]) * cos7_16;
			array2[8] = array3[8] + array3[15];
			array2[9] = array3[9] + array3[14];
			array2[10] = array3[10] + array3[13];
			array2[11] = array3[11] + array3[12];
			array2[12] = (array3[8] - array3[15]) * cos1_16;
			array2[13] = (array3[9] - array3[14]) * cos3_16;
			array2[14] = (array3[10] - array3[13]) * cos5_16;
			array2[15] = (array3[11] - array3[12]) * cos7_16;
			array3[0] = array2[0] + array2[3];
			array3[1] = array2[1] + array2[2];
			array3[2] = (array2[0] - array2[3]) * cos1_8;
			array3[3] = (array2[1] - array2[2]) * cos3_8;
			array3[4] = array2[4] + array2[7];
			array3[5] = array2[5] + array2[6];
			array3[6] = (array2[4] - array2[7]) * cos1_8;
			array3[7] = (array2[5] - array2[6]) * cos3_8;
			array3[8] = array2[8] + array2[11];
			array3[9] = array2[9] + array2[10];
			array3[10] = (array2[8] - array2[11]) * cos1_8;
			array3[11] = (array2[9] - array2[10]) * cos3_8;
			array3[12] = array2[12] + array2[15];
			array3[13] = array2[13] + array2[14];
			array3[14] = (array2[12] - array2[15]) * cos1_8;
			array3[15] = (array2[13] - array2[14]) * cos3_8;
			array2[0] = array3[0] + array3[1];
			array2[1] = (array3[0] - array3[1]) * cos1_4;
			array2[2] = array3[2] + array3[3];
			array2[3] = (array3[2] - array3[3]) * cos1_4;
			array2[4] = array3[4] + array3[5];
			array2[5] = (array3[4] - array3[5]) * cos1_4;
			array2[6] = array3[6] + array3[7];
			array2[7] = (array3[6] - array3[7]) * cos1_4;
			array2[8] = array3[8] + array3[9];
			array2[9] = (array3[8] - array3[9]) * cos1_4;
			array2[10] = array3[10] + array3[11];
			array2[11] = (array3[10] - array3[11]) * cos1_4;
			array2[12] = array3[12] + array3[13];
			array2[13] = (array3[12] - array3[13]) * cos1_4;
			array2[14] = array3[14] + array3[15];
			array2[15] = (array3[14] - array3[15]) * cos1_4;
			array[19] = 0f - (array[4] = (array[12] = array2[7]) + array2[5]) - array2[6];
			array[27] = 0f - array2[6] - array2[7] - array2[4];
			array[6] = (array[10] = (array[14] = array2[15]) + array2[11]) + array2[13];
			array[17] = 0f - (array[2] = array2[15] + array2[13] + array2[9]) - array2[14];
			float num2;
			array[21] = (num2 = 0f - array2[14] - array2[15] - array2[10] - array2[11]) - array2[13];
			array[29] = 0f - array2[14] - array2[15] - array2[12] - array2[8];
			array[25] = num2 - array2[12];
			array[31] = 0f - array2[0];
			array[0] = array2[1];
			array[23] = 0f - (array[8] = array2[3]) - array2[2];
			array2[0] = (subbandSamples[0] - subbandSamples[31]) * cos1_64;
			array2[1] = (subbandSamples[1] - subbandSamples[30]) * cos3_64;
			array2[2] = (subbandSamples[2] - subbandSamples[29]) * cos5_64;
			array2[3] = (subbandSamples[3] - subbandSamples[28]) * cos7_64;
			array2[4] = (subbandSamples[4] - subbandSamples[27]) * cos9_64;
			array2[5] = (subbandSamples[5] - subbandSamples[26]) * cos11_64;
			array2[6] = (subbandSamples[6] - subbandSamples[25]) * cos13_64;
			array2[7] = (subbandSamples[7] - subbandSamples[24]) * cos15_64;
			array2[8] = (subbandSamples[8] - subbandSamples[23]) * cos17_64;
			array2[9] = (subbandSamples[9] - subbandSamples[22]) * cos19_64;
			array2[10] = (subbandSamples[10] - subbandSamples[21]) * cos21_64;
			array2[11] = (subbandSamples[11] - subbandSamples[20]) * cos23_64;
			array2[12] = (subbandSamples[12] - subbandSamples[19]) * cos25_64;
			array2[13] = (subbandSamples[13] - subbandSamples[18]) * cos27_64;
			array2[14] = (subbandSamples[14] - subbandSamples[17]) * cos29_64;
			array2[15] = (subbandSamples[15] - subbandSamples[16]) * cos31_64;
			array3[0] = array2[0] + array2[15];
			array3[1] = array2[1] + array2[14];
			array3[2] = array2[2] + array2[13];
			array3[3] = array2[3] + array2[12];
			array3[4] = array2[4] + array2[11];
			array3[5] = array2[5] + array2[10];
			array3[6] = array2[6] + array2[9];
			array3[7] = array2[7] + array2[8];
			array3[8] = (array2[0] - array2[15]) * cos1_32;
			array3[9] = (array2[1] - array2[14]) * cos3_32;
			array3[10] = (array2[2] - array2[13]) * cos5_32;
			array3[11] = (array2[3] - array2[12]) * cos7_32;
			array3[12] = (array2[4] - array2[11]) * cos9_32;
			array3[13] = (array2[5] - array2[10]) * cos11_32;
			array3[14] = (array2[6] - array2[9]) * cos13_32;
			array3[15] = (array2[7] - array2[8]) * cos15_32;
			array2[0] = array3[0] + array3[7];
			array2[1] = array3[1] + array3[6];
			array2[2] = array3[2] + array3[5];
			array2[3] = array3[3] + array3[4];
			array2[4] = (array3[0] - array3[7]) * cos1_16;
			array2[5] = (array3[1] - array3[6]) * cos3_16;
			array2[6] = (array3[2] - array3[5]) * cos5_16;
			array2[7] = (array3[3] - array3[4]) * cos7_16;
			array2[8] = array3[8] + array3[15];
			array2[9] = array3[9] + array3[14];
			array2[10] = array3[10] + array3[13];
			array2[11] = array3[11] + array3[12];
			array2[12] = (array3[8] - array3[15]) * cos1_16;
			array2[13] = (array3[9] - array3[14]) * cos3_16;
			array2[14] = (array3[10] - array3[13]) * cos5_16;
			array2[15] = (array3[11] - array3[12]) * cos7_16;
			array3[0] = array2[0] + array2[3];
			array3[1] = array2[1] + array2[2];
			array3[2] = (array2[0] - array2[3]) * cos1_8;
			array3[3] = (array2[1] - array2[2]) * cos3_8;
			array3[4] = array2[4] + array2[7];
			array3[5] = array2[5] + array2[6];
			array3[6] = (array2[4] - array2[7]) * cos1_8;
			array3[7] = (array2[5] - array2[6]) * cos3_8;
			array3[8] = array2[8] + array2[11];
			array3[9] = array2[9] + array2[10];
			array3[10] = (array2[8] - array2[11]) * cos1_8;
			array3[11] = (array2[9] - array2[10]) * cos3_8;
			array3[12] = array2[12] + array2[15];
			array3[13] = array2[13] + array2[14];
			array3[14] = (array2[12] - array2[15]) * cos1_8;
			array3[15] = (array2[13] - array2[14]) * cos3_8;
			array2[0] = array3[0] + array3[1];
			array2[1] = (array3[0] - array3[1]) * cos1_4;
			array2[2] = array3[2] + array3[3];
			array2[3] = (array3[2] - array3[3]) * cos1_4;
			array2[4] = array3[4] + array3[5];
			array2[5] = (array3[4] - array3[5]) * cos1_4;
			array2[6] = array3[6] + array3[7];
			array2[7] = (array3[6] - array3[7]) * cos1_4;
			array2[8] = array3[8] + array3[9];
			array2[9] = (array3[8] - array3[9]) * cos1_4;
			array2[10] = array3[10] + array3[11];
			array2[11] = (array3[10] - array3[11]) * cos1_4;
			array2[12] = array3[12] + array3[13];
			array2[13] = (array3[12] - array3[13]) * cos1_4;
			array2[14] = array3[14] + array3[15];
			array2[15] = (array3[14] - array3[15]) * cos1_4;
			array[5] = (array[11] = (array[13] = (array[15] = array2[15]) + array2[7]) + array2[11]) + array2[5] + array2[13];
			array[7] = (array[9] = array2[15] + array2[11] + array2[3]) + array2[13];
			array[16] = 0f - (array[1] = (num2 = array2[13] + array2[15] + array2[9]) + array2[1]) - array2[14];
			array[18] = 0f - (array[3] = num2 + array2[5] + array2[7]) - array2[6] - array2[14];
			array[22] = (num2 = 0f - array2[10] - array2[11] - array2[14] - array2[15]) - array2[13] - array2[2] - array2[3];
			array[20] = num2 - array2[13] - array2[5] - array2[6] - array2[7];
			array[24] = num2 - array2[12] - array2[2] - array2[3];
			float num3;
			array[26] = num2 - array2[12] - (num3 = array2[4] + array2[6] + array2[7]);
			array[30] = (num2 = 0f - array2[8] - array2[12] - array2[14] - array2[15]) - array2[0];
			array[28] = num2 - num3;
			subbandSamples = array;
			float[] array4 = actual_v;
			array4[actual_write_pos] = subbandSamples[0];
			array4[16 + actual_write_pos] = subbandSamples[1];
			array4[32 + actual_write_pos] = subbandSamples[2];
			array4[48 + actual_write_pos] = subbandSamples[3];
			array4[64 + actual_write_pos] = subbandSamples[4];
			array4[80 + actual_write_pos] = subbandSamples[5];
			array4[96 + actual_write_pos] = subbandSamples[6];
			array4[112 + actual_write_pos] = subbandSamples[7];
			array4[128 + actual_write_pos] = subbandSamples[8];
			array4[144 + actual_write_pos] = subbandSamples[9];
			array4[160 + actual_write_pos] = subbandSamples[10];
			array4[176 + actual_write_pos] = subbandSamples[11];
			array4[192 + actual_write_pos] = subbandSamples[12];
			array4[208 + actual_write_pos] = subbandSamples[13];
			array4[224 + actual_write_pos] = subbandSamples[14];
			array4[240 + actual_write_pos] = subbandSamples[15];
			array4[256 + actual_write_pos] = 0f;
			array4[272 + actual_write_pos] = 0f - subbandSamples[15];
			array4[288 + actual_write_pos] = 0f - subbandSamples[14];
			array4[304 + actual_write_pos] = 0f - subbandSamples[13];
			array4[320 + actual_write_pos] = 0f - subbandSamples[12];
			array4[336 + actual_write_pos] = 0f - subbandSamples[11];
			array4[352 + actual_write_pos] = 0f - subbandSamples[10];
			array4[368 + actual_write_pos] = 0f - subbandSamples[9];
			array4[384 + actual_write_pos] = 0f - subbandSamples[8];
			array4[400 + actual_write_pos] = 0f - subbandSamples[7];
			array4[416 + actual_write_pos] = 0f - subbandSamples[6];
			array4[432 + actual_write_pos] = 0f - subbandSamples[5];
			array4[448 + actual_write_pos] = 0f - subbandSamples[4];
			array4[464 + actual_write_pos] = 0f - subbandSamples[3];
			array4[480 + actual_write_pos] = 0f - subbandSamples[2];
			array4[496 + actual_write_pos] = 0f - subbandSamples[1];
		}

		private void compute_pcm_samples0(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[num] * array2[0] + array[15 + num] * array2[1] + array[14 + num] * array2[2] + array[13 + num] * array2[3] + array[12 + num] * array2[4] + array[11 + num] * array2[5] + array[10 + num] * array2[6] + array[9 + num] * array2[7] + array[8 + num] * array2[8] + array[7 + num] * array2[9] + array[6 + num] * array2[10] + array[5 + num] * array2[11] + array[4 + num] * array2[12] + array[3 + num] * array2[13] + array[2 + num] * array2[14] + array[1 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples1(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[1 + num] * array2[0] + array[num] * array2[1] + array[15 + num] * array2[2] + array[14 + num] * array2[3] + array[13 + num] * array2[4] + array[12 + num] * array2[5] + array[11 + num] * array2[6] + array[10 + num] * array2[7] + array[9 + num] * array2[8] + array[8 + num] * array2[9] + array[7 + num] * array2[10] + array[6 + num] * array2[11] + array[5 + num] * array2[12] + array[4 + num] * array2[13] + array[3 + num] * array2[14] + array[2 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples2(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[2 + num] * array2[0] + array[1 + num] * array2[1] + array[num] * array2[2] + array[15 + num] * array2[3] + array[14 + num] * array2[4] + array[13 + num] * array2[5] + array[12 + num] * array2[6] + array[11 + num] * array2[7] + array[10 + num] * array2[8] + array[9 + num] * array2[9] + array[8 + num] * array2[10] + array[7 + num] * array2[11] + array[6 + num] * array2[12] + array[5 + num] * array2[13] + array[4 + num] * array2[14] + array[3 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples3(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[3 + num] * array2[0] + array[2 + num] * array2[1] + array[1 + num] * array2[2] + array[num] * array2[3] + array[15 + num] * array2[4] + array[14 + num] * array2[5] + array[13 + num] * array2[6] + array[12 + num] * array2[7] + array[11 + num] * array2[8] + array[10 + num] * array2[9] + array[9 + num] * array2[10] + array[8 + num] * array2[11] + array[7 + num] * array2[12] + array[6 + num] * array2[13] + array[5 + num] * array2[14] + array[4 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples4(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[4 + num] * array2[0] + array[3 + num] * array2[1] + array[2 + num] * array2[2] + array[1 + num] * array2[3] + array[num] * array2[4] + array[15 + num] * array2[5] + array[14 + num] * array2[6] + array[13 + num] * array2[7] + array[12 + num] * array2[8] + array[11 + num] * array2[9] + array[10 + num] * array2[10] + array[9 + num] * array2[11] + array[8 + num] * array2[12] + array[7 + num] * array2[13] + array[6 + num] * array2[14] + array[5 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples5(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[5 + num] * array2[0] + array[4 + num] * array2[1] + array[3 + num] * array2[2] + array[2 + num] * array2[3] + array[1 + num] * array2[4] + array[num] * array2[5] + array[15 + num] * array2[6] + array[14 + num] * array2[7] + array[13 + num] * array2[8] + array[12 + num] * array2[9] + array[11 + num] * array2[10] + array[10 + num] * array2[11] + array[9 + num] * array2[12] + array[8 + num] * array2[13] + array[7 + num] * array2[14] + array[6 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples6(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[6 + num] * array2[0] + array[5 + num] * array2[1] + array[4 + num] * array2[2] + array[3 + num] * array2[3] + array[2 + num] * array2[4] + array[1 + num] * array2[5] + array[num] * array2[6] + array[15 + num] * array2[7] + array[14 + num] * array2[8] + array[13 + num] * array2[9] + array[12 + num] * array2[10] + array[11 + num] * array2[11] + array[10 + num] * array2[12] + array[9 + num] * array2[13] + array[8 + num] * array2[14] + array[7 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples7(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[7 + num] * array2[0] + array[6 + num] * array2[1] + array[5 + num] * array2[2] + array[4 + num] * array2[3] + array[3 + num] * array2[4] + array[2 + num] * array2[5] + array[1 + num] * array2[6] + array[num] * array2[7] + array[15 + num] * array2[8] + array[14 + num] * array2[9] + array[13 + num] * array2[10] + array[12 + num] * array2[11] + array[11 + num] * array2[12] + array[10 + num] * array2[13] + array[9 + num] * array2[14] + array[8 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples8(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[8 + num] * array2[0] + array[7 + num] * array2[1] + array[6 + num] * array2[2] + array[5 + num] * array2[3] + array[4 + num] * array2[4] + array[3 + num] * array2[5] + array[2 + num] * array2[6] + array[1 + num] * array2[7] + array[num] * array2[8] + array[15 + num] * array2[9] + array[14 + num] * array2[10] + array[13 + num] * array2[11] + array[12 + num] * array2[12] + array[11 + num] * array2[13] + array[10 + num] * array2[14] + array[9 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples9(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[9 + num] * array2[0] + array[8 + num] * array2[1] + array[7 + num] * array2[2] + array[6 + num] * array2[3] + array[5 + num] * array2[4] + array[4 + num] * array2[5] + array[3 + num] * array2[6] + array[2 + num] * array2[7] + array[1 + num] * array2[8] + array[num] * array2[9] + array[15 + num] * array2[10] + array[14 + num] * array2[11] + array[13 + num] * array2[12] + array[12 + num] * array2[13] + array[11 + num] * array2[14] + array[10 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples10(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[10 + num] * array2[0] + array[9 + num] * array2[1] + array[8 + num] * array2[2] + array[7 + num] * array2[3] + array[6 + num] * array2[4] + array[5 + num] * array2[5] + array[4 + num] * array2[6] + array[3 + num] * array2[7] + array[2 + num] * array2[8] + array[1 + num] * array2[9] + array[num] * array2[10] + array[15 + num] * array2[11] + array[14 + num] * array2[12] + array[13 + num] * array2[13] + array[12 + num] * array2[14] + array[11 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples11(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[11 + num] * array2[0] + array[10 + num] * array2[1] + array[9 + num] * array2[2] + array[8 + num] * array2[3] + array[7 + num] * array2[4] + array[6 + num] * array2[5] + array[5 + num] * array2[6] + array[4 + num] * array2[7] + array[3 + num] * array2[8] + array[2 + num] * array2[9] + array[1 + num] * array2[10] + array[num] * array2[11] + array[15 + num] * array2[12] + array[14 + num] * array2[13] + array[13 + num] * array2[14] + array[12 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples12(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[12 + num] * array2[0] + array[11 + num] * array2[1] + array[10 + num] * array2[2] + array[9 + num] * array2[3] + array[8 + num] * array2[4] + array[7 + num] * array2[5] + array[6 + num] * array2[6] + array[5 + num] * array2[7] + array[4 + num] * array2[8] + array[3 + num] * array2[9] + array[2 + num] * array2[10] + array[1 + num] * array2[11] + array[num] * array2[12] + array[15 + num] * array2[13] + array[14 + num] * array2[14] + array[13 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples13(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[13 + num] * array2[0] + array[12 + num] * array2[1] + array[11 + num] * array2[2] + array[10 + num] * array2[3] + array[9 + num] * array2[4] + array[8 + num] * array2[5] + array[7 + num] * array2[6] + array[6 + num] * array2[7] + array[5 + num] * array2[8] + array[4 + num] * array2[9] + array[3 + num] * array2[10] + array[2 + num] * array2[11] + array[1 + num] * array2[12] + array[num] * array2[13] + array[15 + num] * array2[14] + array[14 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples14(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[14 + num] * array2[0] + array[13 + num] * array2[1] + array[12 + num] * array2[2] + array[11 + num] * array2[3] + array[10 + num] * array2[4] + array[9 + num] * array2[5] + array[8 + num] * array2[6] + array[7 + num] * array2[7] + array[6 + num] * array2[8] + array[5 + num] * array2[9] + array[4 + num] * array2[10] + array[3 + num] * array2[11] + array[2 + num] * array2[12] + array[1 + num] * array2[13] + array[num] * array2[14] + array[15 + num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples15(ABuffer buffer)
		{
			float[] array = actual_v;
			float[] tmpOut = _tmpOut;
			int num = 0;
			for (int i = 0; i < 32; i++)
			{
				float[] array2 = d16[i];
				float num2 = (array[15 + num] * array2[0] + array[14 + num] * array2[1] + array[13 + num] * array2[2] + array[12 + num] * array2[3] + array[11 + num] * array2[4] + array[10 + num] * array2[5] + array[9 + num] * array2[6] + array[8 + num] * array2[7] + array[7 + num] * array2[8] + array[6 + num] * array2[9] + array[5 + num] * array2[10] + array[4 + num] * array2[11] + array[3 + num] * array2[12] + array[2 + num] * array2[13] + array[1 + num] * array2[14] + array[num] * array2[15]) * scalefactor;
				tmpOut[i] = num2;
				num += 16;
			}
		}

		private void compute_pcm_samples(ABuffer buffer)
		{
			switch (actual_write_pos)
			{
			case 0:
				compute_pcm_samples0(buffer);
				break;
			case 1:
				compute_pcm_samples1(buffer);
				break;
			case 2:
				compute_pcm_samples2(buffer);
				break;
			case 3:
				compute_pcm_samples3(buffer);
				break;
			case 4:
				compute_pcm_samples4(buffer);
				break;
			case 5:
				compute_pcm_samples5(buffer);
				break;
			case 6:
				compute_pcm_samples6(buffer);
				break;
			case 7:
				compute_pcm_samples7(buffer);
				break;
			case 8:
				compute_pcm_samples8(buffer);
				break;
			case 9:
				compute_pcm_samples9(buffer);
				break;
			case 10:
				compute_pcm_samples10(buffer);
				break;
			case 11:
				compute_pcm_samples11(buffer);
				break;
			case 12:
				compute_pcm_samples12(buffer);
				break;
			case 13:
				compute_pcm_samples13(buffer);
				break;
			case 14:
				compute_pcm_samples14(buffer);
				break;
			case 15:
				compute_pcm_samples15(buffer);
				break;
			}
			buffer?.AppendSamples(m_ChannelIndex, _tmpOut);
		}

		public void calculate_pcm_samples(ABuffer buffer)
		{
			compute_new_v();
			compute_pcm_samples(buffer);
			actual_write_pos = (actual_write_pos + 1) & 0xF;
			actual_v = ((actual_v == v1) ? v2 : v1);
			for (int i = 0; i < 32; i++)
			{
				m_SubbandSamples[i] = 0f;
			}
		}

		private static float[] load_d()
		{
			return null;
		}

		private static float[][] splitArray(float[] array, int blockSize)
		{
			int num = array.Length / blockSize;
			float[][] array2 = new float[num][];
			for (int i = 0; i < num; i++)
			{
				array2[i] = subArray(array, i * blockSize, blockSize);
			}
			return array2;
		}

		private static float[] subArray(float[] array, int offs, int len)
		{
			if (offs + len > array.Length)
			{
				len = array.Length - offs;
			}
			if (len < 0)
			{
				len = 0;
			}
			float[] array2 = new float[len];
			for (int i = 0; i < len; i++)
			{
				array2[i] = array[offs + i];
			}
			return array2;
		}
	}
}
