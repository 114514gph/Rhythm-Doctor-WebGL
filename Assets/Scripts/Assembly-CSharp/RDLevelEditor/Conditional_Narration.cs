namespace RDLevelEditor
{
	[ConditionalInfo(false)]
	public class Conditional_Narration : Conditional
	{
		[JsonProperty("narration", "", null, "", false, true, null, null)]
		public bool narrationEnabled { get; set; }

		public Conditional_Narration(int id)
			: base(id)
		{
		}

		public Conditional_Narration(string gid)
			: base(gid)
		{
		}

		public override bool Check(LevelEvent_Base levelEvent, bool negated)
		{
			return (Narration.IsEnabled == narrationEnabled).NegateIf(negated);
		}

		public override string ToString()
		{
			return RDString.Get("editor.Conditionals.narrationEnabled");
		}
	}
}
