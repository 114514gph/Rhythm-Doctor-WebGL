using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Injury : LevelBase
{
	private scrInjuryBubbles injuryBubbles;

	private BubbleMask bubbleMask;

	private Collider2D[] bubbleShapes;

	private RDHoodiePhone phone;

	private float phoneRoomX;

	private List<LevelEvent_MakeDecorationBase> decorationData;

	private RDAthleteWard _stadium;

	private RDAthleteWard _physioWard;

	private RDMainWard _ward;

	private Tween bubbleTween;

	private Tween phoneTween;

	private Tween streamTween;

	private const float phoneScreenSizeX = 480f;

	private const float phoneScreenSizeY = 280f;

	private const float bottomHandY = -150f;

	private bool ballInited;

	private float fallingBaseballsBaseSpd = -1f;

	private bool wardScrolling;

	private float wardScrollSpeed;

	private float wardStartX;

	private float wardDistance;

	private const float seamlessPartWidth = 510f;

	private CustomSprite janitorTV;

	private RDAthleteWard stadium
	{
		get
		{
			if (_stadium == null)
			{
				_stadium = base.room0.athleteWard.GetComponent<RDAthleteWard>();
			}
			return _stadium;
		}
	}

	private RDAthleteWard physioWard
	{
		get
		{
			if (_physioWard == null)
			{
				_physioWard = base.room1.athleteWard.GetComponent<RDAthleteWard>();
			}
			return _physioWard;
		}
	}

	private RDMainWard ward
	{
		get
		{
			if (_ward == null)
			{
				_ward = base.room2.hospitalWard.GetComponent<RDMainWard>();
			}
			return _ward;
		}
	}

	public Level_Injury(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		injuryBubbles = LoadLevelAsset("RDAthleteWard/Injury/BubbleParticles").GetComponent<scrInjuryBubbles>();
		float rDHeight = RDClass.Vfx.RDHeight;
		injuryBubbles.bubbleWallTrans.LocalMoveXY(0f, rDHeight);
		injuryBubbles.bubbleTrans.LocalMoveXY(0f, rDHeight);
		injuryBubbles.barrierTrans.LocalMoveXY(0f, rDHeight - 16f);
		MoveBubblesToRoom(0);
		injuryBubbles.ToggleSystems(on: false);
		bubbleMask = LoadLevelAsset("RDAthleteWard/Injury/BubbleMask").GetComponent<BubbleMask>();
		bubbleMask.shapes.Add(new GameObject("Bubble Shape Circle").AddComponent<CircleCollider2D>());
		bubbleMask.shapes.Add(new GameObject("Bubble Shape Square").AddComponent<BoxCollider2D>());
		bubbleMask.transform.SetParent(base.game.rooms[3].transform, worldPositionStays: false);
		foreach (Collider2D shape in bubbleMask.shapes)
		{
			shape.transform.parent = bubbleMask.transform;
			if (shape is CircleCollider2D)
			{
				(shape as CircleCollider2D).radius = 1f;
			}
		}
		phone = LoadLevelAsset("RDHoodiePhone/RDHoodiePhone").GetComponent<RDHoodiePhone>();
		phoneRoomX = 6000f;
		phone.transform.LocalMoveX(phoneRoomX + RDClass.Vfx.RDWidth * 0.5f);
		phone.transform.LocalMoveY((0f - RDClass.Vfx.RDHeight) * 0.75f);
		decorationData = RDLevelData.current.decorations;
		yield return null;
	}

	public override void preactions()
	{
		int num = base.conductor.barNumber;
		if (num == 1)
		{
			base.room3.renderTexture.Release();
			UnityEngine.Object.Destroy(base.room3.renderTexture);
			RenderTexture renderTexture = RDClass.Vfx.CreateRenderTexture(1);
			base.room3.renderTexture = renderTexture;
			base.room3.camera.targetTexture = renderTexture;
			base.room3.quadMaterial.mainTexture = renderTexture;
			int scale = ((!base.game.currentLevel.fullResolutionRenderTextures) ? GC.RoomRenderTextureScale : 0);
			base.room2.renderTexture.Release();
			UnityEngine.Object.Destroy(base.room2.renderTexture);
			RenderTexture renderTexture2 = RDClass.Vfx.CreateRenderTexture(scale, "Debug", 24);
			base.room2.renderTexture = renderTexture2;
			base.room2.camera.targetTexture = renderTexture2;
			base.room2.quadMaterial.mainTexture = renderTexture2;
		}
		stadium.PrepareBaseballs(num);
	}

	public override void OnHit(HitType hitType, Beat beat = null)
	{
		stadium.HitBaseball(hitType, beat);
	}

	public override void actions()
	{
	}

	public void ToggleBubbles(bool on)
	{
		injuryBubbles.ToggleSystems(on);
	}

	public void FlipBubbles(bool flipped)
	{
		injuryBubbles.transform.localRotation = Quaternion.Euler(0f, 0f, flipped ? 180f : 0f);
	}

	public void MoveBubblesToRoom(int room)
	{
		injuryBubbles.transform.LocalMoveX(3000f * (float)room + RDClass.Vfx.RDWidth * 0.5f);
	}

	public void MoveBubbles(float percent, float durBeats, string easeType)
	{
		if (Enum.TryParse<Ease>(easeType, out var result))
		{
			bubbleTween?.Kill();
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
			float destY = RDClass.Vfx.RDHeight * ((percent - 50f) / 100f);
			bubbleTween = injuryBubbles.transform.DOLocalMoveY(destY, duration).SetEase(result).OnKill(() =>
			{
				injuryBubbles.transform.LocalMoveY(destY);
			});
		}
	}

	public void RunBubbleMask()
	{
		bubbleMask.playing = true;
	}

	public void PauseBubbleMask()
	{
		bubbleMask.playing = false;
	}

	public void DisableBubbleMask()
	{
		bubbleMask.gameObject.SetActive(value: false);
	}

	public void EnableBubbleMask()
	{
		bubbleMask.gameObject.SetActive(value: true);
	}

	public void MoveBubbleShape(int shapeID, float x, float y, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		Transform transform = bubbleMask.shapes[shapeID].transform;
		Vector2 vector = new Vector2(x / 100f * 352f, y / 100f * 198f);
		transform.DOLocalMove(vector, duration).SetEase(ease);
	}

	public void ScaleBubbleShape(int shapeID, float x, float y, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		bubbleMask.shapes[shapeID].transform.DOScale(new Vector2(x, y), duration).SetEase(ease);
	}

	public void ToggleScoreboardLights(bool on)
	{
		stadium.usingLights = on;
		if (on)
		{
			ClearAllScoreboardLights();
		}
	}

	public void SetScoreboardLights(bool home, string text, int startIndex)
	{
		stadium.SetScoreboardLightsWithString(home, text, startIndex);
	}

	public void ClearScoreboardLights(bool home)
	{
		stadium.ClearScoreboardLights(home);
	}

	public void ClearAllScoreboardLights()
	{
		stadium.ClearAllLights();
	}

	public void TogglePhone(bool on)
	{
		phone.gameObject.SetActive(on);
	}

	public void ToggleStream(bool on)
	{
		phone.ToggleStream(on);
	}

	public void ToggleStreamUI(bool on)
	{
		phone.ToggleStreamUI(on, 0f);
	}

	public void MovePhone(float xPerc, float yPerc, float durBeats, string easeStr)
	{
		float num = xPerc / 100f * RDClass.Vfx.RDWidth;
		float y = yPerc / 100f * RDClass.Vfx.RDHeight;
		Vector2 vector = new Vector2(num + phoneRoomX, y);
		phoneTween?.Kill();
		if (durBeats != 0f)
		{
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
			Ease ease = Enum.Parse<Ease>(easeStr);
			phoneTween = phone.transform.DOLocalMove(vector, duration).SetEase(ease);
		}
		else
		{
			phone.transform.LocalMoveXY(vector.x, vector.y);
		}
	}

	public void MoveStream(float yPerc, float durBeats, string easeStr)
	{
		float y = (yPerc - 50f) / 100f * 280f;
		Vector2 vector = new Vector2(0f, y);
		streamTween?.Kill();
		if (durBeats != 0f)
		{
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
			Ease ease = Enum.Parse<Ease>(easeStr);
			streamTween = phone.streamPivot.DOAnchorPos(vector, duration).SetEase(ease);
		}
		else
		{
			phone.streamPivot.anchoredPosition = vector;
		}
	}

	public void ShowOrHideStreamUI(bool show, float durBeats)
	{
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		phone.ToggleStreamUI(show, dur);
	}

	public void MoveHand(float yPerc, float durBeats, string easeStr)
	{
		float y = -150f * (1f - yPerc / 100f);
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		phone.MoveHand(new Vector2(0f, y), dur, ease);
	}

	public void ToggleHandPress(bool pressing, float durBeats, string easeStr)
	{
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		phone.ToggleHandPress(pressing, dur, ease);
	}

	public void SetTransferTargetForRow(int sourceRow, int destRow)
	{
		base.game.rows[sourceRow].transferTarget = destRow;
	}

	public void ToggleBaseball(bool on)
	{
		RDRoom targetRoom = base.room2;
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
		RDRoom rDRoom = base.room2;
		rDRoom.rotatingVoxelMode = on;
		if (on)
		{
			rDRoom.voxelObj.baseball.SetActive(value: false);
			rDRoom.voxelObj.SetFallingBaseballs(enableBaseballs: true);
		}
	}

	public void SetFallingBaseballsSpeed(float spdPercent)
	{
		ParticleSystem.MainModule main = base.room2.voxelObj.fallingBaseballs.GetComponent<ParticleSystem>().main;
		if (fallingBaseballsBaseSpd == -1f)
		{
			fallingBaseballsBaseSpd = main.simulationSpeed;
		}
		main.simulationSpeed = fallingBaseballsBaseSpd * (spdPercent / 100f);
	}

	public void FadePhysioFG(float opPercent, float durBeats)
	{
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		physioWard.physioFg.DOFade(opPercent / 100f, duration);
	}

	public void FadePhysioLights(float opPercent, float durBeats)
	{
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		physioWard.TweenPhysioLightingAlpha(opPercent / 100f, dur);
	}

	public void StartWardScroll(float scrollSpeed)
	{
		wardScrollSpeed = scrollSpeed;
		wardScrolling = true;
		scrExecuteOnUpdate.Add(() =>
		{
			WardUpdate();
		});
		ward.transform.LocalMoveX(7530f);
		wardStartX = ward.transform.localPosition.x;
		ward.seamlessExtra.SetActive(value: true);
		ward.rightEnd.SetActive(value: false);
	}

	public void StopWardScroll()
	{
		wardScrolling = false;
	}

	public void WardUpdate()
	{
		if (wardScrolling)
		{
			wardDistance += Time.deltaTime * wardScrollSpeed;
			float num = wardDistance % 510f;
			ward.transform.LocalMoveX(wardStartX - num);
		}
	}

	public void ToggleJanitorTVSync(bool on)
	{
		if (janitorTV == null)
		{
			string text = "JanitorTV";
			foreach (LevelEvent_MakeDecorationBase decorationDatum in decorationData)
			{
				if (decorationDatum is LevelEvent_Sprite levelEvent_Sprite && levelEvent_Sprite.filename == text)
				{
					janitorTV = sprites[decorationDatum.decorationId];
				}
			}
		}
		janitorTV.customAnimation.onClipFrameUpdate = (on ? new Action<CustomAnimation, CustomAnimationClip, int>(JanitorTVUpdate) : null);
	}

	private void JanitorTVUpdate(CustomAnimation anim, CustomAnimationClip clip, int frame)
	{
		float x = (float)((frame % 2 != 0) ? 179 : 177) - RDClass.Vfx.RDWidth / 2f;
		base.room0.renderQuadPivot.LocalMoveX(x);
	}
}
