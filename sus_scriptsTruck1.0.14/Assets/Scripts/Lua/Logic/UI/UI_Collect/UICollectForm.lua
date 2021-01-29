local BaseClass = core.Class
local UIView = core.UIView
---@class UICollectForm
local UICollectForm = BaseClass("UICollectForm", UIView)
local uiid = logic.uiid
local diamondsNum,keysNum
local needX2 = false
local PosList = {
    {core.Vector2.New(375,-300)},
    {core.Vector2.New(280,-300),core.Vector2.New(470,-300)},
    {core.Vector2.New(185,-300),core.Vector2.New(375,-300),core.Vector2.New(565,-300)},
    {core.Vector2.New(280,-205),core.Vector2.New(470,-205),core.Vector2.New(280,-395),
     core.Vector2.New(470,-395)},
    {core.Vector2.New(185,-205),core.Vector2.New(375,-205),core.Vector2.New(565,-205),
     core.Vector2.New(280,-395),core.Vector2.New(470,-395)},
    {core.Vector2.New(185,-205),core.Vector2.New(375,-205),core.Vector2.New(565,-205),
     core.Vector2.New(185,-395),core.Vector2.New(375,-395),core.Vector2.New(565,-395)},
    {core.Vector2.New(185,-110),core.Vector2.New(375,-110),core.Vector2.New(565,-110),
     core.Vector2.New(185,-300),core.Vector2.New(375,-300),core.Vector2.New(565,-300),
     core.Vector2.New(375,-490)},
    {core.Vector2.New(185,-110),core.Vector2.New(375,-110),core.Vector2.New(565,-110),
     core.Vector2.New(185,-300),core.Vector2.New(375,-300),core.Vector2.New(565,-300),
     core.Vector2.New(280,-490),core.Vector2.New(470,-490)},
    {core.Vector2.New(185,-110),core.Vector2.New(375,-110),core.Vector2.New(565,-110),
     core.Vector2.New(185,-300),core.Vector2.New(375,-300),core.Vector2.New(565,-300),
     core.Vector2.New(185,-490),core.Vector2.New(375,-490),core.Vector2.New(565,-490)},
}
local RewardTrans = {}


UICollectForm.config = {
    ID = uiid.UICollectForm,
    AssetName = 'UI/Resident/UI/UICollectForm'
}

function UICollectForm:OnInitView()
    UIView.OnInitView(self)
    local this=self.uiform
    self.Item =CS.DisplayUtil.GetChild(this.gameObject, "Item")
    self.LayoutGroup =CS.DisplayUtil.GetChild(this.gameObject, "Layout Group").transform
    self.CLAIM =CS.DisplayUtil.GetChild(this.gameObject, "CLAIM")
    self.CLAIMText = CS.DisplayUtil.GetChild(this.gameObject, "CLAIMText"):GetComponent(typeof(logic.cs.Text))
    self.CLAIMAd =CS.DisplayUtil.GetChild(this.gameObject, "CLAIMAd")
    self.CLAIMAdText = CS.DisplayUtil.GetChild(this.gameObject, "CLAIMAdText"):GetComponent(typeof(logic.cs.Text))
    self.CLAIMShare =CS.DisplayUtil.GetChild(this.gameObject, "CLAIMShare")
    self.CLAIMShareText = CS.DisplayUtil.GetChild(this.gameObject, "CLAIMShareText"):GetComponent(typeof(logic.cs.Text))
    self.CloseButton = CS.DisplayUtil.GetChild(this.gameObject, "CloseButton"):GetComponent(typeof(logic.cs.Button))
    self.Title =CS.DisplayUtil.GetChild(this.gameObject, "Title")
end

function UICollectForm:SetData(_diamondsNum,_keysNum,_CLAIMBtnText,_CLAIMCallback, _itemData, _AdBtnText, _AdBtnCallBack, _ShareBtnText, _ShareBtnCallBack)
    RewardTrans = {}
    if(_diamondsNum and tonumber(_diamondsNum) > 0)then
        local item = logic.cs.GameObject.Instantiate(self.Item,self.LayoutGroup,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".._diamondsNum;
        Icon.sprite = Cache.PropCache.SpriteData[1]
        Icon:SetNativeSize()
        Icon.transform.localScale = core.Vector3.New(0.8,0.8,1)
        table.insert(RewardTrans,item)
    end
    if(_keysNum and tonumber(_keysNum) > 0)then
        local item = logic.cs.GameObject.Instantiate(self.Item,self.LayoutGroup,false)
        local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
        local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
        Num.text = "x".._keysNum;
        Icon.sprite = Cache.PropCache.SpriteData[2]
        Icon:SetNativeSize()
        Icon.transform.localScale = core.Vector3.New(0.8,0.8,1)
        table.insert(RewardTrans,item)
    end
    if _itemData then
        for k, v in pairs(_itemData) do
            local item = logic.cs.GameObject.Instantiate(self.Item,self.LayoutGroup,false)
            local Num = CS.DisplayUtil.GetChild(item, "Num"):GetComponent(typeof(logic.cs.Text))
            local Icon = CS.DisplayUtil.GetChild(item, "Icon"):GetComponent(typeof(logic.cs.Image))
            Num.text = "x".. v.num;
            if 1000<tonumber(v.id) and tonumber(v.id)<10000 then
                local sprite=DataConfig.Q_DressUpData:GetSprite(v.id)
                Icon.sprite = sprite
                Icon:SetNativeSize()
                Icon.transform.localScale = core.Vector3.New(0.4,0.4,1)
            else
                Icon.sprite = Cache.PropCache.SpriteData[v.id]
                Icon:SetNativeSize()
                Icon.transform.localScale = core.Vector3.New(0.5,0.5,1)
            end
            table.insert(RewardTrans,item)
        end
    end
    
    if _CLAIMBtnText  then
        logic.cs.UIEventListener.AddOnClickListener(self.CLAIM,function(data)
            _CLAIMCallback();
            self:OnExitClick();
        end)
        self.CLAIMText.text = tostring(_CLAIMBtnText)
        self.CLAIM.gameObject:SetActiveEx(true);
    else
        self.CLAIM.gameObject:SetActiveEx(false);
    end
    
    if _AdBtnText  then
        logic.cs.UIEventListener.AddOnClickListener(self.CLAIMAd,function(data)
            _AdBtnCallBack();
            self:OnExitClick();
        end)
        self.CLAIMAdText.text = tostring(_AdBtnText)
        self.CLAIMAd.gameObject:SetActiveEx(true);
    else
        self.CLAIMAd.gameObject:SetActiveEx(false);
    end
    
    if _ShareBtnText then
        logic.cs.UIEventListener.AddOnClickListener(self.CLAIMShare,function(data)
            _ShareBtnCallBack();
            self:OnExitClick();
        end)
        self.CLAIMShareText.text = tostring(_ShareBtnText)
        self.CLAIMShare.gameObject:SetActiveEx(true);
    else
        self.CLAIMShare.gameObject:SetActiveEx(false);
    end

    self.CloseButton.onClick:RemoveAllListeners()
    self.CloseButton.onClick:AddListener(function()
        self:OnExitClick()
    end)
    
    self:PlayAnim()
end

function UICollectForm:CsRewardedAd_Chapter()
    local chapterSwitch = logic.UIMgr:GetView2(logic.uiid.BookReading).chapterSwitch.ui.gameObject:GetComponent("UIChapterSwitch");
    local bookData = logic.bookReadingMgr.bookData
    self:SetData(1,0,nil,nil,nil,
            "CLAIM",function ()
                --CS.GoogleAdmobAds.Instance.acitityRewardedAd:ShowRewardedAd(function()
                    logic.gameHttp:GetChapterAdsReward(
                            bookData.BookID,
                            bookData.ChapterID,
                            function(result)
                                local jsonData = core.json.Derialize(result)
                                local code = jsonData.code
                                if code == 200 then
                                    if jsonData and jsonData.data then
                                        if jsonData.data.bkey then
                                            logic.cs.UserDataManager:ResetMoney(1, tonumber(jsonData.data.bkey))
                                        end
                                        if jsonData.data.diamond then
                                            logic.cs.UserDataManager:ResetMoney(2, tonumber(jsonData.data.diamond))
                                        end
                                    end
                                end
                            end);
                --end);
            end,
            "CLAIM",function ()
                local linkUrl = "https://play.google.com/store/apps/details?id=" .. CS.SdkMgr.packageName
                if core.config.os == OS.iOS then
                    linkUrl = "https://www.facebook.com/Scripts-Untold-Secrets-107729237761206/"
                end
                local picUrl = logic.cs.GameHttpNet:GetResourcesUrl() .. "image/book_banner/" .. bookData.BookID .. ".jpg";
                chapterSwitch:FBShareLink(linkUrl, "Scripts: Untold Secrets", "Welcome to Scripts", picUrl)
            end)
end

function UICollectForm:PlayAnim()
    local size = #RewardTrans > #PosList and #PosList or #RewardTrans
    for i = 1, size do
        RewardTrans[i].transform.anchoredPosition =PosList[size][i]
        RewardTrans[i]:SetActiveEx(true)
        --RewardTrans[i].transform:DOAnchorPos(PosList[#RewardTrans][i],1):SetEase(core.tween.Ease.Flash):OnComplete(function()  end)
    end
    --coroutine.start(function()
    --    coroutine.wait(0.5) 
    --    self.Title.transform:DORotate( core.Vector3(0,0,0),1)
    --end)
end

function UICollectForm:OnOpen()
    UIView.OnOpen(self)
end

function UICollectForm:OnClose()
    UIView.OnClose(self)
end

function UICollectForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UICollectForm