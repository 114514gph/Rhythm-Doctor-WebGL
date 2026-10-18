using UnityEngine.UI;

public class CustomInputField : InputField
{
	private bool lastFocusState;

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (!lastFocusState && base.isFocused)
		{
			MoveTextEnd(shift: true);
		}
		lastFocusState = base.isFocused;
	}
}
