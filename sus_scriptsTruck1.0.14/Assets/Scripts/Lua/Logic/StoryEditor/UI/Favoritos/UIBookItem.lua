
local BaseClass = core.Class
---@class UIBookItem
local UIBookItem = BaseClass("UIBookItem")

function UIBookItem:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(logic.cs.UIBinding))
    self.button = gameObject:GetComponent(typeof(logic.cs.UITweenButton))
    self.lbName = self.uiBinding:Get('bookName', typeof(logic.cs.Text))
    --self.lbDesc = get(self.transform, 'lbDesc', typeof(logic.cs.Text))
    self.imgCover = self.uiBinding:Get('imgCover', typeof(logic.cs.Image))
    self.defaultCoverSize = self.imgCover.transform.rect.size
    self.defaultSprite = self.imgCover.sprite

    self.button.onClick:AddListener(function()
        if self.onClick then
            self:onClick()
        end
    end)
    self.m_downloadSeq = 1
end

function UIBookItem:__delete()
    if not logic.IsNull(self.gameObject) then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject = nil
    self.transform = nil
    self.button = nil
end

---@param storyDetial StoryEditor_BookDetial
function UIBookItem:SetData(storyDetial)
    self.m_downloadSeq = self.m_downloadSeq + 1
    self.storyDetial = storyDetial
    self:RefresUI()


    self:SetBookCover(nil)
    local cover_image = storyDetial.cover_image
    if not string.IsNullOrEmpty(cover_image) then
        --local bookID = storyDetial.id
        local url,md5 = logic.StoryEditorMgr:ParseImageUrl(cover_image)
        local filename = logic.config.WritablePath .. '/cache/story_image/'..md5
        
        local downloadSeq = self.m_downloadSeq
        logic.StoryEditorMgr.data:LoadSprite(filename, md5, url,function(sprite)
            if downloadSeq ~= self.m_downloadSeq then
                logic.debug.LogError('seq mismatch:'..downloadSeq..'<=>'..self.m_downloadSeq)
                return
            end
            self:SetBookCover(sprite)
        end)
    end
end

function UIBookItem:RefresUI()
    self.lbName.text = self.storyDetial.title
    --self.lbDesc.text = self.storyDetial.desc
end


function UIBookItem:SetBookCover(sprite)
    if logic.IsNull(self.gameObject) then
        return
    end
    if logic.IsNull(sprite) then
        sprite = self.defaultSprite
    end
    local uiImage = self.imgCover
    logic.StoryEditorMgr:SetCover(uiImage,sprite,self.defaultCoverSize)
end

return UIBookItem