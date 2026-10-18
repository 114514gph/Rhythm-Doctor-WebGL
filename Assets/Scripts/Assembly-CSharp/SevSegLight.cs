using System.Collections.Generic;
using UnityEngine;

public class SevSegLight : MonoBehaviour
{
	public GameObject[] lights;

	private static Dictionary<char, int> charTo7Seg = new Dictionary<char, int>
	{
		{ '0', 119 },
		{ '1', 36 },
		{ '2', 93 },
		{ '3', 109 },
		{ '4', 46 },
		{ '5', 107 },
		{ '6', 123 },
		{ '7', 37 },
		{ '8', 127 },
		{ '9', 47 },
		{ 'A', 63 },
		{ 'a', 125 },
		{ 'B', 127 },
		{ 'b', 122 },
		{ 'C', 83 },
		{ 'c', 88 },
		{ 'D', 124 },
		{ 'd', 124 },
		{ 'E', 91 },
		{ 'e', 95 },
		{ 'F', 27 },
		{ 'f', 27 },
		{ 'G', 115 },
		{ 'g', 111 },
		{ 'h', 58 },
		{ 'H', 62 },
		{ 'I', 36 },
		{ 'i', 32 },
		{ 'J', 100 },
		{ 'j', 96 },
		{ 'K', 59 },
		{ 'k', 59 },
		{ 'L', 82 },
		{ 'l', 36 },
		{ 'N', 55 },
		{ 'n', 56 },
		{ 'O', 119 },
		{ 'o', 120 },
		{ 'P', 31 },
		{ 'p', 31 },
		{ 'Q', 47 },
		{ 'q', 47 },
		{ 'R', 19 },
		{ 'r', 24 },
		{ 'S', 107 },
		{ 's', 107 },
		{ 'T', 90 },
		{ 't', 90 },
		{ 'U', 118 },
		{ 'u', 112 },
		{ 'y', 110 },
		{ 'Z', 93 },
		{ 'z', 93 },
		{ '[', 83 },
		{ ']', 101 },
		{ '_', 64 },
		{ '-', 8 },
		{ '^', 1 },
		{ '=', 72 },
		{ '|', 18 },
		{ '"', 18 },
		{ '║', 54 },
		{ '°', 15 }
	};

	public void SetLight(int segmentFlags)
	{
		for (int i = 0; i < 7; i++)
		{
			lights[i].SetActive((segmentFlags & (1 << i)) != 0);
		}
	}

	public void ClearLight()
	{
		for (int i = 0; i < 7; i++)
		{
			lights[i].SetActive(value: false);
		}
	}

	public void SetLightWithChar(char value)
	{
		if (charTo7Seg.TryGetValue(value, out var value2))
		{
			SetLight(value2);
		}
		else
		{
			ClearLight();
		}
	}
}
