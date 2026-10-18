using System;
using System.Collections.Generic;
using System.Linq;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_ChangePlayersRows : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		public PlayerMode playerMode { get; set; } = PlayerMode.OneOrTwoPlayers;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public RDPlayer[] players { get; set; } = Enumerable.Repeat(RDPlayer.NoChange, 16).ToArray();

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Character[] cpuMarkers { get; set; } = Enumerable.Repeat(Character.Otto, 16).ToArray();

		[JsonProperty("", "", null, "", false, true, "EnableFlashingIf", null)]
		public bool flashingOnBeat { get; set; } = true;

		public bool EnableFlashingIf()
		{
			return !flashingOnBeat;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (!dict.ContainsKey("flashingOnBeat"))
			{
				flashingOnBeat = RDLevelData.current.settings.version >= 25;
			}
			if (!dict.ContainsKey("cpuMarkers"))
			{
				Array.Fill(cpuMarkers, Character.None);
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				bool singleplayer = ((playerMode == PlayerMode.OneOrTwoPlayers) ? (!GC.twoPlayerMode) : (playerMode == PlayerMode.OnePlayer));
				base.game.ChangeRowPlayers(singleplayer, players, base.beat - 1f, 0, flashingOnBeat, !LevelEvent_Base.level.disableRowChangeWarningFlashes, cpuMarkers);
			});
		}

		public override void TaggedActionVariant()
		{
			bool singleplayer = ((playerMode == PlayerMode.OneOrTwoPlayers) ? (!GC.twoPlayerMode) : (playerMode == PlayerMode.OnePlayer));
			base.game.ChangeRowPlayersImmediately(singleplayer, players, base.beat - 1f, 0, flashingOnBeat, !LevelEvent_Base.level.disableRowChangeWarningFlashes, cpuMarkers);
			base.TaggedActionVariant();
		}

		public override string GetTooltipText()
		{
			return RDString.GetEnumValue(playerMode);
		}
	}
}
