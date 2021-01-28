local LimitTimeActivityInfo = core.Class("LimitTimeActivityInfo");

function LimitTimeActivityInfo:__init()
    --【活动id】
    self.id=0;
    --【标题】
    self.title="";
    --【开始时间】
    self.start_date="";
    --【结束时间】
    self.end_date="";
    --【结束保留时间,小时】
    self.retention_time=0;
    --【倒计时】
    self.countdown=0;
    --【状态, 1开启中 0已过期】
    self.is_open=0;
    --【活动bannner图片绝对路径】
    self.img_src="";
    --【跳转位置类型 ：1.跳转外部链接（跳到jump_url）， 2.跳到events界面， 3.跳到News界面 4.打开最后书本的书本弹窗】
    self.type=0;
    --【跳转外部链接url】
    self.jump_url="";
end

function LimitTimeActivityInfo:UpdateData(data)
    self.id=data.id;
    self.title=data.title;
    self.start_date=data.start_date;
    self.end_date=data.end_date;
    self.retention_time=data.retention_time;
    self.countdown=data.countdown;
    self.is_open=data.is_open;
    self.img_src=data.img_src;
    self.type=data.type;
    self.jump_url=data.jump_url;
end

--销毁
function LimitTimeActivityInfo:__delete()
    self.id=nil;
    self.title=nil;
    self.start_date=nil;
    self.end_date=nil;
    self.retention_time=nil;
    self.countdown=nil;
    self.is_open=nil;
    self.img_src=nil;
    self.type=nil;
    self.jump_url=nil;
end


return LimitTimeActivityInfo
