-- 单例模式
local BaseClass = core.Class
local SignInCache = BaseClass("SignInCache", core.Singleton)


-- 构造函数
function SignInCache:__init()
    self.ceche = {}
    self.activity_login= {};

    self.IsSign=false;

--id; --奖励id
--type; --奖励类型 1钥匙 2钻石 4组合包
--bkey_qty; --奖励钥匙
--diamond_qty;  --奖励钻石
--is_receive; --是否已签到领取 1:是 0否
end

-- 析构函数
function SignInCache:__delete()

end


SignInCache = SignInCache:GetInstance()
return SignInCache