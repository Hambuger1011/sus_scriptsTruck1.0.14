using Framework;

/*
颜色渐变
http://www.jianshu.com/p/bebe5aa4b3cf
https://2013.mutoo.im/2015/10/text-on-a-circle-in-ugui.html
*/
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


namespace UiEffect
{
    [AddComponentMenu ("UI/Effects/Gradient Color")]
    [RequireComponent (typeof (Graphic))]
    public class GradientColor : BaseMeshEffect
    {
        public enum DIRECTION
        {
            Vertical,
            Horizontal,
            Both,
        }

        public DIRECTION direction = DIRECTION.Both;
        public Color colorTop = Color.white;
        public Color colorBottom = Color.black;
        public Color colorLeft = Color.red;
        public Color colorRight = Color.blue;
        
        

        /// <summary>
        /// Refresh Gradient Color on playing.
        /// </summary>
        public void Refresh ()
        {
            if (graphic != null) {
                graphic.SetVerticesDirty ();
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (IsActive() == false)
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
            /*
            for (var i = 0; i < count; i++)
            {
                var vertex = new UIVertex();
                vh.PopulateUIVertex(ref vertex, i);
                vertexs.Add(vertex);
            }
            */

            #region 修改

            float topX = 0f, topY = 0f, bottomX = 0f, bottomY = 0f;
            foreach (var vertex in vertexs)
            {
                topX = Mathf.Max(topX, vertex.position.x);
                topY = Mathf.Max(topY, vertex.position.y);
                bottomX = Mathf.Min(bottomX, vertex.position.x);
                bottomY = Mathf.Min(bottomY, vertex.position.y);
            }
            float width = topX - bottomX;
            float height = topY - bottomY;

            UIVertex tempVertex = vertexs[0];
            for (int i = 0; i < vertexs.Count; i++)
            {
                tempVertex = vertexs[i];
                byte orgAlpha = tempVertex.color.a;
                Color colorOrg = tempVertex.color;
                Color colorV = Color.Lerp(colorBottom, colorTop, (tempVertex.position.y - bottomY) / height);
                Color colorH = Color.Lerp(colorLeft, colorRight, (tempVertex.position.x - bottomX) / width);
                switch (direction)
                {
                    case DIRECTION.Both:
                        tempVertex.color = colorOrg * colorV * colorH;
                        break;
                    case DIRECTION.Vertical:
                        tempVertex.color = colorOrg * colorV;
                        break;
                    case DIRECTION.Horizontal:
                        tempVertex.color = colorOrg * colorH;
                        break;
                }
                tempVertex.color.a = orgAlpha;
                vertexs[i] = tempVertex;
            }
            #endregion
            //vh.Clear();
            vh.AddUIVertexTriangleStream(vertexs);

        }
    }
}
