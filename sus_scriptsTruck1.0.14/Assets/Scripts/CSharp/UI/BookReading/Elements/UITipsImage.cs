namespace BookReading
{
    using DG.Tweening;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;

    [XLua.LuaCallCSharp]
    public class UITipsImage :
#if !NOT_USE_LUA
        MonoBehaviour
#else
        UIBookReadingElement
#endif
    {


        public Image TipsGo,UImask;
        public Text TipsText;
        public CanvasGroup tipsT;

        public RectTransform  Peson;
        private bool nofirst = false, cantouch = false;
        private RectTransform TipsTextTect;

#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            form.tipsImage = this;

            TipsTextTect = TipsText.GetComponent<RectTransform>();
        }

        public override void ResetUI()
        {
            this.TipsGo.gameObject.SetActive(false);
        }

        public override void SetSkin() { }

        public override void Dispose()
        {
            nofirst = false;
            //UIEventListener.RemoveOnClickListener(UImask.gameObject, Close);
        }
#endif


        public void Close(PointerEventData data)
        {
            if (!cantouch)
            {
                return;
            }
            
            //tipsT.DOFade(0,0.5f).SetEase(Ease.Flash);
            Peson.DOAnchorPos(new Vector2(800, 347), 0.5f).OnComplete(()=> {

                CancelInvoke("HideTips");
                cantouch = false;
                this.TipsGo.gameObject.SetActive(false);
            });
            
        }

        public void ShowTips(string vTips)
        {

           
            if (!string.IsNullOrEmpty(vTips))
            {
                //入
                //CancelInvoke("HideTips");
                this.TipsText.text = StringUtils.ReplaceChar(vTips);
                this.TipsGo.rectTransform.anchoredPosition = new Vector2(0, 900);
                this.TipsGo.gameObject.SetActive(true);
                this.TipsGo.rectTransform.DOAnchorPos(new Vector2(0, 525), 0.5f).SetEase(Ease.Flash).OnComplete(() =>
                {
                    if(TipsTextTect == null)
                    {
                        return;
                    }
                    float TipTextHeight= TipsTextTect.rect.height;

                    this.TipsGo.rectTransform.sizeDelta = new Vector2(549, TipTextHeight+65);

                    Invoke("HideTips", 5f);//出
                });

                //Invoke("changeHight",0.3f);

                cantouch = false;
                //tipsT.DOFade(0,0.1f);

                //Peson.anchoredPosition = new Vector2(800,347);              
                //Peson.DOAnchorPos(new Vector2(271, 347), 0.5f).SetEase(Ease.Flash).OnComplete(() => {
                //    tipsT.DOFade(1, 0.8f).OnComplete(()=> {
                //        cantouch = true;
                //        Invoke("HideTips", 5f);//出
                //    });
                   
                //});
            }
            else
            {
                //CancelInvoke("HideTips");
                //this.TipsGo.gameObject.SetActive(false);
                //this.TipsGo.rectTransform.anchoredPosition = new Vector2(0, 900);

                HideTips();
            }
        }

        private void HideTips()
        {
            if(this.TipsGo == null)
            {
                return;
            }

            this.TipsGo.rectTransform.DOAnchorPos(new Vector2(0, 900), 0.5f).SetEase(Ease.Flash).OnComplete(()=> {
                this.TipsGo.gameObject.SetActive(false);
            });

            //tipsT.DOFade(0, 0.8f).SetEase(Ease.Flash);
            //Peson.DOAnchorPos(new Vector2(800, 347), 0.5f).SetEase(Ease.Flash).OnComplete(() =>
            //{
            //    CancelInvoke("HideTips");
            //    this.TipsGo.gameObject.SetActive(false);
            //});
        }

        /// <summary>
        /// 这个是随着文本内容的大小，改变高度
        /// </summary>
        private void changeHight()
        {
            if (!nofirst)
            {
                nofirst = true;
                UIEventListener.AddOnClickListener(UImask.gameObject, Close);
            }
            
            //得到文本内容的高度
            float textHight = this.TipsText.rectTransform.rect.height;

            //LOG.Info("文本的高度是："+textHight);
            if (textHight>80)
            {
                float newHeight = 105+ textHight;
                tipsT.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(628, newHeight);

                //LOG.Info("框适应高度是：" + newHeight);
            }
            else
            {
                tipsT.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(628, 148);

            }
        }

    }
}