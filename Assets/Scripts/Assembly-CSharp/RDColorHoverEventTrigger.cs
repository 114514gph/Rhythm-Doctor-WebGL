using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RDColorHoverEventTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public float duration = 0.1f;

	public Ease ease = Ease.InOutQuad;

	public Color enterColor;

	public Color exitColor;

	private Tween colorTween;

	private Graphic graphic;

	private void Awake()
	{
		graphic = GetComponent<Graphic>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Animate(enterColor);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Animate(exitColor);
	}

	private void Animate(Color color)
	{
		if (colorTween != null && colorTween.active)
		{
			colorTween.Kill();
		}
		if (graphic != null)
		{
			colorTween = graphic.DOColor(color, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true);
		}
	}
}
