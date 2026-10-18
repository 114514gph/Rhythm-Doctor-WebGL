using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class RDInspectorPanelManager : RDEditorBase
	{
		public GameObject[] eventListPanels;

		public RectTransform emptyPanel;

		public RectTransform incompatiblePanel;

		public RectTransform bulkPanel;

		public RectTransform viewport;

		public Image background;

		public Text emptyInspectorLabel;

		public Text incompatibleInspectorLabel;

		public ScrollRect scrollRect;

		public Scrollbar scrollBar;

		public Button expandInspectorButton;

		public Text gridViewLabel;

		public GameObject emptyInspectorPanel;

		public GameObject positionPrefab;

		private Dictionary<Type, InspectorPanel> inspectorPanels;

		public RectTransform content;

		private bool didSetup;

		private string emptyPanelText = "";

		private void Start()
		{
			Setup();
		}

		private void Setup()
		{
			if (didSetup)
			{
				return;
			}
			didSetup = true;
			inspectorPanels = new Dictionary<Type, InspectorPanel>();
			InspectorPanel[] componentsInChildren = content.GetComponentsInChildren<InspectorPanel>(includeInactive: true);
			List<Type> manualInspectorTypes = new List<Type>();
			InspectorPanel[] array = componentsInChildren;
			foreach (InspectorPanel inspectorPanel in array)
			{
				Type type = inspectorPanel.GetType();
				if (inspectorPanels.ContainsKey(type))
				{
					MonoBehaviour.print("key: " + type?.ToString() + " already exists");
				}
				else
				{
					inspectorPanels.Add(type, inspectorPanel);
				}
				inspectorPanel.gameObject.SetActive(value: true);
				inspectorPanel.gameObject.SetActive(value: false);
				manualInspectorTypes.Add(inspectorPanel.GetType());
			}
			IEnumerable<Type> enumerable = from type2 in typeof(InspectorPanel).Assembly.GetTypes()
				where type2.IsSubclassOf(typeof(InspectorPanel)) && !manualInspectorTypes.Contains(type2)
				select type2;
			List<Type> list = new List<Type>(manualInspectorTypes);
			list.AddRange(enumerable);
			foreach (Type item in enumerable)
			{
				bool flag = false;
				foreach (Type item2 in list)
				{
					if (item2.IsSubclassOf(item))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					InspectorPanel value = CreatePanel(item);
					inspectorPanels.Add(item, value);
				}
			}
			RDStringToUIText.Apply(emptyInspectorLabel);
			InspectorPanel CreatePanel(Type panelType)
			{
				string text = panelType.ToString().Replace("RDLevelEditor.InspectorPanel_", "");
				GameObject gameObject = UnityEngine.Object.Instantiate(emptyInspectorPanel, content.transform);
				gameObject.name = text;
				gameObject.SetActive(value: false);
				InspectorPanel obj = gameObject.AddComponent(panelType) as InspectorPanel;
				if (!gameObject.TryGetComponent<RectTransform>(out var _))
				{
					gameObject.AddComponent<RectTransform>();
				}
				obj.auto = true;
				gameObject.SetActive(value: true);
				return obj;
			}
		}

		public InspectorPanel Get(Type type)
		{
			return inspectorPanels[type];
		}

		public T Get<T>() where T : InspectorPanel
		{
			return (T)inspectorPanels[typeof(T)];
		}

		public InspectorPanel GetCurrent()
		{
			foreach (KeyValuePair<Type, InspectorPanel> inspectorPanel in inspectorPanels)
			{
				if (inspectorPanel.Value.gameObject.activeInHierarchy)
				{
					return inspectorPanel.Value;
				}
			}
			return null;
		}

		public void HideAll()
		{
			Setup();
			foreach (KeyValuePair<Type, InspectorPanel> inspectorPanel in inspectorPanels)
			{
				inspectorPanel.Value.gameObject.SetActive(value: false);
			}
			GameObject[] array = eventListPanels;
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null)
				{
					gameObject.gameObject.SetActive(value: false);
				}
			}
			emptyPanel.gameObject.SetActive(value: false);
			incompatiblePanel.gameObject.SetActive(value: false);
			bulkPanel.gameObject.SetActive(value: false);
			base.editor.conditionalsPanel.gameObject.SetActive(value: false);
			background.color = RDConstants.data.colorPalette[9];
		}

		public void UpdateContentHeight(RectTransform panel)
		{
			float y = panel.sizeDelta.y;
			content.sizeDelta = content.sizeDelta.WithY(y);
			panel.AnchorPosY(0f);
			content.AnchorPosY(0f);
		}

		public void UpdateLevelEventHeight(float height, bool scrollToTop)
		{
			content.sizeDelta = content.sizeDelta.WithY(height + 20f);
			if (scrollToTop)
			{
				content.AnchorPosY(0f);
			}
		}

		public void ShowBlankPanel(string text = "")
		{
			HideAll();
			emptyPanelText = text;
			emptyPanel.gameObject.SetActive(value: true);
			emptyPanel.SetAsLastSibling();
			emptyInspectorLabel.gameObject.SetActive(value: true);
			text += GetEmptyPanelStrings();
			emptyInspectorLabel.text = text;
			UpdateContentHeight(emptyPanel);
			base.editor.inspectorTitle.text = Path.GetFileName(base.editor.openedFilePath);
		}

		public void UpdateBlankPanel(bool onlyIfVisible = false)
		{
			if (!onlyIfVisible || emptyPanel.gameObject.activeSelf)
			{
				string text = emptyPanelText;
				text += GetEmptyPanelStrings();
				emptyInspectorLabel.text = text;
			}
		}

		public void ShowIncompatiblePanel(LevelEvent_Base levelEvent)
		{
			HideAll();
			incompatiblePanel.gameObject.SetActive(value: true);
			incompatiblePanel.SetAsLastSibling();
			UpdateContentHeight(incompatiblePanel);
			incompatibleInspectorLabel.text = RDString.Get("editor.incompatibleEvent");
			base.editor.inspectorTitle.text = RDString.Get("editor." + levelEvent.type);
		}

		public void ShowBulkPanel()
		{
			HideAll();
			bulkPanel.GetComponent<BulkSelectPanel>().Initialize();
			bulkPanel.gameObject.SetActive(value: true);
			UpdateContentHeight(bulkPanel);
			base.editor.inspectorTitle.text = RDString.Get("editor.eventCount").Replace("[amount]", base.editor.selectedControls.Count.ToString());
		}

		public string GetMistakesString()
		{
			if (base.game == null || base.game.mistakesManager.mistakes == 0f)
			{
				return "";
			}
			string text = Math.Round(base.game.mistakesManager.mistakes, 4).ToString();
			if (GC.twoPlayerMode)
			{
				text = $"{Math.Round(base.game.mistakesManager.mistakesP1, 4)} + {Math.Round(base.game.mistakesManager.mistakesP2, 4)} = {text}";
			}
			return RDString.Get("rankscreen.mistakes").Replace("[mistakes]", text);
		}

		public string GetRDCodeVariables()
		{
			if (base.game == null)
			{
				return "";
			}
			LevelBase currentLevel = base.game.currentLevel;
			string text = "";
			bool flag = !base.game.currentLevel.booleansDefaultToTrue;
			if (currentLevel.b0 == flag)
			{
				text += $"b0: {currentLevel.b0}\n";
			}
			if (currentLevel.b1 == flag)
			{
				text += $"b1: {currentLevel.b1}\n";
			}
			if (currentLevel.b2 == flag)
			{
				text += $"b2: {currentLevel.b2}\n";
			}
			if (currentLevel.b3 == flag)
			{
				text += $"b3: {currentLevel.b3}\n";
			}
			if (currentLevel.b4 == flag)
			{
				text += $"b4: {currentLevel.b4}\n";
			}
			if (currentLevel.b5 == flag)
			{
				text += $"b5: {currentLevel.b5}\n";
			}
			if (currentLevel.b6 == flag)
			{
				text += $"b6: {currentLevel.b6}\n";
			}
			if (currentLevel.b7 == flag)
			{
				text += $"b7: {currentLevel.b7}\n";
			}
			if (currentLevel.b8 == flag)
			{
				text += $"b8: {currentLevel.b8}\n";
			}
			if (currentLevel.b9 == flag)
			{
				text += $"b9: {currentLevel.b9}\n";
			}
			if (currentLevel.i0 != 0)
			{
				text += $"i0: {currentLevel.i0}\n";
			}
			if (currentLevel.i1 != 0)
			{
				text += $"i1: {currentLevel.i1}\n";
			}
			if (currentLevel.i2 != 0)
			{
				text += $"i2: {currentLevel.i2}\n";
			}
			if (currentLevel.i3 != 0)
			{
				text += $"i3: {currentLevel.i3}\n";
			}
			if (currentLevel.i4 != 0)
			{
				text += $"i4: {currentLevel.i4}\n";
			}
			if (currentLevel.i5 != 0)
			{
				text += $"i5: {currentLevel.i5}\n";
			}
			if (currentLevel.i6 != 0)
			{
				text += $"i6: {currentLevel.i6}\n";
			}
			if (currentLevel.i7 != 0)
			{
				text += $"i7: {currentLevel.i7}\n";
			}
			if (currentLevel.i8 != 0)
			{
				text += $"i8: {currentLevel.i8}\n";
			}
			if (currentLevel.i9 != 0)
			{
				text += $"i9: {currentLevel.i9}\n";
			}
			if (currentLevel.f0 != 0f)
			{
				text += $"f0: {currentLevel.f0}\n";
			}
			if (currentLevel.f1 != 0f)
			{
				text += $"f1: {currentLevel.f1}\n";
			}
			if (currentLevel.f2 != 0f)
			{
				text += $"f2: {currentLevel.f2}\n";
			}
			if (currentLevel.f3 != 0f)
			{
				text += $"f3: {currentLevel.f3}\n";
			}
			if (currentLevel.f4 != 0f)
			{
				text += $"f4: {currentLevel.f4}\n";
			}
			if (currentLevel.f5 != 0f)
			{
				text += $"f5: {currentLevel.f5}\n";
			}
			if (currentLevel.f6 != 0f)
			{
				text += $"f6: {currentLevel.f6}\n";
			}
			if (currentLevel.f7 != 0f)
			{
				text += $"f7: {currentLevel.f7}\n";
			}
			if (currentLevel.f8 != 0f)
			{
				text += $"f8: {currentLevel.f8}\n";
			}
			if (currentLevel.f9 != 0f)
			{
				text += $"f9: {currentLevel.f9}\n";
			}
			return text;
		}

		public string GetEmptyPanelStrings()
		{
			string mistakesString = GetMistakesString();
			return "\n\n" + mistakesString + ((mistakesString == "") ? "" : "\n\n") + GetRDCodeVariables();
		}

		public bool IsSelectEventPanelVisible()
		{
			Transform obj = content.transform;
			Transform child = obj.GetChild(obj.childCount - 1);
			SelectLevelEventPanel component;
			if (child.gameObject.activeSelf)
			{
				return child.TryGetComponent<SelectLevelEventPanel>(out component);
			}
			return false;
		}

		private void Update()
		{
			bool active = base.editor.leftPanelIsExpanded || scrollBar.gameObject.activeSelf;
			expandInspectorButton.gameObject.SetActive(active);
		}
	}
}
