using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WarningScene : RDBase
{
	public Background background;

	public tk2dSpriteCollectionData collData;

	public GameObject statusText;

	private float lastTimeAnyKeyDown = -5f;

	private const float VisibleDuration = 2f;

	private bool mFirstFrame = true;

	public tk2dSpriteAnimator image;

	public Text dialogueText;

	private void Start()
	{
		Time.timeScale = 1f;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			scnBase.GoToLevelSelect();
		}
		if (RDInput.anyKeyDown)
		{
			lastTimeAnyKeyDown = Time.timeSinceLevelLoad;
		}
		bool active = Time.timeSinceLevelLoad - lastTimeAnyKeyDown < 2f;
		statusText.SetActive(active);
		if (mFirstFrame)
		{
			StartCoroutine(PlaySequence());
			mFirstFrame = false;
		}
	}

	private IEnumerator PlaySequence()
	{
		yield return new WaitForSeconds(0.5f);
		base.conductor.SetBPM(88f);
		base.conductor.PlaySong("sndTutorial");
		RDBase.Vfx.FadeIn();
		yield return new WaitForSeconds(2f);
		dialogueText.gameObject.SetActive(value: true);
		base.ink.Run("diaWarning", "Slam");
		image.Play("Slam");
		yield return new WaitForSeconds(9f);
		base.ink.Run("diaWarning", "Tap");
		image.Play("Tap");
		yield return new WaitForSeconds(11f);
		RDBase.Vfx.FadeOut();
		yield return new WaitForSeconds(2f);
		scnBase.GoToLevelSelect();
		yield return null;
	}
}
