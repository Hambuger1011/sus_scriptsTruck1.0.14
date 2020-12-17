local BaseClass = core.Class
local StoryList = BaseClass("StoryList")

local StoryItem = require('Logic/UI/UI_Comuniada/Item/StoryItem');

function StoryList:__init(gameObject)
    self.gameObject=gameObject;

    self.ScrollRect =CS.DisplayUtil.GetChild(gameObject, "ScrollRect"):GetComponent("ScrollRect");
    self.ScrollRectTransform=self.ScrollRect.gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.TitleTxt =CS.DisplayUtil.GetChild(gameObject, "TitleTxt"):GetComponent("Text");
    self.SeeAllBtn =CS.DisplayUtil.GetChild(gameObject, "SeeAllBtn");

    --获取预设体 prefab
    self.storyItemObj=CS.XLuaHelper.GetStoryItem();
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.SeeAllBtn,function(data) self:OnSeeAllBtn() end)

    self.storyType=nil;

    self.ItemList={};
end

--region【SeeAllBtn】

function StoryList:SetSeeAllBtn(storyType)
    self.storyType=storyType;   --【EStoryList.MostPopular】 【EStoryList.LatestRelease】
end

function StoryList:OnSeeAllBtn()

    if(self.storyType==EStoryList.MostPopular)then
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMasForm);
        if(uiform==nil)then
            uiform = logic.UIMgr:Open(logic.uiid.UIMasForm);
            uiform:SetType(EStoryList.MostPopular);
        else
            uiform:SetType(EStoryList.MostPopular);
        end
    elseif(self.storyType==EStoryList.LatestRelease)then
        local uiform = logic.UIMgr:GetView2(logic.uiid.UIMasForm);
        if(uiform==nil)then
            uiform = logic.UIMgr:Open(logic.uiid.UIMasForm);
            uiform:SetType(EStoryList.LatestRelease);
        else
            uiform:SetType(EStoryList.LatestRelease);
        end
    end
end

--endregion


function StoryList:UpdateList(InfoList,type)
    self:SetSeeAllBtn(type)
    self:ClearList();
    if(self.ItemList==nil)then
        self.ItemList={};
    end

    local len = table.length(InfoList);
    for i = 1, len do
        local go = logic.cs.GameObject.Instantiate(self.storyItemObj, self.ScrollRect.content.transform);
        go.transform.localPosition = core.Vector3.zero;
        go.transform.localScale = core.Vector3.one;
        go:SetActive(true);

        local item =StoryItem.New(go);
        table.insert(self.ItemList,item);

        item:SetItemData(InfoList[i],i);
        item:SetType(type);
    end
end


function StoryList:ClearList()
    if (self.ItemList)then
        local len=#self.ItemList;
        if(len>0)then
            for i = 1, len do
                self.ItemList[i]:Delete();
            end
        end
    end
    self.ItemList={};
    self.ItemList=nil;
end


--销毁
function StoryList:__delete()

    if(self.SeeAllBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.SeeAllBtn,function(data) self:OnSeeAllBtn() end)
    end

    self.ScrollRect =nil;
    self.ScrollRectTransform =nil;
    self.TitleTxt =nil;
    self.SeeAllBtn =nil;
    self.storyItemObj=nil;
    self.storyType=nil;
    self.ItemList=nil;
    self:ClearList();
    self.gameObject=nil;
end


return StoryList
