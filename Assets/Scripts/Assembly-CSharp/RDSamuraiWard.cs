using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RDSamuraiWard : RDBase
{
	private bool isOpen;

	private bool isEmpty;

	private bool lanternsStarted;

	private bool openedOnce;

	private float baseLanternsY;

	public float oldLanternsY;

	public float newLanternsY;

	public Vector2 oldVasePos;

	public Vector2 newVasePos;

	[Header("Show Lanterns")]
	public tk2dSpriteAnimator[] lanternTails;

	public Transform lanternsMasterContainer;

	public Transform lanternsContainer;

	public float showLanternsDuration;

	public Ease showLanternsEase;

	public float showLanternsTargetY;

	public float fastLanternsDuration;

	public Ease fastLanternsEase;

	public bool legacyLanternBehavior = true;

	[Header("Lanterns Idle")]
	public float lanternIdleDuration;

	public Ease lanternIdleEase;

	public float lanternIdleHeight;

	public tk2dSpriteAnimator[] lanterns;

	[Header("Open Doors")]
	public tk2dSpriteAnimator backgroundDoors;

	public tk2dSpriteAnimator vase;

	public tk2dSpriteAnimator particleEffects;

	public GameObject landscape;

	public tk2dSprite[] lanternGlows;

	public Color lanternGlowColor;

	public tk2dSprite particle1;

	public tk2dSprite particle2;

	public GameObject letterBox;

	private void Awake()
	{
		baseLanternsY = lanternsContainer.transform.localPosition.y;
	}

	public void ShowLanterns()
	{
		ToggleLanternPositions(enabled: true, fast: false);
	}

	public void ToggleLanternPositions(bool enabled, bool fast)
	{
		MoveLanterns(enabled, fast ? fastLanternsDuration : showLanternsDuration, fast ? fastLanternsEase : showLanternsEase);
	}

	public void MoveLanterns(bool show, float duration, Ease ease)
	{
		float endValue = (show ? showLanternsTargetY : baseLanternsY);
		lanternsContainer.transform.DOLocalMoveY(endValue, duration).SetEase(ease);
	}

	public void MoveLanternsInstant(bool show)
	{
		lanternsContainer.transform.localPosition = lanternsContainer.transform.localPosition.WithY(show ? showLanternsTargetY : baseLanternsY);
	}

	public IEnumerator OpenDoors(bool opened = true, bool instant = false, bool lightLanterns = true)
	{
		if (isOpen == opened)
		{
			yield break;
		}
		isOpen = opened;
		if (!openedOnce)
		{
			particle1.DOColor(Color.clear, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
			particle2.DOColor(Color.white, 0.4f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo)
				.SetDelay(0.2f);
			particleEffects.Play();
		}
		if (lightLanterns)
		{
			tk2dSprite[] array = lanternGlows;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DOColor(opened ? lanternGlowColor : lanternGlowColor.WithAlpha(0f), instant ? 0f : 0.5f);
			}
		}
		if (instant)
		{
			backgroundDoors.Sprite.SetSprite(scrVfxControl.instance.scBackground, "SamuraiWard_BackgroundDoorsEmpty/" + (opened ? "3" : "0"));
			if (!isEmpty)
			{
				vase.Sprite.SetSprite(scrVfxControl.instance.scBackground, "SamuraiWard_VaseSS/" + (opened ? "0" : "3"));
			}
			if (lightLanterns)
			{
				PlayLanternIdle(opened);
			}
			PlayLanternTailIdle(base.enabled);
		}
		else
		{
			backgroundDoors.Play("SamuraiWard_BackgroundDoorsEmpty" + (opened ? "" : "_Reverse"));
			if (!isEmpty)
			{
				vase.Play("SamuraiWard_VaseSS" + (opened ? "" : "_Reverse"));
			}
			if (lightLanterns)
			{
				tk2dSpriteAnimator[] array2 = lanterns;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].Play("SamuraiWard_LanternTurn" + (opened ? "On" : "Off"));
				}
			}
			yield return new WaitUntil(() => !backgroundDoors.Playing);
			if (lightLanterns)
			{
				PlayLanternIdle(opened);
			}
			if (!legacyLanternBehavior)
			{
				PlayLanternTailIdle(base.enabled);
			}
		}
		if (opened)
		{
			openedOnce = true;
		}
	}

	public void PlayLanternIdle(bool enabled = true)
	{
		if (!lanternsStarted && !enabled)
		{
			return;
		}
		int num = 0;
		tk2dSpriteAnimator[] array = lanterns;
		foreach (tk2dSpriteAnimator tk2dSpriteAnimator2 in array)
		{
			if (enabled)
			{
				tk2dSpriteAnimator2.Play("SamuraiWard_LanternIdle");
			}
			else
			{
				tk2dSpriteAnimator2.Sprite.SetSprite(scrVfxControl.instance.scBackground, "SamuraiWard_Lantern2/0");
			}
			if (!lanternsStarted)
			{
				float delay = lanternIdleDuration / (float)lanterns.Length * (float)num++;
				tk2dSpriteAnimator2.transform.DOLocalMoveY(tk2dSpriteAnimator2.transform.localPosition.y + lanternIdleHeight, lanternIdleDuration).SetEase(lanternIdleEase).SetLoops(-1, LoopType.Yoyo)
					.SetDelay(delay);
			}
			else if (!enabled)
			{
				tk2dSpriteAnimator2.transform.DOPause();
			}
			else
			{
				tk2dSpriteAnimator2.transform.DOPlay();
			}
		}
		lanternsStarted = enabled;
	}

	public void PlayLanternTailIdle(bool enabled = true)
	{
		tk2dSpriteAnimator[] array = lanternTails;
		foreach (tk2dSpriteAnimator tk2dSpriteAnimator2 in array)
		{
			if (enabled)
			{
				tk2dSpriteAnimator2.Play("SamuraiWard_LanternTailLoop");
			}
			else
			{
				tk2dSpriteAnimator2.Stop();
			}
		}
	}

	public void SetToEmptyVersion(bool empty)
	{
		isEmpty = empty;
		landscape.SetActive(!empty);
		particle1.gameObject.SetActive(!empty);
		particle2.gameObject.SetActive(!empty);
		particleEffects.gameObject.SetActive(!empty);
		vase.Sprite.SetSprite(scrVfxControl.instance.scBackground, "SamuraiWard_VaseSS/" + ((isOpen && !empty) ? "0" : "3"));
	}

	public void SetOldVersion(bool old)
	{
		lanternsMasterContainer.LocalMoveY(old ? oldLanternsY : newLanternsY);
		vase.transform.localPosition = (old ? oldVasePos : newVasePos);
		SetVaseForeground(!old);
		letterBox.SetActive(old);
	}

	public void SetVaseForeground(bool foreground)
	{
		string sortingLayerName = (foreground ? RDSortingLayer.Foreground : RDSortingLayer.Background).ToString();
		vase.GetComponent<Renderer>().sortingLayerName = sortingLayerName;
	}

	private void OnDestroy()
	{
		tk2dSpriteAnimator[] array = lanterns;
		foreach (tk2dSpriteAnimator tk2dSpriteAnimator2 in array)
		{
			if (tk2dSpriteAnimator2 != null)
			{
				DOTween.Kill(tk2dSpriteAnimator2.transform);
			}
		}
	}
}
