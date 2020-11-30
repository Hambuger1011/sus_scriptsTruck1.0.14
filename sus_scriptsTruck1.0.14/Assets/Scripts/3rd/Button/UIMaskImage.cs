using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.UI;

[XLua.Hotfix, XLua.LuaCallCSharp]

public class UIMaskImage : Image
{

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        //base.OnPopulateMesh(toFill);
        toFill.Clear();
        UIVertex uiVertex = UIVertex.simpleVert;
        uiVertex.color = this.color;
        for (int i = 0; i < verts.Count; ++i)
        {
            uiVertex.position = verts[i];
            uiVertex.uv0 = uvs[i];
            toFill.AddVert(uiVertex);
        }
        for(int i = 0; i < indices.Count; i += 3)
        {
            toFill.AddTriangle(indices[i], indices[i+1], indices[i+2]);
        }
    }

    List<Vector2> verts = new List<Vector2>();
    List<int> indices = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    public void SetRectangle(Rect inner, Rect outter)
    {
        verts.Clear();
        indices.Clear();
        uvs.Clear();
        float w = outter.width;
        float h = outter.height;
        var dmin = inner.min - outter.min;
        var dmax = inner.max - outter.min;
        dmin.x /= w;
        dmin.y /= h;
        dmax.x /= w;
        dmax.y /= h;

        var min = inner.min;
        var max = inner.max;
        verts.Add(new Vector2(min.x, min.y));
        verts.Add(new Vector2(max.x, min.y));
        verts.Add(new Vector2(max.x, max.y));
        verts.Add(new Vector2(min.x, max.y));

        uvs.Add(new Vector2(dmin.x, dmin.y));
        uvs.Add(new Vector2(dmax.x, dmin.y));
        uvs.Add(new Vector2(dmax.x, dmax.y));
        uvs.Add(new Vector2(dmin.x, dmax.y));

        min = outter.min;
        max = outter.max;
        verts.Add(new Vector2(min.x, min.y));
        verts.Add(new Vector2(max.x, min.y));
        verts.Add(new Vector2(max.x, max.y));
        verts.Add(new Vector2(min.x, max.y));

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));

        for (int i = 0; i < 4; ++i)
        {
            int n = (i == 3) ? 0 : (i + 1);
            int n1 = (i == 3) ? 4 : (i + 4 + 1);

            //逆时针画三角
            indices.Add(i + 4);
            indices.Add(i);
            indices.Add(n1);


            indices.Add(i);
            indices.Add(n);
            indices.Add(n1);
        }
        this.SetVerticesDirty();
    }


    public void SetInnerRectangle(Rect inner)
    {
        var outter = this.rectTransform.rect;
        this.SetRectangle(inner, outter);
    }

    [ContextMenu("Test")]
    public void Test()
    {
        var r = CUIManager.s_resolution;
        var inner = new Rect(-350, -552, 700, 1104);
        var outter = new Rect(-r.x * 0.5f, -r.y * 0.5f, r.x, r.y);
        this.SetRectangle(inner, outter);
        this.SetVerticesDirty();
    }

    [ContextMenu("Test2")]
    public void Test2()
    {
        var r = CUIManager.s_resolution;
        var inner = new Rect(-350, r.y * 0.5f - 1104 - 50, 700, 1104);
        this.SetInnerRectangle(inner);
    }
}
