using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class scrPlayerbox : Entity
{
	public int rowID;

	public RowEntity ent;

	public bool doingEntranceAnimation;

	public Beat currentHoldBeat;

	private bool beatReleased;

	[NonSerialized]
	public float defaultPosX;

	public bool holdAssist;

	public bool forceTrueOffset;

	public bool wobblyLines;

	private Color pulseGlowOnFilledHoldBar = new Color(1f, 0f, 237f / 255f, 1f);

	public float clumsyCPUTiming;

	private int lastHitOffset;

	[NonSerialized]
	public Color lastGlowColor = Color.clear;

	public float hitLineLength = 6f;

	private Color glowColor
	{
		get
		{
			return shaderData.borderColor;
		}
		set
		{
			shaderData.borderColor = value;
		}
	}

	private RDPlayer player => ent.row.GetCurrentPlayer();

	public float HitlinePadding => Mathf.Max(0f, hitLineLength - 1f);

	public float PlayerboxDistance => hitLineLength * 3f;

	public float HalfHitlineWidth => HitlinePadding + PlayerboxDistance * 2f + 12f;

	public float HitlineWidth => HalfHitlineWidth * 2f;

	private scrHoldBar holdBar => ent.holdBar;

	public OffsetType releaseOffsetType
	{
		get
		{
			double audioPos = base.conductor.audioPos;
			float releaseMargin = scnGame.GetReleaseMargin(player);
			if ((ent.row.cpuControlled || RDBase.debugSettings.Auto || ((player == RDPlayer.P1) ? scnGame.p1DefibMode : scnGame.p2DefibMode) == DefibMode.Unmissable) && !forceTrueOffset)
			{
				return OffsetType.Perfect;
			}
			if (audioPos < currentHoldBeat.releaseTime - (double)releaseMargin)
			{
				return OffsetType.SlightlyEarly;
			}
			if (audioPos > currentHoldBeat.releaseTime + (double)releaseMargin)
			{
				return OffsetType.SlightlyLate;
			}
			return OffsetType.Perfect;
		}
	}

	public void Setup(int _rowID)
	{
		Setup();
		base.gameObject.tag = "Playerbox";
		animPrefixName = "Beatbox";
		rowID = _rowID;
		base.animator.AnimationCompleted = onAnimEnd;
		base.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneExceptWhenMusicPaused;
		UpdateSpriteColor();
	}

	private void UpdateSpriteColor()
	{
		if (!(ent == null) && ent.row != null && ent.row != null)
		{
			RDPlayer currentPlayer = ent.row.GetCurrentPlayer();
			Color color = ((currentPlayer == RDPlayer.CPU) ? base.gc.playerboxCPUColor : ((!GC.twoPlayerMode) ? base.gc.playerboxColor : ((currentPlayer == RDPlayer.P1) ? base.gc.playerboxP1Color : base.gc.playerboxP2Color)));
			color = color.WithAlpha(base.sprite.color.a);
			ent.lineHitLeft.sprite.color = color;
			ent.lineHitRight.sprite.color = color;
			base.sprite.color = color;
		}
	}

	public void SpaceBarEvent(bool CPUTriggered)
	{
		if (ent.row.dead)
		{
			return;
		}
		double audioPos = base.conductor.audioPos;
		if (CPUTriggered)
		{
			_ = RDBase.Vfx.rooms[ent.row.room];
			base.game.hitStripManager.ShowHitstrip(RDPlayer.CPU);
			if (clumsyCPUTiming == 0f)
			{
				SpriteAnimation characterMarkerAnimation = ent.characterMarkerAnimation;
				if ((object)characterMarkerAnimation != null)
				{
					characterMarkerAnimation.Stop();
					characterMarkerAnimation.PlayFromFrame(1);
				}
			}
		}
		Beat beat = null;
		double num = double.MaxValue;
		List<Beat> beats = base.game.beats;
		for (int num2 = beats.Count - 1; num2 >= 0; num2--)
		{
			Beat beat2 = beats[num2];
			if (!beat2.unhittable && beat2.player == player && beat2.inputTime < num)
			{
				num = beat2.inputTime;
				beat = beat2;
			}
		}
		double num3 = double.MaxValue;
		if (!HitCheckBeat(scnGame.GetHitMargin(player), beat, audioPos) && !base.game.currentLevel.noSmartJudgment)
		{
			foreach (Beat beat4 in base.game.beats)
			{
				if (beat4.inputTime < num3 && beat4.inputTime > num && hitCheckTimeWithinMargin(scnGame.GetHitMargin(player), beat4.inputTime))
				{
					num3 = beat4.inputTime;
				}
			}
		}
		bool flag = false;
		if (num3 != double.MaxValue)
		{
			flag = true;
			num = num3;
		}
		for (int num4 = beats.Count - 1; num4 >= 0; num4--)
		{
			if (num4 < beats.Count)
			{
				Beat beat3 = beats[num4];
				if (beat3.rowID == rowID)
				{
					float num5 = (beat3.bomb ? 0.08f : 0.4f);
					if (player != RDPlayer.CPU && RDInput.emuStates[(int)player].GetKey(RDInput.PlayerEmuKey.Down))
					{
						base.game.spacebarOnNothing = false;
					}
					float hitMargin = scnGame.GetHitMargin(player);
					float margin = ((scnGame.drumMode && beat3.inputTime > audioPos) ? hitMargin : num5);
					if (HitCheckBeat(margin, beat3, audioPos) && Math.Abs(beat3.inputTime - num) < 0.009999999776482582)
					{
						base.game.spacebarOnNothing = false;
						double num6 = audioPos;
						float num7 = (float)(num6 - beat3.inputTime);
						if (beat3.specialRowToTransferHitTo != -1)
						{
							base.game.rows[beat3.specialRowToTransferHitTo].ent.playerBox.Pulse(num7, beat3, CPUTriggered);
						}
						else
						{
							if (beat != null && !beat3.bomb)
							{
								float num8 = Math.Abs(num7);
								if (player == RDPlayer.P2 && num8 < hitMargin)
								{
									scnGame.p2HitTimes.Add(num8);
								}
								else if (player == RDPlayer.P1 && num8 < hitMargin)
								{
									scnGame.p1HitTimes.Add(num8);
								}
							}
							if (!currentHoldBeat)
							{
								if (beat3.playerDrives7thBeat)
								{
									beat3.playerDriven7thBeatHitTime = num6;
								}
								Pulse(num7, beat3, CPUTriggered, beat3.bomb);
							}
						}
						if ((CPUTriggered ? (clumsyCPUTiming == 0f) : HitCheckBeat(RDBase.debugSettings.Auto ? 0.4f : hitMargin, beat3, num6)) && !currentHoldBeat)
						{
							beat3.Create8thBeat();
						}
					}
				}
			}
		}
		if ((bool)currentHoldBeat)
		{
			RDPlayer rDPlayer = player;
			if (rDPlayer == RDPlayer.P1 || rDPlayer == RDPlayer.P2)
			{
				base.game.playerCaughtHoldBeat[(int)player] = currentHoldBeat;
			}
		}
	}

	public bool HitCheckBeat(float margin, Beat beat, double positiontouse = 999.0)
	{
		if (positiontouse == 999.0)
		{
			positiontouse = base.conductor.audioPos;
		}
		if (beat == null)
		{
			return false;
		}
		if (beat.bomb)
		{
			return Math.Abs(positiontouse - beat.inputTime) < 0.07999999821186066;
		}
		if (!beat.isLenientMargins)
		{
			return Math.Abs(positiontouse - beat.inputTime) < (double)margin;
		}
		if (!beat.playerDrives7thBeat)
		{
			return Math.Abs(positiontouse - beat.inputTime) < 0.4000000059604645;
		}
		return positiontouse > beat.inputTime;
	}

	public static bool hitCheckTimeWithinMargin(float margin, double beattime, double positiontouse = 999.0)
	{
		if (positiontouse == 999.0)
		{
			positiontouse = scrConductor.instance.audioPos;
		}
		return Math.Abs(positiontouse - beattime) < (double)margin;
	}

	public HitType Pulse(float timeOffset, Beat beat, bool CPUTriggered = false, bool bomb = false)
	{
		if ((ent.appearance == RowVisibilityMode.Visible || ent.appearance == RowVisibilityMode.OnlyRow) && !ent.entranceSequence.IsActive())
		{
			base.visible = true;
		}
		bool hasHeldPulses = beat.hasHeldPulses;
		OffsetType offsetType = OffsetType.VeryEarly;
		int num = -2;
		if (!beat.isLenientMargins && !bomb && ((!RDBase.debugSettings.Auto && !CPUTriggered) || (CPUTriggered && clumsyCPUTiming != 0f)))
		{
			if (timeOffset > -0.2f)
			{
				offsetType = OffsetType.SlightlyEarly;
				num = -1;
			}
			if (timeOffset > 0f - scnGame.GetHitMargin(player))
			{
				offsetType = OffsetType.Perfect;
				num = 0;
			}
			if (timeOffset > scnGame.GetHitMargin(player))
			{
				offsetType = OffsetType.SlightlyLate;
				num = 1;
			}
			if (timeOffset > 0.2f)
			{
				offsetType = OffsetType.VeryLate;
				num = 2;
			}
		}
		else
		{
			offsetType = OffsetType.Perfect;
			num = 0;
		}
		string text = "";
		string mistakeGroupPathFor2P = GetMistakeGroupPathFor2P();
		if (Mathf.Abs(num) == 0 && !bomb)
		{
			text = "happy";
			RDBase.Vfx.BeatboxPulsed(ent.hitstripCenter.position, 7);
			if (!CPUTriggered && !beat.isHeldClap)
			{
				base.game.FlashBorderFeedback(correct: true, ent.row);
			}
		}
		else if (Mathf.Abs(num) == 1)
		{
			text = "barely";
			bool flag = false;
			if (ent.row.isShadowRealtime && ent.row.shadowOrHostIDRealtime != -1)
			{
				flag = true;
			}
			float pitch = 1f;
			float multiplier = 1f;
			if (num == 1)
			{
				pitch = 0.8f;
				multiplier = 1.25f;
			}
			if ((!flag || ent.row.isShadowRealtime || base.game.rows[ent.row.shadowOrHostIDRealtime].GetCurrentPlayer() != player) && !beat.dontPlayMistakeSound)
			{
				scrConductor.PlayFeedback(GameSoundType.SmallMistake, multiplier, RDUtils.GetMixerGroup(mistakeGroupPathFor2P), pitch, player);
			}
		}
		else if (Mathf.Abs(num) == 2)
		{
			text = "missed";
			bool flag2 = false;
			if (ent.row.isShadowRealtime && ent.row.shadowOrHostIDRealtime != -1)
			{
				flag2 = true;
			}
			float pitch2 = 1f;
			float multiplier2 = 1f;
			if (num == 2)
			{
				pitch2 = 0.8f;
				multiplier2 = 1.25f;
			}
			if ((!flag2 || ent.row.isShadowRealtime || base.game.rows[ent.row.shadowOrHostIDRealtime].GetCurrentPlayer() != player) && !beat.dontPlayMistakeSound)
			{
				scrConductor.PlayFeedback(GameSoundType.BigMistake, multiplier2, RDUtils.GetMixerGroup(mistakeGroupPathFor2P), pitch2, player);
			}
		}
		if (LEDSign.showMarginError)
		{
			LEDSign.statusColor = Color.white;
			string key = "status.tooEarlyFeedback";
			if (num == 0)
			{
				key = "status.perfectFeedback";
			}
			if (num > 0)
			{
				key = "status.tooLateFeedback";
			}
			base.game?.statusText.SetStatusText(RDString.Get(key), null, 4f, narrate: true);
		}
		if (bomb)
		{
			offsetType = OffsetType.Missed;
			text = "missed";
			scrConductor.PlayFeedback(GameSoundType.BigMistake, 1f, RDUtils.GetMixerGroup(mistakeGroupPathFor2P), 1f, player);
		}
		if (!beat.hasHeldPulses || offsetType != OffsetType.Perfect || base.game.currentLevel.countHeldPresses)
		{
			base.game.AddHitOffset(rowID, offsetType);
		}
		if ((num != 0) | bomb)
		{
			if (Time.frameCount > ent.row.lastMissedFrame)
			{
				ent.row.lastMissedFrame = Time.frameCount;
				base.game.OnMistakeOrHeal(timeOffset, beat.weight, ent.row);
				ent.CrackAdvance(beat.weight);
				if (!CPUTriggered)
				{
					base.game.FlashBorderFeedback(correct: false, ent.row);
				}
				if (bomb)
				{
					Beat.RunEventsTaggedOnMiss(rowID);
				}
				if (beat is BeatOneshot beatOneshot)
				{
					beatOneshot.KillSubdivBuddyWeights();
				}
			}
			beat.deadByMistake = true;
			if (beat.hasHeldPulses)
			{
				beat.dead = true;
			}
		}
		if (beat.isHeldClap && num == 0)
		{
			ent.holdBar.visibleHold = true;
			_ = RDBase.Vfx.rooms[ent.row.room];
			RDPlayer currentPlayer = ent.row.GetCurrentPlayer();
			base.game.hitStripManager.ShowHitstrip(currentPlayer, beat);
			currentHoldBeat = beat;
			base.sprite.SortingOrder = ent.lineHitLeft.sprite.SortingOrder - 1;
		}
		HitType hitType = HitType.Hit;
		if (Mathf.Abs(num) == 0)
		{
			hitType = HitType.Hit;
		}
		if (Mathf.Abs(num) == 1)
		{
			hitType = HitType.JustMiss;
		}
		if (Mathf.Abs(num) == 2)
		{
			hitType = HitType.BigMiss;
		}
		if (!beat.unhittable)
		{
			if (hasHeldPulses)
			{
				if (text != "happy")
				{
					ent.ExpressionPlusFX(text);
				}
			}
			else
			{
				ent.ExpressionPlusFX(text);
			}
			foreach (scrExecuteOnHit item in RDExecuteOn.FindAll<scrExecuteOnHit>())
			{
				if ((item.rowID == -1 || item.rowID == rowID) && item.isActive)
				{
					bool flag3 = false;
					bool num2 = num == 0 && item.hitType == HitType.Hit;
					bool flag4 = num < 0 && item.hitType == HitType.EarlyOnly;
					bool flag5 = num > 0 && item.hitType == HitType.LateOnly;
					bool flag6 = Mathf.Abs(num) == 1 && (item.hitType == HitType.JustMiss || item.hitType == HitType.AnyMiss || item.hitType == HitType.AnyMissWithinCounted);
					bool flag7 = Mathf.Abs(num) == 2 && (item.hitType == HitType.BigMiss || item.hitType == HitType.AnyMiss || item.hitType == HitType.AnyMissWithinCounted);
					if (num2 | flag4 | flag5 | flag6 | flag7)
					{
						item.Execute(beat);
						flag3 = true;
					}
					if ((!item.persistent & flag3) || item.autodelete)
					{
						item.Remove();
					}
				}
			}
			bool flag8 = !beat.hasHeldPulses || base.game.currentLevel.countHeldPresses;
			bool hasHeldPulses2 = beat.hasHeldPulses;
			LevelBase currentLevel = base.game.currentLevel;
			if (currentLevel.data != null)
			{
				if (Math.Abs(num) == 0)
				{
					Dictionary<string, List<LevelEvent_Base>> dict = currentLevel.taggedEvents.Filter((KeyValuePair<string, List<LevelEvent_Base>> kvp) => !kvp.Key.Contains("[row"));
					if (flag8)
					{
						currentLevel.RunTagContainingGeneric(dict, true, "[onHit]");
					}
					if (hasHeldPulses2)
					{
						currentLevel.RunTagContainingGeneric(dict, true, "[onHeldPressHit]");
					}
					if (flag8)
					{
						currentLevel.RunTagContaining(true, "[onHit]", "[row" + rowID + "]");
					}
					if (hasHeldPulses2)
					{
						currentLevel.RunTagContaining(true, "[onHeldPressHit]", "[row" + rowID + "]");
					}
				}
				else
				{
					Dictionary<string, List<LevelEvent_Base>> dict2 = currentLevel.taggedEvents.Filter((KeyValuePair<string, List<LevelEvent_Base>> kvp) => !kvp.Key.Contains("[row"));
					if (flag8)
					{
						currentLevel.RunTagContainingGeneric(dict2, true, "[onMiss]");
					}
					if (hasHeldPulses2)
					{
						currentLevel.RunTagContainingGeneric(dict2, true, "[onHeldPressMiss]");
					}
					if (flag8)
					{
						currentLevel.RunTagContaining(true, "[onMiss]", "[row" + rowID + "]");
					}
					if (hasHeldPulses2)
					{
						currentLevel.RunTagContaining(true, "[onHeldPressMiss]", "[row" + rowID + "]");
					}
				}
			}
			if (flag8)
			{
				base.game.currentLevel.OnHit(hitType, beat);
			}
			if (hasHeldPulses2)
			{
				base.game.currentLevel.OnHeldPress(hitType, beat);
			}
		}
		SetHorizontalPosition(num);
		if (hasHeldPulses && num == 0)
		{
			Shake((float)(currentHoldBeat.releaseTime - base.conductor.audioPos), 0, 3);
		}
		if (beat.isLastBeat && base.game.mistakesManager.mistakes == 0f)
		{
			if (Mathf.Abs(num) == 0)
			{
				scrConductor.PlayImmediately("sndApplause", 1f, RDUtils.GetMixerGroup("Applause"));
			}
			else
			{
				scrConductor.PlayImmediately("sndAww", 1f, RDUtils.GetMixerGroup("Applause"));
			}
		}
		if (!hasHeldPulses || (hasHeldPulses && num != 0))
		{
			if (num != 0)
			{
				PlayAnim("pulseAndDieLong");
			}
			else
			{
				string animationName = (bomb ? "X_on_off" : "pulseAndDie");
				PlayAnim(animationName);
			}
		}
		else
		{
			beatReleased = false;
			PlayAnim("hold");
			float PopDuration = 0.3f;
			base.sprite.DOScaleY(1f, PopDuration).SetEase(Ease.OutElastic).OnComplete(() =>
			{
				if (!beatReleased)
				{
					float num6 = (float)(currentHoldBeat.releaseTime - currentHoldBeat.inputTime);
					base.sprite.DOScaleY(1.2f, num6 - PopDuration).SetEase(Ease.OutQuad);
				}
			});
			base.game.currentLevel.lastHeldPress = offsetType;
		}
		beat.hasPulsed7thBeat = true;
		if (beat is BeatClassic beatClassic)
		{
			scrBeatbox[] beatboxes = ent.classicRowController.beatboxes;
			foreach (scrBeatbox scrBeatbox2 in beatboxes)
			{
				if (scrBeatbox2.theBeatThatPulsedMe == beat)
				{
					scrBeatbox2.Flatten();
				}
			}
			beatClassic.currentBeatbox = 7;
			beatClassic.shouldCallRefresh = true;
			if (!beat.hasHeldPulses)
			{
				beat.dead = true;
			}
		}
		if (beat is BeatOneshot beatOneshot2)
		{
			RDWaveManager waveManager = ent.oneshotRowController.waveManager;
			if (wobblyLines || base.game.currentLevel.wobblyLines)
			{
				waveManager.PlayWobble();
			}
			else if (beatOneshot2.assignedToWave != null)
			{
				beatOneshot2.assignedToWave.Flatten();
			}
			else
			{
				Debug.LogWarning("This beat doesn't have a wave assigned to it???");
				waveManager.firstWave.Flatten();
			}
			if (!beat.isHeldClap)
			{
				beat.dead = true;
			}
		}
		if (!CPUTriggered && Time.frameCount != Row.lastHitFrame[(int)player])
		{
			Row.lastHitFrame[(int)player] = Time.frameCount;
			int num4 = Mathf.RoundToInt(timeOffset * 60f);
			if (RDBase.debugSettings.Auto || Mathf.Abs(num4) <= 1)
			{
				num4 = 0;
			}
			base.game.mistakesManager.AddAbsoluteMistake(player, num4);
		}
		base.game.mistakesManager.UpdateConsecutiveMistakes(player, offsetType);
		if (base.game.showMarginsNumerically && !CPUTriggered)
		{
			int num5 = (int)(timeOffset * 1000f);
			string text2 = num5.ToString();
			if (num5 > 0)
			{
				text2 = "+" + text2;
			}
			base.game?.statusText.SetStatusText("[ " + text2 + " " + RDString.Get("editor.unit.ms") + " ]", null, 4f, narrate: false, useUnscaledTime: true);
			Narration.Say(text2 ?? "", NarrationCategory.Notification, false);
		}
		return hitType;
	}

	public void SetHorizontalPosition(int hitOffset)
	{
		SetHorizontalPositionInternal(hitOffset, 24f);
	}

	public void UpdateSize()
	{
		SetHorizontalPositionInternal(lastHitOffset, 0f);
	}

	private void SetHorizontalPositionInternal(int hitOffset, float playerboxWidth)
	{
		if (!doingEntranceAnimation)
		{
			lastHitOffset = hitOffset;
			float num = (float)hitOffset * PlayerboxDistance;
			if (hitOffset != 0 && HitlineWidth > 24f)
			{
				float num2 = Mathf.Sign(hitOffset);
				num = Mathf.Max(18f, Mathf.Abs(num)) * num2;
			}
			base.x = HalfHitlineWidth + num;
			base.transform.localScale = Vector3.one.WithX(playerboxWidth / 24f);
			float num3 = HalfHitlineWidth - playerboxWidth / 2f + num;
			float num4 = num3 / 2f;
			float num5 = HalfHitlineWidth - playerboxWidth / 2f - num;
			float num6 = HitlineWidth - num5 / 2f;
			ent.lineHitLeft.transform.localPosition = Vector3.right * num4;
			ent.lineHitLeft.transform.localScale = Vector3.one.WithX(num3 / 24f);
			ent.lineHitRight.transform.localPosition = Vector3.right * num6;
			ent.lineHitRight.transform.localScale = Vector3.one.WithX(num5 / 24f);
			base.sprite.SortingOrder = ent.lineHitLeft.sprite.SortingOrder - 1;
			if (hitOffset != 0)
			{
				base.sprite.SortingOrder--;
			}
		}
	}

	private void Hide()
	{
		if (!ent.entranceSequence.IsActive())
		{
			base.visible = false;
			SetHorizontalPositionInternal(0, 0f);
		}
	}

	public void onAnimEnd(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		if (IsCurrentEntityClip("die") || IsCurrentEntityClip("pulseAndDie") || IsCurrentEntityClip("X_on_off"))
		{
			Hide();
		}
		else if ((IsCurrentEntityClip("pulseInstant") || IsCurrentEntityClip("pulse")) && !currentHoldBeat)
		{
			Flatten();
		}
	}

	public void Flatten()
	{
		PlayAnim("die");
		base.sprite.DOColor(base.gc.playerboxColor, 0.5f);
	}

	public void SpaceBarReleased(RDPlayer player, bool cpuTriggered = false)
	{
		if (player != this.player)
		{
			return;
		}
		if (cpuTriggered)
		{
			base.game.hitStripManager.Release(player);
			if (player != RDPlayer.CPU)
			{
				RDInput.emuStates[(int)player].SetKey(RDInput.PlayerEmuKey.Up);
			}
		}
		if (!currentHoldBeat || (holdAssist && !(holdAssist & cpuTriggered)))
		{
			return;
		}
		OffsetType offsetType = releaseOffsetType;
		if (scnGame.drumMode && offsetType == OffsetType.SlightlyEarly)
		{
			return;
		}
		StopShake();
		base.game.currentLevel.lastHeldRelease = offsetType;
		float pitch = 1f;
		float multiplier = 1f;
		string mistakeGroupPathFor2P = GetMistakeGroupPathFor2P();
		switch (offsetType)
		{
		case OffsetType.SlightlyEarly:
			SetHorizontalPosition(-2);
			base.game.FlashBorderFeedback(correct: false, ent.row);
			scrConductor.PlayFeedback(GameSoundType.BigMistake, multiplier, RDUtils.GetMixerGroup(mistakeGroupPathFor2P), pitch, player);
			base.game.OnMistakeOrHeal((float)(base.conductor.audioPos - currentHoldBeat.releaseTime), currentHoldBeat.weight, ent.row);
			ent.CrackAdvance();
			PlayAnim("pulseAndDie");
			holdBar.PlayerReleasedEarly();
			ent.heart.RevertAnimationsToNormal();
			break;
		case OffsetType.SlightlyLate:
			SetHorizontalPosition(2);
			base.game.FlashBorderFeedback(correct: false, ent.row);
			pitch = 0.8f;
			multiplier = 1.15f;
			scrConductor.PlayFeedback(GameSoundType.BigMistake, multiplier, RDUtils.GetMixerGroup(mistakeGroupPathFor2P), pitch, player);
			base.game.OnMistakeOrHeal((float)(base.conductor.audioPos - currentHoldBeat.releaseTime), currentHoldBeat.weight, ent.row);
			ent.CrackAdvance();
			ent.heart.RevertAnimationsToNormal();
			Flatten();
			break;
		case OffsetType.Perfect:
			base.game.FlashBorderFeedback(correct: true, ent.row);
			ent.ExpressionPlusFX("happy");
			currentHoldBeat.Create8thBeat();
			ReleaseEffect();
			Flatten();
			if (player != RDPlayer.P1 && player != RDPlayer.P2)
			{
				break;
			}
			foreach (Beat item in base.game.beats.Where((Beat beat) => beat.player == player && beat != currentHoldBeat && beat.inputTime >= currentHoldBeat.inputTime - 0.01 && base.conductor.audioPos >= currentHoldBeat.releaseTime - 0.4100000059604645 && beat.inputTime <= currentHoldBeat.releaseTime + 0.01))
			{
				item.row.playerBox.Pulse((float)(base.conductor.audioPos - item.inputTime), item, CPUTriggered: true);
				item.Create8thBeat();
			}
			break;
		}
		base.game.AddHitOffset(rowID, offsetType);
		LevelBase currentLevel = base.game.currentLevel;
		if (currentLevel.data != null)
		{
			if (offsetType == OffsetType.Perfect)
			{
				Dictionary<string, List<LevelEvent_Base>> dict = currentLevel.taggedEvents.Filter((KeyValuePair<string, List<LevelEvent_Base>> kvp) => !kvp.Key.Contains("[row"));
				currentLevel.RunTagContainingGeneric(dict, true, "[onHit]");
				currentLevel.RunTagContainingGeneric(dict, true, "[onHeldReleaseHit]");
				currentLevel.RunTagContaining(true, "[onHit]", "[row" + rowID + "]");
				currentLevel.RunTagContaining(true, "[onHeldReleaseHit]", "[row" + rowID + "]");
			}
			else
			{
				Dictionary<string, List<LevelEvent_Base>> dict2 = currentLevel.taggedEvents.Filter((KeyValuePair<string, List<LevelEvent_Base>> kvp) => !kvp.Key.Contains("[row"));
				currentLevel.RunTagContainingGeneric(dict2, true, "[onMiss]");
				currentLevel.RunTagContainingGeneric(dict2, true, "[onHeldReleaseMiss]");
				currentLevel.RunTagContaining(true, "[onMiss]", "[row" + rowID + "]");
				currentLevel.RunTagContaining(true, "[onHeldReleaseMiss]", "[row" + rowID + "]");
			}
			currentLevel.OnHit(offsetType, currentHoldBeat);
		}
		glowColor = lastGlowColor;
		base.sprite.DOKill();
		base.sprite.scale = Vector3.one;
		holdBar.visibleHold = false;
		base.game.playerCaughtHoldBeat[(int)player] = null;
		currentHoldBeat.dead = true;
		base.game.autoHeldBeatCounter--;
		if (base.game.showMarginsNumerically && !cpuTriggered)
		{
			int num = (int)((base.conductor.audioPos - currentHoldBeat.releaseTime) * 1000.0);
			string text = num.ToString();
			if (num > 0)
			{
				text = "+" + text;
			}
			LEDSign.status = "[ <color=#AAA>" + text + " " + RDString.Get("editor.unit.ms") + "</color> ]";
			Narration.Say(text ?? "", NarrationCategory.Notification, false);
		}
		bool flag = releaseOffsetType == OffsetType.Perfect;
		currentHoldBeat.DestroyBeat(!flag, evenIfHalfwayPlaying: true, !flag);
		currentHoldBeat = null;
	}

	public string GetMistakeGroupPathFor2P()
	{
		string result = "MistakesParent";
		switch (player)
		{
		case RDPlayer.P1:
			result = "PlayerOneMistakes";
			break;
		case RDPlayer.P2:
			result = "PlayerTwoMistakes";
			break;
		}
		return result;
	}

	private void Update()
	{
		UpdateHold();
	}

	private void UpdateHold()
	{
		if (!currentHoldBeat)
		{
			return;
		}
		float num = RXMath.InverseLerp(currentHoldBeat.inputTime, currentHoldBeat.releaseTime, base.conductor.audioPos);
		float num2 = Mathf.Pow(Mathf.Clamp01(num), 3f);
		glowColor = Color.Lerp(lastGlowColor, pulseGlowOnFilledHoldBar, num2);
		holdBar.SetBarPercentage(num2);
		ent.heart.sprite.flash = Mathf.Lerp(0f, 1f, num2);
		base.game.hitStripManager.SetHoldProgress(player, num);
		if (!beatReleased && base.conductor.audioPos > currentHoldBeat.releaseTime)
		{
			if (ent.row.cpuControlled || RDBase.debugSettings.Auto || holdAssist)
			{
				SpaceBarReleased(player, cpuTriggered: true);
				base.game.handController0.cpuHandRelease();
			}
			else if (releaseOffsetType == OffsetType.Perfect)
			{
				ReleasePop();
				ReleaseEffect();
			}
		}
	}

	private new void LateUpdate()
	{
		base.LateUpdate();
		UpdateSpriteColor();
	}

	public static List<scrPlayerbox> ListAll()
	{
		List<scrPlayerbox> list = new List<scrPlayerbox>();
		Row[] rows = scnGame.instance.rows;
		foreach (Row row in rows)
		{
			if (row.ent != null && row.ent.playerBox != null)
			{
				list.Add(row.ent.playerBox);
			}
		}
		return list;
	}

	public void ReleasePop()
	{
		PlayAnim("pulseInstant");
	}

	public void ReleaseEffect()
	{
		if (!base.game.currentLevel.noHitStrips)
		{
			base.game.hitStripManager.Release(player);
			base.sprite.SortingOrder = 501;
			beatReleased = true;
		}
	}
}
