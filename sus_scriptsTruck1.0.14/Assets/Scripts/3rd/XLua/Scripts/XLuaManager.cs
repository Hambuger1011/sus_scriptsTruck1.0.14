using AB;
using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UGUI;
using UnityEngine;
using UnityEngine.SceneManagement;
using XLua;
using ResMgr = AB.ABSystem;

[Hotfix]
[LuaCallCSharp]
public class XLuaManager : MonoBehaviour
{
    public bool isDebug = false;
    public static XLuaManager Instance { get; private set; }

    LuaEnv luaEnv = null;
    LuaUpdater luaUpdater = null;

    [System.NonSerialized]
    public List<string> searchPaths = new List<string>();

    LuaBundle luaBundle;


#if ENABLE_DEBUG
    HashSet<string> loaded = new HashSet<string>();
#endif

    public bool HasGameStart
    {
        get;
        protected set;
    }

    void Awake()
    {
        Instance = this;

#if !UNITY_EDITOR
        isDebug = false;
#endif
#if UNITY_5_4_OR_NEWER
        SceneManager.sceneLoaded += OnSceneLoaded;
#endif
    }


    void OnDestroy()
    {
#if UNITY_5_4_OR_NEWER
        SceneManager.sceneLoaded -= OnSceneLoaded;
#endif 
        //Dispose();
    }


    private void Update()
    {
        if (luaEnv != null)
        {
            luaEnv.Tick();

            if (Time.frameCount % 100 == 0)
            {
                luaEnv.FullGc();
            }
        }
    }

#if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
#else
    private void OnLevelWasLoaded()
#endif
    {
        if (luaEnv != null && HasGameStart)
        {
            Execute("GameMain.OnLevelWasLoaded()","XLuaManager");
        }
    }

    private void OnApplicationQuit()
    {
        if (luaEnv != null && HasGameStart)
        {
            Execute("GameMain.OnApplicationQuit()", "XLuaManager");
        }
    }

    void Dispose()
    {
        if (luaUpdater != null)
        {
            luaUpdater.OnDispose();
        }
        if (luaEnv != null)
        {
            try
            {
                luaEnv.Dispose();
                luaEnv = null;
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("xLua exception : {0}\n {1}", ex.Message, ex.StackTrace);
                Debug.LogError(msg, null);
            }
        }
    }


    public LuaEnv GetLuaEnv()
    {
        return luaEnv;
    }

    void InitLuaEnv()
    {
        luaEnv = new LuaEnv();
        HasGameStart = false;
        if (luaEnv != null)
        {
            this.searchPaths.Clear();
            this.searchPaths.Add("");
            luaEnv.AddLoader(CustomLoader);
            luaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
            luaEnv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
            luaEnv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);
            luaEnv.AddBuildin("ffi", XLua.LuaDLL.Lua.LoadFFI);

            LoadScript("GameMain");
            luaUpdater = gameObject.GetComponent<LuaUpdater>();
            if (luaUpdater == null)
            {
                luaUpdater = gameObject.AddComponent<LuaUpdater>();
            }
            luaUpdater.OnInit(luaEnv);
            HasGameStart = true;
        }
        else
        {
            Debug.LogError("InitLuaEnv null!!!");
        }
    }

    // 重启虚拟机：热更资源以后被加载的lua脚本可能已经过时，需要重新加载
    // 最简单和安全的方式是另外创建一个虚拟器，所有东西一概重启
    public void Restart()
    {
        Dispose();
        InitLuaEnv();
    }

    public void Execute(string scriptContent, string chunkName = "chunk", LuaTable env = null)
    {
        if (luaEnv != null)
        {
            try
            {
                luaEnv.DoString(scriptContent,chunkName, env);
            }
            catch (System.Exception ex)
            {
                string msg = string.Format("Lua程序执行错误:\n {0}\n {1}", ex.Message, ex.StackTrace);
                Debug.LogError(msg);
            }
        }
    }


    public void ReloadScript(string scriptName)
    {
        Execute(string.Format("package.loaded['{0}'] = nil", scriptName), "XLuaManager");
        LoadScript(scriptName);
    }

    void LoadScript(string scriptName)
    {
        Execute(string.Format("require('{0}')", scriptName), "XLuaManager");
    }

    public byte[] CustomLoader(ref string filepath)
    {
        //Debug.LogError("CustomLoader:" + filepath+","+ this.isDebugMode);
        //        string scriptPath = string.Empty;
        //        filepath = filepath.Replace(".", "/") + ".lua";
        //#if UNITY_EDITOR
        //        if (!ResMgr.Instance.isUseAssetBundle)
        //        {
        //            scriptPath = Path.Combine(Application.dataPath, luaScriptsFolder);
        //            scriptPath = Path.Combine(scriptPath, filepath);
        //            //Logger.Log("Load lua script : " + scriptPath);
        //            return GameUtility.SafeReadAllBytes(scriptPath);
        //        }
        //#endif

        //        scriptPath = string.Format("{0}/{1}.bytes", luaAssetbundleAssetName, filepath);
        //        string assetbundleName = null;
        //        string assetName = null;
        //        bool status = AssetBundleManager.Instance.MapAssetPath(scriptPath, out assetbundleName, out assetName);
        //        if (!status)
        //        {
        //            Debug.LogError("MapAssetPath failed : " + scriptPath);
        //            return null;
        //        }
        //        var asset = AssetBundleManager.Instance.GetAssetCache(assetName) as TextAsset;
        //        if (asset != null)
        //        {
        //            //Logger.Log("Load lua script : " + scriptPath);
        //            return asset.bytes;
        //        }
        //        Debug.LogError("Load lua script failed : " + scriptPath + ", You should preload lua assetbundle first!!!");
        //        return null;



        byte[] code = null;

        //using (zstring.Block())
        //{
        //    uiText1.text = (zstring)"hello world" + " you";
        //    uiText2.text = zstring.format("{0},{1}", "hello", "world");
        //}
#if ENABLE_DEBUG || UNITY_EDITOR
        if (this.isDebug || !ABSystem.Instance.isUseAssetBundle)
#else
        if(false)
#endif
        {

            using (CString.Block())
            {
                CString sb = CString.Alloc(256);
                for (int i = 0, iMax = searchPaths.Count; i < iMax; ++i)
                {
                    sb.Clear();
                    sb.Append("assets/scripts/lua/");
                    sb.Append(searchPaths[i]);
                    sb.Append(filepath);
                    sb.Replace(".", "/");
                    sb.Append(".lua");
                    string strVal = sb.ToString();
                    if (File.Exists(strVal))
                    {
                        code = File.ReadAllBytes(strVal);
                        break;
                    }
                }
            }
        }
        else
        {
            using (CString.Block())
            {
                CString sb = CString.Alloc(256);
                for (int i = 0, iMax = searchPaths.Count; i < iMax; ++i)
                {
                    sb.Clear();
                    sb.Append(searchPaths[i]);
                    sb.Append(filepath);
                    sb.Replace(".", "/");
                    sb.Append(".lua");
                    sb.ToLower();
                    string strVal = sb.ToString();
#if false
                    var asset = ResMgr.ui.bundle.LoadImme(AbTag.Lua, enResType.eText,strVal);//this.luaBundle[strVal];
                    if (asset != null)
                    {
                        code = asset.resTextAsset.bytes;
                        break;
                    }
#else
                    code = this.luaBundle.Pop(strVal);
                    if(code != null)
                    {
                        break;
                    }
#endif
                }
            }
        }
        if (code == null)
        {
            Debug.LogError("ab未发现lua文件:" + filepath);
        }else
        {

#if ENABLE_DEBUG
            loaded.Add(filepath);
#endif
            //Debug.LogError("加载lua文件:" + filepath);
        }
        return code;
    }


    public bool mIsStartup = false;
    public void Startup()
    {
        Debug.Log("XLuaManager 开始启动Xlua + curtime：" + DateTime.Now);
        if (mIsStartup)
        {
            LOG.Error("重复Startup!!!!!");
            return;
        }
        mIsStartup = true;
        UberLogger.Logger.ForwardMessages = GameFrameworkImpl.Instance.debugForwardToUnity;
        LOG.Info("xlua startup");
#if ENABLE_DEBUG
        UnityEngine.Debug.unityLogger.filterLogType = LogType.Log;
#else
        UnityEngine.Debug.unityLogger.filterLogType = LogType.Warning;
#endif
        TalkingDataManager.Instance.OpenApp(EventEnum.LoadConfigStart);

        LaunchLoadingForm uiLoading = null;
        if (!XLuaHelper.isHotUpdate)
        {
            GamePointManager.Instance.BuriedPoint(EventEnum.LoadConfig);
            CUIManager.Instance.OpenForm(UIFormName.LoadingForm);
            uiLoading = CUIManager.Instance.GetForm<LaunchLoadingForm>(UIFormName.LoadingForm);
            uiLoading.CurLoadContInfoTxt.text = "config loading.....";
            uiLoading.VersionInfoTxt.text =  "V "+SdkMgr.Instance.GameVersion();
        }

        ABSystem.ui.LoadBundleConfig((bSuc) =>
        {
            if (!bSuc)
            {
                //UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218).text.ToString()/*"TIPS"*/, GameDataMgr.Instance.table.GetLocalizationById(219).text.ToString() /*"Download Config Error!"*/);
                UIAlertMgr.Instance.Show("TIPS", CTextManager.Instance.GetText(276));//资源未加载，不能读取配置表
                TalkingDataManager.Instance.OpenApp(EventEnum.LoadConfigResultFail);
                return;
            }
#if ENABLE_DEBUG
            ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(CUIID.Canvas_Debug)).LoadAsync(AbTag.Debug, enResType.ePrefab, CUIID.Canvas_Debug, (_) =>
            {
                Tiinoo.DeviceConsole.DeviceConsoleLoader.Load();
                if (GameFrameworkImpl.Instance.isShowDebugPanel)
                {
                    CUIManager.Instance.OpenForm(CUIID.Canvas_Debug, true, false);
                }
            });
#endif
            if (!XLuaHelper.isHotUpdate)
            {
                if (uiLoading != null && uiLoading.MiniBarGroup != null)
                    uiLoading.SetMiniProgressStage(false);
            }
           

            TalkingDataManager.Instance.OpenApp(EventEnum.LoadConfigResultSucc);
            GamePointManager.Instance.BuriedPoint(EventEnum.LoadConfigSuccess);
            if (!ABSystem.Instance.isUseAssetBundle)
            {
                this.InitLuaEnv();
                Execute("GameMain.Startup()", "XLuaManager");
            }
            else
            {
                TalkingDataManager.Instance.OpenApp(EventEnum.LoadLuaStart);
                GamePointManager.Instance.BuriedPoint(EventEnum.LoadLua);
                var task = ABSystem.Instance.LoadAsync(AbTag.Lua, enResType.eScriptableObject, "Assets/Bundle/lua/Lua.asset");
                //this.StartCoroutine(OnUpdateProgress(task));
                task.AddCall((asset) =>
                {
                    this.luaBundle = asset.resObject as LuaBundle;
                    if (this.luaBundle == null)
                    {
                        TalkingDataManager.Instance.OpenApp(EventEnum.LoadLuaResultFail);
                        LOG.Error("加载lua脚本失败");
                        UIAlertMgr.Instance.Show("TIPS", "Download Resources Error!");
                    }
                    else
                    {
                        TalkingDataManager.Instance.OpenApp(EventEnum.LoadLuaResultSucc);
                        GamePointManager.Instance.BuriedPoint(EventEnum.LoadLuaSuccess);
                        this.InitLuaEnv();
                        Execute("GameMain.Startup()", "XLuaManager");
                    }
                });
            }
        }, (progress) =>
        {
            if (!XLuaHelper.isHotUpdate)
            {
                if (uiLoading != null && uiLoading.MiniBarCurProgress != null && progress >= 0)
                    uiLoading.MiniBarCurProgress.fillAmount = progress;
            }
            //LOG.Error("---curProgress--->" + progress);
        });
    }

    IEnumerator OnUpdateProgress(CAsset asset)
    {
        var uiLoading = CUIManager.Instance.GetForm<LaunchLoadingForm>(UIFormName.LoadingForm);
        while (!asset.IsDone())
        {
            uiLoading.SetProgress(asset.Progress(), asset.GetAllSize(), true);
            yield return null;
        }
        uiLoading.SetProgress(0, asset.GetAllSize(), true);
        this.luaBundle = asset.resObject as LuaBundle;
        if (this.luaBundle == null)
        {
            LOG.Error("加载lua脚本失败");
            UIAlertMgr.Instance.Show("TIPS", "Download Resources Error!");
        }
        else
        {
            this.InitLuaEnv();
            Execute("GameMain.Startup()", "XLuaManager");
        }
    }

    [ContextMenu("Reload")]
    public void Reload()
    {
#if ENABLE_DEBUG
        using (LuaTable tb = luaEnv.NewTable())
        {
            int i = 0;
            foreach(var name in loaded)
            {

                tb.Set(++i, name);
            }
            loaded.Clear();
            var func = luaEnv.Global.Get<Action<object>>("Reload");
            func(tb);
        }
#endif
    }

    public void TestHotfix()
    {
        throw new Exception("Xlua Hotfix Fail!!!");
    }


    /// <summary>
    /// 调用Lua脚本 全局方法
    /// </summary>
    /// <param name="脚本名称"></param>
    /// <param name="方法名"></param>
    /// <param name="参数"></param>
    public void CallFunction(string script,string funcName, params object[] args)
    {
        LuaTable scriptEnv = luaEnv.Global.Get<LuaTable>(script);

        if (scriptEnv!=null)
        {
            Action<object> action = scriptEnv.GetInPath<Action<object>>(funcName);
            if (action != null)
            {
                action(args);
            }
        }
    }

}
