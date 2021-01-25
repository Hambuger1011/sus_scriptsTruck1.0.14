namespace UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using DG.Tweening;
    using UnityEngine.UI;

#if !NOT_USE_LUA
    using DialogDisplaySystem = BookReadingWrapper;
#endif


    [XLua.Hotfix, XLua.CSharpCallLua, XLua.LuaCallCSharp]
    public abstract class UIBubbleItem
    {
        public GameObject gameObject;
        public RectTransform transform;
        public CanvasGroup canvasGroup;

        public UIBubbleBox[] boxes = new UIBubbleBox[(int)EBubbleBoxType.Count];
        public int index;
        public EBubbleType type;
        public EBubbleBoxType boxType;

        public UIBubbleBox curBox;
        public float width = 750;
        public UIBubbleData data;
        public UIBubbleList bubbleList;
        public bool isTween = false;
        public Action<UIBubbleItem> OnItemClick;

        private Transform headGroup;
        private Text nameText;
        private Image HeadImg;
        private Image textbg;
        private Image voicebg;

        public abstract Vector2 size { get; }
        
        public UIBubbleItem(EBubbleType type, UIBubbleList list, GameObject pfb)
        {
            this.type = type;
            bubbleList = list;
            this.gameObject = GameObject.Instantiate(pfb,list.transform);
            this.transform = this.gameObject.transform as RectTransform;
            this.canvasGroup = this.gameObject.GetComponent<CanvasGroup>();

            headGroup = this.transform.Find("head");
            HeadImg = headGroup.GetComponent<Image>();
            nameText = headGroup.Find("name").GetComponent<Text>();
            nameText = headGroup.Find("name").GetComponent<Text>();
       
            boxes[(int)EBubbleBoxType.Text] = new UIBubbleBox_Text(this,this.transform.Find("box/text"));
            boxes[(int)EBubbleBoxType.Image] = new UIBubbleBox_Image(this, this.transform.Find("box/image"));
            boxes[(int)EBubbleBoxType.Voice] = new UIBubbleBox_Voice(this, this.transform.Find("box/voice"));

            textbg = this.transform.Find("box/text/bg").GetComponent<Image>();
            voicebg = this.transform.Find("box/voice/bg").GetComponent<Image>();
        }



        public void SetList(UIBubbleList list)
        {
            if(bubbleList != list)
            {
                bubbleList = list;
                this.transform.SetParent(list.transform, false);
                this.transform.offsetMin = new Vector2(0, 0);
                this.transform.offsetMax = new Vector2(0, 0);
            }
        }

        public void SetIndex(int idx)
        {
            this.index = idx;
        }
        public virtual void SetBoxType(EBubbleBoxType type)
        {
            boxType = type;
            int idx = (int)type;
            this.curBox = boxes[idx];
            for(int i=0,iMax=this.boxes.Length;i<iMax;++i)
            {
                var box = boxes[i];
                box.SetActive(i == idx);
            }
        }
        public void SetActive(bool isOn)
        {
            this.gameObject.SetActiveEx(isOn);
        }

        public void DOFade(float endValue, float duration = 0)
        {
            if(duration <= 0)
            {
                canvasGroup.alpha = endValue;
            }
            else
            {

                canvasGroup.DOFade(endValue, duration).SetEase(Ease.Flash);
            }
        }

        public virtual void SetData(UIBubbleData data)
        {
            this.data = data;
            data.ui = this;
            this.SetBoxType(data.type2);
            this.curBox.RefreshUI();
            if(nameText != null)
            {
                if(data.cfgType == 1)
                {

                }
                else if(data.cfgType == 2 && data.bookCfg != null)
                {
                    nameText.text = DialogDisplaySystem.Instance.GetRoleName(data.bookCfg.role_id, UserDataManager.Instance.UserData.CurSelectBookID);
                    HeadImg.sprite = DialogDisplaySystem.Instance.GetUITexture("UI/PhoneCallHeadIcon/" + data.bookCfg.role_id);

                    Debug.LogError("dialog_type:" + data.bookCfg.dialog_type);

                    if (data.bookCfg.dialog_type==36)
                    {
                        textbg.sprite = ResourceManager.Instance.GetUISprite("BubbleForm/bg_think");
                        voicebg.sprite = ResourceManager.Instance.GetUISprite("BubbleForm/bg_think");
                    }
                    else if (data.bookCfg.dialog_type == 27)
                    {
                        textbg.sprite = ResourceManager.Instance.GetUISprite("BubbleForm/bg_chat_left");
                        voicebg.sprite = ResourceManager.Instance.GetUISprite("BubbleForm/bg_chat_left");
                    }
           
                }
            }
        }

        public void OnDeactive()
        {
            this.data = null;
        }

        public void SetSize()
        {
            this.curBox.SetSize();
        }
    }

    [XLua.LuaCallCSharp]
    public enum EBubbleType
    {
        Middle = 0, //旁白
        Left = 1,       //对方
        Right = 2,   //自己
        Count,
    }

    [XLua.LuaCallCSharp]
    public enum EBubbleBoxType
    {
        Text = 0,
        Image = 1,
        Voice = 2,
        Count,
    }
}