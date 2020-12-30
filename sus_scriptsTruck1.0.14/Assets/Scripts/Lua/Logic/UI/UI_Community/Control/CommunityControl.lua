local BaseClass = core.Class
local CommunityControl = BaseClass("CommunityControl", core.Singleton)

local UICommunityForm=nil;
--region【构造函数】
function CommunityControl:__init()
    self.m_curPage=0;
end
--endregion


--region【设置界面】

function CommunityControl:SetData(uicommunity)
    UICommunityForm=uicommunity;
    self.m_curPage=0;
end

--endregion


--region 【获取作者详情】
function CommunityControl:GetWriterInfoRequest(uid)
    logic.gameHttp:GetWriterInfo(uid,function(result) self:GetWriterInfo(uid,result); end)
end
--endregion


--region 【获取作者详情*响应】
function CommunityControl:GetWriterInfo(uid,result)
    logic.debug.Log("----GetWriterInfo---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        --【获取作者详情】
        Cache.ComuniadaCache:UpdateWriterInfo(json.data)
        Cache.ComuniadaCache.WriterInfo.uid=uid;

        logic.UIMgr:Close(logic.uiid.UIBusquedaForm);

        logic.UIMgr:Open(logic.uiid.UICommunityForm)
        if(uid)then
            self:GetActionLogPageRequest(uid,1)
        end
    end
end
--endregion


--region 【获取动态列表(分页)】
function CommunityControl:GetActionLogPageRequest(uid,page)
    if (self.m_curPage >= page)then  --已经请求过
        return;
    end
    self.m_curPage = page;
    logic.gameHttp:GetActionLogPage(uid,page,function(result) self:GetActionLogPage(uid,page,result); end)
end
--endregion


--region 【获取动态列表(分页)*响应】
function CommunityControl:GetActionLogPage(uid,page,result)
    logic.debug.Log("----GetActionLogPage---->".. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        if(page>1)then
            Cache.ComuniadaCache:UpdateDynamicList(json.data);
        else
            Cache.ComuniadaCache.DynamicList=json.data.data;
            Cache.ComuniadaCache.DynamicList_Count=json.data.count;
        end
        --刷新界面
        if(UICommunityForm)then
            UICommunityForm:UpdateWriterInfo(page);
        end

        --【获取作者首页书本列表】
        self:GetWriterHomeBookListRequest(uid)
    end
end
--endregion



--region 【获取作者首页书本列表】
function CommunityControl:GetWriterHomeBookListRequest(uid)
    logic.gameHttp:GetWriterHomeBookList(uid,function(result) self:GetWriterHomeBookList(uid,result); end)
end
--endregion


--region 【获取作者首页书本列表*响应】
function CommunityControl:GetWriterHomeBookList(uid,result)
    logic.debug.Log("----GetWriterHomeBookList---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        --【作者创作故事列表】
        Cache.ComuniadaCache:UpdateWriterList(json.data);
        --【作者历史看过的故事列表】
        Cache.ComuniadaCache:UpdateWriterhistoryList(json.data);

        --刷新界面
        if(UICommunityForm)then
            UICommunityForm:UpdateWriterList();
        end
    end
end
--endregion



--region 【关注作者】
function CommunityControl:SetWriterFollowRequest(uid)
    logic.gameHttp:SetWriterFollow(uid,function(result) self:SetWriterFollow(uid,result); end)
end
--endregion


--region 【关注作者*响应】
function CommunityControl:SetWriterFollow(uid,result)
    logic.debug.Log("----SetWriterFollow---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        if(Cache.ComuniadaCache.WriterInfo)then
            Cache.ComuniadaCache.WriterInfo.is_follow=json.data.status;
            if(json.data.status==1)then
                Cache.ComuniadaCache.WriterInfo.fans_count=Cache.ComuniadaCache.WriterInfo.fans_count+1;--【粉丝+1】
            else
                Cache.ComuniadaCache.WriterInfo.fans_count=Cache.ComuniadaCache.WriterInfo.fans_count-1;--【粉丝-1】
            end
        end

        --刷新界面
        if(UICommunityForm)then
            UICommunityForm:UpdateWriterFollow();
        end
    end
end
--endregion

--region 【赞同作者】
function CommunityControl:SetWriterAgreeRequest(uid)
    logic.gameHttp:SetWriterAgree(uid,function(result) self:SetWriterAgree(uid,result); end)
end
--endregion


--region 【赞同作者*响应】
function CommunityControl:SetWriterAgree(uid,result)
    logic.debug.Log("----SetWriterAgree---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        if(Cache.ComuniadaCache.WriterInfo)then
            Cache.ComuniadaCache.WriterInfo.agree_count=json.data.agree_count;
            Cache.ComuniadaCache.WriterInfo.is_agree=json.data.status;
        end

        --刷新界面
        if(UICommunityForm)then
            UICommunityForm:UpdateWriterAgree();
        end
    end
end
--endregion



function CommunityControl:ViewMoreBtnClick()
    if(UICommunityForm)then
        UICommunityForm:ViewMoreBtnClick();
    end
end



--region 【delete】
function CommunityControl:__delete()
end
--endregion


CommunityControl = CommunityControl:GetInstance()
return CommunityControl