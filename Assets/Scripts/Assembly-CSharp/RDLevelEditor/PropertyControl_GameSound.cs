using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_GameSound : MonoBehaviour
	{
		public RectTransform rectTransform;

		public Text label;

		public Button labelButton;

		public PropertyControl_SoundInput soundInput;

		private string offString;

		public void AddListeners(PropertyControl_SetGameSound propertyControl, InspectorPanel.ChangeAction action, int index)
		{
			offString = RDString.colon + RDString.Get("editor.off");
			labelButton.gameObject.GetComponent<RDColorHoverEventTrigger>().enterColor = RDConstants.data.colorPalette[0];
			labelButton.onClick.AddListener(() =>
			{
				soundInput.soundData = soundInput.soundData.WithNewUsed(!soundInput.soundData.used);
				propertyControl.UpdateSound(soundInput.soundData, index);
				action(null, soundInput.soundData.used ? "sndButtonClick" : "sndButtonClickOn");
			});
			soundInput.AddListeners(propertyControl, action, useOptions: false);
		}

		public void UpdateUI(SoundDataStruct soundData, GameSoundType soundType, string labelText, bool isGroup, int index, int gameSoundHeight)
		{
			label.text = labelText;
			soundInput.soundData = soundData;
			string overrideSoundFilename = null;
			if (soundData.filename.IsNullOrEmpty())
			{
				GameSoundType key = (isGroup ? RDEditorConstants.gameSoundGroups[soundType][index] : soundType);
				overrideSoundFilename = RDGameSounds.defaultSounds[key].filename;
			}
			soundInput.SetOverrideSoundFilename(overrideSoundFilename);
			soundInput.SetMixerPath(GetMixerPath(soundType));
			soundInput.SetShowOffset(SoundUsesOffset(soundData, soundType));
			labelButton.gameObject.SetActive(isGroup);
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, gameSoundHeight);
			UpdateControlUsed();
			soundInput.UpdateUI(null);
		}

		public void Save()
		{
			soundInput.Save();
		}

		private void UpdateControlUsed()
		{
			soundInput.gameObject.SetActive(soundInput.soundData.used);
			label.text = label.text.Replace(offString, "") + (soundInput.soundData.used ? "" : offString);
			label.color = (soundInput.soundData.used ? Color.white : Color.gray);
			label.GetComponent<RDColorHoverEventTrigger>().exitColor = label.color;
		}

		public static bool SoundUsesOffset(SoundDataStruct sound, GameSoundType soundType)
		{
			if (!sound.filename.IsNullOrEmpty() && sound.IsExternalClip() && soundType != GameSoundType.Hand1PopSound && soundType != GameSoundType.Hand2PopSound && soundType != GameSoundType.SmallMistake)
			{
				return soundType != GameSoundType.BigMistake;
			}
			return false;
		}

		public static string GetMixerPath(GameSoundType soundType)
		{
			switch (soundType)
			{
			case GameSoundType.SmallMistake:
			case GameSoundType.BigMistake:
				return "MistakesParent";
			case GameSoundType.Hand1PopSound:
				return "PlayerOneHandPops";
			case GameSoundType.Hand2PopSound:
				return "PlayerTwoHandPops";
			case GameSoundType.HeartExplosion:
				return "HeartExplosion00";
			case GameSoundType.HeartExplosion2:
				return "HeartExplosion01";
			case GameSoundType.HeartExplosion3:
				return "HeartExplosion02";
			case GameSoundType.ClapSoundHold:
				return "PlayerOneClapHoldStart";
			case GameSoundType.ClapSoundHoldP2:
				return "PlayerTwoClapHoldStart";
			case GameSoundType.PulseSoundHold:
				return "BeatsoundsParent";
			case GameSoundType.PulseSoundHoldP2:
				return "BeatsoundsParent";
			case GameSoundType.FreezeshotSound:
			case GameSoundType.BurnshotSound:
			case GameSoundType.Skipshot:
			case GameSoundType.HoldshotSound:
				return "SpecialOneshotCues";
			default:
				return "GameplaySoundsParent";
			}
		}
	}
}
