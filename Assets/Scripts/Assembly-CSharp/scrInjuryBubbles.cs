using UnityEngine;

public class scrInjuryBubbles : MonoBehaviour
{
	public ParticleSystem[] allSystems;

	public Transform bubbleWallTrans;

	public Transform bubbleTrans;

	public Transform barrierTrans;

	public void ToggleSystems(bool on)
	{
		ParticleSystem[] array = allSystems;
		foreach (ParticleSystem particleSystem in array)
		{
			if (on)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
		}
		bubbleWallTrans.gameObject.SetActive(on);
	}
}
