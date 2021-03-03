
local res = {}
-- local enResType = CS.GameCore.enResType
-- local csResMgr = CS.Gamecore.ResMgr.Instance
-- local game = CS.Gamecore.ResMgr.game
-- res.AbTask = CS.GameCore.AbTask

local enResType = CS.AB.enResType
local csResMgr = CS.AB.ABSystem.Instance
res.AbTask = CS.AB.AbTask

res.Load = function(tag,type,res) 
    return csResMgr:LoadImme(tag,type,res) 
end
res.LoadAsync = function (tag,type,res,callback)
    --core.debug.Log(tostring(tag).." "..tostring(type).." "..res)
    return csResMgr:LoadAsync(tag,type,res,callback) 
end

res.LoadImme = function (tag,type,res)
    --core.debug.Log(tag.." "..tostring(type).." "..res)
    return csResMgr:LoadImme(tag,type,res) 
end

res.ClearTag = function(tag)
    csResMgr:ClearAssetTag(tag) 
end

res.IsProsessLoading = function() return csResMgr:IsProsessLoading() end
res.GC = function()
    csResMgr:GC()
end
res.type = {
    Invalid = enResType.eInvalid,
    Object = enResType.eObject,
    Prefab = enResType.ePrefab,
    Audio = enResType.eAudio,
    Text = enResType.eText,
    Texture2D = enResType.eTexture2D,
    Sprite = enResType.eSprite,
    Font = enResType.eFont,
    ScriptableObject = enResType.eScriptableObject,
    Atlas = enResType.eAtlas,
    Scene = enResType.eScene,
    Shader = enResType.eShader,
    Material = enResType.eMaterial,
    Count = enResType.eMax,
}
res.tag = {
    Null = nil,
    Debug = "Debug",
    Game = "Game",
    Global = "Global",
    Map = "Map",
    Atlas = "Atlas",
    Lua = "Lua",
    Cat = "Cat",
    DialogDisplay = "DialogDisplay",
    StoryEditor = 'StoryEditor',
    ImageWall = "ImageWall",
}

res.res = {
    pfb = {
        --Game = "Assets/Bundle/prefabs/_game.prefab"
    }
}


res.LoadGameObject = function(tag,assetName)
    local asset = res.Load(tag, res.type.Prefab,assetName)
    local pfb = asset.resObject
    if logic.IsNull(pfb) then
        logic.debug.LogError('加载失败:'..assetName)
    end
    return pfb
end

--res.ResPreLoader = require("Logic/Res/ResPreLoader")
return res