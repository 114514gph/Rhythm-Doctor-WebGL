using System;

public class SelectableObject : SelectableEntity
{
	[NonSerialized]
	public string action;

	public void ActionToPerform(Difficulty difficulty, string objectName)
	{
		string text = ((difficulty == Difficulty.Normal) ? "_Normal" : "_Hard");
		objectName += text;
		base.ink.Run("diaLevelSelect", objectName, () =>
		{
			scnLevelSelect instance = scnLevelSelect.instance;
			instance.controlsAreLocked = false;
			instance.dialogueIsRunning = false;
		});
	}
}
