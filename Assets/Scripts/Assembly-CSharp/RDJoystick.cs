using UnityEngine;

public class RDJoystick : MonoBehaviour
{
	public class AxisControl
	{
		private const float deadCenter = 0.01f;

		public bool deadSignal;

		public bool dead = true;

		private string axisName;

		private float axis => Input.GetAxis(axisName);

		public bool negativeTrigger
		{
			get
			{
				if (axis < -0.01f && dead)
				{
					deadSignal = true;
					return true;
				}
				return false;
			}
		}

		public bool positiveTrigger
		{
			get
			{
				if (axis > 0.01f && dead)
				{
					deadSignal = true;
					return true;
				}
				return false;
			}
		}

		public AxisControl(bool horizontalAxis, int schemeIndex)
		{
			string text = "Win";
			string text2 = (horizontalAxis ? "Horizontal" : "Vertical");
			axisName = "JoyCon" + ((schemeIndex == 0) ? "L" : "R") + text2 + text;
		}

		public void Update()
		{
			if (!dead && Mathf.Abs(axis) < 0.01f)
			{
				dead = true;
			}
		}

		public void LateUpdate()
		{
			if (deadSignal)
			{
				dead = false;
				deadSignal = false;
			}
		}

		public override string ToString()
		{
			return axisName + ": " + axis;
		}
	}

	private static RDJoystick _instance;

	private AxisControl[] xAxisControl;

	private AxisControl[] yAxisControl;

	private KeyCode[] validPlayer1EnterButtons = new KeyCode[6]
	{
		KeyCode.Joystick1Button0,
		KeyCode.Joystick1Button1,
		KeyCode.Joystick1Button2,
		KeyCode.Joystick1Button3,
		KeyCode.Joystick1Button14,
		KeyCode.Joystick1Button15
	};

	private KeyCode[] validPlayer2EnterButtons = new KeyCode[6]
	{
		KeyCode.Joystick2Button0,
		KeyCode.Joystick2Button1,
		KeyCode.Joystick2Button2,
		KeyCode.Joystick2Button3,
		KeyCode.Joystick2Button14,
		KeyCode.Joystick2Button15
	};

	private KeyCode[] validPlayer1CancelButtons = new KeyCode[4]
	{
		KeyCode.Joystick1Button8,
		KeyCode.Joystick1Button9,
		KeyCode.Joystick1Button12,
		KeyCode.Joystick1Button13
	};

	private KeyCode[] validPlayer2CancelButtons = new KeyCode[4]
	{
		KeyCode.Joystick2Button8,
		KeyCode.Joystick2Button9,
		KeyCode.Joystick2Button12,
		KeyCode.Joystick2Button13
	};

	public static RDJoystick instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new GameObject("RDJoystick").AddComponent<RDJoystick>();
				_instance.Setup();
			}
			return _instance;
		}
	}

	public bool axisXIsInverted => PlayerPrefs.GetInt("JoyconXFlip", 1) == -1;

	public bool player1Left => xAxisControl[0].negativeTrigger;

	public bool player1Right => xAxisControl[0].positiveTrigger;

	public bool player1Down => yAxisControl[0].negativeTrigger;

	public bool player1Up => yAxisControl[0].positiveTrigger;

	public bool player2Left => xAxisControl[1].negativeTrigger;

	public bool player2Right => xAxisControl[1].positiveTrigger;

	public bool player2Down => yAxisControl[1].negativeTrigger;

	public bool player2Up => yAxisControl[1].positiveTrigger;

	public bool player1Enter => RDInput.CheckForStateInKeys(validPlayer1EnterButtons, RDButtonState.IsDown);

	public bool player2Enter => RDInput.CheckForStateInKeys(validPlayer2EnterButtons, RDButtonState.IsDown);

	public bool player1EnterDown => RDInput.CheckForStateInKeys(validPlayer1EnterButtons, RDButtonState.WentDown);

	public bool player2EnterDown => RDInput.CheckForStateInKeys(validPlayer2EnterButtons, RDButtonState.WentDown);

	public bool player1EnterUp => RDInput.CheckForStateInKeys(validPlayer1EnterButtons, RDButtonState.WentUp);

	public bool player2EnterUp => RDInput.CheckForStateInKeys(validPlayer2EnterButtons, RDButtonState.WentUp);

	public bool player1CancelDown => RDInput.CheckForStateInKeys(validPlayer1CancelButtons, RDButtonState.WentDown);

	public bool player2CancelDown => RDInput.CheckForStateInKeys(validPlayer2CancelButtons, RDButtonState.WentDown);

	private void Setup()
	{
		Input.GetJoystickNames();
		xAxisControl = new AxisControl[2];
		xAxisControl[0] = new AxisControl(horizontalAxis: true, 0);
		xAxisControl[1] = new AxisControl(horizontalAxis: true, 1);
		yAxisControl = new AxisControl[2];
		yAxisControl[0] = new AxisControl(horizontalAxis: false, 0);
		yAxisControl[1] = new AxisControl(horizontalAxis: false, 1);
	}

	private void Update()
	{
		xAxisControl[0].Update();
		xAxisControl[1].Update();
		yAxisControl[0].Update();
		yAxisControl[1].Update();
	}

	private void LateUpdate()
	{
		xAxisControl[0].LateUpdate();
		xAxisControl[1].LateUpdate();
		yAxisControl[0].LateUpdate();
		yAxisControl[1].LateUpdate();
	}
}
