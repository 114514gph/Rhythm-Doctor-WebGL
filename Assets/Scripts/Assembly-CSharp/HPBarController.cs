using System;
using DG.Tweening;
using UnityEngine;

public class HPBarController : RDBase
{
	public RectTransform rectTransform;

	public CanvasGroup canvasGroup;

	public HPBar player1HP;

	public HPBar player2HP;

	public HPBar bossHP;

	public bool settingVisibilityManually;

	public bool showingVirus;

	public bool noP2BossBar;

	[NonSerialized]
	public bool visible;

	[NonSerialized]
	public HPBar lastNarratedHPBar;

	public float CrackCounterP1
	{
		get
		{
			float num = 0f;
			Row[] rows = base.game.rows;
			foreach (Row row in rows)
			{
				if ((bool)row.ent && (GC.twoPlayerMode ? row.multiplayer : row.singleplayer) == RDPlayer.P1)
				{
					num += row.ent.rowMisses;
				}
			}
			return num;
		}
	}

	public float CrackCounterP2
	{
		get
		{
			float num = 0f;
			Row[] rows = base.game.rows;
			foreach (Row row in rows)
			{
				if ((bool)row.ent && (GC.twoPlayerMode ? row.multiplayer : row.singleplayer) == RDPlayer.P2)
				{
					num += row.ent.rowMisses;
				}
			}
			return num;
		}
	}

	public void Setup()
	{
		bossHP.color = base.gc.bossHPBarColor.WithAlpha(1f);
		bossHP.narrationKey = "bossHealth";
		player1HP.color = base.gc.player1HPBarColor.WithAlpha(1f);
		player1HP.narrationKey = (GC.twoPlayerMode ? "patient1Health" : "patientHealth");
		player2HP.color = base.gc.player2HPBarColor.WithAlpha(1f);
		player2HP.narrationKey = "patient2Health";
		showingVirus = true;
	}

	public void ShowVirus(bool show)
	{
		bossHP.canvasGroup.alpha = (show ? 1f : 0f);
		showingVirus = show;
	}

	public void RefreshBars(bool playRecoverySound = true)
	{
		if (!base.game.currentLevel.shouldMakeHealthBar || !base.gameObject.activeSelf)
		{
			return;
		}
		float missesToCrackHeart = base.game.currentLevel.missesToCrackHeart;
		float crackCounterP = CrackCounterP1;
		float crackCounterP2 = CrackCounterP2;
		bool flag = GC.twoPlayerMode && !noP2BossBar;
		Row[] rows = base.game.rows;
		foreach (Row row in rows)
		{
			RDPlayer rDPlayer = (flag ? row.multiplayer : row.singleplayer);
			if ((bool)row.ent && rDPlayer == RDPlayer.P1)
			{
				if (player1HP.useMistakes || player1HP.damageDealt != crackCounterP)
				{
					player1HP.maxHP = missesToCrackHeart;
					player1HP.damageDealt = crackCounterP;
					player1HP.UpdateBar();
					NarrateHealth(player1HP);
				}
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		rows = base.game.rows;
		foreach (Row row2 in rows)
		{
			RDPlayer rDPlayer2 = (GC.twoPlayerMode ? row2.multiplayer : row2.singleplayer);
			if ((bool)row2.ent && rDPlayer2 == RDPlayer.P2)
			{
				if (player2HP.useMistakes || player2HP.damageDealt != crackCounterP2)
				{
					player2HP.maxHP = missesToCrackHeart;
					player2HP.damageDealt = crackCounterP2;
					player2HP.UpdateBar();
					NarrateHealth(player2HP);
				}
				break;
			}
		}
	}

	public void Show(bool show, float beats = 1f)
	{
		if (base.gameObject.activeSelf)
		{
			visible = show;
			float num = base.conductor.crotchet * beats;
			float num2 = (showingVirus ? 0f : 11f);
			float endValue = (show ? num2 : rectTransform.sizeDelta.y);
			rectTransform.DOKill();
			rectTransform.DOAnchorPosY(endValue, base.conductor.crotchet * beats, snapping: true);
			canvasGroup.DOKill();
			if (show)
			{
				canvasGroup.DOFade(1f, num / 3f).SetEase(Ease.OutExpo).Play();
			}
			else
			{
				canvasGroup.DOFade(0f, num / 3f).SetEase(Ease.OutExpo).SetDelay(num * 2f / 3f)
					.Play();
			}
			player2HP.gameObject.SetActive(GC.twoPlayerMode && !noP2BossBar);
		}
	}

	public void NarrateHealth(HPBar hpBar)
	{
		if (base.game.startTheGameCalled && hpBar.startCalled && !(hpBar == bossHP) && hpBar.lastNarratedHealth != 0 && base.game.currentLevel.narrateHP && visible)
		{
			int lastNarratedHealth = Math.Max(Convert.ToInt32(hpBar.Health * 100f), 0);
			if (lastNarratedHPBar != hpBar)
			{
				Narration.Say(RDString.Get("narration.gameplay." + hpBar.narrationKey).Replace("[percentage]", lastNarratedHealth.ToString()), NarrationCategory.Notification, false);
			}
			else
			{
				Narration.Say(lastNarratedHealth.ToString(), NarrationCategory.Notification, false);
			}
			hpBar.lastNarratedHealth = lastNarratedHealth;
			lastNarratedHPBar = hpBar;
		}
	}
}
