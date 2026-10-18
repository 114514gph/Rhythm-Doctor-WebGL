using MP3Sharp;
using UnityEngine;

public class RDMP3Stream
{
	private const int ChunkSize = 4096;

	private const int MinimumStreamPosDiff = 1000;

	public const float UPDATE_DATA_TIME = 0.03f;

	private string name;

	private int samplesRead;

	private MP3Stream stream;

	public RDMP3Stream(string clipName)
	{
		name = clipName;
		stream = new MP3Stream(name);
	}

	public AudioClip CreateMP3Stream()
	{
		int frequency = stream.Frequency;
		short channelCount = stream.ChannelCount;
		AudioClip audioClip = AudioClip.Create(name, (int)stream.Length * 10, channelCount, frequency, stream: true, OnAudioRead, OnAudioSetPosition);
		Debug.Log(audioClip.loadType);
		return audioClip;
	}

	private void OnAudioRead(float[] data)
	{
		int num = data.Length * 2;
		byte[] array = new byte[num];
		int num2 = num;
		samplesRead = 0;
		num2 = stream.Read(array, 0, num);
		for (int i = 0; i < num2; i += 2)
		{
			byte b = array[i];
			float num3 = (float)(short)((array[i + 1] << 8) | b) * 1f / 32767f;
			data[samplesRead] = num3;
			samplesRead++;
		}
		if (num2 != num)
		{
			Debug.Log("Closing: " + name);
			stream.Close();
		}
	}

	private void OnAudioSetPosition(int newPosition)
	{
		samplesRead = newPosition;
	}
}
