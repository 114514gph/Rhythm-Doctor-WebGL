using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_SetFont : InspectorPanel
	{
		public Dropdown textFont;

		public Button browseButton;

		public GameObject styleSettings;

		public GameObject defaultExplanation;

		public RectTransform wrappingWidthRect;

		public RectTransform extraSettings;

		public Toggle bold;

		public Toggle italic;

		public Toggle underline;

		public InputField wrappingWidth;

		public InputField characterSpacing;

		public InputField outlineWidth;

		protected new virtual void Awake()
		{
			base.Awake();
			AddOnEditListeners(null, bold, italic, underline, characterSpacing);
			AddOnEditListeners(() =>
			{
				UpdateUI(currentLevelEvent);
			}, textFont);
			AddOnEditListeners(EnforceWrappingWidth, wrappingWidth);
			AddOnEditListeners(EnforceOutlineWidth, outlineWidth);
			browseButton.onClick.AddListener(() =>
			{
				ShowSelectorForFont();
			});
			textFont.AddOptionsWithEnumValues<TextFont>();
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetFont levelEvent_SetFont = (LevelEvent_SetFont)levelEvent;
			textFont.SetValueForEnumArray(RDEditorUtils.GetAllValuesFromEnum<TextFont>(), levelEvent_SetFont.font);
			bold.isOn = levelEvent_SetFont.bold;
			italic.isOn = levelEvent_SetFont.italic;
			underline.isOn = levelEvent_SetFont.underline;
			wrappingWidth.text = levelEvent_SetFont.wrappingWidth.ToString();
			characterSpacing.text = levelEvent_SetFont.characterSpacing.ToString();
			outlineWidth.text = levelEvent_SetFont.outlineWidth.ToString();
			styleSettings.SetActive(levelEvent_SetFont.SupportsFontStyles());
			extraSettings.gameObject.SetActive(levelEvent_SetFont.ShowExtraSettings());
			defaultExplanation.SetActive(levelEvent_SetFont.IsDefault());
			AdjustHeightPositions(levelEvent_SetFont);
		}

		private void AdjustHeightPositions(LevelEvent_SetFont data)
		{
			float num = -57f;
			float num2 = -75f;
			if (!data.SupportsFontStyles())
			{
				num += 18f;
				num2 += 18f;
			}
			wrappingWidthRect.localPosition = wrappingWidthRect.localPosition.WithY(num);
			extraSettings.offsetMax = extraSettings.offsetMax.WithY(num2);
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetFont obj = (LevelEvent_SetFont)levelEvent;
			obj.font = GetCurrentFont();
			obj.bold = bold.isOn;
			obj.italic = italic.isOn;
			obj.underline = underline.isOn;
			obj.wrappingWidth = Mathf.Max(0f, Convert.ToSingle(wrappingWidth.text));
			obj.characterSpacing = Convert.ToSingle(characterSpacing.text);
			obj.outlineWidth = Mathf.Clamp(Convert.ToSingle(outlineWidth.text), 0f, 1f);
		}

		private string GetCurrentFont()
		{
			TextFont[] allValuesFromEnum = RDEditorUtils.GetAllValuesFromEnum<TextFont>();
			if (textFont.value < allValuesFromEnum.Length)
			{
				return allValuesFromEnum[textFont.value].ToString();
			}
			return textFont.options[textFont.options.Count - 1].text;
		}

		public void ShowSelectorForFont()
		{
			string text = RDEditorUtils.ShowFileSelectorForFont(RDString.Get("editor.dialog.selectFont"), -1L);
			if (!string.IsNullOrEmpty(text))
			{
				(currentLevelEvent as LevelEvent_SetFont).font = text;
				UpdateUI(currentLevelEvent);
			}
		}

		public void EnforceOutlineWidth()
		{
			outlineWidth.text = Mathf.Clamp(Convert.ToSingle(outlineWidth.text), 0f, 1f).ToString();
		}

		public void EnforceWrappingWidth()
		{
			wrappingWidth.text = Mathf.Max(0f, Convert.ToSingle(wrappingWidth.text)).ToString();
		}
	}
}
