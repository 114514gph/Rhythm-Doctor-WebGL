using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiWindow;
using UnityEngine;

public class RealWindowChoreographer : WindowChoreographer
{
	public readonly bool mainWindowIsDancer;

	public static float WindowedSwitchTime
	{
		get
		{
			if (!scnBase.isMac)
			{
				return 0.2f;
			}
			return 1f;
		}
	}

	public override Vector2Int WindowDanceResolution => base.platformHelper.WindowDanceResolution;

	public RealWindowChoreographer(bool mainWindowIsDancer)
	{
		this.mainWindowIsDancer = mainWindowIsDancer;
		printe("mainWindowIsDancer: " + mainWindowIsDancer);
		if (!Persistence.GetHasWindowDanceChangedResolution())
		{
			Persistence.SaveWindowState();
		}
	}

	public override IEnumerator Setup(int dancerCount)
	{
		dancers = new WindowDancer[dancerCount];
		for (int i = 0; i < dancerCount; i++)
		{
			Window window = ((i == 0 && mainWindowIsDancer) ? ((RealWindow)new UnityPlayerWindowWindows(i, this)) : ((RealWindow)new CustomWindowWindows(i, this, transparent: true)));
			if (window == null)
			{
				Debug.LogError($"Window {i} was not initialized, it's null");
			}
			dancers[i] = new WindowDancer(window, this);
			if (RDClass.debugSettings.Debug || Debug.isDebugBuild || Application.isEditor)
			{
				window.SetTitle("Window " + i);
			}
		}
		yield return MinimizeIfFullscreen();
		if (!mainWindowIsDancer)
		{
			if (!base.editorMode)
			{
				WindowChoreographer.playerWindow.Reset(this);
			}
			if (RDClass.debugSettings.Debug || Debug.isDebugBuild)
			{
				WindowChoreographer.playerWindow.SetTitle("Unity Player Window " + DateTime.Now.ToShortTimeString());
			}
		}
		WindowDancer[] array = dancers;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].window.ResetSize(this);
		}
		if (dancers.Any((WindowDancer dancer) => dancer.window is CustomWindow))
		{
			base.game.StartCoroutine(CustomWindowLoop());
		}
		setup = true;
	}

	public IEnumerator MinimizeIfFullscreen()
	{
		if (Screen.fullScreen)
		{
			Screen.fullScreen = false;
			yield return new WaitForSecondsRealtime(WindowedSwitchTime);
		}
	}

	public void UpdateWindowInputs()
	{
		WindowDancer[] array = dancers;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].window is CustomWindow customWindow)
			{
				customWindow.UpdateWindowInputs();
			}
		}
	}

	public bool CheckInputState(KeyCode key, RDButtonState state)
	{
		bool flag = false;
		WindowDancer[] array = dancers;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].window is CustomWindow customWindow)
			{
				flag |= customWindow.CheckInputState(key, state);
			}
		}
		return flag;
	}

	public override void OnPauseChanged(bool paused)
	{
		base.OnPauseChanged(paused);
		if (!mainWindowIsDancer && !base.editorMode)
		{
			if (scnBase.isMac)
			{
				WindowChoreographer.playerWindow.Show(paused);
			}
			else if (paused)
			{
				WindowChoreographer.playerWindow.Reset(this);
			}
			else
			{
				WindowChoreographer.playerWindow.SetPosition(WindowDanceResolution * 3);
			}
		}
	}

	private IEnumerator CustomWindowLoop()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			WindowDancer[] array = dancers;
			foreach (WindowDancer windowDancer in array)
			{
				if (windowDancer.window is CustomWindow customWindow && windowDancer.visible)
				{
					customWindow.window.Present();
				}
			}
			MultiWindowPlugin.IssueRenderEvent();
		}
	}

	protected override void UpdateWindowZOrders()
	{
		List<BaseWindow> list = (from i in zOrder
			where i < dancers.Length
			select dancers[i] into x
			where x.visible
			select ((RealWindow)x.window).Window).ToList();
		if (!mainWindowIsDancer)
		{
			list.Insert(0, WindowChoreographer.playerWindow.Window);
		}
		if (list.Count > 1)
		{
			BaseWindow.Arrange(list.ToArray());
		}
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
		if (!WindowChoreographer.playerWindow.Window.IsFocused)
		{
			WindowDancer[] array = dancers;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].window is CustomWindow customWindow && customWindow.window.IsFocused)
				{
					WindowChoreographer.playerWindow.Window.Focus();
					break;
				}
			}
		}
		if (RDClass.debugSettings.Debug)
		{
			scnGame.customDebugText = $"playerWindow.IsFocused: {WindowChoreographer.playerWindow.Window.IsFocused}";
		}
	}

	protected override IEnumerator PrepareForDancing()
	{
		printe("trying to prepare");
		changingWindowSize = true;
		bool showGlitch = preparedFirstTime && base.conductor.barNumber <= 4;
		if (showGlitch)
		{
			ShowGlitch(show: true);
			yield return new WaitForSecondsRealtime(1f);
		}
		yield return MinimizeIfFullscreen();
		OnPauseChanged(!base.game.startTheGameCalled || base.game.paused);
		CheckDancers();
		if (showGlitch)
		{
			ShowGlitch(show: false);
		}
		yield return new WaitForSecondsRealtime(1f);
		preparedFirstTime = true;
		changingWindowSize = false;
	}
}
