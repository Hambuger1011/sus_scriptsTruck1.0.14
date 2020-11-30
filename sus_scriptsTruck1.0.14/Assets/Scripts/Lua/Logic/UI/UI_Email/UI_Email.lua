local BaseClass = core.Class
local UIView = core.UIView
---@class UI_Email
local UI_Email = BaseClass("UI_Email", UIView)
local base = UIView

local uiid = logic.uiid
local UIBubbleEvent = CS.UI.UIBubbleEvent

local BubbleData = require('Logic/StoryEditor/UI/Preview/BubbleItem/BubbleData')
local UIBubbleItem = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleItem')
local UIDialogList = require('Logic/StoryEditor/UI/Utils/UIDialogList')
local UIBubbleBox = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleBox')

UI_Email.config = {
    ID = uiid.Email,
    AssetName = 'UI/Resident/UI/Canvas_NewEmail'
}

local pageNum = 1



function UI_Email:OnInitView()
    self.bookId = logic.bookReadingMgr.selectBookId
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.EmailItem = self.uiBinding:Get('EmailItem')
    self.itemListView = UIDialogList.New(self.uiBinding:Get('Layout'),5)
    self.closeButton = self.uiBinding:Get('CloseButton', typeof(logic.cs.Button))
    self.contactUsButton = self.uiBinding:Get('ContactUsButton', typeof(logic.cs.Button))
    self.Center = self.uiBinding:Get('Center')
    self.NoEmail = self.uiBinding:Get('NoEmail')
    
    
    self.EmailItem:SetActiveEx(false)
    self.paddingBottom = self.itemListView:GetPaddingBottom()
    self.itemListView.onCreateItem = function(index, data, reset)
        return self:OnCreateItem(index, data, reset)
    end
    self.closeButton.onClick:RemoveAllListeners()
    self.closeButton.onClick:AddListener(function()
        self:OnExitClick()
    end)
    self.contactUsButton.onClick:RemoveAllListeners()
    self.contactUsButton.onClick:AddListener(function()
        logic.cs.IGGSDKMrg:OpenTSH()
    end)

    pageNum = 1
    self:UpdateEmail(pageNum)


    local uiform1 = root:GetComponent("CUIForm")
    local safeArea = logic.cs.ResolutionAdapter:GetSafeArea()
    local safeAreaHeight = uiform1:yPixel2View(safeArea.y)
    if safeAreaHeight and safeAreaHeight > 0 then
        local TopTile = root:Find('Canvas/TopTile').transform
        local Center = root:Find('Canvas/Center').transform

        local pos = TopTile.anchoredPosition
        pos.y = pos.y - safeAreaHeight
        TopTile.anchoredPosition = pos

        local w = Center.rect.width
        local h = Center.rect.height-safeAreaHeight
        Center.anchorMax = core.Vector2.New(0.5, 1);
        Center.anchorMin = core.Vector2.New(0.5, 1);
        Center.pivot = core.Vector2.New(0.5, 1);
        Center.sizeDelta =  {x=w,y=h}
        local pos1 = Center.anchoredPosition
        pos1.y = pos1.y - safeAreaHeight*1.5
        Center.anchoredPosition = pos1
    end

end

function UI_Email:UpdateEmail(_pageNum)
    logic.gameHttp:GetSystemMsg(_pageNum,function(result)
        logic.debug.Log("----GetSystemMsg---->" .. result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            if _pageNum == 1 then
                self:ClearItem()
                self.itemDataList =  json.data.sysarr
                if #self.itemDataList == 0 then
                    self.Center.gameObject:SetActiveEx(false)
                    self.NoEmail.gameObject:SetActiveEx(true)
                end
                for i = 1 , #self.itemDataList do
                    if i == #self.itemDataList then
                        self.itemDataList[i].pageNum = pageNum
                        self.itemDataList[i].GetNextPage = function()
                            pageNum = pageNum + 1
                            self:UpdateEmail(pageNum)
                        end
                    end
                    self:AddUIItem(false,false,false)
                end
                self.itemListView:MoveToIndex(1)
            else
                local newData = json.data.sysarr
                for i = 1 , #newData do
                    if i == #newData then
                        newData[i].pageNum = pageNum
                        newData[i].GetNextPage = function()
                            pageNum = pageNum + 1
                            self:UpdateEmail(pageNum)
                        end
                    end
                    table.insert(self.itemDataList,newData[i])
                    self:AddUIItem(true,false,false)
                end
            end

        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            self:OnExitClick()
        end
    end)
end

function UI_Email:AddUIItem(newInstance,useTween,scrollToBottom)
    local height = self.itemListView:AddVirtualItem(UIBubbleBox.BoxType.EmailItem,newInstance)
    self.itemListView:SetHeight(height, scrollToBottom, useTween)
    self:RefreshDialogList()
end

function UI_Email:RefreshDialogList()
    self.itemListView:MarkDirty()
end

function UI_Email:OnCreateItem(index, itemData)
    local go = logic.cs.GameObject.Instantiate(self.EmailItem,self.itemListView.transform)
    go:SetActiveEx(true)

    local item = UIBubbleItem.New(go,UIBubbleBox.BoxType.EmailItem)
    item.GetBubbleDataByIndex = function(index)
        return self.itemDataList[index]
    end
    return item
end

function UI_Email:ClearItem()
    self.itemListView:ClearItem()
    self.itemListView:SetPaddingBottom(self.paddingBottom)
    self.itemDataList = {}
end

function UI_Email:OnOpen()
    UIView.OnOpen(self)
end

function UI_Email:OnClose()
    UIView.OnClose(self)
end

function UI_Email:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UI_Email