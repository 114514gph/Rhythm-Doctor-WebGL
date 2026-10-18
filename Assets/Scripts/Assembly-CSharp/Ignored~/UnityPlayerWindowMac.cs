using MultiWindow;
using UnityEngine;

public class UnityPlayerWindowMac : UnityPlayerWindow
{
	public readonly Vector2Int DontModify = new Vector2Int(int.MinValue, int.MinValue);

	private int lastValidScalingFactor = 1;

	public Vector2Int lastSizeRequest = Vector2Int.zero;

	public bool hasChangedSize;

	public float lastTimeChangedSize;

	private const float SafeTimeToUpdateMetalBackbuffer = 0.025f;

	public UnityPlayerWindowMac(int index, WindowChoreographer choreographer)
		: base(index, choreographer)
	{
	}

	public override void SetPosition(Vector2Int position, bool takesTaskbarIntoAccount = false)
	{
		PlatformHelper.Rectangle monitorsRect = base.platformHelper.GetMonitorsRect(base.platformHelper.Monitors, onlyHorizontal: true);
		position = new Vector2Int(Mathf.RoundToInt(monitorsRect.Left + position.x), Mathf.RoundToInt(PlatformHelperMac.maxTop - monitorsRect.Bottom + position.y));
		SetFrame(position, DontModify);
	}

	public override Vector2Int GetViewSize()
	{
		if (Application.isEditor)
		{
			int screenScalingFactor = GetScreenScalingFactor();
			return new Vector2Int(Mathf.RoundToInt((float)Screen.width / (float)screenScalingFactor), Mathf.RoundToInt((float)Screen.height / (float)screenScalingFactor));
		}
		Size viewSize = UnityPlayerWindow.playerWindow.GetViewSize();
		return new Vector2Int(viewSize.width, viewSize.height);
	}

	private int GetScreenScalingFactor()
	{
		int screenScalingFactor = PlatformHelperMac.Native.GetScreenScalingFactor();
		if (screenScalingFactor == -1)
		{
			screenScalingFactor = lastValidScalingFactor;
		}
		else
		{
			lastValidScalingFactor = screenScalingFactor;
		}
		return screenScalingFactor;
	}

	public override void ResetSize(WindowChoreographer choreographer)
	{
		lastTimeChangedSize = 0f;
		base.ResetSize(choreographer);
	}

	public override void SetFrame(Vector2Int position, Vector2Int viewSize)
	{
		if (viewSize != DontModify)
		{
			viewSize = Vector2Int.Max(viewSize, WindowChoreographer.MinimumViewSize);
		}
		hasChangedSize |= viewSize != DontModify && viewSize != lastSizeRequest;
		lastSizeRequest = viewSize;
		bool flag = Time.realtimeSinceStartup - lastTimeChangedSize > 0.025f;
		if (!hasChangedSize || (hasChangedSize & flag))
		{
			if (position == DontModify)
			{
				position = UnityPlayerWindow.playerWindow.GetPosition();
			}
			Vector2Int vector2Int = ((viewSize == DontModify) ? UnityPlayerWindow.playerWindow.GetSize().RoundToInt() : (viewSize + GetBorderSize()));
			UnityPlayerWindow.playerWindow.Move(position.x, position.y, vector2Int.x, vector2Int.y);
			hasChangedSize = false;
			lastTimeChangedSize = Time.realtimeSinceStartup;
		}
	}
}
