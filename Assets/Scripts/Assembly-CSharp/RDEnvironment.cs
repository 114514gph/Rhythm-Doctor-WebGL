using DG.Tweening;
using UnityEngine;

public class RDEnvironment : RDBase
{
	public GameObject ambientSoundsContainer;

	public RDAmbienceAudioSource[] ambiences;

	private Tween stopTween;

	protected virtual void Start()
	{
		if (base.enabled)
		{
			ambientSoundsContainer.SetActive(value: false);
			ambiences = ambientSoundsContainer?.GetComponentsInChildren<RDAmbienceAudioSource>(includeInactive: true);
			if (ambiences == null || ambiences.Length == 0)
			{
				RDBaseDllDummy.printem(base.gameObject.name + " has no ambient sound sources, disabling ambience behaviours");
				base.enabled = false;
			}
			else
			{
				StartAmbiences();
			}
		}
	}

	public void StartAmbiences(float fadeDuration = 0.2f)
	{
		if (!base.enabled)
		{
			return;
		}
		stopTween.Kill();
		if (base.currentSceneName == "scnGame")
		{
			RDBaseDllDummy.printem($"currentLevel levelType is {base.game.currentLevel?.levelType}");
		}
		bool flag = base.currentSceneName == "scnLevelSelect";
		bool flag2;
		bool flag3;
		if (!flag)
		{
			flag2 = base.currentSceneName == "scnGame";
			if (flag2)
			{
				LevelType? levelType = base.game.currentLevel?.levelType;
				if (levelType.HasValue)
				{
					LevelType valueOrDefault = levelType.GetValueOrDefault();
					if ((uint)(valueOrDefault - 7) <= 1u)
					{
						flag3 = true;
						goto IL_00cc;
					}
				}
				flag3 = false;
				goto IL_00cc;
			}
			goto IL_00cf;
		}
		goto IL_00d1;
		IL_00cf:
		flag = flag2;
		goto IL_00d1;
		IL_00cc:
		flag2 = flag3;
		goto IL_00cf;
		IL_00d1:
		if (flag)
		{
			ambientSoundsContainer.SetActive(value: true);
			RDAmbienceAudioSource[] array = ambiences;
			foreach (RDAmbienceAudioSource obj in array)
			{
				obj.fadeInDuration = fadeDuration;
				obj.Restart();
			}
		}
	}

	public void StopAmbiences(float duration = 1f)
	{
		if (base.enabled)
		{
			if (base.currentSceneName == "scnGame")
			{
				RDBaseDllDummy.printem("stopping " + base.gameObject.name + " ambiences");
			}
			RDAmbienceAudioSource[] array = ambiences;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FadeOut(duration);
			}
			DOVirtual.DelayedCall(duration, () =>
			{
				ambientSoundsContainer.SetActive(value: false);
			});
		}
	}
}
