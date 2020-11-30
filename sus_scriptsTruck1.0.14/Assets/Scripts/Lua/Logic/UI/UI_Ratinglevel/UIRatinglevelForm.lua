local BaseClass = core.Class
local UIView = core.UIView
local UIRatinglevelForm = BaseClass("UIRatinglevelForm", UIView)
local base = UIView

local uiid = logic.uiid
UIRatinglevelForm.config = {
    ID = uiid.UIRatinglevelForm,
    AssetName = 'UI/Resident/UI/UIRatinglevelForm',
}

local Cur_enum=nil;

--region 【Awake】
function UIRatinglevelForm:OnInitView()
    UIView.OnInitView(self)
    local get = logic.cs.LuaHelper.GetComponent
    local root = self.uiform.transform

    self.UIMask= root:Find('UIMask').gameObject
  --  self.FrameTrans = get(root,'Canvas/Frame',typeof(logic.cs.RectTransform))
    self.BtnOK = get(root,'Canvas/Frame/BtnOK',typeof(logic.cs.Button))
    self.BtnOK2 = get(root,'Canvas/Frame/BtnOK2',typeof(logic.cs.Button))
    self.BtnCancel = get(root,'Canvas/Frame/BtnCancel',typeof(logic.cs.Button))
    self.BtnCancelTxt = get(root,'Canvas/Frame/BtnCancel/BtnText',typeof(logic.cs.Text))
    self.BtnOkTxt = get(root,'Canvas/Frame/BtnOK/BtnText',typeof(logic.cs.Text))
    self.BtnOk2Txt = get(root,'Canvas/Frame/BtnOK2/BtnText',typeof(logic.cs.Text))
    self.TxtContent = get(root,'Canvas/Frame/ContentBg/Context',typeof(logic.cs.Text))
    self.BtnClose = get(root,'Canvas/Frame/BtnClose',typeof(logic.cs.Button))
    self.feedbackInput = get(root,'Canvas/Frame/feedbackInput',typeof(logic.cs.InputField))
    self.ContentBg= root:Find('Canvas/Frame/ContentBg').gameObject
    self.Star= root:Find('Canvas/Frame/Star').gameObject
    self.SettingBG= root:Find('Canvas/Frame/SettingBG').gameObject
    self.SettingBG1= root:Find('Canvas/Frame/SettingBG1').gameObject

    self.Diamonbg =root:Find('Canvas/Frame/Money/Diamonbg').gameObject;
    self.Keybg =root:Find('Canvas/Frame/Money/Keybg').gameObject;
    self.MoneyText = get(root,'Canvas/Frame/Money/MoneyText',typeof(logic.cs.Text))

    self.MoneyRect = get(root,'Canvas/Frame/Money',typeof(logic.cs.RectTransform));
    self.MoneyTip = get(root,'Canvas/Frame/Money/MoneyTip',typeof(logic.cs.Text))


    self.BtnOK2Rect=get(root,'Canvas/Frame/BtnOK2',typeof(logic.cs.RectTransform))

    --self.Title = get(root,'Canvas/Frame/Title',typeof(logic.cs.Text))
    
   -- self.FrameTrans.DOScaleY(1, 0.25).SetEase(Ease.OutBack).Play();
    
    self.BtnOK.onClick:AddListener(function() self:OnOKclick() end)
    self.BtnCancel.onClick:AddListener(function() self:OnCancelClick() end)
    self.BtnOK2.onClick:AddListener(function() self:OnOK2Click() end)
    self.BtnClose.onClick:AddListener(function() self:OnExitClick() end)

    --发送弹幕
    logic.cs.UIEventListener.AddOnClickListener(self.UIMask, function(data)
        self:OnExitClick()
    end)

end
--endregion


--region 【OnOpen】
function UIRatinglevelForm:OnOpen()
    UIView.OnOpen(self)

    CS.AppRatinglevel.Instance:Init();

    GameHelper.isRating=false;

    logic.cs.UserDataManager.userInfo.data.userinfo.is_store_score=1;
    --设置用户关闭评分  设置类型3 是否已经提示去商店评分 (1是  0否) </param>
    logic.gameHttp:SetUserInfo(function(result) end);

    --默认为游戏内评星界面
    self:SetEnumRatinglevel(EnumRatinglevel.InGame);
end
--endregion


--region 【OnClose】
function UIRatinglevelForm:OnClose()
    UIView.OnClose(self)

    self.BtnOK.onClick:RemoveAllListeners()
    self.BtnCancel.onClick:RemoveAllListeners()
    self.BtnOK2.onClick:RemoveAllListeners()
    self.BtnClose.onClick:RemoveAllListeners()

    logic.cs.UIEventListener.RemoveOnClickListener(self.UIMask, function(data) self:OnExitClick() end)

    self.UIMask = nil;
    self.BtnOK = nil;
    self.BtnOK2 = nil;
    self.BtnCancel = nil;
    self.BtnOkTxt = nil;
    self.BtnOk2Txt = nil;
    self.TxtContent = nil;
    self.BtnClose = nil;
    self.feedbackInput= nil;
    self.ContentBg= nil;
    self.Star= nil;
    self.SettingBG= nil;
    self.SettingBG1= nil;
    self.Diamonbg = nil;
    self.Keybg = nil;
    self.MoneyText = nil;
    self.MoneyRect = nil;
    self.MoneyTip = nil;
    self.BtnOK2Rect = nil;

end
--endregion


--region 【切换页面】
function UIRatinglevelForm:SetEnumRatinglevel(_enum)
    Cur_enum=_enum;
   
    if(_enum==EnumRatinglevel.InGame)then  --游戏内评星系统
        self.BtnOK2.gameObject:SetActiveEx(false);
        self.feedbackInput.gameObject:SetActiveEx(false);

        local cmisc =  logic.cs.GameDataMgr.table:GetBookDetailsById(4);
        self.MoneyText.text="x5";

        --self.Diamonbg:SetActiveEx(true);
        --self.Keybg:SetActiveEx(false);

        --"喜欢";
        self.BtnOkTxt.text=CS.CTextManager.Instance:GetText(305);
        self.BtnCancelTxt.text=CS.CTextManager.Instance:GetText(304);

         --"亲爱的书友，很高兴您来体验我们的游戏，您喜欢目前所体验到的内容吗？";
        self.TxtContent.text=CS.CTextManager.Instance:GetText(299);

    elseif(_enum==EnumRatinglevel.IGGPlatform)then  --平台评星系统界面
        
        self.BtnOK.gameObject:SetActiveEx(false);
        self.BtnCancel.gameObject:SetActiveEx(false);
        self.BtnOK2.gameObject:SetActiveEx(true);


        self.MoneyRect.gameObject:SetActiveEx(false);

        --local cmisc =  logic.cs.GameDataMgr.table:GetBookDetailsById(5);
        --self.MoneyText.text="x2";
        --self.Diamonbg:SetActiveEx(false);
        --self.Keybg:SetActiveEx(true);

        --"前往评分";
        self.BtnOk2Txt.text=CS.CTextManager.Instance:GetText(306);
        --"亲爱的书友，您的鼓励和支持是我们的动力，请为我们评分，您的评分是我们的动力源泉，非常感谢，";
        self.TxtContent.text=CS.CTextManager.Instance:GetText(300);
    elseif(_enum==EnumRatinglevel.IGGPlatformComplete)then  --平台评星系统 完成界面

        self.BtnOK.gameObject:SetActiveEx(false);
        self.BtnCancel.gameObject:SetActiveEx(false);
        self.BtnOK2.gameObject:SetActiveEx(false);
        self.feedbackInput.gameObject:SetActiveEx(false);
        self.Star:SetActiveEx(false);

        --local cmisc =  logic.cs.GameDataMgr.table:GetBookDetailsById(4);
        --self.MoneyText.text="x2";
        --self.Diamonbg:SetActiveEx(false);
        --self.Keybg:SetActiveEx(true);
        self.MoneyRect.gameObject:SetActiveEx(false);

        self.MoneyTip.text="You get";

        --"再次感谢您的鼓励和支持，祝您游戏愉快！";
        self.TxtContent.text=CS.CTextManager.Instance:GetText(301);
    elseif(_enum==EnumRatinglevel.Feedback)then  --反馈建议界面
        self.BtnOK.gameObject:SetActiveEx(false);
        self.BtnCancel.gameObject:SetActiveEx(false);
        self.BtnOK2.gameObject:SetActiveEx(true);

        --local cmisc =  logic.cs.GameDataMgr.table:GetBookDetailsById(4);
        --self.MoneyText.text="x2";
        --self.Diamonbg:SetActiveEx(false);
        --self.Keybg:SetActiveEx(true);
        self.MoneyRect.gameObject:SetActiveEx(false);


        --"前往建议";
        self.BtnOk2Txt.text=CS.CTextManager.Instance:GetText(308);

        --"非常抱歉，没有提供最好的体验给到您，您有什么建议可以提供给我们吗？";
        self.TxtContent.text=CS.CTextManager.Instance:GetText(302);
    elseif(_enum==EnumRatinglevel.FeedbackInput)then  --反馈建议 输入界面
        

        self.SettingBG.gameObject:SetActiveEx(false);
        self.SettingBG1.gameObject:SetActiveEx(true);
        self.Star:SetActiveEx(false);
        self.ContentBg.gameObject:SetActiveEx(false);
        self.feedbackInput.gameObject:SetActiveEx(true);
       -- self.Title.gameObject:SetActiveEx(true);
        self.MoneyRect.anchoredPosition={x=0,y=-36};

        --local cmisc =  logic.cs.GameDataMgr.table:GetBookDetailsById(4);
        --self.MoneyText.text="x2";
        --self.Diamonbg:SetActiveEx(false);
        --self.Keybg:SetActiveEx(true);
        self.MoneyRect.gameObject:SetActiveEx(false);

        self.MoneyTip.text="You Will get";
        self.BtnOK2Rect.anchoredPosition = {x=0,y=-118};
        --"提交";
        self.BtnOk2Txt.text=CS.CTextManager.Instance:GetText(308);
        
    elseif(_enum==EnumRatinglevel.FeedbackSend)then   --反馈建议 提交后界面
        
        --self.ContentBg.gameObject:SetActiveEx(true);
        --self.feedbackInput.gameObject:SetActiveEx(false);
        --self.BtnOK2.gameObject:SetActiveEx(false);
        --self.MoneyTip.text="You get";
        --self.MoneyText.text="x2";
        --self.Diamonbg:SetActiveEx(false);
        --self.Keybg:SetActiveEx(true);

        --"感谢您的建议，您的建议已经提交至游戏研发团队，感谢您的支持！";
        --self.TxtContent.text=CS.CTextManager.Instance:GetText(303);

    end
    
end

--endregion


--region【点击按钮1】
function UIRatinglevelForm:OnOKclick()

    if(Cur_enum==EnumRatinglevel.InGame)then
        ----点击喜欢 前往平台评星系统界面
        --self:SetEnumRatinglevel(EnumRatinglevel.IGGPlatform);
        ----请求列表 【领取游戏内评分奖励】
        --logic.gameHttp:ReceiveGameScoreAward(function(result) self:ReceiveGameScoreAward(result); end)



        --请求列表 【领取游戏内评分奖励】
        logic.gameHttp:ReceiveGameScoreAward(function(result) self:ReceiveGameScoreAward(result); end)

        --平台SDK回调发起
        CS.XLuaHelper.RequestRating(1,"","");

        --请求列表 【领取游戏内评分奖励】
       -- logic.gameHttp:ReceivePlatformAward(function(result) self:ReceivePlatformAward(result); end)

    end
    
end
--endregion


--region 【点击不喜欢按钮】
function UIRatinglevelForm:OnCancelClick()
    
    if(Cur_enum==EnumRatinglevel.InGame)then
        --点击不喜欢 前往反馈建议界面
        self:SetEnumRatinglevel(EnumRatinglevel.Feedback);

        --请求列表 【领取游戏内评分奖励】
        logic.gameHttp:ReceiveGameScoreAward(function(result) self:ReceiveGameScoreAward(result); end)
    end
    
end
--endregion


--region【点击按钮2】
function UIRatinglevelForm:OnOK2Click()

    if(Cur_enum==EnumRatinglevel.IGGPlatform)then  --点击前往评分按钮  前往平台评星系统界面

        logic.debug.Log("跳转到平台评星");
        
        --平台SDK回调发起
        CS.XLuaHelper.RequestRating(1,"","");

        --请求列表 【领取游戏内评分奖励】
        logic.gameHttp:ReceivePlatformAward(function(result) self:ReceivePlatformAward(result); end)

    elseif(Cur_enum==EnumRatinglevel.Feedback)then  --反馈建议界面

        --点击前往建议 前往反馈建议输入界面
        self:SetEnumRatinglevel(EnumRatinglevel.FeedbackInput);
        
      
    elseif(Cur_enum==EnumRatinglevel.FeedbackInput)then  --反馈建议 输入界面


        if (self.feedbackInput.text==nil or self.feedbackInput.text=="")then
            local str=CS.CTextManager.Instance:GetText(430)   --["反馈内容不能为空"]
            logic.cs.UITipsMgr:PopupTips(str, false);
            return;
        end

        local description = string.trim(self.feedbackInput.text);
        local len = string.GetUtf8Len(description);
        
        if (len< 10)then
            local str=CS.CTextManager.Instance:GetText(431)   --["您的建议字数不足，最少需要10个字符。"]
            logic.cs.UITipsMgr:PopupTips(str, false);
            return;
        end

        if (len>300)then
            local str=CS.CTextManager.Instance:GetText(432)   --["字数超过限制提示：您输入的字数超过300字限制！"]
            logic.cs.UITipsMgr:PopupTips(str, false);
            return;
        end


        CS.XLuaHelper.RequestRating(2,self.feedbackInput.text,"5");
            --平台建议发送发起
            --CS.XLuaHelper.feedBack(self.feedbackInput.text,"5");



        --请求列表 【领取游戏内评分奖励】
        logic.gameHttp:ReceivePlatformAward(function(result) self:ReceivePlatformAward2(result); end)

        --请求列表 【评分建议保存】
        logic.gameHttp:ScoreSuggest(self.feedbackInput.text,function(result) self:ScoreSuggest(result); end)

    end
    
end
--endregion


--region 【领取钻石响应】
local hasGet1=false;
function UIRatinglevelForm:ReceiveGameScoreAward(result)
    logic.debug.Log("----ReceiveGameScoreAward---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        if(hasGet1==false)then
            --刷新自己的钱
            logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
            logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

            hasGet1=true;
        end
    elseif(code == 10301)then
    else
        logic.debug.LogError("----updateReadingTaskTime----> ERROR:"..json.msg);
        -- logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion


--region【领取钥匙响应】
local hasGet2=false;
function UIRatinglevelForm:ReceivePlatformAward(result)
    logic.debug.Log("----ReceivePlatformAward---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        if(hasGet2==false)then
            --刷新自己的钱
            logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
            logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

            hasGet2=true;
        end
    elseif(code == 10301)then
    else
        logic.debug.LogError("----updateReadingTaskTime----> ERROR:"..json.msg);
        -- logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end


function UIRatinglevelForm:ReceivePlatformAward2(result)
    logic.debug.Log("----ReceivePlatformAward---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        if(hasGet2==false)then
            --刷新自己的钱
            logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
            logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))

            local str=CS.CTextManager.Instance:GetText(303);
            logic.cs.UITipsMgr:PopupTips(str, false);

            hasGet2=true;

            self:OnExitClick();
        end
    elseif(code == 10301)then
    else
        logic.debug.LogError("----updateReadingTaskTime----> ERROR:"..json.msg);
        -- logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end

--endregion


--region 【设置用户关闭评分 响应】
function UIRatinglevelForm:SetUserRating(result)
    logic.debug.Log("----SetUserRating---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then
        logic.cs.UserDataManager.userInfo.data.userinfo.is_store_score=1;
    else
        logic.debug.LogError("----SetUserRating----> ERROR:"..json.msg);
        -- logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion


--region 【评分建议保存】
function UIRatinglevelForm:ScoreSuggest(result)
    logic.debug.Log("----ScoreSuggest---->" .. result);
    local json = core.json.Derialize(result)
    local code = tonumber(json.code)
    if(code == 200)then

    elseif(code == 10305)then
    elseif(code == 10305)then
    else
        logic.debug.LogError("----ScoreSuggest----> ERROR:"..json.msg);
        -- logic.cs.UIAlertMgr:Show("TIPS",json.msg)
    end
end
--endregion


--region 【点击关闭按钮】
function UIRatinglevelForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion


return UIRatinglevelForm