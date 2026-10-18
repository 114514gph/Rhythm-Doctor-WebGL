using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Button : PropertyControl
	{
		public Button button;

		public Text text;

		public override GameObject controlObject => button.gameObject;

		private ButtonAttribute controlAttribute => (ButtonAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			text.text = (controlAttribute.key.IsNullOrEmpty() ? controlAttribute.methodName : RDString.Get(controlAttribute.key));
			RDStringToUIText.Apply(text);
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
	}
}
