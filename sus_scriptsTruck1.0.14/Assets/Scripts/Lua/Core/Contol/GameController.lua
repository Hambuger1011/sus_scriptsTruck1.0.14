GameController={};



local isInit=false;
-- 构造函数
function GameController.Init()
    --判断是否初始化
    if(isInit)then
        return;
    end
    GameController.MainFormControl=require("Logic/UI/UI_Main/Control/MainFormControl");
    GameController.SearchControl=require("Logic/UI/UI_Search/Control/SearchControl");
    GameController.ActivityControl=require("Logic/UI/UI_Activity/Control/ActivityControl");
    GameController.DressUpControl=require("Logic/UI/UI_DressUp/Control/DressUpControl");
    GameController.ActivityBannerControl=require("Logic/UI/UI_ActivityBanner/Control/ActivityBannerControl");

    isInit = true
end

