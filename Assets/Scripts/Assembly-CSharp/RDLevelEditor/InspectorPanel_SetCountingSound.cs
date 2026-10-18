using System.Linq;

namespace RDLevelEditor
{
	public class InspectorPanel_SetCountingSound : InspectorPanel
	{
		public static readonly CountingVoiceSource[] countingVoiceSources = new CountingVoiceSource[20]
		{
			CountingVoiceSource.JyiCount,
			CountingVoiceSource.JyiCountFast,
			CountingVoiceSource.JyiCountCalm,
			CountingVoiceSource.JyiCountTired,
			CountingVoiceSource.JyiCountVeryTired,
			CountingVoiceSource.JyiCountJapanese,
			CountingVoiceSource.IanCount,
			CountingVoiceSource.IanCountFast,
			CountingVoiceSource.IanCountCalm,
			CountingVoiceSource.IanCountSlow,
			CountingVoiceSource.IanCountSlower,
			CountingVoiceSource.WhistleCount,
			CountingVoiceSource.BirdCount,
			CountingVoiceSource.ParrotCount,
			CountingVoiceSource.OwlCount,
			CountingVoiceSource.OrioleCount,
			CountingVoiceSource.WrenCount,
			CountingVoiceSource.CanaryCount,
			CountingVoiceSource.SpearCount,
			CountingVoiceSource.JyiCountLegacy
		};

		public static readonly CountingVoiceSource[] subdivisionVoiceSources = new CountingVoiceSource[4]
		{
			CountingVoiceSource.JyiCountEnglish,
			CountingVoiceSource.IanCountEnglish,
			CountingVoiceSource.IanCountEnglishCalm,
			CountingVoiceSource.IanCountEnglishSlow
		};

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetCountingSound levelEvent_SetCountingSound = (LevelEvent_SetCountingSound)levelEvent;
			CountingVoiceSource[] options = ((base.editor.rowsData.Count > 0 && base.editor.rowsData[levelEvent.row].rowType == RowType.Oneshot) ? subdivisionVoiceSources : countingVoiceSources).Append(CountingVoiceSource.Custom).ToArray();
			foreach (Property property in properties)
			{
				if (property.name == "sounds" && scnEditor.instance.rowsData.Count > 0)
				{
					LevelEvent_MakeRow rowData = scnEditor.instance.rowsData[levelEvent_SetCountingSound.row];
					((PropertyControl_SetCountingSound)property.control).SetRowData(rowData);
				}
				if (property.name == "voiceSource")
				{
					((PropertyControl_Dropdown)property.control).SetDropdownOptions(options);
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetCountingSound levelEvent_SetCountingSound = (LevelEvent_SetCountingSound)levelEvent;
			CountingVoiceSource[] array = ((base.editor.rowsData.Count > 0 && base.editor.rowsData[levelEvent.row].rowType == RowType.Oneshot) ? subdivisionVoiceSources : countingVoiceSources).Append(CountingVoiceSource.Custom).ToArray();
			foreach (Property property in properties)
			{
				if (property.name == "sounds" && scnEditor.instance.rowsData.Count > 0)
				{
					LevelEvent_MakeRow rowData = scnEditor.instance.rowsData[levelEvent_SetCountingSound.row];
					((PropertyControl_SetCountingSound)property.control).SetRowData(rowData);
				}
				if (property.name == "voiceSource")
				{
					PropertyControl_Dropdown propertyControl_Dropdown = (PropertyControl_Dropdown)property.control;
					if (propertyControl_Dropdown.dropdownOptions.Length != array.Length)
					{
						propertyControl_Dropdown.SetDropdownOptions(array);
					}
				}
				if (property.name == "subdivOffset")
				{
					((PropertyControl_Slider)property.control).snapInterval = 1f / (float)base.editor.denominator;
				}
				property.Save(levelEvent);
			}
		}
	}
}
