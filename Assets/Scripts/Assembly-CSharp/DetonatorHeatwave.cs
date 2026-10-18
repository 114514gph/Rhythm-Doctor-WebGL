using UnityEngine;

[RequireComponent(typeof(Detonator))]
[AddComponentMenu("Detonator/Heatwave (Pro Only)")]
public class DetonatorHeatwave : DetonatorComponent
{
	public float distortionMultiplier = 1f;

	private GameObject _heatwave;

	private float s;

	private float _startSize;

	private float _maxSize;

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public float zOffset = 0.5f;

	public float distortion = 64f;

	private float _elapsedTime;

	private float _normalizedTime;

	public Material heatwaveMaterial;

	private Material _material;

	public bool dead = true;

	public override void Init()
	{
	}

	private void Awake()
	{
		Setup();
	}

	private void Setup()
	{
		_startSize = 0f;
		_maxSize = size * 10f;
		_material = new Material(Shader.Find("HeatDistort"));
		_heatwave = GameObject.CreatePrimitive(PrimitiveType.Plane);
		_heatwave.name = "Heatwave";
		_heatwave.layer = GC.EffectsLayer;
		_heatwave.transform.parent = base.transform;
		Object.Destroy(_heatwave.GetComponent<MeshCollider>());
		if (!heatwaveMaterial)
		{
			heatwaveMaterial = MyDetonator().heatwaveMaterial;
		}
		_material.CopyPropertiesFromMaterial(heatwaveMaterial);
		_heatwave.GetComponent<Renderer>().material = _material;
		_heatwave.transform.parent = base.transform;
		_delayedExplosionStarted = false;
		_explodeDelay = 0f;
	}

	private void Update()
	{
		if (_delayedExplosionStarted)
		{
			_explodeDelay -= Time.deltaTime;
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
		if ((bool)_heatwave)
		{
			_heatwave.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
			_heatwave.transform.localPosition = localPosition + Vector3.forward * zOffset;
			_elapsedTime += Time.deltaTime;
			_normalizedTime = _elapsedTime / (duration * durationMultiplier);
			s = Mathf.Lerp(_startSize, _maxSize, _normalizedTime);
			_heatwave.GetComponent<Renderer>().material.SetFloat("_BumpAmt", (1f - _normalizedTime) * distortion * distortionMultiplier);
			_heatwave.gameObject.transform.localScale = new Vector3(s, s, s);
			if (_elapsedTime > duration * durationMultiplier)
			{
				dead = true;
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public override void Explode()
	{
		Rewind();
		if ((!(_heatwave != null) || !MyDetonator().cached) && !(detailThreshold > detail) && on)
		{
			if (!_delayedExplosionStarted)
			{
				_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
			}
			if (_explodeDelay <= 0f)
			{
				dead = false;
				Setup();
			}
			else
			{
				_delayedExplosionStarted = true;
			}
		}
	}

	public void Reset()
	{
	}

	public void Rewind()
	{
		dead = false;
		base.gameObject.SetActive(value: true);
		s = 0f;
		_startSize = 0f;
		_maxSize = size * 10f;
		_delayedExplosionStarted = false;
		_explodeDelay = 0f;
		_elapsedTime = 0f;
		_normalizedTime = 0f;
	}
}
