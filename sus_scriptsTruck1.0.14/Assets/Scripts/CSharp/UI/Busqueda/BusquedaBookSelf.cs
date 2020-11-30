using AB;
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using XLua;
using System;

public class BusquedaBookSelf : MonoBehaviour
{
    private BusquedaBookInfo m_bookInfo;
    private Text bookName;
    private Text authorName;
    private Image progress;
    private Text Chapter;
    private Text Content;
    private Text LookNumberText;

    private void Awake()
    {
        imgCover = this.transform.Find("iconMask/icon").GetComponent<Image>();
        defaultSize = imgCover.rectTransform.rect.size;
        defaultSprite = this.imgCover.sprite;


        bookName = transform.Find("name").GetComponent<Text>();
        authorName = transform.Find("AutorPost/AutorName").GetComponent<Text>();
        progress = transform.Find("prossBg/progress").GetComponent<Image>();
        Chapter = transform.Find("Chapter").GetComponent<Text>();
        Content = transform.Find("Content").GetComponent<Text>();
        LookNumberText = transform.Find("LookNumber/LookNumberText").GetComponent<Text>();



        UIEventListener.AddOnClickListener(gameObject, ItemOnclicke);
    }

    public void DestroyGameObject()
    {
        UIEventListener.RemoveOnClickListener(gameObject, ItemOnclicke);
        if (gameObject != null)
            Destroy(gameObject);
    }

    /// <summary>
    /// 这个是挑选出来的书本初始化
    /// </summary>
    public void init(BusquedaBookInfo bookInfo)
    {
        m_bookInfo = bookInfo;

        RefreshUI();
        LoadSprite();

        if (bookInfo.read_count > 1000)
        {
            LookNumberText.text = string.Format("{0}k", (bookInfo.read_count / 1000.0f).ToString("0.0"));
        }
        else
        {
            LookNumberText.text = bookInfo.read_count.ToString();
        }
    }


    void RefreshUI()
    {
        if (m_bookInfo != null)
        {
            bookName.text = m_bookInfo.title;
            authorName.text = m_bookInfo.writer_name;
            Chapter.text = string.Format("Chapter:{0}", m_bookInfo.total_chapter_count);
            Content.text = m_bookInfo.description;

            ToUpDateProgress();
        }
        else
        {
            bookName.text = CTextManager.Instance.GetText(280);
            progress.fillAmount = 0;
        }
      
        //GameHttpNet.Instance.GetBookDetailInfo(m_bookDetailCfg.id, GetBookDetailInfoCallBack);
    }

    private void ToUpDateProgress()
    {
        //int lastDialogId = 2000;
        //if (m_bookInfo != null)
        //{
        //    if (m_bookInfo.ChapterDivisionArray != null && m_bookInfo.ChapterDivisionArray.Length > 0)
        //    {
        //        lastDialogId = m_bookInfo.ChapterDivisionArray[m_bookInfo.ChapterDivisionArray.Length - 1];
        //    }

        //    BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == m_bookInfo.id);
        //    if (bookData != null)
        //        progress.fillAmount = (float)(bookData.DialogueID / (lastDialogId * 1.0f));
        //    else
        //        progress.fillAmount = 0;
        //}
    }


    private void ItemOnclicke(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.book_click);
        if (m_bookInfo == null)
        {
            UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(225)/*"COMING SOON"*/, GameDataMgr.Instance.table.GetLocalizationById(226)/*"This book will be available soon!\nStay tuned!"*/);
            return;
        }
        
        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString(@"
return function(bookID)
    logic.StoryEditorMgr:ReadingOtherBook(bookID)
end");
        using (var func = (LuaFunction)res[0])
        {
            func.Action<int>(this.m_bookInfo.id);
        }
    }

    private int m_downloadSeq = 1;
    private Image imgCover;
    private Vector2 defaultSize;
    private Sprite defaultSprite;
    void LoadSprite()
    {
        this.imgCover.sprite = defaultSprite;
        var downloadSeq = m_downloadSeq;

        var luaenv = XLuaManager.Instance.GetLuaEnv();
        var res = luaenv.DoString(@"
return function(filename, fileMd5, cover_image,callback)
        logic.StoryEditorMgr.data:LoadSprite(filename,fileMd5, cover_image,callback)
end"
);
        using (var func = (LuaFunction)res[0])
        {
            //var bookID = this.WriterBookList.id;
            string imgUrl;
            string md5;
            XLuaHelper.ParseImageUrl(this.m_bookInfo.cover_image, out imgUrl, out md5);
            var filename = GameUtility.WritablePath + "/cache/story_image/" + md5;
            var f = (Action<object, object, object, object>)func.Cast(typeof(Action<object, object, object, object>));
            f(filename, md5, imgUrl, new Action<Sprite>((sprite) =>
            {
                if (downloadSeq != m_downloadSeq)
                {
                    return;
                }
                if (this.imgCover == null)
                {
                    return;
                }
                if (sprite == null)
                {
                    sprite = this.defaultSprite;
                }
                //XLuaHelper.SetSprite(this.imgCover, sprite, defaultSize);
                var doSetCover = luaenv.DoString(@"
                    return function(uiImage,sprite,defaultSize)
                            logic.StoryEditorMgr:SetCover(uiImage,sprite,defaultSize)
                    end"
                    );
                using (var func2 = (LuaFunction)doSetCover[0])
                {
                    func2.Call(this.imgCover, sprite, defaultSize);
                }
            }));
        }
    }

}
