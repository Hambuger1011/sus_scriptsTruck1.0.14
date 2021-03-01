local BaseClass = core.Class
local BookList = BaseClass("BookList")

local BookItem = require('Logic/UI/UI_PersonalCenterForm/BookItem');

function BookList:__init(gameObject)
    self.gameObject=gameObject;

    self.ScrollRect =CS.DisplayUtil.GetChild(gameObject, "ScrollRect"):GetComponent("ScrollRect");
    self.ScrollRectTransform=self.ScrollRect.gameObject:GetComponent(typeof(logic.cs.RectTransform));

    --获取预设体 prefab
    self.bookItemObj=CS.XLuaHelper.GetBookItem();

    self.ItemList={};
end


function BookList :UpdateList(InfoList)
    self:ClearList();
    if(self.ItemList==nil)then
        self.ItemList={};
    end

    local len = table.length(InfoList);
    for i = 1, len do
        local infoItem = InfoList[i]
        if infoItem ~= nil and logic.cs.JsonDTManager:GetJDTBookDetailInfo(infoItem.book_id) ~= nil then
            local go = logic.cs.GameObject.Instantiate(self.bookItemObj, self.ScrollRect.content.transform);
            go.transform.localPosition = core.Vector3.zero;
            go.transform.localScale = core.Vector3.one;
            go:SetActive(true);
            local item =BookItem.New(go);
            table.insert(self.ItemList,item);
            item._index = i;
            item:SetInfo(InfoList[i]);
        end
    end
end



function BookList :ClearList()
    if (self.ItemList)then
        local len=#self.ItemList;
        if(len>0)then
            for i = 1, len do
                self.ItemList[i]:Delete();
            end
        end
    end
    self.ItemList={};
    self.ItemList=nil;
end


--销毁
function BookList :__delete()
    self.bookItemObj=nil;
    self.ScrollRect=nil;
    self.ScrollRectTransform=nil;
    self:ClearList();
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return BookList 
