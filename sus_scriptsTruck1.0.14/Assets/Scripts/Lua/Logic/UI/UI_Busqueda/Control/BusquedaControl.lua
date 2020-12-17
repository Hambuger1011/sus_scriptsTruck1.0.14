local BaseClass = core.Class
local BusquedaControl = BaseClass("BusquedaControl", core.Singleton)

local UIBusquedaForm=nil;

--region【构造函数】
function BusquedaControl:__init()
    self.m_curPage=0;
    self.m_maxPage=1;

    self.booktypeArr="";
    self.title="";
end
--endregion


--region【设置界面】

function BusquedaControl:SetData(uibusqueda)
    UIBusquedaForm=uibusqueda;
end

--endregion


--region 【创作更多界面】【获得热门书本】【获得最新书本】
function BusquedaControl:RequestBook(page,booktypeArr,title)
    if (self.m_curPage >= page)then  --已经请求过
        return;
    end

    if (page> self.m_maxPage)then  --超过最大页码数
        return;
    end

    if(booktypeArr)then
        self.booktypeArr=booktypeArr;
    end

    if(title)then
        self.title=title;
    end

    self.m_curPage = page;
    -- 【默认书本搜索列表(浏览量排序)】
    logic.gameHttp:GetWriterBookList(page,self.booktypeArr,self.title,function(result) self:GetWriterBookList(page,result); end)
end
--endregion


--region 【创作更多界面】【获得默认书本搜索列表(浏览量排序)*响应】
function BusquedaControl:GetWriterBookList(page,result)

    logic.debug.Log("----GetWriterBookList---->" .. result);
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

        if(page>1)then
            --存入缓存数据；
            Cache.BusquedaCache:UpdateList(json.data);
        else
            --存入缓存数据；
            Cache.BusquedaCache:UpdateTypeList(json.data);
        end

        if(UIBusquedaForm)then
            UIBusquedaForm:UpdateWriterBookList(page,self.booktypeArr);
        end
    end
end

function BusquedaControl:Reset()
    self.m_curPage=0;
    self.m_maxPage=1;
    self.booktypeArr="";
    self.title="";
end

--endregion


--region 【delete】
function BusquedaControl:__delete()
end
--endregion


BusquedaControl = BusquedaControl:GetInstance()
return BusquedaControl