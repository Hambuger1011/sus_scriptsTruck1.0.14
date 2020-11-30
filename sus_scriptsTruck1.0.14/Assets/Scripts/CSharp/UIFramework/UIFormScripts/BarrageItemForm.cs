using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UGUI;

public class BarrageItemForm : MonoBehaviour
{

    public CanvasGroup CanvasGroup;
    public Text BarrageTexte;
    public Image TopFace, Bg;
    public RectTransform thisRect;
    private GameObject barrageFalse;
    private commentlist commentlist;

    private bool starts = false, Upmove = false;
    public void Inite(commentlist commentlist, GameObject barrageFalse)
    {
        this.commentlist = commentlist;
        UIEventListener.AddOnClickListener(Bg.gameObject, BarrageOnclicke);
        gameObject.transform.localScale = Vector3.one;
        this.barrageFalse = barrageFalse;
        BarrageTexte.text = commentlist.content.ToString();
        int Face = 0;
        if (string.IsNullOrEmpty(commentlist.face))
            Face = Random.Range(1, 3);
        else
        {
            Face = int.Parse(commentlist.face);
            if (Face > 3)
            {
                Face = Random.Range(1, 3);
            }
            else if (Face == 0)
                Face = 1;
        }

        TopFace.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/img_renwu" + Face);

        if (commentlist.is_vip == 1)
        {
            Bg.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/bg_shuruk");
        }
        else
        {
            Bg.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/bg_duihuak");
        }
        GameFalse();
    }

    /// <summary>
    /// 自己发送的弹幕
    /// </summary>
    /// <param name="vTr"></param>
    public void sendBarrage(string vTr)
    {
        BarrageTexte.text = vTr.ToString();
        Bg.sprite = ResourceManager.Instance.GetUISprite("ProfileForm/bg_duihuak1");

        if (TopFace!=null)
        {
            //【调用lua公共方法 加载头像】   -1代码当前装扮的头像    
            XLuaManager.Instance.CallFunction("GameHelper", "ShowAvatar", -1, TopFace);
        }
        GameFalse();
    }
    private void Update()
    {
        if (starts)
        {
            if (gameObject.transform.localPosition.y > 500)
            {
                Upmove = false;
                starts = false;
                MyBooksDisINSTANCE.Instance.SetPool(gameObject);

                if (barrageFalse == null)
                {
                    return;
                }
                gameObject.transform.SetParent(barrageFalse.transform);
                return;
            }
            //gameObject.transform.Translate(Vector3.up * 0.015f);
        }

        if (Upmove)
        {
            gameObject.transform.Translate(Vector3.up * 0.015f);
        }
    }

    public void gameMoveFalse()
    {
        Upmove = true;


        if (barrageFalse != null)
            gameObject.transform.SetParent(barrageFalse.transform);
        //transform.DOLocalMoveY(600, 3).SetEase(Ease.Linear).OnComplete(() =>
        //{

        //});
    }
    private void GameFalse()
    {
        float times = 0.5f;
        gameObject.transform.localScale = new Vector3(0, 0, 1);
        gameObject.transform.DOScaleX(1, times);
        gameObject.transform.DOScaleY(1, times);
        thisRect.sizeDelta = new Vector2(thisRect.rect.width, 0);
        DOTween.To(() => 0, (value) => { thisRect.sizeDelta = new Vector2(thisRect.rect.width, value); }, 80, times);//times秒，从0变到80
        starts = true;
    }

    private void BarrageOnclicke(PointerEventData data)
    {
        CUIManager.Instance.OpenForm(UIFormName.BarrageForm);//
        CUIManager.Instance.GetForm<BarrageForm>(UIFormName.BarrageForm).Inite(commentlist);
    }

    public void DestroySelf()
    {
        UIEventListener.RemoveOnClickListener(Bg.gameObject, BarrageOnclicke);
        TopFace.sprite = null;
        Bg.sprite = null;

        Destroy(gameObject);
    }

}
