using UnityEngine;

public static class AudioSamplesToTexture
{
	public const int textureSize = 128;

	public const int textureSamplesLength = 16384;

	public static Texture2D AudioClipToTexture(AudioClip clip)
	{
		Texture2D texture2D = new Texture2D(128, 128, TextureFormat.RGBA32, mipChain: false);
		if (GC.levelEditor_streamMP3Loading)
		{
			return texture2D;
		}
		float[] array = new float[clip.samples * clip.channels];
		clip.GetData(array, 0);
		for (int i = 0; i < 128; i++)
		{
			for (int j = 0; j < 128; j++)
			{
				int num = Mathf.FloorToInt((float)(i * 128 + j) * 1f * (float)array.Length / 16384f);
				float num2 = Mathf.Abs(array[num]);
				texture2D.SetPixel(j, i, new Color(num2, 0f, 0f, num2));
			}
		}
		texture2D.filterMode = FilterMode.Bilinear;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.name = "Waveform";
		texture2D.Apply();
		return texture2D;
	}
}
