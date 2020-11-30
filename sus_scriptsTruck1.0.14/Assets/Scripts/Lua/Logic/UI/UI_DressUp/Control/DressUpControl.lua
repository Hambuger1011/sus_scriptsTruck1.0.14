local BaseClass = core.Class
local DressUpControl = BaseClass("DressUpControl", core.Singleton)

local UIDressUpForm=nil;
-- 构造函数
function DressUpControl:__init()
    self.usinglist={};
    self.isShow=false;
end


--region【Using状态切换】
function DressUpControl:ClearUsinglist()
    if(self.usinglist)then
        local len= table.length(self.usinglist);
        if(len>0)then
            for i = 1, len do
                if(self.usinglist[i])then
                    self.usinglist[i]:SetActive(false);
                    break;
                end
            end
            self.usinglist={};
        end
    end
end

function DressUpControl:AddUsinglist(obj)
    if(self.usinglist)then
        table.insert(self.usinglist,obj);
    end
end
--endregion


--region【SetData】
function DressUpControl:SetData(dressupForm)
    UIDressUpForm=dressupForm;
    self.usinglist={};
    if(dressupForm)then
        self.isShow=true;
    else
        self.isShow=false;
    end
end
--endregion


--region 【获取用户头像及框列表】
--旧的时间
local oldTime=0;

function DressUpControl:GetUserFrameListRequest(_type)
    --请求列表 【获取用户头像及框列表】
    logic.gameHttp:GetUserFrameList(function(result) self:GetUserFrameList(result,_type); end)
end

function DressUpControl:TimerRequest(_type)
    --local curTime = os.time();
    ----时间差 大于800秒
    --local _sunTime =curTime-oldTime;
    --if (_sunTime > 800 and oldTime > 0)then
    --    --请求列表 【获取用户头像及框列表】
    --    logic.gameHttp:GetUserFrameList(function(result) self:GetUserFrameList(result,_type); end)
    --else
    --    if(oldTime==0)then
    --        --请求列表 【获取用户头像及框列表】
    --        logic.gameHttp:GetUserFrameList(function(result) self:GetUserFrameList(result,_type); end)
    --    else
    --        self:Request(_type);
    --    end
    --end
    --oldTime = curTime;
end



function DressUpControl:Request(_type)
    --【如果本列表有数据  就不再次请求】
    local dressupList = Cache.DressUpCache:GetListByType(_type);

    if(GameHelper.islistHave(dressupList)==true)then
        if(UIDressUpForm)then
            UIDressUpForm:UpdateList(dressupList,_type);
        end
    else
        --请求列表 【获取用户头像及框列表】
        logic.gameHttp:GetUserFrameList(function(result) self:GetUserFrameList(result,_type); end)
    end
end

--endregion


--region 【获取用户头像及框列表 响应】

function DressUpControl:GetUserFrameList(result,_type)
    logic.debug.Log("----GetUserFrameList---->" .. result);

    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.DressUpCache:UpdateData(json.data);

        local dressupList = Cache.DressUpCache:GetListByType(_type);
        if(GameHelper.islistHave(dressupList)==true)then
            if(UIDressUpForm)then
                UIDressUpForm:UpdateList(dressupList,_type);
            end
        end
    else
        logic.debug.LogError("----GetUserFrameList----> error...");
       -- logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end

--endregion


--region【点击 头像或框 展示详细信息】
function DressUpControl:SetHeadInfoData(_info,obj,_index)
    if(UIDressUpForm)then
        UIDressUpForm:SetHeadInfoData(_info,obj,_index);
    end
end

function DressUpControl:SetUsing(_index)
    if(UIDressUpForm)then
        UIDressUpForm:SetUsing(_index);
    end
end


--endregion


--region【展示装扮 头像Avatar | 头像框Avatarframe | 弹幕框Barrageframe | 评论框Commentframe 】
function DressUpControl:ShowDressUp(id,_image)
    if(id and id>0)then
        --读取配置表
        local config_info=DataConfig.Q_DressUpData:GetMapData(id);
        if(config_info)then
            --加载图片
            _image.sprite = CS.ResourceManager.Instance:GetUISprite("DressUpForm/"..config_info.resources);
        else
            logic.debug.LogError("DressUpControl:ShowDressUp config is null...");
        end
    end
end
--endregion


--region【设置用户头像】
function DressUpControl:SetUserAvatarRequest(_type,dressupID,_index)
    local curAvatarID=Cache.DressUpCache.avatar;
    local curAvatarFrameID=Cache.DressUpCache.avatar_frame;
    local curBarrageFrameID=Cache.DressUpCache.barrage_frame;
    local curCommentFrameID=Cache.DressUpCache.comment_frame;
    if(_type==DressUp.Avatar)then
        if(curAvatarID==nil or curAvatarID==0)then curAvatarID=1001; else curAvatarID=dressupID; end
    elseif(_type==DressUp.AvatarFrame)then
        if(curAvatarFrameID==nil or curAvatarFrameID==0)then curAvatarFrameID=2001; else curAvatarFrameID=dressupID; end
    elseif(_type==DressUp.BarrageFrame)then
        if(curBarrageFrameID==nil or curBarrageFrameID==0)then curBarrageFrameID=3001; else curBarrageFrameID=dressupID; end
    elseif(_type==DressUp.CommentFrame)then
        if(curCommentFrameID==nil or curCommentFrameID==0)then curCommentFrameID=4001; else curCommentFrameID=dressupID; end
    end


    --请求 【设置用户头像(SUS)】
    logic.gameHttp:SetUserAvatar(curAvatarID,curAvatarFrameID,curBarrageFrameID,curCommentFrameID,function(result) self:SetUserAvatar(result,_type,dressupID,_index); end)
end
--endregion


--region【SetUserAvatar】
function DressUpControl:SetUserAvatar(result,_type,dressupID,_index)
    logic.debug.Log("----SetUserAvatar---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        if(dressupID and dressupID>0)then
            --【个人中心界面 如果正在打开】
            local profileform = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.ProfileForm):GetComponent(typeof(CS.ProfileForm))

            if(_type==DressUp.Avatar)then
                --存入缓存数据；
                Cache.DressUpCache.avatar=dressupID;
                --【个人中心界面 如果正在打开】
                if(profileform)then
                    profileform:ShowAvatar(dressupID);
                end
            elseif(_type==DressUp.AvatarFrame)then
                Cache.DressUpCache.avatar_frame=dressupID;
                --【个人中心界面 如果正在打开】
                if(profileform)then
                    profileform:ShowAvatarframe(dressupID);
                end
            elseif(_type==DressUp.BarrageFrame)then
                Cache.DressUpCache.barrage_frame=dressupID;
            elseif(_type==DressUp.CommentFrame)then
                Cache.DressUpCache.comment_frame=dressupID;
            end
            --刷新状态
            if(UIDressUpForm)then
                UIDressUpForm:Haved(_index);
            end


        end
    else
        logic.debug.LogError("----GetUserFrameList----> error...");
        -- logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion

--析构函数
function DressUpControl:__delete()

end


DressUpControl = DressUpControl:GetInstance()
return DressUpControl