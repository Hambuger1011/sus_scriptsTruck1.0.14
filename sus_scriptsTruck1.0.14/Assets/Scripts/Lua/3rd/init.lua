--CS.XLuaHelper.AddSearchPath("3rd/PLoop/")
core = core or {}

core.json = require("3rd/rapidjson/json")
core.lpeg = require('3rd/lpeg/re')

core.Protobuf = {}
core.Protobuf.pb = require 'pb'
core.Protobuf.protoc = require '3rd/pb2/protoc'
core.Protobuf.protobuf = require("3rd/protobuf/protobuf")

core.ffi = require 'ffi'
require("3rd/ToLua/tolua")
core.UpdateBeat = UpdateBeat
core.Timer = Timer