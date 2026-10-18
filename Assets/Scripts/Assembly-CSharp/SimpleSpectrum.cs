using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SimpleSpectrum : MonoBehaviour
{
	public enum SourceType
	{
		AudioSource = 0,
		AudioListener = 1,
		MicrophoneInput = 2,
		StereoMix = 3,
		Custom = 4
	}

	[SerializeField]
	public AudioMixerGroup muteGroup;

	[Tooltip("Enables or disables the processing and display of spectrum data. ")]
	public bool isEnabled = true;

	[Tooltip("The type of source for spectrum data.")]
	public SourceType sourceType;

	[Tooltip("The AudioSource to take data from.")]
	public AudioSource audioSource;

	[Tooltip("The audio channel to use when sampling.")]
	public int sampleChannel;

	[Tooltip("The number of samples to use when sampling. Must be a power of two.")]
	public int numSamples = 256;

	[Tooltip("The FFTWindow to use when sampling.")]
	public FFTWindow windowUsed = FFTWindow.BlackmanHarris;

	[Tooltip("If true, audio data is scaled logarithmically.")]
	public bool useLogarithmicFrequency = true;

	[Tooltip("If true, the values of the spectrum are multiplied based on their frequency, to keep the values proportionate.")]
	public bool multiplyByFrequency = true;

	[Tooltip("The lower bound of the freuqnecy range to sample from. Leave at 0 when unused.")]
	public float frequencyLimitLow;

	[Tooltip("The upper bound of the freuqnecy range to sample from. Leave at 22050 (44100/2) when unused.")]
	public float frequencyLimitHigh = 22050f;

	[Tooltip("The amount of bars to use. Does not have to be equal to Num Samples, but probably should be lower.")]
	public int barAmount = 32;

	[Tooltip("Stretches the values of the bars.")]
	public float barYScale = 50f;

	[Tooltip("Sets a minimum scale for the bars.")]
	public float barMinYScale = 0.1f;

	[Tooltip("The prefab of bar to use when building. Choose one from SimpleSpectrum/Bar Prefabs, or refer to the documentation to use a custom prefab.")]
	public GameObject barPrefab;

	[Tooltip("Stretches the bars sideways.")]
	public float barXScale = 1f;

	[Tooltip("Increases the spacing between bars.")]
	public float barXSpacing;

	[Range(0f, 360f)]
	[Tooltip("Bends the Spectrum using a given angle. Set to 360 for a circle.")]
	public float barCurveAngle;

	[Tooltip("Rotates the Spectrum inwards or outwards. Especially useful when using barCurveAngle.")]
	public float barXRotation;

	[Range(0f, 1f)]
	[Tooltip("The amount of dampening used when the new scale is higher than the bar's existing scale.")]
	public float attackDamp = 0.3f;

	[Range(0f, 1f)]
	[Tooltip("The amount of dampening used when the new scale is lower than the bar's existing scale.")]
	public float decayDamp = 0.15f;

	[Tooltip("Determines whether to apply a color gradient on the bars, or just use a solid color.")]
	public bool useColorGradient;

	[Tooltip("The minimum (low value) color if useColorGradient is true, else the solid color to use.")]
	public Color colorMin = Color.black;

	[Tooltip("The maximum (high value) color.")]
	public Color colorMax = Color.white;

	[Tooltip("The curve that determines the interpolation between colorMin and colorMax.")]
	public AnimationCurve colorValueCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

	[Range(0f, 1f)]
	[Tooltip("The amount of dampening used when the new color value is higher than the existing color value.")]
	public float colorAttackDamp = 1f;

	[Range(0f, 1f)]
	[Tooltip("The amount of dampening used when the new color value is lower than the existing color value.")]
	public float colorDecayDamp = 1f;

	private float[] spectrum;

	private Transform[] bars;

	private Material[] barMaterials;

	private float[] oldYScales;

	private float[] oldColorValues;

	private int materialValId;

	private bool materialColourCanBeUsed = true;

	private float highestLogFreq;

	private float frequencyScaleFactor;

	private string microphoneName;

	public bool animate = true;

	public int perspectivePreset;

	public float t;

	public float e;

	public float newYScaleQuadraticConstant = 0.1f;

	public float leftTop;

	public float middleTop;

	public float rightTop;

	public float leftBottom;

	public float middleBottom;

	public float rightBottom;

	private float speed = 5f / 32f;

	private float freqLim;

	private float[] trueSampleIndexes;

	public float[] spectrumInputData
	{
		get
		{
			return spectrum;
		}
		set
		{
			if (sourceType == SourceType.Custom)
			{
				spectrum = value;
			}
			else
			{
				Debug.LogError("Error from SimpleSpectrum: spectrumInputData cannot be set while sourceType is not Custom.");
			}
		}
	}

	public float[] spectrumOutputData => oldYScales;

	private void Start()
	{
		if (audioSource == null && sourceType == SourceType.AudioSource)
		{
			Debug.LogError("An audio source has not been assigned. Please assign a reference to a source, or set useAudioListener instead.");
		}
		RebuildSpectrum();
	}

	public void RebuildSpectrum()
	{
		isEnabled = false;
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			UnityEngine.Object.Destroy(base.transform.GetChild(i).gameObject);
		}
		RestartMicrophone();
		numSamples = Mathf.ClosestPowerOfTwo(numSamples);
		spectrum = new float[numSamples];
		bars = new Transform[barAmount];
		barMaterials = new Material[barAmount];
		oldYScales = new float[barAmount];
		oldColorValues = new float[barAmount];
		materialColourCanBeUsed = true;
		float num = (float)barAmount * (1f + barXSpacing);
		float num2 = num / 2f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		Vector3 vector = Vector3.zero;
		if (barCurveAngle > 0f)
		{
			num3 = barCurveAngle / 360f * (MathF.PI * 2f);
			num4 = num / num3;
			num5 = num3 / 2f;
			num6 = barCurveAngle / 2f;
			vector = new Vector3(0f, 0f, 1f * (0f - num4));
			if (barCurveAngle == 360f)
			{
				vector = new Vector3(0f, 0f, 0f);
			}
		}
		for (int j = 0; j < barAmount; j++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(barPrefab, base.transform, worldPositionStays: false);
			gameObject.transform.localScale = new Vector3(barXScale, barMinYScale, 1f);
			if (barCurveAngle > 0f)
			{
				float num7 = (float)j / (float)barAmount;
				float f = num7 * num3 - num5;
				float y = num7 * barCurveAngle - num6;
				gameObject.transform.localPosition = new Vector3(Mathf.Sin(f) * num4, 0f, Mathf.Cos(f) * num4) + vector;
				gameObject.transform.localRotation = Quaternion.Euler(barXRotation, y, 0f);
			}
			else
			{
				gameObject.transform.localPosition = new Vector3((float)j * (1f + barXSpacing) - num2, 0f, 0f);
			}
			bars[j] = gameObject.transform;
			Renderer component = gameObject.transform.GetChild(0).GetComponent<Renderer>();
			if (component != null)
			{
				barMaterials[j] = component.material;
			}
			else
			{
				Image component2 = gameObject.transform.GetChild(0).GetComponent<Image>();
				if (component2 != null)
				{
					component2.material = new Material(component2.material);
					barMaterials[j] = component2.material;
				}
				else if (materialColourCanBeUsed)
				{
					Debug.LogWarning("Warning from SimpleSpectrum: The Bar Prefab you're using doesn't have a Renderer or Image component as its first child. Dynamic colouring will not work.");
					materialColourCanBeUsed = false;
				}
			}
			int nameID = Shader.PropertyToID("_Color1");
			int nameID2 = Shader.PropertyToID("_Color2");
			barMaterials[j].SetColor(nameID, colorMin);
			barMaterials[j].SetColor(nameID2, colorMax);
		}
		materialValId = Shader.PropertyToID("_Val");
		highestLogFreq = Mathf.Log(barAmount + 1, 2f);
		frequencyScaleFactor = 1f / (float)(AudioSettings.outputSampleRate / 2) * (float)numSamples;
		freqLim = frequencyLimitHigh;
		trueSampleIndexes = new float[bars.Length];
		for (int k = 0; k < bars.Length; k++)
		{
			if (useLogarithmicFrequency)
			{
				trueSampleIndexes[k] = Mathf.Lerp(frequencyLimitLow, freqLim, (highestLogFreq - Mathf.Log(barAmount + 1 - k, 2f)) / highestLogFreq) * frequencyScaleFactor;
			}
			else
			{
				trueSampleIndexes[k] = Mathf.Lerp(frequencyLimitLow, freqLim, (float)k / (float)barAmount) * frequencyScaleFactor;
			}
		}
		isEnabled = true;
	}

	public void RestartMicrophone()
	{
		if (sourceType == SourceType.MicrophoneInput || sourceType == SourceType.StereoMix || sourceType == SourceType.AudioSource)
		{
			Debug.LogError("Error from SimpleSpectrum: Microphone, Stereo Mix or AudioSource cannot be used in WebGL!");
		}
	}

	private void Update()
	{
		if (!isEnabled)
		{
			return;
		}
		if (sourceType != SourceType.Custom)
		{
			if (sourceType == SourceType.AudioListener)
			{
				AudioListener.GetSpectrumData(spectrum, sampleChannel, windowUsed);
			}
			else
			{
				audioSource.GetSpectrumData(spectrum, sampleChannel, windowUsed);
			}
		}
		if (animate)
		{
			t = (t + Time.deltaTime * speed) % 1f;
		}
		for (int i = 0; i < bars.Length; i++)
		{
			Transform obj = bars[i];
			float num = trueSampleIndexes[i];
			int value = Mathf.FloorToInt(num);
			value = Mathf.Clamp(value, 0, spectrum.Length - 2);
			float num2 = Mathf.SmoothStep(spectrum[value], spectrum[value + 1], num - (float)value);
			if (multiplyByFrequency)
			{
				num2 *= num + 1f;
			}
			num2 = Mathf.Sqrt(num2);
			float num3 = oldYScales[i];
			float f = Mathf.Lerp(t: (num2 * barYScale > num3) ? attackDamp : decayDamp, a: num3, b: Mathf.Max(num2 * barYScale, barMinYScale));
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = i;
			float f2 = ((perspectivePreset == 1) ? t : (0f - t));
			float num8 = 119f;
			if (perspectivePreset == 1)
			{
				leftTop = 159f;
				middleTop = 150f;
				rightTop = 177f;
				leftBottom = 69f;
				middleBottom = 82f;
				rightBottom = 61f;
			}
			else
			{
				leftTop = 69f;
				middleTop = 82f;
				rightTop = 61f;
				leftBottom = -20f;
				middleBottom = 16f;
				rightBottom = -53f;
			}
			if (num7 <= num8)
			{
				float num9 = (num7 - 0f) / num8;
				num5 = Mathf.Lerp(leftBottom, middleBottom, num9);
				num6 = Mathf.Lerp(leftTop, middleTop, num9);
				num4 = Mathf.LerpUnclamped(num6 + e, num5 - e, (perspectivePreset == 0) ? (1f - t) : t);
			}
			else
			{
				float num10 = (num7 - num8) / (352f - num8);
				num5 = Mathf.Lerp(middleBottom, rightBottom, num10);
				num6 = Mathf.Lerp(middleTop, rightTop, num10);
				num4 = Mathf.LerpUnclamped(num6 + e, num5 - e, (perspectivePreset == 0) ? (1f - t) : t);
			}
			obj.localPosition = new Vector3(num7 - 176f, Mathf.Clamp(num4 + Mathf.Sign(f2) * Mathf.Round(f), num5, num6), 0f);
		}
	}

	public static float[] GetLogarithmicSpectrumData(AudioSource source, int spectrumSize, int sampleSize, FFTWindow windowUsed = FFTWindow.BlackmanHarris, int channelUsed = 0)
	{
		float[] array = new float[spectrumSize];
		channelUsed = Mathf.Clamp(channelUsed, 0, 1);
		float[] array2 = new float[Mathf.ClosestPowerOfTwo(sampleSize)];
		source.GetSpectrumData(array2, channelUsed, windowUsed);
		float num = Mathf.Log(array.Length + 1, 2f);
		float num2 = (float)sampleSize / num;
		for (int i = 0; i < array.Length; i++)
		{
			float num3 = (num - Mathf.Log(array.Length + 1 - i, 2f)) * num2;
			int value = Mathf.FloorToInt(num3);
			value = Mathf.Clamp(value, 0, array2.Length - 2);
			float num4 = Mathf.SmoothStep(array[value], array[value + 1], num3 - (float)value);
			num4 *= num3;
			num4 = Mathf.Sqrt(num4);
			array[i] = num4;
		}
		return array;
	}

	public static float[] GetLogarithmicSpectrumData(int spectrumSize, int sampleSize, FFTWindow windowUsed = FFTWindow.BlackmanHarris, int channelUsed = 0)
	{
		float[] array = new float[spectrumSize];
		channelUsed = Mathf.Clamp(channelUsed, 0, 1);
		float[] array2 = new float[Mathf.ClosestPowerOfTwo(sampleSize)];
		AudioListener.GetSpectrumData(array2, channelUsed, windowUsed);
		float num = Mathf.Log(array.Length + 1, 2f);
		float num2 = (float)sampleSize / num;
		for (int i = 0; i < array.Length; i++)
		{
			float num3 = (num - Mathf.Log(array.Length + 1 - i, 2f)) * num2;
			int value = Mathf.FloorToInt(num3);
			value = Mathf.Clamp(value, 0, array2.Length - 2);
			float num4 = Mathf.SmoothStep(array[value], array[value + 1], num3 - (float)value);
			num4 *= num3 + 1f;
			num4 = Mathf.Sqrt(num4);
			array[i] = num4;
		}
		return array;
	}
}
