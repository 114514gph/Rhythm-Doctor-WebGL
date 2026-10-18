using UnityEngine;

public class RDImageEffectTile : MonoBehaviour
{
	public Shader effectShader;

	public float tileX = 2f;

	public float tileY = 2f;

	public float zoom = 1f;

	public float offsetX;

	public float offsetY;

	public float anchorX;

	public float anchorY;

	private Material mEffectMat;

	private void Start()
	{
		mEffectMat = new Material(effectShader);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		mEffectMat.SetFloat("_TileX", tileX);
		mEffectMat.SetFloat("_TileY", tileY);
		mEffectMat.SetFloat("_Zoom", zoom);
		mEffectMat.SetFloat("_OffsetX", offsetX);
		mEffectMat.SetFloat("_OffsetY", offsetY);
		mEffectMat.SetFloat("_Zoom", zoom);
		mEffectMat.SetFloat("_AnchorX", anchorX);
		mEffectMat.SetFloat("_AnchorY", anchorY);
		Graphics.Blit(src, dst, mEffectMat);
	}

	public void UpdateState()
	{
		bool flag = zoom != 1f;
		bool flag2 = tileX != 1f || tileY != 1f;
		bool flag3 = offsetX != 0f || offsetY != 0f;
		base.enabled = flag | flag2 | flag3;
	}
}
