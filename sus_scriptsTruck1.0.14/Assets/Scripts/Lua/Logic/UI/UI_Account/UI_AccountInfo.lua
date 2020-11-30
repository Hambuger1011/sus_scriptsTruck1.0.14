local BaseClass = core.Class
local UIView = core.UIView
---@class UI_AccountInfo
local UI_AccountInfo = BaseClass("UI_AccountInfo", UIView)
local UI_PlatformQuickLogin = require('Logic/UI/UI_Account/UI_PlatformQuickLogin')
local uiid = logic.uiid
local LoginType = {
    Facebook = 1,
    IGG = 2,
    Google = 3,
    Guest = 4,
    GameCenter = 5,
    Apple = 6,
}

UI_AccountInfo.config = {
    ID = uiid.AccountInfo,
    AssetName = 'UI/Resident/UI/Canvas_AccountInfo'
}

function UI_AccountInfo:SetUI()
    if logic.cs.GameHttpNet.SYSTEMTYPE == 1 then
        self.AppleBind.gameObject:SetActiveEx(false)
    else
        self.AppleBind.gameObject:SetActiveEx(true)
    end
    self.GoogleBind.gameObject:SetActiveEx(false)
    self.GameCenterBind.gameObject:SetActiveEx(false)

    local UserInfo = GameHelper.DicToLuaTable(logic.cs.IGGSDKMrg.UserInfo)
    logic.debug.PrintTable(UserInfo,"UserInfo")
    self:UpdataBind(self.FacebookBind,tonumber(UserInfo.FBIsBind) == 1,UserInfo.FBBindInfo,LoginType.Facebook)
    self:UpdataBind(self.DeviceBind,true,UserInfo.GuestBindInfo,LoginType.Guest)
    self:UpdataBind(self.GoogleBind,tonumber(UserInfo.GoogleIsBind) == 1,UserInfo.GoogleBindInfo,LoginType.Google)
    self:UpdataBind(self.IGGBind,tonumber(UserInfo.IGGIsBind) == 1,UserInfo.IGGBindInfo,LoginType.IGG)
    self:UpdataBind(self.AppleBind,tonumber(UserInfo.AppleIsBind) == 1,UserInfo.AppleBindInfo,LoginType.Apple)
    self:UpdataBind(self.GameCenterBind,tonumber(UserInfo.GameCenterIsBind) == 1,UserInfo.GameCenterBindInfo,LoginType.GameCenter)
    self.Land.text = UserInfo.LoginType
    if(UserInfo.LoginType=="IGGAccount")then
        self.Land.text = "IGG Account";
    end

    self.IDText.text = "IGG ID:"..UserInfo.IGGID

    --显示头像      --加载头像框
    self:ShowAvatar()

    self.NameText.text = logic.cs.UserDataManager.userInfo.data.userinfo.nickname

    if UserInfo.isAccountSafe and tonumber(UserInfo.isAccountSafe) ~= 0 then
        self.Safety:SetActiveEx(false)
        self.Tips:SetActiveEx(false)
    end

    self.SwitchButton.onClick:RemoveAllListeners()
    self.SwitchButton.onClick:AddListener(function()
        UI_PlatformQuickLogin:SetSignInType(UserInfo.LoginType)
        logic.UIMgr:Open(logic.uiid.PlatformQuickLogin)
    end)
    self.closeButton.onClick:RemoveAllListeners()
    self.closeButton.onClick:AddListener(function()
        self:OnExitClick()
    end)
end

function UI_AccountInfo:ShowAvatar()
    --显示头像
    GameHelper.luaShowDressUpForm(-1,self.HeadIcon,DressUp.Avatar,1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(-1,self.HeadFrame,DressUp.AvatarFrame,2001);
end



function UI_AccountInfo:UpdataBind(trans, isBind , bindMsg , type)
    local Info = trans:Find('BindInfo'):GetComponent(typeof(logic.cs.Text))
    local Button = trans:Find('Button'):GetComponent(typeof(logic.cs.Button))
    
    Info.gameObject:SetActiveEx(isBind)
    Button.gameObject:SetActiveEx(not isBind)
    
    if isBind then
        if type == LoginType.Guest then
            if bindMsg and #bindMsg > 0 then
                Info.text = CS.CTextManager.Instance:GetText(tonumber(bindMsg))
            else
                Info.text = CS.CTextManager.Instance:GetText(428)
            end
        else
            if bindMsg and #bindMsg > 0 then
                Info.text = bindMsg
            else
                Info.text = CS.CTextManager.Instance:GetText(439)
            end
        end
    else
        Button.onClick:RemoveAllListeners()
        Button.onClick:AddListener(function()
            logic.gameHttp:CheckAccessToken(function(result)
                local json = core.json.Derialize(result)
                local code = tonumber(json.code)
                if code == 200 then
                    logic.cs.IGGSDKMrg.bindCallBack = function()
                        self:SetUI();
                    end
                    if type == LoginType.Google then
                        logic.cs.IGGSDKMrg:BindGooglePlay()
                    elseif type == LoginType.IGG then
                        logic.cs.IGGSDKMrg:BindIGGAccount()
                    elseif type == LoginType.Facebook then
                        logic.cs.IGGSDKMrg:BindFacebook()
                    elseif type == LoginType.GameCenter then
                        logic.cs.IGGSDKMrg:BindGameCenter()
                    elseif type == LoginType.Apple then
                        logic.cs.IGGSDKMrg:BindApple()
                    end
                end
            end)
        end)
    end
end

function UI_AccountInfo:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.closeButton = self.uiBinding:Get('CloseButton', typeof(logic.cs.Button))
    self.HeadIcon = self.uiBinding:Get('HeadIcon', typeof(logic.cs.Image))
    self.HeadFrame = self.uiBinding:Get('HeadFrame', typeof(logic.cs.Image))
    self.IDText = self.uiBinding:Get('IDText', typeof(logic.cs.Text))
    self.NameText = self.uiBinding:Get('NameText', typeof(logic.cs.Text))
    self.Land = self.uiBinding:Get('Land', typeof(logic.cs.Text))
    self.Safety = self.uiBinding:Get('Safety').gameObject
    self.Tips = self.uiBinding:Get('Tips').gameObject
    self.DeviceBind = self.uiBinding:Get('DeviceBind').transform
    self.FacebookBind = self.uiBinding:Get('FacebookBind').transform
    self.IGGBind = self.uiBinding:Get('IGGBind').transform
    self.GoogleBind = self.uiBinding:Get('GoogleBind').transform
    self.AppleBind = self.uiBinding:Get('AppleBind').transform
    self.GameCenterBind = self.uiBinding:Get('GameCenterBind').transform
    self.SwitchButton = self.uiBinding:Get('SwitchButton',typeof(logic.cs.UITweenButton))

    self:SetUI()
end

function UI_AccountInfo:OnOpen()
end

function UI_AccountInfo:OnOpen()
    UIView.OnOpen(self)
end

function UI_AccountInfo:OnClose()
    UIView.OnClose(self)
end

function UI_AccountInfo:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UI_AccountInfo