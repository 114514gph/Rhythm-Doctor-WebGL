using System;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_Text : MakeDecorationInspectorPanel
	{
		public InputField nameInput;

		public Dropdown textFont;

		public Button browseButton;

		public Dropdown layerDropdown;

		private RDSortingLayer[] validLayers = new RDSortingLayer[3]
		{
			RDSortingLayer.Foreground,
			RDSortingLayer.Default,
			RDSortingLayer.Background
		};

		protected new virtual void Awake()
		{
			base.Awake();
			AddOnEditListeners(base.tabSection_decorations.UpdateUI, nameInput);
			AddOnEditListeners(() =>
			{
				UpdateUI(currentLevelEvent);
			}, textFont, layerDropdown);
			layerDropdown.AddOptionsWithEnumArray(validLayers);
			textFont.AddOptionsWithEnumValues<TextFont>();
			browseButton.onClick.AddListener(() =>
			{
				ShowSelectorForFont();
			});
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_Text levelEvent_Text = (LevelEvent_Text)levelEvent;
			nameInput.text = levelEvent_Text.decoName;
			textFont.SetValueForEnumArray(RDEditorUtils.GetAllValuesFromEnum<TextFont>(), levelEvent_Text.font);
			layerDropdown.value = Array.IndexOf(validLayers, levelEvent_Text.sortingLayer);
			base.UpdateUIInternal(levelEvent);
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_Text obj = (LevelEvent_Text)levelEvent;
			obj.decoName = nameInput.text;
			obj.font = GetCurrentFont();
			obj.sortingLayer = validLayers[layerDropdown.value];
			base.SaveInternal(levelEvent);
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
				(currentLevelEvent as LevelEvent_Text).font = text;
				UpdateUI(currentLevelEvent);
			}
		}
	}
}
