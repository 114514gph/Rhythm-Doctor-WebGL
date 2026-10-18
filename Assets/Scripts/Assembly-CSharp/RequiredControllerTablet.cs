using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RequiredControllerTablet : RDBase
{
	public bool useAux;

	[SerializeField]
	private RectTransform container;

	[SerializeField]
	private RectTransform chicken;

	[SerializeField]
	private GameObject p1Disconnect;

	[SerializeField]
	private GameObject p2Disconnect;

	[SerializeField]
	private Text description;

	[SerializeField]
	private Vector2 showPos;

	[SerializeField]
	private Vector2 showPosAux;

	[SerializeField]
	private float hidePosY;

	private Sequence shakeSequence;

	public void Show(float duration, float delay, bool resetPos = false)
	{
	}

	public void Hide(float duration)
	{
	}

	public void HighlightRequiredController()
	{
	}
}
