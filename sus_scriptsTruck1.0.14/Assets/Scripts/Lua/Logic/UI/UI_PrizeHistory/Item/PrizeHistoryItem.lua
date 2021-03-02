local PrizeHistoryItem = core.Class("PrizeHistoryItem")

function PrizeHistoryItem:__init(gameObject)
    self.gameObject=gameObject;

    self.Icon =CS.DisplayUtil.GetChild(gameObject, "Icon"):GetComponent("Image");
    self.ItemCount =CS.DisplayUtil.GetChild(gameObject, "ItemCount"):GetComponent("Text");
    self.Time =CS.DisplayUtil.GetChild(gameObject, "Time"):GetComponent("Text");
end

function PrizeHistoryItem:SetItemData(itemData,itemIndex)
    self.mItemIndex = itemIndex;

    self.Time.text=itemData.create_time;


    local sprite = Cache.PropCache.SpriteData[tonumber(itemData.item_list[1].id)];
    self.Icon.sprite = sprite;
    self.Icon:SetNativeSize()
    if(itemData.id==1 and itemData.id==2 and itemData.id==3)then
        self.Icon.transform.localScale = core.Vector3.New(0.4,0.4,1)
    else
        self.Icon.transform.localScale = core.Vector3.New(0.5,0.5,1)
    end

    self.ItemCount.text = "x".. itemData.item_list[1].num;
    self.ItemCount.gameObject:SetActive(true);
end


--销毁
function PrizeHistoryItem:__delete()
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return PrizeHistoryItem
