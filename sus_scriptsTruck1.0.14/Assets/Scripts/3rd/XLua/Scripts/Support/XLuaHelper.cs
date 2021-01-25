using AB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IGG.SDK.Modules.AppRating;
using pb;
using Script.Game.Helpers;
using UGUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using XLua;
using Object = UnityEngine.Object;

public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

[LuaCallCSharp]
public static class XLuaHelper
{
    public static void AddSearchPath(string path, bool front = false)
    {
        XLuaManager.Instance.searchPaths.Add(path);
    }
    public static void RemoveSearchPath(string path)
    {
        XLuaManager.Instance.searchPaths.Remove(path);
    }

    // 说明：扩展NGUITools.AddMissingComponent方法
    public static Component AddMissingComponent(GameObject go, Type cmpType)
    {
        Component comp = go.GetComponent(cmpType);
        if (comp == null)
        {
            comp = go.AddComponent(cmpType);
        }
        return comp;
    }

    // 说明：扩展CreateInstance方法
    public static Array CreateArrayInstance(Type itemType, int itemCount)
    {
        return Array.CreateInstance(itemType, itemCount);
    }

    public static IList CreateListInstance(Type itemType)
    {
        return (IList)Activator.CreateInstance(MakeGenericListType(itemType));
    }

    public static IDictionary CreateDictionaryInstance(Type keyType, Type valueType)
    {
        return (IDictionary)Activator.CreateInstance(MakeGenericDictionaryType(keyType, valueType));
    }

    // 说明：创建委托
    // 注意：重载函数的定义顺序很重要：从更具体类型（Type）到不具体类型（object）,xlua生成导出代码和lua侧函数调用匹配时都是从上到下的，如果不具体类型（object）写在上面，则永远也匹配不到更具体类型（Type）的重载函数，很坑爹
    public static Delegate CreateActionDelegate(Type type, string methodName, params Type[] paramTypes)
    {
        return InnerCreateDelegate(MakeGenericActionType, null, type, methodName, paramTypes);
    }

    public static Delegate CreateActionDelegate(object target, string methodName, params Type[] paramTypes)
    {
        return InnerCreateDelegate(MakeGenericActionType, target, null, methodName, paramTypes);
    }

    public static Delegate CreateCallbackDelegate(Type type, string methodName, params Type[] paramTypes)
    {
        return InnerCreateDelegate(MakeGenericCallbackType, null, type, methodName, paramTypes);
    }

    public static Delegate CreateCallbackDelegate(object target, string methodName, params Type[] paramTypes)
    {
        return InnerCreateDelegate(MakeGenericCallbackType, target, null, methodName, paramTypes);
    }

    delegate Type MakeGenericDelegateType(params Type[] paramTypes);
    static Delegate InnerCreateDelegate(MakeGenericDelegateType del, object target, Type type, string methodName, params Type[] paramTypes)
    {
        if (target != null)
        {
            type = target.GetType();
        }

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        MethodInfo methodInfo = (paramTypes == null || paramTypes.Length == 0) ? type.GetMethod(methodName, bindingFlags) : type.GetMethod(methodName, bindingFlags, null, paramTypes, null);
        Type delegateType = del(paramTypes);
        return Delegate.CreateDelegate(delegateType, target, methodInfo);
    }

    // 说明：构建List类型
    public static Type MakeGenericListType(Type itemType)
    {
        return typeof(List<>).MakeGenericType(itemType);
    }

    // 说明：构建Dictionary类型
    public static Type MakeGenericDictionaryType(Type keyType, Type valueType)
    {
        return typeof(Dictionary<,>).MakeGenericType(new Type[] { keyType, valueType });
    }

    // 说明：构建Action类型

    public static Type MakeGenericActionType(params Type[] paramTypes)
    {
        if (paramTypes == null || paramTypes.Length == 0)
        {
            return typeof(Action);
        }
        else if (paramTypes.Length == 1)
        {
            return typeof(Action<>).MakeGenericType(paramTypes);
        }
        else if (paramTypes.Length == 2)
        {
            return typeof(Action<,>).MakeGenericType(paramTypes);
        }
        else
        {
            return typeof(Action<,,,>).MakeGenericType(paramTypes);
        }
    }

    // 说明：构建Callback类型
    public static Type MakeGenericCallbackType(params Type[] paramTypes)
    {
        if (paramTypes == null || paramTypes.Length == 0)
        {
            return typeof(Callback);
        }
        else if (paramTypes.Length == 1)
        {
            return typeof(Callback<>).MakeGenericType(paramTypes);
        }
        else if (paramTypes.Length == 2)
        {
            return typeof(Callback<,>).MakeGenericType(paramTypes);
        }
        else
        {
            return typeof(Callback<,,>).MakeGenericType(paramTypes);
        }
    }




    #region Component
    public static Component GetComponent(Transform tf, Type type)
    {
        var c = tf.GetComponent(type);
        return c;
    }

    public static Component GetComponent(Transform tf, string name, Type type)
    {
        var node = tf.Find(name);
        if (node == null)
        {
            return null;
        }
        var c = node.GetComponent(type);
        return c;
    }
    #endregion

    public static GameObject Instantiate(GameObject pfb, Transform parent)
    {
        var go = GameObject.Instantiate(pfb, parent, false);
        return go;
    }

    public static void AddClick(Button btn, UnityAction func)
    {
        btn.onClick.AddListener(func);
    }

    public static Color ParseHtmlString(string value)
    {
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(value, out color);
        return color;
    }

    public static Vector2 GetSpriteSize(Sprite spt)
    {
        if (spt == null)
        {
            return Vector2.zero;
        }
        var size = spt.rect.size / spt.pixelsPerUnit;
        return size;
    }

    public static Type GetType(string assemblyName, string typeName)
    {
        var t = Type.GetType(typeName + "," + assemblyName);
        return t;
    }

    public static AbAtlas GetAtlas(string refTag, enResType resType, string strAssetName)
    {
        AbAtlas atlas;
        var asset = ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(strAssetName)).LoadImme(refTag, resType, strAssetName);
        atlas = asset.Get<AbAtlas>();
        return atlas;
    }

    public static Sprite LoadSprite(string filename, bool readable = false)
    {
        if (!File.Exists(filename))

        {
            return null;
        }

        var buffer = File.ReadAllBytes(filename);
        if(buffer.Length == 0)
        {
            return null;
        }
        VInt2 size = new VInt2(610, 300);
        var texture2d = new Texture2D(size.x, size.y);
        texture2d.LoadImage(buffer, !readable);
        //texture2d.Apply();
        Sprite spt = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
        spt.name = filename;
        return spt;
    }

    public static void LoadSpriteAsync(string filename, Action<Sprite> callback, bool readable = false)
    {
        if (!File.Exists(filename))
        {
            callback(null);
            return;
        }
        var webRequest = UnityWebRequest.Get("file://" + filename);
        DownloadHandlerTexture texHandle = new DownloadHandlerTexture(readable);
        webRequest.downloadHandler = texHandle;
        var requestOperation = webRequest.SendWebRequest();
        requestOperation.completed += (opt) =>
        {
            //Debug.LogError(opt);
            if (opt.isDone)
            {
                try
                {
                    /*这里把贴图转换的过程放到了后台线程来完成，
                    并且相比于直接脚本加载图片做了优化，减少了内存分配
                    */
                    Texture2D texture2d = DownloadHandlerTexture.GetContent(webRequest);
                    Sprite spt = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                    callback(spt);
                }
                catch(Exception ex)
                {
                    Debug.LogError("加载图片失败:"+ filename+"\n"+ex);
                    callback(null);
                }
            }
            else
            {
                callback(null);
            }
        };
    }

    public static void UnloadSprite(Sprite spt)
    {
        if (!spt)
        {
            return;
        }
        var texture2d = spt.texture;
        Object.Destroy(spt);
        Object.Destroy(texture2d);
    }


    public static UnityWebRequestAsyncOperation UploadImage(string url, WWWForm form, Action<UnityWebRequest> callback)
    {
        string userToken = GameHttpNet.Instance.TOKEN;
        var webRequest = UnityWebRequest.Post(url, form);
        //webRequest.SetRequestHeader("Content-Type", "multipart/form-data");
        webRequest.SetRequestHeader("token", userToken);


        UploadHandler uploadHandler = new UploadHandlerRaw(form.data);
        webRequest.uploadHandler = uploadHandler;
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        var requestOperation = webRequest.SendWebRequest();
        requestOperation.completed += (opt) =>
        {
            //Debug.LogError(opt);//UnityWebRequestAsyncOperation
            if (opt.isDone)
            {
                callback(webRequest);
            }
            else
            {
                callback(webRequest);
            }
        };
        return requestOperation;
    }


    public static UnityWebRequestAsyncOperation DownloadSprite(string url, Action<Sprite> callback)
    {
        var webRequest = UnityWebRequest.Get(url);
        DownloadHandlerTexture texHandle = new DownloadHandlerTexture(true);
        webRequest.downloadHandler = texHandle;
        var requestOperation = webRequest.SendWebRequest();
        requestOperation.completed += (opt) =>
        {
            if (webRequest.isDone)
            {
                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    Debug.LogError("[E]DownloadSprite:" + webRequest.error);
                    callback(null);
                }
                else
                {
                    var texture2d = texHandle.texture;
                    Sprite spt = Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f));
                    callback(spt);
                }
            }
            else
            {
                callback(null);
            }
        };
        return requestOperation;
    }

    public static void SetUISize(RectTransform trans, float x, float y)
    {
        trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, x);
        trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, y);
    }

#if false
    public static UnityWebRequestAsyncOperation DownloadFile(string url, string filename, Action<bool> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            LOG.Error("url is empty："+ filename);
            callback(false);
            return null;
        }
        if (string.IsNullOrEmpty(filename))
        {
            LOG.Error("filename is empty："+ url);
            callback(false);
            return null;
        }
        var webRequest = UnityWebRequest.Get(url);
        //webRequest.redirectLimit = 0;
        var downloadHandler = new DownloadHandlerFile(filename);
        webRequest.downloadHandler = downloadHandler;
        var requestOperation = webRequest.SendWebRequest();
        requestOperation.completed += (opt) =>
        {
            if (webRequest.isDone)
            {
                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    Debug.LogError("[E]DownloadFile:" + webRequest.error + "\n" + url + "\n" + filename);
                    callback(false);
                }
                else if(webRequest.downloadedBytes > 0)
                {
                    Debug.LogError("[+]DownloadFile:" + webRequest.url+"\n"+ url+"\n"+filename);
                    webRequest.Dispose();
                    callback(true);
                }else
                {
                    callback(false);
                }
            }
            else
            {
                callback(false);
            }
        };
        return requestOperation;
    }
#else
    public static DownloadMgr.Task DownloadFile(string url, string filename, Action<bool> callback)
    {
        if (string.IsNullOrEmpty(url))
        {
            LOG.Error("url is empty：" + filename);
            callback(false);
            return null;
        }
        if (string.IsNullOrEmpty(filename))
        {
            LOG.Error("filename is empty：" + url);
            callback(false);
            return null;
        }
        var task = DownloadMgr.Instance.Download(url, filename, UserDataManager.Instance.ResVersion, 0, true,false);
        task.AddComplete(()=>
        {
            if (task.isDone)
            {
                callback(true);
            }
            else
            {
                Debug.LogError("[E]DownloadFile:" + task.strError + "\n" + url + "\n" + filename);
                callback(false);
            }
        });
        return task;
    }
#endif



    //public static Vector2 SetSprite(Image uiImage, Sprite sprite, Vector2 defaultSize)
    //{
    //    var size = defaultSize;
    //    uiImage.sprite = sprite;
    //    //获取image大小
    //    if (sprite == null)
    //        size = defaultSize;
    //    else
    //    {
    //        var imgSize = sprite.rect.size;
    //        imgSize = new Vector2(imgSize.x, imgSize.y) / uiImage.pixelsPerUnit;//图片真实分辨率
    //        var imgRatio = (imgSize.x / imgSize.y);
    //        var ratio = (defaultSize.x / defaultSize.y);
    //        if (imgRatio > ratio)
    //        {

    //            imgSize.x = defaultSize.x;
    //            imgSize.y = defaultSize.x / imgRatio;
    //        }
    //        else
    //        {

    //            imgSize.y = defaultSize.y;
    //            imgSize.x = defaultSize.y * imgRatio;
    //        }
    //        size = imgSize;
    //    }
    //    var t = uiImage.rectTransform;
    //    SetUISize(t, size.x, size.y);
    //    return size;
    //}

    public static void ParseImageUrl(string url,out string imgUrl, out string md5)
    {
        imgUrl = "";
        md5 = "";
        if (!string.IsNullOrEmpty(url))
        {
            var idx = url.LastIndexOf('?');
            if(idx > -1)
            {
                imgUrl = url.Substring(0, idx);
                md5 = url.Substring(idx + 1);
            }else
            {
                imgUrl = url;
                md5 = "";
            }
        }

    }

    public static Texture2D Texture2dClipper(Texture2D src,int x, int y, int w, int h)
    {
        if (x < 0)
        {
            x = 0;
        }
        if(w < 0)
        {
            w = 1;
        }
        if(h < 0)
        {
            h = 1;
        }
        var dst = new Texture2D(w,h);
        var pixels = src.GetPixels(x, y, w, h);
        dst.SetPixels(pixels);
        dst.Apply();
        return dst;
    }

    public static string WriteTexture2D(string dir,Texture2D src)
    {
        var buff = src.EncodeToJPG();
        var md5 = CFileManager.GetMd5(buff);
        var filename = dir + "/" + md5+".png";
        File.WriteAllBytes(filename, buff);
        return md5;
    }

    public static bool isHotUpdate = false;

    /// <summary>
    /// 初始化评星系统
    /// </summary>
    public static void initAppRating()
    {
        // Debug.LogError("CTimerManager:CTimerManager");

        if (UserDataManager.Instance.userInfo != null)
        {
            //是否已经评星过     0：否   1：是
            int IsRatingLevel = UserDataManager.Instance.userInfo.data.userinfo.is_store_score;

            //Debug.LogError("is_store_score:is_store_score:" + IsRatingLevel);
            if (IsRatingLevel == 0)
            {
                Debug.Log("是否已经评星过  0：否 ");
                //根据IGG后台Appconf 判断 是否开启评星
                IGG.SDK.Modules.AppConf.VO.IGGAppRatingStatus ratingstate = IGGSDKManager.Instance.GetRatingStatus();

                if (ratingstate != null)
                {
                    Debug.Log("IGG后台Appconf：当前评星状态是" + ratingstate.GetMode());
                    if (ratingstate.GetMode() != IGG.SDK.Modules.AppConf.VO.IGGAppRatingMode.Disable)  //当评星状态不是 Disable
                    {
#if !UNITY_EDITOR && UNITY_ANDROID
                            //当简易模式时  安卓平台 不进入简易模式
                            if (ratingstate.GetMode() == IGG.SDK.Modules.AppConf.VO.IGGAppRatingMode.Minimized) { return; }
#endif
     
                        XLuaManager.Instance.GetLuaEnv().DoString(@"GameHelper.isRating=true;");
                    }
                }
            }
            else
            {

            }
        }
        else
        {
            Debug.LogError("initAppRating  UserDataManager.Instance.userInfo isNull");
        }
    }


    public static void RequestRating(int like,string content,string score)
    {
        AppRatinglevel.Instance.RequestRating(like,content,score);
    }


    public static BookData GetBookData(JDT_Book m_bookDetailCfg)
    {
        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == m_bookDetailCfg.id);
        return bookData;
    }

    public static void SetSelfBookInfo(string result)
    {
        UserDataManager.Instance.selfBookInfo = JsonHelper.JsonToObject<HttpInfoReturn<SelfBookInfo>>(result);
    }

    /// <summary>
    /// 是否为Null
    /// </summary>
    /// <param name="obj"></param>
    public static bool is_Null(object obj)
    {
        bool isBoo = true;
        if (obj != null)
        {
            isBoo = false;
        }
        return isBoo;
    }


    public static void OpenBookDisplayForm(int bookid)
    {
        var view = CUIManager.Instance.GetForm<BookDisplayForm>(UIFormName.BookDisplayForm);
        //设置展示书本ID  要展示的书本 
        view.InitByBookID(bookid);
    }


    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public static void PlayBgMusic(string path)
    {
        var asset = ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(string.Concat(path, ".mp3"))).LoadImme(AbTag.Global, enResType.eAudio, string.Concat(path, ".mp3"));
        AudioManager.Instance.PlayBGM(asset.resAudioClip);
    }

    public static void SetMainTopClose(string name)
    {
        var UIForm = CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);
        if (UIForm != null)
        {
            UIForm.GamePlayTopOpen(name);
        }
    }

    public static void MainTopCloseShow()
    {
        var UIForm = CUIManager.Instance.GetForm<MainTopSprite>(UIFormName.MainFormTop);
        if (UIForm != null)
        {
            UIForm.CtrlIconShow(false);
        }
    }


    /// <summary>
    /// 是否有刘海 返回偏移距离
    /// </summary>
    public static float isHasUnSafeArea(CUIForm myForm)
    {
        float offectY = 0;
        offectY=UnSafeAreaNotFit(myForm, null, 0, 0);
        return offectY;
    }


    public static float UnSafeAreaNotFit(CUIForm uiform,RectTransform RectTrans,float _x,float _y)
    {
        float offect = 0;
#if !UNITY_EDITOR && UNITY_ANDROID
        if (ResolutionAdapter.androidisSafeArea == true)
        {
            offect=60;
        }
#endif
#if UNITY_EDITOR || UNITY_IOS
        if (ResolutionAdapter.HasUnSafeArea)
        {
            var safeArea = ResolutionAdapter.GetSafeArea();
            var _offset = uiform.Pixel2View(safeArea.position);
            offect = _offset.y;
        }
#endif
        if (RectTrans != null)
        {
            RectTrans.sizeDelta = new Vector2(_x, _y + offect);
        }
      
        return offect;
    }







    //判断当前所在的平台 0:编辑器 1:Android 2:IOS
    public static int GetPlatFormType()
    {
        int platform = 0;
#if UNITY_EDITOR
        platform = 0;
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        platform = 1;
#endif
#if !UNITY_EDITOR && UNITY_IOS
         platform = 2;
#endif
        return platform;
    }




    /// <summary>
    /// back键退出游戏操作
    /// </summary>
    public static void BackQuitGame(string tips, string _content, string _againplaytex1, string _againplaytex2, string _quit)
    {
        UIAlertMgr.Instance.Show(tips, _content, AlertType.SureOrCancel, (isOK) =>
        {
            if (!isOK)
            {
                LoomUtil.QueueOnMainThread((param) =>
                {

                    Debug.LogError("------------------api_Logout------------------>1.");

                    //用户登出操作 请求日志记录
                    GameHttpNet.Instance.Logout((arg) =>
                    {
                        string result = arg.ToString();
                        Debug.LogError("------------------api_Logout------------------>2."+ result);

#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
                    AndroidJavaClass system = new AndroidJavaClass("java.lang.System");
                    system.CallStatic("exit", 0);
#endif
#if !UNITY_EDITOR && UNITY_IOS
                    Application.Quit();
#endif

                    });

                }, null);
            }
        }, _againplaytex1, _againplaytex2, _quit);
    }



    //是否弹出 热更新弹窗
    public static bool isHotfixPanel = false;
 

    //预设对象
    public static Object book_prefab = null;
    public static Object GetBookItem()
    {
        if (book_prefab==null)
        {
            book_prefab = Resources.Load("UI/BookItem/BookItem");
        }
        return book_prefab;
    }

    //预设对象
    public static Object story_prefab = null;
    public static Object GetStoryItem()
    {
        if (story_prefab == null)
        {
            story_prefab = Resources.Load("UI/BookItem/StoryItem");
        }
        return story_prefab;
    }



    //限时活动开关
    public static int LimitTimeActivity = 0;


    public static void DebugLog(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            Debug.Log(str);
        }
    }

}

