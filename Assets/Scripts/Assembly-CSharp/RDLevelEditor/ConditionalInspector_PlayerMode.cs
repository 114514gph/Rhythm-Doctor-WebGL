using System.Collections.Generic;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalInspector_PlayerMode : ConditionalInspector
	{
		public Dropdown playerMode;

		private void Awake()
		{
			playerMode.AddOptions(new List<string>
			{
				RDString.Get("editor.Conditionals.playerModeOne"),
				RDString.Get("editor.Conditionals.playerModeTwo")
			});
			playerMode.onValueChanged.AddListener((int index) =>
			{
				base.editor.conditionalsPanel.UpdateConditional();
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
		}

		public override void UpdateData(Conditional rawConditional)
		{
			Conditional_PlayerMode conditional_PlayerMode = (Conditional_PlayerMode)rawConditional;
			playerMode.value = (conditional_PlayerMode.playerModeIsTwoPlayer ? 1 : 0);
		}

		public override Conditional Save(int id)
		{
			return new Conditional_PlayerMode(id)
			{
				playerModeIsTwoPlayer = (playerMode.value == 1)
			};
		}
	}
}
