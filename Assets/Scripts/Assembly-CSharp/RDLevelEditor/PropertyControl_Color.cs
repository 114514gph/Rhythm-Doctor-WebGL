using System;
using UnityEngine;

namespace RDLevelEditor
{
	public class PropertyControl_Color : PropertyControl
	{
		public ColorPicker colorPicker;

		public override GameObject controlObject => colorPicker.gameObject;

		private ColorAttribute controlAttribute => (ColorAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			if (!controlAttribute.alignToLeft)
			{
				RectTransform component = colorPicker.GetComponent<RectTransform>();
				Vector2 vector = (component.anchorMin = new Vector2(1f, 0.5f));
				Vector2 pivot = (component.anchorMax = vector);
				component.pivot = pivot;
				component.anchoredPosition = Vector2.zero;
			}
			ColorPicker obj = colorPicker;
			obj.onEndEdit = (ColorPicker.EndHandler)Delegate.Combine(obj.onEndEdit, (ColorPicker.EndHandler)((string _) =>
			{
				action(null, "sndEditorValueChange");
			}));
		}

		public override void Setup()
		{
			colorPicker.storesAlpha = ((ColorPropertyInfo)base.propertyInfo.NullableUnderlying).hasAlpha;
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			ColorOrPalette colorOrPalette = (ColorOrPalette)GetEventValue(levelEvent);
			colorPicker.colorOrPalette = colorOrPalette;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			ColorOrPalette colorOrPalette = colorPicker.colorOrPalette;
			SetEventValue(levelEvent, colorOrPalette);
		}
	}
}
