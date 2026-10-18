using System.Collections.Generic;

namespace SA.GoogleDoc
{
	public class SpreadsheetData
	{
		public Dictionary<int, Dictionary<string, string>> worksheets = new Dictionary<int, Dictionary<string, string>>();

		private string englishRowData = string.Empty;

		public void SetData(int worksheetIndex, int row, int col, string data)
		{
			if (col == 2)
			{
				englishRowData = data;
			}
			else if (col > 2 && data == "RDNull")
			{
				data = englishRowData;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (worksheets.TryGetValue(worksheetIndex, out var value))
			{
				dictionary = value;
			}
			else
			{
				worksheets.Add(worksheetIndex, dictionary);
			}
			dictionary[$"{row}:{col}"] = data;
		}

		public string GetData(int worksheetIndex, int row, int col)
		{
			if (!worksheets.TryGetValue(worksheetIndex, out var value) || !value.TryGetValue($"{row}:{col}", out var value2))
			{
				return string.Empty;
			}
			return value2;
		}
	}
}
