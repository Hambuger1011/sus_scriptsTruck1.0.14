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
    self.mWriterList=nil;
    self.WriterList=nil;
    self.mHistoryList=nil;
    self.HistoryList=nil;
    self.m_page = 0;
end
--endregion


--region【OnOpen】

function UICommunityForm:OnOpen()
    UIView.OnOpen(self)
    GameController.CommunityControl:SetData(self);

    --在MainTop里标记  是在排行榜界面里打开  主界面顶框的
    CS.XLuaHelper.SetMainTopClose("UICommunityForm");
end

--endregion


--region 【OnClose】

function UICommunityForm:OnClose()
    UIView.OnClose(self)
    GameController.CommunityControl:SetData(nil);
end

--endregion



--region 【点击更多动态按钮】

function UICommunityForm:ViewMoreBtnClick()

    local len=table.length(Cache.ComuniadaCache.DynamicList);
    local _AllCount=Cache.ComuniadaCache.DynamicList_Count;

    if(allCount==6 and len > 1)then   --【说明是折叠着】【只显示1条】
        allCount=len+5;
    else
        if(len%5==0 and _AllCount%5~=0)then  --【如果当前页 已经满数】【并且总数不是当前数】 【请求获取下一页】
            if(Cache.ComuniadaCache.WriterInfo.uid)then
                GameController.CommunityControl:GetActionLogPageRequest(Cache.ComuniadaCache.WriterInfo.uid,self.m_page+1);
                return;
            end
        else
            allCount=6; --【折叠成 单个动态】
        end
    end
    --【设置列表总数量】
    self.LoopListView2:SetListItemCount(allCount);
end

--endregion



--region 【刷新数据】

function UICommunityForm:UpdateWriterInfo(page)

    if(Cache.ComuniadaCache.DynamicList==nil)then return; end

    if (page > 0)then
        self.m_page = page;
    end

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
            elseif(len > 1)then  --【当只有一条动态的情况】
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
        local itemData = Cache.ComuniadaCache:GetDynamicList(_index)

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


--region 【界面关闭】
function UICommunityForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UICommunityForm