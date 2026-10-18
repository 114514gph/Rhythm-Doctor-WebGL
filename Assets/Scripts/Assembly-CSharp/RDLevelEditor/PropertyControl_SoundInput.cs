using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_SoundInput : MonoBehaviour
	{
		public Dropdown optionsDropdown;

		public InputField filenameInputField;

		public EventTrigger optionsDropdownEventTrigger;

		public EventTrigger filenameInputFieldEventTrigger;

		public GameObject filenameContainer;

		public Button browseButton;

		public Button settingsButton;

		public Button switchButton;

		public AudioclipToOggEncoder oggEncoder;

		public static PropertyControl_SoundInput currentSoundInput;

		private PropertyControl propertyControl;

		[NonSerialized]
		public SoundDataStruct soundData;

		private bool hasOptions;

		private bool hasCustom;

		private bool showOffset;

		private bool inFileSelector;

		private bool soundWasJustSelected;

		private bool justPickedFile;

		private string[] dropdownValues;

		private string mixerPath = "CustomCueSound";

		private string overrideSoundFilename;

		private SoundAttribute controlAttribute => (SoundAttribute)propertyControl.propertyInfo.controlAttribute;

		public SoundSettingsPopup soundSettingsPopup => propertyControl.editor.soundSettingsPopup;

		public bool IsInCustom
		{
			get
			{
				if (hasCustom && hasOptions)
				{
					return optionsDropdown.value == optionsDropdown.options.Count - 1;
				}
				return false;
			}
		}

		private bool UseFilename
		{
			get
			{
				if (hasOptions)
				{
					return IsInCustom;
				}
				return true;
			}
		}

		public void SetMixerPath(string mixerPath)
		{
			this.mixerPath = mixerPath;
		}

		public void SetOverrideSoundFilename(string overrideSoundFilename)
		{
			this.overrideSoundFilename = overrideSoundFilename;
		}

		private void ClickEvent(PointerEventData eventData, Action registerForChange)
		{
			if (eventData.button == PointerEventData.InputButton.Right && eventData.eligibleForClick)
			{
				if (IsInCustom)
				{
					SwitchToDropdown(registerForChange);
				}
				else
				{
					SwitchToInputField(registerForChange);
				}
			}
		}

		public void AddListeners(PropertyControl propertyControl, InspectorPanel.ChangeAction action, bool useOptions)
		{
			this.propertyControl = propertyControl;
			hasOptions = useOptions;
			hasCustom = controlAttribute.customFile || !hasOptions;
			soundWasJustSelected = controlAttribute.isSong;
			if (hasOptions && hasCustom)
			{
				EventTrigger.Entry entry = new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerClick
				};
				entry.callback.AddListener((BaseEventData data) =>
				{
					ClickEvent((PointerEventData)data, () =>
					{
						action(null);
					});
				});
				optionsDropdownEventTrigger.triggers.Add(entry);
				EventTrigger.Entry entry2 = new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerClick
				};
				entry2.callback.AddListener((BaseEventData data) =>
				{
					ClickEvent((PointerEventData)data, () =>
					{
						action(null);
					});
				});
				filenameInputFieldEventTrigger.triggers.Add(entry2);
				Text textComponent = filenameInputField.textComponent;
				TextAnchor alignment = (filenameInputField.placeholder.GetComponent<Text>().alignment = TextAnchor.MiddleLeft);
				textComponent.alignment = alignment;
			}
			EnableControls(hasOptions);
			RDEventTrigger volume = soundSettingsPopup.volumeSlider.GetOrAddComponent<RDEventTrigger>();
			RDEventTrigger rDEventTrigger = volume;
			rDEventTrigger.onEndDrag = (RDPointerEventData)Delegate.Combine(rDEventTrigger.onEndDrag, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != volume.lastEventFrame && currentSoundInput == this)
				{
					action(PlaySoundOnEdit);
				}
			}));
			RDEventTrigger rDEventTrigger2 = volume;
			rDEventTrigger2.onClick = (RDPointerEventData)Delegate.Combine(rDEventTrigger2.onClick, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != volume.lastEventFrame && currentSoundInput == this)
				{
					action(PlaySoundOnEdit, "sndSliderGrab");
				}
			}));
			RDEventTrigger pitch = soundSettingsPopup.pitchSlider.GetOrAddComponent<RDEventTrigger>();
			RDEventTrigger rDEventTrigger3 = pitch;
			rDEventTrigger3.onEndDrag = (RDPointerEventData)Delegate.Combine(rDEventTrigger3.onEndDrag, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != pitch.lastEventFrame && currentSoundInput == this)
				{
					action(PlaySoundOnEdit);
				}
			}));
			RDEventTrigger rDEventTrigger4 = pitch;
			rDEventTrigger4.onClick = (RDPointerEventData)Delegate.Combine(rDEventTrigger4.onClick, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != pitch.lastEventFrame && currentSoundInput == this)
				{
					action(PlaySoundOnEdit, "sndSliderGrab");
				}
			}));
			RDEventTrigger pan = soundSettingsPopup.panSlider.GetOrAddComponent<RDEventTrigger>();
			RDEventTrigger rDEventTrigger5 = pan;
			rDEventTrigger5.onEndDrag = (RDPointerEventData)Delegate.Combine(rDEventTrigger5.onEndDrag, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != pan.lastEventFrame && currentSoundInput == this)
				{
					action(PlaySoundOnEdit);
				}
			}));
			RDEventTrigger rDEventTrigger6 = pan;
			rDEventTrigger6.onClick = (RDPointerEventData)Delegate.Combine(rDEventTrigger6.onClick, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != pan.lastEventFrame && currentSoundInput == this)
				{
					action(PlaySoundOnEdit, "sndSliderGrab");
				}
			}));
			soundSettingsPopup.volumeInputField.onEndEdit.AddListener((string _) =>
			{
				if (currentSoundInput == this)
				{
					action(PlaySoundOnEdit, "sndEditorValueChange");
				}
			});
			soundSettingsPopup.pitchInputField.onEndEdit.AddListener((string _) =>
			{
				if (currentSoundInput == this)
				{
					action(PlaySoundOnEdit, "sndEditorValueChange");
				}
			});
			soundSettingsPopup.panInputField.onEndEdit.AddListener((string _) =>
			{
				if (currentSoundInput == this)
				{
					action(PlaySoundOnEdit, "sndEditorValueChange");
				}
			});
			soundSettingsPopup.audioOffsetInputField.onEndEdit.AddListener((string _) =>
			{
				if (currentSoundInput == this)
				{
					action(PlaySoundOnEdit, "sndEditorValueChange");
				}
			});
			settingsButton.onClick.AddListener(() =>
			{
				currentSoundInput = this;
				ShowSoundSettings();
			});
			browseButton.onClick.AddListener(() =>
			{
				currentSoundInput = this;
				HideAndUpdatePopup();
				ShowFileSelector(() =>
				{
					action(null);
				});
			});
			switchButton.onClick.AddListener(() =>
			{
				currentSoundInput = this;
				SwitchToDropdown(() =>
				{
					action(null);
				});
			});
			filenameInputField.onEndEdit.AddListener((string _) =>
			{
				currentSoundInput = this;
				HideAndUpdatePopup();
				action(PlaySoundOnEdit, "sndEditorValueChange");
			});
			optionsDropdown.onValueChanged.AddListener((int _) =>
			{
				currentSoundInput = this;
				HideAndUpdatePopup();
				action(PlaySoundOnEdit, "sndListClick");
			});
		}

		public void HideAndUpdatePopup()
		{
			soundSettingsPopup.Show(show: false);
			soundSettingsPopup.UpdateData(soundData);
		}

		public void PlaySoundOnEdit()
		{
			if (base.gameObject.activeSelf && controlAttribute.playSoundOnEdit && !inFileSelector)
			{
				new SoundDataStruct(overrideSoundFilename ?? soundData.filename, offset: Convert.ToInt32(soundSettingsPopup.audioOffsetInputField.text), volume: Convert.ToInt32(soundSettingsPopup.volumeInputField.text), pitch: Convert.ToInt32(soundSettingsPopup.pitchInputField.text), pan: Convert.ToInt32(soundSettingsPopup.panInputField.text), used: soundData.used).Validated().ToSoundData().PlayImmediately(mixerPath);
			}
		}

		private void SwitchToDropdown(Action registerForChange)
		{
			EnableControls(enableOptions: true);
			string text = soundData.filename.Replace("snd", "");
			int num = Array.IndexOf(dropdownValues, text);
			optionsDropdown.value = ((num != -1) ? num : 0);
			soundData = soundData.WithNewFilename((num == -1) ? dropdownValues[0] : text);
			registerForChange();
		}

		private void SwitchToInputField(Action registerForChange)
		{
			EnableControls(enableOptions: false);
			optionsDropdown.value = optionsDropdown.options.Count - 1;
			registerForChange();
		}

		private void EnableControls(bool enableOptions)
		{
			optionsDropdown.gameObject.SetActive(enableOptions);
			filenameContainer.SetActive(!enableOptions);
			switchButton.gameObject.SetActive(!enableOptions && IsInCustom);
		}

		private void ShowFileSelector(Action registerForChange)
		{
			inFileSelector = true;
			string text = RDEditorUtils.ShowFileSelectorForAudio(RDString.Get("editor.dialog.selectSound"), -1L);
			if (!string.IsNullOrEmpty(text))
			{
				if (hasOptions)
				{
					optionsDropdown.value = optionsDropdown.options.Count - 1;
				}
				filenameInputField.text = text;
				soundSettingsPopup.messageText.text = "";
				soundWasJustSelected = true;
				justPickedFile = true;
				if (controlAttribute.updateTimeline)
				{
					propertyControl.inspectorPanel.timeline.UpdateUI(updateControls: false);
				}
				registerForChange();
				inFileSelector = false;
				StartCoroutine(CheckFileStatus(propertyControl.inspectorPanel.currentLevelEvent));
			}
			else
			{
				inFileSelector = false;
			}
		}

		public void SetShowOffset(bool showOffset)
		{
			this.showOffset = showOffset;
		}

		private void ShowSoundSettings()
		{
			soundSettingsPopup.ToggleOffset(showOffset);
			soundSettingsPopup.UpdateData(soundData);
			soundSettingsPopup.Toggle(settingsButton.GetComponent<RectTransform>());
		}

		public void UpdateUI(string[] options, bool localize = true, string[] localizedOptions = null)
		{
			dropdownValues = options;
			hasOptions = dropdownValues != null && dropdownValues.Length != 0;
			hasCustom = controlAttribute.customFile || !hasOptions;
			browseButton.gameObject.SetActive(hasCustom);
			if (hasOptions)
			{
				SetDropdownOptions(dropdownValues, localize, localizedOptions);
			}
			if (hasOptions)
			{
				int num = Array.IndexOf(dropdownValues, soundData.filename);
				num = ((num != -1 || hasCustom) ? num : 0);
				filenameInputField.text = ((num != -1) ? "" : soundData.filename);
				optionsDropdown.SetValueWithoutNotify((num != -1) ? num : (optionsDropdown.options.Count - 1));
				EnableControls(num != -1);
			}
			else
			{
				filenameInputField.text = soundData.filename;
				EnableControls(enableOptions: false);
			}
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(CheckFileStatus(propertyControl.inspectorPanel.currentLevelEvent));
			}
		}

		public void Save()
		{
			EnableControls(!UseFilename);
			if (IsInCustom && dropdownValues.Contains(soundData.filename))
			{
				filenameInputField.text = "snd" + soundData.filename;
			}
			soundData = new SoundDataStruct(UseFilename ? filenameInputField.text : dropdownValues[optionsDropdown.value], offset: GetNewValue(soundData.offset, soundSettingsPopup.audioOffsetInputField), volume: GetNewValue(soundData.volume, soundSettingsPopup.volumeInputField, soundSettingsPopup.volumeSlider), pitch: GetNewValue(soundData.pitch, soundSettingsPopup.pitchInputField, soundSettingsPopup.pitchSlider), pan: GetNewValue(soundData.pan, soundSettingsPopup.panInputField, soundSettingsPopup.panSlider), used: soundData.used).Validated();
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(CheckFileStatus(propertyControl.inspectorPanel.currentLevelEvent));
			}
		}

		private IEnumerator CheckFileStatus(LevelEvent_Base levelEvent)
		{
			bool justPicked = justPickedFile;
			justPickedFile = false;
			CoroutineWithData loadAudio = new CoroutineWithData(this, RDEditorUtils.AudioClipFromFilename(soundData.filename));
			yield return loadAudio.coroutine;
			if (controlAttribute.updateTimeline)
			{
				propertyControl.inspectorPanel.timeline.UpdateUI(updateControls: false);
			}
			RDAudioLoadResult loadResult = (RDAudioLoadResult)loadAudio.result;
			RDAudioLoadType type = loadResult.type;
			soundSettingsPopup.messageText.text = RDString.GetEnumValue(type);
			if (controlAttribute.isSong)
			{
				LevelEvent_PlaySong levelEvent_PlaySong = levelEvent as LevelEvent_PlaySong;
				string lastFilename = ((levelEvent_PlaySong != null) ? (levelEvent_PlaySong.lastSong ?? levelEvent_PlaySong.song.filename) : null);
				levelEvent_PlaySong.lastSong = soundData.filename;
				if (type == RDAudioLoadType.SuccessExternalClipLoaded && soundWasJustSelected)
				{
					string filename = soundData.filename;
					if (!(Path.GetExtension(filename).ToLowerInvariant() == ".ogg") & justPicked)
					{
						scnEditor editor = scnEditor.instance;
						string[] options = new string[2]
						{
							RDString.Get("editor.ok"),
							RDString.Get("editor.dialog.later")
						};
						int selectedConvertOption = -1;
						editor.dialog.Show(RDString.Get("editor.dialog.convertToOgg"), options, (int option) =>
						{
							selectedConvertOption = option;
						});
						yield return new WaitUntil(() => selectedConvertOption >= 0);
						if (selectedConvertOption == 0)
						{
							yield return StartCoroutine(OGGConverter.Convert(filename, loadResult.clip, (string ogg) =>
							{
								soundData = soundData.WithNewFilename(ogg);
								soundSettingsPopup.UpdateData(soundData);
								propertyControl.SetEventValue(levelEvent, soundData);
								filenameInputField.text = soundData.filename;
								if (editor.levelSettings.previewSongName == filename)
								{
									editor.levelSettings.previewSongName = ogg;
								}
								StartCoroutine(CheckFileStatus(levelEvent));
							}));
							yield break;
						}
						editor.dialog.Hide();
					}
					if (lastFilename != null && Path.GetFileNameWithoutExtension(lastFilename) != Path.GetFileNameWithoutExtension(soundData.filename))
					{
						int newOffset = 0;
						AudioClip clip = loadResult.clip;
						float[] array = new float[clip.samples];
						if (!GC.levelEditor_streamMP3Loading)
						{
							clip.GetData(array, 0);
							int num = 0;
							for (int num2 = 0; num2 < array.Length; num2 += clip.channels)
							{
								if (array[num2] != 0f)
								{
									float num3 = num2;
									float num4 = clip.channels;
									float num5 = clip.frequency;
									newOffset = Convert.ToInt16(num3 / num4 / num5 * 1000f);
									break;
								}
								num++;
							}
							soundData = soundData.WithNewOffset(newOffset);
							InputField audioOffsetInputField = soundSettingsPopup.audioOffsetInputField;
							int offset = soundData.offset;
							audioOffsetInputField.text = offset.ToString();
						}
					}
					soundSettingsPopup.UpdateData(soundData);
					propertyControl.SetEventValue(levelEvent, soundData);
				}
			}
			if (soundWasJustSelected)
			{
				soundWasJustSelected = false;
				PlaySoundOnEdit();
			}
		}

		public void ForceConversionPopup(LevelEvent_Base e, SoundDataStruct sound)
		{
			justPickedFile = true;
			soundWasJustSelected = true;
			soundData = sound;
			StartCoroutine(CheckFileStatus(e));
		}

		private void SetDropdownOptions(string[] options, bool localize = true, string[] localizedOptions = null)
		{
			dropdownValues = options;
			List<string> list = ((!localize) ? dropdownValues.ToList() : ((localizedOptions != null) ? localizedOptions.ToList() : (from object value in options
				select (!localize) ? value.ToString() : RDString.Get($"enum.SoundEffect.{value}")).ToList()));
			if (hasCustom)
			{
				dropdownValues.Append("Edit");
				list.Add(RDString.Get("editor.edit"));
			}
			optionsDropdown.ClearOptions();
			optionsDropdown.AddOptions(list);
		}

		private int GetNewValue(int current, InputField text, Slider slider)
		{
			if (currentSoundInput != null && currentSoundInput != this)
			{
				return current;
			}
			int num = Convert.ToInt32(text.text);
			int result = Mathf.RoundToInt(slider.value);
			if (current != num)
			{
				slider.value = num;
				return num;
			}
			text.text = result.ToString();
			return result;
		}

		private int GetNewValue(int current, InputField text)
		{
			if (currentSoundInput != null && currentSoundInput != this)
			{
				return current;
			}
			int num = Convert.ToInt32(text.text);
			if (current != num)
			{
				return num;
			}
			return current;
		}
	}
}
