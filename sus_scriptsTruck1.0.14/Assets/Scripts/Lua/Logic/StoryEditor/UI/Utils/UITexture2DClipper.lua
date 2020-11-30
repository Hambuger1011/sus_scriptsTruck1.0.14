--[[
    图片裁剪
]]
local BaseClass = core.Class
local UIView = core.UIView
---@class UITexture2DClipper
local UITexture2DClipper = BaseClass("UITexture2DClipper", UIView)
local base = UIView

local uiid = logic.uiid

local BubbleData = require('Logic/StoryEditor/UI/Editor/BubbleItem/BubbleData')
local UIBubbleItem = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleItem')

local DataDefine = require('Logic/StoryEditor/Data/DataDefine')
local StoryData = require('Logic/StoryEditor/Data/StoryData')
local EBubbleType = DataDefine.EBubbleType
local EBubbleBoxType = DataDefine.EBubbleBoxType

UITexture2DClipper.config = {
	ID = uiid.Texture2DClipper,
	AssetName = 'UI/StoryEditorRes/UI/Canvas_ImageCut'
}


function UITexture2DClipper:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.root = self.uiBinding:Get('root').transform
    self.clipperRoot = self.uiBinding:Get('clipperRoot')
    self.clipperScrollView = self.uiBinding:Get('clipperRoot',typeof(logic.cs.ScrollRect))
    self.clipperImage = self.uiBinding:Get('clipperImage',typeof(logic.cs.Image))

    self.mask_bg = self.uiBinding:Get('mask_bg',typeof(CS.UIMaskImage))
    self.mask_handle = self.clipperScrollView.transform--self.uiBinding:Get('mask_handle').transform
    self.maskSize = self.mask_handle.rect.size
    self.maskOffset = self.mask_handle.anchoredPosition.y

    
    self.btnOK = self.uiBinding:Get('btnOK',typeof(logic.cs.UITweenButton))
    self.btnCancel = self.uiBinding:Get('btnCancel',typeof(logic.cs.UITweenButton))
    self.btnOK.onClick:AddListener(function()
        self:__Close()
        
        local w = 0
        local h = 0
        local x = 0
        local y = 0
        local pos = self.clipperImage.transform.anchoredPosition
        if self.vertical then   --裁剪vertical
            w = self.imgSize.x
            h = math.floor(w / self.ratio)
            x = 0
            y = math.floor((self.imgSize.y - h) * self.clipperScrollView.verticalNormalizedPosition)
        else    --裁剪horizontal
            h = self.imgSize.y
            w = math.floor(h * self.ratio)
            x = math.floor((self.imgSize.x - w) * self.clipperScrollView.horizontalNormalizedPosition)
            y = 0
        end
        if (x < 0) then
            x = 0
        end
        if(w < 0) then
            w = 1
        end
        if(h < 0) then
            h = 1
        end
        self.callback(true,x,y,w,h)
    end)
    self.btnCancel.onClick:AddListener(function()
        self:__Close()
        self.callback(false,nil)
    end)

    self:InitClipperRoot()
end


function UITexture2DClipper:OnOpen()
    UIView.OnOpen(self)
end

function UITexture2DClipper:OnClose()
end

function UITexture2DClipper:InitClipperRoot()
    local viewScale = self.uiform:GetScale()
    local viewSize = self.uiform:GetViewSize()
    --local resolution = self.uiform.m_referenceResolution

    -- local anchoredPosition = self.mask_handle.transform.anchoredPosition
    -- local innerRect = self.mask_handle.rect
    -- --local offset = self.mask_handle.pivot - core.Vector2.New(0.5,0.5)
    -- local pos = core.Vector2.New(-innerRect.width*0.5,-innerRect.height*0.5)
    -- pos.y = -anchoredPosition.y + pos.y
    -- innerRect.position = pos
    
    local s = self.maskSize--core.Vector2.New(700,1104)
    local innerRect = CS.UnityEngine.Rect(-s.x*0.5,-s.y*0.5,s.x,s.y)
    local pos = innerRect.position
    pos.y = viewSize.y*0.5 - s.y + self.maskOffset
    pos.y = pos.y + self.root.offsetMax.y
    innerRect.position = pos

    local outterRect = CS.UnityEngine.Rect(-viewSize.x*0.5,-viewSize.y*0.5,viewSize.x,viewSize.y)
    self.mask_bg:SetRectangle(innerRect, outterRect)
    --self.mask_bg:Test2()

    
    local t = self.clipperImage.transform
    logic.cs.LuaHelper.SetUISize(t, 0, 0)
end

function UITexture2DClipper:SetData(sprite,callback)
    self.callback = callback
    local uiImage = self.clipperImage
    local defaultSize = self.maskSize--core.Vector2.New(700,1104)
    uiImage.sprite = sprite
    local imgSize = sprite.rect.size
    imgSize = core.Vector2.New(imgSize.x, imgSize.y) / uiImage.pixelsPerUnit --图片真实分辨率

    local imgRatio = (imgSize.x / imgSize.y)    --图片比例
    local ratio = (defaultSize.x / defaultSize.y)   --图片框比例
    self.imgSize = imgSize:Clone()
    self.ratio = ratio

    if imgRatio < ratio then    --y超出框体
        imgSize.x = defaultSize.x
        imgSize.y = defaultSize.x / imgRatio
        self.clipperScrollView.horizontal = false
        self.clipperScrollView.vertical = true
        self.vertical = true
    else    --x超出框体
        imgSize.y = defaultSize.y
        imgSize.x = defaultSize.y * imgRatio
        self.clipperScrollView.horizontal = true
        self.clipperScrollView.vertical = false
        self.vertical = false
    end
    local t = uiImage.transform
    logic.cs.LuaHelper.SetUISize(t, imgSize.x, imgSize.y)
end


return UITexture2DClipper