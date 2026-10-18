using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public abstract class TextAttributesInspectorPanel : InspectorPanel
	{
		public InputField lyrics;

		public ColorPicker color;

		public ColorPicker outlineColor;

		public ToggleGroup horizontalAlignment;

		public ToggleGroup verticalAlignment;

		public PositionPicker textPosition;

		public InputField size;

		public InputField angle;

		public Button advanceTextButton;

		public InputField fadeOutDuration;

		public ToggleGroup mode;

		public Image fillIcon;

		public Image outlineIcon;

		public Toggle narrate;

		public Dropdown narrationCategory;

		public GameObject narrationCategoryGO;

		public GameObject narrateToggleGO;

		private string oldText = "";

		protected new virtual void Awake()
		{
			base.Awake();
			AddOnEditListeners(null, lyrics, horizontalAlignment, verticalAlignment, size, fadeOutDuration);
			AddOnEditListeners(() =>
			{
				UpdateUI(currentLevelEvent);
			}, color, outlineColor, narrate, narrationCategory);
			narrationCategoryGO.SetActive(Narration.IsAvailable);
			narrateToggleGO.SetActive(Narration.IsAvailable);
			narrationCategory.AddOptionsWithEnumArray(RDEditorConstants.availableNarrationCategories);
		}

		protected virtual void Update()
		{
			if (oldText != lyrics.text)
			{
				RDEditorUtils.UpdateUIText(lyrics.textComponent, lyrics.text);
				oldText = lyrics.text;
			}
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_TextAttributesBase levelEvent_TextAttributesBase = (LevelEvent_TextAttributesBase)levelEvent;
			lyrics.text = levelEvent_TextAttributesBase.text;
			RDEditorUtils.UpdateUIText(lyrics.textComponent, lyrics.textComponent.text);
			color.colorOrPalette = levelEvent_TextAttributesBase.color;
			outlineColor.colorOrPalette = levelEvent_TextAttributesBase.outlineColor;
			size.text = levelEvent_TextAttributesBase.size.ToString();
			fadeOutDuration.text = levelEvent_TextAttributesBase.fadeOutDuration.ToString();
			mode.SetValue(levelEvent_TextAttributesBase.mode.ToString());
			narrate.isOn = levelEvent_TextAttributesBase.narrate;
			narrationCategory.value = Array.IndexOf(RDEditorConstants.availableNarrationCategories, levelEvent_TextAttributesBase.narrationCategory);
			UpdateShowNarrationCategory();
			int index = Mathf.FloorToInt((int)levelEvent_TextAttributesBase.anchor % 3);
			horizontalAlignment.SetIndex(index);
			int index2 = Mathf.FloorToInt((int)levelEvent_TextAttributesBase.anchor / 3);
			verticalAlignment.SetIndex(index2);
			fillIcon.color = levelEvent_TextAttributesBase.color.ToColor(useEditorPalette: true);
			outlineIcon.color = levelEvent_TextAttributesBase.outlineColor.ToColor(useEditorPalette: true);
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_TextAttributesBase levelEvent_TextAttributesBase = (LevelEvent_TextAttributesBase)levelEvent;
			levelEvent_TextAttributesBase.text = lyrics.text;
			levelEvent_TextAttributesBase.color = color.colorOrPalette;
			levelEvent_TextAttributesBase.outlineColor = outlineColor.colorOrPalette;
			levelEvent_TextAttributesBase.size = Convert.ToInt32(size.text);
			FloatingTextMode enumValueFromActiveToggle = mode.GetEnumValueFromActiveToggle(FloatingTextMode.FadeOut);
			levelEvent_TextAttributesBase.fadeOutDuration = ((enumValueFromActiveToggle != FloatingTextMode.Never) ? Convert.ToSingle(fadeOutDuration.text) : 0f);
			levelEvent_TextAttributesBase.mode = enumValueFromActiveToggle;
			levelEvent_TextAttributesBase.narrate = narrate.isOn;
			if (narrate.isOn)
			{
				levelEvent_TextAttributesBase.narrationCategory = RDEditorConstants.availableNarrationCategories[narrationCategory.value];
			}
			int indexFromActiveToggle = horizontalAlignment.GetIndexFromActiveToggle();
			TextAnchor anchor = (TextAnchor)(verticalAlignment.GetIndexFromActiveToggle() * 3 + indexFromActiveToggle);
			levelEvent_TextAttributesBase.anchor = anchor;
		}

		public void UpdateShowNarrationCategory()
		{
			narrationCategoryGO.SetActive(narrate.isOn && Narration.IsAvailable);
		}

		public int CountOcurrencesInString(char character, string s, int endIndex)
		{
			int num = 0;
			int num2 = 0;
			foreach (char c in s)
			{
				if (num2 > endIndex)
				{
					return num;
				}
				if (c == character)
				{
					num++;
				}
				num2++;
			}
			return num;
		}
	}
}
