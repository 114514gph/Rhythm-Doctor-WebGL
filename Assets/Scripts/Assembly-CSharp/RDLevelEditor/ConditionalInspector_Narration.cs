using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalInspector_Narration : ConditionalInspector
	{
		public Toggle onToggle;

		public Toggle offToggle;

		private void OnEnable()
		{
			onToggle.onValueChanged.AddListener(delegate
			{
				base.editor.conditionalsPanel.UpdateConditional();
			});
		}

		public override void UpdateData(Conditional rawCond)
		{
			(((Conditional_Narration)rawCond).narrationEnabled ? onToggle : offToggle).isOn = true;
		}

		public override Conditional Save(int id)
		{
			return new Conditional_Narration(id)
			{
				narrationEnabled = onToggle.isOn
			};
		}
	}
}
