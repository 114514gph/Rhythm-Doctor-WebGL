using UnityEngine;
using UnityEngine.UI;

public class PropertyControl_RowLegacy : MonoBehaviour
{
	public RectTransform rectTransform;

	public Dropdown dropdown;

	public GameObject dropdownContainer;

	public Text noRowsDescription;

	public void UpdateHeight()
	{
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, noRowsDescription.gameObject.activeInHierarchy ? 42 : 14);
	}
}
