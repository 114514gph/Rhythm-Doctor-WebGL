using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel : RDEditorBase
	{
		public delegate void ChangeAction(Action extraActions, string sound = null, string group = null);

		protected const string sndSliderRelease = "sndSliderRelease";

		[NonSerialized]
		public BarBeatPosition position;

		public Transform extraParent;

		[NonSerialized]
		public bool auto;

		public LevelEventInfo levelEventInfo;

		private string panelName;

		private Tab tab;

		private PropertyControl_RowLegacy row;

		protected bool localize = true;

		protected bool playSoundOnEdit;

		private readonly List<RDColorHoverEventTrigger> colorHovers = new List<RDColorHoverEventTrigger>();

		private readonly List<Toggle> toggles = new List<Toggle>();

		[NonSerialized]
		public ScrollRect scrollRect;

		[NonSerialized]
		public LevelEventControl_Base currentEventControl;

		[NonSerialized]
		public LevelEvent_Base currentLevelEvent;

		[NonSerialized]
		public bool isUpdatingUI;

		[NonSerialized]
		public string defaultGroup;

		[NonSerialized]
		public VerticalLayoutGroup propertiesContainer;

		[NonSerialized]
		public ImmutableList<Property> properties;

		private const string EditorKey = "editor.";

		public bool theresRows
		{
			get
			{
				if (base.editor.eventControls_rows.Count == 0)
				{
					return levelEventInfo.attribute.defaultRow == -1;
				}
				return true;
			}
		}

		protected RectTransform content => base.editor.inspectorPanelManager.content;

		protected Transform propertiesParent
		{
			get
			{
				if (!(scrollRect != null))
				{
					return base.transform;
				}
				return scrollRect.content;
			}
		}

		public void Awake()
		{
			panelName = base.name;
			if (auto)
			{
				AwakeAuto();
			}
			else
			{
				propertiesParent.Find("position")?.TryGetComponent<BarBeatPosition>(out position);
			}
			if (position != null)
			{
				position.gameObject.SetActive(value: true);
				position.Setup(this);
			}
			defaultGroup = "LevelEditorInspectorPanel";
		}

		private void AwakeAuto()
		{
			position = UnityEngine.Object.Instantiate(base.editor.inspectorPanelManager.positionPrefab, base.transform).GetComponent<BarBeatPosition>();
			propertiesContainer = UnityEngine.Object.Instantiate(base.gc.propertiesList, base.transform).GetComponent<VerticalLayoutGroup>();
			string text = GetType().Name.Replace("InspectorPanel_", "");
			LevelEventType levelEventType = ((text != "Base") ? RDUtils.ParseEnum(text, LevelEventType.None) : LevelEventType.None);
			if (levelEventType == LevelEventType.None)
			{
				return;
			}
			if (!GC.levelEventsInfo.ContainsKey(levelEventType))
			{
				throw new Exception(base.name + " is missing a LevelEventInfoAttribute. Maybe make this more descriptive later");
			}
			levelEventInfo = GC.levelEventsInfo[levelEventType];
			if (levelEventInfo.showsRowControl)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(base.gc.rowPrefab, propertiesContainer.transform, worldPositionStays: false);
				gameObject.name = "row";
				row = gameObject.GetComponent<PropertyControl_RowLegacy>();
				row.dropdown.onValueChanged.AddListener((int _) =>
				{
					RegisterForChangeAction(null, "sndListClick");
				});
				row.noRowsDescription = gameObject.transform.Find("noRowsDescription").GetComponent<Text>();
			}
			properties = (from info in levelEventInfo.propertiesInfo
				where info.controlAttribute.GetType() != typeof(DontShowAttribute)
				select Property.Create(info, this)).ToImmutableList();
		}

		private void Start()
		{
			Localize();
		}

		public void Show(LevelEventControl_Base levelEventControl)
		{
			LevelEvent_Base levelEvent = levelEventControl.levelEvent;
			position.beatLabel.gameObject.SetActive(levelEvent.usesBeat);
			position.roomsUsage = levelEvent.roomsUsage;
			Transform child = base.transform.parent.GetChild(base.transform.parent.childCount - 1);
			Transform transform = base.transform;
			if (child != transform)
			{
				RDInspectorPanelManager inspectorPanelManager = base.editor.inspectorPanelManager;
				inspectorPanelManager.UpdateContentHeight(GetComponent<RectTransform>());
				inspectorPanelManager.HideAll();
				TabSection tabSection = levelEventControl.tabSection;
				inspectorPanelManager.emptyInspectorLabel.gameObject.SetActive(value: false);
				base.editor.inspectorTitle.color = tabSection.color;
				base.editor.inspectorTitle.text = RDString.Get("editor." + levelEvent.type);
				SetToggleAndHoverColor(tabSection.color);
				base.gameObject.transform.SetAsLastSibling();
				base.gameObject.SetActive(value: true);
			}
			else if (currentLevelEvent != null && currentLevelEvent.type == levelEvent.type && levelEvent.tab != tab)
			{
				tab = levelEvent.tab;
				TabSection tabSection2 = levelEventControl.tabSection;
				base.editor.inspectorTitle.color = tabSection2.color;
				SetToggleAndHoverColor(tabSection2.color);
			}
			currentLevelEvent = levelEvent;
			currentEventControl = levelEventControl;
			UpdateUI(levelEvent);
			if (levelEvent is ICompatibleDecorationTypeChecker { isCompatibleDecorationType: false })
			{
				base.editor.inspectorPanelManager.ShowIncompatiblePanel(levelEvent);
			}
			else if (base.editor.conditionalsPanel.currentPanel != null && base.editor.conditionalsPanel.showingListPanel)
			{
				if (position.conditionsButton.gameObject.activeSelf)
				{
					base.editor.ShowPanelConditionals(this);
				}
				else
				{
					base.editor.conditionalsPanel.ShowListPanel(visible: false);
				}
			}
		}

		public static void ShowPanel(Transform panel, string title, Tab tab)
		{
			scnEditor instance = scnEditor.instance;
			instance.DeselectAllEventControls(updateInspectorUI: false);
			instance.inspectorTitle.text = title;
			Color color = instance.tabSections[(int)tab].color;
			instance.inspectorTitle.color = color;
			panel.GetComponent<InspectorPanel>()?.SetToggleAndHoverColor(color);
			instance.inspectorPanelManager.HideAll();
			panel.transform.SetAsLastSibling();
			panel.gameObject.SetActive(value: true);
			instance.inspectorPanelManager.UpdateContentHeight(panel.GetComponent<RectTransform>());
		}

		public void Save(LevelEvent_Base levelEvent, bool isNew)
		{
			if (!isNew && position != null)
			{
				levelEvent.barAndBeat = position.barAndBeat;
				levelEvent.rooms = position.rooms;
				levelEvent.tag = position.eventTag;
				levelEvent.tagRunNormally = position.tagRunNormally;
			}
			SaveInternal(levelEvent);
			if (auto && base.gameObject.activeSelf)
			{
				StartCoroutine(UpdateLevelEventHeight(scrollToTop: false));
			}
		}

		protected virtual void SaveInternal(LevelEvent_Base levelEvent)
		{
			if (auto)
			{
				SaveAuto(levelEvent);
			}
			else
			{
				RDEditorUtils.NotImplemented();
			}
		}

		public void UpdateUI(LevelEvent_Base levelEvent)
		{
			if (!isUpdatingUI && base.gameObject.activeSelf)
			{
				if (position != null)
				{
					position.levelEvent = levelEvent;
					position.rooms = levelEvent.rooms;
					position.barAndBeat = levelEvent.barAndBeat;
					position.eventTag = levelEvent.tag;
					position.tagRunNormally = levelEvent.tagRunNormally;
					position.UpdateConditionalsButton();
				}
				isUpdatingUI = true;
				Localize();
				UpdateUIInternal(levelEvent);
				isUpdatingUI = false;
				if (auto)
				{
					StartCoroutine(UpdateLevelEventHeight(scrollToTop: true));
				}
			}
		}

		private IEnumerator UpdateLevelEventHeight(bool scrollToTop)
		{
			yield return new WaitForEndOfFrame();
			base.ipm.UpdateLevelEventHeight(propertiesContainer.GetComponent<RectTransform>().sizeDelta.y, scrollToTop);
		}

		public virtual void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			if (auto)
			{
				UpdateUIAuto(levelEvent);
			}
			else
			{
				RDEditorUtils.NotImplemented();
			}
		}

		public void AddOnEditListenersNoSave(Action extraActions, params object[] objects)
		{
			AddOnEditListenersGeneric(RegisterForChangeActionNoSave, extraActions, objects);
		}

		public void AddOnEditListenersSaveLast(Action extraActions, params object[] objects)
		{
			AddOnEditListenersGeneric(RegisterForChangeActionSaveLast, extraActions, objects);
		}

		public void AddOnEditListeners(Action extraActions, params object[] objects)
		{
			AddOnEditListenersGeneric(RegisterForChangeAction, extraActions, objects);
		}

		public void AddOnEditListenersGeneric(ChangeAction action, Action extraActions, object[] objects)
		{
			foreach (object obj in objects)
			{
				if (obj == null)
				{
					MonoBehaviour.print("theres a null object on inspector panel " + base.gameObject.name);
				}
				else if (obj is InputField inputField)
				{
					inputField.onEndEdit.AddListener((string _) =>
					{
						action(extraActions, "sndEditorValueChange");
					});
				}
				else if (obj is ColorPicker colorPicker)
				{
					colorPicker.onEndEdit = (ColorPicker.EndHandler)Delegate.Combine(colorPicker.onEndEdit, (ColorPicker.EndHandler)((string _) =>
					{
						action(extraActions, "sndEditorValueChange");
					}));
				}
				else if (obj is PositionPicker positionPicker)
				{
					positionPicker.x.onEndEdit.AddListener((string _) =>
					{
						action(extraActions, "sndEditorValueChange");
					});
					positionPicker.y.onEndEdit.AddListener((string _) =>
					{
						action(extraActions, "sndEditorValueChange");
					});
				}
				else if (obj is ExpPositionPicker expPositionPicker)
				{
					expPositionPicker.x.onEndEdit.AddListener((string _) =>
					{
						action(extraActions, "sndEditorValueChange");
					});
					expPositionPicker.y.onEndEdit.AddListener((string _) =>
					{
						action(extraActions, "sndEditorValueChange");
					});
				}
				else if (obj is ToggleGroup toggleGroup)
				{
					foreach (Transform item in toggleGroup.transform)
					{
						if (item.TryGetComponent<Toggle>(out var component))
						{
							RegisterToggle(component, action, extraActions, "sndButtonClick");
						}
					}
				}
				else if (obj is Dropdown dropdown)
				{
					dropdown.onValueChanged.AddListener((int _) =>
					{
						action(extraActions, "sndListClick");
					});
				}
				else if (obj is Toggle toggle)
				{
					toggle.onValueChanged.AddListener((bool _) =>
					{
						action(extraActions, "sndButtonRadio");
					});
				}
				else if (obj is Slider slider)
				{
					RDEventTrigger eventTrigger = slider.GetOrAddComponent<RDEventTrigger>();
					RDEventTrigger rDEventTrigger = eventTrigger;
					rDEventTrigger.onEndDrag = (RDPointerEventData)Delegate.Combine(rDEventTrigger.onEndDrag, (RDPointerEventData)((PointerEventData _) =>
					{
						if (Time.frameCount != eventTrigger.lastEventFrame)
						{
							action(extraActions);
						}
					}));
					RDEventTrigger rDEventTrigger2 = eventTrigger;
					rDEventTrigger2.onClick = (RDPointerEventData)Delegate.Combine(rDEventTrigger2.onClick, (RDPointerEventData)((PointerEventData _) =>
					{
						if (Time.frameCount != eventTrigger.lastEventFrame)
						{
							action(extraActions, "sndSliderGrab");
						}
					}));
				}
				else if (obj is Button button)
				{
					button.onClick.AddListener(() =>
					{
						action(extraActions, "sndButtonClick");
					});
				}
				else if (obj is CharacterPicker characterPicker)
				{
					characterPicker.onEndEdit = (CharacterPicker.EndHandler)Delegate.Combine(characterPicker.onEndEdit, (CharacterPicker.EndHandler)((Character _, string _, string _) =>
					{
						action(extraActions, "sndButtonClick");
					}));
				}
				else if (!(obj is LetterMovementPicker))
				{
					MonoBehaviour.print($"{obj} is not supported yet.");
				}
			}
		}

		public virtual void PreExtraActions()
		{
		}

		public void RegisterForChangeAction(Action extraActions, string sound = null, string group = null)
		{
			if (base.gameObject.activeSelf && currentLevelEvent != null && !currentLevelEvent.inspectorPanel.isUpdatingUI)
			{
				PreExtraActions();
				if (sound != null)
				{
					LevelEditorPlaySound(sound, group ?? defaultGroup);
				}
				if (currentEventControl != null)
				{
					currentEventControl.SaveAndUpdateUI();
				}
				else
				{
					currentLevelEvent.SaveData();
				}
				if (extraActions != null)
				{
					playSoundOnEdit = sound != "sndSliderRelease";
					extraActions();
				}
			}
		}

		private void RegisterForChangeActionSaveLast(Action extraActions, string sound = null, string group = null)
		{
			if (!currentLevelEvent.inspectorPanel.isUpdatingUI)
			{
				PreExtraActions();
				extraActions?.Invoke();
				if (currentEventControl != null)
				{
					currentEventControl.SaveAndUpdateUI();
				}
				else
				{
					currentLevelEvent.SaveData();
				}
			}
		}

		private void RegisterForChangeActionNoSave(Action extraActions, string sound = null, string group = null)
		{
			extraActions?.Invoke();
		}

		public static void RegisterToggle(Toggle toggle, ChangeAction action, Action extraActions = null, string sound = null, string group = null)
		{
			ClickEvent orAddComponent = toggle.gameObject.GetOrAddComponent<ClickEvent>();
			orAddComponent.PointerClick = new UnityEvent();
			orAddComponent.PointerClick.AddListener(() =>
			{
				action(extraActions, sound, group);
			});
		}

		public void AddLocalizationToInputFields(params InputField[] inputFields)
		{
			foreach (InputField inputField in inputFields)
			{
				inputField.onValueChanged.AddListener((string s) =>
				{
					RDEditorUtils.UpdateUIText(inputField.textComponent, s);
				});
			}
		}

		public void Localize()
		{
			if (auto || !localize)
			{
				return;
			}
			localize = false;
			if (this is InspectorPanel_MakeRow || this is InspectorPanel_CharacterTemplateGenerator)
			{
				tab = Tab.Rows;
			}
			else if (this is MakeDecorationInspectorPanel)
			{
				tab = Tab.Decorations;
			}
			else if (this is InspectorPanel_LevelSettings)
			{
				tab = Tab.Song;
			}
			else
			{
				string value = GetType().ToString().Replace("RDLevelEditor.InspectorPanel_", "");
				LevelEventType levelEventType = Enum.Parse<LevelEventType>(value);
				tab = RDEditorConstants.levelEventTabs.Keys.First((Tab t) => RDEditorConstants.levelEventTabs[t].Contains(levelEventType));
			}
			Localize(propertiesParent);
			if ((bool)extraParent)
			{
				Localize(extraParent);
			}
		}

		public void Localize(Transform t)
		{
			foreach (Transform item in t)
			{
				string key = item.gameObject.name;
				Text component = item.GetComponent<Text>();
				if (component != null)
				{
					string localizedString = GetLocalizedString(key);
					RDStringToUIText.Apply(component);
					component.text = localizedString;
					RDColorHoverEventTrigger component2 = component.GetComponent<RDColorHoverEventTrigger>();
					if (component2 != null)
					{
						component2.enterColor = base.gc.colorPalette[(int)tab];
						colorHovers.Add(component2);
					}
				}
				foreach (Transform item2 in item)
				{
					if (item2.GetComponent<ToggleGroup>() != null)
					{
						foreach (Transform item3 in item2)
						{
							string key2 = item3.gameObject.name;
							Transform transform3 = item3.Find("text");
							if (transform3 != null && !transform3.TryGetComponent<RDStringToUIText>(out var _))
							{
								Text component4 = transform3.GetComponent<Text>();
								component4.text = GetLocalizedString(key2);
								RDStringToUIText.Apply(component4);
							}
							if (!item2.gameObject.name.Contains("noColor"))
							{
								bool num = this is InspectorPanel_AddFreeTimeBeat;
								bool flag = this is InspectorPanel_PulseFreeTimeBeat;
								Toggle component5 = item3.GetComponent<Toggle>();
								if (!num && !flag && component5 != null)
								{
									component5.graphic.color = base.gc.colorPalette[(int)tab];
									toggles.Add(component5);
								}
							}
						}
					}
					Dropdown component6 = item2.GetComponent<Dropdown>();
					if (component6 != null)
					{
						LocalizeDropdown(component6);
					}
				}
			}
		}

		private void LocalizeSingleProperty(Transform property)
		{
			string propertyName = property.gameObject.name;
			Text component = property.GetComponent<Text>();
			if (component != null)
			{
				LocalizeLabel(component, propertyName);
			}
			foreach (Transform item in property)
			{
				ToggleGroup component2 = item.GetComponent<ToggleGroup>();
				if (component2 != null)
				{
					LocalizeToggleGroup(item, component2);
				}
				Dropdown component3 = item.GetComponent<Dropdown>();
				if (component3 != null)
				{
					LocalizeDropdown(component3);
				}
			}
		}

		public void LocalizeLabel(Text label, string propertyName, bool fromKey = false)
		{
			string text = ((!fromKey) ? GetLocalizedString(propertyName) : RDString.Get(propertyName));
			if (text.EndsWith(":") || text.EndsWith("："))
			{
				text = text.Substring(0, text.Length - 1);
			}
			RDStringToUIText.Apply(label);
			label.text = text;
			RDColorHoverEventTrigger component = label.GetComponent<RDColorHoverEventTrigger>();
			if (component != null)
			{
				component.enterColor = base.gc.colorPalette[(int)tab];
				colorHovers.Add(component);
			}
		}

		public void LocalizeToggleGroup(Transform toggleGroupTransform, ToggleGroup toggleGroup)
		{
			foreach (Transform item in toggleGroupTransform)
			{
				string key = item.gameObject.name;
				Transform transform2 = item.Find("text");
				if (transform2 != null)
				{
					Text component = transform2.GetComponent<Text>();
					component.text = GetLocalizedString(key);
					RDStringToUIText.Apply(component);
				}
				if (!toggleGroupTransform.gameObject.name.Contains("noColor"))
				{
					bool num = this is InspectorPanel_AddFreeTimeBeat;
					bool flag = this is InspectorPanel_PulseFreeTimeBeat;
					Toggle component2 = item.GetComponent<Toggle>();
					if (!num && !flag && component2 != null)
					{
						component2.graphic.color = base.gc.colorPalette[(int)tab];
						toggles.Add(component2);
					}
				}
			}
		}

		private void SetupToggles(Transform toggleGroupTransform)
		{
			foreach (Transform item in toggleGroupTransform)
			{
				if (!toggleGroupTransform.gameObject.name.Contains("noColor"))
				{
					bool num = this is InspectorPanel_AddFreeTimeBeat;
					bool flag = this is InspectorPanel_PulseFreeTimeBeat;
					Toggle component = item.GetComponent<Toggle>();
					if (!num && !flag && component != null)
					{
						component.graphic.color = base.gc.colorPalette[(int)tab];
						toggles.Add(component);
					}
				}
			}
		}

		public void SetupPropertyControl(Transform property)
		{
			foreach (Transform item in property)
			{
				if (item.GetComponent<ToggleGroup>() != null)
				{
					SetupToggles(item);
				}
			}
		}

		public void LocalizeDropdown(Dropdown dropdown)
		{
			RDStringToUIText.Apply(dropdown.template.GetChild(0).GetChild(0).GetChild(0)
				.GetChild(2)
				.GetComponent<Text>());
			RDStringToUIText.Apply(dropdown.captionText);
		}

		public string GetLocalizedString(string key)
		{
			string text = "editor." + panelName + "." + key;
			string withCheck = RDString.GetWithCheck(text, out var exists);
			if (exists)
			{
				return withCheck;
			}
			string text2 = "editor." + key;
			withCheck = RDString.GetWithCheck(text2, out exists);
			if (exists)
			{
				return withCheck;
			}
			RDBaseDllDummy.printesw("localizedStringKey " + text + " or " + text2 + " doesn't exist");
			return "no key: " + text;
		}

		public void UpdateUIAuto(LevelEvent_Base levelEvent)
		{
			if (levelEventInfo.showsRowControl)
			{
				row.dropdownContainer.SetActive(theresRows);
				row.noRowsDescription.gameObject.SetActive(!theresRows);
				row.UpdateHeight();
				if (theresRows)
				{
					row.dropdown.ClearOptions();
					if (levelEventInfo.attribute.defaultRow == -1)
					{
						row.dropdown.AddOptions(new List<string> { RDString.Get("editor.TintRows.rows.allRows") });
					}
					row.dropdown.AddOptionsWithRowsList();
					if (levelEventInfo.attribute.defaultRow == -1)
					{
						if (levelEvent.row == -1)
						{
							row.dropdown.value = 0;
						}
						else if (levelEvent.row < base.editor.rowsData.Count)
						{
							row.dropdown.value = levelEvent.row + 1;
						}
						else
						{
							row.dropdown.value = 0;
						}
					}
					else
					{
						row.dropdown.value = Mathf.Clamp(levelEvent.row, 0, row.dropdown.options.Count - 1);
					}
				}
			}
			UpdateUIProperties(levelEvent);
		}

		public void SaveAuto(LevelEvent_Base levelEvent)
		{
			if (levelEventInfo.showsRowControl)
			{
				levelEvent.row = row.dropdown.value + levelEventInfo.attribute.defaultRow;
			}
			SaveProperties(levelEvent);
		}

		public virtual void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			foreach (Property property in properties)
			{
				property.UpdateUI(levelEvent);
			}
		}

		public virtual void SaveProperties(LevelEvent_Base levelEvent)
		{
			foreach (Property property in properties)
			{
				property.Save(levelEvent);
			}
		}

		public int GetNewValue(int current, InputField text, Slider slider)
		{
			int num = Convert.ToInt32(text.text);
			int result = Mathf.RoundToInt(slider.value);
			if (current != num)
			{
				slider.value = num;
				return num;
			}
			text.text = result.ToString();
			return result;
		}

		private void SetToggleAndHoverColor(Color color)
		{
			foreach (Toggle toggle in toggles)
			{
				toggle.graphic.color = color;
			}
			foreach (RDColorHoverEventTrigger colorHover in colorHovers)
			{
				colorHover.enterColor = color;
			}
		}
	}
}
