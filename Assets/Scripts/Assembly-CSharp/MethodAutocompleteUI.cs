using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MethodAutocompleteUI : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public enum CustomMethodVisibility
	{
		Public = 0,
		DevOnly = 1,
		ClassOnly = 2
	}

	public class CachedMethodInfo
	{
		public string name;

		public string stringified;

		public string key;

		public string args;

		public bool isMod;

		public CustomMethodVisibility visibility;
	}

	public RectTransform resultsContainer;

	public GameObject resultPrefab;

	public bool isHovering;

	private int resultCount;

	private int selectedResult;

	private Button selectedButton;

	private static BindingFlags scopeFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod;

	private static readonly Dictionary<string, Type> scopes = new Dictionary<string, Type>
	{
		["level"] = typeof(LevelBase),
		["vfx"] = typeof(scrVfxControl),
		["room"] = typeof(RDRoom)
	};

	private static readonly Dictionary<Type, string[]> supportedArgumentTypes = new Dictionary<Type, string[]>
	{
		[typeof(int)] = new string[2] { "int", "0" },
		[typeof(float)] = new string[2] { "float", "0" },
		[typeof(string)] = new string[2] { "string", "\"\"" },
		[typeof(bool)] = new string[2] { "bool", "false" }
	};

	private static readonly Dictionary<Type, List<CachedMethodInfo>> methodInfoCache = new Dictionary<Type, List<CachedMethodInfo>>();

	private scnEditor editor => scnEditor.instance;

	private void Awake()
	{
		resultPrefab.SetActive(value: false);
	}

	private static bool StringifyMethod(MethodInfo methodInfo, out string stringified, out string defaultArgs)
	{
		stringified = null;
		defaultArgs = null;
		ParameterInfo[] parameters = methodInfo.GetParameters();
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		ParameterInfo[] array = parameters;
		foreach (ParameterInfo parameterInfo in array)
		{
			Type parameterType = parameterInfo.ParameterType;
			if (!supportedArgumentTypes.TryGetValue(parameterType, out var value))
			{
				return false;
			}
			list.Add(string.Join(' ', value[0], parameterInfo.Name));
			list2.Add(GetDefaultArg(parameterInfo.Name, value[0], value[1]));
		}
		stringified = methodInfo.Name + "(" + string.Join(", ", list) + ")";
		defaultArgs = string.Join(", ", list2);
		return true;
	}

	private static bool StringifyField(FieldInfo field, out string stringified, out string defaultValue)
	{
		stringified = null;
		defaultValue = null;
		bool flag = field.FieldType == typeof(bool);
		bool flag2 = field.FieldType == typeof(float);
		bool flag3 = field.FieldType == typeof(int);
		if (!flag && !flag2 && !flag3)
		{
			return false;
		}
		defaultValue = (flag ? "true" : "1");
		stringified = field.Name + " = " + defaultValue;
		return true;
	}

	private static string GetDefaultArg(string name, string type, string defaultVal)
	{
		if (type == "string")
		{
			if (name.StartsWith("ease"))
			{
				return "\"Linear\"";
			}
			if (name.StartsWith("color") || name.StartsWith("colHex"))
			{
				return "\"ffffff\"";
			}
		}
		else if (type == "float")
		{
			switch (name)
			{
			case "scale":
			case "alpha":
			case "opacity":
				return "1";
			}
		}
		return defaultVal;
	}

	public void ShowPopup()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
	}

	public void HidePopup()
	{
		if (base.gameObject.activeSelf)
		{
			isHovering = false;
			base.gameObject.SetActive(value: false);
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		isHovering = true;
	}

	public void OnPointerExit(PointerEventData data)
	{
		isHovering = false;
	}

	public void Search(InputField inputField, string inputString)
	{
		if (inputField.text.Length < 1 || !inputField.isFocused)
		{
			HidePopup();
			return;
		}
		Type type = typeof(LevelBase);
		if (Enum.TryParse<Level>(editor.levelSettings.customClass, out var result))
		{
			type = LevelSelector.GetLevelTypeFromEnum(result);
		}
		foreach (Transform item in resultsContainer)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		string[] array = inputString.Split('(');
		if (array.Length > 2)
		{
			return;
		}
		string[] array2 = array[0].Split('.');
		if (array2.Length > 2)
		{
			return;
		}
		string value = array2[^1];
		string text = ((array2.Length == 2) ? array2[0] : null);
		string[] array3;
		if (array.Length != 2 || array[1].Length <= 1)
		{
			array3 = new string[0];
		}
		else
		{
			string text2 = array[1];
			array3 = text2.Substring(0, text2.Length - 1).Split(',');
		}
		string[] array4 = array3;
		bool isTypingArguments = array4.Length != 0;
		resultCount = 0;
		foreach (KeyValuePair<string, Type> scope in scopes)
		{
			if (text != null && text.Length > 0 && !scope.Key.Contains(text))
			{
				continue;
			}
			bool flag = text == null && scope.Key.Contains(value);
			bool flag2 = scope.Key == "level";
			List<Type> list = new List<Type> { scope.Value };
			if (flag2 && type != scope.Value)
			{
				list.Insert(0, type);
			}
			foreach (Type item2 in list)
			{
				if (!methodInfoCache.ContainsKey(item2))
				{
					MethodInfo[] methods = item2.GetMethods(scopeFlags);
					List<CachedMethodInfo> list2 = new List<CachedMethodInfo>();
					MethodInfo[] array5 = methods;
					foreach (MethodInfo methodInfo in array5)
					{
						if (!(methodInfo.ReturnType != typeof(void)))
						{
							ListedMethodAttribute customAttribute = methodInfo.GetCustomAttribute<ListedMethodAttribute>();
							if ((RDBase.isDev || customAttribute != null) && StringifyMethod(methodInfo, out var stringified, out var defaultArgs))
							{
								string key = "customMethod." + scope.Key + "." + methodInfo.Name;
								bool flag3 = flag2 && scope.Value.GetMethod(methodInfo.Name, scopeFlags) == null;
								list2.Add(new CachedMethodInfo
								{
									name = methodInfo.Name,
									stringified = stringified,
									key = key,
									args = defaultArgs,
									visibility = (flag3 ? CustomMethodVisibility.ClassOnly : ((customAttribute == null) ? CustomMethodVisibility.DevOnly : CustomMethodVisibility.Public))
								});
							}
						}
					}
					if (flag2)
					{
						FieldInfo[] fields = item2.GetFields(scopeFlags);
						foreach (FieldInfo fieldInfo in fields)
						{
							ListedModAttribute customAttribute2 = fieldInfo.GetCustomAttribute<ListedModAttribute>();
							if (customAttribute2 != null && StringifyField(fieldInfo, out var stringified2, out var defaultValue))
							{
								string key2 = ((customAttribute2 != null && customAttribute2.showDescription) ? ("levelMod." + fieldInfo.Name) : null);
								list2.Add(new CachedMethodInfo
								{
									name = fieldInfo.Name,
									stringified = stringified2,
									key = key2,
									args = defaultValue,
									isMod = true,
									visibility = ((customAttribute2 == null) ? CustomMethodVisibility.DevOnly : CustomMethodVisibility.Public)
								});
							}
						}
					}
					list2 = (from x in list2
						orderby (int)x.visibility, x.isMod
						select x).ThenBy((CachedMethodInfo x) => x.name, StringComparer.OrdinalIgnoreCase).ToList();
					methodInfoCache.Add(item2, list2);
				}
				foreach (CachedMethodInfo method in methodInfoCache[item2])
				{
					if (!flag && !method.name.Contains(value, StringComparison.CurrentCultureIgnoreCase))
					{
						continue;
					}
					GameObject obj = UnityEngine.Object.Instantiate(resultPrefab, resultsContainer);
					Text componentInChildren = obj.GetComponentInChildren<Text>();
					Button componentInChildren2 = obj.GetComponentInChildren<Button>();
					obj.SetActive(value: true);
					componentInChildren.text = ((scope.Key == "level" && method.visibility != CustomMethodVisibility.ClassOnly) ? method.stringified : string.Join('.', scope.Key, method.stringified));
					componentInChildren.color = ((method.visibility == CustomMethodVisibility.DevOnly) ? new Color(0.5f, 0f, 0f) : ((method.visibility == CustomMethodVisibility.ClassOnly) ? new Color(0f, 0f, 0.5f) : Color.black));
					componentInChildren2.onClick.AddListener(() =>
					{
						if (!isTypingArguments)
						{
							string text3 = ((!(scope.Key == "level")) ? ((scope.Key == "room") ? "room1." : (scope.Key + ".")) : ((method.visibility == CustomMethodVisibility.ClassOnly) ? "level." : ""));
							inputField.text = ((!method.isMod) ? (text3 + method.name + "(" + method.args + ")") : (method.name + " = " + method.args));
							inputField.Select();
							inputField.MoveTextEnd(shift: false);
							inputField.caretPosition = inputField.text.Length - 1;
							InspectorPanel_CallCustomMethod.UpdateMethodDesc(method.name, method.key);
						}
					});
					resultCount++;
				}
			}
		}
		if (resultCount < 1)
		{
			HidePopup();
			return;
		}
		RectTransform component = GetComponent<RectTransform>();
		component.position = component.position.WithY(inputField.transform.position.y);
		component.anchoredPosition = component.anchoredPosition.WithY(component.anchoredPosition.y - 17f);
		float num = (float)resultCount * 16f + 4f;
		resultsContainer.sizeDelta = new Vector2(resultsContainer.sizeDelta.x, num);
		float y = Mathf.Min(num, 100f);
		component.sizeDelta = new Vector2(component.sizeDelta.x, y);
		selectedResult = 0;
		ShowPopup();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			selectedResult++;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			selectedResult--;
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			resultsContainer.transform.GetChild(selectedResult).GetComponent<Button>().onClick.Invoke();
		}
		UpdateSelection();
	}

	private void UpdateSelection()
	{
		if (resultCount >= 1)
		{
			selectedResult = Math.Max(0, Math.Min(selectedResult, resultCount - 1));
			if (selectedButton != null)
			{
				selectedButton.SetNormalColor(Color.clear);
			}
			selectedButton = resultsContainer.GetChild(selectedResult).GetComponent<Button>();
			selectedButton.SetNormalColor(selectedButton.colors.selectedColor);
		}
	}

	public static void DumpPublicCustomMethods(bool justPrintErrors = false)
	{
		if (!Application.isPlaying)
		{
			MonoBehaviour.print("The game must be running to do this!");
			return;
		}
		List<string> list = new List<string>();
		List<string> unlocalizedMethods = new List<string>();
		List<string> localizedMethods = new List<string>();
		int methodCount = 0;
		foreach (KeyValuePair<string, Type> scope in scopes)
		{
			MethodInfo[] methods = scope.Value.GetMethods(scopeFlags);
			foreach (MethodInfo methodInfo in methods)
			{
				ListedMethodAttribute customAttribute = methodInfo.GetCustomAttribute<ListedMethodAttribute>();
				if (customAttribute != null)
				{
					string text = "customMethod." + scope.Key + "." + methodInfo.Name;
					if (methodInfo.ReturnType != typeof(void) || !StringifyMethod(methodInfo, out var _, out var _))
					{
						list.Add(text);
					}
					else
					{
						CountMethod(text, customAttribute.showDescription);
					}
				}
			}
			if (!(scope.Key == "level"))
			{
				continue;
			}
			FieldInfo[] fields = scope.Value.GetFields(scopeFlags);
			foreach (FieldInfo fieldInfo in fields)
			{
				ListedModAttribute customAttribute2 = fieldInfo.GetCustomAttribute<ListedModAttribute>();
				if (customAttribute2 != null)
				{
					string text2 = "levelMod." + fieldInfo.Name;
					if (!StringifyField(fieldInfo, out var _, out var _))
					{
						list.Add(text2);
					}
					else
					{
						CountMethod(text2, customAttribute2.showDescription);
					}
				}
			}
		}
		if (justPrintErrors)
		{
			foreach (string item in list)
			{
				Debug.LogWarning("<b>[CUSTOM METHOD]</b> Public custom method <b>" + item + "</b> is invalid and will not be listed.");
			}
			if (unlocalizedMethods.Count > 0)
			{
				Debug.LogWarning($"<b>[CUSTOM METHODS]</b> {unlocalizedMethods.Count} public custom method(s) below have no localization key yet! " + "Add them to the localization sheet, or use [ListedMethod(false)] to disable their description.\n" + string.Join("\n", unlocalizedMethods) + "\n");
			}
			return;
		}
		List<string> list2 = new List<string>
		{
			$"<b>Total methods: {methodCount}</b> (click for details!)",
			""
		};
		if (list.Count > 0)
		{
			list.Sort();
			list2.Add("<b>!!! These methods are BROKEN and DO NOT APPEAR !!!</b>");
			foreach (string item2 in list)
			{
				list2.Add(item2 ?? "");
			}
			list2.Add("");
		}
		if (unlocalizedMethods.Count > 0)
		{
			unlocalizedMethods.Sort();
			list2.Add($"<b>> {unlocalizedMethods.Count} methods missing localization key:</b>");
			foreach (string item3 in unlocalizedMethods)
			{
				list2.Add(item3);
			}
			list2.Add("");
		}
		localizedMethods.Sort();
		list2.Add($"<b>> {localizedMethods.Count} methods configured and localized:</b>");
		foreach (string item4 in localizedMethods)
		{
			list2.Add(item4);
		}
		list2.Add("\n==========");
		MonoBehaviour.print(string.Join("\n", list2));
		void CountMethod(string key, bool showDescription)
		{
			methodCount++;
			RDString.GetWithCheck(key, out var exists);
			if (!exists & showDescription)
			{
				unlocalizedMethods.Add(key);
			}
			else
			{
				localizedMethods.Add(key);
			}
		}
	}
}
