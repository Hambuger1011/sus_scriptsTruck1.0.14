CS.XLuaHelper.AddSearchPath("Logic/")
CS.XLuaHelper.AddSearchPath("Core/")
CS.XLuaHelper.AddSearchPath("3rd/")

require('3rd/init')
require('Core/init')

if CS.GameUtility.isDebugMode then
    --ctr+shit+p -> EmmyLua:Insert Emmy Debugger Code
    -- package.cpath = package.cpath .. string.format(';%s/Z_Work/emmy/windows/x64/?.dll',CS.System.Environment.CurrentDirectory)
    
    if CS.GameUtility.isEditorMode and core.config.os ~= OS.iOS then
        local dir = CS.System.Environment.CurrentDirectory..'/Z_Work/debugger/emmy/windows/x64/'
        if CS.CFileManager.IsDirectoryExist(dir) then
            package.cpath = package.cpath .. ";"..dir.."?.dll"
            --local dbg = require("emmy_core")
            --dbg.tcpListen("localhost", 9966)
            print("debugger:"..dir)
        end
    end
    
    -- --local breakSocketHandle,debugXpCall = require("Core/Debug/LuaDebugjit")("localhost",7003)
    -- local breakSocketHandle,debugXpCall = require("Core/Debug/LuaDebug")("localhost",7003)
    
    -- coroutine.start(function()
    --     while(true) do
    --         breakSocketHandle()
    --         coroutine.step()
    --     end
    -- end)

end

require('Logic/init')

