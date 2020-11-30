--[[
    创作详情
]]
local BaseClass = core.Class
local UIView = core.UIView
local UIStory_Detials = BaseClass("UIStory_Detials", UIView)
local base = UIView
local uiid = logic.uiid

UIStory_Detials.config = {
	ID = uiid.Story_Detials,
	AssetName = 'UI/StoryEditorRes/UI/Canvas_Story_Detials'
}
local UIChapterItem = require("Logic/StoryEditor/UI/Detials/UIChapterItem")
local DataDefine = require('Logic/StoryEditor/Data/DataDefine')


function UIStory_Detials:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))

    self.btnClose = self.uiBinding:Get('btnClose', typeof(logic.cs.UITweenButton))
    self.btnMore = self.uiBinding:Get('btnMore', typeof(logic.cs.UITweenButton))
    self.btnEditor = self.uiBinding:Get('btnEditor', typeof(logic.cs.UITweenButton))
   
    self.chapterPfb = self.uiBinding:Get('chapterPfb')
    self.chapterRoot = self.uiBinding:Get('chapterRoot').transform
    self.txtBookName = self.uiBinding:Get('txtBookName', typeof(logic.cs.Text))
    self.txtBookDesc = self.uiBinding:Get('txtBookDesc', typeof(logic.cs.Text))
    self.txtAuthor = self.uiBinding:Get('txtAuthor', typeof(logic.cs.Text))

    self.btnPickImage = self.uiBinding:Get('btnPickImage',typeof(logic.cs.UITweenButton))
    self.imgNone = self.uiBinding:Get('imgNone',typeof(logic.cs.Image))
    self.imgCover = self.uiBinding:Get('imgCover', typeof(logic.cs.Image))
    self.coverMask = self.uiBinding:Get('BookIconMask',typeof(logic.cs.Image))
    self.defaultCoverSize = self.imgCover.transform.rect.size
    self.defaultSprite = self.imgCover.sprite
    self.imgNone.gameObject:SetActiveEx(true)
    self.imgCover.gameObject:SetActiveEx(false)
    self.coverMask.gameObject:SetActiveEx(false)
    self.btnPickImage.onClick:AddListener(function()
        -- local uiView = logic.UIMgr:Open(logic.uiid.Story_NewBook)
        -- uiView:SetData(self.storyDetial,function(isCreate,newDetials)
        --     self.storyDetial = newDetials
        --     self:SetBookDetials(
        --         newDetials.id,
        --         newDetials.title,
        --         newDetials.description,
        --         newDetials.writer_name,
        --         newDetials.read_count,
        --         newDetials.favorite_count,
        --         newDetials.word_count,
        --         newDetials.cover_image
        --     )
        -- end)
        -- uiView:GotoStep2()
        self:OnUploadImageClick()
    end)

    self.chapterPfb:SetActiveEx(false)
    self.btnMore.onClick:AddListener(function()
        self.chapterEditorView:Show()
        self.chapterEditorView:SetData('',0,function(strDesc)
            self:OnChapterNew('',strDesc)
        end)
    end)
    self.btnClose.onClick:AddListener(function()
        if self.moreMenuView.isActive then
            self.moreMenuView:Hide()
            return
        end
        self:OnExitClick()
    end)
    self.btnEditor.onClick:AddListener(function()
        local uiView = logic.UIMgr:Open(logic.uiid.Story_NewBook)
        uiView:SetData(self.storyDetial,function(isCreate,newDetials)
            self.storyDetial = newDetials
            self:SetBookDetials(
                newDetials.id,
                newDetials.title,
                newDetials.description,
                newDetials.writer_name,
                newDetials.read_count,
                newDetials.favorite_count,
                newDetials.word_count,
                newDetials.cover_image
            )
        end)
    end)


    self.moreMenuView = require('Logic/StoryEditor/UI/Detials/UIMoreMenuView').New(self.uiBinding:Get('moreMenuView'))
    self.moreMenuView:Hide()

    self.deleteBookView = require('Logic/StoryEditor/UI/Detials/UIDeleteBookView').New(self.uiBinding:Get('body_delete'))
    self.deleteBookView:Hide()
    
    self.chapterMenuView = require('Logic/StoryEditor/UI/Detials/UIMenu_Chapter').New(self.uiBinding:Get('menu_chapter'))
    self.chapterMenuView:Hide(true)

    self.chapterEditorView = require('Logic/StoryEditor/UI/Detials/UIChapterEditorView').New(self.uiBinding:Get('edit_chapter'))
    self.chapterEditorView:Hide()

    self.btnMoreMenu = self.uiBinding:Get('btnMoreMenu',typeof(logic.cs.UITweenButton))
    self.btnMoreMenu.onClick:AddListener(function()
        if self.moreMenuView.isActive then
            self.moreMenuView:Hide()
        else
            self.moreMenuView:Show()
        end
    end)

    

    
    self.lbLectoresCount = self.uiBinding:Get('lbLectoresCount', typeof(logic.cs.Text))
    self.lbFavoritosCount = self.uiBinding:Get('lbFavoritosCount', typeof(logic.cs.Text))
    self.lbPalabrasCount = self.uiBinding:Get('lbPalabrasCount', typeof(logic.cs.Text))

end

function UIStory_Detials:OnOpen()
    UIView.OnOpen(self)
    
    logic.StoryEditorMgr:EnterStoryEditorMode()
    logic.EventDispatcher:AddListener(logic.EventName.on_story_delete_book,self.OnDeleteBookClick,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_chapter_editor,self.OnChapterEdit,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_chapter_refresh,self.OnChapterRefresh,self)
    logic.EventDispatcher:AddListener(logic.EventName.on_story_chapter_content_editor,self.OnEditContentClick,self)
end

function UIStory_Detials:OnClose()
    UIView.OnClose(self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_delete_book,self.OnDeleteBookClick,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_chapter_editor,self.OnChapterEdit,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_chapter_refresh,self.OnChapterRefresh,self)
    logic.EventDispatcher:RemoveListener(logic.EventName.on_story_chapter_content_editor,self.OnEditContentClick,self)

    self.moreMenuView:Delete()
    self.deleteBookView:Delete()
    self.chapterMenuView:Delete()
    self.chapterEditorView:Delete()
end

function UIStory_Detials:OnExitClick()
    logic.UIMgr:Close(logic.uiid.Story_Detials)
    logic.StoryEditorMgr:BackToMainClick()
end

function UIStory_Detials:SetBookDetials(
    bookID,
    bookName,
    bookDesc,
    writerName,
    LectoresCount,
    FavoritosCount,
    PalabrasCount,
    cover_image
)
    self.txtBookName.text = bookName
    self.txtBookDesc.text = bookDesc
    self.txtAuthor.text = 'Author: '..tostring(writerName)
    self.lbLectoresCount.text = tostring(LectoresCount)
    self.lbFavoritosCount.text = tostring(FavoritosCount)
    self.lbPalabrasCount.text = tostring(PalabrasCount)


    self:SetBookCover(nil)
    if not string.IsNullOrEmpty(cover_image) then
        local url,md5 = logic.StoryEditorMgr:ParseImageUrl(cover_image)
        local filename = logic.config.WritablePath .. '/cache/story_image/'..md5
        logic.StoryEditorMgr.data:LoadSprite(filename, md5, url, function(sprite)
            self:SetBookCover(sprite)
        end)
    end
end


function UIStory_Detials:SetBookCover(sprite)
    local uiImage = self.imgCover
    if logic.IsNull(uiImage) then
        return
    end
    if logic.IsNull(sprite) then
        --sprite = self.defaultSprite
        self.imgNone.gameObject:SetActiveEx(true)
        self.imgCover.gameObject:SetActiveEx(false)
        self.coverMask.gameObject:SetActiveEx(false)
    else
        self.imgNone.gameObject:SetActiveEx(false)
        self.imgCover.gameObject:SetActiveEx(true)
        self.coverMask.gameObject:SetActiveEx(true)
        logic.StoryEditorMgr:SetCover(uiImage,sprite,self.defaultCoverSize)
    end
end

---@param chapterDetialList StoryEditor_ChapterDetial[]
local setBookChapters = function(self,chapterDetialList)
    self.uiChapters = self.uiChapters or {}
    local count = table.length(chapterDetialList)
    for i=#self.uiChapters, count do
        local go = logic.cs.GameObject.Instantiate(self.chapterPfb,self.chapterRoot)
        --go:SetActiveEx(true)
        local uiItem = UIChapterItem.New(go)
        table.insert(self.uiChapters, uiItem)
        uiItem.onClick = function()
            if self.isReadonly then
                local chapterData = uiItem.chapterData
                local DataDefine = logic.StoryEditorMgr.DataDefine
                local chapterID = chapterData.chapter_number
                --logic.cs.UINetLoadingMgr:Show()
                logic.StoryEditorMgr:LoadStoryEditorData(self.storyDetial,chapterID,chapterData.update_version, function(storyTable)
                    --logic.cs.UINetLoadingMgr:Close()
                    local storyNodeRoot = DataDefine.t_StoryNode.Create(storyTable)
                    --storyNodeRoot.name = self.storyDetial.title
                    local uiView = logic.UIMgr:Open(logic.uiid.Story_Preview)
                    uiView:SetData(self.storyDetial, chapterID, storyNodeRoot, true)
                end)
            else
                self.chapterMenuView:Show()
                self.chapterMenuView:SetData(uiItem)
            end
        end
    end
    for i = 1, #self.uiChapters do
        local uiItem = self.uiChapters[i]
        local isOn = (i <= count)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(i,self.storyDetial,chapterDetialList[i],self.isReadonly)
        else
            uiItem.gameObject:SetActiveEx(false)
        end
    end
    
    self.btnMore.transform.parent:SetAsLastSibling() 
    --self.btnMore.gameObject:SetActiveEx(not self.isReadonly)
end
                                                                               

---@param storyDetial StoryEditor_BookDetials
function UIStory_Detials:SetData(storyDetial, isReadonly)
    isReadonly = isReadonly or false
    self.isReadonly = isReadonly
    self.moreMenuView.isReadonly = isReadonly
    self.btnEditor.gameObject:SetActiveEx(not isReadonly)
    self.btnMore.gameObject:SetActiveEx(not isReadonly)

    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial
    self:SetBookDetials(
        storyDetial.id,
        storyDetial.title,
        storyDetial.description,
        storyDetial.writer_name,
        storyDetial.read_count,
        storyDetial.favorite_count,
        storyDetial.word_count,
        storyDetial.cover_image
        )
    self:InitCatagoryList()
    self:OnChapterRefresh()
end

function UIStory_Detials:InitCatagoryList()
    self.selectCatagorys = self.selectCatagorys or {}
    ---@type UICatagoryItem[]
    self.uiCatagorys = self.uiCatagorys or {}
    
    if self.catagoryRoot == nil then
        self.catagoryRoot = self.uiBinding:Get('catagoryRoot').transform
        local getComponent = logic.cs.LuaHelper.GetComponent
        for i=0, 2 do
            local tf = self.catagoryRoot:GetChild(i)
            --go:SetActiveEx(true)
            local uiItem = {
                transform = tf,
                gameObject = tf.gameObject,
                lbName = getComponent(tf,'Text',typeof(logic.cs.Text))
            }
            uiItem.SetData = function(self,index)
                local DataDefine = logic.StoryEditorMgr.DataDefine
                local name = DataDefine.BookTags[index]
                self.lbName.text = name
            end
            table.insert(self.uiCatagorys, uiItem)
        end
    end

    for i = 1, #self.uiCatagorys do
        local uiItem = self.uiCatagorys[i]
        local kv = self.storyDetial.tagArray:GetKVByIndex(i)
        local isOn = (kv and kv.Value)
        if isOn then
            uiItem.gameObject:SetActiveEx(true)
            uiItem:SetData(kv.Key)
        else
            uiItem.gameObject:SetActiveEx(false)
        end
    end
end

---@param chapterData t_Chapter
function UIStory_Detials:OnChapterNew(title,description)
    local book_id = self.storyDetial.id
    local chapter_id = 1
    if self.storyDetial.total_chapter_count then
        chapter_id = self.storyDetial.total_chapter_count + 1
    end
    logic.gameHttp:StoryEditor_NewChapter(book_id, chapter_id, title, description,function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            self.storyDetial.total_chapter_count = chapter_id 
            ---@type StoryEditor_ChapterDetial
            local data = {}
            data.id = json.chapter_id
            data.book_id = book_id
            data.chapter_number = chapter_id
            data.title = title
            data.description = description
            data.status = 0
            table.insert(self.chapterDetialList,data)
            setBookChapters(self, self.chapterDetialList)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
end


function UIStory_Detials:OnChapterRefresh()
    local storyDetial =self.storyDetial
    if self.isReadonly then
        logic.gameHttp:StoryEditor_GetChapterList(storyDetial.id, function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                self.chapterDetialList = json.data
                setBookChapters(self, self.chapterDetialList)
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
    else
        logic.gameHttp:StoryEditor_GetMyChapterList(storyDetial.id, function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                self.chapterDetialList = json.data
                setBookChapters(self, self.chapterDetialList)
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
    end
end

function UIStory_Detials:OnChapterEdit(index)
    self.chapterEditorView:Show()
    local chapterData = self.chapterDetialList[index]
    self.chapterEditorView:SetData(chapterData.description,index,function(strDesc)
        logic.StoryEditorMgr:ModifyChapter(
            chapterData.book_id, 
            chapterData.chapter_number,
            chapterData.title,
            strDesc, 
            function()
                chapterData.description = strDesc
                local uiItem = self.uiChapters[index]
                uiItem:SetData(index,self.storyDetial,chapterData,self.isReadonly)
            end)

    end)
end

function UIStory_Detials:OnDeleteBookClick()
    local book_id = self.storyDetial.id
    self.deleteBookView:Show(function(isOK)
        if not isOK then
            return
        end
        logic.debug.LogError("delete book:"..book_id)
        logic.gameHttp:StoryEditor_DeleteBook(book_id,function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                self:OnExitClick()
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
    end)
end


--编辑对话
function UIStory_Detials:OnEditContentClick(chapterData)
    local DataDefine = logic.StoryEditorMgr.DataDefine
    local chapterID = chapterData.chapter_number
    --logic.cs.UINetLoadingMgr:Show()
    local uiView = logic.UIMgr:Open(logic.uiid.Story_Editor)
    uiView:SetData(self.storyDetial, chapterData)
    
    coroutine.start(function()
        coroutine.wait(0.25)
        logic.StoryEditorMgr:LoadStoryEditorData(self.storyDetial,chapterID,chapterData.update_version, function(storyTable)
            --logic.cs.UINetLoadingMgr:Close()
            uiView:SetData(self.storyDetial, chapterData,storyTable)
        end)
    end)
end


local uploadImage = function(self,filemd5)
    local complete = function(imgUrl)
        local title = self.storyDetial.title
        local description = self.storyDetial.description
        local penName = self.storyDetial.writer_name
        local tag = self.storyDetial.tagArray
        local cover_image = imgUrl..'?'..filemd5
        logic.gameHttp:StoryEditor_SaveBook(penName, self.storyDetial.id,title,description,tag,cover_image,function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                self.storyDetial.cover_image = cover_image
                local filename = logic.config.WritablePath..'cache/story_image/'..filemd5
                logic.StoryEditorMgr.data:LoadSprite(filename, filemd5, imgUrl, function(sprite)
                    self:SetBookCover(sprite)
                end)
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
    end

    local imgUrl = logic.StoryEditorMgr.data:GetImageUrlByMd5(filemd5)
    if imgUrl then  --服务器上已经有文件
        self.storyDetial.cover_image = imgUrl..'?'..filemd5
        complete(imgUrl)
    else    --需要上次图片到服务器
        --logic.cs.UINetLoadingMgr:Show(-1)
        local filename = logic.config.WritablePath..'cache/story_image/'..filemd5..'.png'
        logic.StoryEditorMgr:UploadImage(self.storyDetial.id, 0, filename, function(isOK,imgUrl)
            if logic.IsNull(self.uiform) then
                return
            end
            --logic.cs.UINetLoadingMgr:Close()
            if isOK then
                --local md5 = logic.cs.CFileManager.GetMd5(imgUrl)
                --logic.cs.CFileManager.CopyFile(filepath, dstFile)
                logic.StoryEditorMgr.data:AddImageUrlByMd5(filemd5, imgUrl)
                logic.StoryEditorMgr.data:SaveData()
                complete(imgUrl)

            else
                logic.cs.UIAlertMgr:Show("TIPS",'upload image failed')
            end
        end)
    end
end

function UIStory_Detials:OnUploadImageClick()
    logic.cs.NativeGallery.GetImageFromGallery(function(path)
        path = path or ''
        if not logic.cs.CFileManager.IsFileExist(path) then
            return
        end
        logic.cs.LuaHelper.LoadSpriteAsync(path,function(sprite)
            if sprite == nil then
                return
            end
            local imageCutView = logic.UIMgr:Open(logic.uiid.Texture2DClipper)
            imageCutView:SetData(sprite,function(isOK, x,y,w,h)
                if not isOK then
                    return
                else
                    local clipTexture2D = logic.cs.LuaHelper.Texture2dClipper(sprite.texture,x,y,w,h)
                    logic.cs.LuaHelper.UnloadSprite(sprite)
                    local filemd5 = logic.cs.LuaHelper.WriteTexture2D(logic.config.WritablePath..'/cache/story_image/', clipTexture2D)
                    logic.cs.Object.Destroy(clipTexture2D)
                    clipTexture2D = nil
                    uploadImage(self, filemd5)
                end
            end)
        end, true)
    end, "请选择图片")
end

return UIStory_Detials