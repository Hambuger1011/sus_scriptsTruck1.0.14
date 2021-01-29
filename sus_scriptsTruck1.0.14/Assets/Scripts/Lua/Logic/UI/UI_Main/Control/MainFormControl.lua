local BaseClass = core.Class
local MainFormControl = BaseClass("MainFormControl", core.Singleton)


local UIMainForm=nil;
local MainBookInfoList=nil;



-- 构造函数
function MainFormControl:__init()
    MainBookInfoList={};
end

local refresh_count=0;
function MainFormControl:SetData(mainForm)
    UIMainForm=mainForm;
   -- lastUUID = "";
end


local lastUUID = "";


--region【刷新全部书本】

function MainFormControl:RefreshBooks(isRefreshAll)

    -- 账号是否切换
    if(lastUUID ~= logic.cs.UserDataManager.userInfo.data.userinfo.phoneimei)then
        lastUUID = logic.cs.UserDataManager.userInfo.data.userinfo.phoneimei;
        isRefreshAll = true;
    end

    --region 【请求我的书本】
    self:GetSelfBookInfoRequest();
    self:GetIndexScrollListRequest();
    --endregion

    --region 【请求获取特殊限时活动状态】【彩蛋】【转盘】【全书免费】
    --【获取通用活动列表】
    GameController.ActivityControl:GetActivityListRequest();
    --endregion

    if(isRefreshAll)then
        logic.debug.Log("----booksUpdatedWeekly---->");


--region 【请求排行榜列表】

        local ishave1=GameHelper.islistHave(Cache.MainCache.platform_ranklist);
        local ishave2=GameHelper.islistHave(Cache.MainCache.newbook_ranklist);
        local ishave3=GameHelper.islistHave(Cache.MainCache.popularity_ranklist);
        if(UIMainForm and ishave1==true and ishave2==true and ishave3)then
            --刷新排行榜列表
            UIMainForm:ResetRankList();

        else
            --请求排行榜列表
            logic.gameHttp:GetbookRanking(function(result) self:GetbookRanking(result); end)
        end
--endregion

    end
end

--endregion【刷新全部书本】


--region 【请求我的书本】
function MainFormControl:GetSelfBookInfoRequest()
    logic.gameHttp:GetSelfBookInfo(function(result) self:GetSelfBookInfo(result); end)
end
--endregion


--region 【获取首页顶部滚动栏书本】
function MainFormControl:GetIndexScrollListRequest()
    logic.gameHttp:GetIndexScrollList(function(result) self:GetIndexScrollListCallBack(result); end)
end
--endregion


--region 【获取首页顶部滚动栏书本响应】
--请求我的书本
function MainFormControl:GetIndexScrollListCallBack(result)
    logic.debug.Log("----GetIndexScrollListCallBack---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        UIMainForm.TopBookView:ShowTopBook(json.data.book_ids);

    else
        logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion


--region 【周更新列表*推荐列表 响应】
function MainFormControl:OnGetbooksUpdatedWeekly(result)
    logic.debug.Log("----OnGetbooksUpdatedWeekly---->" .. result);

    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.MainCache:UpdatedWeekly(json);
        --获取周更列表长度
        local weeklyList_Len=table.length(Cache.MainCache.weekly_list);
        if(UIMainForm and weeklyList_Len>0)then
            --刷新周更新列表
            UIMainForm:UpdateWeeklyList(Cache.MainCache.weekly_list);
            --刷新推荐列表
            UIMainForm:UpdateRecommendList(Cache.MainCache.recommend_list);
        end
    else
        logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion


--region 【排行榜列表响应】

--请求排行榜列表
function MainFormControl:GetbookRanking(result)
    logic.debug.Log("----api_bookRankingNew---->" .. result);

    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then

        --存入缓存数据；
        Cache.MainCache:UpdatedRankList(json);

        local ishave1=GameHelper.islistHave(Cache.MainCache.platform_ranklist);
        local ishave2=GameHelper.islistHave(Cache.MainCache.newbook_ranklist);
        local ishave3=GameHelper.islistHave(Cache.MainCache.popularity_ranklist);
        if(UIMainForm and ishave1==true and ishave2==true and ishave3)then
            --刷新排行榜列表
            UIMainForm:ResetRankList();
        end
    else
        logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end

--endregion


--region 【获取我的书本列表响应】
local isFirst=true;
--请求我的书本
function MainFormControl:GetSelfBookInfo(result)

    logic.debug.Log("----GetSelfBookInfo---->" .. result);

    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.MainCache:UpdatedMyBooks(json);
        --C#  也要存数据
        CS.XLuaHelper.SetSelfBookInfo(result);
        --c# 记录玩家书本阅读的进度
        logic.cs.UserDataManager:InitRecordServerBookData()
        logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.BookProgressUpdate)

        --刷新我的书本
        self:ResetMyBookList()

        --region【请求刷新周更新列表*刷新推荐列表】

        --获取周更列表长度
        local weeklyList_Len=table.length(Cache.MainCache.weekly_list);
        if(UIMainForm and weeklyList_Len>0)then
            --刷新周更新列表
            UIMainForm:UpdateWeeklyList(Cache.MainCache.weekly_list);
            --刷新推荐列表
            UIMainForm:UpdateRecommendList(Cache.MainCache.recommend_list);
        else
            --请求刷新周更新列表*刷新推荐列表
            logic.gameHttp:GetbooksUpdatedWeekly(function(result) self:OnGetbooksUpdatedWeekly(result); end)
        end

        --endregion

        if(isFirst)then
            self:GetWindowConfigRequest();
            isFirst=false;
        end

        --【临时】【临时】
        if(Cache.MainCache.migration.migration_web_switch==1)then
            GameController.ActivityControl:UpdateActivityBanner();
        end
        --【临时】【临时】

    else
        logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end


function MainFormControl:GetWindowConfigRequest()
    logic.gameHttp:GetWindowConfig(function(result) self:GetWindowConfigCallBack(result) end)
end

function MainFormControl:ResetWindowConfigStatus()
    GameController.WindowConfig:ResetStatus();
end

function MainFormControl:GetWindowConfigCallBack(result)
    logic.debug.Log("----GetWindowConfigCallBack---->" .. result)
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if code == 200 then
        --【存入缓存数据】
        Cache.PopWindowCache:UpdateList(json.data);

        GameController.WindowConfig:SetWindowsList();
        GameController.WindowConfig:ShowNextWindow();
        GameController.DayPassController:DayPassUpdate();
    end
end

--刷新我的书本
function MainFormControl:ResetMyBookList()
    if(UIMainForm)then
        UIMainForm:ResetMyBookList();
    end
end
--endregion


--region 【获取LGBT列表响应】


function MainFormControl:GetIndexBookList(result)
    logic.debug.Log("----GetIndexBookList---->" .. result);

    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.MainCache:UpdatedLGBTList(json.data.list2);
        --存入缓存数据；
        Cache.MainCache:UpdatedRomanceList(json.data.list1);
        --存入缓存数据；
        Cache.MainCache:UpdatedSuspenseList(json.data.list11);

        if(UIMainForm)then
            if(GameHelper.islistHave(Cache.MainCache.LGBT_list)==true)then
                --刷新列表
                UIMainForm:UpdateLGBTList(Cache.MainCache.LGBT_list);
            end
            if(GameHelper.islistHave(Cache.MainCache.Romance_list)==true)then
                --刷新列表
                UIMainForm:UpdateRomanceList(Cache.MainCache.Romance_list);
            end
            if(GameHelper.islistHave(Cache.MainCache.Suspense_list)==true)then
                --刷新列表
                UIMainForm:UpdateSuspenseList(Cache.MainCache.Suspense_list);
            end
        end
    else
        logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end

--endregion


--region 【主界面移入移出动画】
function MainFormControl:MainFormMove(_type)
    if(UIMainForm)then
        UIMainForm:MainFormMove(_type);
    end
end
--endregion


--region 【红点请求】

local oldTime=0;

--【延时红点请求】
function MainFormControl:DelayRedPointRequest()
    local curTime = os.time();
    --时间差 大于60秒
    local _sunTime =curTime-oldTime;
    if (_sunTime > 60 and oldTime > 0)then
        self:RedPointRequest();
    end
    oldTime = curTime;
end


local RedTimer=nil;
--【定时红点请求】
function MainFormControl:TimerRedPointRequest()
    if(RedTimer)then
        RedTimer:Stop();
        RedTimer = nil
    end
    RedTimer = core.Timer.New(function() self:RedTimerUpdate(); end,120,-1);
    RedTimer:Start();
end

--移出定时器
function MainFormControl:ClearTimer()
    if RedTimer then
        RedTimer:Stop()
        RedTimer = nil
    end
end



function MainFormControl:RedPointRequest()
    logic.gameHttp:GetRedDot(function(result) self:GetRedDot(result); end)
end


--【两分钟一次】
function MainFormControl:RedTimerUpdate()
    --【红点请求】
    self:RedPointRequest();
    refresh_count=refresh_count+1;


    if(Cache.LimitTimeActivityCache:IsOpen()==true)then
        if(refresh_count%30==0)then
            --【获取通用活动列表】
            GameController.ActivityControl:GetActivityListRequest();
        end

        --刷新Banner图 倒计时
        GameController.ActivityBannerControl:UpdateCountdown();

        --刷新DayPass 倒计时
        GameController.DayPassController:UpdateCountdown();
    end

end

--endregion


--region 【红点响应】

function MainFormControl:GetRedDot(result)
    logic.debug.Log("----GetRedDot---->" .. result);

    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then

        --存入缓存数据；
        Cache.RedDotCache:UpdateData(json.data);

        --任务完成未领取的数量
        local task_receive = Cache.RedDotCache.task_receive;
        --未领取奖励的邮件数
        local unreceive_msg_count = Cache.RedDotCache.unreceive_msg_count;
        --未读的邮件数
        local unread_msg_count = Cache.RedDotCache.unread_msg_count;
        --今天是否签到的红点，1已签到，0未签到
        local is_login_receive = Cache.RedDotCache.is_login_receive;
        --每天第一次登陆有任务未完成时显示红点，1.今天已提示过 0.未提示过
        local first_task_notice = Cache.RedDotCache.first_task_notice;
        --【在线阅读未领取奖励 1有未领取，0没有】
        local unreceive_reading_task = Cache.RedDotCache.unreceive_reading_task;
        --【第三方登录绑定奖励未领取 1有未领取，0没有】
        local third_party_award = Cache.RedDotCache.third_party_award;
        --【关注社媒奖励未领取: 1.有未领取 0.没有】
        local attention_media = Cache.RedDotCache.attention_media;
        --【用户迁移奖励未领取: 1.有未领取 0.没有】
        local se_move_finish = Cache.RedDotCache.se_move_finish;
        --【首冲奖励未领取: 1.有未领取 0.没有】
        local first_recharge = Cache.RedDotCache.first_recharge;
        --【邀请奖奖励未领取: 1.有未领取 0.没有】
        local invite_award = Cache.RedDotCache.invite_award;
        --【商城免费钻石未领取: 1.有未领取 0.没有】
        local mall_award = Cache.RedDotCache.mall_award;

        local ui_downform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);

        --【判断常规活动  签到  常规任务  在线阅读时长   有没有红点】
        if (first_task_notice==0 or is_login_receive == 0 or task_receive > 0 or unreceive_reading_task==1)then
            --红点标识 【活动页面里】【常规活动 标签  的红点】【开关】
            Cache.RedDotCache.ActivityPanelRed=true;
        else
            --红点标识 【活动页面里】【常规活动 标签  的红点】【开关】
            Cache.RedDotCache.ActivityPanelRed=false;
        end

        if(unread_msg_count>0)then
            --邮件按钮上的红点
            logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.EmailNumberShow, 1);
            --显示个人中心红点
            if(ui_downform)then
                ui_downform:Profile_RedImg_show(true);
            end
        else
            --邮件未领取奖励数
            if (unreceive_msg_count > 0)then
                --邮件按钮上的红点
                logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.EmailNumberShow, 1);
                --显示个人中心红点
                if(ui_downform)then
                    ui_downform:Profile_RedImg_show(true);
                end
            else
                --邮件按钮上的红点
                logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.EmailNumberShow, 2);
                --关闭个人中心红点
                if(ui_downform)then
                    ui_downform:Profile_RedImg_show(false);
                end
            end
        end



        --【关注社媒奖励未领取: 1.有未领取 0.没有】
        if(attention_media==1)then
            --红点标识 【活动页面里】【限时活动页】【关注社媒有礼红点】【开关】
            Cache.RedDotCache.FollowRedPoint=true;
        else
            --红点标识 【活动页面里】【限时活动页】【关注社媒有礼红点】【开关】
            Cache.RedDotCache.FollowRedPoint=false;
        end

        --【用户迁移奖励未领取: 1.有未领取 0.没有】
        if(se_move_finish==1)then
            --红点标识 【活动页面里】【限时活动页】【用户迁移红点】【开关】
            Cache.RedDotCache.MoveRedPoint=true;
        else
            --红点标识 【活动页面里】【限时活动页】【用户迁移红点】【开关】
            Cache.RedDotCache.MoveRedPoint=false;
        end

        --【首冲奖励未领取: 1.有未领取 0.没有】
        if(first_recharge==1)then
            --红点标识 【活动页面里】【限时活动页】【首冲奖励红点】【开关】
            Cache.RedDotCache.FirstRechargePoint=true;
        else
            --红点标识 【活动页面里】【限时活动页】【首冲奖励红点】【开关】
            Cache.RedDotCache.FirstRechargePoint=false;
        end

        --【邀请奖励未领取: 1.有未领取 0.没有】
        if(invite_award==1)then
            --红点标识 【活动页面里】【限时活动页】【邀请奖励红点】【开关】
            Cache.RedDotCache.InviteAwardPoint=true;
        else
            --红点标识 【活动页面里】【限时活动页】【邀请奖励红点】【开关】
            Cache.RedDotCache.InviteAwardPoint=false;
        end

        --【商城免费钻石未领取: 1.有未领取 0.没有】
        if(mall_award==1)then
            --红点标识 【主页面】【商城免费钻石红点】【开关】
            Cache.RedDotCache.MallAwardPoint=true;
        else
            --红点标识 【主页面】【商城免费钻石红点】【开关】
            Cache.RedDotCache.MallAwardPoint=false;
        end

        --【第三方登录绑定奖励未领取 1已领取，0未领取】
        if(third_party_award==0)then
            local bindStatus = GameHelper.GetBindStatus();--获取绑定状态
            local userData = logic.cs.UserDataManager.userInfo.data.userinfo;
            if(bindStatus and userData)then
                --红点标识 【活动页面里】【限时活动页】【绑定按钮红点】【开关】
                Cache.RedDotCache.BindRedPoint=true;
            else
                --每日首次红点
                local DailyfirstRedPoint_Bind = logic.cs.PlayerPrefs.GetInt("BindRedPoint");
                if (DailyfirstRedPoint_Bind == 0 or DailyfirstRedPoint_Bind == 1)then
                    --红点标识 【活动页面里】【限时活动页】【绑定按钮红点】【开关】
                    Cache.RedDotCache.BindRedPoint=true;
                end
            end
        else
            --红点标识 【活动页面里】【限时活动页】【绑定按钮红点】【开关】
            Cache.RedDotCache.BindRedPoint=false;
        end

        --每日首次红点 【关注有礼】【关注有礼】
        local userData = logic.cs.UserDataManager.userInfo.data.userinfo;
        if(userData)then
            if(tonumber(userData.attention_media_award) == 1)then
            elseif(tonumber(userData.attention_media_award) == 2)then
            else
                --每日首次红点
                local DailyfirstRedPoint_Follow = logic.cs.PlayerPrefs.GetInt("FollowRedPoint");
                if (DailyfirstRedPoint_Follow == 0 or DailyfirstRedPoint_Follow == 1)then
                    --红点标识 【活动页面里】【限时活动页】【关注社媒有礼红点】【开关】
                    Cache.RedDotCache.FollowRedPoint=true;
                end
            end
        end

        --限时全书免费
        local FreeKeyInfo=Cache.LimitTimeActivityCache:GetActivityInfo(EnumActivity.FreeKey);
        if(FreeKeyInfo and FreeKeyInfo.is_open==1)then
            --每日首次红点
            local DailyfirstRedPoint_Free = logic.cs.PlayerPrefs.GetInt("FreeRedPoint");
            if (DailyfirstRedPoint_Free == 0 or DailyfirstRedPoint_Free == 1)then
                --红点标识 【活动页面里】【限时活动页】【全书免费红点】【开关】
                Cache.RedDotCache.FreeRedPoint=true;
            end
        end

        --每日首次红点  News 标签
        local DailyfirstRedPoint_News = logic.cs.PlayerPrefs.GetInt("NewsPanel");
        if (DailyfirstRedPoint_News == 0 or DailyfirstRedPoint_News == 1)then
            --红点标识 【活动页面里】【限时活动 News标签  的红点】【开关】
            Cache.RedDotCache.NewsPanelRed=true;
        end


        if(Cache.RedDotCache.MoveRedPoint==true or Cache.RedDotCache.FreeRedPoint==true or Cache.RedDotCache.FollowRedPoint==true or Cache.RedDotCache.BindRedPoint==true or Cache.RedDotCache.FirstRechargePoint==true or Cache.RedDotCache.InviteAwardPoint==true)then
            --显示主界面底栏 活动中心红点
            if(ui_downform)then
                ui_downform:Rward_RedImg_show(true);
            end
            --【显示主界面 礼品按钮红点】
            if(UIMainForm)then
                UIMainForm.RedImg:SetActive(true);
            end
            --红点标识 【活动页面里】【限时活动 标签  的红点】【开关】
            Cache.RedDotCache.LimitedTimePanelRed=true;

        else
            if(Cache.RedDotCache.ActivityPanelRed==false)then
                --显示主界面底栏 活动中心红点
                if(ui_downform)then
                    ui_downform:Rward_RedImg_show(false);
                end
                --【显示主界面 礼品按钮红点】
                if(UIMainForm)then
                    UIMainForm.RedImg:SetActive(false);
                end
            end
            --红点标识 【活动页面里】【限时活动 标签  的红点】【开关】
            Cache.RedDotCache.LimitedTimePanelRed=false;
        end

        GameController.ActivityControl:RefreshRed();

        --刷新商城免费钻石红点
        local uiform = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.MainFormTop)
        if uiform then
            local MainTopSprite = uiform:GetComponent(typeof(CS.MainTopSprite))
            MainTopSprite:SetRedImage(Cache.RedDotCache.MallAwardPoint);
        end

    else
        logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion


--region 【获更新每天首次登陆有任务未完成时红点状态  请求请求】
function MainFormControl:FirstTaskRequest()
    logic.gameHttp:FirstTaskNotice(function(result) self:FirstTaskNotice(result); end)
end

--endregion


--region 【获更新每天首次登陆有任务未完成时红点状态  响应响应】
function MainFormControl:FirstTaskNotice(result)
    logic.debug.Log("----GetRedDot---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then

        --每天第一次登陆有任务未完成时显示红点，1.今天已提示过 0.未提示过
        --存入缓存数据；
        Cache.RedDotCache.first_task_notice=1;
        --红点请求
        self:RedPointRequest();
    else
        ogic.debug.LogError(json.msg)
    end
end

--endregion


--region【刷新限时阅读】
function MainFormControl:Limit_time_Free()

    if(UIMainForm)then
        --刷新列表
        UIMainForm.RecommendList:Limit_time_Free();
        --刷新列表
        UIMainForm.WeeklyUpdateList:Limit_time_Free();
        --刷新列表
        UIMainForm.MyBookList:Limit_time_Free();
        --排行榜
        UIMainForm.BookRanksList:Limit_time_Free();
    end
end
--endregion


--region【刷新DayPass】
function MainFormControl:DayPass()
    if(UIMainForm)then
        --刷新列表
        UIMainForm.RecommendList:DayPass();
        --刷新列表
        UIMainForm.WeeklyUpdateList:DayPass();
        --刷新列表
        UIMainForm.MyBookList:DayPass();
        --排行榜
        UIMainForm.BookRanksList:DayPass();
    end
end
--endregion


--region【有Banner条】【主界面 自适应往下移】
function MainFormControl:MoveBanner()
    if(UIMainForm)then
        UIMainForm:Limit_time_Free();
    end
end
--endregion

--region【停止顶部轮播】
function MainFormControl:StopTopBookMove()
    if(UIMainForm)then
        UIMainForm.TopBookView:StopMove();
    end
end
--endregion

--region【开始顶部轮播】
function MainFormControl:StartTopBookMove()
    if(UIMainForm)then
        UIMainForm.TopBookView:StartMove();
    end
end
--endregion


--析构函数
function MainFormControl:__delete()

end


MainFormControl = MainFormControl:GetInstance()
return MainFormControl