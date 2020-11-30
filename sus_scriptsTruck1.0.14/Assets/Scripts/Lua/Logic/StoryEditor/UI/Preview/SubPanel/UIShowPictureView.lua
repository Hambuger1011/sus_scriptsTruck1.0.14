local BaseClass = core.Class

---@class UIStory_ShowPictureView
local UIShowPictureView = BaseClass("UIShowPictureView")

function UIShowPictureView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    self.btnPictureShow = self.uiBinding:Get('btnPictureShow', typeof(logic.cs.Button))
    self.imgPictureShow = self.uiBinding:Get('imgPicture', typeof(logic.cs.Image))

    self.btnPictureShow.onClick:AddListener(function()
        self:Hide()
    end)
end

function UIShowPictureView:Hide()
    self.isActive = false
    self.gameObject:SetActiveEx(false)
end

---@param uiItem UIBubbleItem
function UIShowPictureView:Show(uiform, uiItem)
    self.isActive = true
    self.gameObject:SetActiveEx(true)

    
    local srcImg = uiItem.box.activeBox.imgPicture
    local viewSize = uiform:GetViewSize()

    local size = srcImg.transform.rect.size

    local pos = logic.cs.CUIUtility.World_To_UGUI_LocalPoint(
        uiform:GetCamera(), 
        uiform:GetCamera(), 
        srcImg.transform.position, 
        uiform.transform
    )
    local offset = core.Vector2.New(0.5,0.5) - srcImg.transform.pivot
    pos.x = pos.x + offset.x * size.x
    pos.y = pos.y + offset.y * size.y
    local maskTf = self.btnPictureShow.transform
    maskTf.sizeDelta = size
    maskTf.anchoredPosition = pos
    --maskTf.gameObject:SetActiveEx(true)
    maskTf:DOAnchorPos(core.Vector2.New(0,0),0.25)
    :OnComplete(function()
    end):SetEase(core.tween.Ease.Flash)
    maskTf:DOSizeDelta(viewSize,0.25):SetEase(core.tween.Ease.Flash)
    self.imgPictureShow.sprite = srcImg.sprite
    self.imgPictureShow.transform.sizeDelta = size
    self.imgPictureShow.transform:DOSizeDelta(core.Vector2.New(viewSize.x, size.y / size.x * viewSize.x),0.25):SetEase(core.tween.Ease.Flash)
end

return UIShowPictureView