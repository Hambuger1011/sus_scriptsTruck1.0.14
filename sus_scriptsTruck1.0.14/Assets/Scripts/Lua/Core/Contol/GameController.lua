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
    GameController.ComuniadaControl=require("Logic/UI/UI_Comuniada/Control/ComuniadaControl");
    GameController.BusquedaControl=require("Logic/UI/UI_Busqueda/Control/BusquedaControl");
    GameController.CommunityControl=require("Logic/UI/UI_Community/Control/CommunityControl");
    GameController.EmailControl=require("Logic/UI/UI_Email/Control/EmailControl");
    GameController.ChatControl=require("Logic/UI/UI_Chat/Control/ChatControl");
    GameController.WindowConfig=require("Logic/Config/WindowConfig");
    GameController.DayPassController=require("Logic/UI/UI_DayPass/Control/DayPassController");
    GameController.FirstChargeControl=require("Logic/UI/UI_FirstCharge/Control/FirstChargeControl");
    GameController.InvestmentControl=require("Logic/UI/UI_Investment/Control/InvestmentControl");

    isInit = true
end

