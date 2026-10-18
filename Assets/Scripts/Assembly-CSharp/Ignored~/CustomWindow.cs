using System.Collections.Generic;
using MultiWindow;
using MultiWindow.Events;
using UnityEngine;

public class CustomWindow : RealWindow
{
	private const int DefaultTitleBarHeight = 28;

	private readonly Vector2Int initialPosition = new Vector2Int(10, 10);

	private readonly Vector2Int initialSize = new Vector2Int(100, 100);

	public MultiWindow.Window window;

	private Icon icon;

	public HashSet<KeyCode> keyDowns = new HashSet<KeyCode>();

	public HashSet<KeyCode> keys = new HashSet<KeyCode>();

	public HashSet<KeyCode> keyUps = new HashSet<KeyCode>();

	public override BaseWindow Window => window;

	public CustomWindow(int index, WindowChoreographer choreographer, bool transparent = false)
		: base(index, choreographer)
	{
		Vector2Int vector2Int = (Screen.fullScreen ? initialPosition : PlatformHelper.instance.playerWindow.Window.GetPosition());
		window = new MultiWindow.Window(title, vector2Int.x, vector2Int.y, initialSize.x, initialSize.y, frameless: false, !transparent);
		icon = new Icon(RDConstants.data.defaultCustomWindowIcon);
		window.SetIcon(icon);
		window.SetSize(initialSize.x, initialSize.y);
		if (renderTexture == null)
		{
			Debug.LogWarning("RenderTexture is null in CustomWindow Setup!");
		}
		else
		{
			window.SetTexture(renderTexture);
			window.EnableInput();
		}
		SamplerConfig samplerConfig = new SamplerConfig
		{
			Filter = WindowFilter.Nearest
		};
		window.SetSamplerConfig(samplerConfig);
		printe($"Created CustomWindow {index}");
	}

	public override void OnDestroy()
	{
		window?.Dispose();
		window = null;
		icon?.Dispose();
		icon = null;
	}

	public void UpdateWindowInputs()
	{
		keyDowns.Clear();
		keyUps.Clear();
		WindowEvent ev;
		while (window.PopEvent(out ev))
		{
			if (ev is KeyDownEvent keyDownEvent)
			{
				keyDowns.Add(keyDownEvent.Key);
				keys.Add(keyDownEvent.Key);
			}
			else if (ev is KeyUpEvent keyUpEvent)
			{
				keys.Remove(keyUpEvent.Key);
				keyUps.Add(keyUpEvent.Key);
			}
		}
	}

	public bool CheckInputState(KeyCode key, RDButtonState state)
	{
		return state switch
		{
			RDButtonState.WentDown => keyDowns.Contains(key), 
			RDButtonState.WentUp => keyUps.Contains(key), 
			RDButtonState.IsDown => keys.Contains(key), 
			_ => false, 
		};
	}

	public override void Show(bool visible)
	{
		if (visible)
		{
			window.Show();
		}
		else
		{
			window.Hide();
		}
	}

	public override void SetFrameless(bool frameless)
	{
		base.SetFrameless(frameless);
		Window.FrameVisible = !frameless;
	}
}
