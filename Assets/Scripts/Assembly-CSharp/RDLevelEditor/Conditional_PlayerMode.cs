using UnityEngine;

namespace RDLevelEditor
{
	[ConditionalInfo(true)]
	public class Conditional_PlayerMode : Conditional
	{
		[JsonProperty("twoPlayerMode", "", null, "", false, true, null, null)]
		public bool playerModeIsTwoPlayer { get; set; }

		public Conditional_PlayerMode(int id)
			: base(id)
		{
		}

		public Conditional_PlayerMode(string gid)
			: base(gid)
		{
		}

		public override bool Check(LevelEvent_Base levelEvent, bool negated)
		{
			return (GC.twoPlayerMode == playerModeIsTwoPlayer).NegateIf(negated);
		}

		public override string ToString()
		{
			return RDString.Get(playerModeIsTwoPlayer ? "editor.Conditionals.playerModeTwo" : "editor.Conditionals.playerModeOne");
		}

		public override Sprite GetSprite()
		{
			if (!playerModeIsTwoPlayer)
			{
				return Resources.Load<Sprite>("ConditionalIcons/PlayerMode_1p");
			}
			return base.GetSprite();
		}
	}
}
