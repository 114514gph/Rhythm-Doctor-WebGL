using System.Linq;

namespace OggVorbisEncoder.Setup
{
	public class PsyGlobal
	{
		public const int EnvelopeBands = 7;

		public const int PacketBlobs = 15;

		public int EighthOctaveLines { get; }

		public float[] PreEchoThreshold { get; }

		public float[] PostEchoThreshold { get; }

		public float StretchPenalty { get; }

		public float PreEchoMinEnergy { get; }

		public float AmpMaxAttPerSec { get; set; }

		public int[] CouplingPerKilohertz { get; }

		public int[][] CouplingPointLimit { get; }

		public int[] CouplingPrePointAmp { get; set; }

		public int[] CouplingPostPointAmp { get; set; }

		public int[][] SlidingLowPass { get; }

		public PsyGlobal(int eighthOctaveLines, float[] preEchoThreshold, float[] postEchoThreshold, float stretchPenalty, float preEchoMinEnergy, float ampMaxAttPerSecond, int[] couplingPerKilohertz, int[][] couplingPointLimit, int[] couplingPrePointAmp, int[] couplingPostPointAmp, int[][] slidingLowPass)
		{
			EighthOctaveLines = eighthOctaveLines;
			PreEchoThreshold = preEchoThreshold.ToFixedLength(7);
			PostEchoThreshold = postEchoThreshold.ToFixedLength(7);
			StretchPenalty = stretchPenalty;
			PreEchoMinEnergy = preEchoMinEnergy;
			AmpMaxAttPerSec = ampMaxAttPerSecond;
			CouplingPerKilohertz = couplingPerKilohertz.ToFixedLength(15);
			CouplingPrePointAmp = couplingPrePointAmp.ToFixedLength(15);
			CouplingPostPointAmp = couplingPostPointAmp.ToFixedLength(15);
			CouplingPointLimit = couplingPointLimit.Select((int[] s) => s.ToFixedLength(15)).ToArray();
			SlidingLowPass = slidingLowPass.Select((int[] s) => s.ToFixedLength(15)).ToArray();
		}

		public PsyGlobal Clone()
		{
			return new PsyGlobal(EighthOctaveLines, PreEchoThreshold.ToArray(), PostEchoThreshold.ToArray(), StretchPenalty, PreEchoMinEnergy, AmpMaxAttPerSec, CouplingPerKilohertz.ToArray(), CouplingPointLimit.Select((int[] s) => s.ToArray()).ToArray(), CouplingPrePointAmp.ToArray(), CouplingPostPointAmp.ToArray(), SlidingLowPass.Select((int[] s) => s.ToArray()).ToArray());
		}
	}
}
