-- 单例模式
local BaseClass = core.Class
local ReadTimeCache = BaseClass("ReadTimeCache", core.Singleton)


-- 构造函数
function ReadTimeCache:__init()
    --消息码
    self.code=0;

    --【已阅读秒数】
    self.online_time=0;
    --【已完成级别】
    self.finish_level=0;
    --【已领取奖品级别】
    self.receive_level=0;

    self.isEND=false;
end

--更新 列表
function ReadTimeCache:UpdateData(data)
    self.online_time=data.online_time;
    self.finish_level=tonumber(data.finish_level);
    self.receive_level=data.receive_level;

    --设置开关
    self:SetEnd();
end

function ReadTimeCache:SetEnd()
    if(self.online_time>=3600 or self.finish_level==5 or self.receive_level==5)then
        self.isEND=true;
    end
end


function ReadTimeCache:__delete()
    self.code=nil;
    self.online_time=nil;
    self.finish_level=nil;
    self.receive_level=nil;
end


ReadTimeCache = ReadTimeCache:GetInstance()
return ReadTimeCache