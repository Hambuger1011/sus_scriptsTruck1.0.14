using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class ScrollViewListener : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{

    private float smooting;                          //滑动速度
    private float normalSpeed = 5;
    private float highSpeed = 100;
    private int pageCount = 1;                           //每页显示的项目
    public GameObject listItem;  //content 物体
    private ScrollRect sRect;
    private float pageIndex;                            //总页数
    private bool isDrag = false;                                //是否拖拽结束
    private List<float> listPageValue = new List<float> { 0 };  //总页数索引比列 0-1


    private float m_targetPos = 0;                                //滑动的目标位置

    private float nowindex = 0;                                 //当前位置索引
    private float beginDragPos;
    private float endDragPos;

    private float sensitivity = 0.15f;                          //灵敏度

    public Button nextPage;
    public Button prePage;
    public Text pageNumber;//显示页数 1/3 页
    private int onePageCount = 10;

    private int mChildNum;

    public ChargeTipsForm ChargeTipsForms;
    void Awake()
    {
        sRect = this.GetComponent<ScrollRect>();
        //Invoke("InitInfo", 0.3f);
    } 
    public void InitInfo(int vChildNum)
    {
        mChildNum = vChildNum;
        CancelInvoke();
        ListPageValueInit();
        smooting = normalSpeed;

        pageIndex = (mChildNum / pageCount) - 1;
        pageNumber.text = (nowindex + 1) + "/" + (pageIndex + 1);

        ButtonInit();

        ChargeTipsForms.BtnOKInfoChange((int)nowindex);

        buttonFo();
    }

    //每页比例
    void ListPageValueInit()
    {
        pageIndex = (mChildNum / pageCount) - 1;
        if (listItem != null && mChildNum != 0)
        {
            for (float i = 1; i <= pageIndex; i++)
            {
                listPageValue.Add((i / pageIndex));
            }
        }
    }

    void ButtonInit()
    {
        nextPage.onClick.AddListener(BtnRightGo);
        prePage.onClick.AddListener(BtnLeftGo);
    }

    void OnDestroy()
    {
        nextPage.onClick.RemoveListener(BtnRightGo);
        prePage.onClick.RemoveListener(BtnLeftGo);
    }

    void Update()
    {
        if (!isDrag)
            sRect.horizontalNormalizedPosition = Mathf.Lerp(sRect.horizontalNormalizedPosition, m_targetPos, Time.deltaTime * smooting);
    }

    /// <summary>
    /// 拖动开始
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        beginDragPos = sRect.horizontalNormalizedPosition;
    }

    /// <summary>
    /// 拖拽结束
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        endDragPos = sRect.horizontalNormalizedPosition; //获取拖动的值
        endDragPos = endDragPos > beginDragPos ? endDragPos + sensitivity : endDragPos - sensitivity;
        int index = 0;
        float offset = Mathf.Abs(listPageValue[index] - endDragPos);    //拖动的绝对值
        for (int i = 1; i < listPageValue.Count; i++)
        {
            float temp = Mathf.Abs(endDragPos - listPageValue[i]);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }
        m_targetPos = listPageValue[index];
        nowindex = index;

        pageNumber.text = (nowindex + 1) + "/" + (pageIndex + 1);
        ChargeTipsForms.BtnOKInfoChange((int)nowindex);
    }

    public void BtnLeftGo()
    {
        nowindex = Mathf.Clamp(nowindex - 1, 0, pageIndex);
        m_targetPos = listPageValue[Convert.ToInt32(nowindex)];

        pageNumber.text = (nowindex + 1) + "/" + (pageIndex + 1);
        ChargeTipsForms.BtnOKInfoChange((int)nowindex);
    }

    /// <summary>
    /// 把选中中间的档数
    /// </summary>
    private void buttonFo()
    {
        int mu = 0;
        int paytotalCount = 0;
        float Paytotal = 0;
        //返回商品付费的类型 1.key  2.钻石
        int type = ChargeTipsForms.returnbuyType();
        ArrayList paytotalPriceList = MyBooksDisINSTANCE.Instance.paytotalTypeListGet();
        paytotalCount = paytotalPriceList.Count;
       
        if (type==1)
        {
            //获得key的档次
            Paytotal =float.Parse(UserDataManager.Instance.userInfo.data.userinfo.paybkeytotal);//获得付费的档次的价格
        }
        else
        {
            //获得钻石的档次
            Paytotal = float.Parse(UserDataManager.Instance.userInfo.data.userinfo.paydiamondtotal);//获得付费的档次的价格
        }

        for (int i=0;i<paytotalCount;i++)
        {
            float price = (float)paytotalPriceList[i];

            if (price== Paytotal)
            {
                //得到对应的价格档数
                mu = i + 1;
            }
        }

        if (mu == 0|| Paytotal==0)
        {
            LOG.Info("没有推荐的价格");
            return;
        }
        for (int i=0;i<mu;i++)
        {
            BtnRightGo();
        }
        //LOG.Info("付费价格列表中保存的个数是：" + paytotalCount + "--保存的档数价格是：" + Paytotal + "--应该推荐的价格是："+ mu);
    }
    

    public void BtnRightGo()
    {
        nowindex = Mathf.Clamp(nowindex + 1, 0, pageIndex);
        m_targetPos = listPageValue[Convert.ToInt32(nowindex)];

        pageNumber.text = (nowindex + 1) + "/" + (pageIndex + 1);
        ChargeTipsForms.BtnOKInfoChange((int)nowindex);
    }
}

