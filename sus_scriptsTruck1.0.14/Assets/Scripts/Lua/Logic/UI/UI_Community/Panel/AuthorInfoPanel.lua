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

    --飞鸽按钮
    self.MessageBtn =CS.DisplayUtil.GetChild(gameObject, "MessageBtn");
    self.DynamicTitleTxt =CS.DisplayUtil.GetChild(gameObject, "DynamicTitleTxt"):GetComponent("Text");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.MessageBtn,function(data) self:MessageBtnClick() end)
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


    else
        --显示头像
        GameHelper.luaShowDressUpForm(-1,self.HeadIcon,DressUp.Avatar,1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(-1,self.HeadFrame,DressUp.AvatarFrame,2001);
    end


end

function AuthorInfoPanel:MessageBtnClick(data)

end



--销毁
function AuthorInfoPanel:__delete()

end

return AuthorInfoPanel
