using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Smokin : LevelBase
{
	private int _tiredness;

	private bool doingLongCue;

	private bool playedRea;

	private bool playedHitActionThisFrame;

	private RDCoffeeShop coffeeShopBg;

	private RDCoffeeShop coffeeShopFg;

	private int currentCup;

	private tk2dSprite[] currentStack;

	private Vector2[] currentStackPos;

	private bool startedStack;

	private Vector2[] cupPos_4;

	private Vector2[] cupPos_8;

	private Vector2[] cupPos_12;

	public int tiredness
	{
		get
		{
			return _tiredness;
		}
		set
		{
			_tiredness = value;
			SetExpressions(startedStack);
		}
	}

	public RowEntity oneshotRow => base.game.rows[2].ent;

	public RowEntity otherOneshotRow => base.game.rows[3].ent;

	public Level_Smokin(RDLevelData data)
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
		coffeeShopBg = LoadLevelAsset<RDCoffeeShop>();
		coffeeShopBg.transform.MoveX(coffeeShopBg.transform.position.x + 3000f);
		coffeeShopBg.OnlyBackground();
		coffeeShopFg = LoadLevelAsset<RDCoffeeShop>();
		coffeeShopFg.transform.MoveX(coffeeShopBg.transform.position.x + 3000f);
		coffeeShopFg.OnlyForeground();
		coffeeShopFg.SetLights(0);
		cupPos_4 = GetCupPositions(coffeeShopBg.cups_4);
		cupPos_8 = GetCupPositions(coffeeShopBg.cups_8);
		cupPos_12 = GetCupPositions(coffeeShopBg.cups_12);
		yield return null;
	}

	public override void OnMistake(RowEntity ent = null)
	{
	}

	public override void OnHit(HitType hitType, Beat beat)
	{
		int rowID = beat.rowID;
		if (!playedHitActionThisFrame && rowID <= 1)
		{
			playedHitActionThisFrame = true;
			bool flag = hitType != HitType.Hit;
			ThrowCoffee(flag);
			if (flag)
			{
				FlashBubble();
			}
		}
	}

	public void SetTiredness(int t)
	{
		tiredness = t;
	}

	public void ChangeCountertopLayer()
	{
		coffeeShopFg.countertop.SetActive(value: false);
		coffeeShopFg.countertopSpecial.SetActive(value: true);
		coffeeShopBg.countertopBackground.SetActive(value: true);
	}

	private void FlashBubble()
	{
		RDRoom obj = base.game.rooms[0];
		obj.quadMaterial.DOKill();
		obj.quadMaterial.SetFloat("_Opacity", 1f);
		obj.quadMaterial.DOFloat(0f, "_Opacity", 2f).SetEase(Ease.InSine);
	}

	public void StartStack(int newStack = -1)
	{
		RDCoffeeShop rDCoffeeShop = coffeeShopFg;
		FastBeatTrigger(isFast: true);
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
		SetExpressions(stacking: true);
	}

	private void ShowOrHideCustomers(int from, int to, bool hide, float dur = -1f)
	{
		if (dur == -1f)
		{
			dur = 2f;
		}
		coffeeShopBg.ShowOrHideCustomers(from, to, hide, dur);
		coffeeShopFg.ShowOrHideCustomers(from, to, hide, dur);
	}

	public void HideCustomers(int from, int count)
	{
		int to = from + count - 1;
		ShowOrHideCustomers(from, to, hide: true);
	}

	public void HideCole(int from)
	{
		ShowOrHideCustomers(from, from, hide: true, 4.5f);
	}

	public void ShowCustomers(int from, int count)
	{
		int to = from + count - 1;
		ShowOrHideCustomers(from, to, hide: false);
	}

	public void EndStack()
	{
		DOVirtual.DelayedCall(0.15f, () =>
		{
			coffeeShopFg.SlideCoffeeMan(inside: false);
		});
		startedStack = false;
		currentCup = 0;
		FastBeatTrigger(isFast: false);
		SetExpressions(stacking: false);
	}

	public void TransitionToNight()
	{
		coffeeShopFg.TransitionToNight();
		coffeeShopBg.TransitionToNight();
	}

	public void DisableNightLights()
	{
		coffeeShopFg.lights_night.gameObject.SetActive(value: false);
	}

	private void SetExpressions(bool stacking)
	{
		string text = ((tiredness == 0) ? "" : ((tiredness == 1) ? "Tired" : "VeryTired"));
		string text2 = "neutral" + text;
		string happyAnimName = (stacking ? ("throw" + text) : text2);
		oneshotRow.character.noHappy = !stacking;
		oneshotRow.character.noMiss = tiredness != 0;
		oneshotRow.character.neutralAnimName = text2;
		oneshotRow.character.happyAnimName = happyAnimName;
	}

	private void ThrowCoffee(bool miss)
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
			if (miss)
			{
				startedStack = false;
				DOVirtual.DelayedCall(0.5f, () =>
				{
					CollapseStack();
				});
				SetExpressions(stacking: false);
			}
		});
		currentCup++;
	}

	private void CollapseStack()
	{
		for (int i = 0; i < currentStack.Length; i++)
		{
			tk2dSprite cup = currentStack[i];
			cup.DOComplete(withCallbacks: true);
			Vector2 origPosition = currentStackPos[i];
			float duration = Random.Range(0.1f, 0.5f);
			int num = Random.Range(-60, -30);
			int num2 = Random.Range(90, 420);
			cup.transform.DOLocalJump(origPosition.WithY(num), 10f, 1, duration).SetEase(Ease.Linear);
			cup.transform.DORotate(new Vector3(0f, 0f, num2), duration).SetEase(Ease.Linear);
			cup.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
			{
				cup.transform.LocalMoveY(origPosition.y);
				cup.color = cup.color.WithAlpha(1f);
				cup.Rotate2D(0f);
				cup.gameObject.SetActive(value: false);
			});
			coffeeShopFg.coffeeMan.Play("Coffeeman_miss");
		}
	}

	public override void Update()
	{
		playedHitActionThisFrame = false;
	}

	public override void BeepGet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		coffeeShopFg.SetLights(doingLongCue ? 3 : 2);
	}

	public override void BeepSet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		coffeeShopFg.SetLights(4);
	}

	public override void BeepGo(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		coffeeShopFg.SetLights(0);
		doingLongCue = false;
		playedRea = false;
	}

	public void OpenDoor()
	{
		coffeeShopFg.OpenDoor();
	}

	private IEnumerator EnableHitAction()
	{
		yield return new WaitForEndOfFrame();
		playedHitActionThisFrame = false;
	}

	public override void BeepReady(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		doingLongCue = true;
		coffeeShopFg.SetLights((!playedRea) ? 1 : 2);
		playedRea = true;
	}

	public void SetWaveSize(float width, float height)
	{
		oneshotRow.oneshotRowController.waveHeightMultiplier = height;
		oneshotRow.oneshotRowController.waveWidthMultiplier = width;
		otherOneshotRow.oneshotRowController.waveHeightMultiplier = height;
		otherOneshotRow.oneshotRowController.waveWidthMultiplier = width;
	}

	public void FastBeatTrigger(bool isFast)
	{
		if (isFast)
		{
			SetWaveSize(0.75f, 0.75f);
			SetOneshotType(2, "Spring");
			SetOneshotType(3, "Spring");
		}
		else
		{
			SetWaveSize(1f, 1f);
			SetOneshotType(2, "BoomAndRush");
			SetOneshotType(3, "BoomAndRush");
		}
	}
}
