using System;
using UnityEngine;
using UnityEngine.UI;

public class SimulateWDController : RDBase
{
	[Header("Tweakables")]
	public float PixelScaleMultiplier = 2f;

	[Header("Components")]
	public RectTransform canvasRT;

	public Image background;

	public GraphicRaycaster graphicRaycaster;

	public RectTransform[] scalablePanels;

	public RectTransform viewContainer;

	public GameObject virtualWindowPrefab;

	public Shader viewShader;

	public Camera camera;

	[NonSerialized]
	public VirtualWindowView[] views;

	public RectOffset border => new RectOffset(2, 2, 12, 2);

	public void Setup(int windowCount)
	{
		views = new VirtualWindowView[windowCount];
		for (int i = 0; i < windowCount; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(virtualWindowPrefab, viewContainer);
			obj.transform.SetAsFirstSibling();
			obj.name = $"VirtualWindow{i}";
			VirtualWindowView component = obj.GetComponent<VirtualWindowView>();
			component.Setup(this, i);
			views[i] = component;
		}
		background.color = RDConstants.data.simulatedDesktopColor;
	}

	private void LateUpdate()
	{
		RectTransform[] array = scalablePanels;
		foreach (RectTransform obj in array)
		{
			obj.sizeDelta = canvasRT.sizeDelta / PixelScaleMultiplier;
			obj.localScale = new Vector3(PixelScaleMultiplier, PixelScaleMultiplier, 1f);
		}
	}
}
