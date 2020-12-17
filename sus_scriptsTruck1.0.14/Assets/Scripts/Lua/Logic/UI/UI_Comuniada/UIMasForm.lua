local UIView = core.UIView

local UIMasForm = core.Class("UIMasForm", UIView)
local uiid = logic.uiid
UIMasForm.config = {
    ID = uiid.UIMasForm,
    AssetName = 'UI/Resident/UI/UIMasForm'
}

local StoryItem = require('Logic/UI/UI_Comuniada/Item/StoryItem');

--region【Awake】

local this=nil;
--【书本集合】
local storyItemList={};
--【类型】
local mtype=EStoryList.LatestRelease;
function UIMasForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.TitleTxt =CS.DisplayUtil.GetChild(this.gameObject, "TitleTxt"):GetComponent("Text");
    self.ScrollRect =CS.DisplayUtil.GetChild(this.gameObject, "ScrollRect"):GetComponent("ScrollRect");
    self.LoopGridView=self.ScrollRect.gameObject:GetComponent("LoopGridView");
    self.LoopGridView:InitGridView(0,self.OnGetItemByRowColumn)

    self.LoopGridView.ScrollRect.onValueChanged:AddListener(function(value)
        self:OnBookScrollChanged(value)
    end)

    --【总编号】
    self.TotalCount=0;
    self.m_page = 0;
    --等待消息返回
    self.m_waitBookRefresh=false;
    --等待ui刷新
    self.m_waitUiRefresh=false;
end
--endregion


--region【OnOpen】

function UIMasForm:OnOpen()
    UIView.OnOpen(self)

    GameController.ComuniadaControl:SetData2(self)

    --在MainTop里标记  是在排行榜界面里打开  主界面顶框的
    CS.XLuaHelper.SetMainTopClose("UIMasForm");

    --请求列表
    GameController.ComuniadaControl:GetwriterIndexRequest();
end

--endregion


--region 【OnClose】

function UIMasForm:OnClose()
    UIView.OnClose(self)
    --把本界面置空
    GameController.ComuniadaControl:SetData2(nil);

    --【清除列表所有对象 和 脚本】
    if(storyItemList)then
        for _key, _value in pairs(storyItemList) do
            if(_value)then
                _value:Delete();--【销毁】
            end
        end
    end
    --重置页数
    GameController.ComuniadaControl:Reset();

    if(self.LoopGridView)then
        self.LoopGridView.ScrollRect.onValueChanged:RemoveListener(function(value)
            self:OnBookScrollChanged(value)
        end)
    end


    self.TitleTxt = nil;
    self.ScrollRect = nil;
    self.LoopGridView = nil;
    self.TotalCount = nil;
    self.m_page = nil;
    self.m_waitBookRefresh = nil;
    self.m_waitUiRefresh = nil;

    --【删除所有服务器数据】
    Cache.ComuniadaCache:ClearMas();
end

--endregion

function UIMasForm:SetType(_type)
    mtype=_type;
    GameController.ComuniadaControl:RequestBook(self.m_page+1,mtype)
end



--region 【刷新数据】
function UIMasForm:UpdateWriterHotOrNewBookList(_type,page)

    if (page > 0)then
        self.m_page = page;
        self.m_waitBookRefresh = false;
    end

    if(_type==EStoryList.MostPopular)then
        self.TitleTxt.text=CS.CTextManager.Instance:GetText(283);
        self.TotalCount=table.length(Cache.ComuniadaCache.allHotList);
    elseif(_type==EStoryList.LatestRelease)then
        self.TitleTxt.text=CS.CTextManager.Instance:GetText(284);
        self.TotalCount=table.length(Cache.ComuniadaCache.allNewList);
    end

    if(self.TotalCount and self.TotalCount>0)then
        --【设置列表总数量】
        self.LoopGridView:SetListItemCount(self.TotalCount,false);
    end
end


function UIMasForm.OnGetItemByRowColumn(gridView,index,row,column)
    if (index < 0)then
        return nil;
    end

    local _index=index+1;
    local itemData=nil;
    if(mtype==EStoryList.MostPopular)then
        itemData =Cache.ComuniadaCache:GetHotInfoByIndex(_index);
    elseif(mtype==EStoryList.LatestRelease)then
        itemData =Cache.ComuniadaCache:GetNewInfoByIndex(_index);
    end

    if(itemData == nil)then
        return nil;
    end

    --【生成一个新的实例】
   local loopItem = gridView:NewListViewItem("StoryItem");

    --【GameObect唯一编号】
    local onlyID=loopItem.gameObject:GetInstanceID();

    --【书本 脚本】
    local storyItem = table.trygetvalue(storyItemList,onlyID);
    if(storyItem==nil)then
        storyItem = StoryItem.New(loopItem.gameObject);
        storyItemList[onlyID]=storyItem;
    end

    if (loopItem.IsInitHandlerCalled == false)then
        loopItem.IsInitHandlerCalled = true;
    end

    --【赋值】
    if(storyItem)then
        storyItem:SetItemData(itemData,_index);
    end

    return loopItem;
end

function UIMasForm:OnBookScrollChanged(value)
    if(self.m_waitBookRefresh==true)then
        return;
    end
    if(self.m_waitUiRefresh==true)then
        if (value.y >= 0.1)then
            self.m_waitUiRefresh = false;
        end

    else
        if (value.y < 0.1)then
            self.m_waitUiRefresh = true;--等待ui刷新
            self.m_waitBookRefresh = true;--等待消息返回
            GameController.ComuniadaControl:RequestBook(self.m_page+1,mtype)
        end
    end
end


--endregion


--region 【界面关闭】
function UIMasForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion


return UIMasForm