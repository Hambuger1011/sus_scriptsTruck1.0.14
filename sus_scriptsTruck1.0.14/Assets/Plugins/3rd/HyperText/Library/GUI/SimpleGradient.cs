
using UnityEngine;
using System.Collections.Generic;

namespace Candlelight.UI
{
	/// <summary>
	/// A simple vertical gradient effect that tints the vertex colors of UI objects.
	/// </summary>
	[AddComponentMenu("UI/Candlelight/Effects/Simple Gradient")]
	public class SimpleGradient : UnityEngine.UI.BaseMeshEffect
	{
		/// <summary>
		/// Gradient fill mode.
		/// </summary>
		public enum GradientFillMode
		{
			/// <summary>
			/// Apply gradient extents to each quad on the graphic.
			/// </summary>
			PerQuad,
			/// <summary>
			/// Use the <see cref="UnityEngine.RectTransform"/> to determine the gradient extents.
			/// </summary>
			RectTransform,
			/// <summary>
			/// Use the geometry bounds to determine the gradient extents.
			/// </summary>
			GeometryBounds
		}

		#region Backing Fields
		[SerializeField]
		private Color32 m_TopColor = Color.white;
		[SerializeField]
		private Color32 m_BottomColor = Color.black;
		[SerializeField]
		private ColorTintMode m_ColorTintMode = ColorTintMode.Constant;
		[SerializeField]
		private GradientFillMode m_FillMode = GradientFillMode.PerQuad;
		#endregion

		/// <summary>
		/// Gets or sets the bottom color of the gradient.
		/// </summary>
		/// <value>The bottom colors of the gradient.</value>
		public Color32 BottomColor
		{
			get { return m_BottomColor; }
			set
			{
				if (!m_BottomColor.Equals(value))
				{
					m_BottomColor = value;
					this.graphic.SetVerticesDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets the color tint mode.
		/// </summary>
		/// <value>The color tint mode.</value>
		public ColorTintMode ColorTintMode
		{
			get { return m_ColorTintMode; }
			set
			{
				if (m_ColorTintMode != value)
				{
					m_ColorTintMode = value;
					this.graphic.SetVerticesDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets the fill mode.
		/// </summary>
		/// <value>The fill mode.</value>
		public GradientFillMode FillMode
		{
			get { return m_FillMode; }
			set
			{
				if (m_FillMode != value)
				{
					m_FillMode = value;
					this.graphic.SetVerticesDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets the top color of the gradient.
		/// </summary>
		/// <value>The top color of the gradient.</value>
		public Color32 TopColor
		{
			get { return m_TopColor; }
			set
			{
				if (!m_TopColor.Equals(value))
				{
					m_TopColor = value;
					this.graphic.SetVerticesDirty();
				}
			}
		}

		/// <summary>
		/// Applies the tint value to the specified vertex.
		/// </summary>
		/// <param name="vertices">Vertices.</param>
		/// <param name="index">Index.</param>
		/// <param name="tint">Tint.</param>
		private void ApplyTint(List<UIVertex> vertices, int index, Color tint)
		{
			UIVertex v = vertices[index];
			switch (m_ColorTintMode)
			{
			case ColorTintMode.Additive:
				v.color = v.color + tint;
				break;
			case ColorTintMode.Constant:
				v.color = tint;
				break;
			case ColorTintMode.Multiplicative:
				v.color = v.color * tint;
				break;
			}
			vertices[index] = v;
		}
        

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		/// <param name="mesh">Vertex buffer object.</param>
		public override void ModifyMesh(Mesh mesh) {
            base.ModifyMesh(mesh);
        }

		/// <summary>
		/// Modifies the mesh.
		/// </summary>
		/// <param name="vh">Vertex buffer object.</param>
		public override void ModifyMesh(UnityEngine.UI.VertexHelper vh)
		{
			if (!IsActive())
			{
				return;
			}
			using (ListPool<UIVertex>.Scope uiVertices = new ListPool<UIVertex>.Scope())
            {
				vh.GetUIVertexStream(uiVertices.List);
				int count = uiVertices.List.Count;

				if (count == 0)
				{
					return;
				}
				float yMin = this.graphic.rectTransform.rect.yMin;
				float yMax = this.graphic.rectTransform.rect.yMax;
				switch (m_FillMode)
				{
				case GradientFillMode.PerQuad:
					for (int i = 0; i < count; i += 6)
					{
						ApplyTint(uiVertices.List, i, m_TopColor);
						ApplyTint(uiVertices.List, i + 1, m_TopColor);
						ApplyTint(uiVertices.List, i + 2, m_BottomColor);
						ApplyTint(uiVertices.List, i + 3, m_BottomColor);
						ApplyTint(uiVertices.List, i + 4, m_BottomColor);
						ApplyTint(uiVertices.List, i + 5, m_TopColor);
					}
					vh.Clear();
					vh.AddUIVertexTriangleStream(uiVertices.List);
					return;
				case GradientFillMode.GeometryBounds:
					yMin = uiVertices.List[0].position.y;
					yMax = uiVertices.List[0].position.y;
					for (int i = 1; i < count; i++)
					{
						float y = uiVertices.List[i].position.y;
						if (y > yMax)
						{
							yMax = y;
						}
						if (y < yMin)
						{
							yMin = y;
						}
					}
					break;
				}
				float div = 1f / (yMax - yMin);
				for (int i = 0; i < count; ++i)
				{
					ApplyTint(
						uiVertices.List,
						i,
						Color32.Lerp(m_TopColor, m_BottomColor, (yMax - uiVertices.List[i].position.y) * div)
					);
				}
				vh.Clear();
				vh.AddUIVertexTriangleStream(uiVertices.List);

			}
		}

		/// <summary>
		/// Opens the API reference page.
		/// </summary>
		[ContextMenu("API Reference")]
		private void OpenAPIReferencePage()
		{
			this.OpenReferencePage("uas-hypertext");
		}
	}
}