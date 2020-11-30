local Class = core.Class
local UIView = core.UIView

local UIManager = Class("UIManager")

local views = {}
local uiClass = {}

function UIManager:__init()
end

function UIManager:__delete()
end

function UIManager:Creat(modname)
    local uiFile = require(modname)
    return uiFile
end

---@param UIConfig core.UIView
function UIManager:Startup(UIConfig)
    for _,_view in pairs(UIConfig) do
        local _cfg = _view.config
        local uiid = _cfg.ID
        assert(uiid ~= nil, "no exsits Config.ID: ".._view.__cname)
        --assert(cfg.AssetName ~= nil, "AssetName is null : "..uuid)       
        uiClass[uiid] = _view      -- 按key保存这种模块 require "Logic/UI/UI_BookLoading/UIBookLoadingView",
        views[uiid] = nil
    end
end

function UIManager:Open(uiid)
    local clazz = uiClass[uiid]
    if not clazz then
        logic.debug.LogError('未注册UI:'..uiid);
        return nil
    end

    local view = views[uiid]
    if not view then
        view = clazz.New()   --去调用 UIView:__init  require "Logic/UI/UI_BookLoading/UIBookLoadingView"    
        views[uiid] = view
    end

    if not view.isOpen then
        UIView.__Open(view)  --去调用 UIView
    end
    return view
end

function UIManager:Close(uiid)
    -- local clazz = uiClass[uiid]
    -- if not clazz then
    --     core.debug.LogError('未注册UI:'..uiid)
    --     return
    -- end

    local view = views[uiid]
    if view and view.isOpen then
        UIView.__Close(view)
        
        if not view.isOpen then
            views[uiid] = nil
        end
    end
end

function UIManager:CloseAll()
    for i, view in pairs(views) do
        if(view and view.isOpen)then
            UIView.__Close(view);

            if not view.isOpen then
                view = nil
            end
        end
    end
end



function UIManager:RemoveView(uiid)
    views[uiid] = nil  
end


function UIManager:GetView(uuid)
    local v = views[uuid]
    if(v==nil)then
        logic.debug.LogError('未打开UI:'..tostring(uuid));
        return nil
    end
    return v
end

function UIManager:GetView2(uuid)
    local v = views[uuid]
    if(v==nil)then
        return nil
    end
    return v
end

function UIManager:GetViewClass(uiid)
    local clazz = uiClass[uiid]
    if not clazz then
        logic.debug.LogError('未注册UI:'..uiid);
    end
    return clazz
end

return UIManager.New()