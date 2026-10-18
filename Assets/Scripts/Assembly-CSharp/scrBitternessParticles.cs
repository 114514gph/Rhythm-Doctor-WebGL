using UnityEngine;

public class scrBitternessParticles : MonoBehaviour
{
	public ParticleSystem rainSystem;

	public ParticleSystem fastRainSystem;

	public ParticleSystem burstSystem;

	public void ToggleAllSystemsActive(bool toggleActive)
	{
		rainSystem.gameObject.SetActive(toggleActive);
		fastRainSystem.gameObject.SetActive(toggleActive);
		burstSystem.gameObject.SetActive(toggleActive);
	}
}
