local Class = core.Class
local UIView = core.UIView
local UIBookReadingView = Class("UIBookReadingView", UIView)
local base = UIView

local BookReadingFormTopBar = require('Logic/UI/UI_BookReading/BookReadingFormTopBar')
local AdsTicker = require('Logic/UI/UI_BookReading/AdsTicker')
local UIChoiceButtonGroup = require('Logic/UI/UI_BookReading/ChoiceGroup/UIChoiceButtonGroup')
local ChapterSwitch = require('Logic/UI/UI_BookReading/ChapterSwitch')

local uiid = logic.uiid
UIBookReadingView.config = {
	ID = uiid.BookReading,
	AssetName = "Assets/Bundle/UI/BookReading/Canvas_BookReading.prefab",
}


---@class SceneBG
local SceneBG = Class('SceneBG')
SceneBG.__init = function(self,root)
    self.transform = root
    self.gameObject = root.gameObject
    self.canvasGroup = root:GetComponent(typeof(logic.cs.CanvasGroup))
    self.image = root:GetComponent(typeof(logic.cs.Image))
    self.uidepth = root:GetComponent(typeof(logic.cs.UIDepth))
    self.size = core.Vector2.zero
    self.offset = core.Vector2.zero
end
function SceneBG:Clear()
    self.gameObject:SetActiveEx(false)
    self.transform:ClearAllChild()
    self.image.sprite = nil
    self.image.color = core.Color.black
end

function UIBookReadingView:OnInitView()
    UIView.OnInitView(self)
    
    self.resolution = self.uiform.m_referenceResolution
    self.viewScale = self.uiform:GetScale()
    self.viewSize = self.uiform:GetViewSize()

    local get = logic.cs.LuaHelper.GetComponent
    local root = self.uiform.transform
    self.transLayer = root:Find('Frame/Layer')--get(root,"Frame/Layer",typeof(logic.cs.Image))
    self.transComponent = root:Find('Frame/Layer/Component')
    self.eggTransComponent = root:Find('EggsComponent')
    ---@type SceneBG
    self.curSceneBG = nil
    self.sceneBGArray = {
        SceneBG.New(root:Find("Frame/SceneBG1")),
        SceneBG.New(root:Find("Frame/SceneBG2")),
    }
    self.btnSceneTouch = get(root,"Frame/SceneTochListenerImag",typeof(logic.cs.Button))
    logic.cs.LuaHelper.AddClick(self.btnSceneTouch,function()
        self:ResetOperationTips()
        logic.bookReadingMgr:OnSceneClick()
    end)


    self.adsTicker = AdsTicker.New(root:Find('Frame/Layer/AdsTicker'))
    self.btnBackMain = get(root,'Frame/Layer/TopBar/TopBarButton/TopGB/TopRect/LeftBar/BackButton',typeof(logic.cs.Image))
    self.BarrageOpenClosegame = get(root,'Frame/Layer/BottomBar/GameObject/Bg/Broadcast',typeof(logic.cs.Image))
    self.btnSend = get(root,'Frame/Layer/BottomBar/GameObject/Bg/InputFieldBg/SendButton',typeof(logic.cs.Image))
    self.choiceGroup = UIChoiceButtonGroup.New(root:Find('Frame/Layer/ChoiceButtonGroup'))
    --self.topBar = BookReadingFormTopBar.New(root:Find('Frame/Layer/TopBar'))
    self.topBar = get(root,'Frame/Layer/TopBar',typeof(CS.BookReadingFormTopBarController))
    self.bottomBar = get(root,'Frame/Layer/BottomBar',typeof(CS.BookReadingBottomBar))
    self.OperationTipsGo = root:Find('Frame/Layer/ClickTips').gameObject
    self.bookProgress = get(root,'Frame/Layer/Pass/Image',typeof(logic.cs.Image))
    self.chapterSwitch = ChapterSwitch.New(root:Find('Frame/Layer/ChapterSwitch'))
    self.tipsImage = get(root,'Frame/Layer/TipsImage',typeof(CS.BookReading.UITipsImage))

    logic.cs.MyBooksDisINSTANCE:BarrageInit()
    self.topBar:Init()

    --返回主界面
    logic.cs.UIEventListener.AddOnClickListener(self.btnBackMain.gameObject, function(data)
        self:OnBackToMainClick()
    end)

    --打开弹幕
    logic.cs.UIEventListener.AddOnClickListener(self.BarrageOpenClosegame.gameObject, function(data)
        --self:OnBarrageOpenCloseClick()
    end)

    --发送弹幕
    logic.cs.UIEventListener.AddOnClickListener(self.btnSend.gameObject, function(data)
        self:OnSendButtonOnclick()
    end)
    --logic.cs.UIEventListener.AddOnClickListener(closinputButton, closinputButtonOnclick);
    --InputFieldSend.onValueChanged.AddListener(InputChangeHandler);
    --InputField.onEndEdit.AddListener(ReplyButtonOnclicke);

end

function UIBookReadingView:OnOpen()
    UIView.OnOpen(self)
end

function UIBookReadingView:OnClose()
    UIView.OnClose(self)
	
	--self.topBar:Delete()
    self.topBar:Dispose()
	self.topBar = nil
    
    if self.adsTicker then
        self.adsTicker:Delete()
        self.adsTicker = nil
    end
    
    if self.operationTipsTimer then
		self.operationTipsTimer:Stop()
        self.operationTipsTimer = nil
    end

end

function UIBookReadingView:StartReading()
    self:InitSceneBG()
    if logic.config.channel == Channel.Spain then
        self.adsTicker:Start()
    end
    self:StartOperationTips()


    logic.cs.MyBooksDisINSTANCE:GetAllGameInite():Clear()
    logic.cs.MyBooksDisINSTANCE:GetNowUseGame():Clear()
    logic.cs.MyBooksDisINSTANCE:SetIsPlaying(true)
    self:NewBookTipsChange()
end

function UIBookReadingView:InitSceneBG()
    self.curSceneBG = self.sceneBGArray[1]
    self.curSceneBG.gameObject:SetActiveEx(true)
    self.useSceneIdx = 1
    for i = 2, #self.sceneBGArray do
        self.sceneBGArray[i].gameObject:SetActiveEx(false)
    end
end


function UIBookReadingView:ChangeSceneWithoutTween(component)
    self:SetSceneBG(component,self.curSceneBG)
end


---@param curSceneBG SceneBG
function UIBookReadingView:SetSceneBG(component,curSceneBG)
    if component == nil then
        logic.debug.LogError("component is nil")
        return
    end
    local img = curSceneBG.image
    if not component.cfg.sceneID then
        component.cfg.sceneID = ''
    end
    img.sprite = logic.bookReadingMgr.Res:GetSceneBG(component.cfg.sceneID)
    img.color = logic.cs.StringUtils.HexToColor(component.cfg.sceneColor)
    curSceneBG.transform:ClearAllChild()

    local sceneParticalArray = component.cfg.SceneParticalsArray
    if sceneParticalArray then
        for i = 1,#sceneParticalArray do
            local prefab = logic.bookReadingMgr.Res:GetPrefab(logic.bookReadingMgr.Res.bookFolderPath.."UIParticle/" .. sceneParticalArray[i]..".prefab")
            if prefab then
                local go = logic.cs.GameObject.Instantiate(prefab, curSceneBG.transform)
                local t = go.transform
                t:ResetTransform();
                t.localScale = core.Vector3.New(self.viewScale.x,self.viewScale.y,1)
            end
        end
    end

    curSceneBG.uidepth:ResetOrder()
    curSceneBG.size = self:changeBGSize(img)
end

function UIBookReadingView:changeBGSize(img)
    local sprite = img.sprite
    if logic.IsNull(sprite) then
        return core.Vector2.zero
    end
    local imgSize = sprite.rect.size
    imgSize = core.Vector2.New(imgSize.x, imgSize.y) / img.pixelsPerUnit --图片真实分辨率

    local ratio = (imgSize.x / imgSize.y)
    imgSize.x = self.viewSize.y * ratio * self.viewScale.x
    imgSize.y = self.viewSize.y
    local t = img.rectTransform
    --t.anchorMax = t.anchorMin
    --t.sizeDelta = imgSize
    --t.anchoredPosition = core.Vector2.New((imgSize.x - 750)/2,0)
    t:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, imgSize.x)
    t:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical,   imgSize.y)
    t.anchoredPosition = core.Vector2.New(0,0)
    return imgSize
end

function UIBookReadingView:sceneBGMove(toX,callback)
    local duration = 0.4
    local trans = self.curSceneBG.transform
    trans:DOKill()
    if Mathf.Abs(trans.anchoredPosition.x - toX) < 0.1 then
        trans.anchoredPosition = core.Vector2.New(toX,0)
        if callback then callback() end
    else
        --logic.debug.LogError('move:from'..trans.anchoredPosition.x..'->'..toX.."|"..Mathf.Abs(trans.anchoredPosition.x - toX))
        trans:DOAnchorPos(core.Vector2.New(toX,0), duration)
        :OnUpdate(function()
            logic.cs.BookReadingWrapper.IsTextTween = true
        end)
        :SetEase(core.tween.Ease.Flash)
        :OnComplete(function()
            if callback then callback() end
        end):Play()
    end
end

function UIBookReadingView:textDialogTween(component,DialogBox,DialogText,offsetH,value,callback)
    local duration = 0.2
    local basePadding = 120
    if logic.bookReadingMgr.bookDetailCfg then
        basePadding = basePadding + logic.bookReadingMgr.bookDetailCfg.DialogFrameHeight
    end
    DialogText.text = value
    local rectHeight = DialogText.preferredHeight + basePadding + offsetH
    --local rectHeight = uiText:GetPreferredHeight(value) + basePadding + offsetH
    --logic.debug.LogError('rectHeight='..tostring(rectHeight))
    if rectHeight < 180 then
        rectHeight = 180
    end
    
    local width = 650
    DialogText.text = ""
    DialogBox.sizeDelta = core.Vector2.New(width,160)

    --显示对话窗口
    --缩小tween
    local onComplete = function()
        --缩放到原始尺寸
        core.tween.DoTween.To(function()
            return DialogBox.sizeDelta.y
        end,function(height)
            DialogBox.sizeDelta = core.Vector2.New(width,height)
        end,rectHeight, duration * 0.5)
        :SetEase(core.tween.Ease.Flash)

        DialogText.gameObject:SetActiveEx(true)
        DialogText.text = value
        --logic.debug.LogError('set text:'..value)
        if self.doubleClck then
            DialogText.Progress = 1
        else
            DialogText:DoTyperTween()
        end
        if component.cfg.is_tingle == 2 then --文字抖动
            DialogText:DoShake()
        end
        if callback then
            callback()
        end
    end

    --放大tween
    core.tween.DoTween.To(function()
        return DialogBox.sizeDelta.y
    end,function(height)
        DialogBox.sizeDelta = core.Vector2.New(width,height)
    end,rectHeight * 1.05, duration)
    :OnUpdate(function()
        logic.cs.BookReadingWrapper.IsTextTween = true
    end)
    :SetEase(core.tween.Ease.Flash)
    :OnComplete(onComplete)
    :Play()
end

function UIBookReadingView:OnBackToMainClick()
    logic.bookReadingMgr:BackToMainClick()
end

function UIBookReadingView:ExchangeSceneBG()
    local lastSceneIdx = self.useSceneIdx
    ---@type SceneBG
    local lastSceneBG = self.sceneBGArray[lastSceneIdx]
    self.useSceneIdx = self.useSceneIdx + 1
    if self.useSceneIdx > 2 then
        self.useSceneIdx = 1
    end
    self.curSceneBG = self.sceneBGArray[self.useSceneIdx]
    self.curSceneBG.gameObject:SetActiveEx(true)
    lastSceneBG.transform:SetAsFirstSibling()



    return lastSceneBG
end

function UIBookReadingView:GetUnuseSceneBG()
    local idx = self.useSceneIdx - 1
    if idx < 1 then
        idx = 2
    end
    return self.sceneBGArray[idx]
end

function UIBookReadingView:StartOperationTips()
    self:ResetOperationTips()
    self.operationTipsTimer = core.Timer.New(function()
        if logic.cs.GameDataMgr.InAutoPlay then
            self.operationTipsTick = 0
        else
            self.operationTipsTick = self.operationTipsTick + 1
            if not self.isOperationTips and self.operationTipsTick > 15 then
                self.isOperationTips = true
                self.OperationTipsGo:SetActiveEx(true)
            end
        end
	 end,1,-1)
	 self.operationTipsTimer:Start()
end

function UIBookReadingView:ResetOperationTips()
    self.isOperationTips = false
    self.OperationTipsGo:SetActiveEx(false)
    self.operationTipsTick = 0
end

function UIBookReadingView:StopOperationTips()
    self.isOperationTips = false
    self.OperationTipsGo:SetActiveEx(false)
    self.operationTipsTick = -99999999
end


---@param component BaseComponent
function UIBookReadingView:updateReadingProgress(component)
    local bookDetailCfg = logic.bookReadingMgr.Res.bookDetailCfg
    local progress = 0
    if component.cfg.chapterID <= 1 then
        local chapterInfo = logic.cs.JsonDTManager:GetJDTChapterInfo(bookDetailCfg.id,1)
        if chapterInfo ~= nil then
            progress = component.cfg.dialogID / chapterInfo.chapterfinish
        end
        
    else
        
        local chapterInfo = logic.cs.JsonDTManager:GetJDTChapterInfo(bookDetailCfg.id,component.cfg.chapterID)
        if chapterInfo ~= nil then
            local beginID = chapterInfo.chapterstart
            local endID = chapterInfo.chapterfinish
            local num = component.cfg.dialogID - beginID
            local sum = endID - beginID
            progress = num / sum
        end
    end
    self.bookProgress.fillAmount = progress
end

function UIBookReadingView:NewBookTipsChange()
    local cfg = logic.bookReadingMgr.Res.bookDetailCfg
    logic.cs.MyBooksDisINSTANCE:setMyBooksFirstId(cfg.id)
    logic.cs.PlayerPrefs.SetInt("BookChapterCount" .. cfg.id, cfg.chaptercount)
    logic.debug.Log("BookChapterCount:id=" .. cfg.id..",count=".. cfg.chaptercount)
end

function UIBookReadingView:GetPiexlX(x)
    if not x then
        return 0
    end
    return self.viewScale.x * x
end

function UIBookReadingView:GetPiexlY(y)
    if not y then
        return 0
    end
    return self.viewScale.y * y
end

function UIBookReadingView:ShowChargeTips(cost)
    local type = logic.cs.MyBooksDisINSTANCE:GameOpenUItype()
        if type == 1 then
            local userInfo = logic.cs.UserDataManager.userInfo
            if userInfo and userInfo.data and userInfo.data.userinfo.newpackage_status == 1 then
                local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.FirstGigtGroup);
                local c = uiform:GetComponent(typeof(CS.FirstGigtGroup))
                c:GetType(1);
            end
        elseif type == 2 then
            logic.cs.MyBooksDisINSTANCE:VideoUI()
        else
            if logic.config.channel == Channel.Spain then
                local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.ChargeTipsForm);
                local tipForm = uiform:GetComponent(typeof(CS.ChargeTipsForm))
                tipForm:Init(2, cost, cost * 0.99)
            else
                local uiform = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.NewChargeTips);
                local tipForm = uiform:GetComponent(typeof(CS.NewChargeTips))
                tipForm:Init(2, cost, cost * 0.99)
            end
        end
end

function UIBookReadingView:BgAddBlurEffect(isOn)
    local img = self.curSceneBG.image
    img.material = (isOn and CS.ShaderUtil.BlurEffevtMaterial()) or nil
end

function UIBookReadingView:SetBottomActive(isOn)
    self.bottomBar.gameObject:SetActiveEx(isOn)
end

return UIBookReadingView