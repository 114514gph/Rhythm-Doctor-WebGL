using DG.Tweening;
using UnityEngine;

public class RDBoyWard : RDBase
{
	public GameObject windowGirl;

	public tk2dSpriteAnimator windowGirlAnim;

	public void GirlRun()
	{
		windowGirlAnim.Play("BoyWard_Girl_Run");
		windowGirl.transform.DOLocalMoveX(150f, 2f).SetEase(Ease.Linear);
	}
}
