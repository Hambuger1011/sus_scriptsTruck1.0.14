--[[
	__add: 对+进行重载

	__sub: 对-进行重载

	__mul: 对*进行重载

	__div: 对/进行重载

	__unm: 对相反数进行重载

	__mod: 对%进行重载

	__pow: 对^进行重载

	__concat: 对连接操作符进行重载

	__eq: 对==进行重载

	__lt: 对<进行重载

	__le: 对<=进行重载

	__tostring: 类似于C++中对<<的重载 只要做了该重载，在使用print时就会使用对应的函数做处理后再输出
]]

local BaseClass = function(classname, super)
	assert(type(classname) == "string" and #classname > 0, string.format("class() - invalid class name \"%s\"", tostring(classname)))
	assert(not super or type(super) == "table",'super must be table')

    local cls
	if super then
		cls = {}
		--把cls的metatable设置为super(cls找不到时，对metatable的__index进行查找)
		setmetatable(cls, {__index = super})
		cls.super = super
	else
		cls = {__init = function() end,__gc = true}
	end

	cls.__cname = classname
	cls.__index = cls --指向自己

	function cls.New(...)
		local instance = setmetatable({}, cls) --创建新table，元表为cls
		instance.__class = cls

		-- 注册一个delete方法
		instance.Delete = function(self)
			if self.__delete then
				self:__delete()
			end
		end

		if instance.__init then
			instance:__init(...)
		end
		return instance
	end
	return cls
end
return BaseClass