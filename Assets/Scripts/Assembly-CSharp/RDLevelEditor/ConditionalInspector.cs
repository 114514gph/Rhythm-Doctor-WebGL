using UnityEngine;

namespace RDLevelEditor
{
	public abstract class ConditionalInspector : RDEditorBase
	{
		public ConditionalType typeName;

		public GameObject container;

		public RectTransform rectTransform;

		public void Toggle(bool show)
		{
			container.SetActive(show);
		}

		public abstract void UpdateData(Conditional conditional);

		public abstract Conditional Save(int id);
	}
}
