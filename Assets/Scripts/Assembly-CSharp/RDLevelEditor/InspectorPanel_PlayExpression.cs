namespace RDLevelEditor
{
	public class InspectorPanel_PlayExpression : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_PlayExpression levelEvent_PlayExpression = (LevelEvent_PlayExpression)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "expression" && scnEditor.instance.rowsData.Count > 0)
				{
					LevelEvent_MakeRow rowData = scnEditor.instance.rowsData[levelEvent_PlayExpression.row];
					((PropertyControl_CharacterPicker)property.control).SetRowData(rowData);
				}
				property.UpdateUI(levelEvent);
			}
			foreach (Property property2 in properties)
			{
				if (property2.name == "expression")
				{
					PropertyControl_CharacterPicker propertyControl_CharacterPicker = (PropertyControl_CharacterPicker)property2.control;
					bool replace = levelEvent_PlayExpression.replace;
					propertyControl_CharacterPicker.SetNoneOption(replace);
					propertyControl_CharacterPicker.SetNewOption(includeNewOption: true);
					if (levelEvent_PlayExpression.expression.IsNullOrEmpty() && !replace)
					{
						levelEvent_PlayExpression.expression = "neutral";
						propertyControl_CharacterPicker.characterPicker.SetExpression(levelEvent_PlayExpression.expression);
						property2.Save(levelEvent);
					}
				}
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_PlayExpression levelEvent_PlayExpression = (LevelEvent_PlayExpression)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "expression" && scnEditor.instance.rowsData.Count > 0)
				{
					LevelEvent_MakeRow rowData = scnEditor.instance.rowsData[levelEvent_PlayExpression.row];
					((PropertyControl_CharacterPicker)property.control).SetRowData(rowData);
				}
				property.Save(levelEvent);
			}
			foreach (Property property2 in properties)
			{
				if (property2.name == "expression")
				{
					PropertyControl_CharacterPicker propertyControl_CharacterPicker = (PropertyControl_CharacterPicker)property2.control;
					bool replace = levelEvent_PlayExpression.replace;
					propertyControl_CharacterPicker.SetNoneOption(replace);
					if (levelEvent_PlayExpression.expression.IsNullOrEmpty() && !replace)
					{
						levelEvent_PlayExpression.expression = "neutral";
						propertyControl_CharacterPicker.characterPicker.SetExpression(levelEvent_PlayExpression.expression);
						property2.Save(levelEvent);
					}
				}
			}
		}
	}
}
