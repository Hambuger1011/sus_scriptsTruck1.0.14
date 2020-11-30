using UnityEngine;
using System.Collections.Generic;
using System;

namespace Candlelight.UI
{
    public partial class HyperText
    {


#if UNITY_EDITOR

        /// <summary>
		/// Raises the validate event.
		/// </summary>
		protected override void OnValidate()
        {
            base.OnValidate();
            ClearQuadMaskMaterial();
        }

        /// <summary>
        /// Raises the rebuild requested event.
        /// </summary>
        public override void OnRebuildRequested()
        {
            FontUpdateTracker.UntrackHyperText(this);
            FontUpdateTracker.TrackHyperText(this);
            base.OnRebuildRequested();
        }
#endif



        #region UI.IClippable
#if true
        /// <summary>
        /// Cull this object's <see cref="UnityEngine.CanvasRenderer"/> if it is outside <paramref name="clipRect"/>.
        /// </summary>
        /// <param name="clipRect">Clipping rectangle.</param>
        /// <param name="validRect">
        /// If set to <see langword="true"/> then <paramref name="clipRect"/> is a valid rectangle.
        /// </param>
        public override void Cull(Rect clipRect, bool validRect)
        {
            base.Cull(clipRect, validRect);
            for (int i = 0; i < m_QuadRenderersPool.Count; ++i)
            {
                if (m_QuadRenderersPool[i] != null)
                {
#if USE_QUAD_IMAGE
                    m_QuadRenderersPool[i].canvasRenderer.cull = this.canvasRenderer.cull;
#else
                    m_QuadRenderersPool[i].cull = this.canvasRenderer.cull;
#endif
                }
            }
        }

        /// <summary>
        /// Sets the clipping rectangle on this object's <see cref="UnityEngine.CanvasRenderer"/>.
        /// </summary>
        public override void SetClipRect(Rect clipRect, bool validRect)
        {
            base.SetClipRect(clipRect, validRect);
            for (int i = 0; i < m_QuadRenderersPool.Count; ++i)
            {
                if (m_QuadRenderersPool[i] != null)
                {
#if USE_QUAD_IMAGE
                    if (validRect)
                    {
                        m_QuadRenderersPool[i].canvasRenderer.EnableRectClipping(clipRect);
                    }
                    else
                    {
                        m_QuadRenderersPool[i].canvasRenderer.DisableRectClipping();
                    }
#else
                    if (validRect)
                    {
                        m_QuadRenderersPool[i].EnableRectClipping(clipRect);
                    }
                    else
                    {
                        m_QuadRenderersPool[i].DisableRectClipping();
                    }
#endif
                }
            }
        }
#endif
#endregion


                    /// <summary>
                    /// Raises the canvas group changed event. Copied from UnityEngine.UI.Selectable.
                    /// </summary>
        protected override void OnCanvasGroupChanged()
        {
            // figure out if parent groups allow interaction
            bool doGroupsAllowInteraction = true;
            Transform t = this.transform;
            using (ListPool<CanvasGroup>.Scope canvasGroups = new ListPool<CanvasGroup>.Scope())
            {
                while (t != null)
                {
                    t.GetComponents(canvasGroups.List);
                    bool shouldBreak = false;
                    for (var i = 0; i < canvasGroups.List.Count; ++i)
                    {
                        if (!canvasGroups.List[i].interactable)
                        {
                            doGroupsAllowInteraction = false;
                            shouldBreak = true;
                        }
                        if (canvasGroups.List[i].ignoreParentGroups)
                        {
                            shouldBreak = true;
                        }
                    }
                    if (shouldBreak)
                    {
                        break;
                    }
                    t = t.parent;
                }
            }
            // trigger a state change if needed
            if (doGroupsAllowInteraction != m_DoGroupsAllowInteraction)
            {
                m_DoGroupsAllowInteraction = doGroupsAllowInteraction;
                OnInteractableChanged();
            }
        }


        /// <summary>
		/// Sets the material dirty.
		/// </summary>
		public override void SetMaterialDirty()
        {
            base.SetMaterialDirty();
            ClearQuadMaskMaterial();
        }

        /// <summary>
        /// Sets the vertices dirty.
        /// </summary>
        public override void SetVerticesDirty()
        {
            base.SetVerticesDirty();
            m_AreVerticesDirty = true;
        }

        /// <summary>
        /// Updates the geometry.
        /// </summary>
        protected override void UpdateGeometry()
        {
            m_LinkUnderCursor = null;
            m_LinkOnPointerDown = null;
            if (this.FontToUse == null)
            {
                m_AreVerticesDirty = false;
                return;
            }
            m_ShouldInvokeExternalDependencyCallback = false;
            // populate cachedTextGenerator, links, quads, and uiVertices
            // do not call base implementation of UpdateGeometry(), as it requires this.font to be set
            if (this.rectTransform.rect.width >= 0f && this.rectTransform.rect.height >= 0f)
            {
                OnPopulateMesh(s_VertexHelper);
            }
            // update the renderer to set link colors
            UpdateVertexColors();
            m_ShouldInvokeExternalDependencyCallback = true;
            m_AreVerticesDirty = false;
            UpdateMaterial(); // TODO: why is this necessary when enabling/disabling modifiers with quads?
        }



        protected override void OnPopulateMesh(UnityEngine.UI.VertexHelper vertexHelper)
        {
            // NOTE: Early out if already inside this method (i.e. font texture changed callback is disabled).
            // For some reason, the first call to cachedTextGenerator.Populate() triggers an immediate call to
            // UpdateGeometry() on Snapdragon 805/Adreno 420 devices.
            if (this.FontToUse == null || m_DisableFontTextureChangedCallback)
            {
                return;
            }
            // disable font texture changed callback
            m_DisableFontTextureChangedCallback = true;
            // get UI vertices from text generator
            Rect inputRect = this.rectTransform.rect;

            var settings = GetGenerationSettings(inputRect.size);
            var genText = PostprocessText();//处理文本
            this.cachedTextGenerator.Populate(genText, settings);//生成顶点


            this.cachedTextGenerator.GetVertices(m_UIVertices);//获取顶点
            UIVertex vertex;
            float unitsPerPixel = 1f / this.pixelsPerUnit;
#if UNITY_5_2 || UNITY_5_3_OR_NEWER
            // last 4 verts are always a new line as of Unity 5.2.0
            if (m_UIVertices.Count > 0)
            {
                m_UIVertices.RemoveRange(m_UIVertices.Count - 4, 4);
            }
#endif

#if true
            Vector2 textAnchorPivot = GetTextAnchorPivot(this.alignment);
            Vector2 refPoint = Vector2.zero;
            refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
            refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);
#else
            if(m_UIVertices.Count <= 0)
            {
                vertexHelper.Clear();
                return;
            }
            var refPoint = new Vector2(m_UIVertices[0].position.x, m_UIVertices[0].position.y) * unitsPerPixel;
#endif
            Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;
            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < m_UIVertices.Count; ++i)
                {
                    vertex = m_UIVertices[i];
                    vertex.position *= unitsPerPixel;
                    vertex.position.x = vertex.position.x + roundingOffset.x;
                    vertex.position.y = vertex.position.y + roundingOffset.y;
                    m_UIVertices[i] = vertex;
                }
            }
            else
            {
                for (int i = 0; i < m_UIVertices.Count; ++i)
                {
                    vertex = m_UIVertices[i];
                    vertex.position *= unitsPerPixel;
                    m_UIVertices[i] = vertex;
                }
            }
            // set final position, color, and UV for quads
            float fontSizeActuallyUsed = this.resizeTextForBestFit ?
            Mathf.CeilToInt(this.cachedTextGenerator.fontSizeUsedForBestFit / this.pixelsPerUnit) :
            this.FontSizeToUse;

#region quad
            for (int quadIndex = 0; quadIndex < m_QuadGeometryData.Count; ++quadIndex)
            {
                if (m_QuadGeometryData[quadIndex].VertexIndices.EndIndex >= m_UIVertices.Count)
                {
                    continue;
                }
                Rect uv = m_QuadGeometryData[quadIndex].UVRect;
                s_UVTransform[0] = new Vector2(uv.min.x, uv.max.y); // (0, 1)
                s_UVTransform[1] = new Vector2(uv.max.x, uv.max.y); // (1, 1)
                s_UVTransform[2] = new Vector2(uv.max.x, uv.min.y); // (1, 0)
                s_UVTransform[3] = new Vector2(uv.min.x, uv.min.y); // (0, 0)
                int scrollIndex = 0;
                bool clearColor = !m_QuadGeometryData[quadIndex].Style.ShouldRespectColorization;
                for (int i = 0; i < m_QuadGeometryData[quadIndex].VertexIndices.Count; ++i)
                {
                    int vertexIndex = m_QuadGeometryData[quadIndex].VertexIndices[i];
                    vertex = m_UIVertices[vertexIndex];
                    vertex.position += Vector3.up * m_QuadGeometryData[quadIndex].GetVerticalOffset(fontSizeActuallyUsed);
                    if (clearColor)
                    {
                        vertex.color = s_UntintedVertexColor;
                    }
                    vertex.uv0 = s_UVTransform[scrollIndex];
                    m_UIVertices[vertexIndex] = vertex;
                    ++scrollIndex;
                }
            }
#endregion

#region link
            // apply vertical offsets to all link and custom text styles
            Vector3 offset;
            // BUG: call to ctor in ObjectPool<List<TagGeometry>>.Get() throws MethodAccessException on Web Player
            List<TagGeometryData> tagData = s_TagData;
            int capacity = m_LinkGeometryData.Count + m_CustomTagGeometryData.Count;
            if (tagData.Capacity < capacity)
            {
                tagData.Capacity = capacity;
            }
            for (int i = 0; i < m_LinkGeometryData.Count; ++i)
            {
                tagData.Add(m_LinkGeometryData[i]);
            }
            for (int i = 0; i < m_CustomTagGeometryData.Count; ++i)
            {
                tagData.Add(m_CustomTagGeometryData[i]);
            }
            for (int i = 0; i < tagData.Count; ++i)
            {
                offset = tagData[i].GetVerticalOffset(fontSizeActuallyUsed) * Vector3.up;
                for (int j = 0; j < tagData[i].VertexIndices.Count; ++j)
                {
                    int vertexIndex = tagData[i].VertexIndices[j];
                    if (vertexIndex < 0 || vertexIndex >= m_UIVertices.Count)
                    {
                        continue;
                    }
                    vertex = m_UIVertices[vertexIndex];
                    vertex.position += offset;
                    m_UIVertices[vertexIndex] = vertex;
                }
            }
            tagData.Clear();
#endregion

            // get all the effects on this object
            List<Component> effects = ListPool<Component>.Get();
            GetComponents(typeof(UnityEngine.UI.IMeshModifier), effects);

#region meshLayout
            // offset values in character index tables to account for vertex modifier effects
#pragma warning disable 219
            MeshTopology meshLayout;
#pragma warning restore 219

            using (var customTagIndexRanges = new ListPool<IndexRange>.Scope())
            {
                capacity = m_LinkGeometryData.Count + m_QuadGeometryData.Count + m_CustomTagGeometryData.Count;
                if (customTagIndexRanges.List.Capacity < capacity)
                {
                    customTagIndexRanges.List.Capacity = capacity;
                }
                for (int i = 0; i < m_LinkGeometryData.Count; ++i)
                {
                    customTagIndexRanges.List.Add(m_LinkGeometryData[i].VertexIndices);
                }
                for (int i = 0; i < m_QuadGeometryData.Count; ++i)
                {
                    customTagIndexRanges.List.Add(m_QuadGeometryData[i].VertexIndices);
                }
                for (int i = 0; i < m_CustomTagGeometryData.Count; ++i)
                {
                    customTagIndexRanges.List.Add(m_CustomTagGeometryData[i].VertexIndices);
                }
                using (var customTagRedrawIndexRanges = new ListPool<List<IndexRange>>.Scope())
                {
                    if (customTagRedrawIndexRanges.List.Capacity < capacity)
                    {
                        customTagRedrawIndexRanges.List.Capacity = capacity;
                    }
                    for (int i = 0; i < m_LinkGeometryData.Count; ++i)
                    {
                        customTagRedrawIndexRanges.List.Add(m_LinkGeometryData[i].RedrawVertexIndices);
                    }
                    for (int i = 0; i < m_QuadGeometryData.Count; ++i)
                    {
                        customTagRedrawIndexRanges.List.Add(m_QuadGeometryData[i].RedrawVertexIndices);
                    }
                    for (int i = 0; i < m_CustomTagGeometryData.Count; ++i)
                    {
                        customTagRedrawIndexRanges.List.Add(m_CustomTagGeometryData[i].RedrawVertexIndices);
                    }
                    meshLayout = PostprocessVertexIndexRanges(effects, genText, customTagIndexRanges.List, customTagRedrawIndexRanges.List);
                }
            }
#endregion

            // fill vertex buffer


            vertexHelper.Clear();
            for (int i = 0; i < m_UIVertices.Count; ++i)
            {
                int quadIdx = i & 3;
                s_GlyphVertices[quadIdx] = m_UIVertices[i];
                if (quadIdx == 3)
                {
                    vertexHelper.AddUIVertexQuad(s_GlyphVertices);//添加顶点
                }
            }

            // apply any vertex modification effects to cached vertex buffer
            for (int i = 0; i < effects.Count; ++i)
            {
                if (!(effects[i] is Behaviour) || !((Behaviour)effects[i]).enabled)
                {
                    continue;
                }
                ((UnityEngine.UI.IMeshModifier)effects[i]).ModifyMesh(vertexHelper);
            }
            ListPool<Component>.Release(effects);

            Mesh glyphMesh = this.GlyphMesh;
            Action doSetMesh = () =>
            {
                vertexHelper.FillMesh(glyphMesh);
                // store colors and vertex positions
                m_VertexColors.Clear();
                m_VertexColors.AddRange(this.GlyphMesh.colors32);

                m_BaseVertexColors.Clear();
                m_BaseVertexColors.AddRange(this.GlyphMesh.colors32);

                m_VertexPositions.Clear();
                m_VertexPositions.AddRange(this.GlyphMesh.vertices);
            };
            doSetMesh();
            if(UpdateUnderline(vertexHelper, ref settings, roundingOffset))
            {
                doSetMesh();
            }

            // set up quad materials
            m_QuadMaterials.Clear();
            m_QuadTextures.Clear();
            for (int quadIdx = 0; quadIdx < m_QuadGeometryData.Count; ++quadIdx)
            {
                Texture2D quadTx = m_QuadGeometryData[quadIdx].Style.Sprite == null ? Texture2D.blackTexture : m_QuadGeometryData[quadIdx].Style.Sprite.texture;
                if (!m_QuadMaterials.ContainsKey(quadTx))
                {
                    m_QuadMaterials[quadTx] = null;
                    m_QuadTextures.Add(quadTx);
                }
            }
            // copy mesh data to quad canvas renderers and degenerate quad vertices on text VBO
            var textNormals = ListPool<Vector3>.Get();
            var textTangents = ListPool<Vector4>.Get();
            var textUV1 = ListPool<Vector2>.Get();
            var textUV2 = ListPool<Vector2>.Get();
            var textVertices = ListPool<Vector3>.Get();

            textNormals.AddRange(glyphMesh.normals);
            textTangents.AddRange(glyphMesh.tangents);
            textUV1.AddRange(glyphMesh.uv);
            textUV2.AddRange(glyphMesh.uv2);
            textVertices.AddRange(glyphMesh.vertices);

            var quadNormals = ListPool<Vector3>.Get();
            var quadTangents = ListPool<Vector4>.Get();
            var quadTriangles = ListPool<int>.Get();
            var quadUV1 = ListPool<Vector2>.Get();
            var quadUV2 = ListPool<Vector2>.Get();
            var quadVertexColors = ListPool<Color32>.Get();
            var quadVertices = ListPool<Vector3>.Get();

            using (var indexRanges = new ListPool<IndexRange>.Scope())
            {
                for (int quadIndex = 0; quadIndex < m_QuadGeometryData.Count; ++quadIndex)
                {
                    indexRanges.List.Clear();
                    indexRanges.List.AddRange(m_QuadGeometryData[quadIndex].RedrawVertexIndices);
                    indexRanges.List.Add(m_QuadGeometryData[quadIndex].VertexIndices);
                    quadVertices.Clear();
                    quadNormals.Clear();
                    quadTangents.Clear();
                    quadTriangles.Clear();
                    quadUV1.Clear();
                    quadUV2.Clear();
                    quadVertexColors.Clear();
                    for (int i = 0; i < indexRanges.List.Count; ++i)
                    {
                        if (indexRanges.List[i].StartIndex >= textVertices.Count)
                        {
                            continue;
                        }
                        for (int j = 0; j < indexRanges.List[i].Count; ++j)
                        {
                            int vertexIndex = indexRanges.List[i][j];
                            if (vertexIndex >= textVertices.Count)
                            {
                                continue;
                            }
                            quadNormals.Add(textNormals[vertexIndex]);
                            quadTangents.Add(textTangents[vertexIndex]);
                            quadUV1.Add(textUV1[vertexIndex]);
                            quadUV2.Add(textUV2[vertexIndex]);
                            quadVertexColors.Add(m_VertexColors[vertexIndex]);
                            quadVertices.Add(textVertices[vertexIndex]);
                            textVertices[vertexIndex] = textVertices[indexRanges.List[i].StartIndex];
                        }
                        int baseIdx;
                        switch (meshLayout)
                        {
                            case MeshTopology.Quads:
                                baseIdx = i * 4;
                                quadTriangles.Add(baseIdx);
                                quadTriangles.Add(baseIdx + 1);
                                quadTriangles.Add(baseIdx + 2);
                                quadTriangles.Add(baseIdx + 2);
                                quadTriangles.Add(baseIdx + 3);
                                quadTriangles.Add(baseIdx);
                                break;
                            case MeshTopology.Triangles:
                                baseIdx = i * 6;
                                for (int j = 0; j < indexRanges.List[i].Count; ++j)
                                {
                                    quadTriangles.Add(baseIdx + j);
                                }
                                break;
                        }
                    }
                    m_QuadMeshes[quadIndex].Clear();
                    m_QuadMeshes[quadIndex].SetVertices(quadVertices);
                    m_QuadMeshes[quadIndex].SetNormals(quadNormals);
                    m_QuadMeshes[quadIndex].SetTangents(quadTangents);
                    m_QuadMeshes[quadIndex].SetTriangles(quadTriangles, 0);
                    m_QuadMeshes[quadIndex].SetUVs(0, quadUV1);
                    m_QuadMeshes[quadIndex].SetUVs(1, quadUV2);
                }
                glyphMesh.SetVertices(textVertices);
                ListPool<Vector3>.Release(quadNormals);
                ListPool<Vector4>.Release(quadTangents);
                ListPool<int>.Release(quadTriangles);
                ListPool<Vector2>.Release(quadUV1);
                ListPool<Vector2>.Release(quadUV2);
                ListPool<Color32>.Release(quadVertexColors);
                ListPool<Vector3>.Release(quadVertices);
                ListPool<Vector3>.Release(textNormals);
                ListPool<Vector4>.Release(textTangents);
                ListPool<Vector2>.Release(textUV1);
                ListPool<Vector2>.Release(textUV2);
                ListPool<Vector3>.Release(textVertices);
            }

            // populate hitboxes of links
            UpdateLinkHitboxRects();
            // re-enable font texture changed callback
            m_DisableFontTextureChangedCallback = false;
        }

        private bool UpdateUnderline(UnityEngine.UI.VertexHelper vertexHelper,ref TextGenerationSettings settings, Vector2 roundingOffset)
        {
            bool isDirty = false;
            for (int linkIdx = 0; linkIdx < m_LinkGeometryData.Count; ++linkIdx)
            {
                var data = m_LinkGeometryData[linkIdx];
                if (!data.Style.TextStyle.Underline)
                {
                    continue;
                }
                if (m_LinkGeometryData[linkIdx].VertexIndices.StartIndex >= m_VertexPositions.Count)
                {
                    continue;
                }
                isDirty = true;
                UpdateUnderline2(vertexHelper, ref settings, roundingOffset, data.VertexIndices, data.Style.TextStyle, this.Styles.CascadedLinkHitboxPadding);
            }

            //padding = this.Styles.CascadedLinkHitboxPadding;

            for (int linkIdx = 0; linkIdx < this.m_CustomTagGeometryData.Count; ++linkIdx)
            {
                var data = m_CustomTagGeometryData[linkIdx];
                if(!data.Style.TextStyle.Underline)
                {
                    continue;
                }

                if (data.VertexIndices.StartIndex >= m_VertexPositions.Count)
                {
                    continue;
                }
                isDirty = true;
                UpdateUnderline2(vertexHelper,ref settings, roundingOffset, data.VertexIndices, data.Style.TextStyle, new ImmutableRectOffset());
            }
            return isDirty;
        }
        private void UpdateUnderline2(UnityEngine.UI.VertexHelper vertexHelper, ref TextGenerationSettings settings, Vector2 roundingOffset, IndexRange VertexIndices, RichTextStyle TextStyle, ImmutableRectOffset padding)
        {
            var position = m_VertexPositions[VertexIndices.StartIndex];
            var bounds = new Bounds(position, Vector3.zero);
            for (int i = 0, j = 0; i < VertexIndices.Count; ++i, ++j)
            {
                if (VertexIndices[i] >= m_VertexPositions.Count)
                {
                    continue;
                }
                position = m_VertexPositions[VertexIndices[i]];
                if (j % 4 == 0 && (position.x < bounds.min.x || position.y < bounds.min.y))//换行
                {
                    //hitboxes.List.Add(
                    //    new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y)
                    //);
                    GenerateUnderline(vertexHelper, ref settings, ref roundingOffset, ref bounds, TextStyle);
                    bounds = new Bounds(position, Vector3.zero);
                }
                else//同一行
                {
                    bounds.Encapsulate(position);
                }
            }
            bounds.min -= new Vector3(padding.Left, padding.Bottom);
            bounds.max += new Vector3(padding.Right, padding.Top);
            GenerateUnderline(vertexHelper, ref settings, ref roundingOffset, ref bounds, TextStyle);
        }

        /* 面片结构
         * p0-------p1
         *  |       | 
         *  |       |
         * p3-------p2
         * */
        private void GenerateUnderline(UnityEngine.UI.VertexHelper vertexHelper, ref TextGenerationSettings settings, ref Vector2 roundingOffset, ref Bounds bounds, RichTextStyle Style)
        {
            float unitsPerPixel = 1 / pixelsPerUnit;
            float fontSizeActuallyUsed = this.resizeTextForBestFit ?
            Mathf.CeilToInt(this.cachedTextGenerator.fontSizeUsedForBestFit / this.pixelsPerUnit) :
            this.FontSizeToUse;

            cachedTextGenerator.Populate("_", settings);
            var underlineVerts = cachedTextGenerator.verts;
            
            //Rect box = new Rect(bounds.min, bounds.size);
            //box.position -= new Vector2(0, 5); //向下偏移一个单位

            Vector3 p0 = underlineVerts[0].position;//TL
            Vector3 p1 = underlineVerts[1].position;//TR
            Vector3 p2 = underlineVerts[2].position;//BR

            float height = p1.y - p2.y;
            float width = p1.x - p0.x;

            Vector2 uv0 = underlineVerts[0].uv0;
            Vector2 uv1 = underlineVerts[1].uv0;
            Vector2 uv2 = underlineVerts[2].uv0;
            Vector2 uv3 = underlineVerts[3].uv0;

            //顶部中心uv
            Vector2 topCenterUv = uv0 + (uv1 - uv0) * 0.5f;
            //底部中心uv
            Vector2 bottomCenterUv = uv3 + (uv2 - uv3) * 0.5f;

            s_GlyphVertices[0] = underlineVerts[0];
            s_GlyphVertices[1] = underlineVerts[1];
            s_GlyphVertices[2] = underlineVerts[2];
            s_GlyphVertices[3] = underlineVerts[3];

            if (Style.ShouldReplaceUnderlineColor)
            {
                for (int i = 0; i < 4; ++i)
                {
                    s_GlyphVertices[i].color = Style.UnderlineColor;
                }
            }

            //var linkUnderlineColor = Color.red;
            //s_GlyphVertices[0].color = linkUnderlineColor;
            //s_GlyphVertices[1].color = linkUnderlineColor;
            //s_GlyphVertices[2].color = linkUnderlineColor;
            //s_GlyphVertices[3].color = linkUnderlineColor;

            float xMin = bounds.min.x;
            float yMin = bounds.min.y - fontSizeActuallyUsed * 0.1f;

            float xMax = bounds.max.x;
            float yMax = yMin + height;


            //left
            {
                s_GlyphVertices[0].position = new Vector3(xMin, yMax) * unitsPerPixel;
                s_GlyphVertices[0].uv0 = uv0;

                s_GlyphVertices[1].position = new Vector3(xMin + width * 0.5f, yMax) * unitsPerPixel;
                s_GlyphVertices[1].uv0 = topCenterUv;

                s_GlyphVertices[2].position = new Vector3(xMin + width * 0.5f, yMin) * unitsPerPixel;
                s_GlyphVertices[2].uv0 = bottomCenterUv;

                s_GlyphVertices[3].position = new Vector3(xMin, yMin) * unitsPerPixel;
                s_GlyphVertices[3].uv0 = uv3;

                vertexHelper.AddUIVertexQuad(s_GlyphVertices);
            }
            {
                s_GlyphVertices[0].position = new Vector3(xMin + width * 0.5f, yMax) * unitsPerPixel;
                s_GlyphVertices[0].uv0 = topCenterUv;

                s_GlyphVertices[1].position = new Vector3(xMax - width * 0.5f, yMax) * unitsPerPixel;
                s_GlyphVertices[1].uv0 = topCenterUv;

                s_GlyphVertices[2].position = new Vector3(xMax - width * 0.5f, yMin) * unitsPerPixel;
                s_GlyphVertices[2].uv0 = bottomCenterUv;

                s_GlyphVertices[3].position = new Vector3(xMin + width * 0.5f, yMin) * unitsPerPixel;
                s_GlyphVertices[3].uv0 = bottomCenterUv;

                vertexHelper.AddUIVertexQuad(s_GlyphVertices);
            }

            //right
            {
                s_GlyphVertices[0].position = new Vector3(xMax - width * 0.5f, yMax) * unitsPerPixel;
                s_GlyphVertices[0].uv0 = topCenterUv;

                s_GlyphVertices[1].position = new Vector3(xMax, yMax) * unitsPerPixel;
                s_GlyphVertices[1].uv0 = uv1;

                s_GlyphVertices[2].position = new Vector3(xMax, yMin) * unitsPerPixel;
                s_GlyphVertices[2].uv0 = uv2;

                s_GlyphVertices[3].position = new Vector3(xMax - width * 0.5f, yMin) * unitsPerPixel;
                s_GlyphVertices[3].uv0 = bottomCenterUv;

                vertexHelper.AddUIVertexQuad(s_GlyphVertices);
            }
        }



        /// <summary>
        /// Updates the vertex colors on all <see cref="UnityEngine.CanvasRenderer"/>s.
        /// </summary>
        private void UpdateVertexColors()
        {
            int vertexCount = m_VertexColors.Count;
            #region colorize links
            for (int i = 0; i < m_LinkGeometryData.Count; ++i)
            {
                HyperTextStyles.Link style = m_LinkGeometryData[i].Style;
                Color stateColor = m_LinkGeometryData[i].Tint;
                for (
                    int vertexIndex = m_LinkGeometryData[i].VertexIndices.StartIndex;
                    vertexIndex < Mathf.Min(m_LinkGeometryData[i].VertexIndices.EndIndex + 1, vertexCount);
                    ++vertexIndex
                )
                {
                    Color vertexColor = stateColor;
                    Color baseColor = m_BaseVertexColors[vertexIndex];
                    switch (style.ColorTintMode)
                    {
                        case ColorTintMode.Additive:
                            vertexColor = stateColor + baseColor;
                            break;
                        case ColorTintMode.Constant:
                            vertexColor = stateColor;
                            break;
                        case ColorTintMode.Multiplicative:
                            vertexColor = stateColor * baseColor;
                            break;
                    }
                    switch (style.ColorTweenMode)
                    {
                        case ColorTween.Mode.RGB:
                            vertexColor.a = baseColor.a;
                            break;
                        case ColorTween.Mode.Alpha:
                            vertexColor.r = baseColor.r;
                            vertexColor.g = baseColor.g;
                            vertexColor.b = baseColor.b;
                            break;
                    }
                    m_VertexColors[vertexIndex] = vertexColor;
                }
            }
            #endregion

            #region colorize quads and set the vertices on managed CanvasRenderers
            using (ListPool<IndexRange>.Scope indexRanges = new ListPool<IndexRange>.Scope())
            {
                using (ListPool<Color32>.Scope quadVertexColors = new ListPool<Color32>.Scope())
                {
                    for (int quadIndex = 0; quadIndex < m_QuadGeometryData.Count; ++quadIndex)
                    {
                        // empty out renderers for quads that are clipped
                        if (m_QuadGeometryData[quadIndex].VertexIndices.EndIndex >= vertexCount)
                        {
#if USE_QUAD_IMAGE
                            m_QuadGeometryData[quadIndex].Renderer.sprite = null;
                            //m_QuadGeometryData[quadIndex].Renderer.enabled = false;
#else
                            m_QuadGeometryData[quadIndex].Renderer.Clear();
#endif
                        }
                        else
                        {
                            indexRanges.List.Clear();
                            indexRanges.List.AddRange(m_QuadGeometryData[quadIndex].RedrawVertexIndices);
                            indexRanges.List.Add(m_QuadGeometryData[quadIndex].VertexIndices);
                            // copy colors from vertex list and apply to quad renderer
                            quadVertexColors.List.Clear();
                            for (int i = 0; i < indexRanges.List.Count; ++i)
                            {
                                bool doSwizzle = i == indexRanges.List.Count - 1 && HyperText.ShouldSwizzleQuadRedBlue;
                                bool clearColor = i == indexRanges.List.Count - 1 &&
                                    !m_QuadGeometryData[quadIndex].Style.ShouldRespectColorization;
                                for (int j = 0; j < indexRanges.List[i].Count; ++j)
                                {
                                    int vertexIndex = indexRanges.List[i][j];
                                    if (vertexIndex >= vertexCount)
                                    {
                                        continue;
                                    }

                                    Color32 vertexColor = m_VertexColors[vertexIndex];
                                    if (clearColor)
                                    {
                                        vertexColor = s_UntintedVertexColor;
                                    }
                                    else if (doSwizzle)
                                    {
                                        vertexColor = new Color32(vertexColor.b, vertexColor.g, vertexColor.r, vertexColor.a);
                                    }
                                    quadVertexColors.List.Add(vertexColor);
                                }
                            }

                            m_QuadMeshes[quadIndex].SetColors(quadVertexColors.List);
                            m_QuadGeometryData[quadIndex].Renderer.SetMesh(m_QuadMeshes[quadIndex]);
                        }
                    }
                }
            }
            #endregion

            this.GlyphMesh.SetColors(m_VertexColors);
            this.canvasRenderer.SetMesh(this.GlyphMesh);
        }


        /// <summary>
        /// Updates the material.
        /// </summary>
        protected override void UpdateMaterial()
        {
            base.UpdateMaterial();
            if (!IsActive())
            {
                return;
            }
            Material quadMaterialForRendering = this.QuadMaterialForRendering;
            for (int i = 0; i < m_QuadGeometryData.Count; ++i)
            {
                var quad = m_QuadGeometryData[i];
#if USE_QUAD_IMAGE
                quad.Renderer.material = quadMaterialForRendering;
                quad.Renderer.sprite = quad.Texture;
#else
                quad.Renderer.SetMaterial(quadMaterialForRendering, quad.Texture);
#endif
#if UNITY_EDITOR
                quad.RectTransform.name = quad.Style.ClassName;
#endif
            }
        }

    }
}
