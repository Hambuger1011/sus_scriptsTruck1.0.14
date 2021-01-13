local BaseClass = core.Class
local EmailInfo = BaseClass("EmailInfo")

function EmailInfo:__init()
    --邮件ID
    self.msgid=0;
    --邮件标题
    self.title="";
    --邮件内容
    self.content="";
    --邮件创建时间
    self.createtime="";
    --邮件类型 1系统消息 2奖励信息 3问卷调查 4.书本用户回复
    self.msg_type=0;
    --图片地址
    self.email_pic="";
    --图片跳转地址
    self.email_url="";
    --奖励领取状态 0未领取 1已领取
    self.price_status=0;
    --是否读取  0否 1是
    self.status=0;
    --当msg_type=4返回，书本id
    self.book_id=0;
    --当msg_type=4返回，书名
    self.book_name="";
    --当msg_type=4返回，评论者的昵称
    self.comment_nickname="";
    --当msg_type=4返回，评论者的头像
    self.comment_face_icon="";
    --当msg_type=4返回，评论者的内容
    self.comment_content="";
    --当msg_type=4返回，评论的人是不是自己 1：是 0：否
    self.comment_is_self="";
    --当msg_type=4返回，被回复者的内容
    self.replied_content="";
    --当msg_type=4返回，被回复者的昵称
    self.replied_nickname="";
    --当msg_type=4返回，回复的是否是自己 1：是  0：否
    self.replied_is_self="";
    --当msg_type=3返回，点击问卷按钮的跳转链接
    self.button_link="";
    --当msg_type=3返回，按钮名称
    self.button_name="";
    --当msg_type=2返回，奖励钥匙数量
    self.price_bkey=0;
    --当msg_type=2返回，奖励钻石数量
    self.price_diamond=0;
    --当msg_type=4返回，评论id
    self.comment_id="";
    --当msg_type=4返回，评论的人是不是自己 1：是 0：否
    self.comment_is_self="";
    --当msg_type=4返回，回复id
    self.reply_id="";
    --当msg_type=4返回，回复的是否是自己 1：是  0：否
    self.replied_is_self="";


    self.comment_avatar=0;
    self.comment_avatar_frame=0;
    self.comment_comment_frame=0;
    self.item_list={};
end

function EmailInfo:UpdateData(data)
    self.msgid=data.msgid;
    self.title=data.title;
    self.content=data.content;
    self.createtime=data.createtime;
    self.msg_type=data.msg_type;
    self.email_pic=data.email_pic;
    self.email_url=data.email_url;
    self.price_status=data.price_status;
    self.status=data.status;
    self.book_id=data.book_id;
    self.book_name=data.book_name;
    self.comment_nickname=data.comment_nickname;
    self.comment_face_icon=data.comment_face_icon;
    self.comment_content=data.comment_content;
    self.comment_is_self=data.comment_is_self;
    self.replied_content=data.replied_content;
    self.replied_nickname=data.replied_nickname;
    self.replied_is_self=data.replied_is_self;
    self.button_link=data.button_link;
    self.button_name=data.button_name;
    self.price_bkey=data.price_bkey;
    self.price_diamond=data.price_diamond;
    self.comment_id=data.comment_id;
    self.comment_is_self=data.comment_is_self;
    self.reply_id=data.reply_id;
    self.replied_is_self=data.replied_is_self;
    self.comment_avatar=data.comment_avatar;
    self.comment_avatar_frame=data.comment_avatar_frame;
    self.comment_comment_frame=data.comment_comment_frame;
    self.item_list=data.item_list;
end

--销毁
function EmailInfo:__delete()
    self.msgid=nil;
    self.title=nil;
    self.content=nil;
    self.createtime=nil;
    self.msg_type=nil;
    self.email_pic=nil;
    self.email_url=nil;
    self.price_status=nil;
    self.status=nil;
    self.book_id=nil;
    self.book_name=nil;
    self.comment_nickname=nil;
    self.comment_face_icon=nil;
    self.comment_content=nil;
    self.comment_is_self=nil;
    self.replied_content=nil;
    self.replied_nickname=nil;
    self.replied_is_self=nil;
    self.button_link=nil;
    self.button_name=nil;
    self.price_bkey=nil;
    self.price_diamond=nil;
    self.comment_id=nil;
    self.comment_is_self=nil;
    self.reply_id=nil;
    self.replied_is_self=nil;
    self.item_list={};
end


return EmailInfo
