using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class SoundPlayer : MonoBehaviour
{
	private AudioSource audiosrc;

	public void SetDefaultValues()
	{
		base.hideFlags = HideFlags.HideAndDontSave;
		audiosrc = GetComponent<AudioSource>();
	}

	public void Play(AudioClip clip, float volume, float minPitch, float maxPitch)
	{
		audiosrc.Stop();
		audiosrc.pitch = Random.Range(minPitch, maxPitch);
		audiosrc.clip = clip;
		audiosrc.volume = volume;
		audiosrc.Play();
		StopAllCoroutines();
		StartCoroutine(DestroySoundPlayer());
	}

	public void Play(AudioClip clip, float volume)
	{
		audiosrc.Stop();
		audiosrc.pitch = 1f;
		audiosrc.clip = clip;
		audiosrc.volume = volume;
		audiosrc.Play();
		StopAllCoroutines();
		StartCoroutine(DestroySoundPlayer());
	}

	public void Stop()
	{
		audiosrc.Stop();
	}

	private IEnumerator DestroySoundPlayer()
	{
		do
		{
			yield return new WaitForSecondsRealtime(0.1f);
		}
		while (audiosrc.isPlaying);
		Object.DestroyImmediate(base.gameObject, allowDestroyingAssets: false);
	}
}
