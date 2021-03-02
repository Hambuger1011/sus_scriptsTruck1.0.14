local LuckyWinnersItem = core.Class("LuckyWinnersItem")

function LuckyWinnersItem:__init(gameObject)
    self.gameObject=gameObject;

    self.HeadIcon =CS.DisplayUtil.GetChild(gameObject, "HeadIcon"):GetComponent("Image");
    self.HeadFrame =CS.DisplayUtil.GetChild(gameObject, "HeadFrame"):GetComponent("Image");
    self.PlayerName =CS.DisplayUtil.GetChild(gameObject, "PlayerName"):GetComponent("Text");
    self.Icon =CS.DisplayUtil.GetChild(gameObject, "Icon"):GetComponent("Image");
    self.ItemCount =CS.DisplayUtil.GetChild(gameObject, "ItemCount"):GetComponent("Text");
end

function LuckyWinnersItem:SetItemData(itemData,itemIndex)
    self.mItemIndex = itemIndex;

    self.PlayerName.text=itemData.nickname;


    local sprite = Cache.PropCache.SpriteData[tonumber(itemData.item_list[1].id)];
    self.Icon.sprite = sprite;
    self.Icon:SetNativeSize()
    if(itemData.id==1 and itemData.id==2)then
        self.Icon.transform.localScale = core.Vector3.New(0.4,0.4,1)
    else
        self.Icon.transform.localScale = core.Vector3.New(0.5,0.5,1)
    end

    self.ItemCount.text = "x".. itemData.item_list[1].num;
    self.ItemCount.gameObject:SetActive(true);
end


--销毁
function LuckyWinnersItem:__delete()
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return LuckyWinnersItem
