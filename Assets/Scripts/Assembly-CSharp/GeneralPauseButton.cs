using UnityEngine;

public abstract class GeneralPauseButton : RDBase
{
	public int index;

	protected RectTransform rectTransform;

	public abstract void SetFocus(bool value);

	public abstract void Select();
}
