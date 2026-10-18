using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityFileDialog;

namespace RDLevelEditor
{
	public class InspectorPanel_MakeRow : InspectorPanel
	{
		public ToggleGroup rowTypeToggleGroup;

		public ToggleGroup playerToggleGroup;

		public CharacterPicker character;

		public CharacterPicker cpuMarker;

		public Dropdown pulseSound;

		public Dropdown room;

		public InputField volume;

		public InputField offset;

		public InputField pitch;

		public InputField pan;

		public Slider volumeSlider;

		public Slider pitchSlider;

		public Slider panSlider;

		public ToggleGroup visibilityToggleGroup;

		public Text pulseSoundStatus;

		public RectTransform container;

		public InputField length;

		public Text rowTypeLabel;

		public Text characterErrorText;

		public Image cpuMarkerImage;

		public RectTransform cpuLabelTrans;

		public float rectSize;

		private bool updateRoomDropdown;

		private new void Awake()
		{
			base.Awake();
			AddOnEditListeners(base.tabSection_rows.UpdateUI, rowTypeToggleGroup, playerToggleGroup, visibilityToggleGroup, character, cpuMarker, pulseSound, length);
			AddOnEditListeners(() =>
			{
				base.tabSection_rows.UpdateUI();
				if (currentLevelEvent != null)
				{
					LevelEvent_MakeRow levelEvent_MakeRow = currentLevelEvent as LevelEvent_MakeRow;
					string mixerPath = MixerPath.BeatsoundsRows[levelEvent_MakeRow.row];
					levelEvent_MakeRow.pulseSound.PlayImmediately(mixerPath);
				}
			}, offset, volume);
			AddOnEditListeners(() =>
			{
				if (base.gameObject.activeSelf && playSoundOnEdit)
				{
					LevelEvent_MakeRow levelEvent_MakeRow = currentLevelEvent as LevelEvent_MakeRow;
					string mixerPath = MixerPath.BeatsoundsRows[levelEvent_MakeRow.row];
					levelEvent_MakeRow.pulseSound.PlayImmediately(mixerPath);
				}
			}, pitch, pan, volumeSlider, pitchSlider, panSlider);
			AddOnEditListeners(RoomDropdownWasUpdated, room);
			character.availableCharacters = RDEditorConstants.AvailableCharacters;
			cpuMarker.availableCharacters = RDEditorConstants.AvailableCPUCharacters;
			pulseSound.AddOptionsWithEnumArray(RDEditorConstants.BeatSounds);
			updateRoomDropdown = true;
			CharacterPicker characterPicker = character;
			characterPicker.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(characterPicker.onEndEdit, (CharacterPicker.EndHandler)((Character character, string expression, string customCharacter) =>
			{
				switch (character)
				{
				case Character.Custom:
					LoadCustomCharacterFromCharactersFolder(customCharacter);
					break;
				case Character.New:
					if (string.IsNullOrEmpty(base.editor.openedFilePath))
					{
						RDEditorUtils.AskUserToSaveLevel(RDString.Get("editor.dialog.saveBeforeImporting"));
					}
					else
					{
						InspectorPanel_CharacterTemplateGenerator inspectorPanel_CharacterTemplateGenerator = base.editor.inspectorPanelManager.Get<InspectorPanel_CharacterTemplateGenerator>();
						InspectorPanel.ShowPanel(inspectorPanel_CharacterTemplateGenerator.transform, RDString.Get("editor.CharacterTemplateGenerator"), Tab.Rows);
						inspectorPanel_CharacterTemplateGenerator.generator.targetRow = (currentLevelEvent as LevelEvent_MakeRow).row;
						inspectorPanel_CharacterTemplateGenerator.UpdateUI(null);
					}
					break;
				}
			}));
			CharacterPicker characterPicker2 = cpuMarker;
			characterPicker2.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(characterPicker2.onEndEdit, (CharacterPicker.EndHandler)((Character character, string expression, string customCharacter) =>
			{
				if (character == Character.Otto)
				{
					base.editor.ShowOttoHeart();
				}
			}));
			RectTransform component = GetComponent<RectTransform>();
			rectSize = component.sizeDelta.y;
		}

		public void ToggleLengthPicker(bool active)
		{
			GameObject gameObject = length.transform.parent.gameObject;
			GetComponent<RectTransform>().SizeDeltaY(active ? rectSize : (rectSize - gameObject.GetComponent<RectTransform>().sizeDelta.y));
			gameObject.SetActive(active);
		}

		private void LoadCustomCharacterFromCharactersFolder(string customCharName)
		{
			string text = Path.Combine(LevelValidation.CustomCharactersPath, customCharName);
			if (RDDirectory.Exists(text))
			{
				string ccJsonPath = Path.Combine(text, customCharName + ".json");
				LoadCustomCharacterFromPath(ccJsonPath, placeFilesInSubFolder: true, fromCustomCharactersFolder: true);
			}
		}

		public void RoomDropdownWasUpdated()
		{
			int num = currentLevelEvent.room;
			int value = room.value;
			if (num == value)
			{
				return;
			}
			using (new SaveStateScope())
			{
				int num2 = 0;
				foreach (LevelEvent_MakeRow rowsDatum in base.editor.rowsData)
				{
					if (rowsDatum.room == value)
					{
						num2++;
					}
				}
				if (num2 < 4)
				{
					LevelEvent_MakeRow item = currentLevelEvent as LevelEvent_MakeRow;
					int index = base.editor.rowsData.IndexOf(item);
					List<LevelEventControl_Base> list = base.editor.eventControls_rows[index];
					currentLevelEvent.room = value;
					foreach (LevelEventControl_Base item2 in list)
					{
						item2.levelEvent.room = value;
						item2.transform.SetParent(base.tabSection_rows.container[value].transform, worldPositionStays: false);
					}
					while (base.editor.MoveRowVertically(currentLevelEvent as LevelEvent_MakeRow, VerticalDirection.Down))
					{
					}
					foreach (List<LevelEventControl_Base> eventControls_row in base.editor.eventControls_rows)
					{
						foreach (LevelEventControl_Base item3 in eventControls_row)
						{
							item3.UpdateUI();
						}
					}
					base.editor.tabSection_rows.ChangePage(value, calledByUnity: false);
					base.tabSection_rows.UpdateUI();
					UpdateRoomDropdown();
				}
				else
				{
					room.value = num;
					printe("room is full");
				}
			}
		}

		public void UpdateRoomDropdown()
		{
			room.ClearOptions();
			int value = currentLevelEvent.room;
			int[] array = new int[4];
			foreach (LevelEvent_MakeRow rowsDatum in base.editor.rowsData)
			{
				array[rowsDatum.room]++;
			}
			List<string> list = new List<string>();
			for (int i = 0; i < 4; i++)
			{
				string text = "";
				if (array[i] == 4)
				{
					text = RDString.Get("editor.MakeRow.fullRoomSuffix");
				}
				string text2 = RDString.Get("editor.roomIndex").Replace("[index]", (i + 1).ToString());
				list.Add(text2 + " " + text);
			}
			room.AddOptions(list);
			room.value = value;
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_MakeRow levelEvent_MakeRow = (LevelEvent_MakeRow)currentLevelEvent;
			rowTypeToggleGroup.SetValue(levelEvent_MakeRow.rowType.ToString());
			playerToggleGroup.SetValue(levelEvent_MakeRow.player.ToString());
			visibilityToggleGroup.SetValue(levelEvent_MakeRow.hideAtStart ? "hide" : "show");
			if (levelEvent_MakeRow.character != Character.Custom)
			{
				levelEvent_MakeRow.character.ToString();
			}
			else
			{
				_ = levelEvent_MakeRow.customCharacterName;
			}
			character.SetCharacter(levelEvent_MakeRow.character, levelEvent_MakeRow.customCharacterName);
			cpuMarker.SetCharacter(levelEvent_MakeRow.cpuMarker);
			UpdateCPUMarkerImage();
			if (levelEvent_MakeRow.failedLoadingCustomCharacter)
			{
				characterErrorText.text = RDString.Get("editor.MakeRow.customCharacter.failedLoading") + " " + levelEvent_MakeRow.customCharacterName;
				characterErrorText.font = RDString.GetAppropiateFontForString(characterErrorText.text);
				container.anchoredPosition = new Vector2(container.anchoredPosition.x, -69f);
			}
			else
			{
				characterErrorText.text = string.Empty;
				container.anchoredPosition = new Vector2(container.anchoredPosition.x, -54f);
			}
			volume.text = levelEvent_MakeRow.pulseSound.volume.ToString();
			offset.text = levelEvent_MakeRow.pulseSound.offset.ToString();
			base.editor.soundSettingsPopup.ToggleOffset(levelEvent_MakeRow.pulseSound.IsExternalClip());
			pitch.text = levelEvent_MakeRow.pulseSound.pitch.ToString();
			pan.text = levelEvent_MakeRow.pulseSound.pan.ToString();
			volumeSlider.value = levelEvent_MakeRow.pulseSound.volume;
			pitchSlider.value = levelEvent_MakeRow.pulseSound.pitch;
			panSlider.value = levelEvent_MakeRow.pulseSound.pan;
			pulseSound.SetValueForEnumArray(RDEditorConstants.BeatSounds, levelEvent_MakeRow.pulseSound.filename);
			pulseSoundStatus.text = string.Empty;
			if (updateRoomDropdown)
			{
				UpdateRoomDropdown();
			}
			updateRoomDropdown = true;
			length.text = (levelEvent_MakeRow.length ?? 7).ToString();
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			LevelEvent_MakeRow data = (LevelEvent_MakeRow)levelEvent;
			RowType rowTypeInToggle = rowTypeToggleGroup.GetEnumValueFromActiveToggle(RowType.Classic);
			if (data.rowType != rowTypeInToggle)
			{
				List<LevelEventControl_Base> rowControlsList = base.editor.eventControls_rows[base.editor.selectedRowIndex];
				if (rowControlsList.Count > 0)
				{
					string text = ((rowTypeInToggle == RowType.Classic) ? RDString.Get("editor.MakeRow.classic") : RDString.Get("editor.MakeRow.oneshot"));
					text = "<color=#1161FFFF>" + text + "</color>";
					string message = RDString.Get("editor.MakeRow.dialog.changeRowType").Replace("[rowType]", text);
					string text2 = RDString.Get("editor.MakeRow.dialog.change").Replace("[rowType]", text);
					string text3 = RDString.Get("editor.dialog.cancel");
					base.editor.dialog.Show(message, new string[2] { text3, text2 }, (int option) =>
					{
						base.editor.dialog.Hide(() =>
						{
							if (option == 1)
							{
								using (new SaveStateScope())
								{
									while (rowControlsList.Count > 0)
									{
										base.editor.DeleteEventControl(rowControlsList[0], selectControlToTheLeft: false);
									}
								}
								data.rowType = rowTypeInToggle;
								base.tabSection_rows.UpdateUI();
							}
							else
							{
								rowTypeToggleGroup.SetValue(data.rowType.ToString());
							}
						});
					});
				}
				else
				{
					data.rowType = rowTypeInToggle;
				}
			}
			data.player = playerToggleGroup.GetEnumValueFromActiveToggle(RDPlayer.P1);
			data.hideAtStart = visibilityToggleGroup.GetValueFromActiveToggle() == "hide";
			data.cpuMarker = cpuMarker.character;
			Character character = data.character;
			if (this.character.character != Character.Custom)
			{
				data.character = this.character.character;
			}
			else if (!string.IsNullOrEmpty(data.customCharacterName))
			{
				data.character = Character.Custom;
			}
			if (character != data.character)
			{
				string text4 = (GC.defaultCharacterPulseSounds.ContainsKey(character) ? GC.defaultCharacterPulseSounds[character] : "sndShaker");
				text4 = text4.Remove(0, 3);
				string text5 = (GC.defaultCharacterPulseSounds.ContainsKey(data.character) ? GC.defaultCharacterPulseSounds[data.character] : "sndShaker");
				text5 = text5.Remove(0, 3);
				if (data.pulseSound.filename == text4 && text4 != text5)
				{
					isUpdatingUI = true;
					data.pulseSound.filename = text5;
					pulseSound.SetValueForEnumArray(RDEditorConstants.BeatSounds, data.pulseSound.filename);
					isUpdatingUI = false;
				}
			}
			else
			{
				data.pulseSound.filename = pulseSound.GetCurrentSound(RDEditorConstants.BeatSounds);
			}
			data.pulseSound.offset = Convert.ToInt32(offset.text);
			data.pulseSound.volume = GetNewValue(data.pulseSound.volume, volume, volumeSlider);
			data.pulseSound.pitch = GetNewValue(data.pulseSound.pitch, pitch, pitchSlider);
			data.pulseSound.pan = GetNewValue(data.pulseSound.pan, pan, panSlider);
			data.pulseSound.Validate();
			data.length = ((int.TryParse(length.text, out var result) && result < 7 && result > 0) ? new int?(result) : ((int?)null));
			updateRoomDropdown = false;
			UpdateUIInternal(levelEvent);
		}

		public void ShowSelectorForPulseSound()
		{
			string text = RDEditorUtils.ShowFileSelectorForAudio(RDString.Get("editor.dialog.selectBeatSound"), -1L);
			if (!string.IsNullOrEmpty(text))
			{
				(currentLevelEvent as LevelEvent_MakeRow).pulseSound.filename = text;
				UpdateUI(currentLevelEvent);
			}
		}

		public void ShowSelectorForCustomCharacter()
		{
			if (string.IsNullOrEmpty(base.editor.openedFilePath))
			{
				RDEditorUtils.AskUserToSaveLevel(RDString.Get("editor.dialog.saveBeforeImporting"));
				return;
			}
			scnEditor.PauseIfUnpaused();
			string ccJsonPath = FileBrowser.PickFile(scnEditor.GetLastUsedFolder(), RDString.Get("editor.dialog.jsonFileFormat"), GC.SupportedJSONFiles, RDString.Get("editor.dialog.selectCustomCharacter"));
			LoadCustomCharacterFromPath(ccJsonPath);
		}

		public void UpdateCPUMarkerImage()
		{
			Dictionary<string, Sprite> markersDict = RowEntity.markersDict;
			string key = cpuMarker.character.ToString() + "1";
			if (markersDict.ContainsKey(key))
			{
				cpuMarkerImage.sprite = markersDict[key];
				cpuMarkerImage.rectTransform.sizeDelta = cpuMarkerImage.sprite.rect.size;
			}
			cpuLabelTrans.offsetMax = cpuLabelTrans.offsetMax.WithX((cpuMarker.character == Character.BlankCPU) ? 0f : (-20f));
		}

		public void LoadCustomCharacterFromPath(string ccJsonPath, bool placeFilesInSubFolder = false, bool fromCustomCharactersFolder = false)
		{
			if (RDEditorUtils.InitializeCustomCharacterFromPath(ccJsonPath, placeFilesInSubFolder, fromCustomCharactersFolder))
			{
				LevelEvent_MakeRow obj = currentLevelEvent as LevelEvent_MakeRow;
				obj.character = Character.Custom;
				obj.customCharacterName = Path.GetFileNameWithoutExtension(ccJsonPath);
				obj.UpdateCustomCharacter(justReturnErrors: false, editorRowHeader: true);
				(currentLevelEvent as LevelEvent_MakeRow).muteBeats = false;
				base.editor.tabSection_rows.UpdateUI();
				UpdateUI(currentLevelEvent);
			}
		}
	}
}
