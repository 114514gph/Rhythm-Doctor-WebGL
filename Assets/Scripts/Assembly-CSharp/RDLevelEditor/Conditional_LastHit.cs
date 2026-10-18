using System.Collections.Generic;

namespace RDLevelEditor
{
	[ConditionalInfo(false)]
	public class Conditional_LastHit : Conditional
	{
		[JsonProperty("row", "", null, "", false, true, null, null)]
		public int row { get; set; }

		[JsonProperty("result", "", null, "", false, true, null, null)]
		public OffsetType resultType { get; set; }

		public Conditional_LastHit(int id)
			: base(id)
		{
		}

		public override bool Check(LevelEvent_Base levelEvent, bool negated)
		{
			List<scnGame.HitInfo> list = ((row == -1) ? base.game.allHitOffsets : base.game.rowsHitOffsets[row]);
			if (list.Count == 0)
			{
				return false;
			}
			OffsetType offsetType = list.GetLastObject().offsetType;
			if (resultType == OffsetType.AnyEarlyOrLate)
			{
				if (!negated)
				{
					if (offsetType != OffsetType.SlightlyEarly && offsetType != OffsetType.SlightlyLate && offsetType != OffsetType.VeryEarly)
					{
						return offsetType == OffsetType.VeryLate;
					}
					return true;
				}
				if (offsetType != OffsetType.Perfect)
				{
					return offsetType == OffsetType.Missed;
				}
				return true;
			}
			if (!negated)
			{
				return offsetType == resultType;
			}
			if (offsetType != OffsetType.Null)
			{
				return offsetType != resultType;
			}
			return false;
		}

		public override string ToString()
		{
			string text = RDString.Get("enum.OffsetType." + resultType);
			return RDString.Get("editor.Conditionals.lastHitDescription") + " " + text;
		}
	}
}
