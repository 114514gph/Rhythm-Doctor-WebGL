using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_PulsePicker : PropertyControl
	{
		public ToggleGroup toggle;

		public Image[] backgrounds;

		private Tween heartShake;

		public override GameObject controlObject => toggle.gameObject;

		private IntPropertyInfo intPropertyInfo => (IntPropertyInfo)base.propertyInfo;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			_ = toggle.transform;
			Image[] array = backgrounds;
			foreach (Image image in array)
			{
				Transform parent = image.transform.parent;
				if (!parent.TryGetComponent<Toggle>(out var component))
				{
					continue;
				}
				if (parent.name != "-1")
				{
					InspectorPanel.RegisterToggle(component, action, null, "sndButtonClick");
					continue;
				}
				InspectorPanel.RegisterToggle(component, action, () =>
				{
					if (heartShake != null && heartShake.active)
					{
						heartShake.Complete();
					}
					heartShake = image.transform.DOShakePosition(0.2f, 1.3f, 40).SetUpdate(isIndependentUpdate: true);
				}, "sndMistakeSmall3");
			}
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			toggle.SetValue(eventValue.ToString());
			UpdateToggleBackground(Convert.ToInt32(eventValue));
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			int value = Convert.ToInt32(toggle.GetValueFromActiveToggle());
			value = Mathf.Clamp(value, intPropertyInfo.min, intPropertyInfo.max);
			SetEventValue(levelEvent, value);
			UpdateToggleBackground(value);
		}

		private void UpdateToggleBackground(int selectedIndex)
		{
			for (int i = 0; i < backgrounds.Length; i++)
			{
				backgrounds[i].enabled = i != selectedIndex;
			}
		}
	}
}
