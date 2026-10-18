using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class EntitlementsService : MonoBehaviour
{
	public string playerIdentifier;

	public List<string> entitlements;

	private void Start()
	{
		GetPlayerIdentifier();
		GetEntitlements();
	}

	private void DeserializeEntitlements(string json)
	{
		string[] source = null;
		try
		{
			source = JsonConvert.DeserializeObject<string[]>(json);
		}
		catch (Exception arg)
		{
			Debug.Log($"There was an error deserializing {json}.\nException: {arg}");
		}
		entitlements = source.ToList();
		Debug.Log("entitlements successfully retrieved: " + json);
	}

	public void GetPlayerIdentifier()
	{
		// WebGL 版本不使用 Steam
		playerIdentifier = "webgl";
	}

	public void GetEntitlements()
	{
		StartCoroutine(GetEntitlementsRequest());
	}

	private IEnumerator GetEntitlementsRequest()
	{
		UnityWebRequest request = new UnityWebRequest("https://7thbe.at/api/entitlements/check?player_identifier=" + playerIdentifier, "GET")
		{
			downloadHandler = new DownloadHandlerBuffer(),
			redirectLimit = 0
		};
		request.SetRequestHeader("Accept", "application/json");
		yield return request.SendWebRequest();
		Debug.Log("GetEntitlementsRequest: " + playerIdentifier);
		if (request.responseCode == 200)
		{
			DeserializeEntitlements(request.downloadHandler.text);
		}
	}

	public void RedeemCode(string code, Action<bool, string> onCompleteAction)
	{
		if (playerIdentifier.IsNullOrEmpty())
		{
			onCompleteAction(arg1: false, "error.entitlement.playerIdentifier");
			return;
		}
		code = code.Trim().ToUpper();
		if (!new Regex("^[A-Z0-9]{8}$").IsMatch(code))
		{
			onCompleteAction(arg1: false, "error.entitlement.code");
		}
		else
		{
			StartCoroutine(RedeemCodeRequest(code, onCompleteAction));
		}
	}

	private IEnumerator RedeemCodeRequest(string code, Action<bool, string> onCompleteAction)
	{
		string s = "{\"code\":\"" + code + "\", \"player_identifier\": \"" + playerIdentifier + "\"}";
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		UnityWebRequest request = new UnityWebRequest("https://7thbe.at/api/entitlements/redeem", "POST", new DownloadHandlerBuffer(), new UploadHandlerRaw(bytes)
		{
			contentType = "application/json"
		})
		{
			redirectLimit = 0
		};
		request.SetRequestHeader("Accept", "application/json");
		yield return request.SendWebRequest();
		string arg = "levelSelect.entitlement.complete";
		if (request.result == UnityWebRequest.Result.ConnectionError)
		{
			arg = "error.connection";
		}
		else if (request.responseCode == 403)
		{
			arg = "error.entitlement.response.403";
		}
		else if (request.responseCode == 404)
		{
			arg = "error.entitlement.response.404";
		}
		else if (request.responseCode == 409)
		{
			arg = "error.entitlement.response.409";
		}
		else if (request.responseCode == 410)
		{
			arg = "error.entitlement.response.410";
		}
		else if (request.responseCode != 200)
		{
			arg = "error.entitlement.response.unknown";
		}
		if (request.responseCode == 200)
		{
			DeserializeEntitlements(request.downloadHandler.text);
		}
		onCompleteAction(request.responseCode == 200, arg);
	}
}
