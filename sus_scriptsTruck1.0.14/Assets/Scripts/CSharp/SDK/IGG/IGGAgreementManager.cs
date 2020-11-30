using System.Collections.Generic;
using IGG.SDK.Core.Error;
using IGG.SDK.Modules.AgreementSigning;
using IGG.SDK.Modules.AgreementSigning.UI;
using IGG.SDK.Modules.AgreementSigning.VO;
using IGGUtils;
using Script.Game.Helpers;
using UGUI;
using UnityEngine;

[XLua.LuaCallCSharp]
public class IGGAgreementManager : Singleton<IGGAgreementManager>
{

    public IGGAgreementSigningFile iGGAgreementSigningStatus;
    public List<IGGAgreement> IggAgreementList;
    private IGGAgreementSigning _agreementSigning;

    public void Init()
    {
        if (null == _agreementSigning)
        {
            _agreementSigning = KungfuInstance.Get().GetPreparedAgreementSigning();
            _agreementSigning.RequestAssignedAgreements(delegate (IGGError ex, IGGAssignedAgreements assignedAgreements)
            {
                if (ex.IsOccurred())
                {
                    LOG.ShowIGGException(ex);
                    return;
                }
                IggAgreementList = assignedAgreements.Agreements;
            });
        }
    }
    
    /// <summary>
    /// 请求当前IGGID的协议同意情况（自定义请求时机，目前后台仅配置了asap与kindly，asap代表尽快请求协议，比如游戏一启动就需要请求协议，这时候
    /// 就需要asap的类型。kindly代表延后请求协议，比如游戏会在玩家过了新手引导之后，才去请求游戏协议，这时候就
    /// 需要kindly）。
    /// </summary>
    public void OnRequestStatusCustomClick()
    {
        
        _agreementSigning.Signing().InformIfMatch("kindly", delegate (IGGError error, IGGAgreementSigningStatus status)
        {
            ShowSigningDialog(error, status);
        });
    }
    
    /// <summary>
    /// 显示需要签署协议的对话框
    /// </summary>
    /// <param name="error"></param>
    /// <param name="status"></param>
    private void ShowSigningDialog(IGGError error, IGGAgreementSigningStatus status)
    {
        // Tips:当iggAgreementSigningStatus != null且从agreementSigningFile中取到协议列表不为空则说明该玩家还
        // 有未同意的协议，这时候需要显示协议同意对话框给玩家吗，让玩家执行同意之后再进游戏。
        // 1(IGGAgreementType.IGGAgreementTermsOfService) 用户条款 | 2(IGGAgreementType.IGGAgreementPrivacyPolicy)  隐私协议 | 3(IGGAgreementType.IGGAgreementTermsOfSubscription) 订阅协议
        if (null != status)
        {
            List<int> agreementOrderInType = new List<int>(){(int)IGGAgreementType.TermsOfService, (int)IGGAgreementType.PrivacyPolicy
            };
            var agreementSigningFile = status.PrepareFileToBeSigned(null); // prepareFileToBeSigned接口将根据其参数agreementOrderInType来排序协议列表, 不传将返回服务端的顺序。
            if (null != agreementSigningFile)
            {
                // 显示协议同意弹窗的时候，法务部会归档协议列表显示的顺序，游戏研发那边请让运营跟法务部确定好该顺序
                iGGAgreementSigningStatus = agreementSigningFile;
                CUIManager.Instance.OpenForm(UIFormName.Announcement);
                return;
            }
        }
        if (error.IsOccurred())
        {
            if (!StringHelper.Equals(error.GetCode(), IGG.SDK.Modules.AgreementSigning.Error.REQUEST_STATUS_FOR_HAS_AGREE))
            {
                LOG.ShowIGGException(error);
            }
            else
            {
                // 代表该账号已同意过协议，游戏那边可以不用提示，Demo这边只是为了方便QA测试。
                LOG.ShowIGGException(error);
            }
                
            return;
        }
        else
        {
            // 玩家上次同意协议还未超过24小时，所以就不向服务端请求该玩家的同意的协议情况。
            LOG.Error("24小时后再试或者这个场景下不需要请求协议同意情况。");
        }  
    }
    
    /// <summary>
    /// 请求当前IGGID的协议同意情况(asap)
    /// </summary>
    public void OnRequestStatusAsapClick()
    {
        _agreementSigning.Signing().InformAsap(delegate (IGGError error, IGGAgreementSigningStatus status)
        {
            ShowSigningDialog(error, status);
        });
    }
    
    /// <summary>
    /// 请求当前IGGID的协议同意情况(kindly)
    /// </summary>
    public void OnRequestStatusKindlyClick()
    {
        _agreementSigning.Signing().InformKindly(delegate (IGGError error, IGGAgreementSigningStatus status)
        {
            ShowSigningDialog(error, status);
        });
    }
    
    /// <summary>
    /// 请求该游戏在IGG后台配置的所有游戏协议列表，并显示，研发请关注协议中各字段的值怎么用于UI显示
    /// </summary>
    public void OnRequestAgreementSettingClick()
    {
        _agreementSigning.RequestAssignedAgreements(delegate (IGGError ex, IGGAssignedAgreements assignedAgreements)
        {
            // 拿到游戏配置的协议列表后做相应的视图显示
            if (ex.IsOccurred())
            {
                LOG.ShowIGGException(ex);
                return;
            }
            ShowAgreements(assignedAgreements.Agreements);
        });
    }
    
    /// <summary>
    /// 显示协议列表
    /// </summary>
    private void ShowAgreements(List<IGGAgreement> agreements)
    {
        
        if (agreements == null || agreements.Count == 0)
        {
            Debug.LogError("未配置协议，请在后台配置！");
        }

        foreach (var VARIABLE in agreements)
        {
            LOG.Error(VARIABLE.ID + "\nLocalizedName=" + VARIABLE.LocalizedName + "\nTitle=" + VARIABLE.Title + "\nType=" + VARIABLE.Type
                      + "\nVersion=" + VARIABLE.Version + "\nInnerVersion=" + VARIABLE.InnerVersion + "\nURL=" + VARIABLE.URL);
        }
    }
    
    /// <summary>
    /// 请求该IGGID已签署的的所有游戏协议列表，并显示，研发请关注协议中各字段的值怎么用于UI显示
    /// </summary>
    public void OnRequestAgreementSignedClick()
    {
        _agreementSigning.Termination().RequestAssignedAgreements(delegate (IGGError error, IGGAgreementSigningFile agreementSigningFile, IGGAgreementTerminationAlert agreementTerminationAlert)
        {
            // 拿到游戏配置的协议列表后做相应的视图显示
            if (error.IsOccurred())
            {
                LOG.ShowIGGException(error);
                return;
            }

            // 显示已签署的协议
            if (null != agreementSigningFile)
            {
                ShowAgreements(agreementSigningFile.GetAgreements());
                ShowSignedDialog(agreementSigningFile, agreementTerminationAlert);
                return;
            }
            
            LOG.Error($"请求已签署协议出错（网络错误或业务错误，业务错误可以查看code:{error.GetBaseErrorCode()}）的定义。"); 
        });
    }
    
    /// <summary>
    /// 显示已签署协议的对话框（用于终止协议提示）
    /// </summary>
    /// <param name="agreementSigningFile"></param>
    /// <param name="agreementTerminationAlert"></param>
    private void ShowSignedDialog(IGGAgreementSigningFile agreementSigningFile, IGGAgreementTerminationAlert agreementTerminationAlert)
    {
        // 显示已签署协议的对话框,主要关注UI文案的取值（用于终止协议提示）
        LOG.Error("\nLocalizedTitle=" + agreementSigningFile.LocalizedTitle + "\nLocalizedCaption=" + 
                  agreementSigningFile.LocalizedCaption + "\nLocalizedActionSign=" + agreementSigningFile.LocalizedActionSign);
        ShowDisagreeConfirmDialog(agreementTerminationAlert);
    }
    
    /// <summary>
    /// 显示终止协议二次确认框
    /// </summary>
    /// <param name="agreementTerminationAlert"></param>
    private void ShowDisagreeConfirmDialog(IGGAgreementTerminationAlert agreementTerminationAlert)
    {
        // 显示终止协议二次确认框,主要各文案显示的取值
    }
}

    
