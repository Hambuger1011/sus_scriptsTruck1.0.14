local config = core.config
local GameUtility = CS.GameUtility

config.channel = GameUtility.ChannelID
config.useServerData = GameUtility.useServerData
config.WritablePath = GameUtility.WritablePath
config.ReadonlyPath = GameUtility.ReadonlyPath


Channel = {
    None = 'None',
    Huawei = 'Huawei',
    Spain = "Spain",
    Onyx = "Onyx",
}
return config