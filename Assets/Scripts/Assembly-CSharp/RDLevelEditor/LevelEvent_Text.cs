using System.Collections;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -10, RoomsUsage.OneRoom, false, false, true, false, 0, false)]
	public class LevelEvent_Text : LevelEvent_MakeDecorationBase
	{
		public LyricsGame lyrics;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public string decoName { get; set; } = "";

		[JsonProperty("", "", null, "", false, true, null, null)]
		public string font { get; set; } = TextFont.Default.ToString();

		[JsonProperty("", "", null, "", false, true, null, null)]
		public RDSortingLayer sortingLayer { get; set; } = RDSortingLayer.Foreground;

		public override void Init()
		{
			base.Init();
			string withCheck = RDString.GetWithCheck("editor.Text.decorationHeaderName", out var exists);
			if (exists)
			{
				decoName = withCheck;
			}
		}

		public override IEnumerator Prepare()
		{
			prepared = true;
			lyrics = LyricsGame.Create().Setup(base.decorationId, font, sortingLayer, base.depth);
			yield break;
		}

		public override void Run()
		{
		}
	}
}
