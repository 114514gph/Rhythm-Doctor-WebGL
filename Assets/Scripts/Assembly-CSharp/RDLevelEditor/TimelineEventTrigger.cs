using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDLevelEditor
{
	public class TimelineEventTrigger : RDEventTrigger, IScrollHandler, IEventSystemHandler
	{
		public override void OnPointerClick(PointerEventData data)
		{
			if (isDragging)
			{
				return;
			}
			Int2 cellPointedByMouse = base.timeline.cellPointedByMouse;
			int num = base.timeline.cellWidth * cellPointedByMouse.x;
			bool flag = cellPointedByMouse.y == -1 || base.timeline.mouseIsOnNumbers;
			if (data.button == PointerEventData.InputButton.Left)
			{
				int y = cellPointedByMouse.y;
				if (flag)
				{
					BarAndBeat barAndBeat = new BarAndBeat(base.timeline.GetBarAndBeatWithPosX(num).bar, 1f);
					base.editor.ScrubToBar(barAndBeat.bar);
				}
				else
				{
					if (y > base.timeline.usedRowCount)
					{
						return;
					}
					BarAndBeat barAndBeatWithPosX = base.timeline.GetBarAndBeatWithPosX(num);
					int num2 = y;
					Tab currentTab = base.editor.currentTab;
					if (currentTab == Tab.Rows)
					{
						int rowDataIndex = RowHeader.GetRowDataIndex(y, base.rowsTab.pageIndex);
						if (rowDataIndex >= base.editor.eventControls_rows.Count)
						{
							return;
						}
						RowType rowType = base.editor.rowsData[rowDataIndex].rowType;
						base.editor.lastUsedRow = rowDataIndex;
						LevelEvent_Base levelEvent_Base = rowType switch
						{
							RowType.Oneshot => new LevelEvent_AddOneshotBeat(), 
							RowType.Classic => new LevelEvent_AddClassicBeat(), 
							_ => null, 
						};
						if (levelEvent_Base != null)
						{
							levelEvent_Base.OnCreate();
							levelEvent_Base.row = rowDataIndex;
							levelEvent_Base.barAndBeat = barAndBeatWithPosX;
							LevelEventControl_Base levelEventControl_Base = base.editor.CreateEventControl(levelEvent_Base, base.editor.currentTab);
							LevelEventControl_Base selectedControl = base.editor.selectedControl;
							if (selectedControl != null && selectedControl.levelEvent.GetType() == levelEvent_Base.GetType())
							{
								levelEventControl_Base.levelEvent.CopyFromInternal(selectedControl.levelEvent);
								if (levelEvent_Base is LevelEvent_AddOneshotBeat)
								{
									(levelEvent_Base as LevelEvent_AddOneshotBeat).CapXPosForPositiveFreezeBurnCueTime();
								}
							}
							base.editor.SelectEventControl(levelEventControl_Base);
							levelEventControl_Base.UpdateUI();
							scrConductor.PlayImmediately("sndEditorEventCreate", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
							levelEvent_Base.y = levelEventControl_Base.visualRowIndex;
						}
						else
						{
							Debug.LogWarning($"We don't yet support {rowType} row type");
						}
					}
					else if (currentTab != Tab.Rows)
					{
						if ((currentTab == Tab.Rooms && y >= 4) || (currentTab == Tab.Windows && y >= 4))
						{
							return;
						}
						if (currentTab == Tab.Decorations)
						{
							int decorationDataIndex = DecorationHeader.GetDecorationDataIndex(y, base.decorationsTab.pageIndex);
							if (base.editor.decorationsData == null || base.editor.decorationsData.Count == 0 || decorationDataIndex >= base.editor.decorationsData.Count)
							{
								RDBaseDllDummy.printem("no decorations or index is too high!");
								return;
							}
							base.editor.lastUsedDecoration = base.editor.decorationsData[decorationDataIndex].decorationId;
						}
						GameObject newControl = base.editor.currentTabSection.GetNewControl();
						LevelEventControl_Base component = newControl.GetComponent<LevelEventControl_Base>();
						component.levelEvent.barAndBeat = barAndBeatWithPosX;
						if (currentTab == Tab.Decorations)
						{
							int decorationDataIndex2 = DecorationHeader.GetDecorationDataIndex(y, base.decorationsTab.pageIndex);
							component.levelEvent.row = decorationDataIndex2;
						}
						newControl.GetComponent<RectTransform>();
						Tab currentTab2 = base.editor.currentTab;
						base.editor.AddNewEventControl(component, currentTab2);
						base.editor.SelectEventControl(component);
						component.UpdateUI();
						scrConductor.PlayImmediately("sndEditorEventCreate", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
						component.levelEvent.y = num2;
						switch (currentTab)
						{
						case Tab.Actions:
						{
							List<SelectLevelEventButton> list = base.editor.inspectorPanelManager.eventListPanels[2].GetComponent<SelectLevelEventPanel>().events.FindAll((SelectLevelEventButton levelEvent) => levelEvent.selected);
							if (list.Count == 1)
							{
								base.editor.SetLevelEventControlType(list[0].type);
							}
							break;
						}
						case Tab.Windows:
							component.levelEvent.rooms = new int[1] { num2 };
							break;
						}
					}
					else
					{
						MonoBehaviour.print("non important tab YET");
					}
				}
			}
			else
			{
				if (data.button != PointerEventData.InputButton.Right)
				{
					return;
				}
				if (flag)
				{
					using (new SaveStateScope())
					{
						if (base.timeline.IsMousePointerCloserToCellOnRight)
						{
							num += base.cellWidth;
						}
						tlBookmark tlBookmark2 = null;
						foreach (tlBookmark bookmark in base.timeline.bookmarks)
						{
							if (bookmark.rt.anchoredPosition.x == (float)num)
							{
								tlBookmark2 = bookmark;
								break;
							}
						}
						if (tlBookmark2 != null)
						{
							if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
							{
								tlBookmark2.GetComponent<tlBookmark>().NextColor();
								scrConductor.PlayImmediately("sndEditorBookmarkJump", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
							}
							else if (RDEditorUtils.ControlIsPressed())
							{
								tlBookmark2.GetComponent<tlBookmark>().PreviousColor();
								scrConductor.PlayImmediately("sndEditorBookmarkJump", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
							}
							else
							{
								GameObject obj = tlBookmark2.gameObject;
								base.timeline.bookmarks.Remove(tlBookmark2);
								tlBookmark2.DestroyLine();
								Object.Destroy(obj);
								scrConductor.PlayImmediately("sndEditorBookmarkDelete", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
								base.timeline.UpdateUI(updateControls: false);
							}
						}
						else
						{
							base.timeline.AddBookmark(num);
							scrConductor.PlayImmediately("sndEditorBookmarkCreate", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
							base.timeline.UpdateUI(updateControls: false);
						}
						return;
					}
				}
				base.editor.DeselectAllEventControls(updateInspectorUI: true, sound: true);
			}
		}

		public override void OnBeginDrag(PointerEventData data)
		{
			base.OnBeginDrag(data);
			base.timeline.UnfollowPlayhead();
			if (data.button == PointerEventData.InputButton.Right)
			{
				base.editor.SaveState();
				base.editor.changingState++;
			}
		}

		public override void OnDrag(PointerEventData data)
		{
			base.OnDrag(data);
			Vector2 mousePositionOnTimeline = base.timeline.MousePositionOnTimeline;
			if (data.button != PointerEventData.InputButton.Right)
			{
				return;
			}
			List<LevelEventControl_Base> list = new List<LevelEventControl_Base>();
			foreach (Transform item in base.editor.currentTabSection.containerTransform)
			{
				if (item.gameObject.activeSelf)
				{
					RectTransform component = item.GetComponent<RectTransform>();
					Vector2 anchoredPosition = component.anchoredPosition;
					Vector2 sizeDelta = component.sizeDelta;
					if (mousePositionOnTimeline.x >= anchoredPosition.x && mousePositionOnTimeline.x <= anchoredPosition.x + sizeDelta.x && mousePositionOnTimeline.y >= 0f - anchoredPosition.y && mousePositionOnTimeline.y <= 0f - anchoredPosition.y + sizeDelta.y)
					{
						list.Add(item.GetComponent<LevelEventControl_Base>());
					}
				}
			}
			if (list.Count != 0)
			{
				base.editor.DeleteEventControlsFromList(list);
				scrConductor.PlayImmediately("sndEditorEventDelete", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
		}

		public override void OnEndDrag(PointerEventData data)
		{
			base.OnEndDrag(data);
			if (data.button == PointerEventData.InputButton.Right)
			{
				base.editor.changingState--;
			}
		}

		public void OnScroll(PointerEventData data)
		{
			if (!Input.GetKey(KeyCode.LeftShift))
			{
				base.timeline.UnfollowPlayhead();
			}
		}

		public override void OnPointerEnter(PointerEventData data)
		{
			base.OnPointerEnter(data);
		}

		public override void OnPointerExit(PointerEventData data)
		{
			base.OnPointerExit(data);
		}
	}
}
