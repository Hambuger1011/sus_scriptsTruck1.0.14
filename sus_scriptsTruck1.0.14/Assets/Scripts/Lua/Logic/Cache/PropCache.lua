-- 单例模式
local BaseClass = core.Class
local PropCache = BaseClass("PropCache", core.Singleton)


-- 构造函数
function PropCache:__init()
    self.SpriteData ={}
    local sprite1 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_10_outfit_discount")
    local sprite2 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_25_outfit_discount")
    local sprite3 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_10_choice_discount")
    local sprite4 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_25_choice_iscount")
    local sprite5 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_outfit_coupon")
    local sprite6 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_choice_coupon")
    local sprite7 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_key_oupon")
    local sprite8 = CS.ResourceManager.Instance:GetUISprite("PakageForm/com_icon_messenger_dove")
    local sprite9 = CS.ResourceManager.Instance:GetUISprite("PakageForm/dup_toukuang_light")--临时奖励头像框
    local sprite10 = CS.ResourceManager.Instance:GetUISprite("PakageForm/com_icon_diamand1")--钻石
    local sprite11 = CS.ResourceManager.Instance:GetUISprite("PakageForm/com_icon_kyes1")--钥匙
    local sprite12 = CS.ResourceManager.Instance:GetUISprite("PakageForm/Props_icon_Key Coupon_1")
    local sprite13 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_outfit_coupon_1")
    local sprite14 = CS.ResourceManager.Instance:GetUISprite("PakageForm/props_icon_choice_coupon_1")
    self.SpriteData[1] = sprite10
    self.SpriteData[2] = sprite11
    self.SpriteData[3] = sprite9
    self.SpriteData[4] = sprite13
    self.SpriteData[5] = sprite12
    self.SpriteData[6] = sprite14
    self.SpriteData[10011] = sprite1
    self.SpriteData[10012] = sprite2
    self.SpriteData[10013] = sprite3
    self.SpriteData[10014] = sprite4
    self.SpriteData[10015] = sprite1
    self.SpriteData[10016] = sprite2
    self.SpriteData[10017] = sprite3
    self.SpriteData[10018] = sprite4
    self.SpriteData[10019] = sprite1
    self.SpriteData[10020] = sprite2
    self.SpriteData[10021] = sprite3
    self.SpriteData[10022] = sprite4
    self.SpriteData[10023] = sprite1
    self.SpriteData[10024] = sprite2
    self.SpriteData[10025] = sprite3
    self.SpriteData[10026] = sprite4
    self.SpriteData[10101] = sprite5
    self.SpriteData[10102] = sprite6
    self.SpriteData[10103] = sprite7
    self.SpriteData[10104] = sprite5
    self.SpriteData[10105] = sprite6
    self.SpriteData[10106] = sprite7
    self.SpriteData[10107] = sprite5
    self.SpriteData[10108] = sprite6
    self.SpriteData[10109] = sprite7
    self.SpriteData[10110] = sprite5
    self.SpriteData[10111] = sprite6
    self.SpriteData[10112] = sprite7
    self.SpriteData[10201] = sprite8
end

-- 析构函数
function PropCache:__delete()

end

PropCache = PropCache:GetInstance()
return PropCache