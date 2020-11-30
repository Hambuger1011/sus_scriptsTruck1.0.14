local BaseClass = core.Class
local DressUpInfoPanel = BaseClass("DressUpInfoPanel")

function DressUpInfoPanel:__init(gameObject)
    self.gameObject=gameObject;

    self.avatar =CS.DisplayUtil.GetChild(gameObject, "avatar"):GetComponent("Image");
    self.avatar_frame =CS.DisplayUtil.GetChild(gameObject, "avatar_frame"):GetComponent("Image");
    self.IconName =CS.DisplayUtil.GetChild(gameObject, "IconName"):GetComponent("Text");
    self.IconDescription =CS.DisplayUtil.GetChild(gameObject, "IconDescription"):GetComponent("Text");
    self.IconTimeTag =CS.DisplayUtil.GetChild(gameObject, "IconTimeTag"):GetComponent("Text");
    self.StateBtn =CS.DisplayUtil.GetChild(gameObject, "StateBtn"):GetComponent("Image");

    self.StateBtnText =CS.DisplayUtil.GetChild(gameObject, "StateBtnText"):GetComponent("Text");
    logic.cs.UIEventListener.AddOnClickListener(self.StateBtn.gameObject,function(data) self:OnStateBtnClick() end)

    self._info=nil;
    self._index=nil;
    self.islock=false;
end


function DressUpInfoPanel:OnStateBtnClick(vdata)
    if(self.islock)then return; end
    if(self._info)then
        GameController.DressUpControl:SetUserAvatarRequest(self._info.frame_type,self._info.id,self._index);
    end
end


function DressUpInfoPanel:SetInfo(Info,_index)
    self._info=Info;
    self._index=_index;
    local config_info=DataConfig.Q_DressUpData:GetMapData(Info.id);

    self.IconName.text=config_info.title;
    self.IconDescription.text=config_info.explain;

    --剩余小时  等于
    if(Info.expire_time<0)then
        self.IconTimeTag.text="Permanently"
    elseif(Info.expire_day>0)then  --天数大于1
        self.IconTimeTag.text=string.format(Info.expire_day,"d ",Info.expire_time,"h ","reamining");
    elseif(Info.expire_day<=0 and Info.expire_time>0)then  --天数等于0  剩余小时大于0
        self.IconTimeTag.text=string.format(Info.expire_time,"h ","reamining");
    end

    --如果ID等于 当前装扮的 头像、头像框、评论框、弹幕框  按钮切换状态  已使用
    if(Info.frame_type==DressUp.Avatar)then
        if(Info.id==Cache.DressUpCache.avatar)then
            self:Haved();  --"已使用";
            GameController.DressUpControl:SetUsing(_index);
        else
            self:IsLock(Info);
        end

        --加载头像
        GameController.DressUpControl:ShowDressUp(Info.id,self.avatar)

        --加载当前头像框
        GameHelper.luaShowDressUpForm(-1,self.avatar_frame,DressUp.AvatarFrame,2001)
    elseif(Info.frame_type==DressUp.AvatarFrame)then
        if(Info.id==Cache.DressUpCache.avatar_frame)then
            self:Haved();   --"已使用";
        else
            self:IsLock(Info);
        end

        --加载头像框
        GameController.DressUpControl:ShowDressUp(Info.id,self.avatar_frame)

        --加载当前头像
        GameHelper.luaShowDressUpForm(-1,self.avatar,DressUp.Avatar,1001)
    elseif(Info.frame_type==DressUp.BarrageFrame)then
        if(Info.id==Cache.DressUpCache.barrage_frame)then
            self:Haved();    --"已使用";
        else
            self:IsLock(Info);
        end
    elseif(Info.frame_type==DressUp.CommentFrame)then
        if(Info.id==Cache.DressUpCache.comment_frame)then
            self:Haved();    --"已使用";
        else
            self:IsLock(Info);
        end
    end

end

--已经使用
function DressUpInfoPanel:Haved()
    self.islock=true;
    self.StateBtn.material= CS.ShaderUtil.GrayMaterial();
    local str=CS.CTextManager.Instance:GetText(423);  --已使用
    self.StateBtnText.text=str;
end



function DressUpInfoPanel:IsLock(Info)
    --是否解锁
    if(Info.is_active==0)then   --未解锁
        -- self.avatar.material= CS.ShaderUtil.GrayMaterial();
        --self.StateBtn.enabled=false;
        self.islock=true;
        self.StateBtn.material= CS.ShaderUtil.GrayMaterial();
        local str=CS.CTextManager.Instance:GetText(425);  --未解锁
        self.StateBtnText.text=str;
    elseif(Info.is_active==1)then  --已解锁
        -- self.avatar.material=nil;
        --self.StateBtn.enabled=true;
        self.StateBtn.material= nil;
        self.islock=false;
        local str=CS.CTextManager.Instance:GetText(424);  --使用
        self.StateBtnText.text=str;
    end
end



--销毁
function DressUpInfoPanel:__delete()
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return DressUpInfoPanel
