--[[
-- 启动场景
--]]

local Class = core.Class
local UserInfoGetter = require("Logic/VersionUpdate/UserInfoGetter")

local BaseScene = logic.BaseScene
local LoadingScene = Class("LoadingScene", BaseScene)
local base = BaseScene

function LoadingScene:__init()
end

-- 创建：准备预加载资源
function LoadingScene:OnEnter()
    base.OnEnter(self)

    --VersionUpdater = VersionUpdater.New()
    UserInfoGetter = UserInfoGetter.New()

    logic.debug.LogError("<color=green>enter loading state</color>")

    logic.cs.UserDataManager:GetLoginInfoByLocal()
    logic.cs.UserDataManager:GetVersion()
    logic.cs.GameDataMgr.BaseResLoadFinish = false
    --logic.UIMgr:Open(logic.uiid.Loading)
    local curTime = os.date("%Y-%m-%d %H:%M:%S");--当前时间
    CS.XLuaHelper.DebugLog("lua LoadingScene 准备预加载资源："..curTime);
    self:StartLoadResource()
end

-- 离开场景
function LoadingScene:OnLeave()
    base.OnLeave(self)

    --VersionUpdater:Delete()
    --VersionUpdater = nil

    UserInfoGetter:Delete()
    UserInfoGetter = nil
end

function LoadingScene.StartLoading2()

end



function LoadingScene:StartLoadResource()



    logic.debug.Log("2、开始预加载资源")

    if(CS.XLuaHelper.isHotUpdate)then
        local uiform = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.UIUpdateModule)

        if(IsNull(uiform)==false)then
            local mUIUpdateModule = uiform:GetComponent(typeof(CS.UIUpdateModule))
            mUIUpdateModule:StartLoading2(function(isOK)
                self:OnLoadFinish(isOK)
            end)
        end

    else
        logic.UIMgr:Open(logic.uiid.Loading)
        local UIGuide = require("Logic/UI/UI_Guide/UIGuide")
        UIGuide:GetRecommandBook(
                function()
                    logic.cs.ABSystem.ui.BannerLoadOk = true
                end
        )
        local uiLoading = logic.UIMgr:GetView(logic.uiid.Loading)
        uiLoading.SetLoadingContInfo("Loading game resources...")
        uiLoading.SetProgress(0, true);

        local curTime = os.date("%Y-%m-%d %H:%M:%S");--当前时间
        CS.XLuaHelper.DebugLog("lua LoadingScene 开始加载资源："..curTime);

        uiLoading.StartLoading(function(isOK)
            self:OnLoadFinish(isOK)
        end)
    end

end

function LoadingScene:OnLoadFinish(isOK)
    if not isOK then
        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.LoadResResultFail)
        return
    end
    local curTime = os.date("%Y-%m-%d %H:%M:%S");--当前时间
    CS.XLuaHelper.DebugLog("lua LoadingScene 加载资源完成："..curTime);

    logic.debug.Log("3、加载资源完成")
    logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.LoadResResultSucc)
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.LoadResourceOk)
    logic.cs.GameDataMgr.BaseResLoadFinish = true
    
    if core.config.isDebugMode then
        CS.Tiinoo.DeviceConsole.DeviceConsoleLoader:Load()
        if logic.cs.GameMain.isShowDebugPanel then
            logic.cs.CUIManager:OpenForm(logic.cs.CUIID.Canvas_Debug, true, false)
        end
    end
    
    logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.PopupTipsForm);
    logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.Global);
    self:LoadResFinishAndGetInfo()
end

function LoadingScene:LoadResFinishAndGetInfo()
    if(logic.cs.UserDataManager.IGGid~="" and logic.cs.UserDataManager.Accesskey~="")then
        self:Getserverdata(logic.cs.UserDataManager.IGGid,logic.cs.UserDataManager.Accesskey);
    else
        local errorText = CS.CTextManager.Instance:GetText(386)
        if logic.cs.IGGSDKMrg.isSessionExpired then
            logic.cs.IGGSDKMrg.isSessionExpired = false
            errorText = CS.CTextManager.Instance:GetText(327)
        end
        logic.cs.UIAlertMgr:Show("TIPS", errorText,logic.cs.AlertType.Sure,
                function(isOK)
                    local LoginForm=logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.LoginForm)
                    local go=LoginForm.transform:GetComponent(typeof(CS.LoginForm))
                    go:IsTimeOutOpenFanc()
                end)
    end
end
function LoadingScene:Getserverdata(IGGId,accessToken)

    logic.cs.IGGAgreementMrg:Init()
    if logic.config.useServerData then
        local curTime = os.date("%Y-%m-%d %H:%M:%S");--当前时间
        CS.XLuaHelper.DebugLog("lua LoadingScene 5、获取服务端数据："..curTime);

        logic.debug.Log("5、获取服务端数据")
        --logic.cs.UINetLoadingMgr:Show()

        if(CS.XLuaHelper.isHotUpdate)then
        else
            local uiLoading = logic.UIMgr:GetView(logic.uiid.Loading)
            uiLoading.SetLoadingContInfo("Retrieving user information...")
        end

        if not IsNull(logic.cs.SdkMgr.jpushSDK) then
            logic.cs.SdkMgr.jpushSDK:GetPushId()
        end

        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.GetUserInfoStart)
        UserInfoGetter:Run(IGGId,accessToken,function()   --获取用户信息
            self:DoEnter()
        end)
    else
        logic.debug.Log("5、直接进入游戏")

        local curTime = os.date("%Y-%m-%d %H:%M:%S");--当前时间
        CS.XLuaHelper.DebugLog("lua LoadingScene 5、直接进入游戏："..curTime);

        if logic.cs.UserDataManager.UserData.IsSelectFirstBook == 0 then
            logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.GuideForm)

            if(CS.XLuaHelper.isHotUpdate)then
                logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.UIUpdateModule)
            else
                logic.UIMgr:Close(logic.uiid.Loading)
            end
        else

            logic.UIMgr:Open(logic.uiid.UIMainForm)

            if(CS.XLuaHelper.isHotUpdate)then
                logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.UIUpdateModule)
            else
                logic.UIMgr:Close(logic.uiid.Loading)
            end
        end
    end

end



function LoadingScene:DoEnter()
    if self.isEnter then
        return
    end
    self.isEnter = true

    --【初始化评星系统】
    CS.XLuaHelper.initAppRating();

    --【AF事件 首页 首次记录获取】
    CS.AppsFlyerManager.Instance:GetFirstActionLog();

    --logic.cs.UINetLoadingMgr:Close();
    logic.cs.AudioManager = CS.AudioManager.Instance;

    local curTime = os.date("%Y-%m-%d %H:%M:%S");--当前时间
    CS.XLuaHelper.DebugLog("lua LoadingScene 6、进入游戏："..curTime);

    logic.debug.Log("6、进入游戏")
    logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.EnterGame)
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.EnterGameOnyx)
    if logic.cs.UserDataManager.userInfo.data.userinfo.firstplay == 0 then
        logic.cs.IGGSDKMrg.isNewUser = true
        logic.cs.talkingdata:OpenApp("OpenGuideForm")
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NewerStart)
        logic.UIMgr:Open(logic.uiid.Guide)
    else
        logic.cs.IGGSDKMrg.isNewUser = false
        logic.cs.talkingdata:OpenApp("OpenMainForm")
        logic.UIMgr:Open(logic.uiid.UIMainForm);
        if(CS.XLuaHelper.isHotUpdate)then
            logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.UIUpdateModule)
        else
            logic.UIMgr:Close(logic.uiid.Loading)
            self:isShowSignIn()
        end
        --【AF事件 请求服务器 记录第一次数据】
        CS.AppsFlyerManager.Instance:GetFirstActionLog();
    end

    --【初始化广告位】
    CS.GoogleAdmobAds.Instance:InitRewardedAd();

    logic.cs.PurchaseRecordManager:CheckRecordByLocal()
    logic.DataManager:DoEnter()
end

--是否自动签到
function LoadingScene:isShowSignIn()

    if(Cache.SignInCache.activity_login.is_receive==0)then
        logic.UIMgr:Open(logic.uiid.UISignTipForm);
    end

end



return LoadingScene