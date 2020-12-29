local BaseClass = core.Class
local ReadTimePanel = BaseClass("ReadTimePanel")



--region【init】

function ReadTimePanel:__init(gameObject)
    self.gameObject=gameObject;
    self.TitleText =CS.DisplayUtil.GetChild(gameObject, "TitleText"):GetComponent("Text");
    self.DescriptionText =CS.DisplayUtil.GetChild(gameObject, "DescriptionText"):GetComponent("Text");

    self.ReadTimeRewardList =CS.DisplayUtil.GetChild(gameObject, "ReadTimeRewardList");
    self.ReadTimeReward1 =CS.DisplayUtil.GetChild(self.ReadTimeRewardList, "ReadTimeReward1"):GetComponent("Image");
    self.ReadTimeReward2 =CS.DisplayUtil.GetChild(self.ReadTimeRewardList, "ReadTimeReward2"):GetComponent("Image");
    self.ReadTimeReward3 =CS.DisplayUtil.GetChild(self.ReadTimeRewardList, "ReadTimeReward3"):GetComponent("Image");
    self.ReadTimeReward4 =CS.DisplayUtil.GetChild(self.ReadTimeRewardList, "ReadTimeReward4"):GetComponent("Image");
    self.ReadTimeReward5 =CS.DisplayUtil.GetChild(self.ReadTimeRewardList, "ReadTimeReward5"):GetComponent("Image");

    self.ReadTimeBar =CS.DisplayUtil.GetChild(gameObject, "ReadTimeBar"):GetComponent("Image");

    self.ReadTimeGoto =CS.DisplayUtil.GetChild(gameObject, "ReadTimeGoto");
    self.ReadTimeBtn =CS.DisplayUtil.GetChild(gameObject, "ReadTimeBtn");
    self.CompletedBtn =CS.DisplayUtil.GetChild(gameObject, "CompletedBtn");

    self.GiftTips =CS.DisplayUtil.GetChild(self.ReadTimeRewardList, "GiftTips");
    self.GiftTipsRect =self.GiftTips:GetComponent("RectTransform");
    self.KeyGift =CS.DisplayUtil.GetChild(self.GiftTips, "KeyGift");
    self.DimandGift =CS.DisplayUtil.GetChild(self.GiftTips, "DimandGift");
    self.KeyGiftText =CS.DisplayUtil.GetChild(self.GiftTips, "KeyGiftText"):GetComponent("Text");
    self.DimandGiftText =CS.DisplayUtil.GetChild(self.GiftTips, "DimandGiftText"):GetComponent("Text");
    self.GiftTipCloseBtn =CS.DisplayUtil.GetChild(self.GiftTips, "GiftTipCloseBtn");


    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.GiftTipCloseBtn,function(data) self:OnGiftTipCloseBtnClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ReadTimeGoto,function(data) self:OnReadTimeGotoClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ReadTimeBtn,function(data) self:OnReadTimeClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ReadTimeReward1.gameObject,function(data) self:OnReadTimeRewardClick1() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ReadTimeReward2.gameObject,function(data) self:OnReadTimeRewardClick2() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ReadTimeReward3.gameObject,function(data) self:OnReadTimeRewardClick3() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ReadTimeReward4.gameObject,function(data) self:OnReadTimeRewardClick4() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ReadTimeReward5.gameObject,function(data) self:OnReadTimeRewardClick5() end)

    self.TitleText.text="ONLINE REWARDS";
    self.DescriptionText.text="Collect rewards when you achieve the event goals. Rewards reset every day at (00:00 GMT-5)";

    GameHelper.CloseReadTimer();

    --【请求获取在线阅读任务状态】
    GameController.ActivityControl:ReadingTaskStatusRequest();
end
--endregion


--region【刷新数据】
local isClick=false;
function ReadTimePanel:UpdateReadTimePanel()
    self.GiftTips:SetActive(false);
    --显示进度
    if(Cache.ReadTimeCache.finish_level>0)then
        self.ReadTimeBar.fillAmount=Cache.ReadTimeCache.finish_level/5;
    end

    if(Cache.ReadTimeCache.receive_level>0)then
        self.ReadTimeReward1.sprite =CS.ResourceManager.Instance:GetUISprite("ActivityAndNews/act_icon_gift box2");
        if(Cache.ReadTimeCache.receive_level>1)then
            self.ReadTimeReward2.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityAndNews/act_icon_gift box2");
            if(Cache.ReadTimeCache.receive_level>2)then
                self.ReadTimeReward3.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityAndNews/act_icon_gift box2");
                if(Cache.ReadTimeCache.receive_level>3)then
                    self.ReadTimeReward4.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityAndNews/act_icon_gift box2");
                    if(Cache.ReadTimeCache.receive_level>4)then
                        self.ReadTimeReward5.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityAndNews/act_icon_gift box2");
                    end
                end
            end
        end
    end

    if(Cache.ReadTimeCache.receive_level==5)then
        self.ReadTimeGoto:SetActive(false);
        self.ReadTimeBtn:SetActive(false);
        self.CompletedBtn:SetActive(true);
    else
        if(Cache.ReadTimeCache.finish_level>Cache.ReadTimeCache.receive_level)then
            self.ReadTimeBtn:SetActive(true);
            self.ReadTimeGoto:SetActive(false);
            self.CompletedBtn:SetActive(false);
        else
            self.ReadTimeBtn:SetActive(false);
            self.ReadTimeGoto:SetActive(true);
            self.CompletedBtn:SetActive(false);
        end
    end
    isClick=false;
end
--endregion


--region【领取奖励点击】
function ReadTimePanel:OnReadTimeClick()
    self.GiftTips:SetActive(false);
    if(isClick)then return; end
    isClick = true;
    --请求领取奖励
    GameController.ActivityControl:ReadingTaskPrizeRequest()
end
--endregion


--region【点击宝箱1】
function ReadTimePanel:OnReadTimeRewardClick1()
    self:ComReadTimeRewardClick(1,self.ReadTimeReward1.gameObject.transform);
    self:SetKeyDiamondsText(1);
end
--endregion


--region【点击宝箱2】
function ReadTimePanel:OnReadTimeRewardClick2()
    self:ComReadTimeRewardClick(2,self.ReadTimeReward2.gameObject.transform);
    self:SetKeyDiamondsText(2);
end
--endregion


--region【点击宝箱3】
function ReadTimePanel:OnReadTimeRewardClick3()
    self:ComReadTimeRewardClick(3,self.ReadTimeReward3.gameObject.transform);
    self:SetKeyDiamondsText(3);
end
--endregion


--region【点击宝箱4】
function ReadTimePanel:OnReadTimeRewardClick4()
    self:ComReadTimeRewardClick(4,self.ReadTimeReward4.gameObject.transform);
    self:SetKeyDiamondsText(4);
end
--endregion


--region【点击宝箱5】
function ReadTimePanel:OnReadTimeRewardClick5()
    self:ComReadTimeRewardClick(5,self.ReadTimeReward5.gameObject.transform);
    self:SetKeyDiamondsText(5);
end
--endregion


--region【展示TIPs】
local num=0;
local oldIndex=0;
function ReadTimePanel:ComReadTimeRewardClick(_index,transform)
    if(_index==nil and transform==nil)then
        num=num+1; if(num%2==0)then self.GiftTips:SetActive(false); end return;
    end

    if(oldIndex~=_index)then
        num=0;
    end
    oldIndex=_index;
    num=num+1;
    if(num%2==0)then
        self.GiftTips:SetActive(false);
    else
        self.GiftTips.transform:SetParent(transform);
        self.GiftTips:SetActive(true);
        self.GiftTips.transform.localPosition = core.Vector3.zero;
    end

end
--endregion


--region【展示奖励数量】
function ReadTimePanel:SetKeyDiamondsText(_index)
    local award= DataConfig.Q_AwardData:GetMapData(_index);
    if(award.key>0)then
        self.KeyGift:SetActive(true);
    else
        self.KeyGift:SetActive(false);
    end

    if(award.diamonds>0)then
        self.DimandGift:SetActive(true);
    else
        self.DimandGift:SetActive(false);
    end

    self.KeyGiftText.text="x"..tostring(award.key);
    self.DimandGiftText.text="x"..tostring(award.diamonds);
end
--endregion


--region【Go 去看书本按钮】
function ReadTimePanel:OnReadTimeGotoClick()
    if(GameHelper.CurBookId and GameHelper.CurBookId>0)then
        local bookinfo={};
        bookinfo.book_id=GameHelper.CurBookId;
        GameHelper.BookClick(bookinfo);

        --埋点*点击前往阅读
        logic.cs.GamePointManager:BuriedPoint("activity_cumulative_read_go");
    end
end
--endregion


--region【关闭奖励TIPs点击】
function ReadTimePanel:OnGiftTipCloseBtnClick()
    self:ComReadTimeRewardClick(nil,nil)
end
--endregion


--region【销毁】
function ReadTimePanel:__delete()
    --按钮监听
    logic.cs.UIEventListener.RemoveOnClickListener(self.ReadTimeGoto,function(data) self:OnReadTimeGotoClick() end)
    logic.cs.UIEventListener.RemoveOnClickListener(self.ReadTimeBtn,function(data) self:OnReadTimeClick() end)
    logic.cs.UIEventListener.RemoveOnClickListener(self.ReadTimeReward1.gameObject,function(data) self:OnReadTimeRewardClick1() end)
    logic.cs.UIEventListener.RemoveOnClickListener(self.ReadTimeReward2.gameObject,function(data) self:OnReadTimeRewardClick2() end)
    logic.cs.UIEventListener.RemoveOnClickListener(self.ReadTimeReward3.gameObject,function(data) self:OnReadTimeRewardClick3() end)
    logic.cs.UIEventListener.RemoveOnClickListener(self.ReadTimeReward4.gameObject,function(data) self:OnReadTimeRewardClick4() end)
    logic.cs.UIEventListener.RemoveOnClickListener(self.ReadTimeReward5.gameObject,function(data) self:OnReadTimeRewardClick5() end)

    self.TitleText = nil;
    self.DescriptionText = nil;
    self.ReadTimeRewardList = nil;
    self.ReadTimeReward1 = nil;
    self.ReadTimeReward2 = nil;
    self.ReadTimeReward3 = nil;
    self.ReadTimeReward4 = nil;
    self.ReadTimeReward5 = nil;
    self.ReadTimeBar = nil;
    self.ReadTimeGoto = nil;
    self.ReadTimeBtn = nil;
    self.CompletedBtn = nil;
    self.GiftTips = nil;

    self.gameObject = nil;
end
--endregion


return ReadTimePanel
