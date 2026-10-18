using System;
using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Trailer : LevelBase
{
	public BeatOneshot lastOneshot;

	private bool ballInited;

	private float fallingBaseballsBaseSpd = -1f;

	public Level_Trailer(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		yield return null;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 34)
		{
			int scale = ((!base.game.currentLevel.fullResolutionRenderTextures) ? GC.RoomRenderTextureScale : 0);
			base.room1.renderTexture.Release();
			UnityEngine.Object.Destroy(base.room1.renderTexture);
			RenderTexture renderTexture = RDClass.Vfx.CreateRenderTexture(scale, "Debug", 24);
			base.room1.renderTexture = renderTexture;
			base.room1.camera.targetTexture = renderTexture;
			base.room1.quadMaterial.mainTexture = renderTexture;
		}
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 26)
		{
			base.game.hpController.Show(show: true);
			base.game.hpController.bossHP.SetMaxHP(CountHits());
			SetUpFlashMarginFeedback(RowType.Oneshot);
			scrVfxControl.instance.flashText[0].fontSize = 0;
			scrVfxControl.instance.flashText[1].fontSize = 0;
			scrVfxControl.instance.flashText[3].fontSize = 0;
		}
		if (base.conductor.barNumber == 27)
		{
			base.game.currentLevel.flashTextEnabled = false;
		}
		if (base.conductor.barNumber == 28)
		{
			base.game.hpController.player1HP.gameObject.SetActive(value: false);
			base.game.hpController.player2HP.gameObject.SetActive(value: false);
			base.game.hpController.bossHP.gameObject.SetActive(value: false);
		}
		if (base.conductor.barNumber == 40)
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.infiniteZoom = true;
				RDClass.Vfx.hudCamera.cullingMask = LayerMask.GetMask("GameHUD", "Effects", "InfiniteZoom");
				base.game.hpController.player1HP.gameObject.SetActive(value: true);
				base.game.hpController.bossHP.gameObject.SetActive(value: true);
			}, 1f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.infiniteZoom = false;
				RDClass.Vfx.hudCamera.cullingMask = LayerMask.GetMask("GameHUD", "Effects");
				base.game.hpController.player1HP.gameObject.SetActive(value: false);
				base.game.hpController.bossHP.gameObject.SetActive(value: false);
			}, 2f);
		}
	}

	public override void OnHit(HitType hitType, Beat beat = null)
	{
		if (base.conductor.barNumber >= 24 && base.conductor.barNumber < 28)
		{
			base.game.hpController.bossHP.DealDamage(hitType);
		}
	}

	public void MoveSky(int room, float y, float durBeats, string easeStr)
	{
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		base.game.rooms[room].rooftop.GetComponent<RDRooftop>().TweenSky(y, dur, ease);
	}

	public void MoveSkyscrapers(int room, float y, float durBeats, string easeStr)
	{
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		base.game.rooms[room].rooftop.GetComponent<RDRooftop>().TweenSkyscraper(y, dur, ease);
	}

	public void MoveRooftop(int room, float y, float durBeats, string easeStr)
	{
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		base.game.rooms[room].rooftop.GetComponent<RDRooftop>().TweenRoof(y, dur, ease);
	}

	public void ToggleBaseball(bool on)
	{
		RDRoom targetRoom = base.room1;
		targetRoom.rotatingVoxelMode = on;
		if (on)
		{
			targetRoom.voxelObj.baseball.SetActive(value: true);
			targetRoom.voxelObj.SetFallingBaseballs(enableBaseballs: false);
			targetRoom.voxelObj.SetVoxelMode(RotatingVoxelMode.Baseball);
		}
		if (ballInited)
		{
			return;
		}
		Vector3 startScale = targetRoom.voxelObj.baseballPivot.localScale;
		scrVolumeTracker.Add((float s) =>
		{
			if (targetRoom.voxelObj.gameObject.activeSelf)
			{
				targetRoom.voxelObj.baseball.transform.localScale = startScale * s;
			}
		}, 1.5f, 2.5f);
		ballInited = true;
	}

	public void ToggleFallingBaseballs(bool on)
	{
		RDRoom rDRoom = base.room1;
		rDRoom.rotatingVoxelMode = on;
		if (on)
		{
			rDRoom.voxelObj.baseball.SetActive(value: false);
			rDRoom.voxelObj.SetFallingBaseballs(enableBaseballs: true);
		}
	}

	public void SetFallingBaseballsSpeed(float spdPercent)
	{
		ParticleSystem.MainModule main = base.room1.voxelObj.fallingBaseballs.GetComponent<ParticleSystem>().main;
		if (fallingBaseballsBaseSpd == -1f)
		{
			fallingBaseballsBaseSpd = main.simulationSpeed;
		}
		main.simulationSpeed = fallingBaseballsBaseSpd * (spdPercent / 100f);
	}

	public void TriggerTwoPlayer()
	{
		GC.twoPlayerMode = !GC.twoPlayerMode;
		if (base.game != null)
		{
			base.game.EnsureSwitchPlayersState();
		}
		if (base.game != null)
		{
			base.game.OnChangedPlayerMode();
		}
	}
}
