using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : RDBase
{
	public const float HealthAnimDuration = 0.5f;

	[NonSerialized]
	public float maxHP = 1f;

	[NonSerialized]
	public float damageDealt;

	[NonSerialized]
	public float compensatedDamage;

	[NonSerialized]
	public bool compensateMisses;

	[NonSerialized]
	public bool useMistakes;

	[NonSerialized]
	public int lastNarratedHealth;

	[NonSerialized]
	public bool startCalled;

	[NonSerialized]
	public string narrationKey;

	public RectTransform fill;

	public Image bar;

	public Image flash;

	public Image thermometer;

	public CanvasGroup canvasGroup;

	private Tween healthTween;

	public float Health
	{
		get
		{
			if (useMistakes)
			{
				return 1f - base.game.currentLevel.numMistakes / base.game.currentLevel.missesToCrackHeart;
			}
			return 1f - damageDealt / (maxHP - compensatedDamage);
		}
	}

	public Color color
	{
		set
		{
			bar.color = value;
		}
	}

	public void SetMaxHP(int hp, bool compensateMisses = true)
	{
		maxHP = hp;
		damageDealt = 0f;
		compensatedDamage = 0f;
		this.compensateMisses = compensateMisses;
		UpdateBar(instant: true);
	}

	public void DealDamage(HitType hitType = HitType.Hit, int amount = 1, bool instant = false)
	{
		if (hitType == HitType.Hit)
		{
			damageDealt += amount;
		}
		else if (compensateMisses)
		{
			compensatedDamage += amount;
		}
		if (hitType == HitType.Hit || Health <= 0f)
		{
			UpdateBar(instant);
		}
	}

	public void ResetDamage()
	{
		damageDealt = 0f;
		UpdateBar(instant: true);
	}

	public void UpdateBar(bool instant = false)
	{
		Vector2 vector = new Vector2(Health, 1f);
		if (instant)
		{
			if (healthTween != null && healthTween.IsActive())
			{
				healthTween.Complete();
			}
			fill.anchorMax = vector;
			return;
		}
		if (healthTween != null && healthTween.IsActive())
		{
			healthTween?.Kill();
		}
		flash.color = Color.white;
		healthTween = fill.DOAnchorMax(vector, 0.5f).OnComplete(() =>
		{
			flash.color = Color.clear;
		});
	}

	private void Start()
	{
		lastNarratedHealth = 100;
		startCalled = true;
	}

	private void Update()
	{
		bool flag = Health <= 0.3f;
		bool flag2 = Time.timeSinceLevelLoad % base.conductor.crotchet < base.conductor.crotchet / 2f;
		thermometer.color = ((flag & flag2) ? Color.red : Color.white);
	}
}
