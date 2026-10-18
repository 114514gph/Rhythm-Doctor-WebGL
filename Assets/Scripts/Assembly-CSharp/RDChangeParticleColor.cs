using UnityEngine;

public class RDChangeParticleColor : RDBase
{
	public RowEntity rowEntity;

	public Renderer particleRenderer;

	private void Update()
	{
		RDShaderProperties rDShaderProperties = ((base.game.currentLevel.data != null) ? rowEntity.shaderData : scrVfxControl.instance.globalShaderData);
		Color overlayColor = Color.Lerp(rDShaderProperties.overlayColor, Color.black, rDShaderProperties.black);
		SetOverlayColor(overlayColor);
	}

	private void SetOverlayColor(Color color)
	{
		particleRenderer.material.SetColor("_ColorOverlay", color);
	}
}
