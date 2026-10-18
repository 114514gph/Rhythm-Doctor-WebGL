using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class TabSection : RDEditorBase
	{
		[Header("Tab Section")]
		public Tab tab;

		public RectTransform tabRT;

		public RectTransform tabPanel;

		public Toggle[] pageToggles;

		public Button tabButton;

		[NonSerialized]
		public LevelEventType[] availableEvents;

		[NonSerialized]
		public RDEditorKeycode[] shortcuts;

		[NonSerialized]
		public GameObject[] container;

		[NonSerialized]
		public int pageIndex;

		public int tabIndex => (int)tab;

		public Color color => base.gc.colorPalette[tabIndex];

		public Transform containerTransform => container[pageIndex].transform;

		public string tabName => tab.ToString();

		public virtual void Setup()
		{
			List<LevelEventType> list = new List<LevelEventType>();
			foreach (LevelEventType item2 in RDEditorConstants.levelEventTabs[tab])
			{
				list.Add(item2);
			}
			availableEvents = list.ToArray();
			List<RDEditorKeycode> list2 = new List<RDEditorKeycode>();
			LevelEventType[] array = availableEvents;
			foreach (LevelEventType item in array)
			{
				int index = RDEditorConstants.levelEventTabs[tab].IndexOf(item);
				list2.Add(RDEditorConstants.eventKeyCodes[tab][index]);
			}
			shortcuts = list2.ToArray();
			if (this is TabSection_Rows || this is TabSection_Decorations)
			{
				container = new GameObject[4];
				for (int j = 0; j < 4; j++)
				{
					container[j] = GameObject.Find("LevelEventControls_" + tabName + j);
				}
			}
			else
			{
				container = new GameObject[1];
				container[0] = GameObject.Find("LevelEventControls_" + tabName);
			}
			Hide();
		}

		public void Show(bool animated = true)
		{
			SetVisible(visible: true, animated);
		}

		public void Hide(bool animated = true)
		{
			SetVisible(visible: false, animated);
		}

		private void SetVisible(bool visible, bool animated)
		{
			GameObject[] array = container;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			if (visible)
			{
				container[pageIndex].SetActive(value: true);
				UpdateUI();
			}
			tabPanel.gameObject.SetActive(visible);
			RectTransform rectTransform = tabRT;
			float x = (visible ? 20f : 15f);
			Vector2 vector = rectTransform.sizeDelta.WithX(x);
			rectTransform.DOKill();
			if (animated)
			{
				rectTransform.DOSizeDelta(vector, 0.1f).SetUpdate(UpdateType.Normal, isIndependentUpdate: true).SetId("noKillOnReload");
			}
			else
			{
				rectTransform.sizeDelta = vector;
			}
			Color color = (visible ? Color.white : 0.85f.GrayColor());
			tabButton.GetComponent<Button>().SetNormalColor(color);
			UpdatePanelAppeareance();
		}

		public void ChangePageExternal(int index)
		{
			ChangePage(index, calledByUnity: true);
		}

		public void ChangePage(int index, bool calledByUnity)
		{
			using (new SaveStateScope())
			{
				if (base.editor.currentTabSection != this)
				{
					base.editor.ShowTabSection(tab, animateTab: true, playSound: true);
				}
				if (!pageToggles[index].isOn && !calledByUnity)
				{
					pageToggles[index].isOn = true;
					base.editor.ClearHoveredEventControl();
				}
				container[pageIndex].SetActive(value: false);
				pageIndex = index;
				container[pageIndex].SetActive(value: true);
				if (tab == Tab.Rows)
				{
					foreach (LevelEvent_MakeRow rowsDatum in base.editor.rowsData)
					{
						if (rowsDatum.room == pageIndex)
						{
							RowHeader component = base.tabSection_rows.rowHeaders[0].GetComponent<RowHeader>();
							if (component.isActiveAndEnabled)
							{
								component.Select();
							}
							break;
						}
					}
				}
				else if (tab == Tab.Decorations)
				{
					base.tabSection_decorations.UpdateHeadersToPage(pageIndex);
					foreach (LevelEvent_MakeDecorationBase decorationsDatum in base.editor.decorationsData)
					{
						if (decorationsDatum.room == pageIndex)
						{
							DecorationHeader component2 = base.tabSection_decorations.decorationHeaders[0].GetComponent<DecorationHeader>();
							if (component2.isActiveAndEnabled)
							{
								component2.Select();
							}
							break;
						}
					}
				}
			}
			UpdateUI();
		}

		private void UpdatePanelAppeareance()
		{
		}

		public void ShowNothingSelectedPanel()
		{
			string text = RDString.Get("editor.noEventSelected");
			base.ipm.ShowBlankPanel(text);
		}

		public void ShowMultipleSelectedPanel()
		{
			base.ipm.ShowBlankPanel();
			base.ipm.ShowBulkPanel();
		}

		public virtual void UpdateUI()
		{
		}

		public virtual string DefaultControlName()
		{
			return null;
		}

		public GameObject GetNewControl()
		{
			return UnityEngine.Object.Instantiate(Resources.Load<GameObject>("LevelEventControl_" + DefaultControlName()));
		}

		public virtual void LateUpdate()
		{
			int num = 0;
			if (tab != Tab.Rows && tab != Tab.Decorations)
			{
				List<LevelEventControl_Base> list = null;
				if (tab == Tab.Actions)
				{
					list = base.editor.eventControls_actions;
				}
				else if (tab == Tab.Song)
				{
					list = base.editor.eventControls_sounds;
				}
				else if (tab == Tab.Rooms)
				{
					list = base.editor.eventControls_rooms;
				}
				else if (tab == Tab.Windows)
				{
					list = base.editor.eventControls_windows;
				}
				if (list == null)
				{
					return;
				}
				{
					foreach (LevelEventControl_Base item in list)
					{
						if (item.shouldUpdateUI)
						{
							item.UpdateUIInternal();
							num++;
						}
					}
					return;
				}
			}
			if (tab == Tab.Rows)
			{
				foreach (List<LevelEventControl_Base> eventControls_row in base.editor.eventControls_rows)
				{
					foreach (LevelEventControl_Base item2 in eventControls_row)
					{
						if (item2.shouldUpdateUI)
						{
							item2.UpdateUIInternal();
							num++;
						}
					}
				}
				return;
			}
			foreach (List<LevelEventControl_Base> eventControls_decoration in base.editor.eventControls_decorations)
			{
				foreach (LevelEventControl_Base item3 in eventControls_decoration)
				{
					if (item3.shouldUpdateUI)
					{
						item3.UpdateUIInternal();
						num++;
					}
				}
			}
		}
	}
}
