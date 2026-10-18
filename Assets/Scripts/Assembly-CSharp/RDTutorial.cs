using DG.Tweening;
using UnityEngine;

public class RDTutorial : RDBase
{
	public Transform notesTr;

	public tk2dTiledSprite notes;

	private void Start()
	{
		notesTr.localPosition = new Vector2(-40f, -40f);
		notesTr.DOLocalMove(new Vector2(40f, 40f), 1f).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
	}

	public void SetNotesColor(Color color)
	{
		notes.color = color;
	}
}
