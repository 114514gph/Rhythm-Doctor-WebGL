using System;
using System.Linq;

namespace OggVorbisEncoder.Setup
{
	public class Mapping
	{
		public int SubMaps { get; }

		public int[] ChannelMuxList { get; }

		public int[] FloorSubMap { get; }

		public int[] ResidueSubMap { get; }

		public int CouplingSteps { get; }

		public int[] CouplingMag { get; }

		public int[] CouplingAng { get; }

		public Mapping(int submaps, int[] channelMuxList, int[] floorSubMap, int[] residueSubMap, int couplingSteps, int[] couplingMag, int[] couplingAng)
		{
			if (floorSubMap?.Length != residueSubMap?.Length)
			{
				throw new ArgumentException("floorSubMap and residueSubMap must be the same size");
			}
			if (couplingMag?.Length != couplingAng?.Length)
			{
				throw new ArgumentException("couplingMag and couplingAng must be the same size");
			}
			SubMaps = submaps;
			ChannelMuxList = channelMuxList;
			FloorSubMap = floorSubMap;
			ResidueSubMap = residueSubMap;
			CouplingSteps = couplingSteps;
			CouplingMag = couplingMag;
			CouplingAng = couplingAng;
		}

		public Mapping Clone()
		{
			return new Mapping(SubMaps, ChannelMuxList.ToArray(), FloorSubMap.ToArray(), ResidueSubMap.ToArray(), CouplingSteps, CouplingMag.ToArray(), CouplingAng.ToArray());
		}
	}
}
