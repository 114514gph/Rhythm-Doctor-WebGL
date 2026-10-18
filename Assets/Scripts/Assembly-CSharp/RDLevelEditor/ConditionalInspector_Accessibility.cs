using System.Collections.Generic;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalInspector_Accessibility : ConditionalInspector
	{
		public Dropdown effectList;

		public static readonly List<ConditionalEffectType> AvailableEffectTypes = new List<ConditionalEffectType>
		{
			ConditionalEffectType.Flashy,
			ConditionalEffectType.Narration
		};

		private void OnEnable()
		{
			effectList.ClearOptions();
			effectList.AddOptionsWithEnumArray(AvailableEffectTypes.ToArray());
			effectList.onValueChanged.RemoveAllListeners();
			effectList.value = 0;
			effectList.onValueChanged.AddListener((int index) =>
			{
				base.editor.conditionalsPanel.UpdateConditional();
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
		}

		public override void UpdateData(Conditional rawConditional)
		{
			Conditional_Accessibility conditional_Accessibility = (Conditional_Accessibility)rawConditional;
			effectList.value = AvailableEffectTypes.IndexOf(conditional_Accessibility.effectType);
		}

		public override Conditional Save(int id)
		{
			return new Conditional_Accessibility(id)
			{
				effectType = AvailableEffectTypes[effectList.value]
			};
		}
	}
}
