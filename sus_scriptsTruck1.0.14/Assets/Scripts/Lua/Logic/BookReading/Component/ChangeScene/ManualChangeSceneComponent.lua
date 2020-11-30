local Class = core.Class
local Base = logic.BookReading.BaseComponent

---@class ManualChangeSceneComponent
---@field cfg pb.t_BookDialog
local ManualChangeSceneComponent = Class("ManualChangeSceneComponent", Base)
local ui = nil


--收集所用到的资源
function ManualChangeSceneComponent:CollectRes(resTable)
    Base.CollectRes(self,resTable)
    resTable["Assets/Bundle/UI/BookReading/ManualChangeScene.prefab"] = BookResType.UIRes
    resTable[logic.bookReadingMgr.Res.bookCommonPath.."Atlas/Dialog/bg_chat.png"] = BookResType.BookRes
end


function ManualChangeSceneComponent:Clean()
	Base.Clean(self)
	ui = nil
end

function ManualChangeSceneComponent.InstallUI()
    if ui then
        return
    end
    local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/ManualChangeScene.prefab")
    ui = {}
    ui.gameObject = logic.cs.GameObject.Instantiate(prefab,logic.bookReadingMgr.view.transComponent,false)
    ui.gameObject:SetActiveEx(false)
    ui.transform = ui.gameObject.transform
    ui.Reset = function()
        --ui.gameObject:SetActiveEx(false)
    end
    ui.arrow = {}
    ui.arrow[1] = ui.transform:Find('Left').gameObject
    ui.arrow[2] = ui.transform:Find('Up').gameObject
    ui.arrow[3] = ui.transform:Find('Right').gameObject
    ui.arrow[4] = ui.transform:Find('Down').gameObject
    ui.uiDrag = ui.gameObject:GetComponent(typeof(CS.UIDrag))
end


function ManualChangeSceneComponent:Play()
    self.InstallUI()
    ui.Reset()
    for i=1,#ui.arrow do
        ui.arrow[i]:SetActiveEx(i <= self.cfg.trigger)
    end
    ui.gameObject:SetActiveEx(true)
    local view = logic.bookReadingMgr.view
    local lastSceneBG = view:ExchangeSceneBG()
    view.curSceneBG.canvasGroup.alpha = 1
    lastSceneBG.canvasGroup.alpha = 1

    local img = view.curSceneBG.image
    local spt = logic.bookReadingMgr.Res:GetSceneBG(self.cfg.sceneID)
    img.sprite = spt
    img.color = logic.cs.StringUtils.HexToColor(self.cfg.sceneColor)
    --img:SetNativeSize()
    --img.color = core.Color.white
    local size = logic.cs.LuaHelper.GetSpriteSize(spt)
    local viewSize = view.uiform:GetViewSize()
    size = core.Vector2.New(size.x * viewSize.y / size.y,viewSize.y)
    view.curSceneBG.transform.sizeDelta = size
    local LRPos = (size.x + viewSize.x) * 0.5
    local UDPos = viewSize.y
    if self.cfg.trigger == 1 then
        view.curSceneBG.transform.anchoredPosition = core.Vector2.New(LRPos,0)
    elseif self.cfg.trigger == 2 then
        view.curSceneBG.transform.anchoredPosition = core.Vector2.New(logic.bookReadingMgr.view:GetPiexlX(self.cfg.Scenes_X),-UDPos)
    elseif self.cfg.trigger == 3 then
        view.curSceneBG.transform.anchoredPosition = core.Vector2.New(-LRPos,0)
    elseif self.cfg.trigger == 4 then
        view.curSceneBG.transform.anchoredPosition = core.Vector2.New(logic.bookReadingMgr.view:GetPiexlX(self.cfg.Scenes_X),UDPos)
    end
    
    self.fingerTouchState = 0
    ui.uiDrag.onDown = function(data)
        self:onDown(data)
    end
    ui.uiDrag.onUp = function(data)
        self:onUp(data)
    end
    ui.uiDrag.onDrag = function(data)
        self:onDrag(data)
    end
end

function ManualChangeSceneComponent:onDown(data)
    if self.fingerTouchState == 0 then
        self.fingerTouchState = 1
        self.fingerBeginPos = data.position
    end
end

function ManualChangeSceneComponent:onUp(data)
    self.fingerTouchState = 0
end

function ManualChangeSceneComponent:onDrag(data)
    if self.fingerTouchState ~= 1 then
        return
    end
    local view = logic.bookReadingMgr.view
    local viewSize = view.uiform:GetViewSize()
    local pos = data.position
    self.fingerSegment = pos - self.fingerBeginPos
    local fingerActionSensitivity = viewSize.x * 0.05 --手指动作的敏感度，这里设定为 二十分之一的屏幕宽度.
    if self.fingerSegment.sqrMagnitude > fingerActionSensitivity * fingerActionSensitivity then
        self:ToAddFingerAction()
    end
end

function ManualChangeSceneComponent:ToAddFingerAction()
    self.fingerTouchState = 2
    if core.Mathf.Abs(self.fingerSegment.x) > core.Mathf.Abs(self.fingerSegment.y) then
        self.fingerSegment.y = 0
    else
        self.fingerSegment.x = 0
    end
    for i=1,#ui.arrow do
        ui.arrow[i]:SetActiveEx(false)
    end
    self:ManualChangeSceneMove(self.cfg.trigger)
end


function ManualChangeSceneComponent:ManualChangeSceneMove(type)
    local view = logic.bookReadingMgr.view
    local unuseSceneBG = view:GetUnuseSceneBG()
    unuseSceneBG.transform:ClearAllChild()

    local sceneBG = view.curSceneBG
    local toPos = core.Vector2(logic.bookReadingMgr.view:GetPiexlX(self.cfg.Scenes_X),0)
    sceneBG.transform:DOAnchorPos(toPos,1):OnComplete(function()
        view:SetSceneBG(self,sceneBG)
        sceneBG.transform.anchoredPosition = toPos
        unuseSceneBG.gameObject:SetActiveEx(false)
        ui.gameObject:SetActiveEx(false)
        self:ShowNextDialog()
    end)
end

return ManualChangeSceneComponent