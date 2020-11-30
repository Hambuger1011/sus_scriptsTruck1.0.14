using System.Collections;
using System;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using pb;
using System.Text;
using AB;
using Spine.Unity;
using System.Collections.Generic;

public class CatClothForm : MonoBehaviour {

    public int CatId;
    public int ShopId;

    private Image mCatIcon;
    private firstpetarr firstpetarr;

    private Coroutine mCorou;
    public SkeletonGraphic mSkeGra;
    private RectTransform target;
    private bool hadSetSize = false;
    public void Inite(firstpetarr firstpetarr)
    {
        mSkeGra = transform.GetComponent<SkeletonGraphic>();
        target = gameObject.GetComponent<RectTransform>();
      
        this.firstpetarr = firstpetarr;
        CatId = firstpetarr.pid;
        ShopId = firstpetarr.shop_id;
        gameObject.name = CatId + "_cat";

        //if (mCatIcon == null) mCatIcon = gameObject.GetComponent<Image>();
        UIEventListener.AddOnClickListener(gameObject, CatOnbuttonClicke);
        if (mCorou != null) StopCoroutine(mCorou);

        PlayCatFrame(firstpetarr.pid, UserDataManager.Instance.GetCatAtion(firstpetarr.pid));

        EventDispatcher.AddMessageListener(EventEnum.SetTringerRange, SetTringerRange);
    }

    /// <summary>
    /// 这里是设置猫的点击区域大小
    /// </summary>
    /// <param name="notification"></param>
    private void SetTringerRange(Notification notification)
    {       
        if (mSkeGra == null|| hadSetSize) return;
        var mesh = mSkeGra.GetLastMesh();
        var b = new Bounds();
        List<Vector3> vertices = new List<Vector3>();
        mesh.GetVertices(vertices);

        if (vertices.Count>0)
        {
            foreach (var itr in vertices)
            {
                b.Encapsulate(itr);
            }

            if (target == null) return;
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, b.size.x);
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, b.size.y);
            target.anchoredPosition = mSkeGra.rectTransform.anchoredPosition;
            hadSetSize = true;
        }

        //Debug.Log("fdadfa");
    }

    private void AnimationEnd(Spine.TrackEntry trackEntry)
    {
        if (UserDataManager.Instance.isFirstCatEnt)
        {
            return;
        }
        //LOG.Info("动画播放结束");
        SetTringerRange(null);
    }

    private int cid, aid;
    /// <summary>
    /// 播放猫的序列帧
    /// </summary>
    /// <param name="image"></param>
    /// <param name="cid">猫宠物ID</param>
    /// <param name="aid">动作ID</param>
    /// <param name="frameCount">帧数</param>
    public void PlayCatFrame(int cid, int aid)
    {
        this.cid = cid;
        this.aid = aid;

        LOG.Error("猫：" + cid + "--正在播放的动作：" + aid);

        string ForCid = cid.ToString("D2");
        string ForAid = aid.ToString("D2");
        string Num = ForCid + ForAid + "01";
        LOG.Info("ForCid:" + ForCid + "--ForAid:" + ForAid + "--Num:" + Num);

        SkeletonDataAsset obj = ABSystem.ui.GetObject(AbTag.DialogDisplay, "Assets/Bundle/Catre/" + cid + "/" + ForAid + "/" + Num + "_SkeletonData.asset") as SkeletonDataAsset;

        if (mSkeGra == null)
        {
            mSkeGra = transform.GetComponent<SkeletonGraphic>();
        }
        mSkeGra.skeletonDataAsset = obj;
        mSkeGra.initialSkinName = "default";
        mSkeGra.startingAnimation = "animation";
        mSkeGra.Initialize(true);
        mSkeGra.startingLoop = true;
        mSkeGra.enabled = true;

        mSkeGra.AnimationState.Complete += AnimationEnd;

        //t_gif tmpGif = GameDataMgr.Instance.table.GetCatGifById(cid, aid);
        //StringBuilder sb = new StringBuilder();
        //sb.Append("assets/Bundle/Cat/");
        //sb.Append(cid.ToString("D2"));
        //sb.Append(aid.ToString("D2"));
        //if (tmpGif == null)
        //{
        //    LOG.Info("找不到Gif数据");//該動作只有一幀
        //    sb.Append("01");
        //    mCatIcon.sprite = ABSystem.ui.GetUITexture(AbTag.Global,sb.ToString() + ".png");
        //    mCatIcon.SetNativeSize();
        //    return;
        //}
        //int frameCount = 0;
        //var tmp = tmpGif.frame.Split(',');
        //frameCount = tmp.Length;

        //if (frameCount == 1)
        //{
        //    sb.Append(frameCount.ToString("D2"));
        //    mCatIcon.sprite = ABSystem.ui.GetUITexture(AbTag.Global,sb.ToString() + ".png");
        //    mCatIcon.SetNativeSize();
        //    return;
        //}
        //else
        //{
        //    mCorou = StartCoroutine(AnimationPlayThread(mCatIcon, cid, aid, tmpGif, frameCount));
        //}
    }



    /// <summary>
    /// 播放序列帧
    /// </summary>
    /// <param name="img">目标图片</param>
    /// <param name="cid">宠物ID</param>
    /// <param name="aid">动作ID</param>
    /// <param name="gif">动作数据</param>
    /// <param name="count">帧数</param>
    /// <returns></returns>
    IEnumerator AnimationPlayThread(Image img, int cid, int aid, t_gif gif, int count)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("assets/Bundle/Cat/");
        sb.Append(cid.ToString("D2"));
        sb.Append(aid.ToString("D2"));
        int i = 1;
        int tmploop = 0;
        int tmpmin = int.Parse(gif.num.Split(',')[0]);
        int tmpmax = int.Parse(gif.num.Split(',')[1]);
        int loop = GetRandom(tmpmin, tmpmax);//獲取循環次數
        int tmp_min = int.Parse(gif.frequency.Split(',')[0]);
        int tmp_max = int.Parse(gif.frequency.Split(',')[1]);
        int frequence = GetRandom(tmp_min, tmp_max);//跟据表里數據獲取播放頻率
        string[] id = gif.frame.Split(',');
        bool flag = true;
        while (flag)
        {
            if (i <= count)
            {
                int picid = int.Parse(id[i - 1]);
                if (img != null)
                {
                    img.sprite = ABSystem.ui.GetUITexture(AbTag.Global,sb.ToString() + picid.ToString("D2") + ".png");
                    img.SetNativeSize();

                }
            }
            else
            {
                i = 1;
                int picid = int.Parse(id[i - 1]);
                if (img != null)
                {
                    img.sprite = ABSystem.ui.GetUITexture(AbTag.Global,sb.ToString() + picid.ToString("D2") + ".png");
                    img.SetNativeSize();
                    tmploop += 1;
                    if (tmploop == loop)
                    {
                        tmploop = 0;
                        yield return new WaitForSeconds(frequence);
                    }
                }
            }

            string[] tmpsec = gif.time.Split(',');
            float tmpflo = Convert.ToSingle(tmpsec[i - 1]);//每一幀的控制時間
            i++;
            yield return new WaitForSeconds(tmpflo);
        }
    }
    /// <summary>
    /// 获取随机数
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <returns></returns>
    private int GetRandom(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    private void CatOnbuttonClicke(PointerEventData data)
    {
       
        if (UserDataManager.Instance.InPlaceCatThings != 0) return;
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.CatDetails);
        CUIManager.Instance.GetForm<CatDetailsForm>(UIFormName.CatDetails).Inite(firstpetarr.pid,1,null);
    }
    /// <summary>
    /// 这个是猫的收养结果返回
    /// </summary>
    /// <param name="st"></param>
    private void CatAdoptionCallBack(string st)
    {
        //这个表示猫收养成功了
        //gameObject.SetActive(false);
    }
    public void Dispose()
    {
        if(mCorou != null)
            StopCoroutine(mCorou);
        //mCatIcon.sprite = null;
        UIEventListener.RemoveOnClickListener(gameObject, CatOnbuttonClicke);
        EventDispatcher.RemoveMessageListener(EventEnum.SetTringerRange, SetTringerRange);

        mSkeGra.AnimationState.Complete -= AnimationEnd;

        GameObject.Destroy(this.gameObject);
    }
}
