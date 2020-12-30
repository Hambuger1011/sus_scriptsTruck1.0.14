local UIView = core.UIView

local UIChatForm = core.Class("UIChatForm", UIView)

local ChatItem = require('Logic/UI/UI_Chat/Item/ChatItem')

UIChatForm.config = {
    ID = logic.uiid.UIChatForm,
    AssetName = 'UI/Resident/UI/UIChatForm'
}

--region【Awake】

local this=nil;

--【总数量】
local allCount=0;

--【ChatItem脚本列表】
local ItemList={};
function UIChatForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.LoopListView2 = CS.DisplayUtil.GetChild(this.gameObject, "ScrollRect"):GetComponent("LoopListView2");
    --list插件
    self.LoopListView2:InitListView(allCount,UIChatForm.OnGetItemByRowColumn);

    self.LoopListView2.ScrollRect.onValueChanged:AddListener(function(value)
        self:OnBookScrollChanged(value)
    end)


    self.TopTile = CS.DisplayUtil.GetChild(this.gameObject, "TopTile");
    self.Bottom = CS.DisplayUtil.GetChild(this.gameObject, "Bottom");
    self.TopRect = self.TopTile:GetComponent("RectTransform");
    self.BottomRect = self.Bottom:GetComponent("RectTransform");


    self.InputField = CS.DisplayUtil.GetChild(self.Bottom, "InputField"):GetComponent("InputField");
    self.InputField.shouldHideMobileInput = true;
    self.CloseBtn = CS.DisplayUtil.GetChild(this.gameObject, "CloseBtn");
    self.PlayerName =CS.DisplayUtil.GetChild(self.TopTile , "PlayerName"):GetComponent("Text");

    self.SubmitBtn = CS.DisplayUtil.GetChild(self.Bottom, "SubmitBtn"):GetComponent("Image");
    self.SubmitText =CS.DisplayUtil.GetChild(self.SubmitBtn.gameObject, "SubmitText"):GetComponent("Text");
    self.SubmitNum =CS.DisplayUtil.GetChild(self.SubmitBtn.gameObject, "SubmitNum"):GetComponent("Text");

    self.SubmitBtnMask = CS.DisplayUtil.GetChild(self.Bottom, "SubmitBtnMask");

    logic.cs.UIEventListener.AddOnClickListener(self.SubmitBtn.gameObject,function(data) self:SubmitBtnClick(); end)
    logic.cs.UIEventListener.AddOnClickListener(self.CloseBtn,function(data) self:OnExitClick(); end)

    --对方uid
    self.Author_Uid=0;

    self.m_page = 0;
    --等待消息返回
    self.m_waitBookRefresh=false;
    --等待ui刷新
    self.m_waitUiRefresh=false;
end
--endregion


--region【OnOpen】

function UIChatForm:OnOpen()
    UIView.OnOpen(self)
    GameController.ChatControl:SetData(self);

    --【屏幕适配】
    local offect = CS.XLuaHelper.UnSafeAreaNotFit(self.uiform, nil, 750, 120);
    local size = self.TopRect.sizeDelta;
    size.y = size.y + offect;
    self.TopRect.sizeDelta = size;

    local size2 = self.BottomRect.sizeDelta;
    size2.y = size2.y + offect;
    self.BottomRect.sizeDelta=size2;
end

--endregion


--region 【OnClose】

function UIChatForm:OnClose()
    UIView.OnClose(self)
    GameController.ChatControl:SetData(nil);

    --【清除列表所有对象 和 脚本】
    if(ItemList)then
        for _key, _value in pairs(ItemList) do
            if(_value)then
                _value:Delete();--【销毁】
            end
        end
    end

    --重置页数
    GameController.ChatControl:Reset();

    if(self.LoopListView2)then
        self.LoopListView2.ScrollRect.onValueChanged:AddListener(function(value)
            self:OnBookScrollChanged(value);
        end)
    end

    --【删除所有服务器数据】
    Cache.ChatCache:ClearMas();

    if(self.CloseBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.CloseBtn,function(data) self:OnExitClick() end);
    end

    self.LoopListView2 = nil;
    self.Bottom = nil;
    self.InputField = nil;
    self.SubmitBtn = nil;
    self.CloseBtn = nil;
    self.Author_Uid = nil;
    self.m_page = nil;
    self.m_waitBookRefresh = nil;
    self.m_waitUiRefresh = nil;
    ItemList={};
    this=nil;
    allCount=0;
end

--endregion

--region【设置列表Item】
function UIChatForm:UpdateChatInfo(uid,page,nickname)
    self.Author_Uid=uid;

    if (page and page > 0)then
        self.m_page = page;
        self.m_waitBookRefresh = false;
    end

    if(Cache.ChatCache.ChatList==nil)then return; end
    allCount=table.length(Cache.ChatCache.ChatList);
    --【设置列表总数量】
    self.LoopListView2:SetListItemCount(allCount);

    local _index = allCount - 1;
    self:OnJumpToItem(_index);

    if(nickname)then
        self.PlayerName.text=nickname;
    end

    --同步发送信鸽次数
    self:UpdateSendCount()
end
--endregion


--region【刷新数据】
function UIChatForm.OnGetItemByRowColumn(listView,index)

    if (index < 0)then
        return nil;
    end

    local _index=index+1;

    local itemData = nil;
    if(Cache.ChatCache.ChatList)then
        itemData=Cache.ChatCache.ChatList[_index];
    end

    if(itemData == nil)then return nil; end


    local item=nil;
    if(itemData.is_self==0)then
        item = listView:NewListViewItem("ChatItem1");  --【左边其他人】
    elseif(itemData.is_self==1)then
        item = listView:NewListViewItem("ChatItem2");  --【右边我自己】
    end

    --【GameObect唯一编号】
    local onlyID=item.gameObject:GetInstanceID();

    --【书本 脚本】
    local chatItem = table.trygetvalue(ItemList,onlyID);
    if(chatItem==nil)then
        chatItem = ChatItem.New(item.gameObject);
        ItemList[onlyID]=chatItem;
    end

    if (item.IsInitHandlerCalled == false)then
        item.IsInitHandlerCalled = true;
    end

    --【赋值】
    if(chatItem)then
        chatItem:SetItemData(itemData,index);
    end
    return item;
end

--endregion


--region 【重新刷新列表所有Item】【位置计算】
function UIChatForm:Compute()
    self.LoopListView2:RefreshAllShownItem();
end
--endregion


--region【发送消息】

function UIChatForm:SubmitBtnClick()

    if(self.InputField==nil or self.InputField.text==nil or self.InputField.text=="" or self.Author_Uid==nil)then
        logic.cs.UITipsMgr:PopupTips("This cannot be left empty.", false);
        return;
    end
    GameController.ChatControl:SendWriterLetterRequest(self.Author_Uid,self.InputField.text);

end

--endregion


--region 【跳转到指定Item】

function UIChatForm:OnJumpToItem(itemIndex)
    if(self.LoopListView2==nil)then return; end
    if(itemIndex < 0)then return nil; end
    self.LoopListView2:MovePanelToItemIndex(itemIndex, 0);
end

--endregion


function UIChatForm:OnBookScrollChanged(value)
    if(self.m_waitBookRefresh==true)then
        return;
    end
    if(self.m_waitUiRefresh==true)then
        if (value.y >= 0.1)then
            self.m_waitUiRefresh = false;
        end

    else
        if (value.y < 0.1)then
            self.m_waitUiRefresh = true;--等待ui刷新
            self.m_waitBookRefresh = true;--等待消息返回

            if(self.Author_Uid==nil)then return; end
            GameController.ChatControl:GetPrivateLetterPageRequest(self.Author_Uid,self.m_page+1);
        end
    end
end


function UIChatForm:UpdateSendCount()

    --【发送信息 总次数】
    local SendChatAll= Cache.ChatCache.total;
    if(SendChatAll>0)then
        self.SubmitBtn.gameObject:SetActive(true)
        local FreeCount= Cache.ChatCache.free;
        if(FreeCount>0)then
            self.SubmitText.text="Free";
            self.SubmitNum.text=FreeCount.."/5";
        else
            local backpack= Cache.ChatCache.backpack;
            self.SubmitText.text="Send";
            self.SubmitNum.text=tostring(backpack);
        end
    else
        self.SubmitBtn.gameObject:SetActive(false);
        self.SubmitBtnMask:SetActive(true);
    end


end



--region 【界面关闭】
function UIChatForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIChatForm