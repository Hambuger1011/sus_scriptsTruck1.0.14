using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;
using UnityEngine.UI;
using DG.Tweening;
using UGUI;
using WebSocketSharp;

public class UITipsMgr : CMonoSingleton<UITipsMgr>
{
    public float speed = 10;
    public GameObject[] itemTemplate;

    [System.NonSerialized]
    public RectTransform[] layers;
    [System.NonSerialized]
    public CUIForm uiForm;

    List<RectTransform> m_tipsItems = new List<RectTransform>(32);
    Stack<RectTransform> m_pools = new Stack<RectTransform>();

    RectTransform m_tipsItemPrefab;
    int m_idx = 0;


    int m_tipsNum = 0;
    float m_tipsRootHeight = 0;
    float m_sceenHeight;

    protected override void Init()
    {
        base.Init();
        uiForm = this.GetComponent<CUIForm>();

        layers = new RectTransform[3];
        for (int i=0;i< layers.Length; ++i)
        {
            layers[i] = this.transform.GetChild(i) as RectTransform;
        }
        m_sceenHeight = uiForm.m_referenceResolution.y * 0.5f;

    }
    

    

    private void SetupTransform(RectTransform rectTransform)
    {
        //rectTransform.offsetMin = new Vector2(0,1);
        //rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.pivot = new Vector2(0.5f, 0);
        //rectTransform.anchoredPosition = Vector2.zero;
    }

    void PlayTipsTween(RectTransform trans,float startY,float height,bool bFadeout = true)
    {
        ++m_tipsNum;
        float needMove = m_sceenHeight * uiForm.GetScale().y;// SceenHeight - m_tipsRootHeight;
        if(needMove < 0)
        {
            RemoveTopTips();
        }
		else if(bFadeout)
		{
			trans.GetComponent<CanvasGroup>().DOFade(0, 2).SetEase(Ease.Linear).OnComplete(RemoveTopTips).Play();
        }
        else
		{
			trans.DOAnchorPosY(needMove,0.8f).SetEase(Ease.InQuart).OnComplete(() =>
				{
					RemoveTopTips();
				}).SetDelay(1).Play();
        }
    }

    void RemoveTopTips()
    {
        --m_tipsNum;
        var trans = m_tipsItems[0];
        m_tipsItems.RemoveAt(0);
        //PushToPool(trans);
        GameObject.Destroy(trans.gameObject);
        if (m_tipsNum == 0)
        {
            m_tipsRootHeight = 0;
            layers[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_tipsRootHeight);
        }
    }

    //private void Update()
    //{
    //    for(int i=0; i < m_tipsItems.Count; ++i)
    //    {
    //        if(!ProccesTipsItem(m_tipsItems[i]))
    //        {
    //            m_tipsItems.RemoveAt(i);
    //            --i;
    //        }
    //    }
    //}

    //bool ProccesTipsItem(RectTransform tipsTrans)
    //{
    //    var localPos = tipsTrans.anchoredPosition;
    //    localPos.y += Time.deltaTime * speed;
    //    if(localPos.y >= 650)
    //    {
    //        PushToPool(tipsTrans);
    //        return false;
    //    }
    //    return true;
    //}

    void PushToPool(RectTransform trans)
    {
        m_pools.Push(trans);
        trans.gameObject.CustomSetActive(false);
    }

    RectTransform GetTipsItem()
    {
        if(m_pools.Count <= 0)
        {
            var newItem = GameObject.Instantiate(m_tipsItemPrefab);
            return newItem;
        }
        var t = m_pools.Pop();
        return t;
    }


    #region 弹文字tips
	public void PopupTips(string strMsg, bool bFadeout = true, int tipsTplIdx = 0)
    {
        GameObject prefab = itemTemplate[tipsTplIdx];
        var h = ((RectTransform)prefab.transform).rect.height;
		if (m_tipsItems.Count > 0)
		{
			var lastTrans = m_tipsItems[m_tipsItems.Count - 1];
			if (m_tipsRootHeight + lastTrans.anchoredPosition.y < h)
			{
				var height = layers[1].rect.height;
				m_tipsRootHeight = height + h;
                layers[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_tipsRootHeight);
			}
		}
		else
		{
			m_tipsRootHeight = h;
            layers[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_tipsRootHeight);
		}

        GameObject prefabBcItem = GameObject.Instantiate(prefab);
        var t = prefabBcItem.transform as RectTransform;
		t.SetParent(layers[1], false);
		SetupTransform(t);
		t.anchoredPosition = new Vector2(0, -m_tipsRootHeight);
		m_tipsItems.Add(t);
		t.Find ("Text").GetComponent<Text> ().text = strMsg;
		PlayTipsTween(t, -m_tipsRootHeight, h, bFadeout);
	}
    #endregion
    
    
    #region 弹文字tips
    public void ShowTips(string strMsg, bool bFadeout = false, int tipsTplIdx = 2)
    {
        if(strMsg.IsNullOrEmpty())
            return;
        GameObject prefab = itemTemplate[tipsTplIdx];
        var h = ((RectTransform)prefab.transform).rect.height;
        if (m_tipsItems.Count > 0)
        {
            var lastTrans = m_tipsItems[m_tipsItems.Count - 1];
            if (m_tipsRootHeight + lastTrans.anchoredPosition.y < h)
            {
                var height = layers[1].rect.height;
                m_tipsRootHeight = height + h;
                layers[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_tipsRootHeight);
            }
        }
        else
        {
            m_tipsRootHeight = h;
            layers[1].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_tipsRootHeight);
        }

        GameObject prefabBcItem = GameObject.Instantiate(prefab);
        var t = prefabBcItem.transform as RectTransform;
        t.SetParent(layers[1], false);
        SetupTransform(t);
        t.anchoredPosition = new Vector2(0, -m_tipsRootHeight);
        m_tipsItems.Add(t);
        t.Find ("BG/Text").GetComponent<Text> ().text = strMsg;
        PlayTipsTween(t, -m_tipsRootHeight, h, bFadeout);
    }
    #endregion

    private float mInterval = 1f;
    private float mCurTime = 0;
    void Update()
    {
        mCurTime += Time.deltaTime;
        if(mCurTime > mInterval)
        {
            mCurTime = 0;
            if(mInShowPopTips)
            {
                CheckLeftPopTips();
            }
        }
    }

    private List<PopTipsItemView> mItemViewList = new List<PopTipsItemView>();
    private List<string> mPopTipsList = new List<string>();
    private Vector2 mStartPos;
    private bool mInShowPopTips = false;
    public void ShowPopTips(string msg,Vector3 vScreenPos)
    {
        mStartPos = CUIUtility.Screen_To_UGUI_LocalPoint(this.uiForm.GetCamera(), vScreenPos, this.uiForm.rectTransform());
        if (!mInShowPopTips)
        {
            mCurTime = 0;
            mInShowPopTips = true;
            DoShowPopTipsItem(msg);
        }
        else
        {
            mPopTipsList.Add(msg);
        }
    }


    public void ShowUIPopTips(string msg, Vector3 uiLocalPos)
    {
        mStartPos = uiLocalPos;
        if (!mInShowPopTips)
        {
            mCurTime = 0;
            mInShowPopTips = true;
            DoShowPopTipsItem(msg);
        }
        else
        {
            mPopTipsList.Add(msg);
        }
    }

    private void CheckLeftPopTips()
    {
        if(mPopTipsList.Count>0)
        {
            DoShowPopTipsItem(mPopTipsList[0]);
            mPopTipsList.RemoveAt(0);
        }
        else
        {
            mInShowPopTips = false;
        }
    }

    private void DoShowPopTipsItem(string msg)
    {
        PopTipsItemView itemView = GetItemView();
        itemView.Init(msg,mStartPos, ItemShowFinishHandler);
    }

    private PopTipsItemView GetItemView()
    {
        PopTipsItemView itemView = null;

        if(mItemViewList.Count>0)
        {
            itemView = mItemViewList[0];
            mItemViewList.RemoveAt(0);
        }
        else
        {
            GameObject prefab = itemTemplate[1];
            GameObject prefabBcItem = GameObject.Instantiate(prefab);
            var t = prefabBcItem.transform as RectTransform;
            t.SetParent(layers[1], false);
            SetupTransform(t);
            itemView = t.GetComponent<PopTipsItemView>();
        }
        return itemView;
    }

    private void ItemShowFinishHandler(PopTipsItemView vItem)
    {
        mItemViewList.Add(vItem);
    }

}
