using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RDBossStageText : RDBase
{
	public TextMesh upperText;

	public TextMesh lowerText;

	public bool glitchMode;

	public bool noFlash;

	private int glitchPhase;

	private const float distance = 200f;

	private float centerPositionBossText;

	private float centerPositionStageText;

	public Image indicator;

	private void Start()
	{
		centerPositionBossText = upperText.transform.localPosition.x;
		centerPositionStageText = lowerText.transform.localPosition.x;
		upperText.transform.Translate(Vector3.left * 200f);
		lowerText.transform.Translate(Vector3.right * 200f);
	}

	public IEnumerator PlayAnimation(float enterSpeed = 1f, float waitSpeed = 1f, float exitSpeed = 1f)
	{
		float num = 1.3f / enterSpeed;
		float waitingDuration = 1f / waitSpeed;
		float exitDuration = 1.3f / exitSpeed;
		upperText.transform.DOLocalMoveX(centerPositionBossText, num);
		lowerText.transform.DOLocalMoveX(centerPositionStageText, num);
		yield return new WaitForSeconds(num);
		if (!noFlash && !base.game.reducedFlash)
		{
			scrVfxControl.instance.Flash();
		}
		yield return new WaitForSeconds(waitingDuration);
		upperText.transform.DOLocalMoveX(800f, exitDuration);
		lowerText.transform.DOLocalMoveX(-800f, exitDuration);
		yield return new WaitForSeconds(exitDuration);
		base.gameObject.SetActive(value: false);
		yield return null;
	}

	public IEnumerator PlayGlitchAnimationPart1()
	{
		float translationDuration = 2f * base.conductor.unscaledCrotchet;
		float translationDurationBeforePause = 5f * base.conductor.unscaledCrotchet;
		yield return new WaitForSeconds(3f * base.conductor.unscaledCrotchet);
		Narration.Say(RDString.Get("status.bossStage"), NarrationCategory.Notification, false);
		upperText.transform.DOLocalMoveX(Mathf.Lerp(centerPositionBossText, upperText.transform.localPosition.x, 0f), translationDuration).SetEase(Ease.InQuad);
		lowerText.transform.DOLocalMoveX(Mathf.Lerp(centerPositionStageText, lowerText.transform.localPosition.x, 0f), translationDuration).SetEase(Ease.InQuad);
		float BossEndX = Mathf.Lerp(centerPositionBossText, upperText.transform.localPosition.x, 0.6f);
		float StageEndX = Mathf.Lerp(centerPositionBossText, lowerText.transform.localPosition.x, 0.6f);
		yield return new WaitForSeconds(translationDurationBeforePause - 0.5f);
		upperText.transform.DOLocalMoveX(BossEndX, 0f);
		lowerText.transform.DOLocalMoveX(StageEndX, 0f);
		yield return new WaitForSeconds(0.6f);
		scrConductor.instance.Pause(isPause: true);
		scnGame.pauseBlocked = true;
		glitchPhase = 1;
		yield return null;
	}

	public IEnumerator PlayGlitchAnimationPart2()
	{
		RDBaseDllDummy.printem("boo!");
		glitchPhase = 2;
		scrConductor.PlayImmediately("sndInsomniacError", 1f, RDUtils.GetMixerGroup("LevelCutscene"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		_ = scrVfxControl.instance;
		Image frontOfAllOverlay = scnGame.instance.frontOfAllOverlay;
		frontOfAllOverlay.gameObject.SetActive(value: true);
		frontOfAllOverlay.color = new Color(1f, 1f, 1f, 0f);
		frontOfAllOverlay.DOFade(0.5f, 1.5f);
		yield return new WaitForSeconds(0.5f);
		Narration.Say(RDString.Get("status.loading"), NarrationCategory.Notification, false);
		yield return new WaitForSeconds(1f);
		base.game.activityIndicator.gameObject.SetActive(value: true);
		if (!scnBase.isBooth)
		{
			yield return new WaitForSeconds(3f);
		}
		else
		{
			yield return new WaitForSeconds(1f);
		}
		base.game.activityIndicatorPivot.GetComponent<Animation>().Play();
		Narration.Say(RDString.Get("InsomniacHard.desc1"), NarrationCategory.Description, false);
		yield return new WaitForSeconds(0.5f);
		base.conductor.Pause(isPause: false);
		yield return null;
	}

	public void Play(float enterSpeed = 1f, float waitSpeed = 1f, float exitSpeed = 1f)
	{
		if (glitchMode)
		{
			StartCoroutine(PlayGlitchAnimationPart1());
			return;
		}
		Narration.Say(RDString.Get("status.bossStage"), NarrationCategory.Notification, false);
		StartCoroutine(PlayAnimation(enterSpeed, waitSpeed, exitSpeed));
	}

	public void Update()
	{
		if (glitchMode && RDInput.anyKeyDown && glitchPhase == 1 && !scnBase.isBooth)
		{
			StartCoroutine(PlayGlitchAnimationPart2());
		}
		if (glitchMode && glitchPhase == 1 && (scnBase.isBooth || DebugSettings.instance.RecordingMode))
		{
			StartCoroutine(PlayGlitchAnimationPart2());
		}
	}

	public void SetColor(Color col)
	{
		upperText.color = col;
		lowerText.color = col;
	}
}
