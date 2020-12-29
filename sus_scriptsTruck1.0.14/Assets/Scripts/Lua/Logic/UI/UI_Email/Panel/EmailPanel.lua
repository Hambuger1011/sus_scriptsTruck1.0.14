local EmailPanel = core.Class("EmailPanel")

local EmailItem = require('Logic/UI/UI_Email/Item/EmailItem');

local pageNum = 1
local ItemList={};

local allCount=0;
function EmailPanel:__init(gameObject)
    self.gameObject=gameObject;

    self.ScrollRect =CS.DisplayUtil.GetChild(gameObject, "ScrollRect");

    self.UIVirtualList =CS.DisplayUtil.GetChild(self.ScrollRect, "MainContent"):GetComponent("UIVirtualList");
    --【Item刷新】
    self.UIVirtualList.onItemRefresh =EmailPanel.OnGetItemByRowColumn;


    self.NoEmail =CS.DisplayUtil.GetChild(gameObject, "NoEmail");
    self.bookId = logic.bookReadingMgr.selectBookId
    pageNum = 1;

    --请求获取邮箱信息
    GameController.EmailControl:GetSystemMsgRequest(pageNum);
end



function EmailPanel:UpdateEmail(page)

    pageNum=page

    allCount=table.length(Cache.EmailCache.EmailList);

    if(allCount and allCount>0)then
        --【设置列表总数量】
        self.UIVirtualList:SetItemCount(allCount);
        self.NoEmail:SetActiveEx(false);
    else
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

    self.ScrollRect = nil;
    self.UIVirtualList = nil;
    self.NoEmail = nil;
    self.bookId = nil;
    self.gameObject = nil;
    pageNum = 0;

end

return EmailPanel
