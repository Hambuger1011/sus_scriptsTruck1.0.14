local BaseClass = core.Class
local MainRankList = BaseClass("MainRankList")

local BookRankItem = require('Logic/UI/UI_Main/Item/BookRankItem');

function MainRankList:__init(gameObject)
    self.gameObject=gameObject;

    self.TitleTxt =CS.DisplayUtil.GetChild(gameObject, "TitleTxt"):GetComponent("Text");
    self.GridList =CS.DisplayUtil.GetChild(gameObject, "GridList");
    self.SeeAllBtn =CS.DisplayUtil.GetChild(gameObject, "SeeAllBtn");

    self.RankBookItem1_Obj =CS.DisplayUtil.GetChild(self.GridList, "RankBookItem1");
    self.RankBookItem2_Obj =CS.DisplayUtil.GetChild(self.GridList, "RankBookItem2");
    self.RankBookItem3_Obj =CS.DisplayUtil.GetChild(self.GridList, "RankBookItem3");

    logic.cs.UIEventListener.AddOnClickListener(self.SeeAllBtn,function(data) self:OnSeeAll() end)
    self.ItemList=nil;
    self._type=nil;
end

function MainRankList:OnSeeAll()

    --搜索界面
    local uirank = logic.UIMgr:GetView(logic.uiid.UIRankForm);
    if(uirank==nil)then
        --打开主界面
        uirank= logic.UIMgr:Open(logic.uiid.UIRankForm);
    else
        uirank.uiform:Appear();
    end

    if(self._type== RankType.Platform)then
        uirank.PlatformTab.isOn=true;
        uirank:PlatformTabClick(nil);

        --埋点*seeall
     logic.cs.GamePointManager:BuriedPoint("seeall","home_8");
    elseif(self._type== RankType.Newbook)then
        uirank.NewbookTab.isOn=true;
        uirank:NewbookTabClick(nil);

        --埋点*seeall
        logic.cs.GamePointManager:BuriedPoint("seeall","home_9");
    elseif(self._type== RankType.Popularity)then
        uirank.PopularityTab.isOn=true;
        uirank:PopularityTabClick(nil);

        --埋点*seeall
        logic.cs.GamePointManager:BuriedPoint("seeall","home_10");
    end

end

function MainRankList:UpdateRankList(list,titleName,_type)
    self._type=_type;
    self:ClearList()
    if(self.ItemList==nil)then
        self.ItemList={};
    end

    self.TitleTxt.text = titleName;
    local rankBooks = list;
    if(GameHelper.islistHave(rankBooks)==false)then return; end
    local _len=table.length(rankBooks);
    logic.debug.Log("生成书本的数量：".. _len);

    --少于3本书 return；
    if(_len<3)then return; end


    local bookInfo1=  Cache.MainCache:GetRankByIndex(list,1);
    local item1 =BookRankItem.New(self.RankBookItem1_Obj);
    self.RankBookItem1_Obj:SetActive(true);
    if(bookInfo1)then
        item1:SetInfo(bookInfo1,_type,1);
        table.insert(self.ItemList,item1);
    end

    local bookInfo2=  Cache.MainCache:GetRankByIndex(list,2);
    local item2 =BookRankItem.New(self.RankBookItem2_Obj);
    self.RankBookItem2_Obj:SetActive(true);
    if(bookInfo2)then
        item2:SetInfo(bookInfo2,_type,2);
        table.insert(self.ItemList,item2);
    end

    local bookInfo3=  Cache.MainCache:GetRankByIndex(list,3);
    local item3 =BookRankItem.New(self.RankBookItem3_Obj);
    self.RankBookItem3_Obj:SetActive(true);
    if(bookInfo3)then
        item3:SetInfo(bookInfo3,_type,3);
        table.insert(self.ItemList,item3);
    end

end

--【限时活动免费读书 显示标签】
function MainRankList:Limit_time_Free()
    if(self.ItemList==nil)then return; end
    local len = table.length(self.ItemList);
    for i = 1, len do
        self.ItemList[i]:Limit_time_Free();
    end
end



function MainRankList:ClearList()
    if (self.ItemList)then
        local len=#self.ItemList;
        if(len>0)then
            for i = 1, len do
                self.ItemList[i]:Delete();
            end
        end
    end
    self.ItemList=nil;
end



--销毁
function MainRankList:__delete()
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;

    self.TitleTxt =nil;
    self.GridList =nil;
    self.SeeAllBtn =nil;

    self.RankBookItem1_Obj =nil;
    self.RankBookItem2_Obj =nil;
    self.RankBookItem3_Obj =nil;
    self:ClearList();

end

return MainRankList
