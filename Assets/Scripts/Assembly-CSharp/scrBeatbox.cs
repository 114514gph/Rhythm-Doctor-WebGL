using System;
using DG.Tweening;
using UnityEngine;

public class scrBeatbox : Entity
{
	public enum XState
	{
		On = 0,
		Off = 1,
		Gap = 2
	}

	private const string PulseUpArrowAnim = "pulseUpArrow";

	private const string PulseDownArrowAnim = "pulseDownArrow";

	private const string DieUpArrowAnim = "dieUpArrow";

	private const string DieDownArrowAnim = "dieDownArrow";

	private const string PulseInstantAnim = "pulseInstant";

	private const string PulseInstantDoubleAnim = "pulseInstantDouble";

	private const string PulseInstantTripleAnim = "pulseInstantTriple";

	private const string HoldAnim = "hold";

	private const string ReleaseAnim = "release";

	private const string DieAnim = "die";

	private const string FlatAnim = "flat";

	private const string XOnAnim = "X_on";

	private const string XOffAnim = "X_off";

	private const string XGapAnim = "X_gap";

	private const string XGapToOnAnim = "X_gap_to_on";

	private const string XGapToOffAnim = "X_gap_to_off";

	private const string XOnToGapAnim = "X_on_to_gap";

	private const string SyncoAnim = "synco";

	private const string SyncoOnAnim = "synco_on";

	private const string SyncoOffAnim = "synco_off";

	private const float pulseDelay = 0f;

	private const float maxBend = 8f;

	private const float minBend = -8f;

	private const string SmokeTextureShaderProperty = "_SmokeTexture";

	public const string ShineShaderProperty = "_Shine";

	public Entity beatSkip;

	[NonSerialized]
	public Beat theBeatThatPulsedMe;

	[NonSerialized]
	public int index;

	[NonSerialized]
	public bool removeOnNextBar;

	[NonSerialized]
	public char pulseMode = '-';

	[NonSerialized]
	public bool animate;

	[NonSerialized]
	public bool beatboxRefreshedThisFrame;

	[NonSerialized]
	public float shine;

	private Renderer renderer;

	private int rowID;

	private RowEntity ent;

	private int numOfPulsesOnBeatbox;

	private bool isSynco;

	private bool isBendPull;

	private bool isBendRelease;

	private Tween bendTween;

	private Tween arrowFlashTween;

	private Tween arrowSpeedTween;

	private bool isAFlattenableClip
	{
		get
		{
			if (!IsCurrentEntityClip("pulseInstant") && !IsCurrentEntityClip("pulseInstantDouble") && !IsCurrentEntityClip("pulseInstantTriple") && !IsCurrentEntityClip("hold"))
			{
				return IsCurrentEntityClip("release");
			}
			return true;
		}
	}

	private bool isPulsed
	{
		get
		{
			if (!isAFlattenableClip && !IsCurrentEntityClip("pulseDownArrow"))
			{
				return IsCurrentEntityClip("pulseUpArrow");
			}
			return true;
		}
	}

	private bool isRollingAnim => IsCurrentEntityClip("hold");

	private float Bend
	{
		get
		{
			return base.sprite.curve;
		}
		set
		{
			base.sprite.Bend(value);
		}
	}

	public void Setup(int index, int rowID, bool animate, RowEntity ent)
	{
		Setup();
		this.rowID = rowID;
		this.animate = animate;
		this.ent = ent;
		this.index = index;
		PlayAnim("flat");
		base.animator.AnimationCompleted = OnAnimEnd;
		beatSkip.animator.AnimationCompleted = OnBeatSkipAnimEnd;
		theBeatThatPulsedMe = null;
		base.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneExceptWhenMusicPaused;
		beatSkip.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneExceptWhenMusicPaused;
		renderer = GetComponent<Renderer>();
		renderer.material.SetTexture("_SmokeTexture", base.gc.beatboxSmokeTexture);
		UpdateSpriteColor();
	}

	private void Update()
	{
		if ((bool)ent)
		{
			beatboxRefreshedThisFrame = false;
			Vector2 value = new Vector2(Time.timeSinceLevelLoad % 1f * base.gc.beatboxSmokeSpeed.x, Time.timeSinceLevelLoad % 1f * base.gc.beatboxSmokeSpeed.y);
			renderer.material.SetTextureOffset("_SmokeTexture", value);
		}
	}

	public void RefreshPulseStatus()
	{
		bool flag = false;
		bool flag2 = false;
		bool forcePulse = false;
		BeatClassic beat = null;
		int num = 0;
		foreach (Beat beat2 in base.game.beats)
		{
			if (!(beat2 is BeatClassic { dead: false } beatClassic) || beatClassic.rowID != rowID)
			{
				continue;
			}
			if (beatClassic.currentBeatbox != -1)
			{
				if (beatClassic.currentBeatbox == index + 1)
				{
					beat = beatClassic;
					flag = true;
					num++;
					if (beatClassic.justPulsed)
					{
						forcePulse = true;
					}
				}
			}
			else if (beatClassic.beatboxStatuses[index] == BeatboxStatus.Rolling)
			{
				flag2 = true;
			}
		}
		if (flag | flag2)
		{
			PulseOrXPulseOrRoll(beat, flag2, num, forcePulse);
		}
		else if (isPulsed)
		{
			Flatten();
		}
		numOfPulsesOnBeatbox = num;
		beatboxRefreshedThisFrame = true;
	}

	public void PulseOrXPulseOrRoll(Beat beat, bool roll, int _numOfPulses, bool forcePulse = false)
	{
		BeatClassic beatClassic = beat as BeatClassic;
		if (beat != null && beat.playerDrives7thBeat && beatClassic.currentActivatedArrayPosition == 6)
		{
			return;
		}
		ent.SwitchRowTypeAppearance(RowType.Classic);
		base.game.hitStripManager.PingRow(ent.row);
		bool flag = CheckXState() == XState.Off;
		if (flag && !isSynco)
		{
			ent.heart.Beat();
			if (!roll)
			{
				bool flag2 = (_numOfPulses > numOfPulsesOnBeatbox && !base.game.currentLevel.doublePulse) || (_numOfPulses != numOfPulsesOnBeatbox && base.game.currentLevel.doublePulse);
				if ((!isPulsed || isRollingAnim) | flag2 | forcePulse)
				{
					char c = pulseMode;
					if (c == '-' || c == 'b' || c == 'r' || c == 's')
					{
						if (_numOfPulses <= 1 || !base.game.currentLevel.doublePulse)
						{
							PlayAnim("pulseInstant");
							if (!base.game.currentLevel.noBananaBeats && pulseMode != 's')
							{
								isBendPull = (isBendRelease = false);
								bool flag3 = false;
								int flipX = 1;
								int currentActivatedArrayPosition = beatClassic.currentActivatedArrayPosition;
								if (beatClassic.currentActivatedArrayPosition >= 0)
								{
									if (currentActivatedArrayPosition > 0 && beatClassic.beats[currentActivatedArrayPosition].beatboxNumber < beatClassic.beats[currentActivatedArrayPosition - 1].beatboxNumber)
									{
										flipX = -1;
									}
									switch (beatClassic.beats[currentActivatedArrayPosition].bend)
									{
									case BananaBend.Pull:
										isBendPull = true;
										break;
									case BananaBend.PullLittle:
										isBendPull = true;
										flag3 = true;
										break;
									case BananaBend.Release:
										isBendRelease = true;
										break;
									case BananaBend.ReleaseNoSpring:
										isBendRelease = true;
										flag3 = true;
										break;
									}
									if (pulseMode == 'b')
									{
										isBendPull = true;
									}
									if (pulseMode == 'r')
									{
										isBendRelease = true;
									}
								}
								if (isBendPull || isBendRelease)
								{
									bendTween?.Kill();
									float a = (float)beatClassic.beats[currentActivatedArrayPosition].timeToNextBeat;
									if (isBendPull)
									{
										float num = Mathf.Min(a, base.gc.bananaPullMaxDuration);
										bendTween = DOTween.To(() => Bend * (float)flipX, (float x) =>
										{
											Bend = x * (float)flipX;
										}, -8f, num).SetEase(flag3 ? base.gc.beatPullQuick : base.gc.beatPull).SetDelay(num * base.gc.bananaBeatboxDelay)
											.SetUpdate(isIndependentUpdate: true);
									}
									else
									{
										float duration = Mathf.Min(a, flag3 ? base.gc.bananaReleaseNoSpringMaxDuration : base.gc.bananaReleaseMaxDuration);
										bendTween = DOTween.To(() => Bend * (float)flipX, (float x) =>
										{
											Bend = x * (float)flipX;
										}, 8f, duration).SetEase(flag3 ? base.gc.beatReleaseNoSpring : base.gc.beatRelease).SetUpdate(isIndependentUpdate: true);
									}
								}
							}
						}
						else if (_numOfPulses == 2)
						{
							PlayAnim("pulseInstantDouble");
						}
						else
						{
							PlayAnim("pulseInstantTriple");
						}
					}
					else if (pulseMode == 'u')
					{
						PlayAnim("pulseUpArrow");
					}
					else if (pulseMode == 'd')
					{
						PlayAnim("pulseDownArrow");
					}
				}
			}
			else if (!isRollingAnim)
			{
				PlayAnim("hold");
			}
			Timer.Add(() =>
			{
				if (base.sprite != null)
				{
					RDBase.Vfx.rooms[ent.row.room].BeatboxPulsed(base.sprite.transform.position, index + 1);
				}
			}, 0.03f);
			return;
		}
		if (!flag)
		{
			tk2dSprite beatboxXSprite = beatSkip.sprite;
			if (beatboxXSprite != null)
			{
				Timer.Add(() =>
				{
					SpriteFX.Flash(beatboxXSprite, 1f, 0.5f);
				}, 0f);
				beatboxXSprite.DOKill();
				beatboxXSprite.ScaleY(1.5f);
				beatboxXSprite.DOScaleXY(1f, 0.3f).SetEase(Ease.OutCubic).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				RDBaseDllDummy.printem("beatboxSprite es null");
			}
		}
		if (isSynco)
		{
			tk2dSprite tk2dSprite2 = base.sprite;
			if (tk2dSprite2 != null)
			{
				float duration2 = (float)beatClassic.beats[beatClassic.currentActivatedArrayPosition].timeToNextBeat;
				tk2dSprite2.DOKill();
				ArrowPulseAnimation(tk2dSprite2, duration2);
			}
			else
			{
				RDBaseDllDummy.printem("synco arrow sprite es null");
			}
		}
	}

	public void TweenBend(float amount, float duration, Ease ease)
	{
		bendTween?.Kill();
		bendTween = DOTween.To(() => Bend, (float x) =>
		{
			Bend = x;
		}, amount, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.Done();
	}

	public void ArrowPulseAnimation(tk2dSprite syncoSprite, float duration)
	{
		if (arrowFlashTween != null)
		{
			arrowFlashTween.Kill();
		}
		if (arrowSpeedTween != null)
		{
			arrowSpeedTween.Kill();
		}
		arrowFlashTween = DOTween.To(() => syncoSprite.flash, (float x) =>
		{
			syncoSprite.flash = x;
		}, 0.7f, 0.1f);
		Timer.Add(() =>
		{
			SpriteFX.Flash(syncoSprite, 0.5f, 0.7f).OnStart(() =>
			{
				RDBaseDllDummy.printem("flash start");
			}).OnComplete(() =>
			{
				RDBaseDllDummy.printem("flash complete");
			});
		}, 0f + duration);
		float fps = base.animator.CurrentClip.fps;
		base.animator.ClipFps = fps * 3f;
		arrowSpeedTween = DOTween.To(() => base.animator.ClipFps, (float x) =>
		{
			base.animator.ClipFps = x;
		}, fps, 0.5f).SetEase(Ease.OutQuad).Pause()
			.SetDelay(duration)
			.Play();
	}

	public void Flatten()
	{
		if (isBendPull || isBendRelease)
		{
			bendTween = DOTween.To(() => Bend, (float x) =>
			{
				Bend = x;
			}, 0f, 0.1f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		}
		if (isAFlattenableClip)
		{
			PlayAnim("die");
		}
		else if (IsCurrentEntityClip("pulseUpArrow"))
		{
			PlayAnim("dieUpArrow");
		}
		else if (IsCurrentEntityClip("pulseDownArrow"))
		{
			PlayAnim("dieDownArrow");
		}
	}

	public void OnAnimEnd(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		if (IsCurrentEntityClip("die") || IsCurrentEntityClip("dieDownArrow") || IsCurrentEntityClip("dieUpArrow") || IsCurrentEntityClip("synco_on"))
		{
			PlayAnim(isSynco ? "synco" : "flat");
		}
	}

	public void OnBeatSkipAnimEnd(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		if (IsCurrentEntityClip("X_off") || IsCurrentEntityClip("X_gap_to_off"))
		{
			beatSkip.visible = false;
		}
	}

	public void SetBeatSkip(bool shouldPulse)
	{
		XState xState = CheckXState();
		if (!shouldPulse)
		{
			if (base.visible)
			{
				beatSkip.visible = true;
			}
			switch (xState)
			{
			case XState.Off:
				if (isSynco)
				{
					beatSkip.PlayAnim("X_gap");
				}
				else
				{
					beatSkip.PlayAnim("X_on");
				}
				return;
			case XState.Gap:
				if (!isSynco)
				{
					beatSkip.PlayAnim("X_gap_to_on");
					return;
				}
				break;
			}
			if (xState == XState.On && isSynco)
			{
				beatSkip.PlayAnim("X_on_to_gap");
			}
		}
		else
		{
			switch (xState)
			{
			case XState.On:
				beatSkip.PlayAnim("X_off");
				break;
			case XState.Gap:
				beatSkip.PlayAnim("X_gap_to_off");
				break;
			}
		}
	}

	public void SetSynco(bool shouldShow)
	{
		isSynco = shouldShow;
		if (isSynco && (IsCurrentEntityClip("flat") || IsCurrentEntityClip("synco_off")))
		{
			PlayAnim("synco_on");
		}
		if (!isSynco && (IsCurrentEntityClip("synco") || IsCurrentEntityClip("synco_on")))
		{
			PlayAnim("synco_off");
		}
	}

	public XState CheckXState()
	{
		if (beatSkip.IsCurrentEntityClip("X_on") || beatSkip.IsCurrentEntityClip("X_gap_to_on"))
		{
			return XState.On;
		}
		if (beatSkip.IsCurrentEntityClip("X_gap") || beatSkip.IsCurrentEntityClip("X_on_to_gap"))
		{
			return XState.Gap;
		}
		return XState.Off;
	}

	private new void LateUpdate()
	{
		base.LateUpdate();
		UpdateSpriteColor();
		renderer.material.SetFloat("_Shine", shine - (float)index);
	}

	private void UpdateSpriteColor()
	{
		RDPlayer currentPlayer = ent.row.GetCurrentPlayer();
		Color color = ((currentPlayer == RDPlayer.CPU) ? base.gc.playerCPUStripColor : ((!GC.twoPlayerMode) ? base.gc.beatboxColor : ((currentPlayer == RDPlayer.P1) ? base.gc.player1StripColor : base.gc.player2StripColor)));
		base.sprite.color = color.WithAlpha(base.sprite.color.a);
	}

	public override void OnNewBar()
	{
		if (removeOnNextBar)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
