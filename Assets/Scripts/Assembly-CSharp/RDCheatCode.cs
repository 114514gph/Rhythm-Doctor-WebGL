using Rewired;
using UnityEngine;

public class RDCheatCode : RDBase
{
	public class CheatCode
	{
		private readonly KeyCode[] keys;

		private readonly RDInputAction[] actions;

		private readonly bool usingKeys;

		public int currentIndex;

		private int length
		{
			get
			{
				if (actions.Length != 0)
				{
					return actions.Length;
				}
				return keys.Length;
			}
		}

		private bool completed => currentIndex == length;

		public CheatCode(KeyCode[] _cheatCode)
		{
			keys = _cheatCode;
			actions = new RDInputAction[0];
			usingKeys = true;
			currentIndex = 0;
		}

		public CheatCode(RDInputAction[] _cheatCode)
		{
			actions = _cheatCode;
			keys = new KeyCode[0];
			usingKeys = false;
			currentIndex = 0;
		}

		public bool CheckCheatCode()
		{
			if (completed)
			{
				currentIndex = 0;
			}
			bool flag = false;
			if (!((actions.Length == 0) ? Input.anyKeyDown : ReInput.controllers.GetAnyButtonDown()))
			{
				return completed;
			}
			if ((!usingKeys) ? ActionPressed(actions[currentIndex]) : Input.GetKeyDown(keys[currentIndex]))
			{
				currentIndex++;
			}
			else
			{
				currentIndex = 0;
			}
			return completed;
		}

		private bool ActionPressed(RDInputAction inputAction)
		{
			switch (inputAction)
			{
			case RDInputAction.Cancel:
				return RDInput.cancelPress;
			case RDInputAction.Main:
				return RDInput.anyPlayerPress;
			case RDInputAction.Down:
				return RDInput.downPress;
			case RDInputAction.Left:
				return RDInput.leftPress;
			case RDInputAction.Right:
				return RDInput.rightPress;
			case RDInputAction.Up:
				return RDInput.upPress;
			case RDInputAction.Quit:
				return RDInput.quitPressed;
			case RDInputAction.Restart:
				return RDInput.restartPressed;
			case RDInputAction.Skip:
				return RDInput.skipPressed;
			default:
				Debug.Log($"Cheatcode action not found: {inputAction}");
				return false;
			}
		}
	}

	private CheatCode levelLounge;

	private void Start()
	{
		RDInputAction rDInputAction = RDInputAction.Left;
		RDInputAction rDInputAction2 = RDInputAction.Right;
		RDInputAction[] cheatCode = new RDInputAction[16]
		{
			rDInputAction2, rDInputAction, rDInputAction2, rDInputAction2, rDInputAction, rDInputAction2, rDInputAction, rDInputAction, rDInputAction2, rDInputAction,
			rDInputAction2, rDInputAction2, rDInputAction, rDInputAction2, rDInputAction, rDInputAction
		};
		levelLounge = new CheatCode(cheatCode);
	}

	private void LateUpdate()
	{
		if (!levelLounge.CheckCheatCode())
		{
			return;
		}
		printe("Go to Rhythm Dogtor");
		if (base.scnCurrent is scnLevelSelect scnLevelSelect2)
		{
			if (Persistence.GetStateFromLevel(Level.Lesmis) != LevelState.NotAvailable)
			{
				scnGame.loadDogMode = true;
				Persistence.UnlockAchievement(Achievement.RhythmDogtor, storeStats: true);
				scnGame.levelSpeed = 1f;
				scnLevelSelect2.GoToLevelSequence(Level.Lesmis.ToString());
			}
		}
		else if (base.scnCurrent.GetType().Name == "scnRhythmWeightlifter")
		{
			// WebGL: skip dog mode toggle
		}
	}
}
