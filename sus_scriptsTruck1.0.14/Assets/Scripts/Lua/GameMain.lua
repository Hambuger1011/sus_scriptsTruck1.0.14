-- 全局模块
require "Global"
local ResMgr = core.ResMgr
local Time = core.Time
	
-- 定义为全局模块，整个lua程序的入口类
GameMain = {}
local this = GameMain


-- 场景切换通知
function GameMain.OnLevelWasLoaded(level)
	collectgarbage("collect")
	Time.timeSinceLevelLoad = 0
end

function GameMain.OnApplicationQuit()
end

function GameMain.Startup()
	--CS.AB.AbUtility.loadType = CS.AB.enLoadType.eWebUnity;
	--CS.AB.AbTask.MAX_BUNDLE_ASYNC_NUM = Mathf.Min(CS.AB.AbTask.MAX_BUNDLE_ASYNC_NUM,8)
	--CS.AB.AbTask.MAX_ASSET_ASYNC_NUM = 4
	logic.sceneMgr:GotoScene(logic.SceneConfig.LaunchScene)
	--CS.AB.ABSystem.Instance:LoadAsync(logic.ResMgr.tag.Global,logic.ResMgr.type.Prefab,"",function(task)end)
end


function GameMain.StartReading(isContinue,strBookurl)
	local bookID = logic.cs.BookReadingWrapper.BookID
	logic.bookReadingMgr:StartReading(strBookurl)
	-- --logic.cs.UINetLoadingMgr:Show()
	-- logic.gameHttp:GetBookDetailInfo(bookID,function(result)
	-- 	--logic.cs.UINetLoadingMgr:Close()
	-- 	logic.debug.LogError('-----bookID='..bookID)
	-- 	local json = core.json.Derialize(result)
	-- 	local code = tonumber(json.code)
	-- 	if code == 200 then
	-- 		---type BookData
	-- 		local bookData = logic.cs.BookReadingWrapper.CurrentBookData
	-- 		local userlog = json.data.userlog
	-- 		if userlog then
	-- 			bookData.IsPhoneCallMode = (userlog.phonesceneid==1)
	-- 			-- if userlog.bookid ~= 0 then
	-- 			-- 	bookData.BookID = userlog.bookid
	-- 			-- end
	-- 			if type(userlog.role_name) == 'number' and userlog.clothid ~= 0 then
	-- 				bookData.PlayerClothes = userlog.clothid
	-- 			end
	-- 			if type(userlog.role_name) == 'number' and userlog.chapterid ~= 0 then
	-- 				bookData.ChapterID = userlog.chapterid
	-- 			end
	-- 			if type(userlog.role_name) == 'number' and userlog.dialogid ~= 0 then
	-- 				bookData.DialogueID = userlog.dialogid
	-- 			end
	-- 			if type(userlog.role_name) == 'number' and userlog.phoneroleid ~= 0 then
	-- 				bookData.PhoneRoleID = userlog.phoneroleid
	-- 			end

	-- 			--role_name value is 'null'
	-- 			if type(userlog.role_name) == 'string' and not string.IsNullOrEmpty(userlog.role_name) then
	-- 				bookData.PlayerName = userlog.role_name
	-- 			end
	-- 		end
	-- 		--bookData.Role = userlog.option
	-- 		logic.debug.LogError(bookData:ToString())
	-- 		logic.bookReadingMgr:StartReading(strBookurl)
	-- 	else
	-- 		if not string.IsNullOrEmpty(json.msg) then
	-- 			logic.cs.UITipsMgr:PopupTips(json.msg, false);
	-- 		end
	-- 	end
	-- end)
end


function GameMain.Reload(files)
	for i,v in pairs(files) do
		logic.debug.Log(v)
		package.loaded[v] = nil
	end
	for i,v in pairs(files) do
		require(v)
	end
end

return GameMain