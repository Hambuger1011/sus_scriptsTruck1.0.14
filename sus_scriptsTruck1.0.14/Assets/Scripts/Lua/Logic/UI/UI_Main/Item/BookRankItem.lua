local BaseClass = core.Class
local BookRankItem = BaseClass("BookRankItem")

function BookRankItem:__init(gameObject)
    self.gameObject=gameObject;

    self.BookBG =CS.DisplayUtil.GetChild(gameObject, "BookBG"):GetComponent("Image");
    self.BookName =CS.DisplayUtil.GetChild(gameObject, "BookName"):GetComponent("Text");
    self.BookTypeImg1 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg1"):GetComponent("Image");
    self.BookTypeImg2 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg2"):GetComponent("Image");
    self.LookNumberText =CS.DisplayUtil.GetChild(gameObject, "LookNumberText"):GetComponent("Text");
    self.BookFree =CS.DisplayUtil.GetChild(gameObject, "BookFree");
    logic.cs.UIEventListener.AddOnClickListener(self.gameObject,function(data) self:OnBookClick() end)

    --书本配置表
    self.m_bookDetailCfg=nil;
    --服务器获取的书本数据
    self.BookInfo=nil;

    self._index=nil;
    self._types=nil;
end


function BookRankItem:SetInfo(Info,_type,_index);
    self.BookInfo = Info;
    self._types=_type;
    self._index=_index;
    --=====================================================================展示 show
    self.BookName.text =Info.ranking.."  "..Info.bookname;
    GameHelper.ShowIcon(Info.book_id,self.BookBG)
    self.LookNumberText.text =tostring(Info.read_count) ;

    --展示标签1
    GameHelper.ShowBookType(Info.book_id,self.BookTypeImg1);
    --展示标签2
    GameHelper.ShowBookType2(Info.book_id,self.BookTypeImg2);
    --【限时活动免费读书 显示标签】
    self:Limit_time_Free();
end

--点击书本
function BookRankItem:OnBookClick()
    GameHelper.BookClick(self.BookInfo);


    if(self._types== RankType.Platform)then
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectBook,"home_8", tostring(self._index),tostring(self.BookInfo.book_id));
    elseif(self._types== RankType.Newbook)then
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectBook,"home_9", tostring(self._index),tostring(self.BookInfo.book_id));
    elseif(self._types== RankType.Popularity)then
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectBook,"home_10", tostring(self._index),tostring(self.BookInfo.book_id));
    end

end

--【限时活动免费读书 显示标签】
function BookRankItem:Limit_time_Free()
    GameHelper.Limit_time_Free(self.BookFree);
end



--销毁
function BookRankItem:__delete()

    if(self.gameObject)then logic.cs.UIEventListener.RemoveOnClickListener(self.gameObject,function(data) self:OnBookClick() end) end
    self.BookBG =nil;
    self.BookName  =nil;
    self.BookTypeImg =nil;
    self.LookNumberText=nil;

    self.m_bookDetailCfg=nil;
    self.gameObject=nil;
end


return BookRankItem
