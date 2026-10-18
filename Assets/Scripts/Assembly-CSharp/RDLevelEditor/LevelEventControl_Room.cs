using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_Room : LevelEventControl_Base
	{
		public Image icon;

		public Text label;

		public RectTransform pivot;

		private new void Awake()
		{
			base.Awake();
			levelEvent = new LevelEvent_MoveRoom();
		}

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			RectTransform component = GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat)), base.timeline.GetPosYFromRowIndex(levelEvent.y));
			icon.sprite = base.iconSprite;
			if (levelEvent is LevelEvent_MoveRoom levelEvent_MoveRoom)
			{
				icon.color = Color.white;
				RectTransform rectTransform = icon.rectTransform;
				Float2? scale = levelEvent_MoveRoom.scale;
				if (scale.HasValue)
				{
					Float2 valueOrDefault = scale.GetValueOrDefault();
					float num = Mathf.Max(valueOrDefault.x, 0.01f);
					float num2 = Mathf.Max(valueOrDefault.y, 0.01f);
					float num3 = Mathf.Max(num, num2);
					float num4 = Mathf.Min(num, num2) / num3;
					rectTransform.localScale = ((num2 > num) ? new Vector3(num4, 1f, 1f) : new Vector3(1f, num4, 1f));
				}
				else
				{
					rectTransform.localScale = Vector3.one;
				}
				float? angle = levelEvent_MoveRoom.angle;
				Vector3 eulerAngles;
				if (angle.HasValue)
				{
					float valueOrDefault2 = angle.GetValueOrDefault();
					eulerAngles = Vector3.forward * valueOrDefault2;
				}
				else
				{
					eulerAngles = Vector3.zero;
				}
				rectTransform.eulerAngles = eulerAngles;
				scale = levelEvent_MoveRoom.pivot;
				if (scale.HasValue)
				{
					Float2 valueOrDefault3 = scale.GetValueOrDefault();
					float num5 = 0.6f;
					Vector2 anchoredPosition = new Vector2(Mathf.LerpUnclamped(num5, 12f - num5, valueOrDefault3.x / 100f), Mathf.LerpUnclamped(num5, 7f - num5, valueOrDefault3.y / 100f));
					float a = Mathf.Min(component.sizeDelta.x / 14f, component.sizeDelta.y / 14f);
					anchoredPosition *= Mathf.Min(a, 1f);
					pivot.anchoredPosition = anchoredPosition;
					if (rectTransform.localScale.x == 0f || rectTransform.localScale.y == 0f)
					{
						pivot.gameObject.SetActive(value: false);
						return;
					}
					pivot.localScale = new Vector3(1f / rectTransform.localScale.x, 1f / rectTransform.localScale.y, 1f);
					pivot.gameObject.SetActive(value: true);
				}
				else
				{
					pivot.gameObject.SetActive(value: false);
				}
			}
			else
			{
				RectTransform rectTransform2 = icon.rectTransform;
				rectTransform2.anchorMin = new Vector2(0f, 0f);
				rectTransform2.anchorMax = new Vector2(1f, 1f);
				rectTransform2.offsetMin = new Vector2(2f, 2f);
				rectTransform2.offsetMax = new Vector2(-2f, -2f);
			}
		}

		private void Update()
		{
		}
	}
}
