using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class SelectLevelEventPanel : RDEditorBase
	{
		private const float YStart = -3f;

		private const float YSeparation = -20f;

		private const float YStartForRoomSelect = -5f;

		private const string SelectAllKey = "editor.selectAll";

		private const string SelectNoneKey = "editor.selectNone";

		private const string ShowAllKey = "editor.showAll";

		private const string ShowManyKey = "editor.showMany";

		private const string ShowNoneKey = "editor.showNone";

		public Tab willDisplayLevelEventsFromTab;

		public GameObject selectLevelEventButtonPrefab;

		public RectTransform content;

		public GameObject selectToggle;

		public GameObject categorySelect;

		[SerializeField]
		private bool hasHiddenCategories;

		public GameObject roomSelect;

		public Text showText;

		public Image[] showIcons;

		public Button[] roomButtons;

		public GridLayoutGroup gridGroup;

		public Button gridViewToggle;

		[NonSerialized]
		public List<SelectLevelEventButton> events = new List<SelectLevelEventButton>();

		[NonSerialized]
		public int lastSelectedEventsNumber;

		[NonSerialized]
		public int[] rooms = new int[5] { 0, 1, 2, 3, 4 };

		private Button selectToggleButton;

		private Dropdown categorySelectDropdown;

		private Text selectToggleText;

		private bool _setSelectButtonToAll;

		private Button gridViewButton;

		private Image gridViewIcon;

		private bool _gridView;

		private EventCategory currentCategory;

		private bool togglesEvents;

		private bool hasRoomSelect;

		private bool hasGridViewButton;

		private static Dictionary<Tab, EventCategory[]> categoriesForTab;

		private Dictionary<int, EventCategory> categoryIds;

		private bool SetSelectButtonToAll
		{
			get
			{
				return _setSelectButtonToAll;
			}
			set
			{
				if (_setSelectButtonToAll != value)
				{
					_setSelectButtonToAll = value;
					UpdateSelectToggle();
				}
			}
		}

		private bool GridView
		{
			get
			{
				return _gridView;
			}
			set
			{
				if (_gridView != value)
				{
					_gridView = value;
					switch (willDisplayLevelEventsFromTab)
					{
					case Tab.Song:
						Persistence.SetLevelEditorCollapseSoundEventList(value);
						break;
					case Tab.Actions:
						Persistence.SetLevelEditorCollapseActionEventList(value);
						break;
					}
					UpdateGridViewToggle();
				}
			}
		}

		private bool hasCategorySelect => categorySelect != null;

		private void Awake()
		{
			if (categoriesForTab == null)
			{
				CategoryDictInit();
			}
			int num = 0;
			TabSection tabSection = base.editor.tabSections[(int)willDisplayLevelEventsFromTab];
			Color color = base.gc.colorPalette[(int)willDisplayLevelEventsFromTab];
			LevelEventType[] availableEvents = tabSection.availableEvents;
			for (int i = 0; i < availableEvents.Length; i++)
			{
				LevelEventType type = availableEvents[i];
				if (RDEditorConstants.hiddenEvents[tabSection.tab].Contains(type))
				{
					num++;
					continue;
				}
				SelectLevelEventButton component = UnityEngine.Object.Instantiate(selectLevelEventButtonPrefab).GetComponent<SelectLevelEventButton>();
				component.rect.SetParent(content, worldPositionStays: false);
				string text = type.ToString();
				component.label.text = RDString.Get("editor." + text);
				RDStringToUIText.Apply(component.label);
				component.shortcut.text = tabSection.shortcuts[num].ToString().Replace("Alpha", "");
				component.shortcut.color = color;
				component.shortcutColor = color;
				component.icon.sprite = LevelEventControl_Base.GetIconFromName(text);
				component.button.onClick.AddListener(() =>
				{
					if (currentCategory == EventCategory.MakeDecoration)
					{
						base.editor.SetDecorationType(type);
					}
					else
					{
						if (type == LevelEventType.NewWindowDance && willDisplayLevelEventsFromTab == Tab.Actions && !base.editor.filterMode)
						{
							base.editor.MoveToWindowTab(createWindowDance: true);
							return;
						}
						base.editor.conditionalsPanel.currentPanel = null;
						bool copyRow = willDisplayLevelEventsFromTab == Tab.Decorations;
						base.editor.SetLevelEventControlType(type, copyRow);
					}
					LevelEditorPlaySound("sndEditorEventSetType");
				});
				component.isAdvanced = RDEditorConstants.advancedEvents[tabSection.tab].Contains(type);
				component.type = type;
				events.Add(component);
				component.selected = true;
				num++;
			}
			togglesEvents = selectToggle != null && showText != null && showIcons != null;
			if (togglesEvents)
			{
				selectToggleButton = selectToggle.GetComponent<Button>();
				selectToggleText = selectToggle.GetComponentInChildren<Text>();
				UpdateSelectToggle();
				lastSelectedEventsNumber = -1;
			}
			if (hasCategorySelect)
			{
				categorySelectDropdown = categorySelect.GetComponentInChildren<Dropdown>();
				if (RDString.isCJK)
				{
					categorySelectDropdown.template.GetComponentInChildren<Text>().font = RDString.currentLanguageFont;
					categorySelectDropdown.captionText.font = RDString.currentLanguageFont;
				}
				EventCategory[] values = categoriesForTab[willDisplayLevelEventsFromTab];
				categoryIds = categorySelectDropdown.AddOptionsWithEnumArray(values);
				categorySelectDropdown.onValueChanged.AddListener(delegate
				{
					ShowCategory(categorySelectDropdown.value);
				});
			}
			hasGridViewButton = gridViewToggle != null;
			if (hasGridViewButton)
			{
				switch (willDisplayLevelEventsFromTab)
				{
				case Tab.Song:
					_gridView = Persistence.GetLevelEditorCollapseSoundEventList();
					break;
				case Tab.Actions:
					_gridView = Persistence.GetLevelEditorCollapseActionEventList();
					break;
				}
				gridViewButton = gridViewToggle.GetComponent<Button>();
				gridViewButton.onClick.AddListener(() =>
				{
					GridView = !GridView;
					OnEnable();
				});
				gridViewIcon = gridViewToggle.GetComponentsInChildren<Image>().Last();
				UpdateGridViewToggle();
			}
			hasRoomSelect = roomSelect != null;
		}

		public void OnEnable()
		{
			int num = 0;
			if (hasCategorySelect)
			{
				RectTransform component = categorySelect.GetComponent<RectTransform>();
				component.anchoredPosition = component.anchoredPosition.WithY(-3f + -20f * (float)num);
				num++;
			}
			bool flag = false;
			if (hasRoomSelect)
			{
				flag = base.editor.eventControls_rooms.Count != 0 && base.editor.filterMode;
				roomSelect.SetActive(flag);
				if (flag)
				{
					BarBeatPosition.UpdateRoomButtons(roomButtons, rooms.ToList());
					if (base.editor.roomsSelectionPopup.gameObject.activeSelf)
					{
						base.editor.roomsSelectionPopup.UpdateUI();
					}
				}
			}
			if (togglesEvents)
			{
				selectToggle.SetActive(base.editor.filterMode);
				if (base.editor.filterMode)
				{
					RectTransform component2 = selectToggle.GetComponent<RectTransform>();
					component2.anchoredPosition = component2.anchoredPosition.WithY(-3f + -20f * (float)num);
					component2.offsetMax = component2.offsetMax.WithX(flag ? (-22) : (-7));
					if (flag)
					{
						RectTransform component3 = roomSelect.GetComponent<RectTransform>();
						component3.anchoredPosition = component3.anchoredPosition.WithY(-5f + -20f * (float)num);
					}
					num++;
				}
			}
			if (hasGridViewButton)
			{
				gridGroup.enabled = GridView;
				gridGroup.padding.top = 23 - (base.editor.filterMode ? Mathf.RoundToInt(-20f) : 0);
			}
			foreach (SelectLevelEventButton levelEvent in events)
			{
				if (base.editor.filterMode)
				{
					levelEvent.ShowAsSelected(levelEvent.selected);
				}
				else
				{
					levelEvent.ShowAsSelected(on: true);
				}
				bool flag2 = (!hasCategorySelect && !hasHiddenCategories) || currentCategory == EventCategory.All || Array.Exists(RDEditorConstants.eventCategories[currentCategory], (LevelEventType t) => t == levelEvent.type);
				if (!RDBase.isAdvanced && levelEvent.isAdvanced)
				{
					flag2 = false;
				}
				levelEvent.gameObject.SetActive(flag2);
				if (flag2)
				{
					if (!GridView)
					{
						levelEvent.rect.anchoredPosition = levelEvent.rect.anchoredPosition.WithY(-3f + -20f * (float)num);
					}
					levelEvent.SetGridMode(GridView);
					num++;
				}
			}
			float y;
			if (GridView)
			{
				float num2 = content.rect.width - (float)gridGroup.padding.left - (float)gridGroup.padding.right;
				float num3 = gridGroup.cellSize.x + gridGroup.spacing.x;
				int num4 = Mathf.FloorToInt(num2 / num3);
				int num5 = num;
				num5 -= (hasCategorySelect ? 1 : 0);
				num5 -= ((togglesEvents && base.editor.filterMode) ? 1 : 0);
				int num6 = num5 / num4;
				num6 += num - num5 + 1;
				y = Mathf.Abs(-3f + -20f * (float)num6);
			}
			else
			{
				y = Mathf.Abs(-3f + -20f * (float)num);
			}
			content.sizeDelta = content.sizeDelta.WithY(y);
			base.editor.inspectorPanelManager.UpdateContentHeight(content);
		}

		private void Update()
		{
			int num = 313;
			for (int i = 306; i <= num; i++)
			{
				if (Input.GetKey((KeyCode)i))
				{
					return;
				}
			}
			TabSection tabSection = base.editor.tabSections[(int)willDisplayLevelEventsFromTab];
			LevelEventType[] availableEvents = tabSection.availableEvents;
			RDEditorKeycode[] shortcuts = tabSection.shortcuts;
			bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			int num2 = 0;
			RDEditorKeycode[] array = shortcuts;
			for (int j = 0; j < array.Length; j++)
			{
				RDEditorKeycode rDEditorKeycode = array[j];
				if (Input.GetKeyDown(rDEditorKeycode.key) && rDEditorKeycode.shift == flag)
				{
					LevelEventType levelEventType = availableEvents[num2];
					if (ShouldUnlistEvent(tabSection.tab, levelEventType))
					{
						num2++;
						continue;
					}
					if (levelEventType == LevelEventType.NewWindowDance && willDisplayLevelEventsFromTab == Tab.Actions)
					{
						scnEditor.instance.MoveToWindowTab(createWindowDance: true);
						break;
					}
					if (!hasHiddenCategories || RDEditorConstants.eventCategories[currentCategory].Contains(levelEventType))
					{
						bool copyRow = willDisplayLevelEventsFromTab == Tab.Decorations;
						if (currentCategory == EventCategory.MakeDecoration)
						{
							base.editor.SetDecorationType(levelEventType);
						}
						else
						{
							base.editor.SetLevelEventControlType(levelEventType, copyRow);
						}
						LevelEditorPlaySound("sndEditorEventSetType");
						break;
					}
					num2++;
				}
				else
				{
					num2++;
				}
			}
			if (!togglesEvents)
			{
				return;
			}
			List<SelectLevelEventButton> list = events.FindAll((SelectLevelEventButton levelEvent) => levelEvent.selected);
			int count = list.Count;
			bool flag2 = count == events.Count;
			SetSelectButtonToAll = !flag2;
			if (count == lastSelectedEventsNumber)
			{
				return;
			}
			lastSelectedEventsNumber = count;
			if (flag2)
			{
				ShowAllEvents();
			}
			else if (count > 4)
			{
				showText.text = RDString.Get("editor.showMany");
				Image[] array2 = showIcons;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].gameObject.SetActive(value: false);
				}
			}
			else if (count > 0)
			{
				showText.text = string.Empty;
				for (int num3 = 0; num3 < showIcons.Length; num3++)
				{
					if (num3 >= count)
					{
						showIcons[num3].gameObject.SetActive(value: false);
						continue;
					}
					showIcons[num3].gameObject.SetActive(value: true);
					string levelEventName = list[num3].type.ToString();
					showIcons[num3].sprite = LevelEventControl_Base.GetIconFromName(levelEventName);
				}
			}
			else
			{
				showText.text = RDString.Get("editor.showNone");
				Image[] array2 = showIcons;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].gameObject.SetActive(value: false);
				}
			}
		}

		private void UpdateSelectToggle()
		{
			selectToggleButton.onClick.RemoveAllListeners();
			selectToggleText.text = (SetSelectButtonToAll ? RDString.Get("editor.selectAll") : RDString.Get("editor.selectNone"));
			selectToggleText.font = RDString.GetAppropiateFontForString(selectToggleText.text);
			selectToggleButton.onClick.AddListener(() =>
			{
				events.ForEach((SelectLevelEventButton levelEvent) =>
				{
					levelEvent.selected = SetSelectButtonToAll;
				});
			});
		}

		public void ShowCategory(int index)
		{
			ShowCategory(categoryIds[index]);
		}

		public void ShowCategory(EventCategory eventCategory)
		{
			currentCategory = eventCategory;
			OnEnable();
		}

		public void ShowAllEvents()
		{
			if (togglesEvents)
			{
				SetSelectButtonToAll = false;
				showText.text = RDString.Get("editor.showAll");
				Image[] array = showIcons;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
			}
		}

		public void ShowRoomsSelectionPopup()
		{
			RDRoomsSelectionPopup roomsSelectionPopup = base.editor.roomsSelectionPopup;
			roomsSelectionPopup.Show();
			roomsSelectionPopup.levelEvent = null;
			roomsSelectionPopup.settingActionEventRooms = true;
			roomsSelectionPopup.UpdateUI();
		}

		public void UpdateGridViewToggle()
		{
			gridViewIcon.sprite = (GridView ? base.editor.gridViewSprite : base.editor.rowViewSprite);
		}

		private void CategoryDictInit()
		{
			Dictionary<Tab, EventCategory[]> dictionary = new Dictionary<Tab, EventCategory[]>();
			dictionary.Add(Tab.Actions, new EventCategory[8]
			{
				EventCategory.All,
				EventCategory.Gameplay,
				EventCategory.Environment,
				EventCategory.RowFX,
				EventCategory.CamFX,
				EventCategory.VisualFX,
				EventCategory.Text,
				EventCategory.Utility
			});
			dictionary.Add(Tab.Rooms, new EventCategory[1]);
			dictionary.Add(Tab.Windows, new EventCategory[1]);
			dictionary.Add(Tab.Song, new EventCategory[4]
			{
				EventCategory.All,
				EventCategory.Song,
				EventCategory.Gameplay,
				EventCategory.Sounds
			});
			dictionary.Add(Tab.Decorations, new EventCategory[3]
			{
				EventCategory.MakeDecoration,
				EventCategory.SpriteDecoration,
				EventCategory.TextDecoration
			});
			categoriesForTab = dictionary;
		}

		private bool ShouldUnlistEvent(Tab tab, LevelEventType type)
		{
			if (!RDEditorConstants.hiddenEvents[tab].Contains(type))
			{
				if (RDEditorConstants.advancedEvents[tab].Contains(type))
				{
					return !RDBase.isAdvanced;
				}
				return false;
			}
			return true;
		}
	}
}
