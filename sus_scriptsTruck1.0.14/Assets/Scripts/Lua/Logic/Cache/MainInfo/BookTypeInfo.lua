local BaseClass = core.Class
local BookTypeInfo = BaseClass("BookTypeInfo")

function BookTypeInfo:__init()
    --书本id
    self.book_id=0;
    --书本名称
    self.bookname="";
    --用户当前阅读到的对话id
    self.dialogid=0;
    self.read_count=0;

end

function BookTypeInfo:UpdateData(data)
    self.book_id=data.book_id;
    self.bookname=data.bookname;
    self.dialogid=data.dialogid;
    self.read_count=data.read_count;
end

--销毁
function BookTypeInfo:__delete()
    self.book_id=nil;
    self.ranking=nil;
    self.dialogid=nil;
    self.read_count=nil;
end


return BookTypeInfo
