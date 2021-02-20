local BaseClass = core.Class
local ActivityPanel = BaseClass("ActivityPanel")


--region【init】

function ActivityPanel:__init(gameObject)

    self.gameObject=gameObject;

    self.ScrollRect =gameObject:GetComponent("ScrollRect");

    --【签到】
    self.mSignInPanel =CS.DisplayUtil.GetChild(self.gameObject, "SignInPanel");
    self.SignInPanel = require('Logic/UI/UI_Activity/Panel/Activity/SignInPanel').New(self.mSignInPanel);

    --【在线阅读时长】
    self.mReadTimePanel =CS.DisplayUtil.GetChild(self.gameObject, "ReadTimePanel");
    self.ReadTimePanel = require('Logic/UI/UI_Activity/Panel/Activity/ReadTimePanel').New(self.mReadTimePanel);

    --【每日任务】
    self.mDailyTasksPanel =CS.DisplayUtil.GetChild(self.gameObject, "DailyTasksPanel");
    self.DailyTasksPanel = require('Logic/UI/UI_Activity/Panel/Activity/DailyTasksPanel').New(self.mDailyTasksPanel);

    --【看广告】
    self.mFreeRewardPanel =CS.DisplayUtil.GetChild(self.gameObject, "FreeRewardPanel");
    self.FreeRewardPanel = require('Logic/UI/UI_Activity/Panel/Activity/FreeRewardPanel').New(self.mFreeRewardPanel);
    self.mFreeRewardPanel:SetActive(logic.cs.UserDataManager.switchStatus and logic.cs.UserDataManager.switchStatus.data.ad_activity_status == 1)

    --第一排序 数组
    self.sort1={};
    --第二排序 数组
    self.sort2={};
end

--endregion


--region【面板排序】
function ActivityPanel:PanelSort()
    --第一排序 数组
    self.sort1={};
    --第二排序 数组
    self.sort2={};

    if(self.mSignInPanel)then
        local toReceivedIndex=0;
        local list=Cache.ActivityCache.signinList;
        if(list and #list > 0)then
            for i = 1, #list do
                if Cache.ActivityCache.is_login_receive == 0 and list[i].is_receive ~= 1 then
                    toReceivedIndex = i;
                end
            end
        end
        if(toReceivedIndex==0)then
            Cache.SignInCache.IsSign=true;
        else
            Cache.SignInCache.IsSign=false;
        end
        --【面板排序】  --【今天无可做的 调换到最底下】
        if(Cache.SignInCache.IsSign==true)then
            --排序到最后  最底下
            table.insert(self.sort2,EActivityPanel.SignIn)
        else
            table.insert(self.sort1,EActivityPanel.SignIn)
        end
    end

    if(self.mReadTimePanel)then
        --【面板排序】  --【【在线阅读时长 是否奖励全部领完】 调换到最底下】
        if(Cache.ReadTimeCache.receive_level==5)then
            --排序到最后  最底下
            table.insert(self.sort2,EActivityPanel.ReadTime)
        else
            table.insert(self.sort1,EActivityPanel.ReadTime)
        end
    end

    if(self.mDailyTasksPanel)then
        --【面板排序】  --【【是否所有任务结束】 调换到最底下】
        if(Cache.ActivityCache:isAllTaskOver()==true)then
            --排序到最后  最底下
            table.insert(self.sort2,EActivityPanel.DailyTasks)
        else
            table.insert(self.sort1,EActivityPanel.DailyTasks)
        end
    end

    if(self.mFreeRewardPanel)then
        --【面板排序】  --【【看广告剩余次数为0】 调换到最底下】
        if(Cache.ActivityCache.ad_number==0)then
            --排序到最后  最底下
            table.insert(self.sort2,EActivityPanel.FreeReward)
        else
            table.insert(self.sort1,EActivityPanel.FreeReward)
        end
    end

    if(self.sort1 and #self.sort1>0)then
        for i = 1, #self.sort1 do
           local index = i-1;
           self:StartSortPanel(self.sort1[i],index);
        end
    end


    if(self.sort2 and #self.sort2>0)then
        for i = 1, #self.sort2 do
            local index=0;
            if(self.sort1 and #self.sort1>0)then
                index= i + #self.sort1 -1;
            else
                index = i-1;
            end
            self:StartSortPanel(self.sort2[i],index);
        end
    end
end

function ActivityPanel:StartSortPanel(obj,index)

    if(obj==EActivityPanel.SignIn)then
        if(self.mSignInPanel)then
            self.mSignInPanel.transform:SetSiblingIndex(index);
        end
    elseif(obj==EActivityPanel.ReadTime)then
        if(self.mReadTimePanel)then
            self.mReadTimePanel.transform:SetSiblingIndex(index);
        end
    elseif(obj==EActivityPanel.DailyTasks)then
        if(self.mDailyTasksPanel)then
            self.mDailyTasksPanel.transform:SetSiblingIndex(index);
        end
    elseif(obj==EActivityPanel.FreeReward)then
        if(self.mFreeRewardPanel)then
            self.mFreeRewardPanel.transform:SetSiblingIndex(index);
        end
    end
end

--endregion


--region【刷新常规活动 签到面板】
function ActivityPanel:UpdateSignInPanel()
    if(self.SignInPanel)then
        self.SignInPanel:UpdateSignInPanel();

        --排序到最后  最底下
        self:PanelSort();
    end
end
--endregion


--region【刷新常规活动 签到领取奖励】
function ActivityPanel:SignInReceiveReward()
    if(self.SignInPanel)then
        self.SignInPanel:SignInReceiveReward();


        --【面板排序】  --【今天无可做的 调换到最底下】
        if(Cache.SignInCache.IsSign==true)then
            --排序到最后  最底下
            self:PanelSort();
        end
    end
end
--endregion


--region【刷新常规活动 每日任务】
function ActivityPanel:UpdateDailyTasksPanel()
    if(self.DailyTasksPanel)then
        self.DailyTasksPanel:UpdateDailyTasksPanel();
        --logic.cs.ContentSizeFitterRefresh:Refresh(self.DailyTasksPanel.Layout.transform);

        --排序到最后  最底下
        self:PanelSort();
    end
end
--endregion


--region【刷新常规活动 在线阅读时长】
function ActivityPanel:UpdateReadTimePanel()
    if(self.ReadTimePanel)then
        self.ReadTimePanel:UpdateReadTimePanel();


        --【面板排序】  --【【是否所有任务结束】 调换到最底下】
        if(Cache.ReadTimeCache.receive_level==5)then
            --排序到最后  最底下
            self:PanelSort();
        end
    end
end
--endregion


--region【刷新常规活动 看广告】
function ActivityPanel:UpdateFreeRewardPanel()
    if(self.FreeRewardPanel)then
        self.FreeRewardPanel:UpdateFreeRewardPanel();

        --【面板排序】  --【【看广告剩余次数为0】 调换到最底下】
        if(Cache.ActivityCache.ad_number==0)then
            --排序到最后  最底下
            self:PanelSort();
        end
    end
end
--endregion


--region【刷新常规活动 领取活动广告奖励】
function ActivityPanel:ReceiveDailyAdAward()
    if(self.FreeRewardPanel)then
        self.FreeRewardPanel:ReceiveDailyAdAward();
    end
end
--endregion


--region 【刷新常规活动】【广告CD开始】
function ActivityPanel:StartCD()
    if(self.FreeRewardPanel)then
        self.FreeRewardPanel:StartCD();
    end
end

--endregion


--region 【刷新常规活动】【广告CD结束】
function ActivityPanel:EndCD()
    if(self.FreeRewardPanel)then
        self.FreeRewardPanel:EndCD();
    end
end

--endregion


--region 【刷新常规活动】【广告CD】【展示文本】
function ActivityPanel:ShowCD(txt)
    if(self.FreeRewardPanel)then
        self.FreeRewardPanel:ShowCD(txt);
    end
end

--endregion


--region【UIActivityForm设置ScrollRect】---【常规活动】
function ActivityPanel:SetVerticalNormalizedPosition()
    if(self.ScrollRect)then
        self.ScrollRect.verticalNormalizedPosition=0;
    end
end
--endregion


--region【销毁】

function ActivityPanel:__delete()

    --关闭销毁 【SignInPanel】lua脚本
    self.SignInPanel:Delete();
    --关闭销毁 【ReadTimePanel】lua脚本
    self.ReadTimePanel:Delete();
    --关闭销毁 【DailyTasksPanel】lua脚本
    self.DailyTasksPanel:Delete();
    --关闭销毁 【FreeRewardPanel】lua脚本
    self.FreeRewardPanel:Delete();

    self.ScrollRect = nil;
    self.mSignInPanel = nil;
    self.SignInPanel = nil;
    self.mReadTimePanel = nil;
    self.ReadTimePanel = nil;
    self.mDailyTasksPanel = nil;
    self.DailyTasksPanel = nil;
    self.mFreeRewardPanel = nil;
    self.FreeRewardPanel = nil;
    self.gameObject =nil;
end

--endregion


return ActivityPanel
