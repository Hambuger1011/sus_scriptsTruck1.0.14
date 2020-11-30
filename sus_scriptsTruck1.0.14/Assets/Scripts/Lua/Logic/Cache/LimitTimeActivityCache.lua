--【限时活动】

-- 单例模式
local BaseClass = core.Class
local LimitTimeActivityCache = BaseClass("LimitTimeActivityCache", core.Singleton)

-- 构造函数
--【限时活动】
function LimitTimeActivityCache:__init()

    --【彩蛋】
    self.ColoredEgg={};
    --【彩蛋】【活动id ,待定】
    self.ColoredEgg.id=0;
    --【彩蛋】【标题】
    self.ColoredEgg.title="";
    --【彩蛋】【开始时间】
    self.ColoredEgg.start_date="";
    --【彩蛋】【结束时间】
    self.ColoredEgg.end_date="";
    --【彩蛋】【结束保存时间,预留】
    self.ColoredEgg.retention_time=0;
    --【彩蛋】【参数,预留】
    self.ColoredEgg.params="";
    --【彩蛋】【倒计时】
    self.ColoredEgg.countdown=0;
    --【彩蛋】【状态, 1开启中 0已过期】
    self.ColoredEgg.is_open=0;

    --【旋转抽奖】
    self.SpinDraw={};
    --【旋转抽奖】【活动id ,待定】
    self.SpinDraw.id=0;
    --【旋转抽奖】【标题】
    self.SpinDraw.title="";
    --【旋转抽奖】【开始时间】
    self.SpinDraw.start_date="";
    --【旋转抽奖】【结束时间】
    self.SpinDraw.end_date="";
    --【旋转抽奖】【结束保存时间,预留】
    self.SpinDraw.retention_time=0;
    --【旋转抽奖】【参数,预留】
    self.SpinDraw.params="";
    --【旋转抽奖】【倒计时】
    self.SpinDraw.countdown=0;
    --【旋转抽奖】【状态, 1开启中 0已过期】
    self.SpinDraw.is_open=0;


    --【免费钥匙】
    self.FreeKey={};
    --【免费钥匙】【活动id ,待定】
    self.FreeKey.id=0;
    --【免费钥匙】【标题】
    self.FreeKey.title="";
    --【免费钥匙】【开始时间】
    self.FreeKey.start_date="";
    --【免费钥匙】【结束时间】
    self.FreeKey.end_date="";
    --【免费钥匙】【结束保存时间,预留】
    self.FreeKey.retention_time=0;
    --【免费钥匙】【参数,预留】
    self.FreeKey.params="";
    --【免费钥匙】【倒计时】
    self.FreeKey.countdown=0;
    --【免费钥匙】【状态, 1开启中 0已过期】
    self.FreeKey.is_open=0;



end

--更新
function LimitTimeActivityCache:UpdateEggInfo(_data)
    --【彩蛋】【活动id ,待定】
    self.ColoredEgg.id=_data.id;
    --【彩蛋】【标题】
    self.ColoredEgg.title=_data.title;
    --【彩蛋】【开始时间】
    self.ColoredEgg.start_date=_data.start_date;
    --【彩蛋】【结束时间】
    self.ColoredEgg.end_date=_data.end_date;
    --【彩蛋】【结束保存时间,预留】
    self.ColoredEgg.retention_time=_data.retention_time;
    --【彩蛋】【参数,预留】
    self.ColoredEgg.params=_data.params;
    --【彩蛋】【倒计时】
    self.ColoredEgg.countdown=_data.countdown;
    --【彩蛋】【状态, 1开启中 0已过期】
    self.ColoredEgg.is_open=_data.is_open;
end

--更新
function LimitTimeActivityCache:UpdateSpinDrawInfo(_data)
    --【旋转抽奖】【活动id ,待定】
    self.SpinDraw.id=_data.id;
    --【旋转抽奖】【标题】
    self.SpinDraw.title=_data.title;
    --【旋转抽奖】【开始时间】
    self.SpinDraw.start_date=_data.start_date;
    --【旋转抽奖】【结束时间】
    self.SpinDraw.end_date=_data.end_date;
    --【旋转抽奖】【结束保存时间,预留】
    self.SpinDraw.retention_time=_data.retention_time;
    --【旋转抽奖】【参数,预留】
    self.SpinDraw.params=_data.params;
    --【旋转抽奖】【倒计时】
    self.SpinDraw.countdown=_data.countdown;
    --【旋转抽奖】【状态, 1开启中 0已过期】
    self.SpinDraw.is_open=_data.is_open;
end

--更新
function LimitTimeActivityCache:UpdateFreeKeyInfo(_data)
    --【免费钥匙】【活动id ,待定】
    self.FreeKey.id=_data.id;
    --【免费钥匙】【标题】
    self.FreeKey.title=_data.title;
    --【免费钥匙】【开始时间】
    self.FreeKey.start_date=_data.start_date;
    --【免费钥匙】【结束时间】
    self.FreeKey.end_date=_data.end_date;
    --【免费钥匙】【结束保存时间,预留】
    self.FreeKey.retention_time = _data.retention_time;
    --【免费钥匙】【参数,预留】
    self.FreeKey.params = _data.params;
    --【免费钥匙】【倒计时】
    self.FreeKey.countdown = _data.countdown;
    --【免费钥匙】【状态, 1开启中 0已过期】
    self.FreeKey.is_open = _data.is_open;
end



function LimitTimeActivityCache:__delete()
    self.ColoredEgg.id=nil;
    self.ColoredEgg.title=nil;
    self.ColoredEgg.start_date=nil;
    self.ColoredEgg.end_date=nil;
    self.ColoredEgg.retention_time=nil;
    self.ColoredEgg.params=nil;
    self.ColoredEgg.countdown=nil;
    self.ColoredEgg.is_open=nil;

    self.SpinDraw.id=nil;
    self.SpinDraw.title=nil;
    self.SpinDraw.start_date=nil;
    self.SpinDraw.end_date=nil;
    self.SpinDraw.retention_time=nil;
    self.SpinDraw.params=nil;
    self.SpinDraw.countdown=nil;
    self.SpinDraw.is_open=nil;

    self.FreeKey.id=nil;
    self.FreeKey.title=nil;
    self.FreeKey.start_date=nil;
    self.FreeKey.end_date=nil;
    self.FreeKey.retention_time=nil;
    self.FreeKey.params=nil;
    self.FreeKey.countdown=nil;
    self.FreeKey.is_open=nil;

    self.ColoredEgg=nil;
    self.SpinDraw=nil;
    self.FreeKey=nil;
end


LimitTimeActivityCache = LimitTimeActivityCache:GetInstance()
return LimitTimeActivityCache