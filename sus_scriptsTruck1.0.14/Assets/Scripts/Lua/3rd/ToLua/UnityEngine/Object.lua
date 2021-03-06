-- added by wsh @ 2017-12-27

-- xlua对UntyEngine的Object判空不能直接判nil
-- https://github.com/Tencent/xLua/blob/master/Assets/XLua/Doc/faq.md
function IsNull(unity_object)
	if unity_object == nil then
		return true
	end
	
	if type(unity_object) == "userdata" then
		return CS.UnityEngineObjectExtention.CheckNull(unity_object)
	end
	
	return false
end
