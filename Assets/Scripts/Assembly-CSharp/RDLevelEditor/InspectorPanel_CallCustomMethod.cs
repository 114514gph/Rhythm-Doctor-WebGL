using System.Linq;
using SA.GoogleDoc;
using UnityEngine;

namespace RDLevelEditor
{
	public class InspectorPanel_CallCustomMethod : InspectorPanel
	{
		public static string filledMethodKey;

		public static PropertyControl_Description methodDescription;

		private void Start()
		{
			foreach (Property property in properties)
			{
				if (property.name == "description")
				{
					methodDescription = (PropertyControl_Description)property.control;
					methodDescription.SetText("");
				}
			}
		}

		private void CheckMethodDesc(LevelEvent_Base levelEvent)
		{
			if (methodDescription == null)
			{
				return;
			}
			string methodName = ((LevelEvent_CallCustomMethod)levelEvent).methodName;
			if (methodName.Length < 5)
			{
				ClearMethodDesc();
				return;
			}
			bool flag = methodName.Contains(" = ");
			string[] array = methodName.Split(flag ? " = " : "(")[0].Split('.');
			string text = array.Last();
			string text2 = ((array.Length == 1) ? "level" : (array[0].StartsWith("room") ? "room" : array[0]));
			string text3 = ((!flag) ? ("customMethod." + text2 + "." + text) : ("levelMod." + text));
			if (text3 != filledMethodKey)
			{
				UpdateMethodDesc(text, text3);
			}
		}

		public static void UpdateMethodDesc(string name, string key)
		{
			if (methodDescription == null)
			{
				return;
			}
			if (!key.IsNullOrEmpty())
			{
				string withCheck = RDString.GetWithCheck(key, out var exists);
				if (exists && (RDString.language == SystemLanguage.English || withCheck != Localization.GetLocalizedString(key, LangSection.CustomMethods, LangCode.English)))
				{
					filledMethodKey = key;
					methodDescription.SetText("\n<color=yellow>" + name + "</color>\n\n" + withCheck);
					return;
				}
			}
			ClearMethodDesc();
		}

		public static void ClearMethodDesc()
		{
			if (!(methodDescription == null))
			{
				filledMethodKey = null;
				methodDescription.SetText("");
			}
		}

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			base.UpdateUIProperties(levelEvent);
			CheckMethodDesc(levelEvent);
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			base.SaveProperties(levelEvent);
			CheckMethodDesc(levelEvent);
		}
	}
}
