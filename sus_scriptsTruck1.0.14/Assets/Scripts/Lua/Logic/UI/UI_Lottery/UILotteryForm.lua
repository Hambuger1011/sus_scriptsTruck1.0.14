local UIView = core.UIView
local UILotteryForm = core.Class("UILotteryForm", UIView)

local LotteryItem = require('Logic/UI/UI_Lottery/Item/LotteryItem');
local LuckyWinnersItem = require('Logic/UI/UI_Lottery/Item/LuckyWinnersItem');

local uiid = logic.uiid
UILotteryForm.config = {
    ID = uiid.UILotteryForm,
    AssetName = 'UI/Resident/UI/UILotteryForm'
}

--region【Awake】
local PlayList={};

local itemScriptList={};

local rotatelist={};
local this=nil;
function UILotteryForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;


    self.SpinBtn = CS.DisplayUtil.GetChild(this.gameObject, "SpinBtn");
    self.SpinBtnMask = CS.DisplayUtil.GetChild(this.gameObject, "SpinBtnMask");
    self.SpinBtnAnima = CS.DisplayUtil.GetChild(self.SpinBtn, "Img1"):GetComponent("Animator");
    self.BigLightListAnima = CS.DisplayUtil.GetChild(this.gameObject, "BigLightList"):GetComponent("Animator");
    self.LightListAnima = CS.DisplayUtil.GetChild(this.gameObject, "LightList"):GetComponent("Animator");
    self.Rotarytable = CS.DisplayUtil.GetChild(this.gameObject, "Rotarytable");
    self.RotarytableBg = CS.DisplayUtil.GetChild(self.Rotarytable, "RotarytableBg");


    for i = 1, 8 do
        local itemobj=CS.DisplayUtil.GetChild(self.RotarytableBg, "ItemList"..i);
        local item =LotteryItem.New(itemobj);
        table.insert(itemScriptList,item);
    end

    self.LotteryCount = CS.DisplayUtil.GetChild(this.gameObject, "LotteryCount"):GetComponent("Text");
    self.LuckyWinners = CS.DisplayUtil.GetChild(this.gameObject, "LuckyWinners");
    self.ViewBtn = CS.DisplayUtil.GetChild(self.LuckyWinners, "ViewBtn");
    self.ViewText = CS.DisplayUtil.GetChild(self.ViewBtn, "ViewText"):GetComponent("Text");
    self.actbtnBg = CS.DisplayUtil.GetChild(self.ViewBtn, "actbtnBg");

    self.Content = CS.DisplayUtil.GetChild(self.LuckyWinners, "Content");
    self.LuckyWinnersItemObj = CS.DisplayUtil.GetChild(self.LuckyWinners, "LuckyWinnersItem");

    self.ScrollViewRect = CS.DisplayUtil.GetChild(self.LuckyWinners, "ScrollView"):GetComponent("RectTransform");

    self.HistoryBtn = CS.DisplayUtil.GetChild(this.gameObject, "HistoryBtn");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.SpinBtn,function(data) self:SpinBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.ViewBtn,function(data) self:ViewBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.HistoryBtn,function(data) self:HistoryBtnClick(data) end);
    --logic.cs.UIEventListener.AddOnClickListener(self.Bookbg.gameObject,function(data) self:ReadBtnClick() end);



    table.insert(rotatelist,720+21.5);
    table.insert(rotatelist,720+66.5);
    table.insert(rotatelist,720+111.5);
    table.insert(rotatelist,720+157.5);
    table.insert(rotatelist,720+202.5);
    table.insert(rotatelist,720+257.5);
    table.insert(rotatelist,720+291.5);
    table.insert(rotatelist,720+336.5);
end
--endregion


--region【OnOpen】

function UILotteryForm:OnOpen()
    UIView.OnOpen(self)

    GameController.LotteryControl:SetData(self)


    --在MainTop里标记
    CS.XLuaHelper.SetMainTopClose("UILotteryForm");

    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if(uiform)then
        uiform.uiform:Hide();
    end

    local itemlist=Cache.ActivityCache.lucky_turntable;
    if(GameHelper.islistHave(itemlist)==false)then
        --【获取通用奖励配置】---【限时活动】【通用奖励】
        GameController.ActivityControl:GetRewardConfigRequest()
    else
        self:UpdateItemList()
    end

    GameController.LotteryControl:GetLuckyTurntableInfoRequest()
end

--endregion


--region 【OnClose】

function UILotteryForm:OnClose()
    UIView.OnClose(self)
   -- logic.cs.UIEventListener.RemoveOnClickListener(self.ReadBtn,function(data) self:ReadBtnClick(data) end);
    GameController.LotteryControl:SetData(nil)

    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if(uiform==nil)then
        logic.UIMgr:Open(logic.uiid.UIMainDownForm);
    else
        uiform.uiform:Appear();
    end


    for i = 1, 8 do
        itemScriptList[i]:Delete();
    end
    itemScriptList={};

    if(self.SpinBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.SpinBtn,function(data) self:SpinBtnClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.ViewBtn,function(data) self:ViewBtnClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.HistoryBtn,function(data) self:HistoryBtnClick(data) end);
    end


    self.SpinBtn = nil;
    self.SpinBtnMask = nil;
    self.SpinBtnAnima = nil;
    self.BigLightListAnima = nil;
    self.LightListAnima = nil;
    self.Rotarytable = nil;
    self.RotarytableBg = nil;

    rotatelist={};

    local len=table.length(PlayList);
    for i = 1, len do
        PlayList[i]:Delete();
    end
    PlayList={};

    self.LotteryCount = nil;
    self.LuckyWinners = nil;
    self.ViewBtn = nil;
    self.ViewText = nil;
    self.actbtnBg = nil;
    self.Content = nil;
    self.LuckyWinnersItemObj = nil;
    self.ScrollViewRect = nil;
    self.HistoryBtn = nil;

end

--endregion


function UILotteryForm:UpdateItemList()
    local itemlist=Cache.ActivityCache.lucky_turntable;
    if(GameHelper.islistHave(itemlist))then
        local len = table.length(itemlist);
        for i = 1, len do
            if(i<=8)then
                --【设置数据】
                itemScriptList[i]:SetItemData(itemlist[i],i);
            end
        end
    end
end




local _index=0;
function UILotteryForm:SpinBtnClick()
    self.SpinBtnAnima.enabled=true;
    self.SpinBtnAnima:Play("Lottery",0,0);
    self.SpinBtnMask:SetActive(true);

    --【开始幸运转盘抽奖】
    GameController.LotteryControl:StartLuckDrawRequest();
end

--region【开始抽奖】
function UILotteryForm:StartLuckDraw(prize_id,grand_prize)
    local itemlist=Cache.ActivityCache.lucky_turntable;
    if(GameHelper.islistHave(itemlist))then
        local len = table.length(itemlist);

        local _index=0;
        for i = 1, len do
            if(i<=8)then
                if(itemlist[i].id==prize_id)then
                    _index=i;
                    break;
                end
            end
        end

        if(_index>0)then

            self.BigLightListAnima:Play("BigLight2",0,0);
            self.LightListAnima:Play("Light2",0,0);
            --720+21.5
            --720+66.5
            --720+111.5
            --720+157.5
            --720+202.5
            --720+257.5
            --720+291.5
            --720+336.5
            local twer1 =self.RotarytableBg.transform:DORotate({x=0,y=0,z=rotatelist[_index]},5,CS.DG.Tweening.RotateMode.FastBeyond360);
            twer1:SetAutoKill(true):SetEase(core.tween.Ease.OutQuad);
            twer1:OnComplete(function()
                self.BigLightListAnima:Play("BigLight",0,0);
                self.LightListAnima:Play("Light",0,0);
                self:SpinBtnMaskReset();


                local diamond_count=0;
                local key_count=0;
                local itemArr={};

                local itemlist=itemlist[_index].item_list;
                local len=table.length(itemlist);
                if(itemlist and len)then
                    for i = 1, len do
                        if(itemlist[i].id==1)then
                            diamond_count=itemlist[i].num;
                        elseif(itemlist[i].id==2)then
                            key_count=itemlist[i].num;
                        else
                            table.insert(itemArr,itemlist[i]);
                        end
                    end
                end
                GameHelper.ShowCollectItem(diamond_count,key_count,"CLAIM",function()   end,itemArr)
            end)
            twer1:Play();
        end
    end
end
--endregion


function UILotteryForm:SpinBtnMaskReset()
    self.SpinBtnMask:SetActive(false);
end



function UILotteryForm:GetLuckyTurntableInfo()

    --【用户还剩多少次】
    if(Cache.LotteryCache.leaf_turntable_num>0)then
        self.LotteryCount.text=tostring(Cache.LotteryCache.leaf_turntable_num);
    else
        self.LotteryCount.text="0";
    end

    local luckplaylist=Cache.LotteryCache.luck_player_list;
    if(GameHelper.islistHave(luckplaylist))then
        local len=table.length(luckplaylist);
        local PlayListlen=table.length(PlayList);
        for i = 1, len do
            if(PlayListlen>=i)then
                PlayList[i]:SetItemData(luckplaylist[i]);
            else
                local go = logic.cs.GameObject.Instantiate(self.LuckyWinnersItemObj, self.Content.transform);
                go.transform.localPosition = core.Vector3.zero;
                go.transform.localScale = core.Vector3.one;
                go:SetActive(true);
                local item =LuckyWinnersItem.New(go);
                table.insert(PlayList,item);
                item:SetItemData(luckplaylist[i]);
            end
        end
    end
end

local num=0;
function UILotteryForm:ViewBtnClick()
    num=num+1;
    local twer1=nil;
    if(num%2==0)then
        twer1 =self.actbtnBg.transform:DORotate({x=0,y=0,z=0},0.6);
        twer1:OnComplete(function()
            self.ScrollViewRect.sizeDelta = {x=self.ScrollViewRect.sizeDelta.x,y=150}
            self.ViewText.text="View";
        end)
    else
        twer1 =self.actbtnBg.transform:DORotate({x=0,y=0,z=180},0.6);
        twer1:OnComplete(function()
            self.ScrollViewRect.sizeDelta = {x=self.ScrollViewRect.sizeDelta.x,y=300}
            self.ViewText.text="Minimize";
        end)
    end

    twer1:SetAutoKill(true):SetEase(core.tween.Ease.OutQuart);
    twer1:Play();
end


function UILotteryForm:HistoryBtnClick()
    logic.UIMgr:Open(logic.uiid.UIPrizeHistoryForm);


end




--region 【界面关闭】
function UILotteryForm:OnExitClick()
    UIView.__Close(self)

    if self.onClose then
        self.onClose()
    end
end
--endregion



return UILotteryForm