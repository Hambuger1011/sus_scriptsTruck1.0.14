local BaseClass = core.Class
local SignInInfo = BaseClass("SignInInfo")

function SignInInfo:__init()
    --奖励id
    self.id=0;
    --奖励类型 1钥匙 2钻石 4组合包
    self.type=0;
    --奖励钥匙
    self.bkey_qty=0;
    --奖励钻石
    self.diamond_qty=0;
    --道具奖励列表数组
    self.item_list={};
    --是否已签到领取 1:是 0否
    self.is_receive=0;
end

function SignInInfo:UpdateData(data)
    self.id=data.id;
    self.type=data.type;
    self.bkey_qty=data.bkey_qty;
    self.diamond_qty=data.diamond_qty;
    self.item_list=data.item_list;
    self.is_receive=data.is_receive;
end

--销毁
function SignInInfo:__delete()
    self.id=nil;
    self.type=nil;
    self.bkey_qty=nil;
    self.diamond_qty=nil;
    self.item_list=nil;
    self.is_receive=nil;
end


return SignInInfo
