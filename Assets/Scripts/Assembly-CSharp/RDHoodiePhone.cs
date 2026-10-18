using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RDHoodiePhone : RDBase
{
	private const int fingerToReflectionDistance = 6;

	[Header("Weather")]
	public SpriteRenderer weatherSprite;

	public Sprite weatherSun;

	public Sprite weatherStorm;

	[Header("Stream")]
	public int streamRoomTarget;

	public RectTransform streamPivot;

	public RawImage streamImage;

	public Image streamUIImage;

	private Tween streamUITween;

	[Header("Hand")]
	public Transform handPivot;

	public Transform handFinger;

	public Transform handReflection;

	private Tween handTween;

	private Tween fingerTween;

	private Tween reflectionTween;

	private void Update()
	{
		if (!(base.game == null) && base.game.rooms != null)
		{
			RDRoom rDRoom = base.game.rooms[streamRoomTarget];
			if (rDRoom.camera.enabled)
			{
				streamImage.texture = rDRoom.renderTexture;
				streamImage.texture.filterMode = FilterMode.Point;
				streamImage.color = Color.white;
			}
			else
			{
				streamImage.texture = null;
				streamImage.color = Color.black;
			}
		}
	}

	public void SetWeather(bool sunny)
	{
		weatherSprite.sprite = (sunny ? weatherSun : weatherStorm);
	}

	public void ToggleStream(bool show)
	{
		streamPivot.gameObject.SetActive(show);
	}

	public void ToggleStreamUI(bool show, float dur)
	{
		streamUITween?.Kill();
		float alpha = (show ? 1f : 0f);
		if (dur != 0f)
		{
			streamUITween = streamUIImage.DOColor(streamUIImage.color.WithAlpha(alpha), dur);
		}
		else
		{
			streamUIImage.color = streamUIImage.color.WithAlpha(alpha);
		}
	}

	public void MoveHand(Vector2 pos, float dur, Ease ease)
	{
		handTween?.Kill();
		if (dur != 0f)
		{
			handTween = handPivot.DOLocalMove(pos, dur).SetEase(ease);
		}
		else
		{
			handPivot.LocalMoveXY(pos.x, pos.y);
		}
	}

	public void ToggleHandPress(bool pressing, float dur, Ease ease)
	{
		fingerTween?.Kill();
		reflectionTween?.Kill();
		int num = (pressing ? (-6) : 0);
		int num2 = (pressing ? 6 : 0);
		if (dur != 0f)
		{
			fingerTween = handFinger.DOLocalMoveX(num, dur).SetEase(ease);
			reflectionTween = handReflection.DOLocalMoveX(num2, dur).SetEase(ease);
		}
		else
		{
			handFinger.LocalMoveX(num);
			handReflection.LocalMoveX(num2);
		}
	}
}
