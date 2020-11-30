local BaseClass = core.Class
local DailyTasksPanel = BaseClass("DailyTasksPanel")


local DailyTaskItem = require('Logic/UI/UI_Activity/Panel/Activity/Item/DailyTaskItem');


--region【init】
function DailyTasksPanel:__init(gameObject)
    self.gameObject=gameObject;

    self.Layout =CS.DisplayUtil.GetChild(gameObject, "Layout");
    self.TaskItem =CS.DisplayUtil.GetChild(self.Layout, "TaskItem");

    self.ItemList=nil;
end
--endregion


--region 【刷新每日任务】
function DailyTasksPanel:UpdateDailyTasksPanel()
    self:ClearList();
    if(self.ItemList==nil)then self.ItemList={}; end

    local InfoList=Cache.ActivityCache.dailytaskList;
    local len = table.length(InfoList);
    for i = 1, len do
        local go = logic.cs.GameObject.Instantiate(self.TaskItem,self.Layout.transform);
        go.transform.localPosition = core.Vector3.zero;
        go.transform.localScale = core.Vector3.one;
        go:SetActive(true);

        local item =DailyTaskItem.New(go);
        table.insert(self.ItemList,item);

        item._index = i;
        item:SetInfo(InfoList[i]);
    end

end
--endregion





--region【清理销毁 所有列表内容】
function DailyTasksPanel:ClearList()
    if (self.ItemList)then
        local len=#self.ItemList;
        if(len>0)then
            for i = 1, len do
                self.ItemList[i]:Delete();
            end
        end
    end
    self.ItemList={};
    self.ItemList=nil;
end
--endregion


--region【销毁】
function DailyTasksPanel:__delete()
    self:ClearList();

    self.Layout = nil;
    self.TaskItem = nil;
    self.gameObject=nil;
end
--endregion


return DailyTasksPanel
