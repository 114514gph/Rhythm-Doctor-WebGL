using System;

namespace RDLevelEditor
{
	[ConditionalInfo(false)]
	public class Conditional_Composite : Conditional
	{
		[JsonProperty("compositeExpression", "", null, "", false, true, null, null)]
		public string compositeExpression { get; set; }

		public Conditional_Composite(int id)
			: base(id)
		{
		}

		public override bool Check(LevelEvent_Base levelEvent, bool negated)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return RDString.Get("enum.ConditionalType.Composite");
		}
	}
}
