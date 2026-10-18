using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RDWrldSignup : scnBase
{
	private const int CanvasWidth = 352;

	private const int CanvasHeight = 198;

	public InputField email;

	public static string signupEmails => Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/signupEmails.json";

	private new void Start()
	{
		base.Start();
		DOTween.logBehaviour = LogBehaviour.Default;
		RDBase.Vfx.hudCameraOverlay.SetColor(Color.clear);
		base.conductor.SetBPM(82f);
		base.conductor.crotchetsPerBar = 4;
		base.conductor.OnPreBar();
		scrConductor.PlayImmediately("sndScribble");
	}

	private new void Update()
	{
		base.Update();
	}

	public override void OnPreBar()
	{
		base.OnPreBar();
		if ((base.conductor.barNumber - 1) % 17 == 0)
		{
			base.conductor.PlaySong("sndIntro2", LevelBase.musicGroup);
		}
	}

	public void CancelButton()
	{
		scnBase.GoToLevelSelect();
	}

	public void SubmitButton()
	{
		Save(email.text);
		scnBase.GoToLevelSelect();
	}

	public static void Save(string email)
	{
		if (!string.IsNullOrEmpty(email))
		{
			string text = "";
			if (RDFile.Exists(signupEmails))
			{
				text = RDFile.ReadAllText(signupEmails);
			}
			text = text + "\n" + email;
			RDFile.WriteAllText(signupEmails, text);
		}
	}
}
