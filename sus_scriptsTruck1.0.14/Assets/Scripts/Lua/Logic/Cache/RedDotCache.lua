-- 单例模式
local BaseClass = core.Class
local RedDotCache = BaseClass("RedDotCache", core.Singleton)


-- 构造函数
function RedDotCache:__init()
    --消息码
    self.code=0;
    --【任务完成未领取的数量】
    self.task_receive=0;
    --【未领取奖励的邮件数】
    self.unreceive_msg_count=0;
    --【未读的邮件数】
    self.unread_msg_count=0;
    --【今天是否签到的红点，1已签到，0未签到】
    self.is_login_receive=0;
    --【每天第一次登陆有任务未完成时显示红点，1.今天已提示过 0.未提示过】
    self.first_task_notice=0;
    --【在线阅读未领取奖励 1有未领取，0没有】
    self.unreceive_reading_task=0;
    --【第三方登录绑定奖励未领取 1有未领取，0没有】
    self.third_party_award=0;
    --【关注社媒奖励未领取: 1.有未领取 0.没有】
    self.attention_media=0;
    --【用户迁移奖励未领取: 1.有未领取 0.没有】
    self.se_move_finish=0;



    --红点标识 【主界面Gif】【t活动按钮上  的红点】【开关】
    self.UIMainFormRed=false;
    --红点标识 【主界面底栏】【活动按钮上  的红点】【开关】
    self.ActivityRed=false;
    --红点标识 【活动页面里】【常规活动 Activity标签  的红点】【开关】
    self.ActivityPanelRed=false;
    --红点标识 【活动页面里】【限时活动 LimitedTime标签  的红点】【开关】
    self.LimitedTimePanelRed=false;
    --红点标识 【活动页面里】【限时活动 News标签  的红点】【开关】
    self.NewsPanelRed=false;
    --红点标识 【活动页面里】【限时活动页】【绑定按钮红点】【开关】
    self.BindRedPoint=false;
    --红点标识 【活动页面里】【限时活动页】【关注社媒有礼红点】【开关】
    self.FollowRedPoint=false;
    --红点标识 【活动页面里】【限时活动页】【全书免费红点】【开关】
    self.FreeRedPoint=false;
    --红点标识 【活动页面里】【限时活动页】【用户迁移红点】【开关】
    self.MoveRedPoint=false;
    --红点标识 【活动页面里】【限时活动页】【首充奖励红点】【开关】
    self.FirstRechargePoint=false;
end

function RedDotCache:UpdateData(data)
    self.task_receive=data.task_receive;
    self.unreceive_msg_count=data.unreceive_msg_count;
    self.unread_msg_count=data.unread_msg_count;
    self.is_login_receive=data.is_login_receive;
    self.first_task_notice=data.first_task_notice;
    self.unreceive_reading_task=data.unreceive_reading_task;
    self.third_party_award=data.third_party_award;
    self.attention_media=data.attention_media;
    self.se_move_finish=data.se_move_finish;
end


-- 析构函数
function RedDotCache:__delete()
    self.task_receive=nil;
    self.unreceive_msg_count=nil;
    self.unread_msg_count=nil;
    self.is_login_receive=nil;
    self.first_task_notice=nil;
    self.unreceive_reading_task=nil;
    self.third_party_award=nil;
    self.attention_media=nil;
    self.attention_media=nil;
end


RedDotCache = RedDotCache:GetInstance()
return RedDotCache