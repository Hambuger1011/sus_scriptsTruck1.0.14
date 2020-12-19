local BaseClass = core.Class
local CommunityControl = BaseClass("CommunityControl", core.Singleton)

local UICommunityForm=nil;
--region【构造函数】
function CommunityControl:__init()
end
--endregion


--region【设置界面】

function CommunityControl:SetData(uicommunity)
    UICommunityForm=uicommunity;
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
        Cache.ComuniadaCache.WriterInfo=json.data;

        logic.UIMgr:Open(logic.uiid.UICommunityForm)

        if(uid)then
            self:GetActionLogPageRequest(uid,1)
        end
    end
end
--endregion


--region 【获取动态列表(分页)】
function CommunityControl:GetActionLogPageRequest(uid,page)
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
        --【获取作者详情】
        Cache.ComuniadaCache.DynamicList=json.data;

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



--region 【delete】
function CommunityControl:__delete()
end
--endregion


CommunityControl = CommunityControl:GetInstance()
return CommunityControl