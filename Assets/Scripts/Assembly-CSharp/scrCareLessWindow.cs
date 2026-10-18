using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class scrCareLessWindow : MonoBehaviour
{
	public bool disableOnAwake;

	public GameObject careLessFilePrefab;

	public RectTransform contentTrans;

	public ScrollRect scrollRect;

	public TextAsset lyrics;

	public TextAsset chineseLyrics;

	public Sprite[] icons;

	private List<scrCareLessFile> files = new List<scrCareLessFile>();

	private const float XStart = 3f;

	private const float YStart = -4f;

	private const float YSeparation = -12f;

	private string[] songExtensions = new string[4] { ".wav", ".mp3", ".ogg", ".midi" };

	private string frtExt = ".frt";

	private string bnnExt = ".bnn";

	private System.Random RNG = new System.Random(2);

	private const string materialPath = "Levels/RDHoodieBoyWard/CareLess/UIDefaultGeometryPlusOne";

	private List<int> removedFiles = new List<int>();

	private void Awake()
	{
		TextAsset textAsset = ((RDString.language == SystemLanguage.ChineseSimplified || RDString.language == SystemLanguage.ChineseTraditional) ? chineseLyrics : lyrics);
		int startingFileindex = AddLines(textAsset.text, 0);
		startingFileindex = AddLines(textAsset.text, startingFileindex);
		if (!disableOnAwake)
		{
			contentTrans.sizeDelta = contentTrans.sizeDelta.WithY(Mathf.Abs(-4f + -12f * (float)startingFileindex));
		}
	}

	private int AddLines(string baseText, int startingFileindex)
	{
		StringReader stringReader = new StringReader(baseText);
		int num = startingFileindex;
		string text;
		while ((text = stringReader.ReadLine()) != null)
		{
			if (!text.IsNullOrEmpty())
			{
				if (text.Last() == ',')
				{
					text = text.Remove(text.Length - 1);
				}
				scrCareLessFile component = UnityEngine.Object.Instantiate(careLessFilePrefab, contentTrans).GetComponent<scrCareLessFile>();
				files.Add(component);
				int num2 = RNG.Next(0, icons.Length);
				component.iconImage.sprite = icons[num2];
				switch (num2)
				{
				case 0:
					component.fileText.text = text + bnnExt;
					break;
				case 1:
					component.fileText.text = text + frtExt;
					break;
				default:
					component.fileText.text = text + songExtensions[RNG.Next(0, songExtensions.Length)];
					break;
				}
				component.fileText.font = RDString.GetAppropiateFontForString(component.fileText.text);
				component.rectTransform.anchoredPosition = new Vector2(3f, -4f + -12f * (float)num);
				if (!disableOnAwake)
				{
					component.iconImage.material = Resources.Load<Material>("Levels/RDHoodieBoyWard/CareLess/UIDefaultGeometryPlusOne");
					component.fileText.material = Resources.Load<Material>("Levels/RDHoodieBoyWard/CareLess/UIDefaultGeometryPlusOne");
				}
				num++;
			}
		}
		return num;
	}

	public void ClearFiles()
	{
		foreach (scrCareLessFile file in files)
		{
			UnityEngine.Object.Destroy(file.gameObject);
		}
		files.Clear();
	}

	public void AddCustomFilesFromText(string textFileName)
	{
		string text = RDEditorUtils.LoadLevelText(scnGame.levelToLoadSource, textFileName);
		if (text != string.Empty)
		{
			AddLines(text, 0);
		}
	}

	public void SetScrollPos(float pos)
	{
		Canvas.ForceUpdateCanvases();
		scrollRect.normalizedPosition = new Vector2(0f, pos);
	}

	public Tween TweenScrollPos(float targetPos, float duration, Ease ease)
	{
		return DOTween.To(() => scrollRect.normalizedPosition.y, (float p) =>
		{
			SetScrollPos(p);
		}, targetPos, duration).SetEase(ease).OnComplete(() =>
		{
			SetScrollPos(targetPos);
		});
	}

	public void RemoveRandomFileFromLast(int lastFiles)
	{
		List<int> list = new List<int>();
		for (int num = files.Count - 1; num >= files.Count - lastFiles; num--)
		{
			if (!removedFiles.Contains(num))
			{
				list.Add(num);
			}
		}
		if (list.Count != 0)
		{
			int index = RNG.Next(0, list.Count);
			files[list[index]].gameObject.SetActive(value: false);
			removedFiles.Add(list[index]);
		}
	}
}
