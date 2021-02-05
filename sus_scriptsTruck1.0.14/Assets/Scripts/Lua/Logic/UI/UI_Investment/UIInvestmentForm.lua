local UIView = core.UIView
local UIInvestmentForm = core.Class("UIInvestmentForm", UIView)

local InvestmentItem = require('Logic/UI/UI_Investment/Item/InvestmentItem')


UIInvestmentForm.config = {
    ID = logic.uiid.UIInvestmentForm,
    AssetName = 'UI/Resident/UI/UIInvestmentForm'
}

--region【Awake】

local this=nil;

function UIInvestmentForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;


    self.First = CS.DisplayUtil.GetChild(this.gameObject, "First");
    self.FirstItem1 = CS.DisplayUtil.GetChild(self.First, "FirstItem1");
    self.FirstItem2 = CS.DisplayUtil.GetChild(self.First, "FirstItem2");
    self.FirstItem3 = CS.DisplayUtil.GetChild(self.First, "FirstItem3");

    self.Second = CS.DisplayUtil.GetChild(this.gameObject, "Second");
    self.SecondItem1 = CS.DisplayUtil.GetChild(self.Second, "SecondItem1");
    self.SecondItem2 = CS.DisplayUtil.GetChild(self.Second, "SecondItem2");
    self.SecondItem3 = CS.DisplayUtil.GetChild(self.Second, "SecondItem3");
    self.SecondItem4 = CS.DisplayUtil.GetChild(self.Second, "SecondItem4");

    self.DiamondBtn = CS.DisplayUtil.GetChild(this.gameObject, "DiamondBtn");
    self.DiamondText =CS.DisplayUtil.GetChild(self.DiamondBtn, "DiamondText"):GetComponent("Text");
    self.KeyBtn = CS.DisplayUtil.GetChild(this.gameObject, "KeyBtn");
    self.KeyText =CS.DisplayUtil.GetChild(self.KeyBtn, "KeyText"):GetComponent("Text");
    self.CollectBtn = CS.DisplayUtil.GetChild(this.gameObject, "CollectBtn");
    self.CollectMaskBtn = CS.DisplayUtil.GetChild(this.gameObject, "CollectMaskBtn");
    self.UIMask = CS.DisplayUtil.GetChild(this.gameObject, "UIMask");
    self.TimeBg = CS.DisplayUtil.GetChild(this.gameObject, "TimeBg");
    self.TimeText =CS.DisplayUtil.GetChild(self.TimeBg, "TimeText"):GetComponent("Text");


    self.ItemList={};


    logic.cs.UIEventListener.AddOnClickListener(self.DiamondBtn,function(data) self:DiamondBtnClick(); end)
    logic.cs.UIEventListener.AddOnClickListener(self.KeyBtn,function(data) self:KeyBtnClick(); end)
    logic.cs.UIEventListener.AddOnClickListener(self.CollectBtn,function(data) self:CollectBtnClick(); end)
    logic.cs.UIEventListener.AddOnClickListener(self.UIMask,function(data) self:OnExitClick(); end)

end
--endregion


--region【OnOpen】

function UIInvestmentForm:OnOpen()
    UIView.OnOpen(self)
    GameController.InvestmentControl:SetData(self);
end

--endregion


--region 【OnClose】

function UIInvestmentForm:OnClose()
    UIView.OnClose(self)
    GameController.InvestmentControl:SetData(nil);
end

--endregion


function UIInvestmentForm:SetInfo()

    local isBuy=false;

    self.DiamondBtn:SetActive(false);
    self.KeyBtn:SetActive(false);
    self.CollectBtn:SetActive(false);

    local InvestInfo=nil;
    if(Cache.InvestmentCache.Investment1.is_join==0 and Cache.InvestmentCache.Investment2.is_join==0)then
        InvestInfo=Cache.InvestmentCache.Investment1;
        self.DiamondBtn:SetActive(true);
        self.KeyBtn:SetActive(true);

    elseif(Cache.InvestmentCache.Investment1.is_join==1 and Cache.InvestmentCache.Investment2.is_join==0)then
        InvestInfo=Cache.InvestmentCache.Investment1;
        self.CollectBtn:SetActive(true);
        isBuy=true;
    elseif(Cache.InvestmentCache.Investment1.is_join==0 and Cache.InvestmentCache.Investment2.is_join==1)then
        InvestInfo=Cache.InvestmentCache.Investment2;
        self.CollectBtn:SetActive(true);
        isBuy=true;
    end


    local onlyID1=self.FirstItem1.gameObject:GetInstanceID();
    if(#InvestInfo.prize1<3)then return; end
    self:SetItem(self.FirstItem1,onlyID1,InvestInfo.prize1[1],1,false);

    local onlyID2=self.FirstItem2.gameObject:GetInstanceID();
    if(#InvestInfo.prize1<3)then return; end
    self:SetItem(self.FirstItem2,onlyID2,InvestInfo.prize1[2],2,false);

    local onlyID3=self.FirstItem3.gameObject:GetInstanceID();
    if(#InvestInfo.prize1<3)then return; end
    self:SetItem(self.FirstItem3,onlyID3,InvestInfo.prize1[3],3,false);

    if(isBuy==true)then
        local onlyID4=self.SecondItem1.gameObject:GetInstanceID();
        if(#InvestInfo.prize2<4)then return; end
        self:SetItem(self.SecondItem1,onlyID4,InvestInfo,4,false);
    end

    local onlyID5=self.SecondItem2.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem2,onlyID5,InvestInfo.prize2[1],5,false);

    local onlyID6=self.SecondItem3.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem3,onlyID6,InvestInfo.prize2[2],6,false);

    local onlyID7=self.SecondItem4.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem4,onlyID7,InvestInfo.prize2[3],7,false);

    self.DiamondText.text = tostring(Cache.InvestmentCache.Investment1.diamond_count);
    self.KeyText.text = tostring(Cache.InvestmentCache.Investment2.key_count);
end

function UIInvestmentForm:SetItem(item,onlyID,itemData,index,isBuyed);
    if(onlyID==nil)then return; end
    local investmentItem = table.trygetvalue(self.ItemList,onlyID);
    if(investmentItem==nil)then
        investmentItem = InvestmentItem.New(item.gameObject);
        self.ItemList[onlyID]=investmentItem;
    end
    --【赋值】
    if(investmentItem)then
        investmentItem:SetItemData(itemData,index);
        if(isBuyed==true)then
            investmentItem:ShowBuyed();
        end
    end
end



function UIInvestmentForm:SetInfo2()
    self.DiamondBtn:SetActive(false);
    self.KeyBtn:SetActive(false);
    self.CollectBtn:SetActive(false);
    self.CollectMaskBtn:SetActive(true);
    self.TimeBg:SetActive(true);

    --【刷新倒计时】
    self:ShowTime();

    local InvestInfo=Cache.InvestmentCache.plan_info;

    local onlyID1=self.FirstItem1.gameObject:GetInstanceID();
    if(#InvestInfo.prize1<3)then return; end
    self:SetItem(self.FirstItem1,onlyID1,InvestInfo.prize1[1],1,true);

    local onlyID2=self.FirstItem2.gameObject:GetInstanceID();
    if(#InvestInfo.prize1<3)then return; end
    self:SetItem(self.FirstItem2,onlyID2,InvestInfo.prize1[2],2,true);

    local onlyID3=self.FirstItem3.gameObject:GetInstanceID();
    if(#InvestInfo.prize1<3)then return; end
    self:SetItem(self.FirstItem3,onlyID3,InvestInfo.prize1[3],3,true);

    local onlyID4=self.SecondItem1.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem1,onlyID4,InvestInfo,4,false);

    local onlyID5=self.SecondItem2.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem2,onlyID5,InvestInfo.prize2[1],5,false);

    local onlyID6=self.SecondItem3.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem3,onlyID6,InvestInfo.prize2[2],6,false);

    local onlyID7=self.SecondItem4.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem4,onlyID7,InvestInfo.prize2[3],7,false);

end

function UIInvestmentForm:ShowTime()
    local _time= Cache.InvestmentCache.join_info.countdown;
    local str="";
    if(_time and _time>0)then
        --【倒计时 显示】
        local day =  math.modf( _time / 86400 )
        _time=math.fmod(_time, 86400);
        local hour =  math.modf( _time / 3600 );
        local minute = math.fmod( math.modf(_time / 60), 60 );
        --local second = math.fmod(_time, 60 );
        --str = string.format("%02d:%02d", hour, minute);
        str = day.."d:"..hour.."h:"..minute.."m";
    end
    --【倒计时 显示】
    self.TimeText.text=str;
end

function UIInvestmentForm:EnterCollect()
    self.CollectMaskBtn:SetActive(false);
    self.TimeBg:SetActive(false);
    self.CollectBtn:SetActive(true);
end


function UIInvestmentForm:EnterCollect2(InvestInfo)
    self.CollectMaskBtn:SetActive(false);
    self.TimeBg:SetActive(false);
    self.CollectBtn:SetActive(false);


    local onlyID4=self.SecondItem1.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem1,onlyID4,InvestInfo,4,true);

    local onlyID5=self.SecondItem2.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem2,onlyID5,InvestInfo.prize2[1],5,true);

    local onlyID6=self.SecondItem3.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem3,onlyID6,InvestInfo.prize2[2],6,true);

    local onlyID7=self.SecondItem4.gameObject:GetInstanceID();
    if(#InvestInfo.prize2<4)then return; end
    self:SetItem(self.SecondItem4,onlyID7,InvestInfo.prize2[3],7,true);

    --把开关关掉
    Cache.LimitTimeActivityCache:InvestmentIsEnd();
    GameController.ActivityControl:InvestmentIsEnd();
end


function UIInvestmentForm:DiamondBtnClick()
    self:JoinInvestPlan(Cache.InvestmentCache.Investment1,1);
end

function UIInvestmentForm:KeyBtnClick()
    self:JoinInvestPlan(Cache.InvestmentCache.Investment2,2);
end

function UIInvestmentForm:CollectBtnClick()
    GameController.InvestmentControl:GetInvestPlanStatusRequest2(Cache.InvestmentCache.plan_info.activity_id);
end


function UIInvestmentForm:JoinInvestPlan(info,_type)
    if(info==nil)then return; end

    if(_type==1)then
        if(info.diamond_count>logic.cs.UserDataManager.UserData.DiamondNum)then
            logic.cs.UITipsMgr:PopupTips("Not enough diamonds", false);
            return;
        end
    elseif(_type==2)then
        if(info.key_count>logic.cs.UserDataManager.UserData.KeyNum)then
            logic.cs.UITipsMgr:PopupTips("Not enough key", false);
            return;
        end
    end

    local uicollect= logic.UIMgr:Open(logic.uiid.UICollectForm);
    if(uicollect)then
        local diamond_count=0;
        local key_count=0;
        local itemArr={};

        local itemlist=info.prize1;
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
        uicollect:SetData(diamond_count,key_count,"CLAIM",
                function()  GameController.InvestmentControl:JoinInvestPlanRequest(info.activity_id,info.number); end,itemArr);
    end
end




--region 【界面关闭】
function UIInvestmentForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIInvestmentForm