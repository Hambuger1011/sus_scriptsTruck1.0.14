
using pb;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class NoviceGuideSprite : BaseUIForm
{
    public GameObject BG;
    public Transform Player, OtherPlayer;
    public GameObject PlayerNotice, OtherPlayerNotice,playerChoce,otherChoce;
    public Image PlayerBg, Playertou, Playerface,playerNoticebg;
    public Image otherPlayerBg, otherTou, OtherFace,otherNoticebg;
    public TextTyperAnimation playerText, otherText;
    public CanvasGroup PlayerCanvaGroup, OtherCanvaGroup;
    public RectTransform PlayNoti, OtherNoti;


    private TextTyperAnimation PlyerTextTyperAnimation, OtherTextTextTyperAnimation;
    private t_BookTutorial m_currentDialogData;
    private bool iscantouch = false,iscanmove=false,iscanendManiUI=false;
    private float BGvalue;
    private int dialog_type = 0;

#if NOT_USE_LUA
    public override void OnOpen()
    {
        base.OnOpen();

        UIEventListener.AddOnClickListener(BG, NoviceGuideBgOnclicke);

        CUIManager.Instance.OpenForm(UIFormName.Global);
        GlobalForm.Instance.EnableTouchEffect();

        PlyerTextTyperAnimation = playerText.GetComponent<TextTyperAnimation>();
        OtherTextTextTyperAnimation = otherText.GetComponent<TextTyperAnimation>();

        SreenBGAdaptive();//背景的自适应

        m_currentDialogData = GetBaseDialogDataByDialogueID(1);//（默认加载第一行）加载对应的行的表的数据

        playerNoticebg.sprite= DialogDisplaySystem.Instance.NewFirstGetUiTexture("Atlas/Dialog/bg_chat_left");
        otherNoticebg.sprite = DialogDisplaySystem.Instance.NewFirstGetUiTexture("Atlas/Dialog/bg_chat_right");
        BG.GetComponent<Image>().sprite = DialogDisplaySystem.Instance.NewFirstGetUiTexture("UI/UITexture/Chapter_1_SceneBG/24");

        playerChoce.SetActive(false);
        otherChoce.SetActive(false);

        if (m_currentDialogData!=null)
        {
            iscantouch = true;

            NoviceGuideBgOnclicke(null);//默认第一进去是点击的
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(BG, NoviceGuideBgOnclicke);
    }

    /// <summary>
    /// 这个是选项按钮点击了之后
    /// </summary>
    /// <param name="next"></param>
   public void noviceGuideChoceChildOnclick(int next)
    {
        BG.GetComponent<Image>().raycastTarget = true;
        ShowNextDialog(next);
        NoviceGuideBgOnclicke(null);
    }
    private void NoviceGuideBgOnclicke(PointerEventData data)
    {
       
        if (iscanendManiUI)
        {           
            //当这个是最后的选项的时候，需要多点一次才跳转
            if (m_currentDialogData.trigger == 2)
            {
                //跳转到书架
                LOG.Info("跳到书架");
                GoToBookeShele();
            }
            else if (m_currentDialogData.trigger == 3)
            {
                //跳转到故事              
                GoToStory();
            }           
            return;
        }

        if (!iscantouch)
        {
            LOG.Info("冷却时间没到");
            return;
        }
        PlyerTextTyperAnimation.StopTyperTween();
        OtherTextTextTyperAnimation.StopTyperTween();

        playerText.text = "";
        otherText.text = "";

      iscantouch = false;
        BGMove(m_currentDialogData.Scenes_X);

        if (dialog_type!= m_currentDialogData.dialog_type)
        {
            //同一个人的话不需要刷新对话框的显示状态
            PlayerNotice.SetActive(false);
            OtherPlayerNotice.SetActive(false);

        }
       
            //LOG.Info("点击的dialog_type是：" + m_currentDialogData.dialog_type+ "---dialogID:"+ m_currentDialogData.dialogID);

            switch (m_currentDialogData.dialog_type)
        {
            case (int)DialogType.Negative://没有
            case (int)DialogType.Narration://旁白
                ShowNextDialog(m_currentDialogData.next);
                break;
            case (int)DialogType.EnterName://输入名称
            case (int)DialogType.EnterNPCname://输入名称
                ShowNextDialog(m_currentDialogData.next);
                break;
            case (int)DialogType.PlayerDialogue://主角对话
               
                PlayerShow();
                break;
            case (int)DialogType.OtherDialogue://其他角色对话
               
                OtherPlayerShow();               
                break;
            case (int)DialogType.ChangeClothes://换装
                ShowNextDialog(m_currentDialogData.next);
                break;
            case (int)DialogType.ChangeSceneByBlack://场景跳转
            case (int)DialogType.ChangeSceneToBlack:
            case (int)DialogType.ChangeSceneByWhite:
            case (int)DialogType.ChangeSceneByWave:
            case (int)DialogType.ChangeSceneByShutter:
            case (int)DialogType.StoryItems:
                ShowNextDialog(m_currentDialogData.next);
                break;
            case (int)DialogType.PlayerImagineDialogue://主角想象语言
                ShowNextDialog(m_currentDialogData.next);
                break;
            case (int)DialogType.SceneTap://点击屏幕的小游戏
                ShowNextDialog(m_currentDialogData.next);
                break;
            case (int)DialogType.OtherImagineDialogue://配角想象语言
            case (int)DialogType.PhoneCallDialogue://手机对话剧情
            case (int)DialogType.BubbleChat:
            case (int)DialogType.BubbleNarration:
            case (int)DialogType.BubblePlayerDialog:
            case (int)DialogType.BubbleOtherPlayerDialog:
            case (int)DialogType.SceneInteraction://场景互动
            case (int)DialogType.ManualChangeScene://手动切换场景
            case (int)DialogType.Puzzle://拼图
            case (int)DialogType.ClockChangeScene://时钟切换场景
                ShowNextDialog(m_currentDialogData.next);
                break;
                default :
                ShowNextDialog(m_currentDialogData.next);
                break;
        }
        dialog_type = m_currentDialogData.dialog_type;
    }

    private void PlayNotiSizeDelta()
    {
        float duration = 0.3f;
        float basePadding = 110;
        playerText.text = StringUtils.ReplaceChar(m_currentDialogData.dialog).ToString();
        PlyerTextTyperAnimation.DoTyperTween();
        float rectHeight = playerText.preferredHeight + basePadding;
        PlayNoti.sizeDelta = new Vector2(650f, basePadding);
        DOTween.To(() => PlayNoti.sizeDelta.y, (height) => PlayNoti.sizeDelta = new Vector2(PlayNoti.sizeDelta.x, height), rectHeight * 1.05f, duration).SetEase(Ease.Flash)
            .OnComplete(() => {
                
                DOTween.To(() => PlayNoti.sizeDelta.y, (height) => PlayNoti.sizeDelta = new Vector2(PlayNoti.sizeDelta.x, height), rectHeight, duration * 0.5f).SetEase(Ease.Flash); 
            });


    }

    private void OtherNotiSizeDelta()
    {
        float duration = 0.3f;
        float basePadding = 110;
        otherText.text = StringUtils.ReplaceChar(m_currentDialogData.dialog).ToString();
        OtherTextTextTyperAnimation.DoTyperTween();
        float rectHeight = otherText.preferredHeight + basePadding;
        OtherNoti.sizeDelta = new Vector2(650f, basePadding);
        DOTween.To(() => OtherNoti.sizeDelta.y, (height) => OtherNoti.sizeDelta = new Vector2(OtherNoti.sizeDelta.x, height), rectHeight * 1.05f, duration).SetEase(Ease.Flash)
            .OnComplete(() => {
                
                
                DOTween.To(() => OtherNoti.sizeDelta.y, (height) => OtherNoti.sizeDelta = new Vector2(OtherNoti.sizeDelta.x, height), rectHeight, duration * 0.5f).SetEase(Ease.Flash); 
            });
    }

    /// <summary>
    /// 这个是主角出现
    /// </summary>
    private void PlayerShow()
    {
        float duration = 0.3f;

        PlayerBg.sprite= DialogDisplaySystem.Instance.NewFirstGetUiTexture("UI/UITexture/RoleHeadFacialExpression/bg_l_03");
        Playertou.sprite= DialogDisplaySystem.Instance.NewFirstGetUiTexture("UI/UITexture/RoleHead/120101");
        Playerface.sprite= DialogDisplaySystem.Instance.NewFirstGetUiTexture("UI/UITexture/RoleHeadFacialExpression/12010"+ m_currentDialogData.phiz_id);

       
        PlayerNotice.SetActive(true);
        OtherPlayerNotice.SetActive(false);

        //其他人物隐蔽
        OtherPlayer.DOScale(new Vector3(0.6f, 0.6f, 1), duration).SetEase(Ease.Flash);
        OtherCanvaGroup.DOFade(0, duration).SetEase(Ease.Flash);

        OtherPlayer.DOLocalMove(new Vector3(540, -200f, 0), duration);
        OtherPlayerNotice.transform.DOLocalMoveX(540, duration);

        if (dialog_type != m_currentDialogData.dialog_type)
        {
            Player.localScale = new Vector3(0f, 0f, 1);
            PlayerCanvaGroup.alpha = 0;

            Player.DOScale(new Vector3(1, 1, 1), duration+0.1f).SetEase(Ease.Flash);
            PlayerCanvaGroup.DOFade(1, duration + 0.1f).SetEase(Ease.Flash);        
        }
        Player.DOLocalMove(new Vector3(25,-20,0), duration + 0.1f).SetEase(Ease.Flash).OnComplete(() => {

        });

        PlayNoti.sizeDelta = new Vector2(650f, 150f);
        PlayNotiSizeDelta();
        PlayerNotice.transform.DOLocalMoveX(0, duration).SetEase(Ease.Flash).OnComplete(()=> 
        {
            
            if (m_currentDialogData.trigger == 0)
            {
                //普通对话
                playerChoce.SetActive(false);
                otherChoce.SetActive(false);

                ShowNextDialog(m_currentDialogData.next);
            }
            else if (m_currentDialogData.trigger == 1)
            {
                BG.GetComponent<Image>().raycastTarget = false;
                //文字选项
                playerChoce.SetActive(true);
                otherChoce.SetActive(false);
                playerChoce.GetComponent<noviceGuideChoce>().Init(m_currentDialogData);
            }
            else if (m_currentDialogData.trigger == 2)
            {
                if (!iscanendManiUI)
                {
                    //当这个是最后的选项的时候，需要多点一次才跳转
                    iscanendManiUI = true;
                    return;
                }
                //到书架
                //playerChoce.SetActive(false);
                //otherChoce.SetActive(false);

                //LOG.Info("跳到书架");
                //GoToBookeShele();
            }
            else if (m_currentDialogData.trigger == 3)
            {
                if (!iscanendManiUI)
                {
                    //当这个是最后的选项的时候，需要多点一次才跳转
                    iscanendManiUI = true;
                    return;
                }
                ////到故事
                //playerChoce.SetActive(false);
                //otherChoce.SetActive(false);

                //int bookid = MyBooksDisINSTANCE.Instance.NoviceGuideToBooke();
                //LOG.Info("跳到故事，书本的id是：" + bookid);

                //UserDataManager.Instance.UserData.CurSelectBookID = bookid;
                //GoToStory();
            }
        });          
    }

   
    private void OtherPlayerShow()
    {
        float duration = 0.3f;
        otherPlayerBg.sprite = DialogDisplaySystem.Instance.NewFirstGetUiTexture("UI/UITexture/RoleHeadFacialExpression/bg_r_jghbn1");
        otherTou.sprite = DialogDisplaySystem.Instance.NewFirstGetUiTexture("UI/UITexture/RoleHead/110101");
        OtherFace.sprite = DialogDisplaySystem.Instance.NewFirstGetUiTexture("UI/UITexture/RoleHeadFacialExpression/11010" + m_currentDialogData.phiz_id);

      
        PlayerNotice.SetActive(false);
        OtherPlayerNotice.SetActive(true);

        //主角隐藏
        Player.DOScale(new Vector3(0.6f, 0.6f, 1), duration).SetEase(Ease.Flash);
        PlayerCanvaGroup.DOFade(0, duration).SetEase(Ease.Flash);
        Player.DOLocalMove(new Vector3(-540, -200, 0), duration);
        PlayerNotice.transform.DOLocalMoveX(-540, duration);

        if (dialog_type != m_currentDialogData.dialog_type)
        {
            //同一个人的话不需要刷新对话框的显示状态
            OtherPlayer.localScale = new Vector3(0f, 0f, 1);
            OtherCanvaGroup.alpha = 0;
            OtherPlayer.DOScale(new Vector3(1, 1, 1), duration + 0.1f).SetEase(Ease.Flash);
            OtherCanvaGroup.DOFade(1, duration + 0.1f).SetEase(Ease.Flash);
        }

        OtherPlayer.DOLocalMove(new Vector3(5,-20,0), duration + 0.1f).SetEase(Ease.Flash).OnComplete(() => {

        });
        //其他人出现     
        OtherNoti.sizeDelta = new Vector2(650f, 150f);
        
        OtherNotiSizeDelta();
        OtherPlayerNotice.transform.DOLocalMoveX(0, duration).SetEase(Ease.Flash).OnComplete(()=> 
        {
            

            if (m_currentDialogData.trigger == 0)
            {
                playerChoce.SetActive(false);
                otherChoce.SetActive(false);

                //普通对话
                ShowNextDialog(m_currentDialogData.next);
            }
            else if (m_currentDialogData.trigger == 1)
            {
                BG.GetComponent<Image>().raycastTarget = false;
                //文字选项
                PlayerNotice.SetActive(true);

                playerChoce.SetActive(false);
                otherChoce.SetActive(true);

                otherChoce.GetComponent<noviceGuideChoce>().Init(m_currentDialogData);
            }
            else if (m_currentDialogData.trigger == 2)
            {
                if (!iscanendManiUI)
                {
                    //当这个是最后的选项的时候，需要多点一次才跳转
                    iscanendManiUI = true;
                    return;
                }

                //playerChoce.SetActive(false);
                //otherChoce.SetActive(false);

                ////到书架
                //LOG.Info("跳到书架");
                //GoToBookeShele();
            }
            else if (m_currentDialogData.trigger == 3)
            {
                if (!iscanendManiUI)
                {
                    //当这个是最后的选项的时候，需要多点一次才跳转
                    iscanendManiUI = true;
                    return;
                }
               
                //GoToStory();
            }
        });
    }

    /// <summary>
    /// 跳转到书架
    /// </summary>
    private void GoToBookeShele()
    {
        playerChoce.SetActive(false);
        otherChoce.SetActive(false);

        iscanendManiUI = false;
         //打开主界面
            XLuaManager.Instance.GetLuaEnv().DoString(@"logic.UIMgr:Open(logic.uiid.UIMainForm);");
        // CUIManager.Instance.OpenForm(UIFormName.MainForm/*, useFormPool: true*/);
        CUIManager.Instance.CloseForm(UIFormName.NoviceGuide);
        CUIManager.Instance.CloseForm(UIFormName.Global);
    }

    /// <summary>
    /// 调到故事
    /// </summary>
    private void GoToStory()
    {
        playerChoce.SetActive(false);
        otherChoce.SetActive(false);
        int bookid = MyBooksDisINSTANCE.Instance.NoviceGuideToBooke();
        LOG.Info("跳到故事，书本的id是：" + bookid);
        UserDataManager.Instance.UserData.CurSelectBookID = bookid;

        iscanendManiUI = false;
        UserDataManager.Instance.UserData.IsSelectFirstBook = 1;

        TalkingDataManager.Instance.SelectBooksInEnter(UserDataManager.Instance.UserData.CurSelectBookID);
        TalkingDataManager.Instance.onStart("ReadChapterStart_" + UserDataManager.Instance.UserData.CurSelectBookID + "_" + 1);

        GameDataMgr.Instance.userData.AddMyBookId(UserDataManager.Instance.UserData.CurSelectBookID,true);
        initDialogDisplaySystemData(0);
        DialogDisplaySystem.Instance.PrepareReading();

        CUIManager.Instance.CloseForm(UIFormName.NoviceGuide);
        CUIManager.Instance.CloseForm(UIFormName.Global);
    }
    private void initDialogDisplaySystemData(int vChapterID)
    {
        int dialogueID = 1;
        DialogDisplaySystem.Instance.ChangeBookDialogPath(UserDataManager.Instance.UserData.CurSelectBookID, vChapterID);
        int index = vChapterID;
        BookData bookData = UserDataManager.Instance.UserData.BookDataList.Find((bookdata) => bookdata.BookID == UserDataManager.Instance.UserData.CurSelectBookID);
        t_BookDetails bookDetails = GameDataMgr.Instance.table.GetBookDetailsById(UserDataManager.Instance.UserData.CurSelectBookID);
        int[] chapterDivisionArray = bookDetails.ChapterDivisionArray;
        int endDialogID = -1;
        int beginDialogID = index - 1 < 0 ? 1 : chapterDivisionArray[index - 1];
        if (index < chapterDivisionArray.Length)
        {
            endDialogID = chapterDivisionArray[index];
        }

        DialogDisplaySystem.Instance.InitByBookID(UserDataManager.Instance.UserData.CurSelectBookID, 1, dialogueID, beginDialogID, endDialogID);   
    }
    private void BGMove(float value)
    {
        if (BGvalue==value|| iscanmove)
        {
            return;
        }
        BGvalue = value;
        iscanmove = true;
        BG.transform.DOLocalMoveX(value, 0.5F).OnComplete(()=> {
            iscanmove = false;
        });
    }

    public void ShowNextDialog(int id, bool forceShow = false)
    {

        if (m_currentDialogData.next != -1)
            m_currentDialogData = GetBaseDialogDataByDialogueID(id);
        if (m_currentDialogData != null)
        {
            iscantouch = true;
        }
        else
        {
            LOG.Error("Next is Null!!!");
        }
    }
    private t_BookTutorial GetBaseDialogDataByDialogueID(int dialogueID)
    {
        if (dialogueID == -1) return null;

        //开始读表
        t_BookTutorial data = GameDataMgr.Instance.table.GetNewFistDialogById(dialogueID);
        if (data == null)
        {
            LOG.Error("对话结束:" + dialogueID);
            return null;
        }      
        return data;
    }

    private void SreenBGAdaptive()
    {
        //给背景设置图片
        Image bg = BG.GetComponent<Image>();
        bg.SetNativeSize();

        //这个是设置图片的大小与屏幕高度一致
        RectTransform rec = bg.gameObject.GetComponent<RectTransform>();
        float Pw = rec.rect.width;
        float Ph = rec.rect.height;
        float w = Screen.width;
        float h = Screen.height;
       // LOG.Info("屏幕宽度：" + w + "--屏幕高度：" + h + "--图片的宽度:" + Pw + "--图片的高度：" + Ph);
        bg.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Pw * h / Ph * 1.0f, h);

    }
    private void OnEnable()
    {
        //存储表的数据入字典
        GameDataMgr.Instance.table.NewFistLoadDialogData();
    }
#endif
}