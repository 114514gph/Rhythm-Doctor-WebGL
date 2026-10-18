using UnityEngine;

public class RDWaveRenderer_Polygon : RDWaveRenderer
{
	public int sides = 3;

	public PolygonWave polygonMain;

	public PolygonWave polygonGlow;

	public PolygonWave polygonOutline;

	private float _polygonAngle;

	private Transform polyTrans => polygonMain.transform;

	public float polygonAngle
	{
		get
		{
			return _polygonAngle;
		}
		set
		{
			_polygonAngle = value;
			polyTrans.localEulerAngles = Vector3.forward * _polygonAngle;
		}
	}

	public override void UpdateVisuals(float t, float wavePos, int pulseIndex = 0)
	{
		if (!(waveAnimations[pulseIndex] == null))
		{
			SetPolygonSideLength(GetWaveHeight(t, pulseIndex) * ent.oneshotRowController.waveHeightMultiplier / 19f * 0.669f);
			SetPolygonSideCount(sides);
			SetPropertiesToMaterial(polygonMain.meshRenderer.material);
			SetPropertiesToMaterial(polygonGlow.meshRenderer.material);
			SetPropertiesToMaterial(polygonOutline.meshRenderer.material);
			float num = Mathf.Lerp(20f, 130f, wavePos);
			polyTrans.gameObject.SetActive(wavePos > 0f && wavePos < 1f);
			polyTrans.transform.localPosition = new Vector3(num, -0.5f, -2.8f);
		}
		void SetPropertiesToMaterial(Material material)
		{
			material.SetFloat(RDShaderProperties.OverlayTexturePowerID, shaderDataToUse.overlayTexturePower);
			material.SetTexture("_OverlayTexture", shaderDataToUse.overlayTexture);
			material.SetFloat(RDShaderProperties.NoiseTexturePowerID, shaderDataToUse.noiseTexturePower);
			material.SetTexture("_NoiseTexture", shaderDataToUse.noiseTexture);
			material.SetFloat(RDShaderProperties.PixelScaleID, GC.RoomRenderTextureScale);
		}
	}

	protected override void LateUpdate()
	{
		if (!visible && !polygonMain.meshRenderer.enabled)
		{
			return;
		}
		base.LateUpdate();
		if (Application.isPlaying)
		{
			if (!isShadow)
			{
				bool flag = shaderDataToUse.outline == 1;
				polygonMain.meshRenderer.enabled = visible;
				polygonGlow.meshRenderer.enabled = visible && !flag;
				polygonOutline.meshRenderer.enabled = visible & flag;
			}
			if (wave != null)
			{
				polygonAngle = (Time.timeSinceLevelLoad - wave.timeSpentFrozen) * -100f + wave.polygonRotOffset;
			}
		}
	}

	public void SetPolygonSideLength(float s)
	{
		polyTrans.transform.localScale = new Vector3(s, s, 1f);
	}

	public void SetPolygonSideCount(int sides)
	{
		polygonMain.sides = sides;
		polygonGlow.sides = sides;
		polygonOutline.sides = sides;
	}

	protected override void UpdateColor()
	{
		base.UpdateColor();
		if (polygonMain.meshRenderer.enabled)
		{
			Color color = GetColor();
			Color.RGBToHSV(color, out var H, out var S, out var V);
			V *= 1.23f;
			Color a = Color.HSVToRGB(H, S, V);
			Color a2 = Color.Lerp(color, shaderDataToUse.overlayColor.WithAlpha(1f), shaderDataToUse.overlayColor.a);
			Color a3 = Color.Lerp(a, shaderDataToUse.overlayColor.WithAlpha(1f), shaderDataToUse.overlayColor.a);
			a3 = Color.Lerp(a3, Color.black, shaderDataToUse.black);
			a3.a *= shaderDataToUse.opacity;
			a2 = Color.Lerp(a2, Color.black, shaderDataToUse.black);
			a2.a *= shaderDataToUse.opacity;
			Color color2 = shaderDataToUse.borderColor.WithAlpha(shaderDataToUse.borderColor.a);
			bool flag = shaderDataToUse.outline == 1;
			polygonMain.color = a3;
			polygonOutline.meshRenderer.enabled = flag;
			polygonGlow.meshRenderer.enabled = !flag;
			polygonOutline.color = color2;
			polygonGlow.color = color2;
		}
	}
}
