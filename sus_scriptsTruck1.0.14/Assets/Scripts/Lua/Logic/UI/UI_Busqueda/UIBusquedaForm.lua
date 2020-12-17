local UIView = core.UIView

local UIBusquedaForm = core.Class("UIBusquedaForm", UIView)
local uiid = logic.uiid
UIBusquedaForm.config = {
    ID = uiid.UIBusquedaForm,
    AssetName = 'UI/Resident/UI/UIBusquedaForm'
}

local StoryItem2 = require('Logic/UI/UI_Busqueda/Item/StoryItem2');


--region【Awake】

local this=nil;
--【书本集合】
local storyItemList={};
function UIBusquedaForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    --Toogle 标签按钮
    self.topTrans = CS.DisplayUtil.GetChild(this.gameObject, "topTrans").transform;
    self.BookGroup = CS.DisplayUtil.GetChild(this.gameObject, "BookGroup")
    self.CloseBtn = CS.DisplayUtil.GetChild(this.gameObject, "CloseBtn")
    self.UIVirtualList=self.BookGroup.gameObject:GetComponent("UIVirtualList");
    self.InputSearch =CS.DisplayUtil.GetChild(this.gameObject, "InputSearch"):GetComponent("InputField");
    self.TileBg =CS.DisplayUtil.GetChild(this.gameObject, "TileBg");
    self.GenresList = require('Logic/UI/UI_Busqueda/GenresList').New(self.TileBg);

    self.UIVirtualList.onItemRefresh =UIBusquedaForm.OnGetItemByRowColumn;
    self.InputSearch.onEndEdit:AddListener(UIBusquedaForm.OnSearchBookName);
    self.UIVirtualList.scrollRect.onValueChanged:AddListener(function(value)
        self:OnBookScrollChanged(value)
    end)

    logic.cs.UIEventListener.AddOnClickListener(self.CloseBtn,function(data) self:OnExitClick() end);

    --【总编号】
    self.TotalCount=1;
    self.m_page = 0;
    --等待消息返回
    self.m_waitBookRefresh=false;
    --等待ui刷新
    self.m_waitUiRefresh=false;

end
--endregion


--region【OnOpen】

function UIBusquedaForm:OnOpen()
    UIView.OnOpen(self)
    GameController.BusquedaControl:SetData(self);
    GameController.BusquedaControl:RequestBook(self.m_page+1,nil,nil);
end

--endregion


--region 【OnClose】

function UIBusquedaForm:OnClose()
    UIView.OnClose(self)
    GameController.BusquedaControl:SetData(nil);
    self.UIVirtualList.onItemRefresh =nil;

    --【清除列表所有对象 和 脚本】
    if(storyItemList)then
        for _key, _value in pairs(storyItemList) do
            if(_value)then
                _value:Delete();--【销毁】
            end
        end
    end
    --重置页数
    GameController.BusquedaControl:Reset();

    if(self.UIVirtualList)then
        self.UIVirtualList.scrollRect.onValueChanged:RemoveListener(function(value)
            self:OnBookScrollChanged(value)
        end)
    end

    --【删除所有服务器数据】
    Cache.BusquedaCache:ClearMas();

    if(self.CloseBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.CloseBtn,function(data) self:OnExitClick() end);
    end

    self.topTrans = nil;
    self.BookGroup = nil;
    self.CloseBtn = nil;
    self.UIVirtualList = nil;
    self.InputSearch = nil;
    self.TileBg = nil;
    self.GenresList = nil;

end

--endregion


--region 【刷新数据】
function UIBusquedaForm:UpdateWriterBookList(page)

    if (page > 0)then
        self.m_page = page;
        self.m_waitBookRefresh = false;
    end

    self.TotalCount=table.length(Cache.BusquedaCache.BusquedaList);

    if(self.TotalCount and self.TotalCount>0)then
        --【设置列表总数量】
        self.UIVirtualList:SetItemCount(self.TotalCount);
    end
end

function UIBusquedaForm.OnGetItemByRowColumn(row)
    local index=row.itemIndex+1;
    local trans = row.rect;

    if(trans)then

        local itemData =Cache.BusquedaCache:GetInfoByIndex(index);
        if(itemData == nil)then return nil; end

        --【GameObect唯一编号】
        local onlyID=trans.gameObject:GetInstanceID();

        --【书本 脚本】
        local storyItem = table.trygetvalue(storyItemList,onlyID);
        if(storyItem==nil)then
            storyItem = StoryItem2.New(trans.gameObject);
            storyItemList[onlyID]=storyItem;
        end
        --【赋值】
        if(storyItem)then
            storyItem:SetItemData(itemData,index);
        end
    end

end

function UIBusquedaForm:OnBookScrollChanged(value)
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
            GameController.BusquedaControl:RequestBook(self.m_page+1,nil,nil);
        end
    end
end



function UIBusquedaForm.OnSearchBookName(value)
    GameController.BusquedaControl.m_curPage=0;
    GameController.BusquedaControl:RequestBook(1,nil,value);
end


--endregion



--region 【界面关闭】
function UIBusquedaForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIBusquedaForm