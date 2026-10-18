using DG.Tweening;
using UnityEngine;

public class Spider : RDBase
{
	public Transform smallSpiderTrans;

	public Transform bigSpiderTrans;

	public Transform handTrans;

	public float minHeight;

	public float maxHeight;

	public float animationDuration;

	public Ease lowerEase;

	public Ease raiseEase;

	private scrExecuteOnUpdate handMovement;

	private void Awake()
	{
		smallSpiderTrans.LocalMoveY(maxHeight);
		bigSpiderTrans.LocalMoveY(maxHeight);
		handMovement = scrExecuteOnUpdate.Add(() =>
		{
			handTrans.rotation = Quaternion.identity;
		});
	}

	public void Lower(bool bigSpider)
	{
		Transform target = (bigSpider ? bigSpiderTrans : smallSpiderTrans);
		target.DOKill();
		target.DOLocalMoveY(minHeight, animationDuration).SetEase(lowerEase);
	}

	public void Raise(bool bigSpider)
	{
		Transform target = (bigSpider ? bigSpiderTrans : smallSpiderTrans);
		target.DOKill();
		target.DOLocalMoveY(maxHeight, animationDuration).SetEase(raiseEase);
	}
}
