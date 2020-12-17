local UIView = core.UIView

local UIComuniadaForm = core.Class("UIComuniadaForm", UIView)
local uiid = logic.uiid
UIComuniadaForm.config = {
    ID = uiid.UIComuniadaForm,
    AssetName = 'UI/Resident/UI/UIComuniadaForm'
}

--region【Awake】

local this=nil;
function UIComuniadaForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    --Toogle 标签按钮
    self.TabScrollRect = CS.DisplayUtil.GetChild(this.gameObject, "TabScrollRect");
    self.CommunityTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "CommunityTab"):GetComponent("UIToggle");
    self.FavoritesTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "FavoritesTab"):GetComponent("UIToggle");

    --社区面板
    self.mCommunityPanel =CS.DisplayUtil.GetChild(this.gameObject, "CommunityPanel");
    self.CommunityPanel = require('Logic/UI/UI_Comuniada/Panel/CommunityPanel').New(self.mCommunityPanel);

    --个人面板
    self.mFavoritesPanel =CS.DisplayUtil.GetChild(this.gameObject, "FavoritesPanel");
    self.FavoritesPanel = require('Logic/UI/UI_Comuniada/Panel/FavoritesPanel').New(self.mFavoritesPanel);


    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.CommunityTab.gameObject,function(data) self:CommunityTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.FavoritesTab.gameObject,function(data) self:FavoritesTabClick(data) end);
end
--endregion


--region【OnOpen】

function UIComuniadaForm:OnOpen()
    UIView.OnOpen(self)

    GameController.ComuniadaControl:SetData(self);

    --请求列表
    GameController.ComuniadaControl:GetwriterIndexRequest();
end

--endregion


--region 【OnClose】

function UIComuniadaForm:OnClose()
    UIView.OnClose(self)
    GameController.ComuniadaControl:SetData(nil);

    if(self.CommunityTab)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.CommunityTab.gameObject,function(data) self:CommunityTabClick(data) end);
        logic.cs.UIEventListener.RemoveOnClickListener(self.FavoritesTab.gameObject,function(data) self:FavoritesTabClick(data) end);
    end

end

--endregion


--region 【刷新列表】
function UIComuniadaForm:UpdateStoryList()
    if(self.CommunityPanel)then
        self.CommunityPanel:UpdateStoryList();
    end
end
--endregion


--region 【Toogle 切换显示状态】

function UIComuniadaForm:ChangeToggle(name)
    --关闭旧的
    self:HideOld(self.oldToggleName);
    if(name~="" and self.oldToggleName~=name)then
        self.oldToggleName=name;
    end
end

--【关闭旧界面】
function UIComuniadaForm:HideOld(name)
    if(name==nil)then return; end

    if(name=="CommunityTab")then
        self.mCommunityPanel:SetActive(false);
    elseif(name=="FavoritesTab")then
        self.mFavoritesPanel:SetActive(false);
    end
end
--endregion


--region 【点击Community】
function UIComuniadaForm:CommunityTabClick(data)
    if(self.oldToggleName=="CommunityTab")then return; end
    self:ChangeToggle("CommunityTab");

    --打开社区面板
    self.mCommunityPanel:SetActive(true);
end
--endregion


--region 【点击Favorites】
function UIComuniadaForm:FavoritesTabClick(data)
    if(self.oldToggleName=="FavoritesTab")then return; end
    self:ChangeToggle("FavoritesTab");

    --【请求】【获取我的写作列表】
    GameController.ComuniadaControl:GetMyWriterBookListRequest();

    --【请求】【获取我看过的书本列表】
    GameController.ComuniadaControl:GetReadingHistoryRequest();

    --打开个人面板
    self.mFavoritesPanel:SetActive(true);
end
--endregion


--region【刷新列表】【自己创作故事列表】
function UIComuniadaForm:UpdateMyWriterList()
    if(self.FavoritesPanel)then
        self.FavoritesPanel:UpdateMyWriterList();
    end
end
--endregion


--region【刷新列表】【历史看过的故事列表】
function UIComuniadaForm:UpdateHistoryList()
    if(self.FavoritesPanel)then
        self.FavoritesPanel:UpdateHistoryList();
    end
end
--endregion


--region【创作】【切换到详情页面】
function UIComuniadaForm:ChangeFavoritesPanel()
    --【切换Toogle】
    self:FavoritesTabClick(nil);
    self.CommunityTab.isOn=false;
    self.FavoritesTab.isOn=true;
end
--endregion



--region 【界面关闭】
function UIComuniadaForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIComuniadaForm