using AB;
using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfileMyBookItem : MonoBehaviour
{
    private Image Ima;
    private JDT_Book m_bookDetailCfg;
    private Text BookName;
    private Image Pross;
    public string bookTypeName = "";
    private Image BookTypeImg;
    private GameObject BookFree;
    private GameObject Tips;
    private GameObject DayPassBg;
    public void Init(JDT_Book cfg)
    {
        FindGameobject();
        m_bookDetailCfg = cfg;
        Ima = transform.GetComponent<Image>();

        Tips = DisplayUtil.GetChild(this.gameObject, "Tips");

        Ima.sprite = ABSystem.ui.GetUITexture(AbTag.Global, string.Concat("assets/bundle/BookPreview/Icon/", (cfg.id), ".png"));

        BookName.text = m_bookDetailCfg.bookname.ToString();

        DayPassBg = DisplayUtil.GetChild(this.gameObject, "DayPassBg");


        UIEventListener.AddOnClickListener(gameObject, OnBookClick);

        ToUpDateProgress();



        if (m_bookDetailCfg != null)
        {
            int bookTypeIndex = int.Parse(m_bookDetailCfg.Type1Array[0]);
            //通过编号获取string
            string bookTypeStr = UserDataManager.Instance.GetBookTypeName(bookTypeIndex);
            BookTypeImg.sprite = ResourceManager.Instance.GetUISprite("Common/com_bq-" + bookTypeStr);
            BookTypeImg.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("m_bookDetailCfg is Null");
        }
        //【限时活动免费读书 显示标签】
        this.Limit_time_Free(this.BookFree);
        
        this.DayPass(cfg.id);
    }


    /// <summary>
    /// 【限时活动免费读书 显示标签】
    /// </summary>
    public void Limit_time_Free(GameObject obj)
    {
        if (obj != null)
        {
            //【调用lua公共方法 限时活动免费读书 显示标签】
            XLuaManager.Instance.CallFunction("GameHelper", "ShowFree", obj);
        }
    }

    public void DayPass(int bookId)
    {
        bool boo = false;
        if (XLuaHelper.DayPassDic.Count <= 0) { return; }

        foreach (var item in XLuaHelper.DayPassDic)
        {
            if (item.Key == bookId)
            {
                boo = true;
                break;
            }

        }

        if (boo == true)
        {
            this.DayPassBg.SetActive(true);
        }
        else
        {
            this.DayPassBg.SetActive(false);
        }
    }




    private void FindGameobject()
    {
        BookName = transform.Find("BookName").GetComponent<Text>();
        Pross = transform.Find("ProssBg/Pross").GetComponent<Image>();
        BookTypeImg = transform.Find("BookTypeImg").GetComponent<Image>();
        BookFree = transform.Find("BookFree").gameObject;
    }

    private void ToUpDateProgress()
    {
        int lastDialogId = 2000;
        if (m_bookDetailCfg != null)
        {
            // if (m_bookDetailCfg.ChapterDivisionArray != null && m_bookDetailCfg.ChapterDivisionArray.Length > 0)
            // {
            //     lastDialogId = m_bookDetailCfg.ChapterDivisionArray[m_bookDetailCfg.ChapterDivisionArray.Length - 1];
            // }

            // BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == m_bookDetailCfg.id);
            // if (bookData != null)
            //     Pross.fillAmount = (float)(bookData.DialogueID / (lastDialogId * 1.0f));
            // else
            //     Pross.fillAmount = 0;

            //【加载进度】
            XLuaManager.Instance.CallFunction("GameHelper", "ShowProgress1", m_bookDetailCfg.id, Pross, Tips);
        }
    }

    private void OnBookClick(PointerEventData data)
    {

        if (m_bookDetailCfg == null)
        {
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(225)/*"COMING SOON"*/, GameDataMgr.Instance.table.GetLocalizationById(226)/*"This book will be available soon!\nStay tuned!"*/);
            return;
        }

        int bookID = m_bookDetailCfg.id;
        UserDataManager.Instance.UserData.CurSelectBookID = bookID;

        AudioManager.Instance.PlayTones(AudioTones.book_click);
        CUIManager.Instance.OpenForm(UIFormName.BookDisplayForm);
        var view = CUIManager.Instance.GetForm<BookDisplayForm>(UIFormName.BookDisplayForm);
        view.InitByBookID(UserDataManager.Instance.UserData.CurSelectBookID);


        if (this.bookTypeName == "mybookItem")
        {
            Debug.LogError("ProfileMyBookItem");
            //埋点*我的书本
            GamePointManager.Instance.BuriedPoint(EventEnum.MyFavoriteBook);
        }

    }



    public void Dispose()
    {
        UIEventListener.RemoveOnClickListener(gameObject, OnBookClick);
        if (Ima.sprite!=null)
           Ima.sprite = null;

    }
}
