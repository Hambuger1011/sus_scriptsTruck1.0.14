local Class = core.Class
local Base = logic.BookReading.BaseComponent

local AutoSelectClothesModel = Class("AutoSelectClothesModel", Base)

--收集所用到的资源
function AutoSelectClothesModel:CollectRes(resTable)
	Base.CollectRes(self,resTable)
	local bookFolderPath = logic.bookReadingMgr.Res.bookFolderPath
	local clothGroupId = 0
	local modelId = tonumber(self.cfg.modelid)
	local roleModel = logic.cs.JsonDTManager:GetJDTRoleModel(logic.bookReadingMgr.bookData.BookID,modelId)
	local clothesID = roleModel.outfit_type3[0]
	if clothesID == nil then
		logic.debug.LogError('clothesID为空')
		return
	end
	clothGroupId = core.Mathf.Ceil(tonumber(clothesID)/4)
	if clothGroupId == 0 then
		clothGroupId = 1
	end
	clothGroupId = 1
	local appearanceID = logic.bookReadingMgr:GetAppearanceID(1, 0, clothGroupId)
	resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
end


function AutoSelectClothesModel:Clean()
	Base.Clean(self)
end

function AutoSelectClothesModel:Play()
    local modelId = tonumber(self.cfg.modelid)
	local roleModel = logic.cs.JsonDTManager:GetJDTRoleModel(logic.bookReadingMgr.bookData.BookID,modelId)
	local clothesID = roleModel.outfit_type3[0]
    if tonumber(clothesID) > 0 then
        local bookData = logic.bookReadingMgr.bookData
		bookData.PlayerClothes = clothesID
		bookData.outfit_id = clothesID
		logic.bookReadingMgr:SavePlayerClothes(clothesID)
		logic.bookReadingMgr:SaveOutfitId(clothesID)
		logic.bookReadingMgr:SaveOption(1)
        logic.bookReadingMgr:SaveProgress(function(result)
            self:SetProgressHandler(result)
        end)
    end
end


function AutoSelectClothesModel:SetProgressHandler(result)
	logic.debug.Log("----SetProgressHandler---->" .. result)
	local json = core.json.Derialize(result)
	local code = tonumber(json.code) --坑，返回来的code是字符串
	if code == 200 then
		logic.cs.UserDataManager:RecordBookOptionSelect(logic.bookReadingMgr.bookData.BookID, self.cfg.dialogID, 1);
		self:ShowNextDialog()
	else
		if not string.IsNullOrEmpty(json.msg) then
			logic.cs.UITipsMgr:PopupTips(json.msg, false);
		end
	end
end

--function AutoSelectClothesModel:GetNextDialogID()
--    local id = self:GetNextDialogIDBySelection(0);
--    return id
--end

return AutoSelectClothesModel