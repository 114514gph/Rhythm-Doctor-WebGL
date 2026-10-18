using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/VHS/VHS")]
public class CameraFilterPack_TV_VHS : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(1f, 256f)]
	public float Cryptage = 64f;

	[Range(1f, 100f)]
	public float Parasite = 32f;

	[Range(-3f, 3f)]
	public float Calibrage;

	[Range(-3f, 3f)]
	public float CalibrageX;

	[Range(0f, 1f)]
	public float WhiteParasite = 1f;

	[Range(0f, 1f)]
	public float VHSEffectDisabled;

	public static float ChangeValue;

	public static float ChangeValue2;

	public static float ChangeValue3;

	public static float ChangeValue4;

	public static float ChangeValue5;

	public static float ChangeValue6;

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
		ChangeValue = Cryptage;
		ChangeValue2 = Parasite;
		ChangeValue3 = Calibrage;
		ChangeValue4 = WhiteParasite;
		ChangeValue5 = CalibrageX;
		SCShader = Shader.Find("CameraFilterPack/TV_VHS");
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
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_Value", Cryptage);
			material.SetFloat("_Value2", Parasite);
			material.SetFloat("_Value3", Calibrage);
			material.SetFloat("_Value4", WhiteParasite);
			material.SetFloat("_Value5", CalibrageX);
			material.SetFloat("_Value6", VHSEffectDisabled);
			material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0f, 0f));
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void OnValidate()
	{
		ChangeValue = Cryptage;
		ChangeValue2 = Parasite;
		ChangeValue3 = Calibrage;
		ChangeValue4 = WhiteParasite;
		ChangeValue5 = CalibrageX;
		ChangeValue6 = VHSEffectDisabled;
	}

	private void Update()
	{
		_ = Application.isPlaying;
	}

	private void OnDisable()
	{
		if ((bool)SCMaterial)
		{
			Object.DestroyImmediate(SCMaterial);
		}
	}
}
