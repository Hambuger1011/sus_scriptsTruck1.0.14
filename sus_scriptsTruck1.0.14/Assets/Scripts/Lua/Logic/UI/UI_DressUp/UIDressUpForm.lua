local UIView = core.UIView

local UIDressUpForm = core.Class("UIDressUpForm", UIView)

local DressUpItem = require('Logic/UI/UI_DressUp/Item/DressUpItem');

local uiid = logic.uiid
UIDressUpForm.config = {
    ID = uiid.UIDressUpForm,
    AssetName = 'UI/Resident/UI/UIDressUpForm'
}

--region【Awake】

local this=nil;
function UIDressUpForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    --标签按钮 Toggle
    self.TabScrollRect = CS.DisplayUtil.GetChild(this.gameObject, "TabScrollRect");
    self.HeadTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "HeadTab"):GetComponent("UIToggle");
    self.HeadFrameTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "HeadFrameTab"):GetComponent("UIToggle");
    self.ScreenFrameTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "ScreenFrameTab"):GetComponent("UIToggle");
    self.CommentFrameTab = CS.DisplayUtil.GetChild(self.TabScrollRect, "CommentFrameTab"):GetComponent("UIToggle");

    --标签按钮 文字文本读配置表
    self.B_HeadText = CS.DisplayUtil.GetChild(self.HeadTab.gameObject, "B_HeadText"):GetComponent("Text");
    self.C_HeadText = CS.DisplayUtil.GetChild(self.HeadTab.gameObject, "C_HeadText"):GetComponent("Text");
    self.B_HeadFrameText = CS.DisplayUtil.GetChild(self.HeadFrameTab.gameObject, "B_HeadFrameText"):GetComponent("Text");
    self.C_HeadFrameText = CS.DisplayUtil.GetChild(self.HeadFrameTab.gameObject, "C_HeadFrameText"):GetComponent("Text");
    self.B_ScreenFrameText = CS.DisplayUtil.GetChild(self.ScreenFrameTab.gameObject, "B_ScreenFrameText"):GetComponent("Text");
    self.C_ScreenFrameText = CS.DisplayUtil.GetChild(self.ScreenFrameTab.gameObject, "C_ScreenFrameText"):GetComponent("Text");
    self.B_CommentFrameText = CS.DisplayUtil.GetChild(self.CommentFrameTab.gameObject, "B_CommentFrameText"):GetComponent("Text");
    self.C_CommentFrameText = CS.DisplayUtil.GetChild(self.CommentFrameTab.gameObject, "C_CommentFrameText"):GetComponent("Text");

    --self.B_HeadText.text=CS.CTextManager.Instance:GetText(396);
    --self.C_HeadText =CS.CTextManager.Instance:GetText(396);
    --self.B_HeadFrameText.text=CS.CTextManager.Instance:GetText(397);
    --self.C_HeadFrameText =CS.CTextManager.Instance:GetText(397);
    --self.B_ScreenFrameText.text=CS.CTextManager.Instance:GetText(398);
    --self.C_ScreenFrameText =CS.CTextManager.Instance:GetText(398);
    --self.B_CommentFrameText.text=CS.CTextManager.Instance:GetText(398);
    --self.C_CommentFrameText =CS.CTextManager.Instance:GetText(398)

    --装扮详情
    self.mDressUpInfoPanel =CS.DisplayUtil.GetChild(this.gameObject, "DressUpInfoPanel");
    self.DressUpInfoPanel = require('Logic/UI/UI_DressUp/Panel/DressUpInfoPanel').New(self.mDressUpInfoPanel);

    --装扮列表
    self.DressUpScrollView =CS.DisplayUtil.GetChild(this.gameObject, "DressUpScrollView"):GetComponent("ScrollRect");
    self.HeadItem =CS.DisplayUtil.GetChild(self.DressUpScrollView.gameObject, "HeadItem");
    self.HeadFrameItem =CS.DisplayUtil.GetChild(self.DressUpScrollView.gameObject, "HeadFrameItem");
    self.ScreenFrameItem =CS.DisplayUtil.GetChild(self.DressUpScrollView.gameObject, "ScreenFrameItem");
    self.CommentFrameItem =CS.DisplayUtil.GetChild(self.DressUpScrollView.gameObject, "CommentFrameItem");
    self.SelectImg =CS.DisplayUtil.GetChild(self.DressUpScrollView.gameObject, "SelectImg");
    self.SelectImgRect=self.SelectImg:GetComponent(typeof(logic.cs.RectTransform));

    self.DressUpMask = CS.DisplayUtil.GetChild(this.gameObject, "DressUpMask");
    logic.cs.UIEventListener.AddOnClickListener(self.DressUpMask,function(data) self:OnExitClick(data) end);


    --标签按钮切换
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.HeadTab.gameObject,function(data) self:HeadTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.HeadFrameTab.gameObject,function(data) self:HeadFrameTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.ScreenFrameTab.gameObject,function(data) self:ScreenFrameTabClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.CommentFrameTab.gameObject,function(data) self:CommentFrameTabClick(data) end);

    --item 缓存列表
    self.ItemList=nil;
    self.oldID=nil;
end
--endregion


--region【OnOpen】

function UIDressUpForm:OnOpen()
    UIView.OnOpen(self)

    --赋值本界面
    GameController.DressUpControl:SetData(self);

    --在MainTop里标记  是在排行榜界面里打开  主界面顶框的
    CS.XLuaHelper.SetMainTopClose("UIDressUpForm");

    self:ChangeToggle("HeadTab");
    --刷新列表 请求
    GameController.DressUpControl:GetUserFrameListRequest(DressUp.Avatar)
end

--endregion


--region 【OnClose】

function UIDressUpForm:OnClose()
    UIView.OnClose(self)

    --赋值本界面 为nil  代码本界面已经关闭 隐藏 或者销毁
    GameController.DressUpControl:SetData(nil);





    -- 移除按钮监听
    logic.cs.UIEventListener.RemoveOnClickListener(self.HeadTab.gameObject,function(data) self:HeadTabClick(data) end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.HeadFrameTab.gameObject,function(data) self:HeadFrameTabClick(data) end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.ScreenFrameTab.gameObject,function(data) self:ScreenFrameTabClick(data) end);
    logic.cs.UIEventListener.RemoveOnClickListener(self.CommentFrameTab.gameObject,function(data) self:CommentFrameTabClick(data) end);

    logic.cs.UIEventListener.RemoveOnClickListener(self.DressUpMask,function(data) self:OnExitClick(data) end);


    --标签按钮 文字文本读配置表
    self.B_HeadText = nil;
    self.C_HeadText = nil;
    self.B_HeadFrameText = nil;
    self.C_HeadFrameText = nil;
    self.B_ScreenFrameText = nil;
    self.C_ScreenFrameText = nil;
    self.B_CommentFrameText = nil;
    self.C_CommentFrameText = nil;
    --标签按钮 Toggle
    self.TabScrollRect = nil;
    self.HeadTab = nil;
    self.HeadFrameTab = nil;
    self.ScreenFrameTab = nil;
    self.CommentFrameTab = nil;

    self.DressUpInfoPanel:Delete();
    self.mDressUpInfo=nil;
    --销毁列表
    self:ClearList();

    self.DressUpScrollView = nil;
    self.HeadItem = nil;
    self.HeadFrameItem = nil;
    self.ScreenFrameItem = nil;
    self.CommentFrameItem = nil;
    self.DressUpMask = nil;

    self.SelectImg=nil;
    self.SelectImgRect=nil;
    self.oldID=nil;
end

--endregion


--region 【切换 刷新列表】
function UIDressUpForm:UpdateList(infolist,_type)
    self:ClearList();
    if(self.ItemList==nil)then
        self.ItemList={};
    end

    if(GameHelper.islistHave(infolist)==false)then return; end
    for i = 1, #infolist do
        local item;
        local go;
        if(_type==DressUp.Avatar)then
            go = logic.cs.GameObject.Instantiate(self.HeadItem, self.DressUpScrollView.content.transform);
        elseif(_type==DressUp.AvatarFrame)then
            go = logic.cs.GameObject.Instantiate(self.HeadFrameItem, self.DressUpScrollView.content.transform);
        elseif(_type==DressUp.BarrageFrame)then
            go = logic.cs.GameObject.Instantiate(self.ScreenFrameItem, self.DressUpScrollView.content.transform);
        elseif(_type==DressUp.CommentFrame)then
            go = logic.cs.GameObject.Instantiate(self.CommentFrameItem, self.DressUpScrollView.content.transform);
        end

        go.transform.localPosition = core.Vector3.zero;
        go.transform.localScale = core.Vector3.one;
        go:SetActive(true);
        item =DressUpItem.New(go);
        table.insert(self.ItemList,item);

        item._index = i;
        item:SetInfo(infolist[i],_type,i);
    end

end

function UIDressUpForm:ClearList()
    if (self.ItemList)then
        local len=#self.ItemList;
        if(len>0)then
            for i = 1, len do
                self.ItemList[i]:Delete();
            end
        end
    end
    self.ItemList=nil;
end

--endregion


--region 【Toogle 切换显示状态】

function UIDressUpForm:ChangeToggle(name)
    --关闭旧的
    if(name~="" and self.oldToggleName~=name)then
        self.oldToggleName=name;
    end

    self.SelectImg.transform:SetParent(self.DressUpScrollView.transform)
    self.SelectImgRect.anchoredPosition ={x=-10000,y=0};
    self.SelectImg.transform.localScale= core.Vector3.one;
end


--endregion


--region 【点击Head】
function UIDressUpForm:HeadTabClick(data)
    if(self.oldToggleName=="HeadTab")then return; end
    self:ChangeToggle("HeadTab");

    --刷新列表 请求
    GameController.DressUpControl:Request(DressUp.Avatar)
end
--endregion


--region 【点击HeadFrame】
function UIDressUpForm:HeadFrameTabClick(data)
    if(self.oldToggleName=="HeadFrameTab")then return; end
    self:ChangeToggle("HeadFrameTab");

    --刷新列表 请求
    GameController.DressUpControl:Request(DressUp.AvatarFrame)
end
--endregion


--region 【点击ScreenFrame】
function UIDressUpForm:ScreenFrameTabClick(data)
    if(self.oldToggleName=="ScreenFrameTab")then return; end
    self:ChangeToggle("ScreenFrameTab");

    --刷新列表 请求
    GameController.DressUpControl:Request(DressUp.BarrageFrame)
end
--endregion


--region 【点击CommentFrame】
function UIDressUpForm:CommentFrameTabClick(data)
    if(self.oldToggleName=="CommentFrameTab")then return; end
    self:ChangeToggle("CommentFrameTab");

    --刷新列表 请求
    GameController.DressUpControl:Request(DressUp.CommentFrame)
end
--endregion


--region【点击 头像或框 展示详细信息】


function UIDressUpForm:SetHeadInfoData(_info,obj,_index)
    if(_info.id==self.oldID)then return; end
    self.oldID=_info.id;
    self.DressUpInfoPanel:SetInfo(_info,_index);

    if(obj and CS.XLuaHelper.is_Null(obj)==false)then
        self.SelectImg.transform:SetParent(obj.transform)
        self.SelectImgRect.anchoredPosition ={x=56,y=-48};
      --  self.SelectImg.transform.localScale= core.Vector3.one;
    end
end

--设置头像Item 显示 Using状态  已使用状态
function UIDressUpForm:SetUsing(_index)
    if(self.ItemList)then
        local len=table.length(self.ItemList);
        if(len>0 and _index and _index>0)then
            for i = 1,len do
                self.ItemList[_index]:Haved();
            end
        end
    end

end

--endregion


--region【切换详情信息状态 已经使用】
function UIDressUpForm:Haved(_index)
    self.DressUpInfoPanel:Haved();
    self:SetUsing(_index)
end
--endregion


--region 【界面关闭】
function UIDressUpForm:OnExitClick()
    UIView.__Close(self)
    --在MainTop里标记  是在排行榜界面里打开  主界面顶框的
    CS.XLuaHelper.MainTopCloseShow();
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIDressUpForm