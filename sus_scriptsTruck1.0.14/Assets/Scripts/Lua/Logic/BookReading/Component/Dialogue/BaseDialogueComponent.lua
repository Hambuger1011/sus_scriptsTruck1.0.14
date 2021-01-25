local Class = core.Class
local Base = logic.BookReading.BaseComponent
local BaseDialogueComponent = Class("BaseDialogueComponent", Base)


function BaseDialogueComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    local bookData = logic.bookReadingMgr.bookData
    local bookContext = logic.bookReadingMgr.context
    if self.cfg.trigger == 1 then
        resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/Choice/bg_chat_choice.png"] = BookResType.BookRes
        resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/Choice/bg_chat_choice2.png"] = BookResType.BookRes
    end


	local bookFolderPath = logic.bookReadingMgr.Res.bookFolderPath
    ----收集背景、人物资源(非旁白、对话模式)
    if (self.__cname ~= 'NarrationComponent') and (not self.isPhoneCallMode) and (self.cfg.icon_bg ~= 0) then
        resTable[logic.bookReadingMgr.Res.bookCommonPath..'UI/RoleHeadFacialExpression/ColorBG_'..self.cfg.icon_bg..'.png'] = BookResType.BookRes
        --收集头像、表情
        local appearanceID = nil
        local facialExpressionID = nil

        -- if self.cfg.dialogID == 166 then
        --     logic.debug.LogError('--------------')
        -- end

        local clothGroupId = 0
        if self.cfg.role_id == 1 then   --player头像
    
            local roleIDs = nil
            if #bookContext.PlayerIDs == 0 then --该章节没有选人
                roleIDs = {}
            else --该章节有选人，遍历所有id
                roleIDs = bookContext.PlayerIDs
            end
            if logic.bookReadingMgr.isChoiceModel then
                --加载默认角色1
                local clothGroupId = 1
                local appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0, clothGroupId)
                resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
                --加载服务端保存的角色
                if bookData.character_id and bookData.character_id ~= 0 then
                    roleIDs[bookData.character_id] = 1
                end
                for roleID, v in pairs(roleIDs) do
                    local clothGroupId = 1
                    if bookData.outfit_id and bookData.outfit_id > 0 then
                        clothGroupId = core.Mathf.Ceil(bookData.outfit_id / 4)
                    end
                    clothGroupId = 1
                    local appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0, clothGroupId)
                    resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
                end
            else
                --加载默认角色1
                local clothGroupId = 1
                local appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0, clothGroupId)
                resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
                --加载服务端保存的角色
                if bookData.PlayerDetailsID and bookData.PlayerDetailsID ~= 0 then
                    roleIDs[bookData.PlayerDetailsID] = 1
                    if bookData.PlayerClothes then
                        --clothGroupId = core.Mathf.Ceil(bookData.PlayerClothes / 4)
						clothGroupId = 1
                    elseif bookData.outfit_id and bookData.outfit_id > 0 then
                        clothGroupId = core.Mathf.Ceil(bookData.outfit_id / 4)
                    end
                    if clothGroupId == 0 then
                        clothGroupId = 1
                    end
                    clothGroupId = 1
                    appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0,clothGroupId)
                    resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
                end

                --本章节有选人或衣服，遍历加载
                for roleID,_ in pairs(roleIDs) do
                    local icon = self.cfg.icon
                    if icon ~= 0 then --不确定衣服
                        local clothesIDs = bookContext.ClothesIDs[roleID]
                        if not clothesIDs then
                            clothesIDs = {}
                            clothesIDs[bookData.PlayerClothes] = 1
                        end
                        for clothesID,_ in pairs(clothesIDs) do
                            --appearanceID = logic.bookReadingMgr:GetAppearanceID(roleID, self.cfg.role_id, clothesID)
                            --clothGroupId = core.Mathf.Ceil(clothesID / 4)
                            --if clothGroupId == 0 then
                            --    clothGroupId = 1
                            --end
							clothGroupId = 1
                            appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0,clothGroupId)
                            resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
                        end
                    else    --配置指定衣服
                        --appearanceID = logic.bookReadingMgr:GetAppearanceID(roleID, self.cfg.role_id, icon)
                        --clothGroupId = core.Mathf.Ceil(icon / 4)
                        --if clothGroupId == 0 then
                         --   clothGroupId = 1
                        --end
						clothGroupId = 1
                        appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0,clothGroupId)
                        resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
                    end
        
                    if logic.config.channel == Channel.Spain and self.cfg.phiz_id ~= 0 then
                        facialExpressionID = logic.bookReadingMgr:GetAppearanceID(roleID,self.cfg.role_id, self.cfg.phiz_id)
                        --resTable[logic.bookReadingMgr.Res.bookCommonPath..'UI/RoleHeadFacialExpression/'..facialExpressionID..'.png'] = BookResType.BookRes
                    end
                end
            end
        else    --npc
            local npcID = self.cfg.role_id
            local clothesID = self.cfg.icon
            local sex = 0
            local NpcDetailIDs = nil
            if bookContext.NpcDetailIDs[npcID] then
                NpcDetailIDs = bookContext.NpcDetailIDs[npcID]
            elseif npcID == bookData.NpcId then
                NpcDetailIDs = {}
                NpcDetailIDs[bookData.NpcDetailId] = 1
            else
                NpcDetailIDs = {}
                NpcDetailIDs[0] = 1
            end
            --logic.debug.LogError(core.table.tostring(NpcDetailIDs))
            for NpcDetailID,_ in pairs(NpcDetailIDs) do
                if NpcDetailID == 0 and self.cfg.role_id == 0 then
                    goto continue
                end
                
                --appearanceID = logic.bookReadingMgr:GetAppearanceID(0, self.cfg.role_id, clothesID) + NpcDetailID
                -- if clothesID ~= 0 then
                --     clothGroupId = core.Mathf.Ceil(clothesID / 4)
                -- else
                --     clothGroupId = 1
                -- end
                --sex = NpcDetailID % 3

                if clothesID ~= 0 then
                    clothGroupId = core.Mathf.Ceil(clothesID / 4)
                else
                    clothGroupId = 1
                end
                local sex = core.Mathf.Ceil(NpcDetailID / 3)

                appearanceID = logic.bookReadingMgr:GetNPCAppearanceID(sex,self.cfg.role_id,clothGroupId)
                resTable[bookFolderPath..'Role/'..appearanceID..'_SkeletonData.asset'] = BookResType.BookRes
                if logic.config.channel == Channel.Spain and self.cfg.phiz_id ~= 0 then
                    facialExpressionID = logic.bookReadingMgr:GetAppearanceID(0,self.cfg.role_id, self.cfg.phiz_id) + NpcDetailID
                    resTable[logic.bookReadingMgr.Res.bookCommonPath..'UI/RoleHeadFacialExpression/'..facialExpressionID..'.png'] = BookResType.BookRes
                end
                ::continue::
            end
        end
    end

end

function BaseDialogueComponent:GetNextDialogID()
    local id = self.cfg.next
    --if self:IsEmojiTrigger() or self.__cname == 'NarrationComponent' then
    --if self:IsDanmakuTrigger() then
    --    id = Base.GetNextDialogID(self)
    --else
    if self.cfg.trigger ~= 0 and self.cfg.selection_num then
        local idx = logic.bookReadingMgr.view.choiceGroup.selectIdx - 1
        id = self:GetNextDialogIDBySelection(idx);
    else
        id = Base.GetNextDialogID(self)
    end
    return id
end

function BaseDialogueComponent:Play()
    self.IsPlayTween = true
    local bookData = logic.bookReadingMgr.bookData
    
    if self:IsDanmakuTrigger() then
        -- local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.EmojiMsgForm)
        -- local emojiForm = uiform:GetComponent(typeof(CS.EmojiMsgForm))
        -- emojiForm:Init(self.cfg.trigger)
    end
end



function BaseDialogueComponent:OnSceneClick(_showNextDialog)
    if self.IsPlayTween or self.cfg.trigger == 1 then
        return
    end
    Base.OnSceneClick(self)
    if logic.cs.BookReadingWrapper.IsTextTween and not _showNextDialog then
        self.ui.DialogText:StopTyperTween()
        logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_click)
    else
        logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_click)
        self:ShowNextDialog()
    end

end

---@param Dialogue gameObject
---@param typertext Text @文本
---@param faceExpr Face @表情
---@param PlayerName Text @玩家名
function BaseDialogueComponent:ShowDetails(Dialogue, typertext, faceExpr, PlayerName, DialogBox, DialogBoxContent, isPhoneCallMode, vSideType,PhoneArrow)
    --logic.debug.Log(self.__cname)

    local showText = logic.bookReadingMgr:ReplaceChar(self.cfg.dialog)
    if not self:IsSameDialogType(self)then
        self:ResetDialogPos(Dialogue)
    end
    --移动背景
    logic.cs.BookReadingWrapper.IsTextTween = true
    logic.bookReadingMgr.view:sceneBGMove(logic.bookReadingMgr.view:GetPiexlX(self.cfg.Scenes_X),function()
        
        logic.bookReadingMgr.view.tipsImage:ShowTips(self.cfg.tips)

        if DialogBoxContent then 
            DialogBoxContent:SetActiveEx(false) 
        end
        Dialogue:SetActiveEx(true)
        Dialogue.transform.localScale = core.Vector3.one

        if not isPhoneCallMode then
            self:characterShowTween(
                self.cfg.role_id, 
                self.cfg.icon, 
                self.cfg.phiz_id, 
                self.cfg.icon_bg, 
                vSideType,
                self.cfg.Orientation,
                faceExpr
                )
        else
            typertext.text = ''
            if(PhoneArrow)then
                if(self.cfg.dialog_type == logic.DialogType.PlayerImagineDialogue)then  --PlayerImagineDialogue = 7,  --主角思考下的对话（主角心理活动）
                    PhoneArrow.sprite = CS.ResourceManager.Instance:GetUISprite("PhoneCall/bg_chatarrow_phonecall5");
                else
                    PhoneArrow.sprite = CS.ResourceManager.Instance:GetUISprite("PhoneCall/bg_chatarrow_phonecall3");
                end
            end
        end
        
        local callback = function()
            if DialogBoxContent then DialogBoxContent:SetActiveEx(true) end
            local PlayerNameTxt = string.Empty
            if self.cfg.role_id == 1 then
                PlayerNameTxt = logic.bookReadingMgr.bookData.PlayerName
            else
                PlayerNameTxt = logic.bookReadingMgr:GetRoleName(self.cfg.role_id)
            end
            PlayerName.text = string.upper(PlayerNameTxt)
            self.IsPlayTween = false
            logic.bookReadingMgr.view:textDialogTween(self,DialogBox,typertext,0,showText,function()
                if self.cfg.trigger == 1 then
                    logic.bookReadingMgr.view.choiceGroup:choicesDialogInit(self,vSideType)
                    logic.bookReadingMgr.view.choiceGroup:show()
                else
                    if logic.cs.GameDataMgr.InAutoPlay and self.cfg.dialogID ~= logic.cs.BookReadingWrapper.EndDialogID then
                        local autoPlayTimer
                        autoPlayTimer = core.Timer.New(function()
                            if logic.bookReadingMgr.playingComponent == self then
                                if logic.cs.GameDataMgr.InAutoPlay then
                                    self:OnSceneClick(true)
                                    autoPlayTimer:Stop()
                                    autoPlayTimer = nil
                                end
                            else
                                if autoPlayTimer then
                                    autoPlayTimer:Stop()
                                end
                                autoPlayTimer = nil
                            end
                        end,4 - tonumber(logic.cs.UserDataManager.UserData.AutoSpeed),-1)
                        autoPlayTimer:Start()
                    end
                    logic.cs.GameDataMgr.AutoPlayOpen = function()
                        self:OnSceneClick()
                        logic.cs.GameDataMgr.AutoPlayOpen = nil
                    end
                    logic.cs.GameDataMgr.AutoPlayClose = function()
                        --core.coroutine.step(nil,co)
                    end
                end
                logic.cs.BookReadingWrapper.IsTextTween = false
            end)
        end
        if not self:IsSameDialogType(self) then
            self:dialogEnterTween(Dialogue, callback)
        else
            callback()
        end

        
        if not isPhoneCallMode then
            self:characterShake(faceExpr, self.cfg.is_tingle == 1)
            --tipsImage.ShowTips(dialogData.tips)
        end
    end)

    -- local saveProgressTick = logic.bookReadingMgr.saveProgressTick or 0
    -- if self.cfg.dialogID - saveProgressTick > 30 then
    --     self.saveProgressTick = self.cfg.dialogID
    --     if self.cfg.trigger == 0 then   --没有选项，自动保存进度
    --         logic.bookReadingMgr:SaveProgress()
    --     end
    -- end
end


function BaseDialogueComponent:characterShowTween(role_id, clothesID, phiz_id, icon_bg, vSideType,vOrientation, faceExpr)
    local clothGroupId = 0
    local appearanceID = 0
    local facialExpressionID = 0
    local duration = 0.3
    local trans = faceExpr.transform
    local bookData = logic.bookReadingMgr.bookData
    trans:DOKill()
    trans.localScale = core.Vector3.one
    if not clothesID then
        clothesID = 0
    end
    if role_id == 1 then
        --local playerClothID = clothesID
        if clothesID ~= 0 and bookData.PlayerClothes and bookData.PlayerClothes ~= 0 then
            clothesID = bookData.PlayerClothes
        end
        --clothGroupId = core.Mathf.Ceil(clothesID / 4)
        --if clothGroupId == 0 then
        --    clothGroupId = 1
        --end
		clothGroupId = 1
        appearanceID = logic.bookReadingMgr:GetAppearanceID(1,0,clothGroupId)
        facialExpressionID = logic.bookReadingMgr:GetAppearanceID(bookData.PlayerDetailsID,role_id,phiz_id)

        
        if logic.config.isDebugMode then
            if logic.bookReadingMgr.Res:GetSkeDataAsset("Role/" .. appearanceID) == nil then
                logic.debug.LogError(string.format( "找不到role资源:dialogID=%d,clothGroupId=%d,appearanceID=%d",self.cfg.dialogID,clothGroupId,appearanceID))
            end
        end

    else
        local npcDetailId = 0 
        if role_id == bookData.NpcId then
            npcDetailId = bookData.NpcDetailId
        end
        if clothesID ~= 0 then
            clothGroupId = core.Mathf.Ceil(clothesID / 4)
        else
            clothGroupId = 1
        end
        local sex = core.Mathf.Ceil(npcDetailId / 3)
        appearanceID = logic.bookReadingMgr:GetNPCAppearanceID(sex,role_id,clothGroupId)
        facialExpressionID = logic.bookReadingMgr:GetAppearanceID(0,role_id,phiz_id) + npcDetailId

        if logic.config.isDebugMode then
            if logic.bookReadingMgr.Res:GetSkeDataAsset("Role/" .. appearanceID) == nil then
                logic.debug.LogError(string.format( "找不到npc资源:dialogID=%d,sex=%d,role_id=%d,clothGroupId=%d,npcDetailId=%d,appearanceID=%d",
                    self.cfg.dialogID,
                    sex,
                    role_id,
                    clothGroupId,
                    npcDetailId,
                    appearanceID
                ))
            end
        end
    end

    local lastComponent = logic.bookReadingMgr.lastComponent
    if lastComponent and lastComponent.cfg.role_id  ~= role_id then
        trans.anchoredPosition = (vSideType == 0) and core.Vector2.New(-300,-100) or core.Vector2.New(300,-100)
        trans.localScale = core.Vector3.New(0.6,0.6,1)
        trans:DOAnchorPos(core.Vector2.New(0, 21.7), duration):SetEase(core.tween.Ease.Flash)
        trans:DOScale(core.Vector3.one, duration)
            :SetEase(core.tween.Ease.Flash)
            :OnComplete(function()
                trans.localScale = core.Vector3.one
            end):Play()
    else
        trans.anchoredPosition = core.Vector2.New(0, 21.7)
    end
    faceExpr:Change(role_id, appearanceID,clothesID, phiz_id, icon_bg,vOrientation)
end


function BaseDialogueComponent:characterShake(faceExpr,isOn)
    if not isOn then
        return
    end
    faceExpr.transform:DOShakeRotation(0.3, core.Vector3.New(0, 0, 3), 200):SetDelay(0.5):OnStart(function()
        logic.cs.Handheld:Vibrate()
    end):SetEase(core.tween.Ease.Flash)
end

function BaseDialogueComponent:ResetDialogPos(dialogBox,callback)
    logic.debug.LogError('未实现方法:ResetDialogPos')
end

function BaseDialogueComponent:dialogEnterTween(dialogBox,callback)
    logic.debug.LogError('未实现方法:dialogEnterTween')
end




function BaseDialogueComponent:IsSameDialogType(component)
    local lastComponent = logic.bookReadingMgr.lastComponent
    if lastComponent then
        return lastComponent.__cname == component.__cname
                and lastComponent.isPhoneCallMode == component.isPhoneCallMode
    end
    return false
end

return BaseDialogueComponent