using UnityEngine;

[ExecuteInEditMode]
public class RDSetSortingLayer : RDBase
{
	public RDSortingLayer sortingLayer;

	public int sortingOrder;

	private Renderer renderer;

	private void Awake()
	{
		renderer = GetComponent<Renderer>();
		if (base.enabled || !Application.isPlaying)
		{
			Update();
		}
	}

	public void Update()
	{
		renderer.sortingLayerName = sortingLayer.ToString();
		renderer.sortingOrder = sortingOrder;
	}
}
