using System;

namespace SA.GoogleDoc
{
	[Serializable]
	public class CellRange
	{
		public RangeDirection direction;

		public Cell StartCell;

		public bool useLinebreak;

		public int LineLength = 1;

		public CellRange()
			: this("A1")
		{
		}

		public CellRange(string StartCellKey)
			: this(StartCellKey, RangeDirection.Row)
		{
		}

		public CellRange(string StartCellKey, RangeDirection rangeDirection)
		{
			direction = rangeDirection;
			StartCell = new Cell(StartCellKey);
		}
	}
}
