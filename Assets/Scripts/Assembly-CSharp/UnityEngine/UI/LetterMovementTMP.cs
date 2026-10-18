using System.Collections.Generic;
using TMPro;

namespace UnityEngine.UI
{
	public class LetterMovementTMP : MonoBehaviour
	{
		public LetterMovementController movementController;

		private TMP_Text textComp;

		private TMP_TextInfo textInfo;

		private void Awake()
		{
			textComp = GetComponent<TMP_Text>();
			textInfo = textComp.textInfo;
			textComp.ForceMeshUpdate();
		}

		public void UpdateGraphic()
		{
			if (string.IsNullOrEmpty(textComp.text))
			{
				return;
			}
			textComp.ForceMeshUpdate();
			int characterCount = textComp.textInfo.characterCount;
			TMP_CharacterInfo[] characterInfo = textComp.textInfo.characterInfo;
			Dictionary<int, Vector3> charOffsets = movementController.charOffsets;
			for (int i = 0; i < characterCount; i++)
			{
				TMP_CharacterInfo tMP_CharacterInfo = characterInfo[i];
				if (!tMP_CharacterInfo.isVisible)
				{
					continue;
				}
				int materialReferenceIndex = tMP_CharacterInfo.materialReferenceIndex;
				int vertexIndex = tMP_CharacterInfo.vertexIndex;
				Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
				Vector3 vector = ((!charOffsets.ContainsKey(i)) ? Vector3.zero : charOffsets[i]);
				if (vertexIndex + 3 < vertices.Length)
				{
					for (int j = 0; j < 4; j++)
					{
						vertices[vertexIndex + j] += vector;
					}
				}
			}
			textComp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
		}
	}
}
