local BaseClass = core.Class
local DressUpInfo = BaseClass("DressUpInfo")

function DressUpInfo:__init()
    --id
    self.id=0;
    --书类型, 1头像,2头像框,3评论框,4弹幕框
    self.frame_type="";
    --是否可选用. 1为是,0为否
    self.is_active=0;
    --是此道具一般时效天数
    self.expire_day=0;
    --有效时间, 如果为0是无限,如果大于0为有效时间戳
    self.expire_time=0;
end

function DressUpInfo:UpdateData(data)
    self.id=data.id;
    self.frame_type=data.frame_type;
    self.is_active=data.is_active;
    self.expire_day=data.expire_day;
    self.expire_time=data.expire_time;
end

--销毁
function DressUpInfo:__delete()
    self.id=nil;
    self.frame_type=nil;
    self.is_active=nil;
    self.expire_day=nil;
    self.expire_time=nil;
end


return DressUpInfo
