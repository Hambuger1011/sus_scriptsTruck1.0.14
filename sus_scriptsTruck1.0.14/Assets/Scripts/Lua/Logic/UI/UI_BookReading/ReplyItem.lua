---@class ReplyItem
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local ReplyItem  = core.Class('ReplyItem',UIBubbleBox)

---@class ReplyItem_Item
local Item = core.Class("ReplyItem.Item")

function Item:__init(uiItem)
    self.transform = uiItem
    self.gameObject = uiItem.gameObject

    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end

--region-------ReplyItem

---@param uiItem UIBubbleItem
function ReplyItem:__init(uiItem)
    UIBubbleBox.__init(self, uiItem)
    local transform = uiItem.transform
    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.activeBox = Item.New(uiItem.transform)
    self.line = self.uiBinding:Get('Line').transform
    self.headImage = self.uiBinding:Get('HeadImage', typeof(logic.cs.Image))
    self.HeadFrame = self.uiBinding:Get('HeadFrame', typeof(logic.cs.Image))
    self.nameText = self.uiBinding:Get('Name',  typeof(logic.cs.Text))
    self.contentText = self.uiBinding:Get('ContentText',  typeof(logic.cs.Text))
    self.dateText = self.uiBinding:Get('Date',  typeof(logic.cs.Text))
    self.replyText = self.uiBinding:Get('ReplyText',  typeof(logic.cs.Text))
    self.replyBtn = self.uiBinding:Get('ReplyBtn',  typeof(logic.cs.UITweenButton))

    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end


function ReplyItem:GetSize()
    return self.boxSize
end

function ReplyItem:SetSize()
    --self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.boxSize.x)
    self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
end

function ReplyItem:SetData(Data)
    if Data.pageNum and Data.pageNum ~= 0 then
        if Data.pageNum == 1 then
            Data.GetNextPage()
            Data.GetNextPage = nil
        end
        Data.pageNum = 0
    elseif Data.pageNum and Data.pageNum == 0 and Data.GetNextPage then
        Data.GetNextPage()
        Data.GetNextPage = nil
    end

    --加载头像
    GameHelper.luaShowDressUpForm(Data.avatar,self.headImage,DressUp.Avatar,1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(Data.avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);
    --self.headImage.sprite = CS.ResourceManager.Instance:GetUISprite("ProfileForm/img_renwu"..Data.face_icon);

    self.nameText.text = Data.nickname
    
    local contentTxt = Data.content
    if Data.reply_id and Data.reply_id ~= 0 then
        local haveName = string.contains(contentTxt,"@"..Data.reply_nickname..":")
        if haveName then
            contentTxt = string.gsub(contentTxt,"@"..Data.reply_nickname..":","",1)
        end
        if Data.is_self == 0 then
            contentTxt = logic.cs.PluginTools:ReplaceBannedWords(contentTxt)
        end
        if haveName then
            contentTxt = "<color='#2088fd'>@"..Data.reply_nickname..":</color>"..contentTxt
        end
    else
        if Data.is_self == 0 then
            contentTxt = logic.cs.PluginTools:ReplaceBannedWords(contentTxt)
        end
    end
    self.contentText.text = contentTxt
    self.contentText.transform:GetComponent("ContentSizeFitter"):SetLayoutVertical()
    self.dateText.text = os.date("%m-%d %H:%M", Data.create_time)
    self.replyText.text = Data.reply_count
    
    self.replyBtn.onClick:RemoveAllListeners()
    self.replyBtn.onClick:AddListener(function()
        Data.replyBtnClick(Data.id,Data.nickname)
    end)
    
    local rootSize = self.contentText.transform.rect.size
    local pos = self.contentText.transform.anchoredPosition
    local linePos = self.line.anchoredPosition
    local height = Mathf.Abs(rootSize.y) + Mathf.Abs(pos.y) + Mathf.Abs(linePos.y)
    self.boxSize = core.Vector2.New(750, height)

end

--endregion


return ReplyItem