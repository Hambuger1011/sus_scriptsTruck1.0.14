local UIView = core.UIView

local UISearchForm = core.Class("UISearchForm", UIView)

local uiid = logic.uiid
UISearchForm.config = {
    ID = uiid.UISearchForm,
    AssetName = 'UI/Resident/UI/UISearchForm'
}

--region【Awake】

local this=nil;
function UISearchForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.TabScrollRect = CS.DisplayUtil.GetChild(this.gameObject, "TabScrollRect");
    self.RomanceTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "RomanceTab"):GetComponent("UIToggle");
    self.Content = CS.DisplayUtil.GetChild(self.TabScrollRect, "Content"):GetComponent("GridLayoutGroup");
    self.LGBTTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "LGBTTab"):GetComponent("UIToggle");
    self.ActionTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "ActionTab"):GetComponent("UIToggle");
    self.YouthTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "YouthTab"):GetComponent("UIToggle");
    self.AdventureTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "AdventureTab"):GetComponent("UIToggle");
    self.DramaTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "DramaTab"):GetComponent("UIToggle");
    self.ComedyTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "ComedyTab"):GetComponent("UIToggle");
    self.HorrorTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "HorrorTab"):GetComponent("UIToggle");
    self.EighteenTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "EighteenTab"):GetComponent("UIToggle");
    self.FantasyTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "FantasyTab"):GetComponent("UIToggle");
    self.SuspenseTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "SuspenseTab"):GetComponent("UIToggle");
    self.OthersTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "OthersTab"):GetComponent("UIToggle");

    -- BookTypeList列表
    self.mBookTypeList =CS.DisplayUtil.GetChild(this.gameObject, "BookTypeList");
    self.BookTypeList = require('Logic/UI/UI_Search/Panel/BookTypeList').New(self.mBookTypeList);

end
--endregion


--region【OnOpen】

function UISearchForm:OnOpen()
    UIView.OnOpen(self)

    GameController.SearchControl:SetData(self);
    self:RomanceTabClick(nil);

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.RomanceTab.gameObject,function(data) self:RomanceTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.LGBTTab.gameObject,function(data) self:LGBTTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.ActionTab.gameObject,function(data) self:ActionTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.YouthTab.gameObject,function(data) self:YouthTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.AdventureTab.gameObject,function(data) self:AdventureTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.DramaTab.gameObject,function(data) self:DramaTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.ComedyTab.gameObject,function(data) self:ComedyTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.HorrorTab.gameObject,function(data) self:HorrorTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.EighteenTab.gameObject,function(data) self:EighteenTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.FantasyTab.gameObject,function(data) self:FantasyTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.SuspenseTab.gameObject,function(data) self:SuspenseTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.OthersTab.gameObject,function(data) self:OthersTabClick(data) end);


    --【AF事件记录* 第一次选择书本类型】
    CS.AppsFlyerManager.Instance:FIRST_SELECT_OFFICIAL_BOOK_TYPE();
end

--endregion


--region 【OnClose】

function UISearchForm:OnClose()
    UIView.OnClose(self)

    if(self.RomanceTab)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.RomanceTab.gameObject,function(data) self:RomanceTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.LGBTTab.gameObject,function(data) self:LGBTTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.ActionTab.gameObject,function(data) self:ActionTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.YouthTab.gameObject,function(data) self:YouthTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.AdventureTab.gameObject,function(data) self:AdventureTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.DramaTab.gameObject,function(data) self:DramaTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.ComedyTab.gameObject,function(data) self:ComedyTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.HorrorTab.gameObject,function(data) self:HorrorTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.EighteenTab.gameObject,function(data) self:EighteenTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.FantasyTab.gameObject,function(data) self:FantasyTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.SuspenseTab.gameObject,function(data) self:SuspenseTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.OthersTab.gameObject,function(data) self:OthersTabClick(data) end);
    end

    GameController.SearchControl:SetData(nil);
end

--endregion


--region 【动态打开某一个标签列表】
function UISearchForm:OpenBookType(_index)


    self.OthersTab =CS.DisplayUtil.GetChild(self.TabScrollRect, "OthersTab"):GetComponent("UIToggle");

    if(_index==BookType.Romance)then
        self.RomanceTab.isOn=true;
        self:RomanceTabClick(nil)
    elseif(_index==BookType.LGBT)then
        self.LGBTTab.isOn=true;
        self:LGBTTabClick(nil)
    elseif(_index==BookType.Action)then
        self.ActionTab.isOn=true;
        self:ActionTabClick(nil)
    elseif(_index==BookType.Youth)then
        self.YouthTab.isOn=true;
        self:YouthTabClick(nil)
    elseif(_index==BookType.Adventure)then
        self.AdventureTab.isOn=true;
        self:AdventureTabClick(nil)
    elseif(_index==BookType.Drama)then
        self.DramaTab.isOn=true;
        self:DramaTabClick(nil)
    elseif(_index==BookType.Comedy)then
        self.ComedyTab.isOn=true;
        self:ComedyTabClick(nil)
    elseif(_index==BookType.Horror)then
        self.HorrorTab.isOn=true;
        self:HorrorTabClick(nil)
    elseif(_index==BookType.Eighteen)then
        self.EighteenTab.isOn=true;
        self:EighteenTabClick(nil)
    elseif(_index==BookType.Fantasy)then
        self.FantasyTab.isOn=true;
        self:FantasyTabClick(nil)
    elseif(_index==BookType.Suspense)then
        self.SuspenseTab.isOn=true;
        self:SuspenseTabClick(nil)
    elseif(_index==BookType.Others)then
        self.OthersTab.isOn=true;
        self:OthersTabClick(nil)
    end
end
--endregion


--region 【Toogle 切换显示状态】

function UISearchForm:ChangeToggle(name)
    --关闭旧的
    if(name~="" and self.oldToggleName~=name)then
        self.oldToggleName=name;
    end
end


--endregion


--region 【刷新列表】

function UISearchForm:UpdateBookList(_index,InfoList)
    if(self.BookTypeList.gameObject)then
        self.BookTypeList:UpdateList(InfoList);
        --埋点*选择故事题材
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectCategory,"search_2",tostring(_index));
    end
end

--endregion


--region 【点击Romance】
function UISearchForm:RomanceTabClick(data)
    if(self.oldToggleName=="RomanceTab")then return; end
    self:ChangeToggle("RomanceTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Romance);
end
--endregion


--region 【点击LGBT】
function UISearchForm:LGBTTabClick(data)
    if(self.oldToggleName=="LGBTTab")then return; end
    self:ChangeToggle("LGBTTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.LGBT);
end
--endregion


--region 【点击Action】
function UISearchForm:ActionTabClick(data)
    if(self.oldToggleName=="ActionTab")then return; end
    self:ChangeToggle("ActionTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Action);
end
--endregion


--region 【点击Youth】
function UISearchForm:YouthTabClick(data)
    if(self.oldToggleName=="YouthTab")then return; end
    self:ChangeToggle("YouthTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Youth);
end
--endregion


--region 【点击Adventure】
function UISearchForm:AdventureTabClick(data)
    if(self.oldToggleName=="AdventureTab")then return; end
    self:ChangeToggle("AdventureTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Adventure);
end
--endregion


--region 【点击Drama】
function UISearchForm:DramaTabClick(data)
    if(self.oldToggleName=="DramaTab")then return; end
    self:ChangeToggle("DramaTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Drama);
end
--endregion


--region 【点击Comedy】
function UISearchForm:ComedyTabClick(data)
    if(self.oldToggleName=="ComedyTab")then return; end
    self:ChangeToggle("ComedyTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Comedy);
end
--endregion


--region 【点击Horror】
function UISearchForm:HorrorTabClick(data)
    if(self.oldToggleName=="HorrorTab")then return; end
    self:ChangeToggle("HorrorTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Horror);
end
--endregion


--region 【点击18+】
function UISearchForm:EighteenTabClick(data)
    if(self.oldToggleName=="EighteenTab")then return; end
    self:ChangeToggle("EighteenTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Eighteen);
end
--endregion


--region 【点击Fantasy】
function UISearchForm:FantasyTabClick(data)
    if(self.oldToggleName=="FantasyTab")then return; end
    self:ChangeToggle("FantasyTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Fantasy);
end
--endregion


--region 【点击Suspense】
function UISearchForm:SuspenseTabClick(data)
    if(self.oldToggleName=="SuspenseTab")then return; end
    self:ChangeToggle("SuspenseTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Suspense);
end
--endregion


--region 【点击Others】
function UISearchForm:OthersTabClick(data)
    if(self.oldToggleName=="OthersTab")then return; end
    self:ChangeToggle("OthersTab");

    --请求服务器书本列表
    GameController.SearchControl:GetSearchBookListRequest(BookType.Others);
end
--endregion


--region 【界面关闭】
function UISearchForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UISearchForm