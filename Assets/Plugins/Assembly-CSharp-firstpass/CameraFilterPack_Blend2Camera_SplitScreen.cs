using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Split Screen/SideBySide")]
public class CameraFilterPack_Blend2Camera_SplitScreen : MonoBehaviour
{
	private string ShaderName = "CameraFilterPack/Blend2Camera_SplitScreen";

	public Shader SCShader;

	public Camera Camera2;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 1f)]
	public float SwitchCameraToCamera2;

	[Range(0f, 1f)]
	public float BlendFX = 1f;

	[Range(-3f, 3f)]
	public float SplitX = 0.5f;

	[Range(-3f, 3f)]
	public float SplitY = 0.5f;

	[Range(0f, 2f)]
	public float Smooth = 0.1f;

	[Range(-3.14f, 3.14f)]
	public float Rotation = 3.14f;

	private bool ForceYSwap;

	public static float ChangeValue;

	public static float ChangeValue2;

	public static float ChangeValue3;

	public static float ChangeValue4;

	public static float ChangeValue5;

	public static float ChangeValue6;

	public static bool ChangeValue7;

	private RenderTexture Camera2tex;

	private Vector2 ScreenSize;

	private Material material
	{
		get
		{
			if (SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return SCMaterial;
		}
	}

	private void Start()
	{
		ScreenSize.x = Screen.width;
		ScreenSize.y = Screen.height;
		if (Camera2 != null)
		{
			Object.DestroyImmediate(Camera2.targetTexture);
			Camera2tex = new RenderTexture((int)ScreenSize.x, (int)ScreenSize.y, 24);
			Camera2.targetTexture = Camera2tex;
		}
		ChangeValue = BlendFX;
		ChangeValue2 = SwitchCameraToCamera2;
		ChangeValue3 = SplitX;
		ChangeValue6 = SplitY;
		ChangeValue4 = Smooth;
		ChangeValue5 = Rotation;
		ChangeValue7 = ForceYSwap;
		SCShader = Shader.Find(ShaderName);
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			TimeX += Time.deltaTime;
			if (TimeX > 100f)
			{
				TimeX = 0f;
			}
			if (Camera2 != null)
			{
				material.SetTexture("_MainTex2", Camera2tex);
			}
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_Value", BlendFX);
			material.SetFloat("_Value2", SwitchCameraToCamera2);
			material.SetFloat("_Value3", SplitX);
			material.SetFloat("_Value6", SplitY);
			material.SetFloat("_Value4", Smooth);
			material.SetFloat("_Value5", Rotation);
			material.SetInt("_ForceYSwap", (!ForceYSwap) ? 1 : 0);
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void OnValidate()
	{
		ChangeValue = BlendFX;
		ChangeValue2 = SwitchCameraToCamera2;
		ChangeValue3 = SplitX;
		ChangeValue6 = SplitY;
		ChangeValue7 = ForceYSwap;
		ChangeValue4 = Smooth;
		ChangeValue5 = Rotation;
	}

	private void Update()
	{
		ScreenSize.x = Screen.width;
		ScreenSize.y = Screen.height;
		if (Application.isPlaying)
		{
			BlendFX = ChangeValue;
			SwitchCameraToCamera2 = ChangeValue2;
			SplitX = ChangeValue3;
			SplitY = ChangeValue6;
			Smooth = ChangeValue4;
			Rotation = ChangeValue5;
			ForceYSwap = ChangeValue7;
		}
	}

	private void OnEnable()
	{
		Start();
	}

	private void OnDisable()
	{
		if (Camera2 != null)
		{
			Object.DestroyImmediate(Camera2.targetTexture);
			Camera2.targetTexture = null;
		}
		if ((bool)SCMaterial)
		{
			Object.DestroyImmediate(SCMaterial);
		}
	}
}
