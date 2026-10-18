namespace MP3Sharp.Decoding.Decoders.LayerIII
{
	internal class ScaleFactorData
	{
		public int[] l;

		public int[][] s;

		public ScaleFactorData()
		{
			l = new int[23];
			s = new int[3][];
			for (int i = 0; i < 3; i++)
			{
				s[i] = new int[13];
			}
		}
	}
}
