

using GameCore.UGUI;
using GameCore.UI;
using IGG.SDK.Core.Error;
using IGG.SDK.Modules.AgreementSigning.VO;
using Script.Game.Helpers;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementForm : BaseUIForm
{
    public Text content;
    public Text title;
    public Text agreementText4;
    public Text agreementText3;
    public Text agreementText2;
    public Text agreementText1;
    public Text btnOKText;
    public UITweenButton agreement4;
    public UITweenButton agreement3;
    public UITweenButton agreement2;
    public UITweenButton agreement1;
    public UITweenButton btnOK;
    private IGGAgreementSigningFile agreementSigningFile;
    
    private void Start()
    {
    }

    public override void OnOpen()
    {
        base.OnOpen();
        btnOK.onClick.AddListener(OnAgreeClick);
        agreementSigningFile = IGGAgreementManager.Instance.iGGAgreementSigningStatus;
        title.text = agreementSigningFile.LocalizedTitle;
        content.text = agreementSigningFile.LocalizedCaption;
        btnOKText.text = agreementSigningFile.LocalizedActionSign;
        var agreements = agreementSigningFile.GetAgreements();
        agreement1.gameObject.SetActiveEx(false);
        agreement2.gameObject.SetActiveEx(false);
        agreement3.gameObject.SetActiveEx(false);
        agreement4.gameObject.SetActiveEx(false);
        if (null != agreements)
        {
            if (agreements.Count > 0)
            {
                agreement1.gameObject.SetActiveEx(true);
                agreementText1.text = agreements[0].LocalizedName;
                agreement1.onClick.RemoveAllListeners();
                agreement1.onClick.AddListener(delegate
                {
                    Application.OpenURL(agreements[0].URL);
                });
            }
        
            if (agreements.Count > 1)
            {
                agreement2.gameObject.SetActiveEx(true);
                agreementText2.text = agreements[1].LocalizedName;
                agreement2.onClick.RemoveAllListeners();
                agreement2.onClick.AddListener(delegate
                {
                    Application.OpenURL(agreements[1].URL);
                });
            }
        
            if (agreements.Count > 2)
            {
                agreement3.gameObject.SetActiveEx(true);
                agreementText3.text = agreements[2].LocalizedName;
                agreement3.onClick.RemoveAllListeners();
                agreement3.onClick.AddListener(delegate
                {
                    Application.OpenURL(agreements[2].URL);
                });
            }
        
            if (agreements.Count > 3)
            {
                agreement4.gameObject.SetActiveEx(true);
                agreementText4.text = agreements[3].LocalizedName;
                agreement4.onClick.RemoveAllListeners();
                agreement4.onClick.AddListener(delegate
                {
                    Application.OpenURL(agreements[3].URL);
                });
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    
    public override void OnClose()
    {
        base.OnClose();
    }


    /// <summary>
    /// 签署协议(可以异步执行签署)。
    /// </summary>
    private void OnAgreeClick()
    {
        this.myForm.Close();
        if (null == agreementSigningFile)
        {
            return;
        }
        var agreementSigning = KungfuInstance.Get().GetPreparedAgreementSigning();
        // 签署协议
        agreementSigning.Sign(agreementSigningFile, delegate (IGGError error)
        {
            if (error.IsOccurred())
            {
                LOG.ShowIGGException(error);
                LOG.Error("签署协议异常error="+error.GetCode());
            }
        });
    }

}
