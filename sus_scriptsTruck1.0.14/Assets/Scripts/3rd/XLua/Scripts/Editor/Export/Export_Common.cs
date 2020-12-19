using AB;
using Candlelight.UI;
using Mopsicus.Plugins;
using Spine.Unity;
using System;
using System.Collections.Generic;
using GameCore.UI;
using UGUI;
using UnityEngine;
using UnityEngine.Events;

using XLua;

public static class Export_Cmmon
{
#if true//!HOTFIX_ENABLE

    /// <summary>
    /// C#导出函数到lua
    /// </summary>
    [LuaCallCSharp]
    [ReflectionUse]
    public static List<Type> export_cs_2_lua = new List<Type>()
    {
        //typeof(GameCore.LuaHelper),
        typeof(Action<AbTask>),
        typeof(Action<CAsset>),
        typeof(Action<CBundle>),
        typeof(AB.ABMgr),
        typeof(AB.ABSystem),
        typeof(Utility),
        typeof(XlsxData),
        typeof(UIEventListener),
        typeof(CUIUtility),
        typeof(MyUtils),
        typeof(Dictionary<string,string>),
        typeof(Dictionary<string,object>),
        typeof(SkeletonGraphic),
        typeof(BoneFollowerGraphic),
        typeof(Func<Vector2>),
        typeof(HyperText),
        typeof(NativeGallery),
        typeof(WWWForm),
        typeof(ImageConversion),
        typeof(MobileInput),
        typeof(MobileInputField),
        typeof(CAssetAsync),
        typeof(CAsset),
        typeof(GoogleAdmobAds),
        typeof(DisplayUtil),
        typeof(Dispatcher),
        typeof(BookItemManage),
        typeof(AppRatinglevel),
        typeof(SuperScrollView.LoopGridView),
        typeof(SuperScrollView.LoopGridViewItem),
        typeof(SuperScrollView.LoopListView2),
        typeof(SuperScrollView.LoopListViewItem2),
        typeof(UIVirtualList),
        typeof(ContentImmediate),

    };

    //导出+热补丁
    [LuaCallCSharp,Hotfix]
    [ReflectionUse]
    public static List<Type> export_hotfix = new List<Type>()
    {
        typeof(BookReadingWrapper),
        typeof(UserDataManager),
        typeof(GameHttpNet),
        typeof(GameUtility),
        typeof(JsonHelper),
        typeof(HyperText),
    };
#else

    static List<Type> _cs_2_lua;
    static List<Type> _hotfix_cs;
    /// <summary>
    /// C#导出函数到lua
    /// </summary>
    [LuaCallCSharp]
    [ReflectionUse]
    public static List<Type> export_cs_2_lua
    {
        get
        {
            if (_cs_2_lua == null)
            {
                _cs_2_lua = new List<Type>()
                {
                    typeof(Dictionary<string,string>),
                    typeof(Dictionary<string,object>),
                    typeof(Component),
                    typeof(Array),
                    typeof(IList),
                    typeof(IDictionary),
                    typeof(Action),
                    typeof(Action<float>),
                    typeof(Action<float, float>),
#if UNITY_IOS
                    typeof(IOSUtils),
#endif
#if UNITY_ANDROID
                    typeof(AndroidUtils),
#endif
                };
                _cs_2_lua.AddRange(hotfix_cs);
                var assemblies = new Assembly[]
                {
                    Assembly.Load("UnityEngine"),
                    Assembly.Load("UnityEngine.UI"),
                    //Assembly.Load("Assembly-CSharp-firstpass"),
                };
                foreach (var assembly in assemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsInterface)
                        {
                            continue;
                        }
                        if (typeof(System.Delegate).IsAssignableFrom(type) || typeof(GraphicRebuildTracker).IsAssignableFrom(type))
                        {
                            continue;
                        }
                        if (type.Name.Contains("<"))
                        {
                            continue;
                        }
                        _cs_2_lua.Add(type);
                    }
                }
                Debug.Log("Export Types:" + _cs_2_lua.Count);
            }
            return _cs_2_lua;
        }
    }


    [Hotfix]
    [ReflectionUse]
    public static List<Type> hotfix_cs
    {
        get
        {
            if (_hotfix_cs == null)
            {
                var assembly = Assembly.Load("Assembly-CSharp");
                var types = assembly.GetTypes();
                _hotfix_cs = new List<Type>(types.Length / 10);
                foreach (var type in types)
                {
                    if (typeof(System.Delegate).IsAssignableFrom(type)
                        || typeof(System.Collections.IEnumerator).IsAssignableFrom(type)
                        || type == typeof(IOSUtils)
                        || type == typeof(AndroidUtils)
                        || typeof(IGCTools).IsAssignableFrom(type)
                       )
                    {
                        continue;
                    }
                    if (type.IsInterface)
                    {
                        continue;
                    }
                    if (type.IsSubclassOf(typeof(UnityEditor.Editor))
                        || type.IsSubclassOf(typeof(PropertyDrawer))
                        )
                    {
                        continue;
                    }
                    if (type.Namespace != null && (
                        type.Namespace.StartsWith("XLua")
                        || type.Namespace.StartsWith("ProtoBuf")
                        || type.Namespace.StartsWith("LZ4Sharp")

                       ))
                    {
                        continue;
                    }
                    if (type.Name.Contains("<"))
                    {
                        continue;
                    }

                    _hotfix_cs.Add(type);
                }
                Debug.Log("Hotfix Types:" + _hotfix_cs.Count);
            }
            return _hotfix_cs;
        }
    }
#endif

    /// <summary>
    /// lua导出函数到c#，一般用于lua回调当参数传递到c#
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
        typeof(Action<Vector2>),
        typeof(Action<object>),
        typeof(Action<object,object>),
        typeof(Action<float>),
        typeof(Action<float, float>),
        typeof(Action<bool, float>),
        typeof(Action<int, UnityObjectRefCount>),
        typeof(Action<LuaTable>),
        typeof(UIVoidPointerEvent),
        typeof(Func<string, bool, UnityEngine.Sprite>),
        typeof(Func<object,object>),
        typeof(UnityEvent),
        typeof(Dictionary<string,string>),
        typeof(Dictionary<string,object>),
        typeof(Type),
        typeof(Action<Notification>),
        typeof(Func<Vector2>),
        typeof(HyperText),
        typeof(System.Func<StoryEditor.UIBubbleLayout, int, StoryEditor.UIBubbleItem>),
        typeof(Func<SuperScrollView.LoopGridView,int,int,int,SuperScrollView.LoopGridViewItem>),
        typeof(Func<SuperScrollView.LoopListView2,int,SuperScrollView.LoopListViewItem2>),
        typeof(Action<UIVirtualList.Row>),
        typeof(UnityEvent<string>),
    };
    
    
    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
        new List<string>(){ "UnityEngine.UI.Graphic", "OnRebuildRequested"},
        new List<string>(){ "UnityEngine.UI.Text", "OnRebuildRequested"},
        new List<string>(){ "Candlelight.UI.HyperText", "OnRebuildRequested"},
        new List<string>(){ "PurchaseManager", "Purchase","System.String","System.String","System.Action`2[System.Boolean,System.String]"},
        new List<string>(){ "PurchaseManager", "RequestProductList","System.String"},
    };
}
