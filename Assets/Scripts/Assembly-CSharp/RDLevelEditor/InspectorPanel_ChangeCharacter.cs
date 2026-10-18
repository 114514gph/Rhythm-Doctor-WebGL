namespace RDLevelEditor
{
	public class InspectorPanel_ChangeCharacter : InspectorPanel
	{
		private void checkForCC(LevelEvent_Base levelEvent, bool isUI)
		{
			LevelEvent_ChangeCharacter levelEvent_ChangeCharacter = (LevelEvent_ChangeCharacter)levelEvent;
			string customCharacter = levelEvent_ChangeCharacter.customCharacter;
			foreach (Property property in properties)
			{
				if (property.name == "character")
				{
					CharacterPicker characterPicker = ((PropertyControl_CharacterPicker)property.control).characterPicker;
					if (isUI)
					{
						characterPicker.SetCharacter(levelEvent_ChangeCharacter.character, levelEvent_ChangeCharacter.customCharacter);
					}
					if (characterPicker.character != Character.Custom)
					{
						levelEvent_ChangeCharacter.customCharacter = "";
						levelEvent_ChangeCharacter.character = characterPicker.character;
					}
					else if (characterPicker.customCharacter.IsNullOrEmpty() && !customCharacter.IsNullOrEmpty())
					{
						characterPicker.SetCharacter(Character.Custom, customCharacter);
					}
					else if (!characterPicker.customCharacter.IsNullOrEmpty() && characterPicker.customCharacter != customCharacter)
					{
						levelEvent_ChangeCharacter.customCharacter = characterPicker.customCharacter;
					}
					else if (characterPicker.customCharacter != customCharacter)
					{
						characterPicker.SetCharacter(Character.Samurai);
						levelEvent_ChangeCharacter.customCharacter = "";
						levelEvent_ChangeCharacter.character = Character.Samurai;
					}
				}
				if (customCharacter != levelEvent_ChangeCharacter.customCharacter && property.name == "customCharacter")
				{
					((PropertyControl_InputField)property.control).inputField.text = levelEvent_ChangeCharacter.customCharacter;
				}
				if (isUI)
				{
					property.UpdateUI(levelEvent);
				}
				else
				{
					property.Save(levelEvent);
				}
			}
		}

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			checkForCC(levelEvent, isUI: true);
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			checkForCC(levelEvent, isUI: false);
		}
	}
}
