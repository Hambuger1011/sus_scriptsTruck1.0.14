---@class EmailItem
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local EmailItem  = core.Class('EmailItem',UIBubbleBox)
local UIEmailInfoForm = require('Logic/UI/UI_Email/UIEmailInfoForm')

---@class EmailItem_Item
local Item = core.Class("EmailItem.Item")

function Item:__init(uiItem)
    self.transform = uiItem
    self.gameObject = uiItem.gameObject

    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end

--region-------EmailItem

---@param uiItem UIBubbleItem
function EmailItem:__init(uiItem)
    UIBubbleBox.__init(self, uiItem)
    local transform = uiItem.transform
    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.bgButton = self.uiBinding:Get('BGButton',typeof(logic.cs.Button))
    self.openImage = self.uiBinding:Get('OpenImage').gameObject
    self.hit = self.uiBinding:Get('Hit').gameObject
    self.title = self.uiBinding:Get('Title',  typeof(logic.cs.Text))
    self.content = self.uiBinding:Get('Content',  typeof(logic.cs.Text))
    self.time = self.uiBinding:Get('Time',  typeof(logic.cs.Text))
    self.giftButton = self.uiBinding:Get('Gift',typeof(logic.cs.Button))
    self.headImage = self.uiBinding:Get('HeadImage', typeof(logic.cs.Image))
    self.HeadFrame = self.uiBinding:Get('HeadFrame', typeof(logic.cs.Image))

    self.headImageMask = self.uiBinding:Get('HeadImageMask').gameObject

    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end


function EmailItem:GetSize()
    return self.boxSize
end

function EmailItem:SetSize()
end

function EmailItem:SetData(Data)
    if Data.pageNum and Data.pageNum ~= 0 then
        Data.pageNum = 0
    elseif Data.pageNum and Data.pageNum == 0 and Data.GetNextPage then
        Data.GetNextPage()
        Data.GetNextPage = nil
    end
    self.time.text = Data.createtime
    self.title.text = Data.title

    local contentTxt = string.gsub(Data.content,'\\n','\n')
    if Data.msg_type == 4 and Data.comment_is_self == 0 then
        contentTxt = logic.cs.PluginTools:ReplaceBannedWords(contentTxt)
    end
    self.content.text = contentTxt
    if Data.msg_type == 4 then

        --加载头像
        GameHelper.luaShowDressUpForm(Data.comment_avatar,self.headImage,DressUp.Avatar,1001);
        --加载头像框
        GameHelper.luaShowDressUpForm(Data.comment_avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);

        --self.headImage.sprite = CS.ResourceManager.Instance:GetUISprite("ProfileForm/img_renwu"..Data.comment_face_icon);
        self.headImageMask:SetActiveEx(true)
    else
        self.headImageMask:SetActiveEx(false)
    end

    if Data.msg_type == 2 and Data.price_status == 0 then
        self.giftButton.gameObject:SetActiveEx(true)
    else
        self.giftButton.gameObject:SetActiveEx(false)
    end

    if Data.status == 1 and Data.msg_type ~= 4 then
        self.openImage:SetActiveEx(true)
    else
        self.openImage:SetActiveEx(false)
    end

    self:updateRedPoint(Data)

    self.bgButton.onClick:RemoveAllListeners()
    self.bgButton.onClick:AddListener(function()
        self:OnItemClick(Data)
    end)

    self.boxSize = core.Vector2.New(715, 155)

end

--endregion
function EmailItem:OnItemClick(Data)
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

function EmailItem:updateRedPoint(_Data)
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
return EmailItem