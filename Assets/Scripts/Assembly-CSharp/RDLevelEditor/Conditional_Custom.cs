using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	[ConditionalInfo(false)]
	public class Conditional_Custom : Conditional
	{
		private enum Operation
		{
			Equals = 0,
			More = 1,
			Less = 2,
			Not = 3
		}

		[JsonProperty("expression", "", null, "", false, true, null, null)]
		public string customExpression { get; set; }

		public Conditional_Custom(int id)
			: base(id)
		{
		}

		public override bool Check(LevelEvent_Base levelEvent, bool negated)
		{
			bool toNegate = false;
			string str = customExpression.Trim();
			str = base.game.currentLevel.EvaluateCurlyBracketsInString(str);
			if (str.StartsWith("atLeastNPerfects"))
			{
				int num = "atLeastNPerfects".Length + 1;
				int length = str.Length - num - 1;
				List<scnGame.HitInfo> allHitOffsets = base.game.allHitOffsets;
				string[] array = str.Substring(num, length).Split(',');
				int val = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				val = Math.Min(allHitOffsets.Count, val);
				int num3 = 0;
				for (int i = allHitOffsets.Count - val; i < allHitOffsets.Count; i++)
				{
					if (allHitOffsets[i].offsetType == OffsetType.Perfect)
					{
						num3++;
					}
				}
				toNegate = num3 >= num2;
			}
			else if (str.StartsWith("atLeastRank"))
			{
				int num4 = "atLeastRank".Length + 1;
				int length2 = str.Length - num4 - 1;
				string text = str.Substring(num4, length2);
				if (text == "S+")
				{
					toNegate = (int)base.game.currentLevel.GetRankFromMistakes() == 15;
				}
				else
				{
					Rank rank = Rank.FromString(text);
					toNegate = base.game.currentLevel.GetRankFromMistakes().ComparativeInt() >= rank.ComparativeInt();
				}
			}
			else if (str.StartsWith("passedLevel"))
			{
				int num5 = "atLeastRank".Length + 1;
				int length3 = str.Length - num5 - 1;
				if (Enum.TryParse<Level>(str.Substring(num5, length3), out var result))
				{
					toNegate = result.Passed();
				}
			}
			else if (str.TrimAllSpaces().StartsWith("buttonPressCount>2"))
			{
				toNegate = base.game.currentLevel.buttonPressCount > 2;
			}
			else if (str.TrimAllSpaces() == "neverRun")
			{
				toNegate = levelEvent.timesRun == 0;
			}
			else if (str.TrimAllSpaces() == "voiceLanguage==English")
			{
				toNegate = RDString.voiceLanguage == SystemLanguage.English;
			}
			else if (str.TrimAllSpaces() == "voiceLanguage==Chinese")
			{
				toNegate = RDString.voiceLanguage == SystemLanguage.Chinese;
			}
			else if (str.TrimAllSpaces() == "platform==Windows")
			{
				toNegate = scnBase.isWindows;
			}
			else if (str.TrimAllSpaces() == "platform==Mac")
			{
				toNegate = scnBase.isMac;
			}
			else if (str.TrimAllSpaces() == "windowDanceCanVerticalWrap==true")
			{
				toNegate = true;
			}
			else if (str.TrimAllSpaces() == "windowDanceCanVerticalWrap==false")
			{
				toNegate = false;
			}
			else if (str.TrimAllSpaces() == "true")
			{
				toNegate = true;
			}
			else if (str.TrimAllSpaces() == "false")
			{
				toNegate = false;
			}
			else
			{
				switch (str)
				{
				case "2pMode":
					toNegate = GC.twoPlayerMode;
					break;
				case "1pMode":
					toNegate = !GC.twoPlayerMode;
					break;
				case "narration":
					toNegate = Narration.IsEnabled;
					break;
				case "booth":
					toNegate = RDC.booth;
					break;
				case "paigeLeaves":
					toNegate = !Persistence.GetPaigeEnding();
					break;
				default:
				{
					int num6 = 0;
					bool flag = false;
					if (str.StartsWith("var"))
					{
						flag = true;
					}
					if (flag)
					{
						num6 = "var".Length + 1;
					}
					Operation operation = Operation.Equals;
					int num7 = -1;
					num7 = str.IndexOf("==");
					if (num7 != -1)
					{
						operation = Operation.Equals;
					}
					else
					{
						num7 = str.IndexOf(">");
						if (num7 != -1)
						{
							operation = Operation.More;
						}
						else
						{
							num7 = str.IndexOf("<");
							if (num7 != -1)
							{
								operation = Operation.Less;
							}
							else
							{
								num7 = str.IndexOf("!=");
								if (num7 != -1)
								{
									operation = Operation.Not;
								}
							}
						}
					}
					int num8 = num7 - 1;
					int length4 = num8 - num6 + 1;
					string text2 = "";
					text2 = ((num7 == -1) ? str.Substring(num6, str.Length - num6 - (flag ? 1 : 0)) : str.Substring(num6, length4));
					object obj = null;
					object obj2 = null;
					bool flag2 = false;
					LevelBase currentLevel = base.game.currentLevel;
					bool flag3 = false;
					bool flag4 = true;
					if (num7 == -1)
					{
						flag4 = false;
						toNegate = (bool)currentLevel.EvalStringWithVariables(text2);
					}
					if (flag4)
					{
						int num9 = num8 + 2;
						if (operation == Operation.Equals || operation == Operation.Not)
						{
							num9++;
						}
						string text3 = str.Substring(num9, str.Length - num9 - (flag ? 1 : 0));
						if (text3 == "true" || text3 == "false")
						{
							flag3 = true;
						}
						if (flag3)
						{
							obj = currentLevel.GetVariable(text2);
							flag2 = text3 == "true";
							toNegate = (bool)obj == flag2;
						}
						else
						{
							obj = currentLevel.EvalStringWithVariables(text2);
							obj2 = currentLevel.EvalStringWithVariables(text3);
						}
					}
					if (!flag4 || flag3)
					{
						break;
					}
					bool flag5 = false;
					if (obj is bool || obj2 is bool)
					{
						flag5 = true;
					}
					if (flag5)
					{
						if (!(obj is bool) || !(obj2 is bool))
						{
							Debug.LogError("attempted bool to non-bool comparison");
							toNegate = false;
							break;
						}
						switch (operation)
						{
						case Operation.Equals:
							toNegate = (bool)obj == (bool)obj2;
							break;
						case Operation.Not:
							toNegate = (bool)obj == (bool)obj2;
							break;
						default:
							Debug.LogError("operation not compatible with bool");
							toNegate = false;
							break;
						}
						break;
					}
					double result2 = 0.0;
					double result3 = 0.0;
					bool flag6 = false;
					if (!double.TryParse(obj.ToString(), out result2))
					{
						flag6 = true;
					}
					if (!double.TryParse(obj2.ToString(), out result3))
					{
						flag6 = true;
					}
					if (flag6)
					{
						Debug.LogError("parsing error - couldnt convert to double");
						return false;
					}
					switch (operation)
					{
					case Operation.Equals:
						toNegate = result2 == result3;
						break;
					case Operation.Less:
						toNegate = result2 < result3;
						break;
					case Operation.More:
						toNegate = result2 > result3;
						break;
					case Operation.Not:
						toNegate = result2 != result3;
						break;
					}
					break;
				}
				}
			}
			return toNegate.NegateIf(negated);
		}

		public override string ToString()
		{
			return customExpression switch
			{
				"voiceLanguage==English" => RDString.Get("enum.SystemLanguage.English") + " " + RDString.Get("editor.Conditionals.customLanguage"), 
				"voiceLanguage==Chinese" => RDString.Get("enum.SystemLanguage.Chinese") + " " + RDString.Get("editor.Conditionals.customLanguage"), 
				"platform==Windows" => "Windows " + RDString.Get("editor.Conditionals.customPlatform"), 
				"platform==Mac" => "Mac " + RDString.Get("editor.Conditionals.customPlatform"), 
				_ => string.Empty, 
			};
		}
	}
}
