using DG.Tweening;
using UnityEngine;

public class RDRooftop : RDEnvironment
{
	public enum RooftopMode
	{
		Regular = 0,
		Summer = 1,
		Autumn = 2
	}

	public GameObject regularParent;

	public GameObject summerParent;

	public GameObject autumnParent;

	public Transform regularSky;

	public Transform summerSky;

	public Transform autumnSky;

	public Transform regularSkyscrapers;

	public Transform summerSkyscrapers;

	public Transform autumnSkyscrapers;

	public Transform regularRoof;

	public Transform summerRoof;

	public Transform autumnRoof;

	private RooftopMode mode;

	private Tween skyTween;

	private Tween skyscraperTween;

	private Tween roofTween;

	private Transform currentSky
	{
		get
		{
			if (mode == RooftopMode.Regular)
			{
				return regularSky;
			}
			if (mode == RooftopMode.Summer)
			{
				return summerSky;
			}
			return autumnSky;
		}
	}

	private Transform currentSkyscraper
	{
		get
		{
			if (mode == RooftopMode.Regular)
			{
				return regularSkyscrapers;
			}
			if (mode == RooftopMode.Summer)
			{
				return summerSkyscrapers;
			}
			return autumnSkyscrapers;
		}
	}

	private Transform currentRoof
	{
		get
		{
			if (mode == RooftopMode.Regular)
			{
				return regularRoof;
			}
			if (mode == RooftopMode.Summer)
			{
				return summerRoof;
			}
			return autumnRoof;
		}
	}

	public void SetMode(RooftopMode newMode)
	{
		mode = newMode;
		regularParent.SetActive(mode == RooftopMode.Regular);
		summerParent.SetActive(mode == RooftopMode.Summer);
		autumnParent.SetActive(mode == RooftopMode.Autumn);
		Reset();
	}

	public void TweenSky(float y, float dur, Ease ease)
	{
		skyTween?.Kill();
		if (dur != 0f)
		{
			skyTween = currentSky.DOLocalMoveY(y, dur).SetEase(ease);
		}
		else
		{
			currentSky.LocalMoveY(y);
		}
	}

	public void TweenSkyscraper(float y, float dur, Ease ease)
	{
		skyscraperTween?.Kill();
		if (dur != 0f)
		{
			skyscraperTween = currentSkyscraper.DOLocalMoveY(y, dur).SetEase(ease);
		}
		else
		{
			currentSkyscraper.LocalMoveY(y);
		}
	}

	public void TweenRoof(float y, float dur, Ease ease)
	{
		roofTween?.Kill();
		if (dur != 0f)
		{
			roofTween = currentRoof.DOLocalMoveY(y, dur).SetEase(ease);
		}
		else
		{
			currentRoof.LocalMoveY(y);
		}
	}

	public void Reset()
	{
		KillTweens();
		currentSkyscraper.LocalMoveY(0f);
		currentRoof.LocalMoveY(0f);
	}

	private void KillTweens()
	{
		skyscraperTween?.Kill();
		roofTween?.Kill();
	}

	public static RooftopMode ModeFromTheme(RDTheme theme)
	{
		return theme switch
		{
			RDTheme.Rooftop => RooftopMode.Regular, 
			RDTheme.RooftopSummer => RooftopMode.Summer, 
			RDTheme.RooftopAutumn => RooftopMode.Autumn, 
			_ => RooftopMode.Regular, 
		};
	}
}
