using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1;

namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled
{
	internal class Blocks
	{
		public class Block0 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page1_0()
				},
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page5_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page6_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter0.Page9_2()
				}
			};
		}

		public class Block1 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page1_0()
				},
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page5_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page6_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo8.Coupled.Chapter1.Page9_2()
				}
			};
		}
	}
}
