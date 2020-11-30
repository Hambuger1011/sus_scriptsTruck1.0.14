local Class = core.Class

---@class BubbleData
local BubbleData = Class('BubbleData')

---@param storyNode t_StoryNode
function BubbleData:__init(storyDetial, storyNode)

    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial
    
    ---@type t_StoryNode
    self.storyNode = storyNode
end

return BubbleData