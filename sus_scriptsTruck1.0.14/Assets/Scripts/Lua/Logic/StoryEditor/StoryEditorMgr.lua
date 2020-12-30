local BaseClass = core.Class
local UIStoryEditorMgr = BaseClass("UIStoryEditorMgr")
local StoryData = require('Logic/StoryEditor/Data/StoryData')
local DataDefine = require('Logic/StoryEditor/Data/DataDefine')
UIStoryEditorMgr.DataDefine = DataDefine
local socket = require "socket"

function UIStoryEditorMgr:__init()
    --BaseClass.__init(self)
    self.data = StoryData.New()
end

function UIStoryEditorMgr:NewStoryDetial()
    local detial = logic.StoryEditorMgr.DataDefine.t_StoryDetial.New()
    detial.id = table.length(self.data.storyDetials) + 1
    return detial
end


function UIStoryEditorMgr:BackToMainClick()
    --logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.BookReadingForm)
    --logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.MainForm)
    --MainForm.OnOpen

    --打开主界面
    local uimain = logic.UIMgr:GetView(logic.uiid.UIMainForm);
    if(uimain==nil)then
        --打开主界面
        logic.UIMgr:Open(logic.uiid.UIMainForm);
    else
        uimain.uiform:Appear();
    end

    local hideFormUI = {
        logic.cs.UIFormName.MainFormTop,
        logic.cs.UIFormName.ComuniadaMas,
        logic.cs.UIFormName.Busqueda,
    }
    for i,uuid in pairs(hideFormUI) do
        local uiForm = logic.cs.CUIManager:GetForm(uuid)
        if not logic.IsNull(uiForm) then
            uiForm:Appear()
        end
    end

    --显示界面
    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if(uiform)then
        uiform.uiform:Appear();
    end


    if self.favoritosForm then
        self.favoritosForm:RefreshBookList()
    end
    self.data:UnloadData()
end

function UIStoryEditorMgr:EnterStoryEditorMode()
    local hideFormUI = {
        logic.cs.UIFormName.MainFormTop,
        logic.cs.UIFormName.MainDown,
        logic.cs.UIFormName.ComuniadaMas,
        logic.cs.UIFormName.Busqueda,
    }
    for i,uuid in pairs(hideFormUI) do
        local uiForm = logic.cs.CUIManager:GetForm(uuid)
        if not logic.IsNull(uiForm) then
            uiForm:Hide()
        end
    end

    --隐藏界面
    local uiform = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if(uiform)then
        uiform.uiform:Hide();
    end

    logic.UIMgr:Close(logic.uiid.UICommunityForm);
    logic.UIMgr:Close(logic.uiid.UIBusquedaForm);
    logic.UIMgr:Close(logic.uiid.UIMasForm);


    self.data:LoadData()
end

---@param storyDetial StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
function UIStoryEditorMgr:GetDialogList(bookID,chapterID,callback)
    logic.gameHttp:StoryEditor_GetDialogList(bookID,chapterID,function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then

            local timestamp = socket.gettime()
            local storyTable = logic.StoryEditorMgr.DataDefine.t_StoryTable.New()
            storyTable:FromTable(json.data)
            -- local bookJson = storyTable:ToJson()
            -- logic.debug.LogError(bookJson)
            -- storyTable.md5 = string.getMd5(bookJson)
            -- storyTable.oldJson = bookJson
            timestamp = socket.gettime() - timestamp
            logic.debug.Log(string.format('json解析耗时%.3fs',timestamp))

            callback(storyTable)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            --logic.cs.UINetLoadingMgr:Close()
        end
    end)
end


---@param storyDetial StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
function UIStoryEditorMgr:GetMyDialogList(bookID,chapterID,callback)
    logic.gameHttp:StoryEditor_GetMyDialogList(bookID,chapterID,function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then

            local timestamp = socket.gettime()
            local storyTable = logic.StoryEditorMgr.DataDefine.t_StoryTable.New()
            storyTable:FromTable(json.data)
            -- local bookJson = storyTable:ToJson()
            -- logic.debug.LogError(bookJson)
            -- storyTable.md5 = string.getMd5(bookJson)
            -- storyTable.oldJson = bookJson
            timestamp = socket.gettime() - timestamp
            logic.debug.Log(string.format('json解析耗时%.3fs',timestamp))

            callback(storyTable)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            --logic.cs.UINetLoadingMgr:Close()
        end
    end)
end

---@param storyDetial StoryEditor_BookDetials
---@param chapterID 章节id
---@param onlyUseLocal 是否只读取本地数据
function UIStoryEditorMgr:LoadStoryEditorData(storyDetial,chapterID, update_version, callback, onlyUseLocal)
    local File = logic.cs.CFileManager
    
    local rolePath = logic.cs.GameUtility.WritablePath..'/cache/story_cache/data_'..storyDetial.id..'.json'
    if File.IsFileExist(rolePath) then
        local roleJson = File.ReadFileString(rolePath)
        local roleData = core.json.Derialize(roleJson) or {}
        local roleTable = DataDefine.t_RoleTable.New()
        roleTable:FromTable(roleData)
        storyDetial.roleTable = roleTable
    end

    local path = logic.cs.GameUtility.WritablePath..'/cache/story_cache/data_'..storyDetial.id..'_'..chapterID..'.json'
    if File.IsFileExist(path) then
        local strJson = File.ReadFileString(path)
        local storyData = core.json.Derialize(strJson) or {}
        if storyData.update_version == update_version then
            -- local tb = storyData
            -- if #tb > 0 then
            --     local roleTable = DataDefine.t_RoleTable.New()
            --     roleTable:FromTable(tb.roleTable)
            --     storyDetial.roleTable = roleTable
            -- end

            -- local storyTable = logic.StoryEditorMgr.DataDefine.t_StoryTable.New()
            -- storyTable:FromTable(tb.storyTable)
            -- local bookJson = storyTable:ToJson()
            -- logic.debug.LogError(bookJson)
            -- storyTable.md5 = string.getMd5(bookJson)
            -- storyTable.oldJson = bookJson
            -- callback(storyTable)
            local storyTable = logic.StoryEditorMgr.DataDefine.t_StoryTable.New()
            storyTable:FromTable(storyData.data)
            -- local bookJson = storyTable:ToJson()
            -- logic.debug.LogError(bookJson)
            -- storyTable.md5 = string.getMd5(bookJson)
            -- storyTable.oldJson = bookJson
            callback(storyTable)
            return
        end
    end
    
    if onlyUseLocal then
        local storyTable = logic.StoryEditorMgr.DataDefine.t_StoryTable.New()
        callback(storyTable)
    else    --从服务端获取
        logic.StoryEditorMgr:GetMyDialogList(storyDetial.id, chapterID,function(storyTable)
            callback(storyTable)
        end)
    end
end


---@param storyDetial StoryEditor_BookDetials
---@param storyTable t_StoryTable
function UIStoryEditorMgr:SaveStoryEditorData(storyDetial,chapterID,storyTable,update_version)
    local roleData = storyDetial.roleTable:ToTable()
    local storyData = {
        data = storyTable:ToTable(),
        update_version = update_version
    }
    local File = logic.cs.CFileManager

    local path = logic.cs.GameUtility.WritablePath..'/cache/story_cache/data_'..storyDetial.id..'.json'
    local json = core.json.Serialize(roleData)
    File.WriteFileString(path,json)

    path = logic.cs.GameUtility.WritablePath..'/cache/story_cache/data_'..storyDetial.id..'_'..chapterID..'.json'
    json = core.json.Serialize(storyData)
    File.WriteFileString(path,json)
end



---@param storyDetial StoryEditor_BookDetials
function UIStoryEditorMgr:SumbitChapter(storyDetial,chapterID,callback)
    local bookID = storyDetial.id
    logic.gameHttp:SubmitChapter(bookID,chapterID, function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            callback()
        else
            --logic.cs.UINetLoadingMgr:Close()
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
end


---@param storyDetial StoryEditor_BookDetials
---@param storyTable t_StoryTable
function UIStoryEditorMgr:UploadChapter(storyDetial, chapterID, storyTable, callback)
    local bookID = storyDetial.id
    local roleTable = storyDetial.roleTable
    local roleJson = roleTable:ToJson()
    local bookJson = storyTable:ToJson()
    local roleMd5 = string.getMd5(roleJson)
    local bookMd5 = string.getMd5(bookJson)

    local isRoleDirty = roleTable.md5 ~= roleMd5
    local isBookDirty = storyTable.md5 ~= bookMd5

    if isRoleDirty then
        logic.debug.Log(tostring(roleTable.md5)..' <=> '.. roleMd5)
        logic.gameHttp:StoryEditor_SaveRoleList(bookID,roleJson, function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 or code == 201 then
                roleTable.md5 = roleMd5
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
                --logic.cs.UINetLoadingMgr:Close()
            end
        end)
    end
    
    if isBookDirty then
        logic.debug.LogError(tostring(storyTable.md5) ..' <=> '.. bookMd5)
        logic.StoryEditorMgr:SaveStoryTable(bookID, chapterID, bookJson,function(isOK,jsonData)
            
            if isOK then
                
                local code = tonumber(jsonData.code)
                if code == 200 then
                    -- storyTable.items:Foreach(function(i,kv)
                    --     ---@type t_StoryItem
                    --     local item = kv.Value
                    --     item.svrID = item.id
                    -- end)
                else
                    storyTable.items:Foreach(function(i,kv)
                        ---@type t_StoryItem
                        local item = kv.Value
                        item.svrID = 0
                    end)
                end
                storyTable.md5 = bookMd5
                callback()
                --logic.cs.UITipsMgr:PopupTips('Save Success', false)
            else
                logic.cs.UIAlertMgr:Show("TIPS",jsonData.msg)
                --logic.cs.UINetLoadingMgr:Close()
            end
        end)
    end
end

---@param storyDetial StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
function UIStoryEditorMgr:StartStoryReading(bookID,chapterID)

    logic.StoryEditorMgr:EnterBookDetials(bookID,function(storyDetial)

        self:GetDialogList(bookID,chapterID,function(storyTable)
            local timestamp = socket.gettime()
            self:EnterStoryEditorMode()
            local uiView = logic.UIMgr:Open(logic.uiid.Story_Preview)
            local storyNodeRoot = DataDefine.t_StoryNode.Create(storyTable)
            storyNodeRoot.name = storyDetial.title
            uiView:SetData(storyDetial, chapterID,storyNodeRoot)
            uiView.onClose = function()
                self:BackToMainClick()
            end
            timestamp = socket.gettime() - timestamp
            logic.debug.Log(string.format('打开界面耗时%.3fs',timestamp))
        end)
        
    end)
end

function UIStoryEditorMgr:OpenFavoritosScrollView(gameObject, isOn)
    if self.favoritosForm == nil or logic.IsNull(self.favoritosForm.gameObject) then
        -- if not isOn then
        --     return
        -- end
        self.favoritosForm = require('Logic/StoryEditor/UI/Favoritos/UIFavoritosForm').New(gameObject)
    end
    self.favoritosForm:SetActive(isOn)
end

function UIStoryEditorMgr:EnterBookDetials(bookID, callback)
    logic.gameHttp:StoryEditor_GetMyBookDetail(bookID,function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then

            ---@type StoryEditor_BookDetials
            local storyDetial = json.data
            storyDetial.roleTable = DataDefine.t_RoleTable.New()
            storyDetial.roleTable:FromTable(core.json.Derialize(storyDetial.role_list))
            storyDetial.role_list = nil
            -- local roleJson = storyDetial.roleTable:ToJson()
            -- storyDetial.roleTable.md5 = string.getMd5(roleJson)

            storyDetial.UpdateTags = function(self)
                self.tagArray = System.Collections.SortedDictionary.New()
                local array = string.split(self.tag,',')
                for i,v in pairs(array) do
                    local val = tonumber(v)
                    self.tagArray:Set(val,true)
                end
            end
            storyDetial:UpdateTags()
            callback(storyDetial)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            --logic.cs.UINetLoadingMgr:Close()
        end
    end)
end

function UIStoryEditorMgr:SaveStoryTable(bookId,chapterId, json, callback)
    logic.debug.LogError(json)
    logic.gameHttp:StoryEditor_SaveDialogList(bookId, chapterId,json, function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 or code == 201 then
            callback(true,json)
        else
            callback(false,json)
            --logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
end

function UIStoryEditorMgr:UploadImage(bookId,chapterId,filename,callback)
    --logic.cs.UINetLoadingMgr:Show(-1)
    logic.gameHttp:StoryEditor_GetBookUploadToken(bookId,chapterId,function(result)
        --logic.cs.UINetLoadingMgr:Close()
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            local data = json.data
            local url = data.action
            
            local form = CS.UnityEngine.WWWForm()
            --form.headers:Add("Content-Type", "multipart/form-data")
            form:AddField("key", data.file_name)
            form:AddField("Content-Type", "image/jpeg")
            form:AddField("acl", data.acl)
            form:AddField("X-Amz-Credential", data["X-Amz-Credential"])
            form:AddField("X-Amz-Algorithm", data["X-Amz-Algorithm"])
            form:AddField("X-Amz-Date", data["X-Amz-Date"])
            form:AddField("Policy", data["Policy"])
            form:AddField("X-Amz-Signature", data["X-Amz-Signature"])

            local buffer = logic.cs.CFileManager.ReadFile(filename)
            if logic.IsNull(buffer) then
                logic.debug.LogError("加载文件失败:"..tostring(filename))
                callback(false)
                return
            end
            form:AddBinaryData("file", buffer);

            --logic.cs.UINetLoadingMgr:Show(-1)
            logic.cs.LuaHelper.UploadImage(url,form,function(webRequest)
                --logic.cs.UINetLoadingMgr:Close()
                if webRequest.isHttpError or webRequest.isNetworkError then
                    logic.debug.LogError('[E]'..webRequest.error)
                    callback(false,webRequest.error)
                else
                    logic.debug.Log('上传成功:'..webRequest.downloadHandler.text)
                    local imgUrl = url..'/'..data.file_name
                    callback(true,imgUrl)
                end
            end)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            callback(false)
        end
    end)
end


---@param storyInfo StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
function UIStoryEditorMgr:ReadingOtherBook(bookID)
    logic.gameHttp:StoryEditor_GetBookDetail(bookID,function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then

            ---@type StoryEditor_BookDetials
            local storyInfo = json.data
            storyInfo.roleTable = DataDefine.t_RoleTable.New()
            storyInfo.roleTable:FromTable(core.json.Derialize(storyInfo.role_list))
            storyInfo.role_list = nil
            -- local roleJson = storyInfo.roleTable:ToJson()
            -- storyInfo.roleTable.md5 = string.getMd5(roleJson)

            storyInfo.UpdateTags = function(self)
                self.tagArray = System.Collections.SortedDictionary.New()
                local array = string.split(self.tag,',')
                for i,v in pairs(array) do
                    local val = tonumber(v)
                    self.tagArray:Set(val,true)
                end
            end
            storyInfo:UpdateTags()
            
            local uiView = logic.UIMgr:Open(logic.uiid.UIStoryChapterForm)
            uiView:SetData(storyInfo)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            --logic.cs.UINetLoadingMgr:Close()
        end
    end)
end


---@param storyDetial StoryEditor_BookDetials
---@param chapterData StoryEditor_ChapterDetial
function UIStoryEditorMgr:ReadingOtherChapter(bookID, chapterID,onPreviewClose)
    
    --logic.cs.UINetLoadingMgr:Show()
    logic.gameHttp:StoryEditor_GetBookDetail(bookID,function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then

            ---@type StoryEditor_BookDetials
            local storyDetial = json.data
            storyDetial.roleTable = DataDefine.t_RoleTable.New()
            storyDetial.roleTable:FromTable(core.json.Derialize(storyDetial.role_list))
            storyDetial.role_list = nil
            -- local roleJson = storyDetial.roleTable:ToJson()
            -- storyDetial.roleTable.md5 = string.getMd5(roleJson)

            storyDetial.UpdateTags = function(self)
                self.tagArray = System.Collections.SortedDictionary.New()
                local array = string.split(self.tag,',')
                for i,v in pairs(array) do
                    local val = tonumber(v)
                    self.tagArray:Set(val,true)
                end
            end
            storyDetial:UpdateTags()
            
            logic.StoryEditorMgr:GetDialogList(storyDetial.id, chapterID,function(storyTable)
                --logic.cs.UINetLoadingMgr:Close()
                local timestamp = socket.gettime()
                logic.StoryEditorMgr:EnterStoryEditorMode()
                local uiView = logic.UIMgr:Open(logic.uiid.Story_Preview)
                local storyNodeRoot = DataDefine.t_StoryNode.Create(storyTable)
                storyNodeRoot.name = storyDetial.title
                timestamp = socket.gettime() - timestamp
                logic.debug.Log(string.format('json解析耗时%.3fs',timestamp))

                timestamp = socket.gettime()
                uiView:SetData(storyDetial, chapterID, storyNodeRoot, true)
                uiView.onClose = onPreviewClose
                timestamp = socket.gettime() - timestamp
                logic.debug.Log(string.format('打开界面耗时%.3fs',timestamp))
            end)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            --logic.cs.UINetLoadingMgr:Close()
        end
    end)
end

function UIStoryEditorMgr:SetCover(uiImage,sprite,defaultSize)
    uiImage.sprite = sprite
end

function UIStoryEditorMgr:SetSprite(uiImage,sprite,defaultSize)
    local size = defaultSize
    uiImage.sprite = sprite
    --获取image大小
    if logic.IsNull(sprite) then
        size = defaultSize
    else
        if not sprite.rect then
            logic.debug.LogError(sprite)
            size = defaultSize
        else
            local imgSize = sprite.rect.size
            imgSize = core.Vector2.New(imgSize.x, imgSize.y) / uiImage.pixelsPerUnit --图片真实分辨率

            local imgRatio = (imgSize.x / imgSize.y)    --图片比例
            local ratio = (defaultSize.x / defaultSize.y)   --图片框比例
            if imgRatio > ratio then    --x超出框体，以x为基准
                imgSize.x = defaultSize.x
                imgSize.y = defaultSize.x / imgRatio
            else    --y超出框体，以y为基准
                imgSize.y = defaultSize.y
                imgSize.x = defaultSize.y * imgRatio
            end
            size = imgSize
        end
    end
    local t = uiImage.transform
    logic.cs.LuaHelper.SetUISize(t, size.x, size.y)
    return size
end

function UIStoryEditorMgr:ParseImageUrl(url)
    local imgUrl = ''
    local md5 = ''
    local idx = -1
    if not string.IsNullOrEmpty(url) then
        for i = #url, 1, -1 do
            local c = string.sub(url, i,i)
            if c == '?' then
                idx = i
                break
            end
        end
        if idx > 0 then
            md5 = string.sub(url,idx + 1)
            imgUrl = string.sub(url,1,idx-1)
        else
            md5 = ''
            imgUrl = url
        end
    end
    return imgUrl,md5
end

---@param chapterData StoryEditor_ChapterDetial
function UIStoryEditorMgr:ModifyChapter(book_id,chapter_number,title,description, callback)
    logic.gameHttp:StoryEditor_ModifyChapter(
        book_id, 
        chapter_number,
        title,
        description,
        function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                callback()
            else
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
                --logic.cs.UINetLoadingMgr:Close()
            end
    end)
end

function UIStoryEditorMgr:FinishReadingChapter(bookID, chapterID,callback)
    logic.gameHttp:StoryEditor_FinishReadingChapter(bookID, chapterID, function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            callback(json.data.max_finish_chapter_time)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            --logic.cs.UINetLoadingMgr:Close()
        end
    end)
end

---@param pay_type 支付类型: 0只是保存进度 1钥匙支付 2观看广告 3倒计时
function UIStoryEditorMgr:SaveReadingRecord(bookID, chapterID, pay_type,callback)
    logic.gameHttp:StoryEditor_SaveReadingRecord(bookID, chapterID, pay_type, function(result)
        local jsonData = core.json.Derialize(result)
        local code = tonumber(jsonData.code)
        if code == 200 then
            
            if jsonData and jsonData.data then
                if jsonData.data.user_key then
                    logic.cs.UserDataManager:ResetMoney(1, jsonData.data.user_key)
                end
                if jsonData.data.user_diamond then
                    logic.cs.UserDataManager:ResetMoney(2, jsonData.data.user_diamond)
                end
            end

            callback(true)
        else
            logic.cs.UIAlertMgr:Show("TIPS",jsonData.msg)
            callback(false)
        end
    end)
end

function UIStoryEditorMgr:SaveFavorite(bookID, isFavorite, callback)
    logic.gameHttp:StoryEditor_SaveFavorite(bookID, isFavorite, function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            callback(true, json.data.status == 1)
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            callback(false)
        end
    end)
end

return UIStoryEditorMgr.New()