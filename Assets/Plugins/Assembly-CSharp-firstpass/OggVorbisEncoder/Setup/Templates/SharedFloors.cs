using OggVorbisEncoder.Setup.Templates.FloorBooks;

namespace OggVorbisEncoder.Setup.Templates
{
	public static class SharedFloors
	{
		private static readonly IStaticCodeBook[] Floor128X4Books = new IStaticCodeBook[5]
		{
			new Line128X4Class0(),
			new Line128X4Sub0(),
			new Line128X4Sub1(),
			new Line128X4Sub2(),
			new Line128X4Sub3()
		};

		private static readonly IStaticCodeBook[] Floor256X4Books = new IStaticCodeBook[5]
		{
			new Line256X4Class0(),
			new Line256X4Sub0(),
			new Line256X4Sub1(),
			new Line256X4Sub2(),
			new Line256X4Sub3()
		};

		private static readonly IStaticCodeBook[] Floor128X7Books = new IStaticCodeBook[8]
		{
			new Line128X7Class0(),
			new Line128X7Class1(),
			new Line128X7_0Sub1(),
			new Line128X7_0Sub2(),
			new Line128X7_0Sub3(),
			new Line128X7_1Sub1(),
			new Line128X7_1Sub2(),
			new Line128X7_1Sub3()
		};

		private static readonly IStaticCodeBook[] Floor256X7Books = new IStaticCodeBook[8]
		{
			new Line256X7Class0(),
			new Line256X7Class1(),
			new Line256X7_0Sub1(),
			new Line256X7_0Sub2(),
			new Line256X7_0Sub3(),
			new Line256X7_1Sub1(),
			new Line256X7_1Sub2(),
			new Line256X7_1Sub3()
		};

		private static readonly IStaticCodeBook[] Floor128X11Books = new IStaticCodeBook[12]
		{
			new Line128X11Class1(),
			new Line128X11Class2(),
			new Line128X11Class3(),
			new Line128X11_0Sub0(),
			new Line128X11_1Sub0(),
			new Line128X11_1Sub1(),
			new Line128X11_2Sub1(),
			new Line128X11_2Sub2(),
			new Line128X11_2Sub3(),
			new Line128X11_3Sub1(),
			new Line128X11_3Sub2(),
			new Line128X11_3Sub3()
		};

		private static readonly IStaticCodeBook[] Floor128X17Books = new IStaticCodeBook[12]
		{
			new Line128X17Class1(),
			new Line128X17Class2(),
			new Line128X17Class3(),
			new Line128X17_0Sub0(),
			new Line128X17_1Sub0(),
			new Line128X17_1Sub1(),
			new Line128X17_2Sub1(),
			new Line128X17_2Sub2(),
			new Line128X17_2Sub3(),
			new Line128X17_3Sub1(),
			new Line128X17_3Sub2(),
			new Line128X17_3Sub3()
		};

		private static readonly IStaticCodeBook[] Floor256X4LowBooks = new IStaticCodeBook[5]
		{
			new Line256X4LowClass0(),
			new Line256X4LowSub0(),
			new Line256X4LowSub1(),
			new Line256X4LowSub2(),
			new Line256X4LowSub3()
		};

		private static readonly IStaticCodeBook[] Floor1024X27Books = new IStaticCodeBook[15]
		{
			new Line1024X27Class1(),
			new Line1024X27Class2(),
			new Line1024X27Class3(),
			new Line1024X27Class4(),
			new Line1024X27_0Sub0(),
			new Line1024X27_1Sub0(),
			new Line1024X27_1Sub1(),
			new Line1024X27_2Sub0(),
			new Line1024X27_2Sub1(),
			new Line1024X27_3Sub1(),
			new Line1024X27_3Sub2(),
			new Line1024X27_3Sub3(),
			new Line1024X27_4Sub1(),
			new Line1024X27_4Sub2(),
			new Line1024X27_4Sub3()
		};

		private static readonly IStaticCodeBook[] Floor2048X27Books = new IStaticCodeBook[15]
		{
			new Line2048X27Class1(),
			new Line2048X27Class2(),
			new Line2048X27Class3(),
			new Line2048X27Class4(),
			new Line2048X27_0Sub0(),
			new Line2048X27_1Sub0(),
			new Line2048X27_1Sub1(),
			new Line2048X27_2Sub0(),
			new Line2048X27_2Sub1(),
			new Line2048X27_3Sub1(),
			new Line2048X27_3Sub2(),
			new Line2048X27_3Sub3(),
			new Line2048X27_4Sub1(),
			new Line2048X27_4Sub2(),
			new Line2048X27_4Sub3()
		};

		private static readonly IStaticCodeBook[] Floor512X17Books = new IStaticCodeBook[12]
		{
			new Line512X17Class1(),
			new Line512X17Class2(),
			new Line512X17Class3(),
			new Line512X17_0Sub0(),
			new Line512X17_1Sub0(),
			new Line512X17_1Sub1(),
			new Line512X17_2Sub1(),
			new Line512X17_2Sub2(),
			new Line512X17_2Sub3(),
			new Line512X17_3Sub1(),
			new Line512X17_3Sub2(),
			new Line512X17_3Sub3()
		};

		public static readonly IStaticCodeBook[][] FloorBooks = new IStaticCodeBook[10][] { Floor128X4Books, Floor256X4Books, Floor128X7Books, Floor256X7Books, Floor128X11Books, Floor128X17Books, Floor256X4LowBooks, Floor1024X27Books, Floor2048X27Books, Floor512X17Books };

		public static readonly Floor[] Floor = new Floor[11]
		{
			new Floor(new int[1], new int[1] { 4 }, new int[1] { 2 }, new int[1], new int[1][] { new int[4] { 1, 2, 3, 4 } }, 4, new int[6] { 0, 128, 33, 8, 16, 70 }, 60f, 30f, 500f, 1f, 18f, 128),
			new Floor(new int[1], new int[1] { 4 }, new int[1] { 2 }, new int[1], new int[1][] { new int[4] { 1, 2, 3, 4 } }, 4, new int[6] { 0, 256, 66, 16, 32, 140 }, 60f, 30f, 500f, 1f, 18f, 256),
			new Floor(new int[2] { 0, 1 }, new int[2] { 3, 4 }, new int[2] { 2, 2 }, new int[2] { 0, 1 }, new int[2][]
			{
				new int[4] { -1, 2, 3, 4 },
				new int[4] { -1, 5, 6, 7 }
			}, 4, new int[9] { 0, 128, 14, 4, 58, 2, 8, 28, 90 }, 60f, 30f, 500f, 1f, 18f, 128),
			new Floor(new int[2] { 0, 1 }, new int[2] { 3, 4 }, new int[2] { 2, 2 }, new int[2] { 0, 1 }, new int[2][]
			{
				new int[4] { -1, 2, 3, 4 },
				new int[4] { -1, 5, 6, 7 }
			}, 4, new int[9] { 0, 256, 28, 8, 116, 4, 16, 56, 180 }, 60f, 30f, 500f, 1f, 18f, 256),
			new Floor(new int[4] { 0, 1, 2, 3 }, new int[4] { 2, 3, 3, 3 }, new int[4] { 0, 1, 2, 2 }, new int[4] { -1, 0, 1, 2 }, new int[4][]
			{
				new int[1] { 3 },
				new int[2] { 4, 5 },
				new int[4] { -1, 6, 7, 8 },
				new int[4] { -1, 9, 10, 11 }
			}, 2, new int[13]
			{
				0, 128, 8, 33, 4, 16, 70, 2, 6, 12,
				23, 46, 90
			}, 60f, 30f, 500f, 1f, 18f, 128),
			new Floor(new int[6] { 0, 1, 1, 2, 3, 3 }, new int[4] { 2, 3, 3, 3 }, new int[4] { 0, 1, 2, 2 }, new int[4] { -1, 0, 1, 2 }, new int[4][]
			{
				new int[1] { 3 },
				new int[2] { 4, 5 },
				new int[4] { -1, 6, 7, 8 },
				new int[4] { -1, 9, 10, 11 }
			}, 2, new int[19]
			{
				0, 128, 12, 46, 4, 8, 16, 23, 33, 70,
				2, 6, 10, 14, 19, 28, 39, 58, 90
			}, 60f, 30f, 500f, 1f, 18f, 128),
			new Floor(new int[1], new int[1] { 4 }, new int[1] { 2 }, new int[1], new int[1][] { new int[4] { 1, 2, 3, 4 } }, 4, new int[6] { 0, 256, 66, 16, 32, 140 }, 60f, 30f, 500f, 1f, 18f, 256),
			new Floor(new int[8] { 0, 1, 2, 2, 3, 3, 4, 4 }, new int[5] { 3, 4, 3, 4, 3 }, new int[5] { 0, 1, 1, 2, 2 }, new int[5] { -1, 0, 1, 2, 3 }, new int[5][]
			{
				new int[1] { 4 },
				new int[2] { 5, 6 },
				new int[2] { 7, 8 },
				new int[4] { -1, 9, 10, 11 },
				new int[4] { -1, 12, 13, 14 }
			}, 2, new int[29]
			{
				0, 1024, 93, 23, 372, 6, 46, 186, 750, 14,
				33, 65, 130, 260, 556, 3, 10, 18, 28, 39,
				55, 79, 111, 158, 220, 312, 464, 650, 850
			}, 60f, 30f, 500f, 3f, 18f, 1024),
			new Floor(new int[8] { 0, 1, 2, 2, 3, 3, 4, 4 }, new int[5] { 3, 4, 3, 4, 3 }, new int[5] { 0, 1, 1, 2, 2 }, new int[5] { -1, 0, 1, 2, 3 }, new int[5][]
			{
				new int[1] { 4 },
				new int[2] { 5, 6 },
				new int[2] { 7, 8 },
				new int[4] { -1, 9, 10, 11 },
				new int[4] { -1, 12, 13, 14 }
			}, 2, new int[29]
			{
				0, 2048, 186, 46, 744, 12, 92, 372, 1500, 28,
				66, 130, 260, 520, 1112, 6, 20, 36, 56, 78,
				110, 158, 222, 316, 440, 624, 928, 1300, 1700
			}, 60f, 30f, 500f, 3f, 18f, 2048),
			new Floor(new int[6] { 0, 1, 1, 2, 3, 3 }, new int[4] { 2, 3, 3, 3 }, new int[4] { 0, 1, 2, 2 }, new int[4] { -1, 0, 1, 2 }, new int[4][]
			{
				new int[1] { 3 },
				new int[2] { 4, 5 },
				new int[4] { -1, 6, 7, 8 },
				new int[4] { -1, 9, 10, 11 }
			}, 2, new int[19]
			{
				0, 512, 46, 186, 16, 33, 65, 93, 130, 278,
				7, 23, 39, 55, 79, 110, 156, 232, 360
			}, 60f, 30f, 500f, 1f, 18f, 512),
			new Floor(new int[1], new int[1], new int[1], new int[1] { -1 }, new int[1][] { new int[1] { -1 } }, 2, new int[2] { 0, 12 }, 60f, 30f, 500f, 1f, 18f, 10)
		};
	}
}
