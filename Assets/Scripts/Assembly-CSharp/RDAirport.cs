using DG.Tweening;
using UnityEngine;

public class RDAirport : RDEnvironment
{
	public Transform[] scrollingClouds;

	public Transform landingPlane;

	protected override void Start()
	{
		base.Start();
		for (int i = 0; i < scrollingClouds.Length; i++)
		{
			scrollingClouds[^(i + 1)].DOLocalMoveX(-600f, 100f * (1f + (float)i * 1.666f)).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
		}
	}
}
