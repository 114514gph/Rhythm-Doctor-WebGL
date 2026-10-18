using UnityEngine;

public class CustomWindowWindows : CustomWindow
{
	private PlatformHelperWindows phWindows => base.platformHelper as PlatformHelperWindows;

	public CustomWindowWindows(int index, WindowChoreographer choreographer, bool transparent)
		: base(index, choreographer, transparent)
	{
	}

	public override Vector2Int GetPosition()
	{
		return phWindows.GetWindowPosition(Window.Hwnd);
	}

	public override Vector2Int GetDefaultCenter(WindowChoreographer choreographer)
	{
		return phWindows.GetMonitorCenter(phWindows.currentMonitorIndex);
	}
}
