using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_CallCustomMethod : LevelEvent_Base
	{
		private enum Operation
		{
			Set = 0,
			Increment = 1,
			Decrement = 2,
			Get = 3
		}

		private MethodInfo method;

		private object instance;

		private bool hasArguments;

		private string[] argString;

		private string strArg = "";

		private Operation operation;

		private FieldInfo field;

		private bool anyErrorDecoding;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("", InputField.LineType.SingleLine, 999, 14, true, true)]
		public string methodName { get; set; } = "";

		[JsonProperty("executionTime", "", null, "", false, true, null, null)]
		public LevelEventExecutionTime levelEventExecution { get; set; } = LevelEventExecutionTime.OnBar;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public int sortOffset { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableDescIf", null)]
		[Description("editor.help.title", 80, 3, TextAnchor.UpperCenter)]
		public bool description => false;

		public static bool EnableDescIf()
		{
			return RDBase.isAdvanced;
		}

		public override IEnumerator Prepare()
		{
			methodName = base.game.currentLevel.EvaluateCurlyBracketsInString(methodName);
			executionTime = levelEventExecution;
			sortOrderOffset = sortOffset;
			Type type = LevelEvent_Base.level.GetType();
			Type type2 = null;
			if (methodName.StartsWith("vfx."))
			{
				methodName = methodName.Remove(0, 4);
				type2 = typeof(scrVfxControl);
				instance = base.vfx;
			}
			else if (methodName.StartsWith("level."))
			{
				methodName = methodName.Remove(0, 6);
			}
			else if (methodName.StartsWith("room"))
			{
				if (methodName.StartsWith("room[0].") || methodName.StartsWith("room[1].") || methodName.StartsWith("room[2].") || methodName.StartsWith("room[3]."))
				{
					int num = methodName[5] - 48;
					methodName = methodName.Remove(0, 8);
					type2 = typeof(RDRoom);
					instance = base.game.rooms[num];
				}
				else if (methodName.StartsWith("room1.") || methodName.StartsWith("room2.") || methodName.StartsWith("room3.") || methodName.StartsWith("room4."))
				{
					int num2 = methodName[4] - 48;
					methodName = methodName.Remove(0, 6);
					type2 = typeof(RDRoom);
					instance = base.game.rooms[num2 - 1];
				}
			}
			bool flag = methodName.IndexOf("(") != -1 && methodName.IndexOf("=") == -1;
			if (!flag)
			{
				_ = methodName;
				int num3 = methodName.IndexOf("=");
				if (num3 != -1)
				{
					operation = Operation.Set;
				}
				else
				{
					num3 = methodName.IndexOf("++");
					if (num3 != -1)
					{
						operation = Operation.Increment;
					}
					else
					{
						num3 = methodName.IndexOf("--");
						operation = Operation.Decrement;
					}
				}
				if (num3 == -1)
				{
					operation = Operation.Get;
				}
				string text = ((num3 != -1) ? methodName.Substring(0, num3).Trim() : methodName.Trim());
				if (operation == Operation.Set)
				{
					strArg = methodName.Substring(num3 + 1, methodName.Length - num3 - 1).Trim();
				}
				if (type2 != null)
				{
					field = type2.GetField(text);
				}
				if (field == null)
				{
					field = type.GetField(text);
				}
				if (field != null && instance == null)
				{
					instance = LevelEvent_Base.level;
				}
				if (field == null)
				{
					flag = true;
				}
			}
			if (flag)
			{
				string text2 = methodName;
				string text3 = null;
				bool flag2 = false;
				int num4 = methodName.IndexOf("(");
				if (num4 > 0)
				{
					text2 = methodName.Substring(0, num4).Trim();
					text3 = methodName.Substring(num4, methodName.Length - num4);
					flag2 = true;
				}
				try
				{
					if (type2 != null)
					{
						method = type2.GetMethod(text2);
					}
					if (method == null && type != null)
					{
						method = type.GetMethod(text2);
					}
				}
				catch (Exception)
				{
					method = null;
				}
				if (method != null && instance == null)
				{
					instance = LevelEvent_Base.level;
				}
				if (flag2)
				{
					argString = text3.GetParameters();
					if (argString != null)
					{
						hasArguments = true;
					}
				}
				if (method == null)
				{
					Debug.LogWarning("CallCustomMethod: Method " + methodName + " doesn't exist");
				}
			}
			prepared = true;
			yield return null;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				anyErrorDecoding = false;
				if (method != null)
				{
					List<object> list = new List<object>();
					if (hasArguments)
					{
						string[] array = argString;
						for (int i = 0; i < array.Length; i++)
						{
							string text = array[i].Trim();
							if (text.Contains("str:"))
							{
								list.Add(text.Remove(0, 4));
							}
							else if (text.StartsWith('"') && text.EndsWith('"') && text.Length > 1)
							{
								list.Add(text.Substring(1, text.Length - 2));
							}
							else if (text.Contains("true"))
							{
								list.Add(true);
							}
							else if (text.Contains("false"))
							{
								list.Add(false);
							}
							else if (text.Contains("."))
							{
								list.Add(DecodeFloatWrapper(text));
							}
							else
							{
								list.Add(DecodeIntWrapper(text));
							}
						}
					}
					if (anyErrorDecoding)
					{
						return;
					}
					method.Invoke(instance, hasArguments ? list.ToArray() : null);
				}
				else if (field != null)
				{
					if (operation == Operation.Set)
					{
						object obj = null;
						if (strArg.Contains("str:"))
						{
							obj = RDEditorUtils.DecodeString(strArg).Remove(0, 4);
						}
						else
						{
							object obj2 = null;
							try
							{
								obj2 = base.game.currentLevel.EvalStringWithVariables(strArg);
							}
							catch (Exception ex)
							{
								anyErrorDecoding = true;
								Debug.Log("ERROR decoding " + strArg + " in custom method " + methodName + ": " + ex.ToString());
								return;
							}
							obj = ((field.FieldType == typeof(bool)) ? ((object)Convert.ToBoolean(obj2)) : ((!(field.FieldType == typeof(int))) ? ((object)Convert.ToSingle(obj2)) : ((object)Convert.ToInt32(obj2))));
						}
						field.SetValue(instance, obj);
					}
					else if (operation == Operation.Increment)
					{
						if (field.FieldType == typeof(int))
						{
							field.SetValue(instance, (int)field.GetValue(instance) + 1);
						}
						else
						{
							field.SetValue(instance, (float)field.GetValue(instance) + 1f);
						}
					}
					else if (operation == Operation.Decrement)
					{
						if (field.FieldType == typeof(int))
						{
							field.SetValue(instance, (int)field.GetValue(instance) - 1);
						}
						else
						{
							field.SetValue(instance, (float)field.GetValue(instance) - 1f);
						}
					}
					else
					{
						_ = operation;
						_ = 3;
					}
				}
				if (base.editor != null)
				{
					base.editor.ipm.UpdateBlankPanel();
				}
			});
		}

		public int DecodeIntWrapper(string str)
		{
			if (int.TryParse(str, out var result))
			{
				return result;
			}
			try
			{
				return Convert.ToInt32(base.game.currentLevel.EvalStringWithVariables(str));
			}
			catch (Exception arg)
			{
				anyErrorDecoding = true;
				Debug.Log($"ERROR decoding {str} in custom method {methodName}: {arg}");
				return 0;
			}
		}

		public float DecodeFloatWrapper(string str)
		{
			if (float.TryParse(str, out var result))
			{
				return result;
			}
			try
			{
				return RDEditorUtils.DecodeFloat(base.game.currentLevel.EvalStringWithVariables(str));
			}
			catch (Exception arg)
			{
				anyErrorDecoding = true;
				Debug.Log($"ERROR decoding {str} in custom method {methodName}: {arg}");
				return 0f;
			}
		}

		public override string GetTooltipText()
		{
			return methodName;
		}
	}
}
