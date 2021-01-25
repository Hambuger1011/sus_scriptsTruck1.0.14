local BaseClass = core.Class
local UIView = core.UIView

local UIMainForm = BaseClass("UIMainForm", UIView)

local uiid = logic.uiid
UIMainForm.config = {
    ID = uiid.UIMainForm,
    AssetName = 'UI/Resident/UI/UIMainForm'
}

--region【Awake】

local this=nil;
function UIMainForm:OnInitView()

    UIView.OnInitView(self)
    local get = logic.cs.LuaHelper.GetComponent
    local root = self.uiform.transform
    this=self.uiform;

    --列表的主 scrollrect
    self.MainScrollView =CS.DisplayUtil.GetChild(this.gameObject, "MainScrollView"):GetComponent("ScrollRect");

    --region【限时活动顶部栏】
    self.MainScrollViewRect=self.MainScrollView.gameObject:GetComponent(typeof(logic.cs.RectTransform));
    --endregion【限时活动顶部栏】

    --顶部推荐栏
    self.mTopBookView =CS.DisplayUtil.GetChild(self.MainScrollView.gameObject, "TopBookView");
    self.TopBookView = require('Logic/UI/UI_Main/Panel/TopBookView').New(self.mTopBookView);

    --周更新 列表
    self.mWeeklyUpdateList =CS.DisplayUtil.GetChild(self.MainScrollView.gameObject, "WeeklyUpdateList");
    self.WeeklyUpdateList = require('Logic/UI/UI_Main/Panel/MainBookList').New(self.mWeeklyUpdateList);

    --推荐列表
    self.mRecommendList =CS.DisplayUtil.GetChild(self.MainScrollView.gameObject, "RecommendList");
    self.RecommendList = require('Logic/UI/UI_Main/Panel/MainBookList').New(self.mRecommendList);

    --我的书本列表
    self.mMyBookList =CS.DisplayUtil.GetChild(self.MainScrollView.gameObject, "MyBookList");
    self.MyBookList = require('Logic/UI/UI_Main/Panel/MainBookList').New(self.mMyBookList);
    self.MyBookEmpty =CS.DisplayUtil.GetChild(self.mMyBookList, "MyBookEmpty");

    --排行榜
    self.mBookRanksList =CS.DisplayUtil.GetChild(self.MainScrollView.gameObject, "BookRanksList");
    self.BookRanksList = require('Logic/UI/UI_Main/Panel/BookRanksList').New(self.mBookRanksList);


    ---- LGBT列表
    --self.mLGBTList =CS.DisplayUtil.GetChild(self.MainScrollView.gameObject, "LGBTList");
    --self.LGBTList = require('Logic/UI/UI_Main/Panel/MainBookList').New(self.mLGBTList);
    --
    ---- Romance列表
    --self.mRomanceList =CS.DisplayUtil.GetChild(self.MainScrollView.gameObject, "RomanceList");
    --self.RomanceList = require('Logic/UI/UI_Main/Panel/MainBookList').New(self.mRomanceList);
    --
    ---- Suspense列表
    --self.mSuspenseList =CS.DisplayUtil.GetChild(self.MainScrollView.gameObject, "SuspenseList");
    --self.SuspenseList = require('Logic/UI/UI_Main/Panel/MainBookList').New(self.mSuspenseList);

    --礼品按钮
    self.GiftButton = CS.DisplayUtil.GetChild(this.gameObject, "GiftButton");
    --礼品按钮红点
    self.RedImg = CS.DisplayUtil.GetChild(this.gameObject, "RedImg");

    self.WhileMask =CS.DisplayUtil.GetChild(this.gameObject, "WhileMask"):GetComponent("Image");
    self.WhileMask.color =Color.New(1,1,1,1)

    GameHelper.isEnterMain = true;
end
--endregion


--region【OnOpen】

function UIMainForm:OnOpen()
    UIView.OnOpen(self)

    GameController.MainFormControl:SetData(self);

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.GiftButton,function(data) self:GiftButtonOnClick() end)

   --重新请求服务器 所有书本列表
    GameController.MainFormControl:RefreshBooks(true);

    --顶部推荐栏
    self:UpdateTopBookView();
    --设置我不在书本中
    logic.cs.MyBooksDisINSTANCE:SetIsPlaying(false)

    self.WhileMask:DOKill();
    self.WhileMask:DOFade(0, 0.5):SetEase(core.tween.Ease.Flash):Play();

    if(Channel.Spain)then
       -- logic.cs.SdkMgr.ads:ShowInterstitial();
    end

    --加载顶端栏
    logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.MainFormTop);

    ----打开主界面底部的按钮栏
    --logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.MainDown)

    --打开主界面底部的按钮栏
    logic.UIMgr:Open(logic.uiid.UIMainDownForm)


    --埋点*打开主界面
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.HomePage)


    self:Limit_time_Free();

end

--endregion


--region 【OnClose】

function UIMainForm:OnClose()
    UIView.OnClose(self)


    if(self.GiftButton)then
        --按钮监听
        logic.cs.UIEventListener.RemoveOnClickListener(self.GiftButton,function(data) self:GiftButtonOnClick() end)
    end

    --关闭销毁 【顶部推荐栏】
    self.TopBookView:Delete();
    --关闭销毁 【推荐列表】
    self.RecommendList:Delete();
    --关闭销毁 【我的书本列表】
    self.MyBookList:Delete();
    --关闭销毁 【周更新 列表】
    self.WeeklyUpdateList:Delete();

    ----关闭销毁 【LGBT列表】
    --self.LGBTList:Delete();
    ----关闭销毁 【Romance列表】
    --self.RomanceList:Delete();
    ----关闭销毁 【Suspense列表】
    --self.SuspenseList:Delete();
    --关闭销毁 【排行榜】
    self.BookRanksList:Delete();

    logic.UIMgr:Close(logic.uiid.UIMainDownForm);

    this = nil;
    self.MainScrollView = nil;
    self.MainScrollViewRect= nil;
    self.mTopBookView = nil;
    self.TopBookView= nil;
    self.mWeeklyUpdateList= nil;
    self.WeeklyUpdateList = nil;
    self.mRecommendList = nil;
    self.RecommendList = nil;
    self.mMyBookList  = nil;
    self.MyBookList = nil;
    self.MyBookEmpty  =nil;
    self.mBookRanksList =nil;
    self.BookRanksList  =nil;
    self.GiftButton =nil;
    self.RedImg =nil;
    self.WhileMask=nil;
    GameHelper.isEnterMain = false;

    GameController.MainFormControl:SetData(nil);
end

--endregion


--region 【刷新列表】

--刷新顶部栏推荐书本
function UIMainForm:UpdateTopBookView()
    --顶部推荐栏
    self.TopBookView.New(self.mTopBookView);
end


--刷新推荐列表
function UIMainForm:UpdateRecommendList(recommend_book)
    if(self.RecommendList.gameObject)then
        self.RecommendList:UpdateList(recommend_book,CS.CTextManager.Instance:GetText(288),BuriedPoint_bookType.Recommen);
    end

end

--刷新我的书本
function UIMainForm:ResetMyBookList()
    --if(self.MyBookList==nil)then return; end

    if(self.MyBookList.gameObject)then
        if(self.MyBookEmpty and CS.XLuaHelper.is_Null(self.MyBookEmpty)==false)then
            self.MyBookEmpty:SetActive(false);
        end
        local mybooks={};

        local myBookIDs =  logic.cs.GameDataMgr.userData:GetMyBookIds();
        local len=myBookIDs.Count;
        for i = 1, len do
            local _index=i-1;
            local t_BookDetails = logic.cs.JsonDTManager:GetJDTBookDetailInfo(myBookIDs[_index]);
            if(t_BookDetails and (CS.XLuaHelper.is_Null(t_BookDetails)==false))then
                local info={};
                info.book_id=t_BookDetails.id;
                info.bookname=t_BookDetails.bookname;
                info.update_time=0;
                table.insert(mybooks,info);
            else
                logic.debug.LogError("t_BookDetails is Null");
            end
        end

        local mybookslen=table.length(mybooks);
        if (mybookslen<=0)then
            self.MyBookEmpty:SetActive(true);
        end

        --刷新列表
        self.MyBookList:UpdateList(mybooks,"Reading",BuriedPoint_bookType.MyBooks);
    end
end

--刷新推荐列表
function UIMainForm:UpdateWeeklyList(weekly_book)
    if(self.WeeklyUpdateList.gameObject)then
        self.WeeklyUpdateList:UpdateList(weekly_book,CS.CTextManager.Instance:GetText(287),BuriedPoint_bookType.WeeklyUpdate);
    end
end


--刷新LGBT列表
function UIMainForm:UpdateLGBTList(LGBT_book)
    if(self.LGBTList.gameObject)then
        self.LGBTList:UpdateList(LGBT_book,"LGBTQ+",BuriedPoint_bookType.LGBT);
    end
end

--刷新Romance列表
function UIMainForm:UpdateRomanceList(Romance_book)
    if(self.RomanceList.gameObject)then
        self.RomanceList:UpdateList(Romance_book,"Romance",BuriedPoint_bookType.Recommen);
    end
end

--刷新Suspense列表
function UIMainForm:UpdateSuspenseList(Suspense_book)
    if(self.SuspenseList.gameObject)then
        self.SuspenseList:UpdateList(Suspense_book,"Suspense",BuriedPoint_bookType.Suspense);
    end
end


--刷新 排行榜列表
function UIMainForm:ResetRankList()
    if(self.BookRanksList.gameObject)then
        self.BookRanksList:UpdateList();
    end
end

--endregion


--region 【活动礼包按钮点击】
function UIMainForm:GiftButtonOnClick()
    local uiform = logic.UIMgr:GetView2(logic.uiid.UIMainDownForm);
    if(uiform)then
        --点击活动按钮
        uiform:RwardToggleClick(nil);
        uiform.HomeToggle.isOn=false;
        uiform.RwardToggle.isOn=true;
    end
end
--endregion


--region 【背景音乐】
function UIMainForm:PlayBgm()
    --播放背景音乐
    CS.XLuaHelper.PlayBgMusic("Assets/Bundle/Music/Audiotones/christmas_bgm");
end

--关闭背景音乐
function UIMainForm:StopBgm()
    logic.cs.AudioManager:StopBGM()
end
--endregion


--region 【主界面移入动画】
function UIMainForm:MainFormMove(data)
    local _type = data;
    self.MainBG.anchoredPosition ={x=0,y=0};
    if(_type==1)then
        --主界面移出
        self.MainBG:DOAnchorPosX(380, 0.5):Play();
    else
        --主界面移入
        self.MainBG.anchoredPosition ={x=0,y=0};
    end
end
--endregion


--region 【界面关闭】
function UIMainForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end
--endregion


--region【限时活动顶部栏】
function UIMainForm:Limit_time_Free()

    if(Cache.LimitTimeActivityCache.ColoredEgg.is_open==1 or Cache.LimitTimeActivityCache.SpinDraw.is_open==1 or Cache.LimitTimeActivityCache.FreeKey.is_open==1 or Cache.MainCache.migration.migration_web_switch==1)then
        -- 1.图片的Top设置为100（偏移的Max是负数）
        self.MainScrollViewRect.offsetMax ={x=self.MainScrollViewRect.offsetMax.x,y=-200};
    else
        -- 1.图片的Top设置为100（偏移的Max是负数）
        self.MainScrollViewRect.offsetMax ={x=self.MainScrollViewRect.offsetMax.x,y=-100};
    end

end
--endregion


return UIMainForm