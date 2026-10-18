using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class SoundSettingsPopup : RDEditorBase
	{
		[NonSerialized]
		public List<AudioSource> audioSources = new List<AudioSource>();

		public Slider volumeSlider;

		public Slider pitchSlider;

		public Slider panSlider;

		public InputField volumeInputField;

		public InputField pitchInputField;

		public InputField panInputField;

		public InputField audioOffsetInputField;

		public RectTransform panel;

		public RectTransform messageRect;

		public Text messageText;

		public ContentSizeFitter messageContentSizeFitter;

		private RectTransform rectTransform;

		private Type currentType;

		private int currentLevelEventControlID;

		private string successExternalMessage;

		private string successInternalMessage;

		public PropertyControl_SoundInput soundInput;

		[NonSerialized]
		public Tween toggleTween;

		public GameObject offsetContainer;

		private const float msgYOffsetOn = -78f;

		private const float msgYOffsetOff = -60f;

		public RectTransform offsetRT;

		public int GetSelectedLevelEventControlID
		{
			get
			{
				if (!(base.editor.selectedControl == null))
				{
					return base.editor.selectedControl.GetInstanceID();
				}
				return base.editor.selectedRowIndex;
			}
		}

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			successExternalMessage = RDString.GetEnumValue(RDAudioLoadType.SuccessExternalClipLoaded);
			successInternalMessage = RDString.GetEnumValue(RDAudioLoadType.SuccessInternalClipLoaded);
		}

		private void Update()
		{
			if (Input.GetMouseButtonUp(0))
			{
				Vector2 point = panel.InverseTransformPoint(Input.mousePosition);
				if (!panel.rect.Contains(point))
				{
					InspectorPanel current = base.editor.inspectorPanelManager.GetCurrent();
					int getSelectedLevelEventControlID = GetSelectedLevelEventControlID;
					bool num = current == null;
					bool flag = currentType != current?.GetType();
					bool flag2 = currentLevelEventControlID != getSelectedLevelEventControlID;
					if ((num | flag) || !flag2)
					{
						Show(show: false);
					}
					else
					{
						currentLevelEventControlID = getSelectedLevelEventControlID;
					}
				}
			}
			if (audioSources.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < audioSources.Count; i++)
			{
				if (audioSources[i] == null)
				{
					audioSources.RemoveAt(i);
					i--;
				}
			}
		}

		public void Toggle(RectTransform button)
		{
			Show(!base.gameObject.activeSelf, button);
		}

		public void Show(bool show, RectTransform button = null)
		{
			if ((!base.gameObject.activeSelf && !show) || (toggleTween != null && toggleTween.active))
			{
				return;
			}
			if (show)
			{
				base.gameObject.SetActive(value: true);
				LevelEditorPlaySound("sndEditorPopupSmall", "LevelEditorInspectorPanel");
				InspectorPanel current = base.editor.inspectorPanelManager.GetCurrent();
				currentType = current.GetType();
				currentLevelEventControlID = GetSelectedLevelEventControlID;
			}
			else if (audioSources.Count > 0)
			{
				for (int i = 0; i < audioSources.Count; i++)
				{
					if (audioSources[i] != null)
					{
						audioSources[i].Stop();
					}
				}
				audioSources = new List<AudioSource>();
			}
			else
			{
				LevelEditorPlaySound("sndEditorPopdownSmall", "LevelEditorInspectorPanel");
			}
			rectTransform.DOKill();
			rectTransform.localScale = (show ? Vector3.zero : Vector3.one);
			Vector3 endValue = (show ? Vector3.one : Vector3.zero);
			float duration = (show ? base.gc.roomsSelectionPopup_showAnimDuration : base.gc.roomsSelectionPopup_hideAnimDuration);
			Ease ease = (show ? base.gc.roomsSelectionPopup_showAnimEase : base.gc.roomsSelectionPopup_hideAnimEase);
			toggleTween = rectTransform.DOScale(endValue, duration).SetId("noKillOnReload").SetEase(ease)
				.SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.OnComplete(() =>
				{
					if (!show)
					{
						base.gameObject.SetActive(value: false);
					}
				});
			if (show)
			{
				bool active = messageText.text != string.Empty && messageText.text != successExternalMessage && messageText.text != successInternalMessage;
				messageText.gameObject.SetActive(active);
				messageContentSizeFitter.SetLayoutVertical();
				messageText.font = RDString.GetAppropiateFontForString(messageText.text);
				SetPanelHeight();
				if (button != null)
				{
					float x = (button.sizeDelta.x + 1f) * (float)Screen.width / base.editor.canvas.GetComponent<CanvasScaler>().referenceResolution.x;
					rectTransform.position = button.position.AddX(x);
				}
				Vector2 anchoredPosition = rectTransform.anchoredPosition;
				float y = 0f;
				float num = (panel.sizeDelta.y - rectTransform.sizeDelta.y - 6f) / 2f;
				if (anchoredPosition.y < -130f)
				{
					y = Mathf.Min(-130f - anchoredPosition.y, num);
				}
				else if (anchoredPosition.y > -44f)
				{
					float a = -44f - anchoredPosition.y;
					num *= -1f;
					y = Mathf.Max(a, num);
				}
				panel.AnchorPosY(y);
			}
		}

		public void UpdateData(SoundDataStruct soundData)
		{
			volumeSlider.value = soundData.volume;
			pitchSlider.value = soundData.pitch;
			panSlider.value = soundData.pan;
			InputField inputField = volumeInputField;
			int volume = soundData.volume;
			inputField.text = volume.ToString();
			InputField inputField2 = pitchInputField;
			volume = soundData.pitch;
			inputField2.text = volume.ToString();
			InputField inputField3 = panInputField;
			volume = soundData.pan;
			inputField3.text = volume.ToString();
			InputField inputField4 = audioOffsetInputField;
			volume = soundData.offset;
			inputField4.text = volume.ToString();
		}

		public void UpdateInputFieldText()
		{
			volumeInputField.text = volumeSlider.value.ToString();
			pitchInputField.text = pitchSlider.value.ToString();
			panInputField.text = panSlider.value.ToString();
		}

		public void ToggleOffset(bool show)
		{
			offsetContainer.SetActive(show);
			float y = (show ? (-78f) : (-60f));
			messageRect.anchoredPosition = messageRect.anchoredPosition.WithY(y);
			SetPanelHeight();
		}

		public void SetPanelHeight()
		{
			bool activeSelf = messageText.gameObject.activeSelf;
			bool activeSelf2 = offsetContainer.activeSelf;
			float num = 62f;
			if (activeSelf2)
			{
				num += offsetRT.sizeDelta.y + 4f;
			}
			if (activeSelf)
			{
				num += messageRect.sizeDelta.y + 4f;
			}
			panel.sizeDelta = panel.sizeDelta.WithY(num);
		}
	}
}
