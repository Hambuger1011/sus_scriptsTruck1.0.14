using AB;
using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;

public class BookSelfBasse:MonoBehaviour
{
   

    /// <summary>
    /// 获取书本名字
    /// </summary>
    /// <param name="m_bookDetailCfg"></param>
    /// <returns></returns>
    public  string GetBookName(t_BookDetails m_bookDetailCfg)
    {
        return m_bookDetailCfg.BookName;
    }

    /// <summary>
    /// 获取书本的Icon
    /// </summary>
    /// <param name="m_bookDetailCfg"></param>
    /// <returns></returns>
    public  Sprite GetBookIcon(t_BookDetails m_bookDetailCfg)
    {
       return (Sprite)ABSystem.ui.GetUITexture(AbTag.Global, string.Concat("Assets/Bundle/BookPreview/Icon/", (m_bookDetailCfg.BookIcon), ".png"));

    }

    /// <summary>
    /// 获取书本的进度
    /// </summary>
    /// <param name="m_bookDetailCfg"></param>
    /// <returns></returns>
    public  float GetBookPross(t_BookDetails m_bookDetailCfg)
    {
        int lastDialogId = 2000;
        float vlue = 0;
        if (m_bookDetailCfg != null)
        {
            if (m_bookDetailCfg.ChapterDivisionArray != null && m_bookDetailCfg.ChapterDivisionArray.Length > 0)
            {
                lastDialogId = m_bookDetailCfg.ChapterDivisionArray[m_bookDetailCfg.ChapterDivisionArray.Length - 1];
            }

            BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == m_bookDetailCfg.id);
            if (bookData != null)
                vlue = (float)(bookData.DialogueID / (lastDialogId * 1.0f));
            else
                vlue= 0;
        }

        return vlue;
    }

}
