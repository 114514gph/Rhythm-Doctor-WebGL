using System;

namespace SA.GoogleDoc
{
	[Serializable]
	public class WorksheetTemplate
	{
		public string listName;

		public int listId;

		public WorksheetTemplate(string name, int id)
		{
			listName = name;
			listId = id;
		}
	}
}
