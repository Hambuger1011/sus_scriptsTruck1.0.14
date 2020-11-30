using System.Collections.Generic;
using UGUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MigrationAccountForm : BaseUIForm
{
    public Button BtnOK,HelpButton,HelpText,CopyBtn,ChangeAccountBtn;
    public Text KeyNum,DiamondNum,CodeNum,UserName;
    private int _loginType = 0;
    private int stepIndex = 0;
    public Transform Step,MoveBG;
    private List<GameObject> stepObjs = new List<GameObject>();
    private readonly int stepNum = 11;

    private void Awake()
    {
        var loginType = UserDataManager.Instance.moveCodeInfo.data.source;
        if (loginType == 0)
            UserDataManager.Instance.logintType = 1;
        else
        {
            UserDataManager.Instance.logintType = loginType == 1 ? 2 : 0;
        }

        Step.gameObject.SetActiveEx(false);
        stepObjs.Clear();
        for (int i = 1; i < stepNum; i++)
        {
           var stepObj =  Step.Find("Step_"+i).gameObject;
           var stepButton = stepObj.GetComponent<Button>();
           stepButton.onClick.RemoveAllListeners();
           stepButton.onClick.AddListener(HelpButtonOn);
           stepObjs.Add(stepObj);
        }
            
        addMessageListener(EventEnum.FaceBookLoginSucc, FaceBookLoginSuccHandler);
        addMessageListener(EventEnum.GoogleLoginSucc, GoogleLoginSuccHandler);
        addMessageListener(EventEnum.DeviceLoginSucc, DeviceLoginSuccHandler);
    }

    private void ShowNextStep()
    {
        for (int i = 0; i < stepNum-1; i++)
            stepObjs[i].SetActiveEx(i==stepIndex);

        if (0 == stepIndex)
        {
            Step.gameObject.SetActiveEx(true);
            MoveBG.gameObject.SetActiveEx(false);
        }

        if (stepNum-1==stepIndex)
        {
            MoveBG.gameObject.SetActiveEx(true);
            Step.gameObject.SetActiveEx(false);
            stepIndex = 0;
        }
        else
            stepIndex += 1;
    }

    private void DeviceLoginSuccHandler(Notification obj)
    {
        UserDataManager.Instance.logintType = 0;
        GameHttpNet.Instance.GetMoveCode(GetMoveCodeCallBack);
    }

    private void GoogleLoginSuccHandler(Notification obj)
    {
        _loginType = 1;
        LoginDataInfo loginInfo = obj.Data as LoginDataInfo;
        LoginByThirdParty(loginInfo, 0);
    }

    private void FaceBookLoginSuccHandler(Notification obj)
    {
        _loginType = 2;
        LoginDataInfo loginInfo = obj.Data as LoginDataInfo;
        LoginByThirdParty(loginInfo, 1);
    }

    private void LoginByThirdParty(LoginDataInfo loginInfo,int vType)
    {
        var vChannel = string.Empty;
#if UNITY_ANDROID
        vChannel = "android";
#else         
        vChannel = "ios";
#endif
        GameHttpNet.Instance.LoginByThirdParty(vType, loginInfo.Email, loginInfo.UserName, loginInfo.UserImageUrl, loginInfo.UserId, loginInfo.Token, "android", LoginByThirdPartyCallBack);
    }
    
    private void LoginByThirdPartyCallBack(object arg, EnumReLogin loginType)
    {
        string result = arg.ToString();
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if(jo != null)
        {
            LoomUtil.QueueOnMainThread((param) => 
            {
                if (jo.code == 200)
                {
                    UserDataManager.Instance.thirdPartyLoginInfo = JsonHelper.JsonToObject<HttpInfoReturn<ThirdPartyReturnInfo>>(result);
                    if (UserDataManager.Instance.thirdPartyLoginInfo != null && UserDataManager.Instance.thirdPartyLoginInfo.data != null)
                    {
                        GameHttpNet.Instance.TOKEN = UserDataManager.Instance.thirdPartyLoginInfo.data.token;
                    }
                    UserDataManager.Instance.logintType = _loginType;
                    GameHttpNet.Instance.GetMoveCode(GetMoveCodeCallBack);
                }
                else if (jo.code == 201)
                {
                    LOG.Info("--LoginByThirdPartyCallBack--->参数不完整");
                }
                else if (jo.code == 208)
                {
                    LOG.Info("--LoginByThirdPartyCallBack--->登录失败");
                }
                else if (jo.code == 277)
                {
                    UIAlertMgr.Instance.Show(GameDataMgr.Instance.table.GetLocalizationById(218)/*"TIPS"*/, jo.msg);
                    return;
                }
                UserDataManager.Instance.SaveLoginInfo();
            }, null);
        }
    }

    public override void OnOpen()
    {
        base.OnOpen();
        UpdateView();
        BtnOK.onClick.AddListener(BtnOKOn);
        CopyBtn.onClick.AddListener(CopyBtnOn);
        ChangeAccountBtn.onClick.AddListener(ChangeAccountBtnOn);
        HelpButton.onClick.AddListener(HelpButtonOn);
        HelpText.onClick.AddListener(HelpButtonOn);
    }

    public override void OnClose()
    {
        base.OnClose();
        BtnOK.onClick.RemoveListener(BtnOKOn);
        CopyBtn.onClick.RemoveListener(CopyBtnOn);
        ChangeAccountBtn.onClick.RemoveListener(ChangeAccountBtnOn);
        HelpButton.onClick.RemoveListener(HelpButtonOn);
        HelpText.onClick.RemoveListener(HelpButtonOn);
    }
    
    private void GetMoveCodeCallBack(object arg)
    {
        string result = arg.ToString();
        JsonObject jo = JsonHelper.JsonToJObject(result);
        if (jo != null)
        {
            if (jo.code == 200)
            {
                UserDataManager.Instance.moveCodeInfo = JsonHelper.JsonToObject<HttpInfoReturn<MoveCodeInfoCont>>(arg.ToString());
                UpdateView();
            }
        }
    }

    private void UpdateView()
    {
        var data = UserDataManager.Instance.moveCodeInfo.data;
        KeyNum.text = ":"+ data.bkey.ToString();
        DiamondNum.text = ":"+ data.diamond.ToString();
        CodeNum.text = data.account_code;
        UserName.text =data.nickname;
    }
    
    private void HelpButtonOn()
    {
        ShowNextStep();
    }
    
    private void BtnOKOn()
    {
        GUIUtility.systemCopyBuffer = CodeNum.text;
        Application.OpenURL("https://passport.igg.com/event/semigration/index");
    }
    
    private void CopyBtnOn()
    {
        GUIUtility.systemCopyBuffer = CodeNum.text;
    }
    
    private void ChangeAccountBtnOn()
    {
        CUIManager.Instance.OpenForm(UIFormName.MoveAccountLogin);
    }
    
}
