local ChatItem = core.Class("ChatItem")

function ChatItem:__init(gameObject)
    self.gameObject=gameObject;

    self.RectTransform = gameObject:GetComponent("RectTransform");
    self.HeadIcon =CS.DisplayUtil.GetChild(gameObject, "HeadIcon"):GetComponent("Image");
    self.HeadFrame =CS.DisplayUtil.GetChild(gameObject, "HeadFrame"):GetComponent("Image");
    self.Content =CS.DisplayUtil.GetChild(gameObject, "Content"):GetComponent("Text");
    self.PlayerName =CS.DisplayUtil.GetChild(gameObject, "PlayerName"):GetComponent("Text");
    self.MsgBg =CS.DisplayUtil.GetChild(gameObject, "MsgBg");
    self.MsgBgTrans = self.MsgBg:GetComponent("RectTransform");
    self.ContentSizeFitter=self.MsgBg:GetComponent("ContentSizeFitter");
    self.ContentImmediate = self.MsgBg:GetComponent("ContentImmediate");

    self.FitWidth=480;
    self.FitString=24;
end

function ChatItem:SetItemData(itemData,itemIndex)
    self.mItemIndex = itemIndex;

    self.Content.text=itemData.content;
    self.PlayerName.text=itemData.user_info.nickname;
    --显示头像
    GameHelper.luaShowDressUpForm(itemData.user_info.avatar,self.HeadIcon,DressUp.Avatar,1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(itemData.user_info.avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);


    local size = self.MsgBgTrans.sizeDelta;
    ----------------------------------------------------------------width
    if (self.Content.preferredWidth <= self.FitWidth)then
        self.ContentSizeFitter.horizontalFit = CS.UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
    else

        self.ContentSizeFitter.horizontalFit = CS.UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
        size = self.MsgBgTrans.sizeDelta;
        size.x = self.FitWidth;
        self.MsgBgTrans.sizeDelta = size;

    end
    self.ContentSizeFitter:SetLayoutHorizontal();
    self.ContentSizeFitter:SetLayoutVertical();

    local fits= self.ContentImmediate:GetPreferredSize();
    local y = fits.y+52;
    --if (y < 75)then
    --    y = 75;
    --end
    self.RectTransform:SetSizeWithCurrentAnchors(CS.UnityEngine.RectTransform.Axis.Vertical, y);
end


--销毁
function ChatItem:__delete()


    self.HeadIcon=nil;
    self.HeadFrame =nil;
    self.Content=nil;
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return ChatItem
