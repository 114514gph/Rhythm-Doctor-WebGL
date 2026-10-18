using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class EditorDropdown : RDBase
{
	private Dropdown dropdown;

	private void Awake()
	{
		dropdown = GetComponent<Dropdown>();
	}

	private void OnEnable()
	{
		dropdown.Hide();
	}

	private void OnDisable()
	{
		dropdown.Hide();
	}
}
