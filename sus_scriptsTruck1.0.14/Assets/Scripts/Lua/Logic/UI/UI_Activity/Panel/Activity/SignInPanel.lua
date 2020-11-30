local BaseClass = core.Class
local SignInPanel = BaseClass("SignInPanel")


--region【init】
function SignInPanel:__init(gameObject)
    self.gameObject=gameObject;

    --【签到奖励Item集合】
    self.RewardItemList = {}
    for i=1,7 do
        self.RewardItemList[i]=CS.DisplayUtil.GetChild(gameObject, "Reward"..i);
    end
    self.Reward7Btn = self.RewardItemList[7];
    self.RewardTips =CS.DisplayUtil.GetChild(self.RewardItemList[7], "RewardTips")

    self.RewardTipsCloseBtn =CS.DisplayUtil.GetChild(self.RewardTips, "RewardTipsCloseBtn");
    self.SignInBtn =CS.DisplayUtil.GetChild(gameObject, "SignInBtn");
    self.BtnMask = CS.DisplayUtil.GetChild(gameObject, "BtnMask");


    logic.cs.UIEventListener.AddOnClickListener(self.Reward7Btn,function(data) self:Reward7BtnClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.RewardTipsCloseBtn,function(data) self:RewardTipsCloseBtnClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.SignInBtn,function(data) self:SignInBtnClick() end)

    self.toReceivedIndex = 0;
end
--endregion


--region【刷新签到状态】

function SignInPanel:UpdateSignInPanel()
    self.toReceivedIndex = 0;

    local rewardData = Cache.ActivityCache.signinList;
    if(rewardData and #rewardData > 0)then
        for i = 1 , #rewardData do
            local isReceived = rewardData[i].is_receive == 1
            if isReceived then
                self.RewardItemList[i].transform:Find('IsReceived').gameObject:SetActiveEx(true)
                self.RewardItemList[i].transform:Find('Normal').gameObject:SetActiveEx(false)
            else
                self.RewardItemList[i].transform:Find('IsReceived').gameObject:SetActiveEx(false)
                self.RewardItemList[i].transform:Find('Normal').gameObject:SetActiveEx(true)
            end
            if Cache.ActivityCache.is_login_receive == 0 and self.toReceivedIndex == 0 and rewardData[i].is_receive ~= 1 then
                self.toReceivedIndex = i
                self.RewardItemList[i].transform:Find('ToReceive').gameObject:SetActiveEx(true)
            else
                self.RewardItemList[i].transform:Find('ToReceive').gameObject:SetActiveEx(false)
            end
            if i < 7 then
                self.RewardItemList[i].transform:Find('Normal/Num'):GetComponent(typeof(logic.cs.Text)).text = tostring(rewardData[i].diamond_qty * tonumber(Cache.ActivityCache.login_award_multiple))
                self.RewardItemList[i].transform:Find('IsReceived/Num'):GetComponent(typeof(logic.cs.Text)).text = tostring(rewardData[i].diamond_qty * tonumber(Cache.ActivityCache.login_award_multiple))
                self.RewardItemList[i].transform:Find('ToReceive/Num'):GetComponent(typeof(logic.cs.Text)).text = tostring(rewardData[i].diamond_qty * tonumber(Cache.ActivityCache.login_award_multiple))
            elseif i == 7 then
                self.RewardItemList[i].transform:Find('Normal/Num'):GetComponent(typeof(logic.cs.Text)).text = tostring(rewardData[i].diamond_qty * tonumber(Cache.ActivityCache.login_award_multiple)).."+"..tostring(rewardData[i].bkey_qty * tonumber(Cache.ActivityCache.login_award_multiple))
                self.RewardItemList[i].transform:Find('IsReceived/Num'):GetComponent(typeof(logic.cs.Text)).text = tostring(rewardData[i].diamond_qty * tonumber(Cache.ActivityCache.login_award_multiple)).."+"..tostring(rewardData[i].bkey_qty * tonumber(Cache.ActivityCache.login_award_multiple))
                self.RewardItemList[i].transform:Find('ToReceive/Num'):GetComponent(typeof(logic.cs.Text)).text = tostring(rewardData[i].diamond_qty * tonumber(Cache.ActivityCache.login_award_multiple)).."+"..tostring(rewardData[i].bkey_qty * tonumber(Cache.ActivityCache.login_award_multiple))
                self.RewardItemList[i].transform:Find('RewardTips/DimandGiftText'):GetComponent(typeof(logic.cs.Text)).text = "x"..tostring(rewardData[i].diamond_qty * tonumber(Cache.ActivityCache.login_award_multiple))
                self.RewardItemList[i].transform:Find('RewardTips/KeyGiftText'):GetComponent(typeof(logic.cs.Text)).text = "x"..tostring(rewardData[i].bkey_qty * tonumber(Cache.ActivityCache.login_award_multiple))
            end
        end
    end

    if(self.toReceivedIndex == 0)then
        self.BtnMask.gameObject:SetActiveEx(true);
        self.SignInBtn.gameObject:SetActiveEx(false);
        Cache.SignInCache.IsSign=true;
    else
        Cache.SignInCache.IsSign=false;
        self.BtnMask.gameObject:SetActiveEx(false);
        self.SignInBtn.gameObject:SetActiveEx(true);
    end
end

--endregion


--region【领取签到奖励刷新】
function SignInPanel:SignInReceiveReward()
    Cache.SignInCache.IsSign=true;
    self.BtnMask.gameObject:SetActiveEx(true);
    self.SignInBtn.gameObject:SetActiveEx(false);


    self.RewardItemList[self.toReceivedIndex].transform:Find('ToReceive').gameObject:SetActiveEx(false);
    local animTrans = self.RewardItemList[self.toReceivedIndex].transform:Find('IsReceived/Image1').gameObject;
    animTrans.transform.localScale = core.Vector3.New(5,5,1);
    animTrans.gameObject:SetActiveEx(true);
    self.RewardItemList[self.toReceivedIndex].transform:Find('IsReceived').gameObject.gameObject:SetActiveEx(true);
    animTrans.transform:DOScale(core.Vector3.New(1,1,1),0.2):Play();

    Cache.ActivityCache.is_login_receive=1;
    GameController.MainFormControl:RedPointRequest();
    Cache.SignInCache.activity_login.is_receive=1;

    --【AF事件记录* 签到1次】
    CS.AppsFlyerManager.Instance:SIGN();

    if(self.toReceivedIndex==7)then
        --【AF事件记录* 领取一次7日签到奖励】
        CS.AppsFlyerManager.Instance:RECEIVE_7DAY_SIGN_REWARD();
    end

end
--endregion


--region【第七天宝箱奖励提示 点击】
function SignInPanel:Reward7BtnClick()
    local activity = not self.RewardTips.gameObject.activeInHierarchy;
    self.RewardTips.gameObject:SetActiveEx(true);
    self.RewardTipsCloseBtn.gameObject:SetActiveEx(true);
end
--endregion


--region【奖励提示关闭点击】
function SignInPanel:RewardTipsCloseBtnClick()
    self.RewardTips.gameObject:SetActiveEx(false)
    self.RewardTipsCloseBtn.gameObject:SetActiveEx(false)
end
--endregion


--region【签到按钮点击】
function SignInPanel:SignInBtnClick()
    --【签到领取奖励请求】
    GameController.ActivityControl:ReceiveDailyLoginAwardRequest();
    --【打点 签到】
    logic.cs.GamePointManager:BuriedPoint(CS.EventEnum.ActivitySign);
end
--endregion


--region【销毁】
function SignInPanel:__delete()
    if(self.Reward7Btn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.Reward7Btn,function(data) self:Reward7BtnClick() end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.RewardTipsCloseBtn,function(data) self:RewardTipsCloseBtnClick() end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.SignInBtn,function(data) self:SignInBtnClick() end)
    end

    for i=1,7 do
        self.RewardItemList[i]=nil;
    end
    self.RewardItemList=nil;
    self.Reward7Btn=nil;
    self.RewardTips =nil;

    self.RewardTipsCloseBtn=nil;
    self.SignInBtn =nil;
    self.BtnMask=nil;
    self.gameObject=nil;
end
--endregion

return SignInPanel
