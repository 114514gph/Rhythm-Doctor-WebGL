using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_SetVFXPreset : InspectorPanel
	{
		public static RDThemeFX[] displayedPresets;

		public static List<RDThemeFX> legacyPresetsInLevel = new List<RDThemeFX>();

		public static void UpdatePresetsToDisplay()
		{
			List<RDThemeFX> list = new List<RDThemeFX>();
			foreach (KeyValuePair<RDThemeFX, RDEditorConstants.VFXInfo> vFXInfo in RDEditorConstants.VFXInfos)
			{
				RDThemeFX key = vFXInfo.Key;
				if (!vFXInfo.Value.legacy || legacyPresetsInLevel.Contains(key))
				{
					list.Add(key);
				}
			}
			displayedPresets = list.ToArray();
		}

		public static void AddLegacyPresetToLevel(RDThemeFX preset)
		{
			if (!legacyPresetsInLevel.Contains(preset))
			{
				legacyPresetsInLevel.RDAdd(preset);
			}
		}

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetVFXPreset levelEvent_SetVFXPreset = (LevelEvent_SetVFXPreset)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "preset")
				{
					PropertyControl_Dropdown propertyControl_Dropdown = (PropertyControl_Dropdown)property.control;
					UpdatePresetsToDisplay();
					propertyControl_Dropdown.SetDropdownOptions(displayedPresets);
					foreach (RDThemeFX item in legacyPresetsInLevel)
					{
						int index = Array.IndexOf(displayedPresets, item);
						Dropdown.OptionData optionData = propertyControl_Dropdown.dropdown.options[index];
						optionData.text = "<color=#8b0000>" + optionData.text + "</color>";
					}
					levelEvent_SetVFXPreset.UpdateRoomsUsage();
				}
				if (property.name == "intensity" && levelEvent_SetVFXPreset.preset == RDThemeFX.Bloom)
				{
					PropertyControl_InputField propertyControl_InputField = (PropertyControl_InputField)property.control;
					propertyControl_InputField.inputField.text = Mathf.Clamp(float.Parse(propertyControl_InputField.inputField.text), -5f, 5f).ToString();
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetVFXPreset levelEvent_SetVFXPreset = (LevelEvent_SetVFXPreset)levelEvent;
			bool flag = true;
			bool hasColor = RDEditorConstants.VFXInfos[levelEvent_SetVFXPreset.preset].hasColor;
			Color valueOrDefault = RDEditorConstants.VFXInfos[levelEvent_SetVFXPreset.preset].defaultColor.GetValueOrDefault(Color.white);
			bool flag2 = true;
			bool hasPosition = RDEditorConstants.VFXInfos[levelEvent_SetVFXPreset.preset].hasPosition;
			Float2 valueOrDefault2 = RDEditorConstants.VFXInfos[levelEvent_SetVFXPreset.preset].defaultPosition.GetValueOrDefault(new Float2(50f, 50f));
			foreach (Property property in properties)
			{
				if ((property.name == "color") & hasColor)
				{
					Color color = ((PropertyControl_Color)property.control).colorPicker.color.HexToColor();
					flag = levelEvent_SetVFXPreset.color.HasValue && (color == valueOrDefault || color == Color.clear);
				}
				if ((property.name == "position") & hasPosition)
				{
					Float2 percent = ((PropertyControl_PositionPicker)property.control).positionPicker.percent;
					flag2 = levelEvent_SetVFXPreset.position.HasValue && percent.x == valueOrDefault2.x && percent.y == valueOrDefault2.y;
				}
				if (property.name == "intensity" && levelEvent_SetVFXPreset.preset == RDThemeFX.Bloom)
				{
					PropertyControl_InputField propertyControl_InputField = (PropertyControl_InputField)property.control;
					propertyControl_InputField.inputField.text = Mathf.Clamp(float.Parse(propertyControl_InputField.inputField.text), -5f, 5f).ToString();
				}
				property.Save(levelEvent);
			}
			foreach (Property property2 in properties)
			{
				if (property2.name == "preset")
				{
					levelEvent_SetVFXPreset.UpdateRoomsUsage();
					UpdateUI(levelEvent);
				}
				if (property2.name == "color")
				{
					ColorOrPalette colorOrPalette = ColorOrPalette.FromColor(RDEditorConstants.VFXInfos[levelEvent_SetVFXPreset.preset].defaultColor.GetValueOrDefault(Color.white));
					((PropertyControl_Color)property2.control).SetToggleOnValue(colorOrPalette);
					if (flag)
					{
						levelEvent_SetVFXPreset.color = colorOrPalette;
						UpdateUI(levelEvent);
					}
				}
				if (property2.name == "position")
				{
					Float2 valueOrDefault3 = RDEditorConstants.VFXInfos[levelEvent_SetVFXPreset.preset].defaultPosition.GetValueOrDefault(new Float2(50f, 50f));
					((PropertyControl_PositionPicker)property2.control).SetToggleOnValue(valueOrDefault3);
					if (flag2)
					{
						levelEvent_SetVFXPreset.position = valueOrDefault3;
						UpdateUI(levelEvent);
					}
				}
			}
		}
	}
}
