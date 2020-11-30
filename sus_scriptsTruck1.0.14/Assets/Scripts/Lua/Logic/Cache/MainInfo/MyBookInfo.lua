local BaseClass = core.Class
local MyBookInfo = BaseClass("MyBookInfo")

function MyBookInfo:__init()
    --书本id
    self.book_id=0;
    --书本名称
    self.bookname="";
    --书本icon
    self.bookicon=0;
    --章节id
    self.chapterid=0;
    --书本章节数
    self.ChapterCount=0;
    --书本上次对话ID
    self.dialogid=0;
    --书本类型
    self.type="";
    --
    self.isfav=0;
end

function MyBookInfo:UpdateData(data)
    self.book_id=data.book_id;
    self.bookname=data.bookname;
    self.bookicon=data.bookicon;
    self.chapterid=data.chapterid;
    self.ChapterCount=data.ChapterCount;
    self.dialogid=data.dialogid;
    self.type=data.type;
    self.isfav=data.isfav;
end

--销毁
function MyBookInfo:__delete()
    self.book_id=nil;
    self.bookname=nil;
    self.bookicon=nil;
    self.chapterid=nil;
    self.ChapterCount=nil;
    self.dialogid=nil;
    self.type=nil;
    self.isfav=nil;
end


return MyBookInfo
