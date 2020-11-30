
-- local BaseClass = core.Class
-- ---@class UIChapterItem
-- local UIChapterItem = BaseClass("UIChapterItem")

-- function UIChapterItem:__init(gameObject)
--     self.gameObject = gameObject
--     self.transform = gameObject.transform
--     self.uiBinding = gameObject:GetComponent(typeof(logic.cs.UIBinding))
    
--     self.lbBookName = self.uiBinding:Get('lbBookName', typeof(logic.cs.Text))
--     self.lbChapterName = self.uiBinding:Get('lbChapterName',  typeof(logic.cs.Text))
--     self.lbChapterDesc = self.uiBinding:Get('lbChapterDesc',  typeof(logic.cs.Text))
--     self.lbAuthor = self.uiBinding:Get('lbAuthor',  typeof(logic.cs.Text))
--     self.lbLectoresCount = self.uiBinding:Get('lbLectoresCount', typeof(logic.cs.Text))

--     self.imgCover = self.uiBinding:Get('imgCover', typeof(logic.cs.Image))
--     self.defaultCoverSize = self.imgCover.transform.rect.size
--     self.defaultSprite = self.imgCover.sprite

--     self.onClick = nil
--     self.btnPlay = self.uiBinding:Get('btnPlay', typeof(logic.cs.UITweenButton))
--     self.btnPlay.onClick:AddListener(function()
--         if self.onClick then
--             self.onClick()
--         end
--     end)
-- end

-- function UIChapterItem:__delete()
--     if not logic.IsNull(self.gameObject) then
--         logic.cs.GameObject.Destroy(self.gameObject)
--     end
--     self.gameObject = nil
--     self.transform = nil
--     self.button = nil
-- end

-- ---@param index number
-- ---@param storyDetial StoryEditor_BookDetials
-- ---@param chapterData StoryEditor_ChapterDetial
-- function UIChapterItem:SetData(index,totalNum, storyDetial, chapterData)
--     self.index = index
--     self.totalNum = totalNum
--     ---@type StoryEditor_BookDetials
--     self.storyDetial = storyDetial
--     ---@type StoryEditor_ChapterDetial
--     self.chapterData = chapterData
--     self:RefresUI()
-- end

-- function UIChapterItem:RefresUI()
--     self:SetBookCover(nil)
--     local cover_image = self.storyDetial.cover_image
--     if not string.IsNullOrEmpty(cover_image) then
--         local url,md5 = logic.StoryEditorMgr:ParseImageUrl(cover_image)
--         local filename = logic.config.WritablePath .. '/cache/story_image/'..md5
--         logic.StoryEditorMgr.data:LoadSprite(filename, md5, url, function(sprite)
--             self:SetBookCover(sprite)
--         end)
--     end

--     self.lbBookName.text = self.storyDetial.title
--     self.lbChapterName.text = 'Chapter.'..self.index..' of '..self.totalNum
--     self.lbChapterDesc.text = self.storyDetial.description
--     self.lbAuthor.text = 'Author: '..self.storyDetial.writer_name
--     self.lbLectoresCount.text = tostring(self.chapterData.word_count)
--     self:InitCatagoryList()
-- end


-- function UIChapterItem:SetBookCover(sprite)
--     local uiImage = self.imgCover
--     if logic.IsNull(uiImage) then
--         return
--     end
--     if logic.IsNull(sprite) then
--         sprite = self.defaultSprite
--     end
--     logic.StoryEditorMgr:SetCover(uiImage,sprite,self.defaultCoverSize)
-- end



-- function UIChapterItem:InitCatagoryList()
--     self.selectCatagorys = self.selectCatagorys or {}
--     ---@type UICatagoryItem[]
--     self.uiCatagorys = self.uiCatagorys or {}
    
--     if self.catagoryRoot == nil then
--         self.catagoryRoot = self.uiBinding:Get('catagoryRoot').transform
--         local getComponent = logic.cs.LuaHelper.GetComponent
--         for i=0, 2 do
--             local tf = self.catagoryRoot:GetChild(i)
--             --go:SetActiveEx(true)
--             local uiItem = {
--                 transform = tf,
--                 gameObject = tf.gameObject,
--                 lbName = getComponent(tf,'Text',typeof(logic.cs.Text))
--             }
--             uiItem.SetData = function(self,index)
--                 local DataDefine = logic.StoryEditorMgr.DataDefine
--                 local name = DataDefine.BookTags[index]
--                 self.lbName.text = name
--             end
--             table.insert(self.uiCatagorys, uiItem)
--         end
--     end

--     for i = 1, #self.uiCatagorys do
--         local uiItem = self.uiCatagorys[i]
--         local kv = self.storyDetial.tagArray:GetKVByIndex(i)
--         local isOn = (kv and kv.Value)
--         if isOn then
--             uiItem.gameObject:SetActiveEx(true)
--             uiItem:SetData(kv.Key)
--         else
--             uiItem.gameObject:SetActiveEx(false)
--         end
--     end
-- end
-- return UIChapterItem