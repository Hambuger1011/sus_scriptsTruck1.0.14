local PrivateLetterItem = core.Class("PrivateLetterItem")

function PrivateLetterItem:__init(gameObject)
    self.gameObject=gameObject;

    self.BG =CS.DisplayUtil.GetChild(gameObject, "BG");
    self.State =CS.DisplayUtil.GetChild(gameObject, "State");
    self.OpenImage =CS.DisplayUtil.GetChild(gameObject, "OpenImage");
    self.TitleText =CS.DisplayUtil.GetChild(gameObject, "TitleText"):GetComponent("Text");
    self.TimeText =CS.DisplayUtil.GetChild(gameObject, "TimeText"):GetComponent("Text");
    self.Content =CS.DisplayUtil.GetChild(gameObject, "Content"):GetComponent("Text");
    self.SelectTab = CS.DisplayUtil.GetChild(gameObject, "SelectTab"):GetComponent("UIToggle");


    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.BG,function(data) self:Openlicke() end)
    logic.cs.UIEventListener.AddOnClickListener(self.SelectTab.gameObject,function(data) self:SelectTabclicke() end)

    self.mItemIndex = 0;
    self.Info=nil;
end

function PrivateLetterItem:SetItemData(itemData,itemIndex)
    if(itemData==nil)then return; end
    self.Info=itemData;
    self.mItemIndex = itemIndex;
    self.Content.text=itemData.content;

    --【显示时间】
    self.TimeText.text = itemData.createtime;
    --标题名字
    self.TitleText.text= itemData.user_info.nickname;

    if(itemData.is_read==0)then
        self.OpenImage:SetActive(true);
    else
        self.State:SetActive(false);
        self.OpenImage:SetActive(false);
    end

end


function PrivateLetterItem:Openlicke()

    self.State:SetActive(false);
    self.OpenImage:SetActive(false);
    if(self.Info)then

        if(self.Info.is_read==0)then
            GameController.EmailControl:ReadPrivateLetterTeamRequest(self.Info.from_uid,true)
        else
            GameController.ChatControl:GetPrivateLetterPageRequest(self.Info.from_uid,1,self.Info.user_info.nickname);
        end


    end
end


function PrivateLetterItem:SelectTabclicke()
    if(self.Info and self.SelectTab.isOn==true)then
        GameController.EmailControl:BatchTest(self.Info.id,2,true);
    else
        GameController.EmailControl:BatchTest(self.Info.id,2,false);
    end
end


--销毁
function PrivateLetterItem:__delete()
    if(self.SelectTab)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.SelectTab.gameObject,function(data) self:SelectTabclicke() end)
        logic.cs.UIEventListener.RemoveOnClickListener(self.BG,function(data) self:Openlicke() end)

    end

    self.BG = nil;
    self.State = nil;
    self.OpenImage= nil;
    self.TitleText = nil;
    self.TimeText= nil;
    self.Content = nil;
    self.SelectTab= nil;

    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return PrivateLetterItem
