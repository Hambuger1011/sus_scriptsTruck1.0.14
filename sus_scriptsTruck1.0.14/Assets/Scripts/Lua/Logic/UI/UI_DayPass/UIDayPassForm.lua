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

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.ReadBtn,function(data) self:ReadBtnClick(data) end);
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

end

--endregion

function UIDayPassForm:SetInfo(daypassInfo)
    --显示书本背景
    if(daypassInfo)then
        GameHelper.ShowChapterBG(daypassInfo.book_id,self.Bookbg);
        self.BookInfo=daypassInfo;
        daypassInfo.isOpened=true;
    else
        self:OnExitClick()
    end
    --显示书本背景
end



function UIDayPassForm:ReadBtnClick()
    if(self.BookInfo==nil)then
        return;
    end
    --【点击书本】
    GameHelper.BookClick(self.BookInfo);
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