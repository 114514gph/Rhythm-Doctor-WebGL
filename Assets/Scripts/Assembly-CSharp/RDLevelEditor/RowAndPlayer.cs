using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class RowAndPlayer : MonoBehaviour
	{
		public Text text;

		public ToggleGroup toggleGroup;

		public CharacterPicker cpuCharacterPicker;

		public Image cpuMarkerImage;

		public Sprite cpuNoChangeSprite;

		public RectTransform cpuLabelTrans;

		public void UpdateCPUMarkerImage()
		{
			Dictionary<string, Sprite> markersDict = RowEntity.markersDict;
			string key = cpuCharacterPicker.character.ToString() + "1";
			if (markersDict.ContainsKey(key))
			{
				cpuMarkerImage.sprite = markersDict[key];
				cpuMarkerImage.rectTransform.sizeDelta = cpuMarkerImage.sprite.rect.size;
			}
			else if (cpuCharacterPicker.character == Character.None)
			{
				cpuMarkerImage.sprite = cpuNoChangeSprite;
				cpuMarkerImage.rectTransform.sizeDelta = cpuMarkerImage.sprite.rect.size;
			}
			cpuLabelTrans.offsetMax = cpuLabelTrans.offsetMax.WithX((cpuCharacterPicker.character == Character.BlankCPU) ? 0f : (-21f));
		}

		public void ShowCharacterPickerIfActive()
		{
			if (toggleGroup.AnyTogglesOn())
			{
				cpuCharacterPicker.ShowCharacterPickerPopup();
			}
		}
	}
}
