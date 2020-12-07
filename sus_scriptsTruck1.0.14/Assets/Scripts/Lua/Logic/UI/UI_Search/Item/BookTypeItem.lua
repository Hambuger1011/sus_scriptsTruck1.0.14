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
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.gameObject,function(data) self:BookOnclicke() end)

    --服务器获取的书本数据
    self.BookInfo=nil;

end

function BookTypeItem:SetInfo(Info)
    self.BookInfo = Info;
    self.BookName.text = Info.bookname;

    --=====================================================================展示 show
    GameHelper.ShowIcon(Info.book_id,self.BookBG);
    self.rectTransform.sizeDelta = {x=205,y=350};

    local readcount=Info.read_count;
    if (readcount > 1000)then
        local num=readcount/1000;
        local _result = string.format("%.1f", num)
        --观看次数
        self.LookNumberText.text =tostring(_result);
    else
        self.LookNumberText.text =tostring(readcount);
    end


    --章节介绍
    local ChapterDiscription = GameHelper.GetChapterDiscription(Info.book_id);
    self.IntroductionText.text=ChapterDiscription;

    local GetChapterCount = GameHelper.GetChapterCount(Info.book_id);
	if(GetChapterCount==nil)then
	return;
	end
	logic.debug.LogError("BookTypeItem:SetInfo"..Info.book_id); 
    self.ChapterProgress.text = "Chapter:"..GetChapterCount;
    --if(bookDetails and ((CS.XLuaHelper.is_Null(bookDetails)==false)) and bookDetails.ChapterDiscriptionArray.Length >= chapterId)then
    --    if (chapterId == 0)then  chapterId = 1; end
    --    self.ChapterDescriptionTxt.text = "<color='#101010'><size=28>Chapter" .. chapterId .. "</size></color>:" .. bookDetails.ChapterDiscriptionArray[chapterId - 1];
    --end
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
