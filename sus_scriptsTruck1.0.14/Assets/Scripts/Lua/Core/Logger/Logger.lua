--[[
-- added by wsh @ 2017-11-30
-- Logger系统：Lua中所有错误日志输出均使用本脚本接口，以便上报服务器
--]]

local BaseClass = core.Class
local Logger = BaseClass("Logger")
local _print = --CS.UnityEngine.Debug.Log
function(msg)
	CS.LOG.Info(tostring(msg))
end

local _error = --CS.UnityEngine.Debug.LogError
function(msg)
	CS.LOG.Error(tostring(msg))
end
local _warn = --CS.UnityEngine.Debug.LogWarning
function(msg)
	CS.LOG.Warn(tostring(msg))
end

function Logger.Log(msg)
	if core.config.isDebugMode then
		_print(debug.traceback(msg, 2))
	else
		--CS.Logger.Log(debug.traceback(msg, 2))
	end
end


function Logger.LogWarning(msg)
	if core.config.isDebugMode then
		_warn(debug.traceback(msg, 2))
	else
		--CS.Logger.LogError(debug.traceback(msg, 2))
	end
end

function Logger.LogError(msg)
	if core.config.isDebugMode then
		_error(debug.traceback(msg, 2))
	else
		--CS.Logger.LogError(debug.traceback(msg, 2))
	end
end

function Logger.PrintTable(tbl,_tip)
	if tbl then
		if type(tbl) == "table" then
			local json = core.json.Serialize(tbl)
			if _tip then
				print(_tip.."="..json);
			else
				print(json);
			end
		else
			print(tostring(tbl).." is not a table value");
		end
	else
		if _tip then
			print(_tip.."=nil");
		end
	end
end

function Logger.Asset(condition,msg)
	if core.config.isDebugMode then
		if not condition then
			_error(debug.traceback(msg, 2))
		end
	else
		--CS.Logger.LogError(debug.traceback(msg, 2))
	end
end

-- 重定向event错误处理函数
event_err_handle = Logger.LogError
error = Logger.LogError
print = Logger.Log



return Logger