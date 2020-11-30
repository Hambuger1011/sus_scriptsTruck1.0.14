local BaseClass = core.Class
local WeeklyInfo = BaseClass("WeeklyInfo")

function WeeklyInfo:__init()
    --书本id
    self.book_id=0;
    --书本icon
    self.bookicon=0;
    --书本更新开始章节
    self.start_chapter=0;
    --书本更新结束章节
    self.end_chapter=0;
    --书本更新时间
    self.update_time=0;
    --用户阅读到第几个对话
    self.dialogid=0;
end

function WeeklyInfo:UpdateData(data)
    self.book_id=data.book_id;
    self.bookname=data.bookname;
    self.bookicon=data.bookicon;
    self.start_chapter=data.start_chapter;
    self.end_chapter=data.end_chapter;
    self.update_time=data.update_time;
    self.dialogid=data.dialogid;
end

--销毁
function WeeklyInfo:__delete()
    self.book_id=nil;
    self.bookicon=nil;
    self.start_chapter=nil;
    self.end_chapter=nil;
    self.update_time=nil;
    self.dialogid=nil;
end


return WeeklyInfo
