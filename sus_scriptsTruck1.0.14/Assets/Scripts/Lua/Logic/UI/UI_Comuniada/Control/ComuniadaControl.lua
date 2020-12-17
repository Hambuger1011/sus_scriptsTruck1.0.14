local BaseClass = core.Class
local ComuniadaControl = BaseClass("ComuniadaControl", core.Singleton)

local UIComuniadaForm=nil;
local UIMasForm=nil;

--region【构造函数】
function ComuniadaControl:__init()
    self.m_curPage=0;
    self.m_maxPage=1;
end
--endregion


--region【设置界面】

function ComuniadaControl:SetData(uicomuniada)
    UIComuniadaForm=uicomuniada;
end

function ComuniadaControl:SetData2(uimas)
    UIMasForm=uimas;
end

function ComuniadaControl:Reset()
    self.m_curPage=0;
    self.m_maxPage=1;
end

--endregion



--region 【获得的创作综合书本的信息】
function ComuniadaControl:GetwriterIndexRequest()
    logic.gameHttp:GetwriterIndex(function(result) self:GetwriterIndex(result); end)
end
--endregion


--region 【获得的创作综合书本的信息*响应】
function ComuniadaControl:GetwriterIndex(result)
    logic.debug.Log("----GetwriterIndex---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.ComuniadaCache:UpdateMostPopularList(json.data);
        Cache.ComuniadaCache:UpdateLatestReleaseList(json.data);

        if(UIComuniadaForm)then
            UIComuniadaForm:UpdateStoryList();
        end

    end
end
--endregion


--region 【创作更多界面】【获得热门书本】【获得最新书本】
function ComuniadaControl:RequestBook(page,_type)
    if (self.m_curPage >= page)then  --已经请求过
        return;
    end

    if (page> self.m_maxPage)then  --已经请求过
        return;
    end
    self.m_curPage = page;

    if(_type==EStoryList.MostPopular)then
        -- 【创作更多界面】【获得热门书本】
        logic.gameHttp:GetWriterHotBookList(page,function(result) self:GetWriterHotOrNewBookList(_type,page,result); end)
    elseif(_type==EStoryList.LatestRelease)then
        logic.gameHttp:GetWriterNewBookList(page,function(result) self:GetWriterHotOrNewBookList(_type,page,result); end)
    end

end
--endregion


--region 【创作更多界面】【获得热门书本*响应】【获得最新书本*响应】
function ComuniadaControl:GetWriterHotOrNewBookList(_type,page,result)

    logic.debug.Log("----GetWriterHotOrNewBookList---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then

         if (json.data.current_page ~= page)then --页码不匹配
            return;
        end

        if(page<=1)then
            if(json.data.per_page <= 0)then
                json.data.per_page=1;
            end
            self.m_maxPage= (json.data.total + json.data.per_page) / json.data.per_page;
        end

        --存入缓存数据；
        Cache.ComuniadaCache:UpdateHotList(json.data);
        Cache.ComuniadaCache:UpdateNewList(json.data);

        if(UIMasForm)then
            UIMasForm:UpdateWriterHotOrNewBookList(_type,page);
        end

    end
end
--endregion


--region 【获得我的写作书本列表】
function ComuniadaControl:GetMyWriterBookListRequest()
    logic.gameHttp:GetMyWriterBookList(function(result) self:GetMyWriterBookList(result); end)
end
--endregion


--region 【获得我的写作书本列表*响应】
function ComuniadaControl:GetMyWriterBookList(result)
    logic.debug.Log("----GetMyWriterBookList---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        --【自己创作故事列表】
        Cache.ComuniadaCache:UpdateMyWriterList(json);

        --刷新界面
        if(UIComuniadaForm)then
            UIComuniadaForm:UpdateMyWriterList();
        end

    end
end
--endregion

--region 【创作】【获取已读过的书本】
function ComuniadaControl:GetReadingHistoryRequest()
    logic.gameHttp:GetReadingHistory(function(result) self:GetReadingHistory(result); end)
end
--endregion


--region 【创作】【获取已读过的书本*响应】
function ComuniadaControl:GetReadingHistory(result)
    logic.debug.Log("----GetReadingHistory---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        --【历史看过的故事列表】
        Cache.ComuniadaCache:UpdateHistoryList(json);

        --刷新界面
        if(UIComuniadaForm)then
            UIComuniadaForm:UpdateHistoryList();
        end
    end
end
--endregion


--region 【创作】【切换到详情页面】
function ComuniadaControl:ChangeFavoritesPanel(result)
    --刷新界面
    if(UIComuniadaForm)then
        UIComuniadaForm:ChangeFavoritesPanel();
    end
end
--endregion






--析构函数
function ComuniadaControl:__delete()
end


ComuniadaControl = ComuniadaControl:GetInstance()
return ComuniadaControl