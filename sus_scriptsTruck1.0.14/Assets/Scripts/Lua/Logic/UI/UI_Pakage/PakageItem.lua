---@class PakageItem
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local PakageItem  = core.Class('PakageItem',UIBubbleBox)
local UIEmailInfoForm = require('Logic/UI/UI_Email/UIEmailInfoForm')

---@class PakageItem_Item
local Item = core.Class("PakageItem.Item")

function Item:__init(uiItem)
    self.transform = uiItem
    self.gameObject = uiItem.gameObject

    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end

--region-------PakageItem

function PakageItem:__init(gameObject,parentUI)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.parentUI = parentUI
    self.button = logic.cs.LuaHelper.GetComponent(self.transform, "BGButton",typeof(logic.cs.Button))
    self.imgIcon = logic.cs.LuaHelper.GetComponent(self.transform, "icon",typeof(logic.cs.Image))
    self.hit = CS.DisplayUtil.FindChild(self.transform, "Hit")
    self.txtNum = logic.cs.LuaHelper.GetComponent(self.transform, "BGButton",typeof(logic.cs.Text))
    self.txtCountDown = logic.cs.LuaHelper.GetComponent(self.transform, "countDowd/text",typeof(logic.cs.Text))
    self.txtNum = logic.cs.LuaHelper.GetComponent(self.transform, "txtName",typeof(logic.cs.Text))

end


function PakageItem:SetData(Data)

end

--endregion
function PakageItem:OnItemClick(Data)
    logic.debug.PrintTable(Data)

    if Data.status == 1 then
        UI_EmailInfo:SetEmailData(Data,self.hit,self);
        logic.UIMgr:Open(logic.uiid.EmailInfo)
    else
        logic.gameHttp:ReadSystemMsg(Data.msgid,function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                logic.cs.UserDataManager.selfBookInfo.data.unreadmsgcount = logic.cs.UserDataManager.selfBookInfo.data.unreadmsgcount - 1

                Data.status = 1
                self:updateRedPoint(Data)
                UIEmailInfoForm:SetEmailData(json.data.sysarr,self.hit,self);
                if(Data.msg_type~=2)then
                    self.openImage:SetActiveEx(true)
                end

                GameController.MainFormControl:RedPointRequest();
                logic.UIMgr:Open(logic.uiid.EmailInfo)
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
    end
end

function PakageItem:updateRedPoint(_Data)
    if(_Data.msg_type==2)then
        if(_Data.price_status==0)then
            self.hit:SetActiveEx(true)
        else
            self.hit:SetActiveEx(false)
        end
    else
        if _Data.status == 1 then
            self.hit:SetActiveEx(false)
        else
            self.hit:SetActiveEx(true)
        end
    end
end
return PakageItem