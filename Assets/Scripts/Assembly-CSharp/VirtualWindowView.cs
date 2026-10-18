using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class VirtualWindowView : RDBase
{
	public RectTransform rectTransform;

	public RawImage rawImage;

	public Image borderImage;

	public Text titleLabel;

	public CanvasGroup canvasGroup;

	private SimulateWDController controller;

	private RectOffset spriteBorder;

	private int index;

	private void Awake()
	{
		spriteBorder = new RectOffset(-5, 5, 15, -5);
	}

	public void Setup(SimulateWDController controller, int windowIndex)
	{
		this.controller = controller;
		if (windowIndex > 0)
		{
			titleLabel.text = $"Window {windowIndex + 1}";
		}
		index = windowIndex;
	}

	private void LateUpdate()
	{
		float pixelScaleMultiplier = controller.PixelScaleMultiplier;
		RectTransform obj = borderImage.rectTransform;
		obj.offsetMin = new Vector2(spriteBorder.left, spriteBorder.bottom) * pixelScaleMultiplier;
		obj.offsetMax = new Vector2(spriteBorder.right, spriteBorder.top) * pixelScaleMultiplier;
		borderImage.pixelsPerUnitMultiplier = 1f / pixelScaleMultiplier;
		if (base.editor != null)
		{
			borderImage.color = (base.editor.coloredWindows ? (RDEditorConstants.windowColors[index] * 1.3f) : Color.white);
		}
	}

	public void RenameWindow(string name, bool append = false)
	{
		string text = (append ? titleLabel.text : "") + name;
		titleLabel.text = ((text.Length > 200) ? (text.Substring(0, 200) + "...") : text);
	}
}
