using System.Collections.Generic;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ConditionalInspector_LastHit : ConditionalInspector
	{
		private readonly OffsetType[] LastHitTypes = new OffsetType[7]
		{
			OffsetType.Perfect,
			OffsetType.SlightlyEarly,
			OffsetType.SlightlyLate,
			OffsetType.VeryEarly,
			OffsetType.VeryLate,
			OffsetType.AnyEarlyOrLate,
			OffsetType.Missed
		};

		public Dropdown lastHitRow;

		public Dropdown lastHitType;

		private void Awake()
		{
			lastHitType.AddOptionsWithEnumArray(LastHitTypes);
			lastHitType.onValueChanged.AddListener((int index) =>
			{
				base.editor.conditionalsPanel.UpdateConditional();
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
		}

		private void OnEnable()
		{
			lastHitRow.ClearOptions();
			lastHitRow.AddOptions(new List<string> { RDString.Get("editor.Conditionals.anyRow") });
			lastHitRow.AddOptionsWithRowsList();
			lastHitRow.onValueChanged.RemoveAllListeners();
			lastHitRow.value = 0;
			lastHitRow.onValueChanged.AddListener((int index) =>
			{
				base.editor.conditionalsPanel.UpdateConditional();
				LevelEditorPlaySound("sndListClick", "LevelEditorInspectorPanel");
			});
		}

		public override void UpdateData(Conditional rawConditional)
		{
			Conditional_LastHit conditional_LastHit = (Conditional_LastHit)rawConditional;
			lastHitRow.value = conditional_LastHit.row + 1;
			int num = 0;
			OffsetType[] lastHitTypes = LastHitTypes;
			foreach (OffsetType offsetType in lastHitTypes)
			{
				if (conditional_LastHit.resultType == offsetType)
				{
					break;
				}
				num++;
			}
			lastHitType.value = num;
		}

		public override Conditional Save(int id)
		{
			return new Conditional_LastHit(id)
			{
				row = lastHitRow.value - 1,
				resultType = LastHitTypes[lastHitType.value]
			};
		}
	}
}
