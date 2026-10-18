using UnityEngine;

public class AudioObserver : MonoBehaviour
{
	private void Update()
	{
		AudioSource[] array = Object.FindObjectsOfType<AudioSource>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.GetInstanceID();
		}
	}
}
