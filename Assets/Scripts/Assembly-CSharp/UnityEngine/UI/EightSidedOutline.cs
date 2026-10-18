using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/EightSidedOutline", 14)]
	public class EightSidedOutline : BaseMeshEffect
	{
		[SerializeField]
		private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);

		[SerializeField]
		private float m_EffectDistance = 1f;

		[SerializeField]
		private bool m_UseGraphicAlpha = true;

		private const float kMaxEffectDistance = 600f;

		private const int copiesCount = 8;

		public Color effectColor
		{
			get
			{
				return m_EffectColor;
			}
			set
			{
				m_EffectColor = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		public float effectDistance
		{
			get
			{
				return m_EffectDistance;
			}
			set
			{
				if ((double)value > 600.0)
				{
					value = 600f;
				}
				if ((double)value < -600.0)
				{
					value = -600f;
				}
				if (m_EffectDistance != value)
				{
					m_EffectDistance = value;
					if (base.graphic != null)
					{
						base.graphic.SetVerticesDirty();
					}
				}
			}
		}

		public bool useGraphicAlpha
		{
			get
			{
				return m_UseGraphicAlpha;
			}
			set
			{
				m_UseGraphicAlpha = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}

		protected EightSidedOutline()
		{
		}

		protected void ApplyEightSidedOutlineZeroAlloc(List<UIVertex> verts, Color32 color, int newVertices, float distance)
		{
			int num = verts.Count + newVertices;
			if (verts.Capacity < num)
			{
				verts.Capacity = num;
			}
			Vector3[] array = new Vector3[8]
			{
				new Vector2(1f, 0f) * distance,
				new Vector2(0f, 1f) * distance,
				new Vector2(-1f, 0f) * distance,
				new Vector2(0f, -1f) * distance,
				new Vector2(1f, 1f) * distance,
				new Vector2(1f, -1f) * distance,
				new Vector2(-1f, 1f) * distance,
				new Vector2(-1f, -1f) * distance
			};
			int num2 = newVertices / 8;
			byte val = (byte)(m_UseGraphicAlpha ? 1u : 0u);
			for (int i = 0; i < newVertices; i++)
			{
				int num3 = i / num2;
				UIVertex uIVertex = verts[i];
				verts.Add(uIVertex);
				uIVertex.position += array[num3];
				Color32 color2 = color;
				byte b = Math.Max(uIVertex.color.a, val);
				color2.a = (byte)(color2.a * b / 255);
				uIVertex.color = color2;
				verts[i] = uIVertex;
			}
		}

		protected void ApplyEightSidedOutline(List<UIVertex> verts, Color32 color, int end, float distance)
		{
			ApplyEightSidedOutlineZeroAlloc(verts, color, end, distance);
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = ListPool<UIVertex>.Get();
				vh.GetUIVertexStream(list);
				ApplyEightSidedOutline(list, effectColor, list.Count * 8, effectDistance);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
				ListPool<UIVertex>.Release(list);
			}
		}
	}
}
