using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommenContenterItem : MonoBehaviour
{
    public Text NameTexte, TimeTexte, ContenText, CommentSizeFitter,likeText,likeUnText;
    public RectTransform UICommetItem;
    public GameObject readMoreButton;
    public ContentSizeFitter contentSizeFitter;
    public Image LikeSprTouch, UnLikeSprTouch;
    public Image LikeSpr;
    public Image UnLikeSpr;
    public Image ContenterItem, headPortraid;


    private float height = 0f;
    private float CommentTextHeight = 0, UICommentItemHeigh, UICommentItemWith;
    private string ConterString;
    private CommentBackInfo mItemInfo;

    private string LikeOnPath = "CommentForm/bg_good_1";
    private string LikeOffPath = "CommentForm/bg_good_2";

    private string UnLikeOnPath = "CommentForm/bg_notgood_1";
    private string UnLikeOffPath = "CommentForm/bg_notgood_2";
    private int discusstype = 0, nowdiscusstype = 0;
    public void Init(CommentBackInfo vItemInfo)
    {
        UIEventListener.AddOnClickListener(readMoreButton, readMoreButtonOnclick);
        UIEventListener.AddOnClickListener(LikeSprTouch.gameObject, LikeHandler);
        UIEventListener.AddOnClickListener(UnLikeSprTouch.gameObject, UnLikeHandler);

        discusstype = -1;
        mItemInfo = vItemInfo;
        ContenText.text = "";
        height = ContenText.preferredHeight;

        if (vItemInfo.nickname.Equals("sysreply"))
        {
            //官方回复

            //设置背景
            ContenterItem.sprite = ResourceManager.Instance.GetUISprite("CommentForm/bg_img");
            //设置头像
            headPortraid.sprite= ResourceManager.Instance.GetUISprite("CommentForm/bg_toux");
        }

        NameTexte.text = vItemInfo.nickname.ToString();
        TimeTexte.text = vItemInfo.createtime.ToString();
        ContenText.text = vItemInfo.content.ToString();
        ConterString= vItemInfo.content.ToString();
        likeText.text = vItemInfo.bestests.ToString();
        likeUnText.text = vItemInfo.noagree.ToString();

        int line = GetLineCount();//计算出这个text文本所占的行数
        RectTransform ConRect = ContenText.gameObject.GetComponent<RectTransform>();

        LOG.Info("行数：" + line);
        if (line >= 2)
        {
            //大于2行，说明文本有益出

            //计算文本和背景的高度比例
            UICommentItemWith = UICommetItem.rect.width;
            UICommentItemHeigh = UICommetItem.rect.height;
            CommentTextHeight = ConRect.rect.height;

           // LOG.Info("UICommentItemWith:" + UICommentItemWith + "-UICommentItemHeigh:" + UICommentItemHeigh + "--CommentTextHeight：" + CommentTextHeight);

            readMoreButton.gameObject.SetActive(true);
        }
        else
        {
            //文本内容没有益出
            readMoreButton.gameObject.SetActive(false);

            float Heigt = 47;
            ConRect.sizeDelta = new Vector2(480, Heigt * line );

            UICommetItem.sizeDelta = new Vector2(750, Heigt * line + 180);

        }

        LikeAndUnlikeSpriteChange();
    }

    /// <summary>
    /// 这个是评论回复评论的时候，生成我当前的评论
    /// </summary>
    public void SpawnInit(string conten)
    {
        ContenText.text = "";
        height = ContenText.preferredHeight;


        string times = System.DateTime.Now.ToString("MM/dd/yyyy");

        NameTexte.text = "Guest";
        TimeTexte.text = times;
        ContenText.text =conten;
        ConterString = conten;

        int line = GetLineCount();//计算出这个text文本所占的行数

        RectTransform ConRect = ContenText.gameObject.GetComponent<RectTransform>();

        LOG.Info("行数：" + line);
        if (line >= 2)
        {
            //大于4行，说明文本有益出

            //计算文本和背景的高度比例
            UICommentItemWith = UICommetItem.rect.width;
            UICommentItemHeigh = UICommetItem.rect.height;
            CommentTextHeight = ConRect.rect.height;

           // LOG.Info("UICommentItemWith:" + UICommentItemWith + "-UICommentItemHeigh:" + UICommentItemHeigh + "--CommentTextHeight：" + CommentTextHeight);

            readMoreButton.gameObject.SetActive(true);
        }
        else
        {
            //文本内容没有益出
            readMoreButton.gameObject.SetActive(false);

            float Heigt = 47;
            ConRect.sizeDelta = new Vector2(480, Heigt * line);

            UICommetItem.sizeDelta = new Vector2(750, Heigt * line + 180);

        }
    }

    private int GetLineCount()
    {
        return (int)(ContenText.preferredHeight / height);
    }

    private void readMoreButtonOnclick(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        LOG.Info("read More 按钮被点击了");
        
        readMoreButton.gameObject.SetActive(false);
        ContenText.gameObject.SetActive(false);
        CommentSizeFitter.gameObject.SetActive(true);
        CommentSizeFitter.text = ConterString;

        Invoke("sss", 0.3f);
    }

    private void sss()
    {
       
        //CancelInvoke();
        float CommentSizeFitterHeight = CommentSizeFitter.gameObject.GetComponent<RectTransform>().rect.height;
        float altitudeDifference = UICommentItemHeigh - CommentTextHeight;//得到高度差

        //LOG.Info("CommentSizeFitterHeight:" + CommentSizeFitterHeight);
        UICommetItem.sizeDelta = new Vector2(UICommentItemWith, CommentSizeFitterHeight + altitudeDifference-50);
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

            likeText.text = mItemInfo.bestests + 1 + "";

        }
        else if (this.discusstype == 0)
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOffPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOnPath);
            likeUnText.text = mItemInfo.noagree + 1 + "";
        }
        else
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOffPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOffPath);
        }
    }
    private void LikeHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (mItemInfo != null)
        {
            if (mItemInfo.discusstype == -1 && discusstype == -1)
            {
                //UINetLoadingMgr.Instance.Show();
                nowdiscusstype = 1;
                GameHttpNet.Instance.DiscussCommentback(mItemInfo.replyid, 1, DiscussLikeCallBack);
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
                GameHttpNet.Instance.DiscussCommentback(mItemInfo.replyid, 0, DiscussLikeCallBack);
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
                    discusstype = nowdiscusstype;
                    SpriteChange();                   
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

    //public override void OnClose()
    //{
    //    base.OnClose();

    //    UIEventListener.RemoveOnClickListener(readMoreButton, readMoreButtonOnclick);
    //    UIEventListener.RemoveOnClickListener(LikeSprTouch.gameObject, LikeHandler);
    //    UIEventListener.RemoveOnClickListener(UnLikeSprTouch.gameObject, UnLikeHandler);
    //}

    public void DisPoste()
    {
        UIEventListener.RemoveOnClickListener(readMoreButton, readMoreButtonOnclick);
        UIEventListener.RemoveOnClickListener(LikeSprTouch.gameObject, LikeHandler);
        UIEventListener.RemoveOnClickListener(UnLikeSprTouch.gameObject, UnLikeHandler);

        Destroy(gameObject);
    }
}
