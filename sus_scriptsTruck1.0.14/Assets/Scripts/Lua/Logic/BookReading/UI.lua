local Class = core.Class
local UI = Class('BookReading.UI')
local CharacterFaceExpressionChange = logic.CharacterFaceExpressionChange

function UI:__init()
    self.PhoneCallDialogue = {}
    self.Phone_PlayerDialogue = {}
    self.Phone_OtherDialogue = {}
    self.Phone_NarrationDialogue = {}

    self.NarrationDialogue = {}
    self.OtherDialogue = {}
    self.PlayerDialogue = {}
    self.OtherImgDialogue = {}
    self.PlayerImgDialogue = {}

    self.ChoiceRole = {}
end


function UI:GetNarrationDialogue()
    local ui = self.NarrationDialogue
	if not ui.gameObject then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/Dialog_Narration.prefab")
        ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
        ui.gameObject:SetActiveEx(false)
        ui.transform = ui.gameObject.transform
        ui.canvasGroup = logic.cs.LuaHelper.AddMissingComponent(ui.gameObject,typeof(logic.cs.CanvasGroup))
        ui.DialogBox = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Middle",typeof(logic.cs.Image))
        ui.DialogText = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Middle/Text",typeof(logic.cs.TextTyperAnimation))
        ui.Reset = function()
            ui.DialogText.color = logic.bookReadingMgr.context.DescriptionColor
            --ui.DialogText.text = ""
            ui.canvasGroup.alpha = 1
            ui.gameObject:SetActiveEx(true)
        end
        --ui.DialogBox.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat",true)
        ui.DialogBox.sprite=CS.ResourceManager.Instance:GetUISprite("BookReadingForm/bg_chat");
    end
    return ui
end

function UI:GetOtherDialogue()
    local ui = self.OtherDialogue
	if not ui.gameObject then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/Dialog_OthersDialogue.prefab")
		ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
        ui.gameObject:SetActiveEx(false)
		ui.transform = ui.gameObject.transform
        ui.canvasGroup = logic.cs.LuaHelper.AddMissingComponent(ui.gameObject,typeof(logic.cs.CanvasGroup))
		ui.Reset = function()
			ui.DialogText.color = logic.bookReadingMgr.context.DialogColor
            --ui.DialogText.text = ""
            ui.canvasGroup.alpha = 1
            ui.gameObject:SetActiveEx(true)
        end

        --ui
        ui.DialogBoxContent = ui.transform:Find('DialogBox_Right').gameObject
        ui.DialogBox = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Right/Bg_Right",typeof(logic.cs.Image))
        ui.DialogText = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Right/Bg_Right/Text",typeof(logic.cs.TextTyperAnimation))
        ui.PlayerName = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Right/CharacterName",typeof(logic.cs.Text))
        ui.FaceExpr = CharacterFaceExpressionChange.New(ui.transform:Find('Character_Right'))
        ui.imgBgDot = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Right/Bg_Dot",typeof(logic.cs.Image))

        
        --ui.DialogBox.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat_right",true)
    end
    return ui
end




function UI:GetPlayerDialogue()
    local ui = self.PlayerDialogue
	if not ui.gameObject then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/Dialog_PlayerDialogue.prefab")
        ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
        ui.gameObject:SetActiveEx(false)
        ui.transform = ui.gameObject.transform
        ui.canvasGroup = logic.cs.LuaHelper.AddMissingComponent(ui.gameObject,typeof(logic.cs.CanvasGroup))
        ui.Reset = function()
			ui.DialogText.color = logic.bookReadingMgr.context.DialogColor
            --ui.DialogText.text = ""
            ui.canvasGroup.alpha = 1
            ui.gameObject:SetActiveEx(true)
        end
        local get = logic.cs.LuaHelper.GetComponent
        ui.DialogBoxContent = ui.transform:Find('DialogBox_Left').gameObject
        ui.DialogBox = get(ui.transform,"DialogBox_Left/Bg_Left",typeof(logic.cs.Image))
        ui.DialogText = get(ui.transform,"DialogBox_Left/Bg_Left/Text",typeof(logic.cs.TextTyperAnimation))
        ui.PlayerName = get(ui.transform,"DialogBox_Left/CharacterName",typeof(logic.cs.Text))
        ui.imgBgDot = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Left/Bg_Dot",typeof(logic.cs.Image))

        ui.FaceExpr = CharacterFaceExpressionChange.New(ui.transform:Find("Character_Left"))
        --ui.bg1 = get(ui.transform,"Character_Left/ColorBG/ColorBG_1",typeof(CS.CharacterFaceExpressionChange))
        --ui.bg2 = get(ui.transform,"Character_Left/ColorBG/ColorBG_2",typeof(CS.CharacterFaceExpressionChange))


        
        --ui.DialogBox.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat_left",true)
        --ui.imgBgDot.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat_2",true)
    end
    return ui
end





function UI:GetOtherImgDialogue()
    local ui = self.OtherImgDialogue
	if not ui.gameObject then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/OtherImagineDialogueComponent.prefab")
		ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
        ui.gameObject:SetActiveEx(false)
		ui.transform = ui.gameObject.transform
        ui.canvasGroup = logic.cs.LuaHelper.AddMissingComponent(ui.gameObject,typeof(logic.cs.CanvasGroup))
		ui.Reset = function()
			ui.DialogText.color = logic.bookReadingMgr.context.DialogColor
            --ui.DialogText.text = ""
            ui.canvasGroup.alpha = 1
            ui.gameObject:SetActiveEx(true)
        end
  
        --ui
        ui.DialogBox = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Right/Bg_Right",typeof(logic.cs.Image))
        ui.DialogText = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Right/Bg_Right/Text",typeof(logic.cs.TextTyperAnimation))
        ui.PlayerName = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Right/CharacterName",typeof(logic.cs.Text))
        ui.DialogBoxContent = ui.transform:Find("DialogBox_Right").gameObject
        ui.faceExpr = CharacterFaceExpressionChange.New(ui.transform:Find('Character_Right'))

        ui.imgBgDot = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Right/Bg_Dot",typeof(logic.cs.Image))

        ui.DialogBox.sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/Dialog/bg_chat_right',true)
        ui.imgBgDot.sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/Dialog/bg_chat_2',true)
    end
    return ui
end




function UI:GetPlayerImgDialogue()
    local ui = self.PlayerImgDialogue
	if not ui.gameObject then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/Dialog_PlayerImagineDialogue.prefab")
        ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
        ui.gameObject:SetActiveEx(false)
        ui.transform = ui.gameObject.transform
        ui.canvasGroup = logic.cs.LuaHelper.AddMissingComponent(ui.gameObject,typeof(logic.cs.CanvasGroup))
        ui.Reset = function()
			ui.DialogText.color = logic.bookReadingMgr.context.DialogColor
            --ui.DialogText.text = ""
            ui.canvasGroup.alpha = 1
            ui.gameObject:SetActiveEx(true)
        end

        ui.DialogBoxContent = ui.transform:Find("DialogBox_Left").gameObject
        ui.DialogBox = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Left/Bg_Left",typeof(logic.cs.Image))
        ui.DialogText = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Left/Bg_Left/Text",typeof(logic.cs.TextTyperAnimation))
        ui.PlayerName = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Left/CharacterName",typeof(logic.cs.Text))
        ui.imgBgDot = logic.cs.LuaHelper.GetComponent(ui.transform,"DialogBox_Left/Bg_Dot",typeof(logic.cs.Image))
        ui.faceExpr = CharacterFaceExpressionChange.New(ui.transform:Find('Character_Left'))

        ui.DialogBox.sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/Dialog/bg_chat_left',true)
        ui.imgBgDot.sprite = logic.bookReadingMgr.Res:GetSprite('Atlas/Dialog/bg_think',true)
    end
    return ui
end

function UI:GetPhoneCallDialogue()
    local ui = self.PhoneCallDialogue
	if not ui.gameObject then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/PhoneCallDialogue.prefab")
		ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
		ui.transform = ui.gameObject.transform
		ui.Reset = function()
			ui.gameObject:SetActiveEx(true)
        end
        local get = logic.cs.LuaHelper.GetComponent
        ui.PhoneCallBG = ui.transform:Find('PhoneBG')
        ui.PhoneCallHeadIcon = get(ui.transform,"PhoneBG/HeadIcon",typeof(logic.cs.Image))
        ui.PhoneCallState = get(ui.transform,"PhoneBG/PhoneState",typeof(logic.cs.Text))
        ui.PhoneCallRoleName = get(ui.transform,"PhoneBG/RoleName",typeof(logic.cs.Text))
        ui.PhoneCallButton = get(ui.transform,"PhoneBG/PhoneButton",typeof(logic.cs.UITweenButton))

        ui.setPhoneState = function()
            ui.gameObject:SetActiveEx(true)
            ui.PhoneCallButton.gameObject:SetActiveEx(false)
            ui.PhoneCallState.text = 'in the call'

            local bookData = logic.bookReadingMgr.bookData
            --if bookData.IsPhoneCallMode then
                ui.PhoneCallRoleName.text = logic.bookReadingMgr:GetRoleName(bookData.PhoneRoleID)
                local spt = logic.bookReadingMgr.Res:GetSprite('UI/PhoneCallHeadIcon/'..bookData.PhoneRoleID,true)
                if IsNull(spt) then
                    logic.debug.LogError("没有电话角色头像:"..bookData.PhoneRoleID)
                else
                    ui.PhoneCallHeadIcon.sprite = spt
                end
            --end
        end
        ui.setPhoneState()
    else
        ui.PhoneCallButton.onClick:RemoveAllListeners()
    end
    return ui
end

function UI:GetPhone_NarrationDialogue()
    local ui = self.Phone_NarrationDialogue
	if not ui.gameObject then
        local root = self:GetPhoneCallDialogue()
		ui.transform = root.transform:Find('PhoneBG/NarrationAnchors')
        ui.gameObject = ui.transform.gameObject
        ui.gameObject:SetActiveEx(false)
        ui.canvasGroup = logic.cs.LuaHelper.AddMissingComponent(ui.gameObject,typeof(logic.cs.CanvasGroup))
        ui.DialogBox = logic.cs.LuaHelper.GetComponent(ui.transform,"Narration",typeof(logic.cs.Image))
        ui.DialogText = logic.cs.LuaHelper.GetComponent(ui.transform,"Narration/NarrationText",typeof(logic.cs.TextTyperAnimation))
        ui.Reset = function()
            --ui.DialogText.text = ""
            ui.canvasGroup.alpha = 1
            ui.gameObject:SetActiveEx(true)
        end
    end
    return ui
end

function UI:GetPhone_OtherDialogue()
    local ui = self.Phone_OtherDialogue
    if not ui.gameObject then
        local root = self:GetPhoneCallDialogue()
		ui.transform = root.transform:Find('PhoneBG/OtherDialogueAnchors')
        ui.gameObject = ui.transform.gameObject
        ui.gameObject:SetActiveEx(false)
		ui.transform = ui.gameObject.transform
        ui.canvasGroup = logic.cs.LuaHelper.AddMissingComponent(ui.gameObject,typeof(logic.cs.CanvasGroup))
		ui.Reset = function()
            --ui.DialogText.text = ""
            ui.canvasGroup.alpha = 1
            ui.gameObject:SetActiveEx(true)
        end

        --ui
        --ui.DialogBoxContent = ui.transform:Find('OtherDialogue').gameObject
        ui.DialogBox = logic.cs.LuaHelper.GetComponent(ui.transform,"OtherDialogue",typeof(logic.cs.Image))
        ui.DialogText = logic.cs.LuaHelper.GetComponent(ui.transform,"OtherDialogue/PlayerDialogueText",typeof(logic.cs.TextTyperAnimation))
        ui.PlayerName = logic.cs.LuaHelper.GetComponent(ui.transform,"OtherDialogue/NameTitle/Name",typeof(logic.cs.Text))
        --ui.FaceExpr = nil
        --ui.imgBgDot = nil
        
        --ui.DialogBox.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat_right",true)
        --ui.imgBgDot.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat_2",true)
	end
    return ui
end

function UI:GetPhone_PlayerDialogue()
    local ui = self.Phone_PlayerDialogue
    if not ui.gameObject then
        local root = self:GetPhoneCallDialogue()
		ui.transform = root.transform:Find('PhoneBG/PlayerDialogueAnchors')
        ui.gameObject = ui.transform.gameObject
        ui.gameObject:SetActiveEx(false)
		ui.transform = ui.gameObject.transform
        ui.canvasGroup = logic.cs.LuaHelper.AddMissingComponent(ui.gameObject,typeof(logic.cs.CanvasGroup))
		ui.Reset = function()
            --ui.DialogText.text = ""
            ui.canvasGroup.alpha = 1
            ui.gameObject:SetActiveEx(true)
        end

        --ui
        --ui.DialogBoxContent = ui.transform:Find('PlayerDialogue').gameObject
        ui.DialogBox = logic.cs.LuaHelper.GetComponent(ui.transform,"PlayerDialogue",typeof(logic.cs.Image))
        ui.DialogText = logic.cs.LuaHelper.GetComponent(ui.transform,"PlayerDialogue/PlayerDialogueText",typeof(logic.cs.TextTyperAnimation))
        ui.PlayerName = logic.cs.LuaHelper.GetComponent(ui.transform,"PlayerDialogue/NameTitle/Name",typeof(logic.cs.Text))
        --ui.FaceExpr = nil
        --ui.imgBgDot = nil
        
        --ui.DialogBox.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat_right",true)
        --ui.imgBgDot.sprite = logic.bookReadingMgr.Res:GetSprite("Atlas/Dialog/bg_chat_2",true)
	end
    return ui
end

function UI:GetChoiceRole()
    local ui = self.ChoiceRole
    if not ui.gameObject then
		local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/ChoiceRole.prefab")
        ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
        ui.gameObject:SetActiveEx(false)
        ui.transform = ui.gameObject.transform
        ui.Reset = function()
            ui.gameObject:SetActiveEx(true)
        end
        local get = logic.cs.LuaHelper.GetComponent
        ui.pfbItem = ui.transform:Find('choice/pfb_role').gameObject
        ui.selectRoot = ui.transform:Find('choice')
        ui.unSelectRoot = ui.transform:Find('unchoice')
        ui.btnConfirm1 = get(ui.transform,'ClotheDetails/ComfirmButton_1',typeof(logic.cs.Button))
        ui.btnConfirm2 = get(ui.transform,'ClotheDetails/ComfirmButton_2',typeof(logic.cs.Button))
        ui.txtConfirmCost2 = get(ui.transform,'ClotheDetails/ComfirmButton_2/DiamondImage/CostText',typeof(logic.cs.Text))
        ui.imgConfirmCost2 = get(ui.transform,'ClotheDetails/ComfirmButton_2/DiamondImage',typeof(logic.cs.Image))
	end
    return ui
end

return UI