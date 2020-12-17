local BaseClass = core.Class
local StoryInfo = BaseClass("StoryInfo")

function StoryInfo:__init()
    --id
    self.id=0;
    --名称
    self.title="";
    --书本封面
    self.cover_image="";
    --是此道具一般时效天数
    self.tag="";
    --
    self.writer_id=0;
    --阅读数量
    self.read_count=0;
    --id
    self.book_id=0;
    --章节序号
    self.chapter_number=0;
end

function StoryInfo:UpdateData(data)
    self.id=data.id;
    self.title=data.title;
    self.cover_image=data.cover_image;
    self.tag=data.tag;
    self.writer_id=data.writer_id;
    self.read_count=data.read_count;
    self.book_id=data.book_id;
    self.chapter_number=data.chapter_number;
end

--销毁
function StoryInfo:__delete()
    self.id=nil;
    self.title=nil;
    self.cover_image=nil;
    self.tag=nil;
    self.writer_id=nil;
    self.read_count=nil;
    self.book_id=nil;
    self.chapter_number=nil;
end


return StoryInfo
