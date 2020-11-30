local BaseClass = core.Class
local Messenger = core.Messenger
local unpack = unpack or table.unpack

local DataManager = BaseClass("DataManager");


function DataManager:__init()
	self.data_message_center = Messenger.New()
	self.client = require('Logic/DataCenter/ClientData/ClientData')
	self.server = require('Logic/DataCenter/ServerData/ServerData')
	self.user = require('Logic/DataCenter/UserData/UserData')
end

function DataManager:__delete()
	self.data_message_center = nil
end

-- 注册消息
function DataManager:AddListener(e_type, e_listener, ...)
	self.data_message_center:AddListener(e_type, e_listener, ...)
end

-- 发送消息
function DataManager:Broadcast(e_type, ...)
	self.data_message_center:Broadcast(e_type, ...)
end

-- 注销消息
function DataManager:RemoveListener(e_type, e_listener)
	self.data_message_center:RemoveListener(e_type, e_listener)
end

function DataManager:DoEnter()
	local asset = logic.ResMgr.LoadImme(logic.ResMgr.tag.Null,logic.ResMgr.type.Text,"Assets/Bundle/Data/pb_define.txt")
	if(not IsNull(asset.resTextAsset)) then
		local pb_define = asset.resTextAsset.text
		--logic.debug.LogError(pb_define)
		self.protoc = core.Protobuf.protoc.new()
		self.protoc:load(pb_define)
		-- local tb = self.protoc:parse(pb_define)
		-- for i,v in pairs(tb['message_type']) do
		-- 	for j,vv in pairs(v['field']) do
		-- 		logic.debug.LogError(j..' '..tostring(vv))
		-- 	end
		-- end
	else
		logic.debug.LogError("初始化protobuf失败")
	end
	asset:Release()
end


return DataManager.New()