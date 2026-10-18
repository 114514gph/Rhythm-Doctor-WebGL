using System;
using DG.Tweening;
using UnityEngine;

public class RDArm : RDBase
{
	private struct VisibilityProperties
	{
		public int layer;

		public string sortingLayer;

		public int renderQueue;
	}

	public const string PressAnimation = "press";

	public const string NotPressAnimation = "notpress";

	public const float HandButtonDiffY = 6f;

	public const float OnePlayerX = 282f;

	public const float TwoPlayerX = 200f;

	public const float HiddenX = -20f;

	public const string GenericPrefix = "HandGrey_Hand_Indexed";

	public const string EdegaPrefix = "HandEdega";

	public const string PaigePrefix = "HandPaige";

	public scrHandController controller;

	public RDPlayer player;

	public Entity hand;

	public Entity button;

	public Entity buttonEffect;

	public Renderer drawing;

	public Entity[] allSprites;

	[NonSerialized]
	public Entity[] dontOutline;

	private Vector2 drawingPos;

	private Vector2 drawingCatchOffset = new Vector2(-10f, 1f);

	private HandAction lastAction;

	private float lastDuration;

	private bool lastTogglingBothHands;

	private bool handPressed;

	private float holdTime;

	private bool isRaised;

	private bool isShown;

	private bool isCatching;

	[NonSerialized]
	public HandAction currentAction = HandAction.Hide;

	[NonSerialized]
	public bool cpuCanUse = true;

	[NonSerialized]
	public bool playerCanUse = true;

	[NonSerialized]
	public Character cpuOwner = Character.Otto;

	[NonSerialized]
	public string armAnimSuffix = "";

	[NonSerialized]
	public bool moveButtonWithArm = true;

	[NonSerialized]
	public Texture2D basePaletteTex;

	private Material handMat;

	private Tween xTween;

	private Tween yTween;

	private bool setup;

	public float onePlayerX => ProcessXOffset(282f);

	public float twoPlayerX => ProcessXOffset(200f);

	public float hiddenX => ProcessXOffset(-20f);

	private float ProcessXOffset(float offset)
	{
		bool num = base.transform.localScale.x == -1f;
		int room = controller.room;
		float rDWidth = RDBase.Vfx.RDWidth;
		float num2 = 1f;
		if (room < 4)
		{
			num2 = base.game.rooms[room].rdCamera.customScale.x;
		}
		else if (base.game.windowDancing)
		{
			num2 = base.game.windowChoreographer.dancers[0].windowScale.x;
		}
		float num3 = rDWidth * (num2 - 1f) / 2f;
		if (!num)
		{
			return rDWidth + num3 - offset;
		}
		return offset - num3;
	}

	private void Awake()
	{
		if (!setup)
		{
			setup = true;
			Entity[] array = allSprites;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Setup();
			}
			buttonEffect.usesOwnShaderData = true;
			dontOutline = new Entity[1] { buttonEffect };
			drawingPos = drawing.transform.localPosition;
			Toggle(HandAction.Hide, 0f);
			UpdateVisibility();
			handMat = hand.shaderRenderer.material;
			handMat.SetFloat("_UsesPalette", 1f);
			basePaletteTex = Resources.Load<Texture2D>("Palettes/Palette_Hand");
			handMat.SetTexture("_PaletteTex", basePaletteTex);
			hand.animPrefixName = "HandGrey_Hand_Indexed";
			DefibMode defibMode = ((player == RDPlayer.P2) ? scnGame.p2DefibMode : scnGame.p1DefibMode);
			SetDefibMode(defibMode);
		}
	}

	public void SetDefibMode(DefibMode defibMode)
	{
		bool flag = Persistence.GetGameCompletion().percentCompletion >= 100f && playerCanUse;
		string text;
		switch (defibMode)
		{
		case DefibMode.Unmissable:
		case DefibMode.VeryEasy:
		case DefibMode.Easy:
			text = "Easy";
			break;
		case DefibMode.Hard:
			text = "Hard";
			break;
		default:
			text = (flag ? "Gold" : "Normal");
			break;
		}
		string text2 = text;
		button.animPrefixName = "HandGreyButton_" + text2;
		buttonEffect.animPrefixName = "HandGreyButtonEffect_" + text2;
		Entity[] array = allSprites;
		foreach (Entity entity in array)
		{
			if (!(entity == buttonEffect))
			{
				entity.PlayAnim("notpress");
			}
		}
		base.game.LoadPlayerHandPopSounds();
	}

	public void ShowHandButtons(float x)
	{
		Transform obj = button.transform;
		obj.LocalMoveXY(x, 0f);
		obj.DOLocalMoveY(controller.currentVisibleY, 0.5f);
	}

	public void HideInstant()
	{
		KillMovementTweens();
		Toggle(HandAction.Hide, 0f);
	}

	private void Update()
	{
		if (!playerCanUse)
		{
			return;
		}
		bool levelFinishedLoading = base.game.levelFinishedLoading;
		if (base.game.paused || !(base.game.receptive | levelFinishedLoading))
		{
			return;
		}
		bool twoPlayerMode = GC.twoPlayerMode;
		bool flag = RDInput.p1Press || (RDInput.p2Press && !twoPlayerMode);
		bool flag2 = RDInput.p1Release || (RDInput.p2Release && !twoPlayerMode);
		RDInput.PlayerEmuState obj = RDInput.emuStates[1];
		bool flag3 = obj.GetKey(RDInput.PlayerEmuKey.Down) || (twoPlayerMode && RDInput.p2Press);
		bool flag4 = obj.GetKey(RDInput.PlayerEmuKey.Up) || (twoPlayerMode && RDInput.p2Release);
		bool flag5 = ((player == RDPlayer.P1) ? flag : flag3);
		bool flag6 = ((player == RDPlayer.P1) ? flag2 : flag4);
		holdTime = Mathf.Max(holdTime - Time.unscaledDeltaTime, 0f);
		bool flag7 = false;
		foreach (scrPlayerbox item in scrPlayerbox.ListAll())
		{
			flag7 = flag7 || item.holdAssist;
		}
		if (flag5 && !handPressed)
		{
			Press();
		}
		if ((flag6 || (handPressed && scnGame.tapsOnly && holdTime == 0f)) && !flag7)
		{
			Depress();
		}
	}

	public void ToggleLast()
	{
		if (lastAction != HandAction.Unset)
		{
			Toggle(lastAction, lastDuration, lastTogglingBothHands);
		}
	}

	public void UnsetLast()
	{
		lastAction = HandAction.Unset;
	}

	public void Toggle(HandAction action, float duration, bool togglingBothHands = false, bool forceRaise = true)
	{
		lastDuration = duration;
		lastTogglingBothHands = togglingBothHands;
		lastAction = action;
		if (base.game != null && base.game.handControllerOnTop.startingAnimation && controller.room != 4)
		{
			return;
		}
		Entity[] array = allSprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DOKill();
		}
		KillMovementTweens();
		switch (action)
		{
		case HandAction.Show:
		{
			HandAction handAction = currentAction;
			if (handAction == HandAction.Lower || handAction == HandAction.Hide)
			{
				SetX(hiddenX);
				if (forceRaise)
				{
					isRaised = true;
					SetY(controller.currentVisibleY);
				}
			}
			float xTarget = (togglingBothHands ? twoPlayerX : onePlayerX);
			xTween = DOMoveX(xTarget, duration).SetEase(Ease.OutExpo).Done();
			isShown = true;
			UpdateVisibility();
			break;
		}
		case HandAction.Hide:
			xTween = DOMoveX(hiddenX, duration).SetEase(Ease.OutExpo).OnComplete(() =>
			{
				isShown = false;
				UpdateVisibility();
			}).Done();
			break;
		case HandAction.Raise:
			yTween = DOMoveY(controller.currentVisibleY, duration).Done();
			isRaised = true;
			UpdateVisibility();
			break;
		case HandAction.Lower:
			yTween = DOMoveY(0f, duration).SetUpdate(base.game.handControllerOnTop.startingAnimation).OnComplete(() =>
			{
				isRaised = false;
				UpdateVisibility();
			}).Done();
			break;
		}
		currentAction = action;
	}

	public void SetX(float x)
	{
		base.transform.LocalMoveX(x);
		if (moveButtonWithArm)
		{
			button.transform.LocalMoveX(x);
		}
	}

	public void SetY(float y)
	{
		base.transform.LocalMoveY(y - 6f);
		if (moveButtonWithArm)
		{
			button.transform.LocalMoveY(y);
		}
	}

	public Tween DOMoveX(float xTarget, float duration)
	{
		return xTween = DOTween.To(() => base.transform.localPosition.x, (float x) =>
		{
			SetX(x);
		}, xTarget, duration);
	}

	public Tween DOMoveY(float yTarget, float duration)
	{
		return yTween = DOTween.To(() => base.transform.localPosition.y + 6f, (float y) =>
		{
			SetY(y);
		}, yTarget, duration);
	}

	public void KillMovementTweens()
	{
		xTween?.Kill();
		yTween?.Kill();
	}

	public void Press()
	{
		hand.PlayAnim("press" + armAnimSuffix);
		handPressed = true;
		holdTime = 0.05f;
		if (!isCatching)
		{
			drawing.transform.LocalMoveXY(drawingPos.x, drawingPos.y + 1f);
		}
		else
		{
			drawing.transform.localPosition = drawingPos + drawingCatchOffset;
		}
		if (!base.game.handControllerOnTop.slidingIn && !controller.slidingIn)
		{
			button.PlayAnim("press");
			buttonEffect.PlayAnim("press");
		}
	}

	public void Depress()
	{
		if (handPressed)
		{
			hand.PlayAnim("notpress" + armAnimSuffix);
			handPressed = false;
			holdTime = 0f;
			if (!isCatching)
			{
				drawing.transform.localPosition = drawingPos;
			}
			else
			{
				drawing.transform.localPosition = drawingPos + drawingCatchOffset;
			}
			if (!base.game.handControllerOnTop.slidingIn && !controller.slidingIn)
			{
				button.PlayAnim("notpress");
			}
		}
	}

	public void ToggleCatching(bool on)
	{
		isCatching = on;
		armAnimSuffix = (on ? "_catch" : "");
		hand.PlayAnim("notpress" + armAnimSuffix);
		if (!on)
		{
			drawing.transform.localPosition = drawingPos;
		}
		else
		{
			drawing.transform.localPosition = drawingPos + drawingCatchOffset;
		}
	}

	public void ToggleNervous(bool on)
	{
		armAnimSuffix = ((on && cpuOwner == Character.Paige) ? "_nervous" : "");
		hand.PlayAnim("notpress" + armAnimSuffix);
	}

	public void SetHandsInFront(bool inFront, bool beforeGameStart = false)
	{
		VisibilityProperties props = new VisibilityProperties
		{
			layer = (beforeGameStart ? 18 : (inFront ? 11 : 0)),
			sortingLayer = (inFront ? RDSortingLayer.Foreground : RDSortingLayer.Default).ToString(),
			renderQueue = (inFront ? 3000 : 2000)
		};
		Entity[] array = allSprites;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer component = array[i].GetComponent<Renderer>();
			SetRendererProperties(component, props);
		}
		SetRendererProperties(drawing, props);
	}

	private void SetRendererProperties(Renderer renderer, VisibilityProperties props)
	{
		renderer.gameObject.layer = props.layer;
		renderer.sortingLayerName = props.sortingLayer;
		renderer.material.renderQueue = props.renderQueue;
	}

	public void SetSkin(ArmSkin armSkin = null)
	{
		Awake();
		bool flag = armSkin != null;
		handMat.SetFloat("_UsesPalette", flag ? 1 : 0);
		drawing.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		Texture2D texture2D = armSkin.drawing;
		if (texture2D != null)
		{
			texture2D.filterMode = FilterMode.Point;
			drawing.material.SetTexture("_Drawing", texture2D);
		}
		else
		{
			drawing.material.SetTexture("_Drawing", base.gc.emptyTexture);
		}
		drawing.material.SetColor("_TintColor", armSkin.sleeveColor);
		Color[] pixels = basePaletteTex.GetPixels();
		for (int i = 1; i < 13; i++)
		{
			Color color = pixels[i];
			if (i < 7)
			{
				pixels[i] = (color * armSkin.skinColor).WithAlpha(color.a);
			}
			else if (i < 11)
			{
				pixels[i] *= (color * armSkin.skinColor).WithAlpha(color.a).WithLightnessBoost(armSkin.palmLightness);
			}
			else
			{
				pixels[i] = armSkin.nailsColor.WithAlpha(pixels[i].a);
			}
		}
		pixels[13] = Color.black.WithAlpha(1f / 255f);
		Texture2D texture2D2 = new Texture2D(256, 1, TextureFormat.ARGB32, mipChain: false);
		texture2D2.SetPixels(pixels);
		texture2D2.Apply();
		texture2D2.filterMode = FilterMode.Point;
		hand.shaderRenderer.material.SetTexture("_PaletteTex", texture2D2);
	}

	public void SetToCharacter(Character character = Character.Player)
	{
		switch (character)
		{
		case Character.Player:
			playerCanUse = true;
			cpuCanUse = false;
			SetSkin((player == RDPlayer.P1) ? Persistence.p1Skin : Persistence.p2Skin);
			SetDefibMode((player == RDPlayer.P1) ? scnGame.p1DefibMode : scnGame.p2DefibMode);
			hand.sprite.FlipX = false;
			hand.animPrefixName = "HandGrey_Hand_Indexed";
			cpuOwner = Character.Otto;
			break;
		case Character.Ian:
			playerCanUse = false;
			cpuCanUse = true;
			hand.sprite.FlipX = false;
			hand.animPrefixName = "HandGrey_Hand_Indexed";
			SetSkin(base.gc.armSkin_ian);
			SetDefibMode(DefibMode.Hard);
			cpuOwner = Character.Ian;
			break;
		case Character.Paige:
			playerCanUse = false;
			cpuCanUse = true;
			hand.sprite.FlipX = false;
			hand.animPrefixName = "HandPaige";
			SetSkin();
			SetDefibMode(DefibMode.Easy);
			cpuOwner = Character.Paige;
			break;
		case Character.Edega:
			playerCanUse = false;
			cpuCanUse = true;
			hand.sprite.FlipX = true;
			hand.animPrefixName = "HandEdega";
			SetSkin();
			SetDefibMode(DefibMode.Normal);
			cpuOwner = Character.Edega;
			break;
		case Character.Politician:
			playerCanUse = false;
			cpuCanUse = true;
			hand.sprite.FlipX = false;
			hand.animPrefixName = "HandGrey_Hand_Indexed";
			SetSkin(base.gc.armSkin_politician);
			SetDefibMode((player == RDPlayer.P1) ? scnGame.p1DefibMode : scnGame.p2DefibMode);
			cpuOwner = Character.Otto;
			break;
		}
		armAnimSuffix = "";
		hand.PlayAnim("notpress");
	}

	private void UpdateVisibility()
	{
		bool visible = isRaised && isShown;
		SetVisible(visible);
	}

	public void SetVisible(bool visible, bool force = false)
	{
		if (force)
		{
			isRaised = (isShown = visible);
		}
		base.gameObject.SetActive(visible);
		if (moveButtonWithArm)
		{
			button.gameObject.SetActive(visible);
		}
	}
}
