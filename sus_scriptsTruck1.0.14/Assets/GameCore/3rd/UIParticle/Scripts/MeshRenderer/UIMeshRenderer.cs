using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;


namespace GameCore.UIExtensions
{
	/// <summary>
	/// Render maskable and sortable particle effect ,without Camera, RenderTexture or Canvas.
	/// </summary>
	[ExecuteInEditMode]
	public class UIMeshRenderer : MaskableGraphic
	{
		//################################
		// Constant or Readonly Static Members.
		//################################
		static readonly int s_IdMainTex = Shader.PropertyToID("_MainTex");
		static readonly List<Vector3> s_Vertices = new List<Vector3>();


        //################################
        // Serialize Members.
        //################################
        [Header("Mesh是否经常变化")]
        public bool isDynamic = false;
        [Tooltip("The ParticleSystem rendered by CanvasRenderer")]
		[SerializeField] MeshRenderer m_meshRenderer;
        [SerializeField] MeshFilter m_meshFilter;


        //################################
        // Public/Protected Members.
        //################################
        public override Texture mainTexture
		{
			get
			{
				Texture tex = null;
				if (!tex && m_meshRenderer)
				{
					Profiler.BeginSample("Check material");
					var mat = Application.isPlaying
							? m_meshRenderer.material
							: m_meshRenderer.sharedMaterial;
					if (mat && mat.HasProperty(s_IdMainTex))
					{
						tex = mat.mainTexture;
					}
					Profiler.EndSample();
				}
				return tex ?? s_WhiteTexture;
			}
		}

		public override Material GetModifiedMaterial(Material baseMaterial)
		{
			return base.GetModifiedMaterial(m_meshRenderer ? m_meshRenderer.sharedMaterial : baseMaterial);
		}

		protected override void OnEnable()
		{
            m_meshRenderer = GetComponent<MeshRenderer>();
            m_meshFilter = GetComponent<MeshFilter>();
            m_meshRenderer.enabled = false;
            base.OnEnable();

            if(isDynamic)
            {
                Canvas.willRenderCanvases += UpdateMesh;
            }
        }

		protected override void OnDisable()
		{
            if (isDynamic)
            {
                Canvas.willRenderCanvases -= UpdateMesh;
            }
            base.OnDisable();
        }

		protected override void UpdateGeometry()
        {
            if (!isDynamic)
            {
                UpdateMesh();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            //base.OnPopulateMesh(vh);
            //vh.Clear();
        }

        void UpdateMesh()
		{
			try
			{
				if (m_meshRenderer != null)
				{
                    // Set mesh to CanvasRenderer.
                    Profiler.BeginSample("Set mesh and texture to CanvasRenderer");
                    canvasRenderer.SetMesh(Application.isPlaying ? m_meshFilter.mesh : m_meshFilter.sharedMesh);
                    canvasRenderer.SetTexture(mainTexture);
                    Profiler.EndSample();
                }
			}
			catch(System.Exception e)
			{
				Debug.LogException(e);
			}
		}
	}
}