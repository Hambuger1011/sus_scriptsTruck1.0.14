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

    --【第一個Banner容器】
    self.ActivityBanner1 =CS.DisplayUtil.GetChild(this.gameObject, "ActivityBanner1"):GetComponent("Image");
    self.Titile1 =CS.DisplayUtil.GetChild(self.ActivityBanner1.gameObject, "Titile1"):GetComponent("Text");
    self.Content1 =CS.DisplayUtil.GetChild(self.ActivityBanner1.gameObject, "Content1"):GetComponent("Text");
    self.TimeObj1 =CS.DisplayUtil.GetChild(self.ActivityBanner1.gameObject, "TimeObj1");
    self.TimeLeftText1 =CS.DisplayUtil.GetChild(self.TimeObj1, "TimeLeftText1"):GetComponent("Text");
    self.TextCanvas1=CS.DisplayUtil.GetChild(this.gameObject, "TextCanvas1"):GetComponent("RectTransform");
    self.Banner1Rect=self.ActivityBanner1:GetComponent("RectTransform");

    self.Text1=CS.DisplayUtil.GetChild(self.TimeObj1, "Text");
    self.Image1=CS.DisplayUtil.GetChild(self.TimeObj1, "Image");
    self.TimeLeftText1Rect=self.TimeLeftText1.gameObject:GetComponent("RectTransform");

    self.Titile1.gameObject:SetActive(false);
    self.Content1.gameObject:SetActive(false);
    self.Text1.gameObject:SetActive(false);
    self.Image1.gameObject:SetActive(false);
    self.TimeLeftText1Rect.anchoredPosition={x=300,y=-58};

    --【第二個Banner容器】
    self.ActivityBanner2 =CS.DisplayUtil.GetChild(this.gameObject, "ActivityBanner2"):GetComponent("Image");
    self.Titile2 =CS.DisplayUtil.GetChild(self.ActivityBanner2.gameObject, "Titile2"):GetComponent("Text");
    self.Content2 =CS.DisplayUtil.GetChild(self.ActivityBanner2.gameObject, "Content2"):GetComponent("Text");
    self.TimeObj2 =CS.DisplayUtil.GetChild(self.ActivityBanner2.gameObject, "TimeObj2");
    self.TimeLeftText2 =CS.DisplayUtil.GetChild(self.TimeObj2, "TimeLeftText2"):GetComponent("Text");
    self.Titile2_Rect=self.Titile2.gameObject:GetComponent("RectTransform");
    self.TextCanvas2=CS.DisplayUtil.GetChild(this.gameObject, "TextCanvas2"):GetComponent("RectTransform");
    self.Banner2Rect=self.ActivityBanner2:GetComponent("RectTransform");

    self.Text2=CS.DisplayUtil.GetChild(self.TimeObj2, "Text");
    self.Image2=CS.DisplayUtil.GetChild(self.TimeObj2, "Image");
    self.TimeLeftText2Rect=self.TimeLeftText2.gameObject:GetComponent("RectTransform");

    self.Titile2.gameObject:SetActive(false);
    self.Content2.gameObject:SetActive(false);
    self.Text2.gameObject:SetActive(false);
    self.Image2.gameObject:SetActive(false);
    self.TimeLeftText2Rect.anchoredPosition={x=300,y=-58};

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.ActivityBanner1.gameObject,function(data) self:ActivityBanner1Click() end)
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.ActivityBanner2.gameObject,function(data) self:ActivityBanner2Click() end)

    self._Info1=nil;
    self._Info2=nil;

    self.showlist={};
    self.AllCount=0;
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
    self._Info1=nil;
    self._Info2=nil;
    self.showlist =nil;
    self.AllCount=nil;
    self.gameObject = nil;

    GameController.MainFormControl:MoveBanner() --把主界面偏移 收回

    twer1=nil
    twer2=nil
    num=0;
    target1=0;
    target2=0;
end

--endregion


--region【设置数据】
function UIActivityBannerForm:SetInfo()
    --【关闭动画计时器】
    GameHelper.ClearBNTimer();

    self.showlist={};
    self.AllCount=0;

    ----【临时】【临时】
    --if(Cache.MainCache.migration.migration_web_switch==1)then
    --    table.insert(self.showlist,EnumActivity.MoveCode);
    --end
    ----【临时】【临时】


    if(GameHelper.islistHave(Cache.LimitTimeActivityCache.ActivityList)==true)then
        local len=table.length(Cache.LimitTimeActivityCache.ActivityList);
        for i = 1, len do
            --如果本活动开启
            if(Cache.LimitTimeActivityCache.ActivityList[i].is_open==1)then
                if(Cache.LimitTimeActivityCache.ActivityList[i].id~=EnumActivity.Investment)then  --如果不是 投资活动
                    table.insert(self.showlist,Cache.LimitTimeActivityCache.ActivityList[i].id);
                end
            end
        end
    end
    local len=table.length(self.showlist);
    if(len==0)then
        GameController.ActivityBannerControl:CloseUI();
    elseif(len==1)then
        self:SetView(self.showlist[1]);
    elseif(len==2)then
        self:SetView(self.showlist[1]);
        self:SetView2(self.showlist[2]);
        --开启计时器
        GameHelper.MainBannerTimer(function() self:Anima() end)
    elseif(len>=3)then
        self:SetView(self.showlist[1]);
        self:SetView2(self.showlist[2]);
        self.AllCount=2;
        --开启计时器
        GameHelper.MainBannerTimer(function() self:Anima() end)
    end

    GameController.MainFormControl:MoveBanner();
end


function UIActivityBannerForm:SetView(state)
    self.TimeObj1:SetActive(true);
    self.TextCanvas1.anchoredPosition={x=375,y=50};

    if(state== EnumActivity.MoveCode)then --【如果是迁移活动】
        --【临时】【临时】
        --显示图片
        self.TimeObj1:SetActive(false);
        self.TextCanvas1.anchoredPosition={x=270,y=50};
        GameHelper.ShowNetImage_Banner(state,Cache.MainCache.migration.migration_banner_img,self.ActivityBanner1);
        self._Info1={};
        self._Info1.type=0;
    else
        local Info=Cache.LimitTimeActivityCache:GetActivityInfo(state);
        if(Info)then
            self._Info1=Info;
            GameHelper.ShowNetImage_Banner(state,Info.img_src,self.ActivityBanner1);
            self:Countdown1(Info.countdown);
        end
    end
end

function UIActivityBannerForm:SetView2(state)
    self.TimeObj2:SetActive(true);
    self.TextCanvas2.anchoredPosition={x=375,y=50};

    if(state== EnumActivity.MoveCode)then --【如果是迁移活动】
        --【临时】【临时】
        self.TimeObj2:SetActive(false);
        self.TextCanvas2.anchoredPosition={x=270,y=50};
        GameHelper.ShowNetImage_Banner(state,Cache.MainCache.migration.migration_banner_img,self.ActivityBanner2);
        self._Info2={};
        self._Info2.type=0;
    else
        local Info=Cache.LimitTimeActivityCache:GetActivityInfo(state);
        if(Info)then
            self._Info2=Info;
            GameHelper.ShowNetImage_Banner(state,Info.img_src,self.ActivityBanner2);
            self:Countdown2(Info.countdown);
        end
    end
end
--endregion


--region 【点击ActivityBanner1横条图】
function UIActivityBannerForm:ActivityBanner1Click()
    if(self._Info1==nil)then return; end
    --【跳转位置类型 ：1.跳转外部链接（跳到jump_url）， 2.跳到events界面， 3.跳到News界面 4.打开最后书本的书本弹窗】
    if(self._Info1.type== 0)then
        if(Cache.MainCache.migration.migration_web_url)then
            local _url=Cache.MainCache.migration.migration_web_url.."?token="..string.encodeURI(logic.cs.GameHttpNet.TOKEN);
            logic.cs.Application.OpenURL(_url);
        end
    elseif(self._Info1.type== 1)then
        if(self._Info1.jump_url)then
            logic.cs.Application.OpenURL(self._Info1.jump_url);
        end
    elseif(self._Info1.type== 2)then
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,1);
        end
    elseif(self._Info1.type== 3)then
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,3);
        end
    elseif(self._Info1.type== 4)then
        if(GameHelper.CurBookId and GameHelper.CurBookId>0)then
            local bookinfo={};
            bookinfo.book_id=GameHelper.CurBookId;
            GameHelper.BookClick(bookinfo);

            --埋点*点击前往阅读
            logic.cs.GamePointManager:BuriedPoint("activity_cumulative_read_go");
        end
    end

end
--endregion


--region 【点击ActivityBanner2横条图】
function UIActivityBannerForm:ActivityBanner2Click()
    if(self._Info2==nil)then return; end
    --【跳转位置类型 ：1.跳转外部链接（跳到jump_url）， 2.跳到events界面， 3.跳到News界面 4.打开最后书本的书本弹窗】
    if(self._Info2.type== 0)then
        if(Cache.MainCache.migration.migration_web_url)then
            local _url=Cache.MainCache.migration.migration_web_url.."?token="..string.encodeURI(logic.cs.GameHttpNet.TOKEN);
            logic.cs.Application.OpenURL(_url);
        end
    elseif(self._Info2.type== 1)then
        if(self._Info2.jump_url)then
            logic.cs.Application.OpenURL(self._Info2.jump_url);
        end
    elseif(self._Info2.type== 2)then
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,1);
        end
    elseif(self._Info2.type== 3)then
        --界面
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
        if(uiform)then
            uiform.RwardToggle:Set(true);
            uiform:RwardToggleClick(nil,3);
        end
    elseif(self._Info1.type== 4)then
        if(GameHelper.CurBookId and GameHelper.CurBookId>0)then
            local bookinfo={};
            bookinfo.book_id=GameHelper.CurBookId;
            GameHelper.BookClick(bookinfo);
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

    if(len==0)then
        GameController.ActivityBannerControl:CloseUI();
    elseif(len==1)then
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