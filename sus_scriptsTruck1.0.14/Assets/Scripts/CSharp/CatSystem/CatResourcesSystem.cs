using AB;
using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatResourcesSystem : SingletonMono<CatResourcesSystem>
{
    List<AbTask> m_needLoadAssetData = new List<AbTask>();
    private Dictionary<string, bool> recordDic = new Dictionary<string, bool>();
    Dictionary<string, AbTask> mLastChapterResDic = new Dictionary<string, AbTask>();

    private int nowResNum = 0, AllResNum = 0;
    private Action<bool> Callback;
    private List<firstpetarr> tem;
    /// <summary>
    /// 加载猫的资源
    /// </summary>
    /// <param name="Callback"></param>
    public void CatPreLoadRes(Action<bool> Callback)
    {
        m_needLoadAssetData.Clear();
        recordDic.Clear();
        UserDataManager.Instance.ReturnCatAtionInfoDic().Clear();

        var catScenePath = "Assets/Bundle/catpreview.prefab";
        ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(catScenePath)).LoadAsync(AbTag.Cat, enResType.ePrefab, catScenePath, (_) =>
        {
            AbBookRes resList = _.Get<AbBookRes>();
            foreach (var res in resList.objs)
            {
                PreLoadAsset(enResType.eObject, res);
            }
        });

        var catPath = "Assets/Bundle/cat.prefab";
        ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(catPath)).LoadAsync(AbTag.Cat, enResType.ePrefab, catPath, (_) =>
        {
            if (UserDataManager.Instance.SceneInfo != null)
            {
                List<firstpetarr> tem = UserDataManager.Instance.SceneInfo.data.firstpetarr;
                if (tem.Count > 0)
                {
                    for (int i = 0; i < tem.Count; i++)
                    {
                        int catNum = tem[i].pid;  //加载的是哪只猫的资源
                        int decId = tem[i].decorations_id;//装饰物的ID


                        if (catNum == 0)
                        {
                            LOG.Info("没有猫出现，id:" + catNum);
                            return;
                        }


                        //获得猫的图片资源路径            
                        var CatResList = SureCatRes(catNum, decId);

                        if (CatResList != null)
                        {
                            foreach (var res in CatResList)
                            {
                                string ResPath = "Assets/Bundle/Cat/" + res + ".png";
                                //Debug.LogError("图片："+ResPath);
                                PreLoadAsset(enResType.eSprite, ResPath);
                            }
                            LOG.Info("猫" + catNum + "的资源存入表中，完毕！");
                            if (tem[tem.Count - 1].pid == catNum)
                            {
                                //这里标识需要加载的资源已经完全存入表中
                                Callback(true);
                            }
                        }
                        else
                        {
                            Callback(true);
                            LOG.Error("服务端返回的数据有问题，Catid:" + catNum + "--decId:" + decId);
                        }

                    }

                    //LOG.Info("加载猫的资源");
                }
                else
                {
                    Callback(true);
                    LOG.Info("不加载猫的资源");
                }
            }
        });
    }


    public void NewCatPreLoadRes(Action<bool> Callback)
    {
        m_needLoadAssetData.Clear();
        recordDic.Clear();
        this.Callback = Callback;
        UserDataManager.Instance.ReturnCatAtionInfoDic().Clear();

        var catScenePath = "Assets/Bundle/catpreview.prefab";
        ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(catScenePath)).LoadAsync(AbTag.Cat, enResType.ePrefab, catScenePath, (_) =>
        {
            AbBookRes resList = _.Get<AbBookRes>();
            foreach (var res in resList.objs)
            {
                PreLoadAsset(enResType.eObject, res);
            }

            if (CatResPath == null)
            {
                CatResPath = new List<string>();
            }
            CatResPath.Clear();

            if (UserDataManager.Instance.SceneInfo != null)
            {
                tem = UserDataManager.Instance.SceneInfo.data.firstpetarr;
                //LOG.Info("111:" + tem.Count);
                if (tem.Count > 0)
                {
                    AllResNum = tem.Count;
                    LOG.Info("tem数量:" + tem.Count);
                    resLoadownRepeat();
                }
                else
                {
                    if (UserDataManager.Instance.isFirstCatEnt)
                    {
                        LOG.Info("处在猫的引导阶段中，预先给定一只猫！");
                        AllResNum = 1;                       
                        resLoadownRepeat();
                    }
                    else
                    {
                        Callback(true);
                        LOG.Info("不加载猫的资源");
                    }

                }
            }
        });
    }

    private void resLoadownRepeat()
    {
        nowResNum++;
        if (nowResNum <= AllResNum)
        {
            //还有资源有下载完继续下载。

            int catNum = 0;
            int decId = 0;

            if (UserDataManager.Instance.isFirstCatEnt)
            {
                //新手指引的时候默认有一只猫

                catNum = 8;  //加载的是哪只猫的资源
                decId = 1;   //装饰物的ID
            }
            else
            {
                 catNum = tem[nowResNum - 1].pid;  //加载的是哪只猫的资源
                 decId = tem[nowResNum - 1].decorations_id;//装饰物的ID
            }

            if (catNum == 0)
            {
                LOG.Info("没有猫出现，id:" + catNum);
                return;
            }

            //LOG.Info("33:" + tem.Count);
            int CatDecorations = decId;//这只猫的装饰物ID                 
            int Num = 0;//取的是第几个动作

            t_contrast tmpContent = GameDataMgr.Instance.table.GetcaIntMapInfoById(catNum, CatDecorations);

            if (tmpContent == null)
            {
                LOG.Error("少年！你又再逗我！表里都没你给的数据");
                LOG.Error("错误的数据信息是：猫ID--" + catNum + "--装饰物id:" + decId);
                //Callback(true);
                //return;
            }
            int CatAction = 0; //确定猫是哪一个动作

            LOG.Info("猫id:" + catNum + "--装饰物id:" + decId + "--总共有的动作是：" + tmpContent.nums.Length);
            if (tmpContent.nums.Length > 1)
            {
                Num = UnityEngine.Random.Range(0, tmpContent.nums.Length);
                CatAction = tmpContent.nums[Num];//这只猫是哪个的动作          
            }
            else
            {
                Num = 0;
                CatAction = tmpContent.nums[0];
            }
            UserDataManager.Instance.CatAtionInfoSave(catNum, CatAction);

            string CatActionSt = CatAction.ToString("D2");
            string catNumSt = catNum.ToString("D2");
            string DataName = catNumSt + CatActionSt + "01";
            var catPath = "Assets/Bundle/CatRe/" + catNum + "/" + CatActionSt + "/" + DataName + "_SkeletonData.asset";

            LOG.Error("加载的资源猫的ID:" + catNum);
            PreLoadAsset(enResType.eObject,catPath);

            resLoadownRepeat();
        }
        else
        {
            //所有资源已经下载统计完了，可以进入游戏了
            nowResNum = 0;
            LOG.Error("加载猫的资源:" + m_needLoadAssetData.Count);
            //这里标识需要加载的资源已经完全存入表中
            Callback(true);
        }
    }

    #region 确定猫的资源是哪一些

    List<string> CatResPath;
    private List<string> SureCatRes(int CatId, int decId)
    {
        LOG.Info("CatId:" + CatId + "--decId:" + decId);
        if (CatResPath == null)
        {
            CatResPath = new List<string>();
        }
        CatResPath.Clear();

        List<int> FromNumList = new List<int>();
        FromNumList.Clear();

        int CatNum = CatId;//哪一只猫
        int CatDecorations = decId;//这只猫的装饰物ID
        string catResPath = "";//需要下载的猫的资源的路径
        int CatFrame = 0;//这个动作的帧数
        int Num = 0;//取的是第几个动作

        t_contrast tmpContent = GameDataMgr.Instance.table.GetcaIntMapInfoById(CatNum, CatDecorations);

        if (tmpContent == null)
        {
            LOG.Error("少年！你又再逗我！表里都没你给的数据");
            return null;
        }
        int CatAction = 0; //确定猫是哪一个动作
                           //Debug.Log("");
                           //随机猫的动作


        if (tmpContent.nums.Length > 1)
        {
            Num = UnityEngine.Random.Range(0, tmpContent.nums.Length);
            CatAction = tmpContent.nums[Num];//这只猫是哪个的动作          
        }
        else
        {
            Num = 0;
            CatAction = tmpContent.nums[0];
        }


        UserDataManager.Instance.CatAtionInfoSave(CatId, CatAction);


        for (int i = 0; i < tmpContent.frames.Length; i++)
        {
            int frameNum = tmpContent.frames[i];
            FromNumList.Add(frameNum);
        }

        CatFrame = FromNumList[Num];

        //LOG.Error("猫的id:" + CatId + "--动作：" + CatAction+"--帧数："+ CatFrame);

        string Top = "";
        string Body = "";
        string Dwon = "";

        Top = StrinForm(CatNum);
        Body = StrinForm(CatAction);

        for (int i = 0; i < CatFrame; i++)
        {
            Dwon = StrinForm(i + 1);
            catResPath = Top + Body + Dwon;//获得该猫的动作帧图片序列
            CatResPath.Add(catResPath);

            //Debug.Log("catResPath:"+ catResPath);
        }

        return CatResPath;
    }

    private string StrinForm(int num)
    {
        string form = "";
        if (num >= 10)
        {
            form = num + "";
        }
        else
        {
            form = "0" + num;
        }

        return form;
    }

    #endregion

    public float GetPreLoadProgress()
    {
        if (m_needLoadAssetData.Count == 0)
        {
            return 0;
        }
        float p = 0;
        foreach (var d in m_needLoadAssetData)
        {
            p += d.Progress();
        }
        p /= m_needLoadAssetData.Count;
        return p;
    }

    /// <summary>
    /// 判断猫的资源是否已经加载完了
    /// </summary>
    /// <returns></returns>
    public bool IsCatPreLoadDone()
    {
        if (m_needLoadAssetData.Count == 0)
        {
            //没有资源要加载
            LOG.Info("没有需要加载的猫的资源");
            return true;
        }
        bool isDone = true;

        foreach (var d in m_needLoadAssetData)
        {

            if (!d.IsDone())
            {
                isDone = false;
                break;
            }
            else
            {
                if ((d is CAsset) && !recordDic.ContainsKey(((CAsset)d).assetName))
                {
                    recordDic.Add(((CAsset)d).assetName, true);
                    //Debug.Log("====isDone--->" + ((CAsset)d).assetName +"---len-->"+m_needLoadAssetData.Count);
                }
            }
        }
        return isDone;
    }


    //public CBundle PreLoadBundle(string strBundleName)
    //{
    //    if (!ABMgr.Instance.isUseAssetBundle)
    //    {
    //        return null;
    //    }
    //    var abFileName = AbUtility.NormalizerAbName(strBundleName);
    //    var context = ABSystem.ui.bundle;
    //    var abConfig = context.resConfig;
    //    var abConfigItem = abConfig.GetConfigItemByAbName(abFileName);
    //    var bundle = CBundle.Get(context, abConfigItem, false);
    //    if (bundle == null)
    //    {
    //        Debug.LogError("预加载失败:bundle=" + strBundleName);
    //        return null;
    //    }
    //    bundle.Retain(this);
    //    m_needLoadAssetData.Add(bundle);
    //    return bundle;
    //}

    /// <summary>
    /// 加载bundle资源
    /// </summary>
    /// <param name="resType"></param>
    /// <param name="strAssetName"></param>
    /// <param name="isChapterAsset">是否加载章节的资源</param>
    private void PreLoadAsset(enResType resType, string strAssetName, bool isChapterAsset = false)
    {
        var asset = ABSystem.ui.bundle(AbResBundle_DataTable.IsDataTableAsset(strAssetName)).LoadAsync(AbTag.Cat, resType, strAssetName);
        if (asset == null)
        {
            LOG.Error("预加载失败:type=" + resType + "," + strAssetName);
            return;
        }
        asset.Retain(this);
        m_needLoadAssetData.Add(asset);
        if (!mLastChapterResDic.ContainsKey(strAssetName))
            mLastChapterResDic.Add(strAssetName, asset);

        //LOG.Info("m_needLoadAssetData数量："+ m_needLoadAssetData.Count);
    }


    /// <summary>
    /// 删除猫的资源
    /// </summary>
    public void ReleasePreLoadData()
    {
        foreach (var data in mLastChapterResDic)
        {
            data.Value.Release(this);
        }
        foreach (var data in m_needLoadAssetData)
        {
            data.Release(this);
        }
        m_needLoadAssetData.Clear();
        ABMgr.Instance.GC();
    }
}
