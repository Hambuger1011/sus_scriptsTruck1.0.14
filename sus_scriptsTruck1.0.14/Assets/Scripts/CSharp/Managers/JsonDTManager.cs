using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XLua.LuaCallCSharp, XLua.Hotfix]
public class JsonDTManager : Singleton<JsonDTManager>
{
    #region Book Json dataTable

    public string LocalVerInfoFlag = "VersionInfoFlag_";
    public string LocalVerBookDetailFlag = "VersionBookDetail_";
    public string LocalVerBookChapterFlag = "VersionBookChapter_";
    public string LocalVerBookDialogFlag = "VersionBookDialog_";
    public string LocalVerBookSkinFlag = "VersionBookSkin_";
    public string LocalVerBookClothesPriceFlag = "VersionBookClothesPrice_";
    public string LocalVerBookModelPriceFlag = "VersionBookModelPrice_";
    public string LocalVerBookRoleModelFlag = "VersionBookRoleModel_";
    
    
    public JsonDataTable serJsonDataTable;


    #region SaveJDTToLocal

    /// <summary>
    /// 把获取到的json包数据存储于本地
    /// </summary>
    public void SaveJDTToLocal(string vInfo)
    {
        serJsonDataTable = JsonHelper.JsonToObject<JsonDataTable>(vInfo);
        if (serJsonDataTable != null)
        {
            LOG.Error(" 加载配置成功: JDT");
        }

        #region save versionLocal
        if (serJsonDataTable.version != null)
        {
            int len = serJsonDataTable.version.Count;
            for (int i = 0; i < len; i++)
            {
                JDT_Version itemJDT = serJsonDataTable.version[i];
                if (itemJDT != null)
                    SaveLocalJDTVersionInfo( itemJDT.book_id, itemJDT);
            }
        }
        #endregion

        #region save bookDetailLocal
        if (serJsonDataTable.book != null)
        {
            int len = serJsonDataTable.book.Count;
            for (int i = 0; i < len; i++)
            {
                JDT_Book itemJDT = serJsonDataTable.book[i];
                if (itemJDT != null)
                    SaveLocalVersionBookDetail(itemJDT.id,itemJDT);
            }
        }
        #endregion
        
        #region saveChapterLocal
        if (serJsonDataTable.chapter != null)
        {
            int len = serJsonDataTable.chapter.Count;
            Dictionary<int,List<JDT_Chapter>> chapterDic = new Dictionary<int, List<JDT_Chapter>>();
            for (int i = 0; i < len; i++)
            {
                JDT_Chapter itemJDT = serJsonDataTable.chapter[i];
                if (itemJDT != null)
                {
                    if (chapterDic.TryGetValue(itemJDT.bookid, out List<JDT_Chapter> bookChapters))
                    {
                        bookChapters.Add(itemJDT);
                    }
                    else
                    {
                        List<JDT_Chapter> tempBookChapters = new List<JDT_Chapter>();
                        tempBookChapters.Add(itemJDT);
                        chapterDic.Add(itemJDT.bookid,tempBookChapters);
                    }
                }
            }
            foreach (var item in chapterDic)
            {
                if (item.Value != null)
                    SaveLocalVersionChapter(item.Key,item.Value);
            }  
        }
        #endregion
        
        #region save clothes_priceLocal
        if (serJsonDataTable.clothes_price != null)
        {
            int len = serJsonDataTable.clothes_price.Count;
            Dictionary<int,List<JDT_ClothesPrice>> itemDic = new Dictionary<int, List<JDT_ClothesPrice>>();
            for (int i = 0; i < len; i++)
            {
                JDT_ClothesPrice itemJDT = serJsonDataTable.clothes_price[i];
                if (itemJDT != null)
                {
                    if (itemDic.TryGetValue(itemJDT.bookid, out List<JDT_ClothesPrice> itemList))
                    {
                        itemList.Add(itemJDT);
                    }
                    else
                    {
                        List<JDT_ClothesPrice> tempList = new List<JDT_ClothesPrice>();
                        tempList.Add(itemJDT);
                        itemDic.Add(itemJDT.bookid,tempList);
                    }
                }
            }
            foreach (var item in itemDic)
            {
                if (item.Value != null)
                    SaveLocalVersionClothesPrice(item.Key,item.Value);
            }  
        }
        #endregion
        
        #region save skin
        if (serJsonDataTable.skin != null)
        {
            int len = serJsonDataTable.skin.Count;
            Dictionary<int,List<JDT_Skin>> itemDic = new Dictionary<int, List<JDT_Skin>>();
            for (int i = 0; i < len; i++)
            {
                JDT_Skin itemJDT = serJsonDataTable.skin[i];
                if (itemJDT != null)
                {
                    if (itemDic.TryGetValue(itemJDT.book_id, out List<JDT_Skin> itemList))
                    {
                        itemList.Add(itemJDT);
                    }
                    else
                    {
                        List<JDT_Skin> tempList = new List<JDT_Skin>();
                        tempList.Add(itemJDT);
                        itemDic.Add(itemJDT.book_id,tempList);
                    }
                }
            }
            foreach (var item in itemDic)
            {
                if (item.Value != null)
                    SaveLocalVersionSkin(item.Key,item.Value);
            }  
        }
        #endregion
        
        #region save model_priceLocal
        if (serJsonDataTable.model_price != null)
        {
            int len = serJsonDataTable.model_price.Count;
            Dictionary<int,List<JDT_ModelPrice>> itemDic = new Dictionary<int, List<JDT_ModelPrice>>();
            for (int i = 0; i < len; i++)
            {
                JDT_ModelPrice itemJDT = serJsonDataTable.model_price[i];
                if (itemJDT != null)
                {
                    if (itemDic.TryGetValue(itemJDT.book_id, out List<JDT_ModelPrice> itemList))
                    {
                        itemList.Add(itemJDT);
                    }
                    else
                    {
                        List<JDT_ModelPrice> tempList = new List<JDT_ModelPrice>();
                        tempList.Add(itemJDT);
                        itemDic.Add(itemJDT.book_id,tempList);
                    }
                }
            }
            foreach (var item in itemDic)
            {
                if (item.Value != null)
                    SaveLocalVersionModelPrice(item.Key,item.Value);
            }  
        }
        #endregion
        
        #region save role_Model Local
        if (serJsonDataTable.role_model != null)
        {
            int len = serJsonDataTable.role_model.Count;
            Dictionary<int,List<JDT_RoleModel>> itemDic = new Dictionary<int, List<JDT_RoleModel>>();
            for (int i = 0; i < len; i++)
            {
                JDT_RoleModel itemJDT = serJsonDataTable.role_model[i];
                if (itemJDT != null)
                {
                    if (itemDic.TryGetValue(itemJDT.book_id, out List<JDT_RoleModel> itemList))
                    {
                        itemList.Add(itemJDT);
                    }
                    else
                    {
                        List<JDT_RoleModel> tempList = new List<JDT_RoleModel>();
                        tempList.Add(itemJDT);
                        itemDic.Add(itemJDT.book_id,tempList);
                    }
                }
            }
            foreach (var item in itemDic)
            {
                if (item.Value != null)
                    SaveLocalVersionRoleModel(item.Key,item.Value);
            }  
        }
        #endregion
    }

    
    public void SaveJDTItemToLocal(string vFlag, string value)
    {
        PlayerPrefs.SetString(vFlag,value);
        PlayerPrefs.Save();
    }

    public string GetJDTItemByLocal(string vFlag)
    {
        return PlayerPrefs.GetString(vFlag);
    }

    #endregion
    
    
    /// <summary>
    /// 获取书本相关的版本信息
    /// </summary>
    /// <param name="vBookId"></param>
    /// <returns></returns>
    public JDT_Version GetJDTVersionInfo(int vBookId)
    {
        string resultStr = GetJDTItemByLocal(LocalVerInfoFlag + vBookId);
        if (!string.IsNullOrEmpty(resultStr))
        {
            JDT_Version result = JsonHelper.JsonToObject<JDT_Version>(resultStr);
            return result;
        }
        return null;
    }

    /// <summary>
    /// 保存在本地-书本详情
    /// </summary>
    /// <param name="vBookId"></param>
    /// <param name="vInfo"></param>
    public void SaveLocalJDTVersionInfo(int vBookId,JDT_Version vInfo)
    {
        SaveJDTItemToLocal(LocalVerInfoFlag + vBookId, JsonHelper.ObjectToJson(vInfo));
    }
    
    /// <summary>
    /// 获取书本详情信息
    /// </summary>
    /// <param name="vBookId"></param>
    /// <returns></returns>
    public JDT_Book GetJDTBookDetailInfo(int vBookId)
    {
        string resultStr = GetJDTItemByLocal(LocalVerBookDetailFlag + vBookId);
        if (!string.IsNullOrEmpty(resultStr))
        {
            JDT_Book result = JsonHelper.JsonToObject<JDT_Book>(resultStr);
            result.Init();
            return result;
        }
        return null;
    }

/// <summary>
/// 保存在本地-书本详情
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vInfo"></param>
public void SaveLocalVersionBookDetail(int vBookId,JDT_Book vInfo)
{
    SaveJDTItemToLocal(LocalVerBookDetailFlag + vBookId, JsonHelper.ObjectToJson(vInfo));
}

/// <summary>
/// 获取书本数据
/// </summary>
/// <param name="resultInfo"></param>
public void SaveLocalVersionBookDetailByStr(string resultInfo)
{
    UserDataManager.Instance.VersionBookDetailInfo = JsonHelper.JsonToObject<HttpInfoReturn<VersionBookDetailInfo>>(resultInfo);
    if (UserDataManager.Instance.VersionBookDetailInfo != null &&
        UserDataManager.Instance.VersionBookDetailInfo.data != null && 
        UserDataManager.Instance.VersionBookDetailInfo.data.info != null)
    {
        SaveJDTItemToLocal(LocalVerBookDetailFlag + UserDataManager.Instance.VersionBookDetailInfo.data.info.id, JsonHelper.ObjectToJson(UserDataManager.Instance.VersionBookDetailInfo.data.info));
    }
}

/// <summary>
/// 根据书本和章节ID，获取章节信息
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vChapterId"></param>
/// <returns></returns>
    public JDT_Chapter GetJDTChapterInfo(int vBookId, int vChapterId)
    {
        string resultStr = GetJDTItemByLocal(LocalVerBookChapterFlag + vBookId);
        if (!string.IsNullOrEmpty(resultStr))
        {
            List<JDT_Chapter> itemList = JsonHelper.JsonToObject<List<JDT_Chapter>>(resultStr);
            if (itemList != null)
            {
                int len = serJsonDataTable.chapter.Count;
                for (int i = 0; i < len; i++)
                {
                    JDT_Chapter result = serJsonDataTable.chapter[i];
                    if (result != null  && result.bookid == vBookId && result.chapter == vChapterId)
                    {
                        return result;
                    }
                }
            }
        }
        return null;
    }

/// <summary>
/// 保存在本地-书本章节信息
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vInfo"></param>
public void SaveLocalVersionChapter(int vBookId ,List<JDT_Chapter> vInfo)
{
    SaveJDTItemToLocal(LocalVerBookChapterFlag + vBookId, JsonHelper.ObjectToJson(vInfo));
}

/// <summary>
/// 获取服装价格
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vClothesId"></param>
/// <returns></returns>
public JDT_ClothesPrice GetJDTClothesPrice(int vBookId, int vClothesId)
{
    string resultStr = GetJDTItemByLocal(LocalVerBookClothesPriceFlag + vBookId);
    if (!string.IsNullOrEmpty(resultStr))
    {
        List<JDT_ClothesPrice> itemList = JsonHelper.JsonToObject<List<JDT_ClothesPrice>>(resultStr);
        if (itemList != null)
        {
            int len = serJsonDataTable.clothes_price.Count;
            for (int i = 0; i < len; i++)
            {
                JDT_ClothesPrice result = serJsonDataTable.clothes_price[i];
                if (result != null  && result.bookid == vBookId && result.clotheid == vClothesId)
                {
                    return result;
                }
            }
        }
    }
    return null;
}

/// <summary>
/// 保存在本地-获取服装价格
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vInfo"></param>
public void SaveLocalVersionClothesPrice(int vBookId,List<JDT_ClothesPrice> vInfo)
{
    SaveJDTItemToLocal(LocalVerBookClothesPriceFlag + vBookId, JsonHelper.ObjectToJson(vInfo));
}

/// <summary>
/// 获取服装描述
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vClothesId"></param>
/// <returns></returns>
public JDT_Skin GetJDTSkin(int vBookId, int vClothesId)
{
    string resultStr = GetJDTItemByLocal(LocalVerBookSkinFlag + vBookId);
    if (!string.IsNullOrEmpty(resultStr))
    {
        List<JDT_Skin> itemList = JsonHelper.JsonToObject<List<JDT_Skin>>(resultStr);
        if (itemList != null)
        {
            int len = serJsonDataTable.skin.Count;
            for (int i = 0; i < len; i++)
            {
                JDT_Skin result = serJsonDataTable.skin[i];
                if (result != null  && result.book_id == vBookId && result.icon_id == vClothesId)
                {
                    return result;
                }
            }
        }
    }
    return null;
}

/// <summary>
/// 保存在本地-服装描述
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vInfo"></param>
public void SaveLocalVersionSkin(int vBookId,List<JDT_Skin> vInfo)
{
    SaveJDTItemToLocal(LocalVerBookSkinFlag + vBookId, JsonHelper.ObjectToJson(vInfo));
}

/// <summary>
 /// 获取模型价格表
 /// </summary>
 /// <param name="vBookId"></param>
 /// <param name="vType"></param>
 /// <param name="vItemId"></param>
 /// <returns></returns>
 public JDT_ModelPrice GetJDTModelPrice(int vBookId, int vType, int vItemId)
 {
     string resultStr = GetJDTItemByLocal(LocalVerBookModelPriceFlag + vBookId);
     if (!string.IsNullOrEmpty(resultStr))
     {
         List<JDT_ModelPrice> itemList = JsonHelper.JsonToObject<List<JDT_ModelPrice>>(resultStr);
         if (itemList != null)
         {
             int len = serJsonDataTable.model_price.Count;
             for (int i = 0; i < len; i++)
             {
                 JDT_ModelPrice result = serJsonDataTable.model_price[i];
                 if (result != null  && result.book_id == vBookId && result.type == vType && result.item_id == vItemId)
                 {
                     return result;
                 }
             }
         }
     }
     return null;
 }

/// <summary>
/// 保存在本地-模型价格表
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vInfo"></param>
public void SaveLocalVersionModelPrice(int vBookId, List<JDT_ModelPrice> vInfo)
{
     SaveJDTItemToLocal(LocalVerBookModelPriceFlag + vBookId, JsonHelper.ObjectToJson(vInfo));
}

/// <summary>
/// 获取角色模型信息
/// </summary>
/// <param name="vModelId"></param>
/// <returns></returns>
public JDT_RoleModel GetJDTRoleModel(int vBookId,int vModelId)
{
    string resultStr = GetJDTItemByLocal(LocalVerBookRoleModelFlag + vBookId);
    if (!string.IsNullOrEmpty(resultStr))
    {
        List<JDT_RoleModel> itemList = JsonHelper.JsonToObject<List<JDT_RoleModel>>(resultStr);
        if (itemList != null)
        {
            int len = serJsonDataTable.role_model.Count;
            for (int i = 0; i < len; i++)
            {
                JDT_RoleModel result = serJsonDataTable.role_model[i];
                if (result != null  && result.id == vModelId)
                {
                    result.Init();
                    return result;
                }
            }
        }
    }
    return null;
}

/// <summary>
/// 保存在本地-角色模型信息表
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vInfo"></param>
public void SaveLocalVersionRoleModel(int vBookId, List<JDT_RoleModel> vInfo)
{
    SaveJDTItemToLocal(LocalVerBookRoleModelFlag + vBookId, JsonHelper.ObjectToJson(vInfo));
}


public Dictionary<int,JDT_ChapterDialogList>  CurBookChapterDialogDic = new Dictionary<int, JDT_ChapterDialogList>();
public void SetDialogByJson(int vBookId,int vChapterId,string vInfo)
{
    if (string.IsNullOrEmpty(vInfo)) return;
    if (CurBookChapterDialogDic.Count > 0)
    {
        foreach (var item in CurBookChapterDialogDic)
        {
            if (item.Value != null && item.Value.bookId != vBookId)
            {
                CurBookChapterDialogDic.Clear();
                break;
            }
        }
    }
    
    JDT_ChapterDialogList chapterDialogListItem = new JDT_ChapterDialogList();
    chapterDialogListItem.bookId = vBookId;
    chapterDialogListItem.chapterId = vChapterId;
    chapterDialogListItem.dialogList = JsonHelper.JsonToObject<List<JDT_Dialog>>(vInfo);
    if (chapterDialogListItem.dialogList != null)
    {
        int len = chapterDialogListItem.dialogList.Count;
        for (int i = 0; i < len; i++)
        {
            JDT_Dialog dialogItem = chapterDialogListItem.dialogList[i];
            if(dialogItem != null)
                dialogItem.Init();
        }
    }

    if (CurBookChapterDialogDic.ContainsKey(chapterDialogListItem.chapterId))
        CurBookChapterDialogDic[chapterDialogListItem.chapterId] = chapterDialogListItem;
    else
        CurBookChapterDialogDic.Add(chapterDialogListItem.chapterId,chapterDialogListItem);
}

/// <summary>
/// 获取对话列表内容
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vChapterId"></param>
/// <param name="vDialogId"></param>
/// <returns></returns>
public List<JDT_Dialog> GetBookChapterDialogList(int vBookId, int vChapterId)
{
    if (CurBookChapterDialogDic.ContainsKey(vChapterId))
    {
        JDT_ChapterDialogList chapterDialogListItem = CurBookChapterDialogDic[vChapterId];
        if (chapterDialogListItem.bookId == vBookId && chapterDialogListItem.dialogList != null)
        {
            return chapterDialogListItem.dialogList;
        }
    }
    return null;
}

/// <summary>
/// 获取对话列表内容
/// </summary>
/// <param name="vBookId"></param>
/// <param name="vChapterId"></param>
/// <param name="vDialogId"></param>
/// <returns></returns>
public JDT_Dialog GetDialogItem(int vBookId, int vChapterId,int vDialogId)
{
    if (CurBookChapterDialogDic.ContainsKey(vChapterId))
    {
        JDT_ChapterDialogList chapterDialogListItem = CurBookChapterDialogDic[vChapterId];
        if (chapterDialogListItem.bookId == vBookId && chapterDialogListItem.dialogList != null)
        {
            int len = chapterDialogListItem.dialogList.Count;
            for (int i = 0; i < len; i++)
            {
                JDT_Dialog item = chapterDialogListItem.dialogList[i];
                if (item != null && item.dialogid == vDialogId)
                {
                    item.Init();
                    return item;
                }
            }
        }
    }
    return null;
}


#endregion
}
