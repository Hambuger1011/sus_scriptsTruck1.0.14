using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;
using pb;
using AB;

public class CatStoryItem : MonoBehaviour {

    private Text CatName;
    private Text StoryNumber, ContText;
    private storypetarr storypetarr;
    private GameObject newtag;
    private Image StorySprite;
    private void Awake()
    {
        CatName = transform.Find("CatName").GetComponent<Text>();
        StoryNumber = transform.Find("StoryNumber").GetComponent<Text>();
        newtag = transform.Find("newtag").gameObject;
        ContText= transform.Find("ContText").GetComponent<Text>();
        StorySprite = transform.Find("StorySprite").GetComponent<Image>();

        UIEventListener.AddOnClickListener(gameObject,GameOnclicke);
    }
    public void Inite(storypetarr storypetarr)
    {

        StorySprite.sprite= ABSystem.ui.GetUITexture(AbTag.Global, "assets/Bundle/CatPreview/Screnes/Story"+storypetarr.pid+ ".png");

        CatName.text = storypetarr.pet_name.ToString();
        StoryNumber.text = storypetarr.storys + "/" + storypetarr.max_story;
        this.storypetarr = storypetarr;

        t_story StoryInfo = GameDataMgr.Instance.table.GetCatStoryById(storypetarr.pid);

        string Qcontss = "";
        if (storypetarr.storys==1)
        {
            Qcontss = StoryInfo.story1.ToString();
        }else if (storypetarr.storys == 2)
        {
            Qcontss = StoryInfo.story2.ToString();
        }
        else if (storypetarr.storys == 3)
        {
            Qcontss = StoryInfo.story3.ToString();
        }

        ContText.text = Qcontss.Replace("/n", "\n");
       
        if (storypetarr.storys_isused==1)
        {
            //有新故事
            newtag.SetActive(true);
        }
        else
        {
            //没有新故事
            newtag.SetActive(false);
        }
    }

    private void GameOnclicke(PointerEventData data)
    {
        
        //可以用，弹出故事界面
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        CUIManager.Instance.OpenForm(UIFormName.CatStoryDetails);
        CatStoryDetailsFrom catStoryDetailsFrom = CUIManager.Instance.GetForm<CatStoryDetailsFrom>(UIFormName.CatStoryDetails);
        if (catStoryDetailsFrom != null)
            catStoryDetailsFrom.Inite(storypetarr.pid, storypetarr.storys, storypetarr.max_story);

        newtag.SetActive(false);
    }

    /// <summary>
    /// 移除这个物体释放内存
    /// </summary>
    public void Disposte()
    {
        UIEventListener.RemoveOnClickListener(gameObject, GameOnclicke);

        Destroy(gameObject);
    }
}
