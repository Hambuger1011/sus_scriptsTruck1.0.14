local BaseClass = core.Class
local TopBookView = BaseClass("TopBookView")

function TopBookView:__init(gameObject)
    self.gameObject=gameObject;
    self.TopBookBg =CS.DisplayUtil.GetChild(gameObject, "TopBookBg"):GetComponent("Image");
    self.TopBookContent =CS.DisplayUtil.GetChild(gameObject, "TopBookContent");
    self.ScrollRect =CS.DisplayUtil.GetChild(gameObject, "Scroll View"):GetComponent(typeof(logic.cs.ScrollRect));
    self.PointBg =CS.DisplayUtil.GetChild(gameObject, "PointBg");
    self.PointContent =CS.DisplayUtil.GetChild(gameObject, "PointContent");
    self.CenterOnChild = self.ScrollRect.gameObject:GetComponent("CenterOnChild")

    --事件监听
    logic.cs.EventDispatcher.AddMessageListener(logic.cs.EventEnum.GotoRead,function() self:OpenRead() end)

    self.bookList = {}
    self.pointList = {}
    self.selectIndex = 1
end

function TopBookView:ShowNext()
    if #self.bookList > 1 then
        if self.co then
            coroutine.stop(self.co)
        end
        self.co = coroutine.start(function()
            coroutine.wait(self.waitTime)
            self:ShowNext()
        end)
        local nextIndex = self.selectIndex
        if self.selectIndex == #self.bookList then
            self.selectIndex = 1
            nextIndex = 0
        end
        self.CenterOnChild:MoveToChild(nextIndex)
    end
end

function TopBookView:StopMove()
    if #self.bookList > 1 then
        if self.co then
            coroutine.stop(self.co)
        end
    end
end

function TopBookView:StartMove()
    if #self.bookList > 1 then
        if self.co then
            coroutine.stop(self.co)
        end
        self.co = coroutine.start(function()
            coroutine.wait(self.waitTime)
            self:ShowNext()
        end)
    end
end

function TopBookView:OnValueChanged(value)
    if #self.pointList > 0 then
        local v = value + 1
        self.selectIndex = v
        for i = 1, #self.pointList do
            self.pointList[i]:SetActiveEx(i == tonumber(v))
        end
        self:StartMove()
    end
end

--点击打开书本
function TopBookView:OpenRead()
    self:OnPlayClick();
end

--点击打开书本
function TopBookView:OnPlayClick()
    if(self.bookList and self.bookList[self.selectIndex])then
        local bookinfo={};
        bookinfo.book_id=self.bookList[self.selectIndex];
        GameHelper.BookClick(bookinfo);
       --埋点*继续阅读模块
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectBook,"home_1", "1" ,tostring(bookinfo.book_id));
    end

end

function TopBookView:ShowTopBook(book_ids,time)
    --book_ids = "72,100,8,82"
    self.TopBookContent.transform:ClearAllChild()
    self.PointContent.transform:ClearAllChild()
    self.pointList = {}
    self.bookList = string.split(book_ids, ",")
    self.waitTime = time or 5
    for i = 1, #self.bookList do
        self.bookList[i] = tonumber(self.bookList[i])
    end
    GameHelper.CurBookId=self.bookList[1];
    for k, v in pairs(self.bookList) do
        local TopBookItem = logic.cs.GameObject.Instantiate(self.TopBookBg.gameObject,self.TopBookContent.transform):GetComponent("Image");
        local NewBg =CS.DisplayUtil.GetChild(TopBookItem.gameObject, "NewBg");
        --【显示New标签】
        GameHelper.ShowNewBg(v,NewBg);

        TopBookItem.sprite = CS.ResourceManager.Instance:GetUISprite("BookDisplayForm/bg_picture");
        logic.cs.UIEventListener.AddOnClickListener(TopBookItem.gameObject,function(data)
            self:OnPlayClick()
            self:StopMove()
        end)
        logic.cs.ABSystem.ui:DownloadBanner(v,function(id,refCount)
            if(TopBookItem==nil)then
                refCount:Release();
                return;
            end
            if (v ~=id)then
                refCount:Release();
                return;
            end
            TopBookItem:DOFade(0, 0):SetEase(core.tween.Ease.Flash):Play();
            TopBookItem.sprite = refCount:GetObject();
            TopBookItem:DOFade(1, 0.2):SetEase(core.tween.Ease.Flash):Play();

        end)
        TopBookItem.gameObject:SetActiveEx(true)

        if #self.bookList > 1 then
            local PointItem = logic.cs.GameObject.Instantiate(self.PointBg,self.PointContent.transform)
            local PointImg = PointItem.transform:Find('PointImg').gameObject
            PointImg:SetActiveEx(k == self.selectIndex)
            table.insert(self.pointList,PointImg)
            PointItem.gameObject:SetActiveEx(true)
        end
    end
    if self.PointContent.transform.childCount == #self.bookList then
        self.CenterOnChild:InitChild()
    else
        coroutine.start(function()
            coroutine.wait(1)
            self.CenterOnChild:InitChild()
        end)
    end
    if #self.bookList > 1 then
        self.co = coroutine.start(function()
            coroutine.wait(self.waitTime)
            self:ShowNext()
        end)
    end


end

function TopBookView:__delete()

    logic.cs.EventDispatcher.RemoveMessageListener(logic.cs.EventEnum.GotoRead,function() self:OpenRead() end);

    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end

    if self.co then
        coroutine.stop(self.co)
        self.co = nil
    end

    self.gameObject = nil;
    self.TopBookBg = nil;
end

return TopBookView
