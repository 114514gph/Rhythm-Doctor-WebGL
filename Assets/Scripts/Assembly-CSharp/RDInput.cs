using System.Collections.Generic;
using System.Linq;
using RDLevelEditor;
using Rewired;
using UnityEngine;

public static class RDInput
{
	public enum PlayerEmuKey
	{
		Down = 0,
		IsDown = 1,
		Up = 2,
		IsUp = 3,
		Null = 4
	}

	public class PlayerEmuState
	{
		private int index;

		private PlayerEmuKey status;

		private PlayerEmuKey preCache;

		private int lastFrameUpdated;

		public PlayerEmuState(int index)
		{
			preCache = (status = PlayerEmuKey.Up);
			lastFrameUpdated = -1;
			this.index = index;
		}

		public bool GetKey(PlayerEmuKey keyType)
		{
			if (Time.frameCount > lastFrameUpdated)
			{
				if (status == PlayerEmuKey.Down)
				{
					status = PlayerEmuKey.IsDown;
				}
				else if (status == PlayerEmuKey.Up || scnGame.tapsOnly)
				{
					status = PlayerEmuKey.IsUp;
				}
			}
			return status == keyType;
		}

		public void SetKey(PlayerEmuKey key)
		{
			preCache = key;
			lastFrameUpdated = Time.frameCount + 1;
		}

		public void LateUpdate()
		{
			if (preCache != PlayerEmuKey.Null)
			{
				status = preCache;
				preCache = PlayerEmuKey.Null;
			}
		}
	}

	public static List<RDInputType> allInputs;

	public static List<RDInputType> p1;

	public static List<RDInputType> p2;

	public static RDInputType_Touch touch;

	public static int touchBlockedFrame = -1;

	private static bool didSetup = false;

	public static bool p1Press;

	public static bool p1IsPressed;

	public static bool p1Release;

	public static float p1HoldTime = 0f;

	public static bool p2Press;

	public static bool p2IsPressed;

	public static bool p2Release;

	public static float p2HoldTime = 0f;

	private const float tHold = 0.25f;

	public static bool anyPlayerPress;

	public static bool anyPlayerIsPressed;

	public static bool anyPlayerRelease;

	public static bool skipPressed;

	public static bool restartPressed;

	public static bool cancelPress;

	public static bool cancelIsPressed;

	public static bool cancelRelease;

	public static bool quitPressed;

	public static bool leftPress;

	public static bool leftIsPressed;

	public static bool leftRelease;

	public static bool rightPress;

	public static bool rightIsPressed;

	public static bool rightRelease;

	public static bool upPress;

	public static bool upIsPressed;

	public static bool upRelease;

	public static bool downPress;

	public static bool downIsPressed;

	public static bool downRelease;

	public static bool finerControlPress;

	public static bool finerControlIsPressed;

	public static bool finerControlRelease;

	public static bool faceLeftPress;

	public static bool faceLeftIsPressed;

	public static bool faceLeftRelease;

	public static bool faceUpPress;

	public static bool faceUpIsPressed;

	public static bool faceUpRelease;

	public static bool selectPress;

	public static bool selectIsPressed;

	public static bool selectRelease;

	public static Vector2 position;

	public static PlayerEmuState[] emuStates;

	public static int lastUpdateFrame { get; private set; } = -1;

	public static bool key1 => Input.GetKeyDown(KeyCode.Alpha1);

	public static bool key2 => Input.GetKeyDown(KeyCode.Alpha2);

	public static bool key3 => Input.GetKeyDown(KeyCode.Alpha3);

	public static bool persistenceSwapped => Persistence.GetSwapP1AndP2Controls();

	public static bool actuallySwapped => p1[0].schemeIndex == 1;

	private static int p1SetupIndex
	{
		get
		{
			if (!persistenceSwapped)
			{
				return 0;
			}
			return 1;
		}
	}

	private static int p2SetupIndex
	{
		get
		{
			if (!persistenceSwapped)
			{
				return 1;
			}
			return 0;
		}
	}

	public static bool touchBlocked
	{
		get
		{
			return touchBlockedFrame == Time.frameCount;
		}
		set
		{
			touchBlockedFrame = (value ? Time.frameCount : (-1));
		}
	}

	public static bool editorMode => scnEditor.instance != null;

	public static bool anyKeyDown
	{
		get
		{
			if (Input.anyKeyDown)
			{
				if (!Input.GetKey(KeyCode.F1) && !Input.GetKey(KeyCode.F2) && !Input.GetKey(KeyCode.F3) && !Input.GetKey(KeyCode.F4) && !Input.GetKey(KeyCode.F5) && !Input.GetKey(KeyCode.F6) && !Input.GetKey(KeyCode.F7) && !Input.GetKey(KeyCode.F8) && !Input.GetKey(KeyCode.F9) && !Input.GetKey(KeyCode.F10) && !Input.GetKey(KeyCode.F11))
				{
					return !Input.GetKey(KeyCode.F12);
				}
				return false;
			}
			return false;
		}
	}

	public static bool controlIsPressed
	{
		get
		{
			if ((Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXPlayer) || Application.isEditor)
			{
				if (!Input.GetKey(KeyCode.LeftControl))
				{
					return Input.GetKey(KeyCode.RightControl);
				}
				return true;
			}
			if (!Input.GetKey(KeyCode.LeftMeta))
			{
				return Input.GetKey(KeyCode.RightMeta);
			}
			return true;
		}
	}

	public static void Setup()
	{
		if (!didSetup)
		{
			didSetup = true;
#if !UNITY_WEBGL
			ReInput.ControllerConnectedEvent += OnControllerConnected;
			ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
#endif
			p1 = new List<RDInputType>();
			p2 = new List<RDInputType>();
			if (scnBase.isSwitch)
			{
				p1.Add(new RDInputType_Keyboard(p1SetupIndex));
				p2.Add(new RDInputType_Keyboard(p2SetupIndex));
			}
			else if (scnBase.isCustomButton)
			{
				p1.Add(new RDInputType_CustomButton(p1SetupIndex));
				p2.Add(new RDInputType_CustomButton(p2SetupIndex));
			}
			else
			{
				p1.Add(new RDInputType_Joystick(p1SetupIndex));
				p2.Add(new RDInputType_Joystick(p2SetupIndex));
			}
			p1.Add(new RDInputType_Keyboard(p1SetupIndex));
			p2.Add(new RDInputType_Keyboard(p2SetupIndex));
			allInputs = new List<RDInputType>();
			allInputs.AddRange(p1);
			allInputs.AddRange(p2);
			emuStates = new PlayerEmuState[2];
			for (int i = 0; i < emuStates.Length; i++)
			{
				emuStates[i] = new PlayerEmuState(i);
			}
			SetPlayerSoundPanning();
		}
	}

#if !UNITY_WEBGL
	private static void OnControllerConnected(ControllerStatusChangedEventArgs args)
	{
		if (args.controllerType != Rewired.ControllerType.Joystick)
		{
			return;
		}
		Player player = ReInput.players.GetPlayer(0);
		Player player2 = ReInput.players.GetPlayer(1);
		if (ReInput.controllers.joystickCount == 1)
		{
			player.controllers.AddController(args.controller, removeFromOtherPlayers: false);
			player2.controllers.AddController(args.controller, removeFromOtherPlayers: false);
		}
		else
		{
			foreach (Joystick joystick in ReInput.controllers.Joysticks)
			{
				if (player.controllers.Joysticks.Contains(joystick) && player2.controllers.Joysticks.Contains(joystick))
				{
					player2.controllers.RemoveController(joystick);
				}
			}
			if (player.controllers.joystickCount <= player2.controllers.joystickCount)
			{
				player.controllers.AddController(args.controller, removeFromOtherPlayers: true);
			}
			else
			{
				player2.controllers.AddController(args.controller, removeFromOtherPlayers: true);
			}
		}
		bool flag = ReInput.controllers.joystickCount == 1;
		player.controllers.maps.SetMapsEnabled(!flag, Rewired.ControllerType.Joystick, 0, 0);
		player2.controllers.maps.SetMapsEnabled(!flag, Rewired.ControllerType.Joystick, 0, 0);
		player.controllers.maps.SetMapsEnabled(flag, Rewired.ControllerType.Joystick, 0, 2);
		player2.controllers.maps.SetMapsEnabled(flag, Rewired.ControllerType.Joystick, 0, 1);
	}

#endif

#if !UNITY_WEBGL
	private static void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
	{
		if (args.controllerType != Rewired.ControllerType.Joystick)
		{
			return;
		}
		Player player = ReInput.players.GetPlayer(0);
		Player player2 = ReInput.players.GetPlayer(1);
		if (ReInput.controllers.joystickCount != 0)
		{
			if (ReInput.controllers.joystickCount == 1)
			{
				Joystick controller = ReInput.controllers.Joysticks.Single();
				player.controllers.AddController(controller, removeFromOtherPlayers: false);
				player2.controllers.AddController(controller, removeFromOtherPlayers: false);
			}
			else if (player.controllers.joystickCount == 0)
			{
				Player.ControllerHelper controllers = player.controllers;
				IList<Joystick> joysticks = ReInput.controllers.Joysticks;
				controllers.AddController(joysticks[joysticks.Count - 1], removeFromOtherPlayers: true);
			}
			else if (player2.controllers.joystickCount == 0)
			{
				Player.ControllerHelper controllers2 = player2.controllers;
				IList<Joystick> joysticks2 = ReInput.controllers.Joysticks;
				controllers2.AddController(joysticks2[joysticks2.Count - 1], removeFromOtherPlayers: true);
			}
		}
		bool flag = ReInput.controllers.joystickCount == 1;
		player.controllers.maps.SetMapsEnabled(!flag, Rewired.ControllerType.Joystick, 0, 0);
		player2.controllers.maps.SetMapsEnabled(!flag, Rewired.ControllerType.Joystick, 0, 0);
		player.controllers.maps.SetMapsEnabled(flag, Rewired.ControllerType.Joystick, 0, 2);
		player2.controllers.maps.SetMapsEnabled(flag, Rewired.ControllerType.Joystick, 0, 1);
	}
#endif

	public static void Update()
	{
		lastUpdateFrame = Time.frameCount;
		scnGame instance = scnGame.instance;
		if ((object)instance != null && instance.windowChoreographer is RealWindowChoreographer realWindowChoreographer)
		{
			realWindowChoreographer.UpdateWindowInputs();
		}
		KeyHelper.UpdateCurrentInfo(ReInput.controllers.GetLastActiveController());
		if (scnGame.tapsOnly)
		{
			p1Press = p1.Any((RDInputType input) => input.Main(RDButtonState.WentDown)) || emuStates[0].GetKey(PlayerEmuKey.Down) || emuStates[0].GetKey(PlayerEmuKey.Up);
			p1HoldTime = (p1Press ? 0.25f : Mathf.Max(p1HoldTime - RDTime.deltaTimeIndependentPauseAffected, 0f));
			p1IsPressed = p1HoldTime > 0f;
			p1Release = false;
			p2Press = p2.Any((RDInputType input) => input.Main(RDButtonState.WentDown)) || emuStates[1].GetKey(PlayerEmuKey.Down) || emuStates[1].GetKey(PlayerEmuKey.Up);
			p2HoldTime = (p2Press ? 0.25f : Mathf.Max(p2HoldTime - RDTime.deltaTimeIndependentPauseAffected, 0f));
			p2IsPressed = p2HoldTime > 0f;
			p2Release = false;
		}
		else
		{
			p1Press = p1.Any((RDInputType input) => input.Main(RDButtonState.WentDown)) || emuStates[0].GetKey(PlayerEmuKey.Down);
			p1IsPressed = p1.Any((RDInputType input) => input.Main(RDButtonState.IsDown)) || emuStates[0].GetKey(PlayerEmuKey.IsDown);
			p1Release = p1.Any((RDInputType input) => input.Main(RDButtonState.WentUp)) || emuStates[0].GetKey(PlayerEmuKey.Up);
			p2Press = p2.Any((RDInputType input) => input.Main(RDButtonState.WentDown)) || emuStates[1].GetKey(PlayerEmuKey.Down);
			p2IsPressed = p2.Any((RDInputType input) => input.Main(RDButtonState.IsDown)) || emuStates[1].GetKey(PlayerEmuKey.IsDown);
			p2Release = p2.Any((RDInputType input) => input.Main(RDButtonState.WentUp)) || emuStates[1].GetKey(PlayerEmuKey.Up);
		}
		anyPlayerPress = p1Press || p2Press;
		anyPlayerIsPressed = p1IsPressed || p2IsPressed;
		anyPlayerRelease = p1Release || p2Release;
		skipPressed = GetState(RDInputAction.Skip, RDButtonState.WentDown);
		restartPressed = GetState(RDInputAction.Restart, RDButtonState.WentDown);
		quitPressed = GetState(RDInputAction.Quit, RDButtonState.WentDown);
		cancelPress = GetState(RDInputAction.Cancel, RDButtonState.WentDown);
		cancelIsPressed = GetState(RDInputAction.Cancel, RDButtonState.IsDown);
		cancelRelease = GetState(RDInputAction.Cancel, RDButtonState.WentUp);
		leftPress = GetState(RDInputAction.Left, RDButtonState.WentDown);
		leftIsPressed = GetState(RDInputAction.Left, RDButtonState.IsDown);
		leftRelease = GetState(RDInputAction.Left, RDButtonState.WentUp);
		rightPress = GetState(RDInputAction.Right, RDButtonState.WentDown);
		rightIsPressed = GetState(RDInputAction.Right, RDButtonState.IsDown);
		rightRelease = GetState(RDInputAction.Right, RDButtonState.WentUp);
		upPress = GetState(RDInputAction.Up, RDButtonState.WentDown);
		upIsPressed = GetState(RDInputAction.Up, RDButtonState.IsDown);
		upRelease = GetState(RDInputAction.Up, RDButtonState.WentUp);
		downPress = GetState(RDInputAction.Down, RDButtonState.WentDown);
		downIsPressed = GetState(RDInputAction.Down, RDButtonState.IsDown);
		downRelease = GetState(RDInputAction.Down, RDButtonState.WentUp);
		finerControlPress = GetState(RDInputAction.FinerControl, RDButtonState.WentDown);
		finerControlIsPressed = GetState(RDInputAction.FinerControl, RDButtonState.IsDown);
		finerControlRelease = GetState(RDInputAction.FinerControl, RDButtonState.WentUp);
		faceLeftPress = GetState(RDInputAction.FaceLeft, RDButtonState.WentDown);
		faceLeftIsPressed = GetState(RDInputAction.FaceLeft, RDButtonState.IsDown);
		faceLeftRelease = GetState(RDInputAction.FaceLeft, RDButtonState.WentUp);
		faceUpPress = GetState(RDInputAction.FaceUp, RDButtonState.WentDown);
		faceUpIsPressed = GetState(RDInputAction.FaceUp, RDButtonState.IsDown);
		faceUpRelease = GetState(RDInputAction.FaceUp, RDButtonState.WentUp);
		selectPress = GetState(RDInputAction.Select, RDButtonState.WentDown);
		selectIsPressed = GetState(RDInputAction.Select, RDButtonState.IsDown);
		selectRelease = GetState(RDInputAction.Select, RDButtonState.WentUp);
		position = GetPosition();
	}

	public static void LateUpdate()
	{
		PlayerEmuState[] array = emuStates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].LateUpdate();
		}
	}

	public static void Reinitialize()
	{
		didSetup = false;
		Setup();
	}

	private static bool GetState(RDInputAction inputAction, RDButtonState state)
	{
		foreach (RDInputType allInput in allInputs)
		{
			if (allInput.Get(inputAction, state))
			{
				return true;
			}
		}
		return false;
	}

	private static Vector2 GetPosition()
	{
		Vector2 zero = Vector2.zero;
		foreach (RDInputType allInput in allInputs)
		{
			zero += allInput.GetPosition();
		}
		return zero;
	}

	public static bool CheckForStateInKeys(KeyCode[] keys, RDButtonState state)
	{
		for (int i = 0; i < keys.Length; i++)
		{
			if (CheckForStateInKey(keys[i], state))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckForStateInKey(KeyCode key, RDButtonState state)
	{
		bool flag = state switch
		{
			RDButtonState.WentDown => Input.GetKeyDown(key), 
			RDButtonState.WentUp => Input.GetKeyUp(key), 
			RDButtonState.IsDown => Input.GetKey(key), 
			_ => false, 
		};
		scnGame instance = scnGame.instance;
		if ((object)instance != null && instance.windowChoreographer is RealWindowChoreographer realWindowChoreographer)
		{
			realWindowChoreographer.CheckInputState(key, state);
		}
		return flag;
	}

	public static void SwapP1AndP2Controls(bool changePersistence)
	{
		List<RDInputType> list = p2;
		List<RDInputType> list2 = p1;
		p1 = list;
		p2 = list2;
		if (changePersistence)
		{
			scrConductor.PlayImmediately("sndPagerCursorMove", 1f, RDUtils.GetMixerGroup("LevelInterface"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		}
		SetPlayerSoundPanning();
		if (scnGame.instance != null)
		{
			scnGame.instance.SwitchPlayers();
			scnGame.instance.LoadPlayerHandPopSounds();
		}
		if (changePersistence)
		{
			Persistence.SetSwapP1AndP2Controls(actuallySwapped);
			string text = RDString.Get("narration.gameplay.player1");
			string text2 = RDString.Get("narration.gameplay.player2");
			string newValue = (actuallySwapped ? text : text2);
			string newValue2 = (actuallySwapped ? text2 : text);
			string text3 = RDString.Get("narration.gameplay.playerPositions");
			text3 = text3.Replace("[playerRight]", newValue);
			text3 = text3.Replace("[playerLeft]", newValue2);
			if (GC.twoPlayerMode)
			{
				Narration.Say(text3, NarrationCategory.Navigation, false);
			}
		}
	}

	public static void SetPlayerSoundPanning()
	{
		GC.TWO_PLAYER_PANNING_STRENGTH = (float)Persistence.GetPanningStrength2P() * 0.01f;
		GC.PanP1 = ((p1[0].schemeIndex == 0) ? GC.TWO_PLAYER_PANNING_STRENGTH : (GC.TWO_PLAYER_PANNING_STRENGTH * -1f));
		GC.PanP2 = ((p2[0].schemeIndex == 0) ? GC.TWO_PLAYER_PANNING_STRENGTH : (GC.TWO_PLAYER_PANNING_STRENGTH * -1f));
	}
}
