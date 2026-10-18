using System;
using System.Collections.Generic;
using TMPro;

namespace UnityEngine.UI
{
	public class LetterMovementController : MonoBehaviour
	{
		public bool ignoreTimescale;

		[Header("Shake")]
		public float letterShakeRadius;

		public float letterShakeInterval;

		private float letterShakeTime;

		private bool applyShakes;

		[Header("Wave")]
		public float letterWaveFrequency;

		public float letterWaveHeight;

		public float letterWavePhasePerChar;

		[Header("Swirl")]
		public float letterSwirlFrequency;

		public float letterSwirlRadius;

		public float letterSwirlPhasePerChar;

		private Dictionary<int, CharMovementInfo> charsToMove = new Dictionary<int, CharMovementInfo>();

		[NonSerialized]
		public Dictionary<int, Vector3> charOffsets = new Dictionary<int, Vector3>();

		private LetterMovement textMovementComp;

		private LetterMovementTMP tmpMovementComp;

		private bool isTMP;

		private bool hasMovingText;

		private void Awake()
		{
			TMP_Text component = GetComponent<TMP_Text>();
			isTMP = component != null;
			if (isTMP)
			{
				tmpMovementComp = GetComponent<LetterMovementTMP>();
			}
			else
			{
				textMovementComp = GetComponent<LetterMovement>();
			}
		}

		private void Update()
		{
			if (Time.timeScale != 0f || ignoreTimescale)
			{
				if (hasMovingText)
				{
					MoveAllChars();
				}
				UpdateGraphic();
			}
		}

		private void UpdateGraphic()
		{
			if (isTMP)
			{
				tmpMovementComp.UpdateGraphic();
			}
			else
			{
				textMovementComp.UpdateGraphic();
			}
		}

		public void AddCharIndex(int charIndex, LetterMovementType movementType, float[] multipliers)
		{
			if (movementType != LetterMovementType.None && !charsToMove.ContainsKey(charIndex))
			{
				charsToMove[charIndex] = new CharMovementInfo(charIndex, movementType, multipliers);
				hasMovingText = true;
				MoveChar(charIndex, forceShake: true);
			}
		}

		public void ClearAllChars()
		{
			foreach (int key in charsToMove.Keys)
			{
				charOffsets[key] = Vector3.zero;
			}
			charsToMove.Clear();
			hasMovingText = false;
			letterShakeTime = 0f;
			UpdateGraphic();
		}

		private void MoveChar(int charIndex, bool forceShake = false)
		{
			LetterMovementType movementType = charsToMove[charIndex].movementType;
			float[] multipliers = charsToMove[charIndex].multipliers;
			switch (movementType)
			{
			case LetterMovementType.Shake:
				if (applyShakes | forceShake)
				{
					ShakeChar(charIndex, multipliers[0]);
				}
				break;
			case LetterMovementType.Wave:
				WaveChar(charIndex, multipliers[0], multipliers[1]);
				break;
			case LetterMovementType.Swirl:
				SwirlChar(charIndex, multipliers[0], multipliers[1]);
				break;
			}
		}

		private void MoveAllChars()
		{
			letterShakeTime += Time.unscaledDeltaTime;
			if (letterShakeTime >= letterShakeInterval)
			{
				applyShakes = true;
				letterShakeTime = 0f;
			}
			foreach (int key in charsToMove.Keys)
			{
				MoveChar(key);
			}
			if (applyShakes)
			{
				applyShakes = false;
			}
		}

		private void ShakeChar(int charIndex, float radiusMultiplier)
		{
			float num = letterShakeRadius * radiusMultiplier;
			charOffsets[charIndex] = new Vector3(Random.Range(0f - num, num), Random.Range(0f - num, num), 0f);
		}

		private void WaveChar(int charIndex, float frequencyMultiplier, float heightMultiplier)
		{
			float num = Mathf.Sin((Time.unscaledTime + (float)charIndex * letterWavePhasePerChar) * letterWaveFrequency * frequencyMultiplier) * letterWaveHeight * heightMultiplier;
			charOffsets[charIndex] = Vector3.up * num;
		}

		private void SwirlChar(int charIndex, float frequencyMultiplier, float radiusMultiplier)
		{
			float y = Mathf.Sin((Time.unscaledTime + (float)charIndex * letterSwirlPhasePerChar) * letterSwirlFrequency * frequencyMultiplier) * letterSwirlRadius * radiusMultiplier;
			float x = (0f - Mathf.Cos((Time.unscaledTime + (float)charIndex * letterSwirlPhasePerChar) * letterSwirlFrequency * frequencyMultiplier)) * letterSwirlRadius * radiusMultiplier;
			charOffsets[charIndex] = new Vector3(x, y, 0f);
		}
	}
}
