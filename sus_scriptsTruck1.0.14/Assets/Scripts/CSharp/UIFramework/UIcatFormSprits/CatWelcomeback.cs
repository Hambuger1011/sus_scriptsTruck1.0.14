using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CatWelcomeback : CatBaseForm
{
    private Transform StoryPre;
    private GameObject Story;
    private GameObject Mask;
    private GameObject Item;
    private ScrollRect ScrollView, StoryScrollView;
    private List<CatWelcomeBackeItem> CatWelcomeBackeItemList;
    private List<CatWelcomBackStory> CatWelcomBackStoryList;
    private GameObject spokeText;
    private bool HadNewStory=false;
    private RectTransform frameTrans;

    private void Awake()
    {
        Mask = transform.Find("Mask").gameObject;
        StoryPre = transform.Find("Bg/StoryPre");
        Story = transform.Find("Bg/StoryPre/Story").gameObject;
        Item = transform.Find("Bg/Item").gameObject;
        ScrollView = transform.Find("Bg/ScrollView").GetComponent<ScrollRect>();
        StoryScrollView = transform.Find("Bg/StoryScrollView").GetComponent<ScrollRect>();
        spokeText = transform.Find("Bg/spoke/spokeText").gameObject;
        frameTrans = transform.Find("Bg").GetComponent<RectTransform>();
    }
    public override void OnOpen()
    {
        base.OnOpen();
        UiFormIndex = (int)CatFormEnum.CAT_WELCOMEBACK;
        UIEventListener.AddOnClickListener(Mask, CloseUi);

        CatWelcomeBackeItemList = new List<CatWelcomeBackeItem>();       
        CatWelcomBackStoryList = new List<CatWelcomBackStory>();
       
        HadNewStory = false;
        Inite();

        if (GameUtility.IpadAspectRatio())
        {
            this.frameTrans.localScale = new Vector3(0.7f, 0, 1);
            this.frameTrans.DOScaleY(0.7f, 0.25f).SetEase(Ease.OutBack).Play();
        }
        else
        {
            StartShow();
        }
        Transform CloseButton = transform.Find("Bg/CloseButton");
        CloseButton.GetComponent<RectTransform>().DOAnchorPosY(-528, 0.4f).SetEase(Ease.OutBack);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(Mask, CloseUi);

        if (CatWelcomeBackeItemList!=null)
        {
            for (int i=0;i< CatWelcomeBackeItemList.Count;i++)
            {
                CatWelcomeBackeItemList[i].DisPost();
            }
        }

        if (CatWelcomBackStoryList != null)
        {
            for (int i = 0; i < CatWelcomBackStoryList.Count; i++)
            {
                CatWelcomBackStoryList[i].Disposte();
            }
        }
    }

    private void CloseUi(PointerEventData data)
    {
        //派发事件，关闭这个界面
        EventDispatcher.Dispatch(EventEnum.CloseUiFormDist.ToString(), UiFormIndex);
    }

    /// <summary>
    /// 界面刚出现的效果显示
    /// </summary>
    private void StartShow()
    {
        Transform BG = transform.Find("Bg");
        BG.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        BG.DOScale(new Vector3(1, 1, 1), 0.4f).SetEase(Ease.OutBack);
    }

    private void Inite()
    {      
        if (UserDataManager.Instance.SceneInfo!=null)
        {
            List<adoptchange> tem = UserDataManager.Instance.SceneInfo.data.adopt_change;
            
            if (tem.Count > 0)
            {
                for (int i = 0; i < tem.Count; i++)
                {
                    if (Item == null) return;

                   
                    for (int j=0;j< UserDataManager.Instance.GetChangValueList().Count;j++)
                    {
                        if (UserDataManager.Instance.GetChangValueList()[j]== tem[i].pid)
                        {
                            //只是实例化出有数值变化的数据项

                            GameObject go = Instantiate(Item);
                            go.SetActive(true);
                            go.transform.SetParent(ScrollView.content);
                            CatWelcomeBackeItem CatWelcomeBackeItem = go.GetComponent<CatWelcomeBackeItem>();
                            CatWelcomeBackeItem.Init(tem[i]);
                            CatWelcomeBackeItemList.Add(CatWelcomeBackeItem);

                            if (tem[i].story_new > 0)
                            {
                                //有新的故事开启
                                GameObject item = Instantiate(Story);
                                item.SetActive(true);
                                item.transform.SetParent(StoryScrollView.content);
                                CatWelcomBackStory CatWelcomBackStory = item.GetComponent<CatWelcomBackStory>();
                                CatWelcomBackStory.Inite(tem[i]);
                                HadNewStory = true;

                            }
                        }
                    }
                }

                if (HadNewStory)
                {
                    //有新故事开启
                    spokeText.SetActive(true);
                }
                else
                {
                    //没有新故事开启
                    spokeText.SetActive(false);
                }
            }
        }
       
    }
}
