using OggVorbisEncoder.Lookups;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder
{
	public class LookupCollection
	{
		public EnvelopeLookup EnvelopeLookup { get; }

		public MdctLookup[] TransformLookup { get; }

		public PsyGlobalLookup PsyGlobalLookup { get; }

		public PsyLookup[] PsyLookup { get; }

		public DrftLookup[] FftLookup { get; }

		public FloorLookup[] FloorLookup { get; }

		public ResidueLookup[] ResidueLookup { get; }

		private LookupCollection(EnvelopeLookup envelopeLookup, MdctLookup[] transformLookup, PsyGlobalLookup psyGlobalLookup, PsyLookup[] psyLookup, DrftLookup[] fftLookup, FloorLookup[] floorLookup, ResidueLookup[] residueLookup)
		{
			EnvelopeLookup = envelopeLookup;
			TransformLookup = transformLookup;
			PsyGlobalLookup = psyGlobalLookup;
			PsyLookup = psyLookup;
			FftLookup = fftLookup;
			FloorLookup = floorLookup;
			ResidueLookup = residueLookup;
		}

		public static LookupCollection Create(VorbisInfo info)
		{
			CodecSetup codecSetup = info.CodecSetup;
			PsyGlobalLookup psyGlobalLookup = new PsyGlobalLookup(codecSetup.PsyGlobalParam);
			EnvelopeLookup envelopeLookup = new EnvelopeLookup(codecSetup.PsyGlobalParam, info);
			MdctLookup[] transformLookup = new MdctLookup[2]
			{
				new MdctLookup(codecSetup.BlockSizes[0]),
				new MdctLookup(codecSetup.BlockSizes[1])
			};
			DrftLookup[] fftLookup = new DrftLookup[2]
			{
				new DrftLookup(codecSetup.BlockSizes[0]),
				new DrftLookup(codecSetup.BlockSizes[1])
			};
			if (codecSetup.FullBooks == null)
			{
				codecSetup.FullBooks = new CodeBook[codecSetup.BookParams.Count];
				for (int i = 0; i < codecSetup.BookParams.Count; i++)
				{
					codecSetup.FullBooks[i] = CodeBook.InitEncode(codecSetup.BookParams[i]);
				}
			}
			PsyLookup[] array = new PsyLookup[codecSetup.PsyParams.Count];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = new PsyLookup(codecSetup.PsyParams[j], codecSetup.PsyGlobalParam, codecSetup.BlockSizes[codecSetup.PsyParams[j].BlockFlag] / 2, info.SampleRate);
			}
			FloorLookup[] array2 = new FloorLookup[codecSetup.FloorParams.Count];
			for (int k = 0; k < array2.Length; k++)
			{
				array2[k] = new FloorLookup(codecSetup.FloorParams[k]);
			}
			ResidueLookup[] array3 = new ResidueLookup[codecSetup.ResidueParams.Count];
			for (int l = 0; l < array3.Length; l++)
			{
				array3[l] = new ResidueLookup(codecSetup.ResidueParams[l], codecSetup.FullBooks);
			}
			return new LookupCollection(envelopeLookup, transformLookup, psyGlobalLookup, array, fftLookup, array2, array3);
		}
	}
}
