local UIView = core.UIView

local UIRankForm = core.Class("UIRankForm", UIView)
local uiid = logic.uiid
UIRankForm.config = {
    ID = uiid.UIRankForm,
    AssetName = 'UI/Resident/UI/UIRankForm'
}

--region【Awake】

local this=nil;
function UIRankForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.TabScrollRect = CS.DisplayUtil.GetChild(this.gameObject, "TabScrollRect");
    self.PlatformTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "PlatformTab"):GetComponent("UIToggle");
    self.NewbookTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "NewbookTab"):GetComponent("UIToggle");
    self.PopularityTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "PopularityTab"):GetComponent("UIToggle");

    self.B_PlatformText = CS.DisplayUtil.GetChild(self.PlatformTab.gameObject, "B_PlatformText"):GetComponent("Text");
    self.C_PlatformText = CS.DisplayUtil.GetChild(self.PlatformTab.gameObject, "C_PlatformText"):GetComponent("Text");
    self.B_NewbookText = CS.DisplayUtil.GetChild(self.NewbookTab.gameObject, "B_NewbookText"):GetComponent("Text");
    self.C_NewbookText = CS.DisplayUtil.GetChild(self.NewbookTab.gameObject, "C_NewbookText"):GetComponent("Text");
    self.B_PopularityText = CS.DisplayUtil.GetChild(self.PopularityTab.gameObject, "B_PopularityText"):GetComponent("Text");
    self.C_PopularityText = CS.DisplayUtil.GetChild(self.PopularityTab.gameObject, "C_PopularityText"):GetComponent("Text");


    -- RankList列表
    self.mRankList =CS.DisplayUtil.GetChild(this.gameObject, "RankList");
    self.RankList = require('Logic/UI/UI_Rank/Panel/RankList').New(self.mRankList);

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.PlatformTab.gameObject,function(data) self:PlatformTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.NewbookTab.gameObject,function(data) self:NewbookTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.PopularityTab.gameObject,function(data) self:PopularityTabClick(data) end);



    self.B_PlatformText.text=CS.CTextManager.Instance:GetText(396);
    self.C_PlatformText.text =CS.CTextManager.Instance:GetText(396);
    self.B_NewbookText.text=CS.CTextManager.Instance:GetText(397);
    self.C_NewbookText.text =CS.CTextManager.Instance:GetText(397);
    self.B_PopularityText.text=CS.CTextManager.Instance:GetText(398);
    self.C_PopularityText.text =CS.CTextManager.Instance:GetText(398);
end
--endregion


--region【OnOpen】

function UIRankForm:OnOpen()
    UIView.OnOpen(self)

    --在MainTop里标记  是在排行榜界面里打开  主界面顶框的
    CS.XLuaHelper.SetMainTopClose("UIRankForm");

    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if(uiform)then
        uiform.uiform:Hide();
    end
end

--endregion


--region 【OnClose】

function UIRankForm:OnClose()
    UIView.OnClose(self)
    if(self.PlatformTab)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.PlatformTab.gameObject,function(data) self:RomanceTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.NewbookTab.gameObject,function(data) self:LGBTTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.PopularityTab.gameObject,function(data) self:ActionTabClick(data) end);

    end

    self.TabScrollRect = nil;
    self.PlatformTab = nil;
    self.NewbookTab = nil;
    self.PopularityTab = nil;

    self.RankList:Delete();
    self.mRankList=nil;
    self.RankList=nil;


    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if(uiform==nil)then
        logic.UIMgr:Open(logic.uiid.UIMainDownForm);
    else
        uiform.uiform:Appear();
    end

end

--endregion


--region 【Toogle 切换显示状态】

function UIRankForm:ChangeToggle(name)
    --关闭旧的
    if(name~="" and self.oldToggleName~=name)then
        self.oldToggleName=name;
    end
end


--endregion


--region 【刷新列表】

function UIRankForm:UpdateBookList(_type)
    if(self.RankList.gameObject)then
        if(_type==RankType.Platform)then
            self.RankList:UpdateList(Cache.MainCache.platform_ranklist,RankType.Platform);
        elseif(_type==RankType.Newbook)then
            self.RankList:UpdateList(Cache.MainCache.newbook_ranklist,RankType.Newbook);
        elseif(_type==RankType.Popularity)then
            self.RankList:UpdateList(Cache.MainCache.popularity_ranklist,RankType.Popularity);
        end
    end
end

--endregion


--region 【点击Platform】
function UIRankForm:PlatformTabClick(data)
    if(self.oldToggleName=="PlatformTab")then return; end
    self:ChangeToggle("PlatformTab");
    self:UpdateBookList(RankType.Platform);
end
--endregion


--region 【点击Newbook】
function UIRankForm:NewbookTabClick(data)
    if(self.oldToggleName=="NewbookTab")then return; end
    self:ChangeToggle("NewbookTab");
    self:UpdateBookList(RankType.Newbook);
end
--endregion


--region 【点击Popularity】
function UIRankForm:PopularityTabClick(data)
    if(self.oldToggleName=="PopularityTab")then return; end
    self:ChangeToggle("PopularityTab");
    self:UpdateBookList(RankType.Popularity);
end
--endregion


--region 【界面关闭】
function UIRankForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIRankForm