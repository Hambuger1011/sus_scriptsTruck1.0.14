
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
public class noviceGuideChoceChild : BaseUIForm
{
    public NoviceGuideSprite NoviceGuideSprite;
    public Text text;

    private int Numtype;

#if NOT_USE_LUA
    void Awake ()
    {
        
	}

    public override void OnOpen()
    {
        base.OnOpen();
        UIEventListener.AddOnClickListener(gameObject, buttonOnclicke);
    }

    public override void OnClose()
    {
        base.OnClose();
        UIEventListener.RemoveOnClickListener(gameObject, buttonOnclicke);
    }

    public void Inite(int num, t_BookTutorial m_currentDialogData)
    {
        string TS = "";
        float timesMove = 0.7f;
        Numtype = num;
        this.m_currentDialogData = m_currentDialogData;
        if (num==1)
        {
            TS = m_currentDialogData.selection_1;

            gameObject.transform.DOLocalMoveY(-395, timesMove);
        }else if (num==2)
        {
            TS = m_currentDialogData.selection_2;
            gameObject.transform.DOLocalMoveY(-491f, timesMove);
        }
        else if (num==3)
        {
            TS = m_currentDialogData.selection_3;
            gameObject.transform.DOLocalMoveY(-588, timesMove);
        }
        text.text = TS.ToString();
    }
	
    private void buttonOnclicke(PointerEventData data)
    {
        int value = 0;
        string TypeN = "";

        LOG.Info("typ1:" + m_currentDialogData.Type1 + "--tyto:" + m_currentDialogData.TypeTotal1);

        gameObject.transform.parent.GetComponent<noviceGuideChoce>().chocesToFalse();
        if (Numtype==1)
        {
            value = m_currentDialogData.next_1;
            MyBooksDisINSTANCE.Instance.NoviceGuideInfoChoce(m_currentDialogData.Type1, m_currentDialogData.TypeTotal1);

            TypeN = m_currentDialogData.Type1;
        }
        else if (Numtype == 2)
        {
            value = m_currentDialogData.next_2;
            MyBooksDisINSTANCE.Instance.NoviceGuideInfoChoce(m_currentDialogData.Type2, m_currentDialogData.TypeTotal2);
            TypeN = m_currentDialogData.Type2;
        }
        else if (Numtype == 3)
        {
            value = m_currentDialogData.next_3;
            MyBooksDisINSTANCE.Instance.NoviceGuideInfoChoce(m_currentDialogData.Type3, m_currentDialogData.TypeTotal3);
            TypeN = m_currentDialogData.Type3;
        }
        NoviceGuideSprite.noviceGuideChoceChildOnclick(value);

        TalkingDataManager.Instance.SelectOptions(0, m_currentDialogData.dialogID, Numtype, 0, 0);  //记录新手的选项

        if (string.IsNullOrEmpty(TypeN))
        {
            LOG.Info("表中dialogID为:"+ this.m_currentDialogData.dialogID+"--所选择相应的选项概率为空");
            return;
        }
       
        GameHttpNet.Instance.Userbookrate(2,this.m_currentDialogData.dialogID, Numtype, UserbookrateCallBacke);
    }
    
    /// <summary>
    /// 记录选择
    /// </summary>
    private void UserbookrateCallBacke(object arg)
    {
        string result = arg.ToString();
        LOG.Info("----UserbookrateCallBacke---->" + result);
        JsonObject jo = JsonHelper.JsonToJObject(result);

        if (jo != null)
        {
            if (jo.code == 200)
            {
                LoomUtil.QueueOnMainThread((param) =>
                {

                }, null);

                LOG.Info("用户选择记录成功");
            }else
            {
                LOG.Info("用户选择记录失败");
            }
        }
    }

    private void OnEnable()
    {
        gameObject.transform.localPosition = new Vector3(-65.23f, -395, 0);
    }
#endif
}