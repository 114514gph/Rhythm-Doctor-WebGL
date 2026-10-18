using UnityEngine;
using UnityEngine.UI;

public class DropdownAutoScroll : MonoBehaviour
{
	private ScrollRect scrollRect;

	private Dropdown dropdown;

	private bool didUpdate;

	private void Update()
	{
		if (!didUpdate)
		{
			didUpdate = true;
			if (dropdown == null)
			{
				dropdown = GetComponentInParent<Dropdown>();
			}
			if (scrollRect == null)
			{
				scrollRect = GetComponent<ScrollRect>();
			}
			if (dropdown.options.Count > 1)
			{
				scrollRect.verticalScrollbar.value = 1f - 1f / (float)(dropdown.options.Count - 1) * (float)dropdown.value;
			}
		}
	}
}
