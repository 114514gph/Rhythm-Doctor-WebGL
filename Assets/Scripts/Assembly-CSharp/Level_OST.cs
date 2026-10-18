using System;
using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_OST : LevelBase
{
	private RDHoodieBoyWard _hoodieBoyWard;

	private RDCoffeeShop _coffeeShopBg;

	private RDCoffeeShop _coffeeShopFg;

	private int currentCup;

	private tk2dSprite[] currentStack;

	private Vector2[] currentStackPos;

	private bool startedStack;

	private Vector2[] cupPos_4;

	private Vector2[] cupPos_8;

	private Vector2[] cupPos_12;

	private scrCareLessWindow careLessWindow;

	private Tween windowScrollTween;

	private Tween windowMoveTween;

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

	private bool cascadingLights;

	private float cascadingOffset;

	private bool currentlyNight = true;

	private RDHoodieBoyWard hoodieBoyWard
	{
		get
		{
			if (_hoodieBoyWard == null)
			{
				_hoodieBoyWard = base.room0.coleWard.GetComponent<RDHoodieBoyWard>();
			}
			return _hoodieBoyWard;
		}
	}

	private RDCoffeeShop coffeeShopBg
	{
		get
		{
			if (_coffeeShopBg == null)
			{
				_coffeeShopBg = base.room2.coffeeShop.GetComponent<RDCoffeeShop>();
			}
			return _coffeeShopBg;
		}
	}

	private RDCoffeeShop coffeeShopFg
	{
		get
		{
			if (_coffeeShopFg == null)
			{
				_coffeeShopFg = base.room3.coffeeShop.GetComponent<RDCoffeeShop>();
			}
			return _coffeeShopFg;
		}
	}

	public Level_OST(RDLevelData data)
		: base(data)
	{
	}

	private static Vector2[] GetCupPositions(tk2dSprite[] cups)
	{
		Vector2[] array = new Vector2[cups.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = cups[i].transform.localPosition;
		}
		return array;
	}

	public override IEnumerator LoadBigAssets()
	{
		careLessWindow = LoadLevelAsset("RDHoodieBoyWard/CareLess/CareLessWindow").GetComponent<scrCareLessWindow>();
		careLessWindow.transform.LocalMoveX(RDClass.Vfx.RDWidth * 0.5f);
		careLessWindow.transform.LocalMoveY(RDClass.Vfx.RDWidth * 1.5f);
		yield return null;
	}

	public override void actions()
	{
		_ = base.conductor.barNumber;
		if (cascadingLights)
		{
			SetLights(cascadingOffset);
		}
	}

	public void ToggleCascadingLights(bool enableCascade, float offset)
	{
		cascadingLights = enableCascade;
		cascadingOffset = offset;
	}

	public void ToggleHoodieBoyWardObjects(bool enableObj)
	{
		hoodieBoyWard.nightContainer.SetActive(currentlyNight & enableObj);
		hoodieBoyWard.sunriseContainer.SetActive(!currentlyNight & enableObj);
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

	private void TurnOffLights(bool gradual = false)
	{
		hoodieBoyWard.ToggleAllLights(on: false);
	}

	public void SetUpCoffeeShop()
	{
		coffeeShopBg.OnlyBackground();
		coffeeShopFg.OnlyForeground();
		coffeeShopFg.SetLights(0);
		cupPos_4 = GetCupPositions(coffeeShopBg.cups_4);
	}

	public void StartStack(int newStack = -1)
	{
		RDCoffeeShop rDCoffeeShop = coffeeShopFg;
		if (currentStack != null)
		{
			tk2dSprite[] array = currentStack;
			foreach (tk2dSprite obj in array)
			{
				obj.DOComplete(withCallbacks: true);
				obj.gameObject.SetActive(value: false);
			}
		}
		switch (newStack)
		{
		case 4:
			currentStack = rDCoffeeShop.cups_4;
			currentStackPos = cupPos_4;
			break;
		case 8:
			currentStack = rDCoffeeShop.cups_8;
			currentStackPos = cupPos_8;
			break;
		case 12:
			currentStack = rDCoffeeShop.cups_12;
			currentStackPos = cupPos_12;
			break;
		}
		rDCoffeeShop.SlideCoffeeMan();
		startedStack = true;
	}

	public void EndStack()
	{
		DOVirtual.DelayedCall(0.15f, () =>
		{
			coffeeShopFg.SlideCoffeeMan(inside: false);
		});
		startedStack = false;
		currentCup = 0;
	}

	public void ThrowCoffee(bool miss)
	{
		if (!startedStack)
		{
			return;
		}
		if (currentStack == null)
		{
			currentStack = coffeeShopFg.cups_4;
		}
		if (currentCup >= currentStack.Length)
		{
			return;
		}
		tk2dSprite cup = currentStack[currentCup];
		Vector2 origPosition = currentStackPos[currentCup];
		Vector2 baristaPosition = new Vector2(15f, 16f);
		float height = (miss ? 30f : 60f);
		DOVirtual.DelayedCall(1f / 6f, () =>
		{
			cup.gameObject.SetActive(value: true);
			cup.transform.localPosition = baristaPosition;
			cup.transform.DOLocalJump(origPosition, height, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
			{
				cup.transform.DOShakePosition(0.2f, 1f, 10, 90f, snapping: true);
			});
			cup.Rotate2D(0f);
			cup.transform.DORotate(new Vector3(0f, 0f, 720f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic);
		});
		currentCup++;
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

	public void ClearFiles()
	{
		careLessWindow.ClearFiles();
	}

	public void AddCustomFilesFromText(string textFileName)
	{
		careLessWindow.AddCustomFilesFromText(textFileName);
	}
}
