using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_SetText : TextAttributesInspectorPanel
	{
		public Button positionButton;

		public Text positionLabel;

		public GameObject positionFields;

		public Button angleButton;

		public Image angleImage;

		public GameObject angleField;

		public GameObject fadeOutDurationGO;

		protected new virtual void Awake()
		{
			base.Awake();
			AddOnEditListeners(null, textPosition, angle);
			AddOnEditListeners(UpdateDurationHeight, mode);
			advanceTextButton.onClick.AddListener(() =>
			{
				CreateAdvanceTextDecorationEvents();
			});
			positionButton.onClick.AddListener(() =>
			{
				TogglePosition();
			});
			angleButton.onClick.AddListener(() =>
			{
				ToggleAngle();
			});
			GetComponent<RectTransform>().SizeDeltaY(Narration.IsAvailable ? 338f : 283f);
		}

		private void TogglePosition()
		{
			LevelEvent_SetText levelEvent_SetText = (LevelEvent_SetText)currentLevelEvent;
			bool hasValue = levelEvent_SetText.textPosition.HasValue;
			Float2? @float = (hasValue ? ((Float2?)null) : new Float2?(new Float2(50f, 50f)));
			if (!hasValue)
			{
				textPosition.percent = new Float2(50f, 50f);
			}
			levelEvent_SetText.textPosition = @float;
			scrConductor.PlayImmediately(hasValue ? "sndButtonClick" : "sndButtonClickOn", 1f, RDUtils.GetMixerGroup("LevelEditorInspectorPanel"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			SetPositionUsed(!hasValue);
			UpdateUI(levelEvent_SetText);
		}

		private void SetPositionUsed(bool used)
		{
			positionFields.SetActive(used);
			string text = RDString.colon + RDString.Get("editor.off");
			positionLabel.text = positionLabel.text.Replace(text, "") + (used ? "" : text);
			positionLabel.color = (used ? Color.white : Color.gray);
			positionLabel.GetComponent<RDColorHoverEventTrigger>().exitColor = positionLabel.color;
		}

		private void ToggleAngle()
		{
			LevelEvent_SetText levelEvent_SetText = (LevelEvent_SetText)currentLevelEvent;
			bool hasValue = levelEvent_SetText.angle.HasValue;
			float? num = (hasValue ? ((float?)null) : new float?(0f));
			if (!hasValue)
			{
				angle.text = "0";
			}
			levelEvent_SetText.angle = num;
			scrConductor.PlayImmediately(hasValue ? "sndButtonClick" : "sndButtonClickOn", 1f, RDUtils.GetMixerGroup("LevelEditorInspectorPanel"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			SetAngleUsed(!hasValue);
			UpdateUI(levelEvent_SetText);
		}

		private void SetAngleUsed(bool used)
		{
			angleField.SetActive(used);
			angleImage.color = (used ? Color.white : Color.gray);
			angleImage.GetComponent<RDColorHoverEventTrigger>().exitColor = angleImage.color;
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetText levelEvent_SetText = (LevelEvent_SetText)levelEvent;
			if (levelEvent_SetText.textPosition.HasValue)
			{
				Float2? @float = levelEvent_SetText.textPosition;
				if (@float.HasValue)
				{
					Float2 valueOrDefault = @float.GetValueOrDefault();
					textPosition.percent = valueOrDefault;
					SetPositionUsed(used: true);
					goto IL_005b;
				}
			}
			textPosition.percent = Float2.empty;
			SetPositionUsed(used: false);
			goto IL_005b;
			IL_005b:
			angle.text = (levelEvent_SetText.angle.HasValue ? levelEvent_SetText.angle.ToString() : "");
			SetAngleUsed(levelEvent_SetText.angle.HasValue);
			base.UpdateUIInternal(levelEvent);
			UpdateDurationHeight();
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetText obj = (LevelEvent_SetText)levelEvent;
			obj.textPosition = (positionFields.activeSelf ? new Float2?(textPosition.percent) : ((Float2?)null));
			obj.angle = ((angleField.activeSelf && !angle.text.IsNullOrEmpty()) ? new float?(Convert.ToSingle(angle.text)) : ((float?)null));
			base.SaveInternal(levelEvent);
		}

		public void CreateAdvanceTextDecorationEvents()
		{
			using (new SaveStateScope())
			{
				LevelEvent_SetText levelEvent_SetText = currentLevelEvent as LevelEvent_SetText;
				UpdateUI(levelEvent_SetText);
				int num = levelEvent_SetText.splitLyrics.Length - 1;
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						LevelEvent_AdvanceTextDecoration levelEvent_AdvanceTextDecoration = new LevelEvent_AdvanceTextDecoration();
						levelEvent_AdvanceTextDecoration.y = currentLevelEvent.y;
						levelEvent_AdvanceTextDecoration.row = currentLevelEvent.row;
						levelEvent_AdvanceTextDecoration.target = currentLevelEvent.target;
						levelEvent_AdvanceTextDecoration.fadeOutDuration = levelEvent_SetText.fadeOutDuration;
						BarAndBeat barAndBeat = currentLevelEvent.barAndBeat;
						barAndBeat.beat += i + 1;
						levelEvent_AdvanceTextDecoration.barAndBeat = base.timeline.FixBarAndBeat(barAndBeat);
						base.editor.CreateEventControl(levelEvent_AdvanceTextDecoration, Tab.Decorations);
					}
				}
			}
		}

		private void UpdateDurationHeight()
		{
			if (mode.GetEnumValueFromActiveToggle(FloatingTextMode.FadeOut) != FloatingTextMode.Never)
			{
				fadeOutDurationGO.SetActive(value: true);
				narrationCategoryGO.transform.localPosition = narrationCategoryGO.transform.localPosition.WithY(-308f);
				narrateToggleGO.transform.localPosition = narrateToggleGO.transform.localPosition.WithY(-295f);
			}
			else
			{
				fadeOutDurationGO.SetActive(value: false);
				narrationCategoryGO.transform.localPosition = narrationCategoryGO.transform.localPosition.WithY(-263f);
				narrateToggleGO.transform.localPosition = narrateToggleGO.transform.localPosition.WithY(-250f);
			}
		}
	}
}
