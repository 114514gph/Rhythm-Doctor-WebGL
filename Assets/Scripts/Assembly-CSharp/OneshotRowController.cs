using DG.Tweening;
using UnityEngine;

public class OneshotRowController : RDBase
{
	public RowEntity rowEntity;

	public RDWaveManager waveManager;

	public GameObject waveShadowPrefab;

	public Transform waveshadowContainer;

	public Entity skipshotIndicator;

	public GameObject polygonsAndLine;

	public Transform line;

	public SpriteRenderer lineRenderer;

	public SpriteRenderer lineGlow;

	public SpriteRenderer lineOutline;

	public bool animateCharacter = true;

	public bool isSubdivision;

	public float waveHeightMultiplier = 1f;

	public float waveWidthMultiplier = 1f;

	public ParticleSystem charSparks;

	public ParticleSystem lineSparks;

	public ParticleSystem heartSparks;

	public RDWave[] waves => waveManager.allWaves;

	public RDWave firstWave => waveManager.firstWave;

	public RDWaveRenderer[] shadows => waveManager.shadows;

	public void Setup()
	{
		if (base.gc.useShadows)
		{
			int shadowsNumber = base.gc.shadowsNumber;
			waveManager.shadows = new RDWaveRenderer[shadowsNumber];
			for (int i = 0; i < shadows.Length; i++)
			{
				GameObject obj = Object.Instantiate(waveShadowPrefab, firstWave.transform.localPosition, firstWave.transform.localRotation, waveshadowContainer);
				obj.name = $"WaveShadow{i}";
				RDWaveRenderer component = obj.GetComponent<RDWaveRenderer>();
				component.ent = rowEntity;
				component.manager = waveManager;
				component.ShadowHide();
				shadows[i] = component;
			}
		}
		skipshotIndicator.sprite.color = skipshotIndicator.sprite.color.WithAlpha(0f);
	}

	private void LateUpdate()
	{
		SetPropertiesToMaterial(lineRenderer.material);
		SetPropertiesToMaterial(lineGlow.material);
		SetPropertiesToMaterial(lineOutline.material);
		void SetPropertiesToMaterial(Material material)
		{
			material.SetFloat(RDShaderProperties.OverlayTexturePowerID, rowEntity.shaderDataToUse.overlayTexturePower);
			material.SetFloat(RDShaderProperties.NoiseTexturePowerID, rowEntity.shaderDataToUse.noiseTexturePower);
			material.SetTexture("_OverlayTexture", rowEntity.shaderDataToUse.overlayTexture);
			material.SetTexture("_NoiseTexture", rowEntity.shaderDataToUse.noiseTexture);
			material.SetFloat(RDShaderProperties.PixelScaleID, GC.RoomRenderTextureScale);
		}
	}

	public void PlaySkipshotAnimation(float duration)
	{
		Entity i = skipshotIndicator;
		Transform target = i.transform;
		i.DOKill();
		i.sprite.DOKill();
		target.DOKill();
		float num = (rowEntity.lineLeft.transform.localScale.x + 0.5f) * 24f;
		float endValue = num + (rowEntity.visualRowLength - 1.5f) * 24f + rowEntity.playerBox.HalfHitlineWidth;
		target.DOLocalMoveX(endValue, duration).From(num).SetEase(Ease.OutCubic);
		target.DOScaleY(1f, duration / 4.5f).From(2f).SetEase(Ease.OutSine);
		i.animator.PlayFrom(0f);
		i.sprite.color = skipshotIndicator.sprite.color.WithAlpha(1f);
		i.visible = true;
		i.sprite.DOFade(0f, duration).OnComplete(() =>
		{
			i.visible = false;
		});
	}

	public void PlaySparks(bool play)
	{
		ParticleSystem[] array = new ParticleSystem[3]
		{
			rowEntity.targetCharacterRowEnt.oneshotRowController.charSparks,
			lineSparks,
			heartSparks
		};
		foreach (ParticleSystem particleSystem in array)
		{
			if (play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	public void EmitSparks(int count)
	{
		charSparks.Emit(count);
		lineSparks.Emit(count * 2);
		heartSparks.Emit(count);
	}

	public void ToggleAllWavesVisible(bool visible)
	{
		RDWave[] array = waves;
		foreach (RDWave rDWave in array)
		{
			if (!visible || !(rDWave.waveRenderer is RDWaveRenderer_Polygon))
			{
				rDWave.visible = visible;
			}
		}
	}

	public void ResizeLine(float width)
	{
		RDWave[] array = waves;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].waveRenderer is RDWaveRenderer_Pulse wave)
			{
				ResizeTransform(wave);
			}
		}
		RDWaveRenderer[] array2 = shadows;
		for (int i = 0; i < array2.Length; i++)
		{
			ResizeTransform(array2[i]);
		}
		void ResizeTransform(RDWaveRenderer rDWaveRenderer)
		{
			rDWaveRenderer.transform.localScale = rDWaveRenderer.transform.localScale.WithX(width);
			rDWaveRenderer.transform.localPosition = Vector3.right * (width / 2f);
			rDWaveRenderer.ResizeCanvas();
		}
	}

	public void FlattenWaves()
	{
		waveManager.FlattenAll();
		waveManager.HideShadows();
	}
}
