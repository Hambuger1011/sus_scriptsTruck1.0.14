---@class UIBubbleBox_Picture
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local UIBubbleBox_Picture  = core.Class('UIBubbleBox_Picture',UIBubbleBox)

---@class UIBubbleBox_Picture_Item
local Item = core.Class("UIBubbleBox_Picture.Item")

function Item:__init(transform)
    self.transform = transform
    self.gameObject = transform.gameObject

    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.headIcon = self.uiBinding:Get('imgIcon', typeof(logic.cs.Image))
    self.txtName = self.uiBinding:Get('lbName', typeof(logic.cs.Text))
    --self.lbMsgRoot = self.uiBinding:Get('msgRoot').transform
    self.imgPicture = self.uiBinding:Get('imgPicture',  typeof(logic.cs.Image))
    self.defaultPicture = self.imgPicture.sprite

    self.SetActive = function(self,isOn)
        self.gameObject:SetActiveEx(isOn)
    end
    self.extends = core.Vector2.New(0,36)

    ---@type UIBubbleItem
    self.refUiItem = 0
end

--region-------UIBubbleBox_Picture

---@param uiItem UIBubbleItem
function UIBubbleBox_Picture:__init(uiItem)
    UIBubbleBox.__init(self, uiItem)
    local transform = uiItem.transform
    local EBubbleType = logic.StoryEditorMgr.DataDefine.EBubbleType
    self.boxes = {
        [EBubbleType.LeadRole] = Item.New(transform:Find("L")),
        [EBubbleType.SupportingRole] = Item.New(transform:Find("R")),
        [EBubbleType.Narration] = Item.New(transform:Find("M")),
    }
    self.boxes[EBubbleType.Narration].extends = core.Vector2.New(0,0)
    
    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.btnEditorMask = self.uiBinding:Get('EditorMask',typeof(logic.cs.UITweenButton))
    self.m_downloadSeq = 1
    
    
    self.btnEditorMask.onClick:AddListener(function()
        self:OnEditorClick()
    end)
    self.roleHeadAtlas = logic.cs.LuaHelper.GetAtlas(
        logic.ResMgr.tag.StoryEditor,
        logic.ResMgr.type.Atlas,
        'UI/StoryEditorRes/RoleHead'
        )
        self.boxSize = core.Vector2.New(300,300)
end

function UIBubbleBox_Picture:SetBoxType(boxType)
    ---@type UIBubbleBox_Picture_Item
    self.activeBox = self.boxes[boxType]
    for _,box in pairs(self.boxes) do
        box:SetActive(self.activeBox == box)
    end
end

function UIBubbleBox_Picture:GetSize()
    return self.boxSize
end

function UIBubbleBox_Picture:SetSize()

    --self.activeBox.labelRoot:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.labelSize.x)
    --self.activeBox.labelRoot:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.labelSize.y)

    self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.boxSize.x)
    self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
    --self.refUiItem.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
end


---@param dialogData BubbleData
function UIBubbleBox_Picture:SetData(bubbleData)
    local EBubbleBoxType = logic.StoryEditorMgr.DataDefine.EBubbleBoxType
    local EBubbleType = logic.StoryEditorMgr.DataDefine.EBubbleType

    local storyNode = bubbleData.storyNode
    ---@type t_StoryItem
    local storyItem = storyNode.storyItem
    ---@type StoryEditor_BookDetials
    local storyDetial = bubbleData.storyDetial
    ---@type StoryEditor_Role
    local roleTable = storyDetial.roleTable

    --logic.debug.LogError(self.lbMsg)
    self:SetBoxType(roleTable:GetDialogType(storyItem.roleID))

    local box = self.activeBox
    ---@type t_Role
    local roleInfo = roleTable:GetRole(storyItem.roleID)
    if roleInfo then
        box.txtName.text = roleInfo.name
    else
        box.txtName.text = "???"
    end
    if roleInfo.icon ~= 0 then
        local headName = roleInfo:GetIconName()
        box.headIcon.sprite = self.roleHeadAtlas:GetSprite(headName)
    end

    self:LoadImage(storyItem)
end

function UIBubbleBox_Picture:OnDeactive()
    self.m_downloadSeq = self.m_downloadSeq + 1
    -- if self.m_requestOperation then
    --     --self.m_requestOperation.webRequest:Abort()
    --     self.m_requestOperation = nil
    -- end
end

function UIBubbleBox_Picture:LoadImage(storyItem)
    
    self.pictureSize = core.Vector2.New(610,300)
    local imgUrl = storyItem.image
    local filemd5 = storyItem.imageMd5 or ''
    local filename = logic.config.WritablePath..'/cache/story_image/'..filemd5
    local downloadSeq = self.m_downloadSeq
    local isDoawnloading = logic.StoryEditorMgr.data:LoadSprite(filename, filemd5, imgUrl,function(sprite)
        if downloadSeq ~= self.m_downloadSeq then
            logic.debug.LogError('seq mismatch:'..downloadSeq..'<=>'..self.m_downloadSeq)
            return
        end
        self:SetSprite(sprite)
    end)
    if isDoawnloading then
        self:SetSprite(nil)
    end
end

function UIBubbleBox_Picture:SetSprite(sprite)
    if self.refUiItem == nil or logic.IsNull(self.refUiItem.gameObject) then
        return
    end
    local box = self.activeBox
    if logic.IsNull(sprite) then
        sprite = box.defaultPicture
    end
    local uiImage = box.imgPicture
    self.boxSize = logic.StoryEditorMgr:SetSprite(uiImage,sprite,self.pictureSize)
    self.boxSize.y = self.boxSize.y + box.extends.y
    self.refUiItem.itemData.isDirty = true --重新计算ui大小
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_list_refresh)
end

--endregion


function UIBubbleBox_Picture:OnEditorClick()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_dialog_click,self.refUiItem)
end

return UIBubbleBox_Picture