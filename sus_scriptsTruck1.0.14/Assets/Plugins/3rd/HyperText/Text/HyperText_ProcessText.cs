using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Candlelight.UI
{
    public partial class HyperText
    {

        /// <summary>
        /// Updates the text processor.
        /// </summary>
        private void UpdateTextProcessor()
        {
            this.TextProcessor.ReferenceFontSize = this.FontSizeToUse;
            this.TextProcessor.InputText = this.text;
            this.TextProcessor.IsDynamicFontDesired = this.FontToUse != null && this.FontToUse.dynamic;
            this.TextProcessor.IsRichTextDesired = this.supportRichText;
            this.TextProcessor.ScaleFactor = 1f;//this.pixelsPerUnit;
        }

        /// <summary>
        /// Postprocess the text data before submitting it to cachedTextGenerator.
        /// </summary>
        private string PostprocessText()
        {
            UpdateTextProcessor();

            // clear existing data
            for (int i = 0; i < m_LinkGeometryData.Count; ++i)
            {
                m_LinkGeometryData[i].ColorTweenInfo.ColorChanged -= OnAnimateLinkColor;
                m_LinkGeometryData[i].Dispose();
            }
            m_LinkGeometryData.Clear();

            m_CustomTagGeometryData.Clear();

            //重置quad
            m_QuadGeometryData.Clear();
            m_QuadRenderersPool.RemoveAll(quadRenderer => quadRenderer == null);
            for (int i = 0; i < m_QuadRenderersPool.Count; ++i)
            {
#if USE_QUAD_IMAGE
                m_QuadRenderersPool[i].sprite = null;
                //m_QuadRenderersPool[i].enabled = false;
#else
                m_QuadRenderersPool[i].Clear();
#endif
#if UNITY_EDITOR
                m_QuadRenderersPool[i].name = "-";
#endif
            }

#region 获取link data
            using (var linkCharacterData = new ListPool<HyperTextProcessor.Link>.Scope())
            {
                this.TextProcessor.GetLinks(linkCharacterData.List);
                for (int i = 0; i < linkCharacterData.List.Count; ++i)
                {
                    m_LinkGeometryData.Add(new Link(i, linkCharacterData.List[i], this));
                }
            }
#endregion
            
            // set up other rich tags if enabled
            if (this.TextProcessor.IsRichTextEnabled)
            {
#region 获取 CustomTags
                // add custom text style tag geometry data
                using (ListPool<HyperTextProcessor.CustomTag>.Scope tagCharacterData =
                    new ListPool<HyperTextProcessor.CustomTag>.Scope()
                )
                {
                    this.TextProcessor.GetCustomTags(tagCharacterData.List);
                    for (int i = 0; i < tagCharacterData.List.Count; ++i)
                    {
                        m_CustomTagGeometryData.Add(new CustomTag(tagCharacterData.List[i]));
                    }
                }
#endregion

#region 获取 quad
                // set up quads if the current object is not a prefab and does not use sub-meshes on the main canvas
                if (!this.IsPrefab)
                {
                    m_QuadTracker.Clear();
                    RectTransform quadTransform = null;
                    using (var quadCharacterData = new ListPool<HyperTextProcessor.Quad>.Scope())
                    {
                        this.TextProcessor.GetQuads(quadCharacterData.List);
                        for (int matchIndex = 0; matchIndex < quadCharacterData.List.Count; ++matchIndex)
                        {
                            // TODO: switch over to ObjectX.GetFromPool()
                            // add new quad data to list
                            m_QuadGeometryData.Add(new Quad(quadCharacterData.List[matchIndex]));
                            // grow pool if needed
                            if (matchIndex >= m_QuadRenderersPool.Count)
                            {

#if USE_QUAD_IMAGE
                                GameObject newQuadObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Hypertext_Quad"));
                                m_QuadRenderersPool.Add(newQuadObject.GetComponent<Image>());
#else
                                GameObject newQuadObject = new GameObject("<quad>", typeof(RectTransform), typeof(CanvasRenderer));
                                m_QuadRenderersPool.Add(newQuadObject.GetComponent<CanvasRenderer>());
#endif

#if UNITY_EDITOR
                                newQuadObject.hideFlags = HideFlags.DontSave;
                                // ensure changes to prefab instances' pools get serialized when not selected
                                if (!Application.isPlaying)
								{
									UnityEditor.EditorUtility.SetDirty(this);
								}
#endif
                            }
#if true
                            if (matchIndex >= m_QuadMeshes.Count)
                            {
                                Mesh mesh = new Mesh();
                                mesh.hideFlags = HideFlags.HideAndDontSave;
                                m_QuadMeshes.Add(mesh);
                            }
#endif
                            // make sure layer is the same
                            m_QuadRenderersPool[matchIndex].gameObject.layer = this.gameObject.layer;
                            // lock transform
                            quadTransform = m_QuadRenderersPool[matchIndex].transform as RectTransform;
                            if (quadTransform != null)
                            {
                                quadTransform.SetParent(this.rectTransform);
                                m_QuadTracker.Add(this, quadTransform, DrivenTransformProperties.All);//禁止手动修改inpector属性

#if USE_QUAD_IMAGE
                                quadTransform.anchorMax = Vector2.one * 0.5f;
                                quadTransform.anchorMin = Vector2.zero * 0.5f;
                                quadTransform.sizeDelta = Vector2.zero;
                                quadTransform.pivot = this.rectTransform.pivot;
                                quadTransform.localPosition = Vector3.zero;
                                quadTransform.localRotation = Quaternion.identity;
                                quadTransform.localScale = Vector3.one;
#else
                                quadTransform.anchorMax = Vector2.one;
                                quadTransform.anchorMin = Vector2.zero;
                                quadTransform.sizeDelta = Vector2.zero;
                                quadTransform.pivot = this.rectTransform.pivot;
                                quadTransform.localPosition = Vector3.zero;
                                quadTransform.localRotation = Quaternion.identity;
                                quadTransform.localScale = Vector3.one;
#endif
                            }
                            // configure quad

#if USE_QUAD_IMAGE
                            m_QuadGeometryData[matchIndex].Renderer = m_QuadRenderersPool[matchIndex];
                            m_QuadGeometryData[matchIndex].Renderer.sprite = null;
                            //m_QuadGeometryData[matchIndex].Renderer.enabled = true;
#else
                            m_QuadGeometryData[matchIndex].Renderer = m_QuadRenderersPool[matchIndex];
                            m_QuadGeometryData[matchIndex].Renderer.Clear();
#endif
                        }
                    }
                }
#endregion
            }

            m_TextGeneratorInput = this.TextProcessor.OutputText;//获取处理后的text
            return m_TextGeneratorInput;
        }

    }
}