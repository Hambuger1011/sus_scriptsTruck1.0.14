local MainBookList = core.Class("MainBookList")

local BookItem = require('Logic/UI/UI_Main/Item/BookItem');

function MainBookList:__init(gameObject)
    self.gameObject=gameObject;

    self.ScrollRect =CS.DisplayUtil.GetChild(gameObject, "ScrollRect"):GetComponent("ScrollRect");
    self.ScrollRectTransform=self.ScrollRect.gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.TitleTxt =CS.DisplayUtil.GetChild(gameObject, "TitleTxt"):GetComponent("Text");
    self.SeeAllBtn =CS.DisplayUtil.GetChild(gameObject, "SeeAllBtn");

    --获取预设体 prefab
    self.bookItemObj=CS.XLuaHelper.GetBookItem();

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.SeeAllBtn,function(data) self:OnSeeAllBtn() end)

    self.booktype=nil;
    --埋点处理时用   就是一个标识
    self.BuriedPoint_bookType = "";

    self.ItemList={};
end

--region【SeeAllBtn】

function MainBookList:SetSeeAllBtn(booktype)
    self.booktype=booktype;   --【BookType.Romance】 【BookType.LGBT】 【BookType.Suspense】
end

function MainBookList:OnSeeAllBtn()
    local maindown = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if(maindown)then
        maindown.SearchToggle.isOn=true;
        maindown:SearchToggleClick(nil,self.booktype);

        if(self.BuriedPoint_bookType and self.BuriedPoint_bookType~="")then
            --埋点*seeall
            logic.cs.GamePointManager:BuriedPoint("seeall",self.BuriedPoint_bookType);
        end
    end
end

--endregion


function MainBookList:UpdateList(InfoList,TitleName,_BuriedPoint_bookType)
    self.BuriedPoint_bookType=_BuriedPoint_bookType;
    self:ClearList();
    if(self.ItemList==nil)then
        self.ItemList={};
    end

    local len = table.length(InfoList);
    for i = 1, len do
        
        local infoItem = InfoList[i]
        if infoItem ~= nil and logic.cs.JsonDTManager:GetJDTBookDetailInfo(infoItem.book_id) ~= nil then
            local go = logic.cs.GameObject.Instantiate(self.bookItemObj, self.ScrollRect.content.transform);
            go.transform.localPosition = core.Vector3.zero;
            go.transform.localScale = core.Vector3.one;
            go:SetActive(true);

            local item =BookItem.New(go);
            table.insert(self.ItemList,item);

            item._index = i;
            item.BuriedPoint_bookType = self.BuriedPoint_bookType;
            item:SetInfo(InfoList[i]);

            --如果是周更列表
            if(_BuriedPoint_bookType==BuriedPoint_bookType.WeeklyUpdate)then

                ----------------------------【下本书 更新日期（周几）】
                local nextIndex=i+1;
                if(nextIndex>len)then
                    nextIndex=nil;
                end
                local Nextdatetemp=nil;
                local nextWeekIndex=nil;
                if(nextIndex)then
                    Nextdatetemp=CS.DateUtil.ConvertIntDateTime(InfoList[nextIndex].update_time);
                    nextWeekIndex= CS.DateUtil.GetWeekDay(Nextdatetemp.Year,Nextdatetemp.Month,Nextdatetemp.Day);
                end
                ----------------------------【下本书 更新日期（周几）】
                --显示周更时间
                item:ShowWeeklyUpdateTime(InfoList[i],nextWeekIndex);
            end
        end
    end
    self.TitleTxt.text =TitleName;
end

--【限时活动免费读书 显示标签】
function MainBookList:Limit_time_Free()
    if(self.ItemList==nil)then return; end
    local len = table.length(self.ItemList);
    for i = 1, len do
        self.ItemList[i]:Limit_time_Free();
    end
end


function MainBookList:ClearList()
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
function MainBookList:__delete()
    if(self.SeeAllBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.SeeAllBtn,function(data) self:OnSeeAllBtn() end)
    end
    self.SeeAllBtn=nil;
    self.bookItemObj=nil;
    self.ScrollRect=nil;
    self.ScrollRectTransform=nil;
    self.TitleTxt=nil;
    self.booktype=nil;
    self.BuriedPoint_bookType =nil;
    self:ClearList();
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return MainBookList
