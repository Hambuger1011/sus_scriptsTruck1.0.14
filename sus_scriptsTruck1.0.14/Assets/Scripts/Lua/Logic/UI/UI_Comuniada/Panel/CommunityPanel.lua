local BaseClass = core.Class
local CommunityPanel = BaseClass("CommunityPanel")

function CommunityPanel:__init(gameObject)
    self.gameObject=gameObject;

    -- 【最受欢迎的】【作者列表】
    self.mAuthorList =CS.DisplayUtil.GetChild(gameObject, "AuthorList");
    self.AuthorList = require('Logic/UI/UI_Comuniada/List/AuthorList').New(self.mAuthorList);


    -- 【最受欢迎的】 作品
    self.mMostPopularList =CS.DisplayUtil.GetChild(gameObject, "MostPopularList");
    self.MostPopularList = require('Logic/UI/UI_Comuniada/List/StoryList').New(self.mMostPopularList);

    -- 【最新更新】 作品
    self.mLatestReleaseList =CS.DisplayUtil.GetChild(gameObject, "LatestReleaseList");
    self.LatestReleaseList = require('Logic/UI/UI_Comuniada/List/StoryList').New(self.mLatestReleaseList);

    --快捷创作按钮
    self.AnchorGoBtn =CS.DisplayUtil.GetChild(gameObject, "AnchorGoBtn");


    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.AnchorGoBtn,function(data) self:AnchorGoBtnClick() end)

end

function CommunityPanel:UpdateStoryList()

    if(self.MostPopularList)then
        -- 【最受欢迎的】 作品  刷新列表
        self.MostPopularList:UpdateList(Cache.ComuniadaCache.mostPopularList,EStoryList.MostPopular);
    end

    if(self.LatestReleaseList)then
        -- 【最新更新】 作品  刷新列表
        self.LatestReleaseList:UpdateList(Cache.ComuniadaCache.latestReleaseList,EStoryList.LatestRelease);
    end

end

function CommunityPanel:AnchorGoBtnClick(data)
    logic.cs.AudioManager:PlayTones(logic.cs.AudioTones.dialog_choice_click);
    local uiView = logic.UIMgr:Open(logic.uiid.Story_NewBook);
    if(uiView)then
        uiView:SetData(self.storyDetial)
    end

    --切换到 我的详情页面；
    GameController.ComuniadaControl:ChangeFavoritesPanel();

    --埋点*点击快捷创作
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.UgcWriteBook);
end


function CommunityPanel:UpdateHotWriter()
    if(self.AuthorList)then
        self.AuthorList:UpdateList(Cache.ComuniadaCache.HotWriterList);
    end
end


--销毁
function CommunityPanel:__delete()
    --关闭销毁 【最受欢迎的】
    self.MostPopularList:Delete();
    --关闭销毁 【最新更新】
    self.LatestReleaseList:Delete();
end

return CommunityPanel
