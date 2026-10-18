using MultiWindow;
using UnityEngine;

public abstract class UnityPlayerWindow : RealWindow
{
	protected static PlayerWindow playerWindow;

	public override BaseWindow Window => playerWindow;

	public UnityPlayerWindow(int index, WindowChoreographer choreographer)
		: base(index, choreographer)
	{
		if (playerWindow == null)
		{
			playerWindow = PlayerWindow.Instance;
			playerWindow.RefreshHandle();
		}
		if (base.editorMode)
		{
			base.editor.gameView.texture = renderTexture;
		}
		SetTitle("Rhythm Doctor");
	}

	protected override void SetTitle(string text)
	{
		if (!Application.isEditor)
		{
			base.SetTitle(text);
		}
	}

	public override void ShowOnTop()
	{
		playerWindow.RefreshHandle();
		playerWindow.MoveToTop();
	}

	public override void Show(bool visible)
	{
		if (visible)
		{
			playerWindow.Show();
		}
		else
		{
			playerWindow.Hide();
		}
	}
}
