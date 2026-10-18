using UnityEngine;

[ExecuteInEditMode]
public class RDImageEffectPixelate : MonoBehaviour
{
	public Shader effectShader;

	public Camera referenceCamera;

	private Material effectMat;

	public float time;

	public Vector2 screenSize = Vector2.zero;

	public bool disableIfNonPixelPerfectResolution;

	[Header("Background Compositing")]
	public Camera backgroundCamera;

	public bool disableBackgroundPixelateIfNonPixelPerfect;

	private RenderTexture bgRT;

	private void Awake()
	{
		if (referenceCamera == null)
		{
			referenceCamera = GetComponent<Camera>();
		}
	}

	private void Start()
	{
		effectMat = new Material(effectShader);
		if (backgroundCamera != null && Application.isPlaying)
		{
			Camera component = GetComponent<Camera>();
			component.clearFlags = CameraClearFlags.Color;
			component.backgroundColor = new Color(0f, 0f, 0f, 0f);
		}
	}

	private void EnsureBackgroundRT()
	{
		if (Application.isPlaying && (!(bgRT != null) || !bgRT.IsCreated() || bgRT.width != Screen.width || bgRT.height != Screen.height))
		{
			if (bgRT != null)
			{
				backgroundCamera.targetTexture = null;
				bgRT.Release();
				Object.DestroyImmediate(bgRT);
			}
			bgRT = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
			bgRT.filterMode = FilterMode.Point;
			backgroundCamera.targetTexture = bgRT;
		}
	}

	private bool IsNonPixelPerfectResolution()
	{
		float num = (float)Screen.height / 198f;
		return Mathf.Abs(num - Mathf.Round(num)) > 0.001f;
	}

	private void SetEffectParams()
	{
		screenSize = new Vector2(198f * referenceCamera.aspect, 198f);
		effectMat.SetVector("_ScreenSize", screenSize);
		effectMat.SetFloat("_Offset", (time != 0f) ? time : 0.0001f);
	}

	private void OnDestroy()
	{
		if (bgRT != null)
		{
			if (backgroundCamera != null)
			{
				backgroundCamera.targetTexture = null;
			}
			bgRT.Release();
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (effectMat == null)
		{
			Start();
			return;
		}
		if (backgroundCamera != null)
		{
			EnsureBackgroundRT();
			if (bgRT != null)
			{
				SetEffectParams();
				bool flag = disableBackgroundPixelateIfNonPixelPerfect && IsNonPixelPerfectResolution();
				bgRT.filterMode = FilterMode.Point;
				src.filterMode = FilterMode.Point;
				effectMat.SetTexture("_OverlayTex", src);
				effectMat.SetFloat("_PixelateBg", flag ? 0f : 1f);
				Graphics.Blit(bgRT, dst, effectMat);
				return;
			}
		}
		if (disableIfNonPixelPerfectResolution && IsNonPixelPerfectResolution())
		{
			Graphics.Blit(src, dst);
			return;
		}
		SetEffectParams();
		src.filterMode = FilterMode.Point;
		Graphics.Blit(src, dst, effectMat);
	}
}
