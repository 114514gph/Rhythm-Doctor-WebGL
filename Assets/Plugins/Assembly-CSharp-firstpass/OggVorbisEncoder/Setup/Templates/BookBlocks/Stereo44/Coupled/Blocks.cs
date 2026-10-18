using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1;

namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled
{
	public class Blocks
	{
		public class BlockNeg1 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[9][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page5_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page7_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page8_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page8_2()
				}
			};
		}

		public class Block0 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[9][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page5_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page7_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page8_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page8_2()
				}
			};
		}

		public class Block1 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[9][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page5_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page7_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page8_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page8_2()
				}
			};
		}

		public class Block2 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page5_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page6_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Page9_2()
				}
			};
		}

		public class Block3 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page5_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page6_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Page9_2()
				}
			};
		}

		public class Block4 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page5_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page6_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Page9_2()
				}
			};
		}

		public class Block5 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page5_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page6_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Page9_2()
				}
			};
		}

		public class Block6 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page4_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page5_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page5_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Page9_2()
				}
			};
		}

		public class Block7 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page4_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page5_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page5_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Page9_2()
				}
			};
		}

		public class Block8 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page4_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page5_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page5_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Page9_2()
				}
			};
		}

		public class Block9 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[10][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page4_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page5_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page5_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page7_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page8_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page9_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page9_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Page9_2()
				}
			};
		}

		public class ManagedBlockNeg1 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[9][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page5_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page7_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1.Page8_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.Page8_2()
				}
			};
		}

		public class ManagedBlock0 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[9][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page5_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page7_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0.Page8_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Page8_2()
				}
			};
		}

		public class ManagedBlock1 : IStaticBookBlock
		{
			public IStaticCodeBook[][] Books { get; } = new IStaticCodeBook[9][]
			{
				new IStaticCodeBook[1],
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page1_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page2_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page3_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page4_0()
				},
				new IStaticCodeBook[3]
				{
					null,
					null,
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page5_0()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page6_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page6_1()
				},
				new IStaticCodeBook[2]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page7_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page7_1()
				},
				new IStaticCodeBook[3]
				{
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page8_0(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1.Page8_1(),
					new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Page8_2()
				}
			};
		}
	}
}
