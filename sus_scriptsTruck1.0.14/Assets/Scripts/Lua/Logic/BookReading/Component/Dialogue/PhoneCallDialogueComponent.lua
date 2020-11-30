local Class = core.Class
local Base = logic.BookReading.BaseDialogueComponent

local PhoneCallDialogueComponent = Class("PhoneCallDialogueComponent", Base)

--收集所用到的资源
function PhoneCallDialogueComponent:CollectRes(resTable)
	Base.CollectRes(self,resTable)
    local bookData = logic.bookReadingMgr.bookData
    resTable["Assets/Bundle/UI/BookReading/PhoneCallDialogue.prefab"] = BookResType.UIRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/EnterName/bg.png"] = BookResType.BookRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/EnterName/bg_input.png"] = BookResType.BookRes
    if bookData.PhoneRoleID > 0 then
        resTable[logic.bookReadingMgr.Res.bookCommonPath.."UI/PhoneCallHeadIcon/"..bookData.PhoneRoleID..".png"] = BookResType.BookRes
    else
        bookData.PhoneRoleID = self.cfg.role_id
    end
    if tonumber(self.cfg.role_id) > 0 then
        resTable[logic.bookReadingMgr.Res.bookCommonPath.."UI/PhoneCallHeadIcon/"..self.cfg.role_id..".png"] = BookResType.BookRes
    end
end

function PhoneCallDialogueComponent:Clean()
	Base.Clean(self)
end

--- @param component BaseComponent @将要播放的component
function PhoneCallDialogueComponent:OnPlayEnd(nextComponent)
    if nextComponent.cfg.dialogID < self.cfg.dialogID then
        self:setPhoneCallMessage(true)
    end
end

function PhoneCallDialogueComponent:Play()
    Base.Play(self)
    self.ui = logic.bookReadingMgr.UI:GetPhoneCallDialogue()
    self.ui.Reset()
    
    self.ui.PhoneCallButton.onClick:AddListener(function()
        self:onPhoneCallClick()
    end)

    -- local bookData = logic.bookReadingMgr.bookData
    -- self.isPhoneCallMode = bookData.IsPhoneCallMode
    self.isPhoneCallMode = false

    local nextComponent = logic.bookReadingMgr.components[self:GetNextDialogID()]
    if nextComponent ~= nil then
        self.isPhoneCallMode = (nextComponent.cfg.PhoneCall == 1)
    end

    self:setPhoneCallMessage(self.isPhoneCallMode)
    
end

function PhoneCallDialogueComponent:OnSceneClick()
    --Base.OnSceneClick(self)
end

function PhoneCallDialogueComponent:setPhoneCallMessage(isPhoneCallMode)
    local bookData = logic.bookReadingMgr.bookData
    bookData.IsPhoneCallMode = isPhoneCallMode
    logic.debug.LogError('--------------------------isPhoneCallMode=:'..tostring(isPhoneCallMode))
    if isPhoneCallMode then
        self.ui.PhoneCallButton.gameObject:SetActiveEx(true)
        self.ui.PhoneCallState.text = 'Calling'
        self.ui.PhoneCallRoleName.text = logic.bookReadingMgr:GetRoleName(self.cfg.role_id)
        
        bookData.PhoneRoleID = self.cfg.role_id
        local spt = logic.bookReadingMgr.Res:GetSprite('UI/PhoneCallHeadIcon/'..self.cfg.role_id,true)
        if IsNull(spt) then
            logic.debug.LogError("没有电话角色头像:"..bookData.PhoneRoleID)
        else
            self.ui.PhoneCallHeadIcon.sprite = spt
        end
    end

    local startPos = isPhoneCallMode and core.Vector2.New(-1024, -1024) or core.Vector2.New(0, 0)
    local endPos = isPhoneCallMode and core.Vector2.New(0, 0) or core.Vector2.New(-1024, -1024)
    self.ui.PhoneCallBG.anchoredPosition = startPos
    self.ui.PhoneCallBG:DOAnchorPos(endPos, 0.6):OnStart(function()
        logic.cs.BookReadingWrapper.IsTextTween = true
        if isPhoneCallMode then
            self.ui.gameObject:SetActiveEx(true)
        end
    end):OnComplete(function()
        logic.cs.BookReadingWrapper.IsTextTween = false

        if not isPhoneCallMode then --退出电话状态
            bookData.IsPhoneCallMode = false
            self.ui.PhoneCallButton.gameObject:SetActiveEx(false)
            self:ShowNextDialog()

            logic.bookReadingMgr:SaveProgress()
        else --进入电话状态
            self.ui.transform:DOShakeRotation(3, core.Vector3.New(0, 0, 3), 200):SetEase(core.tween.Ease.Flash);
            logic.bookReadingMgr.Res:StopBGMQuick()
            local clip = logic.cs.ResourceManager:GetAudioTones("BGM/TelephoneRing_didi.mp3")
            logic.bookReadingMgr.Res:PlayBgmByClip(clip)
        end
    end)
end


function PhoneCallDialogueComponent:onPhoneCallClick()
    logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_click)
    if not logic.cs.BookReadingWrapper.IsTextTween then
        logic.bookReadingMgr.bookData.IsPhoneCallMode = true
        self.ui.PhoneCallState.text = "in the call"
        self.ui.PhoneCallButton.gameObject:SetActiveEx(false)
        logic.bookReadingMgr:SaveProgress()
        self:ShowNextDialog()
        logic.bookReadingMgr.Res:StopBGMQuick()
        logic.bookReadingMgr.currentBGMID = -1
        logic.bookReadingMgr:ChangeBGM(self)
    end
end

return PhoneCallDialogueComponent