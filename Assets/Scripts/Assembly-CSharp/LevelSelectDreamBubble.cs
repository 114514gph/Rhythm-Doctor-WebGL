using UnityEngine;

public class LevelSelectDreamBubble : MonoBehaviour
{
	public Renderer dreamQuad;

	public Camera dreamCam;

	public Camera maskCam;

	public int orderInLayer;

	private RenderTexture InstantiateRenderTexture(Vector2Int res, Camera cam)
	{
		RenderTexture renderTexture = new RenderTexture(res.x, res.y, 16, RenderTextureFormat.ARGB32);
		renderTexture.antiAliasing = 1;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.name = cam.gameObject.name + "RT";
		cam.targetTexture = renderTexture;
		cam.orthographicSize = (float)res.y / 2f;
		return renderTexture;
	}

	private void Awake()
	{
		Vector2Int res = Vector2Int.RoundToInt(dreamQuad.transform.localScale);
		RenderTexture mainTexture = InstantiateRenderTexture(res, dreamCam);
		RenderTexture value = InstantiateRenderTexture(res, maskCam);
		dreamQuad.material = Object.Instantiate(dreamQuad.material);
		Material material = dreamQuad.material;
		material.mainTexture = mainTexture;
		material.SetTexture("_MaskTex", value);
		material.SetInt("_UsingMask", 1);
	}

	private void Update()
	{
		dreamQuad.material.renderQueue = 2001;
		dreamQuad.sortingLayerName = "Default";
		dreamQuad.sortingOrder = orderInLayer;
	}
}
