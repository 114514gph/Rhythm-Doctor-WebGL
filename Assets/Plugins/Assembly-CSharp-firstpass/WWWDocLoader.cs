using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WWWDocLoader : MonoBehaviour
{
	public OnDocLoaded LoadEvent;

	public static WWWDocLoader createRequest()
	{
		return new GameObject("WWWDocLoader").AddComponent<WWWDocLoader>();
	}

	public void send(string url)
	{
		UnityWebRequest www = new UnityWebRequest(url);
		StartCoroutine(WaitForRequest(www));
	}

	public void send(string url, WWWForm form)
	{
		UnityWebRequest www = UnityWebRequest.Post(url, form);
		StartCoroutine(WaitForRequest(www));
	}

	private IEnumerator WaitForRequest(UnityWebRequest www)
	{
		yield return www.SendWebRequest();
		string data = (www.AnyErrors() ? string.Empty : www.downloadHandler.text);
		SendEvent(data);
		Object.Destroy(base.gameObject);
	}

	private void SendEvent(string data)
	{
		if (LoadEvent != null)
		{
			LoadEvent(data);
		}
	}
}
