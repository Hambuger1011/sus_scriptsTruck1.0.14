-- local Class = core.Class
-- local target = CS.XLuaManager
-- local XLuaManager = Class('XLuaManager')

-- xlua.private_accessible(target)

-- local TestHotfix = function(this)
--     logic.debug.Log('XLua Hotfix Success!!!')

    
--     CS.UnityEngine.Debug.unityLogger.logEnabled = true
--     CS.UnityEngine.Debug.unityLogger.filterLogType = CS.UnityEngine.LogType.Log

--     logic.ResMgr.LoadAsync(nil,logic.ResMgr.type.Prefab,logic.cs.CUIID.Canvas_Debug,function(asset)
--         CS.Tiinoo.DeviceConsole.DeviceConsoleLoader:Load()
--         if logic.cs.GameMain.isShowDebugPanel then
--             logic.cs.CUIManager:OpenForm(logic.cs.CUIID.Canvas_Debug, true, false)
--         end
--     end)

-- end

-- function XLuaManager:Register()
-- 	xlua.hotfix(target, "TestHotfix", TestHotfix)
-- 	--util.hotfix_ex(AssetBundleManager, "TestHotfix", AssetBundleManagerTestHotfix)
-- end

-- function XLuaManager:Unregister()
-- 	xlua.hotfix(target, "TestHotfix", nil)
-- 	--xlua.hotfix(target, "TestHotfix", nil)
-- end




-- return XLuaManager