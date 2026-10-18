using UnityEngine;

public class CascadeStutterEffect : MonoBehaviour
{
	public Material effectMaterial;

	public float rows = 4f;

	public float shown = 0.5f;

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		effectMaterial.SetFloat("_Rows", rows);
		effectMaterial.SetFloat("_Shown", shown);
		Graphics.Blit(src, dest, effectMaterial);
	}
}
