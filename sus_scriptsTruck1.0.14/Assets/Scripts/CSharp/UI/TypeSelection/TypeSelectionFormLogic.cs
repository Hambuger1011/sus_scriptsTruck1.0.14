using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using pb;

public class TypeSelectionFormLogic
{
    private TypeSelectionForm TypeSelectionForm;
    private string[] GeneroButtonName;

    public long pickBookTag = 0;
    private List<GeneroButtonItem> GeneroButtonItemList;
    public TypeSelectionFormLogic(TypeSelectionForm TypeSelectionForm)
    {
        this.TypeSelectionForm = TypeSelectionForm;

        pickBookTag = 0;
        GeneroButtonItemList = new List<GeneroButtonItem>();

        GeneroButtonName = TypeSelectionForm.GeneroButtonName;
        AddButttonName();
        SpwanGeneroButton();
        AddButtonListen();

        NuevoButtonOnclicke(null);
    }


    private void AddButttonName()
    {
        GeneroButtonName[(int)TypeSlectNumber.Button1- 3] = "Romance";
        GeneroButtonName[(int)TypeSlectNumber.Button2- 3] = "LGBTQ+";
        GeneroButtonName[(int)TypeSlectNumber.Button3- 3] = "Action";
        GeneroButtonName[(int)TypeSlectNumber.Button4- 3] = "Youth";
        GeneroButtonName[(int)TypeSlectNumber.Button5- 3] = "Adventure";
        GeneroButtonName[(int)TypeSlectNumber.Button6- 3] = "Drama";
        GeneroButtonName[(int)TypeSlectNumber.Button7- 3] = "Comedy";
        GeneroButtonName[(int)TypeSlectNumber.Button8- 3] = "Horror";
        GeneroButtonName[(int)TypeSlectNumber.Button9- 3] = "18+";
        GeneroButtonName[(int)TypeSlectNumber.Button10- 3] = "Fantasy";
        GeneroButtonName[(int)TypeSlectNumber.Button11- 3] = "Suspense";
        GeneroButtonName[(int)TypeSlectNumber.Button12- 3] = "Others";

    }
    private void AddButtonListen()
    {
        UIEventListener.AddOnClickListener(TypeSelectionForm.NuevoOn.transform.parent.gameObject, NuevoButtonOnclicke);
        UIEventListener.AddOnClickListener(TypeSelectionForm.PopularOn.transform.parent.gameObject, PopularOnclicke);
        UIEventListener.AddOnClickListener(TypeSelectionForm.OtrosOn.transform.parent.gameObject, OtrosOnclicke);
    }

    /// <summary>
    /// 生成挑选按钮
    /// </summary>
    private void SpwanGeneroButton()
    {
        for (int i=0;i< GeneroButtonName.Length;i++)
        {
            GameObject go = GameObject.Instantiate(TypeSelectionForm.GeneroButton);
            go.transform.SetParent(TypeSelectionForm.GenresList.transform);
            go.SetActive(true);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            GeneroButtonItem GeneroButtonItem = go.GetComponent<GeneroButtonItem>();
            GeneroButtonItem.Init(this,i+1, GeneroButtonName[i]);
            GeneroButtonItemList.Add(GeneroButtonItem);
        }
    }

    /// <summary>
    /// 增加挑选条件
    /// </summary>
    public void AddSelectionConditions(int tagIndex)
    {
        pickBookTag = BitUtils.SetBit64Mask(pickBookTag, tagIndex, true);
        GetMyPickBook(pickBookTag);
    }

    /// <summary>
    /// 删除挑选条件
    /// </summary>
    /// <param name="Index"></param>
    public void DeletSelectionConditions(int tagIndex)
    {
        pickBookTag = BitUtils.SetBit64Mask(pickBookTag, tagIndex, false);
        GetMyPickBook(pickBookTag);
    }

    /// <summary>
    /// 根据条件挑选出我想要获得的书本
    /// </summary>
    private void GetMyPickBook(long tagFlags)
    {
        TypeSelectionForm.TypeSelectionDB.SearchMyBook.Clear();
        List<BookNotUserInfo> Book = TypeSelectionForm.TypeSelectionDB.SelectBook;

        //Debug.Log("需要搜索的总数剧："+ Book.Count);

        for (int i=0;i< Book.Count;i++)
        {
           
            BookNotUserInfo BookInfo = Book[i];

            if (tagFlags != 0 && ((BookInfo.tagFlags & tagFlags) != tagFlags))
            {
                continue;
            }

            TypeSelectionForm.TypeSelectionDB.SearchMyBook.Add(BookInfo);
        }

        //Debug.Log("需要生成几本书："+ TypeSelectionForm.TypeSelectionDB.SearchMyBook.Count);
        //生成书本
        TypeSelectionForm.TypeSelectionFormView.SpwanItem();
    }

    public void NuevoButtonOnclicke(PointerEventData data)
    {
        ShowButton(0);

        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetBookNotUser(1, GetBookNotUserCallBacke);
    }

    public void PopularOnclicke(PointerEventData data)
    {
        ShowButton(1);
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.GetBookNotUser(2, GetBookNotUserCallBacke);
    }

    public void OtrosOnclicke(PointerEventData data)
    {
        ShowButton(2);
    }

    private void ShowButton(int Index)
    {    
        for (int i=0;i< TypeSelectionForm.On.Count;i++)
        {
            if (i== Index)
            {
                TypeSelectionForm.On[Index].SetActive(true);
            }else
            {
                TypeSelectionForm.On[i].SetActive(false);
            }
        }
    }

    private void GetBookNotUserCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----api_getBookNotUser---->" + result);
        if (result.Equals("error"))
        {
            return;
        }
        LoomUtil.QueueOnMainThread((param) =>
        {
            //UINetLoadingMgr.Instance.Close();
            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.BookNotUserList = JsonHelper.JsonToObject<HttpInfoReturn<BookNotUser>>(arg.ToString());

                    int bookNum=UserDataManager.Instance.BookNotUserList.data.book_list.Count;
                    TypeSelectionForm.TypeSelectionDB.SelectBook.Clear();

                    List<BookNotUserInfo> tempBookList = new List<BookNotUserInfo>();
                    for (int i=0;i< bookNum; i++)
                    {
                        BookNotUserInfo itemInfo = UserDataManager.Instance.BookNotUserList.data.book_list[i];
                        t_BookDetails bookDetail = GameDataMgr.Instance.table.GetBookDetailsById(itemInfo.id);
                        if (bookDetail != null && UserDataManager.Instance.BookOpenState(bookDetail.Availability))  //判断书本所属的区域
                        {
                            TypeSelectionForm.TypeSelectionDB.SelectBook.Add(itemInfo);
                            tempBookList.Add(itemInfo);
                        }  
                    }

                    //选择对于区域的书本内容
                    UserDataManager.Instance.BookNotUserList.data.book_list = tempBookList;
                    GetMyPickBook(pickBookTag);
                }
            }
        }, null);
    }

    public void Close()
    {
        UIEventListener.RemoveOnClickListener(TypeSelectionForm.NuevoOn.transform.parent.gameObject, NuevoButtonOnclicke);
        UIEventListener.RemoveOnClickListener(TypeSelectionForm.PopularOn.transform.parent.gameObject, PopularOnclicke);
        UIEventListener.RemoveOnClickListener(TypeSelectionForm.OtrosOn.transform.parent.gameObject, OtrosOnclicke);

        if (GeneroButtonItemList!=null)
        {
            for (int i=0;i< GeneroButtonItemList.Count;i++)
            {
                GeneroButtonItemList[i].DestroyGameObject();
            }
        }
    }
}
