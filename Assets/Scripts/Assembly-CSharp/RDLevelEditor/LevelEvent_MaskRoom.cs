using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_MaskRoom : LevelEvent_Base
	{
		private Texture2D[] textures;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public MaskType maskType { get; set; }

		[JsonProperty("image", "", null, "", false, true, "EnableIfImage", null)]
		[Image(true)]
		public string[] images { get; set; } = Array.Empty<string>();

		[JsonProperty("", "", null, "", false, true, "EnableFpsIf", null)]
		public float fps { get; set; } = 30f;

		[JsonProperty("", "", null, "", false, true, "EnableIfRoom", null)]
		[Room(false, false)]
		public int sourceRoom { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfColor", null)]
		public ColorOrPalette keyColor { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, "EnableIfColor", null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		public int colorCutoff { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfColor", null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		public int colorFeathering { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableAlphaModeIf", null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public AlphaMode alphaMode { get; set; }

		public bool EnableIfImage()
		{
			return maskType == MaskType.Image;
		}

		public bool EnableFpsIf()
		{
			if (EnableIfImage())
			{
				return images.Length > 1;
			}
			return false;
		}

		public bool EnableIfRoom()
		{
			return maskType == MaskType.Room;
		}

		public bool EnableIfColor()
		{
			return maskType == MaskType.Color;
		}

		public bool EnableAlphaModeIf()
		{
			return maskType != MaskType.None;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (dict.ContainsKey("image") && dict["image"] is string text)
			{
				dict["image"] = ((text == string.Empty) ? new List<object>() : new List<object> { text });
			}
			base.Decode(dict);
			if (dict.ContainsKey("rooms"))
			{
				roomsUsage = RoomsUsage.ManyRooms;
			}
		}

		public override IEnumerator Prepare()
		{
			if (images.Length != 0)
			{
				textures = new Texture2D[images.Length];
				int num = 0;
				string[] array = images;
				foreach (string image in array)
				{
					textures[num] = RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, image, "", fromCustomCharactersFolder: false, isAlpha8: true);
					num++;
				}
			}
			prepared = true;
			yield return null;
		}

		public override void Run()
		{
			int[] array = ((roomsUsage != RoomsUsage.ManyRooms) ? new int[1] { y } : rooms);
			if (maskType == MaskType.Image && (textures == null || textures.Length == 0 || textures[0] == null))
			{
				maskType = MaskType.None;
			}
			int[] array2 = array;
			foreach (int room in array2)
			{
				RunOnBeat(() =>
				{
					RDRoom rDRoom = base.game.rooms[room];
					if (maskType == MaskType.Image)
					{
						rDRoom.roomMask = null;
						bool flag = textures != null && textures.Length != 0;
						if (flag)
						{
							rDRoom.ShowMasks(textures, FilterMode.Point, fps);
						}
						rDRoom.SetUsingMask(flag);
						rDRoom.SetUsingChromakey(usingChromakey: false);
					}
					else if (maskType == MaskType.Room)
					{
						rDRoom.SetRoomMask(sourceRoom);
						rDRoom.SetUsingMask(usingMask: true);
						rDRoom.SetUsingChromakey(usingChromakey: false);
					}
					else if (maskType == MaskType.Color)
					{
						rDRoom.SetColorMask(keyColor.ToColor(), (float)colorCutoff * 0.01f, (float)colorFeathering * 0.01f);
						rDRoom.SetUsingMask(usingMask: false);
						rDRoom.SetUsingChromakey(usingChromakey: true);
					}
					else
					{
						rDRoom.SetUsingMask(usingMask: false);
						rDRoom.SetUsingChromakey(usingChromakey: false);
					}
					rDRoom.SetMaskAlphaMode(maskType != MaskType.None && alphaMode == AlphaMode.Inverted);
				});
			}
		}

		public override string CustomControlName()
		{
			return "Room";
		}

		public override string GetTooltipText()
		{
			if (images == null || images.Length == 0)
			{
				return null;
			}
			if (images.Length > 4)
			{
				return string.Format("{0},\n+{1}...", string.Join(",\n", images.Take(4)), images.Length - 4);
			}
			return string.Join(",\n", images);
		}
	}
}
