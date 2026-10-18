using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityFileDialog;

namespace RDLevelEditor
{
	public class PropertyControl_CharacterPicker : PropertyControl
	{
		public CharacterPicker characterPicker;

		public Button browse;

		public InputField customInput;

		public GameObject inputContainer;

		public Button switchDropdown;

		private LevelEvent_MakeRow rowData;

		private bool includeNoneOption;

		private bool includeNewOption;

		public override GameObject controlObject => characterPicker.gameObject;

		private CharacterPickerAttribute controlAttribute => (CharacterPickerAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			if (controlAttribute.showFilePicker)
			{
				RectTransform component = characterPicker.GetComponent<RectTransform>();
				component.offsetMax = new Vector2(-28f, component.offsetMax.y);
				browse.onClick.AddListener(() =>
				{
					OnFilePicker();
					action(null);
				});
				if (controlAttribute.includesCustomChars)
				{
					CharacterPicker obj = characterPicker;
					obj.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(obj.onEndEdit, (CharacterPicker.EndHandler)delegate
					{
						if (characterPicker.character == Character.Custom && !characterPicker.customCharacter.IsNullOrEmpty())
						{
							string text = Path.Combine(LevelValidation.CustomCharactersPath, characterPicker.customCharacter);
							if (RDDirectory.Exists(text))
							{
								string text2 = Path.Combine(text, characterPicker.customCharacter + ".json");
								if (File.Exists(text2))
								{
									RDEditorUtils.InitializeCustomCharacterFromPath(text2, placeFilesInSubFolder: true, fromCustomCharactersFolder: true);
								}
							}
						}
						action(null);
					});
				}
			}
			else
			{
				browse.gameObject.SetActive(value: false);
			}
			characterPicker.availableCharacters = controlAttribute.availableCharacters;
			characterPicker.portraitPicker = controlAttribute.portraitPicker;
			characterPicker.returnsExpression = controlAttribute.returnsExpression;
			characterPicker.includesCustomChars = controlAttribute.includesCustomChars;
			characterPicker.includesNew = controlAttribute.includesCustomChars && controlAttribute.includesNew;
			characterPicker.includesNone = controlAttribute.includesNone;
			characterPicker.emptyIsIgnore = controlAttribute.emptyIsIgnore;
			CharacterPicker obj2 = characterPicker;
			obj2.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(obj2.onEndEdit, (CharacterPicker.EndHandler)delegate
			{
				action(null, "sndButtonClick");
			});
			characterPicker.GetComponent<Button>().onClick.AddListener(characterPicker.returnsExpression ? new UnityAction(ShowExpressionList) : new UnityAction(characterPicker.ShowCharacterPickerPopup));
			customInput.onEndEdit.AddListener(delegate
			{
				action(null, "sndButtonClick");
			});
			characterPicker.textInput = inputContainer;
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			characterPicker.EnableCustomInput(enable: false);
			if (characterPicker.returnsExpression)
			{
				if (rowData != null)
				{
					string text = GetEventValue(levelEvent).ToString();
					text = ((text == "what") ? "surprised" : text);
					customInput.text = text;
					characterPicker.SetExpression(text);
				}
			}
			else
			{
				Character character = (Character)GetEventValue(levelEvent);
				characterPicker.SetCharacter(character, characterPicker.customCharacter);
			}
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			if (characterPicker.returnsExpression)
			{
				if (inputContainer.activeInHierarchy)
				{
					characterPicker.SetCustomExpression(customInput.text, "Neutral");
				}
				else
				{
					customInput.text = characterPicker.expression;
				}
			}
			object value = (characterPicker.returnsExpression ? characterPicker.expression : ((object)characterPicker.character));
			SetEventValue(levelEvent, value);
		}

		public void SetRowData(LevelEvent_MakeRow rowData)
		{
			if (characterPicker.returnsExpression)
			{
				this.rowData = rowData;
			}
		}

		public void SetNoneOption(bool includeNoneOption)
		{
			this.includeNoneOption = includeNoneOption;
		}

		public void SetNewOption(bool includeNewOption)
		{
			this.includeNewOption = includeNewOption;
		}

		private void ShowExpressionList()
		{
			if (rowData != null)
			{
				characterPicker.ShowExpressionsForCharacter(rowData.character, rowData.customCharacterName, includeNoneOption, includeNewOption);
			}
		}

		public void EnableCustomInput(bool enable)
		{
			characterPicker.SetCustomExpression(customInput.text, "Neutral");
			characterPicker.EnableCustomInput(enable);
		}

		public void OnFilePicker()
		{
			if (string.IsNullOrEmpty(base.editor.openedFilePath))
			{
				RDEditorUtils.AskUserToSaveLevel(RDString.Get("editor.dialog.saveBeforeImporting"));
				return;
			}
			scnEditor.PauseIfUnpaused();
			string text = FileBrowser.PickFile(scnEditor.GetLastUsedFolder(), RDString.Get("editor.dialog.jsonFileFormat"), GC.SupportedJSONFiles, RDString.Get("editor.dialog.selectCustomCharacter"));
			if (RDEditorUtils.InitializeCustomCharacterFromPath(text))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				characterPicker.SetCharacter(Character.Custom, fileNameWithoutExtension);
			}
		}
	}
}
