using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_FloatingText : TextAttributesInspectorPanel
	{
		public Dropdown textFont;

		public Button browseButton;

		public ToggleGroup showChildren;

		protected new virtual void Awake()
		{
			base.Awake();
			AddOnEditListeners(null, showChildren, mode);
			AddOnEditListeners(() =>
			{
				UpdateUI(currentLevelEvent);
			}, textFont);
			AddOnEditListeners(EnforcePosition, textPosition);
			AddOnEditListeners(EnforceAngle, angle);
			advanceTextButton.onClick.AddListener(() =>
			{
				UpdateAdvanceTextEvents();
			});
			browseButton.onClick.AddListener(() =>
			{
				ShowSelectorForFont();
			});
			textFont.AddOptionsWithEnumValues<TextFont>();
			GetComponent<RectTransform>().SizeDeltaY(Narration.IsAvailable ? 380f : 325f);
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_FloatingText levelEvent_FloatingText = (LevelEvent_FloatingText)levelEvent;
			textFont.SetValueForEnumArray(RDEditorUtils.GetAllValuesFromEnum<TextFont>(), levelEvent_FloatingText.font);
			showChildren.SetValue(levelEvent_FloatingText.showChildren ? "on" : "off");
			Float2? @float = levelEvent_FloatingText.textPosition;
			if (@float.HasValue)
			{
				Float2 valueOrDefault = @float.GetValueOrDefault();
				textPosition.percent = valueOrDefault;
			}
			else
			{
				textPosition.percent = Float2.zero;
			}
			angle.text = (levelEvent_FloatingText.angle.HasValue ? levelEvent_FloatingText.angle.ToString() : "0");
			base.UpdateUIInternal(levelEvent);
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_FloatingText levelEvent_FloatingText = (LevelEvent_FloatingText)levelEvent;
			levelEvent_FloatingText.font = GetCurrentFont();
			levelEvent_FloatingText.showChildren = showChildren.GetValueFromActiveToggle() == "on";
			levelEvent_FloatingText.textPosition = textPosition.percent;
			levelEvent_FloatingText.angle = ((!angle.text.IsNullOrEmpty()) ? new float?(Convert.ToSingle(angle.text)) : ((float?)null));
			UpdateShowChildren(levelEvent_FloatingText);
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

		public void UpdateShowChildren(LevelEvent_FloatingText textEvent)
		{
			foreach (LevelEventControl_Base eventControls_action in base.editor.eventControls_actions)
			{
				if (eventControls_action.levelEvent is LevelEvent_AdvanceText && ((LevelEvent_AdvanceText)eventControls_action.levelEvent).id == textEvent.id)
				{
					eventControls_action.UpdateUI();
				}
			}
		}

		public void UpdateAdvanceTextEvents()
		{
			using (new SaveStateScope())
			{
				int num = 0;
				LevelEvent_FloatingText levelEvent_FloatingText = currentLevelEvent as LevelEvent_FloatingText;
				int num2 = levelEvent_FloatingText.splitLyrics.Length - 1;
				levelEvent_FloatingText.showChildren = true;
				UpdateUI(levelEvent_FloatingText);
				UpdateShowChildren(levelEvent_FloatingText);
				foreach (LevelEventControl_Base eventControls_action in base.editor.eventControls_actions)
				{
					if (eventControls_action.levelEvent is LevelEvent_AdvanceText && ((LevelEvent_AdvanceText)eventControls_action.levelEvent).id == levelEvent_FloatingText.id)
					{
						num++;
					}
				}
				int num3 = num2 - num;
				if (num3 > 0)
				{
					for (int i = 0; i < num3; i++)
					{
						LevelEvent_AdvanceText levelEvent_AdvanceText = new LevelEvent_AdvanceText();
						levelEvent_AdvanceText.y = currentLevelEvent.y;
						levelEvent_AdvanceText.id = ((LevelEvent_FloatingText)currentLevelEvent).id;
						BarAndBeat barAndBeat = currentLevelEvent.barAndBeat;
						barAndBeat.beat += i + 1;
						levelEvent_AdvanceText.barAndBeat = base.timeline.FixBarAndBeat(barAndBeat);
						base.editor.CreateEventControl(levelEvent_AdvanceText, Tab.Actions);
					}
				}
			}
		}

		public void ShowSelectorForFont()
		{
			string text = RDEditorUtils.ShowFileSelectorForFont(RDString.Get("editor.dialog.selectFont"), -1L);
			if (!string.IsNullOrEmpty(text))
			{
				(currentLevelEvent as LevelEvent_FloatingText).font = text;
				UpdateUI(currentLevelEvent);
			}
		}

		private void EnforcePosition()
		{
			if (textPosition.x.text == "")
			{
				textPosition.x.text = "0";
			}
			if (textPosition.y.text == "")
			{
				textPosition.y.text = "0";
			}
		}

		private void EnforceAngle()
		{
			if (angle.text == "")
			{
				angle.text = "0";
			}
		}

		private void DEBUG_FormatAllFloatingTextsToKeyboard(bool andReplace = false)
		{
			string fileName = Path.GetFileName(Path.GetDirectoryName(base.editor.openedFilePath));
			List<LevelEvent_FloatingText> list = base.editor.eventControls.Select((LevelEventControl_Base x) => x.levelEvent).OfType<LevelEvent_FloatingText>().ToList();
			list.Sort((LevelEvent_FloatingText left, LevelEvent_FloatingText right) => left.bar1000timesbeat.CompareTo(right.bar1000timesbeat));
			StringWriter stringWriter = new StringWriter();
			using (andReplace ? new SaveStateScope() : null)
			{
				int num = 1;
				foreach (LevelEvent_FloatingText item in list)
				{
					if (item.active)
					{
						string text = $"level.{fileName}.lyrics{num}";
						stringWriter.WriteLine(text + "\t\"" + item.text + "\"");
						if (andReplace)
						{
							item.text = "[[" + text + "]]";
						}
						num++;
					}
				}
			}
			GUIUtility.systemCopyBuffer = stringWriter.ToString();
			stringWriter.Dispose();
			Show(currentEventControl);
			base.editor.LevelEditorPlaySound("sndEditorEventCreateMultiple");
		}
	}
}
