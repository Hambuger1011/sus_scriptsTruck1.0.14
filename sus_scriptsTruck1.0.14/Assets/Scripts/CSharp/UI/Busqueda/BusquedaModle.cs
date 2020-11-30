using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BusquedaType
{
    
    Button1,
    Button2,
    Button3,
    Button4,
    Button5,
    Button6,
    Button7,
    Button8,
    Button9,
    Button10,
    Button11,
    Button12,

    End

}




public class BusquedaModle
{

   
    private BusquedaCtrl BusquedaCtrl;

    private string[] generoPrefbName;

    private List<BusquedaBookInfo> myPickBook;//保存符合挑选条件的书本数据
    public List<BusquedaBookInfo> allBookList;

    public BusquedaModle(BusquedaCtrl BusquedaCtrl)
    {
        this.BusquedaCtrl = BusquedaCtrl;

        generoPrefbName = new string[(int)BusquedaType.End];

        myPickBook = new List<BusquedaBookInfo>();

        AddGeneroPrefbName();
    }

    private void AddGeneroPrefbName()
    {
        generoPrefbName[(int)BusquedaType.Button1] = "Romance";
        generoPrefbName[(int)BusquedaType.Button2] = "LGBT";
        generoPrefbName[(int)BusquedaType.Button3] = "Action";
        generoPrefbName[(int)BusquedaType.Button4] = "Youth";
        generoPrefbName[(int)BusquedaType.Button5] = "Adventure";
        generoPrefbName[(int)BusquedaType.Button6] = "Drama";
        generoPrefbName[(int)BusquedaType.Button7] = "Comedy";
        generoPrefbName[(int)BusquedaType.Button8] = "Horror";
        generoPrefbName[(int)BusquedaType.Button9] = "18+";
        generoPrefbName[(int)BusquedaType.Button10] = "Fantasy";
        generoPrefbName[(int)BusquedaType.Button11] = "Suspense";
        generoPrefbName[(int)BusquedaType.Button12] = "Others";

    }

    public string[] GeneroPrefbName
    {
        get
        {
            return generoPrefbName != null ? generoPrefbName : null;
        }
    }

    /// <summary>
    /// 挑选出来的书本
    /// </summary>
    public List<BusquedaBookInfo> MyPickBook
    {
        get
        {
            return myPickBook != null ? myPickBook : null;
        }set
        {
            myPickBook = value;
        }
    }
    



}
