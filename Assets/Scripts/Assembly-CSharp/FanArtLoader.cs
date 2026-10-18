using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class FanArtLoader : RDBase
{
	public static string artist;

	public static Texture2D art;

	public static string[] tags;

	private const string jsonFilename = "fanart_current.json";

	private const string pngFilename = "fanart_current.png";

	public static bool showCurrentFanArt => false;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this);
		StartCoroutine(LoadCurrentFanArtData());
	}

	private IEnumerator LoadCurrentFanArtData()
	{
		UnityWebRequest getCurrentFanArtData = UnityWebRequest.Get("https://7thbeat.sgp1.digitaloceanspaces.com/rd/fanart/current.json");
		yield return getCurrentFanArtData.SendWebRequest();
		string cacheJsonPath = Path.Combine(Application.persistentDataPath, "fanart_current.json");
		string json = getCurrentFanArtData.downloadHandler.text;
		bool flag = false;
		if (getCurrentFanArtData.AnyErrors() || string.IsNullOrEmpty(json))
		{
			Debug.Log("FA1 : " + getCurrentFanArtData.error);
			if (File.Exists(cacheJsonPath))
			{
				json = RDFile.ReadAllText(cacheJsonPath, Encoding.UTF8);
				flag = true;
			}
		}
		if (string.IsNullOrEmpty(json))
		{
			yield break;
		}
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		artist = dictionary["artist"] as string;
		tags = (dictionary["tags"] as string).Split(',');
		for (int i = 0; i < tags.Length; i++)
		{
			tags[i] = tags[i].Trim();
		}
		string cachePngPath = Path.Combine(Application.persistentDataPath, "fanart_current.png");
		string text = dictionary["url"] as string;
		string text2 = string.Empty;
		if (flag)
		{
			UnityWebRequest getCurrentFanArtTexture = UnityWebRequestTexture.GetTexture(cachePngPath);
			yield return getCurrentFanArtTexture.SendWebRequest();
			try
			{
				if (getCurrentFanArtTexture.AnyErrors())
				{
					Debug.Log("FA2: " + getCurrentFanArtTexture.error);
				}
				else
				{
					art = DownloadHandlerTexture.GetContent(getCurrentFanArtTexture);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("FA3: " + ex.Message);
			}
		}
		else
		{
			if (File.Exists(cacheJsonPath))
			{
				json = RDFile.ReadAllText(cacheJsonPath, Encoding.UTF8);
				text2 = (Json.Deserialize(json) as Dictionary<string, object>)["url"] as string;
			}
			bool usingCacheUrl = false;
			if (text == text2)
			{
				usingCacheUrl = true;
				text = text2;
			}
			UnityWebRequest getCurrentFanArtTexture = UnityWebRequestTexture.GetTexture(text);
			yield return getCurrentFanArtTexture.SendWebRequest();
			try
			{
				if (getCurrentFanArtTexture.AnyErrors())
				{
					Debug.Log("FA4 : " + getCurrentFanArtTexture.error);
				}
				else
				{
					art = DownloadHandlerTexture.GetContent(getCurrentFanArtTexture);
					if (!usingCacheUrl)
					{
						byte[] bytes = art.EncodeToPNG();
						File.WriteAllText(cacheJsonPath, json, Encoding.UTF8);
						File.WriteAllBytes(cachePngPath, bytes);
					}
				}
			}
			catch (Exception ex2)
			{
				Debug.Log("FA5: " + ex2.Message);
			}
		}
		art.filterMode = FilterMode.Point;
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
