using UnityEngine;
using UnityEngine.UI;

public class scrTextBehind : MonoBehaviour
{
	public Text textToFollow;

	private Text thisText;

	private void Start()
	{
		thisText = GetComponent<Text>();
	}

	private void Update()
	{
		thisText.text = textToFollow.text;
	}
}
