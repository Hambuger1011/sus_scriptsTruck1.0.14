local ViewMoreBtn = core.Class("ViewMoreBtn")

function ViewMoreBtn:__init(gameObject)
    self.gameObject=gameObject;

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.gameObject,function(data) self:ViewMoreBtnClick() end)
end


--region【按钮点击】【更多动态按钮】
function ViewMoreBtn:ViewMoreBtnClick()
    GameController.CommunityControl:ViewMoreBtnClick()
end

--endregion

--销毁
function ViewMoreBtn:__delete()
    if(self.gameObject)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.gameObject,function(data) self:ViewMoreBtnClick() end)
    end

    self.gameObject=nil;
end

return ViewMoreBtn
