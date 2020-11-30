
--[[
    章节item
]]
local BaseClass = core.Class
---@class UIChapterItem
local UIChapterItem = BaseClass("UIChapterItem")

function UIChapterItem:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(logic.cs.UIBinding))
    self.button = self.uiBinding:Get('body', typeof(logic.cs.UITweenButton))
    self.imgBG = self.uiBinding:Get('body', typeof(logic.cs.Image))
    self.lbName = self.uiBinding:Get('lbName', typeof(logic.cs.Text))
    self.lbDesc = self.uiBinding:Get('lbDesc',  typeof(logic.cs.Text))
    self.btnMenu = self.uiBinding:Get('btnMenu', typeof(logic.cs.UITweenButton))

    self.button.onClick:AddListener(function()
        logic.EventDispatcher:Broadcast(logic.EventName.on_story_chapter_content_editor, self.chapterData)
    end)

    self.onClick = nil
    self.btnMenu.onClick:AddListener(function()
        if self.onClick then
            self.onClick()
        end
    end)
    
    self.bgColor = self.imgBG.color
    self.states = {}
    for i=1, 6 do
        self.states[i] = self.uiBinding:Get('state_'..i)
    end
end

function UIChapterItem:__delete()
    if not logic.IsNull(self.gameObject) then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject = nil
    self.transform = nil
    self.button = nil
end

---@param index number
---@param storyDetial StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
function UIChapterItem:SetData(index,storyDetial, chapterData, isReadonly)
    isReadonly = isReadonly or false
    self.isReadonly = isReadonly
    self.index = index
    chapterData.chapter_number = index
    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial
    ---@type StoryEditor_ChapterDetial
    self.chapterData = chapterData
    if self.chapterData.status == nil then
        self.chapterData.status = 0
    end
    self:RefresUI()
end

function UIChapterItem:RefresUI()
    -- if math.fmod(self.index, 2) == 0 then
    --     self.imgBG.color = core.Color.white
    -- else
    --     self.imgBG.color = self.bgColor
    -- end
    self.chapterData.title = 'Chapter '..self.index
    self.lbName.text = self.chapterData.title
    self.lbDesc.text = self.chapterData.description

    for index,item in pairs(self.states) do
        local isOn = (index == (self.chapterData.status + 1))
        item:SetActiveEx(isOn)
    end
end


return UIChapterItem