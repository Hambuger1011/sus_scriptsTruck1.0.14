local config = {}
config.isDebugMode = CS.GameUtility.isDebugMode
config.isAbMode = CS.AB.ABSystem.Instance.isUseAssetBundle
config.isEditorMode = CS.GameUtility.isEditorMode
config.os = CS.GameUtility.OS


OS = {
    Standalone = 'Standalone',--PC
    iOS = 'iOS',
    Android = 'Android',
}
return config