using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Row : PropertyControl
	{
		public Dropdown dropdown;

		public override GameObject controlObject => dropdown.gameObject;

		private RowAttribute controlAttribute => (RowAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			SetDropdownOptionsUsingRows();
			dropdown.onValueChanged.AddListener((int _) =>
			{
				action(null, "sndListClick");
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			SetDropdownOptionsUsingRows(controlAttribute.includeAll);
			int num = Convert.ToInt32(eventValue);
			if (controlAttribute.includeAll)
			{
				num++;
			}
			dropdown.value = num;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			int num = dropdown.value;
			if (controlAttribute.includeAll)
			{
				num--;
			}
			SetEventValue(levelEvent, num);
		}

		public void SetDropdownOptionsUsingRows(bool includeAll = false)
		{
			dropdown.ClearOptions();
			if (includeAll)
			{
				dropdown.AddOptions(new List<string> { RDString.Get("editor.TintRows.rows.allRows") });
			}
			dropdown.AddOptionsWithRowsList();
		}
	}
}
