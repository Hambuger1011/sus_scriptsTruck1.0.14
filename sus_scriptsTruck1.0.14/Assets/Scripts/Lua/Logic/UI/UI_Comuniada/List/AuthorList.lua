local BaseClass = core.Class
local AuthorList = BaseClass("AuthorList")

local AuthorItem = require('Logic/UI/UI_Comuniada/Item/AuthorItem');

function AuthorList:__init(gameObject)
    self.gameObject=gameObject;

    self.ScrollRect =CS.DisplayUtil.GetChild(gameObject, "ScrollRect"):GetComponent("ScrollRect");
    self.ScrollRectTransform=self.ScrollRect.gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.TitleTxt =CS.DisplayUtil.GetChild(gameObject, "TitleTxt"):GetComponent("Text");
    self.AuthorItemObj =CS.DisplayUtil.GetChild(gameObject, "AuthorItem");

    self.ItemList={};
end


function AuthorList:UpdateList(InfoList)
    self:ClearList();
    if(self.ItemList==nil)then
        self.ItemList={};
    end

    local len = table.length(InfoList);
    for i = 1, len do
        local go = logic.cs.GameObject.Instantiate(self.AuthorItemObj, self.ScrollRect.content.transform);
        go.transform.localPosition = core.Vector3.zero;
        go.transform.localScale = core.Vector3.one;
        go:SetActive(true);

        local item =AuthorItem.New(go);
        table.insert(self.ItemList,item);

        item:SetItemData(InfoList[i],i);
    end
end


function AuthorList:ClearList()
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
function AuthorList:__delete()
    self:ClearList();
    self.ScrollRect=nil;
    self.ScrollRectTransform=nil;
    self.TitleTxt=nil;
    self.AuthorItemObj=nil;
    self.ItemList=nil;
    self.gameObject=nil;
end


return AuthorList
