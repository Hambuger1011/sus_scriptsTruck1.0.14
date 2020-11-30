using Framework;

/*
透明可点击区域

重写Graphic来代替透明Image响应UGUI事件
通常的做法就是创建1个默认的Image组件并将Color的Alpha值改为0
但即使是完全透明的Image，一样会占用1个Batches和1个SetPass call以及Tris和Verts

*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptySelectable : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}
