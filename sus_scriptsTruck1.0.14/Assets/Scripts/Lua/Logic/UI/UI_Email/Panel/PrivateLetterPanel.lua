
local PrivateLetterPanel = core.Class("PrivateLetterPanel")

local PrivateLetterItem = require('Logic/UI/UI_Email/Item/PrivateLetterItem');


local ItemList={};
function PrivateLetterPanel:__init(gameObject)

    self.MainContent = CS.DisplayUtil.GetChild(gameObject, "MainContent")
    self.UIVirtualList=self.MainContent.gameObject:GetComponent("UIVirtualList");

    --【Item刷新】
    self.UIVirtualList.onItemRefresh =PrivateLetterPanel.OnGetItemByRowColumn;
    self.UIVirtualList.scrollRect.onValueChanged:AddListener(function(value)
        self:OnBookScrollChanged(value)
    end)

    self.NoEmail =CS.DisplayUtil.GetChild(gameObject, "NoEmail");

    --【总编号】
    self.TotalCount=1;
    self.m_page = 0;
    --等待消息返回
    self.m_waitBookRefresh=false;
    --等待ui刷新
    self.m_waitUiRefresh=false;
end


function PrivateLetterPanel:UpdateGetPrivateLetterBoxList(page)

    if (page > 0)then
        self.m_page = page;
        self.m_waitBookRefresh = false;
    end

    self.TotalCount=table.length(Cache.EmailCache.PlayerChatList);

    if(self.TotalCount and self.TotalCount>0)then
        --【设置列表总数量】
        self.UIVirtualList:SetItemCount(self.TotalCount);
        self.NoEmail:SetActiveEx(false);
    else
        self.NoEmail:SetActiveEx(true);
    end
end

function PrivateLetterPanel.OnGetItemByRowColumn(row)
    local index=row.itemIndex+1;
    local trans = row.rect;

    if(trans)then

        if(Cache.EmailCache.PlayerChatList==nil)then return; end

        --【获取一组数据】
        local itemData =Cache.EmailCache.PlayerChatList[index];
        if(itemData == nil)then return nil; end

        --【GameObect唯一编号】
        local onlyID=trans.gameObject:GetInstanceID();

        --【书本 脚本】
        local Item = table.trygetvalue(ItemList,onlyID);
        if(Item==nil)then
            Item = PrivateLetterItem.New(trans.gameObject);
            ItemList[onlyID]=Item;
        end
        --【赋值】
        if(Item)then
            Item:SetItemData(itemData,index);
        end
    end
end

function PrivateLetterPanel:OnBookScrollChanged(value)
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
            --请求获取邮箱信息
            GameController.EmailControl:GetPrivateLetterBoxPageRequest(self.m_page+1);
        end
    end
end

function PrivateLetterPanel:UpdatePrivateLetter(page)




end


--销毁
function PrivateLetterPanel:__delete()
    self.UIVirtualList.onItemRefresh =nil;
    --【清除列表所有对象 和 脚本】
    if(ItemList)then
        for _key, _value in pairs(ItemList) do
            if(_value)then
                _value:Delete();--【销毁】
            end
        end
        ItemList={};
    end

    self.MainContent = nil;
    self.UIVirtualList.scrollRect.onValueChanged:RemoveAllListeners()
    self.UIVirtualList = nil;
    self.NoEmail = nil;
    self.TotalCount = nil;
    self.m_page = nil;
    self.m_waitBookRefresh = nil;
    self.m_waitUiRefresh = nil;
end



return PrivateLetterPanel
