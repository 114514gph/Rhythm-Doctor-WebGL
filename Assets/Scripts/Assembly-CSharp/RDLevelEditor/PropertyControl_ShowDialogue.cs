using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_ShowDialogue : PropertyControl
	{
		public Text dialogueText;

		public Dropdown language;

		public InputField inputField;

		public CharacterPicker portraitPicker;

		public LetterMovementPicker letterMovementPicker;

		private readonly LetterMovementType[] availableLetterMovementTypes = new LetterMovementType[3]
		{
			LetterMovementType.Shake,
			LetterMovementType.Wave,
			LetterMovementType.Swirl
		};

		private string[] allLanguages;

		private int caretPosition;

		private int selectionStart;

		public override GameObject controlObject => inputField.gameObject;

		private ShowDialogueAttribute controlAttribute => (ShowDialogueAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			dialogueText.text = RDString.Get("editor.ShowDialogue.text");
			inputField.onEndEdit.AddListener((string _) =>
			{
				action(UpdateCaretPosition, "sndEditorValueChange");
			});
			inputField.onValueChanged.AddListener((string s) =>
			{
				RDEditorUtils.UpdateUIText(inputField.textComponent, s);
			});
			portraitPicker.availableCharacters = RDEditorUtils.GetAvailableDialogueCharacters().ToArray();
			CharacterPicker characterPicker = portraitPicker;
			characterPicker.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(characterPicker.onEndEdit, (CharacterPicker.EndHandler)((Character character, string expression, string customCharName) =>
			{
				if (character == Character.Custom)
				{
					AddCustomPortrait(customCharName, expression);
				}
				else
				{
					AddPortrait(character, expression);
				}
			}));
			CharacterPicker characterPicker2 = portraitPicker;
			characterPicker2.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(characterPicker2.onEndEdit, (CharacterPicker.EndHandler)delegate
			{
				action(null, "sndButtonClick");
			});
			letterMovementPicker.availableLetterMovementTypes = availableLetterMovementTypes;
			LetterMovementPicker obj = letterMovementPicker;
			obj.onEndEdit = (LetterMovementPicker.EndHandler)Delegate.Combine(obj.onEndEdit, (LetterMovementPicker.EndHandler)((LetterMovementType letterMovementType) =>
			{
				int start = Mathf.Min(selectionStart, caretPosition);
				int end = Mathf.Max(selectionStart, caretPosition);
				AddTextEffect(start, end, letterMovementType);
			}));
		}

		private void UpdateCaretPosition()
		{
			caretPosition = inputField.caretPosition;
			selectionStart = inputField.selectionAnchorPosition;
		}

		private void ResetCaretPosition()
		{
			caretPosition = 0;
			selectionStart = 0;
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			ResetCaretPosition();
			object eventValue = GetEventValue(levelEvent);
			inputField.text = eventValue?.ToString();
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			string text = inputField.text;
			SetEventValue(levelEvent, text);
		}

		private void Update()
		{
			if (RDBase.isDev)
			{
				if (Input.GetKeyDown(KeyCode.F1))
				{
					DEBUG_FormatAllDialogToClipboard();
				}
				else if (Input.GetKeyDown(KeyCode.F2))
				{
					DEBUG_FormatAllDialogToClipboard(andReplace: true);
				}
			}
		}

		private void AddPortrait(Character character, string expression)
		{
			string text = RDEditorConstants.DialogueCharacterAliases.FirstOrDefault((KeyValuePair<string, Character> x) => x.Value == character).Key ?? character.ToString();
			if (expression != null && expression != "neutral")
			{
				text = text + "_" + expression;
			}
			text += ": ";
			string text2 = inputField.text;
			char[] array = text2.ToCharArray();
			int startIndex = 0;
			for (int num = 0; num < caretPosition; num++)
			{
				if (array[num] == '\n')
				{
					startIndex = num + 1;
				}
			}
			inputField.text = text2.Insert(startIndex, text);
		}

		private void AddCustomPortrait(string charName, string expression)
		{
			string text = Path.Combine(LevelValidation.CustomCharactersPath, charName);
			if (!RDDirectory.Exists(text) || !RDEditorUtils.InitializeCustomCharacterFromPath(Path.Combine(text, charName + ".json"), placeFilesInSubFolder: true))
			{
				return;
			}
			string text2 = charName;
			if (expression != null && expression != "neutral")
			{
				text2 = text2 + "_" + expression;
			}
			text2 += ": ";
			string text3 = inputField.text;
			char[] array = text3.ToCharArray();
			int startIndex = 0;
			for (int i = 0; i < caretPosition; i++)
			{
				if (array[i] == '\n')
				{
					startIndex = i + 1;
				}
			}
			inputField.text = text3.Insert(startIndex, text2);
		}

		private void AddTextEffect(int start, int end, LetterMovementType movementType)
		{
			string text = inputField.text;
			string text2 = movementType.ToString().ToLower();
			inputField.text = text.Insert(end, "</" + text2 + ">").Insert(start, "<" + text2 + ">");
		}

		private void DEBUG_FormatAllDialogToClipboard(bool andReplace = false)
		{
			string fileName = Path.GetFileName(Path.GetDirectoryName(base.editor.openedFilePath));
			string text = "dia" + fileName;
			List<LevelEvent_ShowDialogue> list = base.editor.eventControls.Select((LevelEventControl_Base x) => x.levelEvent).OfType<LevelEvent_ShowDialogue>().ToList();
			list.Sort((LevelEvent_ShowDialogue left, LevelEvent_ShowDialogue right) => left.bar1000timesbeat.CompareTo(right.bar1000timesbeat));
			StringWriter stringWriter = new StringWriter();
			using (andReplace ? new SaveStateScope() : null)
			{
				int num = 1;
				foreach (LevelEvent_ShowDialogue item in list)
				{
					if (item.active)
					{
						string text2 = $"dia{num}";
						stringWriter.WriteLine("*" + text2 + "*\n" + item.text + "\n-> END");
						stringWriter.WriteLine();
						if (andReplace)
						{
							item.text = "[[ink:" + text + ",\n" + text2 + "]]";
						}
						num++;
					}
				}
			}
			GUIUtility.systemCopyBuffer = stringWriter.ToString().TrimEnd('\n');
			stringWriter.Dispose();
			base.editor.LevelEditorPlaySound("sndEditorEventCreateMultiple");
		}
	}
}
