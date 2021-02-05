--【限时活动】

-- 单例模式
local BaseClass = core.Class
local LimitTimeActivityCache = BaseClass("LimitTimeActivityCache", core.Singleton)

-- 构造函数
--【限时活动】
function LimitTimeActivityCache:__init()
    --【活动列表】
    self.ActivityList={};
end

--更新
function LimitTimeActivityCache:UpdateActivityList(datas)
    if(GameHelper.islistHave(self.ActivityList)==true)then
        Cache.ClearList(self.ActivityList);
    end
    local activityList=datas;
    local len=table.length(activityList);
    if(activityList and len>0)then
        for i = 1,len do
            local info =require("Logic/Cache/LimitTimeActivityInfo/LimitTimeActivityInfo").New();
            info:UpdateData(activityList[i]);
            table.insert(self.ActivityList,info);
        end
    end
end

--【刷新保存一组数据】
function LimitTimeActivityCache:SetActivityInfo(Info)
    if(GameHelper.islistHave(self.ActivityList)==true)then
        local len=table.length(self.ActivityList);
        for i = 1, len do
            if(self.ActivityList[i].id==Info.id)then
                self.ActivityList[i]:UpdateData(Info);  --刷新保存一组数据
            end
        end
    end
end


--【获取一组活动数据】
function LimitTimeActivityCache:GetActivityInfo(id)
    if(GameHelper.islistHave(self.ActivityList)==true)then
        local len=table.length(self.ActivityList);
        for i = 1, len do
            if(self.ActivityList[i].id==id)then
                return self.ActivityList[i];
            end
        end
    end
    return nil;
end

--【判断是否有开启的活动】
function LimitTimeActivityCache:IsOpen()
    if(GameHelper.islistHave(self.ActivityList)==true)then
        local len=table.length(self.ActivityList);
        for i = 1, len do
            if(self.ActivityList[i].is_open==1 and self.ActivityList[i].id~=EnumActivity.Investment)then
                return true;
            end
        end
    end
    return false;
end

--
function LimitTimeActivityCache:InvestmentIsEnd()
    if(GameHelper.islistHave(self.ActivityList)==true)then
        local len=table.length(self.ActivityList);
        for i = 1, len do
            if(self.ActivityList[i].is_open==1 and self.ActivityList[i].id==EnumActivity.Investment)then
                self.ActivityList[i].is_open=0;
            end
        end
    end
end




function LimitTimeActivityCache:__delete()

end


LimitTimeActivityCache = LimitTimeActivityCache:GetInstance()
return LimitTimeActivityCache
