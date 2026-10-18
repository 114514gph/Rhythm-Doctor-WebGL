using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class SelectLevelEventButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		public Image icon;

		public Text label;

		public Text shortcut;

		public Sprite selectedSprite;

		public Button button;

		[NonSerialized]
		public LevelEventType type;

		[NonSerialized]
		public Color shortcutColor;

		[NonSerialized]
		public RectTransform rect;

		private Image background;

		private Sprite defaultSprite;

		private RectTransform iconRect;

		public bool isAdvanced;

		private bool _selected;

		public bool selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
				ShowAsSelected(_selected);
			}
		}

		private void Awake()
		{
			rect = GetComponent<RectTransform>();
			background = GetComponent<Image>();
			defaultSprite = background.sprite;
			iconRect = icon.rectTransform;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			SetGridViewLabel(on: true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			SetGridViewLabel(on: false);
		}

		public void ShowAsSelected(bool on)
		{
			background.sprite = (on ? defaultSprite : selectedSprite);
			label.color = (on ? Color.black : Color.white);
			shortcut.color = (on ? shortcutColor : Color.white);
		}

		public void SetGridMode(bool on)
		{
			label.gameObject.SetActive(!on);
			shortcut.gameObject.SetActive(!on);
			iconRect.anchorMin = iconRect.anchorMin.WithX(on ? 0.5f : 0f);
			iconRect.anchorMax = iconRect.anchorMax.WithX(on ? 0.5f : 0f);
			iconRect.AnchorPosX(on ? (-5) : 5);
			if (!on)
			{
				rect.anchorMax = rect.anchorMax.WithX(1f);
				rect.offsetMin = rect.offsetMin.WithX(2f);
				rect.offsetMax = rect.offsetMax.WithX(-7f);
			}
		}

		private void SetGridViewLabel(bool on)
		{
			string text = ((!label.gameObject.activeSelf & on) ? (label.text + " (" + shortcut.text + ")") : "");
			Text gridViewLabel = scnEditor.instance.inspectorPanelManager.gridViewLabel;
			gridViewLabel.text = text;
			RDStringToUIText.Apply(gridViewLabel);
		}
	}
}
