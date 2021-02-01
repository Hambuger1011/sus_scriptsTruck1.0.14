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


/// <summary>
/// 获取书本详细信息
/// </summary>
/// <param name="vId"></param>
/// <returns></returns>
    public JDT_Book GetBookDetailsById(int vId)
    {
        JDT_Book bookDetail = null;

        return bookDetail;
    }

   

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
    
    #region t_Skin
    // Dictionary<int, t_Skin> m_skinMap = null;
    // public t_Skin GetSkinById(int vBookId,int vId)
    // {
    //     InitSkinDataMap();
    //     t_Skin cfg;
    //     //if (m_skinMap.TryGetValue(vId, out cfg))
    //     //{
    //     //    return cfg;
    //     //}
    //     foreach(var item in m_skinMap)
    //     {
    //         if (item.Value != null && item.Value.icon_id == vId && item.Value.book_id == vBookId)
    //             return item.Value;
    //     }
    //     return null;
    // }
    //
    // void InitSkinDataMap()
    // {
    //     if (m_skinMap != null)
    //     {
    //         return;
    //     }
    //     m_skinMap = new Dictionary<int, t_Skin>();
    //     var list = Deserialize<List<t_Skin>>(CONF_DIR + "t_Skin.bytes");
    //     foreach (var item in list)
    //     {
    //         m_skinMap.Add(item.skin_id, item);
    //     }
    // }
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
