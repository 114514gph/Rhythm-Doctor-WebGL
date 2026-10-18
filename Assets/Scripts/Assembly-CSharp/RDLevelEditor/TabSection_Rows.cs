using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class TabSection_Rows : TabSection
	{
		public Text roomPagesLabel;

		public List<GameObject> rowHeaders;

		public RectTransform rowsListRect;

		public override void Setup()
		{
			base.Setup();
			roomPagesLabel.text = RDString.Get("editor.room");
			RDStringToUIText.Apply(roomPagesLabel);
		}

		public override void UpdateUI()
		{
			UpdateUIInternal(justUpdateBackgroundColor: false);
		}

		public void UpdateUIInternal(bool justUpdateBackgroundColor)
		{
			List<LevelEvent_MakeRow> currentPageRowsData = base.editor.currentPageRowsData;
			int count = currentPageRowsData.Count;
			for (int i = 0; i < 4; i++)
			{
				bool flag = i < count;
				bool isNextToLastRow = i == count;
				LevelEvent_MakeRow rowData = (flag ? currentPageRowsData[i] : null);
				RowHeader component = rowHeaders[i].GetComponent<RowHeader>();
				if (justUpdateBackgroundColor)
				{
					component.UpdateBackgroundColor(rowData);
				}
				else
				{
					component.UpdateUI(rowData, flag, isNextToLastRow);
				}
			}
		}

		public override string DefaultControlName()
		{
			return "AddClassicBeat";
		}
	}
}
