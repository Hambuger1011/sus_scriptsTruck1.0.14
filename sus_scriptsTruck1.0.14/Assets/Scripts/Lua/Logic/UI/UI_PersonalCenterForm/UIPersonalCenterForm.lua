local UIView = core.UIView
local UIPersonalCenterForm = core.Class("UIPersonalCenterForm", UIView)
local this=UIPersonalCenterForm;

local uiid = logic.uiid
UIPersonalCenterForm.config = {
    ID = uiid.UIPersonalCenterForm,
    AssetName = 'UI/Resident/UI/UIPersonalCenterForm'
}

local ImageWallIsShow = nil;

--region【Awake】
function UIPersonalCenterForm:OnInitView()
    UIView.OnInitView(self)
    
    local gameObject = self.uiform.gameObject

    self.ImageWallParent = CS.DisplayUtil.GetChild(gameObject, "ImageWallParent");
    self.BG = CS.DisplayUtil.GetChild(gameObject, "BG");
    self.ImageWallObj = CS.ResourceManager.Instance:LoadAssetBundleUI(logic.cs.UIFormName.ImageWall);
    self.ImageWallBtn = self.ImageWallObj:GetComponent(typeof(logic.cs.Button));
    
    self.UserInfoObj = CS.DisplayUtil.GetChild(gameObject, "UserInfo");
    self.UserInfo= require('Logic/UI/UI_PersonalCenterForm/UserInfoPanel').New(self.UserInfoObj);
    
    self.BookListObj = CS.DisplayUtil.GetChild(gameObject, "BookList");
    self.BookList= require('Logic/UI/UI_PersonalCenterForm/BookList').New(self.BookListObj);
end
--endregion

--region【OnOpen】

function UIPersonalCenterForm:OnOpen()
    UIView.OnOpen(self)
    
    --按钮监听
    --logic.cs.UIEventListener.AddOnClickListener(self.RomanceTab.gameObject,function(data) self:RomanceTabClick(data) end);

    self.ImageWallObj.transform:SetParent(self.ImageWallParent.transform, false);
    self.ImageWall= require('Logic/UI/UI_ImageWall/ImageWall').New(self.ImageWallObj.gameObject);
    self.ImageWallBtn.onClick:AddListener(function()
        self:ImageWallShow()
        self.ImageWall:SetChoiceBGShow(true)
    end);
    self.ImageWall:SetHideOnClick(function()
        self.ImageWall:SetChoiceBGShow(false)
        self:ImageWallHide()
    end);
    
    self.UserInfo:UpdateInfo();
    local myBookIDs =  logic.cs.GameDataMgr.userData:GetMyBookIds();
    local len=myBookIDs.Count;
    local mybooks={};
    for i = 1, len do
        local _index=i-1;
        local t_BookDetails = logic.cs.JsonDTManager:GetJDTBookDetailInfo(myBookIDs[_index]);
        if(t_BookDetails and (CS.XLuaHelper.is_Null(t_BookDetails)==false))then
            local info={};
            info.book_id=t_BookDetails.id;
            info.bookname=t_BookDetails.bookname;
            info.update_time=0;
            table.insert(mybooks,info);
        else
            logic.debug.LogError("t_BookDetails is Null");
        end
    end
    self.BookList:UpdateList(mybooks);
end

--endregion

--region 【OnClose】

function UIPersonalCenterForm:OnClose()
    UIView.OnClose(self)
    self.ImageWallBtn.onClick:RemoveAllListeners()
    --logic.cs.UIEventListener.RemoveOnClickListener(self.RomanceTab.gameObject,function(data) self:RomanceTabClick(data) end);

    ImageWallIsShow = nil
end

--endregion


--region 【展示背景墙】
function UIPersonalCenterForm:ImageWallShow()
    if ImageWallIsShow then
        return
    end
    self:HideTopAndDown()
    self.BG.transform:DOLocalMoveY(-1334, 0.5):OnComplete(function() ImageWallIsShow = true end)   :Play()
end

function UIPersonalCenterForm:HideTopAndDown()
    local uiForm = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.MainFormTop)
    if not logic.IsNull(uiForm) then
        uiForm:Hide()
    end
    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if (uiform) then
        uiform.uiform:Hide();
    end
end
--endregion

--region 【隐藏背景墙】
function UIPersonalCenterForm:ImageWallHide()
    if ImageWallIsShow then
        self:ShowTopAndDown()
        self.BG.transform:DOLocalMoveY(0, 0.5):OnComplete(function() ImageWallIsShow = false end)   :Play()
    end
end

function UIPersonalCenterForm:ShowTopAndDown()
    local uiForm = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.MainFormTop)
    if not logic.IsNull(uiForm) then
        uiForm:Appear()
    end
    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if (uiform) then
        uiform.uiform:Appear();
    end
end
--endregion

--region 【界面关闭】
function UIPersonalCenterForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIPersonalCenterForm