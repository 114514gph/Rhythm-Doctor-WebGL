namespace OggVorbisEncoder.Setup.Templates.Psyche
{
	internal class Psy16
	{
		internal static readonly AdjStereo[] StereoModes = new AdjStereo[4]
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
				2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f,
				2f, 3f, 3f, 4f, 4f
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
				2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 3f,
				4f, 4f, 4f, 4f, 4f
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
				5, 4, 3, 3, 3, 3, 3, 3, 3, 3,
				3, 3, 3, 3, 3
			}, new float[15]
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
				8f, 8f, 8f, 8f, 8f, 8f, 8f, 8f, 8f, 8f,
				8f, 8f, 8f, 8f, 8f
			}, new float[15]
			{
				99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f, 99f,
				99f, 99f, 99f, 99f, 99f
			})
		};

		internal static readonly Att3[] VpToneMasterAtt = new Att3[4]
		{
			new Att3(new int[3] { 30, 25, 12 }, 0f, 0f),
			new Att3(new int[3] { 25, 22, 12 }, 0f, 0f),
			new Att3(new int[3] { 20, 12, 0 }, 0f, 0f),
			new Att3(new int[3] { 15, 0, -14 }, 0f, 0f)
		};

		internal static readonly AdjBlock[] VpToneMaskAdjOtherBlock = new AdjBlock[4]
		{
			new AdjBlock(new int[17]
			{
				-20, -20, -20, -20, -20, -16, -10, 0, 0, 0,
				0, 10, 0, 0, 0, 0, 0
			}),
			new AdjBlock(new int[17]
			{
				-20, -20, -20, -20, -20, -16, -10, 0, 0, 0,
				0, 10, 0, 0, 0, 0, 0
			}),
			new AdjBlock(new int[17]
			{
				-20, -20, -20, -20, -20, -16, -10, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0
			}),
			new AdjBlock(new int[17]
			{
				-30, -30, -30, -30, -30, -26, -20, -10, -5, 0,
				0, 0, 0, 0, 0, 0, 0
			})
		};

		internal static readonly Noise3[] NoiseBiasShort = new Noise3[4]
		{
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -10, -10, -5, 4, 10,
					10, 10, 10, 12, 12, 14, 20
				},
				new int[17]
				{
					-15, -15, -15, -15, -15, -10, -10, -5, 0, 0,
					4, 5, 5, 6, 8, 8, 15
				},
				new int[17]
				{
					-30, -30, -30, -30, -30, -24, -20, -14, -10, -6,
					-8, -8, -6, -6, -6, -6, -6
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -10, -10, -5, 4, 6,
					6, 6, 6, 8, 10, 12, 20
				},
				new int[17]
				{
					-15, -15, -15, -15, -15, -15, -15, -10, -5, -5,
					-5, 4, 5, 6, 8, 8, 15
				},
				new int[17]
				{
					-30, -30, -30, -30, -30, -24, -20, -14, -10, -10,
					-10, -10, -10, -10, -10, -10, -10
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -12, -10, -8, 0, 2,
					4, 4, 5, 5, 5, 8, 12
				},
				new int[17]
				{
					-20, -20, -20, -20, -16, -12, -20, -14, -10, -10,
					-8, 0, 0, 0, 0, 2, 5
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -26, -26, -26, -26, -26,
					-26, -26, -26, -24, -20, -20, -20
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -12, -10, -8, -5, -5,
					-5, -5, -5, 0, 0, 0, 6
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -22, -20, -14, -12, -12,
					-10, -10, -10, -10, -10, -10, -6
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -26, -26, -26, -26, -26,
					-26, -26, -26, -24, -20, -20, -20
				}
			})
		};

		internal static readonly Noise3[] NoiseBiasImpulse = new Noise3[4]
		{
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -10, -10, -5, 4, 10,
					10, 10, 10, 12, 12, 14, 20
				},
				new int[17]
				{
					-15, -15, -15, -15, -15, -10, -10, -5, 0, 0,
					4, 5, 5, 6, 8, 8, 15
				},
				new int[17]
				{
					-30, -30, -30, -30, -30, -24, -20, -14, -10, -6,
					-8, -8, -6, -6, -6, -6, -6
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -10, -10, -5, 4, 4,
					4, 4, 5, 5, 6, 8, 15
				},
				new int[17]
				{
					-15, -15, -15, -15, -15, -15, -15, -10, -5, -5,
					-5, 0, 0, 0, 0, 4, 10
				},
				new int[17]
				{
					-30, -30, -30, -30, -30, -24, -20, -14, -10, -10,
					-10, -10, -10, -10, -10, -10, -10
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -12, -10, -8, 0, 0,
					0, 0, 0, 0, 0, 4, 10
				},
				new int[17]
				{
					-20, -20, -20, -20, -16, -12, -20, -14, -10, -10,
					-10, -10, -10, -10, -10, -7, -5
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -26, -26, -26, -26, -26,
					-26, -26, -26, -24, -20, -20, -20
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -12, -10, -8, -5, -5,
					-5, -5, -5, 0, 0, 0, 6
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -22, -20, -18, -18, -18,
					-20, -20, -20, -20, -20, -20, -16
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -26, -26, -26, -26, -26,
					-26, -26, -26, -24, -20, -20, -20
				}
			})
		};

		internal static readonly Noise3[] NoiseBias = new Noise3[4]
		{
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-10, -10, -10, -10, -5, -5, -5, 0, 4, 6,
					8, 8, 10, 10, 10, 14, 20
				},
				new int[17]
				{
					-10, -10, -10, -10, -10, -5, -2, -2, 0, 0,
					0, 4, 5, 6, 8, 8, 15
				},
				new int[17]
				{
					-30, -30, -30, -30, -30, -24, -20, -14, -10, -6,
					-8, -8, -6, -6, -6, -6, -6
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-10, -10, -10, -10, -5, -5, -5, 0, 4, 6,
					6, 6, 6, 8, 10, 12, 20
				},
				new int[17]
				{
					-15, -15, -15, -15, -15, -10, -5, -5, 0, 0,
					0, 4, 5, 6, 8, 8, 15
				},
				new int[17]
				{
					-30, -30, -30, -30, -30, -24, -20, -14, -10, -6,
					-8, -8, -6, -6, -6, -6, -6
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -12, -10, -8, 0, 2,
					4, 4, 5, 5, 5, 8, 12
				},
				new int[17]
				{
					-20, -20, -20, -20, -16, -12, -20, -10, -5, -5,
					0, 0, 0, 0, 0, 2, 5
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -26, -26, -26, -26, -26,
					-26, -26, -26, -24, -20, -20, -20
				}
			}),
			new Noise3(new int[3][]
			{
				new int[17]
				{
					-15, -15, -15, -15, -15, -12, -10, -8, -5, -5,
					-5, -5, -5, 0, 0, 0, 6
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -22, -20, -14, -12, -12,
					-10, -10, -10, -10, -10, -10, -6
				},
				new int[17]
				{
					-30, -30, -30, -30, -26, -26, -26, -26, -26, -26,
					-26, -26, -26, -24, -20, -20, -20
				}
			})
		};

		internal static readonly NoiseGuard[] NoiseGuards = new NoiseGuard[4]
		{
			new NoiseGuard(10, 10, -1),
			new NoiseGuard(10, 10, -1),
			new NoiseGuard(20, 20, -1),
			new NoiseGuard(20, 20, -1)
		};

		internal static readonly double[] NoiseThresh = new double[4] { 0.3, 0.5, 0.5, 0.5 };

		internal static readonly double[] Lowpass = new double[4] { 6.5, 8.0, 30.0, 99.0 };

		internal static readonly int[] NoiseStart = new int[3] { 256, 256, 9999 };

		internal static readonly int[] NoisePart = new int[4] { 8, 8, 8, 8 };

		internal static readonly int[] AthFloater = new int[4] { -100, -100, -100, -105 };

		internal static readonly int[] AthAbs = new int[4] { -130, -130, -130, -140 };

		internal static readonly int[] BlocksizeShort = new int[3] { 1024, 512, 512 };

		internal static readonly int[] BlocksizeLong = new int[3] { 1024, 1024, 1024 };

		internal static readonly int[] FloorMappingA = new int[3] { 9, 3, 3 };

		internal static readonly int[] FloorMappingB = new int[3] { 9, 9, 9 };

		internal static readonly int[][] FloorMapping = new int[2][] { FloorMappingA, FloorMappingB };

		internal static readonly double[] RateMap_Coupled = new double[4] { 12000.0, 20000.0, 44000.0, 86000.0 };

		internal static readonly double[] RateMap_Uncoupled = new double[4] { 16000.0, 28000.0, 64000.0, 100000.0 };

		internal static readonly double[] QualityMapping = new double[4] { -0.1, 0.05, 0.5, 1.0 };

		internal static readonly double[] CompandMapping = new double[4] { 0.0, 0.8, 1.0, 1.0 };

		internal static readonly double[] GlobalMapping = new double[4] { 1.0, 2.0, 3.0, 4.0 };
	}
}
