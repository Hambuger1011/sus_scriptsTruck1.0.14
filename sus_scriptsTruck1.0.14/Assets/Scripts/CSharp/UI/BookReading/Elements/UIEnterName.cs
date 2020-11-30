
namespace BookReading
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using pb;

    public class UIEnterName : UIBookReadingElement
    {
        public GameObject EnterName;
        public InputField EnterInputField;
        public Button EnterNameComfirm;
        public Text DescTxt;

        [Header("-------------skin-------------")]
        public Image imgBG;
        public Image imgInputBG;

        private BookReadingForm _form;
        private BaseDialogData mDialogData;

        private string mResultName;

        private int mType;  //0：表示输入主角的名字，1：:表示输入NPC的名字
#if NOT_USE_LUA
        public override void Bind(BookReadingForm form)
        {
            _form = form;
            form.enterName = this;
            UIEventListener.AddOnClickListener(this.EnterNameComfirm.gameObject, setPlayerName);
            this.EnterInputField.onValueChanged.AddListener(onEnterNameInputValueChange);

            t_BookDetails bookDetail = GameDataMgr.Instance.table.GetBookDetailsById(DialogDisplaySystem.Instance.CurrentBookData.BookID);

            Color returnColor = Color.white;
            ColorUtility.TryParseHtmlString("#" + bookDetail.EnterNameColor, out returnColor);
            EnterInputField.textComponent.color = returnColor;
            DescTxt.color = returnColor;

        }
        public override void SetSkin()
        {
            imgBG.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/EnterName/bg.png");
            imgInputBG.sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/EnterName/bg_input.png");
        }

        public override void ResetUI()
        {
            this.EnterName.SetActive(false);
        }

        public void Show(BaseDialogData vDialogData,int vType = 0)
        {
            int leadingSex = 0;
            if(UserDataManager.Instance.UserData != null)
            {
                t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(UserDataManager.Instance.UserData.CurSelectBookID);
                leadingSex = bookDetails.Gender;
            }


            mDialogData = vDialogData;
            mType = vType;
            this.EnterName.SetActive(true);
            this.EnterInputField.text = "";
            onEnterNameInputValueChange(this.EnterInputField.text);
            if (mType == 0)
            {
                DescTxt.text = "Enter your name";
                if(!string.IsNullOrEmpty(UserDataManager.Instance.UserData.bookNickName))
                {
                    EnterInputField.text = UserDataManager.Instance.UserData.bookNickName;
                }else
                {
                    string randomName = GameDataMgr.Instance.table.GetNameByType(leadingSex);
                    EnterInputField.text = (!string.IsNullOrEmpty(randomName)) ? randomName : "";
                }
            }
            else
            {
                DescTxt.text = "Enter your partner's name";
                string randomName  = "";

                if (DialogDisplaySystem.Instance.CurrentBookData != null && DialogDisplaySystem.Instance.CurrentBookData.NpcDetailId > 3)
                {
                    //女生
                    randomName = GameDataMgr.Instance.table.GetNameByType(1);
                }
                else
                {
                    //男生
                    randomName = GameDataMgr.Instance.table.GetNameByType(0);
                }

                
                EnterInputField.text = (!string.IsNullOrEmpty(randomName)) ? randomName : "";
            }
#if ENABLE_DEBUG
            if (GameDataMgr.Instance.InAutoPlay)
            {
                Invoke("AutoDo", 1f);
            }
#endif
        }

        private void ChangeNameCallBackHandler(object arg)
        {
            string result = arg.ToString();
            LOG.Info("----ChangeNameCallBackHandler---->" + result);
            if (result.Equals("error"))
            {
                LOG.Info("设置名字失败，协议返回错误");
                return;
            }

            JsonObject jo = JsonHelper.JsonToJObject(result);
            if (jo != null)
            {
                LoomUtil.QueueOnMainThread((param) =>
                {
                    if (jo.code == 200)
                    {
                        //UITipsMgr.Instance.PopupTips("Name changed successfully!", false);
                        
                        UserDataManager.Instance.userInfo.data.userinfo.nickname = mResultName;
                    }
                }, null);

            }
        }

        private void AutoDo()
        {
            CancelInvoke("AutoDo");
            this.EnterInputField.text = "Test-" + UnityEngine.Random.Range(100, 999);
            setPlayerName(null);
        }

        /// <summary>
        /// 输出值变化
        /// </summary>
        private void onEnterNameInputValueChange(string str)
        {
            if (this.EnterInputField.text.Length < 2 || this.EnterInputField.text.Length > 12)
            {
                this.EnterNameComfirm.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/EnterName/btn_prey_64.png");
            }
            else
            {
                this.EnterNameComfirm.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.LoadSprite("atlas/EnterName/btn_purple_64.png");
            }
            //enterName.EnterInputField.text = enterName.EnterInputField.text.ToUpper();
        }

        /// <summary>
        /// 确定名字
        /// </summary>
        private void setPlayerName(PointerEventData data)
        {
            AudioManager.Instance.PlayTones(AudioTones.dialog_choice_click);
            if (this.EnterInputField.text.Length < 2 || this.EnterInputField.text.Length > 12)
            {
                return;
            }
            string inputName = this.EnterInputField.text;
            int npcId = 0;
            string npcName = string.Empty;
            if (mType == 0)
            {
                npcId = 0;
                DialogDisplaySystem.Instance.CurrentBookData.PlayerName = inputName;
                if (string.IsNullOrEmpty(UserDataManager.Instance.UserData.bookNickName) ||
                    UserDataManager.Instance.UserData.bookNickName != inputName)
                {
                    UserDataManager.Instance.UserData.bookNickName = inputName;
                }

                mResultName = inputName;
                if (string.IsNullOrEmpty(UserDataManager.Instance.userInfo.data.userinfo.nickname) && !string.IsNullOrEmpty(mResultName))
                {
                    GameHttpNet.Instance.SetUserLanguage(mResultName, 2, ChangeNameCallBackHandler);
                }
            }
            else
            {
                npcId = (mDialogData != null)?mDialogData.role_id:2;
                npcName = inputName;
                DialogDisplaySystem.Instance.CurrentBookData.NpcName = inputName;
            }
            GameHttpNet.Instance.SendPlayerProgress(UserDataManager.Instance.UserData.CurSelectBookID,
                DialogDisplaySystem.Instance.CurrentBookData.ChapterID, DialogDisplaySystem.Instance.CurrentBookData.DialogueID,
                0, 0, 0, 0, DialogDisplaySystem.Instance.CurrentBookData.PlayerName, 0, 0, npcId,npcName,0, StartReadChapterCallBack);
            
            this.EnterName.SetActive(false);
            _form.setBGOnClickListenerActive(true);
            EventDispatcher.Dispatch(EventEnum.DialogDisplaySystem_PlayerMakeChoice, null);
        }

        private void SetNameCallBackHandler(object arg)
        {
            string result = arg.ToString();
            LOG.Info("----SetNameCallBackHandler---->" + result);

        }
        private void StartReadChapterCallBack(object arg)
        {
            string result = arg.ToString();
            LOG.Info("----StartReadChapterCallBack---->" + result);

        }

        public override void Dispose()
        {

            UIEventListener.RemoveOnClickListener(this.EnterNameComfirm.gameObject, setPlayerName);
            this.EnterInputField.onValueChanged.RemoveListener(onEnterNameInputValueChange);

            imgBG.sprite = null;
            imgInputBG.sprite = null;
            this.EnterNameComfirm.GetComponent<Image>().sprite = null;
        }
#endif

    }
}