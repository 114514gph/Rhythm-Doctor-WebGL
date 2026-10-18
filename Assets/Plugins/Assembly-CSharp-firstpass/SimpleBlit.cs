using UnityEngine;

public class SimpleBlit : MonoBehaviour
{
	public Material material;

	private void Awake()
	{
		material = new Material(material);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (material != null)
		{
			Graphics.Blit(src, dst, material);
		}
	}
}
