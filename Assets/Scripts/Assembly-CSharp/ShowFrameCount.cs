using UnityEngine;
using UnityEngine.UI;

public class ShowFrameCount : MonoBehaviour
{
	private Text text;

	private void Awake()
	{
		text = GetComponent<Text>();
	}

	private void Update()
	{
		text.text = $"Frame: {Time.frameCount}";
	}
}
