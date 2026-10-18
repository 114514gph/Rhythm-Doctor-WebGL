using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalInspector_Language : ConditionalInspector
	{
		public Dropdown language;

		private void OnEnable()
		{
			language.ClearOptions();
			language.AddOptionsWithEnumArray(RDString.AvailableLanguages.ToArray());
			language.onValueChanged.RemoveAllListeners();
			language.value = 0;
			language.onValueChanged.AddListener((int index) =>
			{
				base.editor.conditionalsPanel.UpdateConditional();
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
		}

		public override void UpdateData(Conditional rawConditional)
		{
			Conditional_Language conditional_Language = (Conditional_Language)rawConditional;
			language.value = RDString.AvailableLanguages.IndexOf(conditional_Language.languageName);
		}

		public override Conditional Save(int id)
		{
			return new Conditional_Language(id)
			{
				languageName = RDString.AvailableLanguages[language.value]
			};
		}
	}
}
