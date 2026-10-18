using System;

namespace SA.GoogleDoc
{
	[Serializable]
	public class CellDictionaryRange
	{
		public CellRange cellRange;

		public int rowShift;

		public int columnShift;

		public CellDictionaryRange()
			: this(new CellRange(), 0, 0)
		{
		}

		public CellDictionaryRange(CellRange range, int colShiftValue, int rowShiftValue)
		{
			cellRange = range;
			columnShift = colShiftValue;
			rowShift = rowShiftValue;
		}

		public CellRange GetCellRangeWithOffset()
		{
			return new CellRange
			{
				useLinebreak = cellRange.useLinebreak,
				LineLength = cellRange.LineLength,
				direction = cellRange.direction,
				StartCell = 
				{
					col = cellRange.StartCell.col + columnShift,
					row = cellRange.StartCell.row + rowShift
				}
			};
		}
	}
}
