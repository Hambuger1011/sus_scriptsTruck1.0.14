local BaseClass = core.Class
local DailyTaskInfo = BaseClass("DailyTaskInfo")

function DailyTaskInfo:__init()
    --任务id
    self.task_id=0;
    --任务名称
    self.task_name="";
    --任务奖励钥匙
    self.prize_key=0;
    --任务奖励钻石
    self.prize_diamond=0;
    --道具奖励列表
    self.item_list = {};
    --任务说明
    self.task_description="";
    --任务需要完成事件总次数
    self.task_total_event=0;
    --已完成次数
    self.task_finish_event=0;
    --当前状态  0未完成  1完成未领取 2 完成(整个流程)
    self.status="" ;
end

function DailyTaskInfo:UpdateData(data)
    self.task_id=data.task_id;
    self.task_name=data.task_name;
    self.prize_key=data.prize_key;
    self.prize_diamond=data.prize_diamond;
    self.task_description=data.task_description;
    self.task_total_event=data.task_total_event;
    self.task_finish_event=data.task_finish_event;
    self.status=data.status;
end

--销毁
function DailyTaskInfo:__delete()
    self.task_id=nil;
    self.task_name=nil;
    self.prize_key=nil;
    self.prize_diamond=nil;
    self.item_list=nil;
    self.task_description=nil;
    self.task_total_event=nil;
    self.task_finish_event=nil;
    self.status=nil;
end


return DailyTaskInfo
