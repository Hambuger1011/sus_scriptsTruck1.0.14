----[[
---- added by wsh @ 2017-01-08
---- 图集管理：为逻辑层透明化图集路径和图集资源加载等底层操作
---- 注意：
---- 1、只提供异步操作，为的是不需要逻辑层取操心图集AB是否已经加载的问题
---- 2、图集管理器不做资源缓存
---- 3、图片名称带后缀
----]]

--local BaseClass = core.Class
--local AtlasMgr = BaseClass("AtlasManager", core.Singleton)

---- 从图集异步加载图片：回调方式
--local function LoadImageAsync(self, atlas_config, image_name, callback, ...)
	--local atlas_path = atlas_config.AtlasPath
	--local image_path = atlas_path.."/"..image_name
	
	--ResMgr.LoadAsync(ResMgr.tag.Global,ResMgr.type.Prefab,image_path, sprite_type, function(sprite, ...)
		--if callback then
			--callback(not IsNull(sprite) and sprite or nil, ...)
		--end
	--end, ...)
--end

--AtlasMgr.LoadImageAsync = LoadImageAsync



--AtlasMgr = AtlasMgr:GetInstance()
--return AtlasMgr
