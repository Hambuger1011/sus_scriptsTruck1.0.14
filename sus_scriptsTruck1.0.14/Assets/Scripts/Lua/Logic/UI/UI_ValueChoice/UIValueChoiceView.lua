local BaseClass = core.Class
local UIView = core.UIView
local UIValueChoiceView = BaseClass("UIValueChoiceView", UIView)
local base = UIView

local uiid = logic.uiid
UIValueChoiceView.config = {
	ID = uiid.ValueChoice,
	AssetName = 'UI/Resident/UI/Canvas_ValueChoice',
}

function UIValueChoiceView:OnInitView()
    UIView.OnInitView(self)
    local get = logic.cs.LuaHelper.GetComponent
    local root = self.uiform.transform

    self.ChoiceRole=nil
    self.iSbuttonOn = 0

    self.btnPay = get(root,'Canvas/Mask/BG/PayButton',typeof(logic.cs.UITweenButton))
    self.btnWatchAds = get(root,'Canvas/Mask/BG/WatchButton',typeof(logic.cs.UITweenButton))
    self.btnClose = get(root,'Canvas/Mask/Close',typeof(logic.cs.UITweenButton))
    self.txtVideoNum = get(root,'Canvas/Mask/BG/Advertising/txtVideocount',typeof(logic.cs.Text))
    self.txtCost = get(root,'Canvas/Mask/BG/PayButton/Text',typeof(logic.cs.Text))
    self.txtTitle = get(root,'Canvas/Mask/BG/Title',typeof(logic.cs.Text))

    
    self.imgDiamonds1 = get(root,'Canvas/Mask/BG/Diamonds',typeof(logic.cs.Image))
    self.imgDiamonds2 = get(root,'Canvas/Mask/BG/PayButton/Image',typeof(logic.cs.Image))
    self.txtDiamondsName = get(root,'Canvas/Mask/BG/Diamonds/Text',typeof(logic.cs.Text))

    self.goVideo1 = root:Find('Canvas/Mask/BG/Advertising').gameObject
    self.goVideo2 = root:Find('Canvas/Mask/BG/WatchButton').gameObject

    self.btnPay.onClick:AddListener(function()
        self:OnPayClick()
    end)
    self.btnWatchAds.onClick:AddListener(function()
        self:OnWatchAdsClick()
    end)
    self.btnClose.onClick:AddListener(function()
        self:__Close()
    end)
end

function UIValueChoiceView:OnOpen()
    UIView.OnOpen(self)
    if logic.bookReadingMgr and logic.bookReadingMgr.view then
        logic.bookReadingMgr.view:StopOperationTips()
    end
end

function UIValueChoiceView:OnClose()

    if self.ChoiceRole ~=nil and self.iSbuttonOn == 0 then
        self.iSbuttonOn = 0
        self.ChoiceRole.ConfirmMask.gameObject:SetActiveEx(false)
    end

    UIView.OnClose(self)
    if logic.bookReadingMgr and logic.bookReadingMgr.view then
        logic.bookReadingMgr.view:ResetOperationTips()
    end
end


function UIValueChoiceView:SetData(type,adsExtra,cost,callback)
    self.type = type
    self.adsExtra = adsExtra
    self.buyType = 1
    self.cost = cost
    self.callback = callback
    self:RefreshUI()
end

function UIValueChoiceView:ChoiceRoleGo(ChoiceRole)
    self.ChoiceRole=ChoiceRole
end


function UIValueChoiceView:RefreshUI()
    self.txtVideoNum.text = tostring(logic.cs.UserDataManager.userInfo.data.userinfo.videocount)

    if logic.cs.UserDataManager.userInfo.data.userinfo.videocount <= 0 then
        logic.cs.LuaHelper.SetUIGrey(self.goVideo1,true)
        logic.cs.LuaHelper.SetUIGrey(self.goVideo2,true)
    end

    if self.type == 2 then
        local icon1 = logic.cs.ResourceManager:GetUISprite("ValueChoice/bg_keys")
        local icon2 = logic.cs.ResourceManager:GetUISprite("ValueChoice/icon_keys")

        self.imgDiamonds1.sprite = icon1
        self.imgDiamonds2.sprite = icon2
        if self.cost > 1 then
            self.txtTitle.text = string.format('Pay %d keys or watch ads to unlock!',self.cost)
            self.txtDiamondsName.text = 'Keys'
        else
            self.txtTitle.text = string.format('Pay %d key or watch ads to unlock!',self.cost)
            self.txtDiamondsName.text = 'Key'
        end
    else
        if self.cost > 1 then
            self.txtTitle.text = string.format('Pay %d diamonds or watch ads to unlock!',self.cost)
            self.txtDiamondsName.text = 'Diamonds'
        else
            self.txtTitle.text = string.format('Pay %d diamond or watch ads to unlock!',self.cost)
            self.txtDiamondsName.text = 'Diamond'
        end
    end
    self.txtCost.text = 'PAY  '.. tostring(self.cost)
end

function UIValueChoiceView:Complete()
    self:__Close()
    self.callback(self.buyType)
end

function UIValueChoiceView:OnPayClick()
    self.buyType = 1
    self.iSbuttonOn = 1
    self:Complete()
end

function UIValueChoiceView:OnWatchAdsClick()
    if logic.cs.UserDataManager.userInfo.data.userinfo.videocount <= 0 then
        return
    end
    self.buyType = 2
    self.iSbuttonOn = 0
    logic.cs.SdkMgr.ads:ShowRewardBasedVideo(self.adsExtra,function(success)

        if not success then
            return
        end
        self.iSbuttonOn = 1    
        self:Complete()
    end)
end

return UIValueChoiceView