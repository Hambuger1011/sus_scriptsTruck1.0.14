local BaseClass = core.Class
local DailyTaskItem = BaseClass("DailyTaskItem")



--region【__init】
function DailyTaskItem:__init(gameObject)
    self.gameObject=gameObject;

    self.Title =CS.DisplayUtil.GetChild(gameObject, "Title"):GetComponent("Text");
    self.Progress =CS.DisplayUtil.GetChild(gameObject, "Progress"):GetComponent("Text");
    self.RewardNum =CS.DisplayUtil.GetChild(gameObject, "RewardNum"):GetComponent("Text");
    self.ButtonGO =CS.DisplayUtil.GetChild(gameObject, "ButtonGO")
    self.ButtonCOMPLETED =CS.DisplayUtil.GetChild(gameObject, "ButtonCOMPLETED")
    self.ButtonRECEIVE =CS.DisplayUtil.GetChild(gameObject, "ButtonRECEIVE")

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.ButtonGO,function(data) self:ButtonGOclicke() end)
    logic.cs.UIEventListener.AddOnClickListener(self.ButtonRECEIVE,function(data) self:ButtonRECEIVEclicke() end)

    self.info=nil;
end
--endregion


--region【刷新数据】
function DailyTaskItem:SetInfo(data)
    self.info=data;

    self.Title.text = data.task_name;
    self.Progress.text = data.task_finish_event.."/"..data.task_total_event
    if(data.prize_diamond and tonumber(data.prize_diamond) > 0)then
        self.RewardNum.text = "x"..data.prize_diamond;
    else
        self.RewardNum.text = "x"..data.prize_key;
    end

    --【按钮状态】
   self:SetButtonStatus(data.status);
end
--endregion


--region【刷新按钮状态】

function DailyTaskItem:SetButtonStatus(status)
    self.ButtonGO.gameObject:SetActiveEx(false);
    self.ButtonRECEIVE.gameObject:SetActiveEx(false);
    self.ButtonCOMPLETED.gameObject:SetActiveEx(false);

    if(tonumber(status) == 0)then
        self.ButtonGO.gameObject:SetActiveEx(true);
    elseif(tonumber(status) == 1)then
        self.ButtonRECEIVE.gameObject:SetActiveEx(true);
    elseif(tonumber(status) == 2)then
        self.ButtonCOMPLETED.gameObject:SetActiveEx(true);
    end
end

--endregion


--region【GO按钮点击】
function DailyTaskItem:ButtonGOclicke()
    if(self.info==nil)then return; end

    --【埋点*】
    logic.cs.GamePointManager:BuriedPoint(CS.EventEnum.ActivityClickTaskGo,"","","","",tostring(self.info.task_id));

    if(tonumber(self.info.task_id) == 7)then
       GameController.ActivityControl:SetVerticalNormalizedPosition();
    else
        if(GameHelper.CurBookId and GameHelper.CurBookId>0)then
            local bookinfo={};
            bookinfo.book_id=GameHelper.CurBookId;
            GameHelper.BookClick(bookinfo);
        end
    end
end
--endregion


--region【RECEIVE按钮点击】
function DailyTaskItem:ButtonRECEIVEclicke()
    if(self.info==nil)then return; end
    --【埋点*】
    logic.cs.GamePointManager:BuriedPoint(CS.EventEnum.ActivityClickTaskReward,"","","","",tostring(self.info.task_id));
    --【每日任务领取奖励】
    GameController.ActivityControl:ReceiveTaskPrizeRequest(self.info.task_id);
end
--endregion


--region【销毁】
function DailyTaskItem:__delete()
    if(self.ButtonGO)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.ButtonGO,function(data) self:ButtonGOclicke() end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.ButtonRECEIVE,function(data) self:ButtonRECEIVEclicke() end)
    end

    self.Title=nil;
    self.Progress=nil;
    self.RewardNum =nil;
    self.ButtonGO=nil;
    self.ButtonCOMPLETED =nil;
    self.ButtonRECEIVE =nil;
    self.info=nil;

    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end
--endregion


return DailyTaskItem
