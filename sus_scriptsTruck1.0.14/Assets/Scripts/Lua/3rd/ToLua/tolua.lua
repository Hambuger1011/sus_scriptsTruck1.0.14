--------------------------------------------------------------------------------
--      Copyright (c) 2015 - 2016 , 蒙占志(topameng) topameng@gmail.com
--      All rights reserved.
--      Use, modification and distribution are subject to the "MIT License"
--------------------------------------------------------------------------------
if jit then		
	if jit.opt then		
		jit.opt.start(3)				
	end		
	
	print("ver"..jit.version_num.." jit: ", jit.status())
	print(string.format("os: %s, arch: %s", jit.os, jit.arch))
end

if DebugServerIp then  
  require("mobdebug").start(DebugServerIp)
end

require "3rd/ToLua/misc/functions"
require "3rd/ToLua/UnityEngine/Object"
UnityEngine = {}
Mathf		= require "3rd/ToLua/UnityEngine/Mathf"
Vector3 	= require "3rd/ToLua/UnityEngine/Vector3"
Quaternion	= require "3rd/ToLua/UnityEngine/Quaternion"
Vector2		= require "3rd/ToLua/UnityEngine/Vector2"
Vector4		= require "3rd/ToLua/UnityEngine/Vector4"
Color		= require "3rd/ToLua/UnityEngine/Color"
Ray			= require "3rd/ToLua/UnityEngine/Ray"
Bounds		= require "3rd/ToLua/UnityEngine/Bounds"
RaycastHit	= require "3rd/ToLua/UnityEngine/RaycastHit"
Touch		= require "3rd/ToLua/UnityEngine/Touch"
LayerMask	= require "3rd/ToLua/UnityEngine/LayerMask"
Plane		= require "3rd/ToLua/UnityEngine/Plane"
Time		= require "3rd/ToLua/UnityEngine/Time"

list		= require "3rd/ToLua/list"
utf8		= require "3rd/ToLua/misc.utf8"

require "3rd/ToLua/event"
require "3rd/ToLua/typeof"
require "3rd/ToLua/slot"
require "3rd/ToLua/System/Timer"
require "3rd/ToLua/System/coroutine"
require "3rd/ToLua/System/ValueType"
require "3rd/ToLua/System/Reflection/BindingFlags"

--require "misc.strict"