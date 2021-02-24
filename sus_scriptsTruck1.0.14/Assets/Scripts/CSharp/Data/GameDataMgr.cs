using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using XLua;

public class GameDataMgr : CSingleton<GameDataMgr>
{
    public XlsxData table { get; private set; }
    public SimulationUserData userData { get; private set; }

    public bool BaseResLoadFinish = false;

    public bool DoLoginGames = false;

    public bool BookReadingFormIsOn = false;

//#if ENABLE_DEBUG
    public bool InAutoPlay = false;     //是否开启自动播放
    public bool AutoPlayPause = false;  //是否暂停自动播放
    public int ServiceType = 1;         //选择服务器类型（1:开发服,2:测试服,3:正式服）
    public int ResourceType = 1;        //加载资源的路径 (1:开发服,2:测试服,3:正式服,0:本地（即资源跟游戏打在一起）)
    public bool UserLocalAddress = false;  //是否使用本地选择路径
    public bool UserLocalVersion = false;  //是否使用本地选择资源版本号
    public bool UserLocalDataVersion = false;  //是否使用本地选择DataTable资源版本号
    public string LocalVersion = "1";  //本地选择资源版本号
    public string LocalDataVersion = "1";  //本地选择DataTable资源版本号
    
    public LuaFunction AutoPlayOpen;
    public LuaFunction AutoPlayClose;
//#endif

    protected override void Init()
    {
        base.Init();
        table = new XlsxData();
        SimulationSvrData();
    }

    void SimulationSvrData()
    {
        userData = new SimulationUserData();

        var testTime = GetLocalUtcTimestamp();
        SetServerTime(testTime);
    }


    #region 游戏时间
    private int m_svrTime;
    private float s_upToLoginSec;

    /// <summary>
    /// 登录成功后,设置服务器utc时间戳
    /// </summary>
    public void SetServerTime(int serverTime)
    {
        m_svrTime = serverTime;
        s_upToLoginSec = Time.realtimeSinceStartup;

        //Debug.Log("==========m_svrTime===========>" + m_svrTime);

#if UNITY_EDITOR || ENABLE_DEBUG
        //DateTime dateTime = Utility.ToUtcTime2Local(serverTime);
        //LOG.Error("服务器时间:" + dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
#endif
    }

    /// <summary>
    /// 获取本地时间戳
    /// </summary>
    public int GetLocalUtcTimestamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
        return (int)ts.TotalSeconds;//获取10位
        //return (int)ts.TotalMilliseconds; //获取13位
    }

    /// <summary>
    /// 获取当前游戏时间戳(服务器)
    /// </summary>
    public int GetCurrentUTCTime()
    {
        int deltaTime = (int)(Time.realtimeSinceStartup - s_upToLoginSec);
        return m_svrTime + deltaTime;
    }
    #endregion
   
   
}


public class SimulationUserData
{
    public int keyNum;
    public int diamondNum;

    private bool isHad = false;
    #region MyBooks

    List<int> MyBookList;
    public List<int> GetMyBookIds()
    {
        //var json = PlayerPrefs.GetString("MYBOOK_ID_LIST");
        //if (string.IsNullOrEmpty(json))
        //{
        //    return new List<int>(0);
        //}
        //return JsonHelper.JsonToObject<List<int>>(json);

        if (MyBookList==null)
        {
            MyBookList = new List<int>();
        }

        return MyBookList;
    }

    public void MyBookListClean()
    {
        //清空书架上上一个账号的数据
        GetMyBookIds().Clear();
        EventDispatcher.Dispatch(EventEnum.MyBookDestry);
    }

    /// <summary>
    /// 刚进入书本的时候获取服务端返回的书架书本信息
    /// </summary>
    /// <param name="bookId"></param>
    public void saveAddMyBookId(int bookId)
    {
        var ids = GetMyBookIds();
        if (!isHad)
        {
            isHad = true;
            ids.Clear();
        }

        if (ids.IndexOf(bookId) == -1)
        {
            //不包含的时候，把书本添加进入书架
            ids.Add(bookId);
            return;
        }
    }

    public void AddMyBookId(int bookId,bool isSendToServer = false)
    {
        
        //Debug.Log("书本增加0");
        var ids = GetMyBookIds();

        if (!isHad)
        {
            isHad = true;
            ids.Clear();
        }

        if(ids.IndexOf(bookId) != -1)
        {
            //书本已经在收藏里面了，读这本书，把这本书放在书架的最前面
            ids.Remove(bookId);
            ids.Insert(0, bookId);
            return;
        }
        else
        {          
            ids.Insert(0,bookId);
        }

        //var json = JsonHelper.ObjectToJson(ids);
        //PlayerPrefs.SetString("MYBOOK_ID_LIST", json);

        if (isSendToServer)
            GameHttpNet.Instance.BookCollectionSettings(bookId, 1, (arg) =>
            {
                this.BookCollectionSettingsCallBacke(bookId, arg);

                //【AF事件记录* 第一次收藏书本】
                AppsFlyerManager.Instance.FIRST_FAVORITE_OFFICIAL_BOOK();
            });
    }

    /// <summary>
    /// 判断是否
    /// </summary>
    private List<int> mNewCollectBookList = new List<int>();
    public void CheckHasBookCollectChange()
    {
        int len = mNewCollectBookList.Count;
        for (int i = 0; i < len; i++)
        {
            var bookId = mNewCollectBookList[i];
            GameHttpNet.Instance.BookCollectionSettings(bookId, 1, (arg) =>
            {
                this.BookCollectionSettingsCallBacke(bookId, arg);

                //【AF事件记录* 第一次收藏书本】
                AppsFlyerManager.Instance.FIRST_FAVORITE_OFFICIAL_BOOK();
            });
        }
        mNewCollectBookList.Clear();
    }

    public void RemoveCollectChange(int vBookId)
    {
        int index = mNewCollectBookList.IndexOf(vBookId);
        if (index != -1)
            mNewCollectBookList.RemoveAt(index);
    }

    public void RemoveMyBookId(int bookId)
    {
        //LOG.Info("取消书本的收藏；"+bookId);
        var ids = GetMyBookIds();
        if (!ids.Contains(bookId))
        {
            return;
        }
        RemoveCollectChange(bookId);
        //取消收藏
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.BookCollectionSettings(bookId,0, (arg) =>
        {
            this.BookCollectionSettingsCallBacke(bookId, arg);
        });

    }

    private void BookCollectionSettingsCallBacke(int bookID, object arg)
    {
        string result = arg.ToString();
        LOG.Info("----BookCollectionSettingsCallBacke---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            //UINetLoadingMgr.Instance.Close();
            if (jo.code == 200)
            {
                var ids = GetMyBookIds();
                var info = JsonHelper.JsonToObject<HttpInfoReturn<UserBookFavInfo>>(result);
                var isfav = info.data.isfav == 1;
                if (isfav)
                {
                    if (!ids.Contains(bookID))
                    {
                        ids.Add(bookID);
                    }
                    LOG.Info("书本id收藏:" + bookID);
                }
                else
                {
                    ids.Remove(bookID);
                    LOG.Info("取消书本id收藏:" + bookID);
                }

            }
            else
            {
                var ids = GetMyBookIds();
                if (!ids.Contains(bookID))
                {
                    return;
                }
                ids.Remove(bookID);
            }
        }
    }
    public void RemoveAllBook()
    {
        PlayerPrefs.SetString("MYBOOK_ID_LIST", "");
    }
    #endregion
}