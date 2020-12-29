-- 单例模式
local BaseClass = core.Class
local DressUpCache = BaseClass("DressUpCache", core.Singleton)


-- 构造函数
function DressUpCache:__init()
    --【所有装扮框列表】
    self.DressUplist= {};

    --【当前正在装扮的】
    --头像id
    self.avatar= nil;
    --头像框
    self.avatar_frame= nil;
    --评论框
    self.comment_frame= nil;
    --弹幕框
    self.barrage_frame= nil;
end

--更新 列表
function DressUpCache:UpdateData(data)
    Cache.ClearList(self.DressUplist);
    local _list=data.list;
    local len=table.length(_list);
    if(_list and len>0)then
        for i = 1,len do
            local dressupinfo =require("Logic/Cache/DressUpInfo/DressUpInfo").New();
            dressupinfo:UpdateData(_list[i]);
            table.insert(self.DressUplist,dressupinfo)
        end
    end
end

--【重新登录】
function DressUpCache:ResetLogin()

    --【当前正在装扮的】
    --头像id
    self.avatar = logic.cs.UserDataManager.userInfo.data.userinfo.avatar;
    --头像框
    self.avatar_frame = logic.cs.UserDataManager.userInfo.data.userinfo.avatar_frame;
    --评论框
    self.comment_frame = logic.cs.UserDataManager.userInfo.data.userinfo.comment_frame;
    --弹幕框
    self.barrage_frame = logic.cs.UserDataManager.userInfo.data.userinfo.barrage_frame;

    local accountInfo = logic.UIMgr:GetView2(logic.uiid.AccountInfo);
    if(accountInfo)then
        --【加载头像、头像框】
        accountInfo:ShowAvatar();
    end


    --【活动界面重置排序】
    GameController.ActivityControl:PanelSort();
    --【获取任务列表】【每日任务】
    GameController.ActivityControl:GetMyTaskListRequest();
    --刷新红点状态
    GameController.MainFormControl:RedPointRequest();
    --【限时活动】【刷新】【绑定有礼】
    GameController.ActivityControl:SetBindStatus()
    --【限时活动】【刷新】【关注有礼】
    GameController.ActivityControl:SetFollowStatus()
    --【限时活动】【刷新】【迁移奖励】
    GameController.ActivityControl:SetMoveRewardStatus()
    --【限时活动】【刷新】【全书免费】
    GameController.ActivityControl:SetFreeBG()
    -- 【请求我的书本】
    GameController.MainFormControl:GetSelfBookInfoRequest();
end



function DressUpCache:GetListByType(_type)
    local _list={};
    local len=table.length(self.DressUplist);
    if(len > 0)then
        for i = 1, len do
            if(self.DressUplist[i].frame_type==_type)then
                table.insert(_list,self.DressUplist[i]);
            end
        end
    end
    return _list;
end



function DressUpCache:__delete()
    Cache.ClearList(self.DressUplist);
    self.DressUplist= nil;
end


DressUpCache = DressUpCache:GetInstance()
return DressUpCache