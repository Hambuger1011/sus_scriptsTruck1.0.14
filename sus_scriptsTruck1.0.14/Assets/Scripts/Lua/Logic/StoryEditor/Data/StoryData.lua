local BaseClass = core.Class
local StoryData = BaseClass("StoryData")
local File = CS.CFileManager
local DataDefine = require('Logic/StoryEditor/Data/DataDefine')
StoryData.DataDefine = DataDefine

-- local pb_define = [[
-- package pb;
-- message t_StoryDetial
-- {
--     optional int32 id = 1;
--     optional string title = 2; 
-- }


-- message t_StoryDialog
-- {
--     optional int32 id = 1;
--     optional int32 dialogType = 2; 
--     optional int32 msgBoxType = 3; 
--     optional string dialogMsg = 4; 
-- }


-- message t_StoryDetialList
-- {
--     repeated t_StoryDetial items = 1;
-- }

-- message t_StoryDialogList
-- {
--     repeated t_StoryDialog items = 1;
-- }

-- ]]


function StoryData:__init()
    --BaseClass.__init(self)
    --self.protoc = core.Protobuf.protoc.new()
    --self.protoc:load(pb_define)
    logic.cs.CFileManager.CreateDirectory(logic.config.WritablePath..'/cache/story_image/')
    logic.cs.CFileManager.CreateDirectory(logic.config.WritablePath..'/cache/story_cache/')
    self.m_image_md5_url = {}--[md5,url]
    self.m_spriteMap = {}--[filename,sprite]
    self.m_dowloadImageQueue = {}
end

function StoryData:AddImageUrlByMd5(md5, url)
    self.m_image_md5_url[md5] = url
end

function StoryData:GetImageUrlByMd5(md5)
    return self.m_image_md5_url[md5]
end


function StoryData:LoadData()
    local filename = logic.config.WritablePath..'/cache/story_image/md5.json'
    if File.IsFileExist(filename) then
        local bytes = File.ReadFileString(filename)
        self.m_image_md5_url = core.json.Derialize(bytes) or {}
    else
        self.m_image_md5_url = {}
    end
end

function StoryData:SaveData()
    local filename = logic.config.WritablePath..'/cache/story_image/md5.json'
    local strJson = core.json.Serialize(self.m_image_md5_url)
    File.WriteFileString(filename, strJson)
end

function StoryData:UnloadData()
end

function StoryData:DownloadImage(imgUrl, filename, callback)
    if string.IsNullOrEmpty(imgUrl) then
        return
    end
    local item = self.m_dowloadImageQueue[filename]
    if item == nil then
        logic.debug.LogError('DownloadImage:'..imgUrl)
        item = {
            callbackList = {}
        }
        self.m_dowloadImageQueue[filename] = item
        item.requestOperation = logic.cs.LuaHelper.DownloadFile(imgUrl, filename, function(isOK)
            logic.debug.Log('DownloadImage完成:'..imgUrl..',call count:'..#item.callbackList..'isOK='..tostring(isOK))
            self.m_dowloadImageQueue[filename] = nil
            for _,func in pairs(item.callbackList) do
                func(isOK)
            end
        end)
    end
    table.insert(item.callbackList, callback)
end

function StoryData:GetDownloadImageItem(filename)
    return self.m_dowloadImageQueue[filename]
end

---根据文件名加载sprite
function StoryData:GetSpriteByFilename(filename, isAutoLoad)
    local sprite = self.m_spriteMap[filename]
    if sprite == nil and isAutoLoad then
        logic.debug.LogError('[+]'..filename)
        sprite = logic.cs.LuaHelper.LoadSprite(filename)
        self.m_spriteMap[filename] = sprite
    end
    return type(sprite) ~= 'string' and sprite or nil
end

function StoryData:GetImageMd5(filenameNoEx)
    local filename = filenameNoEx
    if string.endswith(filenameNoEx,'.png') then
        logic.debug.LogError('[-]'..filenameNoEx)
    else
        filename = filenameNoEx..'.png'
    end
    local md5 = logic.cs.CFileManager.GetFileMd5(filename)
    return md5
end

---@return isDoawnloading
function StoryData:LoadSprite(filenameNoEx,fileMd5, imgUrl,callback)
    if string.IsNullOrEmpty(fileMd5) then
        callback(nil)
        return false
    end
    local filename = filenameNoEx..'.png'
    local sprite = self.m_spriteMap[filename]
    if sprite then
        callback(type(sprite) ~= 'string' and sprite or nil)
        return false
    end

    self:AddImageUrlByMd5(fileMd5, imgUrl)
    local onComplete = function(md5)
        sprite = self.m_spriteMap[filename] 
        if sprite == nil then
            if fileMd5 == md5 then
                logic.cs.LuaHelper.LoadSpriteAsync(filename,function(newSprite)
                    self.m_spriteMap[filename] = newSprite
                    callback(newSprite)
                end)
            else    --md5不匹配
                logic.debug.LogError('md5不匹配'..tostring(fileMd5)..' <=> '..tostring(md5))
                self.m_spriteMap[filename] = 'error'
                sprite = nil
                callback(nil)
            end
        end
    end

    local item = self.m_dowloadImageQueue[filename]
    if item == nil then --没有在下载中
        local md5 = self:GetImageMd5(filenameNoEx)
        if fileMd5 == md5 then  --本地已有图片且md5相同
            onComplete(md5)
            return
        end
    end
    
    --下载图片
    logic.StoryEditorMgr.data:DownloadImage(imgUrl, filename, function(isOK)
        if isOK then
            logic.debug.LogError('下载图片成功:url='..imgUrl..'\nfile='..filename)
            local md5 = self:GetImageMd5(filenameNoEx)
            onComplete(md5)
        else
            callback(nil)
        end
    end)
    return true
end

---@param data t_ReadingRecord
function StoryData:LoadBookReadRecord(bookID, chapterID, data)
    local userID = logic.cs.GameHttpNet.UUID
    local str = logic.cs.PlayerPrefs.GetString(userID..'_storyeditor_readrecord_pos_'..bookID..'_'..chapterid, nil)
    local json = core.json.Derialize(str)
    local data = DataDefine.t_ReadingRecord.New()
    data:FromTable(json)
    return data
end

function StoryData:SaveBookReadRecord(bookID, chapterID, data)
    local userID = logic.cs.GameHttpNet.UUID
    local str = core.json.Serialize(data)
    logic.cs.PlayerPrefs.SetString(userID..'_storyeditor_readrecord_pos_'..bookID..'_'..chapterid, str)
end

return StoryData