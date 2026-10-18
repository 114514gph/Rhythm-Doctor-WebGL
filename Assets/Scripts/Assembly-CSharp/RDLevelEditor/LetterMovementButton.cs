using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LetterMovementButton : RDEditorBase, IPointerEnterHandler, IEventSystemHandler
	{
		public Button button;

		public LetterMovementType letterMovementType;

		public Text text;

		public int yPosition;

		public void Setup(LetterMovementType movementType)
		{
			letterMovementType = movementType;
			text.text = RDString.Get($"enum.LetterMovementType.{movementType}");
			text.font = RDString.GetAppropiateFontForString(text.text);
			LetterMovementController component = text.GetComponent<LetterMovementController>();
			float[] array;
			float[] multipliers;
			switch (movementType)
			{
			case LetterMovementType.Shake:
				array = new float[1] { 1f };
				goto IL_00b4;
			case LetterMovementType.Wave:
				array = new float[2] { 1f, 1f };
				goto IL_00b4;
			case LetterMovementType.Swirl:
				array = new float[2] { 1f, 1f };
				goto IL_00b4;
			default:
				array = new float[0];
				goto IL_00b4;
			case LetterMovementType.None:
				break;
				IL_00b4:
				multipliers = array;
				for (int i = 0; i < text.text.Length; i++)
				{
					component.AddCharIndex(i, movementType, multipliers);
				}
				break;
			}
			button.onClick.AddListener(() =>
			{
				Select();
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
		}

		private void Select()
		{
			base.editor.letterMovementPickerPopup.SelectLetterMovementType(letterMovementType);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			float pitch = RDUtils.PitchSemitonesToPercent((float)yPosition - 3f);
			LevelEditorPlaySound("sndListHover", "LevelEditorInspectorPanel", 1f, pitch);
		}
	}
}
