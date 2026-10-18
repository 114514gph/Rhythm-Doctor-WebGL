namespace OggVorbisEncoder.Setup.Templates.Psyche
{
	internal static class Psy44
	{
		internal static readonly double[] Lowpass = new double[12]
		{
			13.9, 15.1, 15.8, 16.5, 17.2, 18.9, 20.1, 48.0, 999.0, 999.0,
			999.0, 999.0
		};

		internal static readonly Att3[] ToneMasterAtt = new Att3[12]
		{
			new Att3(new int[3] { 35, 21, 9 }, 0f, 0f),
			new Att3(new int[3] { 30, 20, 8 }, -2f, 1.25f),
			new Att3(new int[3] { 25, 12, 2 }, 0f, 0f),
			new Att3(new int[3] { 20, 9, -3 }, 0f, 0f),
			new Att3(new int[3] { 20, 9, -4 }, 0f, 0f),
			new Att3(new int[3] { 20, 9, -4 }, 0f, 0f),
			new Att3(new int[3] { 20, 6, -6 }, 0f, 0f),
			new Att3(new int[3] { 20, 3, -10 }, 0f, 0f),
			new Att3(new int[3] { 18, 1, -14 }, 0f, 0f),
			new Att3(new int[3] { 18, 0, -16 }, 0f, 0f),
			new Att3(new int[3] { 18, -2, -16 }, 0f, 0f),
			new Att3(new int[3] { 12, -2, -20 }, 0f, 0f)
		};

		internal static readonly NoiseGuard[] NoiseGuards = new NoiseGuard[4]
		{
			new NoiseGuard(3, 3, 15),
			new NoiseGuard(3, 3, 15),
			new NoiseGuard(10, 10, 100),
			new NoiseGuard(10, 10, 100)
		};

		internal static readonly CompandBlock[] Compand = new CompandBlock[6]
		{
			new CompandBlock(new int[40]
			{
				0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
				10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
				20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
				30, 31, 32, 33, 34, 35, 36, 37, 38, 39
			}),
			new CompandBlock(new int[40]
			{
				0, 1, 2, 3, 4, 5, 6, 6, 7, 7,
				7, 7, 6, 6, 6, 7, 7, 8, 9, 10,
				11, 12, 13, 14, 15, 16, 17, 17, 17, 18,
				18, 19, 19, 19, 20, 21, 22, 23, 24, 25
			}),
			new CompandBlock(new int[40]
			{
				0, 1, 2, 3, 4, 5, 5, 5, 6, 6,
				6, 5, 4, 4, 4, 4, 4, 4, 5, 5,
				5, 6, 6, 6, 7, 7, 7, 8, 8, 8,
				9, 10, 11, 12, 13, 14, 15, 16, 17, 18
			}),
			new CompandBlock(new int[40]
			{
				0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
				10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
				20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
				30, 31, 32, 33, 34, 35, 36, 37, 38, 39
			}),
			new CompandBlock(new int[40]
			{
				0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
				10, 11, 12, 12, 13, 13, 13, 14, 14, 14,
				15, 15, 15, 15, 16, 16, 17, 17, 17, 18,
				18, 19, 19, 19, 20, 21, 22, 23, 24, 25
			}),
			new CompandBlock(new int[40]
			{
				0, 1, 2, 3, 4, 5, 6, 7, 8, 8,
				7, 6, 5, 4, 4, 4, 4, 4, 5, 5,
				5, 6, 6, 6, 7, 7, 7, 8, 8, 8,
				9, 10, 11, 12, 13, 14, 15, 16, 17, 18
			})
		};

		internal static readonly double[] GlobalMapping = new double[12]
		{
			0.0, 1.0, 1.0, 1.5, 2.0, 2.0, 2.5, 2.7, 3.0, 3.7,
			4.0, 4.0
		};

		internal static readonly PsyGlobal[] Global = new PsyGlobal[5]
		{
			new PsyGlobal(8, new float[7] { 20f, 14f, 12f, 12f, 12f, 12f, 12f }, new float[7] { -60f, -30f, -40f, -40f, -40f, -40f, -40f }, 2f, -75f, -6f, new int[1] { 99 }, new int[2][]
			{
				new int[1] { 99 },
				new int[1] { 99 }
			}, new int[1], new int[1], new int[2][]
			{
				new int[1],
				new int[1]
			}),
			new PsyGlobal(8, new float[7] { 14f, 10f, 10f, 10f, 10f, 10f, 10f }, new float[7] { -40f, -30f, -25f, -25f, -25f, -25f, -25f }, 2f, -80f, -6f, new int[1] { 99 }, new int[2][]
			{
				new int[1] { 99 },
				new int[1] { 99 }
			}, new int[1], new int[1], new int[2][]
			{
				new int[1],
				new int[1]
			}),
			new PsyGlobal(8, new float[7] { 12f, 10f, 10f, 10f, 10f, 10f, 10f }, new float[7] { -20f, -20f, -15f, -15f, -15f, -15f, -15f }, 0f, -80f, -6f, new int[1] { 99 }, new int[2][]
			{
				new int[1] { 99 },
				new int[1] { 99 }
			}, new int[1], new int[1], new int[2][]
			{
				new int[1],
				new int[1]
			}),
			new PsyGlobal(8, new float[7] { 10f, 8f, 8f, 8f, 8f, 8f, 8f }, new float[7] { -20f, -15f, -12f, -12f, -12f, -12f, -12f }, 0f, -80f, -6f, new int[1] { 99 }, new int[2][]
			{
				new int[1] { 99 },
				new int[1] { 99 }
			}, new int[1], new int[1], new int[2][]
			{
				new int[1],
				new int[1]
			}),
			new PsyGlobal(8, new float[7] { 10f, 6f, 6f, 6f, 6f, 6f, 6f }, new float[7] { -15f, -15f, -12f, -12f, -12f, -12f, -12f }, 0f, -85f, -6f, new int[1] { 99 }, new int[2][]
			{
				new int[1] { 99 },
				new int[1] { 99 }
			}, new int[1], new int[1], new int[2][]
			{
				new int[1],
				new int[1]
			})
		};

		internal static readonly AdjStereo[] StereoModes = new AdjStereo[12]
		{
			new AdjStereo(new int[15]
			{
				4, 4, 4, 4, 4, 4, 4, 3, 2, 2,
				1, 0, 0, 0, 0
			}, new int[15]
			{
				8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
				8, 8, 5, 4, 3
			}, new float[15]
			{
				1f, 2f, 3f, 4f, 4f, 4f, 4f, 4f, 4f, 5f,
				6f, 7f, 8f, 8f, 8f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15]
			{
				4, 4, 4, 4, 4, 4, 4, 3, 2, 1,
				0, 0, 0, 0, 0
			}, new int[15]
			{
				8, 8, 8, 8, 6, 6, 5, 5, 5, 5,
				5, 5, 5, 4, 3
			}, new float[15]
			{
				1f, 2f, 3f, 4f, 4f, 5f, 6f, 6f, 6f, 6f,
				6f, 8f, 8f, 8f, 8f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15]
			{
				3, 3, 3, 3, 3, 3, 3, 3, 2, 1,
				0, 0, 0, 0, 0
			}, new int[15]
			{
				8, 8, 8, 8, 6, 6, 5, 5, 5, 5,
				5, 5, 5, 4, 3
			}, new float[15]
			{
				1f, 2f, 3f, 4f, 4f, 5f, 6f, 6f, 6f, 6f,
				6f, 8f, 8f, 8f, 8f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15]
			{
				3, 3, 3, 3, 3, 3, 3, 2, 1, 1,
				0, 0, 0, 0, 0
			}, new int[15]
			{
				8, 8, 6, 6, 5, 5, 4, 4, 4, 4,
				4, 4, 3, 2, 1
			}, new float[15]
			{
				3f, 4f, 4f, 5f, 5f, 6f, 6f, 6f, 6f, 6f,
				6f, 8f, 8f, 8f, 8f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15]
			{
				2, 2, 2, 2, 2, 1, 1, 1, 1, 0,
				0, 0, 0, 0, 0
			}, new int[15]
			{
				5, 4, 4, 4, 4, 4, 4, 4, 4, 4,
				4, 4, 3, 2, 1
			}, new float[15]
			{
				4f, 4f, 5f, 6f, 6f, 6f, 6f, 6f, 8f, 8f,
				10f, 10f, 10f, 10f, 10f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15]
			{
				2, 2, 2, 1, 1, 1, 0, 0, 0, 0,
				0, 0, 0, 0, 0
			}, new int[15]
			{
				4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
				3, 3, 2, 1, 0
			}, new float[15]
			{
				6f, 6f, 6f, 8f, 8f, 8f, 8f, 8f, 8f, 8f,
				10f, 10f, 10f, 10f, 10f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15]
			{
				2, 2, 2, 1, 1, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0
			}, new int[15]
			{
				3, 3, 3, 3, 3, 2, 2, 2, 2, 2,
				2, 0, 0, 0, 0
			}, new float[15]
			{
				6f, 7f, 8f, 8f, 8f, 10f, 10f, 12f, 12f, 12f,
				12f, 12f, 12f, 12f, 12f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15], new int[15]
			{
				3, 3, 3, 2, 2, 2, 0, 0, 0, 0,
				0, 0, 0, 0, 0
			}, new float[15]
			{
				8f, 8f, 8f, 10f, 10f, 12f, 12f, 12f, 12f, 12f,
				12f, 12f, 12f, 12f, 12f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15], new int[15]
			{
				3, 3, 3, 2, 2, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0
			}, new float[15]
			{
				8f, 8f, 10f, 10f, 12f, 12f, 12f, 12f, 12f, 12f,
				12f, 12f, 12f, 12f, 12f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15], new int[15]
			{
				2, 2, 2, 2, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0
			}, new float[15]
			{
				8f, 10f, 10f, 12f, 12f, 12f, 12f, 12f, 12f, 12f,
				12f, 12f, 12f, 12f, 12f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15], new int[15], new float[15]
			{
				4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f,
				4f, 4f, 4f, 4f, 4f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			}),
			new AdjStereo(new int[15], new int[15], new float[15]
			{
				4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f, 4f,
				4f, 4f, 4f, 4f, 4f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			})
		};

		internal static readonly double[] RateMap_Coupled = new double[12]
		{
			22500.0, 32000.0, 40000.0, 48000.0, 56000.0, 64000.0, 80000.0, 96000.0, 112000.0, 128000.0,
			160000.0, 250001.0
		};

		internal static readonly double[] RateMap_Uncoupled = new double[12]
		{
			32000.0, 48000.0, 60000.0, 70000.0, 80000.0, 86000.0, 96000.0, 110000.0, 120000.0, 140000.0,
			160000.0, 240001.0
		};

		internal static readonly double[] QualityMapping = new double[12]
		{
			-0.1, 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8,
			0.9, 1.0
		};

		internal static readonly int[] BlockSizeShort = new int[11]
		{
			512, 256, 256, 256, 256, 256, 256, 256, 256, 256,
			256
		};

		internal static readonly int[] BlockSizeLong = new int[11]
		{
			4096, 2048, 2048, 2048, 2048, 2048, 2048, 2048, 2048, 2048,
			2048
		};

		internal static readonly int[] NoiseStartShort = new int[11]
		{
			32, 16, 16, 16, 32, 9999, 9999, 9999, 9999, 9999,
			9999
		};

		internal static readonly int[] NoiseStartLong = new int[11]
		{
			256, 128, 128, 256, 512, 9999, 9999, 9999, 9999, 9999,
			9999
		};

		internal static readonly int[] NoisePartShort = new int[11]
		{
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8
		};

		internal static readonly int[] NoisePartLong = new int[11]
		{
			32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
			32
		};

		internal static readonly double[] NoiseThresh = new double[11]
		{
			0.2, 0.2, 0.2, 0.4, 0.6, 9999.0, 9999.0, 9999.0, 9999.0, 9999.0,
			9999.0
		};

		internal static readonly double[] NoiseThresh5Only = new double[2] { 0.5, 0.5 };

		internal static readonly int[] FloorMappingA = new int[11]
		{
			1, 0, 0, 2, 2, 4, 5, 5, 5, 5,
			5
		};

		internal static readonly int[] FloorMappingB = new int[11]
		{
			8, 7, 7, 7, 7, 7, 7, 7, 7, 7,
			7
		};

		internal static readonly int[][] FloorMapping = new int[2][] { FloorMappingA, FloorMappingB };

		internal static readonly AdjBlock[] VpToneMaskAdjLongBlock = new AdjBlock[12]
		{
			new AdjBlock(new int[17]
			{
				-3, -8, -13, -15, -10, -10, -10, -10, -10, -10,
				-10, 0, 0, 0, 0, 0, 0
			}),
			new AdjBlock(new int[17]
			{
				-4, -10, -14, -16, -15, -14, -13, -12, -12, -12,
				-11, -1, -1, -1, -1, -1, 0
			}),
			new AdjBlock(new int[17]
			{
				-6, -12, -14, -16, -15, -15, -14, -13, -13, -12,
				-12, -2, -2, -1, -1, -1, 0
			}),
			new AdjBlock(new int[17]
			{
				-12, -13, -14, -16, -16, -16, -15, -14, -13, -12,
				-12, -6, -3, -1, -1, -1, 0
			}),
			new AdjBlock(new int[17]
			{
				-15, -15, -15, -16, -16, -16, -16, -14, -13, -13,
				-13, -10, -4, -2, -1, -1, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-13, -11, -7, -3, -1, -1, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-13, -11, -7, -3, -1, -1, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -8, -4, -2, -2, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -9, -4, -2, -2, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -9, -4, -2, -2, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -9, -4, -2, -2, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -9, -4, -2, -2, 0
			})
		};

		internal static readonly AdjBlock[] VpToneMaskAdjOtherBlock = new AdjBlock[12]
		{
			new AdjBlock(new int[17]
			{
				-3, -8, -13, -15, -10, -10, -9, -9, -9, -9,
				-9, 1, 1, 1, 1, 1, 1
			}),
			new AdjBlock(new int[17]
			{
				-4, -10, -14, -16, -14, -13, -12, -12, -11, -11,
				-10, 0, 0, 0, 0, 0, 0
			}),
			new AdjBlock(new int[17]
			{
				-6, -12, -14, -16, -15, -15, -14, -13, -13, -12,
				-12, -2, -2, -1, 0, 0, 0
			}),
			new AdjBlock(new int[17]
			{
				-12, -13, -14, -16, -16, -16, -15, -14, -13, -12,
				-12, -5, -2, -1, 0, 0, 0
			}),
			new AdjBlock(new int[17]
			{
				-15, -15, -15, -16, -16, -16, -16, -14, -13, -13,
				-13, -10, -4, -2, 0, 0, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-13, -11, -7, -3, -1, -1, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-13, -11, -7, -3, -1, -1, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -8, -4, -2, -2, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -9, -4, -2, -2, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -9, -4, -2, -2, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -9, -4, -2, -2, 0
			}),
			new AdjBlock(new int[17]
			{
				-16, -16, -16, -16, -16, -16, -16, -15, -14, -14,
				-14, -12, -9, -4, -2, -2, 0
			})
		};
	}
}
