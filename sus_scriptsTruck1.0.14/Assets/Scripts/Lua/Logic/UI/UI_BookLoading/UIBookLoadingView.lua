local BaseClass = core.Class
local UIView = core.UIView
local UIBookLoadingView = BaseClass("UIBookLoadingView", UIView)
local base = UIView

local uiid = logic.uiid
UIBookLoadingView.config = {
	ID = uiid.BookLoading,
	AssetName = "UI/Resident/UI/Canvas_BookLoading",
}


function UIBookLoadingView:OnInitView()
    UIView.OnInitView(self)
    self.uiform.canvasScaler.screenMatchMode = logic.cs.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight
    self.LoadDuration = 2
    local root = self.uiform.transform
    self.LogoImage = logic.cs.LuaHelper.GetComponent(root,"Canvas/IconBG",typeof(logic.cs.Image))
    self.LoadingBG = logic.cs.LuaHelper.GetComponent(root,"Canvas/BG",typeof(logic.cs.Image))
    self.progressBar = logic.cs.LuaHelper.GetComponent(root,"Canvas/ProgressBar",typeof(CS.ProgressBar))
    self.Percentage = logic.cs.LuaHelper.GetComponent(root,"Canvas/ProgressBar/Text",typeof(logic.cs.Text))
    self.btnBack = logic.cs.LuaHelper.GetComponent(root,"Canvas/btnBack",typeof(logic.cs.UITweenButton))
    self.ResSizeText = logic.cs.LuaHelper.GetComponent(root,"Canvas/ProgressBar/ResSizeText",typeof(logic.cs.Text))
    self.btnBack.onClick:AddListener(function()
        self:OnBackToMainClick()
    end)
    self.imgBack = self.btnBack:GetComponent(typeof(logic.cs.Image))
    self.imgBack:DOFade(0,0)
    self.imgBack.gameObject:SetActiveEx(false)
end


function UIBookLoadingView:OnClose()
    UIView.OnOpen(self)
    if self.backButtonTimer ~= nil then
        self.backButtonTimer:Stop()
        self.backButtonTimer = nil
    end
    core.UpdateBeat:Remove(self.updateHandle)
    self.progressBar.transform:DOKill()
    if self.CoverRefCount ~= nil then
        self.CoverRefCount:Release()
        self.CoverRefCount = nil
    end
end

function UIBookLoadingView:OnOpen()
    self.m_progress = 0
    self.m_toProgress = 0
    self.isComplete = false
    self.tweener = nil

    --[[
    local t = self.LoadingBG.transform
    if logic.cs.GameUtility.IsLongScreen() then
        t.anchorMax = core.Vector2.New(0.5, 1);
        t.anchorMin = core.Vector2.New(0.5, 0);
        t.offsetMax = core.Vector2.New(t.offsetMax.x, 0);
        t.offsetMin = core.Vector2.New(t.offsetMin.x, 0);
    else
        t.anchorMax = core.Vector2.New(0.5, 0.5);
        t.anchorMin = core.Vector2.New(0.5, 0.5);
    end
    --]]
    self.updateHandle = core.UpdateBeat:Add(self.Update,self)

    self.LogoImage.gameObject:SetActiveEx(false)
    self:UiTween()
    self:SetProgress(0,0,true)

    if logic.config.channel == Channel.Spain then
        logic.cs.SdkMgr.ads:HideBanner()
    end
    self.backButtonTimer = core.Timer.New(function()
        self.imgBack.gameObject:SetActiveEx(true)
        self.imgBack:DOFade(1,1)
    end,10)
    self.backButtonTimer:Start()
end

function UIBookLoadingView:Update()
    if(self.isComplete)then
        return
    end

    local p = logic.bookReadingMgr:GetLoadResProgress()
    if logic.bookReadingMgr:IsLoadResDone() then
        self.isComplete = true
    end
    
    local _allsize=logic.bookReadingMgr:GetAllSize()
    if(_allsize>0)then
        self:DoProgress(p,_allsize)
    else
        self:DoProgress(p,0)
    end

end


function UIBookLoadingView:UiTween()
    local y = 188
    if logic.config.channel == Channel.Spain then
        y = 222
    end
    self.progressBar.transform:DOAnchorPosY(y, 0.6):SetEase(core.tween.Ease.InOutBack):Play()
end

function UIBookLoadingView:HideProgressBar()
    self.progressBar.transform:DOAnchorPosY(-100, 0.3)
    :SetEase(core.tween.Ease.InOutBack)
    :Play()
    :OnComplete(function()
        logic.debug.Log("加载书本资源完成")
        --CUIManager.Instance.CloseForm(logic.cs.UIFormName.BookLoadingForm);
        --DialogDisplaySystem.Instance.StartReading();
        CS.UnityEngine.Shader.WarmupAllShaders()    --资源加载完毕,初始化shader
        logic.UIMgr:Close(logic.uiid.BookLoading)
        logic.bookReadingMgr:DoStartReading()
    end)
end


function UIBookLoadingView:SetProgress(p,allsize,isForce)
    if isForce then
        self.m_progress = p
    else
        self.m_progress = Mathf.Max(self.m_progress, p)
    end
    self.progressBar.Value = self.m_progress
    self.Percentage.text = string.format("%00.02f %%",self.m_progress * 100)

    if(allsize>0)then
        local cursizeM = ((allsize * self.m_progress) / 1024 / 1024);
        local allsizeM = (allsize / 1024/ 1024);
        self.ResSizeText.text = string.format("%.2f",cursizeM).."mb/"..string.format("%.2f",allsizeM).."mb";
    end

end


function UIBookLoadingView:DoProgress(p,allsize)
    if not self.isComplete and self.m_toProgress == p then
        return
    end
    self.m_toProgress = p
    if self.tweener then
        self.tweener:Kill()
        self.tweener = nil
    end
    self.tweener = core.tween.DoTween.To(function()
        return self.m_progress
    end,function(value)
        self:SetProgress(value,allsize)
    end,self.m_toProgress,self.LoadDuration)
    :SetEase(core.tween.Ease.Flash)
    :SetId(self)

    if(self.isComplete) then
        self.tweener:OnComplete(function()
            self:SetProgress(1,allsize)
            core.coroutine.start(function()
                core.coroutine.step(1)--隔一帧在执行
                self:HideProgressBar()
            end)
        end)
    end
    self.tweener:Play()
end


function UIBookLoadingView:SetCoverImage(refCount)
    self.CoverRefCount = refCount
    local sprite = refCount:GetObject()
    if not IsNull(sprite) then
        self.LoadingBG.sprite = sprite
        self.LogoImage.gameObject:SetActiveEx(false)
    end
end

function UIBookLoadingView:OnBackToMainClick()
    self:__Close()
    logic.bookReadingMgr:BackToMainClick()
end

return UIBookLoadingView