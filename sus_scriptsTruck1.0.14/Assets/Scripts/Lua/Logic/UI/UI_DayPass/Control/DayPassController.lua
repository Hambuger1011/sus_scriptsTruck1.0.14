local DayPassController = core.Class("DayPassController", core.Singleton)


--region【构造函数】
function DayPassController:__init()
end
--endregion

--region【设置界面】
function DayPassController:SetData()
end
--endregion


function DayPassController:DayPassUpdate()
    local info2=Cache.PopWindowCache:GetInfoById(2);
    if(info2)then
        local daypasslist= info2.book_list;
        if(daypasslist)then
            if(GameHelper.islistHave(daypasslist)==true)then
                CS.XLuaHelper.DayPassRemoveAll();
                for i = 1, #daypasslist do
                    if(daypasslist[i])then
                        table.insert(Cache.PopWindowCache.daypassList,daypasslist[i].book_id);
                        self:DayPassShow2(daypasslist[i].book_id);


                        --【倒计时 显示】
                        local _time= daypasslist[i].countdown;
                        local str="";
                        if(_time and _time>0)then
                            local hour =  math.modf( _time / 3600 );
                            local minute = math.fmod( math.modf(_time / 60), 60 );
                            --local second = math.fmod(_time, 60 );
                            str = string.format("%02d:%02d", hour, minute);
                        end
                        --【倒计时 显示】
                        --【给C#缓存】
                        CS.XLuaHelper.DayPassAdd(daypasslist[i].book_id,str);
                    end
                end
                self:DayPassShow(true);
            end
        end
    end

    --定时器 检查
    self:TimerRequest();
end


--region***【DayPass】【开启和关闭】
function DayPassController:DayPassShow()
    GameController.MainFormControl:DayPass(Cache.PopWindowCache.daypassList);
end

function DayPassController:DayPassShow2(bookId)
    --【书本详情页面刷新】
    local BookDisplayFormobj = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.BookDisplayForm)
    if(BookDisplayFormobj and CS.XLuaHelper.is_Null(BookDisplayFormobj)==false)then
        local BookDisplayForm=BookDisplayFormobj:GetComponent(typeof(CS.BookDisplayForm));
        if(BookDisplayForm)then
            BookDisplayForm:DayPass(bookId);
        end
    end

    --【章节完成界面】
    local BookReading = logic.UIMgr:GetView2(logic.uiid.BookReading);
    if(BookReading)then
        --【限时活动免费读书 显示标签】
        BookReading.chapterSwitch:DayPass(bookId);
    end
end

function DayPassController:SetDayPass(bookId)
    if(bookId)then
        local isHave=Cache.PopWindowCache:IsDayPassShow(bookId);
        if(isHave==true)then
            self:DayPassShow2(bookId)
        end
    end
end

--endregion***【免费钥匙】【刷新界面】


--region【刷新显示】
local isFirst=false;
function DayPassController:UpdateCountdown()

    local info2=Cache.PopWindowCache:GetInfoById(2);
    if(info2)then
        local daypasslist= info2.book_list;
        if(daypasslist)then
            if(GameHelper.islistHave(daypasslist)==true)then
                for i = 1, #daypasslist do
                    if(daypasslist[i])then
                        --剩余倒计时大于0
                        if(daypasslist[i].countdown>0)then
                            if(isFirst==true)then
                                daypasslist[i].countdown=daypasslist[i].countdown-120;
                            end
                            if(daypasslist[i].countdown<0)then
                                daypasslist[i].countdown=0;
                            end

                            --【书本详情页面刷新】
                            local BookDisplayFormobj = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.BookDisplayForm)
                            if(BookDisplayFormobj and CS.XLuaHelper.is_Null(BookDisplayFormobj)==false)then
                                local BookDisplayForm=BookDisplayFormobj:GetComponent(typeof(CS.BookDisplayForm));
                                if(BookDisplayForm)then

                                    local _time= daypasslist[i].countdown;
                                    local str="";
                                    if(_time and _time>0)then
                                        local day =  math.modf( _time / 86400 )
                                        _time=math.fmod(_time, 86400);
                                        local hour =  math.modf( _time / 3600 )
                                        local minute = math.fmod( math.modf(_time / 60), 60 )
                                        str =day.."d:"..hour.."h:"..minute.."m";
                                    end
                                    --【刷新时间显示】
                                    BookDisplayForm:UpdateCountdown(daypasslist[i].book_id,str);
                                end
                            end
                        end
                    end
                end
            end
        end
    end

    isFirst=true;
end
--endregion



--region 【定时请求】
function DayPassController:TimerRequest()
    --【销毁计时器】
    self:ClearTimer();

    local info2=Cache.PopWindowCache:GetInfoById(2);
    if(info2)then
        local daypasslist= info2.book_list;
        if(daypasslist)then
            if(GameHelper.islistHave(daypasslist)==true)then
                for i = 1, #daypasslist do
                    if(daypasslist[i])then
                        --是否添加
                        local isAdd=true;
                        --剩余倒计时大于0
                        if(daypasslist[i].countdown>0)then
                            --延迟10秒
                            local countdown = daypasslist[i].countdown+10;
                            if(countdown>600)then  --如果剩余时间大于10分钟   不开启定时器
                                isAdd=false;
                            end

                            if(isAdd==true)then
                                local _bookid=daypasslist[i].book_id;
                                --【取出定时器】
                                local _Timer = table.trygetvalue(self.TimerList,_bookid);
                                if(_Timer==nil)then --【如果没有缓存】
                                    _Timer = core.Timer.New(function()

                                        --【销毁计时器】
                                        _Timer:Stop();
                                        self.TimerList[_bookid]=nil;
                                        --【请求数据】【刷新计时】
                                        GameController.MainFormControl:GetWindowConfigRequest();

                                    end,countdown,-1);   --【生成一个新的】

                                    self.TimerList[_bookid]=_Timer; --【缓存定时器】
                                end
                                if(_Timer)then
                                    _Timer:Start();  --【开启定时器】
                                end
                            end
                        end
                    end
                end
            end
        end
    end
end
--endregion


--region【销毁计时器】
function DayPassController:ClearTimer()
    if(self.TimerList)then
        for i, v in pairs(self.TimerList) do
            if(v)then
                v:Stop();
                v=nil;
            end
        end
        self.TimerList={};
    end
end
--endregion


--析构函数
function DayPassController:__delete()
end


DayPassController = DayPassController:GetInstance()
return DayPassController