using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityFileDialog;

namespace RDLevelEditor
{
	public class InspectorPanel_Sprite : MakeDecorationInspectorPanel
	{
		public Text filename;

		public CharacterPicker character;

		public Text characterErrorText;

		public ToggleGroup visibility;

		public ToggleGroup previewToggle;

		private new void Awake()
		{
			base.Awake();
			AddOnEditListeners(base.tabSection_decorations.UpdateUI, visibility);
			AddOnEditListeners(base.tabSection_decorations.UpdateUI, previewToggle);
			List<Character> availableDialogueCharacters = RDEditorUtils.GetAvailableDialogueCharacters();
			availableDialogueCharacters.Insert(0, Character.Beans);
			availableDialogueCharacters.AddRange(RDEditorConstants.AvailableSpriteCharacters);
			character.availableCharacters = availableDialogueCharacters.ToArray();
			CharacterPicker characterPicker = character;
			characterPicker.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(characterPicker.onEndEdit, (CharacterPicker.EndHandler)((Character character, string expression, string customCharacter) =>
			{
				if (character == Character.Custom)
				{
					LoadCustomCharacterFromCharactersFolder(customCharacter);
				}
				else
				{
					LoadDefaultCharacter(character);
				}
			}));
		}

		public void LoadCustomCharacterFromPath(string spritePath, bool placeFilesInSubFolder = false, bool fromCustomCharactersFolder = false)
		{
			bool num = spritePath.EndsWith(".json");
			LevelEvent_Sprite levelEvent_Sprite = currentLevelEvent as LevelEvent_Sprite;
			string text = (num ? Path.GetFileNameWithoutExtension(spritePath) : Path.GetFileName(spritePath));
			bool flag = false;
			if (num)
			{
				if (RDEditorUtils.InitializeCustomCharacterFromPath(spritePath, placeFilesInSubFolder, fromCustomCharactersFolder))
				{
					levelEvent_Sprite.filename = text;
					levelEvent_Sprite.character = Character.Custom;
					flag = true;
				}
			}
			else
			{
				levelEvent_Sprite.filename = text;
				levelEvent_Sprite.character = Character.Custom;
				string directoryName = Path.GetDirectoryName(base.editor.openedFilePath);
				string directoryName2 = Path.GetDirectoryName(spritePath);
				string newPath = Path.Combine(directoryName, levelEvent_Sprite.filename);
				if (directoryName != directoryName2)
				{
					if (RDFile.Exists(newPath))
					{
						base.editor.dialog.Show(RDString.Get("editor.dialog.overwriteFiles").Replace("[files]", Path.GetFileName(newPath)), new string[2]
						{
							RDString.Get("editor.dialog.overwrite"),
							RDString.Get("editor.PublishLevel.dialogCancel")
						}, (int option) =>
						{
							base.editor.dialog.Hide(() =>
							{
								if (option == 0)
								{
									File.Copy(spritePath, newPath, overwrite: true);
								}
							});
						});
					}
					else
					{
						File.Copy(spritePath, newPath);
					}
				}
				flag = true;
			}
			if (flag)
			{
				levelEvent_Sprite.UpdateCustomCharacter(justReturnErrors: false, editorRowHeader: true);
				base.editor.tabSection_decorations.UpdateUI();
				UpdateUI(currentLevelEvent);
			}
		}

		private void LoadCustomCharacterFromCharactersFolder(string customCharName)
		{
			string text = Path.Combine(LevelValidation.CustomCharactersPath, customCharName);
			if (RDDirectory.Exists(text))
			{
				string spritePath = Path.Combine(text, customCharName + ".json");
				LoadCustomCharacterFromPath(spritePath, placeFilesInSubFolder: true, fromCustomCharactersFolder: true);
			}
		}

		private void LoadDefaultCharacter(Character character)
		{
			LevelEvent_Sprite obj = currentLevelEvent as LevelEvent_Sprite;
			obj.filename = string.Empty;
			obj.character = character;
			base.editor.tabSection_decorations.UpdateUI();
			UpdateUI(currentLevelEvent);
		}

		public void OpenFileSelector()
		{
			if (string.IsNullOrEmpty(base.editor.openedFilePath))
			{
				RDEditorUtils.AskUserToSaveLevel(RDString.Get("editor.dialog.saveBeforeImporting"));
				return;
			}
			scnEditor.PauseIfUnpaused();
			string text = FileBrowser.PickFile(scnEditor.GetLastUsedFolder(), RDString.Get("editor.dialog.jsonFileFormat"), GC.SupportedSpriteFiles, RDString.Get("editor.dialog.selectSprite"));
			if (!text.IsNullOrEmpty())
			{
				LoadCustomCharacterFromPath(text);
			}
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_Sprite levelEvent_Sprite = (LevelEvent_Sprite)currentLevelEvent;
			visibility.SetValue(levelEvent_Sprite.visible ? "show" : "hide");
			previewToggle.SetValue(levelEvent_Sprite.preview ? "on" : "off");
			if (levelEvent_Sprite.character != Character.Custom)
			{
				filename.text = RDString.Get($"enum.Character.{levelEvent_Sprite.character}.short");
			}
			else
			{
				filename.text = levelEvent_Sprite.filename;
				if (filename.text.IsNullOrEmpty())
				{
					filename.text = RDString.Get("editor.none");
				}
				if (levelEvent_Sprite.failedLoadingCustomCharacter)
				{
					characterErrorText.text = RDString.Get("editor.Sprite.spriteData.failedLoading") + " " + levelEvent_Sprite.filename;
					characterErrorText.font = RDString.GetAppropiateFontForString(characterErrorText.text);
					container.anchoredPosition = new Vector2(container.anchoredPosition.x, -35f);
				}
				else
				{
					characterErrorText.text = string.Empty;
					container.anchoredPosition = new Vector2(container.anchoredPosition.x, -20f);
				}
			}
			base.UpdateUIInternal(levelEvent);
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			if (base.gameObject.activeInHierarchy)
			{
				LevelEvent_Sprite obj = (LevelEvent_Sprite)levelEvent;
				obj.visible = visibility.GetValueFromActiveToggle() == "show";
				obj.preview = previewToggle.GetValueFromActiveToggle() == "on";
				base.SaveInternal(levelEvent);
			}
		}
	}
}
