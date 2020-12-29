
local EmailItem = core.Class("EmailItem")


function EmailItem:__init(gameObject)
    self.gameObject=gameObject;

    self.BGButton = CS.DisplayUtil.GetChild(gameObject, "BGButton"):GetComponent("Button");
    self.OpenImage =CS.DisplayUtil.GetChild(gameObject, "OpenImage");
    self.Hit =CS.DisplayUtil.GetChild(gameObject, "Hit");
    self.TitleText =CS.DisplayUtil.GetChild(gameObject, "TitleText"):GetComponent("Text");
    self.Content =CS.DisplayUtil.GetChild(gameObject, "Content"):GetComponent("Text");
    self.TimeText =CS.DisplayUtil.GetChild(gameObject, "TimeText"):GetComponent("Text");
    self.GiftBtn = CS.DisplayUtil.GetChild(gameObject, "GiftBtn"):GetComponent("Button");
    self.HeadImage = CS.DisplayUtil.GetChild(gameObject, "HeadImage"):GetComponent("Image");
    self.HeadFrame = CS.DisplayUtil.GetChild(gameObject, "HeadFrame"):GetComponent("Image");
    self.SelectTab = CS.DisplayUtil.GetChild(gameObject, "SelectTab"):GetComponent("UIToggle");
    self.HeadImageMask =CS.DisplayUtil.GetChild(gameObject, "HeadImageMask");

    logic.cs.UIEventListener.AddOnClickListener(self.SelectTab.gameObject,function(data) self:SelectTabClick(data) end);
    self.Info=nil;
end


function EmailItem:GetSize()
    return self.boxSize
end

function EmailItem:SetItemData(info)
    self.Info=info;

    --【显示时间】
    self.TimeText.text = info.createtime;
    --【显示标题】
    self.TitleText.text = info.title;
    --【显示内容】
    local contentTxt = string.gsub(info.content,'\\n','\n');
    if(info.msg_type == 4 and info.comment_is_self == 0)then
        contentTxt = logic.cs.PluginTools:ReplaceBannedWords(contentTxt);
    end
    self.Content.text = contentTxt;

    --【加载头像和头像框】
    if(info.msg_type == 4)then
        --加载头像
        GameHelper.luaShowDressUpForm(info.comment_avatar,self.HeadImage,DressUp.Avatar,1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(info.comment_avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);

        self.HeadImageMask:SetActiveEx(true);
    else
        self.HeadImageMask:SetActiveEx(false);
    end

    --【是否显示有物品图标】
    if(info.msg_type == 2 and info.price_status == 0)then
        self.GiftBtn.gameObject:SetActiveEx(true);
    else
        self.GiftBtn.gameObject:SetActiveEx(false);
    end

    --【是否已读图标】
    if(info.status == 1 and info.msg_type ~= 4)then
        self.OpenImage:SetActiveEx(true);
    else
        self.OpenImage:SetActiveEx(false);
    end

    --【刷新红点】
    self:updateRedPoint(info);

    self.BGButton.onClick:RemoveAllListeners()
    self.BGButton.onClick:AddListener(function()
        self:OnItemClick(info);
    end)

    self.boxSize = core.Vector2.New(715, 155);

end

local UIEmailInfoForm=nil;

--endregion
function EmailItem:OnItemClick(info)
    logic.debug.PrintTable(info)

    if(info.status == 1)then
        UIEmailInfoForm = logic.UIMgr:Open(logic.uiid.UIEmailInfoForm)
        if(UIEmailInfoForm)then
            UIEmailInfoForm:SetEmailData(info.msgid);
        end
    else
        UIEmailInfoForm = logic.UIMgr:Open(logic.uiid.UIEmailInfoForm);
        GameController.EmailControl:ReadSystemMsgRequest(info.msgid);
    end
end


function EmailItem:updateRedPoint(_Data)
    if(_Data.msg_type==2)then
        if(_Data.price_status==0)then
            self.Hit:SetActiveEx(true)
        else
            self.Hit:SetActiveEx(false)
        end
    else
        if _Data.status == 1 then
            self.Hit:SetActiveEx(false)
        else
            self.Hit:SetActiveEx(true)
        end
    end
end

function EmailItem:SelectTabClick()
    if(self.Info and self.SelectTab.isOn==true)then
        GameController.EmailControl:BatchTest(self.Info.msgid,1,true);
    else
        GameController.EmailControl:BatchTest(self.Info.msgid,1,false);
    end
end



--销毁
function EmailItem:__delete()
    if(self.BookIconImage)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.SelectTab.gameObject,function(data) self:SelectTabClick(data) end);
    end

    self.BGButton =nil;
    self.OpenImage =nil;
    self.Hit =nil;
    self.TitleText =nil;
    self.Content =nil;
    self.TimeText =nil;
    self.GiftBtn =nil;
    self.HeadImage =nil;
    self.HeadFrame =nil;
    self.SelectTab =nil;
    self.HeadImageMask =nil;

    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end



return EmailItem