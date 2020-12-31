local BaseClass = core.Class
local EmailControl = BaseClass("EmailControl", core.Singleton)

local UIEmailForm=nil;
local UIEmailInfoForm=nil;

--region【构造函数】
function EmailControl:__init()
    self.BatchList_Email={};
    self.BatchList_Private={};


    self.email_curPage=0;
    self.email_maxPage=1;

    self.m_curPage=0;
    self.m_maxPage=1;
end
--endregion


--region【邮件页数重置】
function EmailControl:EmailReset()
    self.email_curPage=0;
    self.email_maxPage=1;
end
--endregion


--region【设置界面】

function EmailControl:SetData(uiemail)
    UIEmailForm=uiemail;
end


function EmailControl:SetEmailInfo(uiemailinfo)
    UIEmailInfoForm=uiemailinfo;
end


--endregion


--region 【获取用户的邮箱信息】
function EmailControl:GetSystemMsgRequest(page)
    if (self.email_curPage >= page)then  --已经请求过
    return;
    end

    if (page> self.email_maxPage)then  --超过最大页码数
    return;
    end
    self.email_curPage = page;
    logic.gameHttp:GetSystemMsg(page,function(result) self:GetSystemMsg(page,result); end)
end
--endregion


--region 【获取用户的邮箱信息*响应】
function EmailControl:GetSystemMsg(page,result)
    logic.debug.Log("----GetSystemMsg---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        self.email_maxPage= json.data.pages_total;

        if(page>1)then
            --【增加数据】
            Cache.EmailCache:AddEmailList(json.data);
        else
            --存入缓存数据；
            --【获取用户的邮箱信息】
            Cache.EmailCache:UpdateEmailList(json.data);
        end

        if(UIEmailForm)then
            UIEmailForm:UpdateEmail(page)
        end
    end
end
--endregion


--region 【读取用户的邮件】
function EmailControl:ReadSystemMsgRequest(msgid)
    logic.gameHttp:ReadSystemMsg(msgid,function(result) self:ReadSystemMsg(msgid,result); end)
end
--endregion


--region 【读取用户的邮件*响应】
function EmailControl:ReadSystemMsg(msgid,result)
    logic.debug.Log("----ReadSystemMsg---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --存入缓存数据；
        --【获取用户的邮箱信息】
        Cache.EmailCache:UpdateInfo(json.data);
        logic.cs.UserDataManager.selfBookInfo.data.unreadmsgcount = logic.cs.UserDataManager.selfBookInfo.data.unreadmsgcount - 1;

        --设置邮件已读
        Cache.EmailCache:SetStatus(msgid);

        --刷新红点
        GameController.MainFormControl:RedPointRequest();

        if(UIEmailInfoForm)then
            UIEmailInfoForm:SetEmailData(msgid);
        end

        if(UIEmailForm)then
            UIEmailForm:AchieveMsgPrice(msgid);
        end
    end
end
--endregion


--region 【读取用户的邮件】
function EmailControl:AchieveMsgPriceRequest(msgid)
    logic.gameHttp:AchieveMsgPrice(msgid,function(result) self:AchieveMsgPrice(msgid,result); end)
end
--endregion


--region 【读取用户的邮件*响应】
function EmailControl:AchieveMsgPrice(msgid,result)
    logic.debug.Log("----AchieveMsgPrice---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then

        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey));
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond));


        --设置邮件已领取奖励
        Cache.EmailCache:SetPriceStatus(msgid);
        --红点刷新
        GameController.MainFormControl:RedPointRequest();

        --邮件详情界面
        if(UIEmailInfoForm)then
            UIEmailInfoForm.receiveButton.gameObject:SetActiveEx(false);
        end

        if(UIEmailForm)then
            UIEmailForm:AchieveMsgPrice(msgid);
        end

    end
end
--endregion


--region 【创建评论回复】
function EmailControl:CreateBookCommentReplyRequest(comment_id, content, reply_id)
    logic.gameHttp:CreateBookCommentReply(comment_id, content, reply_id,function(result) self:CreateBookCommentReply(result); end)
end
--endregion


--region 【创建评论回复*响应】
function EmailControl:CreateBookCommentReply(result)
    logic.debug.Log("----CreateBookCommentReply---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then

        --邮件详情界面
        if(UIEmailInfoForm)then
            UIEmailInfoForm.inputField.text = "";
            UIEmailInfoForm.inputMask.gameObject:SetActiveEx(false);
        end
    end
end
--endregion


--region 【获取私信组(分页)】【跟哪些人对话的列表  boxlist】
function EmailControl:GetPrivateLetterTeamPageRequest(page)
    if (self.m_curPage >= page)then  --已经请求过
        return;
    end

    if (page> self.m_maxPage)then  --超过最大页码数
        return;
    end
    self.m_curPage = page;
    logic.gameHttp:GetPrivateLetterTeamPage(page,nil,function(result) self:GetPrivateLetterTeamPage(page,result); end)
end
--endregion


--region 【获取私信组(分页)*响应】【跟哪些人对话的列表  boxlist】
function EmailControl:GetPrivateLetterTeamPage(page,result)
    logic.debug.Log("----GetPrivateLetterTeamPage---->" .. result);
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
            Cache.EmailCache:UpdateList(json.data);
        else
            --存入缓存数据；
            Cache.EmailCache:UpdateTypeList(json.data);
        end

        if(UIEmailForm)then
            UIEmailForm:UpdateGetPrivateLetterBoxList(page)
        end
    end
end

function EmailControl:Reset()
    self.m_curPage=0;
    self.m_maxPage=1;
    self.booktypeArr="";
    self.title="";
end


--endregion


--region【批量检测】

function EmailControl:BatchTest(msgid,_type,isAdd)
    local len=0;
    if(_type==1)then  --邮件
        if(isAdd==true)then
            table.insert(self.BatchList_Email,msgid);
        else
            table.removebyvalue(self.BatchList_Email,msgid);
        end
        len =  table.length(self.BatchList_Email);
        if(UIEmailForm)then
            UIEmailForm:BatchTest(msgid,_type,len,isAdd);
        end
    elseif(_type==2)then
        if(isAdd==true)then
            table.insert(self.BatchList_Private,msgid);
        else
            table.removebyvalue(self.BatchList_Private,msgid);
        end
        len=  table.length(self.BatchList_Private);
        if(UIEmailForm)then
            UIEmailForm:BatchTest(msgid,_type,len,isAdd);
        end
    end
end

--endregion


--region【批量删除选中】
function EmailControl:DeleteSelected(_type)
    local str="";

    local len_email = table.length(self.BatchList_Email);
    if(_type==1)then
        if(len_email>0 and self.BatchList_Email)then
            for i = 1, len_email do
                Cache.EmailCache:DeleteEmail(self.BatchList_Email[i]);

                if(i==1)then
                    str=tostring(self.BatchList_Email[i]);
                else
                    str=str..","..self.BatchList_Email[i];
                end
            end
           self:DelMailRequest(str);
        end
    elseif(_type==2)then
        local len = table.length(self.BatchList_Private);
        if(len>0 and self.BatchList_Private)then
            for i = 1, len do
                Cache.EmailCache:DeletePlayer(self.BatchList_Private[i]);
                if(i==1)then
                    str=tostring(self.BatchList_Private[i]);
                else
                    str=str..","..self.BatchList_Private[i];
                end
            end
            self:DelPrivateLetterTeamRequest(str);
        end
    end
end
--endregion


--region【单个、批量删除邮件】
function EmailControl:DelMailRequest(msgid)
    logic.gameHttp:DelMail(msgid,function(result) self:DelMail(msgid,result); end)
end
--endregion


--region 【单个、批量删除邮件】
function EmailControl:DelMail(_msgid,result)
    logic.debug.Log("----DelMail---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        if(UIEmailForm)then
            UIEmailForm:UpdateEmail(1);
            UIEmailForm:DelMail(1);

            self.BatchList_Email={};
        end
    end
end
--endregion


--region【批量已读领取选中】
function EmailControl:ReadSelected(_type)
    local str="";
    local len_email = table.length(self.BatchList_Email);
    if(_type==1)then
        if(len_email>0 and self.BatchList_Email)then
            for i = 1, len_email do
                Cache.EmailCache:ReadEmail(self.BatchList_Email[i]);

                if(i==1)then
                    str=tostring(self.BatchList_Email[i]);
                else
                    str=str..","..self.BatchList_Email[i];
                end
            end
            self:BatchReadReceiveMailRequest(str);
        end
    elseif(_type==2)then
        local len = table.length(self.BatchList_Private);
        if(len>0 and self.BatchList_Private)then
            for i = 1, len do
                if(i==1)then
                    str=tostring(self.BatchList_Private[i]);
                else
                    str=str..","..self.BatchList_Private[i];
                end
            end
            self:ReadPrivateLetterTeamRequest(str,false,nil);
        end
    end
end
--endregion


--region【批量已读或领取奖励邮件】
function EmailControl:BatchReadReceiveMailRequest(msgid)
    logic.gameHttp:BatchReadReceiveMail(msgid,function(result) self:BatchReadReceiveMail(msgid,result); end)
end
--endregion


--region 【批量已读或领取奖励邮件】
function EmailControl:BatchReadReceiveMail(_msgid,result)
    logic.debug.Log("----BatchReadReceiveMail---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey));
        logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond));


        if(UIEmailForm)then
            UIEmailForm:UpdateEmail(1);
            UIEmailForm:DelMail(1);

            self.BatchList_Email={};
            Cache.EmailCache:ClearUnreadList()
        end
    end
end
--endregion


--region【单个、批量删除私信】

function EmailControl:DelPrivateLetterTeamRequest(ids)
    logic.gameHttp:DelPrivateLetterTeam(ids,function(result) self:DelPrivateLetterTeam(ids,result); end)
end

--endregion


--region 【单个、批量删除私信*响应】
function EmailControl:DelPrivateLetterTeam(ids,result)
    logic.debug.Log("----DelPrivateLetterTeam---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then

        local len = table.length(self.BatchList_Private);
        if(len>0 and self.BatchList_Private)then
            for i = 1, len do
                Cache.EmailCache:DeletePlayer(self.BatchList_Private[i]);
            end
        end


        self.BatchList_Private={};
        Cache.EmailCache:ClearUnreadList_Private()


        if(UIEmailForm)then
            UIEmailForm:UpdateGetPrivateLetterBoxList(1);
            UIEmailForm:DelMail(2);
        end

    end
end
--endregion


--region【单个、批量阅读私信组】
function EmailControl:ReadPrivateLetterTeamRequest(ids,isOne,writeUid)
    logic.gameHttp:ReadPrivateLetterTeam(ids,function(result) self:ReadPrivateLetterTeam(ids,isOne,writeUid,result); end)
end
--endregion


--region 【单个、批量阅读私信组*响应】
function EmailControl:ReadPrivateLetterTeam(ids,isOne,writeUid,result)
    logic.debug.Log("----ReadPrivateLetterTeam---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then

        --已读 是否是单条；
        if(isOne==true)then
            --修改缓存为已读
            Cache.EmailCache:SetPrivateState(ids);
            if(writeUid)then
                GameController.ChatControl:GetPrivateLetterPageRequest(writeUid,1,Cache.ComuniadaCache.WriterInfo.nickname);
            end
        else

            local len = table.length(self.BatchList_Private);
            if(len>0 and self.BatchList_Private)then
                for i = 1, len do
                    Cache.EmailCache:ReadPlayer(self.BatchList_Private[i]);
                end
            end

            Cache.EmailCache:ClearUnreadList_Private()
            self.BatchList_Private={};

            if(UIEmailForm)then
                UIEmailForm:UpdateGetPrivateLetterBoxList(1);
                UIEmailForm:DelMail(2);
            end

        end
    end
end
--endregion



--析构函数
function EmailControl:__delete()
end


EmailControl = EmailControl:GetInstance()
return EmailControl