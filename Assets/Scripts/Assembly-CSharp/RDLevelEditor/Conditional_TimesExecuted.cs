using UnityEngine;

namespace RDLevelEditor
{
	[ConditionalInfo(false)]
	public class Conditional_TimesExecuted : Conditional
	{
		[JsonProperty("maxTimes", "", null, "", false, true, null, null)]
		public int maxTimes { get; set; }

		public Conditional_TimesExecuted(int id)
			: base(id)
		{
		}

		public Conditional_TimesExecuted(string gid)
			: base(gid)
		{
		}

		public override bool Check(LevelEvent_Base levelEvent, bool negated)
		{
			return (levelEvent.timesRun < maxTimes).NegateIf(negated);
		}

		public override string ToString()
		{
			return RDString.Get("editor.Conditionals.timesExecutedDescription").Replace("[maxTimes]", maxTimes.ToString());
		}

		public override Sprite GetSprite()
		{
			if (maxTimes == 1)
			{
				return Resources.Load<Sprite>("ConditionalIcons/TimesExecuted_Once");
			}
			return base.GetSprite();
		}
	}
}
