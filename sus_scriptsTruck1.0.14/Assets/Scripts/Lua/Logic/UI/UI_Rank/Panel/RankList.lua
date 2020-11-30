local BaseClass = core.Class
local RankList = BaseClass("RankList")

local BookRankItem = require('Logic/UI/UI_Rank/Item/RankItem');

function RankList:__init(gameObject)
    self.gameObject=gameObject;
    self.RankBookItemObj =CS.DisplayUtil.GetChild(gameObject, "RankBookItem");
    self.BookScrollRect =CS.DisplayUtil.GetChild(gameObject, "BookScrollRect"):GetComponent("ScrollRect");
    self.ScrollRectTransform=self.BookScrollRect.gameObject:GetComponent(typeof(logic.cs.RectTransform));
end

local showIndex = -1;
--BookRankItem
local ItemList={};
function RankList:UpdateList(InfoList,_type)
    self:ClearList();
    if(ItemList==nil)then
        ItemList={};
    end

    showIndex = -1;
    local itemCount = table.length(ItemList);
    local InfoCount = table.length(InfoList);

    for i = 1, InfoCount do
        local item;
        if (itemCount > i)then
            item = ItemList[i];
        else
            local go = logic.cs.GameObject.Instantiate(self.RankBookItemObj, self.BookScrollRect.content.transform);
            go.transform.localPosition = core.Vector3.zero;
            go.transform.localScale = core.Vector3.one;
            go:SetActive(true);
            item =BookRankItem.New(go);
            table.insert(ItemList,item);
        end
        item._index = i;
        item:SetInfo(InfoList[i],_type);
    end
    if (showIndex ~= -1)then
        self.ScrollRectTransform.anchoredPosition ={x=-(showIndex * 200),y=0,z=0};
    end
end

function RankList:ClearList()
    if (ItemList)then
        local len=#ItemList;
        if(len>0)then
            for i = 1, len do
                ItemList[i]:Delete();
            end
        end
    end
    ItemList=nil;
end


--销毁
function RankList:__delete()
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;

    self.RankBookItemObj =nil;
    self.BookScrollRect=nil;
    self.ScrollRectTransform=nil;

    showIndex =nil;
    self:ClearList();
end


return RankList
