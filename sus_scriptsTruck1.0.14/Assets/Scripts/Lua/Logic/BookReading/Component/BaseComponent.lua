local Class = core.Class

---@class BaseComponent
local BaseComponent = Class("BaseComponent")

function BaseComponent:TEMP()
    self.cfg = pb.t_BookDialog
end

function BaseComponent:__init(index, cfg)
    self.index = index
    self.cfg = cfg
    --logic.debug.LogError(tostring(index)..' '..tostring(cfg))
    self.cfg.dialogID = self.cfg.dialogID or 0
    self.cfg.chapterID = self.cfg.chapterID or 0
    self.cfg.sceneID = self.cfg.sceneID or ''
    self.cfg.role_id = self.cfg.role_id or 0
    self.cfg.icon = self.cfg.icon or 0
    self.cfg.phiz_id = self.cfg.phiz_id or 0
    self.cfg.icon_bg = self.cfg.icon_bg or 0
    self.cfg.dialog_type = self.cfg.dialog_type or 0
    self.cfg.tips = self.cfg.tips or ''
    self.cfg.BGMID = self.cfg.BGMID or 0
    self.cfg.Scenes_X = self.cfg.Scenes_X or 0
    self.cfg.dialog = self.cfg.dialog or ''
    self.cfg.trigger = self.cfg.trigger or 0
    self.cfg.next = self.cfg.next or 0
    self.cfg.selection_num = self.cfg.selection_num or 0
    self.cfg.Orientation = self.cfg.Orientation or 0
    self.cfg.SceneParticalsArray = self.cfg.SceneParticalsArray or {}

    self.isPhoneCallMode = (self.cfg.PhoneCall == 1)
end

--收集所用到的资源
function BaseComponent:CollectRes(resTable)
    --bgm
    if self.cfg.BGMID and self.cfg.BGMID ~= 0 then
        resTable[logic.bookReadingMgr.Res.bookFolderPath .. "Music/BGM/" .. self.cfg.BGMID .. ".mp3"] = BookResType.BookRes
    end
    --背景
    if self.cfg.sceneID then
        resTable[self.cfg.sceneID] = BookResType.WebImage
    end

    --背景特效
    if self.cfg.SceneParticalsArray ~= nil then
        local sceneParticalArray = self.cfg.SceneParticalsArray
        --logic.debug.Log(type(sceneParticalArray))
        if type(sceneParticalArray) == 'table' then
            for i = 1, #sceneParticalArray do
                resTable[logic.bookReadingMgr.Res.bookFolderPath .. "UIParticle/" .. sceneParticalArray[i] .. ".prefab"] = BookResType.BookRes
            end
        else
            logic.debug.LogError(string.format('填的什么鬼:dialogID=%d,sceneParticalArray=%s', self.cfg.dialogID, sceneParticalArray))
        end
    end

    --彩蛋
    resTable["Assets/Bundle/UI/BookReading/Eggs.prefab"] = BookResType.UIRes
    --if self.cfg.eggId and #self.cfg.eggId > 0 then
    --    resTable["Assets/Spine/Spine/eggs_SkeletonData.asset"] = BookResType.BookRes
    --end

end

function BaseComponent:Clean()

end

function BaseComponent:OnSceneClick()
end

function BaseComponent:Play()
    for k, v in pairs(logic.DialogType) do
        if (v + 1) == self.cfg.dialog_type then
            logic.debug.LogError(string.format('DialogID=%d,未实现DialogType = %d(%s),class=%s', self.cfg.dialogID, self.cfg.dialog_type, k, self.__cname))
            return
        end
    end
    logic.debug.LogError(string.format('DialogID=%d, 未实现DialogType = %d,class=%s', self.cfg.dialogID, self.cfg.dialog_type, self.__cname))
end

--- @param component BaseComponent @将要播放的component
function BaseComponent:OnPlayEnd(nextComponent)
end


--region 播放下一段对话

function BaseComponent:ShowNextDialog()
    if logic.bookReadingMgr:CheckChapterComplete(self.cfg.dialogID) then
        return
    end
    local nextID = self:GetNextDialogID()
    if nextID == nil then
        nextID = self.cfg.dialogID + 1
        local bookData = logic.bookReadingMgr.bookData
        CS.UnityEngine.Debug.LogError(string.format('BookID=%d, DialogID=%d, trigger = %d,class=%s,下一个ID为空',
                bookData.BookID,
                self.cfg.dialogID,
                self.cfg.trigger,
                self.__cname)
        )
    end
    if nextID == -1 then
        logic.debug.logError('nextID is -1')
        return
    end
    logic.bookReadingMgr:PlayById(nextID)
end

function BaseComponent:GetNextDialogID()
    local bookData = logic.bookReadingMgr.bookData
    local id = self.cfg.next
    if self.cfg.ConsequenceID and self.cfg.ConsequenceID > 0 then
        local recordSelIndex = logic.cs.UserDataManager:GetBookOptionSelectIndex(bookData.BookID, self.cfg.ConsequenceID)
        if recordSelIndex > 0 then
            id = self:GetNextDialogIDBySelection(recordSelIndex - 1);
        end

        local cfg = logic.cs.BookReadingWrapper:GetDialogById(self.cfg.ConsequenceID);
        if cfg and cfg.modelid ~= 0 then
            local outfitId = logic.bookReadingMgr.bookData.outfit_id
            local roleModel = logic.bookReadingMgr.Res:GetRoleMode(cfg.modelid)
            local clothesIDs = roleModel.outfit_type3
            for k, v in pairs(clothesIDs) do
                if v == outfitId then
                    id = self:GetNextDialogIDBySelection(k - 1);
                    break
                end
            end
        end
    end

    if self.cfg.propertyCheck and self.cfg.propertyCheck > 0 then
        for i = 1, self.cfg.selection_num do
            local personalistStr = self.cfg['Personalit_' .. i]
            if string.IsNullOrEmpty(personalistStr) then
                return
            end

            personalistStr = string.gsub(personalistStr, '##', '_')
            local infoList = string.split(personalistStr, '_')

            local conditionIsConform = false
            for j = 1, #infoList do
                local str = infoList[j]
                if string.IsNullOrEmpty(str) then
                    goto continue
                end
                conditionIsConform = true;
                local keyValue = string.split(str, ':')
                if #keyValue < 2 then
                    goto continue
                end
                local key = tonumber(keyValue[1])
                local value = tonumber(keyValue[2])
                if logic.cs.UserDataManager:GetPropertyValueByType(bookData.BookID, key) < value then
                    conditionIsConform = false  --条件不符合时，退出循环
                    break ;
                end
                :: continue ::
            end
            if conditionIsConform then
                id = self:GetNextDialogIDBySelection(i - 1);
                break
            end
        end
    end
    return id
end

function BaseComponent:GetNextDialogIDBySelection(idx)
    if idx == 0 then
        return self.cfg.next_1
    elseif idx == 1 then
        return self.cfg.next_2
    elseif idx == 2 then
        return self.cfg.next_3
    elseif idx == 3 then
        return self.cfg.next_4
    end
end

function BaseComponent:IsDanmakuTrigger()
    return self.cfg.Barrage == 1
end

--endregion


return BaseComponent