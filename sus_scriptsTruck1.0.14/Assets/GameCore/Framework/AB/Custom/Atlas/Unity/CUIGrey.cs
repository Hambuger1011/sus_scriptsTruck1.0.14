using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUIGrey : MonoBehaviour
{
    Graphic g;
    static Dictionary<Material, Material> matMap = new Dictionary<Material, Material>();

    Material mat;

    public void Init(Graphic g)
    {
        this.g = g;
        mat = g.material;
    }

    public void SetGrey(bool isGrey)
    {
        if(isGrey)
        {
            Material greyMat;
            if(!matMap.TryGetValue(mat,out greyMat))
            {
                greyMat = GameObject.Instantiate(mat);
                //greyMat.SetFloat("_IsGrey", 0);
                //greyMat.SetFloat("_IsGrey", 1);
                //greyMat.IsKeywordEnabled("_IsGrey");
                //greyMat.IsKeywordEnabled("UI_GREY");
                greyMat.shader = Shader.Find("UI/Default_Grey");
                matMap.Add(mat, greyMat);
            }
            g.material = greyMat;
        }
        else
        {
            g.material = mat;
        }
    }
}
