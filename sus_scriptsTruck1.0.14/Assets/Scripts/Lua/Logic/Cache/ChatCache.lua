-- 单例模式
local BaseClass = core.Class
local ChatCache = BaseClass("ChatCache", core.Singleton)


-- 构造函数
function ChatCache:__init()
    ---------------------【对话列表】
    --书本总数
    self.count=0;
    --对话结果 列表
    self.ChatList= {};
    ---------------------【对话列表】
end


function ChatCache:UpdateList(datas)
    self.count=datas.count;
    self:UpdateData(datas);
end


function ChatCache:UpdateTypeList(datas)
    self.count=datas.count;
    Cache.ClearList(self.ChatList);
    self:UpdateData(datas)
end

function ChatCache:UpdateData(datas)
    local chatlist=datas.data;
    local len=table.length(chatlist);
    if(chatlist and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/ChatInfo/ChatInfo").New();
            info:UpdateData(chatlist[i]);
            table.insert(self.ChatList,info);
        end
    end
end


function ChatCache:AddNewChat(chatid,uid,content)

    local info =require("Logic/Cache/ChatInfo/ChatInfo").New();
    info.id=chatid;

    if(logic.cs.UserDataManager.userInfo and logic.cs.UserDataManager.userInfo.data)then
        --发信者uid
        info.from_uid=logic.cs.UserDataManager.userInfo.data.userinfo.uid;
        --发送者信息
        info.user_info.uid=logic.cs.UserDataManager.userInfo.data.userinfo.uid;
        info.user_info.nickname=logic.cs.UserDataManager.userInfo.data.userinfo.nickname;
        info.user_info.avatar=Cache.DressUpCache.avatar;
        info.user_info.avatar_frame=Cache.DressUpCache.avatar_frame;
        info.user_info.comment_frame=Cache.DressUpCache.comment_frame;
        info.user_info.barrage_frame=Cache.DressUpCache.barrage_frame;
    end
    --收信者uid
    info.to_uid=uid;
    --发信者内容
    info.content=content;
    --创建时间
    info.create_time=os.date("%Y-%m-%d %H:%M:%S");--当前时间
    info.is_self=1;
    table.insert(self.ChatList,info);
end


function ChatCache:ClearMas()
    if(GameHelper.islistHave(self.ChatList)==true)then
        Cache.ClearList(self.ChatList);
    end
end

-- 析构函数
function ChatCache:__delete()
end


ChatCache = ChatCache:GetInstance()
return ChatCache