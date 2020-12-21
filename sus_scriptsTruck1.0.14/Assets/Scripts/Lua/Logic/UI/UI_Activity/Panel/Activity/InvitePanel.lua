local BaseClass = core.Class
local UIView = core.UIView
---@class InvitePanel
local InvitePanel = BaseClass("InvitePanel", UIView)
local uiid = logic.uiid

InvitePanel.config = {
    ID = uiid.InvitePanel,
    AssetName = 'UI/Resident/UI/Canvas_InvitePanel'
}

function InvitePanel:OnInitView()
    UIView.OnInitView(self)
    local this = self.uiform
    self.closeButton = CS.DisplayUtil.GetChild(this.gameObject, "CloseButton")
    self.Item = CS.DisplayUtil.GetChild(this.gameObject, "Item")
    self.Content = CS.DisplayUtil.GetChild(this.gameObject, "Content")
    logic.cs.UIEventListener.AddOnClickListener(self.closeButton, function(data)
        self:OnExitClick()
    end)

    self:SetUI()
end

function InvitePanel:SetUI()
    logic.gameHttp:GetInviteList(function(result)
        if string.IsNullOrEmpty(result) then
            return
        end
        logic.debug.Log("----GetInviteList---->" .. result)
        local json = core.json.Derialize(result);
        local code = tonumber(json.code);
        if code == 200 then
            local InviteData = json.data
            for i, v in pairs(InviteData) do
                local Item = logic.cs.GameObject.Instantiate(self.Item, self.Content.transform)
                local NumText =CS.DisplayUtil.GetChild(Item, "NumText"):GetComponent(typeof(logic.cs.Text));
                local Name =CS.DisplayUtil.GetChild(Item, "Name"):GetComponent(typeof(logic.cs.Text));
                local DiamondNum =CS.DisplayUtil.GetChild(Item, "DiamondNum"):GetComponent(typeof(logic.cs.Text));
                local InviteButton =CS.DisplayUtil.GetChild(Item, "InviteButton"):GetComponent(typeof(logic.cs.Button));
                local CollectedButton =CS.DisplayUtil.GetChild(Item, "CollectedButton"):GetComponent(typeof(logic.cs.Button));
                local Collected =CS.DisplayUtil.GetChild(Item, "Collected"):GetComponent(typeof(logic.cs.Button));
                local headImage =CS.DisplayUtil.GetChild(Item, "HeadImage"):GetComponent(typeof(logic.cs.Image));
                local HeadFrame =CS.DisplayUtil.GetChild(Item, "HeadFrame"):GetComponent(typeof(logic.cs.Image));

                InviteButton.gameObject:SetActiveEx(false)
                CollectedButton.gameObject:SetActiveEx(false)
                Collected.gameObject:SetActiveEx(false)
                if v.status == 0 then
                    InviteButton.gameObject:SetActiveEx(true)
                    InviteButton.onClick:RemoveAllListeners()
                    InviteButton.onClick:AddListener(function()
                        --logic.cs.UITipsMgr:ShowTips("coming soon");
                        logic.cs.SdkMgr.shareSDK:ShareMsg("http://192.168.0.33/InvitationPage?invite_code="..logic.cs.UserDataManager.userInfo.data.userinfo.invite_code);
                    end)
                elseif v.status == 1 then
                    CollectedButton.gameObject:SetActiveEx(true)
                    CollectedButton.onClick:RemoveAllListeners()
                    CollectedButton.onClick:AddListener(function()
                        logic.gameHttp:ReceiveInvitePrize(v.number, function(result)
                            local json = core.json.Derialize(result)
                            local code = tonumber(json.code)
                            if code == 200 then
                                CollectedButton.gameObject:SetActiveEx(false)
                                Collected.gameObject:SetActiveEx(true)
                                logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
                                logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))
                            end
                        end)
                    end)
                elseif v.status == 2 then
                    Collected.gameObject:SetActiveEx(true)
                end
                GameHelper.luaShowDressUpForm(v.user_info.avatar, headImage, DressUp.Avatar, 1001);
                --GameHelper.luaShowDressUpForm(v.user_info.avatar_frame, HeadFrame, DressUp.AvatarFrame, 2001);
                NumText.text = v.number
                Name.text = v.user_info.nickname
                DiamondNum.text = "x"..v.diamond_count
                Item:SetActiveEx(true)
            end
        end
    end)

end

function InvitePanel:OnOpen()
    UIView.OnOpen(self)
end

function InvitePanel:OnClose()
    UIView.OnClose(self)
end

function InvitePanel:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return InvitePanel