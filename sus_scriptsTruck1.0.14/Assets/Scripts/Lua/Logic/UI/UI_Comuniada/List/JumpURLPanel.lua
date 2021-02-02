local JumpURLPanel = core.Class("JumpURLPanel")

function JumpURLPanel:__init(gameObject)
    self.gameObject=gameObject;
    self.JumpURLBg =CS.DisplayUtil.GetChild(gameObject, "JumpURLBg");
    self.JumpURLText =CS.DisplayUtil.GetChild(gameObject, "JumpURLText"):GetComponent("HyperText");

    self.JumpURLText.text="The desktop version is now live. Use your desktop to open <a href=\"\">http://sus-ugc.igg.com/</a> and start creating your stories now."

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.JumpURLBg,function(data) self:OnJumpURLBg() end)

end

function JumpURLPanel:OnJumpURLBg()
    logic.cs.Application.OpenURL("http://sus-ugc.igg.com/");
end



--销毁
function JumpURLPanel:__delete()
    if(self.JumpURLBg)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.JumpURLBg,function(data) self:OnJumpURLBg() end)
    end
    self.JumpURLBg =nil;
    self.JumpURLText =nil;
    self.gameObject=nil;
end


return JumpURLPanel
