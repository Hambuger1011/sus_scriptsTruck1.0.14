local BaseClass = core.Class
local EmailPanel = BaseClass("EmailPanel")

local UIBubbleItem = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleItem')
local UIDialogList = require('Logic/StoryEditor/UI/Utils/UIDialogList')
local UIBubbleBox = require('Logic/StoryEditor/UI/Preview/BubbleItem/UIBubbleBox')

local pageNum = 1
function EmailPanel:__init(gameObject)
    self.gameObject=gameObject;

    self.ScrollView =CS.DisplayUtil.GetChild(gameObject, "ScrollView");
    self.Layout =CS.DisplayUtil.GetChild(self.ScrollView, "Layout");
    self.itemListView = UIDialogList.New(self.Layout,5)
    self.EmailItem =CS.DisplayUtil.GetChild(self.ScrollView, "EmailItem");
    self.NoEmail =CS.DisplayUtil.GetChild(gameObject, "NoEmail");

    self.paddingBottom = self.itemListView:GetPaddingBottom();
    self.itemListView.onCreateItem = function(index, data, reset)
        return self:OnCreateItem(index, data, reset);
    end

    self.bookId = logic.bookReadingMgr.selectBookId
    pageNum = 1;

    --请求获取邮箱信息
    GameController.EmailControl:GetSystemMsgRequest(pageNum);
    self.EmailItem:SetActiveEx(false);
    self.InfoList=nil;
end

function EmailPanel:UpdateEmail(_pageNum)
    if(_pageNum == 1)then
        --销毁所有Item
        self:ClearItem();

        self.InfoList=Cache.EmailCache.EmailList;

        if(self.InfoList==nil and #self.InfoList == 0)then
            self.NoEmail.gameObject:SetActiveEx(true);
            return;
        end


        for i = 1 , #self.InfoList do
            if i == #self.InfoList then
                self.InfoList[i].pageNum = pageNum
                self.InfoList[i].GetNextPage = function()
                    pageNum = pageNum + 1
                    --请求获取邮箱信息
                    GameController.EmailControl:GetSystemMsgRequest(pageNum);
                end
            end
            self:AddUIItem(false,false,false)
        end
        self.itemListView:MoveToIndex(1)
    else
        local newData = Cache.EmailCache.EmailList;
        for i = 1 , #newData do
            if i == #newData then
                newData[i].pageNum = pageNum
                newData[i].GetNextPage = function()
                    pageNum = pageNum + 1
                    --请求获取邮箱信息
                    GameController.EmailControl:GetSystemMsgRequest(pageNum);
                end
            end
            table.insert(self.InfoList,newData[i])
            self:AddUIItem(true,false,false)
        end
    end

end

function EmailPanel:AddUIItem(newInstance,useTween,scrollToBottom)
    local height = self.itemListView:AddVirtualItem(UIBubbleBox.BoxType.EmailItem,newInstance)
    self.itemListView:SetHeight(height, scrollToBottom, useTween)
    self:RefreshDialogList()
end

function EmailPanel:RefreshDialogList()
    self.itemListView:MarkDirty()
end

function EmailPanel:OnCreateItem(index, itemData)
    local go = logic.cs.GameObject.Instantiate(self.EmailItem,self.itemListView.transform)
    go:SetActiveEx(true)

    local item = UIBubbleItem.New(go,UIBubbleBox.BoxType.EmailItem)
    item.GetBubbleDataByIndex = function(index)

        if(self.InfoList==nil)then
            return nil;
        end

        return self.InfoList[index]
    end
    return item
end

function EmailPanel:ClearItem()
    self.itemListView:ClearItem()
    self.itemListView:SetPaddingBottom(self.paddingBottom)
    self.itemDataList = nil;
end


--销毁
function EmailPanel:__delete()

end

return EmailPanel
