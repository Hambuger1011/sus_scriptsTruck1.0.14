using IGG.SDK.Modules.Compliance.VO;
using IGG.SDK.Modules.RealNameVerification.VO;

/// <summary>
/// 缓存防沉迷信息，游戏那边可以用自己的方案实现缓存（因为防沉迷信息获取之后，在后续的支付会用到，所以需要缓存起来）。
/// </summary>
public class IGGRealnameVerificationConfig
{
    public const string TAG = "IGGRealnameVerificationConfig";

    private static IGGRealnameVerificationConfig instance;
    
    /// <summary>
    /// 合规的状态
    /// </summary>
    private IGGComplianceStatus status;

    /// <summary>
    ///  限制数据（游戏时段、时长、购买限制）
    /// </summary>
    private IGGComplianceRestrictions restrictions;

    private IGGRealnameVerificationConfig() {

    }

    public static IGGRealnameVerificationConfig SharedInstance() {
        if (instance == null) {
            instance = new IGGRealnameVerificationConfig();
        }

        return instance;
    }

    /// <summary>
    /// 实名认证是否开启
    /// </summary>
    /// <returns></returns>
    public bool IsEnableRealnameVerification() {
        if (null == status) {
            return false;
        }
        return status.IsRealNameVerificationEnable;
    }

    /// <summary>
    /// 未成年防沉迷是否开启
    /// </summary>
    /// <returns></returns>
    public bool IsEnableAntiAddiction() {
        if (null == status) {
            return false;
        }
        return status.IsMinorsRestrictEnable;
    }

    /// <summary>
    /// 访客防沉迷是否开启
    /// </summary>
    /// <returns></returns>
    public bool IsEnableGuestRestrict() {
        if (null == status) {
            return false;
        }
        return status.IsGuestRestrictEnable;
    }
    
    public IGGRealNameVerificationState GetRealNameVerificationState() {
        if (null == status || null == status.RealNameVerificationResult) {
            return IGGRealNameVerificationState.Unknow;
        }
        return status.RealNameVerificationResult.State;
    }

    public IGGComplianceStatus GetComplianceStatus() {
        return status;
    }
    
    /// <summary>
    /// 是否是未成年人
    /// </summary>
    /// <returns></returns>
    public bool IsMinor() {
        if (null == status || null == status.RealNameVerificationResult) {
            return false;
        }
        return status.RealNameVerificationResult.IsMinor;
    }


    public IGGComplianceRestrictions GetRestrictions() {
        return restrictions;
    }

    public void SetRestrictions(IGGComplianceRestrictions restrictions) {
        this.restrictions = restrictions;
    }

    public void SetPostponingData(IGGComplianceStatus status) {
        this.status = status;
        SetRestrictions(null);
    }

    public void SetAdultData(IGGComplianceStatus status) {
        this.status = status;
        SetRestrictions(null);
    }

    public void SetMinorData(IGGComplianceStatus status, IGGComplianceRestrictions restrictions) {
        this.status = status;
        SetRestrictions(restrictions);
    }

    public void SetGuestData(IGGComplianceStatus status, IGGComplianceRestrictions restrictions) {
        this.status = status;
        SetRestrictions(restrictions);
    }
}