local BaseClass = core.Class
local EmailControl = BaseClass("EmailControl", core.Singleton)

local UIEmailForm=nil;


--region【构造函数】
function EmailControl:__init()
end
--endregion


--region【设置界面】

function EmailControl:SetData(uiemail)
    UIEmailForm=uiemail;
end

--endregion


--region 【获取用户的邮箱信息】
function EmailControl:GetSystemMsgRequest(page)
    logic.gameHttp:GetSystemMsg(page,function(result) self:GetSystemMsg(page,result); end)
end
--endregion


--region 【获取用户的邮箱信息*响应】
function EmailControl:GetSystemMsg(page,result)
    logic.debug.Log("----GetSystemMsg---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        --【获取用户的邮箱信息】
        Cache.EmailCache:UpdateEmailList(json.data);

        if(UIEmailForm)then
            UIEmailForm:UpdateEmail(page)
        end
    end
end
--endregion



--析构函数
function EmailControl:__delete()
end


EmailControl = EmailControl:GetInstance()
return EmailControl