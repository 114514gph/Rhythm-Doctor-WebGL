#if UNITY_EDITOR || !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Ink.Runtime;
using RDTools;
using UnityEngine;
using UnityEngine.Networking;

namespace SA.GoogleDoc
{
	public static class Reader
	{
		public const string EmptyCell = "RDNull";

		public const string LastInkFetchFilename = "LastInkFetch.txt";

		public const string LastLocFetchFilename = "LastLocFetch.txt";

		private const string DateFormat = "yyyy/MM/dd, (HH:mm)";

		private const string DateCulture = "en-US";

		public static string externalDialoguesDataPath;

		public static string externalLocalizationDataPath;

		public static Dictionary<string, SpreadsheetData> sheets = new Dictionary<string, SpreadsheetData>();

		public static string localizationPassword;

		private static bool escapePressed
		{
			get
			{
				if (!Input.GetKey(KeyCode.Escape))
				{
					return Input.GetKeyDown(KeyCode.Escape);
				}
				return true;
			}
		}

		private static bool saveInternally => Application.isEditor;

		private static bool compileWhileFetching => false;

		public static void RetrievePublicSheetData(DocTemplate doc, bool drawProgressBar = true, bool retrievingDialogues = false, int workSheetIndex = -1, bool building = false, SystemLanguage[] dialogueLanguages = null)
		{
			if (workSheetIndex == -1)
			{
				foreach (WorksheetTemplate worksheet in doc.Worksheets)
				{
					Retrieve(worksheet);
				}
				return;
			}
			Retrieve(doc.Worksheets[workSheetIndex]);
			void Retrieve(WorksheetTemplate worksheet)
			{
				if (retrievingDialogues)
				{
					RetrievePublicSheetDataDialogues(doc, worksheet, dialogueLanguages);
				}
				else
				{
					RetrievePublicSheetDataLocalization(doc, worksheet, drawProgressBar, building);
				}
			}
		}

		public static void RetrievePublicSheetDataLocalization(DocTemplate doc, WorksheetTemplate worksheet, bool drawProgressBar = true, bool building = true)
		{
			if (!sheets.TryGetValue(doc.name, out var value))
			{
				value = new SpreadsheetData();
				sheets.Add(doc.name, value);
			}
			else if (value.worksheets.ContainsKey(worksheet.listId))
			{
				value.worksheets.Remove(worksheet.listId);
			}
			string text = (saveInternally ? System.IO.Path.Combine(Application.dataPath, "Resources", "Localization", "Auto") : externalLocalizationDataPath);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string path = System.IO.Path.Combine(text, worksheet.listName + ".json");
			if (File.Exists(path))
			{
				try
				{
					File.Delete(path);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.Message);
				}
			}
			File.WriteAllText(System.IO.Path.Combine(text, "LastLocFetch.txt"), GetTimestamp());
			RDBaseDll.printem($"doc.name: {doc.docName} worksheet.listName {worksheet.listName} worksheet.listId {worksheet.listId} doc.name {doc.name}");
			if (!LoadSheetData(value, worksheet.listId, doc, drawProgressBar))
			{
				return;
			}
			int row = 1;
			int column = 1;
			Dictionary<string, string> dictionary = sheets[doc.name].worksheets[worksheet.listId];
			Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>();
			while (dictionary.ContainsKey(CellKey()))
			{
				if (dictionary[FirstColumnKey()] == "RDNull")
				{
					row++;
					continue;
				}
				string key = dictionary[FirstColumnKey()].Trim();
				dictionary2[key] = new List<string>();
				column = 2;
				while (dictionary.ContainsKey(CellKey()))
				{
					dictionary2[key].Add(dictionary[CellKey()]);
					column++;
				}
				column = 1;
				row++;
			}
			JsonSerializerOptions options = new JsonSerializerOptions
			{
				WriteIndented = true
			};
			try
			{
				File.WriteAllText(path, JsonSerializer.Serialize(dictionary2, options));
			}
			catch (Exception ex2)
			{
				Debug.LogError(ex2.Message);
			}
			string CellKey()
			{
				return $"{row}:{column}";
			}
			string FirstColumnKey()
			{
				return $"{row}:{1}";
			}
		}

		private static string GetTimestamp()
		{
			DateTime utcNow = DateTime.UtcNow;
			CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
			return utcNow.ToString("yyyy/MM/dd, (HH:mm)\n", cultureInfo);
		}

		private static void RetrievePublicSheetDataDialogues(DocTemplate doc, WorksheetTemplate worksheet, SystemLanguage[] dialogueLanguages = null)
		{
			if (!sheets.TryGetValue(doc.name, out var value))
			{
				value = new SpreadsheetData();
				sheets.Add(doc.name, value);
			}
			else if (value.worksheets.ContainsKey(worksheet.listId))
			{
				value.worksheets.Remove(worksheet.listId);
			}
			if (!LoadSheetData(value, worksheet.listId, doc, drawProgressBar: true, loadingDialogues: true))
			{
				return;
			}
			int row = 0;
			int column = 1;
			Dictionary<string, string> dictionary = sheets[doc.name].worksheets[worksheet.listId];
			string text = (saveInternally ? System.IO.Path.Combine(Application.dataPath, "Resources", "Dialogues", "Auto") : externalDialoguesDataPath);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			File.WriteAllText(System.IO.Path.Combine(text, "LastInkFetch.txt"), GetTimestamp());
			while (dictionary.ContainsKey(CellKey()) && dictionary[CellKey()] != "RDNull")
			{
				string text2 = dictionary[CellKey()];
				if (!Enum.TryParse<SystemLanguage>(text2, out var result))
				{
					column++;
					continue;
				}
				bool flag = false;
				if (dialogueLanguages != null && !dialogueLanguages.Contains(result))
				{
					flag = true;
				}
				if (!flag)
				{
					bool num = Localization.AvailableLanguages.Contains(result);
					bool flag2 = Localization.AlphaOnlyLanguages.Contains(result);
					flag = !num && !flag2;
				}
				if (flag)
				{
					column++;
					continue;
				}
				if (!dictionary.ContainsKey(CellKey()))
				{
					break;
				}
				string text3 = string.Empty;
				while (dictionary.ContainsKey(CellKey()))
				{
					if (dictionary[FirstColumnKey()] == "*constants*")
					{
						row++;
						while (!dictionary[FirstColumnKey()].StartsWith("*"))
						{
							string text4 = ((column > 1 && (!dictionary.ContainsKey(CellKey()) || dictionary[CellKey()] == "RDNull")) ? dictionary[FirstColumnKey()] : dictionary[CellKey()]);
							if (!string.IsNullOrEmpty(text4) && !text4.Contains("RDNull"))
							{
								text3 = text3 + "\n" + text4;
							}
							row++;
						}
						text3 += "\n";
						break;
					}
					row++;
				}
				string text5 = (saveInternally ? "INCLUDE RDFunctions.ink\n" : Resources.Load<TextAsset>("Dialogues/Auto/RDFunctions").text) + "\n" + text3;
				while (dictionary.ContainsKey(CellKey()))
				{
					if (dictionary[CellKey()] != "SKIP")
					{
						string empty = string.Empty;
						empty = ((column > 1 && dictionary[CellKey()] == "RDNull") ? dictionary[FirstColumnKey()] : dictionary[CellKey()]);
						empty = empty.Replace("…", "...");
						if (empty == "RDNull")
						{
							empty = string.Empty;
						}
						else if (empty.StartsWith("*"))
						{
							string text6 = empty.Replace("*", string.Empty);
							empty = (empty.EndsWith("*") ? ("\n=== " + text6 + " ===") : ("\n= " + text6));
						}
						text5 = text5 + "\n" + empty;
					}
					row++;
				}
				text5 = text5.Replace("#", "\\#");
				text5 = text5.Replace("\r\n", "\n");
				string text7 = System.IO.Path.Combine(text, "dia" + worksheet.listName + "_" + text2 + ".ink");
				bool flag3 = true;
				if (File.Exists(text7))
				{
					string text8 = File.ReadAllText(text7).Replace("\r\n", "\n");
					int num2 = text8.IndexOf('\n');
					text8 = ((num2 < 0) ? "" : text8.Substring(num2 + 2));
					if (text8 == text5)
					{
						Debug.Log(worksheet.listName + "_" + text2 + " has the same contents, skipping");
						flag3 = false;
					}
					else
					{
						Debug.Log("previousInkText \n" + text8 + " inkText \n" + text5);
						try
						{
							File.Delete(text7);
						}
						catch (Exception ex)
						{
							Debug.LogError(ex.Message);
						}
					}
				}
				if (flag3)
				{
					try
					{
						string text9 = "// " + GetTimestamp() + "\n";
						File.WriteAllText(text7, text9 + text5);
						if (compileWhileFetching)
						{
							Story story = RDPluginsUtils.CompileInk(text5);
							File.WriteAllText(System.IO.Path.Combine(text, "dia" + worksheet.listName + "_" + text2 + ".json"), story.ToJson());
						}
					}
					catch (Exception ex2)
					{
						Debug.LogError(ex2.Message);
					}
				}
				API.lastWrittenInkFilePath = text7.Replace("\\", "/");
				row = 0;
				column++;
			}
			string CellKey()
			{
				return $"{row}:{column}";
			}
			string FirstColumnKey()
			{
				return $"{row}:{1}";
			}
		}

		private static bool LoadSheetData(SpreadsheetData spreadsheetData, int tableListId, DocTemplate doc, bool drawProgressBar, bool loadingDialogues = false)
		{
			float num = 0f;
			string text = "https://7bg.hyonsu.com/tq?&tq=&key=" + doc.key + "&gid=" + tableListId;
			RDBaseDll.printem("urlB: " + text);
			UnityWebRequest unityWebRequest = new UnityWebRequest(text);
			if (string.IsNullOrEmpty(localizationPassword))
			{
				Debug.LogWarning("Localization password is empty!");
				return false;
			}
			unityWebRequest.SetRequestHeader("Authorization", localizationPassword);
			DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.downloadHandler = downloadHandler;
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
			float num2 = 20f;
			int num3 = 0;
			while (!unityWebRequestAsyncOperation.isDone)
			{
				Thread.Sleep(100);
				num += 0.1f;
				if (escapePressed)
				{
					Debug.LogError("Escaped!!!! Omg");
					unityWebRequest.Abort();
					return false;
				}
				num3 += 100;
				if ((float)num3 >= num2 * 1000f)
				{
					return false;
				}
			}
			if (unityWebRequest.AnyErrors())
			{
				Debug.LogWarning("[GoogleDataAPI] " + unityWebRequest.error);
				return false;
			}
			RDBaseDll.printem("<color=green>Everything went correctly</color>");
			FillSpreadsheetWithData(spreadsheetData, tableListId, unityWebRequest.downloadHandler.text, includeFirstRow: true);
			return true;
		}

		public static bool GetDateTimeOfLocalizationText(string text, out DateTime outDate)
		{
			int num = text.IndexOf('\n');
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			text = text.TrimStart('/').Trim();
			CultureInfo provider = CultureInfo.CreateSpecificCulture("en-US");
			return DateTime.TryParseExact(text, "yyyy/MM/dd, (HH:mm)", provider, DateTimeStyles.None, out outDate);
		}

		public static bool IsTimestampMoreRecent(string timestamp, string timestampToCompareAgainst)
		{
			bool dateTimeOfLocalizationText = GetDateTimeOfLocalizationText(timestamp, out var outDate);
			if (GetDateTimeOfLocalizationText(timestampToCompareAgainst, out var outDate2) & dateTimeOfLocalizationText)
			{
				return outDate > outDate2;
			}
			return false;
		}

		private static void FillSpreadsheetWithData(SpreadsheetData spreadsheet, int tableListId, string data, bool includeFirstRow = false)
		{
			data = data.Replace('’', '\'').Replace("\\u2019", "'");
			Match match = Regex.Match(data, "\\((.+)\\)", RegexOptions.IgnoreCase);
			if (!match.Success || !(Json.Deserialize(match.Groups[1].Value) is Dictionary<string, object> dictionary) || !(dictionary["table"] is Dictionary<string, object> dictionary2))
			{
				return;
			}
			if (includeFirstRow && dictionary2.TryGetValueAs<string, object, List<object>>("cols", out var valueAs))
			{
				for (int i = 0; i < valueAs.Count; i++)
				{
					string text = (string)(valueAs[i] as Dictionary<string, object>)["label"];
					if (text == null)
					{
						text = "RDNull";
					}
					spreadsheet.SetData(tableListId, 0, i + 1, text);
				}
			}
			if (!dictionary2.TryGetValueAs<string, object, List<object>>("rows", out var valueAs2))
			{
				return;
			}
			for (int j = 0; j < valueAs2.Count; j++)
			{
				bool flag = false;
				if (!(valueAs2[j] is Dictionary<string, object> dictionary3))
				{
					continue;
				}
				List<object> list = dictionary3["c"] as List<object>;
				for (int k = 0; k < list.Count; k++)
				{
					Dictionary<string, object> dictionary4 = list[k] as Dictionary<string, object>;
					if (flag || dictionary4 == null || dictionary4["v"] == null)
					{
						if (k == 0)
						{
							flag = true;
						}
						spreadsheet.SetData(tableListId, j + 1, k + 1, "RDNull");
						continue;
					}
					string text2 = dictionary4["v"] as string;
					string data2 = "RDNull";
					if (k == 0)
					{
						flag = text2.StartsWith("//") || string.IsNullOrEmpty(text2);
					}
					if (!flag)
					{
						data2 = text2;
					}
					spreadsheet.SetData(tableListId, j + 1, k + 1, data2);
				}
			}
		}
	}
}

#endif
