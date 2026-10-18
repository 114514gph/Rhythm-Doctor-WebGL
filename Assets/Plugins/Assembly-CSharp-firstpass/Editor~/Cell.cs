using System;
using UnityEngine;

namespace SA.GoogleDoc
{
	[Serializable]
	public class Cell
	{
		[SerializeField]
		private int _col = 1;

		[SerializeField]
		private int _row = 1;

		[SerializeField]
		private string _key = "A1";

		public int row
		{
			get
			{
				return _row;
			}
			set
			{
				_row = value;
				GenerateKey();
			}
		}

		public int col
		{
			get
			{
				return _col;
			}
			set
			{
				_col = value;
				GenerateKey();
			}
		}

		public string key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
				int num = _key.IndexOfAny("0123456789".ToCharArray());
				int num2 = -1;
				string text;
				try
				{
					text = _key.Substring(0, num);
					num2 = int.Parse(_key.Substring(num));
				}
				catch (Exception)
				{
					text = string.Empty;
					num2 = -1;
				}
				text = text.ToUpperInvariant();
				int num3 = 0;
				for (int i = 0; i < text.Length; i++)
				{
					num3 *= 26;
					num3 += text[i] - 65 + 1;
				}
				_col = ((num3 != 0) ? num3 : _col);
				_row = ((num2 != -1) ? num2 : _row);
			}
		}

		public Cell(int colIndex = 0, int rowIndex = 0)
		{
			_col = colIndex;
			_row = rowIndex;
			GenerateKey();
		}

		public Cell(string CellKey)
		{
			key = CellKey;
		}

		private void GenerateKey()
		{
			_key = ((char)(65 + _col - 1)).ToString() + _row;
		}
	}
}
