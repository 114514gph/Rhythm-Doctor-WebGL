using DG.Tweening;
using UnityEngine;

public class BungeeRope : RDBase
{
	public SubdivisionSpotlightController lightsController;

	public Transform rope;

	private void Awake()
	{
		rope.localPosition = Vector2.down * 2f;
		rope.DOLocalMoveY(2f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
		lightsController.Init();
		lightsController.DoSubdiv(0, 0f, 1);
	}
}
