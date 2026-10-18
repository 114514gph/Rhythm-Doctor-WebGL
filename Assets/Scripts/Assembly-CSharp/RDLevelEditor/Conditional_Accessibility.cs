using UnityEngine;

namespace RDLevelEditor
{
	[ConditionalInfo(true)]
	public class Conditional_Accessibility : Conditional
	{
		[JsonProperty("effectType", "", null, "", false, true, null, null)]
		public ConditionalEffectType effectType { get; set; }

		public Conditional_Accessibility(int id)
			: base(id)
		{
		}

		public Conditional_Accessibility(string gid)
			: base(gid)
		{
		}

		public override bool Check(LevelEvent_Base levelEvent, bool negated)
		{
			bool toNegate = false;
			switch (effectType)
			{
			case ConditionalEffectType.Flashy:
				toNegate = !base.game.reducedFlash;
				break;
			case ConditionalEffectType.Narration:
				toNegate = Narration.IsEnabled;
				break;
			}
			return toNegate.NegateIf(negated);
		}

		public override string ToString()
		{
			return RDString.GetEnumValue(effectType);
		}

		public override Sprite GetSprite()
		{
			return effectType switch
			{
				ConditionalEffectType.Flashy => Resources.Load<Sprite>("ConditionalIcons/Accessibility_Flashy"), 
				ConditionalEffectType.Narration => Resources.Load<Sprite>("ConditionalIcons/Narration"), 
				_ => base.GetSprite(), 
			};
		}
	}
}
