-- 单例模式
local BaseClass = core.Class
local EmailCache = BaseClass("EmailCache", core.Singleton)


-- 构造函数
function EmailCache:__init()
    ---------------------【更多书本】
    --邮箱总数量
    self.sysarr_count=0;
    --总页数
    self.pages_total=0;
    --邮件 列表
    self.EmailList= {};
    ---------------------【更多书本】
end



--作者历史看过的故事列表
function EmailCache:UpdateEmailList(datas)
    self.sysarr_count=datas.sysarr_count;
    self.pages_total=datas.pages_total;
    self.EmailList=datas.sysarr;
end



-- 析构函数
function EmailCache:__delete()
end


EmailCache = EmailCache:GetInstance()
return EmailCache