local PopBookInfo = core.Class("PopBookInfo")

function PopBookInfo:__init()
    --书本id
    self.book_id=0;
    --书本名称
    self.bookname="";
    --书本icon
    self.bookicon=nil;
    --该书本免费倒计时
    self.countdown=nil;
    --是否已经打开过
    self.isOpened=false;
    --跳转类型 1.打开书本预览  2.进入书本剧情内
    self.jump_type=0;
end

function PopBookInfo:UpdateData(data)
    self.book_id=data.book_id;
    self.bookname=data.bookname;
    self.bookicon=data.bookicon;
    self.countdown=data.countdown;
    self.jump_type=data.jump_type;
end

--销毁
function PopBookInfo:__delete()
    self.book_id=nil;
    self.bookname=nil;
    self.bookicon=nil;
    self.countdown=nil;
    self.jump_type=nil;
end


return PopBookInfo
