using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UGUI;
using UnityEngine;
using UnityEngine.UI;


public class UICommentItem : CUIListCell
{
    public Text NameTxt;
    public Text ContentTxt, CommentSizeFitter;
    public Text DateTxt;
    public Text LikeTxt;
    public Text UnLikeTxt;
    public Text ContentsTxt;
    public Image LikeSpr;
    public Image UnLikeSpr;
    private float height = 0f;
    public Image LikeSprTouch, UnLikeSprTouch, btnContent,bg,head;
    public RectTransform readMoreButton,UICommetItem, CommentSizeFitterRectTrans;  
    public RepliesBGsprite RepliesBGsprite;

    private string LikeOnPath = "CommentForm/bg_good_1";
    private string LikeOffPath = "CommentForm/bg_good_2";

    private string UnLikeOnPath = "CommentForm/bg_notgood_1";
    private string UnLikeOffPath = "CommentForm/bg_notgood_2";

    private ChapterCommentItemInfo mItemInfo;
    private float CommentTextHeight = 0, UICommentItemHeigh, UICommentItemWith;

    private bool first = false;
    private int newtype = -1;
    public override void Initialize(CUIForm formScript)
    {
        base.Initialize(formScript);
        

    }

    private void LikeHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (mItemInfo != null)
        {
            if (mItemInfo.discusstype == -1)
            {
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.DiscussComment(mItemInfo.discussid, 1, DiscussLikeCallBack);
            }
            else
            {
                UITipsMgr.Instance.PopupTips("You've already commented.", false);
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
                            mItemInfo.discusstype = 1;
                            mItemInfo.bestests += 1;
                            SetData(mItemInfo);
                            UserDataManager.Instance.UpdateCommentList(mItemInfo);
                            this.newtype = 1;
                        }
                    }
                }
                else
                {
                    UITipsMgr.Instance.PopupTips("You've already commented.", false);
                }
            }, null);
        }
    }

    private void UnLikeHandler(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        if (mItemInfo != null)
        {
            if (mItemInfo.discusstype == -1)
            {
                //UINetLoadingMgr.Instance.Show();
                GameHttpNet.Instance.DiscussComment(mItemInfo.discussid, 0, DiscussUnLikeCallBack);
            }
            else
            {
                UITipsMgr.Instance.PopupTips("You've already commented.", false);
            }
        }
    }

    private void btnContentButtonOn(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //LOG.Info("评论点击");
        if (mItemInfo!=null)
        {
            RepliesBGsprite.gameObject.SetActive(true);
            RepliesBGsprite.Inis(mItemInfo, this.gameObject, this.newtype);
        }
      
    }

    private void DiscussUnLikeCallBack(object arg)
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
                            mItemInfo.discusstype = 0;
                            mItemInfo.noagree += 1;
                            SetData(mItemInfo);
                            this.newtype = 0;
                            UserDataManager.Instance.UpdateCommentList(mItemInfo);
                        }
                    }
                }
                else
                {
                    UITipsMgr.Instance.PopupTips("You've already commented.", false);
                }
            }, null);
        }
    }
    
    public void MySentInite(string conte)
    {
        CommentSizeFitter.gameObject.SetActive(false);

        string times = System.DateTime.Now.ToString("MM/dd/yyyy");

            ContentTxt.text = "";
            height = ContentTxt.preferredHeight;

            NameTxt.text = CTextManager.Instance.GetText(281);
            ContentTxt.text = conte;
            DateTxt.text = times;
            LikeTxt.text = "0";
            UnLikeTxt.text = "0";
            CommentSizeFitter.text = conte;
        
            int line = GetLineCount();//计算出这个text文本所占的行数

        RectTransform ConRect = ContentTxt.GetComponent<RectTransform>();

            if (line > 4)
            {
                //大于4行，说明文本有益出


                //计算文本和背景的高度比例
                UICommentItemWith = UICommetItem.rect.width;
                UICommentItemHeigh = UICommetItem.rect.height;
                CommentTextHeight = ConRect.rect.height;

                readMoreButton.gameObject.SetActive(true);
            }
            else
            {
                //文本内容没有益出
                readMoreButton.gameObject.SetActive(false);

            float Heigt = 115.0f / 3;
            ConRect.sizeDelta = new Vector2(577, Heigt* line+1);

            UICommetItem.sizeDelta = new Vector2(748, Heigt * line+200);

        }

    }
    public void SetData(ChapterCommentItemInfo vItemInfo)
    {
        if (!first)
        {
            first = true;
            UIEventListener.AddOnClickListener(btnContent.gameObject, btnContentButtonOn);
            UIEventListener.AddOnClickListener(readMoreButton.gameObject, readMoreButtonOnclick);
            UIEventListener.AddOnClickListener(LikeSprTouch.gameObject, LikeHandler);
            UIEventListener.AddOnClickListener(UnLikeSprTouch.gameObject, UnLikeHandler);
        }

        //Debug.Log("ddss");
        this.newtype = -1;
        CommentSizeFitter.gameObject.SetActive(false);

        mItemInfo = vItemInfo;
        if (mItemInfo != null)
        {
            ContentTxt.text = "";
            height = ContentTxt.preferredHeight;

            if (mItemInfo.nickname.Equals("sysdiscuss"))
            {
                //这个是官方回复

                //背景设置
                bg.sprite= ResourceManager.Instance.GetUISprite("CommentForm/bg_img");

                //头像设置
                head.sprite = ResourceManager.Instance.GetUISprite("CommentForm/bg_toux");
                NameTxt.text = CTextManager.Instance.GetText(282);
            }else
            {
                NameTxt.text = mItemInfo.nickname;
            }

            
            ContentTxt.text = mItemInfo.content;
            DateTxt.text = mItemInfo.createtime;
            LikeTxt.text = mItemInfo.bestests.ToString();
            UnLikeTxt.text = mItemInfo.noagree.ToString();
          
            CommentSizeFitter.text = mItemInfo.content.ToString();
          

            if (mItemInfo.replyarr!=null)
            {
                int count = mItemInfo.replyarr.Count;
                ContentsTxt.text = count.ToString();
            }
           
            int line = GetLineCount();//计算出这个text文本所占的行数
            //Debug.Log("行数是:" + line);

            //int ss = mItemInfo.replyarr.Count;
            //if (ss != 0)
            //{
            //    for (int i = 0; i < mItemInfo.replyarr.Count; i++)
            //    {
            //        int replyid = mItemInfo.replyarr[i].replyid;
            //        int discussid = mItemInfo.replyarr[i].discussid;
            //        string content = mItemInfo.replyarr[i].content;
            //        string createtime = mItemInfo.replyarr[i].createtime;
            //        Debug.Log("回复评论replyid:" + replyid + "--discussid:" + discussid + "--content：" + content + "--createtime：" + createtime);
            //    }
            //}
            //Debug.Log("回复评论有数目：" + ss);

            RectTransform ConRect = ContentTxt.GetComponent<RectTransform>();

            if (line>4)
            {
                //大于4行，说明文本有益出

              
                //计算文本和背景的高度比例
                UICommentItemWith = UICommetItem.rect.width;
                UICommentItemHeigh = UICommetItem.rect.height;
                CommentTextHeight = ContentTxt.gameObject.GetComponent<RectTransform>().rect.height;

                readMoreButton.gameObject.SetActive(true);
            }
            else
            {
                //文本内容没有益出
                readMoreButton.gameObject.SetActive(false);

                float Heigt = 115.0f / 3;
                ConRect.sizeDelta = new Vector2(577, Heigt * line+1);

                UICommetItem.sizeDelta = new Vector2(748, Heigt * line + 200);

            }

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
    }

    public void LikeSprSpriteChange(int type)
    {
        if (this.newtype==-1)
        {
            this.newtype = type;
        }
        if (type == 1)
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOnPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOffPath);

            LikeTxt.text = mItemInfo.bestests+1+"";
           
        }
        else if (type == 0)
        {
            LikeSpr.sprite = ResourceManager.Instance.GetUISprite(LikeOffPath);
            UnLikeSpr.sprite = ResourceManager.Instance.GetUISprite(UnLikeOnPath);
             UnLikeTxt.text = mItemInfo.noagree + 1 + "";
        }
    }
    /// <summary>
    /// 这个是计算文本的行数
    /// </summary>
    /// <returns></returns>
    private int GetLineCount()
    {
        return (int)(ContentTxt.preferredHeight / height);
    }

    private void readMoreButtonOnclick(PointerEventData data)
    {
        AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
        //LOG.Info("read More 按钮被点击了");
        readMoreButton.gameObject.SetActive(false);
        ContentTxt.gameObject.SetActive(false);
        CommentSizeFitter.gameObject.SetActive(true);              
        Invoke("sss",0.3f);
    }

    private void sss()
    {
        CancelInvoke("sss");
        float CommentSizeFitterHeight = CommentSizeFitterRectTrans.rect.height;
        float altitudeDifference = UICommentItemHeigh - CommentTextHeight;//得到高度差
        LOG.Info("altitudeDifference:" + altitudeDifference + "--CommentSizeFitterHeight:" + CommentSizeFitterHeight);
        UICommetItem.sizeDelta = new Vector2(UICommentItemWith, CommentSizeFitterHeight + altitudeDifference);

    }

   

    /// <summary>
    /// 删除物体释放内存
    /// </summary>
    public void Disposte()
    {
        LikeSpr.sprite = null;
        UnLikeSpr.sprite = null;
        LikeSprTouch.sprite = null;
        UnLikeSprTouch.sprite = null;
        btnContent.sprite = null;
        bg.sprite = null;
        head.sprite = null;

        UIEventListener.RemoveOnClickListener(btnContent.gameObject, btnContentButtonOn);
        UIEventListener.RemoveOnClickListener(readMoreButton.gameObject, readMoreButtonOnclick);
        UIEventListener.RemoveOnClickListener(LikeSprTouch.gameObject, LikeHandler);
        UIEventListener.RemoveOnClickListener(UnLikeSprTouch.gameObject, UnLikeHandler);

        Destroy(gameObject);
    }
}

