using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalInspector_Custom : ConditionalInspector
	{
		public InputField customExpression;

		private void OnEnable()
		{
			customExpression.onValueChanged.RemoveAllListeners();
			customExpression.text = "";
			customExpression.onValueChanged.AddListener((string s) =>
			{
				base.editor.conditionalsPanel.UpdateConditional();
			});
		}

		public override void UpdateData(Conditional rawConditional)
		{
			Conditional_Custom conditional_Custom = (Conditional_Custom)rawConditional;
			customExpression.text = conditional_Custom.customExpression;
		}

		public override Conditional Save(int id)
		{
			return new Conditional_Custom(id)
			{
				customExpression = customExpression.text
			};
		}
	}
}
