using UnityEngine;
using UnityEngine.SceneManagement;

public class DisableOnAwake : MonoBehaviour
{
	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		base.gameObject.SetActive(value: false);
	}
}
