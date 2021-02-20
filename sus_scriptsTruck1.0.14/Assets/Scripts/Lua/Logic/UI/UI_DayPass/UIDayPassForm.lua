local UIView = core.UIView
local UIDayPassForm = core.Class("UIDayPassForm", UIView)


local uiid = logic.uiid
UIDayPassForm.config = {
    ID = uiid.UIDayPassForm,
    AssetName = 'UI/Resident/UI/UIDayPassForm'
}

--region【Awake】

local this=nil;
function UIDayPassForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;


    self.ReadBtn = CS.DisplayUtil.GetChild(this.gameObject, "ReadBtn");
    self.CloseBtn = CS.DisplayUtil.GetChild(this.gameObject, "UIMask");
    --标签按钮 文字文本读配置表
    self.Bookbg = CS.DisplayUtil.GetChild(this.gameObject, "Bookbg"):GetComponent("Image");
    self.DayPassBg = CS.DisplayUtil.GetChild(this.gameObject, "DayPassBg");
    self.DayPassBgRect = self.DayPassBg:GetComponent("RectTransform");
    self.TimeBg = CS.DisplayUtil.GetChild(this.gameObject, "TimeBg");
    self.TimeText = CS.DisplayUtil.GetChild(self.TimeBg, "TimeText"):GetComponent("Text");
    self.NewBg = CS.DisplayUtil.GetChild(self.Bookbg.gameObject, "NewBg");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.ReadBtn,function(data) self:ReadBtnClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Bookbg.gameObject,function(data) self:ReadBtnClick() end);
    logic.cs.UIEventListener.AddOnClickListener(self.CloseBtn,function(data)  self:NextShow(data) end);

    self.BookInfo=nil;
end
--endregion


--region【OnOpen】

function UIDayPassForm:OnOpen()
    UIView.OnOpen(self)


end

--endregion


--region 【OnClose】

function UIDayPassForm:OnClose()
    UIView.OnClose(self)
    logic.cs.UIEventListener.RemoveOnClickListener(self.ReadBtn,function(data) self:ReadBtnClick(data) end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.CloseBtn,function(data) self:NextShow() end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.Bookbg.gameObject,function(data) self:ReadBtnClick() end);

end

--endregion

function UIDayPassForm:SetInfo(daypassInfo)
    --显示书本背景
    if(daypassInfo)then
        GameHelper.ShowChapterBG(daypassInfo.book_id,self.Bookbg);
        self.BookInfo=daypassInfo;
        daypassInfo.isOpened=true;
        Cache.PopWindowCache:AddDayPass2(daypassInfo.book_id);
    else
        self:OnExitClick()
    end
    --显示书本背景


    --【倒计时 显示】
    local _time= daypassInfo.countdown;
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
        if(str)then
            self.TimeText.text=str;
        end
        self.TimeBg:SetActive(true);
    else
        self.TimeBg:SetActive(false);
    end

    if(daypassInfo.show_type==1)then
      GameController.DayPassController:PopUpDayPassBookRequest(daypassInfo.book_id);
    end

    --【显示New标签】
    GameHelper.ShowNewBg(daypassInfo.book_id,self.NewBg);
end

function UIDayPassForm:ShowTime(book_id,str)
    if(self.BookInfo.book_id==book_id)then
        self.TimeText.text=str;
    end
end

function UIDayPassForm:ReadBtnClick()
    if(self.BookInfo==nil)then
        return;
    end

    if(self.BookInfo.jump_type and self.BookInfo.jump_type==1)then
        --【点击书本】
        GameHelper.BookClick(self.BookInfo);
    else
        --【直接进入书本】
        GameHelper.EnterReadBook(self.BookInfo.book_id);
    end

    self:OnExitClick();
end


function UIDayPassForm:NextShow()
    local twer1 =self.DayPassBg.transform:DOLocalMoveX(1000, 0.3);
    twer1:SetAutoKill(true):SetEase(core.tween.Ease.OutBack);
    twer1:OnComplete(function()
        self.Bookbg.sprite=nil;
        self.DayPassBgRect.anchoredPosition={x=-1000,y=0};
        self.DayPassBg.transform:DOLocalMoveX(0, 0.3):SetAutoKill(true):SetEase(core.tween.Ease.OutBack):Play();
        GameController.WindowConfig:ShowDayPass();
    end)
    twer1:Play();
end


--region 【界面关闭】
function UIDayPassForm:OnExitClick()
    UIView.__Close(self)

    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIDayPassForm