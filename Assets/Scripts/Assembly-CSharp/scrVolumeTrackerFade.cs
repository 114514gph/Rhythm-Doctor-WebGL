using UnityEngine;

public class scrVolumeTrackerFade : MonoBehaviour
{
	public int freq = 20;

	public AudioSource conductoraudio;

	private float[] spectrum = new float[1024];

	private float maxmultiplier = 1.2f;

	private float minmultiplier = 1f;

	private float currentFade;

	private void Start()
	{
		conductoraudio = scrConductor.instance.gameObject.GetComponent<AudioSource>();
	}

	private void Update()
	{
		AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		float num = Mathf.Log(spectrum[freq]) / 10f * (maxmultiplier - minmultiplier) + minmultiplier;
		currentFade = num;
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(150f, 0f, 100f, 50f), "fade = " + currentFade);
	}
}
