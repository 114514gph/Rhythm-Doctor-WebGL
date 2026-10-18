using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Letter Movement", 15)]
	public class LetterMovement : BaseMeshEffect
	{
		public LetterMovementController movementController;

		private Text textComp;

		protected LetterMovement()
		{
		}

		protected override void Awake()
		{
			base.Awake();
			textComp = GetComponent<Text>();
		}

		public void UpdateGraphic()
		{
			if (base.graphic != null)
			{
				base.graphic.SetVerticesDirty();
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = new List<UIVertex>();
				vh.GetUIVertexStream(list);
				ModifyVertices(list);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
			}
		}

		public void ModifyVertices(List<UIVertex> verts)
		{
			if (!IsActive())
			{
				return;
			}
			if (textComp == null)
			{
				Debug.LogWarning("LetterMovement: Missing Text component");
				return;
			}
			string text = textComp.text;
			_ = text.Length;
			int num = 0;
			Dictionary<int, Vector3> charOffsets = movementController.charOffsets;
			for (int i = 0; i < text.Length; i++)
			{
				int index = num * 6;
				int index2 = num * 6 + 1;
				int index3 = num * 6 + 2;
				int index4 = num * 6 + 3;
				int index5 = num * 6 + 4;
				int num2 = num * 6 + 5;
				if (num2 > verts.Count - 1)
				{
					return;
				}
				UIVertex value = verts[index];
				UIVertex value2 = verts[index2];
				UIVertex value3 = verts[index3];
				UIVertex value4 = verts[index4];
				UIVertex value5 = verts[index5];
				UIVertex value6 = verts[num2];
				Vector3 vector = ((!charOffsets.ContainsKey(i)) ? Vector3.zero : charOffsets[i]);
				value.position += vector;
				value2.position += vector;
				value3.position += vector;
				value4.position += vector;
				value5.position += vector;
				value6.position += vector;
				verts[index] = value;
				verts[index2] = value2;
				verts[index3] = value3;
				verts[index4] = value4;
				verts[index5] = value5;
				verts[num2] = value6;
				num++;
			}
			num++;
		}
	}
}
