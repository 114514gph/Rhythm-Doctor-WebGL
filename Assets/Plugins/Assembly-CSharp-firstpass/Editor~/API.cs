using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.GoogleDoc
{
	public static class API
	{
		public static string lastWrittenInkFilePath = "";

		public static T GetValue<T>(string docName, int row, int col, int workSheetNumber = 0)
		{
			T result = default;
			if (Reader.sheets.TryGetValue(docName, out var value) && value != null)
			{
				string data = value.GetData(workSheetNumber, row, col);
				try
				{
					return (T)Convert.ChangeType(data, typeof(T));
				}
				catch (FormatException)
				{
					return default;
				}
			}
			return result;
		}

		public static T GetValue<T>(string docName, Cell cell, int workSheetNumber = 0)
		{
			return GetValue<T>(docName, cell.row, cell.col, workSheetNumber);
		}

		public static List<T> GetList<T>(string docName, CellRange range, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return new List<T>();
			}
			return GetList<T>(docName, range, docByName.GetWorksheetId(workSheetName));
		}

		public static List<T> GetListWithIndexes<T>(List<int> indexes, string docName, CellRange range, int workSheetNumber = 0)
		{
			List<T> list = new List<T>();
			Cell cell = new Cell(range.StartCell.col, range.StartCell.row);
			foreach (int index in indexes)
			{
				if (range.direction == RangeDirection.Column)
				{
					cell.row = index;
				}
				else
				{
					cell.col = index;
				}
				string text = GetValue<string>(docName, cell, workSheetNumber);
				if (text.Equals("null"))
				{
					text = "";
				}
				T item;
				try
				{
					item = (T)Convert.ChangeType(text, typeof(T));
				}
				catch (FormatException)
				{
					item = default;
				}
				list.Add(item);
			}
			return list;
		}

		public static List<T> GetList<T>(string docName, CellRange range, int workSheetNumber = 0, List<int> indexesOut = null)
		{
			List<T> list = new List<T>();
			Cell cell = new Cell(range.StartCell.col, range.StartCell.row);
			if (range.direction == RangeDirection.Row)
			{
				int num = 1;
				while (true)
				{
					string value = GetValue<string>(docName, cell, workSheetNumber);
					if (value.Equals(string.Empty))
					{
						if (range.useLinebreak && num >= range.LineLength)
						{
							cell.col = range.StartCell.col;
							cell.row++;
							num = 1;
							continue;
						}
						return list;
					}
					if (value == null)
					{
						break;
					}
					indexesOut?.Add(cell.row);
					if (range.useLinebreak && num >= range.LineLength)
					{
						cell.col = range.StartCell.col;
						cell.row++;
						num = 1;
					}
					else
					{
						num++;
						cell.col++;
					}
					T val = default;
					try
					{
						val = (T)Convert.ChangeType(value, typeof(T));
					}
					catch (FormatException)
					{
						val = default;
					}
					list.Add(val);
				}
				return list;
			}
			if (range.direction == RangeDirection.Column)
			{
				int num2 = 1;
				while (true)
				{
					string value = GetValue<string>(docName, cell, workSheetNumber);
					if (value.Equals(string.Empty))
					{
						if (range.useLinebreak && num2 >= range.LineLength)
						{
							cell.row = range.StartCell.row;
							cell.col++;
							num2 = 1;
							continue;
						}
						return list;
					}
					if (value == null)
					{
						break;
					}
					indexesOut?.Add(cell.row);
					if (range.useLinebreak && num2 >= range.LineLength)
					{
						cell.row = range.StartCell.row;
						cell.col++;
						num2 = 1;
					}
					else
					{
						num2++;
						cell.row++;
					}
					T val2 = default;
					try
					{
						val2 = (T)Convert.ChangeType(value, typeof(T));
					}
					catch (FormatException)
					{
						val2 = default;
					}
					list.Add(val2);
				}
				return list;
			}
			return list;
		}

		public static T[] GetArray<T>(string docName, CellRange range, int workSheetNumber = 0)
		{
			return GetList<T>(docName, range, workSheetNumber).ToArray();
		}

		public static T[] GetArray<T>(string docName, CellRange range, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return Array.Empty<T>();
			}
			return GetList<T>(docName, range, docByName.GetWorksheetId(workSheetName)).ToArray();
		}

		public static Dictionary<K, V> GetDictionary<K, V>(string docName, CellDictionaryRange dictionaryRange, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return new Dictionary<K, V>();
			}
			return GetDictionary<K, V>(docName, dictionaryRange, docByName.GetWorksheetId(workSheetName));
		}

		public static Dictionary<K, V> GetDictionary<K, V>(string docName, CellDictionaryRange dictionaryRange, int workSheetNumber = 0)
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			List<int> list = new List<int>();
			List<K> list2 = GetList<K>(docName, dictionaryRange.cellRange, workSheetNumber, list);
			List<V> listWithIndexes = GetListWithIndexes<V>(list, docName, dictionaryRange.GetCellRangeWithOffset(), workSheetNumber);
			for (int i = 0; i < list2.Count; i++)
			{
				if (listWithIndexes.Count > i)
				{
					if (!dictionary.ContainsKey(list2[i]))
					{
						dictionary.Add(list2[i], listWithIndexes[i]);
					}
				}
				else if (dictionary.ContainsKey(list2[i]))
				{
					Debug.Log("it already exists on " + list2[i]);
				}
				else
				{
					dictionary.Add(list2[i], default);
				}
			}
			return dictionary;
		}

		public static bool IsValueOfType<T>(string docName, int row, int col, int workSheetNumber = 0)
		{
			if (Reader.sheets.TryGetValue(docName, out var value) && value != null)
			{
				string data = value.GetData(workSheetNumber, row, col);
				try
				{
					Convert.ChangeType(data, typeof(T));
					return true;
				}
				catch (FormatException)
				{
					return false;
				}
			}
			return false;
		}

		public static bool IsValueOfType<T>(string docName, Cell cell, int workSheetNumber = 0)
		{
			return IsValueOfType<T>(docName, cell.row, cell.col, workSheetNumber);
		}

		public static Cell FindCellByContent(string docName, object content, int workSheetNumber = 0)
		{
			if (Settings.Instance.GetDocByName(docName) != null)
			{
				Cell cell = new Cell(1, 1);
				string value = GetValue<string>(docName, cell, workSheetNumber);
				int num = 0;
				while (num < 2)
				{
					if (value.Equals(content.ToString()))
					{
						return cell;
					}
					if (value.Equals(string.Empty))
					{
						cell.col = 1;
						cell.row++;
						num++;
					}
					else
					{
						cell.col++;
						num = 0;
					}
					value = GetValue<string>(docName, cell, workSheetNumber);
				}
			}
			return null;
		}

		public static Cell FindCellByContent(string docName, object content, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return null;
			}
			return FindCellByContent(docName, content, docByName.GetWorksheetId(workSheetName));
		}

		public static Cell FindCellByContent(string docName, string column, object content, int workSheetNumber = 0)
		{
			if (Settings.Instance.GetDocByName(docName) != null)
			{
				Cell cell = new Cell(column + "1");
				string value = GetValue<string>(docName, cell, workSheetNumber);
				while (!value.Equals(string.Empty))
				{
					if (value.Equals(content.ToString()))
					{
						return cell;
					}
					cell.row++;
					value = GetValue<string>(docName, cell, workSheetNumber);
				}
			}
			return null;
		}

		public static Cell FindCellByContent(string docName, string column, object content, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return null;
			}
			return FindCellByContent(docName, column, content, docByName.GetWorksheetId(workSheetName));
		}

		public static Cell FindCellByContent(string docName, int row, object content, int workSheetNumber = 0)
		{
			if (Settings.Instance.GetDocByName(docName) == null)
			{
				return null;
			}
			Cell cell = new Cell(1, row);
			string value = GetValue<string>(docName, cell, workSheetNumber);
			while (!value.Equals(string.Empty))
			{
				if (value.Equals(content.ToString()))
				{
					return cell;
				}
				cell.col++;
				value = GetValue<string>(docName, cell, workSheetNumber);
			}
			return null;
		}

		public static Cell FindCellByContent(string docName, int row, object content, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return null;
			}
			return FindCellByContent(docName, row, content, docByName.GetWorksheetId(workSheetName));
		}

		public static Cell[] FindCellsByContent(string docName, object content, int workSheetNumber = 0)
		{
			List<Cell> list = new List<Cell>();
			if (Settings.Instance.GetDocByName(docName) != null)
			{
				Cell cell = new Cell(1, 1);
				string value = GetValue<string>(docName, cell, workSheetNumber);
				int num = 0;
				while (num < 2)
				{
					if (value.Equals(content.ToString()))
					{
						list.Add(new Cell(cell.col, cell.row));
					}
					if (value.Equals(string.Empty))
					{
						cell.col = 1;
						cell.row++;
						num++;
					}
					else
					{
						cell.col++;
						num = 0;
					}
					value = GetValue<string>(docName, cell, workSheetNumber);
				}
			}
			return list.ToArray();
		}

		public static Cell[] FindCellsByContent(string docName, object content, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return new List<Cell>().ToArray();
			}
			return FindCellsByContent(docName, content, docByName.GetWorksheetId(workSheetName));
		}

		public static Cell[] FindCellsByContent(string docName, string column, object content, int workSheetNumber = 0)
		{
			List<Cell> list = new List<Cell>();
			if (Settings.Instance.GetDocByName(docName) != null)
			{
				Cell cell = new Cell(column + "1");
				string value = GetValue<string>(docName, cell, workSheetNumber);
				while (!value.Equals(string.Empty))
				{
					if (value.Equals(content.ToString()))
					{
						list.Add(new Cell(cell.key));
					}
					cell.row++;
					value = GetValue<string>(docName, cell, workSheetNumber);
				}
			}
			return list.ToArray();
		}

		public static Cell[] FindCellsByContent(string docName, string column, object content, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return Array.Empty<Cell>();
			}
			return FindCellsByContent(docName, column, content, docByName.GetWorksheetId(workSheetName));
		}

		public static Cell[] FindCellsByContent(string docName, int row, object content, int workSheetNumber = 0)
		{
			List<Cell> list = new List<Cell>();
			if (Settings.Instance.GetDocByName(docName) != null)
			{
				Cell cell = new Cell(1, row);
				string value = GetValue<string>(docName, cell, workSheetNumber);
				while (!value.Equals(string.Empty))
				{
					if (value.Equals(content.ToString()))
					{
						list.Add(new Cell(cell.key));
					}
					cell.col++;
					value = GetValue<string>(docName, cell, workSheetNumber);
				}
			}
			return list.ToArray();
		}

		public static Cell[] FindCellsByContent(string docName, int row, object content, string workSheetName)
		{
			DocTemplate docByName = Settings.Instance.GetDocByName(docName);
			if (docByName == null)
			{
				return Array.Empty<Cell>();
			}
			return FindCellsByContent(docName, row, content, docByName.GetWorksheetId(workSheetName));
		}

		public static void RetrievePublicSheetData(DocTemplate doc, bool drawProgressBar = true, bool retrievingDialogues = false, int workSheetIndex = -1, bool building = false, SystemLanguage[] dialogueLanguages = null)
		{
			Reader.RetrievePublicSheetData(doc, drawProgressBar, retrievingDialogues, workSheetIndex, building, dialogueLanguages);
		}

		public static void RetrievePublicSheetData(DocTemplate doc, WorksheetTemplate worksheet, bool drawProgressBar = true)
		{
			Reader.RetrievePublicSheetDataLocalization(doc, worksheet, drawProgressBar);
		}
	}
}
