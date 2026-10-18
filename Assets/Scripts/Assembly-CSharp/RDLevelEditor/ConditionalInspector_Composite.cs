using System.Collections.Generic;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalInspector_Composite : ConditionalInspector
	{
		public Dropdown conditionalsDropdown;

		public InputField expression;

		private void OnEnable()
		{
			List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
			list.Add(new Dropdown.OptionData(RDString.Get("editor.Conditionals.addTag")));
			foreach (Conditional conditional in base.editor.conditionals)
			{
				if (conditional.id != base.editor.conditionalsPanel.editingId)
				{
					Dropdown.OptionData item = new Dropdown.OptionData(conditional.description + " (" + conditional.tag + ")");
					list.Add(item);
				}
			}
			conditionalsDropdown.onValueChanged.RemoveAllListeners();
			conditionalsDropdown.options = list;
			conditionalsDropdown.value = 0;
			conditionalsDropdown.onValueChanged.AddListener((int s) =>
			{
				OnConditionalsDropdownChanged();
			});
			expression.onValueChanged.RemoveAllListeners();
			expression.text = "";
			expression.onValueChanged.AddListener((string s) =>
			{
				base.editor.conditionalsPanel.UpdateConditional();
			});
		}

		private void OnConditionalsDropdownChanged()
		{
			if (conditionalsDropdown.value != 0)
			{
				conditionalsDropdown.onValueChanged.RemoveAllListeners();
				string text = conditionalsDropdown.options[conditionalsDropdown.value].text;
				int num = text.IndexOf('(') + 1;
				expression.text += text.Substring(num, text.Length - 1 - num);
				conditionalsDropdown.value = 0;
				conditionalsDropdown.onValueChanged.AddListener((int s) =>
				{
					OnConditionalsDropdownChanged();
				});
			}
		}

		public override void UpdateData(Conditional rawConditional)
		{
			Conditional_Composite conditional_Composite = (Conditional_Composite)rawConditional;
			expression.text = conditional_Composite.compositeExpression;
		}

		public override Conditional Save(int id)
		{
			return new Conditional_Composite(id)
			{
				compositeExpression = expression.text
			};
		}
	}
}
