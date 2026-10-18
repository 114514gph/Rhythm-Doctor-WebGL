using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

public class KeyHelper : RDBase
{
	private const float buttonSpacing = 2f;

	public static KeyHelperInfo currentInfo;

	private static bool setDefaultKeyHelperInfo = false;

	private static ControllerType lastControllerType = ControllerType.None;

	public RectTransform rect;

	public Text text;

	public RectTransform textRect;

	public string localizationKey;

	public KeyHelperButton[] buttons;

	[NonSerialized]
	public ControllerType currentControllerType;

	[NonSerialized]
	public bool enabledOnce;

	public bool shouldShow
	{
		get
		{
			KeyHelperButton[] array = buttons;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].foundTypeInfo)
				{
					return false;
				}
			}
			return true;
		}
	}

	public void OnEnable()
	{
		if (!setDefaultKeyHelperInfo)
		{
			ReInput.ControllerHelper controllers = ReInput.controllers;
			UpdateCurrentInfo(controllers.Controllers[0]);
			setDefaultKeyHelperInfo = true;
		}
		if (!string.IsNullOrEmpty(localizationKey))
		{
			string text = RDString.Get(localizationKey);
			this.text.text = text;
			this.text.font = RDString.GetAppropiateFontForString(this.text.text);
			RDStringToUIText.Apply(this.text);
			if (!enabledOnce)
			{
				UpdateWidth();
			}
		}
	}

	private void Start()
	{
		ControllerTypeWasUpdated();
	}

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			KeyHelperButton[] array = buttons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UpdateKeyboardLabels();
			}
		}
	}

	private void Update()
	{
		if (!enabledOnce)
		{
			UpdateWidth();
		}
	}

	private void UpdateWidth()
	{
		Canvas.ForceUpdateCanvases();
		text.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		TextGenerationSettings generationSettings = text.GetGenerationSettings(text.GetComponent<RectTransform>().rect.size);
		text.cachedTextGenerator.Populate(text.text, generationSettings);
		TextGenerator cachedTextGenerator = text.cachedTextGenerator;
		IList<UICharInfo> characters = cachedTextGenerator.characters;
		IList<UIVertex> verts = cachedTextGenerator.verts;
		if (characters.Count == 0 || verts.Count == 0)
		{
			RDBaseDllDummy.printesw($"{base.name} \"{text.text}\": characters.count {characters.Count}, verts.Count {verts.Count}");
			return;
		}
		float num = verts[0].position.x;
		float num2 = verts[0].position.x;
		foreach (UIVertex item in verts)
		{
			if (num > item.position.x)
			{
				num = item.position.x;
			}
			if (num2 < item.position.x)
			{
				num2 = item.position.x;
			}
		}
		float num3 = (text.font.dynamic ? text.canvas.scaleFactor : 1f);
		float num4 = (num2 - num) / num3;
		rect = GetComponent<RectTransform>();
		float num5 = 0f;
		KeyHelperButton[] array = buttons;
		foreach (KeyHelperButton keyHelperButton in array)
		{
			keyHelperButton.rect.anchoredPosition = new Vector2(num5, 0f);
			num5 += keyHelperButton.rect.sizeDelta.x + 2f;
		}
		textRect.anchoredPosition = new Vector2(num5, 1f);
		rect.sizeDelta = rect.sizeDelta.WithX(num5 + 2f + num4);
		enabledOnce = true;
	}

	public void ControllerTypeWasUpdated()
	{
		currentControllerType = currentInfo.type;
		KeyHelperButton[] array = buttons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateControllerType();
		}
		UpdateWidth();
	}

	public static void UpdateCurrentInfo(Controller controller)
	{
		RDConstants data = RDConstants.data;
		if (RDBase.debugSettings.RunningOnSteamDeck)
		{
			currentInfo = Array.Find(data.keyHelperInfos, (KeyHelperInfo k) => k.type == ControllerType.Steam);
		}
		else if (scnBase.isSwitch)
		{
			currentInfo = Array.Find(data.keyHelperInfos, (KeyHelperInfo k) => k.type == ControllerType.Nintendo);
		}
		else if (controller != null && controller.type != Rewired.ControllerType.Keyboard && controller.type != Rewired.ControllerType.Mouse)
		{
			string targetGuid = controller.hardwareTypeGuid.ToString();
			bool flag = false;
			KeyHelperInfo[] keyHelperInfos = data.keyHelperInfos;
			foreach (KeyHelperInfo keyHelperInfo in keyHelperInfos)
			{
				if (keyHelperInfo.type != ControllerType.Keyboard && keyHelperInfo.type != ControllerType.Xbox && keyHelperInfo.type != ControllerType.Steam && FindGuid(keyHelperInfo.guids, targetGuid))
				{
					flag = true;
					currentInfo = keyHelperInfo;
					break;
				}
			}
			if (!flag)
			{
				currentInfo = Array.Find(data.keyHelperInfos, (KeyHelperInfo k) => k.type == ControllerType.Xbox);
			}
		}
		else
		{
			currentInfo = Array.Find(data.keyHelperInfos, (KeyHelperInfo k) => k.type == ControllerType.Keyboard);
		}
		if (KeyInfoPanel.instance != null && lastControllerType != currentInfo.type)
		{
			lastControllerType = currentInfo.type;
			KeyInfoPanel.instance.UpdateShortcutsRow(null);
		}
	}

	public static bool FindGuid(string[] guids, string targetGuid)
	{
		for (int i = 0; i < guids.Length; i++)
		{
			if (guids[i] == targetGuid)
			{
				return true;
			}
		}
		return false;
	}
}
