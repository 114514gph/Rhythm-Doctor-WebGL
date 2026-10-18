using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scnSplash : MonoBehaviour
{
	public GameObject splash;

	public Image logo;

	private bool started;

	private float waitForInit = 0.5f;

	private void Awake()
	{
		started = true;
		splash.SetActive(value: false);
		waitForInit = 0f;
		logo.color = Color.white.WithAlpha(0f);
	}

	private void Start()
	{
		StartCoroutine(PlayLoading());
	}

	private void Update()
	{
	}

	private void GoToLogo()
	{
		scnBase.GoToScene("scnLogo");
	}

	private IEnumerator PlayLoading()
	{
		yield return new WaitForSeconds(waitForInit);
		splash.SetActive(value: false);
		yield return logo.DOFade(1f, 2f).WaitForCompletion();
		float startTime = Time.unscaledTime;
		float endTime = startTime + 15f;
		while (!started && Time.unscaledTime < endTime)
		{
			if (Time.unscaledTime - startTime < 1f)
			{
				yield return null;
			}
			else
			{
				yield return null;
			}
		}
		yield return logo.DOFade(0f, 2f).WaitForCompletion();
		GoToLogo();
	}
}
