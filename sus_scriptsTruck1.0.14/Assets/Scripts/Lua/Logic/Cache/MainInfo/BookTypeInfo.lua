local BaseClass = core.Class
local BookTypeInfo = BaseClass("BookTypeInfo")

function BookTypeInfo:__init()
    --书本id
    self.book_id=0;
    --书本封面
    self.bookicon=0;
    --书本名称
    self.bookname="";
    --最大章节数量【警告：包含未开放章节】
    self.chaptercount=0;
    --最大开放章节数量
    self.chapteropen=0;
    --用户阅读数量
    self.read_count=0;
    --书本类型
    self.type1=0;
end

function BookTypeInfo:UpdateData(data)
    self.book_id=data.book_id;
    self.bookicon=data.bookicon;
    self.bookname=data.bookname;
    self.chaptercount=data.chaptercount;
    self.chapteropen=data.chapteropen;
    self.read_count=data.read_count;
    self.type1=data.type1;
end

--销毁
function BookTypeInfo:__delete()
    self.book_id=nil;
    self.bookicon=nil;
    self.bookname=nil;
    self.chaptercount=nil;
    self.chapteropen=nil;
    self.read_count=nil;
    self.type1=nil;
end


return BookTypeInfo
