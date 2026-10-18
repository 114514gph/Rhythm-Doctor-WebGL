using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class UpdateUIOnFullRoom : RDEditorBase
	{
		private void Start()
		{
			int num = base.transform.parent.GetSiblingIndex() - 1;
			int[] array = new int[4];
			foreach (LevelEvent_MakeRow rowsDatum in base.editor.rowsData)
			{
				array[rowsDatum.room]++;
			}
			GetComponent<Text>().color = ((array[num] == 4) ? Color.gray : Color.black);
		}

		private void Update()
		{
			Start();
		}
	}
}
