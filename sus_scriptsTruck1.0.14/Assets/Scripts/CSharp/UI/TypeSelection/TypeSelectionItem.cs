using AB;
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using GameCore.UI;
using System;

public class TypeSelectionItem : MonoBehaviour
{
    private Image bookIcon;
    private JDT_Book m_bookDetailCfg;
    private Text name;
    private Image progress;
    private Text Chapter;
    private Text Content;
    private Text LookNumberText;

    private void Awake()
    {
        UIEventListener.AddOnClickListener(gameObject, ItemOnclicke);
    }

    /// <summary>
    /// 这个是挑选出来的书本初始化
    /// </summary>
    public void init(JDT_Book BookDetails,int readcount=0)
    {
        m_bookDetailCfg = BookDetails;
        bookIcon = transform.Find("icon").GetComponent<Image>();
        name = transform.Find("name").GetComponent<Text>();
        progress = transform.Find("prossBg/progress").GetComponent<Image>();
        Chapter = transform.Find("Chapter").GetComponent<Text>();
        Content = transform.Find("Content").GetComponent<Text>();
        LookNumberText = transform.Find("LookNumber/LookNumberText").GetComponent<Text>();
        RefreshUI();
        if (readcount > 1000)
        {
            LookNumberText.text = string.Format("{0}k", (readcount / 1000.0f).ToString("0.0"));
        }
        else
        {
            LookNumberText.text = readcount.ToString();
        }
    }

    private void ItemOnclicke(PointerEventData data)
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
        
          
    }

    void RefreshUI()
    {
        if (m_bookDetailCfg == null) return;
        if (m_bookDetailCfg != null)
        {
            name.text = m_bookDetailCfg.bookname;         
            bookIcon.sprite = ABSystem.ui.GetUITexture(AbTag.Global, string.Concat("assets/bundle/BookPreview/Icon/", (m_bookDetailCfg.id), ".png"));

            ToUpDateProgress();

            Chapter.text = string.Format("Chapter:{0}", m_bookDetailCfg.chaptercount);
            //Content.text = m_bookDetailCfg.ChapterDiscriptionArray[0];

        }
        else
        {
            name.text = CTextManager.Instance.GetText(280);
            bookIcon.sprite = ABSystem.ui.GetUITexture(AbTag.Global, "assets/bundle/BookPreview/Icon/1.png");
            progress.fillAmount = 0;
        }
      
        //GameHttpNet.Instance.GetBookDetailInfo(m_bookDetailCfg.id, GetBookDetailInfoCallBack);
    }

    private void ToUpDateProgress()
    {
        int lastDialogId = 2000;
        if (m_bookDetailCfg != null)
        {
            JDT_Chapter chapterInfo = JsonDTManager.Instance.GetJDTChapterInfo(m_bookDetailCfg.id,m_bookDetailCfg.chaptercount);
            if (chapterInfo !=  null)
            {
                lastDialogId = chapterInfo.chapterfinish;
                if (lastDialogId == 0)
                    lastDialogId = 1;
            }

            BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == m_bookDetailCfg.id);
            if (bookData != null)
                progress.fillAmount = (float)(bookData.DialogueID / (lastDialogId * 1.0f));
            else
                progress.fillAmount = 0;
        }
    }

   
    public void DestroyGameObject()
    {
        UIEventListener.RemoveOnClickListener(gameObject, ItemOnclicke);
        if(gameObject!=null)
           Destroy(gameObject);
    }

}
