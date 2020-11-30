
using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class noviceGuideChoce : BaseUIForm
{

    public Image[] choces;
    public RectTransform Notice;
#if NOT_USE_LUA
    public override void OnOpen()
    {
        base.OnOpen();

        for (int i=0;i<choces.Length;i++)
        {
            choces[i].sprite= DialogDisplaySystem.Instance.NewFirstGetUiTexture("Atlas/Choice/bg_chat_choice");
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        for (int i = 0; i < choces.Length; i++)
        {
            choces[i].sprite = null;
        }
    }

    public void Init(t_BookTutorial m_currentDialogData)
    {
        for (int i=0;i<choces.Length;i++)
        {
            choces[i].gameObject.SetActive(false);
        }
       // LOG.Info("selection_num:"+ m_currentDialogData.selection_num);
        //生成选项的个数
        for (int i=0;i<m_currentDialogData.selection_num;i++)
        {
            choces[i].gameObject.SetActive(true);
            choces[i].gameObject.GetComponent<noviceGuideChoceChild>().Inite(i + 1, m_currentDialogData);
        }
    }

    public void chocesToFalse()
    {
        for (int i = 0; i < choces.Length; i++)
        {
            choces[i].gameObject.SetActive(false);
        }
    }

    private void ChangePos()
    {
        CancelInvoke("ChangePos");

        float H = Notice.rect.height;
        gameObject.transform.localPosition = new Vector3(0, 77 - (H - 139), 0);
    }
    private void OnEnable()
    {
        Invoke("ChangePos", 0.1f);
    }
#endif
}