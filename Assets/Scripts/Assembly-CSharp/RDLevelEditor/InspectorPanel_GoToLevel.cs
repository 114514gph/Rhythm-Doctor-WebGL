using System.Collections.Generic;
using System.IO;

namespace RDLevelEditor
{
	public class InspectorPanel_GoToLevel : InspectorPanel
	{
		private string[] GetLevelsInDirectory(string current)
		{
			List<string> list = new List<string> { RDString.Get("editor.LevelSettings.noneSelected") };
			if (!string.IsNullOrEmpty(base.editor.openedFilePath))
			{
				string directoryName = Path.GetDirectoryName(base.editor.openedFilePath);
				string fileName = Path.GetFileName(base.editor.openedFilePath);
				string[] filesWithExtension = directoryName.GetFilesWithExtension("rdlevel", SearchOption.TopDirectoryOnly);
				for (int i = 0; i < filesWithExtension.Length; i++)
				{
					string fileName2 = Path.GetFileName(filesWithExtension[i]);
					if (fileName2 != "backup.rdlevel" && fileName2 != fileName)
					{
						list.Add(fileName2);
					}
				}
			}
			if (!current.IsNullOrEmpty() && !list.Contains(current))
			{
				list.Add(current);
			}
			return list.ToArray();
		}

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_GoToLevel levelEvent_GoToLevel = (LevelEvent_GoToLevel)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "rdlevel")
				{
					PropertyControl_Dropdown obj = (PropertyControl_Dropdown)property.control;
					string rDLevelFilename = RDUtils.GetRDLevelFilename(levelEvent_GoToLevel.rdlevel);
					obj.SetDropdownOptions(GetLevelsInDirectory(rDLevelFilename), localize: false);
				}
				property.UpdateUI(levelEvent);
			}
		}
	}
}
