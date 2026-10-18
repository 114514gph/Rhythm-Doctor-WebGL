using System;
using System.Collections.Generic;
using OggVorbisEncoder.Setup;
using OggVorbisEncoder.Setup.Templates;

namespace OggVorbisEncoder
{
	public class EncodeSetup
	{
		private static readonly IEnumerable<ISetupTemplate> SetupTemplates = CreateTemplates();

		public ISetupTemplate Template { get; }

		public double BaseSetting { get; }

		public double LowPassKilohertz { get; }

		public double AthFloatingDecibel { get; }

		public double AthAbsoluteDecibel { get; }

		public double AmplitudeTrackDbPerSec { get; }

		private static IEnumerable<ISetupTemplate> CreateTemplates()
		{
			return new ISetupTemplate[16]
			{
				new Stereo44SetupDataTemplate(),
				new Uncoupled44SetupDataTemplate(),
				new Stereo32SetupDataTemplate(),
				new Uncoupled32SetupDataTemplate(),
				new Stereo22SetupDataTemplate(),
				new Uncoupled22SetupDataTemplate(),
				new Stereo16SetupDataTemplate(),
				new Uncoupled16SetupDataTemplate(),
				new Stereo11SetupDataTemplate(),
				new Uncoupled11SetupDataTemplate(),
				new Stereo8SetupDataTemplate(),
				new Uncoupled8SetupDataTemplate(),
				new StereoXSetupDataTemplate(),
				new UncoupledXSetupDataTemplate(),
				new StereoXXSetupDataTemplate(),
				new UncoupledXXSetupDataTemplate()
			};
		}

		public EncodeSetup(ISetupTemplate template, double baseSetting)
		{
			Template = template;
			BaseSetting = baseSetting;
			int num = (int)BaseSetting;
			double num2 = BaseSetting - (double)num;
			LowPassKilohertz = template.PsyLowPass[num] * (1.0 - num2) + template.PsyLowPass[num + 1] * num2;
			AthFloatingDecibel = (double)template.PsyAthFloat[num] * (1.0 - num2) + (double)template.PsyAthFloat[num + 1] * num2;
			AthAbsoluteDecibel = (double)template.PsyAthAbs[num] * (1.0 - num2) + (double)template.PsyAthAbs[num + 1] * num2;
			AmplitudeTrackDbPerSec = -6.0;
			if (AthFloatingDecibel > -80.0)
			{
				AthFloatingDecibel = -80.0;
			}
			if (AthFloatingDecibel < -200.0)
			{
				AthFloatingDecibel = -200.0;
			}
			if (AmplitudeTrackDbPerSec > 0.0)
			{
				AmplitudeTrackDbPerSec = 0.0;
			}
			if (AmplitudeTrackDbPerSec < -99999.0)
			{
				AmplitudeTrackDbPerSec = -99999.0;
			}
		}

		public static EncodeSetup GetBestMatch(int channels, int sampleRate, float quality)
		{
			foreach (ISetupTemplate setupTemplate in SetupTemplates)
			{
				if ((setupTemplate.CouplingRestriction != -1 && setupTemplate.CouplingRestriction != channels) || sampleRate < setupTemplate.SampleRateMinRestriction || sampleRate > setupTemplate.SampleRateMaxRestriction)
				{
					continue;
				}
				double[] qualityMapping = setupTemplate.QualityMapping;
				if (!((double)quality < qualityMapping[0]) && !((double)quality > qualityMapping[setupTemplate.Mappings]))
				{
					int i;
					for (i = 0; i < setupTemplate.Mappings && (!((double)quality >= qualityMapping[i]) || !((double)quality < qualityMapping[i + 1])); i++)
					{
					}
					double baseSetting;
					if (i == setupTemplate.Mappings)
					{
						baseSetting = (double)i - 0.001;
					}
					else
					{
						double num = qualityMapping[i];
						double num2 = qualityMapping[i + 1];
						double num3 = ((double)quality - num) / (num2 - num);
						baseSetting = (double)i + num3;
					}
					return new EncodeSetup(setupTemplate, baseSetting);
				}
			}
			throw new InvalidOperationException("No matching template found");
		}
	}
}
