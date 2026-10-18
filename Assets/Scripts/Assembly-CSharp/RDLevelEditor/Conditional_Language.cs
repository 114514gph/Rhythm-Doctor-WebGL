using UnityEngine;

namespace RDLevelEditor
{
	[ConditionalInfo(true)]
	public class Conditional_Language : Conditional
	{
		[JsonProperty("Language", "", null, "", false, true, null, null)]
		public SystemLanguage languageName { get; set; }

		public Conditional_Language(int id)
			: base(id)
		{
		}

		public override bool Check(LevelEvent_Base levelEvent, bool negated)
		{
			return (RDString.language == languageName).NegateIf(negated);
		}

		public override string ToString()
		{
			return RDString.Get("editor.language") + " " + RDString.Get("enum.SystemLanguage." + languageName);
		}
	}
}
