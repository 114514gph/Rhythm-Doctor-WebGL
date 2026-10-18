using DG.Tweening;
using UnityEngine;

public class scrHeart : Entity
{
	public RowEntity ent;

	private bool explodeCharging;

	private int prevCrackFrame;

	private const string HeartBlueAnimPrefix = "HeartBlue";

	public float rotationMultiplier = 1f;

	public float rotationSpeedMultiplier = 1f;

	private void Update()
	{
		RDUtils.Rotate2D(base.transform, Mathf.Sin((float)base.conductor.audioPos * rotationSpeedMultiplier) * 10f * rotationMultiplier);
	}

	public new void Setup()
	{
		base.Setup();
		animPrefixName = "Heart";
		if (base.game.currentLevel is Level_Unbeatable)
		{
			animPrefixName = "HeartUnbeatable";
		}
		if (RDBase.IsHalloweenWeek())
		{
			LevelBase currentLevel = base.game.currentLevel;
			if (currentLevel is Level_OrientalTechno || currentLevel is Level_OrientalDubstep)
			{
				animPrefixName = "HeartPumpkin";
			}
		}
		base.animator.AnimationCompleted = OnAnimEnd;
		PlayAnim("slowdamage");
		base.animator.Pause();
		base.animator.SetFrame(0);
		explodeCharging = false;
	}

	public override void OnBeat()
	{
		if (IsCurrentEntityClip("chickling"))
		{
			PlayAnim("chickling");
		}
	}

	private void OnAnimEnd(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		if (IsCurrentEntityClip("fastfracture") && AnimExists("chickling"))
		{
			PlayAnim("chickling");
		}
		if (animPrefixName == "HeartBlue" && IsCurrentEntityClip("infection"))
		{
			base.animator.Paused = true;
			PlayAnim("slowdamage");
			base.animator.SetFrame(prevCrackFrame);
			rotationSpeedMultiplier = 2f;
			DOTween.To(() => rotationMultiplier, (float x) =>
			{
				rotationMultiplier = x;
			}, 2f, 5f).SetEase(Ease.InSine);
		}
	}

	public void HitHappy(bool forceInstant = false)
	{
		if (base.game.currentLevel.heartExplodeType != HeartExplodeType.Disabled)
		{
			if (!forceInstant && base.game.currentLevel.heartExplodeType != HeartExplodeType.Instant)
			{
				explodeCharging = true;
				base.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
				SpriteFX.ReverseFlash(base.sprite);
			}
			else
			{
				SpriteFX.Flash(base.sprite, 1f, 0.5f);
				Explode();
				Beat();
			}
		}
	}

	public void Explode()
	{
		if (base.game.currentLevel.heartExplodeType != HeartExplodeType.Disabled)
		{
			SpriteFX.Flash(base.sprite, 1f);
			base.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
			if (base.game.currentLevel.heartExplodeType != HeartExplodeType.Instant)
			{
				explodeCharging = false;
				Beat(hbAction: false);
			}
			RDBase.Vfx.rooms[ent.row.room].MakeShockwave(base.transform.position, ent);
		}
	}

	public void Beat(bool hbAction = true)
	{
		if (IsCurrentEntityClip("slowdamage") && !explodeCharging)
		{
			base.transform.DOKill();
			base.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
			base.transform.DOScale(Vector3.one, 0.3f);
		}
		else if (IsCurrentEntityClip("chickling"))
		{
			base.transform.localScale = Vector3.one;
		}
		foreach (scrExecuteOnEveryHB item in RDExecuteOn.FindAll<scrExecuteOnEveryHB>())
		{
			if (!item.removed && (item.rowID == ent.rowID || item.rowID == -1) && hbAction && item.isActive)
			{
				item.action();
			}
		}
		base.game.currentLevel.OnHeartBeat();
	}

	public void RevertAnimationsToNormal()
	{
		base.sprite.KillAllTweens();
		base.sprite.flash = 0f;
		base.transform.DOKill();
		base.transform.DOScale(Vector3.one, 0.1f);
	}

	public void Infect()
	{
		DOTween.To(() => rotationMultiplier, (float x) =>
		{
			rotationMultiplier = x;
		}, 0f, 1f).SetEase(Ease.OutSine);
		DOVirtual.DelayedCall(base.conductor.crotchet * 2f, () =>
		{
			base.transform.DOShakePosition(0.5f, 2f, 40);
			InfectAnimation();
			DOVirtual.DelayedCall(base.conductor.crotchet * 7f, () =>
			{
				base.transform.DOShakePosition(0.5f, 5f, 80);
			});
		});
	}

	public void InfectAnimation()
	{
		animPrefixName = "HeartBlue";
		base.animator.Paused = false;
		PlayAnim("infection");
	}

	public void QuickInfect()
	{
		ChangeSprite("HeartBlue");
		rotationSpeedMultiplier = 2f;
	}

	public void Reset()
	{
		ChangeSprite("Heart");
		rotationSpeedMultiplier = 1f;
	}

	public void SetCrackFrame(int frame)
	{
		base.animator.SetFrame(frame);
		prevCrackFrame = frame;
	}

	public void SetMisses(float misses)
	{
		if (IsCurrentEntityClip("slowdamage"))
		{
			LevelBase currentLevel = base.game.currentLevel;
			float missesToCrackHeart = base.game.currentLevel.missesToCrackHeart;
			bool num = misses >= missesToCrackHeart;
			int num2 = base.animator.CurrentClip.frames.Length;
			float num3 = misses / missesToCrackHeart;
			int crackFrame = ((num3 != 0f) ? ((int)Mathf.Lerp(1f, num2, num3)) : 0);
			if (!num)
			{
				SetCrackFrame(crackFrame);
			}
			else if (!currentLevel.shouldMakeHealthBar)
			{
				DoFinalCrack();
				RDUtils.Rotate2D(base.transform, 0f);
			}
		}
	}

	public void ChangeSprite(string prefix)
	{
		animPrefixName = prefix;
		PlayAnim("slowdamage");
		base.animator.SetFrame(prevCrackFrame);
	}

	public void DoFinalCrack()
	{
		if (AnimExists("fastfracture"))
		{
			DOTween.To(() => rotationMultiplier, (float x) =>
			{
				rotationMultiplier = x;
			}, 0f, 0.25f).SetEase(Ease.Linear);
			base.animator.Paused = false;
			PlayAnim("fastfracture");
		}
	}
}
