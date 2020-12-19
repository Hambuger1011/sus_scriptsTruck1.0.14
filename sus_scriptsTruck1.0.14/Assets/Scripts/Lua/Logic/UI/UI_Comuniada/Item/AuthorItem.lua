local BaseClass = core.Class
local AuthorItem = BaseClass("AuthorItem")

function AuthorItem:__init(gameObject)
    self.gameObject=gameObject;

    self.HeadIcon =CS.DisplayUtil.GetChild(gameObject, "HeadIcon"):GetComponent("Image");
    self.State =CS.DisplayUtil.GetChild(gameObject, "State");
    self.StateEnd =CS.DisplayUtil.GetChild(gameObject, "StateEnd");
    self.nickname =CS.DisplayUtil.GetChild(gameObject, "nickname"):GetComponent("Text");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.HeadIcon.gameObject,function(data) self:HeadIconclicke() end)

    self.info=nil;
end

function AuthorItem:SetItemData(itemData,itemIndex)
    self.info=itemData;
    if(itemData.user_info)then
        --显示头像
        GameHelper.luaShowDressUpForm(itemData.user_info.avatar,self.HeadIcon,DressUp.Avatar,1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(itemData.user_info.avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);
    end
    if(itemData.is_online==0)then
        self.State:SetActive(false);
        self.StateEnd:SetActive(true);
    elseif(itemData.is_online==1)then
        self.State:SetActive(true);
        self.StateEnd:SetActive(false);
    end

    self.nickname.text=itemData.user_info.nickname;
end


--点击头像
function AuthorItem:HeadIconclicke()
    if(self.info)then
        GameController.CommunityControl:GetWriterInfoRequest(self.info.uid);
    end
end


--销毁
function AuthorItem:__delete()
    if(self.HeadIcon)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.HeadIcon.gameObject,function(data) self:HeadIconclicke() end)
    end
    self.HeadIcon=nil;
    self.State=nil;
    self.nickname=nil;
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return AuthorItem
