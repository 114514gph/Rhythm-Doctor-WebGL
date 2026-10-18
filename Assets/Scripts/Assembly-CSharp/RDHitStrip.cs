using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RDHitStrip : RDBase
{
	private const float InfiniteHeight = 4000f;

	private const float cheapWidth = 11f;

	private const float normalWidth = 24f;

	private const float easyWidth = 39f;

	private readonly Color DefaultColor = RDPresetColors.Teal.ARGBToColor();

	[SerializeField]
	private Color HeldColor;

	[SerializeField]
	private Color ParticleColor;

	[Header("Runtime")]
	public bool flashing;

	public bool holding;

	public Color normalColor;

	[Header("References")]
	public ParticleSystem leftParticles;

	public ParticleSystem rightParticles;

	public SpriteRenderer quad;

	[NonSerialized]
	public List<Row> rows = new List<Row>();

	private Material holdMaterialLeft;

	private Material holdMaterialRight;

	private float width;

	public Row mainRow
	{
		get
		{
			List<Row> list = rows;
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			return rows[0];
		}
		set
		{
			rows = new List<Row> { value };
		}
	}

	private float height
	{
		get
		{
			return quad.size.y;
		}
		set
		{
			quad.size = quad.size.WithY(value);
		}
	}

	public float holdParticlesSpeed
	{
		set
		{
			ParticleSystem.MainModule main = rightParticles.main;
			main.simulationSpeed = value;
			main = leftParticles.main;
			main.simulationSpeed = value;
		}
	}

	public void SetPlayer(RDPlayer player)
	{
		DefibMode defibMode = ((player == RDPlayer.P2) ? scnGame.p2DefibMode : scnGame.p1DefibMode);
		SetWidth(defibMode);
		normalColor = ((player == RDPlayer.CPU) ? base.gc.playerboxCPUColor : ((!GC.twoPlayerMode) ? DefaultColor : ((player == RDPlayer.P1) ? base.gc.playerboxP1Color : base.gc.playerboxP2Color)));
	}

	public void SetWidth(DefibMode defibMode)
	{
		float num;
		switch (defibMode)
		{
		case DefibMode.Hard:
			num = 11f;
			break;
		case DefibMode.Unmissable:
		case DefibMode.VeryEasy:
		case DefibMode.Easy:
			num = 39f;
			break;
		default:
			num = 24f;
			break;
		}
		width = num;
	}

	public void Awake()
	{
		quad.sortingOrder = 500;
		quad.color = normalColor;
		holdMaterialLeft = leftParticles.GetComponent<Renderer>().material;
		holdMaterialRight = rightParticles.GetComponent<Renderer>().material;
		leftParticles.gameObject.SetActive(value: true);
		rightParticles.gameObject.SetActive(value: true);
		Hide();
	}

	public void Flash(bool hold = false)
	{
		base.xGlobal = GC.StripCenterX;
		if ((bool)base.game && base.game.BeatIsBeingHeldByPlayer(mainRow.GetCurrentPlayer()))
		{
			return;
		}
		if (!hold)
		{
			flashing = true;
			SetHoldProgress(0f);
			bool isIndependentUpdate = Time.timeScale > 1f;
			quad.FlashPartial(1f).SetUpdate(UpdateType.Normal, isIndependentUpdate).OnComplete(() =>
			{
				flashing = false;
			});
		}
		else
		{
			StartHolding();
		}
	}

	public void SetHoldProgress(float t)
	{
		quad.color = Color.Lerp(normalColor.WithAlpha(0.5f), HeldColor, t);
		Color value = Color.Lerp(normalColor, ParticleColor, t).WithAlpha(1f);
		float value2 = DOVirtual.EasedValue(0f, 1f, t, Ease.OutQuad);
		holdMaterialLeft.SetColor("_ColorOverlay", value);
		holdMaterialRight.SetColor("_ColorOverlay", value);
		holdMaterialLeft.SetFloat("_Opacity", value2);
		holdMaterialRight.SetFloat("_Opacity", value2);
	}

	public void StartHolding()
	{
		SetHoldProgress(0f);
		flashing = true;
		holding = true;
		quad.DOKill();
		holdMaterialLeft.DOKill();
		holdMaterialRight.DOKill();
		holdMaterialLeft.SetFloat("_Opacity", 0f);
		holdMaterialRight.SetFloat("_Opacity", 0f);
		quad.color = quad.color.WithAlpha(0.5f);
	}

	public void StopHolding()
	{
		float duration = 0.5f;
		if (holding)
		{
			flashing = false;
		}
		holding = false;
		quad.DOKill();
		quad.DOFade(0f, duration);
		holdMaterialLeft.DOKill();
		holdMaterialRight.DOKill();
		holdMaterialLeft.DOFloat(0f, "_Opacity", duration);
		holdMaterialRight.DOFloat(0f, "_Opacity", duration);
	}

	public void Hide()
	{
		quad.color = normalColor.WithAlpha(0f);
		holdMaterialLeft.SetFloat("_Opacity", 0f);
		holdMaterialRight.SetFloat("_Opacity", 0f);
	}

	private void LateUpdate()
	{
		float num = holdMaterialLeft.GetFloat("_Opacity");
		float num2 = holdMaterialRight.GetFloat("_Opacity");
		leftParticles.gameObject.SetActive(num != 0f);
		rightParticles.gameObject.SetActive(num2 != 0f);
		if (mainRow?.ent == null)
		{
			base.transform.localScale = Vector3.zero;
			return;
		}
		Transform hitstripCenter = mainRow.ent.hitstripCenter;
		float num3 = hitstripCenter.eulerAngles.z * (MathF.PI / 180f);
		Vector2 xAxis = new Vector2(MathF.Cos(num3), MathF.Sin(num3));
		float num4 = ProjectX(hitstripCenter.position);
		float num5 = num4;
		foreach (Row row in rows)
		{
			float num6 = ProjectX(row.ent.hitstripCenter.position);
			if (num6 < num4)
			{
				num4 = num6;
			}
			if (num6 > num5)
			{
				num5 = num6;
			}
		}
		float num7 = num5 - num4;
		float num8 = width + num7;
		quad.size = quad.size.WithX(num8);
		float num9 = (num5 + num4) * 0.5f;
		Vector2 vector = hitstripCenter.position;
		float num10 = ProjectX(vector);
		Vector2 vector2 = vector + (num9 - num10) * xAxis;
		base.transform.eulerAngles = Vector3.forward * hitstripCenter.eulerAngles.z;
		float num11 = 20f;
		float num12 = num8 / 2f + num11;
		leftParticles.transform.LocalMoveX(0f - num12);
		rightParticles.transform.LocalMoveX(num12);
		base.transform.localScale = new Vector3(hitstripCenter.lossyScale.x, 1f, 1f);
		if (mainRow.ent.customPosition || !GC.twoPlayerMode)
		{
			base.xGlobal = vector2.x;
			base.yGlobal = vector2.y;
			height = 4000f;
		}
		else
		{
			RDPlayer currentPlayer = mainRow.GetCurrentPlayer();
			Range range = base.game.rooms[mainRow.room].playerBounds[(int)currentPlayer];
			base.xGlobal = hitstripCenter.position.x;
			height = range.max - range.min + 32f;
			base.yGlobal = (range.max + range.min) / 2f;
		}
		float ProjectX(Vector2 pos)
		{
			return Vector2.Dot(pos, xAxis);
		}
	}
}
