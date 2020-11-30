using AB;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using XLua;

public static class Export_GameCore
{
    /// <summary>
    /// C#导出函数到lua
    /// </summary>
    [LuaCallCSharp, Hotfix]
    [ReflectionUse]
    public static List<Type> export_cs_2_lua = new List<Type>()
    {
        typeof(AssetLoader),
        typeof(AssetLoader_AB),
        typeof(AB.AssetLoader_Resources),
        typeof(AB.ABLoader_File),
        typeof(AB.ABLoader),
        typeof(AB.ABLoader_WebClient),
        typeof(AB.ABLoader_WebUnity),
        typeof(AB.AbTask),
        typeof(AB.AbWork),
        typeof(AB.CAsset),
        typeof(AB.CBundle),
        typeof(AB.ABMgr),
        typeof(AB.AbResBundle),
        typeof(AB.AbResConfig),
        typeof(AB.AbUtility),
        typeof(AB.ABSystem),
        typeof(AB.AbUISystem),

        typeof(AB.AbAtlas),
        typeof(AB.AbAtlasMgr),
        typeof(CTimerManager),
        typeof(CTimer),
        typeof(GameUtility),
        typeof(Utility),
        typeof(CUIManager),
        typeof(CUIForm),
        typeof(UniHttp)
    };


    /// <summary>
    /// lua导出函数到c#，一般用于回调
    /// </summary>
    [CSharpCallLua]
    [ReflectionUse]
    public static List<Type> export_lua_2_cs = new List<Type>()
    {
        typeof(Action<AbTask>),
        typeof(Action<CAsset>),
        typeof(Action<CBundle>),
        typeof(Action<bool>),
        typeof(Action<int>),
        typeof(Action<string>),
        typeof(Action<object>),
    };
    

    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {

                new List<string>(){ "AB.ABSystem", "asyncBundleList"},
                new List<string>(){ "AB.ABSystem", "asyncAssetList"},
                new List<string>(){ "AB.ABSystem", "loadCount"},
                new List<string>(){ "AB.ABSystem", "AddAsyncList", "System.String"},
                new List<string>(){ "AB.ABSystem", "RemoveAsyncList", "System.String"},
                new List<string>(){ "AB.ABSystem", "LoadAssetInEditor", "System.String", "System.Type"},

                new List<string>(){ "AB.ABMgr", "AddAsyncList", "System.String"},
                new List<string>(){ "AB.ABMgr", "RemoveAsyncList", "System.String"},
                new List<string>(){ "AB.ABMgr", "LoadAssetInEditor", "System.String", "System.Type"},
                new List<string>(){ "AB.AbTask", "lastName"},


                new List<string>(){ "AB.AbAtlas", "UpdateSprites"},
                new List<string>(){ "AB.AbAtlas", "LoadInEditor", "System.String"},
                new List<string>(){ "AB.AbAtlas", "SaveInEditor"},

                new List<string>(){ "AB.AbResConfig", "UpdateLastModify"},
                new List<string>(){ "AB.AbResConfig", "SaveInEditor"},
                new List<string>(){ "AB.AbResConfig", "LoadInEditor", "System.String"},
                new List<string>(){ "UIDepth", "sorderInLayer"},
            };
}
