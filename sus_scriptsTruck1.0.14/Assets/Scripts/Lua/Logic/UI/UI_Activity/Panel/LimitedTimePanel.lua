local BaseClass = core.Class
local LimitedTimePanel = BaseClass("LimitedTimePanel")


--region【init】

function LimitedTimePanel:__init(gameObject)
    self.gameObject=gameObject;
    self.ScrollRect = CS.DisplayUtil.GetChild(gameObject, "ScrollRect");

    --region【绑定有礼】
    self.BindBG = CS.DisplayUtil.GetChild(self.ScrollRect, "BindBG");
    self.BindButton = CS.DisplayUtil.GetChild(self.BindBG, "BindButton");
    self.Received = CS.DisplayUtil.GetChild(self.BindBG, "Received");
    self.ReceiveButton = CS.DisplayUtil.GetChild(self.BindBG, "ReceiveButton");
    self.BindDetailText = CS.DisplayUtil.GetChild(self.BindBG, "BindDetailText"):GetComponent("Text");
    self.BindRedPoint = CS.DisplayUtil.GetChild(self.BindBG, "RedPoint");

    --【设置绑定状态】
    logic.cs.IGGSDKMrg.bindCallBack = function() self:SetBindStatus(); end
    self:SetBindStatus();
    self.RedImg = CS.DisplayUtil.GetChild(self.ReceiveButton, "RedImg");

    logic.cs.UIEventListener.AddOnClickListener(self.BindButton,function(data) logic.UIMgr:Open(logic.uiid.AccountInfo)
        logic.cs.PlayerPrefs.SetInt("BindRedPoint", 2);
        ------【红点功能】
        Cache.RedDotCache.BindRedPoint=false;
        ------【红点功能】
        --【红点请求】
        GameController.MainFormControl:RedPointRequest();
    end)
    logic.cs.UIEventListener.AddOnClickListener(self.ReceiveButton,function(data) self:ReceiveBindRewards() end)
    --endregion


    --region【关注有礼】
    self.mFollowWindow =CS.DisplayUtil.GetChild(self.gameObject, "FollowWindow");
    self.FollowWindow = require('Logic/UI/UI_Activity/Panel/LimitedTime/FollowWindow').New(self.mFollowWindow);

    self.FollowBG = CS.DisplayUtil.GetChild(self.ScrollRect, "FollowBG");
    self.FollowBtn = CS.DisplayUtil.GetChild(self.FollowBG, "FollowBtn");
    self.ClaimBtn = CS.DisplayUtil.GetChild(self.FollowBG, "ClaimBtn");
    self.CompletedBtn = CS.DisplayUtil.GetChild(self.FollowBG, "CompletedBtn");
    self.FollowDetailText =CS.DisplayUtil.GetChild(self.FollowBG, "FollowDetailText"):GetComponent("Text");
    self.FollowRedPoint = CS.DisplayUtil.GetChild(self.FollowBG, "RedPoint");
    self:SetFollowStatus();

    logic.cs.UIEventListener.AddOnClickListener(self.ClaimBtn,function(data) self:ReceiveRewards() end)
    logic.cs.UIEventListener.AddOnClickListener(self.FollowBtn,function(data) self.mFollowWindow:SetActiveEx(true);
        logic.cs.PlayerPrefs.SetInt("FollowRedPoint", 2);
        ------【红点功能】
        Cache.RedDotCache.FollowRedPoint=false;
        ------【红点功能】
        --【红点请求】
        GameController.MainFormControl:RedPointRequest();
    end);
    --endregion

    --region【账号迁移奖励】
    self.MoveRewardBG = CS.DisplayUtil.GetChild(self.gameObject, "MoveRewardBG");
    self.CLAIMButton = CS.DisplayUtil.GetChild(self.gameObject, "CLAIMButton");
    self.PictureNum = CS.DisplayUtil.GetChild(self.gameObject, "PictureNum"):GetComponent(typeof(logic.cs.Text));
    self.KeyNum = CS.DisplayUtil.GetChild(self.gameObject, "KeyNum"):GetComponent(typeof(logic.cs.Text));
    self.DiamondNum = CS.DisplayUtil.GetChild(self.gameObject, "DiamondNum"):GetComponent(typeof(logic.cs.Text));
    self.MoveRewardDetailText = CS.DisplayUtil.GetChild(self.gameObject, "Text3"):GetComponent(typeof(logic.cs.Text));
    self.MoveRedPoint = CS.DisplayUtil.GetChild(self.MoveRewardBG, "RedPoint");

    self:SetMoveRewardStatus();

    logic.cs.UIEventListener.AddOnClickListener(self.CLAIMButton,function(data) self:CLAIMButtonOnClick() end)
    --endregion

    --region【邀请奖励】
    --self.mInviteWindow =CS.DisplayUtil.GetChild(self.gameObject, "InviteWindow");
    --self.InviteWindow = require('Logic/UI/UI_Activity/Panel/LimitedTime/FollowWindow').New(self.mInviteWindow);
    
    self.InviteBG = CS.DisplayUtil.GetChild(self.ScrollRect, "InviteBG");
    self.InviteButton = CS.DisplayUtil.GetChild(self.InviteBG, "InviteButton");
    self.InviteDetailText = CS.DisplayUtil.GetChild(self.InviteBG, "InviteDetailText"):GetComponent("Text");
    self.InviteAwardPoint = CS.DisplayUtil.GetChild(self.InviteBG, "RedPoint");

    self.InviteDetailText.text = "You can get up to 70 Diamonds when you invite your friends to register for an account.";

    logic.cs.UIEventListener.AddOnClickListener(self.InviteButton,function(data) logic.UIMgr:Open(logic.uiid.InvitePanel); end)

    --endregion

    --region【首充奖励】
    
    self.FirstchargeBG = CS.DisplayUtil.GetChild(self.ScrollRect, "FirstchargeBG");
    self.ChargeButton = CS.DisplayUtil.GetChild(self.FirstchargeBG, "ChargeButton");
    self.ClaimFirstcharge = CS.DisplayUtil.GetChild(self.FirstchargeBG, "ClaimFirstcharge");
    self.FirstchargeDetailText = CS.DisplayUtil.GetChild(self.FirstchargeBG, "FirstchargeDetailText"):GetComponent("Text");
    self.FirstRechargePoint = CS.DisplayUtil.GetChild(self.FirstchargeBG, "RedPoint");
    self.Item = CS.DisplayUtil.GetChild(self.FirstchargeBG, "Item");

    self.FirstchargeDetailText.text = "Top up any amount to get insane rewards. Each account is only entitled to one Pack.";
    self.FirstchargeBG:SetActiveEx(tonumber(logic.cs.UserDataManager.selfBookInfo.data.first_recharge_switch) ~= 0)

    logic.cs.UIEventListener.AddOnClickListener(self.ChargeButton,function(data) self:ChargeButtonOnClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ClaimFirstcharge,function(data) self:ClaimFirstchargeOnClick() end)
    
    --endregion

    --region 【全书免费】
    self.FreeBG =CS.DisplayUtil.GetChild(self.gameObject, "FreeBG")
    self.GoBtn =CS.DisplayUtil.GetChild(self.FreeBG, "GoBtn")
    self.LimitTimeText =CS.DisplayUtil.GetChild(self.FreeBG, "LimitTimeText"):GetComponent("Text");
    self.FreeRedPoint = CS.DisplayUtil.GetChild(self.FreeBG, "RedPoint");


    local FreeKeyInfo=Cache.LimitTimeActivityCache:GetActivityInfo(EnumActivity.FreeKey);
    if(FreeKeyInfo and FreeKeyInfo.end_date)then
        self.LimitTimeText.text=FreeKeyInfo.end_date;
    end

    logic.cs.UIEventListener.AddOnClickListener(self.GoBtn,function(data) self:GoBtnClick() end);
    --endregion

    --region【迁移Code】
    if(Cache.MainCache.migration.migration_web_switch==1)then     --迁移web入口开关，1: 开 0:关

        self.MoveCodeBG =CS.DisplayUtil.GetChild(self.gameObject, "MoveCodeBG")
        self.MoveCodeRedPoint = CS.DisplayUtil.GetChild(self.MoveCodeBG, "RedPoint");
        self.MoveCodeBtn = CS.DisplayUtil.GetChild(self.MoveCodeBG, "MoveCodeBtn");
        self.MoveCodeText =CS.DisplayUtil.GetChild(self.MoveCodeBG, "MoveCodeText"):GetComponent("Text");
        self.MoveCodeDetailText =CS.DisplayUtil.GetChild(self.MoveCodeBG, "MoveCodeDetailText"):GetComponent("Text");
        self.MoveCodeBtnText =CS.DisplayUtil.GetChild(self.MoveCodeBtn, "MoveCodeBtnText"):GetComponent("Text");
        self.MoveCodeBG:SetActive(true);

        self.MoveCodeText.text=Cache.MainCache.migration.migration_title;
        self.MoveCodeDetailText.text=Cache.MainCache.migration.migration_content;
        self.MoveCodeBtnText.text=Cache.MainCache.migration.migration_btn;
        logic.cs.UIEventListener.AddOnClickListener(self.MoveCodeBG,function(data) self:MoveCodeBtnClick() end);
    end

    --endregion

end

--endregion


--region【OnOpen】
function LimitedTimePanel:OnOpen()
    --【获取通用奖励配置】---【限时活动】【通用奖励】
    GameController.ActivityControl:GetRewardConfigRequest()

    --【请求获取限时活动状态】
    GameController.ActivityControl:GetActivityInfoRequest(EnumActivity.FreeKey);

    --【刷新全书免费】
    self:SetFreeBG()
    --刷新红点
    self:RedPointShow();

    self.MsgListener = function() self:PaySuccessMsgListener() end
    logic.cs.EventDispatcher.AddMessageListener(logic.cs.EventEnum.PaySuccess, self.MsgListener)
end
--endregion


--region【OnClose】
function LimitedTimePanel:OnClose()
    logic.cs.EventDispatcher.AddMessageListener(logic.cs.EventEnum.PaySuccess, self.MsgListener)
end
--endregion


--region【临时】【临时】
function LimitedTimePanel:MoveCodeBtnClick()

    --【临时】【临时】
    if(Cache.MainCache.migration.migration_web_url)then
        local _url=Cache.MainCache.migration.migration_web_url.."?token="..string.encodeURI(logic.cs.GameHttpNet.TOKEN);
        logic.cs.Application.OpenURL(_url);
    end

end
--endregion



--region【PaySuccessMsgListener】
function LimitedTimePanel:PaySuccessMsgListener()
    self.FirstRechargePoint:SetActiveEx(true);
    self.ChargeButton:SetActiveEx(false);
    self.ClaimFirstcharge:SetActiveEx(true);
end
--endregion

function LimitedTimePanel:GetRewardText(DiamondsNum,KeysNum,ItemList)
    local rewardText = "Get "
    local needAnd = false
    if DiamondsNum and tonumber(DiamondsNum) > 0 then
        rewardText = rewardText..DiamondsNum.." Diamonds"
        needAnd = true
    end
    if KeysNum and tonumber(KeysNum) > 0 then
        if needAnd then
            rewardText = rewardText.." and "
        end
        rewardText = rewardText..KeysNum.." Keys"
        needAnd = true
    end
    for k, v in ipairs(ItemList) do
        if needAnd then
            rewardText = rewardText.." and "
        end
        rewardText = rewardText..v.num.." "..v.name
        needAnd = true
    end
    rewardText = rewardText.."!"
    if not needAnd then
        rewardText = string.Empty
    end
    return rewardText
end


--region【刷新通用奖励数据】
function LimitedTimePanel:GetRewardConfig_Response()

    local attentionMedia = Cache.ActivityCache.attention_media;
    self.FollowDetailText.text = self:GetRewardText(attentionMedia.diamond_count,attentionMedia.key_count,attentionMedia.item_list);

    local thirdPartyBind = Cache.ActivityCache.third_party_bind;
    self.BindDetailText.text = self:GetRewardText(thirdPartyBind.diamond_count,thirdPartyBind.key_count,thirdPartyBind.item_list);
    
    local rewardText=nil;

    local userMove = Cache.ActivityCache.user_move;
    if(userMove)then
        if(userMove.award_type == 1)then
            rewardText = "Get "..userMove.key_count.." keys!";
        elseif(userMove.award_type == 2)then
            rewardText = "Get "..userMove.diamond_count.." diamonds!";
        elseif(userMove.award_type == 4)then
            rewardText = "Get "..userMove.diamond_count.." diamonds and "..userMove.key_count.." keys";
            if(userMove.user_frame_id)then
                rewardText = rewardText .. "\nand a picture frame!";
            else
                rewardText = rewardText .. "!";
            end
        end
        self.MoveRewardDetailText.text = rewardText;
        if(userMove.key_count)then
            self.KeyNum.text = "x"..userMove.key_count;
        else
            self.KeyNum.text = "";
        end
        if(userMove.diamond_count)then
            self.DiamondNum.text = "x"..userMove.diamond_count
        else
            self.DiamondNum.text = ""
        end
        if(userMove.user_frame_id)then
            self.PictureNum.text = "x1"
        else
            self.PictureNum.text = ""
        end
    end
    
    local firstRecharge = Cache.ActivityCache.first_recharge;

    local RewardTrans = {}
    if tonumber(firstRecharge.diamond_count) > 0 then
        local item = logic.cs.GameObject.Instantiate(self.Item,self.FirstchargeBG.transform,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".. firstRecharge.diamond_count;
        Icon.sprite = Cache.PropCache.SpriteData[1]
        Icon:SetNativeSize()
        Icon.transform.localScale = core.Vector3.New(0.4,0.4,1)
        table.insert(RewardTrans,item)
    end
    if tonumber(firstRecharge.key_count) > 0 then
        local item = logic.cs.GameObject.Instantiate(self.Item,self.FirstchargeBG.transform,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".. firstRecharge.key_count;
        Icon.sprite = Cache.PropCache.SpriteData[2]
        Icon:SetNativeSize()
        Icon.transform.localScale = core.Vector3.New(0.4,0.4,1)
        table.insert(RewardTrans,item)
    end

    if firstRecharge.item_list then
        for k, v in pairs(firstRecharge.item_list) do
            local item = logic.cs.GameObject.Instantiate(self.Item,self.FirstchargeBG.transform,false)
            local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
            local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
            Num.text = "x".. v.num;
            if 1000<tonumber(v.id) and tonumber(v.id)<10000 then
                local sprite=DataConfig.Q_DressUpData:GetSprite(v.id)
                Icon.sprite = sprite
            else
                Icon.sprite = Cache.PropCache.SpriteData[v.id]
            end
            Icon:SetNativeSize()
            Icon.transform.localScale = core.Vector3.New(0.2,0.2,1)
            table.insert(RewardTrans,item)
        end
    end

    local PosList =
    {
        {core.Vector2.New(422,-102)},

        {core.Vector2.New(434,-70),core.Vector2.New(380,-153)},

        {core.Vector2.New(478,-54),core.Vector2.New(406,-108),
         core.Vector2.New(367,-187)},

        {core.Vector2.New(500,-32),core.Vector2.New(433,-72),
         core.Vector2.New(390,-126),core.Vector2.New(363,-193)},

        {core.Vector2.New(500,-32),core.Vector2.New(445,-68),
         core.Vector2.New(395.4,-108.3),core.Vector2.New(363,-157.8),
         core.Vector2.New(360.4,-219.5),}
    }
    
    local size = #RewardTrans > #PosList and #PosList or #RewardTrans
    for i = 1, size do
        RewardTrans[i].transform.anchoredPosition =PosList[size][i]
        RewardTrans[i]:SetActive(true)
    end

end
--endregion


--region【设置绑定状态】
function LimitedTimePanel:SetBindStatus()
    if(self.BindButton and self.ReceiveButton and self.Received)then
        self.BindButton.gameObject:SetActiveEx(false)
        self.ReceiveButton.gameObject:SetActiveEx(false)
        self.Received.gameObject:SetActiveEx(false)
        local bindStatus = GameHelper.GetBindStatus();--获取绑定状态
        local userData = logic.cs.UserDataManager.userInfo.data.userinfo;
        if(bindStatus and userData)then
            if(tonumber(userData.third_party_award) == 0)then
                self.ReceiveButton.gameObject:SetActiveEx(true);
                --刷新红点状态
                GameController.MainFormControl:RedPointRequest();
            else
                self.Received.gameObject:SetActiveEx(true);
                --self.BindBG.gameObject:SetActiveEx(false);
            end
        else
            self.BindButton.gameObject:SetActiveEx(true);
        end
    end
end
--endregion


--region【设置follow】
function LimitedTimePanel:SetFollowStatus()
    if(self.FollowBtn and self.ClaimBtn and self.CompletedBtn)then
        self.FollowBtn.gameObject:SetActiveEx(false);
        self.ClaimBtn.gameObject:SetActiveEx(false);
        self.CompletedBtn.gameObject:SetActiveEx(false);
        local userData = logic.cs.UserDataManager.userInfo.data.userinfo;
        if(userData)then
            if(tonumber(userData.attention_media_award) == 1)then
                --self.FollowBG:SetActiveEx(false);
               self.CompletedBtn.gameObject:SetActiveEx(true);
            elseif(tonumber(userData.attention_media_award) == 2)then
                self.ClaimBtn.gameObject:SetActiveEx(true);
            else
                self.FollowBtn.gameObject:SetActiveEx(true);
            end
        else
            --self.FollowBG:SetActiveEx(false);
            self.CompletedBtn.gameObject:SetActiveEx(true);
        end
    end
end
--endregion


--region【设置迁移奖励】
function LimitedTimePanel:SetMoveRewardStatus()
    self.MoveRewardBG.gameObject:SetActiveEx(tonumber(logic.cs.UserDataManager.userInfo.data.userinfo.se_move_finish) == 1)
end

--endregion


--region【设置全书免费】
function LimitedTimePanel:SetFreeBG()
    if(self.FreeBG)then
        --限时全书免费
        local FreeKeyInfo=Cache.LimitTimeActivityCache:GetActivityInfo(EnumActivity.FreeKey);
        if(FreeKeyInfo and FreeKeyInfo.is_open==1)then
            self.FreeBG:SetActiveEx(true);
        else
            self.FreeBG:SetActiveEx(false);
        end
    end
end
--endregion



--region【GO点击 去读书】
function LimitedTimePanel:GoBtnClick()
    logic.cs.PlayerPrefs.SetInt("FreeRedPoint", 2);
    Cache.RedDotCache.FreeRedPoint=false;
    --【红点请求】
    GameController.MainFormControl:RedPointRequest();

    if(GameHelper.CurBookId and GameHelper.CurBookId>0)then
        local bookinfo={};
        bookinfo.book_id=GameHelper.CurBookId;
        GameHelper.BookClick(bookinfo);

        --埋点*点击前往阅读
        logic.cs.GamePointManager:BuriedPoint("activity_cumulative_read_go");
    end
end
--endregion


--region【接收绑定奖励】
function LimitedTimePanel:ReceiveBindRewards()
    local uicollect= logic.UIMgr:Open(logic.uiid.UICollectForm);
    if(uicollect)then                                   --【请求领取第三方登录绑定的奖励】---【限时活动】【账号绑定奖励】
        uicollect:SetData(Cache.ActivityCache.third_party_bind.diamond_count,Cache.ActivityCache.third_party_bind.key_count,"CLAIM",
                function() GameController.ActivityControl:ReceiveThirdPartyAwardRequest(); end,Cache.ActivityCache.third_party_bind.item_list);
    end
end
--endregion


--region 【领取关注社媒奖励】
function LimitedTimePanel:ReceiveRewards()
    local uicollect= logic.UIMgr:Open(logic.uiid.UICollectForm);
    if(uicollect)then                                            --【领取关注社媒奖励】---【限时活动】【关注社媒奖励】
        uicollect:SetData(Cache.ActivityCache.attention_media.diamond_count,Cache.ActivityCache.attention_media.key_count,"CLAIM",
                function() GameController.ActivityControl:ReceiveAttentionMediaRewardRequest(); end,Cache.ActivityCache.attention_media.item_list);
    end
end
--endregion


--region【领取迁移奖励】
function LimitedTimePanel:CLAIMButtonOnClick()
    local uicollect= logic.UIMgr:Open(logic.uiid.UICollectForm);
    if(uicollect)then                                            --【领取用户迁移的奖励】---【限时活动】【账号迁移奖励】
        uicollect:SetData(Cache.ActivityCache.user_move.diamond_count,Cache.ActivityCache.user_move.key_count,"CLAIM",
                function() GameController.ActivityControl:ReceiveUserMoveAwardRequest(); end,Cache.ActivityCache.user_move.item_list);
    end
end
--endregion


--region【领取首充奖励】
function LimitedTimePanel:ClaimFirstchargeOnClick()
    local uicollect= logic.UIMgr:Open(logic.uiid.UICollectForm);
    if(uicollect)then
        uicollect:SetData(Cache.ActivityCache.first_recharge.diamond_count,Cache.ActivityCache.first_recharge.key_count,"CLAIM",
                function() GameController.ActivityControl:ReceiveFirstRechargeAwardRequest(); end,Cache.ActivityCache.first_recharge.item_list);
    end
end
--endregion


--region【前往充值】
function LimitedTimePanel:ChargeButtonOnClick()
    local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.ChargeMoneyForm)
    local tapForm = uiform:GetComponent(typeof(CS.ChargeMoneyForm))
    tapForm:SetFormStyle(2);
    uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.MainFormTop)
    tapForm = uiform:GetComponent(typeof(CS.MainTopSprite))
    tapForm:GamePlayTopOpen("UIChargeMoneyForm1");
end
--endregion


--region【领取用户迁移的奖励*响应】---【限时活动】【限时活动*账号迁移奖励】
function LimitedTimePanel:ReceiveFirstRechargeAward_Response()
    self.FirstchargeBG.gameObject:SetActiveEx(false)
end
--endregion


--region【领取首充奖励*响应】---【限时活动】【限时活动*账号迁移奖励】
function LimitedTimePanel:ReceiveUserMoveAward_Response()
    self.MoveRewardBG.gameObject:SetActiveEx(false);
end
--endregion


--region【领取第三方登录绑定的奖励*响应】---【限时活动】【账号绑定奖励】
function LimitedTimePanel:ReceiveThirdPartyAward_Response()
    self.ReceiveButton.gameObject:SetActiveEx(false);
    self.Received.gameObject:SetActiveEx(true);
    --self.BindBG.gameObject:SetActiveEx(false);
end
--endregion


--region【领取关注社媒奖励*响应】---【限时活动】【关注社媒奖励】
function LimitedTimePanel:ReceiveAttentionMediaReward_Response()
    self.ClaimBtn.gameObject:SetActiveEx(false);
    --self.FollowBG:SetActiveEx(false);
    self.CompletedBtn.gameObject:SetActiveEx(true);
end
--endregion


--region【更新社媒状态奖励为可领取*响应】---【限时活动】【关注社媒】
function LimitedTimePanel:UpdateAttentionMedia_Response()
    self.FollowBtn.gameObject:SetActiveEx(false);
    self.ClaimBtn.gameObject:SetActiveEx(true);
    self.mFollowWindow:SetActiveEx(false);
end
--endregion


--region【红点功能】
function LimitedTimePanel:RedPointShow()
    ------【红点功能】【关注有礼】
    if(Cache.RedDotCache.FollowRedPoint==true)then
        self.FollowRedPoint:SetActiveEx(true);
    else
        self.FollowRedPoint:SetActiveEx(false);
    end
    ------【红点功能】

    ------【红点功能】【绑定有礼】
    if(Cache.RedDotCache.BindRedPoint==true)then
        self.BindRedPoint:SetActiveEx(true);
    else
        self.BindRedPoint:SetActiveEx(false);
    end
    ------【红点功能】


    ------【红点功能】【全书免费】
    if(Cache.RedDotCache.FreeRedPoint==true)then
        self.FreeRedPoint:SetActiveEx(true);
    else
        self.FreeRedPoint:SetActiveEx(false);
    end
    ------【红点功能】

    ------【红点功能】【账号迁移】
    if(Cache.RedDotCache.MoveRedPoint==true)then
        self.MoveRedPoint:SetActiveEx(true);
    else
        self.MoveRedPoint:SetActiveEx(false);
    end
    ------【红点功能】

    ------【红点功能】【账号迁移】
    if(Cache.RedDotCache.FirstRechargePoint==true)then
        self.FirstRechargePoint:SetActiveEx(true);
        self.ChargeButton:SetActiveEx(false);
        self.ClaimFirstcharge:SetActiveEx(true);
    else
        self.FirstRechargePoint:SetActiveEx(false);
        self.ChargeButton:SetActiveEx(true);
        self.ClaimFirstcharge:SetActiveEx(false);
    end
    ------【红点功能】

    ------【红点功能】【邀请】
    if(Cache.RedDotCache.InviteAwardPoint==true)then
        self.InviteAwardPoint:SetActiveEx(true);
    else
        self.InviteAwardPoint:SetActiveEx(false);
    end
    ------【红点功能】
end
--endregion








--region【销毁】
function LimitedTimePanel:__delete()

    if(self.BindButton)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.BindButton,function(data) logic.UIMgr:Open(logic.uiid.AccountInfo) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.BindButton,function(data) logic.UIMgr:Open(logic.uiid.AccountInfo) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.ReceiveButton,function(data) self:ReceiveBindRewards() end);
    end

    if(self.ClaimBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.ClaimBtn,function(data) self:ReceiveRewards() end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.FollowBtn,function(data) self.mFollowWindow:SetActiveEx(true); end);
    end
    if(self.CLAIMButton)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.CLAIMButton,function(data) self:CLAIMButtonOnClick() end);
    end
    if(self.GoBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.GoBtn,function(data) self:GoBtnClick() end);
    end


    if(self.MoveCodeBG)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.MoveCodeBG,function(data) self:MoveCodeBtnClick() end);
    end


    self.ScrollRect = nil;
    self.BindBG = nil;
    self.BindButton = nil;
    self.Received = nil;
    self.ReceiveButton = nil;
    self.BindDetailText = nil;
    self.RedImg = nil;
    self.mFollowWindow = nil;
    self.FollowWindow = nil;
    self.FollowBG = nil;
    self.FollowBtn = nil;
    self.ClaimBtn = nil;
    self.CompletedBtn = nil;
    self.FollowDetailText = nil;
    self.MoveRewardBG = nil;
    self.CLAIMButton = nil;
    self.PictureNum = nil;
    self.KeyNum = nil;
    self.DiamondNum = nil;
    self.MoveRewardDetailText = nil;
    self.FreeBG = nil;
    self.GoBtn = nil;
    self.LimitTimeText = nil;


    self.gameObject=nil;
end
--endregion

return LimitedTimePanel
