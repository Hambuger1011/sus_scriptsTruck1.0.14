using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
//热更新时候需要打上 Hotfix 和 LuaCallCSharp 这两个标签
[Hotfix]
[LuaCallCSharp]
public class LuaTestForm : BaseUIForm
{ 
    public override void OnOpen()
    {
        base.OnOpen();

        //LuaHitFixeTest();
        LuaHitPrivate();

        //InvokeRepeating("LuaHitPrivate", 0, 1);
    }

    public override void OnClose()
    {
        base.OnClose();
    }
    //public static LuaTestForm Instance { get; private set; }

    //public void Awake()
    //{
    //    Instance = this;
    //    LuaHitFixeTest();
       
    //}
    
    /// <summary>
    /// 这里是测试lua热更
    /// </summary>
    public void LuaHitFixeTest()
    {
      
        LOG.Error("LuaTest测试C#返回=========================================");
    }

    private void LuaHitPrivate()
    {
        LOG.Error("LuaTest测试C#返回私有方法=========================================");

        string name = transform.name;
        Text st = transform.Find("Canvas").GetComponent<Text>();

        transform.localPosition = new Vector3(0, 0, 0);      
    }
}
