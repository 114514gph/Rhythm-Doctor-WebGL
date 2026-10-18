using UnityEngine;

public class RDChangeParticleAlpha : RDBase
{
	public RowEntity rowEntity;

	public Renderer particleRenderer;

	private void Update()
	{
		RDShaderProperties rDShaderProperties = ((base.game.currentLevel.data != null) ? rowEntity.shaderData : scrVfxControl.instance.globalShaderData);
		SetAlpha(rDShaderProperties.opacity);
	}

	private void SetAlpha(float alpha)
	{
		particleRenderer.material.SetFloat("_Opacity", alpha);
	}
}
