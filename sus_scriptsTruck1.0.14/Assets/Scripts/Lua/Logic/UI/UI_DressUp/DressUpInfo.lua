local BaseClass = core.Class
local DressUpInfo = BaseClass("DressUpInfo")

function DressUpInfo:__init(gameObject)
    self.gameObject=gameObject;

    self.IconImg =CS.DisplayUtil.GetChild(gameObject, "IconImg"):GetComponent("Image");
    self.IconName =CS.DisplayUtil.GetChild(gameObject, "IconName"):GetComponent("Text");
    self.IconDescription =CS.DisplayUtil.GetChild(gameObject, "IconDescription"):GetComponent("Text");
    self.IconTimeTag =CS.DisplayUtil.GetChild(gameObject, "IconTimeTag"):GetComponent("Text");
    self.IconStateTag =CS.DisplayUtil.GetChild(gameObject, "IconStateTag"):GetComponent("Text");

    logic.cs.UIEventListener.AddOnClickListener(self.IconImg.gameObject,function(data) self:OnIconImgClick() end)

end


function DressUpInfo:OnIconImgClick(vdata)


end





function DressUpInfo:SetInfo(Info)

end




--销毁
function DressUpInfo:__delete()
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return DressUpInfo
