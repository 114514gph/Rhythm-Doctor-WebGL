namespace MP3Sharp.Decoding.Decoders.LayerIII
{
	internal class ScaleFactorTable
	{
		private LayerIIIDecoder enclosingInstance;

		public int[] l;

		public int[] s;

		public LayerIIIDecoder Enclosing_Instance => enclosingInstance;

		public ScaleFactorTable(LayerIIIDecoder enclosingInstance)
		{
			InitBlock(enclosingInstance);
			l = new int[5];
			s = new int[3];
		}

		public ScaleFactorTable(LayerIIIDecoder enclosingInstance, int[] thel, int[] thes)
		{
			InitBlock(enclosingInstance);
			l = thel;
			s = thes;
		}

		private void InitBlock(LayerIIIDecoder enclosingInstance)
		{
			this.enclosingInstance = enclosingInstance;
		}
	}
}
