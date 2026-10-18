using DG.Tweening;
using Kino;
using UnityEngine;

public class RDKaleidoscope : MonoBehaviour
{
	public float speed;

	public ParticleSystem[] particleSystems;

	private Material[] particleMats;

	private scrVolumeTracker volumeTracker;

	public Camera kaleidoCam;

	private Mirror mirrorComp;

	private Tween offsetTween;

	private Tween rollTween;

	private Tween colorATween;

	private Tween colorBTween;

	public RDRoom room;

	public float colorPowerA = 30f;

	public float colorPowerB = 30f;

	public Color colorA = new Color(1f, 0f, 0.4f, 1f);

	public Color colorB = new Color(0f, 0.4f, 1f, 1f);

	private void Awake()
	{
		mirrorComp = kaleidoCam.GetComponent<Mirror>();
		particleMats = new Material[particleSystems.Length];
		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleMats[i] = particleSystems[i].GetComponent<Renderer>().material;
		}
	}

	private void Update()
	{
		SetBgColor(room.camera.backgroundColor);
	}

	private void LateUpdate()
	{
		kaleidoCam.enabled = room.camera.enabled;
	}

	public void ToggleVolumeTracker(bool on)
	{
		if (volumeTracker != null)
		{
			volumeTracker.Delete(1f);
		}
		if (!on)
		{
			return;
		}
		scnGame instance = scnGame.instance;
		int num;
		float num2;
		if ((object)instance == null)
		{
			num = 0;
		}
		else
		{
			num = (instance.reducedFlash ? 1 : 0);
			if (num != 0)
			{
				num2 = 0.035f;
				goto IL_0046;
			}
		}
		num2 = 0.03f;
		goto IL_0046;
		IL_0046:
		float min = num2;
		float max = ((num == 0) ? 0.3f : 0.18f);
		volumeTracker = scrVolumeTracker.Add((float x) =>
		{
			Color color = (x * colorA * 10f).WithAlpha(1f);
			Color color2 = (x * colorB * 10f).WithAlpha(1f);
			SetColor(0, color, x * colorPowerA);
			SetColor(1, color2, x * colorPowerB);
		}, min, max);
	}

	public void SetColor(int index, Color color, float brightness)
	{
		if (index < particleMats.Length)
		{
			Material obj = particleMats[index];
			obj.color = color;
			Color value = new Color(color.r * brightness, color.g * brightness, color.b * brightness);
			obj.SetColor("_EmissionColor", value);
		}
	}

	public void SetColorA(Color color, float durationBeats, Ease ease)
	{
		colorATween?.Kill();
		float duration = scrConductor.instance.DurationBeatsToTime(durationBeats);
		colorATween = DOTween.To(() => colorA, (Color x) =>
		{
			colorA = x;
		}, color, duration).SetEase(ease).Done();
	}

	public void SetColorB(Color color, float durationBeats, Ease ease)
	{
		colorBTween?.Kill();
		float duration = scrConductor.instance.DurationBeatsToTime(durationBeats);
		colorBTween = DOTween.To(() => colorB, (Color x) =>
		{
			colorB = x;
		}, color, duration).SetEase(ease).Done();
	}

	public void SetSpeed(float speed)
	{
		this.speed = speed;
		ParticleSystem[] array = particleSystems;
		foreach (ParticleSystem particleSystem in array)
		{
			ParticleSystem.VelocityOverLifetimeModule velocityOverLifetime = particleSystem.velocityOverLifetime;
			velocityOverLifetime.z = new ParticleSystem.MinMaxCurve(0f - speed);
			ParticleSystem.Particle[] array2 = new ParticleSystem.Particle[particleSystem.particleCount + 1];
			int particles = particleSystem.GetParticles(array2);
			for (int j = 0; j < particles; j++)
			{
				array2[j].velocity = new Vector3(0f, 0f, 0f - speed);
			}
			particleSystem.SetParticles(array2, particles);
		}
	}

	public void SetEmissionRate(float rate = 100f)
	{
		ParticleSystem[] array = particleSystems;
		foreach (ParticleSystem obj in array)
		{
			ParticleSystem.MinMaxCurve rateOverTime = new ParticleSystem.MinMaxCurve(rate);
			ParticleSystem.EmissionModule emission = obj.emission;
			emission.rateOverTime = rateOverTime;
		}
	}

	public void SetGravity(float grav)
	{
		ParticleSystem[] array = particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem.MainModule main = array[i].main;
			main.gravityModifier = new ParticleSystem.MinMaxCurve(grav);
		}
	}

	public void SetSimulationSpeed(float speed)
	{
		ParticleSystem[] array = particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem.MainModule main = array[i].main;
			main.simulationSpeed = speed;
		}
	}

	public void EnableRotationOverLifetime(bool activ)
	{
		ParticleSystem[] array = particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem.RotationOverLifetimeModule rotationOverLifetime = array[i].rotationOverLifetime;
			rotationOverLifetime.enabled = activ;
		}
	}

	public void SetBgColor(Color color)
	{
		kaleidoCam.backgroundColor = color;
	}

	public void SetRepeat(int repeat)
	{
		mirrorComp._repeat = repeat;
	}

	public void SetOffset(float offset, float durationBeats, Ease ease)
	{
		offsetTween?.Kill();
		float duration = scrConductor.instance.DurationBeatsToTime(durationBeats);
		offsetTween = DOTween.To(() => mirrorComp._offset, (float x) =>
		{
			mirrorComp._offset = x;
		}, offset, duration).SetEase(ease);
	}

	public void SetRoll(float roll, float durationBeats, Ease ease)
	{
		rollTween?.Kill();
		float duration = scrConductor.instance.DurationBeatsToTime(durationBeats);
		rollTween = DOTween.To(() => mirrorComp._roll, (float x) =>
		{
			mirrorComp._roll = x;
		}, roll, duration).SetEase(ease);
	}

	public bool ToggleSymmetry(bool on)
	{
		return mirrorComp._symmetry = on;
	}

	public void SyncSeed(RDKaleidoscope other)
	{
		for (int i = 0; i < particleSystems.Length; i++)
		{
			ParticleSystem obj = particleSystems[i];
			ParticleSystem particleSystem = other.particleSystems[i];
			particleSystem.Pause();
			obj.Pause();
			obj.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
			obj.randomSeed = particleSystem.randomSeed;
			obj.Simulate(particleSystem.time, withChildren: true);
			obj.Play();
			particleSystem.Play();
		}
	}
}
