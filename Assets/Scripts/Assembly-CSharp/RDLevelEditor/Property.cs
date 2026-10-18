using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class Property : MonoBehaviour
	{
		public Text label;

		public Button labelButton;

		public RectTransform controlContainer;

		public string offString;

		public static readonly int propertySize = 154;

		public static readonly int labelSize = 77;

		public static readonly int controlSize = 77;

		public PropertyControl control { get; set; }

		public BasePropertyInfo propertyInfo { get; set; }

		protected bool UpdateControlUsed(LevelEvent_Base levelEvent)
		{
			if (propertyInfo is NullablePropertyInfo nullablePropertyInfo)
			{
				bool flag = nullablePropertyInfo.IsUsed(levelEvent);
				control.gameObject.SetActive(flag);
				label.text = label.text.Replace(offString, "") + (flag ? "" : offString);
				label.color = (flag ? Color.white : Color.gray);
				label.GetComponent<RDColorHoverEventTrigger>().exitColor = label.color;
				int num = ((flag && !propertyInfo.controlAttribute.multiline) ? labelSize : propertySize);
				if (!flag)
				{
					UpdatePropertySizeWithLabel(num, label.rectTransform.sizeDelta.y);
				}
				else
				{
					control.SetPropertyControlHeight(controlContainer.sizeDelta.y, updateControlObject: true, updateProperty: true);
				}
				return flag;
			}
			return true;
		}

		private void UpdateControlVisibility(LevelEvent_Base levelEvent)
		{
			bool flag = propertyInfo.enableIf == null || propertyInfo.enableIf(levelEvent);
			bool flag2 = !levelEvent.inspectorPanel.levelEventInfo.usesRow || levelEvent.inspectorPanel.theresRows;
			controlContainer.parent.gameObject.SetActive(flag & flag2);
		}

		private void UpdatePropertySizeWithLabel(float width, float height)
		{
			label.rectTransform.sizeDelta = new Vector2(width, height);
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(component.sizeDelta.x, height);
			VerticalLayoutGroup component2 = base.transform.parent.GetComponent<VerticalLayoutGroup>();
			component2.enabled = false;
			component2.enabled = true;
		}

		public void UpdateUI(LevelEvent_Base levelEvent)
		{
			bool num = UpdateControlUsed(levelEvent);
			UpdateControlVisibility(levelEvent);
			if (num)
			{
				control.UpdateUI(levelEvent);
			}
		}

		public void Save(LevelEvent_Base levelEvent)
		{
			bool num = UpdateControlUsed(levelEvent);
			UpdateControlVisibility(levelEvent);
			if (num)
			{
				control.Save(levelEvent);
			}
			else
			{
				control.SetEventValue(levelEvent, null);
			}
		}

		public static Property Create(BasePropertyInfo propertyInfo, InspectorPanel inspectorPanel)
		{
			GameObject gameObject = Object.Instantiate(RDConstants.data.propertyPrefab, inspectorPanel.propertiesContainer.transform, worldPositionStays: false);
			Property property = gameObject.GetComponent<Property>();
			property.gameObject.name = propertyInfo.name;
			property.propertyInfo = propertyInfo;
			property.control = PropertyControl.Create(property);
			property.control.AddListeners(inspectorPanel.RegisterForChangeAction);
			property.control.inspectorPanel = inspectorPanel;
			if (property.control is PropertyControl_Button propertyControl_Button)
			{
				ButtonAttribute controlAttribute = (ButtonAttribute)propertyInfo.controlAttribute;
				propertyControl_Button.button.onClick.AddListener(() =>
				{
					LevelEvent_Base currentLevelEvent = inspectorPanel.currentLevelEvent;
					BasePropertyInfo.GetAction(controlAttribute.methodName, propertyInfo.propertyInfo.DeclaringType, currentLevelEvent)();
					inspectorPanel.UpdateUI(currentLevelEvent);
				});
			}
			if (property.control is PropertyControl_BPMCalculator propertyControl_BPMCalculator)
			{
				propertyControl_BPMCalculator.bpmCalculator.currentInspectorPanel = inspectorPanel;
			}
			NullablePropertyInfo nullablePropertyInfo = propertyInfo as NullablePropertyInfo;
			if ((object)nullablePropertyInfo != null)
			{
				property.labelButton.onClick.AddListener(() =>
				{
					bool num = nullablePropertyInfo.IsUsed(inspectorPanel.currentLevelEvent);
					object value = (num ? null : nullablePropertyInfo.toggleOnValue);
					propertyInfo.propertyInfo.SetValue(inspectorPanel.currentLevelEvent, value);
					scrConductor.PlayImmediately(num ? "sndButtonClick" : "sndButtonClickOn", 1f, RDUtils.GetMixerGroup("LevelEditorInspectorPanel"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
					property.UpdateUI(inspectorPanel.currentLevelEvent);
					inspectorPanel.currentEventControl.SaveAndUpdateUI();
				});
			}
			else
			{
				property.labelButton.enabled = false;
				property.labelButton.gameObject.DisableComponent<RDColorHoverEventTrigger>();
			}
			UpdateProperty(property, propertyInfo);
			inspectorPanel.SetupPropertyControl(property.control.transform);
			if (propertyInfo.controlAttribute.labelHeight != 0)
			{
				inspectorPanel.LocalizeLabel(property.label, propertyInfo.customLocalizationKey ?? propertyInfo.name, propertyInfo.customLocalizationKey != null);
			}
			property.offString = RDString.colon + RDString.Get("editor.off");
			return property;
		}

		public static void UpdateProperty(Property property, BasePropertyInfo propertyInfo)
		{
			RectTransform component = property.gameObject.GetComponent<RectTransform>();
			bool multiline = propertyInfo.controlAttribute.multiline;
			Vector2 vector = new Vector2(multiline ? propertySize : labelSize, 14f);
			Vector2 vector2 = new Vector2(multiline ? propertySize : controlSize, 14f);
			bool flag = propertyInfo.controlAttribute.labelHeight != 0;
			property.label.rectTransform.sizeDelta = new Vector2(vector.x, propertyInfo.controlAttribute.labelHeight);
			property.controlContainer.sizeDelta = new Vector2(vector2.x, property.control.GetComponent<RectTransform>().sizeDelta.y);
			property.label.gameObject.SetActive(flag);
			property.controlContainer.anchoredPosition = new Vector2((!flag | multiline) ? 0f : vector.x, (flag && multiline) ? (-propertyInfo.controlAttribute.labelHeight) : 0);
			int labelHeight = propertyInfo.controlAttribute.labelHeight;
			float y = property.controlContainer.sizeDelta.y;
			component.sizeDelta = new Vector2(y: ((!flag) ? 0f : ((!multiline) ? Mathf.Max(labelHeight, y) : ((float)labelHeight))) + (multiline ? y : 0f), x: component.sizeDelta.x);
		}
	}
}
