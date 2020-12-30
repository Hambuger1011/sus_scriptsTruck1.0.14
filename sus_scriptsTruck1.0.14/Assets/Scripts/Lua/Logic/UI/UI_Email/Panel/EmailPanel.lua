local EmailPanel = core.Class("EmailPanel")

local EmailItem = require('Logic/UI/UI_Email/Item/EmailItem');


local ItemList={};
function EmailPanel:__init(gameObject)
    self.gameObject=gameObject;

    self.ScrollRect =CS.DisplayUtil.GetChild(gameObject, "ScrollRect");

    self.UIVirtualList =CS.DisplayUtil.GetChild(self.ScrollRect, "EmailList"):GetComponent("UIVirtualList");
    --【Item刷新】
    self.UIVirtualList.onItemRefresh =EmailPanel.OnGetItemByRowColumn;
    self.UIVirtualList.scrollRect.onValueChanged:AddListener(function(value)
        self:OnBookScrollChanged(value)
    end)

    self.NoEmail =CS.DisplayUtil.GetChild(gameObject, "NoEmail");
    self.bookId = logic.bookReadingMgr.selectBookId

    --【总编号】
    self.TotalCount=1;
    self.m_page = 0;
    --等待消息返回
    self.m_waitBookRefresh=false;
    --等待ui刷新
    self.m_waitUiRefresh=false;

    --请求获取邮箱信息
    GameController.EmailControl:GetSystemMsgRequest(1);
end



function EmailPanel:UpdateEmail(page)

    if (page > 0)then
        self.m_page = page;
        self.m_waitBookRefresh = false;
    end

    self.TotalCount=table.length(Cache.EmailCache.EmailList);

    if(self.TotalCount and self.TotalCount>0)then
        --【设置列表总数量】
        self.UIVirtualList:SetItemCount(self.TotalCount);
        self.NoEmail:SetActiveEx(false);
    else
        self.UIVirtualList:SetItemCount(0);
        self.NoEmail:SetActiveEx(true);
    end
end




function EmailPanel.OnGetItemByRowColumn(row)
    local index=row.itemIndex+1;
    local trans = row.rect;

    if(trans)then

        --【获取一组数据】
        local itemData =Cache.EmailCache.EmailList[index];
        if(itemData == nil)then return nil; end

        --【GameObect唯一编号】
        local onlyID=trans.gameObject:GetInstanceID();

        --【书本 脚本】
        local Item = table.trygetvalue(ItemList,onlyID);
        if(Item==nil)then
            Item = EmailItem.New(trans.gameObject);
            ItemList[onlyID]=Item;
        end

        --【赋值】
        if(Item)then
            Item:SetItemData(itemData,index);
        end
    end

end

function EmailPanel:OnBookScrollChanged(value)
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
            GameController.EmailControl:GetSystemMsgRequest(self.m_page+1);
        end
    end
end




function EmailPanel:SelectAll(show,isResetAll)

    if(ItemList)then
        local len = table.count(ItemList);
        if(len and len>0)then
            for k,_value in pairs(ItemList) do
                if(_value)then
                    if(isResetAll==true)then
                        _value.SelectTab.isOn=false;
                    end
                    _value.SelectTab.gameObject:SetActiveEx(show);
                end
            end
        end
    end
end

--region【领取奖励】
function EmailPanel:AchieveMsgPrice(msgid)
    if(ItemList)then
        local len = table.count(ItemList);
        if(len)then
            for k,_value in pairs(ItemList) do
                if(_value and _value.Info)then
                    if(_value.Info.msgid==msgid)then
                        local info = Cache.EmailCache:GetInfoById(msgid);
                        _value:SetItemData(info);
                        return;
                    end
                end
            end
        end
    end
end
--endregion




--销毁
function EmailPanel:__delete()
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

    self.UIVirtualList.scrollRect.onValueChanged:RemoveAllListeners();
    self.TotalCount = nil;
    self.m_page = nil;
    self.m_waitBookRefresh = nil;
    self.m_waitUiRefresh = nil;
    self.ScrollRect = nil;
    self.UIVirtualList = nil;
    self.NoEmail = nil;
    self.bookId = nil;
    self.gameObject = nil;
end

return EmailPanel
