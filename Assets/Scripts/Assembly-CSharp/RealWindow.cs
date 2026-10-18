#if !UNITY_WEBGL
using MultiWindow;
using UnityEngine;

public abstract class RealWindow : Window
{
	public abstract BaseWindow Window { get; }

	public RealWindow(int index, WindowChoreographer choreographer)
		: base(index, choreographer)
	{
	}

	public override Vector2Int GetSize()
	{
		return Window.GetSize().RoundToInt();
	}

	public override void SetViewSize(Vector2Int viewSize)
	{
		SetFrame(GetPosition(), viewSize);
	}

	public override void SetCenter(Vector2Int center)
	{
		SetPosition(center - GetSize() / 2);
	}

	public override Vector2Int GetCenter()
	{
		return GetPosition() + GetSize() / 2;
	}

	public override Vector2Int GetViewSize()
	{
		return Window.GetViewSize().RoundToInt();
	}

	public override Vector2Int GetPosition()
	{
		return Window.GetPosition();
	}

	public override void ShowOnTop()
	{
		Window.MoveToTop();
	}

	protected override void SetTitle(string text)
	{
		Window.SetTitle(text);
	}

	public override Vector2Int GetBorderSize()
	{
		return Vector2Int.Max(Vector2Int.zero, GetSize() - GetViewSize());
	}

	public override void SetFrame(Vector2Int position, Vector2Int viewSize)
	{
		viewSize = Vector2Int.Max(viewSize, WindowChoreographer.MinimumViewSize);
		Vector2Int windowSize = viewSize + GetBorderSize();
		position = base.platformHelper.TranslateWindowPos(position, windowSize);
		Window?.Move(position.x, position.y, windowSize.x, windowSize.y);
	}

	public override void SetPosition(Vector2Int position, bool takesTaskbarIntoAccount = false)
	{
		Vector2Int size = GetSize();
		position = base.platformHelper.TranslateWindowPos(position, size);
		Window?.SetPosition(position.x, position.y);
	}
}

#endif
