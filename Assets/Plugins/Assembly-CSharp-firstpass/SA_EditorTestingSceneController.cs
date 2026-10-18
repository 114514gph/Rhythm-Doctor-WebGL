using UnityEngine;
using UnityEngine.UI;

public class SA_EditorTestingSceneController : MonoBehaviour
{
	public Button ShowInterstitial_Button;

	public Button ShowInterstitial_Video;

	public void LoadInterstitial()
	{
		SA_Singleton<SA_EditorAd>.Instance.LoadInterstitial();
	}

	public void ShowInterstitial()
	{
		SA_Singleton<SA_EditorAd>.Instance.ShowInterstitial();
	}

	public void LoadVideo()
	{
		SA_Singleton<SA_EditorAd>.Instance.LoadVideo();
	}

	public void ShowVideo()
	{
		SA_Singleton<SA_EditorAd>.Instance.ShowVideo();
	}

	private void FixedUpdate()
	{
		if (SA_Singleton<SA_EditorAd>.Instance.IsInterstitialReady)
		{
			ShowInterstitial_Button.interactable = true;
		}
		else
		{
			ShowInterstitial_Button.interactable = false;
		}
		if (SA_Singleton<SA_EditorAd>.Instance.IsVideoReady)
		{
			ShowInterstitial_Video.interactable = true;
		}
		else
		{
			ShowInterstitial_Video.interactable = false;
		}
	}
}
