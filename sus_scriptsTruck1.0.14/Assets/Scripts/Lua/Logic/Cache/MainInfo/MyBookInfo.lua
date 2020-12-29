local BaseClass = core.Class
local MyBookInfo = BaseClass("MyBookInfo")

function MyBookInfo:__init()
    --书本id
    self.bookid=0;
    --书本上次对话ID
    self.dialogid=0;
    --最大开放章节对话ID
    self.end_dialogid=0;
    --是否收藏
    self.isfav=0;
end

function MyBookInfo:UpdateData(data)
    self.bookid=tonumber(data.bookid);
    self.dialogid=data.dialogid;
    self.end_dialogid=data.end_dialogid;
    self.isfav=data.isfav;
end

--销毁
function MyBookInfo:__delete()
    self.bookid=nil;
    self.dialogid=nil;
    self.end_dialogid=nil;
    self.isfav=nil;
end


return MyBookInfo
