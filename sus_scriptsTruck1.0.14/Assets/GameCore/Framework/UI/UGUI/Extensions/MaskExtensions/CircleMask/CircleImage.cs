using Framework;

/*
 * 功能描述：1、无需Mask，修改顶点形成圆形Image,只有一个DrawCall，可以跟其它Image合批
 *           2、精确点击

Unity自带Mask渲染消耗
许多游戏项目里免不了有很多图片是以圆形形式展示的，如头像，技能Icon等，一般做法是使用Image组件，再加上一个圆形的Mask。实现非常简单，但因为影响效率，许多关于ui方面的Unity效率优化文章，都会建议开发者少用Mask。

使用Mask会额外消耗多一个Drawcall来创建Mask，做像素剔除。
Mask不利于层级合并。原本同一图集里的ui可以合并层级，仅需一个Drawcall渲染，如果加入Mask，就会将一个ui整体分割成了Mask下的子ui与其他ui，两者只能各自进行层级合并，至少要两个Drawcall。Mask用得多了，一个ui整体会被分割得四分五裂，就会严重影响层次合并的效率了。
无法精确点击
Image+Mask的实现的圆形，点击判断不精确，点击到圆形外的四个边角仍会触发点击，虽然可以通过另外设置eventAlphaThreshold实现像素级判断，但这个方法有天生缺陷，并不是好的选择。

*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

[AddComponentMenu("UI/MaskEx/CircleMaskImage")]
public class CircleImage : Image
{

    [Header("圆形或扇形填充比例")]
    [Range(0, 1)]
    public float fillPercent = 1f;

    [Header("是否填充圆形")]
    public bool isSolidCircle = true;

    [Header("圆环宽度")]
    public float thickness = 5;

    [Header("圆形")]
    [Range(3, 100)]
    public int segements = 20;

    [Header("图片Scale")]
    [Range(0, 2)]
    public float uiScale = 1.0f;

    /// <summary>
    /// 内圆顶点
    /// </summary>
    private List<Vector3> innerVertices;

    /// <summary>
    /// 外圆顶点
    /// </summary>
    private List<Vector3> outterVertices;



    protected override void Awake()
    {
        base.Awake();
        innerVertices = new List<Vector3>();
        outterVertices = new List<Vector3>();
    }

#if UNITY_EDITOR
    void Update()
    {
        this.thickness = (float)Mathf.Clamp(this.thickness, 0, rectTransform.rect.width / 2);
    }
#endif


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        innerVertices.Clear();
        outterVertices.Clear();

        float degreeDelta = (float)(2 * Mathf.PI / segements);//每个扇形的角度
        int curSegements = (int)(segements * fillPercent);//显示的段数

        float tw = rectTransform.rect.width;
        float th = rectTransform.rect.height;
        float outerRadius = rectTransform.pivot.x * tw;
        float innerRadius = rectTransform.pivot.x * tw - thickness;

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;//图片uv

        //中心点
        float uvCenterX = (uv.x + uv.z) * 0.5f;
        float uvCenterY = (uv.y + uv.w) * 0.5f;

        //大小
        float uvScaleX = (uv.z - uv.x) / tw * uiScale;
        float uvScaleY = (uv.w - uv.y) / th * uiScale;

        float curDegree = 0;
        UIVertex uiVertex;
        int verticeCount;
        int triangleCount;
        Vector2 curVertice;

        if (isSolidCircle) //圆形
        {
            curVertice = Vector2.zero;
            verticeCount = curSegements + 1;
            uiVertex = new UIVertex();
            uiVertex.color = color;
            uiVertex.position = curVertice;
            uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
            vh.AddVert(uiVertex);//设置ui顶点

            for (int i = 1; i < verticeCount; i++)
            {
                float cosA = Mathf.Cos(curDegree);//x轴
                float sinA = Mathf.Sin(curDegree);//y轴
                curVertice = new Vector2(cosA * outerRadius, sinA * outerRadius);
                curDegree += degreeDelta;

                uiVertex = new UIVertex();
                uiVertex.color = color;
                uiVertex.position = curVertice;
                uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                vh.AddVert(uiVertex);

                outterVertices.Add(curVertice);
            }

            triangleCount = curSegements*3;
            for (int i = 0, vIdx = 1; i < triangleCount - 3; i += 3, vIdx++)
            {
                vh.AddTriangle(vIdx, 0, vIdx+1);//设置三角形顶点数据
            }
            if (fillPercent == 1)
            {
                //首尾顶点相连
                vh.AddTriangle(verticeCount - 1, 0, 1);//设置三角形顶点数据
            }
        }
        else//圆环
        {
            verticeCount = curSegements*2;
            for (int i = 0; i < verticeCount; i += 2)
            {
                float cosA = Mathf.Cos(curDegree);
                float sinA = Mathf.Sin(curDegree);
                curDegree += degreeDelta;

                curVertice = new Vector3(cosA * innerRadius, sinA * innerRadius);
                uiVertex = new UIVertex();
                uiVertex.color = color;
                uiVertex.position = curVertice;
                uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                vh.AddVert(uiVertex);
                innerVertices.Add(curVertice);

                curVertice = new Vector3(cosA * outerRadius, sinA * outerRadius);
                uiVertex = new UIVertex();
                uiVertex.color = color;
                uiVertex.position = curVertice;
                uiVertex.uv0 = new Vector2(curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
                vh.AddVert(uiVertex);
                outterVertices.Add(curVertice);
            }

            triangleCount = curSegements*3*2;
            for (int i = 0, vIdx = 0; i < triangleCount - 6; i += 6, vIdx += 2)
            {
                vh.AddTriangle(vIdx+1, vIdx, vIdx+3);
                vh.AddTriangle(vIdx, vIdx + 2, vIdx + 3);
            }
            if (fillPercent == 1)
            {
                //首尾顶点相连
                vh.AddTriangle(verticeCount - 1, verticeCount - 2, 1);
                vh.AddTriangle(verticeCount - 2, 0, 1);
            }
        }

    }

    #region 点击事件
    /// <summary>
    /// 判断点击区域是否有效
    /// </summary>
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Sprite sprite = overrideSprite;
        if (sprite == null)
            return true;

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);
        return Contains(local, outterVertices, innerVertices);
    }

    /// <summary>
    /// Ray-Crossing算法大概思路是从指定点p发出一条射线，与多边形相交，假若交点个数是奇数，说明点p落在多边形内，交点个数为偶数说明点p在多边形外。
    /// </summary>
    private bool Contains(Vector2 p, List<Vector3> outterVertices, List<Vector3> innerVertices)
    {
        var crossNumber = 0;
        RayCrossing(p, innerVertices, ref crossNumber);//检测内环
        RayCrossing(p, outterVertices, ref crossNumber);//检测外环
        return (crossNumber & 1) == 1;
    }

    /// <summary>
    /// 使用RayCrossing算法判断点击点是否在封闭多边形里
    /// </summary>
    /// <param name="p"></param>
    /// <param name="vertices"></param>
    /// <param name="crossNumber"></param>
    private void RayCrossing(Vector2 p, List<Vector3> vertices, ref int crossNumber)
    {
        for (int i = 0, count = vertices.Count; i < count; i++)
        {
            var v1 = vertices[i];
            var v2 = vertices[(i + 1) % count];

            //点击点水平线必须与两顶点线段相交
            if (((v1.y <= p.y) && (v2.y > p.y))
                || ((v1.y > p.y) && (v2.y <= p.y)))
            {
                //只考虑点击点右侧方向，点击点水平线与线段相交，且交点x > 点击点x，则crossNumber+1
                if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                {
                    crossNumber += 1;
                }
            }
        }
    }
    #endregion

}
