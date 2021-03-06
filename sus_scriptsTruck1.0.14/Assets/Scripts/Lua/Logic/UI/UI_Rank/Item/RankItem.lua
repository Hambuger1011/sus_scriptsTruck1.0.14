local BaseClass = core.Class
local RankItem = BaseClass("RankItem")

function RankItem:__init(gameObject)
    self.gameObject=gameObject;

    self.BookBG =CS.DisplayUtil.GetChild(gameObject, "BookBG"):GetComponent("Image");
    self.BookName =CS.DisplayUtil.GetChild(gameObject, "BookName"):GetComponent("Text");
    self.BookTypeImg1 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg1"):GetComponent("Image");
    self.BookTypeImg2 =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg2"):GetComponent("Image");
    self.LookNumber =CS.DisplayUtil.GetChild(gameObject, "LookNumber"):GetComponent("Image");
    self.LookNumberText =CS.DisplayUtil.GetChild(gameObject, "LookNumberText"):GetComponent("Text");
    self.BookFree =CS.DisplayUtil.GetChild(gameObject, "BookFree");
    self.DayPassBg =CS.DisplayUtil.GetChild(self.BookBG.gameObject, "DayPassBg");
    logic.cs.UIEventListener.AddOnClickListener(self.BookBG.gameObject,function(data) self:OnBookClick() end)

    --服务器获取的书本数据
    self.BookInfo=nil;

end


function RankItem:SetInfo(Info,_type)
    self.BookInfo = Info;
    --=====================================================================展示 show
    self.BookName.text =Info.ranking.."  "..Info.bookname;
    GameHelper.ShowIcon(Info.book_id,self.BookBG);

    --【观看次数】【观看次数】【观看次数】
    GameHelper.ShowLookNumber(Info.read_count,self.LookNumberText);

    --展示标签
    GameHelper.ShowBookType(Info.book_id,self.BookTypeImg1);
    GameHelper.ShowBookType2(Info.book_id,self.BookTypeImg2);

    --【限时活动免费读书 显示标签】
    self:Limit_time_Free();


    if(_type==RankType.Platform)then
        self.LookNumber.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_smg_trending");
    elseif(_type==RankType.Newbook)then
        self.LookNumber.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_smg_new");
    elseif(_type==RankType.Popularity)then
        self.LookNumber.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_smg_popular");
    end

    --【DayPass】
    self:DayPass();
end

--【限时活动免费读书 显示标签】
function RankItem:Limit_time_Free()
    GameHelper.Limit_time_Free(self.BookFree);
end

--【DayPass】
function RankItem:DayPass()
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

--点击书本
function RankItem:OnBookClick()
    GameHelper.BookClick(self.BookInfo);
end


--销毁
function RankItem:__delete()

    if(self.BookBG)then logic.cs.UIEventListener.RemoveOnClickListener(self.BookBG.gameObject,function(data) self:OnBookClick() end) end
    self.BookBG =nil;
    self.BookName  =nil;
    self.BookTypeImg =nil;
    self.LookNumberText=nil;
    self.LookNumber=nil;
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return RankItem
