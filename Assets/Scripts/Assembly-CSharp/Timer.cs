using DG.Tweening;
using UnityEngine;

public class Timer : MonoBehaviour
{
	public static void Add(TweenCallback action, float delay, bool ignoreTimeScale = true)
	{
		DOVirtual.DelayedCall(delay, action, ignoreTimeScale);
	}
}
