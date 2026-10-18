using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using RDTools;

public class scrExecuteOnCertainBeat : RDExecuteOn
{
	public class TweenCachedInfo
	{
		public float duration;

		public float position;
	}

	public string debugStats = "";

	public int barToExecute;

	public float timeToExecuteFromBarStart;

	public double timeToExecuteAbs;

	public bool isAudioEvent;

	public LevelEventType levelEventType;

	public static Dictionary<Tween, TweenCachedInfo> tweensStillLeftToScrub = new Dictionary<Tween, TweenCachedInfo>();

	public bool isReadyToExecute => (isAudioEvent ? base.conductor.audioPos : base.conductor.visualPos) > timeToExecuteAbs;

	public override void Update()
	{
		if (!removed && isReadyToExecute)
		{
			RunAction();
		}
	}

	public static scrExecuteOnCertainBeat Add(Action _onBeatAction, float _timeInBeats, string debugStats = null, bool minusLatency = false, bool calledCloseToNextBarMaybe = false, LevelEventType levelEventType = LevelEventType.None)
	{
		if (_timeInBeats < 0f)
		{
			_onBeatAction();
			return null;
		}
		scrConductor instance = scrConductor.instance;
		scrExecuteOnCertainBeat obj = new scrExecuteOnCertainBeat
		{
			action = _onBeatAction,
			debugStats = debugStats,
			barToExecute = scrConductor.instance.barNumber,
			isAudioEvent = minusLatency,
			timeToExecuteFromBarStart = instance.BeatToTime(_timeInBeats, calledCloseToNextBarMaybe) - (minusLatency ? RDCalibration.latency : 0f)
		};
		obj.timeToExecuteAbs = (double)obj.timeToExecuteFromBarStart + (calledCloseToNextBarMaybe ? instance.startOfNextBarRefreshOnlyOnBar : instance.startOfNextBar);
		obj.levelEventType = levelEventType;
		return obj;
	}

	public static void Clear(bool execute, string tag = null)
	{
		RDClassDll.printem("clearing");
		IOrderedEnumerable<RDExecuteOn> orderedEnumerable = from exe1 in RDExecuteOn.FindAll<scrExecuteOnCertainBeat>()
			orderby (exe1 as scrExecuteOnCertainBeat).timeToExecuteFromBarStart
			select exe1;
		if (string.IsNullOrEmpty(tag))
		{
			foreach (scrExecuteOnCertainBeat item in orderedEnumerable)
			{
				if (execute)
				{
					item.RunAction();
				}
				else
				{
					item.Remove();
				}
			}
			return;
		}
		foreach (scrExecuteOnCertainBeat item2 in orderedEnumerable)
		{
			if (item2.debugStats == tag)
			{
				if (execute)
				{
					item2.RunAction();
				}
				else
				{
					item2.Remove();
				}
			}
		}
	}

	public static void MoveBackBy(double time, bool andPlay)
	{
		float num = scrConductor.instance.BeatToTime(scrConductor.instance.crotchetsPerBar);
		Tween[] array = tweensStillLeftToScrub.Keys.ToArray();
		foreach (Tween tween in array)
		{
			TweenCachedInfo tweenCachedInfo = tweensStillLeftToScrub[tween];
			tweenCachedInfo.position += num;
			if (tweenCachedInfo.position >= tweenCachedInfo.duration)
			{
				tween.Complete(withCallbacks: true);
				tweensStillLeftToScrub.Remove(tween);
			}
			else if (andPlay)
			{
				tween.Goto(tweenCachedInfo.position, andPlay: true);
			}
		}
		if (andPlay)
		{
			tweensStillLeftToScrub.Clear();
		}
		IOrderedEnumerable<RDExecuteOn> orderedEnumerable = from exe1 in RDExecuteOn.FindAll<scrExecuteOnCertainBeat>()
			orderby (exe1 as scrExecuteOnCertainBeat).timeToExecuteAbs
			select exe1;
		IterateQuery(orderedEnumerable);
		orderedEnumerable = from exe1 in RDExecuteOn.FindAll<scrExecuteOnCertainBeat>().Except(orderedEnumerable)
			orderby (exe1 as scrExecuteOnCertainBeat).timeToExecuteAbs
			select exe1;
		IterateQuery(orderedEnumerable);
		void IterateQuery(IOrderedEnumerable<RDExecuteOn> query)
		{
			scrConductor instance = scrConductor.instance;
			List<Tween> list = DOTween.PlayingTweens();
			foreach (scrExecuteOnCertainBeat item in query)
			{
				if (!item.removed)
				{
					double num2 = item.timeToExecuteAbs - time;
					item.timeToExecuteAbs = num2;
					if (item.isReadyToExecute)
					{
						double num3 = instance.visualPos - item.timeToExecuteAbs;
						item.RunAction();
						if (item.levelEventType == LevelEventType.ShowDialogue)
						{
							RDInk.mainDialogue.FastForward(num3);
						}
						else
						{
							List<Tween> list2 = DOTween.PlayingTweens();
							if (list2 != null)
							{
								IEnumerable<Tween> enumerable2;
								if (list == null)
								{
									IEnumerable<Tween> enumerable = list2;
									enumerable2 = enumerable;
								}
								else
								{
									enumerable2 = list2.Except(list);
								}
								foreach (Tween item2 in enumerable2)
								{
									TweenCachedInfo tweenCachedInfo2 = new TweenCachedInfo
									{
										position = (float)num3,
										duration = item2.Duration()
									};
									if (tweenCachedInfo2.position >= tweenCachedInfo2.duration)
									{
										item2.Complete(withCallbacks: true);
									}
									else if (andPlay)
									{
										item2.Goto(tweenCachedInfo2.position, andPlay: true);
									}
									else
									{
										tweensStillLeftToScrub.TryAdd(item2, tweenCachedInfo2);
									}
								}
							}
						}
					}
					if (item.conductor.barNumber > item.barToExecute)
					{
						item.RunAction();
					}
				}
			}
		}
	}

	public void RunAction()
	{
		if (!removed && enabled)
		{
			enabled = false;
			action();
			Remove();
		}
	}
}
