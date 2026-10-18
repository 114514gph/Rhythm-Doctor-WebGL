using DG.Tweening;
using UnityEngine;

public abstract class Window : RDClass
{
	public const string DefaultTitle = "Rhythm Doctor";

	public int index = -1;

	public bool shouldRenderUI;

	public bool isTransparent;

	public bool isFrameless;

	public RenderTexture renderTexture;

	public Material material;

	public string title = "Rhythm Doctor";

	public Color borderColor;

	private Tween flashBorderTween;

	public virtual bool Validate()
	{
		return true;
	}

	public abstract Vector2Int GetBorderSize();

	public abstract Vector2Int GetSize();

	public abstract Vector2Int GetViewSize();

	public abstract void SetViewSize(Vector2Int viewSize);

	public abstract void SetFrame(Vector2Int position, Vector2Int viewSize);

	public abstract void SetPosition(Vector2Int position, bool takesTaskbarIntoAccount = false);

	public abstract void SetCenter(Vector2Int center);

	public abstract void ShowOnTop();

	public abstract void Show(bool show);

	public abstract Vector2Int GetPosition();

	public abstract Vector2Int GetCenter();

	protected abstract void SetTitle(string text);

	public virtual void OnDestroy()
	{
	}

	public void SetTitle(string text, bool append = false)
	{
		title = (append ? (title + text) : text);
		SetTitle(title);
	}

	public void ResetTitle()
	{
		SetTitle("Rhythm Doctor", false);
	}

	public virtual void SetTransparent(bool transparent)
	{
		isTransparent = transparent;
	}

	public virtual void SetFrameless(bool frameless)
	{
		isFrameless = frameless;
	}

	public Window(int index, WindowChoreographer choreographer)
	{
		this.index = index;
		if (choreographer != null)
		{
			int depth = ((index == 0) ? 24 : 16);
			shouldRenderUI = index == 0;
			renderTexture = RDClass.Vfx.CreateRenderTexture(base.game.roomScale, "windowRenderTexture" + index, depth);
			renderTexture.Create();
			material = new Material(base.gc.renderTextureShader);
		}
	}

	public void FlashBorder(Color color, float duration, bool animationIsSpeedIndependent)
	{
		borderColor = color;
		flashBorderTween?.Kill();
		flashBorderTween = DOTween.To(() => borderColor, (Color x) =>
		{
			borderColor = x;
		}, color.WithAlpha(0f), duration).SetUpdate(UpdateType.Normal, animationIsSpeedIndependent).Done();
	}

	public virtual Vector2Int GetDefaultCenter(WindowChoreographer choreographer)
	{
		return (choreographer?.WindowDanceResolution ?? base.platformHelper.WindowDanceResolution) / 2;
	}

	public Vector2Int GetDefaultSize(WindowChoreographer choreographer)
	{
		return scrVfxControl.GetResolutionForScale(choreographer.GetWindowScaleForDancing());
	}

	public void Reset(WindowChoreographer choreographer)
	{
		Vector2Int defaultSize = GetDefaultSize(choreographer);
		Vector2Int vector2Int = GetBorderSize() + defaultSize;
		Vector2Int position = GetDefaultCenter(choreographer) - vector2Int / 2;
		SetFrame(position, defaultSize);
	}

	public virtual void ResetPosition(WindowChoreographer choreographer)
	{
		SetCenter(GetDefaultCenter(choreographer));
	}

	public virtual void ResetSize(WindowChoreographer choreographer)
	{
		SetViewSize(GetDefaultSize(choreographer));
	}

	public void ClearRenderTexture()
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		Color backgroundColor = (isTransparent ? Color.clear : Color.black);
		GL.Clear(clearDepth: true, clearColor: true, backgroundColor);
		RenderTexture.active = active;
	}

	public bool CheckIfCurrentViewSizeIsCorrect(Vector2Int currentViewSize, Vector2 windowScale, Vector2Int targetViewSize, Vector2Int extraBounds)
	{
		Vector2Int vector2Int = ((currentViewSize - Vector2Int.one) / windowScale).RoundToInt();
		Vector2Int vector2Int2 = ((currentViewSize + Vector2Int.one) / windowScale).RoundToInt();
		bool flag = targetViewSize.x >= vector2Int.x - extraBounds.x && targetViewSize.y >= vector2Int.y - extraBounds.y && targetViewSize.x <= vector2Int2.x + extraBounds.x && targetViewSize.y <= vector2Int2.y + extraBounds.y;
		if (!flag)
		{
			string text = (flag ? "green" : "red");
			Debug.Log($"Within bounds: <color={text}>{flag}</color> | currentWindowSize {currentViewSize} desiredViewSize {targetViewSize}, windowScale {windowScale:R}, lowerBound: {vector2Int}, higherBound: {vector2Int2}, extraBounds {extraBounds}");
		}
		return flag;
	}
}
