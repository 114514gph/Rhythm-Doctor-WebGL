using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RDLevelEditor;
using UnityEngine;

public class CharacterTemplateGenerator
{
	public Character character;

	public string name;

	public string prevName;

	public int targetRow = -1;

	public Vector2Int resolution = new Vector2Int(40, 40);

	public int frameCount = 16;

	public bool running;

	private bool shouldSave;

	private Dictionary<string, byte[]> filesToWrite;

	private const string CharactersResourcesFolder = "Characters";

	private const string CharactersSpeechResourcesFolder = "Sfx/SpeechTickers";

	public CharacterTemplateGenerator()
	{
		name = RDString.Get("editor.CharacterTemplateGenerator.defaultName");
		prevName = name;
	}

	public void ValidateValues()
	{
		name = (name.IsNullOrEmpty() ? prevName : name);
		resolution = new Vector2Int(Mathf.Max(1, resolution.x), Mathf.Max(1, resolution.y));
		frameCount = Math.Max(1, frameCount);
		prevName = name;
	}

	public IEnumerator Generate(string destinationPath, Action<bool> callback = null)
	{
		running = true;
		filesToWrite = new Dictionary<string, byte[]>();
		if (character != Character.Custom)
		{
			yield return GetInternalCharacterResources();
		}
		else
		{
			yield return MakeSpritesheet();
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, byte[]> item in filesToWrite)
		{
			if (File.Exists(Path.Combine(destinationPath, item.Key)))
			{
				list.Add(item.Key);
			}
		}
		bool showingDialogBox = false;
		if (list.Count > 0)
		{
			showingDialogBox = true;
			scnEditor.instance?.dialog.Show(RDString.Get("editor.dialog.overwriteFiles").Replace("[files]", string.Join(", ", list)), new string[2]
			{
				RDString.Get("editor.dialog.overwrite"),
				RDString.Get("editor.dialog.cancel")
			}, (int option) =>
			{
				scnEditor.instance.dialog.Hide(() =>
				{
					showingDialogBox = false;
					if (option != 0)
					{
						shouldSave = false;
					}
				});
			});
		}
		while (showingDialogBox)
		{
			yield return null;
		}
		if (shouldSave)
		{
			yield return SaveToDisk(destinationPath);
		}
		callback?.Invoke(shouldSave);
		running = false;
	}

	public IEnumerator MakeSpritesheet()
	{
		Vector2Int gridSize = GetDimensions(frameCount);
		Vector2Int vector2Int = new Vector2Int(resolution.x * gridSize.x, resolution.y * gridSize.y);
		if (resolution.x * gridSize.x >= SystemInfo.maxTextureSize || resolution.y * gridSize.y >= SystemInfo.maxTextureSize)
		{
			string newValue = $"{vector2Int.x}x{vector2Int.y}";
			string newValue2 = $"{SystemInfo.maxTextureSize}x{SystemInfo.maxTextureSize}";
			string message = RDString.Get("editor.CharacterTemplateGenerator.tooLarge").Replace("[size]", newValue).Replace("[maxSize]", newValue2);
			scnEditor.instance?.dialog.Show(message, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
			{
				scnEditor.instance.dialog.Hide();
			});
			yield break;
		}
		Texture2D spritesheet = new Texture2D(vector2Int.x, vector2Int.y, TextureFormat.RGBA32, mipChain: false)
		{
			filterMode = FilterMode.Point
		};
		Color32 evenColor = 1034878975u.RGBAToColor();
		Color32 oddColor = 6729727u.RGBAToColor();
		_ = (Color32)uint.MaxValue.RGBAToColor();
		Vector2Int digitResolution = new Vector2Int(3, 3);
		Color32[][] digitData = new Color32[10][];
		ResourceRequest numbersTextureReq = Resources.LoadAsync<Texture2D>(Path.Combine("Characters", "SpritesheetNumbers"));
		yield return numbersTextureReq;
		Texture2D texture2D = numbersTextureReq.asset as Texture2D;
		Color32[] pixels = texture2D.GetPixels32();
		for (uint num = 0u; num < 10; num++)
		{
			Color32[] array = new Color32[digitResolution.x * digitResolution.y];
			for (uint num2 = 0u; num2 < digitResolution.x; num2++)
			{
				for (uint num3 = 0u; num3 < digitResolution.y; num3++)
				{
					array[digitResolution.x * num3 + num2] = pixels[texture2D.width * (texture2D.height - 1 - num3) + num * digitResolution.x + num2];
				}
			}
			digitData[num] = array;
		}
		Color32[] array2 = new Color32[spritesheet.width * spritesheet.height];
		for (uint num4 = 0u; num4 < gridSize.x; num4++)
		{
			for (uint num5 = 0u; num5 < gridSize.y; num5++)
			{
				for (uint num6 = 0u; num6 < resolution.x; num6++)
				{
					for (uint num7 = 0u; num7 < resolution.y; num7++)
					{
						long num8 = num4 * resolution.x + num6;
						long num9 = num5 * resolution.y + num7;
						Color32 color = (((num4 + num5) % 2 == 0) ? evenColor : oddColor);
						array2[spritesheet.width * (spritesheet.height - 1 - num9) + num8] = color;
					}
				}
				if (resolution.x < 5 || resolution.y < 5)
				{
					continue;
				}
				int[] array3 = GetDigits((int)(gridSize.x * num5 + num4)).Reverse().ToArray();
				for (int num10 = 0; num10 < array3.Length; num10++)
				{
					int num11 = array3[num10];
					long num12 = (num4 + 1) * resolution.x - 1 - num10 - (num10 + 1) * digitResolution.x;
					long num13 = (num5 + 1) * resolution.y - 1 - digitResolution.y;
					for (int num14 = 0; num14 < digitResolution.x; num14++)
					{
						for (int num15 = 0; num15 < digitResolution.y; num15++)
						{
							int num16 = digitResolution.x * num15 + num14;
							Color32 b = digitData[num11][num16];
							long num17 = num12 + num14;
							long num18 = num13 + num15;
							if (num17 > num4 * resolution.x)
							{
								long num19 = spritesheet.width * (spritesheet.height - 1 - num18) + num17;
								array2[num19] = Color32.Lerp(array2[num19], b, (int)b.a);
							}
						}
					}
				}
			}
		}
		spritesheet.SetPixels32(array2);
		filesToWrite.Add(name + ".png", spritesheet.EncodeToPNG());
		ResourceRequest jsonReq = Resources.LoadAsync<TextAsset>(Path.Combine("Characters", "Template"));
		yield return jsonReq;
		string text = ((TextAsset)jsonReq.asset).text.Replace("[sizeX]", resolution.x.ToString()).Replace("[sizeY]", resolution.y.ToString());
		filesToWrite.Add(name + ".json", new TextAsset(text).bytes);
		shouldSave = true;
	}

	public IEnumerator GetInternalCharacterResources()
	{
		string internalCharacter = character.ToString();
		string speechSound = GC.defaultCharacterSpeechSounds.GetValueOrDefault(character);
		yield return CopyInternalJSON("Characters", internalCharacter, name);
		yield return CopyInternalPNG("Characters", internalCharacter, name);
		yield return CopyInternalPNG("Characters", internalCharacter, name, "_glow");
		yield return CopyInternalPNG("Characters", internalCharacter, name, "_freeze");
		yield return CopyInternalPNG("Characters", internalCharacter, name, "_outline");
		if (!speechSound.IsNullOrEmpty())
		{
			yield return CopyInternalOGG("Sfx/SpeechTickers", "sndSpeech" + speechSound, name);
		}
		shouldSave = true;
		IEnumerator CopyInternalJSON(string path, string internalName, string finalName, string suffix = "")
		{
			string path2 = internalName + suffix;
			ResourceRequest req = Resources.LoadAsync<TextAsset>(Path.Combine(path, path2));
			yield return req;
			if (!(req.asset == null))
			{
				filesToWrite.Add(finalName + suffix + ".json", ((TextAsset)req.asset).bytes);
			}
		}
		IEnumerator CopyInternalOGG(string path, string internalName, string finalName, string suffix = "")
		{
			string path2 = internalName + suffix;
			ResourceRequest req = Resources.LoadAsync<AudioClip>(Path.Combine(path, path2));
			yield return req;
			if (!(req.asset == null))
			{
				AudioClip audioClip = (AudioClip)req.asset;
				MemoryStream memoryStream = null;
				yield return scnEditor.instance.GetComponent<AudioclipToOggEncoder>().EncodeToOgg(audioClip, 0f, audioClip.length, null, (float percentage, MemoryStream newMemoryStream) =>
				{
					memoryStream = newMemoryStream;
				});
				filesToWrite.Add(finalName + suffix + ".ogg", memoryStream.ToArray());
			}
		}
		IEnumerator CopyInternalPNG(string path, string internalName, string finalName, string suffix = "")
		{
			string path2 = internalName + suffix;
			ResourceRequest req = Resources.LoadAsync<Texture2D>(Path.Combine(path, path2));
			yield return req;
			if (!(req.asset == null))
			{
				filesToWrite.Add(finalName + suffix + ".png", ((Texture2D)req.asset).EncodeToPNG());
			}
		}
	}

	public IEnumerator CreateCustomTexture()
	{
		yield return new NotImplementedException();
	}

	public IEnumerator SaveToDisk(string destinationPath)
	{
		foreach (KeyValuePair<string, byte[]> item in filesToWrite)
		{
			yield return File.WriteAllBytesAsync(Path.Combine(destinationPath, item.Key), item.Value);
		}
		filesToWrite.Clear();
	}

	private static Vector2Int GetDimensions(int n)
	{
		float tempSqrt = Mathf.Sqrt(n);
		List<int> list = new List<int>();
		for (int i = 1; i <= n; i++)
		{
			if (n % i == 0)
			{
				list.Add(i);
			}
		}
		int num = list.IndexOf(list.OrderBy((int d) => Math.Abs((float)d - tempSqrt)).First());
		if (list.Count == 2)
		{
			return new Vector2Int(Mathf.CeilToInt(tempSqrt), Mathf.RoundToInt(tempSqrt));
		}
		if (list[num] * list[num] == n)
		{
			return new Vector2Int(list[num], list[num]);
		}
		int index = num + 1;
		return new Vector2Int(list[index], list[num]);
	}

	public static IEnumerable<int> GetDigits(int source)
	{
		int individualFactor = 0;
		int tennerFactor = Convert.ToInt32(Math.Pow(10.0, source.ToString().Length));
		do
		{
			source -= tennerFactor * individualFactor;
			tennerFactor /= 10;
			individualFactor = source / tennerFactor;
			yield return individualFactor;
		}
		while (tennerFactor > 1);
	}
}
