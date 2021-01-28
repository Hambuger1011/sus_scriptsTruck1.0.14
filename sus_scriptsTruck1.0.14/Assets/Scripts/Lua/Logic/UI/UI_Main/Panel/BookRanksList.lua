local BaseClass = core.Class
local BookRanksList = BaseClass("BookRanksList")

function BookRanksList:__init(gameObject)
    self.gameObject=gameObject;

    self.ScrollViewCUIList =CS.DisplayUtil.GetChild(gameObject, "ScrollViewCUIList"):GetComponent("CUIList");
    self.TitleTxt =CS.DisplayUtil.GetChild(gameObject, "TitleTxt"):GetComponent("Text");
    self.mContent =CS.DisplayUtil.GetChild(gameObject, "mContent");


    -- Platform排行榜
    self.mPlatformRankList =CS.DisplayUtil.GetChild(self.mContent, "PlatformRankList");
    self.PlatformRankList = require('Logic/UI/UI_Main/Panel/MainRankPanl/MainRankList').New(self.mPlatformRankList);

    -- Newbook排行榜
    self.mNewbookRankList =CS.DisplayUtil.GetChild(self.mContent, "NewbookRankList");
    self.NewbookRankList = require('Logic/UI/UI_Main/Panel/MainRankPanl/MainRankList').New(self.mNewbookRankList);

    --Popularity排行榜
    self.mPopularityRankList =CS.DisplayUtil.GetChild(self.mContent, "PopularityRankList");
    self.PopularityRankList = require('Logic/UI/UI_Main/Panel/MainRankPanl/MainRankList').New(self.mPopularityRankList);

end

function BookRanksList:UpdateList()
    --(list,titleName,_type)

    --Recommendation
    local str_Recommendation=CS.CTextManager.Instance:GetText(396);
    --New Book
    local str_NewBook=CS.CTextManager.Instance:GetText(397);
    --Popular Book
    local str_PopularBook=CS.CTextManager.Instance:GetText(398);

    --Platform排行榜  刷新列表
    self.PlatformRankList:UpdateRankList(Cache.MainCache.platform_ranklist,str_Recommendation,RankType.Platform);

    --Newbook排行榜  刷新列表
    self.NewbookRankList:UpdateRankList(Cache.MainCache.newbook_ranklist,str_NewBook,RankType.Newbook);

    --Popularity排行榜  刷新列表
    self.PopularityRankList:UpdateRankList(Cache.MainCache.popularity_ranklist,str_PopularBook,RankType.Popularity);

end

--【限时活动免费读书 显示标签】
function BookRanksList:Limit_time_Free()
    self.PlatformRankList:Limit_time_Free()
    self.NewbookRankList:Limit_time_Free()
    self.PopularityRankList:Limit_time_Free()
end

--【限时活动免费读书 显示标签】
function BookRanksList:DayPass();
    self.PlatformRankList:DayPass();
    self.NewbookRankList:DayPass();
    self.PopularityRankList:DayPass();
end

--销毁
function BookRanksList:__delete()

    --关闭销毁 【Platform排行榜】
    self.PlatformRankList:Delete();
    --关闭销毁 【Newbook排行榜】
    self.NewbookRankList:Delete();
    --关闭销毁 【Popularity排行榜】
    self.PopularityRankList:Delete();

    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;

    self.ScrollViewCUIList=nil;
    self.TitleTxt =nil;
    self.mContent=nil;
    self.mPlatformRankList =nil;
    self.PlatformRankList=nil;
    self.mNewbookRankList=nil;
    self.NewbookRankList =nil;
    self.mPopularityRankList =nil;
    self.PopularityRankList=nil;

end

return BookRanksList
