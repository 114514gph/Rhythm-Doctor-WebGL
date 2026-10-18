using System;
using RDLevelEditor;
using UnityEngine;

[Serializable]
public struct RDEditorKeycode
{
	public KeyCode key;

	public bool shift;

	public override string ToString()
	{
		return RDEditorUtils.KeyComboToString(control: false, shift, key, noJoinerColor: true, lowercase: false);
	}
}
