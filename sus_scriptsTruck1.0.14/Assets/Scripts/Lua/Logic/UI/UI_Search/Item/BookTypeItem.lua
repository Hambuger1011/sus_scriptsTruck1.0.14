local BaseClass = core.Class
local BookTypeItem = BaseClass("BookTypeItem")

function BookTypeItem:__init(gameObject)
    self.gameObject=gameObject;
    self.rectTransform=gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.BookBG=CS.DisplayUtil.GetChild(gameObject, "BookBG"):GetComponent("Image");
    self.LookNumberText =CS.DisplayUtil.GetChild(gameObject, "LookNumberText"):GetComponent("Text");
    self.IntroductionText =CS.DisplayUtil.GetChild(gameObject, "IntroductionText"):GetComponent("Text");
    self.ChapterProgress =CS.DisplayUtil.GetChild(gameObject, "ChapterProgress"):GetComponent("Text");
    self.BookFree =CS.DisplayUtil.GetChild(gameObject, "BookFree");
    self.BookName =CS.DisplayUtil.GetChild(gameObject, "BookName"):GetComponent("Text");
    self.Tips =CS.DisplayUtil.GetChild(self.BookBG.gameObject, "Tips");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.gameObject,function(data) self:BookOnclicke() end)

    --服务器获取的书本数据
    self.BookInfo=nil;

end

function BookTypeItem:SetInfo(Info)
    self.BookInfo = Info;
    self.BookName.text = Info.bookname;

    --=====================================================================展示 show
    --【书本封面】
    GameHelper.ShowIcon(Info.book_id,self.BookBG);
    self.rectTransform.sizeDelta = {x=205,y=350};

    --【观看次数】【观看次数】【观看次数】
    GameHelper.ShowLookNumber(Info.read_count,self.LookNumberText);

    --【最大开放章节】【最大开放章节】【最大开放章节】
    self.ChapterProgress.text = "Chapter:"..Info.chapteropen;

    --【章节介绍】【章节介绍】【章节介绍】
    local ChapterDiscription = GameHelper.GetChapterDiscription(Info.book_id);
    self.IntroductionText.text=ChapterDiscription;

    --【New和Update】【New和Update】【New和Update】
    local bookInfo=Cache.MainCache:GetMyBookByIndex(Info.book_id);
    if(bookInfo==nil)then
        GameHelper.ShowNewUpdate(Info.book_id,0,self.Tips);
    else
        GameHelper.ShowNewUpdate(Info.book_id,bookInfo.finish_max_chapter,self.Tips);
    end
    --=====================================================================展示 show

    --【限时活动免费读书 显示标签】
    self:Limit_time_Free();
end


--【限时活动免费读书 显示标签】
function BookTypeItem:Limit_time_Free()
    GameHelper.Limit_time_Free(self.BookFree);
end


--点击书本
function BookTypeItem:BookOnclicke()
    GameHelper.BookClick(self.BookInfo);
end

--销毁
function BookTypeItem:__delete()
    if(self.gameObject and CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.gameObject,function(data) self:BookOnclicke() end)
    end
    self.BookBG =nil;
    self.BookName =nil;
    self.rectTransform=nil;
    self.BookInfo=nil;
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return BookTypeItem
