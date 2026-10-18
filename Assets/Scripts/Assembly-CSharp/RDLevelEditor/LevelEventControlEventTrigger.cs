using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControlEventTrigger : RDEventTrigger
	{
		protected enum DraggingPosition
		{
			Left = 0,
			Middle = 1,
			Right = 2,
			TopRight = 3
		}

		private static List<LevelEventControl_Base> eventControls;

		public Transform currentTransform;

		public float soundTriggerCooldown = 0.075f;

		public float soundTriggerTimer;

		[NonSerialized]
		public LevelEventControl_Base control;

		private float currentPosX;

		private int currentPosY;

		private int currentCellX;

		private int currentCellY;

		private DraggingPosition draggingPosition;

		private DraggingPosition mouseOverPosition;

		private bool updateMouseOverPosition;

		private int _lastRowHeaderIndex;

		[NonSerialized]
		public float lastClickTime;

		private int LastRowHeaderIndex
		{
			get
			{
				return _lastRowHeaderIndex;
			}
			set
			{
				if (_lastRowHeaderIndex != value && value < 4)
				{
					if (LastRowHeaderIndex >= 0)
					{
						LastRowHeaderTypeIcon.color = RDConstants.data.rowHeaderDefaultColor;
					}
					_lastRowHeaderIndex = value;
				}
			}
		}

		private Image LastRowHeaderTypeIcon => base.rowsTab.rowHeaders[LastRowHeaderIndex].GetComponent<RowHeader>().rowTypeIcon;

		public float XPositionInsideCell => base.timeline.MousePositionOnTimeline.x - currentPosX;

		public float YPositionInsideCell => base.timeline.MousePositionOnTimeline.y - (float)currentPosY;

		private void Awake()
		{
			control = GetComponent<LevelEventControl_Base>();
			LastRowHeaderIndex = -1;
		}

		private float SnapXPositionToGrid(float x)
		{
			return (float)(int)(x / base.cellWidthSnap) * base.cellWidthSnap;
		}

		private int SnapYPositionToGrid(float y)
		{
			return (int)(y / (float)base.cellHeight) * base.cellHeight;
		}

		private void Update()
		{
			if (!updateMouseOverPosition)
			{
				return;
			}
			bool num = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			LevelEvent_Base levelEvent = control.levelEvent;
			float xPositionInsideCell = XPositionInsideCell;
			float rightPosition = control.rt.sizeDelta.x - xPositionInsideCell;
			if (!num && (levelEvent is LevelEvent_AddClassicBeat || levelEvent is LevelEvent_AddOneshotBeat))
			{
				mouseOverPosition = DetermineDraggingPosition(levelEvent, rightPosition, YPositionInsideCell);
				LevelEventControl_AddClassicBeat levelEventControl_AddClassicBeat = control as LevelEventControl_AddClassicBeat;
				levelEventControl_AddClassicBeat.LeftPointerExit();
				levelEventControl_AddClassicBeat.HitPulsePointerExit();
				levelEventControl_AddClassicBeat.LoopPointerExit();
				if (mouseOverPosition == DraggingPosition.Right)
				{
					levelEventControl_AddClassicBeat.HitPulsePointerEnter();
				}
				else if (mouseOverPosition == DraggingPosition.TopRight)
				{
					levelEventControl_AddClassicBeat.LoopPointerEnter();
				}
				else if (mouseOverPosition == DraggingPosition.Left)
				{
					levelEventControl_AddClassicBeat.LeftPointerEnter();
				}
			}
			if (soundTriggerTimer > 0f)
			{
				soundTriggerTimer -= Time.unscaledDeltaTime;
			}
		}

		private DraggingPosition DetermineDraggingPosition(LevelEvent_Base levelEvent, float rightPosition, float yPosition)
		{
			if (levelEvent is LevelEvent_AddClassicBeat)
			{
				if (rightPosition < 2f)
				{
					return DraggingPosition.Right;
				}
				if (XPositionInsideCell < 2f)
				{
					return DraggingPosition.Left;
				}
				return DraggingPosition.Middle;
			}
			if (levelEvent is LevelEvent_AddOneshotBeat)
			{
				if (rightPosition < 5f && YPositionInsideCell < 4f)
				{
					return DraggingPosition.TopRight;
				}
				if (rightPosition < 2f)
				{
					return DraggingPosition.Right;
				}
				if (XPositionInsideCell < 2f)
				{
					return DraggingPosition.Left;
				}
				return DraggingPosition.Middle;
			}
			return DraggingPosition.Middle;
		}

		public override void OnPointerEnter(PointerEventData data)
		{
			control.ShowBorders(show: true);
			updateMouseOverPosition = true;
			RecalculateCurrentPos();
			RecalculateCurrentCell();
			if (!isDragging)
			{
				base.editor.SetHoveredEventControl(control);
			}
		}

		public override void OnPointerExit(PointerEventData data)
		{
			base.editor.ClearHoveredEventControl();
			if (!isDragging)
			{
				control.ShowBorders(show: false);
			}
			updateMouseOverPosition = false;
			LevelEvent_Base levelEvent = control.levelEvent;
			if ((levelEvent is LevelEvent_AddClassicBeat || levelEvent is LevelEvent_AddOneshotBeat) && !isDragging)
			{
				LevelEventControl_AddClassicBeat obj = control as LevelEventControl_AddClassicBeat;
				obj.LoopPointerExit();
				obj.HitPulsePointerExit();
			}
		}

		public override void OnPointerDown(PointerEventData data)
		{
			if (data.button == PointerEventData.InputButton.Left)
			{
				currentTransform = base.transform;
				RecalculateCurrentPos();
				RecalculateCurrentCell();
				if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
				{
					base.timeline.disableBoxSelection = true;
				}
				float xPositionInsideCell = XPositionInsideCell;
				float rightPosition = control.rt.sizeDelta.x - xPositionInsideCell;
				draggingPosition = DetermineDraggingPosition(control.levelEvent, rightPosition, YPositionInsideCell);
			}
		}

		public void RecalculateCurrentPos()
		{
			if (!(currentTransform == null))
			{
				currentPosX = currentTransform.localPosition.x;
				currentPosY = (int)Mathf.Abs(currentTransform.localPosition.y);
			}
		}

		public void RecalculateCurrentCell()
		{
			currentCellX = Mathf.FloorToInt(XPositionInsideCell / base.cellWidthSnap);
			currentCellY = Mathf.FloorToInt(YPositionInsideCell / (float)base.cellHeight);
		}

		public override void OnBeginDrag(PointerEventData data)
		{
			if (data.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			bool flag = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			if (!base.editor.selectedControls.Contains(control))
			{
				base.editor.SelectEventControl(control, sound: true);
			}
			bool flag2 = false;
			if (flag)
			{
				base.timeline.disableBoxSelection = true;
				flag2 = true;
				base.editor.Clone(this, flag2);
			}
			foreach (LevelEventControl_Base selectedControl in base.editor.selectedControls)
			{
				if (flag)
				{
					selectedControl.trigger.currentTransform = selectedControl.trigger.transform;
					selectedControl.trigger.RecalculateCurrentPos();
					selectedControl.trigger.RecalculateCurrentCell();
				}
				selectedControl.levelEvent.isDragging = true;
				selectedControl.trigger.isDragging = true;
				selectedControl.trigger.draggingPosition = draggingPosition;
			}
			base.timeline.setCrotchetsPerBarEventsTemp = null;
			List<LevelEvent_SetCrotchetsPerBar> setCrotchetsPerBarEvents = base.timeline.setCrotchetsPerBarEvents;
			base.timeline.setCrotchetsPerBarEventsTemp = new List<LevelEvent_SetCrotchetsPerBar>();
			foreach (LevelEvent_SetCrotchetsPerBar item in setCrotchetsPerBarEvents)
			{
				base.timeline.setCrotchetsPerBarEventsTemp.Add((LevelEvent_SetCrotchetsPerBar)item.Clone());
			}
			if (!flag2)
			{
				base.editor.SaveState();
			}
		}

		public override void OnDrag(PointerEventData data)
		{
			if (data.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			if (draggingPosition != DraggingPosition.Middle && control is LevelEventControl_AddClassicBeat)
			{
				foreach (LevelEventControl_Base selectedControl in base.editor.selectedControls)
				{
					selectedControl.trigger.DragBeat(this, draggingPosition);
				}
				return;
			}
			List<int> list = new List<int>();
			RecalculateCurrentPos();
			float num = XPositionInsideCell - (float)currentCellX * base.cellWidthSnap;
			if (num > base.cellWidthSnap || num < 0f)
			{
				float num2 = SnapXPositionToGrid(currentPosX + num) - currentPosX;
				float num3 = float.MaxValue;
				foreach (LevelEventControl_Base selectedControl2 in base.editor.selectedControls)
				{
					num3 = Mathf.Min(selectedControl2.transform.localPosition.x, num3);
				}
				if (num3 + num2 < 0f)
				{
					num2 = 0f - num3;
				}
				if (Mathf.Abs(num2) > 0.001f)
				{
					foreach (LevelEventControl_Base selectedControl3 in base.editor.selectedControls)
					{
						LevelEvent_Base levelEvent = selectedControl3.levelEvent;
						float num4 = 0f;
						if (levelEvent is LevelEvent_AddOneshotBeat)
						{
							LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = levelEvent as LevelEvent_AddOneshotBeat;
							if (levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Freezeshot || (levelEvent_AddOneshotBeat.hold && levelEvent_AddOneshotBeat.holdCue != HoldCueType.Late))
							{
								num4 = levelEvent_AddOneshotBeat.interval - levelEvent_AddOneshotBeat.tick;
							}
							else if (levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Burnshot)
							{
								num4 = levelEvent_AddOneshotBeat.interval;
							}
						}
						else if (levelEvent is LevelEvent_SetRowXs && levelEvent.active && !list.Contains(levelEvent.row))
						{
							list.Add(levelEvent.row);
						}
						BarAndBeat barAndBeatWithPosX = base.timeline.GetBarAndBeatWithPosX(selectedControl3.transform.localPosition.x + num2, null, num4 * (float)base.cellWidth);
						selectedControl3.bar = barAndBeatWithPosX.bar;
						selectedControl3.beat = barAndBeatWithPosX.beat;
						selectedControl3.UpdateUI();
					}
					UpdateInspectorPanelIfNecessary();
				}
				string sound = ((base.editor.selectedControls.Count > 1) ? "sndEditorEventMoveMultiple" : "sndEditorEventMove");
				PlayPassiveSoundIfCooldown(sound);
			}
			float num5 = YPositionInsideCell - (float)(currentCellY * base.cellHeight);
			if (num5 > (float)base.cellHeight || num5 < 0f)
			{
				int num6 = SnapYPositionToGrid((float)currentPosY + num5) - currentPosY;
				bool flag = true;
				foreach (LevelEventControl_Base selectedControl4 in base.editor.selectedControls)
				{
					int num7 = selectedControl4.levelEvent.y + num6 / base.cellHeight;
					if (num7 < 0 || num7 > base.timeline.usedRowCount - 1)
					{
						flag = false;
					}
					if (base.editor.currentTab == Tab.Rooms && num7 >= 4)
					{
						flag = false;
					}
					else if (base.editor.currentTab == Tab.Windows && num7 >= 4)
					{
						flag = false;
					}
					else if (base.editor.currentTab == Tab.Decorations && num7 >= base.editor.currentPageDecorationsData.Count)
					{
						flag = false;
					}
				}
				if (flag)
				{
					foreach (LevelEventControl_Base eventControl in base.editor.selectedControls)
					{
						LevelEvent_Base levelEvent2 = eventControl.levelEvent;
						int num8 = (int)Mathf.Abs(eventControl.transform.localPosition.y);
						int num9 = (LastRowHeaderIndex = base.timeline.GetRowIndexWithPosY(-num8 - num6));
						if ((control is LevelEventControl_AddClassicBeat || control is LevelEventControl_SetRowXs || control is LevelEventControl_RowAction) && base.editor.currentPageRowsData.Count > num9)
						{
							LevelEvent_MakeRow levelEvent_MakeRow = base.editor.currentPageRowsData[num9];
							bool num10 = levelEvent_MakeRow.rowType == RowType.Classic || levelEvent_MakeRow.rowType == RowType.Hold;
							bool flag2 = levelEvent_MakeRow.rowType == RowType.Oneshot;
							bool flag3 = (num10 && levelEvent2.isClassicRowEvent) || (flag2 && levelEvent2.isOneshotRowEvent);
							if (RDBase.isAdvanced)
							{
								flag3 = true;
							}
							if (!flag3)
							{
								LastRowHeaderTypeIcon.color = RDConstants.data.rowHeaderIncompatibleColor;
								continue;
							}
							int row = levelEvent2.row;
							int rowDataIndex = RowHeader.GetRowDataIndex(num9, base.rowsTab.pageIndex);
							if (levelEvent2 is LevelEvent_SetRowXs && levelEvent2.active)
							{
								if (!list.Contains(row))
								{
									list.Add(row);
								}
								if (!list.Contains(rowDataIndex))
								{
									list.Add(rowDataIndex);
								}
							}
							eventControl.container.Remove(eventControl);
							levelEvent2.row = rowDataIndex;
							levelEvent2.inspectorPanel.UpdateUI(levelEvent2);
							eventControl.container.Add(eventControl);
						}
						else if (control is LevelEventControl_Decoration || (control.levelEvent is LevelEvent_Comment && control.levelEvent.tab == Tab.Decorations))
						{
							string target = levelEvent2.target;
							int pageIndex = base.decorationsTab.pageIndex;
							int num11 = 0;
							foreach (LevelEvent_MakeDecorationBase decorationsDatum in base.editor.decorationsData)
							{
								if (pageIndex == decorationsDatum.room)
								{
									if (num11 == num9)
									{
										levelEvent2.target = decorationsDatum.decorationId;
										break;
									}
									num11++;
								}
							}
							if (target != levelEvent2.target)
							{
								if (target.IsNullOrEmpty())
								{
									int num12 = 0;
									using (List<List<LevelEventControl_Base>>.Enumerator enumerator3 = base.editor.eventControls_decorations.GetEnumerator())
									{
										while (enumerator3.MoveNext() && enumerator3.Current.FindIndex((LevelEventControl_Base ec) => ec == eventControl) == -1)
										{
											num12++;
										}
									}
									base.editor.eventControls_decorations[num12].Remove(eventControl);
								}
								else
								{
									int decorationDataIndex = DecorationHeader.GetDecorationDataIndex(target);
									base.editor.eventControls_decorations[decorationDataIndex].Remove(eventControl);
								}
								int decorationDataIndex2 = DecorationHeader.GetDecorationDataIndex(levelEvent2.target);
								base.editor.eventControls_decorations[decorationDataIndex2].Add(eventControl);
								levelEvent2.row = decorationDataIndex2;
							}
						}
						else if (control.levelEvent.tab == Tab.Windows)
						{
							levelEvent2.rooms = new int[1] { num9 };
						}
						levelEvent2.y = num9;
						eventControl.UpdateUI();
						string sound2 = ((base.editor.selectedControls.Count > 1) ? "sndEditorEventMoveMultiple" : "sndEditorEventMove");
						PlayPassiveSoundIfCooldown(sound2);
					}
					UpdateInspectorPanelIfNecessary();
				}
			}
			foreach (int item in list)
			{
				base.timeline.UpdateClassicControlsForRow(item);
			}
		}

		private void UpdateInspectorPanelIfNecessary()
		{
			if (base.editor.selectedControls.Count == 1)
			{
				LevelEvent_Base levelEvent = base.editor.selectedControl.levelEvent;
				if (!levelEvent.isBaseEvent)
				{
					levelEvent.inspectorPanel.Show(base.editor.selectedControl);
				}
			}
		}

		public override void OnEndDrag(PointerEventData data)
		{
			base.timeline.setCrotchetsPerBarEventsTemp = null;
			foreach (LevelEventControl_Base selectedControl in base.editor.selectedControls)
			{
				selectedControl.levelEvent.isDragging = false;
				selectedControl.trigger.isDragging = false;
				if (control is LevelEventControl_AddClassicBeat && control.levelEvent is LevelEvent_AddOneshotBeat)
				{
					(control.levelEvent as LevelEvent_AddOneshotBeat).Validate();
				}
				selectedControl.UpdateUI();
				selectedControl.ShowBorders(show: false);
				base.editor.UpdateTimelineAccordingToLevelEventType(selectedControl.levelEvent.type);
			}
			if (LastRowHeaderIndex > 0)
			{
				LastRowHeaderTypeIcon.color = RDConstants.data.rowHeaderDefaultColor;
			}
			LastRowHeaderIndex = -1;
		}

		public override void OnPointerClick(PointerEventData data)
		{
			bool sound = true;
			if (isDragging)
			{
				return;
			}
			if (data.button == PointerEventData.InputButton.Right)
			{
				if (base.editor.selectedControls.Count > 1)
				{
					base.editor.DeselectAllEventControls(updateInspectorUI: true, sound: true);
				}
				else
				{
					base.editor.DeleteEventControl(control, selectControlToTheLeft: false, sound: true);
				}
			}
			else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftMeta) || Input.GetKey(KeyCode.RightMeta))
			{
				base.editor.AddEventControlToSelection(control, sound: true);
			}
			else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				List<int> list = new List<int>();
				if (!base.editor.selectedControls.Contains(control))
				{
					LevelEvent_Base levelEvent = control.levelEvent;
					levelEvent.active = !levelEvent.active;
					control.UpdateUI();
					if (levelEvent is LevelEvent_SetRowXs && !list.Contains(levelEvent.row))
					{
						list.Add(levelEvent.row);
					}
					if (levelEvent.active)
					{
						base.editor.LevelEditorPlaySound("sndEditorEventEnable");
					}
					else
					{
						base.editor.LevelEditorPlaySound("sndEditorEventDisable");
					}
				}
				else
				{
					int num = 0;
					int num2 = 0;
					foreach (LevelEventControl_Base selectedControl in base.editor.selectedControls)
					{
						LevelEvent_Base levelEvent2 = selectedControl.levelEvent;
						levelEvent2.active = !levelEvent2.active;
						selectedControl.UpdateUI();
						if (levelEvent2.active)
						{
							num2++;
						}
						else
						{
							num++;
						}
						if (levelEvent2 is LevelEvent_SetRowXs && !list.Contains(levelEvent2.row))
						{
							list.Add(levelEvent2.row);
						}
					}
					if (num2 > num)
					{
						base.editor.LevelEditorPlaySound("sndEditorEventEnable");
					}
					else
					{
						base.editor.LevelEditorPlaySound("sndEditorEventDisable");
					}
				}
				foreach (int item in list)
				{
					base.timeline.UpdateClassicControlsForRow(item);
				}
			}
			else
			{
				PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
				{
					pointerId = -1
				};
				pointerEventData.position = Input.mousePosition;
				List<RaycastResult> list2 = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointerEventData, list2);
				List<LevelEventControl_Base> list3 = new List<LevelEventControl_Base>();
				foreach (RaycastResult item2 in list2)
				{
					LevelEventControl_Base component = item2.gameObject.GetComponent<LevelEventControl_Base>();
					if (component != null)
					{
						list3.Add(component);
					}
				}
				LevelEventControl_Base levelEventControl_Base = null;
				LevelEvent_Base levelEvent3 = control.levelEvent;
				if (levelEvent3 is LevelEvent_PlaySound || levelEvent3 is LevelEvent_SetBeatSound)
				{
					sound = false;
				}
				if (eventControls == null || eventControls.Except(list3).Any() || list3.Except(eventControls).Any())
				{
					eventControls = list3;
					levelEventControl_Base = control;
					base.editor.SelectEventControl(levelEventControl_Base, sound);
				}
				else
				{
					int num3 = eventControls.IndexOf(base.editor.selectedControl);
					num3++;
					if (num3 >= eventControls.Count)
					{
						num3 = 0;
					}
					if (eventControls.Count > num3)
					{
						levelEventControl_Base = eventControls[num3];
						base.editor.SelectEventControl(levelEventControl_Base, sound);
					}
				}
				if (levelEventControl_Base != null && levelEventControl_Base.levelEvent is LevelEvent_AddOneshotBeat)
				{
					(levelEventControl_Base as LevelEventControl_AddClassicBeat).loop.gameObject.SetActive(value: true);
				}
				if (control.levelEvent is LevelEvent_PlaySound levelEvent_PlaySound)
				{
					levelEvent_PlaySound.PreviewSound();
				}
				if (control.levelEvent is LevelEvent_SetBeatSound levelEvent_SetBeatSound)
				{
					levelEvent_SetBeatSound.PreviewSound();
				}
			}
			_ = Time.unscaledTime;
			if (Time.unscaledTime - lastClickTime < 0.5f)
			{
				control.OnDoubleClick();
			}
			lastClickTime = Time.unscaledTime;
		}

		public void PlayPassiveSoundIfCooldown(string sound, float pitch = 1f)
		{
			if (soundTriggerTimer <= 0f)
			{
				scrConductor.PlayImmediately(sound, 1f, RDUtils.GetMixerGroup("LevelEditorPassive"), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
				soundTriggerTimer = soundTriggerCooldown;
			}
		}

		public float PitchFromTick(float beginTick, float newTick)
		{
			return 1f;
		}

		protected void DragBeat(LevelEventControlEventTrigger rootBeat, DraggingPosition draggingPosition)
		{
			bool playSound = this == rootBeat;
			if (rootBeat.control.levelEvent.type != control.levelEvent.type || rootBeat.control.levelEvent.row != control.levelEvent.row)
			{
				return;
			}
			if (control.levelEvent is LevelEvent_AddClassicBeat)
			{
				switch (draggingPosition)
				{
				case DraggingPosition.Left:
					DragClassicFromLeft(playSound);
					break;
				case DraggingPosition.Right:
					DragClassicFromRight(playSound);
					break;
				}
			}
			else if (control.levelEvent is LevelEvent_AddOneshotBeat)
			{
				switch (draggingPosition)
				{
				case DraggingPosition.Left:
					DragOneshotFromLeft(playSound);
					break;
				case DraggingPosition.Right:
					DragOneshotFromRight(playSound);
					break;
				case DraggingPosition.TopRight:
					DragOneshotFromTopRight(playSound);
					break;
				}
			}
		}

		protected void DragClassicFromLeft(bool playSound)
		{
			SnapXPositionToGrid(XPositionInsideCell);
			LevelEventControl_AddClassicBeat levelEventControl_AddClassicBeat = control as LevelEventControl_AddClassicBeat;
			LevelEvent_AddClassicBeat levelEvent_AddClassicBeat = control.levelEvent as LevelEvent_AddClassicBeat;
			float num = currentPosX;
			currentPosX = Mathf.Max(SnapXPositionToGrid(base.timeline.MousePositionOnTimeline.x), 0f);
			levelEventControl_AddClassicBeat.rt.anchoredPosition = levelEventControl_AddClassicBeat.rt.anchoredPosition.WithX(currentPosX);
			levelEventControl_AddClassicBeat.rt.sizeDelta = levelEventControl_AddClassicBeat.rt.sizeDelta.WithX(levelEventControl_AddClassicBeat.rt.sizeDelta.x + (num - currentPosX));
			levelEventControl_AddClassicBeat.hitPulse.anchoredPosition = levelEventControl_AddClassicBeat.hitPulse.anchoredPosition.WithX(levelEventControl_AddClassicBeat.rt.sizeDelta.x * (float)(levelEvent_AddClassicBeat.GetLength() - 1));
			BarAndBeat barAndBeatWithPosX = base.timeline.GetBarAndBeatWithPosX(levelEventControl_AddClassicBeat.rt.anchoredPosition.x);
			control.bar = barAndBeatWithPosX.bar;
			control.beat = barAndBeatWithPosX.beat;
			float num2 = 0f;
			LevelEvent_SetRowXs closestSetBeatModifiersEventWithRow = base.timeline.GetClosestSetBeatModifiersEventWithRow(levelEvent_AddClassicBeat.row, barAndBeatWithPosX);
			if (closestSetBeatModifiersEventWithRow != null && closestSetBeatModifiersEventWithRow.syncoBeat != -1)
			{
				float syncoSwing = closestSetBeatModifiersEventWithRow.syncoSwing;
				num2 = ((!(syncoSwing > 0f)) ? (-0.5f) : (0f - syncoSwing));
			}
			int num3 = levelEvent_AddClassicBeat.GetLength() - 1;
			float num4 = levelEventControl_AddClassicBeat.rt.sizeDelta.x / (float)base.cellWidth / (float)num3;
			num4 *= (float)num3 / ((float)num3 + num2);
			if (playSound)
			{
				PlayPassiveSoundIfCooldown("sndEditorEventStretch", PitchFromTick(levelEvent_AddClassicBeat.tick, num4));
			}
			levelEvent_AddClassicBeat.tick = num4;
			levelEvent_AddClassicBeat.Validate();
			(levelEvent_AddClassicBeat.inspectorPanel as InspectorPanel_AddClassicBeat).UpdateUI(levelEvent_AddClassicBeat);
			levelEventControl_AddClassicBeat.UpdateUI();
		}

		protected void DragClassicFromRight(bool playSound)
		{
			LevelEventControl_AddClassicBeat levelEventControl_AddClassicBeat = control as LevelEventControl_AddClassicBeat;
			LevelEvent_AddClassicBeat levelEvent_AddClassicBeat = control.levelEvent as LevelEvent_AddClassicBeat;
			float num = SnapXPositionToGrid(XPositionInsideCell);
			levelEventControl_AddClassicBeat.hitPulse.anchoredPosition = levelEventControl_AddClassicBeat.hitPulse.anchoredPosition.WithX(num);
			int num2 = levelEvent_AddClassicBeat.GetLength() - 1;
			float num3 = num / (float)base.cellWidth / (float)num2;
			num3 *= (float)num2 / ((float)num2 + levelEventControl_AddClassicBeat.syncoOffset);
			if (playSound)
			{
				PlayPassiveSoundIfCooldown("sndEditorEventStretch", PitchFromTick(levelEvent_AddClassicBeat.tick, num3));
			}
			levelEvent_AddClassicBeat.tick = num3;
			levelEvent_AddClassicBeat.Validate();
			(levelEvent_AddClassicBeat.inspectorPanel as InspectorPanel_AddClassicBeat).UpdateUI(levelEvent_AddClassicBeat);
			levelEventControl_AddClassicBeat.UpdateUI();
		}

		protected void DragOneshotFromLeft(bool playSound)
		{
			LevelEventControl_AddClassicBeat levelEventControl_AddClassicBeat = control as LevelEventControl_AddClassicBeat;
			LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = control.levelEvent as LevelEvent_AddOneshotBeat;
			SnapXPositionToGrid(XPositionInsideCell);
			float num = currentPosX;
			currentPosX = Mathf.Max(SnapXPositionToGrid(base.timeline.MousePositionOnTimeline.x), 0f);
			levelEventControl_AddClassicBeat.rt.anchoredPosition = levelEventControl_AddClassicBeat.rt.anchoredPosition.WithX(currentPosX);
			levelEventControl_AddClassicBeat.rt.sizeDelta = levelEventControl_AddClassicBeat.rt.sizeDelta.WithX(levelEventControl_AddClassicBeat.rt.sizeDelta.x + (num - currentPosX));
			levelEventControl_AddClassicBeat.hitPulse.anchoredPosition = levelEventControl_AddClassicBeat.hitPulse.anchoredPosition.WithX(levelEventControl_AddClassicBeat.rt.sizeDelta.x * 6f);
			float num2 = ((levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Burnshot) ? levelEvent_AddOneshotBeat.interval : 0f);
			BarAndBeat barAndBeatWithPosX = base.timeline.GetBarAndBeatWithPosX(levelEventControl_AddClassicBeat.rt.anchoredPosition.x, null, num2 * (float)base.cellWidth);
			control.bar = barAndBeatWithPosX.bar;
			control.beat = barAndBeatWithPosX.beat;
			float num3 = levelEventControl_AddClassicBeat.rt.sizeDelta.x / (float)base.cellWidth - ((float)levelEvent_AddOneshotBeat.loops * levelEvent_AddOneshotBeat.interval + levelEvent_AddOneshotBeat.actualDelay);
			if (playSound)
			{
				PlayPassiveSoundIfCooldown("sndEditorEventStretch", PitchFromTick(levelEvent_AddOneshotBeat.tick, num3));
			}
			levelEvent_AddOneshotBeat.tick = num3;
			levelEvent_AddOneshotBeat.Validate();
			(levelEvent_AddOneshotBeat.inspectorPanel as InspectorPanel_AddOneshotBeat).UpdateUI(levelEvent_AddOneshotBeat);
			levelEventControl_AddClassicBeat.UpdateUI();
		}

		protected void DragOneshotFromRight(bool playSound)
		{
			LevelEventControl_AddClassicBeat levelEventControl_AddClassicBeat = control as LevelEventControl_AddClassicBeat;
			LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = control.levelEvent as LevelEvent_AddOneshotBeat;
			float num = SnapXPositionToGrid(XPositionInsideCell);
			levelEventControl_AddClassicBeat.hitPulse.anchoredPosition = levelEventControl_AddClassicBeat.hitPulse.anchoredPosition.WithX(num - (float)base.cellWidth);
			float num2 = num / (float)base.cellWidth - ((float)levelEvent_AddOneshotBeat.loops * levelEvent_AddOneshotBeat.interval + levelEvent_AddOneshotBeat.actualDelay);
			if (levelEvent_AddOneshotBeat.freezeBurnMode == FreezeBurnMode.Freezeshot)
			{
				num2 = Mathf.Max(num2, levelEvent_AddOneshotBeat.interval - currentPosX / (float)base.cellWidth);
			}
			if (playSound)
			{
				PlayPassiveSoundIfCooldown("sndEditorEventStretch", PitchFromTick(levelEvent_AddOneshotBeat.tick, num2));
			}
			levelEvent_AddOneshotBeat.tick = num2;
			levelEvent_AddOneshotBeat.Validate();
			(levelEvent_AddOneshotBeat.inspectorPanel as InspectorPanel_AddOneshotBeat).UpdateUI(levelEvent_AddOneshotBeat);
			levelEventControl_AddClassicBeat.UpdateUI();
		}

		protected void DragOneshotFromTopRight(bool playSound)
		{
			LevelEventControl_AddClassicBeat obj = control as LevelEventControl_AddClassicBeat;
			LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = control.levelEvent as LevelEvent_AddOneshotBeat;
			int num = Mathf.Max(Mathf.RoundToInt(SnapXPositionToGrid(XPositionInsideCell) / ((float)base.cellWidth * levelEvent_AddOneshotBeat.interval)), 0);
			if (num != levelEvent_AddOneshotBeat.loops)
			{
				float num2 = ((num > levelEvent_AddOneshotBeat.loops) ? 1.2f : 1f);
				float pitch = num2;
				scrConductor.PlayImmediately("sndEditorEventLoop", 1f, RDUtils.GetMixerGroup("LevelEditorPassive"), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
			levelEvent_AddOneshotBeat.loops = num;
			levelEvent_AddOneshotBeat.inspectorPanel.UpdateUI(levelEvent_AddOneshotBeat);
			obj.UpdateUI();
		}
	}
}
