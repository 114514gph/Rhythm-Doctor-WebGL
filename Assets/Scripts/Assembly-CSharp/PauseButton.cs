using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PauseButton : GeneralPauseButton
{
	public Button button;

	private bool focused;

	public void Initialize(PauseContentName pauseContentName)
	{
		button = GetComponent<Button>();
	}

	public override void SetFocus(bool focus)
	{
		_ = base.game.pauseMenu;
		focused = focus;
	}

	public override void Select()
	{
		printe("buttonselect: " + index + " go: " + base.gameObject.name);
		_ = base.game.pauseMenu;
	}

	public void ShowAsSelected()
	{
		focused = false;
	}

	private void Update()
	{
		_ = base.game.pauseMenu;
		_ = focused;
	}
}
