using AB;
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;

public class NextUpBookItem : BaseUIForm
{
    public Image bookIcon;
    public Text txtBookName;
    public GameObject newImage,sexImage;
    private JDT_Book bookDetail;
    private int bookCount;
    private GameObject ChapterSwitch;
    //List<t_BookDetails> m_bookDetailList = new List<t_BookDetails>();
    public void Init(int bookid,GameObject ChapterSwitch)
    {
        UIEventListener.AddOnClickListener(bookIcon.gameObject, IconButtonOn);

        this.ChapterSwitch = ChapterSwitch;

        bookDetail = JsonDTManager.Instance.GetJDTBookDetailInfo(bookid);
        bookCount = bookDetail.chaptercount;

       // LOG.Info("推荐书本id:"+bookDetail.id);

        bookIcon.sprite = ABSystem.ui.GetUITexture(AbTag.Global,string.Concat("Assets/Bundle/BookPreview/Icon/", bookDetail.id, ".png"));
        txtBookName.text = bookDetail.bookname;

       

        TipsState();
    }

    public override void OnOpen()
    {
        base.OnOpen();
        
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(bookIcon.gameObject, IconButtonOn);
    }

    //这个是显示这本书是否有新的内容
    private void TipsState()
    {

        if (bookDetail.isNew==1)
        {
            newImage.SetActive(true);
        }else
        {
            newImage.SetActive(false);
        }

        // if (bookDetail.isLGBT==1)
        // {
        //     sexImage.SetActive(true);
        // }else
        // {
        //     sexImage.SetActive(false);
        // }
        //if (PlayerPrefs.GetInt("BookChapterCount" + bookDetail.id) == bookCount)
        //{
        //    newImage.SetActive(false);
        //}
        //else
        //{
        //    //表示这个本书是有新的内容
        //    newImage.SetActive(true);
        //}
    }

    public void IconButtonOn(PointerEventData data)
    {
        // LOG.Info("推荐书本被点击了");

        BookReadingWrapper.Instance.CloseForm();
        /*, useFormPool: true*/
        // CUIManager.Instance.OpenForm(UIFormName.MainForm/*, useFormPool: true*/);

        //打开主界面
        XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIMainForm);");
        CUIManager.Instance.OpenForm(UIFormName.BookDisplayForm);
        var view = CUIManager.Instance.GetForm<BookDisplayForm>(UIFormName.BookDisplayForm);
        view.InitByBookID(bookDetail.id, true);
        this.ChapterSwitch.SetActiveEx(false);
        EventDispatcher.Dispatch(EventEnum.BookProgressUpdate);
    }

    private void OnDisable()
    {
        Destroy(this.gameObject);
    }
}
