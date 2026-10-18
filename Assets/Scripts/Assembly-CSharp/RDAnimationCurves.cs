using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationCurves", menuName = "RhythmDoctor/Animation Curves", order = 1)]
public class RDAnimationCurves : ScriptableObject
{
	[Serializable]
	public struct AnimCurve
	{
		public string name;

		public AnimationCurve curve;
	}

	public AnimCurve[] curves;

	public AnimationCurve Get(string key)
	{
		AnimCurve[] array = curves;
		for (int i = 0; i < array.Length; i++)
		{
			AnimCurve animCurve = array[i];
			if (animCurve.name == key)
			{
				return animCurve.curve;
			}
		}
		Debug.Log("Curve " + key + " not found!");
		return null;
	}
}
