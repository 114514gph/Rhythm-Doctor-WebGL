using System.Linq;
using OggVorbisEncoder.Setup;
using OggVorbisEncoder.Setup.Templates;

namespace OggVorbisEncoder
{
	public class VorbisInfo
	{
		private static readonly Mode[] ModeTemplate = new Mode[2]
		{
			new Mode(0, 0, 0, 0),
			new Mode(1, 0, 0, 1)
		};

		public int Channels { get; }

		public int SampleRate { get; }

		public int BitRateNominal { get; }

		public CodecSetup CodecSetup { get; }

		private VorbisInfo(CodecSetup codecSetup, int channels, int sampleRate, int bitRateNominal)
		{
			CodecSetup = codecSetup;
			Channels = channels;
			SampleRate = sampleRate;
			BitRateNominal = bitRateNominal;
		}

		public static VorbisInfo InitVariableBitRate(int channels, int sampleRate, float baseQuality)
		{
			EncodeSetup encodeSetup = GetEncodeSetup(channels, sampleRate, baseQuality);
			CodecSetup codecSetup = new CodecSetup(encodeSetup);
			ISetupTemplate template = encodeSetup.Template;
			BlockSizeSetup(codecSetup, (int)encodeSetup.BaseSetting, template.BlockSizeShort, template.BlockSizeLong);
			bool flag = codecSetup.BlockSizes[0] == codecSetup.BlockSizes[1];
			int[][] floorMappings = template.FloorMappings;
			foreach (int[] templateFloorMappings in floorMappings)
			{
				FloorSetup(codecSetup, (int)encodeSetup.BaseSetting, template.FloorBooks, template.FloorParams, templateFloorMappings);
			}
			GlobalPsychSetup(codecSetup, encodeSetup.BaseSetting, template.GlobalParams, template.GlobalMapping);
			GlobalStereo(codecSetup, sampleRate, template.StereoModes);
			PsyParamSetup(codecSetup, (int)encodeSetup.BaseSetting, template.PsyNoiseNormalStart[0], template.PsyNoiseNormalPartition[0], template.PsyNoiseNormalThreshold, 0);
			PsyParamSetup(codecSetup, (int)encodeSetup.BaseSetting, template.PsyNoiseNormalStart[0], template.PsyNoiseNormalPartition[0], template.PsyNoiseNormalThreshold, 1);
			if (!flag)
			{
				PsyParamSetup(codecSetup, (int)encodeSetup.BaseSetting, template.PsyNoiseNormalStart[1], template.PsyNoiseNormalPartition[1], template.PsyNoiseNormalThreshold, 2);
				PsyParamSetup(codecSetup, (int)encodeSetup.BaseSetting, template.PsyNoiseNormalStart[1], template.PsyNoiseNormalPartition[1], template.PsyNoiseNormalThreshold, 3);
			}
			ToneMaskSetup(codecSetup, encodeSetup.BaseSetting, 0, template.PsyToneMasterAtt, template.PsyTone0Decibel, template.PsyToneAdjImpulse);
			ToneMaskSetup(codecSetup, encodeSetup.BaseSetting, 1, template.PsyToneMasterAtt, template.PsyTone0Decibel, template.PsyToneAdjOther);
			if (!flag)
			{
				ToneMaskSetup(codecSetup, encodeSetup.BaseSetting, 2, template.PsyToneMasterAtt, template.PsyTone0Decibel, template.PsyToneAdjOther);
				ToneMaskSetup(codecSetup, encodeSetup.BaseSetting, 3, template.PsyToneMasterAtt, template.PsyTone0Decibel, template.PsyToneAdjLong);
			}
			CompandSetup(codecSetup, encodeSetup.BaseSetting, 0, template.PsyNoiseCompand, template.PsyNoiseCompandShortMapping);
			CompandSetup(codecSetup, encodeSetup.BaseSetting, 1, template.PsyNoiseCompand, template.PsyNoiseCompandShortMapping);
			if (!flag)
			{
				CompandSetup(codecSetup, encodeSetup.BaseSetting, 2, template.PsyNoiseCompand, template.PsyNoiseCompandLongMapping);
				CompandSetup(codecSetup, encodeSetup.BaseSetting, 3, template.PsyNoiseCompand, template.PsyNoiseCompandLongMapping);
			}
			PeakSetup(codecSetup, encodeSetup.BaseSetting, 0, template.PsyToneDecibelSuppress);
			PeakSetup(codecSetup, encodeSetup.BaseSetting, 1, template.PsyToneDecibelSuppress);
			if (!flag)
			{
				PeakSetup(codecSetup, encodeSetup.BaseSetting, 2, template.PsyToneDecibelSuppress);
				PeakSetup(codecSetup, encodeSetup.BaseSetting, 3, template.PsyToneDecibelSuppress);
			}
			NoiseBiasSetup(codecSetup, encodeSetup.BaseSetting, 0, template.PsyNoiseDecibelSuppress, template.PsyNoiseBiasImpulse, template.PsyNoiseGuards);
			NoiseBiasSetup(codecSetup, encodeSetup.BaseSetting, 1, template.PsyNoiseDecibelSuppress, template.PsyNoiseBiasPadding, template.PsyNoiseGuards);
			if (!flag)
			{
				NoiseBiasSetup(codecSetup, encodeSetup.BaseSetting, 2, template.PsyNoiseDecibelSuppress, template.PsyNoiseBiasTrans, template.PsyNoiseGuards);
				NoiseBiasSetup(codecSetup, encodeSetup.BaseSetting, 3, template.PsyNoiseDecibelSuppress, template.PsyNoiseBiasLong, template.PsyNoiseGuards);
			}
			AthSetup(codecSetup, 0);
			AthSetup(codecSetup, 1);
			if (!flag)
			{
				AthSetup(codecSetup, 2);
				AthSetup(codecSetup, 3);
			}
			MapAndResSetup(codecSetup, sampleRate, channels, template.Maps);
			int approxBitRate = GetApproxBitRate(encodeSetup, channels);
			return new VorbisInfo(codecSetup, channels, sampleRate, approxBitRate);
		}

		private static void PsyParamSetup(CodecSetup codecSetup, int encodeSetupBaseSetting, int[] noiseNormalStart, int[] noiseNormalPartition, double[] noiseNormalThreshold, int block)
		{
			PsyInfo psyInfo = Psy.PsyInfoTemplate.Clone();
			codecSetup.PsyParams.Add(psyInfo);
			psyInfo.BlockFlag = block >> 1;
			psyInfo.Normalize = true;
			psyInfo.NormalStart = noiseNormalStart[encodeSetupBaseSetting];
			psyInfo.NormalPartition = noiseNormalPartition[encodeSetupBaseSetting];
			psyInfo.NormalThreshold = noiseNormalThreshold[encodeSetupBaseSetting];
		}

		private static void FloorSetup(CodecSetup codecSetup, int encodeSetupBaseSetting, IStaticCodeBook[][] templateFloorBooks, Floor[] templateFloorParams, int[] templateFloorMappings)
		{
			int num = templateFloorMappings[encodeSetupBaseSetting];
			Floor floor = templateFloorParams[num].Clone();
			int num2 = -1;
			int num3 = -1;
			int[] partitionClass = floor.PartitionClass;
			foreach (int num4 in partitionClass)
			{
				if (num4 > num2)
				{
					num2 = num4;
				}
			}
			for (int j = 0; j <= num2; j++)
			{
				if (floor.ClassBook[j] > num3)
				{
					num3 = floor.ClassBook[j];
				}
				floor.ClassBook[j] += codecSetup.BookParams.Count;
				for (int k = 0; k < 1 << floor.ClassSubs[j]; k++)
				{
					if (floor.ClassSubBook[j][k] > num3)
					{
						num3 = floor.ClassSubBook[j][k];
					}
					if (floor.ClassSubBook[j][k] >= 0)
					{
						floor.ClassSubBook[j][k] += codecSetup.BookParams.Count;
					}
				}
			}
			for (int l = 0; l <= num3; l++)
			{
				IStaticCodeBook item = templateFloorBooks[num][l];
				codecSetup.BookParams.Add(item);
			}
			codecSetup.FloorParams.Add(floor);
		}

		private static void MapAndResSetup(CodecSetup codecSetup, int sampleRate, int channels, IMappingTemplate[] templateMaps)
		{
			int num = (int)codecSetup.EncodeSetup.BaseSetting;
			Mapping[] mapping = templateMaps[num].Mapping;
			IResidueTemplate[] residueTemplate = templateMaps[num].ResidueTemplate;
			int num2 = 2;
			if (codecSetup.BlockSizes[0] == codecSetup.BlockSizes[1])
			{
				num2 = 1;
			}
			for (int i = 0; i < num2; i++)
			{
				codecSetup.ModeParams.Add(ModeTemplate[i]);
				codecSetup.MapParams.Add(mapping[i].Clone());
				for (int j = 0; j < mapping[i].SubMaps; j++)
				{
					ResidueSetup(codecSetup, sampleRate, channels, mapping[i].ResidueSubMap[j], i, residueTemplate[mapping[i].ResidueSubMap[j]]);
				}
			}
		}

		private static void ResidueSetup(CodecSetup codecSetup, int sampleRate, int channels, int number, int block, IResidueTemplate residueTemplate)
		{
			ResidueEntry residueEntry = residueTemplate.Residue.Clone(residueTemplate.ResidueType, residueTemplate.Grouping);
			codecSetup.ResidueParams.Add(residueEntry);
			FillBooks(codecSetup, residueEntry, residueTemplate.BookAux, residueTemplate.BooksBase);
			double num = codecSetup.EncodeSetup.LowPassKilohertz * 1000.0;
			Floor floor = codecSetup.FloorParams[block];
			double num2 = (double)sampleRate / 2.0;
			int num3 = codecSetup.BlockSizes[block] >> 1;
			if (num > num2)
			{
				num = num2;
			}
			floor.N = (int)(num / num2 * (double)num3);
			switch (residueTemplate.LimitType)
			{
			case ResidueLimitType.PointStereo:
				num = codecSetup.PsyGlobalParam.CouplingPerKilohertz[7] * 1000;
				if (num > num2)
				{
					num = num2;
				}
				break;
			case ResidueLimitType.LowFrequencyEffects:
				num = 250.0;
				break;
			}
			if (residueEntry.ResidueType == ResidueType.Two)
			{
				int num4 = 0;
				for (int i = 0; i < codecSetup.MapParams.Count; i++)
				{
					if (num4 != 0)
					{
						break;
					}
					Mapping mapping = codecSetup.MapParams[i];
					for (int j = 0; j < mapping.SubMaps; j++)
					{
						if (num4 != 0)
						{
							break;
						}
						if (mapping.ResidueSubMap[j] != number)
						{
							continue;
						}
						for (int k = 0; k < channels; k++)
						{
							if (mapping.ChannelMuxList[k] == j)
							{
								num4++;
							}
						}
					}
				}
				residueEntry.End = (int)(num / num2 * (double)num3 * (double)num4 / (double)residueEntry.Grouping + 0.9) * residueEntry.Grouping;
				if (residueEntry.End > num3 * num4)
				{
					residueEntry.End = num3 * num4 / residueEntry.Grouping * residueEntry.Grouping;
				}
			}
			else
			{
				residueEntry.End = (int)(num / num2 * (double)num3 / (double)residueEntry.Grouping + 0.9) * residueEntry.Grouping;
				if (residueEntry.End > num3)
				{
					residueEntry.End = num3 / residueEntry.Grouping * residueEntry.Grouping;
				}
			}
			if (residueEntry.End == 0)
			{
				residueEntry.End = residueEntry.Grouping;
			}
		}

		private static void FillBooks(CodecSetup codecSetup, ResidueEntry r, IStaticCodeBook bookAux, IStaticBookBlock bookBlock)
		{
			for (int i = 0; i < r.Partitions; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (i < bookBlock.Books.Length && j < bookBlock.Books[i].Length && bookBlock.Books[i][j] != null)
					{
						r.SecondStages[i] |= 1 << j;
					}
				}
			}
			r.GroupBook = GetOrAddBook(codecSetup, bookAux);
			int num = 0;
			for (int k = 0; k < r.Partitions; k++)
			{
				for (int l = 0; l < 4; l++)
				{
					if (k < bookBlock.Books.Length && l < bookBlock.Books[k].Length)
					{
						IStaticCodeBook staticCodeBook = bookBlock.Books[k][l];
						if (staticCodeBook != null)
						{
							int orAddBook = GetOrAddBook(codecSetup, staticCodeBook);
							r.BookList[num++] = orAddBook;
						}
					}
				}
			}
		}

		private static int GetOrAddBook(CodecSetup codecSetup, IStaticCodeBook codeBook)
		{
			for (int i = 0; i < codecSetup.BookParams.Count; i++)
			{
				if (codecSetup.BookParams[i] == codeBook)
				{
					return i;
				}
			}
			codecSetup.BookParams.Add(codeBook);
			return codecSetup.BookParams.Count - 1;
		}

		private static void GlobalPsychSetup(CodecSetup codecSetup, double encodeSetupTriggerSetting, PsyGlobal[] templateGlobalParams, double[] templateGlobalMapping)
		{
			int num = (int)encodeSetupTriggerSetting;
			double num2 = encodeSetupTriggerSetting - (double)num;
			int num3 = (int)templateGlobalMapping[num];
			PsyGlobal psyGlobal = (codecSetup.PsyGlobalParam = templateGlobalParams[num3].Clone());
			PsyGlobal psyGlobal3 = psyGlobal;
			num2 = templateGlobalMapping[num] * (1.0 - num2) + templateGlobalMapping[num + 1] * num2;
			num = (int)num2;
			num2 -= (double)num;
			if (num2 <= 0.0 && num > 0)
			{
				num--;
				num2 = 1.0;
			}
			for (int i = 0; i < 4; i++)
			{
				psyGlobal3.PreEchoThreshold[i] = (float)((double)templateGlobalParams[num].PreEchoThreshold[i] * (1.0 - num2) + (double)templateGlobalParams[num + 1].PreEchoThreshold[i] * num2);
				psyGlobal3.PostEchoThreshold[i] = (float)((double)templateGlobalParams[num].PostEchoThreshold[i] * (1.0 - num2) + (double)templateGlobalParams[num + 1].PostEchoThreshold[i] * num2);
			}
			psyGlobal3.AmpMaxAttPerSec = (float)codecSetup.EncodeSetup.AmplitudeTrackDbPerSec;
		}

		private static void GlobalStereo(CodecSetup codecSetup, int sampleRate, AdjStereo[] templateStereoModes)
		{
			int num = (int)codecSetup.EncodeSetup.BaseSetting;
			double num2 = codecSetup.EncodeSetup.BaseSetting - (double)num;
			PsyGlobal psyGlobalParam = codecSetup.PsyGlobalParam;
			int num3 = 15;
			if (templateStereoModes != null)
			{
				psyGlobalParam.CouplingPrePointAmp = templateStereoModes[num].Pre.ToArray();
				psyGlobalParam.CouplingPostPointAmp = templateStereoModes[num].Post.ToArray();
				double num4 = (double)templateStereoModes[num].Kilohertz[num3 / 2] * (1.0 - num2) + (double)templateStereoModes[num + 1].Kilohertz[num3 / 2] * num2;
				for (int i = 0; i < num3; i++)
				{
					psyGlobalParam.CouplingPointLimit[0][i] = (int)(num4 * 1000.0 / (double)sampleRate * (double)codecSetup.BlockSizes[0]);
					psyGlobalParam.CouplingPointLimit[1][i] = (int)(num4 * 1000.0 / (double)sampleRate * (double)codecSetup.BlockSizes[1]);
					psyGlobalParam.CouplingPerKilohertz[i] = (int)num4;
				}
				num4 = (double)templateStereoModes[num].LowPassKilohertz[num3 / 2] * (1.0 - num2) + (double)templateStereoModes[num + 1].LowPassKilohertz[num3 / 2] * num2;
				for (int j = 0; j < num3; j++)
				{
					psyGlobalParam.SlidingLowPass[0][j] = (int)(num4 * 1000.0 / (double)sampleRate * (double)codecSetup.BlockSizes[0]);
					psyGlobalParam.SlidingLowPass[1][j] = (int)(num4 * 1000.0 / (double)sampleRate * (double)codecSetup.BlockSizes[1]);
				}
			}
			else
			{
				for (int k = 0; k < num3; k++)
				{
					psyGlobalParam.SlidingLowPass[0][k] = codecSetup.BlockSizes[0];
					psyGlobalParam.SlidingLowPass[1][k] = codecSetup.BlockSizes[1];
				}
			}
		}

		private static void BlockSizeSetup(CodecSetup codecSetup, int index, int[] templateBlockSizeShort, int[] templateBlockSizeLong)
		{
			int num = templateBlockSizeShort[index];
			int num2 = templateBlockSizeLong[index];
			codecSetup.BlockSizes[0] = num;
			codecSetup.BlockSizes[1] = num2;
		}

		private static void ToneMaskSetup(CodecSetup codecSetup, double toneMaskSetting, int block, Att3[] templatePsyToneMasterAtt, int[] templatePsyTone0Decibel, AdjBlock[] templatePsyToneAdjLong)
		{
			int num = (int)toneMaskSetting;
			double num2 = toneMaskSetting - (double)num;
			PsyInfo psyInfo = codecSetup.PsyParams[block];
			psyInfo.ToneMasterAtt[0] = (float)((double)templatePsyToneMasterAtt[num].Att[0] * (1.0 - num2) + (double)templatePsyToneMasterAtt[num + 1].Att[0] * num2);
			psyInfo.ToneMasterAtt[1] = (float)((double)templatePsyToneMasterAtt[num].Att[1] * (1.0 - num2) + (double)templatePsyToneMasterAtt[num + 1].Att[1] * num2);
			psyInfo.ToneMasterAtt[2] = (float)((double)templatePsyToneMasterAtt[num].Att[2] * (1.0 - num2) + (double)templatePsyToneMasterAtt[num + 1].Att[2] * num2);
			psyInfo.ToneCenterBoost = (float)((double)templatePsyToneMasterAtt[num].Boost * (1.0 - num2) + (double)templatePsyToneMasterAtt[num + 1].Boost * num2);
			psyInfo.ToneDecay = (float)((double)templatePsyToneMasterAtt[num].Decay * (1.0 - num2) + (double)templatePsyToneMasterAtt[num + 1].Decay * num2);
			psyInfo.MaxCurveDecibel = (float)((double)templatePsyTone0Decibel[num] * (1.0 - num2) + (double)templatePsyTone0Decibel[num + 1] * num2);
			for (int i = 0; i < 17; i++)
			{
				psyInfo.ToneAtt[i] = (float)((double)templatePsyToneAdjLong[num].Block[i] * (1.0 - num2) + (double)templatePsyToneAdjLong[num + 1].Block[i] * num2);
			}
		}

		private static void CompandSetup(CodecSetup codecSetup, double noiseCompandSetting, int block, CompandBlock[] templatePsyNoiseCompand, double[] templatePsyNoiseCompandShortMapping)
		{
			int num = (int)noiseCompandSetting;
			double num2 = noiseCompandSetting - (double)num;
			PsyInfo psyInfo = codecSetup.PsyParams[block];
			num2 = templatePsyNoiseCompandShortMapping[num] * (1.0 - num2) + templatePsyNoiseCompandShortMapping[num + 1] * num2;
			num = (int)num2;
			num2 -= (double)num;
			if (num2 <= 0.0 && num > 0)
			{
				num--;
				num2 = 1.0;
			}
			for (int i = 0; i < psyInfo.NoiseCompand.Length; i++)
			{
				psyInfo.NoiseCompand[i] = (float)((double)templatePsyNoiseCompand[num].Data[i] * (1.0 - num2) + (double)templatePsyNoiseCompand[num + 1].Data[i] * num2);
			}
		}

		private static void PeakSetup(CodecSetup codecSetup, double tonePeakLimitSetting, int block, int[] templatePsyToneDecibelSuppress)
		{
			int num = (int)tonePeakLimitSetting;
			double num2 = tonePeakLimitSetting - (double)num;
			codecSetup.PsyParams[block].ToneAbsLimit = (float)((double)templatePsyToneDecibelSuppress[num] * (1.0 - num2) + (double)templatePsyToneDecibelSuppress[num + 1] * num2);
		}

		private static void NoiseBiasSetup(CodecSetup codecSetup, double noiseBiasSetting, int block, int[] templatePsyNoiseDecibelSuppress, Noise3[] templatePsyNoiseBiasLong, NoiseGuard[] templatePsyNoiseGuards)
		{
			int num = (int)noiseBiasSetting;
			double num2 = noiseBiasSetting - (double)num;
			PsyInfo psyInfo = codecSetup.PsyParams[block];
			psyInfo.NoiseMaxSuppress = (float)((double)templatePsyNoiseDecibelSuppress[num] * (1.0 - num2) + (double)templatePsyNoiseDecibelSuppress[num + 1] * num2);
			psyInfo.NoiseWindowLowMin = templatePsyNoiseGuards[block].Low;
			psyInfo.NoiseWindowHighMin = templatePsyNoiseGuards[block].High;
			psyInfo.NoiseWindowFixed = templatePsyNoiseGuards[block].Fixed;
			for (int i = 0; i < psyInfo.NoiseOffset.Length; i++)
			{
				for (int j = 0; j < psyInfo.NoiseOffset[i].Length; j++)
				{
					psyInfo.NoiseOffset[i][j] = (float)((double)templatePsyNoiseBiasLong[num].Data[i][j] * (1.0 - num2) + (double)templatePsyNoiseBiasLong[num + 1].Data[i][j] * num2);
				}
			}
			float[][] noiseOffset = psyInfo.NoiseOffset;
			foreach (float[] array in noiseOffset)
			{
				float num3 = array[0] + 6f;
				for (int l = 0; l < array.Length; l++)
				{
					if (array[l] < num3)
					{
						array[l] = num3;
					}
				}
			}
		}

		private static void AthSetup(CodecSetup codecSetup, int block)
		{
			PsyInfo psyInfo = codecSetup.PsyParams[block];
			psyInfo.AthAdjAtt = (float)codecSetup.EncodeSetup.AthFloatingDecibel;
			psyInfo.AthMaxAtt = (float)codecSetup.EncodeSetup.AthAbsoluteDecibel;
		}

		private static EncodeSetup GetEncodeSetup(int channels, int sampleRate, float quality)
		{
			quality += 1E-07f;
			if (quality >= 1f)
			{
				quality = 0.9999f;
			}
			return EncodeSetup.GetBestMatch(channels, sampleRate, quality);
		}

		private static int GetApproxBitRate(EncodeSetup encodeSetup, int channels)
		{
			ISetupTemplate template = encodeSetup.Template;
			int num = (int)encodeSetup.BaseSetting;
			double num2 = encodeSetup.BaseSetting - (double)num;
			if (template.SampleRateMapping == null)
			{
				return -1;
			}
			return (int)((template.SampleRateMapping[num] * (1.0 - num2) + template.SampleRateMapping[num + 1] * num2) * (double)channels);
		}
	}
}
