using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class OfertesView
{
    private OfertesCtrl OfertesCtrl;
    private GameObject ADIteam, ADIteamPrent;
    private List<ADIteam> ADIteamList;
    private Text Time;
    private int TimeSequence;
    public OfertesView(OfertesCtrl OfertesCtrl)
    {
        this.OfertesCtrl = OfertesCtrl;
        if (ADIteamList == null)
            ADIteamList = new List<ADIteam>();

        FindGameObject();
    }

    private void FindGameObject()
    {
        ADIteam = OfertesCtrl.transform.Find("BG/ScrollView/Viewport/Content/BG/Prefb/ADIteam").gameObject;
        ADIteamPrent = OfertesCtrl.transform.Find("BG/ScrollView/Viewport/Content/BG/ADIteamList").gameObject;
        Time = OfertesCtrl.transform.Find("BG/ScrollView/Viewport/Content/BG/TimeBg/Time").GetComponent<Text>();
    }
    

    public void OnActiveInfoRefresh()
    {
        this.ShowADSItem();

        if(TimeSequence>0) 
            CTimerManager.Instance.RemoveTimer(TimeSequence);

        ShowTime();
    }

    private void ShowTime()
    {
        TimeSequence = CTimerManager.Instance.AddTimer(1000, -1, TimeReapety);
        TimeReapety(0);
    }
    private void TimeReapety(int seq)
    {
        if (Time == null)
        {
            return;
        }
        var TimeNumber = UserDataManager.Instance.lotteryDrawInfo.data.countdown;
        if (TimeNumber <= 0)
        {
            CTimerManager.Instance.RemoveTimer(TimeSequence);
            OfertesCtrl.OfertesLogic.GetADSInfo();
        }
        else
        {
            Time.text = string.Format("{0:d2}:{1:d2}:{2:d2}", TimeNumber / 3600, (TimeNumber / 60) % 60, TimeNumber % 60);
        }
        UserDataManager.Instance.lotteryDrawInfo.data.countdown--;
    }
    public void GifDiamanTextShow(string st)
    {
        if(OfertesCtrl.GifDiamanText != null)
           OfertesCtrl.GifDiamanText.text = st.ToString()+ " Diamonds";
    }

    public void GifKeyTextShow(string st)
    {
        if (OfertesCtrl.GifKeyText != null)
            OfertesCtrl.GifKeyText.text = st.ToString()+ " Keys";
    }

    public void GifPriceShow(string st)
    {
        if (OfertesCtrl.price != null)
            OfertesCtrl.price.text = "$"+st.ToString();
    }
    public void GifOldPrice(string st)
    {
        if (OfertesCtrl.DiscountPrice != null)
            OfertesCtrl.DiscountPrice.text = "$ "+st.ToString(); ;
    }
    public void GifBgShow(bool bo)
    {
        if(OfertesCtrl.GifBg!=null)
           OfertesCtrl.GifBg.SetActive(bo);
    }

    public void ShowADSItem()
    {
        ClearADItem();
        List<ADSInfo> ADSList = OfertesCtrl.OfertesDB.GetADSInfoList();
        if (ADSList == null) return;

        for (int i=0;i<ADSList.Count;i++)
        {
            if (ADIteam!=null)
            {
                GameObject go = GameObject.Instantiate(ADIteam);
                go.transform.SetParent(ADIteamPrent.transform);
                go.SetActive(true);
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                ADIteam IteamSprit = go.transform.GetComponent<ADIteam>();
                IteamSprit.Init(ADSList[i], OfertesCtrl);
                ADIteamList.Add(IteamSprit);
            }
          
        }
    }

    public void ShowADSItemStep()
    {
        if (ADIteamList != null)
        {
            for (int i = 0; i < ADIteamList.Count; i++)
            {
                ADIteamList[i].ShowStep();
            }
        }
    }

    public void Close()
    {
        ClearADItem();
        CTimerManager.Instance.RemoveTimer(TimeSequence);
    }

    void ClearADItem()
    {
        if (ADIteamList != null)
        {
            for (int i = 0; i < ADIteamList.Count; i++)
            {
                ADIteamList[i].DestroyGameObject();
            }
            ADIteamList.Clear();
        }
    }
}
