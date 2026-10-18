using System;
using UnityEngine;

public class SongLoop
{
	private const int RescaleFactor = 32767;

	private const float Padding = 0f;

	public AudioClip clip;

	public AudioSource audioSource;

	private int samplesRead;

	private int bytesRead;

	private byte[] audioClipBytes;

	private void Start()
	{
		audioClipBytes = AudioclipToByteArray(clip);
		AudioClip audioClip = AudioClip.Create(clip.name, clip.samples, clip.channels, clip.frequency, stream: true, PCMReaderCallback, PCMSetPositionCallback);
		audioSource.clip = audioClip;
		audioSource.Play();
		samplesRead = 0;
	}

	public byte[] AudioclipToByteArray(AudioClip clip)
	{
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		float[] array2 = new float[array.Length];
		Array.Copy(array, 0, array2, 0, array.Length);
		byte[] array3 = new byte[array2.Length * 2];
		for (int i = 0; i < array2.Length; i++)
		{
			short value = (short)((!(array2[i] > 1f)) ? ((!(array2[i] < -1f)) ? ((short)(array2[i] * 32767f)) : (-32767)) : short.MaxValue);
			BitConverter.GetBytes(value).CopyTo(array3, i * 2);
		}
		return array3;
	}

	public void PCMReaderCallback(float[] data)
	{
		int num = data.Length * 2;
		byte[] array = new byte[num];
		int num2 = num;
		samplesRead = 0;
		bool flag = false;
		if (bytesRead + num > audioClipBytes.Length)
		{
			flag = true;
			int num3 = audioClipBytes.Length - bytesRead;
			int length = bytesRead + num - audioClipBytes.Length;
			Array.Copy(audioClipBytes, bytesRead, array, 0, num3);
			Array.Copy(audioClipBytes, 0, array, num3, length);
		}
		else
		{
			Array.Copy(audioClipBytes, bytesRead, array, 0, num);
		}
		num2 = num;
		for (int i = 0; i < num2; i += 2)
		{
			byte b = array[i];
			float num4 = (float)(short)((array[i + 1] << 8) | b) * 1f / 32767f;
			data[samplesRead] = num4;
			samplesRead++;
		}
		bytesRead = (flag ? (bytesRead + num - audioClipBytes.Length) : (bytesRead + num));
	}

	public void PCMSetPositionCallback(int position)
	{
		samplesRead = position;
	}
}
