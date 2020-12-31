-- 单例模式
local BaseClass = core.Class
local EmailCache = BaseClass("EmailCache", core.Singleton)


-- 构造函数
function EmailCache:__init()
    ---------------------【邮件】
    --邮箱总数量
    self.sysarr_count=0;
    --总页数
    self.pages_total=0;
    --邮件 列表
    self.EmailList= {};
    ---------------------【邮件】

    ---------------------【信鸽】
    --信鸽总数量
    self.count=0;
    --信鸽 列表
    self.PlayerChatList= {};
    ---------------------【信鸽】
end


--作者历史看过的故事列表
function EmailCache:UpdateEmailList(datas)
    self.sysarr_count=datas.sysarr_count;
    self.pages_total=datas.pages_total;

    if(GameHelper.islistHave(self.EmailList)==true)then
        Cache.ClearList(self.EmailList);
    end
    local eList=datas.sysarr;
    local len=table.length(eList);
    if(eList and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/EmailInfo/EmailInfo").New();
            info:UpdateData(eList[i]);
            table.insert(self.EmailList,info);
        end
    end
end

function EmailCache:AddEmailList(datas)
    self.sysarr_count=datas.sysarr_count;
    self.pages_total=datas.pages_total;

    if(GameHelper.islistHave(self.EmailList)==true)then
        local eList=datas.sysarr;
        local len=table.length(eList);
        if(eList and len>0)then
            for i = 1,len do
                local info =require("Logic/Cache/EmailInfo/EmailInfo").New();
                info:UpdateData(eList[i]);
                table.insert(self.EmailList,info);
            end
        end
    end
end



function EmailCache:UpdateInfo(info)

    if(GameHelper.islistHave(self.EmailList)==true)then
        local len=table.length(self.EmailList);
        for i = 1, len do
            if(self.EmailList[i].msgid==info.sysarr.msgid)then
                self.EmailList[i]:UpdateData(info.sysarr);
                if(self.EmailList[i].msg_type~=2)then
                    self.EmailList[i].status=1;
                end
            end
        end
    end
end


function EmailCache:SetStatus(msgid)
    if(GameHelper.islistHave(self.EmailList)==true)then
        local len=table.length(self.EmailList);
        for i = 1, len do
            if(self.EmailList[i].msgid==msgid)then
                self.EmailList[i].status=1;
            end
        end
    end
end


function EmailCache:SetPriceStatus(msgid)
    if(GameHelper.islistHave(self.EmailList)==true)then
        local len=table.length(self.EmailList);
        for i = 1, len do
            if(self.EmailList[i].msgid==msgid)then
                self.EmailList[i].price_status=1;
            end
        end
    end
end

function EmailCache:GetInfoById(msgid)
    if(GameHelper.islistHave(self.EmailList)==true)then
        local len=table.length(self.EmailList);
        for i = 1, len do
            if(self.EmailList[i].msgid==msgid)then
               return self.EmailList[i];
            end
        end
    end
    return nil;
end


local UnreadList={};
function EmailCache:IsUnread(msgid,isAdd)
    if(msgid==nil)then return false; end
    if(isAdd==true)then
        if(GameHelper.islistHave(self.EmailList)==true)then
            local len=table.length(self.EmailList);
            for i = 1, len do
                if(self.EmailList[i].msgid==msgid and self.EmailList[i].status==0)then
                    table.insert(UnreadList,msgid);
                    break;
                end
            end
        end

    else
        table.removebyvalue(UnreadList,msgid);
    end

    local lens=table.length(UnreadList);
    if(lens>0)then
        return true;
    end

    return false;
end


function EmailCache:ClearUnreadList()
    UnreadList={};
end



local UnreadList_Private={};
function EmailCache:IsUnreadPrivate(msgid,isAdd)
    if(msgid==nil)then return false; end
    if(isAdd==true)then
        if(GameHelper.islistHave(self.PlayerChatList)==true)then
            local len=table.length(self.PlayerChatList);
            for i = 1, len do
                if(self.PlayerChatList[i].id==msgid and self.PlayerChatList[i].is_read==0)then
                    table.insert(UnreadList_Private,msgid);
                    break;
                end
            end
        end

    else
        table.removebyvalue(UnreadList_Private,msgid);
    end

    local lens=table.length(UnreadList_Private);
    if(lens>0)then
        return true;
    end

    return false;
end


function EmailCache:ClearUnreadList_Private()
    UnreadList_Private={};
end



--信鸽人物列表
function EmailCache:UpdateList(datas)
    self.count=datas.count;

    local playerchatList=datas.data;
    local len=table.length(playerchatList);
    if(playerchatList and len>0)then
        for i = 1,len do
            table.insert(self.PlayerChatList,playerchatList[i]);
        end
    end
end

function EmailCache:UpdateTypeList(datas)
    self.count=datas.count;
    self.PlayerChatList={};
    local playerchatList=datas.data;
    local len=table.length(playerchatList);
    if(playerchatList and len>0)then
        for i = 1,len do
            table.insert(self.PlayerChatList,playerchatList[i]);
        end
    end
end


--读取领取邮件
function EmailCache:ReadEmail(msgid)
    if(GameHelper.islistHave(self.EmailList)==true)then
        local len=table.length(self.EmailList);
        for i = 1, len do
            if(self.EmailList[i].msgid==msgid)then
                self.EmailList[i].status=1;
                self.EmailList[i].price_status=1;
                break;
            end
        end
    end
end

--读取领取邮件
function EmailCache:ReadPlayer(msgid)
    if(GameHelper.islistHave(self.PlayerChatList)==true)then
        local len=table.length(self.PlayerChatList);
        for i = 1, len do
            if(self.PlayerChatList[i].id==msgid)then
                self.PlayerChatList[i].is_read=1;
                break;
            end
        end
    end
end


--删除邮件
function EmailCache:DeleteEmail(msgid)
    if(GameHelper.islistHave(self.EmailList)==true)then
        local len=table.length(self.EmailList);
        for i = 1, len do
            if(self.EmailList[i].msgid==msgid)then
                 table.remove(self.EmailList, i)
                 break;
            end
        end
    end
end


--删除信鸽
function EmailCache:DeletePlayer(msgid)
    if(GameHelper.islistHave(self.PlayerChatList)==true)then
        local len=table.length(self.PlayerChatList);
        for i = 1, len do
            if(self.PlayerChatList[i].id==msgid)then
                table.remove(self.PlayerChatList, i)
                break;
            end
        end
    end
end

function EmailCache:SetPrivateState(id)
    if(GameHelper.islistHave(self.PlayerChatList)==true)then
        local len=table.length(self.PlayerChatList);
        for i = 1, len do
            if(self.PlayerChatList[i].id==id)then
                self.PlayerChatList[i].is_read=1;
            end
        end
    end
end






-- 析构函数
function EmailCache:__delete()
end


EmailCache = EmailCache:GetInstance()
return EmailCache