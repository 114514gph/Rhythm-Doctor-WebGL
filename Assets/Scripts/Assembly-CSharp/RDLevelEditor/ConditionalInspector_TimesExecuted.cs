using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalInspector_TimesExecuted : ConditionalInspector
	{
		public InputField timesExecutedMaxTimes;

		private void OnEnable()
		{
			timesExecutedMaxTimes.onValueChanged.RemoveAllListeners();
			timesExecutedMaxTimes.text = "0";
			timesExecutedMaxTimes.onValueChanged.AddListener((string s) =>
			{
				base.editor.conditionalsPanel.UpdateConditional();
			});
		}

		public override void UpdateData(Conditional rawConditional)
		{
			Conditional_TimesExecuted conditional_TimesExecuted = (Conditional_TimesExecuted)rawConditional;
			timesExecutedMaxTimes.text = conditional_TimesExecuted.maxTimes.ToString();
		}

		public override Conditional Save(int id)
		{
			return new Conditional_TimesExecuted(id)
			{
				maxTimes = int.Parse(timesExecutedMaxTimes.text)
			};
		}
	}
}
