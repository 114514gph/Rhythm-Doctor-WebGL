using UnityEngine;

public class WaveTestManager : MonoBehaviour
{
	public int WaveCount;

	private bool shadows;

	private int borderType;

	private int waveIndex;

	private bool heart;

	public GameObject wavePrefab;

	public GameObject[] waveGOs;

	private void Awake()
	{
		waveGOs = new GameObject[WaveCount];
		for (int i = 0; i < WaveCount; i++)
		{
			waveGOs[i] = Object.Instantiate(wavePrefab);
			waveGOs[i].transform.position = new Vector3(0f, -50f + (float)i * 0.1f, 0f);
		}
		RDWaveRenderer_Pulse.SetupLocalKeywords(waveGOs[0].GetComponent<MeshRenderer>().material);
		GameObject[] array = waveGOs;
		for (int j = 0; j < array.Length; j++)
		{
			Material material = array[j].GetComponent<MeshRenderer>().material;
			material.SetKeyword(in RDWaveRenderer_Pulse.waves4Keyword, value: true);
			material.SetKeyword(in RDWaveRenderer_Pulse.glowKeyword, value: true);
		}
	}

	private void OnGUI()
	{
		bool flag = false;
		if (GUI.Button(new Rect(0f, 0f, 200f, 100f), "Shadows " + (shadows ? "ON" : "OFF")))
		{
			shadows = !shadows;
			flag = true;
		}
		if (GUI.Button(new Rect(200f, 0f, 200f, 100f), "Heart " + (heart ? "ON" : "OFF")))
		{
			heart = !heart;
			flag = true;
		}
		if (GUI.Button(new Rect(400f, 0f, 200f, 100f), "Border " + ((borderType == 0) ? "NONE" : ((borderType == 1) ? "OUTLINE" : "GLOW"))))
		{
			borderType = (borderType + 1) % 3;
			flag = true;
		}
		if (GUI.Button(new Rect(600f, 0f, 200f, 100f), "Waves " + (waveIndex + 1)))
		{
			waveIndex = (waveIndex + 1) % 4;
			flag = true;
		}
		if (flag)
		{
			GameObject[] array = waveGOs;
			for (int i = 0; i < array.Length; i++)
			{
				Material material = array[i].GetComponent<MeshRenderer>().material;
				material.SetKeyword(in RDWaveRenderer_Pulse.shadowKeyword, shadows);
				material.SetKeyword(in RDWaveRenderer_Pulse.heartKeyword, heart);
				material.SetKeyword(in RDWaveRenderer_Pulse.outlineKeyword, borderType == 1);
				material.SetKeyword(in RDWaveRenderer_Pulse.glowKeyword, borderType == 2);
				int num = waveIndex + 1;
				material.SetKeyword(in RDWaveRenderer_Pulse.waves2Keyword, num == 2);
				material.SetKeyword(in RDWaveRenderer_Pulse.waves3Keyword, num == 3);
				material.SetKeyword(in RDWaveRenderer_Pulse.waves4Keyword, num == 4);
			}
		}
	}
}
