using System;
using UnityEngine;

public class SA_EditorAd : SA_Singleton<SA_EditorAd>
{
	public const float MIN_LOAD_TIME = 1f;

	public const float MAX_LOAD_TIME = 3f;

	private bool _IsInterstitialLoading;

	private bool _IsVideoLoading;

	private bool _IsInterstitialReady;

	private bool _IsVideoReady;

	private int _FillRate = 100;

	private SA_Ad_EditorUIController _EditorUI;

	public bool IsVideoReady => _IsVideoReady;

	public bool IsVideoLoading => _IsVideoLoading;

	public bool IsInterstitialReady => _IsInterstitialReady;

	public bool IsInterstitialLoading => _IsInterstitialLoading;

	public bool HasFill
	{
		get
		{
			if (UnityEngine.Random.Range(1, 100) <= _FillRate)
			{
				return true;
			}
			return false;
		}
	}

	public int FillRate => _FillRate;

	private SA_Ad_EditorUIController EditorUI => _EditorUI;

	public static event Action<bool> OnInterstitialFinished = delegate
	{
	};

	public static event Action<bool> OnInterstitialLoadComplete = delegate
	{
	};

	public static event Action OnInterstitialLeftApplication = () =>
	{
	};

	public static event Action<bool> OnVideoFinished = delegate
	{
	};

	public static event Action<bool> OnVideoLoadComplete = delegate
	{
	};

	public static event Action OnVideoLeftApplication = () =>
	{
	};

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void SetFillRate(int fillRate)
	{
		_FillRate = fillRate;
	}

	public void LoadInterstitial()
	{
		if (!_IsInterstitialLoading && !IsInterstitialReady)
		{
			_IsInterstitialLoading = true;
			float time = UnityEngine.Random.Range(1f, 3f);
			Invoke("OnInterstitialRequestComplete", time);
		}
	}

	public void ShowInterstitial()
	{
		_ = IsInterstitialReady;
	}

	public void LoadVideo()
	{
		if (!_IsVideoLoading && !IsVideoReady)
		{
			_IsVideoLoading = true;
			float time = UnityEngine.Random.Range(1f, 3f);
			Invoke("OnVideoRequestComplete", time);
		}
	}

	public void ShowVideo()
	{
		_ = IsVideoReady;
	}

	private void OnVideoRequestComplete()
	{
		_IsVideoLoading = false;
		_IsVideoReady = HasFill;
		OnVideoLoadComplete(_IsVideoReady);
	}

	private void OnInterstitialRequestComplete()
	{
		_IsInterstitialLoading = false;
		_IsInterstitialReady = HasFill;
		OnInterstitialLoadComplete(_IsInterstitialReady);
	}

	private void OnInterstitialFinished_UIEvent(bool IsRewarded)
	{
		_IsInterstitialReady = false;
		OnInterstitialFinished(IsRewarded);
	}

	private void OnVideoFinished_UIEvent(bool IsRewarded)
	{
		_IsVideoReady = false;
		OnVideoFinished(IsRewarded);
	}

	private void OnInterstitialLeftApplication_UIEvent()
	{
		OnInterstitialLeftApplication();
	}

	private void OnVideoLeftApplication_UIEvent()
	{
		OnVideoLeftApplication();
	}
}
