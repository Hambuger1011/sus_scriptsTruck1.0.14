local BaseClass = core.Class
local BookItem = BaseClass("BookItem")

function BookItem:__init(gameObject)
    self.gameObject=gameObject;

    self.rectTransform=gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.BookBG =CS.DisplayUtil.GetChild(gameObject, "BookBG"):GetComponent("Image");
    self.BookName =CS.DisplayUtil.GetChild(gameObject, "BookName"):GetComponent("Text");
    self.BookWeek =CS.DisplayUtil.GetChild(gameObject, "BookWeek"):GetComponent("Image");
    self.BookTypeImg =CS.DisplayUtil.GetChild(gameObject, "BookTypeImg"):GetComponent("Image");
    self.Tips =CS.DisplayUtil.GetChild(gameObject, "Tips");
    self.BookFree =CS.DisplayUtil.GetChild(gameObject, "BookFree");
    self.BookWeekBg =CS.DisplayUtil.GetChild(gameObject, "BookWeekBg"):GetComponent("Image");
    self.BookWeekBgRect =self.BookWeekBg.gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.BookWeekText =CS.DisplayUtil.GetChild(gameObject, "BookWeekText"):GetComponent("Text");

    --书本阅读进度条
    self.ProgressBar =CS.DisplayUtil.GetChild(gameObject, "ProgressBar"):GetComponent("ProgressBar");

    --埋点处理时用  传入索引
    self._index = 0;
    --埋点处理时用   就是一个标识
    self.bookType = "";
    --服务器获取的书本数据
    self.BookInfo=nil;
    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.BookBG.gameObject,function(data) self:BookOnclicke() end)
end


function BookItem:SetInfo(Info)
    self.BookInfo = Info;
    --=====================================================================展示 show
    --【显示封面】
    GameHelper.ShowIcon(Info.book_id,self.BookBG);
    --【显示书本名称】
    self.BookName.text = Info.bookname;
    --【展示标签】
    GameHelper.ShowBookType(Info.book_id,self.BookTypeImg);
    --【展示书本进度条】
    GameHelper.ShowProgress(Info.book_id,self.ProgressBar,self.Tips);
    ----【Tips】
    --if (CS.BookItemManage.Instance:BookIsNew(Info.book_id))then
    --    self.Tips:SetActive(true);
    --else
    --    self.Tips:SetActive(false);
    --end

    --【限时活动免费读书 显示标签】
    self:Limit_time_Free();
    --=====================================================================展示 show
end

--【显示书本 周更更新时间】
function BookItem:ShowWeeklyUpdateTime(Info,nextWeekIndex)
    if(Info.update_time==nil or Info.update_time==0)then return; end
    local BookBGRect=self.BookBG.gameObject:GetComponent(typeof(logic.cs.RectTransform));
    BookBGRect.anchoredPosition ={x=0,y=-30};

    self.BookWeekBg.gameObject:SetActive(true);
    self.BookWeek.gameObject:SetActive(true);
    self.BookWeekText.gameObject:SetActive(true);
    self.Tips:SetActive(false);

    local datetemp=CS.DateUtil.ConvertIntDateTime(Info.update_time);
    --周更新推荐   时间展示
    local weekIndex = CS.DateUtil.GetWeekDay(datetemp.Year,datetemp.Month,datetemp.Day);
    if(weekIndex and weekIndex >=1 and weekIndex<=7)then
        self.BookWeek.sprite = CS.ResourceManager.Instance:GetUISprite("MainForm/main_bg_week"..weekIndex);
        self.BookWeekBg.sprite = CS.ResourceManager.Instance:GetUISprite("MainForm/main_bg_week"..weekIndex.."_"..weekIndex);
    end

    if(nextWeekIndex==nil or nextWeekIndex~=weekIndex)then
        self.BookWeekBgRect.sizeDelta ={x=200,y=356};
    else
        self.BookWeekBgRect.sizeDelta ={x=260,y=356};
    end

    if(Info.start_chapter==Info.end_chapter)then
        self.BookWeekText.text=Info.tag..": CH."..Info.start_chapter;
    else
        self.BookWeekText.text=Info.tag..": CH."..Info.start_chapter.." - "..Info.end_chapter;
    end

    self.ProgressBarRect=self.ProgressBar.gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.ProgressBarRect.anchoredPosition ={x=194,y=-362};
end

--【限时活动免费读书 显示标签】
function BookItem:Limit_time_Free()
    GameHelper.Limit_time_Free(self.BookFree);
end


--点击书本
function BookItem:BookOnclicke()
    --【点击书本】
    GameHelper.BookClick(self.BookInfo);
    --【埋点处理】
    if (self.bookType and self.bookType ~= "")then   --类型查看枚举BuriedPoint_bookType
        --埋点*推荐模块
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectBook,self.bookType, tostring(self._index), tostring(self.BookInfo.book_id));
    end
end

--销毁
function BookItem:__delete()
    logic.cs.UIEventListener.RemoveOnClickListener(self.BookBG.gameObject,function(data) self:BookOnclicke() end)

    self.rectTransform=nil;
    self.BookBG =nil;
    self.BookName=nil;
    self.BookWeek=nil;
    self.BookTypeImg =nil;
    self.Tips =nil;
    self.BookFree =nil;
    self._index =nil;
    self.bookType =nil;
    self.BookInfo=nil;
    self.ProgressBar =nil;
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return BookItem
