using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Checkbox : PropertyControl
	{
		public Toggle checkbox;

		public Text text;

		public GameObject disabledIcon;

		public override GameObject controlObject => checkbox.gameObject;

		private CheckboxAttribute controlAttribute => (CheckboxAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			text.GetComponent<Button>().onClick.AddListener(() =>
			{
				checkbox.isOn = !checkbox.isOn;
				action(null, "sndButtonClick");
			});
			RDStringToUIText.Apply(text);
			InspectorPanel.RegisterToggle(checkbox, action, null, "sndButtonClick");
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			checkbox.isOn = bool.Parse(eventValue.ToString());
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			bool isOn = checkbox.isOn;
			SetEventValue(levelEvent, isOn);
		}

		public void ShowDisabledIcon(bool active)
		{
			if (disabledIcon != null)
			{
				disabledIcon.SetActive(!active);
			}
		}

		private void Start()
		{
			if (text != null)
			{
				inspectorPanel.LocalizeLabel(text, base.propertyInfo.customLocalizationKey ?? base.propertyInfo.name, base.propertyInfo.customLocalizationKey != null);
			}
		}
	}
}
