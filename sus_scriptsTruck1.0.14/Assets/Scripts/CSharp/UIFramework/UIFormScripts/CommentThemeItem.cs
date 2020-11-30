using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommentThemeItem : MonoBehaviour
{
    public Text NameTexte, TimeTexte, ContenText,likTexte,likUnTexte,ContensNumberTexte;
    public GameObject readMoreButton;
    public RectTransform UICommetItem;
    public Text CommentSizeFitter;
    public ContentSizeFitter contentSizeFitter;
    public Image LikeSprTouch, UnLikeSprTouch;
    public Image LikeSpr;
    public Image UnLikeSpr;

    public RepliesBGsprite RepliesBGsprite;
    private float height = 0f;
    private float CommentTextHeight = 0, UICommentItemHeigh, UICommentItemWith;
    private int discusstype = 0, nowdiscusstype=0;
    private ChapterCommentItemInfo mItemInfo;

    private string LikeOnPath = "CommentForm/bg_good_1";
    private string LikeOffPath = "CommentForm/bg_good_2";

    private string UnLikeOnPath = "CommentForm/bg_notgood_1";
    private string UnLikeOffPath = "CommentForm/bg_notgood_2";


  
    public void DisPoste()
    {
        UIEventListener.RemoveOnClickListener(LikeSprTouch.gameObject, LikeHandler);
        UIEventListener.RemoveOnClickListener(UnLikeSprTouch.gameObject, UnLikeHandler);
        UIEventListener.RemoveOnClickListener(readMoreButton, readMoreButtonOnclick);

        Destroy(gameObject);
    }

    public void Init(ChapterCommentItemInfo vItemInfo,int type)
    {
        UIEventListener.AddOnClickListener(LikeSprTouch.gameObject, LikeHandler);
        UIEventListener.AddOnClickListener(UnLikeSprTouch.gameObject, UnLikeHandler);
        UIEventListener.AddOnClickListener(readMoreButton, readMoreButtonOnclick);

        discusstype = -1;
        ContenText.text = "";
        height = ContenText.preferredHeight;
       

        mItemInfo = vItemInfo;
        
        NameTexte.text = vItemInfo.nickname.ToString();
        TimeTexte.text = vItemInfo.createtime.ToString();
        ContenText.text = vItemInfo.content.ToString();
        likTexte.text= vItemInfo.bestests.ToString();
        likUnTexte.text= vItemInfo.noagree.ToString();
        int ss = vItemInfo.replyarr.Count;
        ContensNumberTexte.text = ss.ToString();


        int line = GetLineCount();//计算出这个text文本所占的行数

        RectTransform ConRect = ContenText.gameObject.GetComponent<RectTransform>();
       // LOG.Info("行数：" + line);
        if (line > 4)
        {
            //大于4行，说明文本有益出

            //计算文本和背景的高度比例
            UICommentItemWith = UICommetItem.rect.width;
            UICommentItemHeigh = UICommetItem.rect.height;
            CommentTextHeight = ConRect.rect.height;

            //LOG.Info("UICommentItemWith:" + UICommentItemWith + "-UICommentItemHeigh:" + UICommentItemHeigh + "--CommentTextHeight：" + CommentTextHeight);
           
            readMoreButton.gameObject.SetActive(true);
        }
        else
        {
            //文本内容没有益出
            readMoreButton.gameObject.SetActive(false);

            float Heigt = 141.0f / 3;
            ConRect.sizeDelta = new Vector2(530, Heigt * line + 6);

            UICommetItem.sizeDelta = new Vector2(750, Heigt * line + 180);

        }

        //LOG.Info("type:" + type);
        if (type!=-1)
        {
            if (type == 1)
            {
                LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOnPath);
                UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOffPath);

                likTexte.text = mItemInfo.bestests.ToString();

            }
            else if (type == 0)
            {
                LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOffPath);
                UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOnPath);
                likUnTexte.text = mItemInfo.noagree.ToString();
            }
        }
        else
        {
            LikeAndUnlikeSpriteChange();
        }

       
    }

    private void LikeHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (mItemInfo != null)
        {
            if (mItemInfo.discusstype == -1&& discusstype==-1)
            {
                //UINetLoadingMgr.Instance.Show();
                nowdiscusstype = 1;
                GameHttpNet.Instance.DiscussComment(mItemInfo.discussid, 1, DiscussLikeCallBack);
            }
            else
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(153);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("You've already commented.", false);
            }
        }
    }
    private void UnLikeHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (mItemInfo != null)
        {
            if (mItemInfo.discusstype == -1 && discusstype == -1)
            {
                //UINetLoadingMgr.Instance.Show();
                nowdiscusstype = 0;
                GameHttpNet.Instance.DiscussComment(mItemInfo.discussid, 0, DiscussLikeCallBack);
            }
            else
            {
                var Localization = GameDataMgr.Instance.table.GetLocalizationById(153);
                UITipsMgr.Instance.PopupTips(Localization, false);

                //UITipsMgr.Instance.PopupTips("You've already commented.", false);
            }
        }
    }

    private void DiscussLikeCallBack(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----DiscussLikeCallBack---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            LoomUtil.QueueOnMainThread((param) =>
            {
                //UINetLoadingMgr.Instance.Close();
                if (jo.code == 200)
                {
                    UserDataManager.Instance.discussCommentResultInfo = JsonHelper.JsonToObject<HttpInfoReturn<DiscussCommentResultInfo>>(result);
                    if (UserDataManager.Instance.discussCommentResultInfo != null && UserDataManager.Instance.discussCommentResultInfo.data != null)
                    {
                        if (mItemInfo != null && mItemInfo.discussid == UserDataManager.Instance.discussCommentResultInfo.data.discussid)
                        {
                            discusstype=nowdiscusstype;
                            SpriteChange();

                            RepliesBGsprite.CommentThemeItemLikeChange(discusstype);
                        }
                    }
                }
                else
                {
                    var Localization = GameDataMgr.Instance.table.GetLocalizationById(153);
                    UITipsMgr.Instance.PopupTips(Localization, false);

                    //UITipsMgr.Instance.PopupTips("You've already commented.", false);
                }
            }, null);
        }
    }   
    private void readMoreButtonOnclick(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //LOG.Info("read More 按钮被点击了");
       
        readMoreButton.gameObject.SetActive(false);
        ContenText.gameObject.SetActive(false);
        CommentSizeFitter.gameObject.SetActive(true);
        CommentSizeFitter.text = mItemInfo.content;

        Invoke("sss",0.3f);
    }
    private void sss()
    {
      
        //CancelInvoke();
        float CommentSizeFitterHeight = CommentSizeFitter.gameObject.GetComponent<RectTransform>().rect.height;
        float altitudeDifference = UICommentItemHeigh - CommentTextHeight;//得到高度差

       // LOG.Info("CommentSizeFitterHeight:" + CommentSizeFitterHeight);
        UICommetItem.sizeDelta = new Vector2(UICommentItemWith, CommentSizeFitterHeight + altitudeDifference);
       
        Invoke("contentSizeFitterFalse", 0.2f);
    }
    private void contentSizeFitterFalse()
    {
        CancelInvoke();
        contentSizeFitter.enabled = false;
        contentSizeFitter.enabled = true;
    }

    private void LikeAndUnlikeSpriteChange()
    {
        if (mItemInfo.discusstype == 1)
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOnPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOffPath);
        }
        else if (mItemInfo.discusstype == 0)
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOffPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOnPath);
        }
        else
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOffPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOffPath);
        }
    }

    private void SpriteChange()
    {
        if (this.discusstype == 1)
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOnPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOffPath);

            likTexte.text = mItemInfo.bestests+1+"";
           
        }
        else if (this.discusstype == 0)
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOffPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOnPath);
            likUnTexte.text = mItemInfo.noagree + 1 + "";
        }
        else
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOffPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOffPath);
        }
    }
    private int GetLineCount()
    {
        return (int)(ContenText.preferredHeight / height);
    }
}
