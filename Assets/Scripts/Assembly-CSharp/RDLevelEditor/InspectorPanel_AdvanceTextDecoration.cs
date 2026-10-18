using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_AdvanceTextDecoration : SyllableInspectorPanel
	{
		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_AdvanceTextDecoration levelEvent_AdvanceTextDecoration = (LevelEvent_AdvanceTextDecoration)levelEvent;
			fragment.text = "";
			if (levelEvent_AdvanceTextDecoration.active)
			{
				int num = -1;
				LevelEvent_SetText lastSetTextEvent = levelEvent_AdvanceTextDecoration.lastSetTextEvent;
				if (lastSetTextEvent != null)
				{
					num = lastSetTextEvent.sortOrder;
					fadeOutDuration.placeholder.GetComponent<Text>().text = lastSetTextEvent.fadeOutDuration.ToString();
					int num2 = 1;
					foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
					{
						if (eventControl.levelEvent is LevelEvent_AdvanceTextDecoration levelEvent_AdvanceTextDecoration2 && eventControl.levelEvent.active && !(levelEvent_AdvanceTextDecoration2.target != levelEvent_AdvanceTextDecoration.target) && levelEvent_AdvanceTextDecoration2.sortOrder < levelEvent_AdvanceTextDecoration.sortOrder && levelEvent_AdvanceTextDecoration2.sortOrder > num)
						{
							num2++;
						}
					}
					string[] array = LyricsGame.SplitLyricsLines(lastSetTextEvent.GetLocalizedText(), useLinebreak: true);
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
	}
}
