using DG.Tweening;
using UnityEngine;

public class RDGirlWard : RDBase
{
	public GameObject windowBoy;

	public tk2dSpriteAnimator windowBoyAnim;

	public void BoyRun()
	{
		windowBoyAnim.Play("GirlWard_Boy_Run");
		windowBoy.transform.DOLocalMoveX(150f, 2f).SetEase(Ease.Linear);
	}
}
