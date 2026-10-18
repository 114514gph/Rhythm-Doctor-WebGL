using UnityEngine;

[CreateAssetMenu(fileName = "NarrationData", menuName = "RhythmDoctor/NarrationData", order = 6)]
public class NarrationData : ScriptableObject
{
	public NarrationAction[] actions;
}
