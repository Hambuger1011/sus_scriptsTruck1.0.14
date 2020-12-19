local UIView = core.UIView

local UICommunityForm = core.Class("UICommunityForm", UIView)
local uiid = logic.uiid
UICommunityForm.config = {
    ID = uiid.UICommunityForm,
    AssetName = 'UI/StoryEditorRes/UI/UICommunityForm'
}

--region【Awake】

local this=nil;
--【Item集合】
local ItemList={};

local allCount=0;
-- 【我写作的故事】 作品
local WriterList=nil;
-- 【历史看过的故事】 作品
local HistoryList=nil;

local AuthorInfoPanel=nil;
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


--region 【刷新数据】

function UICommunityForm:UpdateWriterInfo(page)

    if(Cache.ComuniadaCache.DynamicList==nil)then return; end

    allCount=table.length(Cache.BusquedaCache.BusquedaList);

    if(allCount and allCount>=0)then
        if(page==1)then
            allCount=4;
            --【设置列表总数量】
            self.LoopListView2:SetListItemCount(allCount);
        else
            if(allCount>5)then
                allCount=allCount+5;
            else
                allCount=allCount+4;
            end
            --【设置列表总数量】
            self.LoopListView2:SetListItemCount(allCount);
        end
    end
end

function UICommunityForm.OnGetItemByRowColumn(listView,index)
    if (index < 0)then
        return nil;
    end

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
    elseif(index==_allCount-2)then
        item = listView:NewListViewItem("ViewMoreBtn");
        if(item.gameObject.name~="ViewMoreBtn")then
            item.gameObject.name="ViewMoreBtn";
        end
    elseif(index==_allCount-1)then
        item = listView:NewListViewItem("WriterList");   --【作者创作故事列表】
        if(item.gameObject.name~="WriterList")then
            item.gameObject.name="WriterList";
            -- 【我写作的故事】 作品
            WriterList = require('Logic/UI/UI_Comuniada/List/StoryList').New(item.gameObject);
        end
    elseif(index==_allCount)then
        item = listView:NewListViewItem("HistoryList");     --【正在阅读的书本列表】
        if(item.gameObject.name~="HistoryList")then
            item.gameObject.name="HistoryList";
            -- 【历史看过的故事】 作品
           HistoryList = require('Logic/UI/UI_Comuniada/List/StoryList').New(item.gameObject);
        end
    end


    --local itemData = ChatCache:GetInstance():GetChatMsgByIndex(_index);
    --if (itemData == nil)then
    --    return nil;
    --end

    --local itemData =Cache.BusquedaCache:GetInfoByIndex(index);
    --if(itemData == nil)then return nil; end
    --

    ----【GameObect唯一编号】
    --local onlyID=item.gameObject:GetInstanceID();
    --
    ----【书本 脚本】
    --local storyItem = table.trygetvalue(ItemList,onlyID);
    --if(storyItem==nil)then
    --    storyItem = StoryItem2.New(item.gameObject);
    --    ItemList[onlyID]=storyItem;
    --end


    if (item.IsInitHandlerCalled == false)then
        item.IsInitHandlerCalled = true;
    end

    ----【赋值】
    --if(storyItem)then
    --    storyItem:SetItemData(itemData,index);
    --end
    return item;
end


--endregion


function UICommunityForm:UpdateWriterList()
    if(WriterList)then
        WriterList:UpdateList(Cache.ComuniadaCache.WriterList,EStoryList.WriterList);
    end
    if(HistoryList)then
        HistoryList:UpdateList(Cache.ComuniadaCache.WriterhistoryList,EStoryList.WriterList);
    end
end




--region 【界面关闭】
function UICommunityForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UICommunityForm