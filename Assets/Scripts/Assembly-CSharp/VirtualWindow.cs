using UnityEngine;

public class VirtualWindow : Window
{
	public SimulateWDController controller;

	private VirtualWindowView view;

	private RectTransform rectTransform;

	private RectOffset border;

	public VirtualWindow(SimulateWDController controller, int index, VirtualWindowChoreographer choreographer)
		: base(index, choreographer)
	{
		this.controller = controller;
		view = controller.views[index];
		rectTransform = view.rectTransform;
		border = controller.border;
		SetTitle("Rhythm Doctor");
	}

	public override bool Validate()
	{
		WindowChoreographer windowChoreographer = base.game.windowChoreographer;
		Vector2Int resolutionForScale = scrVfxControl.GetResolutionForScale(windowChoreographer.GetWindowScaleForDancing());
		Vector2Int viewSize = GetViewSize();
		if (!CheckIfCurrentViewSizeIsCorrect(viewSize, windowChoreographer.dancers[index].windowScale, resolutionForScale, Vector2Int.zero))
		{
			return false;
		}
		return true;
	}

	public override void SetCenter(Vector2Int center)
	{
		SetPosition(center - GetSize() / 2);
	}

	public override Vector2Int GetSize()
	{
		return GetViewSize() + GetBorderSize();
	}

	public override Vector2Int GetViewSize()
	{
		return rectTransform.sizeDelta.RoundToInt();
	}

	public override Vector2Int GetBorderSize()
	{
		return new Vector2Int(border.horizontal, border.vertical);
	}

	public override Vector2Int GetCenter()
	{
		return GetPosition() + GetSize() / 2;
	}

	public override void SetPosition(Vector2Int position, bool takesTaskbarIntoAccount = false)
	{
		rectTransform.anchoredPosition = new Vector2Int(position.x + border.left, position.y + border.bottom);
	}

	public override void SetViewSize(Vector2Int viewSize)
	{
		viewSize = Vector2Int.Max(viewSize, WindowChoreographer.MinimumViewSize);
		rectTransform.sizeDelta = viewSize;
		material.SetFloat("_ExtraRatio", (float)viewSize.x / (float)viewSize.y / 1.7777778f);
	}

	public override void SetFrame(Vector2Int position, Vector2Int viewSize)
	{
		SetPosition(position);
		SetViewSize(viewSize);
	}

	public override Vector2Int GetPosition()
	{
		return new Vector2Int(Mathf.RoundToInt(rectTransform.anchoredPosition.x - (float)border.left), Mathf.RoundToInt(rectTransform.anchoredPosition.y - (float)border.bottom));
	}

	protected override void SetTitle(string text)
	{
		controller.views[index].RenameWindow(text);
	}

	public override void ShowOnTop()
	{
		rectTransform.transform.SetAsLastSibling();
	}

	public override void Show(bool visible)
	{
		view.gameObject.SetActive(visible);
	}

	public override void SetFrameless(bool frameless)
	{
		base.SetFrameless(frameless);
		controller.views[index].borderImage.gameObject.SetActive(!frameless);
	}
}
