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
    self.SignInBtn =CS.DisplayUtil.GetChild(gameObject, "SignInBtn");
    self.BtnMask = CS.DisplayUtil.GetChild(gameObject, "BtnMask");

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
            local numText
            local sprite
            if rewardData[i].diamond_qty and rewardData[i].diamond_qty > 0 then
                numText = rewardData[i].diamond_qty * tonumber(Cache.ActivityCache.login_award_multiple)
                sprite = Cache.PropCache.SpriteData[1]
            elseif rewardData[i].bkey_qty and rewardData[i].bkey_qty > 0 then
                numText = rewardData[i].bkey_qty * tonumber(Cache.ActivityCache.login_award_multiple)
                sprite = Cache.PropCache.SpriteData[2]
            elseif rewardData[i].item_list and #rewardData[i].item_list > 0 then
                for k, v in pairs(rewardData[i].item_list) do
                    numText = v.num
                    if 1000<tonumber(v.id) and tonumber(v.id)<10000 then
                        sprite=DataConfig.Q_DressUpData:GetSprite(v.id)
                    else
                        sprite = Cache.PropCache.SpriteData[v.id]
                    end
                end
            end
            self.RewardItemList[i].transform:Find('Normal/Num'):GetComponent(typeof(logic.cs.Text)).text = numText
            self.RewardItemList[i].transform:Find('IsReceived/Num'):GetComponent(typeof(logic.cs.Text)).text = numText
            self.RewardItemList[i].transform:Find('ToReceive/Num'):GetComponent(typeof(logic.cs.Text)).text = numText
            self.RewardItemList[i].transform:Find('Normal/Image'):GetComponent(typeof(logic.cs.Image)).sprite = sprite
            self.RewardItemList[i].transform:Find('IsReceived/Image'):GetComponent(typeof(logic.cs.Image)).sprite = sprite
            self.RewardItemList[i].transform:Find('ToReceive/Image'):GetComponent(typeof(logic.cs.Image)).sprite = sprite
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
    logic.cs.UIEventListener.RemoveOnClickListener(self.SignInBtn,function(data) self:SignInBtnClick() end)

    for i=1,7 do
        self.RewardItemList[i]=nil;
    end
    self.RewardItemList=nil;
    self.SignInBtn =nil;
    self.BtnMask=nil;
    self.gameObject=nil;
end
--endregion

return SignInPanel
