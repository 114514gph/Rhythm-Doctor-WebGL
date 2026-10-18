using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_PlayAnimation : LevelEvent_Base, ICompatibleDecorationTypeChecker
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(null, false, false)]
		public string expression { get; set; } = "";

		public bool isCompatibleDecorationType => base.ownerDecoration is LevelEvent_Sprite;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType)
				{
					if (!LevelEvent_Base.level.sprites.ContainsKey(target))
					{
						Debug.Log("PlayAnimation R - didn't found matching id " + target);
					}
					else
					{
						LevelEvent_Base.level.sprites[target].PlayExpression(expression);
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			string withCheck = RDString.GetWithCheck("editor.expression." + expression, out var exists);
			if (!exists)
			{
				return expression;
			}
			return withCheck;
		}
	}
}
