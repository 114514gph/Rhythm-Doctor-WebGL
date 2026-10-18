using DG.Tweening;
using UnityEngine;

public class MovingArrow : MonoBehaviour
{
	public bool invertDirection;

	private float mInitialPosition;

	private bool mIsUp;

	private bool mIsPulsing;

	private bool movedBackAfterPulse = true;

	private Tween pulseTween;

	private void Start()
	{
		mInitialPosition = base.transform.localPosition.y;
	}

	private void Update()
	{
		mIsUp = Time.timeSinceLevelLoad % 1f < 0.5f;
		if (!mIsUp && !mIsPulsing)
		{
			movedBackAfterPulse = true;
		}
		if (movedBackAfterPulse)
		{
			float num = (invertDirection ? (-1f) : 1f);
			SetHeight(mIsUp ? num : 0f);
		}
	}

	private void SetHeight(float height)
	{
		Vector2 vector = base.transform.localPosition;
		vector.y = mInitialPosition + height;
		base.transform.localPosition = vector;
	}

	public void Pulse()
	{
		if (pulseTween != null)
		{
			pulseTween.Kill();
		}
		mIsPulsing = true;
		movedBackAfterPulse = false;
		float num = (invertDirection ? (-1f) : 1f);
		SetHeight(num * 3f);
		pulseTween = base.transform.DOLocalMoveY(mInitialPosition, 0.25f).SetEase(Ease.OutCubic).OnComplete(() =>
		{
			mIsPulsing = false;
		});
	}
}
