using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SoundFileOpener : MonoBehaviour
{
	public InputField pathBox;

	private void Start()
	{
	}

	public void PlayFile()
	{
		AudioType audioType = RDUtils.GetAudioType(Path.GetExtension(pathBox.text));
		UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip(pathBox.text, audioType);
		while (!audioClip.isDone)
		{
		}
		if (audioClip.AnyErrors())
		{
			MonoBehaviour.print(audioClip.error);
		}
		DownloadHandlerAudioClip downloadHandlerAudioClip = audioClip.downloadHandler as DownloadHandlerAudioClip;
		GetComponent<AudioSource>().clip = downloadHandlerAudioClip.audioClip;
		GetComponent<AudioSource>().Play();
	}
}
