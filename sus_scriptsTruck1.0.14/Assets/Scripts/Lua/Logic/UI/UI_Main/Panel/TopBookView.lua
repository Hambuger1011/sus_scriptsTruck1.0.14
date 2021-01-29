local BaseClass = core.Class
local TopBookView = BaseClass("TopBookView")

function TopBookView:__init(gameObject)
    self.gameObject=gameObject;
    self.TopBookBg =CS.DisplayUtil.GetChild(gameObject, "TopBookBg"):GetComponent("Image");
    self.TopBookContent =CS.DisplayUtil.GetChild(gameObject, "TopBookContent");
    self.Scrollbar =CS.DisplayUtil.GetChild(gameObject, "Scrollbar Horizontal"):GetComponent("Scrollbar");
    self.ScrollRect =CS.DisplayUtil.GetChild(gameObject, "Scroll View"):GetComponent(typeof(logic.cs.ScrollRect));
    self.PointBg =CS.DisplayUtil.GetChild(gameObject, "PointBg");
    self.PointContent =CS.DisplayUtil.GetChild(gameObject, "PointContent");

    --事件监听
    logic.cs.EventDispatcher.AddMessageListener(logic.cs.EventEnum.GotoRead,function() self:OpenRead() end)

    self.Scrollbar.onValueChanged:AddListener(function(value)
        self:OnScrollbarChanged(value)
    end)

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
    end
    local nextIndex = self.selectIndex
    if self.selectIndex == #self.bookList then
        self.selectIndex = 1
        nextIndex = 0
    end
    self.ScrollRect.horizontalNormalizedPosition=nextIndex / (#self.bookList - 1);
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

function TopBookView:OnScrollbarChanged(value)
    if #self.pointList > 0 then
        local v = value * (#self.pointList - 1) + 1
        v = math.floor(v + 0.5)
        self.selectIndex = v
        for i = 1, #self.pointList do
            self.pointList[i]:SetActiveEx(i == tonumber(v))
        end
    end
end

--点击打开书本
function TopBookView:OpenRead()
    self:OnPlayClick();
end

--点击打开书本章节页面
function TopBookView:OnPlayClick()
    if(self.bookList and self.bookList[self.selectIndex])then
        local bookinfo={};
        bookinfo.book_id=self.bookList[self.selectIndex];
        GameHelper.BookClick(bookinfo);
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectBook,"home_1", "1" ,tostring(bookinfo.book_id));
    end

end

--点击阅读书本
function TopBookView:ReadBook()
    local bookID = self.bookList[self.selectIndex]
    logic.cs.UserDataManager.UserData.CurSelectBookID = bookID
    logic.cs.GameHttpNet:GetBookDetailInfo(tonumber(bookID),function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            logic.cs.UserDataManager.bookDetailInfo = json
            local chapterId = logic.cs.UserDataManager.bookDetailInfo.data.finish_max_chapter + 1
            if (chapterId > logic.cs.UserDataManager.bookDetailInfo.data.book_info.chaptercount) then
                chapterId = logic.cs.UserDataManager.bookDetailInfo.data.book_info.chaptercount
            end
            logic.cs.GameHttpNet:BuyChapter(tonumber(bookID),chapterId,function(result)
                if result.code == 200 then
                    logic.cs.UserDataManager:SetBuyChapterResultInfo(bookID,result)
                    logic.cs.TalkingDataManager:SelectBooksInEnter(bookID)
                    logic.cs.GameDataMgr.userData:AddMyBookId(bookID)

                    logic.cs.BookReadingWrapper:ChangeBookDialogPath(bookID,chapterId)
                    local chapterid= tonumber(result.data.step_info.chapterid)
                    local dialogid = tonumber(result.data.step_info.dialogid)

                    local chapterInfo = logic.cs.JsonDTManager:GetJDTChapterInfo(bookID,chapterid);
                    local beginDialogID = 1
                    local endDialogID = 0
                    if chapterInfo ~= nil then
                        beginDialogID = chapterInfo.chapterstart
                        endDialogID = chapterInfo.chapterfinish
                    end
                    if dialogid < beginDialogID then
                        dialogid = beginDialogID
                    end
                    if dialogid > endDialogID then
                        dialogid = endDialogID
                    end
                    logic.bookReadingMgr.isReading = false
                    logic.cs.BookReadingWrapper:InitByBookID(
                            bookID,
                            chapterId,
                            dialogid,
                            beginDialogID,
                            endDialogID
                    )
                    logic.cs.BookReadingWrapper:PrepareReading(true)

                    logic.cs.GameHttpNet:GetBookDetailInfo(tonumber(bookID),function(result)
                        local json = core.json.Derialize(result)
                        local code = tonumber(json.code)
                        if code == 200 then
                            logic.cs.UserDataManager.bookDetailInfo = json
                        elseif code == 277 then
                            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
                        end
                    end)
                    logic.cs.GameHttpNet:GetBookBarrageCountList(tonumber(bookID),chapterId,function(result)
                        local json = core.json.Derialize(result)
                        local code = tonumber(json.code)
                        if code == 200 then
                            logic.cs.UserDataManager.BookBarrageCountList = json
                        elseif code == 277 then
                            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
                        end
                    end)

                    logic.cs.UserDataManager.isReadNewerBook = true
                    self:__Close()
                elseif code == 277 then
                    logic.cs.UIAlertMgr:Show("TIPS",json.msg)
                end
            end)
        elseif code == 277 then
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
end

function TopBookView:ShowTopBook(book_ids)
    --book_ids = "72,100,8,82"
    self.bookList = string.split(book_ids, ",")
    self.waitTime = 5
    for i = 1, #self.bookList do
        self.bookList[i] = tonumber(self.bookList[i])
    end
    GameHelper.CurBookId=self.bookList[1];
    for k, v in pairs(self.bookList) do
        local TopBookItem = logic.cs.GameObject.Instantiate(self.TopBookBg.gameObject,self.TopBookContent.transform):GetComponent("Image");
        TopBookItem.sprite = CS.ResourceManager.Instance:GetUISprite("BookDisplayForm/bg_picture");
        logic.cs.UIEventListener.AddOnClickListener(TopBookItem.gameObject,function(data)
            --self:OnPlayClick()
            self:ReadBook()
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
    self.Scrollbar.numberOfSteps = #self.bookList
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
