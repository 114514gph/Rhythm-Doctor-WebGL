using UnityEngine;
using UnityEngine.UI;

public class Survey : MonoBehaviour
{
	public StarSlider rating;

	public StarSlider difficulty;

	public Text email;

	public InputField commentsInputField;

	public Text commentsPlaceholder;

	public Button sendButton;

	public Text sendButtonText;

	private int mTimesErrorShown;

	public void SendButtonAction()
	{
		if (commentsInputField.text != "")
		{
			string text = "version: " + GC.VersionText;
			string text2 = "rating: " + rating.NumberOfStars();
			string text3 = "difficulty: " + difficulty.NumberOfStars();
			string text4 = "comments: " + commentsInputField.text;
			string text5 = "email: " + email.text;
			string text6 = text + ", " + text2 + ", " + text3 + ", " + text4 + ", " + text5;
			sendButton.interactable = false;
			sendButtonText.text = "Thanks for your feedback!";
			MonoBehaviour.print("sent :\n" + text6);
		}
		else if (mTimesErrorShown == 0)
		{
			commentsPlaceholder.color = RDPresetColors.Orange.ARGBToColor().WithAlpha(1f);
			commentsPlaceholder.text += "<- here";
			mTimesErrorShown++;
		}
	}

	public void GoBackButtonAction()
	{
		GameObject.Find("Wrld").GetComponent<scrWrldMonthlyLevel>().TransitionFromQuitToIntroScene();
	}

	public void QuitButtonAction()
	{
		MonoBehaviour.print("quitting");
		Application.Quit();
	}
}
