using Framework;

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UiEffect
{
    [AddComponentMenu ("UI/Effects/Blend Color")]
    [RequireComponent (typeof (Graphic))]
    public class BlendColor : BaseMeshEffect
    {
        public enum BLEND_MODE
        {
            Multiply,
            Additive,
            Subtractive,
            Override,
        }

        public BLEND_MODE blendMode = BLEND_MODE.Multiply;
        public Color color = Color.grey;

        //Graphic graphic;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (IsActive () == false) 
            {
                return;
            }

            var count = vh.currentVertCount;
            if (count == 0)
            {
                return;
            }

            List<UIVertex> vertexs = new List<UIVertex>();
            vh.GetUIVertexStream(vertexs);

            UIVertex tempVertex = vertexs[0];
            for (int i = 0; i < vertexs.Count; i++) {
                tempVertex = vertexs[i];
                byte orgAlpha = tempVertex.color.a;
                switch (blendMode) {
                    case BLEND_MODE.Multiply:
                        tempVertex.color *= color;
                        break;
                    case BLEND_MODE.Additive:
                        tempVertex.color += color;
                        break;
                    case BLEND_MODE.Subtractive:
                        tempVertex.color -= color;
                        break;
                    case BLEND_MODE.Override:
                        tempVertex.color = color;
                        break;
                }
                tempVertex.color.a = orgAlpha;
                vertexs[i] = tempVertex;
            }
        }

        /// <summary>
        /// Refresh Blend Color on playing.
        /// </summary>
        public void Refresh ()
        {
            //if (graphic == null) {
            //    graphic = GetComponent<Graphic> ();
            //}
            if (graphic != null) {
                graphic.SetVerticesDirty ();
            }
        }
    }
}
