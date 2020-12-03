-- 单例模式
local BaseClass = core.Class
local ActivityCache = BaseClass("ActivityCache", core.Singleton)


-- 构造函数
function ActivityCache:__init()
    --签到列表
    self.signinList= {};
    --每日任务列表
    self.dailytaskList= {};


    self.box_status={}; --【宝箱领取状态】【已领取的宝箱id集合】
    --宝箱累计天数
    self.box_cumulative_days=0;
    --已登录领取次数
    self.receive_number=0;
    --活动广告可领取次数
    self.ad_number=0;
    --今天是否领取登录奖励
    self.is_login_receive=0;
    --活动登录奖励倍数
    self.login_award_multiple=0;
    --广告倒计时秒数
    self.ad_countdown=0;


    --【关注社媒奖励】
    self.attention_media={};
    --奖励类型 1钥匙 2钻石 4组合包
    self.attention_media.award_type=0;
    --钥匙数量
    self.attention_media.key_count=0;
    --钻石数量
    self.attention_media.diamond_count=0;


    --【用户迁移奖励】
    self.user_move={};
    --奖励类型 1钥匙 2钻石 4组合包
    self.user_move.award_type=0;
    --钥匙数量
    self.user_move.key_count=0;
    --钻石数量
    self.user_move.diamond_count=0;
    --头像框id
    self.user_move.user_frame_id=0;

    
    --【用户首充奖励】
    self.first_recharge={};
    --奖励类型 1钥匙 2钻石 4组合包
    self.first_recharge.award_type=0;
    --钥匙数量
    self.first_recharge.key_count=0;
    --钻石数量
    self.first_recharge.diamond_count=0;
    --奖品列表
    self.first_recharge.item_list={};


end

--更新【签到】数据
function ActivityCache:UpdatedSignList(datas)
    if(GameHelper.islistHave(self.signinList)==true)then
        Cache.ClearList(self.signinList);
    end
    local signlist=datas.login_reward_list;
    local len=table.length(signlist);
    if(signlist and len>0)then
        for i = 1,len do
            local signinfo =require("Logic/Cache/ActivityInfo/SignInInfo").New();
            signinfo:UpdateData(signlist[i]);
            table.insert(self.signinList,signinfo);
        end
    end
end

--更新活动状态
function ActivityCache:UpdatedActivityState(datas)
    self.box_status=datas.box_status;
    self.box_cumulative_days=datas.box_cumulative_days;
    self.receive_number=datas.receive_number;
    self.ad_number=datas.ad_number;
    self.is_login_receive=datas.is_login_receive;
    self.login_award_multiple=datas.login_award_multiple;
    self.ad_countdown=datas.ad_countdown;
end

--更新【每日任务】数据
function ActivityCache:UpdatedDailyTaskList(datas)
    if(GameHelper.islistHave(self.dailytaskList)==true)then
        Cache.ClearList(self.dailytaskList);
    end
    local tasklist=datas;
    local len=table.length(tasklist);
    if(tasklist and len>0)then
        for i = 1,len do
            local daytaskinfo =require("Logic/Cache/ActivityInfo/DailyTaskInfo").New();
            daytaskinfo:UpdateData(tasklist[i]);
            table.insert(self.dailytaskList,daytaskinfo);
        end
    end
end



--刷新任务状态
function ActivityCache:SetTaskStatus(taskid)
    if(GameHelper.islistHave(self.dailytaskList)==true)then
        for i = 1, #self.dailytaskList do
            if(self.dailytaskList[i].task_id==taskid)then
                self.dailytaskList[i].status=2;
                --把这个元素移动到数组最后一位
                self.dailytaskList = table.movevaluelast(self.dailytaskList, i);
                break;
            end
        end
    end
end

--【是否所有任务结束】
function ActivityCache:isAllTaskOver()
    local isover=true;
    if(GameHelper.islistHave(self.dailytaskList)==true)then
        for i = 1, #self.dailytaskList do
            if(self.dailytaskList[i].status~=2)then
                isover=false;
                break;
            end
        end
    else
        isover=false;
    end
    return isover;
end



--【关注社媒奖励】 --【用户迁移奖励】
function ActivityCache:UpdatedRewardConfig(datas)
    self.attention_media.award_type=datas.attention_media.award_type;
    self.attention_media.key_count=datas.attention_media.key_count;
    self.attention_media.diamond_count=datas.attention_media.diamond_count;

    self.user_move.award_type=datas.user_move.award_type;
    self.user_move.key_count=datas.user_move.key_count;
    self.user_move.diamond_count=datas.user_move.diamond_count;
    self.user_move.user_frame_id=datas.user_move.user_frame_id;
    
    self.first_recharge=datas.first_recharge;
end




-- 析构函数
function ActivityCache:__delete()

end


ActivityCache = ActivityCache:GetInstance()
return ActivityCache