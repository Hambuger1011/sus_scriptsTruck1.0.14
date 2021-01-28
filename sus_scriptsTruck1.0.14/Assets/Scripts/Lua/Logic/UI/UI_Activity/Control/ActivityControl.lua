local BaseClass = core.Class
local ActivityControl = BaseClass("ActivityControl", core.Singleton)

local UIActivityForm=nil;

-- 构造函数
function ActivityControl:__init()

end

function ActivityControl:SetData(uiactivity)
    UIActivityForm=uiactivity;
end



--region 【请求获取在线阅读任务状态】---【常规活动】【在线阅读】
function ActivityControl:ReadingTaskStatusRequest()
logic.gameHttp:GetReadingTaskStatus(function(result) self:GetReadingTaskStatus(result); end)
end
--endregion


--region 【获取在线阅读任务状态响应】---【常规活动】【在线阅读】
function ActivityControl:GetReadingTaskStatus(result)
    logic.debug.Log("----GetReadingTaskStatus---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.ReadTimeCache:UpdateData(json.data);
        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            UIActivityForm:UpdateReadTimePanel();
        end

    elseif(code == 202)then
        --所有奖品已经领完 锁死
        Cache.ReadTimeCache.receive_level=5;
        Cache.ReadTimeCache.isEND=true;
        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            UIActivityForm:UpdateReadTimePanel();
        end
    else
        logic.debug.LogError("----GetReadingTaskStatus----> ERROR:"..json.msg);
        --logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion


--region 【请求更新在线阅读时长】---【常规活动】【在线阅读】
function ActivityControl:ReadingTaskTimeRequest(AddTime)
    --请求列表 【获取搜索分类的书本】
    logic.gameHttp:updateReadingTaskTime(AddTime,function(result) self:UpdateReadingTaskTime(result); end)
end

function ActivityControl:ReadingTaskTimeRequest2(AddTime)
    --请求列表 【获取搜索分类的书本】
    logic.gameHttp:updateReadingTaskTime(AddTime,function(result) self:UpdateReadingTaskTime2(result); end)
end

--endregion


--region 【获取更新在线阅读时长响应】---【常规活动】【在线阅读】

function ActivityControl:UpdateReadingTaskTime(result)
    logic.debug.Log("----updateReadingTaskTime---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.ReadTimeCache:UpdateData(json.data);

        if(logic.cs.MyBooksDisINSTANCE:GetIsPlaying()==true)then
            --在线阅读时长计算   再次计算
            GameHelper.OnlineReadingtime()
        end
    elseif(code == 202)then
        --所有奖品已经领完 锁死
        Cache.ReadTimeCache.receive_level=5;
        Cache.ReadTimeCache.isEND=true;
        --销毁计时
        GameHelper.CloseReadTimer();
    else
        logic.debug.LogError("----updateReadingTaskTime----> ERROR:"..json.msg);
       -- logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end

function ActivityControl:UpdateReadingTaskTime2(result)
    logic.debug.Log("----updateReadingTaskTime---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.ReadTimeCache:UpdateData(json.data);
    elseif(code == 202)then
        --所有奖品已经领完 锁死
        Cache.ReadTimeCache.finish_level=5;
        Cache.ReadTimeCache.receive_level=5;
        Cache.ReadTimeCache.isEND=true;
        --销毁计时
        GameHelper.CloseReadTimer();

        --刷新红点状态
        GameController.MainFormControl:RedPointRequest();
    else
        logic.debug.LogError("----updateReadingTaskTime----> ERROR:"..json.msg);
        --logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end



--endregion


--region 【领取在线阅读任务奖励】---【常规活动】【在线阅读】
function ActivityControl:ReadingTaskPrizeRequest()
    logic.gameHttp:ReceiveReadingTaskPrize(function(result) self:ReceiveReadingTaskPrize(result); end)
end
--endregion


--region 【领取在线阅读任务奖励】---【常规活动】【在线阅读】
function ActivityControl:ReceiveReadingTaskPrize(result)
    logic.debug.Log("----ReceiveReadingTaskPrize---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.ReadTimeCache.receive_level=Cache.ReadTimeCache.finish_level;
        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            UIActivityForm:UpdateReadTimePanel();
        end

        --刷新红点状态
        GameController.MainFormControl:RedPointRequest();
        --刷新自己的钱
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

        --埋点*点击累计阅读奖励领取
        logic.cs.GamePointManager:BuriedPoint("activity_click_cumulative_read_reward");

        --是否已经评星过     0：否   1：是
        if(logic.cs.UserDataManager.userInfo.data.userinfo.is_store_score==0)then
            --打开评星
            GameHelper.OpenRating();
        end

    else
        logic.debug.LogError("----ReceiveReadingTaskPrize----> ERROR:"..json.msg);
        --logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion


--region【获取登录活动奖励内容】---【常规活动】【签到】
function ActivityControl:GetActivityRewardContentRequest()
    logic.gameHttp:GetActivityRewardContent(function(result) self:GetActivityRewardContent(result); end)
end
--endregion


--region【获取登录活动奖励内容*响应】---【常规活动】【签到】
function ActivityControl:GetActivityRewardContent(result)
    --【获取登录活动领取状态】
    logic.gameHttp:GetActivityReceiveStatus(function(result) self:GetActivityReceiveStatus(result); end)

    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----GetActivityRewardContent---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        Cache.ActivityCache:UpdatedSignList(json.data);
    end
end
--endregion


--region【获取登录活动领取状态*响应】---【常规活动】【活动状态】
function ActivityControl:GetActivityReceiveStatus(result)
    if string.IsNullOrEmpty(result) then
        return
    end
    logic.debug.Log("----GetActivityReceiveStatus---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if code == 200 then
        Cache.ActivityCache:UpdatedActivityState(json.data);
        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            --【刷新常规活动 签到面板】
            UIActivityForm:UpdateSignInPanel();


            --【刷新常规活动 刷新看广告】
            UIActivityForm:UpdateFreeRewardPanel();
        end
    end
end
--endregion


--region【领取每日登录奖励】---【常规活动】【签到领取奖励】
function ActivityControl:ReceiveDailyLoginAwardRequest()
    logic.gameHttp:ReceiveDailyLoginAward(function(result) self:ReceiveDailyLoginAward(result); end)
end
--endregion


--region【领取每日登录奖励*响应】---【常规活动】【签到领取奖励】
function ActivityControl:ReceiveDailyLoginAward(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----ReceiveDailyLoginAward---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            --签到领取奖励
            UIActivityForm:SignInReceiveReward();
        end

    end
end
--endregion


--region【获取任务列表】---【常规活动】【每日任务】
function ActivityControl:GetMyTaskListRequest()
    logic.gameHttp:GetMyTaskList(function(result) self:GetMyTaskList(result); end)
end
--endregion


--region【获取任务列表*响应】---【常规活动】【每日任务】
function ActivityControl:GetMyTaskList(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----GetMyTaskList---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        --【缓存每日任务数据】
        Cache.ActivityCache:UpdatedDailyTaskList(json.data);

        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            --刷新每日任务界面
            UIActivityForm:UpdateDailyTasksPanel();
        end
    end
end
--endregion


--region【领取任务奖励】---【常规活动】【每日任务领取奖励】
function ActivityControl:ReceiveTaskPrizeRequest(taskid)
    logic.gameHttp:ReceiveTaskPrize(taskid,function(result) self:ReceiveTaskPrize(result,taskid); end)
end
--endregion


--region【领取任务奖励*响应】---【常规活动】【每日任务领取奖励】
function ActivityControl:ReceiveTaskPrize(result,taskid)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----ReceiveTaskPrize---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

        --local newData = data
        --newData.status = 2
        --go.gameObject:SetActiveEx(false)
        --AddTaskItem(newData)

        --【红点请求】
        GameController.MainFormControl:RedPointRequest();
        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            --修改缓存状态
           Cache.ActivityCache:SetTaskStatus(taskid);
            UIActivityForm:UpdateDailyTasksPanel();
        end
    end
end
--endregion


--region【领取活动广告奖励】---【常规活动】【看广告】
function ActivityControl:ReceiveDailyAdAwardRequest()
    logic.gameHttp:ReceiveDailyAdAward(function(result) self:ReceiveDailyAdAward(result); end)
end
--endregion


--region【领取活动广告奖励*响应】---【常规活动】【看广告】
function ActivityControl:ReceiveDailyAdAward(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----ReceiveDailyAdAward---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey));
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond));
        if(UIActivityForm)then
            --领取活动广告奖励
            UIActivityForm:ReceiveDailyAdAward();

            --【刷新常规活动 刷新看广告】
            UIActivityForm:UpdateFreeRewardPanel();

            --刷新每日任务界面 请求任务列表
            self:GetMyTaskListRequest();
        end
    end
end
--endregion


--region【UIActivityForm设置ScrollRect】---【常规活动】
function ActivityControl:SetVerticalNormalizedPosition()
    --如果在活动页面 刷新数据
    if(UIActivityForm)then
        --签到领取奖励
        UIActivityForm:SetVerticalNormalizedPosition();
    end
end
--endregion


--region【领取用户迁移的奖励】---【限时活动】【账号迁移奖励】
function ActivityControl:ReceiveUserMoveAwardRequest()
    logic.gameHttp:ReceiveUserMoveAward(function(result) self:ReceiveUserMoveAward(result); end)
end
--endregion


--region【领取首充奖励】---【限时活动】【账号迁移奖励】
function ActivityControl:ReceiveFirstRechargeAwardRequest()
    logic.gameHttp:ReceiveFirstRechargeAward(function(result) self:ReceiveFirstRechargeAward(result); end)
end
--endregion


--region【领取用户迁移的奖励*响应】---【限时活动】【限时活动*账号迁移奖励】
function ActivityControl:ReceiveFirstRechargeAward(result)
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if code == 200 then
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

        --刷新红点状态
        GameController.MainFormControl:RedPointRequest();

        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            UIActivityForm:ReceiveFirstRechargeAward_Response();
        end
    end
end
--endregion


--region【领取用户迁移的奖励*响应】---【限时活动】【限时活动*账号迁移奖励】
function ActivityControl:ReceiveUserMoveAward(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----ReceiveUserMoveAward---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        logic.cs.UserDataManager.userInfo.data.userinfo.se_move_finish = 2;
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey));
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond));

        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            --关闭账号迁移奖励面板
            UIActivityForm:ReceiveUserMoveAward_Response();
        end
    end
end
--endregion


--region【领取第三方登录绑定的奖励】---【限时活动】【账号绑定奖励】
function ActivityControl:ReceiveThirdPartyAwardRequest()
    logic.gameHttp:ReceiveThirdPartyAward(function(result) self:ReceiveThirdPartyAward(result); end)
end
--endregion


--region【领取第三方登录绑定的奖励*响应】---【限时活动】【账号绑定奖励】
function ActivityControl:ReceiveThirdPartyAward(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----ReceiveThirdPartyAward---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        logic.cs.UserDataManager.userInfo.data.userinfo.third_party_award = 1;
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey));
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond));

        --刷新红点状态
        GameController.MainFormControl:RedPointRequest();

        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            UIActivityForm:ReceiveThirdPartyAward_Response();
        end
    end
end
--endregion


--region【领取关注社媒奖励】---【限时活动】【关注社媒奖励】
function ActivityControl:ReceiveAttentionMediaRewardRequest()
    logic.gameHttp:ReceiveAttentionMediaReward(function(result) self:ReceiveAttentionMediaReward(result); end)
end
--endregion


--region【领取关注社媒奖励*响应】---【限时活动】【关注社媒奖励】
function ActivityControl:ReceiveAttentionMediaReward(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----ReceiveAttentionMediaReward---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        logic.cs.UserDataManager.userInfo.data.userinfo.attention_media_award = 1;
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

        --刷新红点状态
        GameController.MainFormControl:RedPointRequest();

        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            UIActivityForm:ReceiveAttentionMediaReward_Response();
        end

    end
end
--endregion


--region【获取通用奖励配置】---【限时活动】【通用奖励】
function ActivityControl:GetRewardConfigRequest()
    logic.gameHttp:GetRewardConfig(function(result) self:GetRewardConfig(result); end)
end
--endregion


--region【获取通用奖励配置*响应】---【限时活动】【通用奖励】
function ActivityControl:GetRewardConfig(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----GetRewardConfig---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then

        --缓存奖励数据
        Cache.ActivityCache:UpdatedRewardConfig(json.data);

        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            UIActivityForm:GetRewardConfig_Response();
        end
    end
end
--endregion


--region【更新社媒状态奖励为可领取】---【限时活动】【关注社媒】
function ActivityControl:UpdateAttentionMediaRequest(url)
    logic.gameHttp:UpdateAttentionMedia(function(result) self:UpdateAttentionMedia(result,url); end)
end
--endregion


--region【更新社媒状态奖励为可领取*响应】---【限时活动】【关注社媒】
function ActivityControl:UpdateAttentionMedia(result,url)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----UpdateAttentionMedia---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        logic.cs.UserDataManager.userInfo.data.userinfo.attention_media_award = 2;
        logic.cs.Application.OpenURL(url);

        --如果在活动页面 刷新数据
        if(UIActivityForm)then
            UIActivityForm:UpdateAttentionMedia_Response();
        end

        --【红点请求】
        GameController.MainFormControl:RedPointRequest();
    end
end
--endregion


--region【常规活动 重置排序】

function ActivityControl:PanelSort()
    --如果在活动页面
    if(UIActivityForm)then
        UIActivityForm:PanelSort();
    end
end

--endregion


--region【限时活动】【刷新】【绑定有礼】

function ActivityControl:SetBindStatus()
    --如果在活动页面
    if(UIActivityForm)then
        UIActivityForm:SetBindStatus();
    end
end

--endregion


--region【限时活动】【刷新】【关注有礼】

function ActivityControl:SetFollowStatus()
    --如果在活动页面
    if(UIActivityForm)then
        UIActivityForm:SetFollowStatus();
    end
end

--endregion


--region【限时活动】【刷新】【迁移奖励】

function ActivityControl:SetMoveRewardStatus()
    --如果在活动页面
    if(UIActivityForm)then
        UIActivityForm:SetMoveRewardStatus();
    end
end

--endregion


--region【限时活动】【刷新】【全书免费】

function ActivityControl:SetFreeBG()
    --如果在活动页面
    if(UIActivityForm)then
        UIActivityForm:SetFreeBG();
    end
end

--endregion


--region【刷新红点】

function ActivityControl:RefreshRed()
    --如果在活动页面 刷新数据
    if(UIActivityForm)then
        UIActivityForm:RefreshRed();
    end
end

--endregion


--region【请求通用活动列表】-【彩蛋】【旋转抽奖】【免费钥匙】
function ActivityControl:GetActivityListRequest()
    logic.gameHttp:GetActivityList(function(result) self:GetActivityList(result); end)
end
--endregion


--region【请求通用活动列表*响应】-【彩蛋】【旋转抽奖】【免费钥匙】
function ActivityControl:GetActivityList(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----GetActivityList---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        if(json.data==nil or #json.data==0)then return; end

        --*****【全书免费】【刷新开关】
        local Free_isUpdate=self:IsUpdateFreeKey(json.data);

        --【Lua】【缓存最新数据】
        Cache.LimitTimeActivityCache:UpdateActivityList(json.data);

        --*****【免费钥匙】【C#开关】
        self:FreeKeycCsharp(Free_isUpdate);

        --*****【免费钥匙】【刷新界面】
        self:UpdateFreeKeyUI(Free_isUpdate)

        --如果状态改变了 【定时器开启】
        GameController.ActivityBannerControl:TimerRequest();

        --【如果有活动开启】【打开Banner界面】
        if(Cache.LimitTimeActivityCache:IsOpen()==true)then
            --【刷新UIActivityBannerForm】
            self:UpdateActivityBanner();
        else
            GameController.MainFormControl:MoveBanner() --把主界面偏移 收回
        end
    end
end

--endregion


--region【请求获取通用活动详情】-【彩蛋】【旋转抽奖】【免费钥匙】
function ActivityControl:GetActivityInfoRequest(activity)
    logic.gameHttp:GetActivityInfo(activity,function(result) self:GetActivityInfo(result); end)
end
--endregion


--region【请求获取通用活动详情*响应】-【彩蛋】【旋转抽奖】【免费钥匙】
function ActivityControl:GetActivityInfo(result)
    if(string.IsNullOrEmpty(result))then return; end
    logic.debug.Log("----GetActivityInfo---->" .. result)
    local json = core.json.Derialize(result);
    local code = tonumber(json.code);
    if(code == 200)then
        --*****【全书免费】【刷新开关】
        local Free_isUpdate=self:IsUpdateFreeKey(json.data);

        --【刷新保存一组数据】
        Cache.LimitTimeActivityCache:SetActivityInfo(json.data);


        if(json.data.id==EnumActivity.FreeKey)then
            --*****【免费钥匙】【刷新界面】
            self:UpdateFreeKeyUI(Free_isUpdate);
        end

        --【刷新UIActivityBannerForm】
        self:UpdateActivityBanner();
        GameController.MainFormControl:MoveBanner() --把主界面偏移 收回
    end
end
--endregion


--region【刷新活动Banner】

function ActivityControl:UpdateActivityBanner()
    --搜索界面
    local uimainform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
    if(uimainform==nil or uimainform.oldToggleName==nil or uimainform.oldToggleName~="HomeToggle")then return; end

    local uiform = logic.UIMgr:GetView2(logic.uiid.UIActivityBannerForm);
    if(uiform==nil)then
        --打开主界面
        uiform = logic.UIMgr:Open(logic.uiid.UIActivityBannerForm);
    end
    if(uiform)then
        --刷新
        uiform:SetInfo();
    end
end

--endregion


--region***【免费钥匙】【C#开关】
function ActivityControl:FreeKeycCsharp(Free_isUpdate)
    --=======================================================【全书免费】【特殊活动】
    if(Free_isUpdate==true)then
        local FreeKeyInfo1=Cache.LimitTimeActivityCache:GetActivityInfo(EnumActivity.FreeKey);
        if(FreeKeyInfo1)then
            --C#开关
            CS.XLuaHelper.LimitTimeActivity=FreeKeyInfo1.is_open;
        end
    end
    --=======================================================【全书免费】【特殊活动】
end
--endregion***【免费钥匙】【C#开关】



--region***【免费钥匙】【判断是否刷新界面】
function ActivityControl:IsUpdateFreeKey(data)
    --=======================================================【全书免费】【特殊活动】
    --【全书免费】【刷新开关】
    local Free_isUpdate=false;
    --【全书免费】【信息】
    local FreeKeyInfo=Cache.LimitTimeActivityCache:GetActivityInfo(EnumActivity.FreeKey);
    if(FreeKeyInfo)then
        for i = 1, #data do
            --【旋转抽奖】
            if(data[i].id==EnumActivity.FreeKey)then
                --【免费钥匙活动是否开启, 1开启,  0或false关闭】
                if(FreeKeyInfo.is_open~=data[i].is_open)then
                    --如果之前缓存的数据   跟  服务器最新数据不一样   就刷新所有界面
                    Free_isUpdate=true;
                end
            end
        end
    else
        Free_isUpdate=true;
    end
    return Free_isUpdate;
    --=======================================================【全书免费】【特殊活动】
end
--endregion***【免费钥匙】【判断是否刷新界面】



--region***【免费钥匙】【刷新界面】
function ActivityControl:UpdateFreeKeyUI(Free_isUpdate)

    if(Free_isUpdate==true)then
        --如果数据改变了  需要刷新状态
        --【刷新主界面】【推荐列表】【周更】【我的书本】【排行榜】
        GameController.MainFormControl:Limit_time_Free();

        --【书本详情页面刷新】
        local BookDisplayFormobj = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.BookDisplayForm)
        if(BookDisplayFormobj and CS.XLuaHelper.is_Null(BookDisplayFormobj)==false)then
            local BookDisplayForm=BookDisplayFormobj:GetComponent(typeof(CS.BookDisplayForm));
            if(BookDisplayForm)then
                BookDisplayForm:Limit_time_Free();
            end
        end

        --【章节完成界面】
        local BookReading = logic.UIMgr:GetView2(logic.uiid.BookReading);
        if(BookReading)then
            --【限时活动免费读书 显示标签】
            BookReading.chapterSwitch:Limit_time_Free();
        end

        --【限时活动界面】
        local activityForm = logic.UIMgr:GetView2(logic.uiid.UIActivityForm);
        if(activityForm and activityForm.LimitedTimePanel)then
            --【限时活动免费读书 显示标签】
            GameHelper.Limit_time_Free(activityForm.LimitedTimePanel.FreeBG);
        end

        --【主界面 Banner条】
    end
end
--endregion***【免费钥匙】【刷新界面】


--region【刷新广告CD开始】

function ActivityControl:StartCD()
    if(UIActivityForm)then
        UIActivityForm:StartCD();
    end
end

--endregion


--region【刷新广告CD结束】

function ActivityControl:EndCD()
    if(UIActivityForm)then
        UIActivityForm:EndCD();
    end
end

--endregion


--region 【刷新常规活动】【广告CD】【展示文本】

function ActivityControl:ShowCD(txt)
    if(UIActivityForm)then
        UIActivityForm:ShowCD(txt);
    end
end

--endregion


--析构函数
function ActivityControl:__delete()

end


ActivityControl = ActivityControl:GetInstance()
return ActivityControl