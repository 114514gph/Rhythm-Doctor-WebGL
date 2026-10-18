namespace OggVorbisEncoder.Setup.Templates.Psyche
{
	internal class Psy8
	{
		internal static readonly Att3[] ToneMasterAtt = new Att3[3]
		{
			new Att3(new int[3] { 32, 25, 12 }, 0f, 0f),
			new Att3(new int[3] { 30, 25, 12 }, 0f, 0f),
			new Att3(new int[3] { 20, 0, -14 }, 0f, 0f)
		};

		internal static readonly AdjBlock[] VpToneMaskAdjOtherBlock = new AdjBlock[3]
		{
			new AdjBlock(new int[17]
			{
				-15, -15, -15, -15, -10, -10, -6, 0, 0, 0,
				0, 10, 0, 0, 99, 99, 99
			}),
			new AdjBlock(new int[17]
			{
				-15, -15, -15, -15, -10, -10, -6, 0, 0, 0,
				0, 10, 0, 0, 99, 99, 99
			}),
			new AdjBlock(new int[17]
			{
				-15, -15, -15, -15, -10, -10, -6, 0, 0, 0,
				0, 0, 0, 0, 99, 99, 99
			})
		};

		internal static readonly Noise3[] NoiseBias = new Noise3[3]
		{
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-10, -10, -10, -10, -5, -5, -5, 0, 4, 8,
					8, 8, 10, 10, 99, 99, 99
				},
				new int[17]
				{
					-10, -10, -10, -10, -5, -5, -5, 0, 0, 4,
					4, 4, 4, 4, 99, 99, 99
				},
				new int[17]
				{
					-30, -30, -30, -30, -30, -24, -20, -14, -10, -6,
					-8, -8, -6, -6, 99, 99, 99
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-10, -10, -10, -10, -5, -5, -5, 0, 4, 8,
					8, 8, 10, 10, 99, 99, 99
				},
				new int[17]
				{
					-10, -10, -10, -10, -10, -10, -5, -5, -5, 0,
					0, 0, 0, 0, 99, 99, 99
				},
				new int[17]
				{
					-30, -30, -30, -30, -30, -24, -20, -14, -10, -6,
					-8, -8, -6, -6, 99, 99, 99
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -12, -10, -8, 0, 2,
					4, 4, 5, 5, 99, 99, 99
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -22, -20, -14, -12, -12,
					-10, -10, -10, -10, 99, 99, 99
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -26, -26, -26, -26, -26,
					-26, -26, -26, -24, 99, 99, 99
				}
			})
		};

		internal static readonly AdjStereo[] StereoModes = new AdjStereo[3]
		{
			new AdjStereo(new int[15]
			{
				4, 4, 4, 3, 3, 3, 3, 3, 3, 3,
				3, 3, 3, 3, 3
			}, new int[15]
			{
				6, 5, 4, 4, 4, 4, 4, 4, 4, 4,
				4, 4, 4, 4, 4
			}, new float[15]
			{
				1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
				1f, 1f, 1f, 1f, 1f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15]
			{
				4, 4, 4, 3, 3, 3, 3, 3, 3, 3,
				3, 3, 3, 3, 3
			}, new int[15]
			{
				6, 5, 4, 4, 4, 4, 4, 4, 4, 4,
				4, 4, 4, 4, 4
			}, new float[15]
			{
				1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
				1f, 1f, 1f, 1f, 1f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15]
			{
				3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
				3, 3, 3, 3, 3
			}, new int[15]
			{
				4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
				4, 4, 4, 4, 4
			}, new float[15]
			{
				1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f,
				1f, 1f, 1f, 1f, 1f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			})
		};

		internal static readonly NoiseGuard[] NoiseGuards = new NoiseGuard[2]
		{
			new NoiseGuard(10, 10, -1),
			new NoiseGuard(10, 10, -1)
		};

		internal static readonly CompandBlock[] Compand = new CompandBlock[2]
		{
			new CompandBlock(new int[40]
			{
				0, 1, 2, 3, 4, 5, 6, 7, 8, 8,
				9, 9, 10, 10, 11, 11, 12, 12, 13, 13,
				14, 14, 15, 15, 16, 16, 17, 17, 17, 18,
				18, 19, 19, 19, 20, 21, 22, 23, 24, 25
			}),
			new CompandBlock(new int[40]
			{
				0, 1, 2, 3, 4, 5, 6, 6, 7, 7,
				6, 6, 5, 5, 4, 4, 3, 3, 3, 4,
				5, 6, 7, 8, 9, 10, 11, 12, 13, 14,
				15, 16, 17, 18, 19, 20, 21, 22, 23, 24
			})
		};

		internal static readonly double[] Lowpass = new double[3] { 3.0, 4.0, 4.0 };

		internal static readonly int[] NoiseStart = new int[2] { 64, 64 };

		internal static readonly int[] NoisePart = new int[2] { 8, 8 };

		internal static readonly int[] AthFloater = new int[3] { -100, -100, -105 };

		internal static readonly int[] AthAbs = new int[3] { -130, -130, -140 };

		internal static readonly int[] BlockSize = new int[2] { 512, 512 };

		internal static readonly int[] FloorMappingA = new int[2] { 6, 6 };

		internal static readonly int[][] FloorMapping = new int[1][] { FloorMappingA };

		internal static readonly double[] RateMap_Coupled = new double[3] { 6000.0, 9000.0, 32000.0 };

		internal static readonly double[] RateMap_Uncoupled = new double[3] { 8000.0, 14000.0, 42000.0 };

		internal static readonly double[] QualityMapping = new double[3] { -0.1, 0.0, 1.0 };

		internal static readonly double[] CompandMapping = new double[3] { 0.0, 1.0, 1.0 };

		internal static readonly double[] GlobalMapping = new double[3] { 1.0, 2.0, 3.0 };
	}
}
