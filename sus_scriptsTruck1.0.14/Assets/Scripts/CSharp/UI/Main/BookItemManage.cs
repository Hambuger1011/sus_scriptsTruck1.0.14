using System;
using AB;
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookItemManage
{
    private static BookItemManage instance;
    public static BookItemManage Instance
    {
        get
        {
            if (instance==null)
            {
                instance = new BookItemManage();
            }

            return instance;
        }
    }


    /// <summary>
    /// 返回书本的数据信息
    /// </summary>
    /// <param name="BookId"></param>
    /// <returns></returns>
    public t_BookDetails GetBookDetails(int BookId)
    {
        t_BookDetails m_bookDetailCfg = GameDataMgr.Instance.table.GetBookDetailsById(BookId);
        return m_bookDetailCfg;
    }


    /// <summary>
    /// 得到书本Icon的图片
    /// </summary>
    /// <param name="BookId"></param>
    /// <returns></returns>
    public Sprite ShowIcon(int BookId)
    {
        t_BookDetails bookDetail = GetBookDetails(BookId);
        if (bookDetail == null) {
            Debug.LogError("[展示书本封面Icon报错：]  BookId:"+ BookId+" [书本配置表没有此书本Id]");
            return null;
        }
        Sprite Icon = ABSystem.ui.GetUITexture(AbTag.Global, string.Concat("Assets/Bundle/BookPreview/Icon/", (bookDetail.BookIcon), ".png"));
        return Icon;
    }

    public string ShowBanner(int BookId)
    {
        t_BookDetails bookDetail = GetBookDetails(BookId);
        if (bookDetail == null)
        {
            Debug.LogError("[展示书本封面Icon报错：]  BookId:" + BookId + " [书本配置表没有此书本Id]");
            return null;
        }
        return "banner_" + (bookDetail.BookIcon - 1);
    }

    /// <summary>
    /// 得到书本的名字
    /// </summary>
    /// <param name="BookId"></param>
    /// <returns></returns>
    public string GetBookName(int BookId)
    {
        t_BookDetails bookDetail = GetBookDetails(BookId);
        if (bookDetail == null)
        {
            Debug.LogError("[展示书本封面Icon报错：]  BookId:" + BookId + " [书本配置表没有此书本Id]");
            return null;
        }

        string Name = bookDetail.BookName.ToString();
        return Name;
    }

    /// <summary>
    /// 得到书本的进度
    /// </summary>
    /// <param name="BookId"></param>
    /// <returns></returns>
    public float GetPross(int BookId)
    {
        t_BookDetails bookDetail = GetBookDetails(BookId);
        if (bookDetail == null)
        {
            Debug.LogError("[展示书本封面Icon报错：]  BookId:" + BookId + " [书本配置表没有此书本Id]");
            return 0;
        }

        int lastDialogId=0;
        float Press = 0.0f;

        if (bookDetail.ChapterDivisionArray != null && bookDetail.ChapterDivisionArray.Length > 0)
        {
            lastDialogId = bookDetail.ChapterDivisionArray[bookDetail.ChapterDivisionArray.Length - 1];
        }

        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == bookDetail.id);
        if (bookData != null)
            Press = (float)(bookData.DialogueID / (lastDialogId * 1.0f));
        else
            Press = 0;

        return Press;
    }

    /// <summary>
    /// 判断这本书是不是新书
    /// </summary>
    /// <param name="BookId"></param>
    /// <returns></returns>
    public bool BookIsNew(int BookId)
    {
        t_BookDetails bookDetail = GetBookDetails(BookId);
        if (bookDetail == null)
        {
            Debug.LogError("[展示书本封面Icon报错：]  BookId:" + BookId + " [书本配置表没有此书本Id]");
            return false;
        }

        bool NewBook = bookDetail.NewBook == 1 ? true : false;
        return NewBook;
    }


}
