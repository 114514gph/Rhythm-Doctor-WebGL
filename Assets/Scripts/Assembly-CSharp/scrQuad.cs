using DG.Tweening;
using UnityEngine;

public class scrQuad : Entity
{
	private const int DontChangeSortingOrder = -999;

	public float alpha
	{
		set
		{
			base.sprite.color = base.sprite.color.WithAlpha(value);
		}
	}

	private new void Awake()
	{
		Setup();
		isTintable = false;
	}

	public void SetSquare(int x1, int y1, int x2, int y2, int layer = -999)
	{
		base.sprite.ScaleX(x2 - x1);
		base.sprite.ScaleY(y2 - y1);
		SetSortingOrder(layer);
		base.x = x1;
		base.y = y2;
	}

	public void SetSquareGlobal(int x1, int y1, int x2, int y2, int layer = -999)
	{
		base.sprite.ScaleX(x2 - x1);
		base.sprite.ScaleY(y2 - y1);
		SetSortingOrder(layer);
		base.xGlobal = x1;
		base.yGlobal = y2;
	}

	public void SetSquareCenter(int _x, int _y, int height, int width, int layer = -999)
	{
		SetSortingOrder(layer);
		base.x = _x;
		base.y = _y;
		base.sprite.ScaleX(width);
		base.sprite.ScaleY(height);
	}

	public void SetColor(Color color)
	{
		base.sprite.color = color;
	}

	public void SetSortingOrder(int layer)
	{
		if (layer != -999)
		{
			base.sprite.SortingOrder = layer;
		}
	}

	public void SetSortingLayerName(string layer)
	{
		GetComponent<MeshRenderer>().sortingLayerName = layer;
	}

	public void Flash(float time = 0.5f, float alpha = 1f)
	{
		base.sprite.DOKill();
		base.sprite.color = base.sprite.color.WithAlpha(alpha);
		base.sprite.DOFade(0f, time).Done();
	}

	public Tweener FlashPartial(float alpha)
	{
		base.sprite.DOKill();
		base.sprite.color = base.sprite.color.WithAlpha(alpha);
		return base.sprite.DOFade(0f, 0.5f).Done();
	}

	public void ReverseFlash(float time = 0.5f, float alpha = 1f)
	{
		if (base.sprite.color.a != 0f)
		{
			base.sprite.color = base.sprite.color.WithAlpha(0f);
		}
		base.sprite.DOFade(alpha, time).Done();
	}

	public Tweener TweenColorTo(Color color, float time)
	{
		base.sprite.DOKill(complete: true);
		return base.sprite.DOColor(color, time).Done();
	}

	public Tweener TweenColorFromTo(Color startColor, Color endColor, float time)
	{
		base.sprite.DOKill(complete: true);
		base.sprite.color = startColor;
		return base.sprite.DOColor(endColor, time).Done();
	}

	public Tweener TweenColorFromTo(Color startColor, Color endColor, float time, Ease ease)
	{
		base.sprite.DOKill(complete: true);
		base.sprite.color = startColor;
		return base.sprite.DOColor(endColor, time).SetEase(ease).Done();
	}

	public new void LateUpdate()
	{
		base.LateUpdate();
		shaderRenderer.enabled = base.sprite.color.a != 0f;
	}
}
