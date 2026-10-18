using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_AddClassicBeat : LevelEventControl_Base
	{
		public List<RectTransform> firstCues;

		public List<RectTransform> secondCues;

		public List<RectTransform> greenPulses;

		public List<RectTransform> bluePulses;

		public List<RectTransform> orangePulses;

		public List<RectTransform> yellowPulses;

		public RectTransform freezePulse;

		public RectTransform hitPulse;

		public RectTransform bombPulse;

		public Text debugText;

		public RectTransform loop;

		public Image heldbeatFill;

		public Image heldbeatBorder;

		public Image skipshotFill;

		public Image skipshotBorder;

		public Image subdivBorder;

		public Image subdivFill;

		public Image hitPulseImage;

		public Text description;

		[NonSerialized]
		public float syncoOffset;

		[NonSerialized]
		public Color heldbeatBorderColor;

		[NonSerialized]
		public Color heldbeatFillColor;

		[NonSerialized]
		public Color skipshotBorderColor;

		[NonSerialized]
		public Color skipshotFillColor;

		[NonSerialized]
		public Color subdivBorderColor;

		[NonSerialized]
		public Color subdivFillColor;

		[NonSerialized]
		public Color hitPulseColor;

		private Image loopImage;

		public Color yellowPulseNormalColor;

		public Color yellowPulseHighlightColor;

		public Color greenPulseNormalColor;

		public Color greenPulseHighlightColor;

		public Sprite loopMouseOverStart;

		public Sprite loopMouseOverEnd;

		private int rowY => levelEvent.row % 4;

		private new void Awake()
		{
			base.Awake();
			levelEvent = new LevelEvent_AddClassicBeat();
			levelEvent.OnCreate();
			hitPulse = yellowPulses[0];
			hitPulseImage = hitPulse.GetComponent<Image>();
			hitPulseColor = hitPulseImage.color;
			if (loop != null)
			{
				loopImage = loop.GetComponent<Image>();
				loopImage.material = UnityEngine.Object.Instantiate(loopImage.material);
				loopImage.material.SetFloat("_StencilComp", 3f);
				loopImage.material.SetFloat("_Stencil", 1f);
				loopImage.material.SetFloat("_StencilReadMask", 1f);
			}
			if (heldbeatBorder != null)
			{
				heldbeatBorderColor = heldbeatBorder.color;
			}
			if (heldbeatFill != null)
			{
				heldbeatFillColor = heldbeatFill.color;
			}
			if (skipshotBorder != null)
			{
				skipshotBorderColor = skipshotBorder.color;
			}
			if (skipshotFill != null)
			{
				skipshotFillColor = skipshotFill.color;
			}
			if (subdivBorder != null)
			{
				subdivBorderColor = subdivBorder.color;
			}
			if (subdivFill != null)
			{
				subdivFillColor = subdivFill.color;
			}
		}

		public new void SaveData()
		{
			InspectorPanel inspectorPanel = levelEvent.inspectorPanel;
			if (inspectorPanel != null)
			{
				inspectorPanel.Save(levelEvent, isNew: false);
			}
		}

		public override void ShowBorders(bool show)
		{
			showingBorders = show;
			if (levelEvent is LevelEvent_AddClassicBeat)
			{
				foreach (RectTransform greenPulse in greenPulses)
				{
					greenPulse.gameObject.SetActive(show);
				}
			}
			else if (levelEvent is LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat)
			{
				bool num = levelEvent_AddOneshotBeat.freezeBurnMode != FreezeBurnMode.None;
				bool flag = num || (levelEvent_AddOneshotBeat.hold && levelEvent_AddOneshotBeat.holdCue != HoldCueType.Late);
				bool flag2 = num;
				foreach (RectTransform greenPulse2 in greenPulses)
				{
					greenPulse2.gameObject.SetActive(show);
				}
				foreach (RectTransform firstCue in firstCues)
				{
					firstCue.gameObject.SetActive(show & flag);
				}
				foreach (RectTransform secondCue in secondCues)
				{
					secondCue.gameObject.SetActive(show & flag2);
				}
			}
			if (loop != null && selected)
			{
				loop.gameObject.SetActive(show);
			}
		}

		public override void ShowAsSelected(bool notBeingActuallySelected = false)
		{
			base.ShowAsSelected(notBeingActuallySelected);
			UpdateHeldbeatBar();
		}

		public override void ShowAsDeselected()
		{
			base.ShowAsDeselected();
			if (levelEvent is LevelEvent_AddOneshotBeat)
			{
				loop.gameObject.SetActive(value: false);
			}
			UpdateHeldbeatBar();
		}

		public override void OnDoubleClick()
		{
			if (!(levelEvent.GetType() == typeof(LevelEvent_AddFreeTimeBeat)))
			{
				return;
			}
			printe("freetime doubleclicked");
			LevelEvent_AddFreeTimeBeat levelEvent_AddFreeTimeBeat = (LevelEvent_AddFreeTimeBeat)levelEvent;
			int num = levelEvent_AddFreeTimeBeat.pulse;
			List<LevelEventControl_Base> list = base.editor.eventControls_rows[levelEvent_AddFreeTimeBeat.row].OrderBy((LevelEventControl_Base i) => i.levelEvent.sortOrder).ToList();
			scrConductor.PlayImmediately("sndEditorEventSelectMultiple", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			foreach (LevelEventControl_Base item in list)
			{
				LevelEvent_Base levelEvent_Base = item.levelEvent;
				if (levelEvent_Base.sortOrder > levelEvent_AddFreeTimeBeat.sortOrder && levelEvent_Base is LevelEvent_PulseFreeTimeBeat)
				{
					LevelEvent_PulseFreeTimeBeat levelEvent_PulseFreeTimeBeat = (LevelEvent_PulseFreeTimeBeat)levelEvent_Base;
					base.editor.AddEventControlToSelection(item);
					num = ((levelEvent_PulseFreeTimeBeat.action == PulseAction.Increment) ? (num + 1) : ((levelEvent_PulseFreeTimeBeat.action != PulseAction.Decrement) ? levelEvent_PulseFreeTimeBeat.customPulse : (num - 1)));
					if (num == 6)
					{
						break;
					}
				}
			}
		}

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			levelEvent.GetType();
			RectTransform component = GetComponent<RectTransform>();
			if (levelEvent is LevelEvent_AddClassicBeat)
			{
				LevelEvent_AddClassicBeat levelEvent_AddClassicBeat = levelEvent as LevelEvent_AddClassicBeat;
				int num = levelEvent_AddClassicBeat.GetLength() - 1;
				int num2 = -1;
				syncoOffset = 0f;
				bool flag = false;
				LevelEvent_SetRowXs closestSetBeatModifiersEventWithRow = base.timeline.GetClosestSetBeatModifiersEventWithRow(levelEvent.row, levelEvent.barAndBeat);
				if (closestSetBeatModifiersEventWithRow != null)
				{
					num2 = closestSetBeatModifiersEventWithRow.syncoBeat;
					flag = num2 != -1 && num2 < num;
					if (flag)
					{
						float syncoSwing = closestSetBeatModifiersEventWithRow.syncoSwing;
						if (syncoSwing > 0f)
						{
							syncoOffset = 0f - syncoSwing;
						}
						else
						{
							syncoOffset = -0.5f;
						}
					}
				}
				Vector2 anchoredPosition = component.anchoredPosition;
				anchoredPosition.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
				anchoredPosition.y = base.timeline.GetPosYFromRowIndex(base.visualRowIndex);
				component.anchoredPosition = anchoredPosition;
				float x = (float)base.cellWidth * (((float)num + syncoOffset) * levelEvent_AddClassicBeat.tick);
				component.sizeDelta = new Vector2(x, base.cellHeight);
				UpdateHeldbeatBar();
				hitPulse.gameObject.SetActive(value: true);
				hitPulse.anchoredPosition = hitPulse.anchoredPosition.WithX((float)base.cellWidth * (((float)num + syncoOffset) * levelEvent_AddClassicBeat.tick) - 2f);
				for (int i = 0; i < greenPulses.Count; i++)
				{
					RectTransform rectTransform = greenPulses[i];
					float num3 = 0f;
					num3 = ((levelEvent_AddClassicBeat.swing == 0f) ? ((float)i * levelEvent_AddClassicBeat.tick) : ((!levelEvent_AddClassicBeat.legacy) ? ((float)LevelEvent_AddClassicBeat.GetOffsetFromSwingNew(i, levelEvent_AddClassicBeat)) : ((float)LevelEvent_AddClassicBeat.GetOffsetFromSwing(i, levelEvent_AddClassicBeat))));
					if (flag && i > num2)
					{
						num3 += syncoOffset * levelEvent_AddClassicBeat.tick;
					}
					rectTransform.anchoredPosition = hitPulse.anchoredPosition.WithX((float)base.cellWidth * num3 - 1f);
					rectTransform.GetComponent<Image>().enabled = i < num;
				}
				ShowBorders(showingBorders);
			}
			else if (levelEvent is LevelEvent_AddOneshotBeat)
			{
				LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = (LevelEvent_AddOneshotBeat)levelEvent;
				Vector2 anchoredPosition2 = component.anchoredPosition;
				anchoredPosition2.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
				anchoredPosition2.y = base.timeline.GetPosYFromRowIndex(base.visualRowIndex);
				bool flag2 = levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Freezeshot;
				bool flag3 = levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Burnshot;
				bool hold = levelEvent_AddOneshotBeat.hold;
				int num4 = levelEvent_AddOneshotBeat.loops + 1;
				float num5 = levelEvent_AddOneshotBeat.tick + levelEvent_AddOneshotBeat.actualDelay;
				component.anchoredPosition = anchoredPosition2;
				float num6 = levelEvent_AddOneshotBeat.interval * (float)levelEvent_AddOneshotBeat.loops + num5;
				component.sizeDelta = new Vector2((float)base.cellWidth * num6, base.cellHeight);
				bool skipshot = levelEvent_AddOneshotBeat.skipshot;
				bool flag4 = levelEvent_AddOneshotBeat.subdivisions > 1;
				float num7 = ((!(flag4 & flag3)) ? levelEvent_AddOneshotBeat.tick : ((levelEvent_AddOneshotBeat.subdivTickOverride > 0f) ? levelEvent_AddOneshotBeat.subdivTickOverride : (levelEvent_AddOneshotBeat.interval / 2f)));
				foreach (RectTransform yellowPulse in yellowPulses)
				{
					yellowPulse.gameObject.SetActive(value: false);
				}
				foreach (RectTransform item in bluePulses.Concat(orangePulses).Concat(greenPulses).Concat(firstCues)
					.Concat(secondCues))
				{
					item.gameObject.SetActive(value: false);
					item.GetComponent<Image>().enabled = false;
				}
				int num8 = ((!flag4) ? 1 : levelEvent_AddOneshotBeat.subdivisions);
				for (int j = 0; j < num4; j++)
				{
					for (int k = 0; k < num8; k++)
					{
						int num9 = num8 * j + k;
						RectTransform rectTransform2;
						if (num9 >= greenPulses.Count)
						{
							rectTransform2 = UnityEngine.Object.Instantiate(greenPulses[0], rt).GetComponent<RectTransform>();
							rectTransform2.name = "greenPulse" + num9;
							greenPulses.Add(rectTransform2);
						}
						else
						{
							rectTransform2 = greenPulses[num9];
							rectTransform2.GetComponent<Image>().enabled = true;
						}
						rectTransform2.gameObject.SetActive(value: true);
						float num10 = levelEvent_AddOneshotBeat.interval * (float)j;
						num10 += levelEvent_AddOneshotBeat.tick * ((float)k / (float)num8);
						rectTransform2.anchoredPosition = rectTransform2.anchoredPosition.WithX((float)base.cellWidth * num10 - 1f);
					}
				}
				if (flag2)
				{
					for (int l = 0; l < num4; l++)
					{
						RectTransform rectTransform2;
						if (l >= bluePulses.Count)
						{
							rectTransform2 = UnityEngine.Object.Instantiate(bluePulses[0], rt).GetComponent<RectTransform>();
							rectTransform2.name = "bluePulse" + l;
							bluePulses.Add(rectTransform2);
						}
						else
						{
							rectTransform2 = bluePulses[l];
							rectTransform2.GetComponent<Image>().enabled = true;
						}
						rectTransform2.gameObject.SetActive(value: true);
						float num11 = levelEvent_AddOneshotBeat.tick + levelEvent_AddOneshotBeat.interval * (float)l;
						rectTransform2.anchoredPosition = rectTransform2.anchoredPosition.WithX((float)base.cellWidth * num11 - 2f);
					}
				}
				if (flag4)
				{
					for (int m = 0; m < num4; m++)
					{
						for (int n = 0; n < levelEvent_AddOneshotBeat.subdivisions; n++)
						{
							int num12 = levelEvent_AddOneshotBeat.subdivisions * m + n;
							RectTransform rectTransform2;
							if (num12 >= orangePulses.Count)
							{
								rectTransform2 = UnityEngine.Object.Instantiate(orangePulses[0], rt).GetComponent<RectTransform>();
								rectTransform2.name = "orangePulse" + num12;
								orangePulses.Add(rectTransform2);
							}
							else
							{
								rectTransform2 = orangePulses[num12];
								rectTransform2.GetComponent<Image>().enabled = true;
							}
							rectTransform2.gameObject.SetActive(value: true);
							float num13 = num5 + levelEvent_AddOneshotBeat.interval * (float)m;
							num13 += num7 * ((float)n / (float)levelEvent_AddOneshotBeat.subdivisions);
							rectTransform2.AnchorPosX((float)base.cellWidth * num13 - 2f);
						}
					}
				}
				for (int num14 = 0; num14 < num4; num14++)
				{
					RectTransform rectTransform2;
					if (num14 >= yellowPulses.Count)
					{
						rectTransform2 = UnityEngine.Object.Instantiate(yellowPulses[0], rt).GetComponent<RectTransform>();
						rectTransform2.name = "yellowPulse" + num14;
						yellowPulses.Add(rectTransform2);
					}
					else
					{
						rectTransform2 = yellowPulses[num14];
					}
					rectTransform2.GetComponent<Image>().color = yellowPulseNormalColor;
					rectTransform2.GetComponent<Outline>().enabled = flag3;
					rectTransform2.gameObject.SetActive(value: true);
					float num15 = num5 + levelEvent_AddOneshotBeat.interval * (float)num14;
					rectTransform2.anchoredPosition = rectTransform2.anchoredPosition.WithX((float)base.cellWidth * num15 - 2f);
					rectTransform2.SetAsLastSibling();
				}
				RectTransform rectTransform3 = subdivBorder.rectTransform;
				float num16 = num7 * (1f - 1f / (float)levelEvent_AddOneshotBeat.subdivisions) * (float)base.cellWidth;
				if (flag4)
				{
					rectTransform3.SizeDeltaX(num16);
				}
				else
				{
					num16 = 0f;
				}
				rectTransform3.gameObject.SetActive(flag4);
				RectTransform rectTransform4 = heldbeatBorder.rectTransform;
				float num17 = (levelEvent_AddOneshotBeat.interval - (levelEvent_AddOneshotBeat.tick + levelEvent_AddOneshotBeat.delay)) * (float)base.cellWidth - num16;
				rectTransform4.AnchorPosX(num16);
				if (levelEvent_AddOneshotBeat.hold && num17 > 0f)
				{
					rectTransform4.SizeDeltaX(num17);
				}
				else
				{
					num17 = 0f;
				}
				rectTransform4.gameObject.SetActive(num17 > 0f);
				RectTransform rectTransform5 = skipshotBorder.rectTransform;
				rectTransform5.SizeDeltaX((levelEvent_AddOneshotBeat.interval - levelEvent_AddOneshotBeat.delay) * (float)base.cellWidth - num16 - num17);
				rectTransform5.AnchorPosX(num16 + num17);
				rectTransform5.gameObject.SetActive(skipshot);
				bombPulse.gameObject.SetActive(skipshot);
				if (skipshot)
				{
					float num18 = levelEvent_AddOneshotBeat.interval * (float)num4 + levelEvent_AddOneshotBeat.tick;
					bombPulse.anchoredPosition = bombPulse.anchoredPosition.WithX((float)base.cellWidth * num18 - 1f);
					bombPulse.SetAsLastSibling();
				}
				if ((flag2 | flag3) || (hold && levelEvent_AddOneshotBeat.holdCue != HoldCueType.Late))
				{
					float num19 = (flag3 ? levelEvent_AddOneshotBeat.interval : (levelEvent_AddOneshotBeat.interval - levelEvent_AddOneshotBeat.tick));
					for (int num20 = 0; num20 < num4; num20++)
					{
						RectTransform rectTransform6;
						RectTransform rectTransform7;
						if (num20 >= firstCues.Count)
						{
							rectTransform6 = UnityEngine.Object.Instantiate(firstCues[0], rt).GetComponent<RectTransform>();
							rectTransform7 = UnityEngine.Object.Instantiate(secondCues[0], rt).GetComponent<RectTransform>();
							rectTransform6.name = "firstCue" + num20;
							rectTransform7.name = "secondCue" + num20;
							firstCues.Add(rectTransform6);
							secondCues.Add(rectTransform7);
						}
						else
						{
							rectTransform6 = firstCues[num20];
							rectTransform6.GetComponent<Image>().enabled = true;
							rectTransform7 = secondCues[num20];
							rectTransform7.GetComponent<Image>().enabled = true;
						}
						rectTransform6.gameObject.SetActive(value: true);
						rectTransform7.gameObject.SetActive(value: true);
						float num21 = levelEvent_AddOneshotBeat.interval * (float)num20 - num19;
						float num22 = num21 + (flag3 ? levelEvent_AddOneshotBeat.tick : levelEvent_AddOneshotBeat.delay);
						rectTransform6.AnchorPosX((float)base.cellWidth * num21 - 1f);
						rectTransform7.AnchorPosX((float)base.cellWidth * num22 - 1f);
						rectTransform6.SetAsLastSibling();
						rectTransform7.SetAsLastSibling();
					}
				}
				if (description != null)
				{
					description.gameObject.SetActive(value: false);
				}
				ShowBorders(showingBorders);
				loop.SetAsLastSibling();
			}
			else if (levelEvent is LevelEvent_AddFreeTimeBeat)
			{
				LevelEvent_AddFreeTimeBeat levelEvent_AddFreeTimeBeat = (LevelEvent_AddFreeTimeBeat)levelEvent;
				Vector2 anchoredPosition3 = component.anchoredPosition;
				anchoredPosition3.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
				anchoredPosition3.y = base.timeline.GetPosYFromRowIndex(base.visualRowIndex);
				component.anchoredPosition = anchoredPosition3;
				component.sizeDelta = new Vector2(Mathf.Min(base.cellWidth, 7f), base.cellHeight);
				UpdateHeldbeatBar();
				bool flag5 = levelEvent_AddFreeTimeBeat.pulse == -1;
				hitPulse.gameObject.SetActive((levelEvent_AddFreeTimeBeat.pulse == 6) | flag5);
				hitPulse.anchoredPosition = hitPulse.anchoredPosition.WithX(-2f);
				hitPulseImage.color = (flag5 ? "EC5E60".HexToColor() : hitPulseColor);
				initialColor = base.gc.addFreetimeControlColor;
				description.gameObject.SetActive(value: true);
				description.text = (flag5 ? "!" : (levelEvent_AddFreeTimeBeat.pulse + 1).ToString());
				if (levelEvent.active)
				{
					float a = image.color.a;
					image.color = initialColor.WithAlpha(a);
				}
			}
			else if (levelEvent is LevelEvent_PulseFreeTimeBeat)
			{
				LevelEvent_PulseFreeTimeBeat levelEvent_PulseFreeTimeBeat = (LevelEvent_PulseFreeTimeBeat)levelEvent;
				bool flag6 = false;
				Vector2 anchoredPosition4 = component.anchoredPosition;
				anchoredPosition4.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
				anchoredPosition4.y = base.timeline.GetPosYFromRowIndex(base.visualRowIndex);
				component.anchoredPosition = anchoredPosition4;
				component.sizeDelta = new Vector2(Mathf.Min(base.cellWidth, 7f), 12f * base.timeline.zoomVertFactor);
				UpdateHeldbeatBar();
				initialColor = base.gc.pulseFreetimeControlColor;
				if (levelEvent.active)
				{
					float a2 = image.color.a;
					image.color = initialColor.WithAlpha(a2);
				}
				description.gameObject.SetActive(value: true);
				if (levelEvent_PulseFreeTimeBeat.action == PulseAction.Increment)
				{
					description.text = ">";
				}
				else if (levelEvent_PulseFreeTimeBeat.action == PulseAction.Decrement)
				{
					description.text = "<";
				}
				else if (levelEvent_PulseFreeTimeBeat.action == PulseAction.Remove)
				{
					description.text = "x";
				}
				else
				{
					flag6 = levelEvent_PulseFreeTimeBeat.customPulse == -1;
					description.text = (flag6 ? "!" : (levelEvent_PulseFreeTimeBeat.customPulse + 1).ToString());
				}
				hitPulse.gameObject.SetActive((levelEvent_PulseFreeTimeBeat.customPulse == 6) | flag6);
				hitPulse.anchoredPosition = hitPulse.anchoredPosition.WithX(-2f);
				hitPulseImage.color = (flag6 ? "EC5E60".HexToColor() : hitPulseColor);
			}
			component.sizeDelta = component.sizeDelta.WithX(Math.Max(2f, component.sizeDelta.x));
		}

		private Image LastAvailablePulse()
		{
			RectTransform rectTransform = null;
			if (levelEvent is LevelEvent_AddClassicBeat)
			{
				rectTransform = yellowPulses[0];
			}
			else
			{
				int loops = (levelEvent as LevelEvent_AddOneshotBeat).loops;
				int index = Math.Min(yellowPulses.Count - 1, loops);
				rectTransform = yellowPulses[index];
			}
			return rectTransform.GetComponent<Image>();
		}

		public void HitPulsePointerEnter()
		{
			LastAvailablePulse().color = yellowPulseHighlightColor;
		}

		public void HitPulsePointerExit()
		{
			LastAvailablePulse().color = yellowPulseNormalColor;
		}

		public void LoopPointerEnter()
		{
			if (loopImage != null)
			{
				loopImage.sprite = loopMouseOverStart;
			}
		}

		public void LoopPointerExit()
		{
			if (loopImage != null)
			{
				loopImage.sprite = loopMouseOverEnd;
			}
		}

		public void LeftPointerEnter()
		{
			greenPulses[0].GetComponent<Image>().color = greenPulseHighlightColor;
		}

		public void LeftPointerExit()
		{
			greenPulses[0].GetComponent<Image>().color = greenPulseNormalColor;
		}

		public void UpdateHeldbeatBar()
		{
			float num = 0f;
			bool flag = false;
			bool flag2 = false;
			if (levelEvent is LevelEvent_AddClassicBeat levelEvent_AddClassicBeat)
			{
				num = levelEvent_AddClassicBeat.hold;
				flag = true;
			}
			else if (levelEvent is LevelEvent_AddFreeTimeBeat levelEvent_AddFreeTimeBeat)
			{
				num = levelEvent_AddFreeTimeBeat.GetHoldLength();
				flag = selected || levelEvent_AddFreeTimeBeat.pulse == 6;
				flag2 = true;
			}
			else
			{
				if (!(levelEvent is LevelEvent_PulseFreeTimeBeat levelEvent_PulseFreeTimeBeat))
				{
					return;
				}
				num = levelEvent_PulseFreeTimeBeat.GetHoldLength();
				flag = (selected || levelEvent_PulseFreeTimeBeat.customPulse == 6) && levelEvent_PulseFreeTimeBeat.action != PulseAction.Remove;
				flag2 = true;
			}
			float num2 = (flag2 ? GetComponent<RectTransform>().sizeDelta.x : 0f);
			float num3 = num * (float)base.cellWidth - num2;
			RectTransform rectTransform = heldbeatBorder.rectTransform;
			rectTransform.sizeDelta = new Vector2(num3, rectTransform.sizeDelta.y);
			rectTransform.gameObject.SetActive(flag && num3 > 0f);
		}
	}
}
