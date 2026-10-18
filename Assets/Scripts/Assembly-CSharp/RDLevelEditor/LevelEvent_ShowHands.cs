using System.Collections.Generic;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_ShowHands : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Hand]
		public Hand hand { get; set; } = Hand.Right;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(null, false, true)]
		public HandAction action { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableExtentIf", null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public HandExtent extent { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableForceRaiseIf", null)]
		[Checkbox]
		public bool forceRaise { get; set; } = true;

		[JsonProperty("align", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public bool alignVisibleArea { get; set; }

		[JsonProperty("", "", "editor.ShowHands.transition", "", false, true, null, null)]
		[ToggleGroup(false, null, "editor.ShowHands.instant", null, true, 14)]
		public bool instant { get; set; }

		public bool EnableForceRaiseIf()
		{
			if (action != HandAction.Show)
			{
				return action == HandAction.Hide;
			}
			return true;
		}

		public bool EnableExtentIf()
		{
			if (action == HandAction.Show)
			{
				return hand != Hand.Both;
			}
			return false;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				float duration = ((action == HandAction.Show || action == HandAction.Hide) ? 0.75f : 0.25f);
				if (instant)
				{
					duration = 0f;
				}
				int[] array = rooms;
				for (int i = 0; i < array.Length; i++)
				{
					scrHandController scrHandController2 = array[i] switch
					{
						0 => base.game.handController0, 
						1 => base.game.handController1, 
						2 => base.game.handController2, 
						3 => base.game.handController3, 
						_ => base.game.handControllerOnTop, 
					};
					scrHandController2.lastHandAction = action;
					scrHandController2.ToggleAlignWithRoomComp(alignVisibleArea);
					bool num = scrHandController2.DoesEnumTargetLeftHand(hand);
					RDArm rDArm = (num ? scrHandController2.leftArm : scrHandController2.rightArm);
					RDArm rDArm2 = (num ? scrHandController2.rightArm : scrHandController2.leftArm);
					if (hand == Hand.Both || (action == HandAction.Show && rDArm2.currentAction == HandAction.Show))
					{
						scrHandController2.leftArm.Toggle(action, duration, togglingBothHands: true, forceRaise);
						scrHandController2.rightArm.Toggle(action, duration, togglingBothHands: true, forceRaise);
					}
					else
					{
						bool togglingBothHands = action == HandAction.Show && extent == HandExtent.Short;
						rDArm.Toggle(action, duration, togglingBothHands, forceRaise);
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			List<string> values = new List<string>
			{
				RDString.GetEnumValue(hand),
				RDString.GetEnumValue(action)
			};
			return string.Join('\n', values);
		}
	}
}
