using RDLevelEditor;
using UnityEngine;

public class Level_LuckyBreak : LevelBase
{
	private RowEntity athleteHomeEnt;

	private RowEntity athleteGuestEnt;

	private RDAthleteWard _stadium;

	private RDAthleteWard _ward;

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

	private RDAthleteWard ward
	{
		get
		{
			if (_ward == null)
			{
				_ward = base.room2.athleteWard.GetComponent<RDAthleteWard>();
			}
			return _ward;
		}
	}

	public Level_LuckyBreak(RDLevelData data)
		: base(data)
	{
	}

	public override void OnHit(HitType hitType, Beat beat = null)
	{
		stadium.HitBaseball(hitType, beat);
		bool home = beat.row.id == 0;
		stadium.UpdateScoreboard(hitType, beat, home);
	}

	public override void preactions()
	{
		int num = base.conductor.barNumber;
		if (num == 1)
		{
			stadium.usingLights = true;
			stadium.ClearAllLights();
		}
		stadium.PrepareBaseballs(num);
	}

	public void SetBaseballTrajectory(string trajectoryString)
	{
		stadium.SetBaseballTrajectory(trajectoryString);
	}

	public void SetBaseballAngleRange(float minAngle, float maxAngle)
	{
		stadium.SetBaseballAngleRange(minAngle, maxAngle);
	}

	public void SetRain(float baseballsPerSecond)
	{
		stadium.SetRain(baseballsPerSecond);
	}

	public void KillRain()
	{
		stadium.KillRain();
	}

	public void CustomBaseball(float startX, float startY, float startZ, float endX, float endY, float endZ, float height, float rotation, float alpha, float duration, string easeString)
	{
		stadium.CustomBaseball(new Vector3(startX, startY, startZ), new Vector3(endX, endY, endZ), height, rotation, alpha, duration, easeString);
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

	public void SetScoreboardLightsGentle(bool home, string text, int startIndex)
	{
		stadium.SetScoreboardLightsWithString(home, text, startIndex, eraseLightsToRight: false);
	}

	public void ClearScoreboardLights(bool home)
	{
		stadium.ClearScoreboardLights(home);
	}

	public void ClearAllScoreboardLights()
	{
		stadium.ClearAllLights();
	}

	public void SetTrackingScore(bool on)
	{
		stadium.trackingScore = on;
	}

	public void SetGlitchyLights(bool on)
	{
		stadium.glitchyLights = on;
	}

	public void AdvanceScoreboardInning()
	{
		stadium.AdvanceScoreboardInning();
	}

	public void DoLightning(bool showTeam)
	{
		stadium.AnimateLightning(showTeam);
	}

	public void SetStorm(bool storm, bool animate)
	{
		stadium.SetStorm(storm, animate);
	}

	public void SetTeamStare()
	{
		stadium.SetTeamStare(stare: true);
		stadium.baseballHolder.gameObject.SetActive(value: false);
		base.rowEnt0.playerParticle.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
	}

	public void ShowPreStorm()
	{
		stadium.SetStorm(storm: true, animate: false, preStorm: true);
	}

	public void ShowEdega(bool show)
	{
		ward.ToggleEdega(show);
	}
}
