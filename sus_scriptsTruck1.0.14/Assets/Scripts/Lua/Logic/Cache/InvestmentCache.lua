-- 单例模式
local InvestmentCache = core.Class("InvestmentCache", core.Singleton)

-- 构造函数
function InvestmentCache:__init()

    --【第一组】
    self.Investment1={};
    --活动id
    self.Investment1.id=0;
    --活动id
    self.Investment1.activity_id= 0;
    --计划序号
    self.Investment1.number = 0;
    --参加需要钻石数
    self.Investment1.diamond_count = 0;
    --参加需要钥匙数
    self.Investment1.key_count = 0;
    --/时间周期,天数
    self.Investment1.day_count = 0;
    --立即获得的奖品
    self.Investment1.prize1 = {};
    --n天后获得的奖品
    self.Investment1.prize2 = {};
    --是否参了, 1是,0否
    self.Investment1.is_join = 0;


    --【第二组】
    self.Investment2={};
    --活动id
    self.Investment2.id=0;
    --活动id
    self.Investment2.activity_id= 0;
    --计划序号
    self.Investment2.number = 0;
    --参加需要钻石数
    self.Investment2.diamond_count = 0;
    --参加需要钥匙数
    self.Investment2.key_count = 0;
    --/时间周期,天数
    self.Investment2.day_count = 0;
    --立即获得的奖品
    self.Investment2.prize1 = {};
    --n天后获得的奖品
    self.Investment2.prize2 = {};
    --是否参了, 1是,0否
    self.Investment2.is_join = 0;



    self.plan_info={};
    --活动id
    self.plan_info.id=0;
    --活动id
    self.plan_info.activity_id= 0;
    --计划序号
    self.plan_info.number = 0;
    --参加需要钻石数
    self.plan_info.diamond_count = 0;
    --参加需要钥匙数
    self.plan_info.key_count = 0;
    --/时间周期,天数
    self.plan_info.day_count = 0;
    --立即获得的奖品
    self.plan_info.prize1 = {};
    --n天后获得的奖品
    self.plan_info.prize2 = {};
    --是否参了, 1是,0否
    self.plan_info.is_join = 0;


    --参与信息
    self.join_info={};
    --当前计划序号
    self.join_info.number=0;
    --时间周期,天数
    self.join_info.day_count= 0;
    --参与时间
    self.join_info.create_time = 0;
    --状态,0未完成, 1已完成(已领取)
    self.join_info.status = 0;
    --倒计时
    self.join_info.countdown = 0;
end


function InvestmentCache:UpdateData(datas)
    --活动id
    self.Investment1.id=datas[1].id;
    self.Investment1.activity_id=datas[1].activity_id;
    self.Investment1.number=datas[1].number;
    self.Investment1.diamond_count=datas[1].diamond_count;
    self.Investment1.key_count=datas[1].key_count;
    self.Investment1.day_count=datas[1].day_count;
    self.Investment1.prize1=datas[1].prize1;
    self.Investment1.prize2=datas[1].prize2;
    self.Investment1.is_join=tonumber(datas[1].is_join);


    --活动id
    self.Investment2.id=datas[2].id;
    self.Investment2.activity_id=datas[2].activity_id;
    self.Investment2.number=datas[2].number;
    self.Investment2.diamond_count=datas[2].diamond_count;
    self.Investment2.key_count=datas[2].key_count;
    self.Investment2.day_count=datas[2].day_count;
    self.Investment2.prize1=datas[2].prize1;
    self.Investment2.prize2=datas[2].prize2;
    self.Investment2.is_join=tonumber(datas[2].is_join);
end

function InvestmentCache:UpdateJoinInfo(datas)

    --活动id
    self.plan_info.id=datas.plan_info.id;
    self.plan_info.activity_id=datas.plan_info.activity_id;
    self.plan_info.number =datas.plan_info.number;
    self.plan_info.diamond_count = datas.plan_info.diamond_count;
    self.plan_info.key_count = datas.plan_info.key_count;
    self.plan_info.day_count = datas.plan_info.day_count;
    self.plan_info.prize1 = datas.plan_info.prize1;
    self.plan_info.prize2 = datas.plan_info.prize2;
    ----是否参了, 1是,0否
    --self.plan_info.is_join =datas.plan_info.is_join;

    self.join_info.number=datas.join_info.number;
    self.join_info.day_count=datas.join_info.day_count;
    self.join_info.create_time =datas.join_info.create_time;
    self.join_info.status =datas.join_info.status;
    self.join_info.countdown =datas.join_info.countdown;
end




-- 析构函数
function InvestmentCache:__delete()
end


InvestmentCache = InvestmentCache:GetInstance()
return InvestmentCache