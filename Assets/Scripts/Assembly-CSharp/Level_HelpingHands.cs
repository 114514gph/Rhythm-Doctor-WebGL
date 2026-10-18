using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_HelpingHands : LevelBase
{
	private RDMainWard room2Ward;

	private const float runningSpeed = 200f;

	private Tween speedTween;

	private RDCredits credits;

	public float creditsSpeed;

	public float preordersSpeed;

	public Level_HelpingHands(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		room2Ward = LoadLevelAsset<RDMainWard>();
		room2Ward.ToggleSeamless(on: true);
		room2Ward.transform.localPosition = new Vector3(7020f, 99f);
		room2Ward.scrollRoom = 2;
		if (credits != null)
		{
			UnityEngine.Object.Destroy(credits.gameObject);
		}
		credits = LoadLevelAsset<RDCredits>();
		credits.credits.isHelpingHands = true;
		credits.credits.UpdateFont();
		credits.transform.MoveX(3000f);
		credits.logo.playSounds = false;
		yield return null;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 1)
		{
			base.game.handControllerOnTop.Show(show: false);
		}
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 30)
		{
			scrExecuteOnUpdate.Add(() =>
			{
				Update2();
			});
			room2Ward.scrollSpeed = 200f;
			creditsSpeed = 0f;
		}
	}

	public void SetWardNight()
	{
		RDMainWard[] array = UnityEngine.Object.FindObjectsOfType<RDMainWard>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetNightMode(night: true);
		}
	}

	public void SetWardDay()
	{
		RDMainWard[] array = UnityEngine.Object.FindObjectsOfType<RDMainWard>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetNightMode(night: false);
		}
	}

	private RowEntity[] GetRoom1Rows()
	{
		List<RowEntity> list = new List<RowEntity>();
		Row[] array = base.game.rows;
		foreach (Row row in array)
		{
			if (row.room == 1)
			{
				list.Add(row.ent);
			}
		}
		return list.ToArray();
	}

	public void BeginCredits()
	{
		RowEntity[] room1Rows = GetRoom1Rows();
		for (int i = 0; i < room1Rows.Length; i++)
		{
			room1Rows[i].transform.SetParent(credits.transform);
		}
		credits.logo.Rewind();
	}

	public void EndCredits()
	{
		RowEntity[] room1Rows = GetRoom1Rows();
		for (int i = 0; i < room1Rows.Length; i++)
		{
			room1Rows[i].transform.SetParent(base.room1.rowContainer);
		}
	}

	public void BeginPreorders()
	{
		if (speedTween != null)
		{
			speedTween.Kill();
		}
		creditsSpeed = 0f;
		credits.SwitchToPreorders();
	}

	public void TweenCreditsSpeed(float s, float d, string e)
	{
		if (speedTween != null)
		{
			speedTween.Kill();
		}
		Ease ease = (Ease)Enum.Parse(typeof(Ease), e);
		speedTween = DOTween.To(() => creditsSpeed, (float x) =>
		{
			creditsSpeed = x;
		}, s, d).SetEase(ease);
	}

	public void Update2()
	{
		if (creditsSpeed > 0f)
		{
			credits.transform.LocalMoveY(credits.transform.localPosition.y + creditsSpeed * Time.deltaTime);
		}
		if (preordersSpeed > 0f)
		{
			credits.transform.LocalMoveX(credits.transform.localPosition.x - preordersSpeed * Time.deltaTime);
		}
	}
}
