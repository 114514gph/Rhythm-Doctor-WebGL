using System;
using DG.Tweening;
using UnityEngine;

public class RDHoodieBoyWard : RDBase
{
	public GameObject nightContainer;

	public GameObject sunriseContainer;

	public GameObject lightBackgroundsContainer;

	[NonSerialized]
	public GameObject[] lights;

	private const int lightsNumber = 16;

	private int incrementalLightCounter;

	private bool _showLights = true;

	public GameObject letterboxContainer;

	public SimpleSpectrum simpleSpectrum;

	[NonSerialized]
	public bool showSpectrum = true;

	[NonSerialized]
	public RDRoom room;

	public bool showLights
	{
		get
		{
			return _showLights;
		}
		set
		{
			_showLights = value;
			if (!_showLights)
			{
				ToggleAllLights(on: false);
			}
		}
	}

	private void Awake()
	{
		lights = new GameObject[16];
		for (int i = 0; i < 16; i++)
		{
			foreach (Transform item in lightBackgroundsContainer.transform)
			{
				if (item.name.Contains(i.ToString()))
				{
					lights[i] = item.gameObject;
					break;
				}
			}
		}
	}

	public void SetMode(bool nightMode)
	{
		nightContainer.SetActive(nightMode);
		sunriseContainer.SetActive(!nightMode);
	}

	private void LateUpdate()
	{
		if (!base.game.startTheGameCalled)
		{
			simpleSpectrum.enabled = false;
		}
		else if (room != null)
		{
			simpleSpectrum.enabled = showSpectrum && room.quadRenderer.enabled;
		}
	}

	public void ToggleAllLights(bool on, bool flash = false)
	{
		if (on && !showLights)
		{
			return;
		}
		GameObject[] array = lights;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(on);
			if (on)
			{
				if (flash)
				{
					gameObject.GetComponent<tk2dSprite>().DOFade(0f, 1f).Done();
				}
			}
			else
			{
				gameObject.GetComponent<tk2dSprite>().DOFade(1f, 0f).Done();
			}
		}
	}

	public void TurnOnLight(int index)
	{
		if (showLights && index < lights.Length)
		{
			ToggleAllLights(on: false);
			lights[index].SetActive(value: true);
		}
	}

	public void IncrementLight(bool turnOnLight)
	{
		if (turnOnLight)
		{
			TurnOnLight(incrementalLightCounter);
		}
		else
		{
			ToggleAllLights(on: false);
		}
		incrementalLightCounter++;
		incrementalLightCounter %= 16;
	}

	public void SetIncrementalLightCounter(int index, bool turnOnLight)
	{
		if (index < lights.Length)
		{
			incrementalLightCounter = index;
			IncrementLight(turnOnLight);
		}
	}
}
