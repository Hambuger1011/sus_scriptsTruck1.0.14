local BaseClass = core.Class
local AuthorInfoPanel = BaseClass("AuthorInfoPanel")

function AuthorInfoPanel:__init(gameObject)
    self.gameObject=gameObject;

    self.HeadIcon =CS.DisplayUtil.GetChild(gameObject, "HeadIcon"):GetComponent("Image");
    self.HeadFrame =CS.DisplayUtil.GetChild(gameObject, "HeadFrame"):GetComponent("Image");
    self.AuthorName =CS.DisplayUtil.GetChild(gameObject, "AuthorName"):GetComponent("Text");
    self.PersonalStatus =CS.DisplayUtil.GetChild(gameObject, "PersonalStatus"):GetComponent("Text");
    self.BookNumsText =CS.DisplayUtil.GetChild(gameObject, "BookNumsText"):GetComponent("Text");
    self.FansNumsText =CS.DisplayUtil.GetChild(gameObject, "FansNumsText"):GetComponent("Text");
    self.LikeToogle =CS.DisplayUtil.GetChild(gameObject, "LikeToogle"):GetComponent("UIToggle");
    self.ThumbUpToogle =CS.DisplayUtil.GetChild(gameObject, "ThumbUpToogle"):GetComponent("UIToggle");
    self.ThumbUpText =CS.DisplayUtil.GetChild(self.ThumbUpToogle.gameObject, "ThumbUpText"):GetComponent("Text");

    --飞鸽按钮
    self.MessageBtn =CS.DisplayUtil.GetChild(gameObject, "MessageBtn");
    self.DynamicTitleTxt =CS.DisplayUtil.GetChild(gameObject, "DynamicTitleTxt"):GetComponent("Text");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.MessageBtn,function(data) self:MessageBtnClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.LikeToogle.gameObject,function(data) self:LikeToogleClick() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ThumbUpToogle.gameObject,function(data) self:ThumbUpToogleClick() end)
end

function AuthorInfoPanel:UpdateInfo()

    if(Cache.ComuniadaCache.WriterInfo)then
        --显示头像
        GameHelper.luaShowDressUpForm(Cache.ComuniadaCache.WriterInfo.avatar,self.HeadIcon,DressUp.Avatar,1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(Cache.ComuniadaCache.WriterInfo.avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);

        --作者名字
        self.AuthorName.text=Cache.ComuniadaCache.WriterInfo.nickname;
        --个性签名
        self.PersonalStatus.text=Cache.ComuniadaCache.WriterInfo.writer_sign;
        --书本数量
        self.BookNumsText.text=Cache.ComuniadaCache.WriterInfo.book_count;
        --粉丝数量
        self.FansNumsText.text=tostring(Cache.ComuniadaCache.WriterInfo.fans_count);
        --self.LikeToogle =CS.DisplayUtil.GetChild(gameObject, "LikeToogle"):GetComponent("UIToggle");

        --【刷新关注作者状态】
        self:UpdateWriterFollow();
        --【刷新 点赞状态】
        self:UpdateWriterAgree();

        if(logic.cs.UserDataManager.userInfo and logic.cs.UserDataManager.userInfo.data)then
            --如果作者uid  等于 自己的uid     【隐藏信鸽按钮】
            if(Cache.ComuniadaCache.WriterInfo.uid==logic.cs.UserDataManager.userInfo.data.userinfo.uid)then
                self.MessageBtn:SetActive(false);
            else
                self.MessageBtn:SetActive(true);
            end
        end

    else
        --显示头像
        GameHelper.luaShowDressUpForm(-1,self.HeadIcon,DressUp.Avatar,1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(-1,self.HeadFrame,DressUp.AvatarFrame,2001);
    end


end

--region【刷新关注作者状态】

function AuthorInfoPanel:UpdateWriterFollow()

    if(Cache.ComuniadaCache.WriterInfo and Cache.ComuniadaCache.WriterInfo.is_follow)then
        if(Cache.ComuniadaCache.WriterInfo.is_follow==0)then
            self.LikeToogle.isOn=false;
        elseif(Cache.ComuniadaCache.WriterInfo.is_follow==1)then
            self.LikeToogle.isOn=true;
        end

        --粉丝数量
        self.FansNumsText.text=tostring(Cache.ComuniadaCache.WriterInfo.fans_count);
    end
end

--endregion

--region【刷新 点赞状态】

function AuthorInfoPanel:UpdateWriterAgree()

    if(Cache.ComuniadaCache.WriterInfo and Cache.ComuniadaCache.WriterInfo.is_agree)then
        if(Cache.ComuniadaCache.WriterInfo.is_agree==0)then
            self.ThumbUpToogle.isOn=false;
        elseif(Cache.ComuniadaCache.WriterInfo.is_agree==1)then
            self.ThumbUpToogle.isOn=true;
        end

        self.ThumbUpText.text=Cache.ComuniadaCache.WriterInfo.agree_count;
    end

end

--endregion



--region【按钮点击】【信鸽按钮】

function AuthorInfoPanel:MessageBtnClick(data)
    if(Cache.ComuniadaCache.WriterInfo.uid)then
        GameController.ChatControl:GetPrivateLetterPageRequest(Cache.ComuniadaCache.WriterInfo.uid,1,Cache.ComuniadaCache.WriterInfo.nickname);
    end
end

--endregion


--region【按钮点击】【关注作者】

function AuthorInfoPanel:LikeToogleClick()

    if(Cache.ComuniadaCache.WriterInfo.uid)then
        GameController.CommunityControl:SetWriterFollowRequest(Cache.ComuniadaCache.WriterInfo.uid);
    end
end

--endregion


--region【按钮点击】【点赞作者】

function AuthorInfoPanel:ThumbUpToogleClick()

    if(Cache.ComuniadaCache.WriterInfo.uid)then
        GameController.CommunityControl:SetWriterAgreeRequest(Cache.ComuniadaCache.WriterInfo.uid);
    end
end

--endregion

--销毁
function AuthorInfoPanel:__delete()
end

return AuthorInfoPanel
