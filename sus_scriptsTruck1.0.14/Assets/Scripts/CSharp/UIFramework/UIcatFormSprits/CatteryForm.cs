using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatteryForm : CatBaseForm {

    private GameObject Mask;
    private GameObject MyCatteryItem;
    private ScrollRect ScrollView;
    private List<MyCatteryItem> MyCatteryItemList;
    private RectTransform frameTrans;


    private void Awake()
    {
        Mask = transform.Find("Mask").gameObject;
       
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_COLLECT;

        MyCatteryItem = transform.Find("BG/MyCatteryItem").gameObject;
        ScrollView = transform.Find("BG/Scroll View").GetComponent<ScrollRect>();
        frameTrans = transform.Find("BG").GetComponent<RectTransform>();

        if (MyCatteryItemList==null)
        {
            MyCatteryItemList = new List<MyCatteryItem>();
        }
        MyCatteryItemList.Clear();

        UIEventListener.AddOnClickListener(Mask, CloseUi);
        GameHttpNet.Instance.Getadoptpet(CallBacketadoptpet);

        EventDispatcher.AddMessageListener(EventEnum.CatteryFormClose, CatteryFormClose);

        
        if (GameUtility.IpadAspectRatio())
        {
            this.frameTrans.localScale = new Vector3(0.7f, 0, 1);
            this.frameTrans.DOScaleY(0.7f, 0.25f).SetEase(Ease.OutBack).Play();
        }else
        {
            StartShow();

        }

        Transform CloseButton = transform.Find("BG/CloseButton");
        CloseButton.GetComponent<RectTransform>().DOAnchorPosY(-590, 0.4f).SetEase(Ease.OutBack);
    }

    public override void OnClose()
    {
        base.OnClose();

        UIEventListener.RemoveOnClickListener(Mask, CloseUi);

        EventDispatcher.RemoveMessageListener(EventEnum.CatteryFormClose, CatteryFormClose);


        for (int i=0;i< MyCatteryItemList.Count;i++)
        {
            MyCatteryItemList[i].Dispote();
        }
    }


    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    private void CatteryFormClose(Notification notification)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    /// <summary>
    /// 界面刚出现的效果显示
    /// </summary>
    private void StartShow()
    {

        frameTrans.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        frameTrans.DOScale(new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutBack);
    }

    private void CallBacketadoptpet(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----getadoptpet-CallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.Getadoptpet = JsonHelper.JsonToObject<HttpInfoReturn<Getadoptpet>>(result);
                    if (UserDataManager.Instance.Getadoptpet != null)
                    {
                       // Debug.Log("max_pet:" + UserDataManager.Instance.Getadoptpet.data.max_pet);

                        List<Getadoptpetpetarr> tem = UserDataManager.Instance.Getadoptpet.data.petarr;

                        SpwanCatteryItem(tem);

                       // Debug.Log("pet_name:" + tem[0].pet_name);

                    }

                }

            }, null);
        }
    }

    private void SpwanCatteryItem(List<Getadoptpetpetarr> tem)
    {
        int CatNum = 0;//这个是动物的编号
        int CatNoAdopt = 0;//没有收养的数量
        int CatHadAdopt = 0;//已经收养的数量
        int CatLock = 0;//未解锁的猫的数量

        CatNoAdopt = UserDataManager.Instance.Getadoptpet.data.max_pet - tem.Count;
        CatHadAdopt = tem.Count;
        CatLock = UserDataManager.Instance.Getadoptpet.data.gold_pet - UserDataManager.Instance.Getadoptpet.data.max_pet;

      
        //生成已经收养的猫
        for (int i=0;i< CatHadAdopt;i++)
        {
            CatNum++;

            GameObject go = Instantiate(MyCatteryItem);
            go.SetActive(true);
            go.transform.SetParent(ScrollView.content);
            go.transform.localScale = Vector3.one;
            MyCatteryItem MycatterySprite = go.GetComponent<MyCatteryItem>();
            MycatterySprite.Inite(CatNum,1);
            MyCatteryItemList.Add(MycatterySprite);
        }

        //生成没有收养的猫
        for (int i = 0; i < CatNoAdopt; i++)
        {
            CatNum++;
            GameObject go = Instantiate(MyCatteryItem);
            go.SetActive(true);
            go.transform.SetParent(ScrollView.content);
            go.transform.localScale = Vector3.one;
            MyCatteryItem MycatterySprite = go.GetComponent<MyCatteryItem>();
            MycatterySprite.Inite(CatNum, 2);
            MyCatteryItemList.Add(MycatterySprite);
        }
        string num = UserDataManager.Instance.Getadoptpet.data.level_next_str;
        string[] numS = num.Split(',');

        //生成没有解锁的猫
        for (int i = 0; i < CatLock; i++)
        {
            CatNum++;
            GameObject go = Instantiate(MyCatteryItem);
            go.SetActive(true);
            go.transform.SetParent(ScrollView.content);
            go.transform.localScale = Vector3.one;
            MyCatteryItem MycatterySprite = go.GetComponent<MyCatteryItem>();
            MycatterySprite.Inite(CatNum, 3);
            MyCatteryItemList.Add(MycatterySprite);

            string st = "Level " + numS[i] + " garden to unlock";
            MycatterySprite.LockTextShow(st);
        }
    }

}
