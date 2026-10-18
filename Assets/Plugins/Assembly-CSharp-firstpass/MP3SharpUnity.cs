using System;
using System.Collections.Generic;
using MP3Sharp;
using UnityEngine;

public class MP3SharpUnity : MonoBehaviour
{
	private const int ChunkSize = 4096;

	private const int MinimumStreamPosDiff = 1000;

	public const float UPDATE_DATA_TIME = 0.03f;

	public static AudioClip CreateAudioClipUsingMP3File(string path)
	{
		List<float> list = new List<float>();
		MP3Stream mP3Stream = new MP3Stream(path);
		byte[] array = new byte[4096];
		_ = DateTime.UtcNow;
		int num = 4096;
		int num2 = ((mP3Stream.ChannelCount != 2) ? 2 : 0);
		while (num == 4096 && mP3Stream.Length - mP3Stream.Position >= 1000)
		{
			num = mP3Stream.Read(array, 0, 4096);
			for (int i = 0; i < num; i += 2 + num2)
			{
				byte b = array[i];
				float item = (float)(short)((array[i + 1] << 8) | b) * 1f / 32767f;
				list.Add(item);
			}
		}
		int frequency = mP3Stream.Frequency;
		short channelCount = mP3Stream.ChannelCount;
		int lengthSamples = list.Count / channelCount;
		AudioClip audioClip = AudioClip.Create("audio", lengthSamples, channelCount, frequency, stream: false);
		audioClip.SetData(list.ToArray(), 0);
		mP3Stream.Close();
		return audioClip;
	}

	public static AudioClip UpdateMP3StreamData(MP3Stream stream, ref int samples, AudioClip clip)
	{
		DateTime utcNow = DateTime.UtcNow;
		string text = ((clip == null) ? "Audio" : clip.name);
		byte[] array = new byte[4096];
		int num = 4096;
		float[] array2 = new float[Mathf.RoundToInt(0.03f * (float)stream.Frequency * (float)stream.ChannelCount * 2f)];
		int num2 = 0;
		while (num == 4096 && stream.Length - stream.Position >= 1000)
		{
			num = stream.Read(array, 0, 4096);
			for (int i = 0; i < num; i += 2)
			{
				byte b = array[i];
				float num3 = (float)(short)((array[i + 1] << 8) | b) * 1f / 32767f;
				array2[num2] = num3;
				num2++;
			}
			if ((float)num2 >= 0.03f * (float)stream.Frequency * (float)stream.ChannelCount)
			{
				break;
			}
		}
		int frequency = stream.Frequency;
		short channelCount = stream.ChannelCount;
		if (clip == null)
		{
			clip = AudioClip.Create(text, (int)stream.Length * 10, channelCount, frequency, stream: false);
		}
		int offsetSamples = Mathf.RoundToInt((float)samples / 2f);
		clip.SetData(array2, offsetSamples);
		samples += num2;
		if (num != 4096)
		{
			stream.Close();
		}
		MonoBehaviour.print("taken: " + (DateTime.UtcNow - utcNow).TotalSeconds);
		return clip;
	}
}
