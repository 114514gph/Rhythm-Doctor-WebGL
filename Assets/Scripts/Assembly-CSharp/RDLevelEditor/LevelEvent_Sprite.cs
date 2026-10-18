using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -10, RoomsUsage.OneRoom, false, false, true, false, 0, false)]
	public class LevelEvent_Sprite : LevelEvent_MakeDecorationBase
	{
		private CustomAnimation customAnimation;

		private const string defaultSpritePath = "DefaultSprite";

		[JsonProperty("", "", null, "", false, true, "EnableFilenameIf", null)]
		public string filename { get; set; } = "";

		[JsonProperty("", "", null, "", false, true, "EnableCharacterIf", null)]
		public Character character { get; set; } = Character.Beans;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public bool visible { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public bool preview { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableFilterIf", null)]
		public TextureFilter filter { get; set; }

		public bool EnableCharacterIf()
		{
			if (filename.IsNullOrEmpty())
			{
				return character != Character.Custom;
			}
			return false;
		}

		public bool EnableFilenameIf()
		{
			return !EnableCharacterIf();
		}

		public bool EnableFilterIf()
		{
			return !filename.IsNullOrEmpty();
		}

		public override void Init()
		{
			base.Init();
			filename = string.Empty;
			character = Character.Beans;
		}

		public override IEnumerator Prepare()
		{
			UpdateCustomCharacter(justReturnErrors: false);
			UpdateCustomCharacter(justReturnErrors: false, editorRowHeader: true);
			prepared = true;
			CreateCharacter();
			PlayExpression("neutral");
			yield break;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (!filename.IsNullOrEmpty())
			{
				character = Character.Custom;
			}
			if (base.editorMode)
			{
				UpdateCustomCharacter(justReturnErrors: true);
				UpdateCustomCharacter(justReturnErrors: false, editorRowHeader: true);
			}
		}

		private void CreateCharacter()
		{
			Transform decorationContainer = base.game.rooms[base.room].decorationContainer;
			GameObject gameObject = Object.Instantiate(base.gc.customSprite, decorationContainer);
			CustomAnimation component = gameObject.GetComponent<CustomAnimation>();
			component.data = ((character == Character.Custom) ? LevelEvent_Base.level.customCharacterData[filename] : scrChar.baseCharacterAnimations[character]);
			component.onAnimationCompleted = OnCustomAnimEnd;
			component.enabled = true;
			customAnimation = component;
			CustomSprite component2 = gameObject.GetComponent<CustomSprite>();
			LevelEvent_Base.level.sprites.Add(base.decorationId, component2);
			gameObject.transform.LocalMoveXY(RDClass.Vfx.RDWidth / 2f, base.vfx.RDHeight / 2f);
			component2.gameObject.SetActive(visible);
			component2.renderer.sortingOrder = -base.depth;
		}

		public void OnCustomAnimEnd(CustomAnimation animation, CustomAnimationClip clip)
		{
		}

		private void PlayExpression(string expression, float speedMultiplier = 1f)
		{
			Dictionary<string, CustomAnimationClip> clips = customAnimation.data.clips;
			if (clips.ContainsKey(expression))
			{
				CustomAnimationClip clip = clips[expression];
				customAnimation.PlayFromClip(clip, speedMultiplier);
			}
		}

		public void UpdateCustomCharacter(bool justReturnErrors, bool editorRowHeader = false)
		{
			if ((editorRowHeader && !base.editorMode) || filename.IsNullOrEmpty())
			{
				return;
			}
			if (filename.HasImageFileExtension())
			{
				Texture2D texture2D = RDEditorUtils.LoadLevelTexture(scnGame.levelToLoadSource, filename);
				if (texture2D == null)
				{
					failedLoadingCustomCharacter = true;
					return;
				}
				failedLoadingCustomCharacter = false;
				LoadCustomCharacterWithTexture(texture2D, filename);
			}
			else if (!LevelEvent_MakeRow.UpdateCustomCharacter(Path.GetFileNameWithoutExtension(filename), justReturnErrors, editorRowHeader))
			{
				failedLoadingCustomCharacter = true;
			}
			void LoadCustomCharacterWithTexture(Texture2D texture, string name)
			{
				string text = Resources.Load<TextAsset>("DefaultSprite").text;
				if (texture == null || string.IsNullOrEmpty(text))
				{
					Debug.LogError("Couldn't load default sprite data? :(");
				}
				else
				{
					FilterMode filterMode = ((filter != TextureFilter.NearestNeighbor) ? FilterMode.Bilinear : FilterMode.Point);
					texture.filterMode = filterMode;
					texture.wrapMode = TextureWrapMode.Clamp;
					List<CustomAnimationData.JsonError> list = new List<CustomAnimationData.JsonError>();
					if (justReturnErrors)
					{
						CustomAnimationData customAnimationData = default;
						customAnimationData.LoadFromJson(text);
						list = customAnimationData.jsonErrors;
					}
					else
					{
						Dictionary<string, CustomAnimationData> dictionary = (editorRowHeader ? base.editor.customCharacterData : LevelEvent_Base.level.customCharacterData);
						list = LevelBase.LoadCustomCharacter(dictionary, name, text, texture, null, null, null);
						if (list.Count == 0)
						{
							CustomAnimationData customAnimationData2 = dictionary[name];
							customAnimationData2.spriteSize = new Vector2Int(texture.width, texture.height);
						}
					}
					if (list.Count > 0)
					{
						Debug.LogError("Failed loading image? :(");
					}
				}
			}
		}

		public override void Run()
		{
			if (!filename.IsNullOrEmpty() && !failedLoadingCustomCharacter)
			{
				FilterMode filterMode = ((filter != TextureFilter.NearestNeighbor) ? FilterMode.Bilinear : FilterMode.Point);
				if (LevelEvent_Base.level.sprites.TryGetValue(base.decorationId, out var value) && value.renderer?.material?.mainTexture != null)
				{
					value.renderer.material.mainTexture.filterMode = filterMode;
				}
			}
		}
	}
}
