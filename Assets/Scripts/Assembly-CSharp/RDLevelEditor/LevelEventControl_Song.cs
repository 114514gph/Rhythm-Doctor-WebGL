using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_Song : LevelEventControl_Base
	{
		public Text nurseSay;

		public Image icon;

		public Sprite cpbIcon;

		public Text cpbCrotchets;

		private string defaultNurseSay;

		private void Start()
		{
			if (levelEvent is LevelEvent_PlaySong)
			{
				Object.Instantiate(base.timeline.wavePrefab).transform.SetParent(base.timeline.wavesContainer, worldPositionStays: false);
			}
			if (levelEvent is LevelEvent_SayReadyGetSetGo)
			{
				defaultNurseSay = nurseSay.text;
			}
		}

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			RectTransform component = GetComponent<RectTransform>();
			Vector2 anchoredPosition = component.anchoredPosition;
			nurseSay.gameObject.SetActive(value: false);
			if (levelEvent is LevelEvent_SetCrotchetsPerBar)
			{
				LevelEvent_SetCrotchetsPerBar levelEvent_SetCrotchetsPerBar = levelEvent as LevelEvent_SetCrotchetsPerBar;
				icon.sprite = cpbIcon;
				cpbCrotchets.gameObject.SetActive(value: true);
				cpbCrotchets.text = ((levelEvent_SetCrotchetsPerBar.crotchetsPerBar > 9) ? "-" : levelEvent_SetCrotchetsPerBar.crotchetsPerBar.ToString());
				component.sizeDelta = new Vector2(Mathf.Min(14, base.cellWidth), base.cellHeight);
				float num = Mathf.Min(component.sizeDelta.x, component.sizeDelta.y);
				cpbCrotchets.fontSize = ((num >= 14f) ? 8 : 3);
			}
			else if (levelEvent is LevelEvent_SayReadyGetSetGo)
			{
				LevelEvent_SayReadyGetSetGo levelEvent_SayReadyGetSetGo = (LevelEvent_SayReadyGetSetGo)levelEvent;
				float num2 = 0f;
				string text = "";
				switch (levelEvent_SayReadyGetSetGo.phraseToSay)
				{
				case OneshotPhraseToSay.SayReaDyGetSetGoNew:
					text = "Rea, Dy, Get, Set, Go!";
					num2 = 4f * levelEvent_SayReadyGetSetGo.tick + 1f;
					break;
				case OneshotPhraseToSay.SayReadyGetSetGo:
					text = "Ready, Get, Set, Go!";
					num2 = 4f * levelEvent_SayReadyGetSetGo.tick + 1f;
					break;
				case OneshotPhraseToSay.SayGetSetOne:
					text = "Get, Set, One!";
					num2 = 2f * levelEvent_SayReadyGetSetGo.tick + 1f;
					break;
				case OneshotPhraseToSay.SayReaDyGetSetOne:
					text = "Rea, Dy, Get, Set, One!";
					num2 = 4f * levelEvent_SayReadyGetSetGo.tick + 1f;
					break;
				case OneshotPhraseToSay.SayGetSetGo:
					text = "Get, Set, Go!";
					num2 = 2f * levelEvent_SayReadyGetSetGo.tick + 1f;
					break;
				case OneshotPhraseToSay.JustSayRea:
					text = "Rea";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.JustSayDy:
					text = "Dy";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.JustSayReady:
					text = "Ready";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.JustSayGet:
					text = "Get";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.JustSaySet:
					text = "Set";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.JustSayGo:
					text = "Go!";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.JustSayStop:
					text = "Stop!";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.JustSayAnd:
					text = "And";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.JustSayAndStop:
					text = "And Stop!";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.SayWatch:
					text = "Watch!";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.SaySwitch:
					text = "Switch!";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.SayListen:
					text = "Listen!!";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count1:
					text = "1";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count2:
					text = "2";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count3:
					text = "3";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count4:
					text = "4";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count5:
					text = "5";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count6:
					text = "6";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count7:
					text = "7";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count8:
					text = "8";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count9:
					text = "9";
					num2 = 1f;
					break;
				case OneshotPhraseToSay.Count10:
					text = "10";
					num2 = 1f;
					break;
				default:
					num2 = 1f;
					break;
				}
				float num3 = component.sizeDelta.x / (float)base.cellWidth;
				if (num3 > 1f && num2 > 1f && num3 != num2 && nurseSay.text != text && nurseSay.text != defaultNurseSay && base.timeline.GetPosXFromBarAndBeat(levelEvent_SayReadyGetSetGo.bar, levelEvent_SayReadyGetSetGo.beat) == component.anchoredPosition.x)
				{
					float posXFromBarAndBeat = base.timeline.GetPosXFromBarAndBeat(levelEvent_SayReadyGetSetGo.bar, levelEvent_SayReadyGetSetGo.beat + num3 - num2);
					levelEvent_SayReadyGetSetGo.barAndBeat = base.timeline.GetBarAndBeatWithPosX(posXFromBarAndBeat);
					base.bar = levelEvent_SayReadyGetSetGo.bar;
					base.beat = levelEvent_SayReadyGetSetGo.beat;
					levelEvent.inspectorPanel.UpdateUI(levelEvent);
				}
				component.sizeDelta = new Vector2((float)base.cellWidth * num2, base.cellHeight);
				nurseSay.gameObject.SetActive(value: true);
				nurseSay.text = text;
				icon.gameObject.SetActive(value: false);
			}
			else
			{
				icon.sprite = base.iconSprite;
				icon.gameObject.SetActive(value: true);
				component.sizeDelta = new Vector2(Mathf.Min(14, base.cellWidth), base.cellHeight);
				if (levelEvent is LevelEvent_PlaySong)
				{
					component.sizeDelta = new Vector2((float)base.cellWidth * 2f, base.cellHeight);
				}
			}
			anchoredPosition.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
			anchoredPosition.y = base.timeline.GetPosYFromRowIndex(levelEvent.y);
			component.anchoredPosition = anchoredPosition;
		}
	}
}
