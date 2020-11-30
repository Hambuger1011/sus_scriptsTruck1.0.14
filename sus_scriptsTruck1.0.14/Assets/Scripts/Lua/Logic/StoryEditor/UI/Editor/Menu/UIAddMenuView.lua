local BaseClass = core.Class

---@class UIStory_AddMenuView
local UIAddMenuView = BaseClass("UIAddMenuView")

function UIAddMenuView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    self.btnEditorRoles = self.uiBinding:Get('editorRoles', typeof(logic.cs.UITweenButton))
    self.btnAddDecision = self.uiBinding:Get('addDecision', typeof(logic.cs.UITweenButton))
    self.btnAddImage = self.uiBinding:Get('addImage', typeof(logic.cs.UITweenButton))

    self.btnEditorRoles.onClick:AddListener(function()
        self:OnEditorRolesClick()
    end)
    self.btnAddDecision.onClick:AddListener(function()
        self:OnAddDecisionClick()
    end)
    self.btnAddImage.onClick:AddListener(function()
        self:OnAddImageClick()
    end)

    self.btnBg = self.uiBinding:Get('btnBg', typeof(logic.cs.Button))
    self.btnBg.onClick:AddListener(function()
        self:Hide()
    end)

    
    self.root = self.uiBinding:Get('root').transform
    self.originRootPos = self.root.anchoredPosition
    self.mask = self.uiBinding:Get('mask', typeof(logic.cs.Image))
end

function UIAddMenuView:Show()
    self.isActive = true
    self.gameObject:SetActiveEx(true)

    
    core.tween.DoTween.Kill(self)
    local p = self.originRootPos
    self.root.anchoredPosition = core.Vector2.New(p.x, p.y - self.root.rect.size.y * 0.1)
    self.root:DOAnchorPosY(p.y,0.2):SetEase(core.tween.Ease.Flash):SetId(self)
    self.mask:DOFade(0,0):SetEase(core.tween.Ease.Flash):SetId(self)
    self.mask:DOFade(0.5, 0.2):SetEase(core.tween.Ease.Flash):SetId(self)
end

function UIAddMenuView:Hide(notUseTween)
    self.isActive = false
    
    if notUseTween then
        self.gameObject:SetActiveEx(false)
    else
        core.tween.DoTween.Kill(self)
        local p = self.originRootPos
        --self.root.anchoredPosition = core.Vector2.New(p.x, p.y - self.root.rect.size.y):SetId(self)
        self.root:DOAnchorPosY(p.y - self.root.rect.size.y * 0.1,0.1):SetEase(core.tween.Ease.Flash):SetId(self):OnComplete(function()
            self.gameObject:SetActiveEx(false)
        end)
        --self.mask:DOFade(0,0):SetEase(core.tween.Ease.Flash):SetId(self)
        self.mask:DOFade(0, 0.2):SetEase(core.tween.Ease.Flash):SetId(self)
    end
end

function UIAddMenuView:OnEditorRolesClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_role_new,true)
end

function UIAddMenuView:OnAddDecisionClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_selection_add)
end

function UIAddMenuView:OnAddImageClick()
    self:Hide()
    logic.EventDispatcher:Broadcast(logic.EventName.on_story_editor_pickimage)
end

return UIAddMenuView