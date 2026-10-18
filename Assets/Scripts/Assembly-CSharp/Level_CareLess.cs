using System;
using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_CareLess : LevelBase
{
	private RDHoodieBoyWard hoodieBoyWard;

	private scrCareLessWindow careLessWindow;

	private scrCareLessAlert careLessAlert1;

	private scrCareLessAlert careLessAlert2;

	private scrCareLessParticles careLessParticles;

	private Tween windowScrollTween;

	private Tween windowMoveTween;

	private readonly string[] lightPatternsLofiIntro = new string[4] { "101010101011101010101010", "1010101010111010", "1010101010111010", "1010111010111010" };

	private readonly string[] lightPatterns = new string[76]
	{
		"1111111111111111", "1111111111111111", "1111111111111111", "1001001001001111", "1111111111111111", "1111111111111111", "1111111110101010", "1001001111111111", "1111111111111111", "1111111111111111",
		"1111111111111111", "1111111111111000", "1010101010101010", "1010101010101010", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111100000000000",
		"1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111",
		"1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111",
		"1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111",
		"1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111",
		"1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111",
		"1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111", "1111111111111111"
	};

	private bool currentlyNight = true;

	public Level_CareLess(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		hoodieBoyWard = LoadLevelAsset("RDHoodieBoyWard/RDHoodieBoyWard").GetComponent<RDHoodieBoyWard>();
		hoodieBoyWard.room = base.room0;
		hoodieBoyWard.transform.LocalMoveX(0f);
		base.room2.SetRowContainerY(-30f);
		careLessWindow = LoadLevelAsset("RDHoodieBoyWard/CareLess/CareLessWindow").GetComponent<scrCareLessWindow>();
		careLessWindow.transform.LocalMoveX(RDClass.Vfx.RDWidth * 0.5f);
		careLessWindow.transform.LocalMoveY(RDClass.Vfx.RDWidth * 1.5f);
		careLessAlert1 = LoadLevelAsset("RDHoodieBoyWard/CareLess/CareLessAlert").GetComponent<scrCareLessAlert>();
		careLessAlert1.transform.LocalMoveY(RDClass.Vfx.RDHeight * 0.5f);
		careLessAlert1.gameObject.SetActive(value: false);
		careLessAlert2 = LoadLevelAsset("RDHoodieBoyWard/CareLess/CareLessAlert").GetComponent<scrCareLessAlert>();
		careLessAlert2.transform.LocalMoveY(RDClass.Vfx.RDHeight * 0.5f);
		careLessAlert2.gameObject.SetActive(value: false);
		careLessParticles = LoadLevelAsset("RDHoodieBoyWard/CareLess/CareLessParticles").GetComponent<scrCareLessParticles>();
		careLessParticles.rainSystem.transform.LocalMoveXY(RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight + 8f);
		careLessParticles.spiralTrans.LocalMoveXY(6000f + RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight * 0.5f);
		careLessParticles.ToggleAllSystemsActive(toggleActive: false);
		yield return null;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 66)
		{
			base.game.handControllerOnTop.Show(show: false);
		}
	}

	public override void actions()
	{
		int num = base.conductor.barNumber;
		if (num == 1)
		{
			string text = lightPatternsLofiIntro[0];
			for (int i = 0; i < 16; i++)
			{
				int x = i;
				bool lightShouldBeOn = text[x] == '1';
				scrExecuteOnCertainBeat.Add(() =>
				{
					if (lightShouldBeOn)
					{
						TurnOnLight(x);
					}
					else
					{
						TurnOffLights();
					}
				}, (i % 2 == 0) ? ((float)i / 4f) : ((float)i / 4f + 0.25f - 0.15f));
			}
			for (int num2 = 0; num2 < 8; num2++)
			{
				int x2 = num2 + 16;
				bool lightShouldBeOn2 = text[x2] == '1';
				scrExecuteOnCertainBeat.Add(() =>
				{
					if (lightShouldBeOn2)
					{
						TurnOnLight(x2 - 16);
					}
					else
					{
						TurnOffLights();
					}
				}, ((num2 % 2 == 0) ? ((float)num2 / 4f) : ((float)num2 / 4f + 0.25f - 0.15f)) + 4f);
			}
		}
		if (num == 2)
		{
			base.game.handControllerOnTop.SetHandsInFront(inFront: false);
			scrExecuteOnCertainBeat.Add(() =>
			{
				TurnOnLights(flash: true);
			}, 0f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				TurnOffLights();
			}, 4f);
		}
		if (lightPatterns.Length > num && num >= 3)
		{
			if (num < 48)
			{
				SetLights(0f);
			}
			else
			{
				SetLights(3f);
			}
		}
	}

	private void SetLights(float beatOffset)
	{
		string text = lightPatterns[Mathf.FloorToInt((base.barNumber - 3) / 2)];
		for (int i = 0; i < 8; i++)
		{
			bool flag = base.barNumber % 2 == 0;
			int x = (flag ? (i + 8) : i);
			bool lightShouldBeOn = text[x] == '1';
			scrExecuteOnCertainBeat.Add(() =>
			{
				if (lightShouldBeOn)
				{
					TurnOnLight(x);
				}
				else
				{
					TurnOffLights();
				}
			}, (float)i / 2f + beatOffset);
		}
	}

	private void TurnOnLight(int index)
	{
		hoodieBoyWard.TurnOnLight(index);
	}

	private void TurnOnLights(bool flash = false)
	{
		hoodieBoyWard.ToggleAllLights(on: true, flash);
	}

	private void TurnOffLights(bool gradual = false)
	{
		hoodieBoyWard.ToggleAllLights(on: false);
	}

	public void ToggleLights(bool enableLights)
	{
		hoodieBoyWard.showLights = enableLights;
	}

	public void SetWardMode(bool nightMode)
	{
		hoodieBoyWard.SetMode(nightMode);
		currentlyNight = nightMode;
	}

	public void ToggleHoodieBoyWardObjects(bool enableObj)
	{
		hoodieBoyWard.nightContainer.SetActive(currentlyNight & enableObj);
		hoodieBoyWard.sunriseContainer.SetActive(!currentlyNight & enableObj);
	}

	public void ActivateGuitarMode()
	{
		scrChar character = base.rows[0].ent.character;
		character.noBeat = true;
		character.noHappy = true;
		character.neutralAnimName = "guitarNeutral";
		if (GC.twoPlayerMode)
		{
			scrChar character2 = base.rows[1].ent.character;
			character2.noBeat = true;
			character2.noHappy = true;
			character2.neutralAnimName = "guitarNeutral";
		}
	}

	public void ScrollWindow(float targetPos, float durationBeats, string easeType)
	{
		if (Enum.TryParse<Ease>(easeType, out var result))
		{
			if (windowScrollTween != null)
			{
				windowScrollTween.Kill(complete: true);
			}
			float duration = base.conductor.DurationBeatsToTime(50f, durationBeats);
			windowScrollTween = careLessWindow.TweenScrollPos(targetPos, duration, result);
		}
	}

	public void MoveWindow(float targetX, float targetY, float durationBeats, string easeType)
	{
		if (Enum.TryParse<Ease>(easeType, out var result))
		{
			if (windowMoveTween != null)
			{
				windowMoveTween.Kill(complete: true);
			}
			float x = RDClass.Vfx.RDWidth * targetX / 100f;
			float y = RDClass.Vfx.RDHeight * targetY / 100f;
			Vector3 targetPos = new Vector3(x, y, careLessWindow.transform.localPosition.z);
			float duration = base.conductor.DurationBeatsToTime(50f, durationBeats);
			windowMoveTween = careLessWindow.transform.DOLocalMove(targetPos, duration).SetEase(result).OnComplete(() =>
			{
				careLessWindow.transform.localPosition = targetPos;
			});
		}
	}

	public void RemoveFiles(int filesToRemove)
	{
		for (int i = 0; i < filesToRemove; i++)
		{
			careLessWindow.RemoveRandomFileFromLast(12);
		}
	}

	public void ToggleAlert(int box, bool toggle)
	{
		((box == 0) ? careLessAlert1.gameObject : careLessAlert2.gameObject).SetActive(toggle);
	}

	public void PlaceAlertInRoom(int box, int roomIndex)
	{
		((box == 0) ? careLessAlert1.transform : careLessAlert2.transform).LocalMoveX(3000f * (float)roomIndex + RDClass.Vfx.RDWidth * 0.5f);
	}

	public void SetAlertButtons(int box, int mode, int type)
	{
		((box == 0) ? careLessAlert1 : careLessAlert2).SetButtons(mode, type);
	}

	public void SetAlertWidth(int box, float width)
	{
		RectTransform canvasRect = ((box == 0) ? careLessAlert1 : careLessAlert2).canvasRect;
		Canvas.ForceUpdateCanvases();
		canvasRect.sizeDelta = canvasRect.sizeDelta.WithX(width);
	}

	public void SetAlertBox(int box, int type)
	{
		((box == 0) ? careLessAlert1 : careLessAlert2).SetBox(type);
	}

	public void SetSwarmPos(float upperY, float lowerY)
	{
		careLessParticles.upperSystem.transform.LocalMoveXY(-5f, RDClass.Vfx.RDHeight * (upperY / 100f));
		careLessParticles.lowerSystem.transform.LocalMoveXY(RDClass.Vfx.RDWidth + 5f, RDClass.Vfx.RDHeight * (lowerY / 100f));
	}

	public void ToggleSwarm(bool toggle)
	{
		careLessParticles.upperSystem.gameObject.SetActive(toggle);
		careLessParticles.lowerSystem.gameObject.SetActive(toggle);
		if (toggle)
		{
			careLessParticles.upperSystem.Play();
			careLessParticles.lowerSystem.Play();
		}
		else
		{
			careLessParticles.upperSystem.Stop();
			careLessParticles.lowerSystem.Stop();
		}
	}

	public void ReplaceSwarmSprite(int index)
	{
		careLessParticles.ReplaceSwarmSprite(index);
	}

	public void DropSwarm(float finalSpeed, float duration)
	{
		careLessParticles.DropSwarm(finalSpeed, duration);
	}

	[ListedMethod(false)]
	public void ToggleIconRain(bool toggle)
	{
		careLessParticles.rainSystem.gameObject.SetActive(toggle);
		if (toggle)
		{
			careLessParticles.rainSystem.Play();
		}
		else
		{
			careLessParticles.rainSystem.Stop();
		}
	}

	[ListedMethod(false)]
	public void DeactivateIconRain()
	{
		careLessParticles.rainSystem.Stop();
		ParticleSystem.MainModule main = careLessParticles.rainSystem.main;
		main.simulationSpeed = 2f;
	}

	[ListedMethod(false)]
	public void ToggleIconSpiral(bool toggle)
	{
		ParticleSystem[] spiralSystems = careLessParticles.spiralSystems;
		foreach (ParticleSystem particleSystem in spiralSystems)
		{
			particleSystem.gameObject.SetActive(toggle);
			if (toggle)
			{
				particleSystem.Simulate(20f);
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	public void MoveSamuraiVaseToBackground()
	{
		UnityEngine.Object.FindObjectOfType<RDSamuraiWard>().SetVaseForeground(foreground: false);
	}
}
