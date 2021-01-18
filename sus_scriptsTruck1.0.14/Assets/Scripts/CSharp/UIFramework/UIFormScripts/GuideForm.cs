using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using pb;
using DG.Tweening;
using UGUI;
public class GuideForm : BaseUIForm
{

    private CanvasGroup BookGroup;
    private Image bookIcon1;
    private Image bookIcon2;
    private Image bookIcon3;
    private Text BookName1;
    private Text BookName2;
    private Text BookName3;
    private Text DescText;
    private bool isSelect;
    private List<GameObject> BookIconList;

    private List<RecommendABookList> recommendList;

    private void Awake()
    {
        BookIconList = new List<GameObject>();
        BookGroup = transform.Find("Canvas/BookGroup").GetComponent<CanvasGroup>();

        bookIcon1 = transform.Find("Canvas/BookGroup/LayoutGroup/BookItem1").GetComponent<Image>();
        BookIconList.Add(bookIcon1.gameObject);

        bookIcon2 = transform.Find("Canvas/BookGroup/LayoutGroup/BookItem2").GetComponent<Image>();
        BookIconList.Add(bookIcon2.gameObject);

        bookIcon3 = transform.Find("Canvas/BookGroup/LayoutGroup/BookItem3").GetComponent<Image>();
        BookIconList.Add(bookIcon3.gameObject);

        //BookName1 = transform.Find("Canvas/BookGroup/BookItem1/BookName").GetComponent<Text>();
        //BookName2 = transform.Find("Canvas/BookGroup/BookItem2/BookName").GetComponent<Text>();
        //BookName3 = transform.Find("Canvas/BookGroup/BookItem3/BookName").GetComponent<Text>();

        DescText = transform.Find("Canvas/BookGroup/Text").GetComponent<Text>();

        for (int i=0;i< BookIconList.Count;i++)
        {
            BookIconList[i].SetActive(false);
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(bookIcon1.gameObject, BookItem1ClickHandler);
        UIEventListener.AddOnClickListener(bookIcon2.gameObject, BookItem2ClickHandler);
        UIEventListener.AddOnClickListener(bookIcon3.gameObject, BookItem3ClickHandler);

        //LogoIcon.rectTransform.anchoredPosition = new Vector2(35, 170);
        BookGroup.alpha = 0;
        //LogoIcon.rectTransform.DOAnchorPos(new Vector2(35, 332), 0.4f).SetDelay(0.2f).Play();
        BookGroup.DOFade(1, 1).SetDelay(0.3f).Play();
        isSelect = false;

        if (DescText != null)
            DescText.text = "Welcome to Secrets! What kind of story would you like to play?";

        //UINetLoadingMgr.Instance.Show();

        float loginTime = 0;
        if (!GameUtility.LoginSuccFlag)
        {
            GameUtility.LoginSuccFlag = true;
            loginTime = Time.time - GameUtility.LoginConsumeTime;
            //LOG.Error("----LoginTime1--->" + loginTime);
        }
        GameHttpNet.Instance.GetRecommendABook(loginTime,RecommendABookCallBack);

        EventDispatcher.Dispatch(EventEnum.NavigationClose);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(bookIcon1.gameObject, BookItem1ClickHandler);
        UIEventListener.RemoveOnClickListener(bookIcon2.gameObject, BookItem2ClickHandler);
        UIEventListener.RemoveOnClickListener(bookIcon3.gameObject, BookItem3ClickHandler);

        if (BookIconList != null)
            BookIconList = null;
    }

    public void BookItem1ClickHandler(UnityEngine.EventSystems.PointerEventData data)
    {
       
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (recommendList!=null)
        {
            UserDataManager.Instance.UserData.CurSelectBookID = recommendList[0].id;
        }else
        {
            UserDataManager.Instance.UserData.CurSelectBookID = 1;
        }
       
        StartReadBook();
    }

    public void BookItem2ClickHandler(UnityEngine.EventSystems.PointerEventData data)
    {
       
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (recommendList != null)
        {
            UserDataManager.Instance.UserData.CurSelectBookID = recommendList[1].id;
        }
        else
        {
            UserDataManager.Instance.UserData.CurSelectBookID = 3;
        }
       
        StartReadBook();
    }
    public void BookItem3ClickHandler(UnityEngine.EventSystems.PointerEventData data)
    {     

        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (recommendList != null)
        {
            UserDataManager.Instance.UserData.CurSelectBookID = recommendList[2].id;
        }
        else
        {
            UserDataManager.Instance.UserData.CurSelectBookID = 16;
        }

        StartReadBook();
    }

    private void StartReadBook()
    {
        //通知服务端扣费
        //UINetLoadingMgr.Instance.Show();
        GameHttpNet.Instance.BuyChapter(UserDataManager.Instance.UserData.CurSelectBookID, 1, BuyChapterCallBack);
    }

    private void BuyChapterCallBack(HttpInfoReturn<BuyChapterResultInfo> buyData)
    {
        //UINetLoadingMgr.Instance.Close();

        if (buyData.code == 200)
        {
            UserDataManager.Instance.UserData.IsSelectFirstBook = 1;

            TalkingDataManager.Instance.SelectBooksInEnter(UserDataManager.Instance.UserData.CurSelectBookID);

            GameDataMgr.Instance.userData.AddMyBookId(UserDataManager.Instance.UserData.CurSelectBookID);
            initDialogDisplaySystemData(1);

#if !NOT_USE_LUA
            BookReadingWrapper.Instance.PrepareReading(true);
#else
        DialogDisplaySystem.Instance.PrepareReading();
#endif
          
        }
        else
        {
            UIAlertMgr.Instance.Show("TIPS", buyData.msg);
        }
    }


    private void initDialogDisplaySystemData(int vChapterID)
    {
        int dialogueID = 1;
#if !NOT_USE_LUA
        BookReadingWrapper.Instance.ChangeBookDialogPath(UserDataManager.Instance.UserData.CurSelectBookID, vChapterID);
#else
        DialogDisplaySystem.Instance.ChangeBookDialogPath(UserDataManager.Instance.UserData.CurSelectBookID, vChapterID);
#endif
        int index = vChapterID - 1;
        
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(UserDataManager.Instance.UserData.CurSelectBookID);
        int[] chapterDivisionArray = bookDetails.ChapterDivisionArray;
        int endDialogID = -1;
        int beginDialogID = index - 1 < 0 ? 1 : chapterDivisionArray[index - 1];
        if (index < chapterDivisionArray.Length)
        {
            endDialogID = chapterDivisionArray[index];
        }

#if !NOT_USE_LUA
        BookReadingWrapper.Instance.InitByBookID(UserDataManager.Instance.UserData.CurSelectBookID, 1, dialogueID, beginDialogID, endDialogID);
#else
        DialogDisplaySystem.Instance.InitByBookID(UserDataManager.Instance.UserData.CurSelectBookID, 1, dialogueID, beginDialogID, endDialogID);
#endif

      
        GameHttpNet.Instance.GetBookDetailInfo(UserDataManager.Instance.UserData.CurSelectBookID, GetBookDetailInfoCallBack);
        //GameHttpNet.Instance.GetCostChapterList(UserDataManager.Instance.UserData.CurSelectBookID, GetCostChapterListCallBack);

    }

    private void UserOptionCostListHandler(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----UserOptionCostListHandler---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo.code == 200)
        {
            UserDataManager.Instance.bookChapterOptionCostList = JsonHelper.JsonToObject<HttpInfoReturn<BookOptionCostCont<BookOptionCostItemInfo>>>(result);
        }
    }

    public void GetBookDetailInfoCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetBookDetailInfoCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo.code == 200)
        {
            UserDataManager.Instance.bookDetailInfo = JsonHelper.JsonToObject<HttpInfoReturn<BookDetailInfo>>(result);
            //UserDataManager.Instance.RecordBookDetailInfo();
        }
    }

    public void GetCostChapterListCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----GetCostChapterListCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo.code == 200)
        {
            UserDataManager.Instance.bookCostChapterList = JsonHelper.JsonToObject<HttpInfoReturn<BookCostChapterListCont<BookCostChapterItemInfo>>>(result);
            if(UserDataManager.Instance.bookCostChapterList != null && UserDataManager.Instance.bookCostChapterList.data != null)
            {
                List<BookCostChapterItemInfo> costList = UserDataManager.Instance.bookCostChapterList.data.costarr;
                if(costList != null)
                {
                    int len = costList.Count;
                    for(int i = 0;i<len;i++)
                    {
                        BookCostChapterItemInfo itemInfo = costList[i];
                        if(itemInfo != null)
                        {
#if !NOT_USE_LUA
                            BookReadingWrapper.Instance.AddChapterzPayId(itemInfo.bookid, itemInfo.chapterid);
#else
                            DialogDisplaySystem.Instance.AddChapterzPayId(itemInfo.bookid, itemInfo.chapterid);
#endif
                        }
                    }
                }
            }
        }
    }
  
    /// <summary>
    /// 这个是返回推荐书本的信息
    /// </summary>
    /// <param name="arg"></param>
    private void RecommendABookCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----RecommendABookCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo.code == 200)
        {
            //UINetLoadingMgr.Instance.Close();
            UserDataManager.Instance.recommendABookListCont = JsonHelper.JsonToObject<HttpInfoReturn<RecommendABookListCont>>(result);
            if (UserDataManager.Instance.recommendABookListCont != null)
            {
                List<RecommendABookList> tempList = UserDataManager.Instance.recommendABookListCont.data.book_list;
                if (recommendList == null)
                    recommendList = new List<RecommendABookList>();
                if (tempList != null)
                {
                    int countryId = UserDataManager.Instance.CountryId();
                    for (int i = 0;i< tempList.Count;i++)
                    {
                        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(tempList[i].id);
                        if(bookDetails != null && bookDetails.Availability == countryId)
                        {
                            recommendList.Add(tempList[i]);
                        }
                    }
                    ShowBookIcon(recommendList);
                }
            }
            else if (jo.code == 277)
            {
                UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                return;
            }
        }    
    }

    private void ShowBookIcon(List<RecommendABookList> vList)
    {
        int num = vList.Count;
        for (int i=0;i< num;i++)
        {
            BookIconList[i].SetActive(true);
            BookIconList[i].transform.Find("BookName").GetComponent<Text>().text= vList[i].booktypename.ToUpper().ToString();
        }
    }
}
