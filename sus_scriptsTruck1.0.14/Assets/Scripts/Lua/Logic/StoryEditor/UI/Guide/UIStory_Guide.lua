local BaseClass = core.Class
local UIView = core.UIView
---@class UIStory_Guide
local UIStory_Guide = BaseClass("UIStory_Guide", UIView)
local base = UIView

local uiid = logic.uiid

UIStory_Guide.config = {
	ID = uiid.Story_Guide,
	AssetName = 'UI/Resident/UI/Canvas_Story_Guide'
}


function UIStory_Guide:OnInitView()
    UIView.OnInitView(self)
    self.maxStep = 10
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    for i=1,10 do
        local go = self.uiBinding:Get('step_'..i)
        self['step_'..i] = go
        local btnNext = go.transform:Find('btnNext'):GetComponent(typeof(logic.cs.Button))
        local btnPrevious = go.transform:Find('btnPrevious'):GetComponent(typeof(logic.cs.Button))
        btnNext.onClick:AddListener(function()
            self:OnNextStepClick()
        end)
        btnPrevious.onClick:AddListener(function()
            self:OnPreStepClick()
        end)
    end
end


function UIStory_Guide:OnOpen()
    UIView.OnOpen(self)
    self:GotoStep(1)
end

function UIStory_Guide:OnClose()
end

function UIStory_Guide:GotoStep(index)
    self.stepIndex = index
    for i=1, self.maxStep do
        self['step_'..i]:SetActiveEx(index == i)
    end
end


function UIStory_Guide:OnNextStepClick()
    local step = self.stepIndex + 1
    if step > self.maxStep then
        self:OnGuideFinish()
    else
        self:GotoStep(step)
    end
end


function UIStory_Guide:OnPreStepClick()
    local step = self.stepIndex - 1
    self:GotoStep(step)
end

function UIStory_Guide:OnGuideFinish()
    logic.gameHttp:StoryEditor_UpdateGuide(function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
        else
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
        logic.cs.UserDataManager.userInfo.data.userinfo.writer_guide = 1
        self:__Close()
    end)
end

return UIStory_Guide