using UnityEngine;
using UnityEngine.UI;

public class PauseArrowButton : GeneralPauseButton
{
	public Text valueLabel;

	public GameObject rightArrow;

	public GameObject leftArrow;

	private Vector2 buttonImageStartSize;

	private void Initialize()
	{
		rectTransform = GetComponent<RectTransform>();
		buttonImageStartSize = rightArrow.GetComponentInChildren<RectTransform>().sizeDelta;
	}

	public override void SetFocus(bool focus)
	{
		_ = base.game.pauseMenu;
		rightArrow.SetActive(focus);
		leftArrow.SetActive(focus);
	}

	public void PlayArrowAnimation(bool isRight)
	{
		_ = base.game.pauseMenu;
		(isRight ? rightArrow : leftArrow).GetComponentInChildren<RectTransform>();
	}

	public override void Select()
	{
	}
}
