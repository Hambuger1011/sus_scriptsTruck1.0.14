local Class = core.Class

local BookReadingMgr = Class("BookReadingMgr")
local componentFactory = require('Logic/BookReading/ComponentFactory')
logic.CharacterFaceExpressionChange = require('Logic/UI/UI_BookReading/CharacterFaceExpressionChange')
logic.SpineCharacter = require('Logic/BookReading/Spine/SpineCharacter')
logic.SpineEgg = require('Logic/BookReading/Spine/SpineEgg')
logic.Eggs = require('Logic/BookReading/Eggs/Eggs')
local UI = require('Logic/BookReading/UI')

function BookReadingMgr:TEMP()
    self.view = require('Logic/UI/UI_BookReading/UIBookReadingView')
    self.playingComponent = require('Logic/BookReading/Component/BaseComponent')

    ---这里只是用于代码提示
    ---@class BookData
    self.bookData = {
        BookID = 0,
        BookName='',
        ChapterID=0,
        DialogueID=0,
        PlayerName=0,
        PlayerDetailsID=0,
        PlayerClothes=0,
        IsPhoneCallMode=false,
        MaxChapterID=0,
        PhoneRoleID=0,
        ClothesStr='',
        ClothesList={},
        ChapterPayStr='',
        ChapterPayList={},
        NpcId=0,
        NpcName='',
        NpcDetailId=0,      --记录npc长什么样子的ID
        IsBubbleMode = 0,   --气泡聊天的模式
        chaptersCount = 0,
        character_id = 0, --model选择形象id ,需要选择时必填
        outfit_id = 0,  --model选择服装id,需要选择时必填
        hair_id = 0,    --model选择头发id,需要选择时必填
    }
end

function BookReadingMgr:__init()
    self.components = {} --[id,component]
    self.Res = require('Logic/BookReading/BookReadingRes')
    self.bookData = nil
    --self:Reset()
end

function BookReadingMgr:__delete()

end

function BookReadingMgr:Reset()
    
    self.isReading = false
    self.UI = UI.New()
    self.view = nil
    self.currentBGMID = 0
    self.playingComponent = nil
    self.lastComponent = nil
    self.bookDataCache = {}
    self.isChoiceModel = false
    self.context = {
        PlayerIDs = {},
        ClothesIDs = {},
        NpcIDs = {},
        NpcDetailIDs = {},
        IsTextTween = false,
    }
    
    for idx,component in pairs(self.components) do
        component:Clean()
    end
    self.components = {}
    
    if self.debugUpdate then
        core.coroutine.stop(self.debugUpdate)
    end

    logic.cs.BookReadingWrapper:Reset()
end


function BookReadingMgr:StartReading(strBookurl)
    if self.isReading then
        logic.debug.LogError('已经在读书中')
        return
    end
    self:Reset()
    self.isReading = true
    logic.bookReadingMgr.Res:StopBGMQuick()
    logic.cs.MyBooksDisINSTANCE:SetIsPlaying(false)
    self.bookID = logic.cs.BookReadingWrapper.BookID
    self.chapterID = logic.cs.BookReadingWrapper.ChapterID
    self.bookData = logic.cs.BookReadingWrapper.CurrentBookData
    logic.debug.LogError(self.bookData:ToString())
    self.Res:SetData(self.bookID,self.chapterID)

    logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.BookDisplayForm);
    logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.MainFormTop);
    --logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.MainForm);
    --logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.BookLoadingForm);
    logic.UIMgr:Close(logic.uiid.UIMainForm);
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.LoadBook,"", "" ,tostring(self.bookID))
    logic.UIMgr:Open(logic.uiid.BookLoading)
    logic.UIMgr:Close(logic.uiid.BookReading)

    self.Res:InitAbSystem(function()
        --加cover图
        self.Res:End()
        self.Res:LoadCoverImage(function(refCount)
             local uiLoading = logic.UIMgr:GetView(logic.uiid.BookLoading)
            if(uiLoading)then
                uiLoading:SetCoverImage(refCount)
            end
            --logic.cs.UserDataManager:AddCoverImage(coverAsset)
        end)
        
        if self.laskBookID == self.bookID and self.lastChapterID == self.chapterID then
            self:StartLoadResources(self.dialogItems)
        else
            self.lastBookID = self.bookID
            self.lastChapterID = self.chapterID
            
            self.Res:LoadBookConfig('',function(cfgItems)
                self:StartLoadResources(cfgItems)
            end)
        end
    end)
end

local function stringSplit(input, delimiter)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if (delimiter=='') then return false end
    local pos,arr = 0, {}
    for st,sp in function() return string.find(input, delimiter, pos, true) end do
        table.insert(arr, string.sub(input, pos, st - 1))
        pos = sp + 1
    end
    table.insert(arr, string.sub(input, pos))
    return arr
end


function BookReadingMgr:InitComponets()
    local maxID = 0
    local minID = math.maxinteger
    self.dialogMap = {}
    
    local eggDialogList = logic.cs.UserDataManager.buyChapterResultInfo.data.egg_dialog_list
    local eggDialogArr ={}
    for k, v in pairs(eggDialogList) do
        local eggDialog = stringSplit(v.dialog_id,",")
        eggDialogArr[v.id] = eggDialog
    end
    logic.Eggs:CleanReceivedEggs()
    for idx,cfg in pairs(self.dialogItems) do
        for k1, v1 in pairs(eggDialogArr) do
            for k2, v2 in pairs(v1) do
                if tonumber(cfg.dialogID) == tonumber(v2) then
                    cfg.eggId = k1
                end
            end
        end
        self.dialogMap[cfg.dialogID] = cfg
        local component = componentFactory.Create(idx,cfg)
        --table.insert(self.components,component)
        self.components[cfg.dialogID] = component
        maxID = math.max(maxID, cfg.dialogID)
        minID = math.min(minID, cfg.dialogID)

        if cfg.dialog_type == logic.DialogType.ChoiceModel then
            self.isChoiceModel = true
        end
    end
    local bookData = self.bookData
    if(bookData and bookData.character_id and bookData.character_id ~=0)then
        self.isChoiceModel = true
    end
    logic.debug.Log('[+]InitComponets:'..minID..' - '..maxID)
end

function BookReadingMgr:StartLoadResources(dialogItems)
    self.dialogItems = dialogItems
    self:InitComponets()
    local useResTable = {}

    --加载UI BookReading
    useResTable[logic.UIMgr:GetViewClass(logic.uiid.BookReading).config.AssetName] = BookResType.UIRes
    
    --加载当前人物模型
    local clothGroupId = 0
    clothGroupId = core.Mathf.Ceil(logic.bookReadingMgr.bookData.outfit_id/4)
    if clothGroupId == 0 then
        clothGroupId = 1
    end
    clothGroupId = 1
    local appearanceID = logic.bookReadingMgr:GetAppearanceID(1, 0, clothGroupId)
    logic.debug.Log("加载当前人物资源-----------》"..appearanceID..'_SkeletonData.asset')
    useResTable[logic.bookReadingMgr.Res.bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes

    for k,v in pairs(self.components) do
        local component = v
        component:CollectRes(useResTable)
    end
    self.Res:Release(useResTable)
    self.Res:DownloadByList(useResTable)
    self.Res:Begin()
end

function BookReadingMgr:GetLoadResProgress()
    return self.Res:GetLoadResProgress()
end

function BookReadingMgr:GetAllSize()
    return self.Res:GetAllSize()
end


function BookReadingMgr:IsLoadResDone()
    return self.Res:IsLoadResDone()
end


function BookReadingMgr:DoStartReading()
    logic.cs.BookReadingWrapper:EnableTouchEffect()
    self.Res:End()
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.ReadBook, "", "",tostring(self.bookID))
    self.view = logic.UIMgr:Open(logic.uiid.BookReading)
    -- CUIManager.Instance.OpenForm(logic.cs.UIFormName.BookReadingForm);
    -- var uiBookReadingForm = CUIManager.Instance.GetForm<BookReadingForm>(logic.cs.UIFormName.BookReadingForm);
    -- uiBookReadingForm.PrepareReading();
    --self:InitSystem()
    self.view:StartReading()
    self.Res:StartReading()
    if self.bookData.DialogueID < logic.cs.BookReadingWrapper.BeginDialogID 
    or self.bookData.DialogueID > logic.cs.BookReadingWrapper.EndDialogID then
        logic.debug.LogError(string.format('错误id=%d,有效范围[%d-%d],bookID=%d,chapterID=%d',
        self.bookData.DialogueID, 
        logic.cs.BookReadingWrapper.BeginDialogID,logic.cs.BookReadingWrapper.EndDialogID,
        self.bookData.BookID, self.bookData.ChapterID
    ))
        self.bookData.DialogueID = Mathf.Clamp(
            self.bookData.DialogueID,
            logic.cs.BookReadingWrapper.BeginDialogID,
            logic.cs.BookReadingWrapper.EndDialogID
        )
    end
    local component = self:GetComponentByID(self.bookData.DialogueID)
    self.view:ChangeSceneWithoutTween(component)
    self:PlayById(self.bookData.DialogueID)
    self.debugUpdate = core.coroutine.start(function()
        while true do
            self:DebugUpdate()
            core.coroutine.step()
        end
    end)
    self.mCurClickOperationTime = core.Time.time
    logic.cs.MyBooksDisINSTANCE:SetIsPlaying(true)


    --【阅读在线时长 记录】
    if(Cache.ReadTimeCache.finish_level < 5 and Cache.ReadTimeCache.receive_level < 5)then
        --【存入缓存】
        if(logic.cs.UserDataManager.buyChapterResultInfo and logic.cs.UserDataManager.buyChapterResultInfo.data.reading_task_info)then
            Cache.ReadTimeCache:UpdateData(logic.cs.UserDataManager.buyChapterResultInfo.data.reading_task_info);
            --【阅读在线时长 以前的记录】请求
            if(Cache.ReadTimeCache.isEND==false)then
                GameHelper.OnlineReadingtime();
            end
        end
    end

end

function BookReadingMgr:PlayByIndex(index)
    if index == table.count(self.dialogItems) then
        logic.debug.LogError('播放结束')
        return
    end
	local cfg = self.dialogItems[index]
	self:PlayById(cfg.dialogID)
end


function BookReadingMgr:CheckChapterComplete(dialogID)
    if logic.cs.BookReadingWrapper.EndDialogID == dialogID then
        self.isReading = false
        self.view.chapterSwitch:Show()
        return true
    end
    return false
end

function BookReadingMgr:GetComponentByID(id)
    local comp = self.components[id]
	if not comp then
        logic.debug.LogError(string.format('找不到component:id=%d,count=%d,bookID=%d,chapterID=%d,[%d-%d]',
        id, table.count(self.components), 
        self.bookData.BookID, self.bookData.ChapterID, 
        logic.cs.BookReadingWrapper.BeginDialogID,logic.cs.BookReadingWrapper.EndDialogID))
		return nil
    end
    return comp
end

function BookReadingMgr:PlayById(id)
    logic.bookReadingMgr.view.choiceGroup:hide()
    id = core.Mathf.Clamp(id,logic.cs.BookReadingWrapper.BeginDialogID,logic.cs.BookReadingWrapper.EndDialogID)
    self.lastComponent = self.playingComponent
    if self.lastComponent then
        if self:CheckChapterComplete(self.lastComponent.cfg.dialogID) then
            return
        end
    end
    self.playingComponent = self:GetComponentByID(id)
    if self.playingComponent == nil then
        return
    end
    logic.debug.LogWarning(string.format('---play dialog id=%d,type=%s',id,self.playingComponent.__cname))
    self.bookData.DialogueID = id
	--播放bgm
	self:ChangeBGM(self.playingComponent)
    self:ShowMSstait() --显示音乐音效状态
    if self.lastComponent then
        self.lastComponent:OnPlayEnd(self.playingComponent)
    end

    if self.playingComponent:IsDanmakuTrigger() then
        self.view:StopOperationTips();
        logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.DanmakuFlagTrigger,self.playingComponent.cfg.dialogID)
    else
        logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.DanmakuFlagTrigger,-1)
    end

    self.playingComponent:Play()
	--显示当前书本的阅读进度
    self.view:updateReadingProgress(self.playingComponent)
    
    --彩蛋
        logic.Eggs:Show(self.playingComponent.cfg)
end


-- function BookReadingMgr:OnPlayComplete(component)
--     local index = component.index
--     self:PlayByIndex(index + 1)
-- end

function BookReadingMgr:ChangeBGM(component)
    if not component.cfg.BGMID then
        component.cfg.BGMID = 0
    end
    if self.currentBGMID == component.cfg.BGMID then
        return
    end
    self.currentBGMID = component.cfg.BGMID
    self.Res:PlayBGM(self.bookID, tostring(self.currentBGMID));
end

function BookReadingMgr:ShowMSstait()
    if (logic.cs.UserDataManager.UserData.BgMusicIsOn ==0) then
        logic.cs.AudioManager:StopBGM()
    end
       
    if (logic.cs.UserDataManager.UserData.TonesIsOn == 0) then
        logic.cs.AudioManager:ChangeTonesVolume(0)
    else
        logic.cs.AudioManager:ChangeTonesVolume(1);
    end
end


function BookReadingMgr:OnSceneClick()
    if not self.playingComponent then
        return
    end

    if core.Time.time - self.mCurClickOperationTime > 0.2 then
        self.mCurClickOperationTime = core.Time.time
        self.playingComponent:OnSceneClick()
    end
end

function BookReadingMgr:GetAppearanceID(playerDetailsID, role_id, clothesID)
    if not playerDetailsID then
        playerDetailsID = 0
    end
    if not role_id then
        role_id = 0
    end
    if not clothesID then
        clothesID = 0
    end

    local appearanceID = (100000 + (playerDetailsID * 10000) + (role_id * 100) + clothesID) * 10000;
    return appearanceID;
end

function BookReadingMgr:GetNPCAppearanceID(npcSex, role_id, clothesID)
    if not npcSex then
        npcSex = 0
    end
    if not role_id then
        role_id = 0
    end
    if not clothesID then
        clothesID = 0
    end

    local appearanceID = (100000 + (role_id * 100) + clothesID) * 10000 + npcSex;
    return appearanceID;
end


function BookReadingMgr:GetRoleName(role_id)
    local bookData = self.bookData
    if role_id == bookData.NpcId and not string.IsNullOrEmpty(bookData.NpcName) then
        return bookData.NpcName
    end
    ---@type pb.t_BookDetails
    local cfg = nil
    cfg = logic.DataManager.client:GetBookDetailByID(bookData.BookID)
    if not cfg or #cfg.strCharacterNames < role_id then
        logic.debug.LogError("角色id错误:roleID = " .. role_id .. ",role count = " .. #cfg.strCharacterNames)
        return 'NoName'
    else
        return cfg.strCharacterNames[role_id]
    end
end


function BookReadingMgr:BackToMainClick()
    local dialogID = 1
    if self and self.playingComponent and self.playingComponent.cfg and self.playingComponent.cfg.dialogID then
        dialogID = self.playingComponent.cfg.dialogID
    end

    if logic.cs.UserDataManager.isReadNewerBook then
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NewerLeaveBook,"","",tostring(self.bookID),tostring(dialogID))
        logic.cs.UserDataManager.isReadNewerBook = false
    else
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.LeaveBook,"","",tostring(self.bookID),tostring(dialogID))
    end

    --region 【在线阅读计时操作】
    --【故事退出  销毁在线阅读计时】
    GameHelper.CloseReadTimer();
    if(Cache.ReadTimeCache.isEND==false)then
        --【请求服务器 存当前计时时间】
        local AddTime=GameHelper.onlinereadTime-Cache.ReadTimeCache.online_time;
        --请求
        GameController.ActivityControl:ReadingTaskTimeRequest2(AddTime);
    end
    --endregion



    logic.bookReadingMgr.Res:StopBGMQuick()
    logic.cs.MyBooksDisINSTANCE:SetIsPlaying(false)
    logic.cs.AudioManager:PlayTones(logic.cs.AudioTones.dialog_choice_click)
    self:markStep()
    logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.BookReadingForm)
    --logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.MainForm);
    logic.UIMgr:Open(logic.uiid.UIMainForm);
    logic.UIMgr:Close(logic.uiid.BookReading)

    --是否自动签到
    if(Cache.SignInCache.activity_login.is_receive==0)then
        logic.UIMgr:Open(logic.uiid.UISignTipForm);

        --【AF事件 请求服务器 记录第一次数据】
        CS.AppsFlyerManager.Instance:GetFirstActionLog();
    end

    logic.cs.EventDispatcher.Dispatch(logic.cs.EventEnum.BookProgressUpdate)
    logic.cs.AudioManager:CleanClip()
    self:Reset()
    self:ClearBookResources()
end

--这个是临时进度保存
function  BookReadingMgr:markStep()
    if self.playingComponent == nil then
        return
    end
    logic.gameHttp:markStep(self.bookID, self.chapterID, self.playingComponent.cfg.dialogID,   
    function(result)
        logic.debug.Log("----markStep---->" .. result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code ==200 then
            print("临时进度保存成功了")
        end
    end)
end


function BookReadingMgr:SaveProgress(callback)
    local params = self.bookDataCache or {}
    self.bookDataCache = {}
    local bookData = self.bookData
    logic.bookReadingMgr.view:ResetOperationTips()
    
    if self.playingComponent then
        logic.debug.Log(string.format( "[+]保存进度:%d",self.playingComponent.cfg.dialogID))
        local is_use_prop = logic.cs.UserDataManager.is_use_prop
        local discount = "0"
        if is_use_prop==1 then
            discount = logic.cs.UserDataManager.propInfoItem.discount_string
        end
        logic.gameHttp:SendPlayerProgress(
            self.bookID,
            self.chapterID,
            self.playingComponent.cfg.dialogID,
            params.option, --选项index
            params.PlayerName, --角色名
            params.NpcId, --npcId
            params.NpcName, --npc名字
            params.NpcDetailId, --性别0默认 1男 2女
            params.character_id, --model skinID
            params.outfit_id, --model clothesID
            params.hair_id, --model hairID
            params.optionList, --选项list
            is_use_prop,--是否使用钥匙优惠价道具: 1.使用道具 0不使用（非必传，默认不使用）
            discount,--折扣率
            function(result)
                --logic.debug.Log("----SendPlayerProgress---->" .. result)
                local jsonData = core.json.Derialize(result)
                local code = jsonData.code
                if code == 200 then
                    logic.cs.UserDataManager:SetSendPlayerProgress(result)
                    if jsonData and jsonData.data then
                        if jsonData.data.user_key then
                            logic.cs.UserDataManager:ResetMoney(1, jsonData.data.user_key)
                        end
                        if jsonData.data.user_diamond then
                            logic.cs.UserDataManager:ResetMoney(2, jsonData.data.user_diamond)
                        end
                    end
                    logic.cs.UserDataManager:UpdatePropItemWhenServerCallback()
                end
                if callback then
                    callback(result)
                end
            end);
    end
end


function BookReadingMgr:DebugUpdate()
    if logic.cs.Input.GetKeyDown(CS.UnityEngine.KeyCode.Keypad0) then
        logic.cs.BookReadingWrapper:StartAutoTest()
    end
    if logic.cs.Input.GetKeyDown(CS.UnityEngine.KeyCode.Keypad8) then
        logic.cs.UserDataManager:ClearCache()
    end
end

function BookReadingMgr:ResetCurBookPlayerName()
    self.bookData.PlayerName = "PLAYER";
end

function BookReadingMgr:ClearBookResources()
    self:Reset()
    
    self.bookID = 0
    self.chapterID = 0
    self.bookData = nil
    self.Res:Release()
end

function BookReadingMgr:ReplaceChar(str)
    if string.IsNullOrEmpty(str) then
        return str
    end
    str = self:GetPersonBySex(str)
    local replaceTable = {
        ["#B#"] = "<b>",
        ["#Be#"] = "</b>",

        ["#I#"] = "<i>",
        ["#Ie#"] = "</i>",

        ["#SIs#"] = "<size=",
        ["#SIv#"] = ">",
        ["#SIe#"] = "</size>",

        ["#COs#"] = "<color=",
        ["#COv#"] = ">",
        ["#COe#"] = "</color>",
    }
    for k,v in pairs(replaceTable) do
        str = string.gsub(str, k, v)
    end
    return str
end


function BookReadingMgr:GetPersonBySex(str)
    if string.IsNullOrEmpty(str) then
        return str
    end
    local sex = self.bookData.NpcDetailId > 3 and 1 or 0
    local replaceTable = {}
    if sex == 0 then
        replaceTable["#STRING#1#"] = "He"
        replaceTable["#STRING#2#"] = "His"
        replaceTable["#STRING#3#"] = "Him"
        replaceTable["#STRING#4#"] = "he"
        replaceTable["#STRING#5#"] = "his"
        replaceTable["#STRING#6#"] = "him"
        replaceTable["#STRING#7#"] = "man"
        replaceTable["#STRING#8#"] = "boyfriend"
        replaceTable["#STRING#9#"] = "member"
        replaceTable["#STRING#10#"] = "Dad"
        replaceTable["#STRING#11#"] = "daddy"
    else
        replaceTable["#STRING#1#"] = "She"
        replaceTable["#STRING#2#"] = "Her"
        replaceTable["#STRING#3#"] = "Her"
        replaceTable["#STRING#4#"] = "she"
        replaceTable["#STRING#5#"] = "her"
        replaceTable["#STRING#6#"] = "her"
        replaceTable["#STRING#7#"] = "woman"
        replaceTable["#STRING#8#"] = "girlfriend"
        replaceTable["#STRING#9#"] = "finder"
        replaceTable["#STRING#10#"] = "Mom"
        replaceTable["#STRING#11#"] = "mommy"
    end
    
    replaceTable["#STRING#0#"] = self.bookData.PlayerName
    if not string.IsNullOrEmpty(self.bookData.NpcName) then
        replaceTable["#STRING#20#"] = self.bookData.NpcName
    else
        replaceTable["#STRING#20#"] = sex == 0 and "Jean-Pierre" or "Sarah"
    end

    for k,v in pairs(replaceTable) do
        str = string.gsub(str, k, v)
    end
    return str
end

--region cache
function BookReadingMgr:SaveOption(option)
    self.bookDataCache.option = option
end

function BookReadingMgr:SaveOptionList(optionList)
    self.bookDataCache.optionList = optionList
end

function BookReadingMgr:SavePlayerDetailsID(PlayerDetailsID)
    self.bookDataCache.PlayerDetailsID = PlayerDetailsID
end

function BookReadingMgr:SaveNpc(NpcId,NpcDetailId)
    self.bookDataCache.NpcId = NpcId
    self.bookDataCache.NpcDetailId = NpcDetailId
end

function BookReadingMgr:SavePlayerClothes(PlayerClothes)
    self.bookDataCache.PlayerClothes = PlayerClothes
end

function BookReadingMgr:SaveOutfitId(OutfitId)
    self.bookDataCache.outfit_id = OutfitId
end

function BookReadingMgr:SavePlayerName(PlayerName)
    self.bookDataCache.PlayerName = PlayerName
end

function BookReadingMgr:SaveNpcName(npcId,NpcName)
    self.bookDataCache.NpcId = npcId
    self.bookDataCache.NpcName = NpcName
end

function BookReadingMgr:SavePhoneRoleID(PhoneRoleID)
    self.bookDataCache.PhoneRoleID = PhoneRoleID
end

function BookReadingMgr:SaveModel(character_id,outfit_id,hair_id)
    self.bookDataCache.character_id = character_id
    self.bookDataCache.outfit_id = outfit_id
    self.bookDataCache.hair_id = hair_id
end
--endregion


function BookReadingMgr:GotoDialogID(id)
    if id > self.bookData.DialogueID then
        self:PlayById(id)
        return
    end
    local bookData = logic.bookReadingMgr.bookData
    local bookID = bookData.BookID
    local bookDetails = logic.cs.GameDataMgr.table:GetBookDetailsById(bookID)
    local chapterDivisionArray = bookDetails.ChapterDivisionArray
    local chapterID = 0
    local dialogID = id
    for idx = 0,chapterDivisionArray.Length - 1 do
        local value = chapterDivisionArray[idx]
        if value > dialogID then
            chapterID = idx + 1
            break
        end
    end
    if chapterID == 0 then
        logic.debug.LogError("未找到章节,dialogID="..dialogID)
        return
    end


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
                    local idx = chapterid - 1
                    local beginDialogID = (idx < 1 and 1) or chapterDivisionArray[idx - 1];
                    local endDialogID = beginDialogID
                    if (idx < chapterDivisionArray.Length) then
                        endDialogID = chapterDivisionArray[idx]
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


function BookReadingMgr:DoGoBackStep(dialogID)
    local bookData = logic.bookReadingMgr.bookData
    local bookDetails = logic.cs.GameDataMgr.table:GetBookDetailsById(bookData.BookID)
    local chapterDivisionArray = bookDetails.ChapterDivisionArray
    local doGoto = function()
        logic.gameHttp:GoBackStep(bookData.BookID,bookData.ChapterID,dialogID, function(result)
			--logic.cs.UINetLoadingMgr:Close()
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                local chapterid= tonumber(json.data.chapterid)
                local dialogid = tonumber(json.data.dialogid)
                
                if chapterid == bookData.ChapterID then	--同一章节里
                    logic.bookReadingMgr:PlayById(dialogid)
                else
                    local idx = chapterid - 1
                    local beginDialogID = (idx < 1 and 1) or chapterDivisionArray[idx - 1];
                    local endDialogID = beginDialogID
                    if (idx < chapterDivisionArray.Length) then
                        endDialogID = chapterDivisionArray[idx]
                    end
                    if dialogid < beginDialogID then
                        dialogid = beginDialogID
                    end
                    if dialogid > endDialogID then
                        dialogid = endDialogID
                    end
        
                    logic.bookReadingMgr.isReading = false
                    logic.cs.BookReadingWrapper:InitByBookID(
                        bookData.BookID,
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

    doGoto()
    
end

function BookReadingMgr:SetSelectBookId(bookId)
    self.selectBookId = bookId
end

return BookReadingMgr.New()