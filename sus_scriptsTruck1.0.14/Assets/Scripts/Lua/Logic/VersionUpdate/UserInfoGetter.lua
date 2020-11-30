--[[
    书本推荐逻辑
]]
local class = core.Class
local UserInfoGetter = class("UserInfoGetter")
local GameUtility = CS.GameUtility

function UserInfoGetter:__init()
end

function UserInfoGetter:__delete()
end

function UserInfoGetter:Run(IGGId,accessToken,doEnter)
    self.doEnter = doEnter
    self:GetUserInfoRun(IGGId,accessToken)
end

function UserInfoGetter:GetUserInfoRun(IGGId,accessToken)
    logic.gameHttp:AddFunList(function() 
        self:GetUserInfoRun(IGGId,accessToken) 
    end)

    --logic.cs.UINetLoadingMgr:Show()
    logic.gameHttp:Login(IGGId,accessToken,0,function(result)
        self:OnLoginCallBack(result)
    end)
end

function UserInfoGetter:OnLoginCallBack(result)
    logic.debug.Log("----Login---->" .. result)

    if string.IsNullOrEmpty(result) then
        logic.debug.LogError('Login Faild')
        return
    end
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if code == 200 then
        logic.cs.GameHttpNet.TOKEN = json.data.token
        logic.gameHttp:GetUserInfo(function(result)
            self:OnGetUserInfoCallBack(result)
        end)
    else
        logic.cs.UIAlertMgr:Show("TIPS", json.msg)
        --Tail--todo:服务器验证accessToken失败后重新登录验证
    end
end

function UserInfoGetter:OnGetUserInfoCallBack(result)
    logic.debug.Log("----GetUserInfo---->" .. result)


    if string.IsNullOrEmpty(result) then
        logic.debug.LogError('GetUserInfo Faild')
        return
    end
    -- if responseCode ~= 200 then
    --     logic.cs.talkingdata:OpenApp("UserInfoProtocolReturn - " .. result)
    --     return
    -- end
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    --local needUpdate = tonumber(json.data.update)
    --if needUpdate and needUpdate == 1 then
    --    local version = GameUtility.version
    --    if tostring(version) ~= "2.0.4" then
    --         logic.cs.UIAlertMgr:Show(
    --             "TIPS", 
    --             "Dear readers,\n Please click \"OK\" to download the new version manually because it is still under review in Google Play", 
    --             logic.cs.AlertType.Sure,
    --             function(isOK)
    --                 if not isOK then
    --                     return
    --                 end
    --                 logic.debug.LogWarning("需要更新，当前版本version=="..tostring(version))
    --                 logic.cs.Application.OpenURL("http://new.onyxgames1.com/SecretsDownload.html");
    --             end
    --         )
    --    else
    --        logic.debug.LogWarning("不需要更新，当前版本version=="..tostring(version))
    --    end
    --end
    logic.cs.talkingdata:OpenApp("UserInfoProtocolReturn - ".. json.code)
    if code == 200 then

        --埋点数据
        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.GetUserInfoResultSucc)
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.LoadUserInfo)
        
        logic.cs.GameDataMgr.DoLoginGames = true
        --c#缓存用户数据
        logic.cs.UserDataManager:SetUserInfo(result)

        --缓存首次签到数据
        Cache.SignInCache.activity_login=json.data.activity_login;

        --首次登陆  缓存【头像】数据
        Cache.DressUpCache.avatar=tonumber(json.data.user_info.avatar);
        --首次登陆  缓存【头像框】数据
        Cache.DressUpCache.avatar_frame=tonumber(json.data.user_info.avatar_frame);
        --首次登陆  缓存【评论框】数据
        Cache.DressUpCache.comment_frame=tonumber(json.data.user_info.comment_frame);
        --首次登陆  缓存【弹幕框】数据
        Cache.DressUpCache.barrage_frame=tonumber(json.data.user_info.barrage_frame);

        --【初始化评星系统】
        CS.XLuaHelper.initAppRating();
        if logic.cs.UserDataManager.isMoveCode then
            logic.cs.GameHttpNet:ShowMovePanel()
            return;
        end

        if(CS.XLuaHelper.isHotUpdate)then
        else
            local uiLoading = logic.UIMgr:GetView(logic.uiid.Loading)
            if uiLoading then
                uiLoading.SetLoadingContInfo("Retrieving game information...")
            end
        end

        if logic.config.channel == Channel.Huawei then
            logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.UIUpdateModule)
            logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.LoadingForm)
            logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.HwLoginForm)
            return
        end
        
        
        --logic.gameHttp:GetSelfBookInfo(function(result) self:OnToLoadSelfBookInfo(result) end)   --获取书本信息

        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.GetMallStart)
        logic.gameHttp:GetShopList(function(result) self:OnGetShopListCallBack(result) end)
       
        logic.DataManager.server:GetDisjunctor(function()
            --DoEnter
            self.doEnter()
        end)

    elseif code == 277 then
        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.GetUserInfoResultFail)
        logic.cs.UIAlertMgr:Show("TIPS", json.msg)
    elseif code == 209 then
        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.GetUserInfoResultFail)
        logic.debug.LogError("登录失败")
        logic.cs.UITipsMgr:PopupTips("Your information is out of date. Please, log in again.", false,0);
        if logic.config.channel == Channel.Huawei then
            logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.LoadingForm)
            logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.HwLoginForm)
        else
            logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.LoadingForm)
            logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.LoginForm)
        end
    end
end


function UserInfoGetter:OnToLoadSelfBookInfo(result)
    logic.debug.Log("----ToLoadSelfBookInfo---->" .. result)
    -- if responseCode ~= 200 then
    --     logic.cs.talkingdata:OpenApp("SelfBookInfoProtocolReturn - " .. result);
    --     return
    -- end
    local json = core.json.Derialize(result)
    logic.cs.talkingdata:OpenApp("SelfBookInfoProtocolReturn - " .. json.code)
    
    local code = tonumber(json.code)
    if code == 200 then
        logic.cs.UserDataManager:SetSelfBookInfo(result)
        logic.cs.UserDataManager:InitRecordServerBookData()
        logic.DataManager.server:GetDisjunctor(function()
            --DoEnter
            self.doEnter()
        end)
    elseif code == 277 then
        logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end

function UserInfoGetter:OnGetShopListCallBack(result)
    if string.IsNullOrEmpty(result) then
        logic.debug.LogError("----GetShopListCallBack---->返回结果为空，什么鬼？")
        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.GetMallResultFail)
        return
    end
    logic.debug.Log("----GetShopListCallBack---->" .. result)
    
    local json = core.json.Derialize(result)
    
    local code = tonumber(json.code)
    if code == 200 then
        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.GetMallResultSucc)
        logic.cs.UserDataManager:SetShopList(result);
        --logic.gameHttp:GetBookShelfInfo(function(result) self:OnBookShelfInfoHandler(result) end);
        logic.cs.PurchaseManager:ToRequestProductList();
    else
        logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.GetMallResultFail)
    end
end

-- function UserInfoGetter:OnBookShelfInfoHandler(result)
--     logic.debug.Log("----BookShelfInfoHandler---->" .. result)
--     logic.cs.UserDataManager:SetBookSelfInfo(result)
-- end


return UserInfoGetter