using UnityEngine;

public class UnityPlayerWindowWindows : UnityPlayerWindow
{
	private bool _visible = true;

	private PlatformHelperWindows phWindows => base.platformHelper as PlatformHelperWindows;

	public UnityPlayerWindowWindows(int index, WindowChoreographer choreographer)
		: base(index, choreographer)
	{
	}

	public override Vector2Int GetViewSize()
	{
		if (Application.isEditor)
		{
			return new Vector2Int(Screen.width, Screen.height);
		}
		return UnityPlayerWindow.playerWindow.GetViewSize().ToVector2Int();
	}

	public override void SetFrame(Vector2Int position, Vector2Int viewSize)
	{
		if (_visible)
		{
			base.SetFrame(position, viewSize);
		}
	}

	public override void SetPosition(Vector2Int position, bool takesTaskbarIntoAccount = false)
	{
		if (_visible)
		{
			base.SetPosition(position, takesTaskbarIntoAccount);
		}
	}

	public override void SetViewSize(Vector2Int viewSize)
	{
		viewSize = Vector2Int.Max(viewSize, WindowChoreographer.MinimumViewSize);
		Vector2Int borderSize = GetBorderSize();
		Vector2Int vector2Int = viewSize + borderSize;
		UnityPlayerWindow.playerWindow.SetSize(vector2Int.x, vector2Int.y);
	}

	public override Vector2Int GetPosition()
	{
		return phWindows.GetWindowPosition(Window.Hwnd);
	}

	public override Vector2Int GetDefaultCenter(WindowChoreographer choreographer)
	{
		return phWindows.GetMonitorCenter(phWindows.currentMonitorIndex);
	}

	public override void Show(bool visible)
	{
		_visible = visible;
		if (!visible)
		{
			UnityPlayerWindow.playerWindow.SetPosition(-10000, -10000);
		}
	}
}
