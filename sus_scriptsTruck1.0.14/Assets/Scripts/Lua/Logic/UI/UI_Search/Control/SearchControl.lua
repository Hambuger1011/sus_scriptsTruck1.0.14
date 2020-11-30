local BaseClass = core.Class
local SearchControl = BaseClass("SearchControl", core.Singleton)

local UISearchForm=nil;
-- 构造函数
function SearchControl:__init()
end


function SearchControl:SetData(searchForm)
    UISearchForm=searchForm;
end

--region 【请求Book列表】【获取搜索分类的书本】
function SearchControl:GetSearchBookListRequest(_index)

    --【如果本列表有数据  就不再次请求】
    local bookList = Cache.SearchCache:GetListByIndex(_index);

    if(GameHelper.islistHave(bookList)==true)then
        if(UISearchForm)then
            UISearchForm:UpdateBookList(_index,bookList);
        end
    else
        --请求列表 【获取搜索分类的书本】
        logic.gameHttp:GetSearchBookList(_index,function(result) self:GetSearchBookList(result); end)
    end

end
--endregion


--region 【获取Book列表响应】

function SearchControl:GetSearchBookList(result)
    logic.debug.Log("----GetSearchBookList---->" .. result);

    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        local bookindex=tonumber(json.data.classify_id);
        --存入缓存数据；
        Cache.SearchCache:UpdateList(bookindex,json);

        local bookList = Cache.SearchCache:GetListByIndex(bookindex);
        if(GameHelper.islistHave(bookList)==true)then
            if(UISearchForm)then
                UISearchForm:UpdateBookList(json.data.classify_id,bookList);
            end
        end
    else
        logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end

--endregion






--析构函数
function SearchControl:__delete()

end


SearchControl = SearchControl:GetInstance()
return SearchControl