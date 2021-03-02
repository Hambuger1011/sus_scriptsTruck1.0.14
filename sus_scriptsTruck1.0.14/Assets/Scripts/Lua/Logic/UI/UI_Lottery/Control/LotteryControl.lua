local LotteryControl = core.Class("LotteryControl", core.Singleton)

local UILotteryForm=nil;

local UIPrizeHistoryForm=nil;
--region【构造函数】

function LotteryControl:__init()
end

--endregion


--region【设置界面】
function LotteryControl:SetData(uilottery)
    UILotteryForm=uilottery;
end

function LotteryControl:SetData2(uiprizehistory)
    UIPrizeHistoryForm=uiprizehistory;
end

--endregion

--region 【刷新奖品列表】
function LotteryControl:UpdateItemList()
    if(UILotteryForm)then
        UILotteryForm:UpdateItemList();
    end
end
--endregion




--region 【开始幸运转盘抽奖】
function LotteryControl:StartLuckDrawRequest()
    logic.gameHttp:StartLuckDraw(function(result) self:StartLuckDraw(result); end)
end
--endregion


--region 【开始幸运转盘抽奖*响应】
function LotteryControl:StartLuckDraw(result)
    logic.debug.Log("----StartLuckDraw---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then

        --logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
        --logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))
        --logic.cs.UserDataManager:ResetMoney(3, tonumber(json.data.leaf))


        if(UILotteryForm)then
            --prize_id  抽奖结果id
            --是否是大奖 1:是 0:不是
            UILotteryForm:StartLuckDraw(json.data.prize_id,json.data.grand_prize);
        end
    else
        if(UILotteryForm)then
            UILotteryForm:SpinBtnMaskReset();
        end
    end
end
--endregion


--region 【获取转盘活动信息】
function LotteryControl:GetLuckyTurntableInfoRequest()
    logic.gameHttp:GetLuckyTurntableInfo(function(result) self:GetLuckyTurntableInfo(result); end)
end
--endregion


--region 【获取转盘活动信息*响应】
function LotteryControl:GetLuckyTurntableInfo(result)
    logic.debug.Log("----GetLuckyTurntableInfo---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        --【缓存】
        Cache.LotteryCache:UpdateList(json.data);

        if(UILotteryForm)then
            UILotteryForm:GetLuckyTurntableInfo();
        end
    end
end
--endregion

--region 【获取我的获奖纪录】
function LotteryControl:GetMyTurntableRecordRequest()
    logic.gameHttp:GetMyTurntableRecord(function(result) self:GetMyTurntableRecord(result); end)
end
--endregion


--region 【获取我的获奖纪录*响应】
function LotteryControl:GetMyTurntableRecord(result)
    logic.debug.Log("----GetMyTurntableRecord---->" .. result);
    local json = core.json.Derialize(result);
    local code = tonumber(json.code)
    if(code == 200)then
        Cache.LotteryCache:UpdateRecordList(json.data)



        if(UIPrizeHistoryForm)then
            UIPrizeHistoryForm:UpdateItemList();
        end
    end
end
--endregion


--析构函数
function LotteryControl:__delete()
end


LotteryControl = LotteryControl:GetInstance()
return LotteryControl