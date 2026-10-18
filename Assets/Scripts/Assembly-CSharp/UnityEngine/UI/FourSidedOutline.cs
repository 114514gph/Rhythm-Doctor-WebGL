using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/FourSidedOutline", 14)]
	public class FourSidedOutline : BaseMeshEffect
	{
		[SerializeField]
		private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);

		[SerializeField]
		private Vector2 m_EffectDistance = new Vector2(1f, -1f);

		[SerializeField]
		private bool m_UseGraphicAlpha = true;

		private const float kMaxEffectDistance = 600f;

		private const int copiesCount = 4;

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

		public Vector2 effectDistance
		{
			get
			{
				return m_EffectDistance;
			}
			set
			{
				if ((double)value.x > 600.0)
				{
					value.x = 600f;
				}
				if ((double)value.x < -600.0)
				{
					value.x = -600f;
				}
				if ((double)value.y > 600.0)
				{
					value.y = 600f;
				}
				if ((double)value.y < -600.0)
				{
					value.y = -600f;
				}
				if (!(m_EffectDistance == value))
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

		protected FourSidedOutline()
		{
		}

		protected void ApplyFourSidedOutlineZeroAlloc(List<UIVertex> verts, Color32 color, int newVertices, float x, float y)
		{
			int num = verts.Count + newVertices;
			if (verts.Capacity < num)
			{
				verts.Capacity = num;
			}
			Vector3[] array = new Vector3[4]
			{
				new Vector2(1f, 0f) * x,
				new Vector2(0f, 1f) * y,
				new Vector2(-1f, 0f) * x,
				new Vector2(0f, -1f) * y
			};
			int num2 = newVertices / 4;
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

		protected void ApplyFourSidedOutline(List<UIVertex> verts, Color32 color, int end, float x, float y)
		{
			ApplyFourSidedOutlineZeroAlloc(verts, color, end, x, y);
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				List<UIVertex> list = ListPool<UIVertex>.Get();
				vh.GetUIVertexStream(list);
				ApplyFourSidedOutline(list, effectColor, list.Count * 4, effectDistance.x, effectDistance.y);
				vh.Clear();
				vh.AddUIVertexTriangleStream(list);
				ListPool<UIVertex>.Release(list);
			}
		}
	}
}
