--[[
    主面板
]]
local BaseClass = core.Class
local UIView = core.UIView
local UIStory_Keyboard = BaseClass("UIStory_Keyboard", UIView)
local base = UIView
local uiid = logic.uiid

UIStory_Keyboard.config = {
	ID = uiid.Story_Keyboard,
	AssetName = 'UI/StoryEditorRes/UI/Canvas_Keyboard'
}
local cs_keyboard = CS.UnityEngine.TouchScreenKeyboard

function UIStory_Keyboard:OnInitView()
    UIView.OnInitView(self)
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.messageTf = self.uiBinding:Get('message').transform
    self.messageSize = self.messageTf.rect.size

    self.input = self.uiBinding:Get('input', typeof(logic.cs.InputField))
    self.input.shouldHideMobileInput = true
    
    self.input.onEndEdit:AddListener(function(val)
        logic.debug.Log('end:'..val)
    end)
end

function UIStory_Keyboard:OnOpen()
    UIView.OnOpen(self)

    CS.Mopsicus.Plugins.MobileInput.OnPrepareKeyboard = function()
        self:OnPrepareKeyboard()
    end
    CS.Mopsicus.Plugins.MobileInput.OnShowKeyboard = function( isShow,  height)
        self.lastHeight = 0
        self:OnShowKeyboard ( isShow,  height)
    end
    self:SetPosition()
end

function UIStory_Keyboard:OnClose()
    UIView.OnClose(self)
    if self.coSetPosition then
        core.coroutine.stop(self.coSetPosition)
        self.coSetPosition = nil
    end
end

function UIStory_Keyboard:OnPrepareKeyboard ()
   --logic.debug.Log ("Keyboad will show")
end

function UIStory_Keyboard:OnShowKeyboard ( isShow,  height)
    --logic.debug.Log ("Keyboad action, show = {0}, height = {1}", isShow, height)
    --local scale = self.uiform:GetScale()
    local offset = self.uiform:Pixel2View(core.Vector2.New(0,height))

    logic.debug.LogError('---height:'..tostring(height)..','..offset.y)
    
    -- height = CS.ResolutionAdapter.GetKeyboardHeight()
    -- offset = self.uiform:Pixel2View(core.Vector2.New(0,height))
    -- logic.debug.LogError('---height:'..tostring(height)..','..offset.y)

    -- height = offset.y + 10

    -- local uiform = self.uiform
    -- local viewSize = uiform.m_referenceResolution
    -- self.messageTf.anchoredPosition = core.Vector2.New(0,height - viewSize.y * 0.5 + self.messageSize.y * 0.5)

end

function UIStory_Keyboard:SetPosition()
    if self.coSetPosition then
        return
    end
    self.coSetPosition = core.coroutine.start(function()
        while true do
            --local height = CS.ResolutionAdapter.GetKeyboardHeight()
            local height = cs_keyboard.area.y
            --local scale = self.uiform:GetScale()
            local offset = self.uiform:Pixel2View(core.Vector2.New(0,height))

            if self.lastHeight ~= offset.y then
                self.lastHeight = offset.y
                logic.debug.LogError('*height:'..tostring(height)..','..offset.y)
            end
            height = offset.y

            local uiform = self.uiform
            -- local viewSize = uiform:GetViewSize()
            -- local size = tf.rect.size

            -- local pos = logic.cs.CUIUtility.World_To_UGUI_LocalPoint(
            --     uiform:GetCamera(), 
            --     uiform:GetCamera(), 
            --     tf.position, 
            --     uiform.transform
            -- )
            -- local offset = core.Vector2.New(0.5,0.5) - tf.pivot
            -- pos.x = 0 --pos.x + offset.x * size.x
            -- pos.y = pos.y + offset.y * size.y
            -- pos.y = pos.y  - self.bodyTrans.rect.size.y * 0.5
            local viewSize = uiform.m_referenceResolution
            self.messageTf.anchoredPosition = core.Vector2.New(0,height - viewSize.y * 0.5 + self.messageSize.y * 0.5)

            -- if not cs_keyboard.active then
            --     self:__Close()
            --     break
            -- end
            core.coroutine.step()
        end
        self.coSetPosition = nil
        logic.debug.LogError('---end----')
    end)
end

return UIStory_Keyboard