using UnityEngine;
using UnityEngine.UI;

public class DialogButton : RDBase
{
	public Text label;

	public GameObject border;

	public Button button;

	public RDDialog dialog { get; set; }

	private void Awake()
	{
		button.onClick.AddListener(() =>
		{
			dialog = base.transform.parent.GetComponent<RDDialog>();
			dialog.SelectedOption(this);
		});
	}

	public void ShowSelected()
	{
		border.SetActive(value: true);
	}

	public void ShowUnselected()
	{
		border.SetActive(value: false);
	}
}
