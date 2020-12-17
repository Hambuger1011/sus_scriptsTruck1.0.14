local BaseClass = core.Class
local StoryInfo2 = BaseClass("StoryInfo2")

function StoryInfo2:__init()
    --id
    self.id=0;
    --名称
    self.title="";
    --书本封面
    self.cover_image="";
    --作者名字 “alan”
    self.writer_name="";
    --作者id
    self.writer_id=0;
    --描述 "What 's new type ?  and who is?"
    self.description="";
    --是此道具一般时效天数
    self.tag="";
    --总章节数量
    self.total_chapter_count=0;
    --更新章节数量
    self.update_chapter_count=0;
    --类型
    self.status=0;
    --阅读数量
    self.read_count=0;
    --个人
    self.favorite_count=0;
    --
    self.word_count=0;
    --
    self.fail_reason="";
    --创建时间  "2020-02-24 15:28:25",
    self.create_time="";
    --更新时间  "2020-02-24 15:42:14",
    self.update_time="";
    --
    self.publish_time=0;

end

function StoryInfo2:UpdateData(data)
    self.id=data.id;
    self.title=data.title;
    self.cover_image=data.cover_image;
    self.writer_name=data.writer_name;
    self.writer_id=data.writer_id;
    self.description=data.description;
    self.tag=data.tag;
    self.total_chapter_count=data.total_chapter_count;
    self.update_chapter_count=data.update_chapter_count;
    self.status=data.status;
    self.read_count=data.read_count;
    self.favorite_count=data.favorite_count;
    self.word_count=data.word_count;
    self.fail_reason=data.fail_reason;
    self.create_time=data.create_time;
    self.update_time=data.update_time;
    self.publish_time=data.publish_time;
end

--销毁
function StoryInfo2:__delete()
    self.id=nil;
    self.title=nil;
    self.cover_image=nil;
    self.writer_name=nil;
    self.writer_id=nil;
    self.description=nil;
    self.tag=nil;
    self.total_chapter_count=nil;
    self.update_chapter_count=nil;
    self.status=nil;
    self.read_count=nil;
    self.favorite_count=nil;
    self.word_count=nil;
    self.fail_reason=nil;
    self.create_time=nil;
    self.update_time=nil;
    self.publish_time=nil;
end


return StoryInfo2
