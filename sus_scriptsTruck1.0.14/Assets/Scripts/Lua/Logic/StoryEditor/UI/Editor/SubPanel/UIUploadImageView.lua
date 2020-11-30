local BaseClass = core.Class

---@class UIStory_UploadImageView
local UIUploadImageView = BaseClass("UIUploadImageView")

function UIUploadImageView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))
    
    self.btnPickImage = self.uiBinding:Get('btnPickImage',typeof(logic.cs.Button))
    self.btnCancel = self.uiBinding:Get('btnCancel',typeof(logic.cs.UITweenButton))
    self.btnSure = self.uiBinding:Get('btnSure',typeof(logic.cs.UITweenButton))
    
    self.imgNone = self.uiBinding:Get('imgNone',typeof(logic.cs.Image))
    self.imgPicture = self.uiBinding:Get('imgPicture',typeof(logic.cs.Image))
    self.pictureSize = self.imgPicture.transform.rect.size

    local button = gameObject:GetComponent(typeof(logic.cs.Button))
    button.onClick:AddListener(function()
        self:Hide()
    end)
    
    self.btnSure.onClick:AddListener(function()
        self:UploadImage(self.filepath)
    end)
    self.btnCancel.onClick:AddListener(function()
        self:Hide()
    end)
    self.btnPickImage.onClick:AddListener(function()
        -- logic.cs.NativeGallery.GetImageFromGallery(function(path)
        --     if string.IsNullOrEmpty(path) then
        --         return
        --     end
        --     self.filepath = path
        --     local sprite = logic.cs.LuaHelper.LoadSprite(path)
        --     self:SetBookCover(sprite)
        -- end, "请选择图片")
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
                        logic.cs.LuaHelper.UnloadSprite(sprite) --释放原图
                        local imgMd5 = logic.cs.LuaHelper.WriteTexture2D(logic.config.WritablePath..'/cache/story_image/', clipTexture2D)
                        logic.cs.Object.Destroy(clipTexture2D)
                        clipTexture2D = nil
                        --self:UploadImage(filemd5)
                        self.filepath = logic.config.WritablePath..'/cache/story_image/'..imgMd5..'.png'
                        local sprite = logic.cs.LuaHelper.LoadSprite(self.filepath)
                        self:SetBookCover(sprite)
                    end
                end)
            end, true)
        end, "请选择图片")
    end)

end

function UIUploadImageView:__delete()
end

function UIUploadImageView:Show()
    self.gameObject:SetActiveEx(true)
end
function UIUploadImageView:Hide()
    self.gameObject:SetActiveEx(false)
end


---@param storyDetial StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
---@param storyNodeRoot t_StoryNode
function UIUploadImageView:SetData(storyDetial, chapterData, storyNodeRoot,url,md5,callback)
    ---@type StoryEditor_BookDetials
    self.storyDetial = storyDetial
    ---@type StoryEditor_ChapterDetial
    self.chapterData = chapterData
    ---@type t_StoryTable
    self.storyNodeRoot = storyNodeRoot
    self.storyNodeRoot.name = storyDetial.title
    self.callback = callback
    
    self:SetImageUrl(url,md5)
end

function UIUploadImageView:SetImageUrl(url,md5)
    self:SetBookCover(nil)
    if not string.IsNullOrEmpty(url) then
        --local url,md5 = logic.StoryEditorMgr:ParseImageUrl(imgUrl)
        local filename = logic.config.WritablePath .. '/cache/story_image/'..md5
        self.filepath = filename..'.png'
        logic.StoryEditorMgr.data:LoadSprite(filename, md5, url, function(sprite)
            self:SetBookCover(sprite)
        end)
    else
        self.filepath = nil
    end
end

function UIUploadImageView:SetBookCover(sprite)
    if logic.IsNull(sprite) then
        self.imgPicture.gameObject:SetActiveEx(false)
        self.imgNone.gameObject:SetActiveEx(true)
    else
        self.imgPicture.sprite = sprite
        local imgSize = sprite.rect.size
        imgSize = core.Vector2.New(imgSize.x, imgSize.y) / self.imgPicture.pixelsPerUnit --图片真实分辨率
        local ratio = (imgSize.x / imgSize.y)
        imgSize.y = self.pictureSize.y
        imgSize.x = self.pictureSize.y * ratio
        local t = self.imgPicture.transform
        logic.cs.LuaHelper.SetUISize(t, imgSize.x, imgSize.y)

        self.imgPicture.gameObject:SetActiveEx(true)
        self.imgNone.gameObject:SetActiveEx(false)
    end
end

function UIUploadImageView:UploadImage(filepath)
    local filemd5 = logic.cs.CFileManager.GetFileMd5(filepath)
    local filename = logic.config.WritablePath..'/cache/story_image/'..filemd5
    local dstFile = filename..'.png'
    local imgUrl = logic.StoryEditorMgr.data:GetImageUrlByMd5(filemd5)
    if imgUrl then  --服务器上已经有文件
        if not logic.cs.CFileManager.IsFileExist(dstFile) then
            logic.cs.CFileManager.CopyFile(filepath, dstFile)   --拷贝到本地
        end
        self:Hide()
        self.callback(imgUrl,filemd5)
        --logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_image_add, filemd5, imgUrl, self.insertPosition)
    else    --需要上次图片到服务器
        logic.StoryEditorMgr:UploadImage(self.storyDetial.id, self.chapterData.chapter_number, filepath,function(isOK,imgUrl)
            if isOK then
                self:Hide()
                --local md5 = logic.cs.CFileManager.GetMd5(imgUrl)
                --logic.cs.CFileManager.CopyFile(filepath, dstFile)
                logic.StoryEditorMgr.data:AddImageUrlByMd5(filemd5, imgUrl)
                logic.StoryEditorMgr.data:SaveData()
                --logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_image_add,filemd5, imgUrl, self.insertPosition)
                
                self.callback(imgUrl,filemd5)
            else
                logic.cs.UIAlertMgr:Show("TIPS",'upload image failed')
            end
        end)
    end
end
return UIUploadImageView