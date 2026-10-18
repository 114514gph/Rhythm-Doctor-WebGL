using System;
using UnityEngine;

[Serializable]
public class SettingCondition
{
	private const string EventNameToken = "[eventName]";

	private const string MinNumberToken = "[minNumber]";

	private const string MaxNumberToken = "[maxNumber]";

	private const string MaxSizeToken = "[maxSize]";

	private const string MaxLengthToken = "[maxLength]";

	private const string ExtensionsToken = "[extensions]";

	private const string DimensionsToken = "[dimensions]";

	private const string FileNameToken = "[fileName]";

	private const string FinishLevelEventToken = "editor.FinishLevel";

	private const string CLSErrorNameToken = "customLevelSelect.levelErrorName.";

	public LevelErrorName name;

	public LevelErrorType type;

	public bool recommended;

	public string GetLocalized(string value = null)
	{
		string text = RDString.Get("settingRequirement." + type);
		switch (type)
		{
		case LevelErrorType.EventNumberNotEnough:
		{
			string newValue = string.Empty;
			if (name == LevelErrorName.FinishLevelEventNumberNotEnough)
			{
				text = text.Replace("[minNumber]", 3.ToString());
				newValue = RDString.Get("editor.FinishLevel");
			}
			else
			{
				Debug.LogError("A new EventNumberNotEnough error has not been added here");
			}
			text = text.Replace("[eventName]", newValue);
			break;
		}
		case LevelErrorType.FileSize:
		{
			string newValue2 = string.Empty;
			if (name == LevelErrorName.PreviewImageFileSize)
			{
				newValue2 = 2 + " MB.";
			}
			else if (name == LevelErrorName.SyringeIconFileSize)
			{
				newValue2 = 20 + " KB.";
			}
			else
			{
				Debug.LogError("A new FileSize error needs to be added here");
			}
			text = text.Replace("[maxSize]", newValue2);
			break;
		}
		case LevelErrorType.Length:
		{
			float num = 0f;
			if (name == LevelErrorName.ArtistLength)
			{
				num = 256f;
			}
			else if (name == LevelErrorName.SongLength)
			{
				num = 256f;
			}
			else if (name == LevelErrorName.AuthorLength)
			{
				num = 256f;
			}
			else if (name == LevelErrorName.DescriptionLength)
			{
				num = 1000f;
			}
			else if (name == LevelErrorName.TagsLength)
			{
				num = 254f;
			}
			else
			{
				Debug.LogError("A new Length error needs to be added here");
			}
			text = text.Replace("[maxLength]", num.ToString());
			break;
		}
		case LevelErrorType.NumberOutRange:
		{
			float num2 = 0f;
			float num3 = 0f;
			if (name == LevelErrorName.PreviewSongDurationOutRange)
			{
				num2 = 1f;
				num3 = 20f;
			}
			else if (name == LevelErrorName.SongLabelHueOutRange)
			{
				num2 = 0f;
				num3 = 100f;
			}
			else
			{
				Debug.LogError("A new NumberOutRange error needs to be added here");
			}
			text = text.Replace("[minNumber]", num2.ToString());
			text = text.Replace("[maxNumber]", num3.ToString());
			break;
		}
		case LevelErrorType.WrongExtension:
		{
			string text2 = string.Empty;
			string[] array = ((name == LevelErrorName.PreviewSongWrongExtension) ? GC.SupportedAudioFiles : GC.SupportedImageFiles);
			string[] array2 = array;
			foreach (string text3 in array2)
			{
				text2 = text2 + "." + text3;
				if (text3 != array[^1])
				{
					text2 += " ";
				}
			}
			text = text.Replace("[extensions]", text2);
			break;
		}
		case LevelErrorType.Dimensions:
		{
			string empty = string.Empty;
			Vector2 vector = default;
			if (name == LevelErrorName.SyringeIconDimensions)
			{
				vector = LevelValidation.SyringeIconRecommendedDimensions;
			}
			else
			{
				Debug.LogError("A new Dimension error needs to be added here");
			}
			empty = vector.x + "x" + vector.y;
			text = text.Replace("[dimensions]", empty);
			break;
		}
		case LevelErrorType.FileNotFound:
			if (!string.IsNullOrEmpty(value))
			{
				text = text.Replace("[fileName]", value);
			}
			else
			{
				Debug.LogError("LevelErrorType.FileNotFound: dynamic value is null");
			}
			break;
		case LevelErrorType.FileCorruption:
			text = text.Replace("[fileName]", value);
			if (name == LevelErrorName.LevelDataNull || name == LevelErrorName.LevelSettingsNull || name == LevelErrorName.LevelRowsNull || name == LevelErrorName.LevelEventsNull)
			{
				text = text + " (" + RDString.Get("customLevelSelect.levelErrorName." + name) + ")";
			}
			else
			{
				Debug.LogError("A new FileCorruption error needs to be added here");
			}
			break;
		}
		return text;
	}
}
