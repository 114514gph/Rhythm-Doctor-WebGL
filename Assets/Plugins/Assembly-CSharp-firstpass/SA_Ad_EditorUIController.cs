using System;
using UnityEngine;
using UnityEngine.UI;

public class SA_Ad_EditorUIController : MonoBehaviour
{
	public GameObject VideoPanel;

	public GameObject InterstitialPanel;

	public Image[] AppIcons;

	public Text[] AppNames;

	public event Action<bool> OnCloseVideo = delegate
	{
	};

	public event Action OnVideoLeftApplication = () =>
	{
	};

	public event Action<bool> OnCloseInterstitial = delegate
	{
	};

	public event Action OnInterstitialLeftApplication = () =>
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		SA_EditorTesting.CheckForEventSystem();
		GetComponent<Canvas>().sortingOrder = 10001;
	}

	private void Start()
	{
	}

	public void InterstitialClick()
	{
		OnInterstitialLeftApplication();
	}

	public void VideoClick()
	{
		OnVideoLeftApplication();
	}

	public void ShowInterstitialAd()
	{
		base.gameObject.SetActive(value: true);
		InterstitialPanel.SetActive(value: true);
	}

	public void ShowVideoAd()
	{
		base.gameObject.SetActive(value: true);
		VideoPanel.SetActive(value: true);
	}

	public void CloseInterstitial()
	{
		base.gameObject.SetActive(value: false);
		InterstitialPanel.SetActive(value: false);
		OnCloseInterstitial(obj: true);
	}

	public void CloseVideo()
	{
		base.gameObject.SetActive(value: false);
		VideoPanel.SetActive(value: false);
		OnCloseVideo(obj: true);
	}
}
