local BaseClass = core.Class
local RankInfo = BaseClass("RankInfo")

function RankInfo:__init()
    --书本id
    self.book_id=0;
    --书本名称
    self.bookname="";
    --书本icon
    self.bookicon=0;
    --章节最大开放
    self.chapteropen=0;
    --书本阅读量
    self.read_count=0;
    self.chapterrelease=0;
    --搜索类型合集
    self.search_type="";
    --排名编号
    self.ranking=0;
    --用户阅读到第几个对话
    self.dialogid=0;
end

function RankInfo:UpdateData(data)
    self.book_id=data.book_id;
    self.bookname=data.bookname;
    self.bookicon=data.bookicon;
    self.chapteropen=data.chapteropen;
    self.read_count=data.read_count;
    self.chapterrelease=data.chapterrelease;
    self.search_type=data.search_type;
    self.ranking=data.ranking;
    self.dialogid=data.dialogid;
end

--销毁
function RankInfo:__delete()
    self.book_id=nil;
    self.bookname=nil;
    self.bookicon=nil;
    self.chapteropen=nil;
    self.read_count=nil;
    self.chapterrelease=nil;
    self.search_type=nil;
    self.ranking=nil;
    self.dialogid=nil;
end


return RankInfo
