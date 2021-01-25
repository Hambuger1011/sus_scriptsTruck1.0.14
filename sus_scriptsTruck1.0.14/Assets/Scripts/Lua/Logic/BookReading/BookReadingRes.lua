local Class = core.Class
local BookReadingRes = Class("BookReadingRes")

BookReadingRes.AudioTones =
{
	book_click = 0,             --点击书本
	dialog_click = 1,           --点击对话的音效
	diamond_click = 2,          --点击钻石的音效
	dialog_choice_click = 3,    --选择选项的音效
	RewardWin = 4,          --成功音效
	LoseFail = 5,           --失败音效
	--RouletteSpin = 6,       --转盘启动的音效
}

BookResType = {
	BookRes = 1,	--书本资源
	UIRes = 2,		--通用资源
	WebImage = 3,		--web下载
}

function BookReadingRes:TEMP()
	self.bookDetailCfg =  pb.t_BookDetails
end

function BookReadingRes:__init()
	self.useResMap = {}
	self.cacheResAsset = {}
	self.cacheWebImage = {}
end

function BookReadingRes:__delete()
	
end

function BookReadingRes:SetData(bookID,chapterID)
	self.AbBookSystem = CS.AB.AbBookSystem.Create(bookID)
	self.lastBookID = self.bookID or 0
	self.bookID = bookID
	self.chapterID = chapterID
	self.bookFolderPath = string.format("Assets/Bundle/Book/%d/",self.bookID)
	self.bookCommonPath = string.format("Assets/Bundle/Book/%d/%s/",self.bookID,self:GetChapterFolderId(true))
end

function BookReadingRes:InitAbSystem(callback)
	self.AbBookSystem:InitSys(callback)
end

function BookReadingRes:GetChapterFolderId(isCommon)
	if isCommon then
		return 'common'
	else
		return "Chapter_"..self.chapterID
	end
end

function BookReadingRes:Begin()
	self.isBegin = true
end

function BookReadingRes:End()
	self.isBegin = false
end

function BookReadingRes:GetLoadResProgress()
	if(self.isBegin==nil or self.isBegin==false)then
		return 0
	end
	local p = 0
	for i,v in pairs(self.cacheResAsset) do
		p = p + v:Progress()
	end
	p = p / table.count(self.cacheResAsset)
	return p
end



local locks=0;
function BookReadingRes:GetCurNum()
	if(self.isBegin==nil or self.isBegin==false)then
		return 0
	end
	local p = 0
	for i,v in pairs(self.cacheResAsset) do

		local progre=v:Progress();
		if(progre>=1)then
			p = p + 1;
		end
	end
	locks=locks+1;
	return p
end


local isALl=false;
local allcount=0;

function BookReadingRes:GetAllNum()
	if(isALl==true and allcount > 0)then
		return allcount;
	else
		return 0;
	end
end





local isALl=false;
local allsize=0;
function BookReadingRes:GetAllSize()
	if(self.isBegin==nil or self.isBegin==false)then
		return 0
	end
	if(isALl==true)then
		return allsize;
	else
		return 0;
	end
	return allsize
end




function BookReadingRes:IsLoadResDone()
	if(self.isBegin==nil or self.isBegin==false)then
		return false
	end
	
	
	if(self.cacheResAsset==nil)then
		return false;
	end
	local len = table.count(self.cacheResAsset);
	if(len==0)then
		return false;
	end

	local isDone = true
	for i,v in pairs(self.cacheResAsset) do
		if v:IsDone() then
			if v.DoneCallback ~= nil then
				v:DoneCallback()
			end
		else
			isDone = false
			break
		end
	end
	return isDone
end

function BookReadingRes:DownloadByList(resTable)
	allsize = 0;
	for url,resType in pairs(resTable) do
		local asset = self:Download(logic.ResMgr.type.Object,url,resType)
		if  asset ~= nil then
			allsize=allsize+asset:GetAllSize();
		end
	end
	isALl=true
end

---@param bookResType 1:书本资源,2:UI资源
function BookReadingRes:Download(abResType,assetName,bookResType,callback)
	local asset = nil
	--logic.debug.LogError('Download:'..assetName)
	if bookResType == BookResType.BookRes then
		asset = self.AbBookSystem:LoadAsync(logic.ResMgr.tag.DialogDisplay,abResType,assetName, callback)
	elseif bookResType == BookResType.UIRes then
		asset = logic.ResMgr.LoadAsync(logic.ResMgr.tag.DialogDisplay,abResType,assetName, callback)
	elseif bookResType == BookResType.WebImage then
		asset = logic.cs.ABSystem.ui:DownloadBookSceneBG(self.bookID,assetName,function(id,refCount)
			if self.bookID ~= id then
				refCount:Release()
				return
			end
			self.cacheWebImage[assetName] = refCount
		end)
	else
		logic.debug.LogError('位置资源类型:'..tostring(bookResType))
	end
	if not asset then
		logic.debug.LogError(string.format('预加载失败:type=%s,filename=%s,type=%d',abResType,assetName,bookResType))
		return nil
	end
	asset:Retain(self.__cname)
	self.cacheResAsset[assetName] = asset
	self:Retain(assetName)
	return asset
end

function BookReadingRes:Retain(assetName)
	if self.useResMap[assetName] then
		self.useResMap[assetName] = self.useResMap[assetName] + 1
	else
		self.useResMap[assetName] = 1
	end
end

function BookReadingRes:Release(useResList)
	--logic.debug.LogError("使用资源count:"..tostring(#useResList))
	--计数重置
	for k,_ in pairs(self.useResMap) do
		self.useResMap[k] = 0
	end
	
	--使用计数
	if useResList then
		for k,resType in pairs(useResList) do
			self.useResMap[k] = resType
			-- if k == 'assets/bundle/book/20004/SceneBG/1.png' then
			-- 	logic.debug.LogError("jjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjjj")
			-- end
		end
	end
	
	--释放未使用
	for k,v in pairs(self.useResMap) do
		if v == 0 then
			local asset = self.cacheResAsset[k]
			if not asset then
				goto continue
			end
			self.cacheResAsset[k] = nil
			asset:Release(self.__cname)
			--logic.debug.Log(string.format('[%s]释放:%s',self.__cname,k))
			-- if k == 'assets/bundle/book/20004/SceneBG/1.png' then
			-- 	logic.debug.LogError("222222222222222222222222222222222222222222")
			-- end
			
			--销毁图片
			local refCount = self.cacheWebImage[k]
			if refCount then
				self.cacheWebImage[k] = nil
				refCount:Release()
			end
		end
		::continue::
	end
	logic.ResMgr.ClearTag(logic.ResMgr.tag.DialogDisplay)
	logic.ResMgr.GC()
	local saveResCnt = table.count(self.cacheResAsset)
	logic.debug.LogError(string.format('[%s]保留资源:%d',self.__cname,saveResCnt))
	if self.lastBookID ~= self.bookID then
		for k,v in pairs(self.cacheResAsset) do
			logic.debug.LogError(k..'\n'..tostring(v))
		end
	end
end


function BookReadingRes:LoadCoverImage(callback)
	logic.cs.ABSystem.ui:DownloadBookCover(self.bookID,function(id,refCount)
		if self.bookID ~= id then
			refCount:Release()
			return
		end
		callback(refCount)
	end)
    --useResTable[self.bookCommonPath ..'Cover/0001.png'] = BookResType.WebImage
	--local filename = self.bookCommonPath ..'Cover/0001.png'
	--self:Download(logic.ResMgr.type.Sprite,filename,1,callback)
end

function BookReadingRes:LoadBookConfig(url,callback)
	self:LoadRoleModelData()
	--加载配置
	local coLoadDialogData = function(url,callback)
		logic.debug.Log("bookUrl:"..url)
		
		
		local doInitBook = function(bytes)
			logic.cs.BookReadingWrapper:LoadBookConfig(bytes)
			local cfg = core.Protobuf.pb.decode('pb.t_BookDialog_List',bytes)
			for _,item in pairs(cfg.items) do
				if not item.SceneParticalsArray then
					local json = item.SceneParticals
					item.SceneParticalsArray = core.json.Derialize(json)
					if not item.SceneParticalsArray then
						logic.debug.LogError(string.format('json解析错误:%s',json))
					end
				end
			end
			self.dialogItems = cfg.items
			--logic.debug.LogError("count:"..tostring(#cfg.items))
			self.bookDetailCfg = logic.cs.JsonDTManager:GetJDTBookDetailInfo(self.bookID)
			callback(cfg.items)
		end
		
		if string.IsNullOrEmpty(url) or not logic.config.isAbMode then --url为空或debug模式，直接加载本地
			local filename = string.format("%s%d/t_BookDialog_%d_%d.bytes",CS.XlsxData.BOOK_DIR, self.bookID, self.bookID, self.chapterID);
			logic.ResMgr.LoadAsync(logic.ResMgr.tag.DialogDisplay,logic.ResMgr.type.Text,filename,function(asset)
					if asset then
						doInitBook(asset.resTextAsset.bytes)
						asset:Release()
					else
						logic.debug.LogError("加载配置错误:" .. filename)
					end
				end)
			return
		end
		
		local req = logic.cs.UnityWebRequest.Get(url)
		local async = req.SendWebRequest()
		while(true) do
			if not req.isDone then
				core.coroutine.step(1)
				goto continue
			end
			if (req.isNetworkError or req.isHttpError or not string.IsNullOrEmpty(req.error)) then
				logic.debug.LogError("加载配置失败:" .. req.error)
				return
			end
			
			local buff = req.downloadHandler.data;
			req:Dispose()
			doInitBook(buff);
			::continue::
		end
	end
	core.coroutine.start(coLoadDialogData,url,callback)
end


--region Audio
function BookReadingRes:StopBGM()
	logic.cs.AudioManager:StopBGM();
end

function BookReadingRes:StopBGMQuick()
	logic.cs.AudioManager:StopBGMQuick();
end

function BookReadingRes:PlayTones(audioTones)
	logic.cs.AudioManager:PlayTones(audioTones);
end

function BookReadingRes:StopTones(audioTones)
	logic.cs.AudioManager:StopTones(audioTones);
end

function BookReadingRes:PlayBGM(bookID, BGMID, isCommon)
	--Debug.Log("-----bookID---" + bookID + "---BGM ID--" + BGMID);
	if (string.IsNullOrEmpty(BGMID)) then
		logic.cs.AudioManager:PlayBGM(nil)
	else
		if (logic.cs.UserDataManager.UserData.BgMusicIsOn == 0) then
			return
		end
		local asset = self.AbBookSystem:LoadImme(logic.ResMgr.tag.DialogDisplay, logic.ResMgr.type.Audio, self.bookFolderPath .. "Music/BGM/" .. BGMID .. ".mp3")
		logic.cs.AudioManager:PlayBGM(asset and asset.resAudioClip or nil);
	end
end


function BookReadingRes:PlayBgmByClip(clip)
	if (logic.cs.UserDataManager.UserData.BgMusicIsOn == 0) then
		return
	end
	logic.cs.AudioManager:PlayBGM(clip)
end


function BookReadingRes:StartReading()
	logic.bookReadingMgr.context.DescriptionColor = logic.cs.LuaHelper.ParseHtmlString("#424242FF")
	logic.bookReadingMgr.context.DialogColor = logic.cs.LuaHelper.ParseHtmlString("#424242FF")
end

--endregion

function BookReadingRes:GetPrefab(key)
	local asset = self.cacheResAsset[key]
	if not asset or not asset.resPrefab then
		logic.debug.LogError("资源未预加载:"..key)
		return
	end
	return asset.resPrefab
end


function BookReadingRes:GetSprite(name, isCommon)
	local asset = nil
	if isCommon then
		asset = self.AbBookSystem:LoadImme(logic.ResMgr.tag.DialogDisplay,logic.ResMgr.type.Sprite,self.bookCommonPath..name..'.png')
	else
		asset = self.AbBookSystem:LoadImme(logic.ResMgr.tag.DialogDisplay,logic.ResMgr.type.Sprite,self.bookFolderPath..name..'.png')
	end
	if not asset then
		return
	end
	if not asset.resSprite then
		logic.debug.LogError('获取sprite失败:'..name)
		return
	end
	return asset.resSprite
end

function BookReadingRes:GetSceneBG(sceneID)
	local item = self.cacheWebImage[sceneID]
	local spt = item and item:GetObject()
	if IsNull(spt) then
		logic.debug.LogError(string.format('没有背景图bookID:%d,chapterID:%d,sceneID:%s',self.bookID,self.chapterID,sceneID))
		return nil
	end
	return spt
end

function BookReadingRes:GetSkeDataAsset(key)
	local asset = self.AbBookSystem:LoadImme(logic.ResMgr.tag.DialogDisplay,logic.ResMgr.type.ScriptableObject,self.bookFolderPath..key..'_SkeletonData.asset')
	if not asset or not asset.resObject then
		logic.debug.LogError("资源未预加载:"..key)
		return nil
	end
	return asset.resObject
end

function BookReadingRes:GetSkeletonDataAsset(key)
	local asset = self.AbBookSystem:LoadImme(logic.ResMgr.tag.DialogDisplay,logic.ResMgr.type.ScriptableObject,key)
	if not asset or not asset.resObject then
		logic.debug.LogError("SkeletonData资源未预加载:"..key)
		return nil
	end
	return asset.resObject
end

function BookReadingRes:LoadRoleModelData()
	if self.roleModel ~= nil then
		return
	end
	self.roleModel = {}
	self.roleModelData = {}

	--local list = logic.DataManager.client:GetCfgList('t_RoleModel')
	--for i,item in pairs(list) do
	--	self.roleModel[item.model_id] = item
	--end
	--list = logic.DataManager.client:GetCfgList('t_RoleModelData')
	--for i,item in pairs(list) do
	--	item.price = item.price or 0
	--	local key = string.format('%d|%d|%d',item.book_id, item.type, item.item_id)
	--	self.roleModelData[key] = item
	--end
end

function BookReadingRes:GetRoleMode(modelID)
	---@type t_RoleModel
	local roleModel = logic.bookReadingMgr.Res.roleModel[modelID]
	if roleModel == nil then
		logic.debug.LogError("not found modelID:"..tostring(modelID))
		return nil
	end
	return roleModel
end

function BookReadingRes:GetRoleModelData(type, item_id)
	local book_id = logic.bookReadingMgr.bookData.BookID
	local key = string.format('%d|%d|%d',book_id, type, item_id)
	local data = self.roleModelData[key]
	if data == nil then
		logic.debug.LogError("not found roleModelData:"..key)
	end
	return data
end

return BookReadingRes.New()