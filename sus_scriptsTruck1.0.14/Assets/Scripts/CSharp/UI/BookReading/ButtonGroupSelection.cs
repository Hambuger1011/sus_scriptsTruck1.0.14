
using AB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroupSelection : MonoBehaviour {

    public Image ButtonBG;
    public Text ButtonSelectionText;
    public Image DiamaondImage;
    public Text CostText;
    public GameObject SelectEffect;

    private Sprite pathButtonBGNormal;// "BookReadingForm/bg_chat_choice";
    private Sprite pathButtonBGDiamond;// "BookReadingForm/bg_chat_choice_2";

    public int Cost { get { return m_iCost; } }
    private int m_iCost;

    public int HiddenEgg { get { return m_hidEgg; } }
    private int m_hidEgg;

    public int Index { get { return mIndex; } }
    private int mIndex;
#if NOT_USE_LUA
    public void Init(string text , int cost,int hiddenEgg,int vIndex)
    {
        pathButtonBGNormal = DialogDisplaySystem.Instance.LoadSprite("atlas/Choice/bg_chat_choice.png");
        pathButtonBGDiamond = DialogDisplaySystem.Instance.LoadSprite("atlas/Choice/bg_chat_choice2.png");

        this.transform.localScale = Vector3.one;
        m_hidEgg = hiddenEgg;
        mIndex = vIndex;
        ButtonSelectionText.text = StringUtils.ReplaceChar(text);

        if(cost != 0)
        {
            if(UserDataManager.Instance.CheckDialogOptionHadCost(UserDataManager.Instance.UserData.CurSelectBookID,
                DialogDisplaySystem.Instance.CurrentBookData.DialogueID,vIndex + 1))
            {
                cost = 0;
            }

            if (UserDataManager.Instance.CheckBookHasBuy(UserDataManager.Instance.UserData.CurSelectBookID))
                cost = 0;
        }


        if (cost != 0)
        {
            ButtonBG.sprite = pathButtonBGDiamond;
            DiamaondImage.gameObject.SetActive(true);
            m_iCost = cost;
            CostText.text = cost.ToString();
            SelectEffect.SetActive(true);
        }
        else
        {
            ButtonBG.sprite = pathButtonBGNormal;
            DiamaondImage.gameObject.SetActive(false);
            SelectEffect.SetActive(false);
        }
    }

    public void Dispose()
    {
        pathButtonBGNormal = null;
        pathButtonBGDiamond = null;
        ButtonBG.sprite = null;
    }
#endif

}