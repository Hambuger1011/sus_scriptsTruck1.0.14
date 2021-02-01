local Class = core.Class

local Base = require("Logic/BookReading/Component/Choice/spain/BaseChoice")
local UICtrl = require("Logic/BookReading/Component/Choice/spain/UIChoiceCtrl")
local ChoiceCharacter = Class('ChoiceCharacter',Base)


function ChoiceCharacter:CollectRes(resTable)
    Base.CollectRes(self,resTable)

    --收集所有角色头像
    if self.isNpc then
        local NpcIDs = logic.bookReadingMgr.context.NpcIDs
        local NpcDetailIDs = logic.bookReadingMgr.context.NpcDetailIDs

        local NpcID = self.cfg.role_id
        NpcIDs[NpcID] = 1
        NpcDetailIDs[NpcID] = (NpcDetailIDs[NpcID] and NpcDetailIDs[NpcID]) or {}

        for i=1,self.cfg.selection_num do
            local NpcDetailID = tonumber(self.cfg['selection_'..i])
            if not NpcDetailID then
                self.cfg['selection_'..i] = '0'
                NpcDetailID = 0
            end
            NpcDetailIDs[NpcID][NpcDetailID] = 1 --收集所有NpcDetailID

            local appearanceID = logic.bookReadingMgr:GetAppearanceID(0,NpcID,self.cfg.icon) + NpcDetailID
            resTable[logic.bookReadingMgr.Res.bookFolderPath..'RoleHead/'..appearanceID..'.png'] = BookResType.BookRes
        end
    else
        local roleIDs = logic.bookReadingMgr.context.PlayerIDs
        for i=1,self.cfg.selection_num do
            local roleID = tonumber(self.cfg['selection_'..i])
            if not roleID then
                self.cfg['selection_'..i] = '0'
                roleID = 0
            end
            roleIDs[roleID] = 1
            
            local appearanceID = logic.bookReadingMgr:GetAppearanceID(roleID,1,0)
            resTable[logic.bookReadingMgr.Res.bookFolderPath..'RoleHead/'..appearanceID..'.png'] = BookResType.BookRes
        end
    end
end


-- function ChoiceCharacter:GetNextDialogID()
--     local id = self:GetNextDialogIDBySelection(self.selectIdx - 1);
--     return id
-- end

function ChoiceCharacter:Play()
    self.imgs = {}
    for i=1,self.cfg.selection_num do
        local roleID = tonumber(self.cfg['selection_'..i])
        local appearanceID = 0
        if self.isNpc then
            appearanceID = logic.bookReadingMgr:GetAppearanceID(0,self.cfg.role_id,self.cfg.icon) + roleID
        else
            appearanceID = logic.bookReadingMgr:GetAppearanceID(roleID, 1, 0)
        end
        local sprite = logic.bookReadingMgr.Res:GetSprite('RoleHead/'..appearanceID)
        table.insert(self.imgs, sprite)
    end

    ---@type UICtrl
    self.uiCtrl = self:getUI()
    self.uiCtrl:show(self)
    self.uiCtrl:SetComfirmType(1)
end

function ChoiceCharacter:GetItems()
    return self.imgs
end

function ChoiceCharacter:OnChoiceIndex(idx)
    self.selectIdx = idx
end


function ChoiceCharacter:OnConfirm(idx)

    --print("0000")
    self.ChoiceRoleGo=idx
	if self.ChoiceRoleGo.ConfirmMask.gameObject~=nil then			
		self.ChoiceRoleGo.ConfirmMask.gameObject:SetActiveEx(true)
	end

    local bookData = logic.bookReadingMgr.bookData
    logic.bookReadingMgr.Res:PlayTones(logic.bookReadingMgr.Res.AudioTones.dialog_choice_click)
    local npcId = 0
    local npcCharacterId = 0
    if self.isNpc then
        npcId = self.cfg.role_id
        npcCharacterId = tonumber(self.cfg['selection_'..self.selectIdx])
        bookData.NpcId = npcId
        bookData.NpcDetailId = npcCharacterId
		logic.bookReadingMgr:SaveNpc(npcId,npcCharacterId)
    else
        bookData.PlayerDetailsID = tonumber(self.cfg['selection_'..self.selectIdx])
        logic.bookReadingMgr:SavePlayerDetailsID(bookData.PlayerDetailsID)
    end
    self.uiCtrl.gameObject:SetActiveEx(false)

	logic.bookReadingMgr:SaveOption(self.selectIdx)
    logic.bookReadingMgr:SaveProgress()
    logic.cs.UserDataManager:RecordBookOptionSelect(bookData.BookID, self.cfg.dialogid, self.selectIdx);



    if self.isNpc then
        logic.cs.TalkingDataManager:SelectNpc(bookData.BookID, npcCharacterId)
    else
        logic.cs.TalkingDataManager:SelectPlayer(bookData.BookID, bookData.PlayerDetailsID)
    end
    self:ShowNextDialog()
end

return ChoiceCharacter