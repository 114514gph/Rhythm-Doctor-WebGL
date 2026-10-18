using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class RowEntity : InvisibleEntity
{
	public const float InitialCharPosX = -100f;

	public const float CharPosX = 0f;

	public const float DefaultHitlineWidth = 106f;

	public const int SmokesNumber = 12;

	private const float lineLeftSize = 19f / 24f;

	private const float lineRightSize = 0.5f;

	public ClassicRowController classicRowController;

	public OneshotRowController oneshotRowController;

	public static Dictionary<string, Sprite> markersDict;

	public ParticleSystem playerParticle;

	public float pivot;

	public Transform pivotTransform;

	public Transform rowContainer;

	public scrHeart heart;

	public Transform heartContainer;

	public scrChar character;

	public Transform characterContainer;

	public scrPlayerbox playerBox;

	public scrHoldBar holdBar;

	public Entity lineLeft;

	public Entity lineHitLeftGreen;

	public Entity lineHitLeft;

	public Entity lineHitRight;

	public Entity lineHitRightGreen;

	public Transform hitstripCenter;

	public Transform lineContainer;

	public Transform lineHitContainer;

	public RDWaveRenderer_Pulse lineEntrance;

	public SpriteRenderer playerIndicator;

	public SpriteRenderer playerIndicatorInner;

	public TextMesh debugText;

	public float hitlineWidth = 106f;

	public SpriteRenderer characterMarker;

	public SpriteAnimation characterMarkerAnimation;

	public GameObject[] smokes;

	public RowEntity targetCharacterRowEnt;

	[NonSerialized]
	public Row row;

	[NonSerialized]
	public RowVisibilityMode appearance;

	[NonSerialized]
	public RowEffect effect;

	[NonSerialized]
	protected scrExecuteOnCertainBeat scheduledEntrance;

	[NonSerialized]
	public Sequence entranceSequence;

	[NonSerialized]
	public Tween positionTweenX;

	[NonSerialized]
	public Tween positionTweenY;

	[NonSerialized]
	public Tween scaleTweenX;

	[NonSerialized]
	public Tween scaleTweenY;

	[NonSerialized]
	public Tween charPositionTweenX;

	[NonSerialized]
	public Tween charPositionTweenY;

	[NonSerialized]
	public Tween charScaleTweenX;

	[NonSerialized]
	public Tween charScaleTweenY;

	[NonSerialized]
	public Tween heartPositionTweenX;

	[NonSerialized]
	public Tween heartPositionTweenY;

	[NonSerialized]
	public Tween heartScaleTweenX;

	[NonSerialized]
	public Tween heartScaleTweenY;

	[NonSerialized]
	public Tween pivotTweenX;

	[NonSerialized]
	public Tween overlayTween;

	[NonSerialized]
	public Tween noiseTween;

	[NonSerialized]
	public Tween beatboxFadeTween;

	[NonSerialized]
	public float rowMisses;

	[NonSerialized]
	public int flashTimer;

	[NonSerialized]
	public bool customPosition;

	[NonSerialized]
	public int beatsLeftToHit;

	[NonSerialized]
	public int sortDepth;

	[NonSerialized]
	public bool animateCharacterEntrance = true;

	[NonSerialized]
	public bool hideHitFX;

	[NonSerialized]
	public float visualRowLength = 7f;

	[NonSerialized]
	public SpinningRows spinningRowsController;

	public SubdivisionSpotlightController subdivisionSpotlightController;

	public FreezeshotIceController freezeshotIceController;

	private float[] angles = new float[3];

	private Tween[] rotationTweens = new Tween[3];

	private MeshRenderer heartMeshRenderer;

	private int lastFrameExplosion;

	private bool hideHeart;

	private float lastTimeLoweredHealth;

	private float tFreezeAnim;

	private bool playingFreezeAnim;

	public Entity iceBlock => freezeshotIceController.iceBlock;

	public int rowID => row.id;

	private bool cpuControlled => row.GetCurrentPlayer() == RDPlayer.CPU;

	public List<InvisibleEntity> tintableEntities { get; protected set; } = new List<InvisibleEntity>();

	public bool heartHasCracked => rowMisses >= base.game.currentLevel.missesToCrackHeart;

	public bool rowIsVisible
	{
		get
		{
			RowVisibilityMode rowVisibilityMode = appearance;
			return rowVisibilityMode == RowVisibilityMode.Visible || rowVisibilityMode == RowVisibilityMode.OnlyRow;
		}
	}

	private new void Awake()
	{
		base.Awake();
		heartMeshRenderer = heart.GetComponent<MeshRenderer>();
	}

	public void UpdatePlayerSprites()
	{
		if (!GC.twoPlayerMode && row.GetCurrentPlayer() != RDPlayer.CPU)
		{
			characterMarker.enabled = false;
			return;
		}
		characterMarker.enabled = true;
		RDPlayer currentPlayer = row.GetCurrentPlayer();
		string text = ((currentPlayer == RDPlayer.CPU) ? row.cpuCharacter.ToString() : currentPlayer.ToString());
		if (markersDict.ContainsKey(text))
		{
			characterMarker.sprite = markersDict[text];
			SpriteAnimation component = characterMarker.GetComponent<SpriteAnimation>();
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			return;
		}
		List<string> list = new List<string>();
		foreach (string key in markersDict.Keys)
		{
			if (key.Contains(text))
			{
				list.Add(key);
			}
		}
		characterMarkerAnimation = characterMarker.gameObject.GetOrAddComponent<SpriteAnimation>();
		SpriteAnimation.SpriteAnimationData spriteAnimationData = new SpriteAnimation.SpriteAnimationData();
		spriteAnimationData.defaultSpriteIndex = 0;
		spriteAnimationData.loops = 1;
		spriteAnimationData.name = text;
		spriteAnimationData.speedMultiplier = 1f;
		spriteAnimationData.sprites = new Sprite[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			spriteAnimationData.sprites[i] = markersDict[list[i]];
		}
		characterMarkerAnimation.currentAnimationData = spriteAnimationData;
		characterMarkerAnimation.singleSpriteDuration = 0.12f;
		characterMarkerAnimation.stopOnLoopEnding = true;
		characterMarkerAnimation.Stop();
	}

	private void Update()
	{
		if (base.game.currentLevel.invisibleChars)
		{
			character.visible = false;
		}
		if (base.game.currentLevel.invisibleHeart)
		{
			heart.visible = false;
		}
		if (row.isFlashing)
		{
			flashTimer++;
			bool num = flashTimer % 4 == 1 || flashTimer % 4 == 2;
			if (shaderDataSource == null)
			{
				shaderDataSource = this;
				shaderData.CopyFrom(RDBase.Vfx.globalShaderData);
			}
			if (playerBox.shaderDataSource == null)
			{
				playerBox.shaderDataSource = playerBox;
				playerBox.shaderData.CopyFrom(RDBase.Vfx.globalShaderData);
			}
			float black = (num ? 0.3f : 0.6f);
			shaderDataSource = this;
			playerBox.shaderDataSource = playerBox;
			shaderData.black = black;
			playerBox.shaderData.black = black;
		}
		debugText.gameObject.SetActive(RDBase.debugSettings.Debug);
		if (RDBase.debugSettings.Debug)
		{
			debugText.text = $"{rowID}/{row.GetCurrentPlayer()}";
		}
		if (playingFreezeAnim)
		{
			tFreezeAnim += Time.deltaTime;
			Texture2D[] freezeshot_textures = base.gc.freezeshot_textures;
			int num2 = Mathf.RoundToInt(tFreezeAnim * base.gc.freezeshot_animationFPS);
			if (num2 >= freezeshot_textures.Length)
			{
				playingFreezeAnim = false;
				num2 = 1;
			}
			SetOverlayTextureAsFreezeshot(num2);
		}
	}

	public void SetOverlayTextureAsFreezeshot(int frameIndex)
	{
		base.usesOwnShaderData = true;
		shaderData.overlayTexture = base.gc.freezeshot_textures[frameIndex];
	}

	public void SetSortOrder(int depth, bool exactDepth = false)
	{
		int num = (exactDepth ? 1 : 10);
		int num2 = sortDepth * num;
		int num3 = depth * num - num2;
		sortDepth = depth;
		foreach (InvisibleEntity tintableEntity in tintableEntities)
		{
			tintableEntity.GetComponent<Renderer>().sortingOrder += num3;
		}
		RDWave[] waves = oneshotRowController.waves;
		foreach (RDWave rDWave in waves)
		{
			RDWaveRenderer rDWaveRenderer = (rDWave.usesMultiPulse ? rDWave.multiPulse_PolygonRenderer : rDWave.waveRenderer);
			if (!(rDWaveRenderer == null))
			{
				RDWaveRenderer_Polygon obj = rDWaveRenderer as RDWaveRenderer_Polygon;
				obj.polygonMain.meshRenderer.sortingOrder += num3;
				obj.polygonGlow.meshRenderer.sortingOrder += num3;
				obj.polygonOutline.meshRenderer.sortingOrder += num3;
			}
		}
		oneshotRowController.charSparks.GetComponent<Renderer>().sortingOrder += num3;
		oneshotRowController.lineSparks.GetComponent<Renderer>().sortingOrder += num3;
		oneshotRowController.heartSparks.GetComponent<Renderer>().sortingOrder += num3;
		characterMarker.sortingOrder += num3;
		playerParticle.GetComponent<Renderer>().sortingOrder = 16 * num + 50;
		UpdateEffectSort();
	}

	public void SetSortLayer(string layerName)
	{
		foreach (InvisibleEntity tintableEntity in tintableEntities)
		{
			tintableEntity.GetComponent<Renderer>().sortingLayerName = layerName;
		}
		RDWave[] waves = oneshotRowController.waves;
		foreach (RDWave rDWave in waves)
		{
			RDWaveRenderer rDWaveRenderer = (rDWave.usesMultiPulse ? rDWave.multiPulse_PolygonRenderer : rDWave.waveRenderer);
			if (!(rDWaveRenderer == null))
			{
				RDWaveRenderer_Polygon obj = rDWaveRenderer as RDWaveRenderer_Polygon;
				obj.polygonMain.meshRenderer.sortingLayerName = layerName;
				obj.polygonGlow.meshRenderer.sortingLayerName = layerName;
				obj.polygonOutline.meshRenderer.sortingLayerName = layerName;
			}
		}
		oneshotRowController.charSparks.GetComponent<Renderer>().sortingLayerName = layerName;
		oneshotRowController.lineSparks.GetComponent<Renderer>().sortingLayerName = layerName;
		oneshotRowController.heartSparks.GetComponent<Renderer>().sortingLayerName = layerName;
		characterMarker.sortingLayerName = layerName;
		playerParticle.GetComponent<Renderer>().sortingLayerName = layerName;
		if (effect == RowEffect.Electric)
		{
			GameObject[] array = smokes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Renderer>().sortingLayerName = layerName;
			}
		}
	}

	public void Present(bool animate)
	{
		if (!base.game.currentLevel.charsOnlyOnStart)
		{
			Show((!animate) ? ShowAnimationType.InstantWithEffects : ((!base.game.currentLevel.noRowAnimsOnStart) ? ShowAnimationType.FullAnimation : ShowAnimationType.InstantNoEffects), setup: true);
		}
		else
		{
			Hide(keepCharacterVisible: true, effects: false);
		}
	}

	public virtual void Setup(int id, Character _char, bool animate, bool customPosition, string customCharacter, bool present, bool hideAtStart = false)
	{
		tintableEntities = new List<InvisibleEntity>
		{
			character,
			lineLeft,
			classicRowController.beatboxes[0],
			classicRowController.beatboxes[1],
			classicRowController.beatboxes[2],
			classicRowController.beatboxes[3],
			classicRowController.beatboxes[4],
			classicRowController.beatboxes[5],
			classicRowController.beatboxes[0].beatSkip,
			classicRowController.beatboxes[1].beatSkip,
			classicRowController.beatboxes[2].beatSkip,
			classicRowController.beatboxes[3].beatSkip,
			classicRowController.beatboxes[4].beatSkip,
			classicRowController.beatboxes[5].beatSkip,
			lineHitLeftGreen,
			lineHitLeft,
			playerBox,
			lineHitRight,
			lineHitRightGreen,
			heart,
			lineEntrance,
			iceBlock
		};
		RDWave[] waves = oneshotRowController.waves;
		foreach (RDWave rDWave in waves)
		{
			if (rDWave.usesMultiPulse)
			{
				AddWaveRenderer(rDWave.multiPulse_PulseRenderer);
				AddWaveRenderer(rDWave.multiPulse_PolygonRenderer);
			}
			else
			{
				AddWaveRenderer(rDWave.waveRenderer);
			}
		}
		SetSortOrder(id);
		if (present)
		{
			Present(animate);
		}
		row = base.game.rows[id];
		base.name = "Row " + rowID;
		appearance = (hideAtStart ? RowVisibilityMode.Hidden : RowVisibilityMode.Visible);
		if (!customPosition)
		{
			base.transform.localPosition = base.transform.localPosition.WithY(RDBase.Vfx.RDHeight / 2f);
			base.game.RepositionRowsAndStrips();
		}
		heart.Setup();
		character.Setup(_char, "neutral", customCharacter);
		scrChar.namedCharacters.Add("rowChar" + rowID, character);
		character.gameObject.name = ((_char == Character.Custom) ? ("Char: " + customCharacter + " (Custom)") : $"Char: {_char}");
		playerBox.Setup(rowID);
		rowMisses = 0f;
		base.game.hitStripManager.PingRow(row);
		foreach (InvisibleEntity tintableEntity in tintableEntities)
		{
			tintableEntity.shaderDataSource = this;
		}
		UpdatePlayerSprites();
		if (base.game.currentLevel.rowReflections)
		{
			tk2dSprite[] componentsInChildren = GetComponentsInChildren<tk2dSprite>();
			SpriteReflection componentInChildren = character.GetComponentInChildren<SpriteReflection>();
			tk2dSprite[] array = componentsInChildren;
			foreach (tk2dSprite tk2dSprite2 in array)
			{
				if (!tk2dSprite2.GetComponent<SpriteReflection>() && !tk2dSprite2.GetComponentInChildren<SpriteReflection>())
				{
					new GameObject(tk2dSprite2.name + "Reflection").GetOrAddComponent<SpriteReflection>().SetSprite(character.customAnimation, isCharacter: false, componentInChildren.transform, rowID);
				}
			}
		}
		if (base.game.currentLevel.data != null && (base.game.currentLevel.data.settings.version < 49 || base.game.currentLevel.classicHitParticles))
		{
			ParticleSystem.MainModule main = playerParticle.main;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.scalingMode = ParticleSystemScalingMode.Local;
		}
		oneshotRowController.Setup();
		if (character.character == Character.Custom)
		{
			Texture2D freezeTexture = character.customAnimation.data.freezeTexture;
			freezeshotIceController.Setup(freezeTexture);
		}
		for (int j = 0; j < classicRowController.beatboxes.Length; j++)
		{
			classicRowController.beatboxes[j].Setup(j, rowID, animate, this);
		}
		subdivisionSpotlightController.Init(character, rowID);
		void AddWaveRenderer(RDWaveRenderer rend)
		{
			if (!tintableEntities.Contains(rend))
			{
				tintableEntities.Add(rend);
			}
		}
	}

	public void Show(ShowAnimationType animationType, bool setup, bool keepCharacterVisible = true, bool shake = true)
	{
		base.gameObject.SetActive(value: true);
		RowVisibilityMode num = appearance;
		bool flag = num == RowVisibilityMode.OnlyCharacter;
		bool flag2 = num == RowVisibilityMode.Visible;
		animateCharacterEntrance = !flag2 && !flag;
		Hide(flag, effects: false);
		appearance = RowVisibilityMode.Visible;
		if (scheduledEntrance != null)
		{
			scheduledEntrance.enabled = false;
		}
		if (setup && animationType != ShowAnimationType.FullAnimation)
		{
			scheduledEntrance = scrExecuteOnCertainBeat.Add(() =>
			{
				DoEntrance(animationType, shake);
			}, 0f);
		}
		else
		{
			DoEntrance(animationType, shake);
		}
		character.visible = keepCharacterVisible;
		appearance = ((!keepCharacterVisible) ? RowVisibilityMode.OnlyRow : RowVisibilityMode.Visible);
	}

	public void Hide(bool keepCharacterVisible = false, bool effects = true, bool shake = true)
	{
		entranceSequence?.Kill(complete: true);
		beatboxFadeTween?.Kill(complete: true);
		if (scheduledEntrance != null)
		{
			scheduledEntrance.enabled = false;
		}
		lineEntrance.visible = false;
		heart.visible = false;
		lineLeft.visible = false;
		lineHitLeftGreen.visible = false;
		lineHitLeft.visible = false;
		lineHitRight.visible = false;
		lineHitRightGreen.visible = false;
		playerBox.visible = false;
		playerBox.SetHorizontalPosition(0);
		SwitchRowTypeAppearance(RowType.None);
		holdBar.visibleHold = false;
		if (keepCharacterVisible)
		{
			character.visible = true;
			appearance = RowVisibilityMode.OnlyCharacter;
		}
		else
		{
			character.visible = false;
			appearance = RowVisibilityMode.Hidden;
		}
		if (keepCharacterVisible)
		{
			character.x = 0f;
		}
		scrBeatbox[] beatboxes = classicRowController.beatboxes;
		foreach (scrBeatbox obj in beatboxes)
		{
			obj.visible = false;
			obj.beatSkip.visible = false;
		}
		if (effects)
		{
			beatboxes = classicRowController.beatboxes;
			foreach (scrBeatbox scrBeatbox2 in beatboxes)
			{
				if ((object)scrBeatbox2 != null && scrBeatbox2.visible && !(base.gameObject == null))
				{
					Smoke.Add(scrBeatbox2.xGlobal, scrBeatbox2.yGlobal, base.transform.localScale);
				}
			}
			ShowRowSmokes();
			if (shake)
			{
				RDBase.Vfx.rooms[row.room].camera.Shake(10, 2);
			}
			DoRemoveRowEffects(shake);
		}
		oneshotRowController.ToggleAllWavesVisible(visible: false);
	}

	public void DoEntrance(ShowAnimationType entranceType, bool shake = true)
	{
		entranceSequence?.Kill(complete: true);
		beatboxFadeTween?.Kill(complete: true);
		bool doCharacterEntrance = animateCharacterEntrance && !customPosition;
		bool instant = entranceType != ShowAnimationType.FullAnimation;
		if (!customPosition)
		{
			character.transform.LocalMoveX(-100f);
		}
		oneshotRowController.ToggleAllWavesVisible(visible: true);
		character.visible = true;
		characterMarker.transform.localScale = Vector3.zero;
		float rowWidth = GetRowWidth();
		playerBox.doingEntranceAnimation = true;
		entranceSequence = (Sequence)DOTween.Sequence().Append(GetCharEntryTween(doCharacterEntrance)).AppendCallback(() =>
		{
			if (!instant & shake)
			{
				RDBase.Vfx.ShakeCam(10, 2, row.room);
			}
			lineEntrance.gameObject.SetActive(value: true);
			lineEntrance.visible = true;
			heart.visible = !hideHeart;
			lineEntrance.material.SetVector(RDWaveRenderer.triangleOffsetID, Vector4X(UnityEngine.Random.Range(0f, 1f)));
			lineEntrance.material.SetFloat("_FlashPower", 1f);
		})
			.Append(lineEntrance.transform.DOScaleX(rowWidth, 0.6f).From(0f).SetEase(Ease.OutSine))
			.Join(lineEntrance.transform.DOLocalMoveX(rowWidth / 2f, 0.6f).From(0f).SetEase(Ease.OutSine))
			.Join(heartContainer.transform.DOLocalMoveX(0f, 0.6f).From(0f - rowWidth).SetEase(Ease.OutSine))
			.Join(heart.transform.DOScale(1f, 0.6f).From(0f).SetEase(Ease.OutBack, 2.5f))
			.Join(lineEntrance.material.DOFloat(rowWidth, RDWaveRenderer.canvasWidthID, 0.6f).From(0f).SetEase(Ease.OutSine))
			.Join(lineEntrance.material.DOVector(Vector4X(rowWidth), RDWaveRenderer.sineWidthID, 0.6f).From(Vector4.zero).SetEase(Ease.OutSine))
			.Join(lineEntrance.material.DOVector(Vector4X(0f), RDWaveRenderer.triangleFrequencyID, 0.6f).From(Vector4X(UnityEngine.Random.Range(0.02f, 0.03f))).SetEase(Ease.Linear))
			.Join(lineEntrance.material.DOVector(Vector4X(0f), RDWaveRenderer.sineHeightID, 0.6f).From(Vector4X(15f)).SetEase(Ease.InOutElastic))
			.Join(lineEntrance.material.DOVector(Vector4X(5f), RDWaveRenderer.triangleOffsetID, 0.6f).From(Vector4X(0f)).SetEase(Ease.OutSine))
			.AppendCallback(() =>
			{
				SwitchRowTypeAppearance(row.rowType);
				lineEntrance.visible = false;
				lineLeft.visible = true;
				lineHitLeftGreen.visible = true;
				lineHitLeftGreen.transform.ScaleX(playerBox.HalfHitlineWidth / 24f);
				lineHitLeftGreen.transform.localPosition = Vector3.right * playerBox.HalfHitlineWidth / 2f;
				lineHitRightGreen.visible = true;
				lineHitRightGreen.transform.ScaleX(playerBox.HalfHitlineWidth / 24f + 0.5f);
				lineHitRightGreen.transform.localPosition = Vector3.right * (playerBox.HitlineWidth + 12f - (playerBox.HalfHitlineWidth + 12f) / 2f);
				lineHitLeft.visible = true;
				lineHitLeft.transform.ScaleX(0f);
				lineHitLeft.transform.localPosition = Vector3.right * playerBox.HalfHitlineWidth;
			})
			.Append(heart.transform.parent.DOShakePosition(0.3f, 2f, 50))
			.Join(characterMarker.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack))
			.Append(lineHitLeftGreen.transform.DOScaleX(0f, 0.6f).SetEase(Ease.OutExpo))
			.Join(lineHitLeftGreen.transform.DOLocalMoveX(0f, 0.6f).SetEase(Ease.OutExpo))
			.Join(lineHitRightGreen.transform.DOScaleX(0.5f, 0.6f).SetEase(Ease.OutExpo))
			.Join(lineHitRightGreen.transform.DOLocalMoveX(playerBox.HitlineWidth + 6f, 0.6f).SetEase(Ease.OutExpo))
			.Join(lineHitLeft.transform.DOScaleX(playerBox.HitlineWidth / 24f, 0.6f).SetEase(Ease.OutExpo))
			.Join(lineHitLeft.transform.DOLocalMoveX(playerBox.HalfHitlineWidth, 0.6f).SetEase(Ease.OutExpo))
			.AppendCallback(() =>
			{
				playerBox.doingEntranceAnimation = false;
				playerBox.UpdateSize();
				playerBox.visible = true;
				lineHitRight.visible = true;
				lineEntrance.visible = false;
			})
			.OnUpdate(() =>
			{
				lineEntrance.UpdateShaderDataSource();
			})
			.Done();
		if (instant)
		{
			entranceSequence.Complete(withCallbacks: true);
			if (entranceType == ShowAnimationType.InstantWithEffects)
			{
				ShowRowSmokes();
			}
		}
		static Vector4 Vector4X(float x)
		{
			return new Vector4(x, 0f, 0f, 0f);
		}
	}

	public void SetRowLength(int length, float duration)
	{
		row.rowLengthForNarration = length;
		int num = length - classicRowController.beatboxes.Length - 1;
		for (int i = 0; i < classicRowController.beatboxes.Length; i++)
		{
			Transform target = classicRowController.beatboxes[i].transform;
			int num2 = num + i;
			if (num2 >= 0)
			{
				target.DOLocalMoveX(12f + (float)(num2 * 24), duration).Done();
				target.DOScaleX(1f, duration).Done();
			}
			else
			{
				target.DOLocalMoveX(0f, duration).Done();
				target.DOScaleX(0f, duration).Done();
			}
		}
		DOTween.To(() => visualRowLength, (float x) =>
		{
			visualRowLength = x;
		}, length, duration).OnUpdate(() =>
		{
			oneshotRowController.ResizeLine(24f * (visualRowLength - 1f));
			playerBox.hitLineLength = visualRowLength - 1f;
			playerBox.UpdateSize();
			playerParticle.transform.localPosition = Vector3.right * playerBox.HalfHitlineWidth;
			hitstripCenter.localPosition = Vector3.right * playerBox.HalfHitlineWidth;
			lineHitRightGreen.transform.localPosition = Vector3.right * (playerBox.HitlineWidth + 6f);
			heartContainer.transform.parent.localPosition = Vector3.right * (playerBox.HitlineWidth + 12f);
			holdBar.transform.localPosition = holdBar.transform.localPosition.WithX(playerBox.HalfHitlineWidth - 15f);
		}).Done();
		lineHitContainer.transform.DOLocalMoveX(19f + (float)((length - 1) * 24), duration).Done();
	}

	public float GetRowWidth()
	{
		return 19f + (visualRowLength - 1f) * 24f + playerBox.HitlineWidth + 12f;
	}

	public float GetLeftmostPivot()
	{
		return 0f;
	}

	public float GetRightmostPivot()
	{
		return GetRowWidth();
	}

	public void ExpressionPlusFX(string expression, bool forceInstantExplosion = false, bool copyingToShadow = false)
	{
		if (base.game.receptive)
		{
			bool flag;
			switch (expression)
			{
			case "happy":
				flag = character.noHappy;
				break;
			case "barely":
			case "missed":
				flag = character.noMiss;
				break;
			default:
				flag = false;
				break;
			}
			if (!flag)
			{
				targetCharacterRowEnt.character.PlayExpression(expression);
			}
		}
		if (!row.isShadowRealtime && base.game.shadowRowStyle == ShadowRowStyle.ShadowToHost)
		{
			foreach (Row item in base.game.rows.Where((Row r) => r.shadowOrHostIDRealtime == rowID))
			{
				if (item.reactToHostsBeatsWhenShadow)
				{
					item.ent.ExpressionPlusFX(expression, forceInstantExplosion: false, copyingToShadow: true);
				}
			}
		}
		if ((hideHitFX | copyingToShadow) || !(expression == "happy") || Time.frameCount <= lastFrameExplosion)
		{
			return;
		}
		lastFrameExplosion = Time.frameCount;
		heart.HitHappy(forceInstantExplosion);
		if (!base.game.currentLevel.noHitParticles)
		{
			RDPlayer currentPlayer = row.GetCurrentPlayer();
			Color color = ((currentPlayer == RDPlayer.CPU) ? base.gc.playerboxCPUColor : ((!GC.twoPlayerMode) ? base.gc.particleColor : ((currentPlayer == RDPlayer.P1) ? base.gc.playerboxP1Color : base.gc.playerboxP2Color)));
			if (GC.twoPlayerMode)
			{
				color = color.WithAlpha(base.gc.particleColor.a);
				color.r *= 1.15f;
				color.g *= 1.15f;
				color.b *= 1.15f;
			}
			ParticleSystem.MainModule main = playerParticle.main;
			main.startColor = new ParticleSystem.MinMaxGradient(color);
			playerParticle.Emit(200);
		}
	}

	public void Heal(float weightAbs = 0.5f)
	{
		rowMisses = Mathf.Max(rowMisses - weightAbs, 0f);
		base.game.OnMistakeOrHeal(0f, 0f - weightAbs, row);
		if (!base.game.currentLevel.manualHeartCrack)
		{
			heart.SetMisses(rowMisses);
		}
		if (base.game.currentLevel.shouldMakeHealthBar)
		{
			base.game.UpdatePlayerHealthBars();
		}
	}

	public void CrackAdvance(float weight = 1f, bool ignoreBoss2Hack = false)
	{
		if (!ignoreBoss2Hack && base.game.currentLevel is Level_Boss2)
		{
			if (Time.realtimeSinceStartup - lastTimeLoweredHealth < 0.02f)
			{
				printe("canceled because too close beats");
				return;
			}
			lastTimeLoweredHealth = Time.realtimeSinceStartup;
		}
		heart.Shake(20, 5);
		character.Shake(20, 2);
		bool shouldMakeHealthBar = base.game.currentLevel.shouldMakeHealthBar;
		if ((GC.twoPlayerMode & shouldMakeHealthBar) && base.game.playersAlive == 1 && weight > 0f)
		{
			weight *= 2f;
		}
		rowMisses += weight;
		if (!base.game.currentLevel.manualHeartCrack)
		{
			heart.SetMisses(shouldMakeHealthBar ? rowMisses : base.game.mistakesManager.mistakes);
		}
		if (!shouldMakeHealthBar)
		{
			return;
		}
		base.game.UpdatePlayerHealthBars();
		if (!heartHasCracked || base.game.currentLevel.noBossFail)
		{
			return;
		}
		if (GC.twoPlayerMode)
		{
			base.game.StopBeatsFromRow(rowID, killHalfwayPlayingSounds: false);
			base.game.DeleteRow(rowID, 0f, deleteShadowRow: false, repositionRows: true);
			row.dead = true;
			if (base.game.playersAlive == 0)
			{
				base.game.FailLevel(this);
			}
		}
		else
		{
			base.game.FailLevel(this);
		}
	}

	public void Reposition(int yNew, bool instant = false)
	{
		if (customPosition)
		{
			return;
		}
		Vector3 localPosition = new Vector3(35f, yNew, base.transform.localPosition.z);
		if (!instant)
		{
			float latency = scrConductor.latency;
			positionTweenX?.Kill();
			positionTweenY?.Kill();
			positionTweenX = base.transform.DOLocalMoveX(localPosition.x, latency);
			positionTweenY = base.transform.DOLocalMoveY(localPosition.y, latency);
			Rotate(0f, latency, Ease.Linear);
			scaleTweenX?.Kill();
			scaleTweenY?.Kill();
			scaleTweenX = base.transform.DOScaleX(1f, latency);
			scaleTweenY = base.transform.DOScaleY(1f, latency);
			pivotTweenX?.Kill();
			pivotTweenX = DOTween.To(() => pivot, (float x) =>
			{
				pivot = x;
			}, 0f, latency);
		}
		else
		{
			positionTweenX?.Kill(complete: true);
			positionTweenY?.Kill(complete: true);
			base.transform.localPosition = localPosition;
			Rotate(0f, 0f, Ease.Linear);
			scaleTweenX?.Kill(complete: true);
			scaleTweenY?.Kill(complete: true);
			base.transform.ScaleXY(1f, 1f);
			pivotTweenX?.Kill(complete: true);
			pivot = 0f;
		}
	}

	public void DoRemoveRowEffects(bool shake = true)
	{
		if (character != null)
		{
			float num = character.transform.position.x;
			float num2 = character.transform.localScale.y * 0.5f;
			Smoke.Add(num, base.yGlobal + num2, base.transform.localScale);
			Smoke.Add(num + 2f, base.yGlobal + num2, base.transform.localScale);
			Smoke.Add(num + 4f, base.yGlobal + num2, base.transform.localScale);
			if (heart.visible)
			{
				Smoke.Add(heart.xGlobal, heart.yGlobal, base.transform.localScale);
			}
		}
		if (shake)
		{
			RDBase.Vfx.ShakeCam(10, 2, row.room);
		}
	}

	public void SetOverrideAdditiveColor(Color additiveColor)
	{
		shaderData.additiveColor = additiveColor;
		foreach (InvisibleEntity tintableEntity in tintableEntities)
		{
			tintableEntity.shaderDataSource = this;
			tintableEntity.shaderData.additiveColor = additiveColor;
		}
	}

	public void FlashChildren(float duration = 2f, float alpha = 1f)
	{
		foreach (InvisibleEntity tintableEntity in tintableEntities)
		{
			if (tintableEntity.gameObject.TryGetComponent<Entity>(out var component))
			{
				SpriteFX.Flash(component.sprite, duration, alpha);
			}
		}
	}

	public void TintIfCPU(bool isEntrance)
	{
		if (cpuControlled)
		{
			if (shaderDataSource == null)
			{
				shaderData.CopyFrom(RDBase.Vfx.globalShaderData);
			}
			if (playerBox.shaderDataSource == null)
			{
				playerBox.shaderData.CopyFrom(RDBase.Vfx.globalShaderData);
			}
			shaderDataSource = this;
			playerBox.shaderDataSource = playerBox;
			shaderData.black = 0.5f;
			playerBox.shaderData.black = 0.5f;
			row.isFlashing = false;
		}
		else if (!isEntrance)
		{
			shaderData.black = 0f;
			playerBox.shaderData.black = 0f;
		}
	}

	public void ChangeCharacter(Character newChar, bool showParticles = false)
	{
		character.ChangeCharacter(newChar);
		if (showParticles)
		{
			ShowRowSmokes();
		}
	}

	public void ChangeCharacterCustom(string charName, bool showParticles = false)
	{
		character.ChangeCharacter(Character.Custom, charName);
		freezeshotIceController.Setup(character.customAnimation.data.freezeTexture);
		if (showParticles)
		{
			ShowRowSmokes();
		}
	}

	public void ChangeCharacterRandom()
	{
		Array values = Enum.GetValues(typeof(Character));
		System.Random random = new System.Random();
		Character newChar = (Character)values.GetValue(random.Next(values.Length));
		character.ChangeCharacter(newChar);
	}

	public void SetMaterial(bool usesOwnShaderData, RDShaderProperties newMaterial = null)
	{
		usesOwnShaderData = true;
		playerBox.shaderData.CopyFrom(newMaterial);
		shaderData.CopyFrom(newMaterial);
	}

	public void PlayFreezeAnimation(float power)
	{
		overlayTween?.Kill();
		base.usesOwnShaderData = true;
		tFreezeAnim = 0f;
		playingFreezeAnim = true;
		SetOverlayTexturePower(power);
	}

	public void SetOverlayTexturePower(float power, bool killTween = true)
	{
		if (killTween)
		{
			overlayTween?.Kill();
		}
		base.usesOwnShaderData = true;
		shaderData.overlayTexturePower = power;
	}

	public void TweenOverlayTexturePower(float power, float dur)
	{
		overlayTween?.Kill();
		overlayTween = DOTween.To(() => shaderData.overlayTexturePower, (float p) =>
		{
			SetOverlayTexturePower(p, killTween: false);
		}, power, dur);
	}

	public void SetNoiseTexturePower(float power, bool killTween = true)
	{
		if (killTween)
		{
			noiseTween?.Kill();
		}
		base.usesOwnShaderData = true;
		shaderData.noiseTexturePower = power;
	}

	public void TweenNoiseTexturePower(float power, float dur)
	{
		noiseTween?.Kill();
		noiseTween = DOTween.To(() => shaderData.noiseTexturePower, (float p) =>
		{
			SetNoiseTexturePower(p, killTween: false);
		}, power, dur);
	}

	public void FadeOutRowExceptChar(float alpha, float duration)
	{
		foreach (InvisibleEntity tintableEntity in tintableEntities)
		{
			tk2dSprite component = tintableEntity.gameObject.GetComponent<tk2dSprite>();
			if (component != null)
			{
				if (tintableEntity == character)
				{
					component.DOFade(alpha, duration);
				}
				else
				{
					component.DOFade(0f, duration);
				}
			}
		}
	}

	public void FadeInRow(float duration)
	{
		foreach (InvisibleEntity tintableEntity in tintableEntities)
		{
			if (tintableEntity.gameObject.TryGetComponent<tk2dSprite>(out var component))
			{
				component.DOFade(1f, duration);
			}
		}
	}

	public void ShowRowSmokes()
	{
		Vector3 position = heart.transform.position;
		Vector3 position2 = character.transform.position;
		for (int i = 1; i <= 12; i++)
		{
			float t = (float)i / 12f;
			Vector3 vector = Vector3.Lerp(position2, position, t);
			Smoke.Add(vector.x, vector.y, base.transform.localScale);
		}
	}

	public void Rotate(float finalAngle, float duration, Ease ease, MoveRowTarget target = MoveRowTarget.WholeRow)
	{
		Transform targetTransform = target switch
		{
			MoveRowTarget.Character => character.transform, 
			MoveRowTarget.Heart => heartContainer, 
			_ => base.transform, 
		};
		int tweenIndex = (int)target;
		rotationTweens[tweenIndex]?.Kill();
		rotationTweens[tweenIndex] = DOTween.To(() => angles[tweenIndex], (float x) =>
		{
			angles[tweenIndex] = x;
		}, finalAngle, duration).SetEase(ease).OnUpdate(() =>
		{
			targetTransform.localEulerAngles = Vector3.forward * angles[tweenIndex];
		})
			.Done();
	}

	public void LateUpdate()
	{
		if (characterMarker.transform.lossyScale.x < 0f)
		{
			characterMarker.transform.localScale = Vector3.Scale(characterMarker.transform.localScale, new Vector3(-1f, 1f, 1f));
		}
		if (!GC.twoPlayerMode && row.GetCurrentPlayer() != RDPlayer.CPU)
		{
			characterMarker.enabled = false;
		}
		else
		{
			characterMarker.enabled = heartMeshRenderer.enabled;
		}
		RDPlayer currentPlayer = row.GetCurrentPlayer();
		Color color = ((currentPlayer == RDPlayer.CPU) ? base.gc.playerCPUStripColor : ((!GC.twoPlayerMode) ? base.gc.beatboxColor : ((currentPlayer == RDPlayer.P1) ? base.gc.player1StripColor : base.gc.player2StripColor)));
		lineLeft.sprite.color = color.WithAlpha(lineLeft.sprite.color.a);
		lineHitLeftGreen.sprite.color = color.WithAlpha(lineHitLeftGreen.sprite.color.a);
		lineHitRightGreen.sprite.color = color.WithAlpha(lineHitRightGreen.sprite.color.a);
		Color color2 = currentPlayer switch
		{
			RDPlayer.P1 => base.gc.player1StripColor, 
			RDPlayer.P2 => base.gc.player2StripColor, 
			_ => Color.white, 
		};
		characterMarker.color = color2.WithAlpha(heart.shaderDataToUse.opacity);
		string expression = character.ExpressionOverride("prehit");
		if (beatsLeftToHit > 0 && character.IsCurrentExpression("neutral") && character.ExpressionExists(expression))
		{
			character.PlayExpression(expression);
		}
		pivotTransform.LocalMoveX(0f - Mathf.LerpUnclamped(GetLeftmostPivot(), GetRightmostPivot(), pivot));
	}

	public void SetEffect(RowEffect effect)
	{
		if (this.effect == effect)
		{
			return;
		}
		this.effect = effect;
		switch (effect)
		{
		case RowEffect.None:
		{
			GameObject[] array = smokes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
			break;
		}
		case RowEffect.Electric:
		{
			GameObject[] array = smokes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: true);
			}
			UpdateEffectSort();
			break;
		}
		}
	}

	public void UpdateEffectSort()
	{
		if (effect == RowEffect.Electric)
		{
			GameObject[] array = smokes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Renderer>().sortingOrder += sortDepth * 10;
			}
		}
	}

	public void SetHeart(RDHeartType heartType, bool smooth)
	{
		if (smooth && heartType != RDHeartType.Infected)
		{
			ShowRowSmokes();
		}
		if (heartType == RDHeartType.None)
		{
			hideHeart = true;
			heart.visible = false;
			return;
		}
		hideHeart = false;
		heart.visible = rowIsVisible;
		switch (heartType)
		{
		case RDHeartType.Infected:
			if (!smooth)
			{
				heart.QuickInfect();
			}
			else
			{
				heart.InfectAnimation();
			}
			break;
		case RDHeartType.Cracked:
			heart.ChangeSprite("Heart_Joined");
			break;
		case RDHeartType.SplitLeft:
			heart.ChangeSprite("Heart_Left");
			break;
		case RDHeartType.SplitRight:
			heart.ChangeSprite("Heart_Right");
			break;
		case RDHeartType.Halloween:
			heart.ChangeSprite("HeartPumpkin");
			break;
		case RDHeartType.Unbeatable:
			heart.ChangeSprite("HeartUnbeatable");
			break;
		default:
			heart.Reset();
			break;
		}
	}

	public void SetRowMarkerCorner(bool rightCorner)
	{
		characterMarker.transform.LocalMoveX(rightCorner ? 7 : (-9));
	}

	public Tween GetCharEntryTween(bool doCharacterEntrance)
	{
		TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = character.transform.DOLocalMoveX(0f, doCharacterEntrance ? 1f : 0f).SetEase(Ease.OutQuad);
		if (!customPosition)
		{
			tweenerCore.From(-100f);
		}
		return tweenerCore;
	}

	public void ShowIceBlock(bool show)
	{
		if (show)
		{
			if (character.character != Character.Custom)
			{
				freezeshotIceController.SetupBasic();
			}
			iceBlock.visible = character.visible;
		}
		else
		{
			iceBlock.visible = false;
			freezeshotIceController.Explode();
		}
		character.shaderRenderer.material.SetFloat(RDShaderProperties.HideOutlineID, show ? 1 : 0);
	}

	public void SwitchRowTypeAppearance(RowType rowType)
	{
		oneshotRowController.transform.localScale = ((rowType == RowType.Oneshot) ? Vector3.one : Vector3.zero);
		classicRowController.transform.localScale = ((rowType == RowType.Classic) ? Vector3.one : Vector3.zero);
		classicRowController.SetVisible(rowType == RowType.Classic && rowIsVisible);
		oneshotRowController.ToggleAllWavesVisible(rowType == RowType.Oneshot && rowIsVisible);
	}
}
