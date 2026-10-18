using RDLevelEditor;
using UnityEngine;

public class RDInputType_Keyboard : RDInputType
{
	private static readonly KeyCode[] MainScheme2 = new KeyCode[17]
	{
		KeyCode.Q,
		KeyCode.W,
		KeyCode.E,
		KeyCode.R,
		KeyCode.T,
		KeyCode.CapsLock,
		KeyCode.A,
		KeyCode.S,
		KeyCode.D,
		KeyCode.F,
		KeyCode.G,
		KeyCode.LeftShift,
		KeyCode.Z,
		KeyCode.X,
		KeyCode.C,
		KeyCode.V,
		KeyCode.B
	};

	private static readonly KeyCode[] MainScheme1 = new KeyCode[21]
	{
		KeyCode.Y,
		KeyCode.U,
		KeyCode.I,
		KeyCode.O,
		KeyCode.P,
		KeyCode.LeftBracket,
		KeyCode.RightBracket,
		KeyCode.H,
		KeyCode.J,
		KeyCode.K,
		KeyCode.L,
		KeyCode.Semicolon,
		KeyCode.Quote,
		KeyCode.N,
		KeyCode.M,
		KeyCode.Comma,
		KeyCode.Period,
		KeyCode.Slash,
		KeyCode.RightShift,
		KeyCode.Return,
		KeyCode.Space
	};

	private static readonly KeyCode[] FinerControlKeys = new KeyCode[2]
	{
		KeyCode.LeftShift,
		KeyCode.RightShift
	};

	protected static readonly KeyCode[] EditorKeys = new KeyCode[7]
	{
		KeyCode.F,
		KeyCode.A,
		KeyCode.M,
		KeyCode.N,
		KeyCode.O,
		KeyCode.S,
		KeyCode.P
	};

	protected KeyCode[] currentMainKeys
	{
		get
		{
			if (schemeIndex != 0)
			{
				return MainScheme2;
			}
			return MainScheme1;
		}
	}

	protected KeyCode currentMouseKey
	{
		get
		{
			if (schemeIndex != 0)
			{
				return KeyCode.Mouse1;
			}
			return KeyCode.Mouse0;
		}
	}

	protected bool mouseButtonsAvailable
	{
		get
		{
			scnGame instance = scnGame.instance;
			bool num = instance != null;
			scnEditor instance2 = scnEditor.instance;
			bool flag = instance2 != null;
			scnLevelSelect instance3 = scnLevelSelect.instance;
			bool flag2 = instance3 != null;
			if ((!num || flag || instance.pauseMenu.showing) && (!flag || !(instance2.inkEditor != null) || !instance2.inkEditor.visible) && (!flag2 || !(instance3.rdInk != null) || !instance3.rdInk.visible))
			{
				if (scnCalibration.instance != null)
				{
					return scnCalibration.instance.page == scnCalibration.CalibrationPage.Input;
				}
				return false;
			}
			return true;
		}
	}

	public RDInputType_Keyboard(int schemeIndex)
		: base(schemeIndex)
	{
	}

	public override bool Main(RDButtonState state)
	{
		bool flag = RDInput.CheckForStateInKey(currentMouseKey, state) && mouseButtonsAvailable;
		if (scnEditor.instance != null && RDInput.CheckForStateInKeys(EditorKeys, state))
		{
			return false;
		}
		return RDInput.CheckForStateInKeys(currentMainKeys, state) | flag;
	}

	public override bool Cancel(RDButtonState state)
	{
		return RDInput.CheckForStateInKey(KeyCode.Escape, state);
	}

	public override bool Skip()
	{
		return RDInput.CheckForStateInKey(KeyCode.S, RDButtonState.WentDown);
	}

	public override bool Restart()
	{
		return RDInput.CheckForStateInKey(KeyCode.R, RDButtonState.WentDown);
	}

	public override bool Quit()
	{
		return RDInput.CheckForStateInKey(KeyCode.Q, RDButtonState.WentDown);
	}

	public override bool Right(RDButtonState state)
	{
		return RDInput.CheckForStateInKey(KeyCode.RightArrow, state);
	}

	public override bool Up(RDButtonState state)
	{
		return RDInput.CheckForStateInKey(KeyCode.UpArrow, state);
	}

	public override bool Left(RDButtonState state)
	{
		return RDInput.CheckForStateInKey(KeyCode.LeftArrow, state);
	}

	public override bool Down(RDButtonState state)
	{
		return RDInput.CheckForStateInKey(KeyCode.DownArrow, state);
	}

	public override bool FinerControl(RDButtonState state)
	{
		return RDInput.CheckForStateInKeys(FinerControlKeys, state);
	}

	public override bool FaceLeft(RDButtonState state)
	{
		return RDInput.CheckForStateInKey(KeyCode.A, state);
	}

	public override bool FaceUp(RDButtonState state)
	{
		return false;
	}

	public override bool Select(RDButtonState state)
	{
		return false;
	}
}
