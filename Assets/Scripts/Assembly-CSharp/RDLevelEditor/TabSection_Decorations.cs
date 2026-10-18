using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class TabSection_Decorations : TabSection
	{
		[Header("Tab-Specific")]
		public Text roomPagesLabel;

		public RectTransform headersListRect;

		public GameObject headerPrefab;

		public List<GameObject> decorationHeaders;

		private float verticalScrollRectLastY = float.NaN;

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
			UpdateHeadersToPage(base.editor.selectedDecorationsTabPageIndex);
			List<LevelEvent_MakeDecorationBase> currentPageDecorationsData = base.editor.currentPageDecorationsData;
			int count = currentPageDecorationsData.Count;
			for (int i = 0; i < count + 1; i++)
			{
				bool flag = i < count;
				bool isNextToLastDecoration = i == count;
				LevelEvent_MakeDecorationBase decorationData = (flag ? currentPageDecorationsData[i] : null);
				DecorationHeader component = decorationHeaders[i].GetComponent<DecorationHeader>();
				if (justUpdateBackgroundColor)
				{
					component.UpdateBackgroundColor(decorationData);
				}
				else
				{
					component.UpdateUI(decorationData, flag, isNextToLastDecoration);
				}
			}
		}

		public void UpdateHeadersToPage(int index)
		{
			List<LevelEvent_MakeDecorationBase> list = new List<LevelEvent_MakeDecorationBase>();
			foreach (LevelEvent_MakeDecorationBase decorationsDatum in base.editor.decorationsData)
			{
				if (decorationsDatum.room == index)
				{
					list.Add(decorationsDatum);
				}
			}
			int count = list.Count;
			int count2 = decorationHeaders.Count;
			if (count + 1 != count2)
			{
				for (int i = 0; i < count2; i++)
				{
					Object.Destroy(decorationHeaders[i]);
				}
				decorationHeaders.Clear();
				for (int j = 0; j < count + 1; j++)
				{
					GameObject gameObject = Object.Instantiate(headerPrefab, headersListRect);
					gameObject.name = "rowHeader" + j;
					gameObject.GetComponent<DecorationHeader>().index = j;
					decorationHeaders.Add(gameObject);
				}
				headersListRect.SizeDeltaY(base.timeline.cellHeight * (count + 1));
				base.timeline.UpdateMaxUsedY();
			}
		}

		public override void LateUpdate()
		{
			base.LateUpdate();
			float y = base.timeline.scrollViewVertContent.anchoredPosition.y;
			if (verticalScrollRectLastY != y)
			{
				verticalScrollRectLastY = y;
				headersListRect.AnchorPosY(y);
			}
		}

		public override string DefaultControlName()
		{
			return "Decoration";
		}
	}
}
