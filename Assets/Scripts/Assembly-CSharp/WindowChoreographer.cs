using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class WindowChoreographer : RDClass
{
	public enum State
	{
		Setup = 0,
		Preparing = 1,
		Ready = 2
	}

	public WindowDancer[] dancers = new WindowDancer[0];

	public int[] zOrder = new int[4] { 0, 1, 2, 3 };

	public bool enabled = true;

	public State state;

	public bool setup;

	public bool changingWindowSize;

	protected bool preparedFirstTime;

	public static readonly Vector2Int MinimumViewSize = Vector2Int.one * 2;

	public static UnityPlayerWindow playerWindow => PlatformHelper.instance.playerWindow;

	public abstract Vector2Int WindowDanceResolution { get; }

	public abstract IEnumerator Setup(int dancerCount);

	public virtual void SetWindowContentToRoom(int windowIndex, int roomIndex)
	{
		WindowDancer obj = dancers[windowIndex];
		obj.roomIndex = roomIndex;
		obj.window.material.SetColor("_BackgroundColor", RDClass.Vfx.GetCamera(roomIndex).camera.backgroundColor);
		UpdateRoomFollowing();
	}

	public virtual void LateUpdate()
	{
		if (!enabled || !setup)
		{
			return;
		}
		scnGame instance = scnGame.instance;
		bool flag = ((!(base.editor != null)) ? instance.paused : (instance.paused || !instance.levelFinishedLoading));
		if (preparedFirstTime && instance.startTheGameCalled && !flag)
		{
			WindowDancer[] array = dancers;
			foreach (WindowDancer windowDancer in array)
			{
				if (windowDancer.enabled)
				{
					windowDancer.LateUpdate();
				}
			}
			RDRoom[] rooms = instance.rooms;
			for (int i = 0; i < rooms.Length; i++)
			{
				rooms[i].PeekWindowModeUpdate();
			}
			UpdateWindowZOrders();
		}
		if (!instance.paused && !changingWindowSize)
		{
			bool flag2 = preparedFirstTime && dancers.All((WindowDancer d) => d.window.Validate()) && (this is VirtualWindowChoreographer || !Screen.fullScreen);
			if (!flag2)
			{
				Debug.Log($"conditionsForWindowDancing {flag2}");
				instance.StartCoroutine(PrepareForDancing());
			}
		}
	}

	protected virtual void UpdateWindowZOrders()
	{
		for (int num = zOrder.Length - 1; num >= 0; num--)
		{
			int num2 = zOrder[num];
			if (num2 < dancers.Length)
			{
				WindowDancer obj = dancers[num2];
				Window window = obj.window;
				if (obj.visible)
				{
					window.ShowOnTop();
				}
			}
		}
	}

	protected void CheckDancers()
	{
		WindowDancer[] array = dancers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CheckDefaultValues(!preparedFirstTime);
		}
	}

	public void StartUsingCustomWindows()
	{
		WindowDancer obj = dancers[0];
		obj.SetVisible(visible: true);
		obj.window.Reset(this);
		OnPauseChanged(paused: false);
	}

	protected abstract IEnumerator PrepareForDancing();

	public virtual void OnPauseChanged(bool paused)
	{
		WindowDancer[] array = dancers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnPauseChanged(paused);
		}
	}

	public int GetWindowScaleForDancing()
	{
		Vector2Int windowDanceResolution = WindowDanceResolution;
		float num = windowDanceResolution.x;
		int val = Math.Min(val2: Mathf.FloorToInt((float)windowDanceResolution.y / 198f * 0.5f), val1: Mathf.FloorToInt(num / 352f * 0.5f));
		bool num2 = this is VirtualWindowChoreographer;
		bool flag = RDClass.debugSettings.RunningOnSteamDeck && SteamIntegration.IsSteamInBigPictureMode;
		int val2 = ((!(!num2 | flag)) ? 1 : 2);
		return Math.Max(val, val2);
	}

	protected void ShowGlitch(bool show)
	{
		if (show)
		{
			scrConductor.PlayImmediately("sndNoiseMed", 0.5f, RDUtils.GetMixerGroup("LevelInterface"));
		}
		WindowDancer[] array = dancers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ShowGlitch(show);
		}
	}

	public void Cancel()
	{
		WindowDancer[] array = dancers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Cancel();
		}
		OnPauseChanged(paused: true);
	}

	public virtual void OnDestroy()
	{
		WindowDancer[] array = dancers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i]?.OnDestroy();
		}
	}

	public void HideWindows()
	{
		WindowDancer[] array = dancers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].window.SetCenter(WindowDanceResolution * 3);
		}
	}

	public void UpdateRoomFollowing()
	{
		RDRoom[] rooms = base.game.rooms;
		foreach (RDRoom room in rooms)
		{
			room.followedByWindow = Array.Exists(dancers, (WindowDancer d) => d.roomIndex == room.index);
		}
	}
}
