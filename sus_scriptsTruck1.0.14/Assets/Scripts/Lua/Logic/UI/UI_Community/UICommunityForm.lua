local UIView = core.UIView

local UICommunityForm = core.Class("UICommunityForm", UIView)
local uiid = logic.uiid

local DynamicItem = require('Logic/UI/UI_Community/Item/DynamicItem');

UICommunityForm.config = {
    ID = uiid.UICommunityForm,
    AssetName = 'UI/StoryEditorRes/UI/UICommunityForm'
}


--region【Awake】

local this=nil;
--【Item集合】
local scriptItemList={};

local allCount=0;
-- 【我写作的故事】 作品
local WriterList=nil;
-- 【历史看过的故事】 作品
local HistoryList=nil;
-- 【更多按钮】
local ViewMoreBtn=nil;

local AuthorInfoPanel=nil;

local ImageWallIsShow = nil;

--是否展示更多动态按钮
local isBestDynam=false;
function UICommunityForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.LoopListView2 = CS.DisplayUtil.GetChild(this.gameObject, "MainScrollView"):GetComponent("LoopListView2");

    --list插件
    self.LoopListView2:InitListView(allCount,UICommunityForm.OnGetItemByRowColumn);

    self.MainContent = CS.DisplayUtil.GetChild(self.LoopListView2.gameObject, "MainContent")

    --logic.cs.UIEventListener.AddOnClickListener(self.CloseBtn,function(data) self:OnExitClick() end);
    self.m_page = 0;

    self.ImageWallParent = CS.DisplayUtil.GetChild(this.gameObject, "ImageWallParent");
    self.BG = CS.DisplayUtil.GetChild(this.gameObject, "BG");
end
--endregion


--region【OnOpen】

function UICommunityForm:OnOpen()
    UIView.OnOpen(self)
    GameController.CommunityControl:SetData(self);

    --在MainTop里标记  是在排行榜界面里打开  主界面顶框的
    CS.XLuaHelper.SetMainTopClose("UICommunityForm");

    self.ImageWallObj = CS.ResourceManager.Instance:LoadAssetBundleUI(logic.cs.UIFormName.ImageWall);
    self.ImageWallObj.transform:SetParent(self.ImageWallParent.transform, false);
    self.ImageWallBtn = self.ImageWallObj:GetComponent(typeof(logic.cs.Button));
    self.ImageWallBtn.onClick:AddListener(function()
        self:ImageWallShow()
    end);
    self.ImageWall= require('Logic/UI/UI_ImageWall/ImageWall').New(self.ImageWallObj.gameObject);
    self.ImageWall:SetHideOnClick(function()
        self:ImageWallHide()
    end);
end

--endregion

--region 【展示背景墙】
function UICommunityForm:ImageWallShow()
    if ImageWallIsShow then
        return
    end
    self.BG.transform:DOLocalMoveY(-1334, 0.5):OnComplete(function() ImageWallIsShow = true end)   :Play()
end
--endregion

--region 【隐藏背景墙】
function UICommunityForm:ImageWallHide()
    if ImageWallIsShow then
        self.BG.transform:DOLocalMoveY(0, 0.5):OnComplete(function() ImageWallIsShow = false end)   :Play()
    end
end
--endregion

--region 【OnClose】

function UICommunityForm:OnClose()
    UIView.OnClose(self)
    self.ImageWallBtn.onClick:RemoveAllListeners()
    GameController.CommunityControl:SetData(nil);

    Cache.ComuniadaCache.DynamicList={};
    Cache.ComuniadaCache.DynamicList_Count=0;

    --【清除列表所有对象 和 脚本】
    if(scriptItemList)then
        for _key, _value in pairs(scriptItemList) do
            if(_value)then
                _value:Delete();--【销毁】
            end
        end
        scriptItemList={};
    end


    if(WriterList)then
        WriterList:Delete();--【销毁】
        WriterList=nil;
    end

    if(HistoryList)then
        HistoryList:Delete();--【销毁】
        HistoryList=nil;
    end

    if(ViewMoreBtn)then
        ViewMoreBtn:Delete();--【销毁】
        ViewMoreBtn=nil;
    end

    if(AuthorInfoPanel)then
        AuthorInfoPanel:Delete();--【销毁】
        AuthorInfoPanel=nil;
    end

    ImageWallIsShow = nil

end

--endregion



--region 【点击更多动态按钮】

function UICommunityForm:ViewMoreBtnClick(func)
    local len=table.length(Cache.ComuniadaCache.DynamicList);
    local _AllCount=Cache.ComuniadaCache.DynamicList_Count;
    if(allCount==6 and len > 1)then   --【说明是折叠着】【只显示1条】
        allCount=len+5;
        --【刷新按钮状态】
        self:UpdateButton(func);
    else
        if(len > 1)then
            if(len%5==0 and len<_AllCount)then   --【如果当前页 已经满数】【并且总数不是当前数】 【请求获取下一页】
                if(Cache.ComuniadaCache.WriterInfo.uid)then
                    GameController.CommunityControl:GetActionLogPageRequest(Cache.ComuniadaCache.WriterInfo.uid,self.m_page+1,func);
                    return;
                end
            elseif(len==_AllCount)then
                allCount=6; --【折叠成 单个动态】`
                --【刷新按钮状态】
                self:UpdateButton(func);
            end
        end
    end

    --【设置列表总数量】
    self.LoopListView2:SetListItemCount(allCount);
end

--【刷新按钮状态】
function UICommunityForm:UpdateButton(func)
    if(func==nil)then return; end

    local len=table.length(Cache.ComuniadaCache.DynamicList);
    local _AllCount=Cache.ComuniadaCache.DynamicList_Count;
    if(allCount==6 and len > 1)then   --【说明是折叠着】【只显示1条】
        func(2);
    else
        if(len > 1)then
            if(len%5==0 and len<_AllCount)then   --【如果当前页 已经满数】【并且总数不是当前数】 【请求获取下一页】
                func(2);
            elseif(len==_AllCount)then
                func(1);
            end
        end
    end
end



--endregion



--region 【刷新数据】

function UICommunityForm:UpdateWriterInfo(page)

    if(Cache.ComuniadaCache.DynamicList==nil)then return; end

    if (page > 0)then
        self.m_page = page;
    end

    local _AllCount=Cache.ComuniadaCache.DynamicList_Count;
    local len=table.length(Cache.ComuniadaCache.DynamicList);
    if(len and len>=0)then
        if(page==1)then

            if(len==0)then   --【当没有动态的情况】
                allCount =4;

                --不展示更多按钮
                isBestDynam=false;
            elseif(len == 1)then   --【当只有一条动态的情况】
                allCount = 5;

                --不展示更多按钮
                isBestDynam=false;
            elseif(len > 1)then  --【当有多条动态的情况】
                allCount = 6;

                --展示更多按钮
                isBestDynam=true;
            end
            --【设置列表总数量】
            self.LoopListView2:SetListItemCount(allCount);
        else
            allCount=len+5;
            --【设置列表总数量】
            self.LoopListView2:SetListItemCount(allCount);
        end
    end
end

function UICommunityForm.OnGetItemByRowColumn(listView,index)
    if (index < 0)then
        return nil;
    end

    local isDynamic=false; --是否是动态

    local _index=index+1;
    local _allCount=allCount-1;
    local item =nil;

    if(index==0)then
        item = listView:NewListViewItem("WhiteNone");  --【空白背景】
        if(item.gameObject.name~="WhiteNone")then
            item.gameObject.name="WhiteNone";
        end
    elseif(index==1)then
        item = listView:NewListViewItem("AuthorInfo");  --【作者详情】
        if(item.gameObject.name~="AuthorInfo")then
            item.gameObject.name="AuthorInfo";
            AuthorInfoPanel= require('Logic/UI/UI_Community/Panel/AuthorInfoPanel').New(item.gameObject);
            AuthorInfoPanel:UpdateInfo();
        end
    elseif(index>1 and index<_allCount-2)then
        item = listView:NewListViewItem("DynamicItem");   --【单个动态】
        isDynamic=true;
    elseif(index==_allCount-2)then

        if(isBestDynam)then
            item = listView:NewListViewItem("ViewMoreBtn");  --【更多按钮】
            if(item.gameObject.name~="ViewMoreBtn")then
                item.gameObject.name="ViewMoreBtn";
                ViewMoreBtn = require('Logic/UI/UI_Community/Panel/ViewMoreBtn').New(item.gameObject);
            end
        else
            item = listView:NewListViewItem("DynamicItem");   --【单个动态】
            isDynamic=true;
        end

    elseif(index==_allCount-1)then
        item = listView:NewListViewItem("WriterList");   --【作者创作故事列表】
        if(item.gameObject.name~="WriterList")then
            item.gameObject.name="WriterList";
            -- 【我写作的故事】 作品
            WriterList = require('Logic/UI/UI_Comuniada/List/StoryList').New(item.gameObject);
            WriterList:UpdateList(Cache.ComuniadaCache.WriterList,EStoryList.WriterList);
        end
    elseif(index==_allCount)then
        item = listView:NewListViewItem("HistoryList");     --【正在阅读的书本列表】
        if(item.gameObject.name~="HistoryList")then
            item.gameObject.name="HistoryList";
            -- 【历史看过的故事】 作品
           HistoryList = require('Logic/UI/UI_Comuniada/List/StoryList').New(item.gameObject);
           HistoryList:UpdateList(Cache.ComuniadaCache.WriterhistoryList,EStoryList.WriterList);
        end
    end

    --如果有动态
    if(isDynamic)then
        local m_Index=_index-2;

        local itemData = Cache.ComuniadaCache:GetDynamicList(m_Index);

        --【GameObect唯一编号】
        local onlyID=item.gameObject:GetInstanceID();

        --【脚本】
        local scriptItem = table.trygetvalue(scriptItemList,onlyID);
        if(scriptItem==nil and itemData)then
            scriptItem = DynamicItem.New(item.gameObject);
            scriptItemList[onlyID]=scriptItem;
            scriptItem:SetItemData(itemData,index);
        end
    end

    if (item.IsInitHandlerCalled == false)then
        item.IsInitHandlerCalled = true;
    end

    return item;
end

--endregion


--region【刷新作者列表】

function UICommunityForm:UpdateWriterList()
    if(WriterList)then
        WriterList:UpdateList(Cache.ComuniadaCache.WriterList,EStoryList.WriterList);
    end
    if(HistoryList)then
        HistoryList:UpdateList(Cache.ComuniadaCache.WriterhistoryList,EStoryList.WriterList);
    end
end

--endregion


--region【刷新作者关注】

function UICommunityForm:UpdateWriterFollow()

    if(AuthorInfoPanel)then
        AuthorInfoPanel:UpdateWriterFollow();
    end

end

--endregion

--region【刷新作者点赞】

function UICommunityForm:UpdateWriterAgree()

    if(AuthorInfoPanel)then
        AuthorInfoPanel:UpdateWriterAgree();
    end

end

--endregion


--region【刷新作者点赞】

function UICommunityForm:UpdateDynamicTitle()

    if(AuthorInfoPanel)then
        AuthorInfoPanel:UpdateDynamicTitle();
    end

end

--endregion




--region 【界面关闭】
function UICommunityForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UICommunityForm