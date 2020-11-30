local BaseClass = core.Class
local RecommendInfo = BaseClass("RecommendInfo")

function RecommendInfo:__init()
    --书本id
    self.book_id=0;
    --书本icon
    self.bookicon=0;
    --书本名称
    self.bookname="";
    --用户阅读到第几个对话
    self.dialogid="";
end

function RecommendInfo:UpdateData(data)
    self.book_id=data.book_id;
    self.bookicon=data.bookicon;
    self.bookname=data.bookname;
    self.dialogid=data.dialogid;
end

--销毁
function RecommendInfo:__delete()
    self.book_id=nil;
    self.bookicon=nil;
    self.bookname=nil;
    self.dialogid=nil;
end


return RecommendInfo
