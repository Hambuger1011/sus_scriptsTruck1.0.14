local BaseClass = core.Class
local BookTypeList = BaseClass("BookTypeList")

local BookTypeItem = require('Logic/UI/UI_Search/Item/BookTypeItem');

function BookTypeList:__init(gameObject)
    self.gameObject=gameObject;
    self.BookTypeItemObj =CS.DisplayUtil.GetChild(gameObject, "BookTypeItem");
    self.BookScrollRect =CS.DisplayUtil.GetChild(gameObject, "BookScrollRect"):GetComponent("ScrollRect");
    self.ScrollRectTransform=self.BookScrollRect.gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.Book_Empty_Pfb =CS.DisplayUtil.GetChild(gameObject, "Book_Empty_Pfb");

    self.ItemList=nil;
end

local showIndex = -1;
--BookTypeItem

function BookTypeList:UpdateList(InfoList)
    self:ClearList();
    if(self.ItemList==nil)then
        self.ItemList={};
    end
    --停止滑动
    --self.BookScrollRect:StopMovement();
    self.BookScrollRect.content.localPosition = {x=0;y=0};


    self.Book_Empty_Pfb:SetActive(false);
    showIndex = -1;
    local itemCount = table.length(self.ItemList);
    local InfoCount = table.length(InfoList);

    for i = 1, InfoCount do
        local infoItem = InfoList[i]
        if infoItem ~= nil and logic.cs.JsonDTManager:GetJDTBookDetailInfo(infoItem.book_id) ~=nil then
                local item;
                if (itemCount > i)then
                    item = self.ItemList[i];
                else
                    local go = logic.cs.GameObject.Instantiate(self.BookTypeItemObj, self.BookScrollRect.content.transform);
                    go.transform.localPosition = core.Vector3.zero;
                    go.transform.localScale = core.Vector3.one;
                    go:SetActive(true);
                    item =BookTypeItem.New(go);
                    table.insert(self.ItemList,item);
                end
                item:SetInfo(InfoList[i]);
        end
        
    end
    
    if (showIndex ~= -1)then
        self.ScrollRectTransform.anchoredPosition ={x=-(showIndex * 200),y=0,z=0};
    end
    
    if(InfoCount<=0)then
        self.Book_Empty_Pfb:SetActive(true);
    end
end



--【限时活动免费读书 显示标签】
function BookTypeList:Limit_time_Free()
    if(self.ItemList==nil)then return; end
    local len = table.length(self.ItemList);
    for i = 1, len do
        self.ItemList[i]:Limit_time_Free();
    end
end




function BookTypeList:ClearList()
    if (self.ItemList)then
        local len=#self.ItemList;
        if(len>0)then
            for i = 1, len do
                self.ItemList[i]:Delete();
            end
        end
    end
    self.ItemList=nil;
end


--销毁
function BookTypeList:__delete()
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
    self.BookTypeItemObj=nil;
    self.BookScrollRect=nil;
    self.ScrollRectTransform=nil;

    showIndex =nil;
    self:ClearList();
end


return BookTypeList
