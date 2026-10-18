using System;
using System.Linq;

namespace RDLevelEditor
{
	public abstract class LevelEvent_MakeDecorationBase : LevelEvent_Base
	{
		public bool failedLoadingCustomCharacter;

		private static Random random = new Random();

		[JsonProperty("id", "", null, "", false, true, null, null)]
		public string decorationId { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public int depth { get; set; } = 1;

		public override void Init()
		{
			base.Init();
			decorationId = RandomString(7);
			usesY = false;
		}

		public static string RandomString(int length)
		{
			return new string((from s in Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz0123456789", length)
				select s[random.Next(s.Length)]).ToArray());
		}

		public override string Encode()
		{
			if (RDLevelData.encodingLevelEvents)
			{
				return null;
			}
			RDEditorUtils.IndentationLevel = 0;
			return base.Encode();
		}
	}
}
