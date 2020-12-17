local BaseClass = core.Class
local FavoritesPanel = BaseClass("FavoritesPanel")

function FavoritesPanel:__init(gameObject)
    self.gameObject=gameObject;

    -- 【我写作的故事】 作品
    self.mMyWriterList =CS.DisplayUtil.GetChild(gameObject, "MyWriterList");
    self.MyWriterList = require('Logic/UI/UI_Comuniada/List/StoryList').New(self.mMyWriterList);

    -- 【历史看过的故事】 作品
    self.mHistoryList =CS.DisplayUtil.GetChild(gameObject, "HistoryList");
    self.HistoryList = require('Logic/UI/UI_Comuniada/List/StoryList').New(self.mHistoryList);


    self.CreateNewBtn =CS.DisplayUtil.GetChild(self.mMyWriterList, "CreateNewBtn");
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.CreateNewBtn,function(data) self:CreateNewBtnClick() end)
end

function FavoritesPanel:UpdateMyWriterList()
    if(self.MyWriterList)then
        -- 【我写作的故事】刷新列表
        self.MyWriterList:UpdateList(Cache.ComuniadaCache.myWriterList,EStoryList.MyWriterList);
    end
end


function FavoritesPanel:UpdateHistoryList()
    if(self.HistoryList)then
        -- 【历史看过的故事】刷新列表
        self.HistoryList:UpdateList(Cache.ComuniadaCache.historyList,EStoryList.HistoryList);
    end
end

function FavoritesPanel:CreateNewBtnClick()
    logic.cs.AudioManager:PlayTones(logic.cs.AudioTones.dialog_choice_click);
    local uiView = logic.UIMgr:Open(logic.uiid.Story_NewBook);
    if(uiView)then
        uiView:SetData(self.storyDetial)
    end
end


function FavoritesPanel:__delete()

    if(self.MyWriterList)then
        --关闭销毁 【我写作的故事】
        self.MyWriterList:Delete();
    end

    if(self.HistoryList)then
        --关闭销毁 【历史看过的故事】
        self.HistoryList:Delete();
    end

end

return FavoritesPanel
