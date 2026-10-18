using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled;
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
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter0;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter1;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter2;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter3;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter4;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter5;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter6;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter7;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter8;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter9;
using OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.ChapterNeg1;

namespace OggVorbisEncoder.Setup.Templates.Residue
{
	internal static class Residue44
	{
		internal static readonly IStaticCodeBook C_HuffmanBookNegOneShort = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.ChapterNeg1Short();

		internal static readonly IStaticCodeBook C_HuffmanBookNegOneShortManaged = new ManagedChapterNeg1Short();

		internal static readonly IStaticCodeBook C_HuffmanBookNegOneLong = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1.ChapterNeg1Long();

		internal static readonly IStaticCodeBook C_HuffmanBookNegOneLongManaged = new ManagedChapterNeg1Long();

		internal static readonly IStaticBookBlock C_BlockNeg1 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.BlockNeg1();

		internal static readonly IStaticBookBlock C_BlockNeg1Managed = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.ManagedBlockNeg1();

		internal static readonly IResidueTemplate[] C_ResidueNegative1 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44Low_Coupled, C_HuffmanBookNegOneShort, C_HuffmanBookNegOneShortManaged, C_BlockNeg1, C_BlockNeg1Managed),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44Low_Coupled, C_HuffmanBookNegOneLong, C_HuffmanBookNegOneLongManaged, C_BlockNeg1, C_BlockNeg1Managed)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook0Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Chapter0Short();

		internal static readonly IStaticCodeBook C_HuffmanBook0ShortManaged = new ManagedChapter0Short();

		internal static readonly IStaticCodeBook C_HuffmanBook0Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0.Chapter0Long();

		internal static readonly IStaticCodeBook C_HuffmanBook0LongManaged = new ManagedChapter0Long();

		internal static readonly IStaticBookBlock C_Block0 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block0();

		internal static readonly IStaticBookBlock C_Block0Managed = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.ManagedBlock0();

		internal static readonly IResidueTemplate[] C_Residue0 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44Low_Coupled, C_HuffmanBook0Short, C_HuffmanBook0ShortManaged, C_Block0, C_Block0Managed),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44Low_Coupled, C_HuffmanBook0Long, C_HuffmanBook0LongManaged, C_Block0, C_Block0Managed)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook1Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Chapter1Short();

		internal static readonly IStaticCodeBook C_HuffmanBook1ShortManaged = new ManagedChapter1Short();

		internal static readonly IStaticCodeBook C_HuffmanBook1Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter1.Chapter1Long();

		internal static readonly IStaticCodeBook C_HuffmanBook1LongManaged = new ManagedChapter1Long();

		internal static readonly IStaticBookBlock C_Block1 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block1();

		internal static readonly IStaticBookBlock C_Block1Managed = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.ManagedBlock1();

		internal static readonly IResidueTemplate[] C_Residue1 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44Low_Coupled, C_HuffmanBook1Short, C_HuffmanBook1ShortManaged, C_Block1, C_Block1Managed),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44Low_Coupled, C_HuffmanBook1Long, C_HuffmanBook1LongManaged, C_Block1, C_Block1Managed)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook2Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Chapter2Short();

		internal static readonly IStaticCodeBook C_HuffmanBook2Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter2.Chapter2Long();

		internal static readonly IStaticBookBlock C_Block2 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block2();

		internal static readonly IResidueTemplate[] C_Residue2 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44Medium_Coupled, C_HuffmanBook2Short, C_HuffmanBook2Short, C_Block2, C_Block2),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44Medium_Coupled, C_HuffmanBook2Long, C_HuffmanBook2Long, C_Block2, C_Block2)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook3Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Chapter3Short();

		internal static readonly IStaticCodeBook C_HuffmanBook3Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3.Chapter3Long();

		internal static readonly IStaticBookBlock C_Block3 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block3();

		internal static readonly IResidueTemplate[] C_Residue3 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44Medium_Coupled, C_HuffmanBook3Short, C_HuffmanBook3Short, C_Block3, C_Block3),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44Medium_Coupled, C_HuffmanBook3Long, C_HuffmanBook3Long, C_Block3, C_Block3)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook4Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Chapter4Short();

		internal static readonly IStaticCodeBook C_HuffmanBook4Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter4.Chapter4Long();

		internal static readonly IStaticBookBlock C_Block4 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block4();

		internal static readonly IResidueTemplate[] C_Residue4 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44Medium_Coupled, C_HuffmanBook4Short, C_HuffmanBook4Short, C_Block4, C_Block4),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44Medium_Coupled, C_HuffmanBook4Long, C_HuffmanBook4Long, C_Block4, C_Block4)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook5Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Chapter5Short();

		internal static readonly IStaticCodeBook C_HuffmanBook5Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter5.Chapter5Long();

		internal static readonly IStaticBookBlock C_Block5 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block5();

		internal static readonly IResidueTemplate[] C_Residue5 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44Medium_Coupled, C_HuffmanBook5Short, C_HuffmanBook5Short, C_Block5, C_Block5),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44Medium_Coupled, C_HuffmanBook5Long, C_HuffmanBook5Long, C_Block5, C_Block5)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook6Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Chapter6Short();

		internal static readonly IStaticCodeBook C_HuffmanBook6Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter6.Chapter6Long();

		internal static readonly IStaticBookBlock C_Block6 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block6();

		internal static readonly IResidueTemplate[] C_Residue6 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44High_Coupled, C_HuffmanBook6Short, C_HuffmanBook6Short, C_Block6, C_Block6),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44High_Coupled, C_HuffmanBook6Long, C_HuffmanBook6Long, C_Block6, C_Block6)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook7Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Chapter7Short();

		internal static readonly IStaticCodeBook C_HuffmanBook7Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7.Chapter7Long();

		internal static readonly IStaticBookBlock C_Block7 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block7();

		internal static readonly IResidueTemplate[] C_Residue7 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44High_Coupled, C_HuffmanBook7Short, C_HuffmanBook7Short, C_Block7, C_Block7),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44High_Coupled, C_HuffmanBook7Long, C_HuffmanBook7Long, C_Block7, C_Block7)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook8Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Chapter8Short();

		internal static readonly IStaticCodeBook C_HuffmanBook8Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8.Chapter8Long();

		internal static readonly IStaticBookBlock C_Block8 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block8();

		internal static readonly IResidueTemplate[] C_Residue8 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44High_Coupled, C_HuffmanBook8Short, C_HuffmanBook8Short, C_Block8, C_Block8),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44High_Coupled, C_HuffmanBook8Long, C_HuffmanBook8Long, C_Block8, C_Block8)
		};

		internal static readonly IStaticCodeBook C_HuffmanBook9Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Chapter9Short();

		internal static readonly IStaticCodeBook C_HuffmanBook9Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter9.Chapter9Long();

		internal static readonly IStaticBookBlock C_Block9 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Blocks.Block9();

		internal static readonly IResidueTemplate[] C_Residue9 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 16, Shared.Residue44High_Coupled, C_HuffmanBook9Short, C_HuffmanBook9Short, C_Block9, C_Block9),
			new ResidueTemplate(ResidueType.Two, ResidueLimitType.LowPass, 32, Shared.Residue44High_Coupled, C_HuffmanBook9Long, C_HuffmanBook9Long, C_Block9, C_Block9)
		};

		public static readonly IMappingTemplate[] MapRes_Coupled = new IMappingTemplate[11]
		{
			new MappingTemplate(Shared.MapNominal_Coupled, C_ResidueNegative1),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue0),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue1),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue2),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue3),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue4),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue5),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue6),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue7),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue8),
			new MappingTemplate(Shared.MapNominal_Coupled, C_Residue9)
		};

		internal static readonly IStaticCodeBook U_HuffmanBookNegOneShort = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.ChapterNeg1.ChapterNeg1Short();

		internal static readonly IStaticCodeBook U_HuffmanBookNegOneLong = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.ChapterNeg1.ChapterNeg1Long();

		internal static readonly IStaticBookBlock U_BlockNeg1 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.BlockNeg1();

		internal static readonly IResidueTemplate[] U_ResidueNegative1 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44Low_Uncoupled, U_HuffmanBookNegOneShort, U_HuffmanBookNegOneShort, U_BlockNeg1, U_BlockNeg1),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44Low_Uncoupled, U_HuffmanBookNegOneLong, U_HuffmanBookNegOneLong, U_BlockNeg1, U_BlockNeg1)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook0Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter0.Chapter0Short();

		internal static readonly IStaticCodeBook U_HuffmanBook0Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter0.Chapter0Long();

		internal static readonly IStaticBookBlock U_Block0 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block0();

		internal static readonly IResidueTemplate[] U_Residue0 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44Low_Uncoupled, U_HuffmanBook0Short, U_HuffmanBook0Short, U_Block0, U_Block0),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44Low_Uncoupled, U_HuffmanBook0Long, U_HuffmanBook0Long, U_Block0, U_Block0)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook1Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter1.Chapter1Short();

		internal static readonly IStaticCodeBook U_HuffmanBook1Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter1.Chapter1Long();

		internal static readonly IStaticBookBlock U_Block1 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block1();

		internal static readonly IResidueTemplate[] U_Residue1 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44Low_Uncoupled, U_HuffmanBook1Short, U_HuffmanBook1Short, U_Block1, U_Block1),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44Low_Uncoupled, U_HuffmanBook1Long, U_HuffmanBook1Long, U_Block1, U_Block1)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook2Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter2.Chapter2Short();

		internal static readonly IStaticCodeBook U_HuffmanBook2Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter2.Chapter2Long();

		internal static readonly IStaticBookBlock U_Block2 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block2();

		internal static readonly IResidueTemplate[] U_Residue2 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44Medium_Uncoupled, U_HuffmanBook2Short, U_HuffmanBook2Short, U_Block2, U_Block2),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44Medium_Uncoupled, U_HuffmanBook2Long, U_HuffmanBook2Long, U_Block2, U_Block2)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook3Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter3.Chapter3Short();

		internal static readonly IStaticCodeBook U_HuffmanBook3Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter3.Chapter3Long();

		internal static readonly IStaticBookBlock U_Block3 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block3();

		internal static readonly IResidueTemplate[] U_Residue3 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44Medium_Uncoupled, U_HuffmanBook3Short, U_HuffmanBook3Short, U_Block3, U_Block3),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44Medium_Uncoupled, U_HuffmanBook3Long, U_HuffmanBook3Long, U_Block3, U_Block3)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook4Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter4.Chapter4Short();

		internal static readonly IStaticCodeBook U_HuffmanBook4Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter4.Chapter4Long();

		internal static readonly IStaticBookBlock U_Block4 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block4();

		internal static readonly IResidueTemplate[] U_Residue4 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44Medium_Uncoupled, U_HuffmanBook4Short, U_HuffmanBook4Short, U_Block4, U_Block4),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44Medium_Uncoupled, U_HuffmanBook4Long, U_HuffmanBook4Long, U_Block4, U_Block4)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook5Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter5.Chapter5Short();

		internal static readonly IStaticCodeBook U_HuffmanBook5Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter5.Chapter5Long();

		internal static readonly IStaticBookBlock U_Block5 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block5();

		internal static readonly IResidueTemplate[] U_Residue5 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44Medium_Uncoupled, U_HuffmanBook5Short, U_HuffmanBook5Short, U_Block5, U_Block5),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44Medium_Uncoupled, U_HuffmanBook5Long, U_HuffmanBook5Long, U_Block5, U_Block5)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook6Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter6.Chapter6Short();

		internal static readonly IStaticCodeBook U_HuffmanBook6Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter6.Chapter6Long();

		internal static readonly IStaticBookBlock U_Block6 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block6();

		internal static readonly IResidueTemplate[] U_Residue6 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44High_Uncoupled, U_HuffmanBook6Short, U_HuffmanBook6Short, U_Block6, U_Block6),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44High_Uncoupled, U_HuffmanBook6Long, U_HuffmanBook6Long, U_Block6, U_Block6)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook7Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter7.Chapter7Short();

		internal static readonly IStaticCodeBook U_HuffmanBook7Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter7.Chapter7Long();

		internal static readonly IStaticBookBlock U_Block7 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block7();

		internal static readonly IResidueTemplate[] U_Residue7 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44High_Uncoupled, U_HuffmanBook7Short, U_HuffmanBook7Short, U_Block7, U_Block7),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44High_Uncoupled, U_HuffmanBook7Long, U_HuffmanBook7Long, U_Block7, U_Block7)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook8Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter8.Chapter8Short();

		internal static readonly IStaticCodeBook U_HuffmanBook8Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter8.Chapter8Long();

		internal static readonly IStaticBookBlock U_Block8 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block8();

		internal static readonly IResidueTemplate[] U_Residue8 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44High_Uncoupled, U_HuffmanBook8Short, U_HuffmanBook8Short, U_Block8, U_Block8),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44High_Uncoupled, U_HuffmanBook8Long, U_HuffmanBook8Long, U_Block8, U_Block8)
		};

		internal static readonly IStaticCodeBook U_HuffmanBook9Short = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter9.Chapter9Short();

		internal static readonly IStaticCodeBook U_HuffmanBook9Long = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter9.Chapter9Long();

		internal static readonly IStaticBookBlock U_Block9 = new OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Blocks.Block9();

		internal static readonly IResidueTemplate[] U_Residue9 = new IResidueTemplate[2]
		{
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 16, Shared.Residue44High_Uncoupled, U_HuffmanBook9Short, U_HuffmanBook9Short, U_Block9, U_Block9),
			new ResidueTemplate(ResidueType.One, ResidueLimitType.LowPass, 32, Shared.Residue44High_Uncoupled, U_HuffmanBook9Long, U_HuffmanBook9Long, U_Block9, U_Block9)
		};

		public static readonly IMappingTemplate[] MapRes_Uncoupled = new IMappingTemplate[11]
		{
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_ResidueNegative1),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue0),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue1),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue2),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue3),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue4),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue5),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue6),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue7),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue8),
			new MappingTemplate(Shared.MapNominal_Uncoupled, U_Residue9)
		};
	}
}
