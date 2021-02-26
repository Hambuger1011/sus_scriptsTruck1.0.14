local BaseClass = core.Class
local UserInfoPanel = BaseClass("UserInfoPanel")

function UserInfoPanel:__init(gameObject)
    self.gameObject=gameObject;

    self.HeadIcon =CS.DisplayUtil.GetChild(gameObject, "HeadIcon"):GetComponent("Image");
    self.HeadFrame =CS.DisplayUtil.GetChild(gameObject, "HeadFrame"):GetComponent("Image");
    self.UserName =CS.DisplayUtil.GetChild(gameObject, "UserName"):GetComponent("Text");
    self.PersonalStatus =CS.DisplayUtil.GetChild(gameObject, "PersonalStatus"):GetComponent("Text");
    self.BookNumsText =CS.DisplayUtil.GetChild(gameObject, "BookNumsText"):GetComponent("Text");
    self.FansNumsText =CS.DisplayUtil.GetChild(gameObject, "FansNumsText"):GetComponent("Text");
    self.ThumbUpToogle =CS.DisplayUtil.GetChild(gameObject, "ThumbUpToogle"):GetComponent("UIToggle");
    self.ThumbUpText =CS.DisplayUtil.GetChild(self.ThumbUpToogle.gameObject, "ThumbUpText"):GetComponent("Text");

    --飞鸽按钮
    self.MessageBtn =CS.DisplayUtil.GetChild(gameObject, "MessageBtn");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.MessageBtn,function(data) self:MessageBtnClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ThumbUpToogle.gameObject,function(data) self:ThumbUpToogleClick() end)
end

function UserInfoPanel:UpdateInfo()
    --显示头像
    GameHelper.luaShowDressUpForm(Cache.DressUpCache.avatar,self.HeadIcon,DressUp.Avatar,1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(Cache.DressUpCache.avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);
    --作者名字
    self.UserName.text=logic.cs.UserDataManager.userInfo.data.userinfo.nickname;
    --个性签名
    --self.PersonalStatus.text=Cache.ComuniadaCache.WriterInfo.writer_sign;
    self.PersonalStatus.text="个性签名";
    --书本数量
    self.BookNumsText.text=logic.cs.UserDataManager.selfBookInfo.data.read_book_count;
    --粉丝数量
    --self.FansNumsText.text=tostring(Cache.ComuniadaCache.WriterInfo.fans_count);
    self.FansNumsText.text="粉丝数量";
    --【刷新 点赞状态】
    self:UpdateWriterAgree();
    
end

--region【刷新 点赞状态】

function UserInfoPanel:UpdateWriterAgree()

    if(Cache.ComuniadaCache.WriterInfo and Cache.ComuniadaCache.WriterInfo.is_agree)then
        if(Cache.ComuniadaCache.WriterInfo.is_agree==0)then
            self.ThumbUpToogle.isOn=false;
        elseif(Cache.ComuniadaCache.WriterInfo.is_agree==1)then
            self.ThumbUpToogle.isOn=true;
        end

        --self.ThumbUpText.text=Cache.ComuniadaCache.WriterInfo.agree_count;
        self.ThumbUpText.text="点赞数量";
    end

end

--endregion



--region【按钮点击】【信鸽按钮】

function UserInfoPanel:MessageBtnClick(data)
    --if(Cache.ComuniadaCache.WriterInfo.uid)then
    --    GameController.ChatControl:GetPrivateLetterPageRequest(Cache.ComuniadaCache.WriterInfo.uid,1,Cache.ComuniadaCache.WriterInfo.nickname);
    --end
end

--endregion

--region【按钮点击】【点赞作者】

function UserInfoPanel:ThumbUpToogleClick()
    --if(Cache.ComuniadaCache.WriterInfo.uid)then
    --    GameController.CommunityControl:SetWriterAgreeRequest(Cache.ComuniadaCache.WriterInfo.uid);
    --end
end

--endregion


--销毁
function UserInfoPanel:__delete()
    --按钮监听
    if(self.MessageBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.MessageBtn,function(data) self:MessageBtnClick() end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.ThumbUpToogle.gameObject,function(data) self:ThumbUpToogleClick() end)
    end
    self.HeadIcon = nil;
    self.HeadFrame = nil;
    self.UserName = nil;
    self.PersonalStatus = nil;
    self.BookNumsText = nil;
    self.FansNumsText = nil;
    self.ThumbUpToogle = nil;
    self.ThumbUpText = nil;
    self.MessageBtn = nil;

    self.gameObject = nil;
end

return UserInfoPanel
