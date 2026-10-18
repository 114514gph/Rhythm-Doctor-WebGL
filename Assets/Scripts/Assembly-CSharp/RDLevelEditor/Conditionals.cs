using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class Conditionals : RDEditorBase
	{
		private const float MinPanelHeight = 101f;

		private static readonly ImmutableArray<ConditionalType> allConditionalTypes = ImmutableArray.Create<ConditionalType>(ConditionalType.LastHit, ConditionalType.Custom, ConditionalType.TimesExecuted, ConditionalType.Language, ConditionalType.PlayerMode, ConditionalType.Accessibility);

		public RectTransform rectTransform;

		[Header("Conditionals List")]
		public ConditionalInspector[] conditionalsInspectors;

		public GameObject backButton;

		public GameObject listContainer;

		public GameObject conditionalButtonPrefab;

		public RectTransform conditionalButtonsContainer;

		public RectTransform addNewButton;

		public RectTransform detachFromEventButton;

		public RectTransform globalConditionalsContainer;

		public Button clone;

		public Button edit;

		public Button delete;

		private float topPadding;

		public float bottomPadding;

		public float leftPadding;

		public float rightPadding;

		public float height;

		public float spacing;

		private Dictionary<ConditionalType, Sprite> _typeIconsDict;

		public GameObject selectConditional;

		[Header("Popup")]
		public GameObject popupButton;

		public RectTransform popup;

		public InputField duration;

		public Image durationIcon;

		public Color continuousConditionalIconColor;

		public Text durationPreviewText;

		private const float popupAnimDuration = 0.15f;

		[Header("Conditional Shared")]
		public GameObject singleContainer;

		public InputField tagText;

		public InputField descriptionText;

		public Text namePlaceholder;

		public Dropdown typeDropdown;

		public RectTransform deleteButton;

		[Header("Global Tooltip")]
		public GameObject globalTooltipBubble;

		public RectTransform globalTooltipTriangle;

		public Text globalTooltipName;

		public Image globalTooltipIcon;

		public Image globalTooltipNegation;

		private static List<Conditional> globalConditionals = new List<Conditional>();

		public static Dictionary<string, string> globalNegatedDescriptions = new Dictionary<string, string>();

		private List<ConditionalButton> conditionalButtons = new List<ConditionalButton>();

		private bool showingAllConditionals;

		[NonSerialized]
		public int editingId;

		[NonSerialized]
		public InspectorPanel currentPanel;

		[NonSerialized]
		public bool showingListPanel;

		private ImmutableArray<ConditionalType> constantConditionalTypes;

		private ImmutableArray<ConditionalType> conditionalsToShow = allConditionalTypes;

		public static List<string> usedGlobalConditionals = new List<string> { "p", "f", "h", "n", "o" };

		public Dictionary<ConditionalType, Sprite> typeIconsDict
		{
			get
			{
				if (_typeIconsDict == null)
				{
					_typeIconsDict = new Dictionary<ConditionalType, Sprite>();
					ConditionalType[] allValuesFromEnum = RDEditorUtils.GetAllValuesFromEnum<ConditionalType>();
					for (int i = 0; i < allValuesFromEnum.Length; i++)
					{
						ConditionalType key = allValuesFromEnum[i];
						Sprite value = Resources.Load<Sprite>("ConditionalIcons/" + key);
						_typeIconsDict.Add(key, value);
					}
				}
				return _typeIconsDict;
			}
		}

		private bool ShowOnlyConstantConditionals
		{
			get
			{
				if (!showingAllConditionals && currentPanel.currentLevelEvent.info.attribute.constantConditionalsOnly)
				{
					if (currentPanel.currentLevelEvent is LevelEvent_SetRowXs)
					{
						return !RDBase.isDev;
					}
					return true;
				}
				return false;
			}
		}

		private void Awake()
		{
			duration.onValueChanged.AddListener((string duration) =>
			{
				try
				{
					float conditionalDuration = RDEditorUtils.DecodeFloat(duration);
					currentPanel.currentLevelEvent.conditionalDuration = conditionalDuration;
					UpdateDurationButton(conditionalDuration);
				}
				catch
				{
					currentPanel.currentLevelEvent.conditionalDuration = 0f;
				}
			});
			tagText.onValueChanged.AddListener((string index) =>
			{
				OnTagTextChanged();
			});
			descriptionText.onValueChanged.AddListener((string index) =>
			{
				UpdateConditional();
			});
			constantConditionalTypes = allConditionalTypes.Where((ConditionalType type) => GC.conditionalsInfo[type.ToString()].attribute.isConstant).ToImmutableArray();
			typeDropdown.AddOptionsWithEnumArray(conditionalsToShow);
			typeDropdown.onValueChanged.AddListener((int typeIndex) =>
			{
				UpdateType(conditionalsToShow[typeIndex]);
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
			base.editor.globalConditionals = GetGlobalConditionals();
			globalTooltipBubble.SetActive(value: false);
			void OnTagTextChanged()
			{
				tagText.onValueChanged.RemoveAllListeners();
				tagText.text = tagText.text.TrimAllSpaces();
				UpdateConditional();
				tagText.onValueChanged.AddListener((string index) =>
				{
					OnTagTextChanged();
				});
			}
		}

		public static List<Conditional> GetGlobalConditionals()
		{
			if (globalConditionals.Count > 0)
			{
				return globalConditionals;
			}
			Conditional_PlayerMode conditional_PlayerMode = new Conditional_PlayerMode("p");
			AddGlobalConditionalDescription(conditional_PlayerMode, "2p");
			conditional_PlayerMode.playerModeIsTwoPlayer = true;
			globalConditionals.Add(conditional_PlayerMode);
			Conditional_Accessibility conditional_Accessibility = new Conditional_Accessibility("f");
			AddGlobalConditionalDescription(conditional_Accessibility, "flashy");
			conditional_Accessibility.effectType = ConditionalEffectType.Flashy;
			globalConditionals.Add(conditional_Accessibility);
			Conditional_Narration conditional_Narration = new Conditional_Narration("n");
			AddGlobalConditionalDescription(conditional_Narration, "narration");
			conditional_Narration.narrationEnabled = true;
			globalConditionals.Add(conditional_Narration);
			Conditional_TimesExecuted conditional_TimesExecuted = new Conditional_TimesExecuted("o");
			AddGlobalConditionalDescription(conditional_TimesExecuted, "once");
			conditional_TimesExecuted.maxTimes = 1;
			globalConditionals.Add(conditional_TimesExecuted);
			return globalConditionals;
			static void AddGlobalConditionalDescription(Conditional c, string k)
			{
				string text = "editor.GlobalConditionals." + k;
				c.description = RDString.Get(text);
				string withCheck = RDString.GetWithCheck(text + ".negated", out var exists);
				globalNegatedDescriptions[c.gid] = (exists ? withCheck : c.description);
			}
		}

		private void UpdateDurationButton(float duration)
		{
			durationIcon.color = ((duration > 0f) ? continuousConditionalIconColor : Color.white);
			int num = Mathf.RoundToInt(duration);
			durationPreviewText.text = ((num > 99) ? "..." : num.ToString());
			durationPreviewText.color = durationIcon.color;
		}

		private void OnEnable()
		{
			showingAllConditionals = currentPanel == null;
			if (showingAllConditionals)
			{
				addNewButton.sizeDelta = addNewButton.sizeDelta.WithX(160f);
				addNewButton.anchoredPosition = addNewButton.anchoredPosition.WithY(-3f);
				detachFromEventButton.gameObject.SetActive(value: false);
				selectConditional.SetActive(value: false);
				backButton.SetActive(value: false);
				popupButton.SetActive(value: false);
				globalConditionalsContainer.gameObject.SetActive(value: false);
				topPadding = 22f;
			}
			else
			{
				currentPanel.gameObject.SetActive(value: false);
				addNewButton.sizeDelta = addNewButton.sizeDelta.WithX(70f);
				addNewButton.anchoredPosition = addNewButton.anchoredPosition.WithY(-22f);
				selectConditional.SetActive(value: true);
				detachFromEventButton.gameObject.SetActive(value: true);
				backButton.SetActive(value: true);
				popupButton.SetActive(value: true);
				globalConditionalsContainer.gameObject.SetActive(value: true);
				topPadding = 61f;
			}
			ShowListPanel(visible: true);
		}

		private void Update()
		{
			if (conditionalButtonsContainer.gameObject.activeSelf)
			{
				bool key = Input.GetKey(KeyCode.LeftShift);
				clone.gameObject.SetActive(!key);
				edit.gameObject.SetActive(!key);
				delete.gameObject.SetActive(key);
			}
		}

		public void ShowListPanel(bool visible)
		{
			showingListPanel = visible;
			listContainer.SetActive(showingListPanel);
			singleContainer.SetActive(!showingListPanel);
			if (!showingListPanel)
			{
				conditionalButtonsContainer.gameObject.SetActive(value: false);
				base.editor.inspectorTitle.text = RDString.Get("editor.Conditionals.editCondition");
				return;
			}
			base.editor.inspectorTitle.text = RDString.Get("editor.Conditionals");
			if (!showingAllConditionals)
			{
				float conditionalDuration = currentPanel.currentLevelEvent.conditionalDuration;
				duration.text = conditionalDuration.ToString();
				UpdateDurationButton(conditionalDuration);
			}
			float currentXPos = leftPadding;
			float currentYPos = topPadding * -1f;
			RectTransform component = listContainer.GetComponent<RectTransform>();
			float width = component.sizeDelta.x - (leftPadding + rightPadding);
			conditionalButtonsContainer.SetParent(this.rectTransform);
			foreach (ConditionalButton conditionalButton2 in conditionalButtons)
			{
				UnityEngine.Object.Destroy(conditionalButton2.gameObject);
			}
			conditionalButtons = new List<ConditionalButton>();
			foreach (Conditional globalConditional in base.editor.globalConditionals)
			{
				ListConditional(globalConditional, isGlobal: true);
			}
			foreach (Conditional conditional in base.editor.conditionals)
			{
				ListConditional(conditional);
			}
			conditionalButtonsContainer.SetAsLastSibling();
			RectTransform rectTransform = ((conditionalButtons.Count > 0) ? conditionalButtons[conditionalButtons.Count - 1].GetComponent<RectTransform>() : addNewButton);
			float num = (rectTransform.anchoredPosition.y - (rectTransform.sizeDelta.y + bottomPadding)) * -1f;
			UpdateContentHeight(num);
			void ListConditional(Conditional conditional, bool isGlobal = false)
			{
				LevelEvent_Base levelEvent_Base = currentPanel?.currentLevelEvent;
				bool flag = (isGlobal ? (levelEvent_Base?.HasGlobalConditional(conditional.gid).HasValue ?? false) : (levelEvent_Base?.HasConditional(conditional.id).HasValue ?? false));
				bool isConstant = conditional.info.attribute.isConstant;
				if (!ShowOnlyConstantConditionals || (isConstant | flag))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(conditionalButtonPrefab, listContainer.transform);
					RectTransform currentConditionalRect = gameObject.GetComponent<RectTransform>();
					if (!isGlobal)
					{
						currentConditionalRect.anchoredPosition = new Vector2(currentXPos, currentYPos);
						currentYPos -= height + spacing;
						currentConditionalRect.sizeDelta = new Vector2(width, height);
					}
					else
					{
						currentConditionalRect.SetParent(globalConditionalsContainer);
						ConditionalButton component2 = currentConditionalRect.GetComponent<ConditionalButton>();
						RectTransform rectTransform2 = component2.icon.rectTransform;
						component2.conditionalName.gameObject.SetActive(value: false);
						Vector2 vector = (rectTransform2.pivot = new Vector2(0.5f, 0.5f));
						Vector2 anchorMin = (rectTransform2.anchorMax = vector);
						rectTransform2.anchorMin = anchorMin;
						component2.icon.rectTransform.anchoredPosition = Vector2.zero;
					}
					ConditionalButton conditionalButton = gameObject.GetComponent<ConditionalButton>();
					conditionalButtons.Add(conditionalButton);
					conditionalButton.conditionId = conditional.id;
					conditionalButton.conditionGId = conditional.gid;
					conditionalButton.conditionalName.text = conditional.description;
					conditionalButton.conditionalName.font = RDString.GetAppropiateFontForString(conditional.description);
					conditionalButton.icon.sprite = conditional.GetSprite();
					conditionalButton.eventTrigger.onPointerEnter = (PointerEventData data) =>
					{
						if (isGlobal)
						{
							conditionalButtonsContainer.gameObject.SetActive(value: false);
							globalTooltipName.text = (conditionalButton.Negated ? globalNegatedDescriptions[conditional.gid] : conditional.description);
							globalTooltipName.color = (conditionalButton.Negated ? new Color(1f, 0.5f, 0.5f) : (conditionalButton.Selected ? Color.cyan : Color.white));
							globalTooltipIcon.sprite = conditionalButton.icon.sprite;
							globalTooltipNegation.enabled = conditionalButton.Negated;
							globalTooltipTriangle.AnchorPosX(currentConditionalRect.anchoredPosition.x + currentConditionalRect.sizeDelta.x / 2f);
							globalTooltipBubble.SetActive(value: true);
						}
						else
						{
							conditionalButtonsContainer.gameObject.SetActive(value: true);
							conditionalButtonsContainer.SetParent(currentConditionalRect);
							conditionalButtonsContainer.anchoredPosition = conditionalButtonsContainer.anchoredPosition.WithY(0f);
							clone.onClick.RemoveAllListeners();
							clone.onClick.AddListener(() =>
							{
								Clone(conditionalButton.conditionId);
								LevelEditorPlaySound("sndEditorPanelCreate");
							});
							edit.onClick.RemoveAllListeners();
							edit.onClick.AddListener(() =>
							{
								Edit(conditionalButton.conditionId);
								LevelEditorPlaySound("sndButtonClick");
							});
							delete.onClick.RemoveAllListeners();
							delete.onClick.AddListener(() =>
							{
								CheckDelete(conditionalButton.conditionId);
							});
						}
					};
					conditionalButton.eventTrigger.onPointerExit = (PointerEventData data) =>
					{
						conditionalButtonsContainer.gameObject.SetActive(value: false);
						globalTooltipBubble.SetActive(value: false);
					};
					conditionalButton.conditional.enabled = !showingAllConditionals;
					if (!showingAllConditionals)
					{
						bool? flag2 = ((!isGlobal) ? currentPanel.currentLevelEvent.HasConditional(conditional.id) : currentPanel.currentLevelEvent.HasGlobalConditional(conditional.gid));
						conditionalButton.Selected = flag2.HasValue;
						conditionalButton.Negated = flag2 == true;
						conditionalButton.conditional.onClick.AddListener(() =>
						{
							ToggleConditionalButton(conditionalButton);
						});
					}
				}
			}
		}

		public void ShowCurrentPanel()
		{
			LevelEventControl_Base currentEventControl = currentPanel.currentEventControl;
			currentPanel = null;
			currentEventControl.ShowDataOnInspector();
			base.gameObject.SetActive(value: false);
			LevelEditorPlaySound("sndButtonClick", "LevelEditorInspectorPanel");
		}

		public void AddNew()
		{
			editingId = GetUniqueId();
			Conditional conditional = (ShowOnlyConstantConditionals ? ((Conditional)new Conditional_Language(editingId)) : ((Conditional)new Conditional_LastHit(editingId)));
			conditional.tag = editingId.ToString();
			conditional.description = string.Empty;
			base.editor.conditionals.Add(conditional);
			LevelEditorPlaySound("sndEditorPanelCreate", "LevelEditorInspectorPanel");
			if (!showingAllConditionals)
			{
				currentPanel.currentLevelEvent.SetConditional(editingId, null, false);
				currentPanel.currentEventControl.UpdateUIInternal();
			}
			Edit(editingId);
		}

		public void DetachFromEvent()
		{
			foreach (ConditionalButton conditionalButton in conditionalButtons)
			{
				if (conditionalButton.Selected)
				{
					ToggleConditionalButton(conditionalButton, forceDisable: true);
				}
			}
			LevelEditorPlaySound("sndButtonClick", "LevelEditorInspectorPanel");
		}

		private void ToggleConditionalButton(ConditionalButton conditionalButton, bool forceDisable = false)
		{
			if (forceDisable)
			{
				conditionalButton.Selected = false;
				conditionalButton.Negated = false;
			}
			else if (conditionalButton.Selected)
			{
				if (conditionalButton.Negated)
				{
					conditionalButton.Selected = false;
					conditionalButton.Negated = false;
					LevelEditorPlaySound("sndButtonClick", "LevelEditorInspectorPanel");
				}
				else
				{
					conditionalButton.Negated = true;
					LevelEditorPlaySound("sndButtonClick", "LevelEditorInspectorPanel");
				}
			}
			else
			{
				conditionalButton.Selected = true;
				conditionalButton.Negated = false;
				LevelEditorPlaySound("sndButtonClickOn", "LevelEditorInspectorPanel");
			}
			if (conditionalButton.Selected)
			{
				currentPanel.currentLevelEvent.SetConditional(conditionalButton.conditionId, conditionalButton.conditionGId, conditionalButton.Negated);
			}
			else
			{
				currentPanel.currentLevelEvent.SetConditional(conditionalButton.conditionId, conditionalButton.conditionGId, null);
			}
			currentPanel.currentEventControl.UpdateUIInternal();
			if (conditionalButton.transform.parent == globalConditionalsContainer)
			{
				ExecuteEvents.Execute(conditionalButton.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
			}
		}

		public void UpdateConditional()
		{
			int index = base.editor.conditionals.FindIndex((Conditional conditional) => conditional.id == editingId);
			ConditionalType currentType = conditionalsToShow[typeDropdown.value];
			base.editor.conditionals[index] = conditionalsInspectors.Single((ConditionalInspector inspector) => inspector.typeName == currentType).Save(editingId);
			base.editor.conditionals[index].tag = (string.IsNullOrEmpty(tagText.text) ? base.editor.conditionals[index].id.ToString() : tagText.text);
			base.editor.conditionals[index].description = (string.IsNullOrEmpty(descriptionText.text) ? base.editor.conditionals[index].ToString() : descriptionText.text);
			namePlaceholder.text = base.editor.conditionals[index].description;
		}

		public void Clone(int id)
		{
			int num = base.editor.conditionals.FindIndex((Conditional c) => c.id == id);
			Conditional conditional = base.editor.conditionals[num];
			int uniqueId = GetUniqueId();
			Conditional conditional2 = base.editor.conditionals[num].CopyWithId(uniqueId);
			conditional2.description = conditional.description + " " + RDString.Get("editor.Conditionals.clone");
			num++;
			base.editor.conditionals.Insert(num, conditional2);
			ShowListPanel(visible: true);
		}

		public void Edit(int id)
		{
			editingId = id;
			Conditional conditional = base.editor.conditionals.Find((Conditional c) => c.id == editingId);
			ShowListPanel(visible: false);
			tagText.text = conditional.tag;
			descriptionText.text = conditional.description;
			conditionalsToShow = ((ShowOnlyConstantConditionals && constantConditionalTypes.Contains(conditional.type)) ? constantConditionalTypes : allConditionalTypes);
			typeDropdown.ClearOptions();
			typeDropdown.AddOptionsWithEnumArray(conditionalsToShow);
			typeDropdown.value = conditionalsToShow.IndexOf(conditional.type);
			UpdateType(conditional.type);
			conditionalsInspectors.Single((ConditionalInspector inspector) => inspector.typeName == conditional.type).UpdateData(conditional);
		}

		public void EditDelete()
		{
			CheckDelete(editingId);
		}

		public void TogglePopup(bool show)
		{
			Vector3 endValue = (show ? Vector3.one : Vector3.zero);
			Ease ease = (show ? Ease.OutBack : Ease.InBack);
			Vector3 localScale = (show ? Vector3.zero : Vector3.one);
			if (show)
			{
				popup.gameObject.SetActive(value: true);
				base.editor.popupBackground.SetActive(value: true);
				LevelEditorPlaySound("sndEditorPopupSmall", "LevelEditorInspectorPanel");
			}
			if (!show && popup.gameObject.activeSelf)
			{
				LevelEditorPlaySound("sndEditorPopdownSmall", "LevelEditorInspectorPanel");
			}
			popup.DOKill();
			popup.localScale = localScale;
			popup.DOScale(endValue, 0.15f).SetEase(ease).SetUpdate(isIndependentUpdate: true)
				.OnComplete(() =>
				{
					if (!show)
					{
						popup.gameObject.SetActive(value: false);
					}
				});
		}

		private void CheckDelete(int id)
		{
			List<LevelEventControl_Base> eventsWithCondition = new List<LevelEventControl_Base>();
			foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
			{
				if (eventControl.levelEvent.HasConditional(id).HasValue)
				{
					eventsWithCondition.Add(eventControl);
				}
			}
			if (eventsWithCondition.Count == 0)
			{
				Delete(eventsWithCondition, id);
				LevelEditorPlaySound("sndEditorPanelDelete", "LevelEditorInspectorPanel");
				return;
			}
			string empty = string.Empty;
			empty = ((eventsWithCondition.Count != 1) ? RDString.Get("editor.Conditionals.deleteDialog").Replace("[eventsNumber]", eventsWithCondition.Count.ToString()) : RDString.Get("editor.Conditionals.deleteDialogSingle"));
			base.editor.dialog.Show(empty, new string[2]
			{
				RDString.Get("editor.dialog.cancel"),
				RDString.Get("editor.Conditionals.delete")
			}, (int option) =>
			{
				base.editor.dialog.Hide(() =>
				{
					if (option == 1)
					{
						Delete(eventsWithCondition, id);
						LevelEditorPlaySound("sndEditorPanelDelete", "LevelEditorInspectorPanel");
					}
				});
			});
		}

		private void Delete(List<LevelEventControl_Base> eventsWithCondition, int id)
		{
			base.editor.conditionals.RemoveAll((Conditional c) => c.id == id);
			foreach (LevelEventControl_Base item in eventsWithCondition)
			{
				item.levelEvent.SetConditional(id, null, null);
				item.UpdateUIInternal();
			}
			conditionalButtonsContainer.gameObject.SetActive(value: false);
			ShowListPanel(visible: true);
		}

		private int GetUniqueId()
		{
			int uniqueId = 0;
			bool flag = false;
			do
			{
				uniqueId++;
			}
			while (base.editor.conditionals.Find((Conditional c) => c.id == uniqueId) != null);
			return uniqueId;
		}

		private void UpdateType(ConditionalType currentType)
		{
			ConditionalInspector[] array = conditionalsInspectors;
			foreach (ConditionalInspector conditionalInspector in array)
			{
				if (conditionalInspector.typeName == currentType)
				{
					conditionalInspector.container.SetActive(value: true);
					float num = conditionalInspector.rectTransform.anchoredPosition.y - (conditionalInspector.rectTransform.sizeDelta.y + bottomPadding);
					deleteButton.anchoredPosition = deleteButton.anchoredPosition.WithY(num);
					float num2 = (num - (deleteButton.sizeDelta.y + bottomPadding)) * -1f;
					UpdateContentHeight(num2);
					UpdateConditional();
					Conditional conditional = base.editor.conditionals.Find((Conditional c) => c.id == editingId);
					namePlaceholder.text = conditional.ToString();
				}
				else
				{
					conditionalInspector.container.SetActive(value: false);
				}
			}
		}

		private void UpdateContentHeight(float height)
		{
			height = Mathf.Max(height, 101f);
			rectTransform.sizeDelta = rectTransform.sizeDelta.WithY(height);
			base.editor.inspectorPanelManager.UpdateContentHeight(rectTransform);
		}
	}
}
