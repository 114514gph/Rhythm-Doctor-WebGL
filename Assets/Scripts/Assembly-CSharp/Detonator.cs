using UnityEngine;

[AddComponentMenu("Detonator/Detonator")]
public class Detonator : MonoBehaviour
{
	private static float _baseSize = 30f;

	private static Color _baseColor = new Color(1f, 0.423f, 0f, 0.5f);

	private static float _baseDuration = 3f;

	public float size = 10f;

	public Color color = _baseColor;

	public bool explodeOnStart = true;

	public float duration = _baseDuration;

	public float detail = 1f;

	public float upwardsBias;

	public float destroyTime = 7f;

	public bool cached;

	public bool useWorldSpace = true;

	public Vector3 direction = Vector3.zero;

	public Material heatwaveMaterial;

	private Component[] components;

	private DetonatorHeatwave _heatwave;

	public bool autoCreateHeatwave;

	private float _lastExplosionTime = 1000f;

	private bool _firstComponentUpdate = true;

	private Component[] _subDetonators;

	public static Material defaultHeatwaveMaterial;

	private void Awake()
	{
		FillDefaultMaterials();
		components = GetComponents(typeof(DetonatorComponent));
		Component[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			DetonatorComponent detonatorComponent = (DetonatorComponent)array[i];
			if (detonatorComponent is DetonatorHeatwave)
			{
				_heatwave = detonatorComponent as DetonatorHeatwave;
			}
		}
		if (!_heatwave && autoCreateHeatwave)
		{
			_heatwave = base.gameObject.AddComponent<DetonatorHeatwave>();
			_heatwave.Reset();
		}
		components = GetComponents(typeof(DetonatorComponent));
	}

	private void FillDefaultMaterials()
	{
		if (!heatwaveMaterial)
		{
			heatwaveMaterial = DefaultHeatwaveMaterial();
		}
	}

	private void Start()
	{
		if (explodeOnStart)
		{
			UpdateComponents();
			Explode();
		}
	}

	private void Update()
	{
		if (destroyTime > 0f && _lastExplosionTime + destroyTime <= Time.time && !cached)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void UpdateComponents()
	{
		Component[] array;
		if (_firstComponentUpdate)
		{
			array = components;
			for (int i = 0; i < array.Length; i++)
			{
				DetonatorComponent obj = (DetonatorComponent)array[i];
				obj.Init();
				obj.SetStartValues();
			}
			_firstComponentUpdate = false;
		}
		if (_firstComponentUpdate)
		{
			return;
		}
		float num = size / _baseSize;
		Vector3 vector = new Vector3(direction.x * num, direction.y * num, direction.z * num);
		float timeScale = duration / _baseDuration;
		array = components;
		for (int i = 0; i < array.Length; i++)
		{
			DetonatorComponent detonatorComponent = (DetonatorComponent)array[i];
			if (detonatorComponent.detonatorControlled)
			{
				detonatorComponent.size = detonatorComponent.startSize * detonatorComponent.sizeMultiplier * num;
				detonatorComponent.timeScale = timeScale;
				detonatorComponent.detail = detonatorComponent.startDetail * detail;
				detonatorComponent.force = new Vector3(detonatorComponent.startForce.x * num + vector.x, detonatorComponent.startForce.y * num + vector.y, detonatorComponent.startForce.z * num + vector.z);
				detonatorComponent.velocity = new Vector3(detonatorComponent.startVelocity.x * num + vector.x, detonatorComponent.startVelocity.y * num + vector.y, detonatorComponent.startVelocity.z * num + vector.z);
				detonatorComponent.color = Color.Lerp(detonatorComponent.startColor, color, color.a);
			}
		}
	}

	public void Explode()
	{
		_lastExplosionTime = Time.time;
		Component[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			DetonatorComponent obj = (DetonatorComponent)array[i];
			UpdateComponents();
			obj.Explode();
		}
	}

	public void Reset()
	{
		size = 10f;
		color = _baseColor;
		duration = _baseDuration;
		FillDefaultMaterials();
	}

	public static Material DefaultHeatwaveMaterial()
	{
		if (defaultHeatwaveMaterial != null)
		{
			return defaultHeatwaveMaterial;
		}
		defaultHeatwaveMaterial = new Material(Shader.Find("HeatDistort"));
		defaultHeatwaveMaterial.name = "Heatwave-Default";
		Texture2D value = Resources.Load("Detonator/Textures/Heatwave") as Texture2D;
		defaultHeatwaveMaterial.SetTexture("_BumpMap", value);
		return defaultHeatwaveMaterial;
	}
}
