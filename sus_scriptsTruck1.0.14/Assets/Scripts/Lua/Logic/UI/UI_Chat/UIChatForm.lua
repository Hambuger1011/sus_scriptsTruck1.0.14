local UIView = core.UIView

local UIChatForm = core.Class("UIChatForm", UIView)

local ChatItem = require('Logic/UI/UI_Chat/Item/ChatItem')

UIChatForm.config = {
    ID = logic.uiid.UIChatForm,
    AssetName = 'UI/Resident/UI/UIChatForm'
}

--region【Awake】

local this=nil;

--【总数量】
local allCount=0;

--【ChatItem脚本列表】
local ItemList={};

function UIChatForm:OnInitView()
    UIView.OnInitView(self)
    this=self.uiform;

    self.LoopListView2 = CS.DisplayUtil.GetChild(this.gameObject, "ScrollRect"):GetComponent("LoopListView2");
    --list插件
    self.LoopListView2:InitListView(allCount,UIChatForm.OnGetItemByRowColumn);

    self.Bottom = CS.DisplayUtil.GetChild(this.gameObject, "Bottom");

    self.InputField = CS.DisplayUtil.GetChild(self.Bottom, "InputField"):GetComponent("InputField");
    self.InputField.shouldHideMobileInput = true;

    self.SubmitBtn = CS.DisplayUtil.GetChild(self.Bottom, "SubmitBtn");


    logic.cs.UIEventListener.AddOnClickListener(self.SubmitBtn,function(data) self:SubmitBtnClick(); end)

    self:Moni()
end
--endregion


--region【OnOpen】

function UIChatForm:OnOpen()
    UIView.OnOpen(self)

end

--endregion


--region 【OnClose】

function UIChatForm:OnClose()
    UIView.OnClose(self)
end

--endregion


--region【模拟对话数据】

local chatlist={};
function UIChatForm:Moni()


    --【模拟对话数据】
    local chatInfo1={};
    chatInfo1.id=1;
    chatInfo1.type=1; --1我自己  2他人
    chatInfo1.playername="monkey";
    chatInfo1.content="You do not have enough words to suggest. A minimum of 10 characters is required."
    chatInfo1.avatar=1005;
    chatInfo1.avatar_frame=2007;


    local chatInfo2={};
    chatInfo2.id=1;
    chatInfo2.type=2;
    chatInfo2.playername="Tom";
    chatInfo2.content="Your secrets user data is being migrated. lt will take a little time. Please wait patiently. After the migration, you can experience it."
    chatInfo2.avatar=1003;
    chatInfo2.avatar_frame=2007;


    local chatInfo3={};
    chatInfo3.id=1;
    chatInfo3.type=2;
    chatInfo3.playername="Tom";
    chatInfo3.content="Fuck."
    chatInfo3.avatar=1003;
    chatInfo3.avatar_frame=2007;


    local chatInfo4={};
    chatInfo4.id=1;
    chatInfo4.type=2;
    chatInfo4.playername="Jack";
    chatInfo4.content="Keys unlock chapters and can be purchased at the Shop."
    chatInfo4.avatar=1003;
    chatInfo4.avatar_frame=2007;


    local chatInfo5={};
    chatInfo5.id=1;
    chatInfo5.type=1; --1我自己  2他人
    chatInfo5.playername="monkey";
    chatInfo5.content="Thank you for your feedback. We will discuss this with our team."
    chatInfo5.avatar=1005;
    chatInfo5.avatar_frame=2007;



    table.insert(chatlist,chatInfo1);
    table.insert(chatlist,chatInfo2);
    table.insert(chatlist,chatInfo3);
    table.insert(chatlist,chatInfo4);
    table.insert(chatlist,chatInfo5);

    self:UpdateInfo();
end
--endregion


--region【设置列表Item】
function UIChatForm:UpdateInfo()
    if(chatlist==nil)then return; end
    allCount=table.length(chatlist);
    --【设置列表总数量】
    self.LoopListView2:SetListItemCount(allCount);
end
--endregion


--region【刷新数据】
function UIChatForm.OnGetItemByRowColumn(listView,index)

    if (index < 0)then
        return nil;
    end

    local _index=index+1;

    local itemData =chatlist[_index];
    if(itemData == nil)then return nil; end

    local item=nil;

    if(itemData.type==2)then
        item = listView:NewListViewItem("ChatItem1");  --【左边其他人】
    elseif(itemData.type==1)then
        item = listView:NewListViewItem("ChatItem2");  --【右边我自己】
    end

    --【GameObect唯一编号】
    local onlyID=item.gameObject:GetInstanceID();

    --【书本 脚本】
    local chatItem = table.trygetvalue(ItemList,onlyID);
    if(chatItem==nil)then
        chatItem = ChatItem.New(item.gameObject);
        ItemList[onlyID]=chatItem;
    end

    if (item.IsInitHandlerCalled == false)then
        item.IsInitHandlerCalled = true;
    end

    --【赋值】
    if(chatItem)then
        chatItem:SetItemData(itemData,index);
    end
    return item;
end

--endregion


--region 【重新刷新列表所有Item】【位置计算】
function UIChatForm:Compute()
    self.LoopListView2:RefreshAllShownItem();
end
--endregion


--region【发送消息】

function UIChatForm:SubmitBtnClick()

    if(self.InputField.text==nil or self.InputField.text=="")then return; end

    local chatInfo={};
    chatInfo.id=1;
    chatInfo.type=1; --1我自己  2他人
    chatInfo.playername="monkey";
    chatInfo.content=self.InputField.text;
    chatInfo.avatar=1005;
    chatInfo.avatar_frame=2007;

    table.insert(chatlist,chatInfo);

    self:UpdateInfo()

    local _index = allCount - 1;
    self:OnJumpToItem(_index);
end

--endregion


--region 【跳转到指定Item】

function UIChatForm:OnJumpToItem(itemIndex)
    if(self.LoopListView2==nil)then return; end
    if(itemIndex < 0)then return nil; end
    self.LoopListView2:MovePanelToItemIndex(itemIndex, 0);
end

--endregion


--region 【界面关闭】
function UIChatForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion



return UIChatForm