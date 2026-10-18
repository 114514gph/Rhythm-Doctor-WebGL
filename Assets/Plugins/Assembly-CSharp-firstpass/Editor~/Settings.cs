using System.Collections.Generic;
using UnityEngine;

namespace SA.GoogleDoc
{
	public class Settings : ScriptableObject
	{
		public const string VERSION_NUMBER = "2.0";

		private const string GDAssetName = "GoogleDocConnectorSettings";

		private const string GDAssetExtension = ".asset";

		public const string SERVICE_NAME = "GoogleDocConnector-v2.0";

		public const string CLIENT_ID = "282525665220-5sc8be65ea34p0l0a3qs53krk2gf8tvk.apps.googleusercontent.com";

		public const string CLIENT_SECRET = "4rrrenfH5s2PcWp9mhucgyFB";

		public const string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";

		public const string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

		public const string GOOGLEDOC_URL_START = "https://docs.google.com/spreadsheets/d/";

		public const string GOOGLEDOC_URL_END = "#gid=";

		public const string SPREADSHEET_URL_START = "https://7bg.hyonsu.com/tq?&tq=&key=";

		public const string SPREADSHEET_URL_END = "&gid=";

		public const string SCRIPT_URL_START = "https://script.google.com/macros/s/AKfycbwrDJWLwK7BuA0KCOap3uavVFdPpoqGoffJu9zxigOO-kuAUFw/exec?key=";

		public const string SCRIPT_URL_PARAM = "&sheetid=";

		public const string PATH_ADDING = "Resources/Cache/";

		public string CachePath = "Plugins/StansAssets/GoogleDocConnector";

		public List<DocTemplate> docs = new List<DocTemplate>();

		private static Settings instance;

		public static Settings Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load("GoogleDocConnectorSettings") as Settings;
					if (instance == null)
					{
						instance = ScriptableObject.CreateInstance<Settings>();
					}
				}
				return instance;
			}
		}

		public static string GetCachePath()
		{
			return RDPluginsUtils.DataPath + "/";
		}

		public static string GetCachePath(string docName)
		{
			return GetCachePath() + docName + ".txt";
		}

		public void AddDoc(DocTemplate tpl)
		{
			docs.Add(tpl);
		}

		public void RemoveDoc(DocTemplate tpl)
		{
			docs.Remove(tpl);
		}

		public int GetDocIndexByName(string name)
		{
			foreach (DocTemplate doc in docs)
			{
				if (doc != null && name.Equals(doc.name))
				{
					return docs.IndexOf(doc) + 1;
				}
			}
			return 0;
		}

		public int GetDocIndexByKey(string key)
		{
			foreach (DocTemplate doc in docs)
			{
				if (doc != null && key.Equals(doc.key))
				{
					return docs.IndexOf(doc) + 1;
				}
			}
			return 0;
		}

		public DocTemplate GetDocByName(string name)
		{
			foreach (DocTemplate doc in docs)
			{
				if (doc != null && name.Equals(doc.name))
				{
					return doc;
				}
			}
			return null;
		}

		public DocTemplate GetDocByKey(string key)
		{
			foreach (DocTemplate doc in docs)
			{
				if (doc != null && key.Equals(doc.key))
				{
					return doc;
				}
			}
			return null;
		}

		public DocTemplate GetDocByIndex(int index)
		{
			if (index == 0)
			{
				return null;
			}
			index--;
			if (index >= docs.Count)
			{
				return null;
			}
			return docs[index];
		}

		public string[] GetDocNames()
		{
			List<string> list = new List<string> { "None" };
			foreach (DocTemplate doc in docs)
			{
				if (doc != null)
				{
					list.Add(doc.name);
				}
			}
			return list.ToArray();
		}

		public static void OpenDocURL(DocTemplate doc, WorksheetTemplate worksheet = null)
		{
			int num = worksheet?.listId ?? 0;
			Application.OpenURL("https://docs.google.com/spreadsheets/d/" + doc.key + "#gid=" + num);
		}
	}
}
