using UnityEngine;
using UnityEngine.UI;

public class OutputVolume : MonoBehaviour
{
	public enum SourceType
	{
		AudioSource = 0,
		AudioListener = 1,
		Custom = 2
	}

	public enum OutputType
	{
		PrefabBar = 0,
		ObjectPosition = 1,
		ObjectRotation = 2,
		ObjectScale = 3
	}

	[Tooltip("Enables or disables the processing and display of volume data.")]
	public bool isEnabled = true;

	[Tooltip("The type of source for volume data.")]
	public SourceType sourceType;

	[Tooltip("The AudioSource to take data from.")]
	public AudioSource audioSource;

	[Tooltip("The number of samples to use when sampling. Must be a power of two.")]
	public int sampleAmount = 256;

	[Tooltip("The audio channel to take data from when sampling.")]
	public int channel;

	[Range(0f, 1f)]
	[Tooltip("The amount of dampening used when the new scale is higher than the bar's existing scale.")]
	public float attackDamp = 0.75f;

	[Range(0f, 1f)]
	[Tooltip("The amount of dampening used when the new scale is lower than the bar's existing scale.")]
	public float decayDamp = 0.25f;

	[Tooltip("How the volume data should be presented to the user.")]
	public OutputType outputType;

	[Tooltip("A multiplier / mask for positioning or rotating. The volume data is multiplied by this vector, so 0 will mask that dimension out.")]
	public Vector3 valueMultiplier = new Vector3(0f, 0f, -90f);

	[Tooltip("The scale used when output volume is lowest (0).")]
	public float outputScaleMin = 1f;

	[Tooltip("The scale used when output volume is highest (1).")]
	public float outputScaleMax = 1.5f;

	[Tooltip("The prefab of bar to use. Use a prefab from SimpleSpectrum/Bar Prefabs or refer to the documentation to use a custom prefab.")]
	public GameObject prefab;

	[Tooltip("Determines whether to scale the bar prefab (i.e. disable for just colouring).")]
	public bool scalePrefab = true;

	[Tooltip("Determines whether to apply a color gradient on the bar.")]
	public bool useColorGradient;

	[Tooltip("The minimum (low value) color.")]
	public Color MinColor = Color.black;

	[Tooltip("The maximum (high value) color.")]
	public Color MaxColor = Color.white;

	[Tooltip("The curve that determines the interpolation between Color Min and Color Max")]
	public AnimationCurve colorCurve;

	[Range(0f, 1f)]
	[Tooltip("The amount of dampening used when the new color value is higher than the existing color value.")]
	public float colorAttackDamp = 1f;

	[Range(0f, 1f)]
	[Tooltip("The amount of dampening used when the new color value is lower than the existing color value.")]
	public float colorDecayDamp = 1f;

	private GameObject bar;

	private Transform barT;

	private float newValue;

	private float oldScale;

	private float oldColorVal;

	private Material mat;

	private int mat_ValId;

	private bool materialColourCanBeUsed = true;

	public float inputValue
	{
		set
		{
			if (sourceType == SourceType.Custom)
			{
				newValue = value;
			}
			else
			{
				Debug.LogError("Error from OutputVolume: inputValue cannot be set while sourceType is not Custom.");
			}
		}
	}

	public float outputValue => oldScale;

	private void Start()
	{
		if (outputType != OutputType.PrefabBar)
		{
			return;
		}
		bar = Object.Instantiate(prefab);
		barT = bar.transform;
		barT.SetParent(base.transform, worldPositionStays: false);
		barT.localPosition = Vector3.zero;
		Renderer component = barT.GetChild(0).GetComponent<Renderer>();
		if (component != null)
		{
			mat = component.material;
		}
		else
		{
			Image component2 = barT.GetChild(0).GetComponent<Image>();
			if (component2 != null)
			{
				component2.material = new Material(component2.material);
				mat = component2.material;
			}
			else
			{
				Debug.LogWarning("Warning from OutputVolume: The Bar Prefab you're using doesn't have a Renderer or Image component as its first child. Dynamic colouring will not work.");
				materialColourCanBeUsed = false;
			}
		}
		mat_ValId = Shader.PropertyToID("_Val");
		mat.SetColor("_Color1", MinColor);
		mat.SetColor("_Color2", MaxColor);
	}

	private void Update()
	{
		if (isEnabled && sourceType != SourceType.Custom)
		{
			if (sourceType == SourceType.AudioListener)
			{
				newValue = GetRMS(sampleAmount, channel);
			}
			else
			{
				newValue = GetRMS(audioSource, sampleAmount, channel);
			}
		}
		float num = (oldScale = ((newValue > oldScale) ? Mathf.Lerp(oldScale, newValue, attackDamp) : Mathf.Lerp(oldScale, newValue, decayDamp)));
		switch (outputType)
		{
		case OutputType.PrefabBar:
		{
			if (scalePrefab)
			{
				barT.localScale = new Vector3(1f, num, 1f);
			}
			if (!useColorGradient || !materialColourCanBeUsed)
			{
				break;
			}
			float num3 = colorCurve.Evaluate(num);
			if (num3 > oldColorVal)
			{
				if (colorAttackDamp != 1f)
				{
					num3 = Mathf.Lerp(oldColorVal, num3, colorAttackDamp);
				}
			}
			else if (colorDecayDamp != 1f)
			{
				num3 = Mathf.Lerp(oldColorVal, num3, colorDecayDamp);
			}
			mat.SetFloat(mat_ValId, num3);
			oldColorVal = num3;
			break;
		}
		case OutputType.ObjectPosition:
			base.transform.localPosition = valueMultiplier * num;
			break;
		case OutputType.ObjectRotation:
			base.transform.localEulerAngles = valueMultiplier * num;
			break;
		case OutputType.ObjectScale:
		{
			float num2 = Mathf.Lerp(outputScaleMin, outputScaleMax, num);
			base.transform.localScale = new Vector3(num2, num2, num2);
			break;
		}
		}
	}

	public static float GetRMS(AudioSource aSource, int sampleSize, int channelUsed = 0)
	{
		sampleSize = Mathf.ClosestPowerOfTwo(sampleSize);
		float[] array = new float[sampleSize];
		aSource.GetOutputData(array, channelUsed);
		float num = 0f;
		float[] array2 = array;
		foreach (float num2 in array2)
		{
			num += num2 * num2;
		}
		return Mathf.Sqrt(num / (float)array.Length);
	}

	public static float GetRMS(int sampleSize, int channelUsed = 0)
	{
		sampleSize = Mathf.ClosestPowerOfTwo(sampleSize);
		float[] array = new float[sampleSize];
		AudioListener.GetOutputData(array, channelUsed);
		float num = 0f;
		float[] array2 = array;
		foreach (float num2 in array2)
		{
			num += num2 * num2;
		}
		return Mathf.Sqrt(num / (float)array.Length);
	}
}
