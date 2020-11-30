local BaseClass = core.Class
local DressUpItem = BaseClass("DressUpItem")

function DressUpItem:__init(gameObject)
    self.gameObject=gameObject;
    self.IconImg =CS.DisplayUtil.GetChild(gameObject, "IconImg"):GetComponent("Image");
    self.LockIcon =CS.DisplayUtil.GetChild(gameObject, "LockIcon");
    self.State =CS.DisplayUtil.GetChild(gameObject, "State");


    logic.cs.UIEventListener.AddOnClickListener(self.IconImg.gameObject,function(data) self:OnIconImgClick() end)
    self.Infodata=nil;
    self._index=nil;
end


function DressUpItem:OnIconImgClick(vdata)
    if(self.Infodata)then
        GameController.DressUpControl:SetHeadInfoData(self.Infodata,self.gameObject,self._index);
    end
end





function DressUpItem:SetInfo(Info,_type,_index)
    self.Infodata = Info;
    self._index=_index;
    local config_info=DataConfig.Q_DressUpData:GetMapData(Info.id);
    --加载图片
    self.IconImg.sprite = CS.ResourceManager.Instance:GetUISprite("DressUpForm/"..config_info.resources);
    --是否解锁
    if(Info.is_active==0)then   --未解锁
        self.LockIcon:SetActive(true);
    elseif(Info.is_active==1)then  --已解锁
        self.IconImg.material=nil;
        self.LockIcon:SetActive(false);
    end

    if(Info.id==Cache.DressUpCache.avatar or Info.id==Cache.DressUpCache.avatar_frame or Info.id==Cache.DressUpCache.barrage_frame or Info.id==Cache.DressUpCache.comment_frame)then
        self:Haved();  --"已使用";
        GameController.DressUpControl:AddUsinglist(self.State)
        GameController.DressUpControl:SetHeadInfoData(Info,self.gameObject,_index);
    end
end


--【已使用】
function DressUpItem:Haved()
    --"已使用";
    GameController.DressUpControl:ClearUsinglist();
    self.State:SetActive(true);
    GameController.DressUpControl:AddUsinglist(self.State)
end


--销毁
function DressUpItem:__delete()
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return DressUpItem
