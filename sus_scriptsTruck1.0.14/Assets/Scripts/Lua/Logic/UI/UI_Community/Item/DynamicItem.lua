local BaseClass = core.Class
local DynamicItem = BaseClass("DynamicItem")

function DynamicItem:__init(gameObject)
    self.gameObject=gameObject;

    self.rectTransform=gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.BookIcon =CS.DisplayUtil.GetChild(gameObject, "BookIcon"):GetComponent("Image");
    self.TitleTxt =CS.DisplayUtil.GetChild(gameObject, "TitleTxt"):GetComponent("Text");
    self.Content =CS.DisplayUtil.GetChild(gameObject, "Content"):GetComponent("Text");
    self.TimeText =CS.DisplayUtil.GetChild(gameObject, "TimeText"):GetComponent("Text");

    self.BookInfo=nil;
    self.mItemIndex = 0;
end

function DynamicItem:SetItemData(itemData,itemIndex)
    if(itemData==nil)then return; end
    self.mItemIndex = itemIndex;
    self.BookInfo = itemData;

    if(self.TitleTxt)then
        self.TitleTxt.text = itemData.chapter_number;
    end

    if(self.Content)then
        self.Content.text = itemData.content;
    end

    if(self.TimeText)then
        self.TimeText.text = itemData.time_code;
    end

    ----【展示创作书本封面】
    --local cover_image = itemData.cover_image
    --GameHelper.ShowUGCStoryBg(cover_image,nil,self.BookIcon)
end

--销毁
function DynamicItem:__delete()

    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return DynamicItem
