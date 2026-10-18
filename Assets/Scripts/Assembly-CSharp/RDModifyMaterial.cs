using System;
using UnityEngine;

public class RDModifyMaterial : MonoBehaviour
{
	public RDShaderProperties shader;

	public Renderer shaderRenderer;

	[NonSerialized]
	public int lastFrameUpdatedGraphics = int.MinValue;

	private void Awake()
	{
		shaderRenderer = GetComponent<Renderer>();
		shader = new RDShaderProperties();
	}

	private void LateUpdate()
	{
		if (lastFrameUpdatedGraphics < shader.frameChanged)
		{
			shader.SetPropertiesInRenderer(shaderRenderer);
			lastFrameUpdatedGraphics = RDTime.currentFrame;
		}
	}
}
