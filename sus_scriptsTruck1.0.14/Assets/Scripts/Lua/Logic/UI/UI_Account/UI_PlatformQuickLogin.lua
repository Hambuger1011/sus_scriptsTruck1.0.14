local BaseClass = core.Class
local UIView = core.UIView
---@class UI_PlatformQuickLogin
local UI_PlatformQuickLogin = BaseClass("UI_PlatformQuickLogin", UIView)
local uiid = logic.uiid
local LoginType = {
    Facebook = "Facebook",
    IGG = "IGGAccount",
    Google = "Google",
    Guest = "Device",
    GameCenter = "GameCenter",
    Apple = "Apple",
}
local SignInType


UI_PlatformQuickLogin.config = {
    ID = uiid.PlatformQuickLogin,
    AssetName = 'UI/Resident/UI/Canvas_PlatformQuickLogin'
}

function UI_PlatformQuickLogin:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.closeButton = self.uiBinding:Get('Backe', typeof(logic.cs.Button))
    self.Device = self.uiBinding:Get('Device', typeof(logic.cs.UITweenButton))
    self.Facebook = self.uiBinding:Get('Facebook', typeof(logic.cs.UITweenButton))
    self.Google = self.uiBinding:Get('Google', typeof(logic.cs.UITweenButton))
    self.IGG = self.uiBinding:Get('IGG', typeof(logic.cs.UITweenButton))
    self.Apple = self.uiBinding:Get('Apple', typeof(logic.cs.UITweenButton))
    self.GameCenter = self.uiBinding:Get('GameCenter', typeof(logic.cs.UITweenButton))

    if logic.cs.GameHttpNet.SYSTEMTYPE == 1 then
        self.Apple.gameObject:SetActiveEx(false)
    else
        self.Apple.gameObject:SetActiveEx(true)
    end
    self.Google.gameObject:SetActiveEx(false)
    self.GameCenter.gameObject:SetActiveEx(false)
    

    self.Device.onClick:RemoveAllListeners()
    self.Device.transform:Find('Logged').gameObject:SetActiveEx(SignInType == LoginType.Guest)
    self.Device.transform:Find('ToLogin').gameObject:SetActiveEx(SignInType ~= LoginType.Guest)
    self.Device.onClick:AddListener(function()
        logic.cs.IGGSDKMrg:DeviceLogin()
    end)

    self.Facebook.onClick:RemoveAllListeners()
    self.Facebook.transform:Find('Logged').gameObject:SetActiveEx(SignInType == LoginType.Facebook)
    self.Facebook.transform:Find('ToLogin').gameObject:SetActiveEx(SignInType ~= LoginType.Facebook)
    self.Facebook.onClick:AddListener(function()
        logic.cs.IGGSDKMrg:FacebookLogin()
    end)

    self.Google.onClick:RemoveAllListeners()
    self.Google.transform:Find('Logged').gameObject:SetActiveEx(SignInType == LoginType.Google)
    self.Google.transform:Find('ToLogin').gameObject:SetActiveEx(SignInType ~= LoginType.Google)
    self.Google.onClick:AddListener(function()
        logic.cs.IGGSDKMrg:GoogleLogin()
    end)

    self.IGG.onClick:RemoveAllListeners()
    self.IGG.transform:Find('Logged').gameObject:SetActiveEx(SignInType == LoginType.IGG)
    self.IGG.transform:Find('ToLogin').gameObject:SetActiveEx(SignInType ~= LoginType.IGG)
    self.IGG.onClick:AddListener(function()
        logic.cs.IGGSDKMrg:IGGAccountLogin()
    end)

    self.Apple.onClick:RemoveAllListeners()
    self.Apple.transform:Find('Logged').gameObject:SetActiveEx(SignInType == LoginType.Apple)
    self.Apple.transform:Find('ToLogin').gameObject:SetActiveEx(SignInType ~= LoginType.Apple)
    self.Apple.onClick:AddListener(function()
        logic.cs.IGGSDKMrg:AppleLogin()
    end)

    self.GameCenter.onClick:RemoveAllListeners()
    self.GameCenter.transform:Find('Logged').gameObject:SetActiveEx(SignInType == LoginType.Apple)
    self.GameCenter.transform:Find('ToLogin').gameObject:SetActiveEx(SignInType ~= LoginType.Apple)
    self.GameCenter.onClick:AddListener(function()
        logic.cs.IGGSDKMrg:GameCenterLogin()
    end)
    
    self.closeButton.onClick:RemoveAllListeners()
    self.closeButton.onClick:AddListener(function()
        self:OnExitClick()
    end)
end

function UI_PlatformQuickLogin:SetSignInType(signInType)
    SignInType = signInType
end

function UI_PlatformQuickLogin:OnOpen()
    UIView.OnOpen(self)
end

function UI_PlatformQuickLogin:OnClose()
    UIView.OnClose(self)
end

function UI_PlatformQuickLogin:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UI_PlatformQuickLogin