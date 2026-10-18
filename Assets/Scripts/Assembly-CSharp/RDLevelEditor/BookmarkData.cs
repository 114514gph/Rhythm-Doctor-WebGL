namespace RDLevelEditor
{
	public struct BookmarkData
	{
		public BarAndBeat barAndBeat;

		public int colorIndex;

		public BookmarkData(BarAndBeat newBarAndBeat, int newColorIndex)
		{
			barAndBeat = newBarAndBeat;
			colorIndex = newColorIndex;
		}

		public override string ToString()
		{
			return $"bar: {barAndBeat.bar}, beat: {barAndBeat.beat}, color: {colorIndex}";
		}
	}
}
