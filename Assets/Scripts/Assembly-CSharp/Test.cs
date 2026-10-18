using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Test : MonoBehaviour
{
	private RDMP3Stream stream;

	private void Start()
	{
		stream = new RDMP3Stream("/Users/giacomo/Desktop/07 _En Gallop_.mp3");
		AudioClip clip = stream.CreateMP3Stream();
		AudioSource component = GetComponent<AudioSource>();
		component.clip = clip;
		component.Play();
	}

	private void Update()
	{
	}
}
