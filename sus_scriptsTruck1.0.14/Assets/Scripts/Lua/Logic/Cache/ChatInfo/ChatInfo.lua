local BaseClass = core.Class
local ChatInfo = BaseClass("ChatInfo")

function ChatInfo:__init()
    --对话ID
    self.id=0;
    --发信者uid
    self.from_uid=0;
    --收信者uid
    self.to_uid=0;
    --发信者内容
    self.content="";
    --创建时间
    self.create_time=0;
    --是否当前用户 1是,0否
    self.is_self=0;
    --发送者信息
    self.user_info={};
    self.user_info.uid=0;
    self.user_info.nickname="";
    self.user_info.avatar=0;
    self.user_info.avatar_frame=0;
    self.user_info.comment_frame=0;
    self.user_info.barrage_frame=0;
    self.user_info.system_type=0;
end

function ChatInfo:UpdateData(data)
    self.id=data.id;
    self.from_uid=data.from_uid;
    self.to_uid=data.to_uid;
    self.content=data.content;
    self.create_time=data.create_time;
    self.is_self=data.is_self;
    self.user_info=data.user_info;
end

--销毁
function ChatInfo:__delete()
    self.id=nil;
    self.from_uid=nil;
    self.to_uid=nil;
    self.content=nil;
    self.create_time=nil;
    self.is_self=nil;
    self.user_info=nil;
end


return ChatInfo
