using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SA.GoogleDoc
{
	[Serializable]
	public class DocTemplate
	{
		public string key;

		[SerializeField]
		private List<WorksheetTemplate> _worksheets = new List<WorksheetTemplate>();

		private string _creationTime = string.Empty;

		public string name = string.Empty;

		public bool IsTableListOpen;

		public bool IsOpen = true;

		public string docName;

		public int docIndex;

		public int selectedWorksheet;

		public List<WorksheetTemplate> Worksheets => _worksheets;

		public string CreationTime
		{
			get
			{
				if (File.Exists(Settings.GetCachePath(name)))
				{
					StreamReader streamReader = new StreamReader(Settings.GetCachePath(name));
					_creationTime = streamReader.ReadLine();
					streamReader.Close();
				}
				else
				{
					_creationTime = string.Empty;
				}
				return _creationTime;
			}
		}

		public void InitDoc()
		{
			int num = 1;
			do
			{
				name = "NewDocument" + (Settings.Instance.docs.Count + num);
				num++;
			}
			while (Settings.Instance.GetDocByName(name) != null);
			_worksheets.Add(new WorksheetTemplate("Sheet1", 0));
		}

		public void AddNewWorksheet()
		{
			_worksheets.Add(new WorksheetTemplate("Sheet" + (_worksheets.Count + 1), _worksheets.Count));
		}

		public void RemoveWorksheet(WorksheetTemplate list)
		{
			_worksheets.Remove(list);
		}

		public int GetWorksheetId(string name)
		{
			int result = 0;
			foreach (WorksheetTemplate worksheet in _worksheets)
			{
				if (worksheet.listName == name)
				{
					return worksheet.listId;
				}
			}
			return result;
		}

		public string GetWorksheetName(int id)
		{
			string result = id.ToString();
			foreach (WorksheetTemplate worksheet in _worksheets)
			{
				if (worksheet.listId == id)
				{
					return worksheet.listName;
				}
			}
			return result;
		}

		public bool WorksheetExist(string name)
		{
			int num = 0;
			foreach (WorksheetTemplate worksheet in _worksheets)
			{
				if (worksheet.listName == name)
				{
					num++;
				}
			}
			return num > 1;
		}

		public bool WorksheetExist(int id)
		{
			int num = 0;
			foreach (WorksheetTemplate worksheet in _worksheets)
			{
				if (worksheet.listId == id)
				{
					num++;
				}
			}
			return num > 1;
		}

		public void UpdateTime(string timeStr)
		{
			_creationTime = timeStr;
		}

		public string[] GetWorksheetNames()
		{
			List<string> list = new List<string>();
			foreach (WorksheetTemplate worksheet in _worksheets)
			{
				list.Add(worksheet.listName);
			}
			return list.ToArray();
		}
	}
}
