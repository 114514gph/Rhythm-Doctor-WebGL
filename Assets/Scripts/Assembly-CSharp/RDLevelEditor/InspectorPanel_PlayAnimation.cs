using System.Collections.Generic;

namespace RDLevelEditor
{
	public class InspectorPanel_PlayAnimation : InspectorPanel
	{
		private string[] GetExpressions()
		{
			List<string> list = new List<string>();
			LevelEvent_PlayAnimation levelEvent_PlayAnimation = currentLevelEvent as LevelEvent_PlayAnimation;
			foreach (LevelEvent_MakeDecorationBase decorationsDatum in base.editor.decorationsData)
			{
				if (!(decorationsDatum.decorationId == levelEvent_PlayAnimation.target) || !(decorationsDatum is LevelEvent_Sprite levelEvent_Sprite))
				{
					continue;
				}
				if (levelEvent_Sprite.character != Character.Custom)
				{
					foreach (string key in scrChar.baseCharacterAnimations[levelEvent_Sprite.character].clips.Keys)
					{
						list.Add(key);
					}
				}
				else
				{
					string filename = levelEvent_Sprite.filename;
					Dictionary<string, CustomAnimationData> customCharacterData = base.editor.customCharacterData;
					if (!customCharacterData.ContainsKey(filename))
					{
						RDBaseDllDummy.printem("Custom character data for " + filename + " hasn't been loaded.");
					}
					else
					{
						foreach (string key2 in customCharacterData[filename].clips.Keys)
						{
							list.Add(key2);
						}
					}
				}
				return list.ToArray();
			}
			return null;
		}

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			if (!((LevelEvent_PlayAnimation)levelEvent).isCompatibleDecorationType)
			{
				return;
			}
			foreach (Property property in properties)
			{
				if (property.name == "expression")
				{
					((PropertyControl_Dropdown)property.control).SetDropdownOptions(GetExpressions(), localize: false);
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			if (((LevelEvent_PlayAnimation)levelEvent).isCompatibleDecorationType)
			{
				base.SaveProperties(levelEvent);
			}
		}
	}
}
