local BaseClass = core.Class
local BookRankItem = BaseClass("BookRankItem")

function BookRankItem:__init(gameObject)
    self.gameObject=gameObject;

    self.BookBG =CS.DisplayUtil.GetChild(gameObject, "BookBG"):GetComponent("Image");
    self.BookName =CS.DisplayUtil.GetChild(gameObject, "BookName"):GetComponent("Text");
    self.LookNumber =CS.DisplayUtil.GetChild(gameObject, "LookNumber"):GetComponent("Image");
    self.LookNumberText =CS.DisplayUtil.GetChild(gameObject, "LookNumberText"):GetComponent("Text");
    self.BookFree =CS.DisplayUtil.GetChild(gameObject, "BookFree");
    logic.cs.UIEventListener.AddOnClickListener(self.gameObject,function(data) self:OnBookClick() end)
    self.DayPassBg =CS.DisplayUtil.GetChild(gameObject, "DayPassBg");

    self.BookTypeImg1 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg1"):GetComponent("Image");
    self.BookTypeImg2 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg2"):GetComponent("Image");
    self.BookTypeImg3 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg3"):GetComponent("Image");
    self.BookTypeImg4 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg4"):GetComponent("Image");
    self.BookTypeImg5 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg5"):GetComponent("Image");
    self.BookTypeImg6 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg6"):GetComponent("Image");

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

    --【观看次数】【观看次数】【观看次数】
    GameHelper.ShowLookNumber(Info.read_count,self.LookNumberText);

    --展示标签 【多个】
    GameHelper.ShowBookTypeX(Info.book_id,self.BookTypeImg1,self.BookTypeImg2,self.BookTypeImg3,self.BookTypeImg4,self.BookTypeImg5)
    --【限时活动免费读书 显示标签】
    self:Limit_time_Free();

    if(_type== RankType.Platform)then
        self.LookNumber.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_Reading volume01");
    elseif(_type== RankType.Newbook)then
        self.LookNumber.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_new01");
    elseif(_type== RankType.Popularity)then
        self.LookNumber.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_popular01");
    end
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

--【DayPass 显示标签】
function BookRankItem:DayPass()
    self.DayPassBg:SetActive(false);
    local daypasslist=Cache.PopWindowCache.daypassList;
    if(GameHelper.islistHave(daypasslist)==true)then
        local len=table.length(daypasslist);
        for i = 1, len do
            if(daypasslist[i]==self.BookInfo.book_id)then
                GameHelper.DayPass(self.DayPassBg);
            end
        end
    end
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
