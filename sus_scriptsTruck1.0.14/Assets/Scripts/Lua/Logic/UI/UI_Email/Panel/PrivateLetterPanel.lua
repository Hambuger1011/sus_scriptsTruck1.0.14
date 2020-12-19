
local PrivateLetterPanel = core.Class("PrivateLetterPanel")

local PrivateLetterItem = require('Logic/UI/UI_Email/Item/PrivateLetterItem');


local allCount=0;

local ItemList={};
function PrivateLetterPanel:__init(gameObject)

    self.MainContent = CS.DisplayUtil.GetChild(gameObject, "MainContent")
    self.UIVirtualList=self.MainContent.gameObject:GetComponent("UIVirtualList");

    --【Item刷新】
    self.UIVirtualList.onItemRefresh =PrivateLetterPanel.OnGetItemByRowColumn;

    self:Moni()
end

local infolist={};
function PrivateLetterPanel:Moni()

    local info={};
    info.id=1;
    info.name="123"
    info.content="In this chapter,  you will have to choose who you wantt save";
    info.state=0;

    local info2={};
    info2.id=2;
    info2.name="sfsdfs"
    info2.content="e who you wantt save";
    info2.state=0;

    table.insert(infolist,info);
    table.insert(infolist,info2);

    allCount=table.length(infolist);

    if(allCount and allCount>0)then
        --【设置列表总数量】
        self.UIVirtualList:SetItemCount(allCount);
    end

end



function PrivateLetterPanel.OnGetItemByRowColumn(row)
    local index=row.itemIndex+1;
    local trans = row.rect;

    if(trans)then

        --【获取一组数据】
        local itemData =infolist[index];
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

end

return PrivateLetterPanel
