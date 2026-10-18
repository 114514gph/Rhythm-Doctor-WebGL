using UnityEngine;

[ExecuteInEditMode]
public class RDWaveTester : MonoBehaviour
{
	public RDWaveAnimation waveAnimation;

	public float playbackEditorTime;

	public float timeScale = 1f;

	public bool loopTime = true;

	private RDWave rdwave;

	private void Update()
	{
	}

	private void OnValidate()
	{
	}

	private void UpdateWave()
	{
		float num = (Application.isPlaying ? (Time.timeSinceLevelLoad * timeScale) : (playbackEditorTime * timeScale));
		if (!(waveAnimation != null))
		{
			return;
		}
		float duration = waveAnimation.duration;
		if (loopTime && duration != 0f)
		{
			num %= duration;
		}
		Material sharedMaterial = GetComponent<Renderer>().sharedMaterial;
		sharedMaterial.SetFloat("_WaveTime", waveAnimation.waveTime.Evaluate(num));
		sharedMaterial.SetFloat("_SineWidth", waveAnimation.sineWidth.Evaluate(num));
		float num2 = waveAnimation.sineHeight.Evaluate(num);
		if (waveAnimation.heightDecays)
		{
			num2 *= Mathf.Exp((0f - waveAnimation.decayConstant) * num) * Mathf.Cos(waveAnimation.angularFrequency * num);
			if (Mathf.Abs(num2) < 1f)
			{
				num2 = 0f;
			}
		}
		sharedMaterial.SetFloat("_SineHeight", num2);
		sharedMaterial.SetFloat("_TriangleOffset", waveAnimation.triangleOffset.Evaluate(num));
		sharedMaterial.SetFloat("_TriangleFrequency", waveAnimation.triangleFrequency.Evaluate(num));
		sharedMaterial.SetFloat("_TriangleOffsetLock", waveAnimation.triangleLock);
	}
}
