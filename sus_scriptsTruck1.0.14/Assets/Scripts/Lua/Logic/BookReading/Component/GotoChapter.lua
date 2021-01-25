local Class = core.Class
local Base = logic.BookReading.BaseComponent

local GotoChapter = Class("GotoChapter", Base)

--收集所用到的资源
function GotoChapter:CollectRes(resTable)
    Base.CollectRes(self,resTable)
	-- local bookFolderPath = logic.bookReadingMgr.Res.bookFolderPath
    -- resTable[bookFolderPath..'RoleHead/'..'.png'] = BookResType.BookRes
end

function GotoChapter:Clean()
	Base.Clean(self)
end

function GotoChapter:Play()
	local bookData = logic.bookReadingMgr.bookData
    local bookID = bookData.BookID
    local chapterID = tonumber(self.cfg.selection_1)
    local dialogID = tonumber(self.cfg.selection_2)

	local doGoto = function()
		logic.gameHttp:GoBackStep(bookID,chapterID,dialogID, function(result)
			--logic.cs.UINetLoadingMgr:Close()
			local json = core.json.Derialize(result)
			local code = tonumber(json.code)
			if code == 200 then
				local chapterid= tonumber(json.data.chapterid)
				local dialogid = tonumber(json.data.dialogid)
				
				if chapterid == bookData.ChapterID then	--同一章节里
					logic.bookReadingMgr:PlayById(dialogid)
				else
					local chapterInfo = logic.cs.JsonDTManager:GetJDTChapterInfo(bookID,chapterid+1);
					local beginDialogID = 1;
					local endDialogID = beginDialogID
					if chapterInfo ~= nil then
						beginDialogID = chapterInfo.chapterstart
						endDialogID = chapterInfo.chapterfinish
					end
					
					if dialogid < beginDialogID then
						dialogid = beginDialogID
					end
					if dialogid > endDialogID then
						dialogid = endDialogID
					end
		
					logic.bookReadingMgr.isReading = false
					logic.cs.BookReadingWrapper:InitByBookID(
						bookID,
						chapterid,
						dialogid,
						beginDialogID,
						endDialogID
					)
					logic.cs.BookReadingWrapper:PrepareReading(true)
				end
			else
				logic.cs.UIAlertMgr:Show("TIPS",json.msg)
			end
		end)
	end

	--logic.cs.UINetLoadingMgr:Show(-1)
	logic.bookReadingMgr:SaveProgress(function(result)
		local json = core.json.Derialize(result)
		local code = tonumber(json.code)
		if code == 200 then
			doGoto()
		else
			--logic.cs.UINetLoadingMgr:Close()
			logic.cs.UIAlertMgr:Show("TIPS",json.msg)
		end
	end)
end

return GotoChapter