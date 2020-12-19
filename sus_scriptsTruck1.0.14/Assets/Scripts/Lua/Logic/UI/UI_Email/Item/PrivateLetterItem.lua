local BaseClass = core.Class
local PrivateLetterItem = BaseClass("PrivateLetterItem")

function PrivateLetterItem:__init(gameObject)
    self.gameObject=gameObject;

    self.BG =CS.DisplayUtil.GetChild(gameObject, "BG");
    self.State =CS.DisplayUtil.GetChild(gameObject, "State");
    self.TitleText =CS.DisplayUtil.GetChild(gameObject, "TitleText"):GetComponent("Text");
    self.Content =CS.DisplayUtil.GetChild(gameObject, "Content"):GetComponent("Text");
    self.SelectTab = CS.DisplayUtil.GetChild(gameObject, "SelectTab"):GetComponent("UIToggle");


    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.SelectTab.gameObject,function(data) self:SelectTabclicke() end)


    self.mItemIndex = 0;
end

function PrivateLetterItem:SetItemData(itemData,itemIndex)
    if(itemData==nil)then return; end
    self.mItemIndex = itemIndex;


end


--销毁
function PrivateLetterItem:__delete()
    --if(self.BookIconImage)then
    --    logic.cs.UIEventListener.RemoveOnClickListener(self.BookIconImage.gameObject,function(data) self:BookOnclicke() end)
    --end
    --
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return PrivateLetterItem
