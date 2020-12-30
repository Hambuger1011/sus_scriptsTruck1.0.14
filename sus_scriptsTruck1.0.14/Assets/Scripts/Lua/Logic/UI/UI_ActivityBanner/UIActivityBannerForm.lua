local UIView = core.UIView

local UIActivityBannerForm = core.Class("UIActivityBannerForm", UIView)


local uiid = logic.uiid
UIActivityBannerForm.config = {
    ID = uiid.UIActivityBannerForm,
    AssetName = 'UI/Resident/UI/UIActivityBannerForm'
}


--region【Awake】

local this=nil;
function UIActivityBannerForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;


    self.ActivityBanner1 =CS.DisplayUtil.GetChild(this.gameObject, "ActivityBanner1"):GetComponent("Image");
    self.Titile1 =CS.DisplayUtil.GetChild(self.ActivityBanner1.gameObject, "Titile1"):GetComponent("Text");
    self.TimeLeftText1 =CS.DisplayUtil.GetChild(self.ActivityBanner1.gameObject, "TimeLeftText1"):GetComponent("Text");
    self.Image6 =CS.DisplayUtil.GetChild(self.ActivityBanner1.gameObject, "Image");

    self.ActivityBanner2 =CS.DisplayUtil.GetChild(this.gameObject, "ActivityBanner2"):GetComponent("Image");
    self.Titile2 =CS.DisplayUtil.GetChild(self.ActivityBanner2.gameObject, "Titile2"):GetComponent("Text");
    self.TimeLeftText2 =CS.DisplayUtil.GetChild(self.ActivityBanner2.gameObject, "TimeLeftText2"):GetComponent("Text");

    self.Titile1.fontSize=38;
    self.Titile2.fontSize=38;
    self.ActivityBanner1.sprite=nil;
    self.ActivityBanner2.sprite=nil;

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.ActivityBanner1.gameObject,function(data) self:ActivityBanner1Click() end)
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.ActivityBanner2.gameObject,function(data) self:ActivityBanner2Click() end)

    self._state1=0;
    self._state2=0;
    self.showlist={};

    self.AllCount=0;

    self.MoveSprite=nil;

end
--endregion


--region【动画控制】

local twer1=nil
local twer2=nil
local num=0;
local target1=0;
local target2=0;
function UIActivityBannerForm:Anima()
    self.AllCount = self.AllCount+1;
    if(self.showlist==nil)then return; end
    local len=table.length(self.showlist);

    num=num+1;
    if(num%2==0)then
        target1=0;
        target2=-750;
    else
        target1=-750;
        target2=0;
    end

    local _index=self.AllCount%len;
    if(_index==0)then _index=len; end

    twer1 =self.ActivityBanner1.gameObject.transform:DOLocalMoveX(target1, 1);
    twer1:SetAutoKill(true):SetEase(core.tween.Ease.Flash)
    twer1:OnComplete(function()
        if(target1==-750)then
            self.Banner1Rect.anchoredPosition={x=750,y=0};

            if(len>=3)then
                self:SetView(self.showlist[_index]);
            end
        end
    end)
    twer1:Play();

    twer2 =self.ActivityBanner2.gameObject.transform:DOLocalMoveX(target2, 1);
    twer2:SetAutoKill(true):SetEase(core.tween.Ease.Flash)
    twer2:OnComplete(function()
        if(target2==-750)then
            self.Banner2Rect.anchoredPosition={x=750,y=0};

            if(len>=3)then
                self:SetView2(self.showlist[_index]);
            end
        end
    end)
    twer2:Play();
end

--endregion


--region【OnOpen】

function UIActivityBannerForm:OnOpen()
    UIView.OnOpen(self)
    GameController.ActivityBannerControl:SetData(self);




end

--endregion


--region 【OnClose】

function UIActivityBannerForm:OnClose()
    UIView.OnClose(self)

    GameController.ActivityBannerControl:SetData(nil);

    if(self.ActivityBanner1)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.ActivityBanner1.gameObject,function(data) self:ActivityBanner1Click() end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.ActivityBanner2.gameObject,function(data) self:ActivityBanner2Click() end)
    end


    if(twer1)then
        twer1:Kill()
    end
    if(twer2)then
        twer2:Kill()
    end

    --【关闭动画计时器】
    GameHelper.ClearBNTimer();

    self.ActivityBanner1 = nil;
    self.Titile1  = nil;
    self.TimeLeftText1  = nil;
    self.Banner1Rect = nil;
    self.ActivityBanner2  = nil;
    self.Titile2 = nil;
    self.TimeLeftText2  = nil;
    self.Banner2Rect = nil;
    self._state1=nil;
    self._state2=nil;
    self.showlist =nil;
    self.AllCount=nil;
    self.gameObject = nil;

    GameController.MainFormControl:MoveBanner() --把主界面偏移 收回
end

--endregion


--region【设置数据】
function UIActivityBannerForm:SetInfo()
    --【关闭动画计时器】
    GameHelper.ClearBNTimer();

    self.showlist={};
    self.AllCount=0;

    --【临时】【临时】
    if(Cache.MainCache.migration.migration_web_switch==1)then
        table.insert(self.showlist,EnumActivity.MoveCode);
    end
    --【临时】【临时】

    if(Cache.LimitTimeActivityCache.ColoredEgg.is_open==1)then
        table.insert(self.showlist,EnumActivity.ColoredEgg);
    end
    if(Cache.LimitTimeActivityCache.SpinDraw.is_open==1)then
        table.insert(self.showlist,EnumActivity.SpinDraw);
    end
    if(Cache.LimitTimeActivityCache.FreeKey.is_open==1)then
        table.insert(self.showlist,EnumActivity.FreeKey);
    end


    local len=table.length(self.showlist);

    if(len==0)then
        self:OnExitClick();
    elseif(len==1)then
        self:SetView(self.showlist[1]);
    elseif(len==2)then
        self:SetView(self.showlist[1]);
        self:SetView2(self.showlist[2]);
        --开启计时器
        GameHelper.MainBannerTimer(function() self:Anima() end)
    elseif(len>3)then
        self:SetView(self.showlist[1]);
        self:SetView2(self.showlist[2]);
        self.AllCount=2;
        --开启计时器
        GameHelper.MainBannerTimer(function() self:Anima() end)
    end


    GameController.MainFormControl:MoveBanner();

    --【设置层级】【设置层级】
    local animaTimer=self.Image6.transform:DOLocalMoveX(233, 0.2):SetAutoKill(true):SetEase(core.tween.Ease.Flash)

    animaTimer:OnComplete(function()
        --设置层级
        if(self.uiform:GetSortingOrder()>3090)then
            --设置层级
            self.uiform:SetDisplayOrder(8);

            if(self.uiform:GetSortingOrder()>3090)then
                animaTimer:Play();
            end
        end
    end)
    animaTimer:Play();

end


function UIActivityBannerForm:SetView(state)

    self._state1=state;
    if(state== EnumActivity.ColoredEgg)then    --彩蛋
        self.ActivityBanner1.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityBanner/act_bg_eggs");
        self.Titile1.text="Hunt Mystery Eggs to win!"
        self:Countdown1(Cache.LimitTimeActivityCache.ColoredEgg.countdown);
    elseif(state== EnumActivity.SpinDraw)then   --转盘抽奖
        self.ActivityBanner1.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityBanner/act_bg_rotary");
        self.Titile1.text="Spin to get Keys & Diamonds!"
        self:Countdown1(Cache.LimitTimeActivityCache.SpinDraw.countdown);
    elseif(state== EnumActivity.FreeKey)then   --全书免费
        self.ActivityBanner1.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityBanner/act_bg_fete1");
        self.Titile1.text="Key-Free Reading Carnival!"
        self:Countdown1(Cache.LimitTimeActivityCache.FreeKey.countdown);
    elseif(state== EnumActivity.MoveCode)then   --迁移
        self.ActivityBanner1.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityBanner/act_img_smg");
        --【临时】【临时】
        --显示图片
        self.Titile1.text=Cache.MainCache.migration.migration_banner_title;
       -- self.Titile1_Clone.text=Cache.MainCache.migration.migration_banner_content;
    end
end

function UIActivityBannerForm:SetView2(state)

    self._state2=state;
    if(state== EnumActivity.ColoredEgg)then    --彩蛋
        self.ActivityBanner2.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityBanner/act_bg_eggs");
        self.Titile2.text="Hunt Mystery Eggs to win!"
        self:Countdown2(Cache.LimitTimeActivityCache.ColoredEgg.countdown);
    elseif(state== EnumActivity.SpinDraw)then   --转盘抽奖
        self.ActivityBanner2.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityBanner/act_bg_rotary");
        self.Titile2.text="Spin to get Keys & Diamonds!"
        self:Countdown2(Cache.LimitTimeActivityCache.SpinDraw.countdown);
    elseif(state== EnumActivity.FreeKey)then   --全书免费
        self.ActivityBanner2.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityBanner/act_bg_fete1");
        self.Titile2.text="Key-Free Reading Carnival!"
        self:Countdown2(Cache.LimitTimeActivityCache.FreeKey.countdown);
    elseif(state== EnumActivity.MoveCode)then   --迁移
        self.ActivityBanner2.sprite = CS.ResourceManager.Instance:GetUISprite("ActivityBanner/act_img_smg");
        --【临时】【临时】
        self.Titile2.text=Cache.MainCache.migration.migration_banner_title;
      --  self.Titile2_Clone.text=Cache.MainCache.migration.migration_banner_content;
    end
end
--endregion


--region 【点击ActivityBanner1横条图】
function UIActivityBannerForm:ActivityBanner1Click()
    if(self._state1== EnumActivity.ColoredEgg)then    --彩蛋
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,3);
        end

    elseif(self._state1== EnumActivity.SpinDraw)then   --转盘抽奖
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,3);
        end
    elseif(self._state1== EnumActivity.FreeKey)then   --转盘抽奖
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,1);
        end
    elseif(self._state1== EnumActivity.MoveCode)then   --迁移
        --【临时】【临时】
        if(Cache.MainCache.migration.migration_web_url)then
            local _url=Cache.MainCache.migration.migration_web_url.."?token="..string.encodeURI(logic.cs.GameHttpNet.TOKEN);
            logic.cs.Application.OpenURL(_url);
        end

    end
end
--endregion


--region 【点击ActivityBanner2横条图】
function UIActivityBannerForm:ActivityBanner2Click()
    if(self._state2== EnumActivity.ColoredEgg)then    --彩蛋
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,3);
        end

    elseif(self._state2== EnumActivity.SpinDraw)then   --转盘抽奖
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,3);
        end
    elseif(self._state2== EnumActivity.FreeKey)then   --转盘抽奖
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,1);
        end
    elseif(self._state2== EnumActivity.MoveCode)then   --迁移
        --【临时】【临时】
        if(Cache.MainCache.migration.migration_web_url)then
            local _url=Cache.MainCache.migration.migration_web_url.."?token="..string.encodeURI(logic.cs.GameHttpNet.TOKEN);
            logic.cs.Application.OpenURL(_url);
        end
    end
end
--endregion


--region【倒计时1】
function UIActivityBannerForm:Countdown1(_time)
    if(_time and _time>0)then
        local day =  math.modf( _time / 86400 )
        _time=math.fmod(_time, 86400);
        local hour =  math.modf( _time / 3600 )
        local minute = math.fmod( math.modf(_time / 60), 60 )
        --local second = math.fmod(_time, 60 )
        -- self.TimeLeftText.text = string.format("%02d:%02d", hour, minute)
        self.TimeLeftText1.text =day.."d:"..hour.."h:"..minute.."m";
    end
end
--endregion


--region【倒计时2】
function UIActivityBannerForm:Countdown2(_time)
    if(_time and _time>0)then
        local day =  math.modf( _time / 86400 )
        _time=math.fmod(_time, 86400);
        local hour =  math.modf( _time / 3600 )
        local minute = math.fmod( math.modf(_time / 60), 60 )
        --local second = math.fmod(_time, 60 )
        -- self.TimeLeftText.text = string.format("%02d:%02d", hour, minute)
        self.TimeLeftText2.text =day.."d:"..hour.."h:"..minute.."m";
    end
end
--endregion


--region【加载默认倒计时】
function UIActivityBannerForm:DefaultCountDown()
    if(self.showlist==nil)then return; end
    local len=table.length(self.showlist);

    if(len==1)then
        self:SetView(self.showlist[1]);
    elseif(len==2)then
        self:SetView(self.showlist[1]);
        self:SetView2(self.showlist[2]);

    elseif(len==3)then
        self:SetView(self.showlist[1]);
        self:SetView2(self.showlist[2]);
        self.AllCount=2;
    end

    GameController.MainFormControl:MoveBanner();
end
--endregion


--region 【界面关闭】
function UIActivityBannerForm:OnExitClick()
    UIView.__Close(self)
    if(self.onClose)then
        self.onClose()
    end
end
--endregion



return UIActivityBannerForm