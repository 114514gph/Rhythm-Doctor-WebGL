using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Description : PropertyControl
	{
		public Text text;

		public override GameObject controlObject => text.gameObject;

		private DescriptionAttribute controlAttribute => (DescriptionAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			RectTransform component = text.GetComponent<RectTransform>();
			component.offsetMin = new Vector2(controlAttribute.padding, component.offsetMin.y);
			component.offsetMax = new Vector2(-controlAttribute.padding, component.offsetMax.y);
			text.alignment = controlAttribute.alignment;
			text.text = RDString.Get(controlAttribute.key);
			RDStringToUIText.Apply(text);
			SetPropertyControlHeight(controlAttribute.height);
		}

		public override void Setup()
		{
			if (base.propertyInfo.propertyInfo.SetMethod == null)
			{
				_ = base.propertyInfo.propertyInfo.GetMethod.GetCustomAttribute(typeof(CompilerGeneratedAttribute)) == null;
			}
			else
				_ = 0;
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
		}

		public void SetText(string text)
		{
			this.text.text = text;
		}
	}
}
