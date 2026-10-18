using System.Collections.Generic;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_FlipScreen : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[FlipScreen]
		public bool[] flip { get; set; } = new bool[2] { true, false };

		public override string Encode()
		{
			return base.Encode().Replace(", \"flip\": [" + flip[0].ToString().ToLower() + ", " + flip[1].ToString().ToLower() + "]", ", \"flipX\": " + flip[0].ToString().ToLower() + ", \"flipY\": " + flip[1].ToString().ToLower());
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (dict.ContainsKey("flipX") && dict.ContainsKey("flipY"))
			{
				flip[0] = bool.Parse(dict["flipX"].ToString());
				flip[1] = bool.Parse(dict["flipY"].ToString());
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				int[] array = rooms;
				foreach (int roomIndex in array)
				{
					base.vfx.SetFlipScreenX(flip[0], roomIndex);
					base.vfx.SetFlipScreenY(flip[1], roomIndex);
				}
			});
		}
	}
}
