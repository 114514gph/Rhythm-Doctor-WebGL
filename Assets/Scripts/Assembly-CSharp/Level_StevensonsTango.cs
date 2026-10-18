using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_StevensonsTango : LevelBase
{
	private RDAthleteWard _physioWard;

	private RDAthleteWard physioWard
	{
		get
		{
			if (_physioWard == null)
			{
				_physioWard = base.room0.athleteWard.GetComponent<RDAthleteWard>();
			}
			return _physioWard;
		}
	}

	public Level_StevensonsTango(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
	}

	public override void preactions()
	{
	}

	public override void actions()
	{
	}

	public void MoveBackground(float posX, float durationInBeats, string ease)
	{
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats, calledCloseToNextBarMaybe: true);
		Ease ease2 = RDUtils.ParseEnum(ease, Ease.Linear);
		physioWard.transform.DOLocalMoveX(posX, duration).SetEase(ease2);
	}

	public void ToggleForeground(bool toggled)
	{
		physioWard.physioFg.GetComponent<MeshRenderer>().enabled = toggled;
	}
}
