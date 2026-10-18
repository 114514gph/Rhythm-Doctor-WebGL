using System;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Cutscene_Airport : LevelBase
{
	private RDMainWard _ward;

	private RDMainWard _ward2;

	private bool wardInitialized;

	private float timescale;

	private float hoursPassed;

	private RDMainWard ward
	{
		get
		{
			if (_ward == null)
			{
				_ward = base.room1.hospitalWard.GetComponent<RDMainWard>();
			}
			return _ward;
		}
	}

	private RDMainWard ward2
	{
		get
		{
			if (_ward2 == null)
			{
				_ward2 = base.room2.hospitalWard.GetComponent<RDMainWard>();
			}
			return _ward2;
		}
	}

	public Level_Cutscene_Airport(RDLevelData data)
		: base(data)
	{
	}

	public void OpenDoor(bool open)
	{
		ward.SetOfficeDoorOpened(open);
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 1)
		{
			hoursPassed = 12.666f;
			WallClock[] clocks = ward.clocks;
			for (int i = 0; i < clocks.Length; i++)
			{
				clocks[i].settingManually = true;
			}
			clocks = ward2.clocks;
			for (int i = 0; i < clocks.Length; i++)
			{
				clocks[i].settingManually = true;
			}
			ward.ToggleTVs(enabled: false);
			ward2.ToggleTVs(enabled: false);
			ward.ToggleFireplace(enabled: false);
			ward2.ToggleFireplace(enabled: false);
			wardInitialized = true;
		}
	}

	public override void Update()
	{
		if (wardInitialized)
		{
			hoursPassed += Time.deltaTime * timescale;
			hoursPassed = Mathf.Repeat(hoursPassed, 24f);
			WallClock[] clocks = ward.clocks;
			for (int i = 0; i < clocks.Length; i++)
			{
				clocks[i].SetTime(hoursPassed * 3600f);
			}
			clocks = ward2.clocks;
			for (int i = 0; i < clocks.Length; i++)
			{
				clocks[i].SetTime(hoursPassed * 3600f);
			}
			float nightMode = ward.nightTimeCurve.Evaluate(hoursPassed);
			ward.SetNightMode(nightMode);
			ward2.SetNightMode(nightMode);
		}
	}

	public void DoTimescale(float timescale, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		DOTween.To(() => this.timescale, (float x) =>
		{
			this.timescale = x;
		}, timescale, duration).SetEase(ease).Done();
	}
}
