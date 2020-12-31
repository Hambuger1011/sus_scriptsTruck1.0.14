local ViewMoreBtn = core.Class("ViewMoreBtn")

function ViewMoreBtn:__init(gameObject)
    self.gameObject=gameObject;

    self.ViewMoreText =CS.DisplayUtil.GetChild(gameObject, "ViewMoreText"):GetComponent("Text");
    self.Arrow =CS.DisplayUtil.GetChild(gameObject, "Arrow");


    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.gameObject,function(data) self:ViewMoreBtnClick() end)
end


--region【按钮点击】【更多动态按钮】
function ViewMoreBtn:ViewMoreBtnClick()
    GameController.CommunityControl:ViewMoreBtnClick(function(_type)  self:Setext(_type) end)
end

--endregion

function ViewMoreBtn:Setext(_type)

    --【设置层级】【设置层级】
    local anima=nil;

    if(_type==1)then
        self.ViewMoreText.text="ViewMore";
        anima=self.Arrow.transform:DORotate({x=0,y=0,z=0 },0.2):SetAutoKill(true):SetEase(core.tween.Ease.Flash);
        anima:Play();
    elseif(_type==2)then
        self.ViewMoreText.text="Show Less";
        anima=self.Arrow.transform:DORotate({x=0,y=0,z=-180 },0.2):SetAutoKill(true):SetEase(core.tween.Ease.Flash);
        anima:Play();
    end

end


--销毁
function ViewMoreBtn:__delete()
    if(self.gameObject)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.gameObject,function(data) self:ViewMoreBtnClick() end)
    end

    self.gameObject=nil;
end

return ViewMoreBtn
