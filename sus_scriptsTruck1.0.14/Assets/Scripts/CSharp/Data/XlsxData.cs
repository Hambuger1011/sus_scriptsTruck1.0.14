using AB;
using pb;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using System;

public class XlsxData
{
    #region 加载数据

    public static readonly string CONF_DIR = "Assets/Bundle/Data/Common/";
    public static readonly string BOOK_DIR = "Assets/Bundle/Data/BookDialog/";

    public static T Deserialize<T>(string strFileName)
    {
        Stopwatch st = new Stopwatch();
        st.Start();
        byte[] rawBytes = null;
        if (ABSystem.Instance.isUseAssetBundle)
        {
            var asset = ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(strFileName)).LoadImme(AbTag.Null, enResType.eText, strFileName);
            rawBytes = asset.resTextAsset.bytes;
        }
        else
        {
            rawBytes = File.ReadAllBytes(strFileName);
        }
        T obj = Deserialize<T>(rawBytes);
        st.Stop();
        LOG.Info("pb解析耗时:" + st.ElapsedMilliseconds + "ms 读取耗时:" + st.ElapsedMilliseconds + "ms");
        return obj;
    }

    public static T Deserialize<T>(byte[] rawBytes)
    {
        T obj = default(T);
        using (MemoryStream ms = new MemoryStream(rawBytes))
        {
            try
            {
                obj = ProtoBuf.Serializer.Deserialize<T>(ms);
            }
            catch (System.Exception ex)
            {
                LOG.Error("反序列化异常" + ex.Message);
            }
        }
        return obj;
    }
    #endregion


    #region t_BookDetails
    Dictionary<int, t_BookDetails> m_bookDetailsMap = null;
    public t_BookDetails GetBookDetailsById(int vId)
    {
        InitBookDetailsMap();
        t_BookDetails cfg;
        if(m_bookDetailsMap.TryGetValue(vId,out cfg))
        {
            return cfg;
        }     
        return null;
    }

    void InitBookDetailsMap()
    {
        if(m_bookDetailsMap != null)
        {
            return;
        }
        m_bookDetailsMap = new Dictionary<int, t_BookDetails>();
        var list = Deserialize<List<t_BookDetails>>(CONF_DIR + "t_BookDetails.bytes");
        foreach(var item in list)
        {
            m_bookDetailsMap.Add(item.id, item);
            //LOG.Info("item.id" + item.id);

        }
    }

    private List<t_BookDetails> BookDetailsCfList;
    public List<t_BookDetails> BookDetailsMapReturn()
    {
        if (BookDetailsCfList == null)
            BookDetailsCfList = new List<t_BookDetails>();
        if (m_bookDetailsMap == null)
        {
            m_bookDetailsMap = new Dictionary<int, t_BookDetails>();
            var list = Deserialize<List<t_BookDetails>>(CONF_DIR + "t_BookDetails.bytes");
            foreach (var item in list)
            {
                m_bookDetailsMap.Add(item.id, item);
                BookDetailsCfList.Add(item);
            }
        }else
        {
            var list = Deserialize<List<t_BookDetails>>(CONF_DIR + "t_BookDetails.bytes");
            foreach (var item in list)
            {              
                BookDetailsCfList.Add(item);
            }
        }
        return BookDetailsCfList;
    }
    #endregion

    #region t_BookShelf
    Dictionary<int, t_BookShelf> m_bookShelfMap = null;
    public t_BookShelf GetBookShelfById(int vId)
    {
        InitBookShelfMap();
        t_BookShelf cfg;
        if (m_bookShelfMap.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    public Dictionary<int, t_BookShelf> GetBookShelfs()
    {
        InitBookShelfMap();
        return m_bookShelfMap;
    }

    void InitBookShelfMap()
    {
        if (m_bookShelfMap != null)
        {
            return;
        }
        m_bookShelfMap = new Dictionary<int, t_BookShelf>();
        var list = Deserialize<List<t_BookShelf>>(CONF_DIR + "t_BookShelf.bytes");
        foreach (var item in list)
        {
            m_bookShelfMap.Add(item.Id, item);
        }
    }
    #endregion

    #region 新手引导
    List<t_BookTutorial> m_NewFistDialogList = null;
    Dictionary<int, t_BookTutorial> m_NewFistDialogMap = new Dictionary<int, t_BookTutorial>();

    /// <summary>
    /// 调用这个方法，获得该行dialogueID的信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_BookTutorial GetNewFistDialogById(int vId)
    {
        t_BookTutorial cfg;
        if (m_NewFistDialogMap.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    /// <summary>
    /// 调用这个方法保存新手引导表中的所有数据到字典中
    /// </summary>
    public void NewFistLoadDialogData()
    {
        
        m_NewFistDialogMap.Clear();
        m_NewFistDialogList = Deserialize<List<t_BookTutorial>>(string.Concat(CONF_DIR+ "t_BookTutorial.bytes"));
        foreach (var item in m_NewFistDialogList)
        {
            if (!m_NewFistDialogMap.ContainsKey(item.dialogID))
                m_NewFistDialogMap.Add(item.dialogID, item);
            else
                LOG.Error("---dialogID已经包含了-->"+ item.dialogID);
        }
    }
    #endregion

    #region 活动表
    List<t_Activity> m_ActivityList = null;
    List<t_Activity> m_ActivityStateList = null;//这个是已开通的活动列表
    Dictionary<int, t_Activity> m_ActivityListMap = new Dictionary<int, t_Activity>();
    private int number = 0;

    public t_Activity ActivityStateListReturn(int num)
    {
        t_Activity AC = null;

        if (num>=m_ActivityStateList.Count)
        {
            AC = null;
        }
        else
        {
            AC = m_ActivityStateList[num];
        }
        return AC;
    }
    public List<t_Activity> ActivityStateListReturnCount()
    {
        return m_ActivityStateList;
    }
    public void ActivityStateListCount()
    {
        number++;
        t_Activity data= GetActivityById(number);

        if (data!=null)
        {
            if (data.state==1)
            {
                //这个活动是已经开启的
                if (m_ActivityStateList==null)
                {
                    m_ActivityStateList = new List<t_Activity>();
                }

                if (data.Activityid==1004)
                {
                    //登录，谷歌登录
                    if (UserDataManager.Instance.LoginInfo.LastLoginChannel ==0)
                    {
                        //这个是新手界面没有购买过的时候加入列表                      
                    }else
                    {
                        ActivityStateListCount();
                        return;
                    }
                }

                if (data.Activityid == 1003)
                {
                    if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                         UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
                    {
                       //这个是新手礼包没有购买过
                    }
                    else
                    {
                        ActivityStateListCount();
                        return;
                    }
                }
                m_ActivityStateList.Add(data);
            }

            ActivityStateListCount();
        }
        else
        {
            return;
        }
    }
    /// <summary>
    /// 调用这个方法，获得FAQ该行dialogueID的信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_Activity GetActivityById(int vId)
    {
        t_Activity cfg;
        if (m_ActivityListMap.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    public int ActivityCount()
    {
        return m_ActivityListMap.Count;
    }
    /// <summary>
    /// 调用这个方法保存FAQ表中的所有数据到字典中
    /// </summary>
    public void ActivityData()
    {
        number = 0;
        //这个活动是已经开启的
        if (m_ActivityStateList == null)
        {
            m_ActivityStateList = new List<t_Activity>();
        }
        m_ActivityStateList.Clear();

        m_ActivityListMap.Clear();
        m_ActivityList = Deserialize<List<t_Activity>>(string.Concat(CONF_DIR + "t_Activity.bytes"));
        foreach (var item in m_ActivityList)
        {
            if (!m_ActivityListMap.ContainsKey(item.ID))
                m_ActivityListMap.Add(item.ID, item);
            else
                LOG.Error("---ID已经包含了-->" + item.ID);
        }

        ActivityStateListCount();
    }
    #endregion

    #region 公告表

    List<t_BulletinBoard> m_BulletinBoardList = null;
    Dictionary<int, t_BulletinBoard> m_BulletinBoardListMap = new Dictionary<int, t_BulletinBoard>();

    /// <summary>
    /// 调用这个方法，获得FAQ该行dialogueID的信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_BulletinBoard GetBulletinBoardById(int vId)
    {
        t_BulletinBoard cfg;
        if (m_BulletinBoardListMap.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    /// <summary>
    /// 调用这个方法保存FAQ表中的所有数据到字典中
    /// </summary>
    public void BulletinBoardData()
    {        
        m_BulletinBoardListMap.Clear();
        m_BulletinBoardList = Deserialize<List<t_BulletinBoard>>(string.Concat(CONF_DIR + "t_BulletinBoard.bytes"));
        foreach (var item in m_BulletinBoardList)
        {
            if (!m_BulletinBoardListMap.ContainsKey(item.id))
                m_BulletinBoardListMap.Add(item.id, item);
            else
                LOG.Error("---ID已经包含了-->" + item.id);
        }
    }
    #endregion


    #region FAQ
    List<t_BookFAQ> m_NFAQList = null;
    Dictionary<int, t_BookFAQ> m_NFAQListMap = new Dictionary<int, t_BookFAQ>();

    /// <summary>
    /// 调用这个方法，获得FAQ该行dialogueID的信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_BookFAQ GetFAQDialogById(int vId)
    {
        t_BookFAQ cfg;
        if (m_NFAQListMap.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    /// <summary>
    /// 调用这个方法保存FAQ表中的所有数据到字典中
    /// </summary>
    public void FAQDialogData()
    {

        m_NFAQListMap.Clear();
        m_NFAQList = Deserialize<List<t_BookFAQ>>(string.Concat(CONF_DIR + "t_BookFAQ.bytes"));
        foreach (var item in m_NFAQList)
        {
            if (!m_NFAQListMap.ContainsKey(item.ID))
                m_NFAQListMap.Add(item.ID, item);
            else
                LOG.Error("---ID已经包含了-->" + item.ID);
        }
    }
    #endregion

    #region 屏蔽字
    List<t_Banned_Words> mBannedWordsList = null;
    public List<t_Banned_Words> GetBannedWordsList()
    {
        InitBannerWordsList();
        return mBannedWordsList;
    }

    void InitBannerWordsList()
    {
        if (mBannedWordsList != null)
        {
            return;
        }
        mBannedWordsList = Deserialize<List<t_Banned_Words>>(CONF_DIR + "t_Banned_Words.bytes");
    }
    #endregion
    
    #region 加载提示
    Dictionary<int, t_Loading_Tips> mLoadingTipsMap = new Dictionary<int, t_Loading_Tips>();

    void InitLoadingTipsList()
    {
        List<t_Loading_Tips> mLoadingTipsList = Deserialize<List<t_Loading_Tips>>(string.Concat(CONF_DIR + "t_Loading_Tips.bytes"));
        foreach (var item in mLoadingTipsList)
        {
            if (!mLoadingTipsMap.ContainsKey(item.ID))
                mLoadingTipsMap.Add(item.ID, item);
            else
                LOG.Error("---ID已经包含了-->" + item.ID);
        }
    }
    
    
    public t_Loading_Tips GetLoadingTipsById(int vId)
    {
        t_Loading_Tips cfg;
        if (mLoadingTipsMap.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    public int LoadingTipsCount()
    {
        if (mLoadingTipsMap.Count == 0)
            InitLoadingTipsList();
        return mLoadingTipsMap.Count;
    }
    #endregion

    #region t_Skin
    Dictionary<int, t_Skin> m_skinMap = null;
    public t_Skin GetSkinById(int vBookId,int vId)
    {
        InitSkinDataMap();
        t_Skin cfg;
        //if (m_skinMap.TryGetValue(vId, out cfg))
        //{
        //    return cfg;
        //}
        foreach(var item in m_skinMap)
        {
            if (item.Value != null && item.Value.icon_id == vId && item.Value.book_id == vBookId)
                return item.Value;
        }
        return null;
    }

    void InitSkinDataMap()
    {
        if (m_skinMap != null)
        {
            return;
        }
        m_skinMap = new Dictionary<int, t_Skin>();
        var list = Deserialize<List<t_Skin>>(CONF_DIR + "t_Skin.bytes");
        foreach (var item in list)
        {
            m_skinMap.Add(item.skin_id, item);
        }
    }
    #endregion

    #region t_Clotheprice
    List<t_Clotheprice> m_clothePriceList = null;
    public t_Clotheprice GetClothePriceById(int vBookId, int vClotheId)
    {
        InitClothePriceDataMap();
        t_Clotheprice item;
        int len = m_clothePriceList.Count;
        for (int i = 0; i < len;i++ )
        {
            item = m_clothePriceList[i];
            if (item != null && item.ClotheID == vClotheId && item.BookID == vBookId)
                return item;
        }
            
        return null;
    }

    void InitClothePriceDataMap()
    {
        if (m_clothePriceList != null)
        {
            return;
        }
        m_clothePriceList = Deserialize<List<t_Clotheprice>>(CONF_DIR + "t_Clotheprice.bytes");
    }
    #endregion

    #region t_ChapterDivide
    List<t_ChapterDivide> m_ChapterDivideList = null;
    public t_ChapterDivide GetChapterDivedeById(int vBookId, int vChapterId)
    {
        InitChapterDivideDataMap();
        t_ChapterDivide item;
        int len = m_ChapterDivideList.Count;
        for (int i = 0; i < len; i++)
        {
            item = m_ChapterDivideList[i];
            if (item != null && item.chapter == vChapterId && item.bookId == vBookId)
                return item;
        }

        return null;
    }

    void InitChapterDivideDataMap()
    {
        if (m_ChapterDivideList != null)
        {
            return;
        }
        m_ChapterDivideList = Deserialize<List<t_ChapterDivide>>(CONF_DIR + "t_ChapterDivide.bytes");
    }
    #endregion


    #region t_MetaGameDetail
    List<t_MetaGameDetails> m_MetaGameDetailList = null;
    public t_MetaGameDetails GetMetaGameById(int vId)
    {
        InitMetaGameDataMap();
        t_MetaGameDetails item;
        int len = m_MetaGameDetailList.Count;
        for (int i = 0; i < len; i++)
        {
            item = m_MetaGameDetailList[i];
            if (item != null && item.ID == vId)
                return item;
        }

        return null;
    }

    void InitMetaGameDataMap()
    {
        if (m_MetaGameDetailList != null)
        {
            return;
        }
        m_MetaGameDetailList = Deserialize<List<t_MetaGameDetails>>(CONF_DIR + "t_MetaGameDetails.bytes");
    }
    #endregion

    #region t_MetaGameDetail
    List<t_PlayerNames> m_PlayerNameList = null;
    List<string> m_MaleNameList = null;
    List<string> m_FemaleNameList = null;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="vType">0:男生，1：女生</param>
    /// <returns></returns>
    public string GetNameByType(int vType = 1)
    {
        if (m_MaleNameList == null)
        {
            m_MaleNameList = new List<string>();
            m_FemaleNameList = new List<string>();
            InitPlayerName();
            t_PlayerNames item;
            int len = m_PlayerNameList.Count;
            for (int i = 0; i < len; i++)
            {
                item = m_PlayerNameList[i];
                if (item != null)
                {
                    m_MaleNameList.Add(item.male_name);
                    m_FemaleNameList.Add(item.female_name);
                }
            }
        }

        if(vType == 0 && m_MaleNameList != null)
        {
            return m_MaleNameList[UnityEngine.Random.Range(0, m_MaleNameList.Count)];
        }
        else if(vType == 1 && m_FemaleNameList != null)
        {
            return m_FemaleNameList[UnityEngine.Random.Range(0, m_FemaleNameList.Count)];
        }
        return "";
    }

    void InitPlayerName()
    {
        if (m_PlayerNameList != null)
        {
            return;
        }
        m_PlayerNameList = Deserialize<List<t_PlayerNames>>(CONF_DIR + "t_PlayerNames.bytes");
    }
    #endregion


    #region t_analysisA
    List<t_analysisA> m_AnalysisAList = null;

    public t_analysisA GetAnalysisAById(int vId)
    {
        InitAnalysisA();
        t_analysisA item;
        int len = m_AnalysisAList.Count;
        for (int i = 0; i < len; i++)
        {
            item = m_AnalysisAList[i];
            if (item != null && item.id == vId)
                return item;
        }

        return null;
    }
    public string GetAnalysisATxtById(int vId,int vTxtIndex)
    {
        InitAnalysisA();
        t_analysisA item;
        int len = m_AnalysisAList.Count;
        for (int i = 0; i < len; i++)
        {
            item = m_AnalysisAList[i];
            if (item != null && item.id == vId)
            {
                switch(vTxtIndex)
                {
                    case 1: return item.text1;
                    case 2: return item.text2;
                    case 3: return item.text3;
                    case 4: return item.text4;
                    case 5: return item.text5;
                    default: return item.text1;
                }
            }
        }

        return string.Empty;
    }

    void InitAnalysisA()
    {
        if (m_AnalysisAList != null)
        {
            return;
        }
        m_AnalysisAList = Deserialize<List<t_analysisA>>(CONF_DIR + "t_analysisA.bytes");
    }
    #endregion

    #region t_analysisB
    List<t_analysisB> m_AnalysisBList = null;
    public string GetAnalysisBTxtById(int vId, int vTxtIndex)
    {
        InitAnalysisB();
        t_analysisB item;
        int len = m_AnalysisBList.Count;
        for (int i = 0; i < len; i++)
        {
            item = m_AnalysisBList[i];
            if (item != null && item.id == vId)
            {
                switch (vTxtIndex)
                {
                    case 1: return item.text1;
                    case 2: return item.text2;
                    case 3: return item.text3;
                    case 4: return item.text4;
                    case 5: return item.text5;
                    default: return item.text1;
                }
            }
        }

        return null;
    }

    void InitAnalysisB()
    {
        if (m_AnalysisBList != null)
        {
            return;
        }
        m_AnalysisBList = Deserialize<List<t_analysisB>>(CONF_DIR + "t_analysisB.bytes");
    }
    #endregion

    #region t_analysisC
    List<t_analysisC> m_AnalysisCList = null;
    public string GetAnalysisCTxtById(int vId, int vTxtIndex)
    {
        InitAnalysisC();
        t_analysisC item;
        int len = m_AnalysisCList.Count;
        for (int i = 0; i < len; i++)
        {
            item = m_AnalysisCList[i];
            if (item != null && item.id == vId)
            {
                switch (vTxtIndex)
                {
                    case 1: return item.text1;
                    case 2: return item.text2;
                    case 3: return item.text3;
                    case 4: return item.text4;
                    case 5: return item.text5;
                    default: return item.text1;
                }
            }
        }

        return null;
    }

    void InitAnalysisC()
    {
        if (m_AnalysisCList != null)
        {
            return;
        }
        m_AnalysisCList = Deserialize<List<t_analysisC>>(CONF_DIR + "t_analysisC.bytes");
    }
    #endregion


    #region personality
    List<t_personality> m_personalityList = null;
    Dictionary<int, t_personality> m_personalityMap = new Dictionary<int, t_personality>();
    public string GetPersonalityTxtById(int vId)
    {
        InitPersonality();
        t_personality item;
        if (m_personalityMap.TryGetValue(vId, out item))
        {
            return item.Personality;
        }
        return "";
    }

    void InitPersonality()
    {
        if (m_personalityList != null)
        {
            return;
        }
        m_personalityList = Deserialize<List<t_personality>>(CONF_DIR + "t_personality.bytes");

        foreach (var item in m_personalityList)
        {
            if (!m_personalityMap.ContainsKey(item.ID))
                m_personalityMap.Add(item.ID, item);
            else
                LOG.Error("---BookID-->" + item.ID + "--DialogIdRepetition-->" + item.Personality);
        }
    }
    #endregion

    #region 猫的商店表格
    List<t_shop> m_catShopList = null;
    Dictionary<int, t_shop> m_catShopListMap = new Dictionary<int, t_shop>();

    /// <summary>
    /// 返回猫商表里面有多少条信息
    /// </summary>
    /// <returns></returns>
    public int ReturnCatShopList()
    {
        if (m_catShopListMap == null)
        {
            return 0;
        }
        int shu=m_catShopListMap.Count;
        return shu;
    }
    /// <summary>
    /// 调用这个方法，获得猫表格中该行存储的信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_shop GetcatShopId(int vId)
    {
        t_shop cfg;
        if (m_catShopListMap.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    /// <summary>
    /// 调用这个方法保存猫商店表中的所有数据到字典中
    /// </summary>
    public void catShopData()
    {
        if (m_catShopListMap.Count != 0) return;
        m_catShopList = Deserialize<List<t_shop>>(string.Concat(CONF_DIR + "t_shop.bytes"));
        foreach (var item in m_catShopList)
        {
            if (!m_catShopListMap.ContainsKey(item.shopid))
                m_catShopListMap.Add(item.shopid, item);
            else
                LOG.Error("---ID已经包含了-->" + item.shopid);
        }
    }
    #endregion

    #region 猫的装饰物的表
    List<t_decorations> m_catDecorationList = null;
    Dictionary<int, t_decorations> m_catDecorationListMap = new Dictionary<int, t_decorations>();

    /// <summary>
    /// 返回猫商表里面有多少条信息
    /// </summary>
    /// <returns></returns>
    public int ReturnDecorationList()
    {
        if (m_catDecorationListMap == null)
        {
            return 0;
        }
        int shu = m_catDecorationListMap.Count;
        return shu;
    }
    /// <summary>
    /// 调用这个方法，获得猫表格中该行存储的信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public string GetcatDecorationResByShopId(int shopId)
    {
        string res = null;
        foreach (var item in m_catDecorationList)
        {
            if (item.type == shopId)
            {
                res = item.res;
                break;
            }
        }
        return res;
    }

    /// <summary>
    /// 调用这个方法保存猫商店表中的所有数据到字典中
    /// </summary>
    public void catDecorationData()
    {
        m_catDecorationListMap.Clear();
        m_catDecorationList = Deserialize<List<t_decorations>>(string.Concat(CONF_DIR + "t_decorations.bytes"));
        foreach (var item in m_catDecorationList)
        {
            if (!m_catDecorationListMap.ContainsKey(item.id))
                m_catDecorationListMap.Add(item.id, item);
            else
                LOG.Info("---ID已经包含了-->" + item.type);
        }
    }
    #endregion

    #region 饰品放置位置表
    List<t_map> m_catMapList = null;
    Dictionary<int, t_map> m_catMapListDic = new Dictionary<int, t_map>();

   
    /// <summary>
    /// 调用这个方法，获得猫表格中该行存储的信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_map GetcatMapInfoById(int vId)
    {
        t_map cfg;
        if (m_catMapListDic.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }
    public t_map  GetCatInMapInfoBySort(int sort)
    {
        t_map tmpMap = null;
        foreach (var item in m_catMapList)
        {
            if (item.sort == sort && item.type == 3)
            {

                tmpMap = item;
                break;
            }
        }
        return tmpMap;
    }

    public List<t_map> GetCatMapPosListByLevel(int vLevel)
    {
        List<t_map> mMapList = new List<t_map>();
        foreach (var item in m_catMapList)
        {
            if (item.sort == vLevel && item.type != 3)
                mMapList.Add(item);
        }
        return mMapList;
    }

    public Dictionary<int, t_map> GetCatMapDic()
    {
        if (m_catMapListDic == null)
        {
            return null;
        }
        return m_catMapListDic;
    }

    /// <summary>
    /// 调用这个方法保存猫商店表中的所有数据到字典中
    /// </summary>
    public void GetMapData()
    {
        m_catMapListDic.Clear();
        m_catMapList = Deserialize<List<t_map>>(string.Concat(CONF_DIR + "t_map.bytes"));
        foreach (var item in m_catMapList)
        {
            if (!m_catMapListDic.ContainsKey(item.id))
                m_catMapListDic.Add(item.id, item);
            else
                LOG.Error("---ID已经包含了-->" + item.id);
        }
    }
    #endregion

    #region  猫在场景相关信息
    List<t_contrast> m_catInMapInfoList = null;
    Dictionary<int, List<t_contrast>> m_catInMapInfoListDic = new Dictionary<int, List<t_contrast>>();


    /// <summary>
    /// 根据宠物ID获取猫在表格中的信息
    /// </summary>
    /// <param name="vId">宠物ID</param>
    /// <param name="decId">装饰物ID</param>
    /// <returns></returns>

    public t_contrast GetcaIntMapInfoById(int vId,int decId)
    {
        t_contrast cfg = null;
        if (m_catInMapInfoListDic.ContainsKey(vId))
        {
            foreach ( var item in m_catInMapInfoListDic[vId])
            {
                if (item.decorations == decId.ToString())
                {
                    cfg = item;
                }
            }
        }
        return cfg;
    }

    /// <summary>
    /// 调用这个方法保存猫商店表中的所有数据到字典中
    /// </summary>
    public void GetCatInMapData()
    {
        m_catInMapInfoListDic.Clear();
        m_catInMapInfoList = Deserialize<List<t_contrast>>(string.Concat(CONF_DIR + "t_contrast.bytes"));
        foreach (var item in m_catInMapInfoList)
        {

            if (!m_catInMapInfoListDic.ContainsKey(item.name))
            {
                List<t_contrast> tmp = new List<t_contrast>();
                tmp.Add(item);
                m_catInMapInfoListDic.Add(item.name, tmp);

            }
            else
            {
                m_catInMapInfoListDic[item.name].Add(item);

            }

        }
    }
    #endregion

    #region 猫粮 
    List<t_food> m_catFoodList = null;
    Dictionary<int, t_food> m_catFoodtDic = new Dictionary<int, t_food>();


    /// <summary>
    /// 根据猫粮Id获取信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_food GetCatFoodById(int vId)
    {
        t_food cfg;
        if (m_catFoodtDic.TryGetValue(vId, out cfg))
        {
            return cfg;
        }
        return null;
    }

    public Dictionary<int, t_food> GetCatFoodDic()
    {
        if (m_catMapListDic == null)
        {
            return null;
        }
        return m_catFoodtDic;
    }

    /// <summary>
    /// 调用这个方法保存猫商店表中的所有数据到字典中
    /// </summary>
    public void GetCatFoodData()
    {
        m_catFoodtDic.Clear();
        m_catFoodList = Deserialize<List<t_food>>(string.Concat(CONF_DIR + "t_food.bytes"));
        foreach (var item in m_catFoodList)
        {
            if (!m_catFoodtDic.ContainsKey(item.id))
                m_catFoodtDic.Add(item.id, item);
            else
                LOG.Error("---ID已经包含了-->" + item.id);
        }
    }
    #endregion

    #region Gif数据配置
    List<t_gif> m_catGifList = null;
    Dictionary<int, List<t_gif>> m_catGifDic = new Dictionary<int, List<t_gif>>();


    /// <summary>
    /// 根据猫Id和动作ID获取信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_gif GetCatGifById(int cId,int actid)
    {
        if (m_catGifDic.ContainsKey(cId))
        {
            foreach (var item in m_catGifDic[cId])
            {
                if (item.act == actid)
                {
                    return item;
                }
            }
        }
        return null;
    }


    /// <summary>
    /// 调用这个方法保存Gif中的所有数据到字典中
    /// </summary>
    public void GetCatGifData()
    {
        m_catGifDic.Clear();
        m_catGifList = Deserialize<List<t_gif>>(string.Concat(CONF_DIR + "t_gif.bytes"));
        foreach (var item in m_catGifList)
        {
            if (!m_catGifDic.ContainsKey(item.name))
            {
                List<t_gif> tmp = new List<t_gif>();
                tmp.Add(item);
                m_catGifDic.Add(item.name, tmp);
            }
            else
            {
                m_catGifDic[item.name].Add(item);
            }
        }
    }

    #endregion

    #region 猫的故事表格
    List<t_story> m_catStoryList = null;
    Dictionary<int, t_story> m_catStoryDic = new Dictionary<int,t_story>();

    /// <summary>
    /// 根据猫Id和动作ID获取信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_story GetCatStoryById(int cId)
    {
        t_story cfg;
        if (m_catStoryDic.TryGetValue(cId, out cfg))
        {
            return cfg;
        }
        return null;
    }


    /// <summary>
    /// 调用这个方法保存Gif中的所有数据到字典中
    /// </summary>
    public void GetCatStoryData()
    {
        m_catStoryDic.Clear();
        m_catStoryList = Deserialize<List<t_story>>(string.Concat(CONF_DIR + "t_story.bytes"));
        foreach (var item in m_catStoryList)
        {
            if (!m_catStoryDic.ContainsKey(item.id))
                m_catStoryDic.Add(item.id, item);
            else
                LOG.Error("---ID已经包含了-->" + item.id);
        }
    }
    #endregion

    #region Attribute表
    List<t_Attribute> m_catAttributeList = null;
    Dictionary<int, t_Attribute> m_catAttributeDic = new Dictionary<int, t_Attribute>();

    /// <summary>
    /// 根据猫Id获取信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public t_Attribute GetCatAttributeById(int cId)
    {
        t_Attribute cfg;
        if (m_catAttributeDic.TryGetValue(cId, out cfg))
        {
            return cfg;
        }
        return null;
    }


    /// <summary>
    /// 调用这个方法保存Gif中的所有数据到字典中
    /// </summary>
    public void GetCatAttributeData()
    {
        m_catAttributeDic.Clear();
        m_catAttributeList = Deserialize<List<t_Attribute>>(string.Concat(CONF_DIR + "t_Attribute.bytes"));
        foreach (var item in m_catAttributeList)
        {
            if (!m_catAttributeDic.ContainsKey(item.id))
                m_catAttributeDic.Add(item.id, item);
            else
                LOG.Error("---ID已经包含了-->" + item.id);
        }
    }
    #endregion

    #region Localization 表

    /// <summary>
    /// 根据Id获取信息
    /// </summary>
    /// <param name="vId"></param>
    /// <returns></returns>

    public string GetLocalizationById(int cId)
    {
        return CTextManager.Instance.GetText(cId);
    }


    /// <summary>
    /// 调用这个方法保存t_Localization中的所有数据到字典中
    /// </summary>
    public void GetLocalizationData()
    {
        CTextManager.Instance.LoadAssetBundle();
    }
    #endregion

}
