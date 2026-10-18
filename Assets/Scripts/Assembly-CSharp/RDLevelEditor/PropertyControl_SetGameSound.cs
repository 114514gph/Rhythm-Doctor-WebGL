using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_SetGameSound : PropertyControl
	{
		public RectTransform gameSoundsParent;

		public Button setForAll;

		public PropertyControl_GameSound[] gameSounds;

		private const int inputsToInit = 8;

		private GameSoundType soundType;

		private SoundDataStruct[] sounds;

		public override GameObject controlObject => gameSoundsParent.gameObject;

		private SetGameSoundAttribute controlAttribute => (SetGameSoundAttribute)base.propertyInfo.controlAttribute;

		private Dictionary<GameSoundType, GameSoundType[]> gameSoundGroups => RDEditorConstants.gameSoundGroups;

		private SoundSettingsPopup soundSettingsPopup => base.editor.soundSettingsPopup;

		private bool SoundTypeIsGroup(GameSoundType t)
		{
			return RDEditorUtils.SoundTypeIsGroup(t);
		}

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			PropertyControl_GameSound propertyControl_GameSound = gameSounds[0];
			gameSounds = new PropertyControl_GameSound[8];
			for (int i = 0; i < gameSounds.Length; i++)
			{
				PropertyControl_GameSound propertyControl_GameSound2 = ((i == 0) ? propertyControl_GameSound : UnityEngine.Object.Instantiate(RDConstants.data.gameSoundControlPrefab, gameSoundsParent).GetComponent<PropertyControl_GameSound>());
				propertyControl_GameSound2.gameObject.name = $"GameSound{i}";
				propertyControl_GameSound2.AddListeners(this, action, i);
				gameSounds[i] = propertyControl_GameSound2;
			}
			RDEventTrigger volume = soundSettingsPopup.volumeSlider.GetOrAddComponent<RDEventTrigger>();
			RDEventTrigger rDEventTrigger = volume;
			rDEventTrigger.onEndDrag = (RDPointerEventData)Delegate.Combine(rDEventTrigger.onEndDrag, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != volume.lastEventFrame && PropertyControl_SoundInput.currentSoundInput == null)
				{
					UpdateAllSettings(action);
				}
			}));
			RDEventTrigger rDEventTrigger2 = volume;
			rDEventTrigger2.onClick = (RDPointerEventData)Delegate.Combine(rDEventTrigger2.onClick, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != volume.lastEventFrame && PropertyControl_SoundInput.currentSoundInput == null)
				{
					UpdateAllSettings(action, "sndSliderGrab");
				}
			}));
			RDEventTrigger pitch = soundSettingsPopup.pitchSlider.GetOrAddComponent<RDEventTrigger>();
			RDEventTrigger rDEventTrigger3 = pitch;
			rDEventTrigger3.onEndDrag = (RDPointerEventData)Delegate.Combine(rDEventTrigger3.onEndDrag, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != pitch.lastEventFrame && PropertyControl_SoundInput.currentSoundInput == null)
				{
					UpdateAllSettings(action);
				}
			}));
			RDEventTrigger rDEventTrigger4 = pitch;
			rDEventTrigger4.onClick = (RDPointerEventData)Delegate.Combine(rDEventTrigger4.onClick, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != pitch.lastEventFrame && PropertyControl_SoundInput.currentSoundInput == null)
				{
					UpdateAllSettings(action, "sndSliderGrab");
				}
			}));
			RDEventTrigger pan = soundSettingsPopup.panSlider.GetOrAddComponent<RDEventTrigger>();
			RDEventTrigger rDEventTrigger5 = pan;
			rDEventTrigger5.onEndDrag = (RDPointerEventData)Delegate.Combine(rDEventTrigger5.onEndDrag, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != pan.lastEventFrame && PropertyControl_SoundInput.currentSoundInput == null)
				{
					UpdateAllSettings(action);
				}
			}));
			RDEventTrigger rDEventTrigger6 = pan;
			rDEventTrigger6.onClick = (RDPointerEventData)Delegate.Combine(rDEventTrigger6.onClick, (RDPointerEventData)((PointerEventData _) =>
			{
				if (Time.frameCount != pan.lastEventFrame && PropertyControl_SoundInput.currentSoundInput == null)
				{
					UpdateAllSettings(action, "sndSliderGrab");
				}
			}));
			soundSettingsPopup.volumeInputField.onEndEdit.AddListener((string _) =>
			{
				action(null, "sndEditorValueChange");
			});
			soundSettingsPopup.pitchInputField.onEndEdit.AddListener((string _) =>
			{
				action(null, "sndEditorValueChange");
			});
			soundSettingsPopup.panInputField.onEndEdit.AddListener((string _) =>
			{
				action(null, "sndEditorValueChange");
			});
			soundSettingsPopup.audioOffsetInputField.onEndEdit.AddListener((string _) =>
			{
				action(null, "sndEditorValueChange");
			});
			setForAll.onClick.AddListener(() =>
			{
				PropertyControl_SoundInput.currentSoundInput = null;
				ShowSoundSettings();
			});
		}

		private void ShowSoundSettings()
		{
			SoundData soundData = new SoundData("");
			SoundDataStruct soundDataStruct = sounds[0];
			bool flag = true;
			bool flag2 = true;
			bool flag3 = true;
			for (int i = 1; i < sounds.Length; i++)
			{
				SoundDataStruct soundDataStruct2 = sounds[i];
				if (flag && soundDataStruct2.volume != soundDataStruct.volume)
				{
					flag = false;
				}
				if (flag2 && soundDataStruct2.pitch != soundDataStruct.pitch)
				{
					flag2 = false;
				}
				if (flag3 && soundDataStruct2.pan != soundDataStruct.pan)
				{
					flag3 = false;
				}
			}
			if (flag)
			{
				soundData.volume = soundDataStruct.volume;
			}
			if (flag2)
			{
				soundData.pitch = soundDataStruct.pitch;
			}
			if (flag3)
			{
				soundData.pan = soundDataStruct.pan;
			}
			base.editor.soundSettingsPopup.ToggleOffset(show: false);
			soundSettingsPopup.messageText.text = "";
			soundSettingsPopup.UpdateData(SoundDataStruct.FromSoundData(soundData));
			soundSettingsPopup.Toggle(setForAll.GetComponent<RectTransform>());
		}

		private void UpdateAllSettings(InspectorPanel.ChangeAction action, string soundString = null)
		{
			if (!(PropertyControl_SoundInput.currentSoundInput != null))
			{
				for (int i = 0; i < sounds.Length; i++)
				{
					SoundDataStruct soundDataStruct = sounds[i];
					sounds[i] = new SoundDataStruct(soundDataStruct.filename, offset: soundDataStruct.offset, volume: GetNewValue(soundDataStruct.volume, soundSettingsPopup.volumeInputField, soundSettingsPopup.volumeSlider), pitch: GetNewValue(soundDataStruct.pitch, soundSettingsPopup.pitchInputField, soundSettingsPopup.pitchSlider), pan: GetNewValue(soundDataStruct.pan, soundSettingsPopup.panInputField, soundSettingsPopup.panSlider), used: soundDataStruct.used).Validated();
				}
				action(null, soundString);
			}
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			sounds = (GetEventValue(levelEvent) as IEnumerable<SoundDataStruct>).ToArray();
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			List<SoundDataStruct> list = new List<SoundDataStruct>();
			for (int i = 0; i < gameSounds.Length; i++)
			{
				PropertyControl_GameSound propertyControl_GameSound = gameSounds[i];
				if (propertyControl_GameSound.gameObject.activeSelf)
				{
					propertyControl_GameSound.Save();
					list.Add(propertyControl_GameSound.soundInput.soundData);
				}
			}
			sounds = list.ToArray();
			SetEventValue(levelEvent, list.ToArray());
		}

		private int GetNewValue(int current, InputField text, Slider slider)
		{
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

		public void UpdateSound(SoundDataStruct sound, int index)
		{
			sounds[index] = sound;
			string overrideSoundFilename = null;
			if (sound.filename.IsNullOrEmpty())
			{
				GameSoundType key = ((sounds.Length > 1) ? gameSoundGroups[soundType][index] : soundType);
				overrideSoundFilename = RDGameSounds.defaultSounds[key].filename;
			}
			gameSounds[index].soundInput.SetOverrideSoundFilename(overrideSoundFilename);
			gameSounds[index].soundInput.SetMixerPath(PropertyControl_GameSound.GetMixerPath(soundType));
			gameSounds[index].soundInput.SetShowOffset(PropertyControl_GameSound.SoundUsesOffset(sound, soundType));
		}

		public void SetSoundType(GameSoundType soundType, bool replaceValues = true)
		{
			if (this.soundType != soundType)
			{
				this.soundType = soundType;
				if (replaceValues)
				{
					int num = ((!SoundTypeIsGroup(soundType)) ? 1 : gameSoundGroups[soundType].Length);
					sounds = new SoundDataStruct[num];
					Array.Fill(sounds, new SoundDataStruct(""));
				}
			}
			UpdateGameSoundsUI();
		}

		private void UpdateGameSoundsUI()
		{
			PropertyControl_GameSound[] array = gameSounds;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
			bool flag = sounds.Length > 1;
			int num = 16;
			int num2 = ((!flag) ? num : 0);
			setForAll.gameObject.SetActive(flag);
			for (int j = 0; j < sounds.Length; j++)
			{
				SoundDataStruct soundData = sounds[j];
				PropertyControl_GameSound propertyControl_GameSound = gameSounds[j];
				propertyControl_GameSound.gameObject.SetActive(value: true);
				string labelText = "";
				if (flag)
				{
					num = (soundData.used ? 30 : 14);
					num2 += num;
					num2 += ((j > 0) ? 4 : 0);
					GameSoundType key = soundType switch
					{
						GameSoundType.PulseSoundHoldP2 => GameSoundType.PulseSoundHold, 
						GameSoundType.ClapSoundHoldP2 => GameSoundType.ClapSoundHold, 
						_ => soundType, 
					};
					labelText = RDString.GetEnumValue(gameSoundGroups[key][j]);
				}
				propertyControl_GameSound.UpdateUI(soundData, soundType, labelText, flag, j, num);
			}
			SetPropertyControlHeight(num2, updateControlObject: true, updateProperty: true);
		}
	}
}
