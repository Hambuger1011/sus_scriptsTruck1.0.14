local BaseClass = core.Class
local UIView = core.UIView
local UILoadingView = BaseClass("UILoadingView", UIView)
local base = UIView
local this = UILoadingView
local uiid = logic.uiid

UILoadingView.config = {
	ID = uiid.Loading,
	AssetName = nil,--不生成，这个比较特殊，进游戏时c#端已经加载UI了
}


function UILoadingView:OnInitView()
    this = self
    UIView.OnInitView(self)
    self.LaunchLoadingForm = self.uiform:GetComponent(typeof(CS.LaunchLoadingForm));
    self.CurLoadContInfoTxt = self.LaunchLoadingForm.CurLoadContInfoTxt;
    self.ResSizeText = self.LaunchLoadingForm.ResSizeText;
    self.ResSizeText.enabled=false;
end

function UILoadingView:OnOpen()
    UIView.OnOpen(self)
    if IsNull(self.uiform) then
        self.uiform = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.LoadingForm)
        self:OnInitView()
    end
end

function UILoadingView:OnClose()
    UIView.OnOpen(self)
    logic.cs.CUIManager:CloseForm(logic.cs.UIFormName.LoadingForm)
    this = nil
end

function UILoadingView.SetLoadingContInfo(msg)
    if this == nil then
        return
    end
    math.random(tonumber(os.time()))--给随机数设置随机数种子
    local randomNum = math.random(1,#logic.loadingTips)
    local tips = logic.loadingTips[tonumber(randomNum)]
    this.CurLoadContInfoTxt.text = tips
end

function UILoadingView.StartLoading(callback)
    if this == nil then
        return
    end
    this.LaunchLoadingForm:StartLoading(callback)
end

function UILoadingView.HideLogo()
    if this == nil then
        return
    end
    this.LaunchLoadingForm:HideLogo()
end


function UILoadingView.SetProgress(value,isForce)
    if this == nil then
        return
    end
    this.LaunchLoadingForm.progressBar.Value = value;
    this.LaunchLoadingForm.Percentage.text = string.format("%00.02f %%",value * 100)
end

return UILoadingView