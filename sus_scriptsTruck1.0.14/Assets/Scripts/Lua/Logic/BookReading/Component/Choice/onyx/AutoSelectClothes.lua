local Class = core.Class
local Base = logic.BookReading.BaseComponent

local AutoSelectClothes = Class("AutoSelectClothes", Base)



--收集所用到的资源
function AutoSelectClothes:CollectRes(resTable)
	Base.CollectRes(self,resTable)
	
	
	local bookData = logic.bookReadingMgr.bookData
	local bookFolderPath = logic.bookReadingMgr.Res.bookFolderPath
	local clothGroupId = 0
	local IDs = logic.bookReadingMgr.context.ClothesIDs
		local roleIDs = {}
		for id,_ in pairs(logic.bookReadingMgr.context.PlayerIDs) do
			roleIDs[id] = 1
		end
		if bookData.PlayerDetailsID and bookData.PlayerDetailsID ~= 0 then
			roleIDs[bookData.PlayerDetailsID] = 1
		end
		for id,_ in pairs(roleIDs) do
			IDs[id] = (IDs[id] and IDs[id] or {})
			for i=1,self.cfg.selection_num do
				local clothesID = tonumber(self.cfg['selection_'..i])
				if clothesID == nil then
					logic.debug.LogError(string.format('bookID=%d,chapter=%d,dialogID=%d,selection_%d填0做什么？',bookData.BookID,bookData.ChapterID,self.cfg.dialogid,i))
					goto continue
				end
				IDs[id][clothesID] = 1
				--clothGroupId = core.Mathf.Ceil(clothesID/4)
				--if clothGroupId == 0 then
				--	clothGroupId = 1
				--end
				clothGroupId = 1;
				local appearanceID = logic.bookReadingMgr:GetAppearanceID(1, 0, clothGroupId)
				resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
				::continue::
			end
		end

end


function AutoSelectClothes:Clean()
	Base.Clean(self)

end



function AutoSelectClothes:Play()
    local clothesID = tonumber(self.cfg['selection_1'])
    if self.cfg.selection_num > 0 and clothesID > 0 then
        local bookData = logic.bookReadingMgr.bookData
		bookData.PlayerClothes = clothesID
		logic.bookReadingMgr:SavePlayerClothes(clothesID)
		logic.bookReadingMgr:SaveOption(1)
        logic.bookReadingMgr:SaveProgress(function(result)
            self:SetProgressHandler(result)
        end)
    end
end


function AutoSelectClothes:SetProgressHandler(result)
    local clothesID = tonumber(self.cfg['selection_1'])
	logic.debug.Log("----SetProgressHandler---->" .. result)
	local json = core.json.Derialize(result)
	local code = tonumber(json.code) --坑，返回来的code是字符串
	if code == 200 then
		
		logic.bookReadingMgr.bookData.PlayerClothes = clothesID;
		logic.bookReadingMgr:SaveOption(1)
    	logic.bookReadingMgr:SaveProgress()
		logic.cs.UserDataManager:RecordBookOptionSelect(logic.bookReadingMgr.bookData.BookID, self.cfg.dialogid, 1);
		self:ShowNextDialog()
	else
		if not string.IsNullOrEmpty(json.msg) then
			logic.cs.UITipsMgr:PopupTips(json.msg, false);
		end
	end
end

function AutoSelectClothes:GetNextDialogID()
    local id = self:GetNextDialogIDBySelection(0);
    return id
end

return AutoSelectClothes