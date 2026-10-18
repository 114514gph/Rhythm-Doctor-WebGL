using DG.Tweening;
using UnityEngine;

namespace RDLevelEditor
{
	public class RDLetterMovementPickerPopup : RDEditorBase
	{
		public GameObject buttonPrefab;

		public Transform letterMovementContainer;

		public RectTransform panel;

		private LetterMovementButton[] letterMovementButtons;

		private LetterMovementPicker letterMovementPicker;

		private void ClearButtons()
		{
			if (letterMovementButtons != null)
			{
				LetterMovementButton[] array = letterMovementButtons;
				for (int i = 0; i < array.Length; i++)
				{
					Object.Destroy(array[i].gameObject);
				}
			}
			letterMovementButtons = new LetterMovementButton[0];
		}

		public void ShowLetterMovementTypes(LetterMovementPicker letterMovementPicker, LetterMovementType[] movementTypes)
		{
			ShowWindow();
			ClearButtons();
			int num = movementTypes.Length;
			letterMovementButtons = new LetterMovementButton[num];
			for (int i = 0; i < letterMovementButtons.Length; i++)
			{
				LetterMovementButton component = Object.Instantiate(buttonPrefab, letterMovementContainer).GetComponent<LetterMovementButton>();
				int num2 = i;
				if (num2 >= 0 && num2 < num)
				{
					component.Setup(movementTypes[num2]);
				}
				component.yPosition = i;
				letterMovementButtons[i] = component;
			}
			this.letterMovementPicker = letterMovementPicker;
		}

		public void SelectLetterMovementType(LetterMovementType movementType)
		{
			letterMovementPicker.SetLetterMovementType(movementType);
			letterMovementPicker.onEndEdit(movementType);
			Hide();
		}

		private void ShowWindow()
		{
			base.gameObject.SetActive(value: true);
			base.editor.popupBackground.SetActive(value: true);
			LevelEditorPlaySound("sndEditorPopupLarge", "LevelEditorInspectorPanel");
			panel.DOKill(complete: true);
			panel.localScale = Vector3.zero;
			panel.DOScale(Vector3.one, base.gc.roomsSelectionPopup_showAnimDuration).SetEase(base.gc.roomsSelectionPopup_showAnimEase).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.SetId("noKillOnReload");
		}

		public void Hide()
		{
			base.editor.popupBackground.SetActive(value: false);
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: false);
				LevelEditorPlaySound("sndEditorPopdownLarge", "LevelEditorInspectorPanel");
			}
		}

		private void Update()
		{
		}

		public void AssignLetterMovementPicker(LetterMovementPicker letterMovementPicker)
		{
			this.letterMovementPicker = letterMovementPicker;
		}
	}
}
