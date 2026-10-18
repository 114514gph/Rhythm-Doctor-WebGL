using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_SetCountingSound : PropertyControl
	{
		public ToggleGroup soundToggle;

		public PropertyControl_SoundInput soundInput;

		private LevelEvent_MakeRow rowData;

		private int selectedSoundIndex;

		private SoundDataStruct[] sounds;

		private string[] options;

		private string[] localizedOptions;

		private CountingVoiceSource[] currentVoiceSources = new CountingVoiceSource[0];

		public override GameObject controlObject => soundInput.gameObject;

		private SetCountingSound controlAttribute => (SetCountingSound)base.propertyInfo.controlAttribute;

		private bool CurrentRowIsOneshot
		{
			get
			{
				if (base.editor.rowsData.Count > 0)
				{
					return rowData.rowType == RowType.Oneshot;
				}
				return false;
			}
		}

		private int HighestCountForRow
		{
			get
			{
				if (!CurrentRowIsOneshot)
				{
					return 7;
				}
				return 10;
			}
		}

		public void SetRowData(LevelEvent_MakeRow rowData)
		{
			this.rowData = rowData;
		}

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			foreach (Transform item in soundToggle.transform)
			{
				if (item.TryGetComponent<Toggle>(out var component))
				{
					ClickEvent orAddComponent = component.gameObject.GetOrAddComponent<ClickEvent>();
					orAddComponent.PointerClick = new UnityEvent();
					orAddComponent.PointerClick.AddListener(() =>
					{
						PropertyControl_SoundInput.currentSoundInput = soundInput;
						soundInput.HideAndUpdatePopup();
						action(OnToggleSelect, "sndButtonClick");
					});
				}
			}
			soundInput.AddListeners(this, action, useOptions: true);
		}

		private void OnToggleSelect()
		{
			UpdateControlsUI();
			soundInput.PlaySoundOnEdit();
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			SoundDataStruct[] array = (GetEventValue(levelEvent) as IEnumerable<SoundDataStruct>).ToArray();
			sounds = array;
			UpdateToggleVisibility();
			if (sounds.Length != HighestCountForRow)
			{
				UpdateSounds();
				selectedSoundIndex = 0;
			}
			UpdateControlsUI();
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			if (base.gameObject.activeInHierarchy && !(PropertyControl_SoundInput.currentSoundInput != soundInput))
			{
				if (sounds.Length != HighestCountForRow)
				{
					UpdateToggleVisibility();
					UpdateSounds();
					UpdateOptions();
					soundInput.soundData = sounds[selectedSoundIndex];
					soundInput.SetShowOffset(soundInput.soundData.IsExternalClip());
					soundInput.UpdateUI(options, localize: true, localizedOptions);
				}
				soundInput.Save();
				sounds[selectedSoundIndex] = soundInput.soundData;
				SetEventValue(levelEvent, sounds);
			}
		}

		private void UpdateControlsUI()
		{
			selectedSoundIndex = Mathf.Min(soundToggle.GetIndexFromActiveToggle(), HighestCountForRow - 1);
			soundToggle.SetIndex(selectedSoundIndex);
			UpdateOptions();
			soundInput.soundData = sounds[selectedSoundIndex];
			soundInput.SetShowOffset(soundInput.soundData.IsExternalClip());
			soundInput.UpdateUI(options, localize: true, localizedOptions);
		}

		private void UpdateToggleVisibility()
		{
			for (int i = 0; i < soundToggle.transform.childCount; i++)
			{
				soundToggle.transform.GetChild(i).gameObject.SetActive(i < HighestCountForRow);
			}
		}

		private void UpdateSounds()
		{
			string countingSoundFilename = GC.GetCountingSoundFilename(CurrentRowIsOneshot ? InspectorPanel_SetCountingSound.subdivisionVoiceSources[0] : InspectorPanel_SetCountingSound.countingVoiceSources[0]);
			sounds = new SoundDataStruct[HighestCountForRow];
			for (int i = 0; i < HighestCountForRow; i++)
			{
				sounds[i] = new SoundDataStruct($"{countingSoundFilename}{i + 1}").Validated();
			}
		}

		private void UpdateOptions()
		{
			CountingVoiceSource[] array = (CurrentRowIsOneshot ? InspectorPanel_SetCountingSound.subdivisionVoiceSources : InspectorPanel_SetCountingSound.countingVoiceSources);
			if (array != currentVoiceSources)
			{
				currentVoiceSources = array;
			}
			options = new string[array.Length];
			localizedOptions = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				CountingVoiceSource countingVoiceSource = array[i];
				string countingSoundFilename = GC.GetCountingSoundFilename(countingVoiceSource);
				string arg = RDString.Get($"enum.CountingVoiceSource.{countingVoiceSource}");
				int num = selectedSoundIndex + 1;
				options[i] = $"{countingSoundFilename}{num}";
				localizedOptions[i] = $"{arg} - {num}";
			}
		}
	}
}
