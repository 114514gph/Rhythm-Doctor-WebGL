using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_Blend : LevelEvent_Base, ICompatibleDecorationTypeChecker
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "None", "Additive", "Multiply", "Invert" }, false, false)]
		public SpriteBlendType blendType { get; set; }

		public bool isCompatibleDecorationType => base.ownerDecoration is LevelEvent_Sprite;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType)
				{
					if (!LevelEvent_Base.level.sprites.ContainsKey(target))
					{
						Debug.Log("Blend - didn't found matching id " + target);
					}
					else
					{
						Material material = LevelEvent_Base.level.sprites[target].GetComponent<Renderer>().material;
						switch (blendType)
						{
						case SpriteBlendType.Additive:
							material.SetInt("_BlendFirst", 1);
							material.SetInt("_BlendSecond", 1);
							break;
						case SpriteBlendType.Multiply:
							material.SetInt("_BlendFirst", 2);
							material.SetInt("_BlendSecond", 10);
							break;
						case SpriteBlendType.Invert:
							material.SetInt("_BlendFirst", 4);
							material.SetInt("_BlendSecond", 10);
							break;
						default:
							material.SetInt("_BlendFirst", 1);
							material.SetInt("_BlendSecond", 10);
							break;
						}
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			return RDString.GetEnumValue(blendType);
		}
	}
}
