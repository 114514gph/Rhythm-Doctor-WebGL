using System;
using DG.Tweening;
using UnityEngine;

public class scrCareLessParticles : MonoBehaviour
{
	public ParticleSystem upperSystem;

	public ParticleSystem lowerSystem;

	public ParticleSystem rainSystem;

	public ParticleSystem[] spiralSystems;

	public Transform spiralTrans;

	public Sprite[] musicSprites;

	private void Awake()
	{
		uint randomSeed = Convert.ToUInt32(UnityEngine.Random.Range(0, 50));
		upperSystem.randomSeed = randomSeed;
		lowerSystem.randomSeed = randomSeed;
	}

	private void ReplaceSystemSprite(ParticleSystem targetSystem, int index)
	{
		targetSystem.textureSheetAnimation.SetSprite(index, musicSprites[index]);
		targetSystem.Simulate(targetSystem.time, withChildren: true);
		targetSystem.Play();
	}

	public void ReplaceSwarmSprite(int index)
	{
		ReplaceSystemSprite(upperSystem, index);
		ReplaceSystemSprite(lowerSystem, index);
	}

	private void DropSystem(ParticleSystem targetSystem, float finalSpeed, float duration)
	{
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[targetSystem.main.maxParticles];
		bool xVelSet = false;
		bool settingXVel = false;
		DOVirtual.Float(0f, 1f, duration, (float t) =>
		{
			int particles2 = targetSystem.GetParticles(particles);
			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].velocity = particles[i].velocity.WithY((0f - finalSpeed) * t);
				if (!xVelSet)
				{
					particles[i].velocity += Vector3.right * UnityEngine.Random.Range(-40f, 40f);
					settingXVel = true;
				}
			}
			if (settingXVel)
			{
				xVelSet = true;
				settingXVel = false;
			}
			targetSystem.SetParticles(particles, particles2);
		}).OnComplete(() =>
		{
			targetSystem.gameObject.SetActive(value: false);
		});
		targetSystem.Stop();
	}

	public void DropSwarm(float finalSpeed, float duration)
	{
		DropSystem(upperSystem, finalSpeed, duration);
		DropSystem(lowerSystem, finalSpeed, duration);
	}

	public void ToggleAllSystemsActive(bool toggleActive)
	{
		upperSystem.gameObject.SetActive(toggleActive);
		lowerSystem.gameObject.SetActive(toggleActive);
		rainSystem.gameObject.SetActive(toggleActive);
		ParticleSystem[] array = spiralSystems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(toggleActive);
		}
	}
}
