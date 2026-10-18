using System;
using UnityEngine;

[Serializable]
public class NarrationAction
{
	public NarrationActionName actionName;

	public KeyCode[] keys;

	public bool combo;

	public NarrationAction(NarrationActionName actionName, KeyCode[] keys, string customActionName)
	{
		this.actionName = actionName;
		this.keys = keys;
	}

	public string GetNarration()
	{
		string text = (combo ? RDString.Get("narration.pressActionCombo") : RDString.Get($"narration.pressAction{keys.Length}"));
		for (int i = 0; i < keys.Length; i++)
		{
			KeyCode keyCode = keys[i];
			text = ((keyCode != KeyCode.None && keyCode != KeyCode.UpArrow && keyCode != KeyCode.DownArrow && keyCode != KeyCode.LeftArrow && keyCode != KeyCode.RightArrow && keyCode != KeyCode.Space && keyCode != KeyCode.Escape && keyCode != KeyCode.Return && keyCode != KeyCode.LeftControl && keyCode != KeyCode.LeftShift) ? text.Replace($"[key{i + 1}]", keyCode.ToString()) : text.Replace($"[key{i + 1}]", RDString.Get("narration." + keyCode)));
		}
		return text.Replace("[action]", RDString.Get("narration." + actionName));
	}
}
