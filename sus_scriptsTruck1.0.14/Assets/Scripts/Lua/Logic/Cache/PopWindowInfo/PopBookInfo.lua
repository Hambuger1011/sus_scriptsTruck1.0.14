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
    --新书弹出标签
    self.tag="";
    --day pass 弹窗类型：1.每天弹一次  2.每次登录都弹
    self.show_type="";
    --跳转类型 1.打开书本预览  2.进入书本剧情内
    self.jump_type="";
    --day pass书本是否已经显示 1已显示 0未显示
    self.is_show=0;


    ---day pass 是否已经打开过
    self.isOpened=false;
end

function PopBookInfo:UpdateData(data)
    self.book_id=tonumber(data.book_id);
    self.bookname=data.bookname;
    self.bookicon=data.bookicon;
    self.countdown=data.countdown;
    self.tag=data.tag;
    self.show_type=data.show_type;
    self.jump_type=data.jump_type;
    self.is_show=data.is_show;
end

--销毁
function PopBookInfo:__delete()
    self.book_id=nil;
    self.bookname=nil;
    self.bookicon=nil;
    self.countdown=nil;
    self.tag=nil;
    self.show_type=nil;
    self.jump_type=nil;
    self.is_show=nil;
end


return PopBookInfo
