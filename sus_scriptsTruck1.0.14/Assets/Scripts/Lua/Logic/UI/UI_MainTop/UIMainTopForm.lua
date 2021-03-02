local UIMainTopForm = core.Class("UIMainTopForm", core.UIView)

UIMainTopForm.config = {
    ID = logic.uiid.UIMainTopForm,
    AssetName = 'UI/Resident/UI/UIMainTopForm'
}

--region【Awake】

local this=nil;
function UIMainTopForm:OnInitView()
    core.UIView.OnInitView(self)
    this=self.uiform;

    self.BG = CS.DisplayUtil.GetChild(this.gameObject, "BG"):GetComponent("RectTransform");
    self.MoneyChange = self.BG:GetComponent("MoneyChange");

    self.ReturnBtn = CS.DisplayUtil.GetChild(this.gameObject, "ReturnBtn");
    self.LogoIcon =CS.DisplayUtil.GetChild(this.gameObject, "LogoIcon"):GetComponent("Image");

    self.CloverBtn = CS.DisplayUtil.GetChild(this.gameObject, "CloverBtn");
    self.KeyBtn = CS.DisplayUtil.GetChild(this.gameObject, "KeyBtn");
    self.DiamondBtn = CS.DisplayUtil.GetChild(this.gameObject, "DiamondBtn");

    self.CloverCount =CS.DisplayUtil.GetChild(self.CloverBtn, "CloverCount"):GetComponent("Text");
    self.KeyCount =CS.DisplayUtil.GetChild(self.KeyBtn, "KeyCount"):GetComponent("Text");
    self.DiamondCount =CS.DisplayUtil.GetChild(self.DiamondBtn, "DiamondCount"):GetComponent("Text");

    self.TimeButton = CS.DisplayUtil.GetChild(this.gameObject, "TimeButton");


    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.ReturnBtn,function(data) self:ReturnBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.CloverBtn,function(data) self:CloverBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.KeyBtn,function(data) self:KeyBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.DiamondBtn,function(data) self:DiamondBtnClick(data) end);

    --当前界面
    self.CurUI="";
end
--endregion


--region【OnOpen】

function UIMainTopForm:OnOpen()
    core.UIView.OnOpen(self)
    --【本界面自适应】
    --获取偏移量；
    local offectY= CS.XLuaHelper.isHasUnSafeArea(self.uiform);
    self.BG.sizeDelta={x=750,y=110+offectY};
end

--endregion


--region 【OnClose】

function UIMainTopForm:OnClose()
    core.UIView.OnClose(self)

end

--endregion


function UIMainTopForm:SetGUI(_curUI,isShow)

    if(isShow==true)then
        self.CurUI=_curUI;
        self.ReturnBtn:SetActiveEx(true);
        self.LogoIcon.gameObject:SetActiveEx(true);
    else
        self.CurUI="";
        self.ReturnBtn:SetActiveEx(false);
        self.LogoIcon.gameObject:SetActiveEx(false);
    end

end

--region【点击返回按钮】
function UIMainTopForm:ReturnBtnClick()
    if(self.CurUI == "UIChargeMoneyForm")then
       logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.ChargeMoneyForm)
       self:OnExitClick();
    elseif(self.CurUI == "UIRankForm")then
        logic.UIMgr:Close(logic.uiid.UIRankForm);
    elseif(self.CurUI == "UILotteryForm")then
        logic.UIMgr:Close(logic.uiid.UILotteryForm);
    elseif(self.CurUI == "UIDressUpForm")then
        logic.UIMgr:Close(logic.uiid.UIDressUpForm);
    elseif(self.CurUI == "UIMasForm")then
        logic.UIMgr:Close(logic.uiid.UIMasForm);
    elseif(self.CurUI == "UIChargeMoneyForm1")then
        logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.ChargeMoneyForm)
    elseif(self.CurUI == "UICommunityForm")then
        self:CloseAllUI();
    end
    --重置
    self:SetGUI("",false);
end

--endregion


--region【关闭所有界面】
function UIMainTopForm:CloseAllUI()
    logic.UIMgr:Close(logic.uiid.UICommunityForm);
    logic.UIMgr:Close(logic.uiid.UIMasForm);
    logic.UIMgr:Close(logic.uiid.UIRankForm);
    logic.UIMgr:Close(logic.uiid.UIDressUpForm);
    logic.UIMgr:Close(logic.uiid.UILotteryForm);
end
--endregion


--region【刷新四叶草货币】
function UIMainTopForm:CloverCountChange(newNum)
    if(self.CloverCount.text=="")then self.CloverCount.text="0"; end
    local oldNum=tonumber(self.CloverCount.text);
    if(newNum==nil)then
        if(newNum >= oldNum)then
            self.MoneyChange:PlayAnimaClover(oldNum,newNum);
        else
            self.MoneyChange:DOTweenToClover(oldNum,newNum);
        end
    else
        self.MoneyChange:DOTweenToClover(oldNum,newNum);
    end
end
--endregion


--region【刷新钥匙】
function UIMainTopForm:KeyCountChange(newNum)
    if(self.KeyCount.text=="")then self.KeyCount.text="0"; end
    local oldNum=tonumber(self.KeyCount.text);
    if(newNum==nil)then
        if(newNum >= oldNum)then
            self.MoneyChange:PlayAnimaKey(oldNum,newNum);
        else
            self.MoneyChange:DOTweenToKey(oldNum,newNum);
        end
    else
        self.MoneyChange:DOTweenToKey(oldNum,newNum);
    end
end
--endregion


--region【刷新钻石】
function UIMainTopForm:DiamondCountChange(newNum)
    if(self.DiamondCount.text=="")then self.DiamondCount.text="0"; end
    local oldNum=tonumber(self.DiamondCount.text);
    if(newNum==nil)then
        if(newNum >= oldNum)then
            self.MoneyChange:PlayAnimaDiamond(oldNum,newNum);
        else
            self.MoneyChange:DOTweenToDiamond(oldNum,newNum);
        end
    else
        self.MoneyChange:DOTweenToDiamond(oldNum,newNum);
    end
end
--endregion



--region 【界面关闭】
function UIMainTopForm:OnExitClick()
    core.UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIMainTopForm