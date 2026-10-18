using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SA_WWWTextureLoader : MonoBehaviour
{
	public static Dictionary<string, Texture2D> LocalCache = new Dictionary<string, Texture2D>();

	private string _url;

	public event Action<Texture2D> OnLoad = delegate
	{
	};

	public static SA_WWWTextureLoader Create()
	{
		return new GameObject("WWWTextureLoader").AddComponent<SA_WWWTextureLoader>();
	}

	public void LoadTexture(string url)
	{
		_url = url;
		if (LocalCache.ContainsKey(_url))
		{
			OnLoad(LocalCache[_url]);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			StartCoroutine(LoadCoroutin());
		}
	}

	private IEnumerator LoadCoroutin()
	{
		UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(_url);
		yield return uwr.SendWebRequest();
		if (uwr.AnyErrors())
		{
			OnLoad(null);
		}
		else
		{
			Texture2D content = DownloadHandlerTexture.GetContent(uwr);
			UpdateLocalCache(_url, content);
			OnLoad(content);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private static void UpdateLocalCache(string url, Texture2D image)
	{
	}
}
