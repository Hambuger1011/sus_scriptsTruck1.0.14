-- 单例模式
local LotteryCache = core.Class("LotteryCache", core.Singleton)


-- 构造函数
function LotteryCache:__init()
    self.consume=0;
    self.leaf_turntable_num=0;
    self.luck_player_list={};


    self.historylist={};
end



--更新 列表
function LotteryCache:UpdateList(datas)
    self.consume=datas.consume;
    self.leaf_turntable_num=datas.leaf_turntable_num;
    self.code=datas.code;
    self.luck_player_list=datas.luck_player_list;
end


function LotteryCache:UpdateRecordList(datas)
    self.historylist=datas.data;
end


-- 析构函数
function LotteryCache:__delete()

end


LotteryCache = LotteryCache:GetInstance()
return LotteryCache