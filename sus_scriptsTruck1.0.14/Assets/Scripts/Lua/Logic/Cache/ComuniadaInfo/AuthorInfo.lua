local BaseClass = core.Class
local AuthorInfo = BaseClass("AuthorInfo")

function AuthorInfo:__init()
    --id
    self.uid=0;
    --最后更新书本时间
    self.last_update_book_time=0;
    --最后更新书本id
    self.last_update_book_id=0;
    --是否在线 .0否,1是
    self.is_online=0;
    --用户信息
    self.user_info={};
    --昵称
    self.user_info.nickname="";
    --头像id
    self.user_info.avatar=0;
    --头像框
    self.user_info.avatar_frame=0;
    --评论框
    self.user_info.comment_frame=0;
    --弹幕框
    self.user_info.barrage_frame=0;
end

function AuthorInfo:UpdateData(data)
    self.uid=data.uid;
    self.last_update_book_time=data.last_update_book_time;
    self.last_update_book_id=data.last_update_book_id;
    self.is_online=data.is_online;
    self.user_info.nickname=data.user_info.nickname;
    self.user_info.avatar=data.user_info.avatar;
    self.user_info.avatar_frame=data.user_info.avatar_frame;
    self.user_info.comment_frame=data.user_info.comment_frame;
    self.user_info.barrage_frame=data.user_info.barrage_frame;
end

--销毁
function AuthorInfo:__delete()
    self.uid=nil;
    self.last_update_book_time=nil;
    self.last_update_book_id=nil;
    self.is_online=nil;
    self.user_info.nickname=nil;
    self.user_info.avatar=nil;
    self.user_info.avatar_frame=nil;
    self.user_info.comment_frame=nil;
    self.user_info.barrage_frame=nil;
    self.user_info=nil;
end


return AuthorInfo
