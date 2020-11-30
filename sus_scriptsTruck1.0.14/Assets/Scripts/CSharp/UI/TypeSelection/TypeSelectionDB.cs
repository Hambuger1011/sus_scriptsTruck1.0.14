using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeSelectionDB
{
    private List<BookNotUserInfo> selectBook;  //这个是热门或者最新的书本的集合
    private List<BookNotUserInfo> searchMyBook;//这个是按照条件挑选出来的书本集合

    public List<BookNotUserInfo> SelectBook
    {
        get
        {
            if (selectBook == null)
            {
                selectBook = new List<BookNotUserInfo>();
            }
            return selectBook;
        }
        set
        {
            if (selectBook==null)
            {
                selectBook = new List<BookNotUserInfo>();
            }

            selectBook = value;
        }
    }

    public List<BookNotUserInfo> SearchMyBook
    {
        get
        {
            if (searchMyBook == null)
            {
                searchMyBook = new List<BookNotUserInfo>();
            }
            return searchMyBook;
        }
        set
        {
            if (searchMyBook == null)
            {
                searchMyBook = new List<BookNotUserInfo>();
            }

            searchMyBook = value;
        }
    }
}
