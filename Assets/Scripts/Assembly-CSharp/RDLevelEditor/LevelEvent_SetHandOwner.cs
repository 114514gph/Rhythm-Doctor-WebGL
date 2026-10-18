using System.Collections.Generic;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_SetHandOwner : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Hand]
		public Hand hand { get; set; } = Hand.Right;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[CharacterPicker(new Character[]
		{
			Character.Player,
			Character.Ian,
			Character.Paige,
			Character.Edega
		}, false, false, false, false, false, false, false, false, true)]
		public Character character { get; set; } = Character.Player;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				int[] array = rooms;
				foreach (int num in array)
				{
					scrHandController scrHandController2 = base.game.handControllerOnTop;
					switch (num)
					{
					case 0:
						scrHandController2 = base.game.handController0;
						break;
					case 1:
						scrHandController2 = base.game.handController1;
						break;
					case 2:
						scrHandController2 = base.game.handController2;
						break;
					case 3:
						scrHandController2 = base.game.handController3;
						break;
					}
					if (scrHandController2.DoesEnumTargetLeftHand(hand))
					{
						scrHandController2.leftArm.SetToCharacter(character);
					}
					if (scrHandController2.DoesEnumTargetRightHand(hand))
					{
						scrHandController2.rightArm.SetToCharacter(character);
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			List<string> values = new List<string>
			{
				RDString.GetEnumValue(hand),
				RDString.GetEnumValue(character)
			};
			return string.Join('\n', values);
		}
	}
}
