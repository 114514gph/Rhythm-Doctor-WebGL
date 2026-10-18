using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class EditorWebServices : RDBase
{
	private const string AllArtists = " ";

	[NonSerialized]
	public WebServiceResult result;

	[NonSerialized]
	public int resultCode;

	public static ArtistData[] artists;

	private UnityWebRequest getArtists;

	private Dictionary<string, SystemLanguage> langCodeToLanguage = new Dictionary<string, SystemLanguage>
	{
		{
			"en",
			SystemLanguage.English
		},
		{
			"ko",
			SystemLanguage.Korean
		},
		{
			"zhs",
			SystemLanguage.ChineseSimplified
		},
		{
			"zht",
			SystemLanguage.ChineseTraditional
		},
		{
			"es",
			SystemLanguage.Spanish
		},
		{
			"pt",
			SystemLanguage.Portuguese
		},
		{
			"ja",
			SystemLanguage.Japanese
		},
		{
			"pl",
			SystemLanguage.Polish
		},
		{
			"ru",
			SystemLanguage.Russian
		},
		{
			"ro",
			SystemLanguage.Romanian
		},
		{
			"vi",
			SystemLanguage.Vietnamese
		},
		{
			"fr",
			SystemLanguage.French
		}
	};

	public static string verifiedArtistsPath => Path.Combine(Application.persistentDataPath, "verified_artists.json");

	public void LoadAllArtists(Action onCompleted = null)
	{
		if (artists == null)
		{
			StartCoroutine(GetArtists(" ", onCompleted));
		}
	}

	public IEnumerator GetArtists(string search = " ", Action onCompleted = null)
	{
		if (getArtists != null)
		{
			getArtists.Abort();
			yield return null;
		}
		new List<IMultipartFormSection>().Add(new MultipartFormDataSection("search", search));
		artists = null;
		getArtists = UnityWebRequest.Get("https://s3.pub1.infomaniak.cloud/object/v1/AUTH_c08a83cee2aa464e81e685e75e56b54e/website-public/rd_dump/rd_artists.json");
		yield return getArtists.SendWebRequest();
		if (getArtists.AnyErrors())
		{
			result = WebServiceResult.NoResponse;
			Debug.Log(getArtists.error);
		}
		else
		{
			List<object> list = Json.Deserialize(getArtists.downloadHandler.text) as List<object>;
			if (list == null)
			{
				result = WebServiceResult.BadResponse;
			}
			resultCode = 1;
			if (resultCode == 1)
			{
				result = WebServiceResult.Correct;
				artists = new ArtistData[list.Count];
				for (int i = 0; i < artists.Length; i++)
				{
					ArtistData artistData = new ArtistData();
					artists[i] = artistData;
					Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
					artistData.id = (int)dictionary["id"];
					artistData.name = (dictionary["name"] as string).Trim().Replace("\n", "");
					artistData.nameLowercase = artistData.name.ToLower();
					string text = dictionary["evidence_url"] as string;
					if (!string.IsNullOrWhiteSpace(text))
					{
						List<object> list2 = Json.Deserialize(text) as List<object>;
						int count = list2.Count;
						artistData.evidenceURLs = new string[count];
						for (int j = 0; j < count; j++)
						{
							artistData.evidenceURLs[j] = list2[j] as string;
						}
					}
					else
					{
						artistData.evidenceURLs = new string[0];
					}
					artistData.link1 = dictionary["link_1"] as string;
					artistData.link2 = dictionary["link_2"] as string;
					object obj = dictionary["status_new"];
					bool flag = Enum.IsDefined(typeof(ApprovalLevel), obj);
					artistData.approvalLevel = (flag ? ((ApprovalLevel)obj) : ApprovalLevel.Declined);
				}
			}
			else
			{
				result = WebServiceResult.ErrorNumber;
			}
		}
		onCompleted?.Invoke();
	}

	private IEnumerator PostArtistRequest()
	{
		MonoBehaviour.print("WebServiceTester.PostArtistRequest()");
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		list.Add(new MultipartFormDataSection("artistName", "Chumbawamba"));
		UnityWebRequest request = UnityWebRequest.Post("https://7thbe.at/api/postArtistRequest", list);
		yield return request.SendWebRequest();
		if (request.AnyErrors())
		{
			Debug.Log(request.error);
		}
		else
		{
			Debug.Log("PostArtistRequest complete!: " + request.downloadHandler.text);
		}
	}

	private IEnumerator UploadArtist(string artistName, string message, string evidenceImagePath, string artistLink1, string artistLink2)
	{
		MonoBehaviour.print("WebServiceTester.UploadArtist()");
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		list.Add(new MultipartFormDataSection("artistName", artistName));
		list.Add(new MultipartFormDataSection("message", message));
		byte[] data = File.ReadAllBytes(evidenceImagePath);
		string fileName = Path.GetFileName(evidenceImagePath);
		Path.GetExtension(fileName);
		list.Add(new MultipartFormFileSection("evidence", data, fileName, "image/png"));
		list.Add(new MultipartFormDataSection("approvalLevel", Convert.ToString(2)));
		list.Add(new MultipartFormDataSection("artistId", Convert.ToString(5)));
		list.Add(new MultipartFormDataSection("artistLink", artistLink1));
		list.Add(new MultipartFormDataSection("artistLink2", artistLink2));
		UnityWebRequest request = UnityWebRequest.Post("https://7thbe.at/api/postArtistUpload", list);
		yield return request.SendWebRequest();
		if (request.AnyErrors())
		{
			Debug.Log(request.error);
		}
		else
		{
			Debug.Log("UploadArtist complete!: " + request.downloadHandler.text);
		}
	}

	private IEnumerator UploadArtistDemo()
	{
		List<IMultipartFormSection> list = new List<IMultipartFormSection>();
		list.Add(new MultipartFormDataSection("artistName", "Eminem"));
		list.Add(new MultipartFormDataSection("message", "This is a message"));
		list.Add(new MultipartFormDataSection("userEmail", "giacomopc@gmail.com"));
		byte[] data = File.ReadAllBytes("/Users/temp/Desktop/ss.png");
		list.Add(new MultipartFormFileSection("evidence", data, "ss.png", "image/png"));
		list.Add(new MultipartFormDataSection("approvalLevel", Convert.ToString(2)));
		list.Add(new MultipartFormDataSection("artistId", Convert.ToString(5)));
		list.Add(new MultipartFormDataSection("artistLink", "https://www.google.com"));
		list.Add(new MultipartFormDataSection("artistLink2", "https://www.youtube.com"));
		UnityWebRequest request = UnityWebRequest.Post("https://7thbe.at/api/postArtistUpload", list);
		yield return request.SendWebRequest();
		if (request.AnyErrors())
		{
			Debug.Log(request.error);
		}
		else
		{
			Debug.Log("UploadArtist complete!: " + request.downloadHandler.text);
		}
	}
}
