local InvestmentItem = core.Class("InvestmentItem")

function InvestmentItem:__init(gameObject)
    self.gameObject=gameObject;

    self.Icon =CS.DisplayUtil.GetChild(gameObject, "Icon"):GetComponent("Image");
    self.ItemCount =CS.DisplayUtil.GetChild(gameObject, "ItemCount"):GetComponent("Text");
    self.BuyedBg =CS.DisplayUtil.GetChild(gameObject, "BuyedBg");

end

function InvestmentItem:SetItemData(itemData,itemIndex)
    self.Icon.gameObject:SetActive(true);
    if(itemIndex==4)then
        local ItemCount1 =CS.DisplayUtil.GetChild(self.gameObject, "ItemCount1"):GetComponent("Text");
        self.ItemCount.text="x"..Cache.InvestmentCache.Investment1.diamond_count;
        ItemCount1.text="x"..Cache.InvestmentCache.Investment2.key_count;
        self.Icon.gameObject:SetActive(false);
    else
        self.ItemCount.text = "x".. itemData.num;
    end

    if 1000<tonumber(itemData.id) and tonumber(itemData.id)<10000 then
        local sprite=DataConfig.Q_DressUpData:GetSprite(itemData.id)
        self.Icon.sprite = sprite;
        self.Icon:SetNativeSize()
        self.Icon.transform.localScale = core.Vector3.New(0.8,0.8,1)
    else
        local sprite = Cache.PropCache.SpriteData[tonumber(itemData.id)]
        self.Icon.sprite = sprite;
        self.Icon:SetNativeSize()
        if(itemData.id==1 and itemData.id==2)then
            self.Icon.transform.localScale = core.Vector3.New(0.4,0.4,1)
        else
            self.Icon.transform.localScale = core.Vector3.New(0.5,0.5,1)
        end
    end
end

function InvestmentItem:ShowBuyed()
    self.BuyedBg:SetActive(true);
end


--销毁
function InvestmentItem:__delete()
    self.Icon =nil;
    self.ItemCount=nil;
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return InvestmentItem
