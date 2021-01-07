local BaseClass = core.Class
local ChatControl = BaseClass("ChatControl", core.Singleton)

local UIChatForm=nil;

--region【构造函数】

function ChatControl:__init()
    self.m_curPage=0;
    self.m_maxPage=1;
end

--endregion


--region【设置界面】

function ChatControl:SetData(uichat)
    UIChatForm=uichat;
end


function ChatControl:Reset()
    self.m_curPage=0;
    self.m_maxPage=1;
end
--endregion


--region 【获取私信对话列表(分页)】【跟某个人单独聊天的具体内容】

function ChatControl:GetPrivateLetterPageRequest(uid,page,nickname)
    if (self.m_curPage >= page)then  --已经请求过
        return;
    end

    if (page> self.m_maxPage)then  --超过最大页码数
        return;
    end
    self.m_curPage = page;
    logic.gameHttp:GetPrivateLetterPage(uid,page,nil,function(result) self:GetPrivateLetterPage(uid,page,nickname,result); end)
end

--endregion


--region 【获取私信对话列表(分页)*响应】【跟某个人单独聊天的具体内容】

function ChatControl:GetPrivateLetterPage(uid,page,nickname,result)
    logic.debug.Log("----GetPrivateLetterPage---->" .. result);
    local json = core.json.Derialize(result);
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
            Cache.ChatCache:UpdateList(json.data);
        else
            --存入缓存数据；
            Cache.ChatCache:UpdateTypeList(json.data);
        end

        -- 【获取信鸽可用次数】
        self:GetFreePrivateLetterCountRequest(uid,page,nickname);

    end
end

--endregion


--region 【获取信鸽可用次数】
function ChatControl:GetFreePrivateLetterCountRequest(uid,page,nickname)
    logic.gameHttp:GetFreePrivateLetterCount(function(result) self:GetFreePrivateLetterCount(result,uid,page,nickname); end)
end
--endregion


--region 【获取信鸽可用次数*响应】
function ChatControl:GetFreePrivateLetterCount(result,uid,page,nickname)
    logic.debug.Log("----GetFreePrivateLetterCount---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        Cache.ChatCache:UpdateChatCount(json.data);

        if(UIChatForm)then
            UIChatForm:UpdateChatInfo(uid,page,nickname)
        else
            local uichat= logic.UIMgr:Open(logic.uiid.UIChatForm);
            if(uichat)then
                uichat:UpdateChatInfo(uid,page,nickname)
            end
        end

    end
end
--endregion



--region 【发送信鸽】
function ChatControl:SendWriterLetterRequest(uid,content)
    logic.gameHttp:SendWriterLetter(uid,content,function(result) self:SendWriterLetter(uid,content,result); end)
end
--endregion


--region 【发送信鸽*响应】
function ChatControl:SendWriterLetter(uid,content,result)
    logic.debug.Log("----SendWriterLetter---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then

        --存入缓存数据；
        --【获取用户的邮箱信息】
        Cache.ChatCache:AddNewChat(json.data.new_id,uid,content);

        --刷新发送信鸽的次数
        Cache.ChatCache:UpdateChatCount(json.data);

        if(UIChatForm)then
            UIChatForm:UpdateChatInfo(uid,nil);
            UIChatForm:SendSuccess();
        end
    end
end
--endregion



--析构函数
function ChatControl:__delete()
end


ChatControl = ChatControl:GetInstance()
return ChatControl