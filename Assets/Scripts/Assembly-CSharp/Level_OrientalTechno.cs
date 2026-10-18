using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using RDLevelEditor;
using UnityEngine;

public class Level_OrientalTechno : LevelBase
{
	private Background fallingStars;

	private List<RDParticleSystem> mParticleSystems;

	private RDSpaceBackground spaceBackground;

	private float row1StartY;

	public RDSamuraiWard samuraiWard;

	private int currentCycleColorID;

	public Level_OrientalTechno(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		songsUsed = new string[1] { "sndOrientalTechno" };
		heartExplodeType = HeartExplodeType.Instant;
		base.isOneRowButNotBoss = true;
		hideHandsAfterStart = true;
	}

	public override IEnumerator LoadBigAssets()
	{
		samuraiWard = LoadLevelAsset<RDSamuraiWard>();
		samuraiWard.transform.localPosition = new Vector3(176f, 99f, 0f);
		scrExecuteOnHit.Add(() =>
		{
			tk2dSpriteAnimator[] lanternTails = samuraiWard.lanternTails;
			for (int i = 0; i < lanternTails.Length; i++)
			{
				lanternTails[i].PlayFromFrame(0);
			}
		}, HitType.Hit, persistent: true);
		spaceBackground = LoadLevelAsset<RDSpaceBackground>();
		yield return null;
	}

	private IEnumerator OpenDoors()
	{
		base.game.StartCoroutine(samuraiWard.OpenDoors());
		float duration = 0.4f;
		for (float i = 0f; i < duration; i += Time.deltaTime)
		{
			float t = i / duration;
			Color additiveColor = Color.Lerp(base.gc.techno_additive0, base.gc.techno_additive1, t);
			Color overlayColor = Color.Lerp(base.gc.techno_overlay0, base.gc.techno_overlay1, t);
			Color borderColor = Color.Lerp(base.gc.techno_glow0, base.gc.techno_glow1, t);
			Row[] array = base.game.rows;
			for (int j = 0; j < array.Length; j++)
			{
				RowEntity ent = array[j].ent;
				if (!(ent == null))
				{
					ent.shaderDataSource = ent;
					ent.shaderData.additiveColor = additiveColor;
					ent.shaderData.overlayColor = overlayColor;
					ent.shaderData.borderColor = borderColor;
				}
			}
			yield return null;
		}
	}

	public override void preactions()
	{
		RDClass.Vfx.debugText = "Bar: " + base.conductor.barNumber;
		switch (base.conductor.barNumber)
		{
		case 1:
		{
			base.conductor.SetBPM(200.095f);
			PlayMusic(songsUsed[0]);
			base.game.shadowRowStyle = ShadowRowStyle.HostToShadowIf2P;
			base.game.MakeRow(RowType.Classic, 1, 1f, Character.Samurai, RDPlayer.P1, RDPlayer.P1, 2);
			if (GC.twoPlayerMode)
			{
				ChangeCharacter("SamuraiBlue", 1);
				if (RDBase.IsHalloweenWeek())
				{
					ChangeCharacter("SamuraiPirate", 2);
				}
			}
			else if (RDBase.IsHalloweenWeek())
			{
				ChangeCharacter("SamuraiPirate", 1);
			}
			float rowSoundVolMultiplier = (GC.twoPlayerMode ? 0.5f : 1f);
			base.game.rows[1].rowSoundVolMultiplier = rowSoundVolMultiplier;
			base.game.rows[1].rowCountingVoiceEnabled = true;
			base.game.rows[1].SetCountingSounds(CountingVoiceSource.JyiCountFast);
			if (GC.twoPlayerMode)
			{
				base.game.rows[2].rowSoundVolMultiplier = rowSoundVolMultiplier;
				base.game.rows[2].rowCountingVoiceEnabled = true;
				base.game.rows[2].SetCountingSounds(CountingVoiceSource.IanCountFast);
			}
			Row[] array = base.game.rows;
			for (int num = 0; num < array.Length; num++)
			{
				RowEntity ent = array[num].ent;
				if (!(ent == null))
				{
					ent.SetAdditiveColor(base.gc.techno_additive0);
					ent.SetOverlayColor(base.gc.techno_overlay0);
				}
			}
			row1StartY = base.rowEnt1.y;
			PlaceSamuraiInTechnoWard();
			RDClass.Vfx.BgColor(Color.black);
			SetHandsAdditiveColor(0, base.gc.techno_additive0);
			SetHandsOverlayColor(0, base.gc.techno_overlay0);
			break;
		}
		case 4:
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientConnect", 0f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.game.rows[1].rowSoundPanAdjustedFor2P);
			}
			if (Narration.IsEnabled && GC.twoPlayerMode)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientConnect", 4f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.game.rows[2].rowSoundPanAdjustedFor2P);
			}
			break;
		case 5:
			OnBeatClassic(1, 1, 0f, 1f);
			break;
		case 7:
		case 11:
			AddBeat(1, 7f, 0.5f);
			break;
		case 8:
		case 12:
			OffBeat(1);
			AddBeat(1, 3f, 0.5f);
			break;
		case 9:
			OnBeat(1);
			break;
		case 13:
			LEDSign.status = "";
			LEDSign.showMarginError = false;
			base.game.rows[1].rowCountingVoiceEnabled = false;
			if (GC.twoPlayerMode)
			{
				base.game.rows[2].rowCountingVoiceEnabled = false;
			}
			OnBeatClassic(2, 1, 0f, 0.5f);
			OnBeatClassic(3, 1, 4f, 0.5f);
			break;
		case 25:
		case 26:
		case 27:
			if (base.conductor.barNumber == 25)
			{
				if (GC.twoPlayerMode)
				{
					base.game.rows[2].rowSound = "sndChuck";
				}
				OffBeat(2);
				OffBeat(3);
			}
			base.game.AddBeat(1, 0f, 0.5f);
			base.game.AddBeat((!GC.twoPlayerMode) ? 1 : 2, 4.5f, 0.5f);
			break;
		case 28:
			AddBeat(1, 3.5f, 0.5f);
			AddBeat(1, 7.5f, 0.5f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				if (!base.game.reducedFlash)
				{
					RDClass.Vfx.Flash();
				}
				RDClass.Vfx.scrollXMultiplier *= 1.2f;
				RDClass.Vfx.scrollYMultiplier *= 1.4f;
			}, 6.5f);
			break;
		case 29:
			scrExecuteOnCertainBeat.Add(() =>
			{
				if (!base.game.reducedFlash)
				{
					RDClass.Vfx.Flash();
				}
				RDClass.Vfx.scrollXMultiplier *= 1.2f;
				RDClass.Vfx.scrollYMultiplier *= 1.4f;
			}, 2.5f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				if (!base.game.reducedFlash)
				{
					RDClass.Vfx.Flash();
				}
				RDClass.Vfx.scrollXMultiplier *= 1.2f;
				RDClass.Vfx.scrollYMultiplier *= 1.4f;
			}, 6.5f);
			AddBeat(1, 3.5f, 0.5f);
			break;
		case 31:
			scrExecuteOnEveryHB.Add(base.BeatShake);
			AddBeat(1, 1f, 0.5f);
			base.game.DeleteRow(1, 6f);
			break;
		case 32:
			OffBeat(1);
			break;
		case 2:
		case 3:
		case 6:
		case 10:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 30:
			break;
		}
	}

	private IEnumerator PixelateAnimation(float time)
	{
		RDImageEffectPixelate pixelate = RDClass.Vfx.hudCamera.GetComponent<RDImageEffectPixelate>();
		pixelate.referenceCamera = RDClass.Vfx.hudCamera;
		float delta = Time.deltaTime;
		pixelate.enabled = true;
		for (float t = 0f; t < time; t += delta)
		{
			pixelate.time = Mathf.Lerp(0f, 8f, t / time);
			yield return null;
		}
		float time2 = time / 2.5f;
		for (float t = 0f; t < time2; t += delta)
		{
			pixelate.time = Mathf.Lerp(8f, 0f, t / time2);
			yield return null;
		}
		pixelate.enabled = false;
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
		{
			RDClass.Vfx.scrollXMultiplier = 1f;
			Camera camera = base.room0.camera;
			spaceBackground.mainCam.depth = camera.depth - 2f;
			spaceBackground.mainCam.targetTexture = camera.targetTexture;
			spaceBackground.starsCam.depth = camera.depth - 1f;
			spaceBackground.starsCam.targetTexture = base.room1.camera.targetTexture;
			base.game.handControllerOnTop.Show(show: false);
			samuraiWard.ShowLanterns();
			NarrateDescription("OrientalTechno.desc1");
			break;
		}
		case 3:
			base.game.statusText.SetStatusText(RDString.Get("status.hitOnSeventhBeat"), null, 4f, narrate: true);
			base.game.handController0.SlideIn();
			break;
		case 4:
			Narration.NarrateRowInfo(base.rows[1], NarrateInfoType.Connect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			base.game.statusText.SetStatusText(RDString.Get("OrientalTechno.statusStart"), null, 4f, narrate: true);
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.NarrateRowInfo(base.rows[2], NarrateInfoType.Connect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			}, 4f);
			break;
		case 8:
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.game.StartCoroutine(OpenDoors());
			}, base.gc.techno_openDoorsBeat);
			break;
		case 9:
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			NarrateDescription("OrientalTechno.desc2");
			break;
		case 12:
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.rowEnt1.transform.DOLocalMoveY(250f, base.gc.techno_rowFlyDuration, snapping: true).SetEase(base.gc.techno_rowFlyEase);
				if (GC.twoPlayerMode)
				{
					base.rowEnt2.transform.DOLocalMoveY(231f, base.gc.techno_rowFlyDuration, snapping: true).SetEase(base.gc.techno_rowFlyEase);
				}
				NarrateDescription("OrientalTechno.desc3");
			}, base.gc.techno_rowFlyBeat);
			scrExecuteOnCertainBeat.Add(() =>
			{
				float time = base.conductor.crotchet * 2f;
				base.game.StartCoroutine(PixelateAnimation(time));
			}, 6f);
			List<RDParticleSystem> list = new List<RDParticleSystem>();
			for (int num = 0; num < 6; num++)
			{
				RDParticleSystem rDParticleSystem = RDClass.Vfx.SpawnParticles("OrientalTechno/" + num % 3, 0);
				rDParticleSystem.durationBetweenSpawns = ((num == 0 || num == 3) ? 5f : 5f);
				rDParticleSystem.angularSpeed = new Range(10f, 18f);
				rDParticleSystem.scale = new Range(0.5f, 1.5f);
				rDParticleSystem.parallaxPower = 1.2f;
				rDParticleSystem.startFromRight = num > 2;
				rDParticleSystem.StartStream();
				list.Add(rDParticleSystem);
				rDParticleSystem.negativeSortingOrder = true;
				rDParticleSystem.sortingLayer = RDSortingLayer.Background.ToString();
			}
			list[0].baseSpeed = (list[3].baseSpeed = 100f);
			list[1].baseSpeed = (list[4].baseSpeed = 90f);
			list[2].baseSpeed = (list[5].baseSpeed = 140f);
			mParticleSystems = list;
			{
				foreach (RDParticleSystem mParticleSystem in mParticleSystems)
				{
					mParticleSystem.gameObject.SetActive(value: true);
				}
				break;
			}
		}
		case 13:
			ShowSpaceBackground(visible: true);
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			base.game.SetVisualSpeed(0.5f);
			NarrateDescription("OrientalTechno.desc4");
			break;
		case 21:
			NarrateDescription("OrientalTechno.desc5");
			break;
		case 25:
			base.game.SetVisualSpeed(0.25f);
			break;
		case 27:
			base.game.SetVisualSpeed(0.5f);
			break;
		case 28:
			base.game.SetVisualSpeed(1f);
			break;
		case 30:
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.FadeOut(2f);
			}, 2f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				for (int i = 0; i < 6; i++)
				{
					mParticleSystems[i].StopStream();
					mParticleSystems[i].KillAllParticles();
					mParticleSystems[i].baseSpeed = 20f;
				}
				base.game.SetVisualSpeed(0.5f);
				NarrateDescription("OrientalTechno.desc6");
			}, 7f);
			break;
		case 31:
			ShowSpaceBackground(visible: false);
			RDClass.Vfx.FadeIn(0.2f);
			break;
		case 32:
			RDClass.Vfx.BgColor(Color.black);
			base.rankscreen.AdvanceGameover();
			break;
		case 33:
			base.rankscreen.AdvanceGameover();
			break;
		case 34:
			base.rankscreen.AdvanceGameover();
			break;
		}
	}

	private void CycleFallingStarsColor()
	{
		uint[] array = new uint[11]
		{
			4291189578u, 4291198794u, 4283090599u, 4283081414u, 4283070918u, 4283058886u, 4285745862u, 4288039622u, 4290136774u, 4291185316u,
			4291185279u
		};
		int num = currentCycleColorID;
		while (currentCycleColorID == num)
		{
			currentCycleColorID = Mathf.FloorToInt(Random.value * (float)array.Length);
		}
		fallingStars.SetColor(array[currentCycleColorID].ARGBToColor());
	}

	private void SetBackgroundScrollingSpeed(float speed)
	{
		foreach (RDParticleSystem mParticleSystem in mParticleSystems)
		{
			mParticleSystem.speedMultiplier = Mathf.Abs(speed);
		}
	}

	private void ShowSpaceBackground(bool visible)
	{
		samuraiWard.gameObject.SetActive(!visible);
		base.room0.wavyRows = visible;
		mParticleSystems.ForEach((RDParticleSystem ps) =>
		{
			ps.gameObject.SetActive(visible);
		});
		base.game.handController0.Show(visible);
		Row[] array;
		if (visible)
		{
			RDClass.Vfx.SetBloomParams(0.19f, 3.52f, 50f, 1058401023u, VideoBloom.BlendingMode.Screen);
			float rowsCenter = -200f;
			float rowOffset = 21f;
			if (GC.twoPlayerMode)
			{
				base.rowEnt1.customPosition = false;
				base.rowEnt2.customPosition = false;
				base.room0.RepositionRowsAndStrips(instant: true);
				base.rowEnt1.transform.LocalMoveY(rowsCenter + rowOffset);
				base.rowEnt2.transform.LocalMoveY(rowsCenter - rowOffset);
			}
			else
			{
				base.rowEnt1.customPosition = false;
				base.rowEnt1.transform.LocalMoveY(rowsCenter);
			}
			scrExecuteOnCertainBeat.Add(() =>
			{
				TweenerCore<float, float, FloatOptions> t = DOTween.To(() => rowsCenter, (float r) =>
				{
					rowsCenter = r;
				}, RDClass.Vfx.RDHeight / 2f, base.gc.techno_rowFly2Duration).SetEase(base.gc.techno_rowFly2Ease);
				if (GC.twoPlayerMode)
				{
					t.OnUpdate(() =>
					{
						base.rowEnt1.transform.LocalMoveY(rowsCenter + rowOffset);
						base.rowEnt2.transform.LocalMoveY(rowsCenter - rowOffset);
					});
				}
				else
				{
					t.OnUpdate(() =>
					{
						base.rowEnt1.transform.LocalMoveY(rowsCenter);
					});
				}
			}, base.gc.techno_rowFly2Beat);
			RDClass.Vfx.BgColor(Color.black);
			array = base.game.rows;
			for (int num = 0; num < array.Length; num++)
			{
				RowEntity ent = array[num].ent;
				if (!(ent == null))
				{
					ent.SetAdditiveColor(Color.clear);
					ent.SetOverlayColor(Color.clear);
					ent.SetBorderColor(Color.white.WithAlpha(0.8f));
				}
			}
			SetHandsAdditiveColor(0, Color.clear);
			SetHandsOverlayColor(0, Color.clear);
			SetHandsBorderColor(0, 1f, 1f, 1f, 0.8f, 0);
			base.room0.wavyRowsFrequency = 5.0010004f;
			base.tkCam.clearFlags = CameraClearFlags.Depth;
			fallingStars = Background.Create("OrientalTechno/4", -1f, -2f);
			scrExecuteOnEveryHB.Add(() =>
			{
				CycleFallingStarsColor();
			});
			return;
		}
		RDClass.Vfx.SetBloomParams(0.19f, 0f, 50f, 1058401023u, VideoBloom.BlendingMode.Screen);
		PlaceSamuraiInTechnoWard();
		RDClass.Vfx.BgColor(Color.black);
		array = base.game.rows;
		for (int num = 0; num < array.Length; num++)
		{
			RowEntity ent2 = array[num].ent;
			if (!(ent2 == null))
			{
				ent2.shaderDataSource = ent2;
				ent2.shaderData.additiveColor = base.gc.techno_additive1;
				ent2.shaderData.overlayColor = base.gc.techno_overlay1;
				ent2.shaderData.borderColor = base.gc.techno_glow1;
			}
		}
		base.tkCam.clearFlags = CameraClearFlags.Color;
		fallingStars.visible = false;
	}

	private void PlaceSamuraiInTechnoWard()
	{
		base.rowEnt1.Reposition((int)(row1StartY - 30f), instant: true);
		base.rowEnt1.customPosition = true;
		if (GC.twoPlayerMode)
		{
			base.rowEnt2.Reposition((int)(row1StartY - 49f), instant: true);
			base.rowEnt2.customPosition = true;
			base.room0.RepositionRowsAndStrips(instant: true);
		}
	}
}
