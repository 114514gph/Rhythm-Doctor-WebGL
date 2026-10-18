using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_AdvanceText : SyllableInspectorPanel
	{
		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_AdvanceText levelEvent_AdvanceText = (LevelEvent_AdvanceText)levelEvent;
			id.text = levelEvent_AdvanceText.id.ToString();
			fragment.text = "";
			if (levelEvent_AdvanceText.active)
			{
				int num = -1;
				LevelEvent_FloatingText levelEvent_FloatingText = null;
				foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
				{
					if (eventControl.levelEvent is LevelEvent_FloatingText levelEvent_FloatingText2 && levelEvent_FloatingText2.id == levelEvent_AdvanceText.id)
					{
						levelEvent_FloatingText = levelEvent_FloatingText2;
						num = levelEvent_FloatingText2.sortOrder;
					}
				}
				if (levelEvent_FloatingText == null)
				{
					MonoBehaviour.print("Missing FloatingText event with id: " + levelEvent_AdvanceText.id);
					return;
				}
				fadeOutDuration.placeholder.GetComponent<Text>().text = levelEvent_FloatingText.fadeOutDuration.ToString();
				_ = levelEvent_AdvanceText.sortOrder;
				int num2 = 1;
				foreach (LevelEventControl_Base eventControl2 in base.editor.eventControls)
				{
					if (eventControl2.levelEvent is LevelEvent_AdvanceText levelEvent_AdvanceText2 && eventControl2.levelEvent.active && levelEvent_AdvanceText.id == levelEvent_AdvanceText2.id && levelEvent_AdvanceText2.sortOrder < levelEvent_AdvanceText.sortOrder && levelEvent_AdvanceText2.sortOrder > num)
					{
						num2++;
					}
				}
				string[] array = LyricsGame.SplitLyricsLines(levelEvent_FloatingText.GetLocalizedText(), useLinebreak: true);
				if (num2 < array.Length)
				{
					fragment.color = Color.white;
					fragment.text = array[num2];
				}
				else
				{
					fragment.color = Color.red;
					fragment.text = RDString.Get("editor.FloatingText.unused");
				}
			}
			else
			{
				fragment.color = Color.gray;
				fragment.text = RDString.Get("editor.FloatingText.disabled");
			}
			RDEditorUtils.UpdateUIText(fragment, fragment.text);
			base.UpdateUIInternal(levelEvent);
		}

		public void SelectFloatingTextEvent()
		{
			foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
			{
				if (eventControl.levelEvent is LevelEvent_FloatingText levelEvent_FloatingText && levelEvent_FloatingText.id == (currentLevelEvent as LevelEvent_AdvanceText).id)
				{
					base.editor.SelectEventControl(eventControl);
				}
			}
		}
	}
}
