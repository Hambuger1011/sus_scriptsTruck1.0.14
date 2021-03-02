local UIPrizeHistoryForm = core.Class("UIPrizeHistoryForm", core.UIView)

local PrizeHistoryItem = require('Logic/UI/UI_PrizeHistory/Item/PrizeHistoryItem');

UIPrizeHistoryForm.config = {
    ID = logic.uiid.UIPrizeHistoryForm,
    AssetName = 'UI/Resident/UI/UIPrizeHistoryForm'
}

--region【Awake】

local PrizeHistorylist={};

local this=nil;
function UIPrizeHistoryForm:OnInitView()
    core.UIView.OnInitView(self)
    this=self.uiform;



    self.Mask = CS.DisplayUtil.GetChild(this.gameObject, "Mask");
    self.ScrollView = CS.DisplayUtil.GetChild(this.gameObject, "ScrollView");
    self.Content = CS.DisplayUtil.GetChild(self.ScrollView, "Content");
    self.PrizeHistoryItemObj = CS.DisplayUtil.GetChild(this.gameObject, "PrizeHistoryItem");
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.Mask,function(data) self:OnExitClick(data) end);
end
--endregion


--region【OnOpen】

function UIPrizeHistoryForm:OnOpen()
    core.UIView.OnOpen(self)

    GameController.LotteryControl:SetData2(self);
    GameController.LotteryControl:GetMyTurntableRecordRequest();
end

--endregion


--region 【OnClose】

function UIPrizeHistoryForm:OnClose()
    core.UIView.OnClose(self)

    GameController.LotteryControl:SetData2(nil);


    local len=table.length(PrizeHistorylist);
    for i = 1, len do
        PrizeHistorylist[i]:Delete();
    end
end

--endregion


function UIPrizeHistoryForm:UpdateItemList()
    local historylist=Cache.LotteryCache.historylist;
    if(GameHelper.islistHave(historylist))then
        local len=table.length(historylist);
        for i = 1, len do
            local go = logic.cs.GameObject.Instantiate(self.PrizeHistoryItemObj, self.Content.transform);
            go.transform.localPosition = core.Vector3.zero;
            go.transform.localScale = core.Vector3.one;
            go:SetActive(true);
            local item =PrizeHistoryItem.New(go);
            table.insert(PrizeHistorylist,item);
            item:SetItemData(historylist[i]);
        end
    end
end



--region 【界面关闭】
function UIPrizeHistoryForm:OnExitClick()
    core.UIView.__Close(self)

    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIPrizeHistoryForm