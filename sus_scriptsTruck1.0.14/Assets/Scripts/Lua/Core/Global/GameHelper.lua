local json=require 'rapidjson';

GameHelper={}
local this = GameHelper;


--region 【退出剧情】
function GameHelper.OnQuitStory(boo)
    if(boo~=nil)then
        if(boo==false)then
            logic.bookReadingMgr:BackToMainClick() --退出剧情
        end
    end
end
--endregion


--region 【Back功能】

GameHelper.isEnterMain = false;

--返回按键回调
function GameHelper.OnKeyBoardBack(_index)
    if(GameHelper.isEnterMain==false)then return; end

    logic.debug.Log("back");
    local curUI= logic.cs.CUIManager:GetBackUIForm();
    if(curUI.m_formPath==nil or curUI.m_formPath=="")then    logic.debug.LogError("CurUICurUICurUICurUI:Null"); return; end

    local _topuistyle = curUI.uistyle;
    local Alertui =nil;

    if(_topuistyle==CS.EnumUIStyle.PopWindowUI)then       --【三级界面】
        --弹窗界面 用户点击BACK  关闭弹窗  （主界面协议界面除外）
        if(curUI ~= nil and curUI.uistyle == CS.EnumUIStyle.PopWindowUI and curUI.m_formPath~="")then
            if(curUI.isLua)then
                local curView = logic.UIMgr:GetView2(curUI.m_formPath)
                if(curView)then
                    curView:OnExitClick();
                    logic.cs.CUIManager:ClearBackUIForm();
                end
            else
                logic.cs.CUIManager:CloseForm(curUI.m_formPath);
                logic.cs.CUIManager:ClearBackUIForm();
            end
        end
    elseif(_topuistyle==CS.EnumUIStyle.TwoUI)then      --【二级界面】
        if(curUI ~= nil and curUI.uistyle == CS.EnumUIStyle.TwoUI and curUI.m_formPath~="")then

            if(curUI.isLua)then
                local curView = logic.UIMgr:GetView2(curUI.m_formPath)
                if(curView)then
                    curView:OnExitClick();
                    logic.cs.CUIManager:ClearBackUIForm();
                end
            else
                local curCsharp =nil;
                curCsharp=curUI.gameObject:GetComponent(typeof(CS.BookDisplayForm))
                if(curCsharp)then
                    local isshow=curCsharp:ReseMaskISOpen();
                    if(isshow)then
                        curCsharp:ReseMaskClose();
                        return;
                    end
                end

                if(curUI.m_formPath=="UI/Resident/UI/Canvas_ChargeMoney" or curUI.m_formPath=="UI/Resident/UI/Canvas_Mas")then
                    curCsharp = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.MainFormTop):GetComponent(typeof(CS.MainTopSprite))
                    if(curCsharp)then
                        curCsharp:CloseIconOnclicke();
                        logic.cs.CUIManager:ClearBackUIForm();
                        return;
                    end
                end
                logic.cs.CUIManager:CloseForm(curUI.m_formPath);
                logic.cs.CUIManager:ClearBackUIForm();
            end
        end
    elseif(_topuistyle==CS.EnumUIStyle.StoryUI)then      --【故事内】
        local bookreading = logic.UIMgr:GetView2(logic.uiid.BookReading)
        if(bookreading)then
            if(bookreading.chapterSwitch.ui.gameObject.activeSelf==true)then
                logic.bookReadingMgr:BackToMainClick() --退出剧情
                --是否已经评星过     0：否   1：是
                if(logic.cs.UserDataManager.userInfo.data.userinfo.is_store_score==0)then
                    --打开评星
                    GameHelper.OpenRating();
                end
                logic.cs.CUIManager:ClearBackUIForm();
                return;
            end
        end
        --弹窗提示  是否要退出剧情
        logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.AlertUiForm)
        Alertui = logic.cs.CUIManager:GetForm(logic.cs.UIFormName.AlertUiForm):GetComponent(typeof(CS.AlertForm))
        if(Alertui)then
            local tips=CS.CTextManager.Instance:GetText(415); --提示
            local _content=CS.CTextManager.Instance:GetText(320);--您不在继续体验一会吗？是否返回主界面？
            local _quit=CS.CTextManager.Instance:GetText(418);  -- "退出"
            local _againplaytex2=CS.CTextManager.Instance:GetText(417);  -- "再玩一会"

            Alertui:SetMessage(tips,_content,CS.AlertType.SureOrCancel,function(boo) GameHelper.OnQuitStory(boo) end,_againplaytex2,_againplaytex2,_quit);
        end
        logic.cs.CUIManager:ClearBackUIForm();
    elseif(_topuistyle==CS.EnumUIStyle.CommonUI)then   --【通用弹窗】
        --弹窗提示  是否要退出游戏
        GameHelper.AlertFormQuit();
        logic.cs.CUIManager:ClearBackUIForm();
    elseif(_topuistyle==CS.EnumUIStyle.None)then
        return;
    elseif(_topuistyle==CS.EnumUIStyle.Special)then  --【特殊界面】

        --弹窗界面 用户点击BACK  关闭弹窗  （主界面协议界面除外）
        if(curUI ~= nil and curUI.uistyle == CS.EnumUIStyle.Special and curUI.m_formPath~="")then
            if(curUI.isLua)then
                local uiLoading = logic.UIMgr:GetView2(logic.uiid.BookLoading)
                if(uiLoading)then
                    if(uiLoading.btnBack.gameObject.activeSelf==true)then
                        uiLoading:OnBackToMainClick()
                        logic.cs.CUIManager:ClearBackUIForm();
                        return;
                    end
                end
            else
                local curCsharp=curUI.gameObject:GetComponent(typeof(CS.ProfileForm))
                if(curCsharp)then
                    if(curCsharp.mCueMenuIndex==1)then
                        curCsharp:PerMenuClickHandler();
                        logic.cs.CUIManager:ClearBackUIForm();
                        return;
                    end

                    if(curCsharp.CommunityWindow.activeSelf==true)then
                        curCsharp:BackButton();
                        logic.cs.CUIManager:ClearBackUIForm();
                        return;
                    end

                    --弹窗提示  是否要退出游戏
                    GameHelper.AlertFormQuit();
                    logic.cs.CUIManager:ClearBackUIForm();
                end
                return;
            end
        end
    end

end
--endregion


--region【弹窗提示  是否要退出游戏】
function GameHelper.AlertFormQuit()
    --弹窗提示  是否要退出游戏
    local tips=CS.CTextManager.Instance:GetText(415); --提示
    local _content=CS.CTextManager.Instance:GetText(416);  -- "您不在继续体验一会吗？"
    local _againplaytex2=CS.CTextManager.Instance:GetText(417);  -- "再玩一会"
    local _quit=CS.CTextManager.Instance:GetText(418);  -- "退出"

    logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.AlertUiForm);
    CS.XLuaHelper.BackQuitGame(tips,_content,_againplaytex2,_againplaytex2,_quit);
end
--endregion


--region 【*评星*  完成评分 进入 完成评分反馈界面】
GameHelper.isRating=false;
--完成评分 进入 完成评分反馈界面
function GameHelper.SetIGGPlatformComplet()
    --完成评分 进入 完成评分反馈界面
    local ratring = logic.UIMgr:GetView(logic.uiid.UIRatinglevelForm)
    if(ratring)then
        ratring:SetEnumRatinglevel(EnumRatinglevel.IGGPlatformComplete);
    end
end
--endregion


--region 【*评星*  完成反馈建议 进入 提交后界面】
--完成反馈建议 进入 提交后界面
function GameHelper.SetFeedbackSend()
    --完成反馈建议 进入 反馈建议 提交后界面
    local ratring = logic.UIMgr:GetView(logic.uiid.UIRatinglevelForm)
    if(ratring)then
        ratring:SetEnumRatinglevel(EnumRatinglevel.FeedbackSend);
    end
end
--endregion


--region 【*评星*  打开评星界面】
function GameHelper.OpenRating()
    if(GameHelper.isRating==true)then
        --打开评星界面
        logic.UIMgr:Open(logic.uiid.UIRatinglevelForm);
    end
end

--endregion


--region【游戏切出】
function GameHelper.OnGameleave()
    --如果锁死了  就跳出
    if(Cache.ReadTimeCache.isEND==true)then GameHelper.CloseReadTimer() return; end
    --停止计时器
    GameHelper.StopReadTimer();
end
--endregion


--region【游戏切回】
function GameHelper.OnGameback()

    if(logic.cs.MyBooksDisINSTANCE:GetIsPlaying()==true)then
        --如果锁死了  就跳出
        if(Cache.ReadTimeCache.isEND==true)then return; end
        --开启计时器
        GameHelper.StartReadTimer()
    end
end
--endregion


--region【在线阅读时长计算】
local num=0;
GameHelper.onlinereadTime=0;
GameHelper.readTimer=nil;

--在线阅读时长计算
function GameHelper.OnlineReadingtime()
    GameHelper.CloseReadTimer();
    --如果锁死了  就跳出
    if(Cache.ReadTimeCache.isEND==true)then return; end
    num=0;
    GameHelper.onlinereadTime=Cache.ReadTimeCache.online_time;
    GameHelper.readTimer = core.Timer.New(function() GameHelper.AddReadTime()  end,1,-1);
    --开启计时器
    GameHelper.StartReadTimer();
end

--计时
function GameHelper.AddReadTime()
    num=num+1;
    GameHelper.onlinereadTime=GameHelper.onlinereadTime+1;
    --logic.debug.LogError("GameHelper.onlinereadTime"..GameHelper.onlinereadTime);
    if(num>=60)then
        GameHelper.ResetStart();
    end
end

--重新请求时间 重新开始计时
function GameHelper.ResetStart()
    GameHelper.CloseReadTimer(); --关闭并销毁计时器
    num=0;
    local AddTime=GameHelper.onlinereadTime-Cache.ReadTimeCache.online_time;

    --logic.debug.LogError("AddTime"..AddTime);
    --请求
    GameController.ActivityControl:ReadingTaskTimeRequest(AddTime);
end

--开启计时器
function GameHelper.StartReadTimer()
    if(GameHelper.readTimer)then
        GameHelper.readTimer:Start();
    end
end

--停止计时器
function GameHelper.StopReadTimer()
    if(GameHelper.readTimer)then
        GameHelper.readTimer:Stop();
    end
end

--关闭并销毁计时器
function GameHelper.CloseReadTimer()
    if(GameHelper.readTimer)then
        GameHelper.readTimer:Stop();
        GameHelper.readTimer=nil;
    end
end

--endregion


--region【点击书本】

GameHelper.CurBookId=nil;
function GameHelper.BookClick(m_bookDetailCfg)

    --书本信息为空  弹窗提示
    if( m_bookDetailCfg==nil and (CS.XLuaHelper.is_Null(m_bookDetailCfg)==true))then
        logic.cs.UIAlertMgr:Show(logic.cs.GameDataMgr.table:GetLocalizationById(225),logic.cs.GameDataMgr.table:GetLocalizationById(226));
        return;
    end

    local bookID = m_bookDetailCfg.book_id;
    --设置当前选择书本
    logic.cs.UserDataManager.UserData.CurSelectBookID = bookID;
    logic.cs.AudioManager:PlayTones(logic.cs.AudioTones.book_click);

    --打开书本详情界面
    logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.BookDisplayForm);
    --设置展示书本ID  要展示的书本
    CS.XLuaHelper.OpenBookDisplayForm(logic.cs.UserDataManager.UserData.CurSelectBookID);

end

--endregion


--region 【比较两个时间差】

function GameHelper.timediff(long_time,short_time)
    local n_short_time,n_long_time,carry,diff = os.date('*t',short_time),os.date('*t',long_time),false,{}
    local colMax = {60,60,24,os.date('*t',os.time{year=n_short_time.year,month=n_short_time.month+1,day=0}).day,12,0}
    n_long_time.hour = n_long_time.hour - (n_long_time.isdst and 1 or 0) + (n_short_time.isdst and 1 or 0) -- handle dst
    for i,v in ipairs({'sec','min','hour','day','month','year'}) do
        diff[v] = n_long_time[v] - n_short_time[v] + (carry and -1 or 0)
        carry = diff[v] < 0
        if carry then
            diff[v] = diff[v] + colMax[i]
        end
    end
    return diff
end

--endregion


--region 【判断list里是否有东西】
function GameHelper.islistHave(_list)
    local len =table.length(_list);
    if(_list and len>0)then
        return true;
    end
    return false;
end
--endregion


--region【展示书本封面】

function GameHelper.ShowIcon(book_id,BookBG)
    local sprite=CS.BookItemManage.Instance:ShowIcon(book_id);
    if(sprite and (CS.XLuaHelper.is_Null(sprite)==false))then
        BookBG.sprite=sprite;
    end
end

--endregion


--region【展示书本背景】

function GameHelper.ShowChapterBG(book_id,BookBG)
    logic.cs.ABSystem.ui:DownloadBookCover(book_id,function(id,refCount)
        if(BookBG==nil or (CS.XLuaHelper.is_Null(BookBG)==true))then
            refCount:Release();
            return;
        end
        if(book_id ~= id)then
            refCount:Release();
            return;
        end
        local sprite = refCount:GetObject();
        BookBG.sprite = sprite;
    end)
end

--endregion


--region 【展示书本标签】

function GameHelper.ShowBookType(book_id,BookTypeImg)
    local m_bookDetailCfg= logic.cs.JsonDTManager:GetJDTBookDetailInfo(book_id);
    if(m_bookDetailCfg and (CS.XLuaHelper.is_Null(m_bookDetailCfg)==false))then
        local bookTypeIndex = m_bookDetailCfg.Type1Array[0];

        local num=bookTypeIndex-1;
        if(num<0)then return; end
        --通过编号获取string
        local bookTypeStr = logic.cs.UserDataManager:GetBookTypeName(num);
        BookTypeImg.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_bq-"..bookTypeStr);
        BookTypeImg.gameObject:SetActive(true);
    else
        if(book_id)then
            logic.debug.LogError("m_bookDetailCfg is Null + book_id:"..book_id);
        else
            logic.debug.LogError("book_id is Null");
        end
    end
end

--展示书本标签2
function GameHelper.ShowBookType2(book_id,BookTypeImg)
    local m_bookDetailCfg= logic.cs.JsonDTManager:GetJDTBookDetailInfo(book_id);
    if(m_bookDetailCfg and (CS.XLuaHelper.is_Null(m_bookDetailCfg)==false))then
        if(m_bookDetailCfg.Type1Array ~= nil and m_bookDetailCfg.Type1Array.Count > 1)then
            local bookTypeIndex = m_bookDetailCfg.Type1Array[1];

            local num=bookTypeIndex-1;
            if(num<0)then return; end
            --通过编号获取string
            local bookTypeStr = logic.cs.UserDataManager:GetBookTypeName(num);
            BookTypeImg.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_bq-"..bookTypeStr);
            BookTypeImg.gameObject:SetActive(true);
        else
            BookTypeImg.gameObject:SetActive(false);
        end
    else
        if(book_id)then
            logic.debug.LogError("m_bookDetailCfg is Null + book_id:"..book_id);
        else
            logic.debug.LogError("book_id is Null");
        end
    end
end

--展示书本标签【多个】
function GameHelper.ShowBookTypeX(book_id,BookTypeImg1,BookTypeImg2,BookTypeImg3,BookTypeImg4,BookTypeImg5)
    local m_bookDetailCfg= logic.cs.JsonDTManager:GetJDTBookDetailInfo(book_id);
    if(m_bookDetailCfg and (CS.XLuaHelper.is_Null(m_bookDetailCfg)==false))then
        if(m_bookDetailCfg.Type1Array ~= nil and m_bookDetailCfg.Type1Array.Count > 0)then


            for i = 1, m_bookDetailCfg.Type1Array.Count do

                local index=i-1;

                local bookTypeIndex = m_bookDetailCfg.Type1Array[index];
                local num=bookTypeIndex-1;
                if(num<0)then return; end
                --通过编号获取string
                local bookTypeStr = logic.cs.UserDataManager:GetBookTypeName(num);
                if(i==1)then
                    BookTypeImg1.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_bq-"..bookTypeStr);
                    BookTypeImg1.gameObject:SetActive(true);
                elseif(i==2)then
                    BookTypeImg2.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_bq-"..bookTypeStr);
                    BookTypeImg2.gameObject:SetActive(true);
                elseif(i==3)then
                    BookTypeImg3.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_bq-"..bookTypeStr);
                    BookTypeImg3.gameObject:SetActive(true);
                elseif(i==4)then
                    BookTypeImg4.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_bq-"..bookTypeStr);
                    BookTypeImg4.gameObject:SetActive(true);
                elseif(i==5)then
                    BookTypeImg5.sprite = CS.ResourceManager.Instance:GetUISprite("Common/com_bq-"..bookTypeStr);
                    BookTypeImg5.gameObject:SetActive(true);
                end
            end

        end
    else
        if(book_id)then
            logic.debug.LogError("m_bookDetailCfg is Null + book_id:"..book_id);
        else
            logic.debug.LogError("book_id is Null");
        end
    end
end




--endregion


--region 【展示书本进度条】

function GameHelper.ShowProgress(book_id,ProgressBar,Tips)
    local bookInfo=Cache.MainCache:GetMyBookByIndex(book_id);
    if(ProgressBar)then
        if(bookInfo==nil)then
            ProgressBar.gameObject:SetActive(false);
            ProgressBar.Value = 0;

            GameHelper.ShowNewUpdate(book_id,0,Tips);
        else
            ProgressBar.gameObject:SetActive(true);
            ProgressBar.Value =bookInfo.dialogid / bookInfo.end_dialogid;

            GameHelper.ShowNewUpdate(book_id,bookInfo.finish_max_chapter,Tips);
        end
    end
end

function GameHelper.ShowProgress1(obj)
    local bookid=obj[0];
    local ProgressBar=obj[1];
    local Tips=obj[2];
    if(bookid and ProgressBar and  CS.XLuaHelper.is_Null(ProgressBar)==false)then
        local bookInfo=Cache.MainCache:GetMyBookByIndex(bookid);
        if(bookInfo==nil)then
            ProgressBar.gameObject:SetActive(false);
            ProgressBar.fillAmount = 0;

            GameHelper.ShowNewUpdate(bookid,0,Tips);
        else
            ProgressBar.gameObject:SetActive(true);
            ProgressBar.fillAmount =bookInfo.dialogid / bookInfo.end_dialogid;

            GameHelper.ShowNewUpdate(bookid,bookInfo.finish_max_chapter,Tips);
        end
    end
end
--endregion


--region 【展示书本New、Update】

function GameHelper.ShowNewUpdate(book_id,finish_max_chapter,Tips)
    local bookinfo = logic.cs.UserDataManager:GetBookItemInfo(book_id);
    if(bookinfo and Tips and CS.XLuaHelper.is_Null(Tips)==false)then

        if(bookinfo.tag=="")then
            Tips:SetActive(false);

        else
            if(bookinfo.chapteropen)then
                if(finish_max_chapter>=bookinfo.chapteropen)then
                    Tips:SetActive(false);
                else
                    Tips:SetActive(true);
                    local BookTagText =CS.DisplayUtil.GetChild(Tips, "Text"):GetComponent("Text");

                    if(bookinfo.tag=="New")then
                        BookTagText.fontSize=20;
                        BookTagText.text="NEW";
                        return;
                    end
                    if(bookinfo.tag=="Update")then
                        BookTagText.fontSize=16;
                        BookTagText.text="Update";
                        return;
                    end
                end
            else
                Tips:SetActive(false);
            end
        end
    else
        Tips:SetActive(false);
    end
end



function GameHelper.ShowNewBg(book_id,Tips)

    local MainbookInfo=Cache.MainCache:GetMyBookByIndex(book_id);
    local _finish_max_chapter=0;
    if(MainbookInfo)then
        _finish_max_chapter=MainbookInfo.finish_max_chapter;
    end
    local bookinfo = logic.cs.UserDataManager:GetBookItemInfo(book_id);
    if(bookinfo and Tips and CS.XLuaHelper.is_Null(Tips)==false)then
        if(bookinfo.tag=="")then
            Tips:SetActive(false);
        else
            if(bookinfo.chapteropen)then
                if(_finish_max_chapter>=bookinfo.chapteropen)then
                    Tips:SetActive(false);
                else
                    if(bookinfo.tag=="New")then
                        Tips:SetActive(true);
                    else
                        Tips:SetActive(false);
                    end
                end
            else
                Tips:SetActive(false);
            end
        end
    else
        Tips:SetActive(false);
    end
end



function GameHelper.ShowNewBg1(obj)
    local book_id=obj[0];
    local Tips=obj[1];
    local MainbookInfo=Cache.MainCache:GetMyBookByIndex(book_id);
    local _finish_max_chapter=0;
    if(MainbookInfo)then
        _finish_max_chapter=MainbookInfo.finish_max_chapter;
    end
    local bookinfo = logic.cs.UserDataManager:GetBookItemInfo(book_id);
    if(bookinfo and Tips and CS.XLuaHelper.is_Null(Tips)==false)then
        if(bookinfo.tag=="")then
            Tips:SetActive(false);
        else
            if(bookinfo.chapteropen)then
                if(_finish_max_chapter>=bookinfo.chapteropen)then
                    Tips:SetActive(false);
                else
                    local BookTagText =CS.DisplayUtil.GetChild(Tips, "BookTagText"):GetComponent("Text");
                    if(bookinfo.tag=="New")then
                        Tips:SetActive(true);
                        BookTagText.text="NEW";
                        return;
                    end
                    if(bookinfo.tag=="Update")then
                        Tips:SetActive(false);
                        BookTagText.text="Update";
                        return;
                    end
                end
            else
                Tips:SetActive(false);
            end
        end
    else
        Tips:SetActive(false);
    end
end

--endregion


--region 【主界面底框切换】
function GameHelper.OpenMainDownToggle(_type)
    local maindown = logic.UIMgr:GetView(logic.uiid.UIMainDownForm);
    if(maindown)then
        if(_type==MainDown.HomeToggle)then
            maindown.HomeToggle.isOn=true;
            maindown:HomeToggleClick(nil);     -- 【点击Home标签】
        elseif(_type==MainDown.SearchToggle)then
            maindown.SearchToggle.isOn=true;
            maindown:SearchToggleClick(nil);    --【点击搜索按钮】
        elseif(_type==MainDown.CommunityToggle)then
            maindown.CommunityToggle.isOn=true;
            maindown:CommunityToggleClick(nil);   --【点击创作按钮】
        elseif(_type==MainDown.RwardToggle)then
            maindown.RwardToggle.isOn=true;
            maindown:RwardToggleClick(nil);      --【点击活动奖励按钮】
        elseif(_type==MainDown.ProfileToggle)then
            maindown.ProfileToggle.isOn=true;
            maindown:ProfileToggleClick(nil);     --【点击个人中心按钮】
        end
    end
end
--endregion


--region 【获取章节介绍】
function GameHelper.GetChapterDiscription(book_id)
    --local m_bookDetailCfg= logic.cs.JsonDTManager:GetJDTBookDetailInfo(book_id);
    local chapterInfo = logic.cs.JsonDTManager:GetJDTChapterInfo(book_id,1)
    if(chapterInfo ~= nil )then
        return chapterInfo.dsc;
    else
        if(book_id)then
            logic.debug.LogError("m_bookDetailCfg is Null + book_id:"..book_id);
        else
            logic.debug.LogError("book_id is Null");
        end
    end
end
--endregion


--region 【获取章节数量】

function GameHelper.GetChapterCount(book_id)
    local m_bookDetailCfg= logic.cs.JsonDTManager:GetJDTBookDetailInfo(book_id);
    if(m_bookDetailCfg and (CS.XLuaHelper.is_Null(m_bookDetailCfg)==false))then
        return m_bookDetailCfg.chaptercount;
    else
        if(book_id)then
            logic.debug.LogError("m_bookDetailCfg is Null + book_id:"..book_id);
        else
            logic.debug.LogError("book_id is Null");
        end
    end
end

--endregion


--region【保存装扮缓存】
function GameHelper.SetDressUpCache(obj)
    local AvatarId=obj[0];
    local AvatarframeId=obj[1];
    if(AvatarId>0 and AvatarframeId>0)then
        Cache.DressUpCache.avatar=AvatarId;
        Cache.DressUpCache.avatar_frame=AvatarframeId;
    end
end
--endregion


--region【展示已装扮的头像】
function GameHelper.ShowAvatar(obj)
    local AvatarId=obj[0];   --AvatarId  为-1  是显示当前已装扮
    local imgObj=obj[1];
    GameHelper.luaShowDressUpForm(AvatarId,imgObj,DressUp.Avatar,1001);
end
--endregion


--region【展示已装扮的头像框】
function GameHelper.ShowAvatarframe(obj)
    local AvatarframeId=obj[0];   --AvatarframeId  为-1  是显示当前已装扮
    local imgObj=obj[1];
    GameHelper.luaShowDressUpForm(AvatarframeId,imgObj,DressUp.AvatarFrame,2001);
end
--endregion


--region【lua内 调用 展示装扮】
--DressUpId 装扮id
--image  显示图片的 image
--type 类型
--defaultID 默认 id
function GameHelper.luaShowDressUpForm(DressUpId,image,_type,defaultID)
    local curDressUpForm;
    if(_type==DressUp.Avatar)then
        curDressUpForm=Cache.DressUpCache.avatar;
    elseif(_type==DressUp.AvatarFrame)then
        curDressUpForm=Cache.DressUpCache.avatar_frame;
    elseif(_type==DressUp.BarrageFrame)then
        curDressUpForm=Cache.DressUpCache.barrage_frame;
    elseif(_type==DressUp.CommentFrame)then
        curDressUpForm=Cache.DressUpCache.comment_frame;
    end
    if(DressUpId)then DressUpId=tonumber(DressUpId); end

    if(DressUpId and image and CS.XLuaHelper.is_Null(image)==false)then
        if(DressUpId<0)then
            if(curDressUpForm and curDressUpForm>0)then
                --【加载头像】
                GameController.DressUpControl:ShowDressUp(curDressUpForm,image);
            else
                --【加载头像】  默认编号
                GameController.DressUpControl:ShowDressUp(defaultID,image);
            end
        elseif(DressUpId>0)then
            --【加载头像】
            GameController.DressUpControl:ShowDressUp(DressUpId,image);
        else
            --【加载头像】  默认编号
            GameController.DressUpControl:ShowDressUp(defaultID,image);
        end
    end
end
--endregion


--region【将C#的Dic转成Lua的Table】
function GameHelper.DicToLuaTable(Dic)
    --将C#的Dic转成Lua的Table
    local dic = {}
    if Dic then
        local iter = Dic:GetEnumerator()
        while iter:MoveNext() do
            local k = iter.Current.Key
            local v = iter.Current.Value
            dic[k] = v
        end
    end
    return dic
end
--endregion


--region【数字是否不为空 并大于0】
function GameHelper.IsNotNilOrZero(Num)
    if(Num)then
        if(tonumber(Num) > 0)then
            return true;
        end
    end
    return false;
end
--endregion


--region【获取账号绑定状态】
function GameHelper.GetBindStatus()
    local UserInfo = GameHelper.DicToLuaTable(logic.cs.IGGSDKMrg.UserInfo)
    logic.debug.PrintTable(UserInfo,"UserInfo")
    if UserInfo.LoginType and #UserInfo.LoginType> 0 and UserInfo.LoginType ~= "Device" then
        return true
    end
    if GameHelper.IsNotNilOrZero(UserInfo.FBIsBind) or GameHelper.IsNotNilOrZero(UserInfo.GoogleIsBind) or GameHelper.IsNotNilOrZero(UserInfo.IGGIsBind)
            or GameHelper.IsNotNilOrZero(UserInfo.AppleIsBind) or GameHelper.IsNotNilOrZero(UserInfo.GameCenterIsBind) then
        return true
    end
    return false
end
--endregion


--region【限时活动免费读书 显示标签】
function GameHelper.Limit_time_Free(obj)
    local FreeKeyInfo=Cache.LimitTimeActivityCache:GetActivityInfo(EnumActivity.FreeKey);
    if(FreeKeyInfo and FreeKeyInfo.is_open==1)then
        if(obj and CS.XLuaHelper.is_Null(obj)==false)then
            obj:SetActive(true);
        end
    else
        if(obj and CS.XLuaHelper.is_Null(obj)==false)then
            obj:SetActive(false);
        end
    end
end
--endregion


--region【DayPass 显示标签】
function GameHelper.DayPass(obj)
    if(obj and CS.XLuaHelper.is_Null(obj)==false)then
        obj:SetActive(true);
    else
        obj:SetActive(false);
    end
end

--region【C#调用 限时活动免费读书 显示标签】
function GameHelper.DayPass1(obj)
    local bookId=obj[0];
    GameController.DayPassController:SetDayPass(bookId);
end
--endregion


--region【C#调用 限时活动免费读书 显示标签】
function GameHelper.ShowFree(obj)
    local BookFree=obj[0];
    local FreeKeyInfo=Cache.LimitTimeActivityCache:GetActivityInfo(EnumActivity.FreeKey);
    if(FreeKeyInfo and FreeKeyInfo.is_open==1)then
        if(BookFree)then
            BookFree:SetActive(true);
        end
    else
        if(BookFree)then
            BookFree:SetActive(false);
        end
    end
end
--endregion


--region【活动页面看广告倒计时】

--看广告倒计时
GameHelper.FR_Timer=nil;
--广告倒计时秒数
GameHelper.FR_timeNum=0;

--活动页面看广告倒计时
function GameHelper.FRPanel_CountDown(Timetext)
    if(GameHelper.FR_Timer==nil)then
        GameHelper.FR_Timer = core.Timer.New(function() GameHelper.UpdateFRTime(Timetext)  end,1,-1);
    end
    --开启计时器
    GameHelper.StartFRTimer();
end


function GameHelper.UpdateFRTime(Timetext)
    GameHelper.FR_timeNum=GameHelper.FR_timeNum-1;
    local hour =  math.modf( GameHelper.FR_timeNum / 3600 );
    local minute = math.fmod( math.modf(GameHelper.FR_timeNum / 60), 60 );
    local second = math.fmod(GameHelper.FR_timeNum, 60 );
    Timetext.text = string.format("%02d:%02d:%02d", hour, minute, second);

    if(GameHelper.FR_timeNum<0)then
        --先停止计时器
        GameHelper.StopFR_Timer();
    end
end


--开始倒计时
function GameHelper.StartFRTimer()
    --广告倒计时秒数
    GameHelper.FR_timeNum = Cache.ActivityCache.ad_countdown;

    if(GameHelper.FR_Timer)then
        GameHelper.FR_Timer:Start();
    end
end

--停止计时器
function GameHelper.StopFR_Timer()
    if(GameHelper.FR_Timer)then
        GameHelper.FR_Timer:Stop();
    end
end

--关闭并销毁计时器
function GameHelper.CloseFR_Timer()
    if(GameHelper.FR_Timer)then
        GameHelper.FR_Timer:Stop();
        GameHelper.FR_Timer=nil;
    end
end


--看广告倒计时
GameHelper.FR_CDTimer=nil;
--广告倒计时 CD冷却秒数
GameHelper.FR_CDNum=0;

--活动页面看广告倒计时
function GameHelper.FRCD_CountDown()
    if(GameHelper.FR_CDTimer==nil)then
        GameHelper.FR_CDTimer = core.Timer.New(function() GameHelper.UpdateFRCDTime()  end,1,-1);
        GameHelper.FR_CDTimer:Start();  --开启计时器
    end
end

function GameHelper.UpdateFRCDTime()
    if(GameHelper.FR_CDNum>0)then
        GameHelper.FR_CDNum=GameHelper.FR_CDNum-1;
        local str = string.format("%02d:%02d:%02d", 0, 0, GameHelper.FR_CDNum);
        GameController.ActivityControl:ShowCD(str);
    end

    if(GameHelper.FR_CDNum<=0)then
        --先停止计时器
        GameHelper.CloseFRCD_Timer();
        --结束CD操作
        GameController.ActivityControl:EndCD()
    end
end

--关闭并销毁计时器
function GameHelper.CloseFRCD_Timer()
    if(GameHelper.FR_CDTimer)then
        GameHelper.FR_CDTimer:Stop();
        GameHelper.FR_CDTimer=nil;
    end
end
--endregion


--region【活动首页Banner图轮播】【倒计时】
GameHelper.BN_Timer=nil;
function GameHelper.MainBannerTimer(callback)
    --销毁
    GameHelper.ClearBNTimer();
    if(GameHelper.BN_Timer==nil)then
        GameHelper.BN_Timer = core.Timer.New(function() GameHelper.UpdateBNTimer(callback)  end,12,-1);
    end
    --开启计时器
    GameHelper.StartBNTimer();
end

function GameHelper.UpdateBNTimer(callback)
    --【回调】
    if(callback)then
        callback();
    end
end

--开始倒计时
function GameHelper.StartBNTimer()
    if(GameHelper.BN_Timer)then
        GameHelper.BN_Timer:Start();
    end
end

--停止计时器
function GameHelper.StopBNTimer()
    if(GameHelper.BN_Timer)then
        GameHelper.BN_Timer:Stop();
    end
end

--关闭并销毁计时器
function GameHelper.ClearBNTimer()
    if(GameHelper.BN_Timer)then
        GameHelper.BN_Timer:Stop();
        GameHelper.BN_Timer=nil;
    end
end

--endregion


--region【显示网图】

function GameHelper.ShowNetImage(_url,picture)

    if(_url and picture and CS.XLuaHelper.is_Null(picture)==false)then
        logic.cs.ABSystem.ui:DownloadImage(_url, function(url, refCount)
            if _url ~= url then
                refCount:Release();
                return;
            end
            local sprite = refCount:GetObject();
            picture.sprite = sprite;
            --picture:SetNativeSize()
        end)
    end

end


local oldSprite=nil;
--【临时】【临时】
function GameHelper.ShowNetImage2(_url,picture)

    if(_url and picture and CS.XLuaHelper.is_Null(picture)==false)then
        if(oldSprite)then
            picture.sprite = oldSprite;
        else
            logic.cs.ABSystem.ui:DownloadImage(_url, function(url, refCount)
                if _url ~= url then
                    refCount:Release();
                    return;
                end
                local sprite = refCount:GetObject();

                picture.sprite = sprite;

            end)
        end
    end
end


--endregion


--region【展示创作书本封面】


local m_downloadSeq=0;
function GameHelper.ShowUGCStoryBg(cover_image,DefaultImg_sprite,BookIconImage)


    m_downloadSeq=m_downloadSeq+1

    if not string.IsNullOrEmpty(cover_image) then
        local url,md5 = logic.StoryEditorMgr:ParseImageUrl(cover_image)
        local filename = logic.config.WritablePath .. '/cache/story_image/'..md5

        local downloadSeq = m_downloadSeq
        logic.StoryEditorMgr.data:LoadSprite(filename, md5, url,function(sprite)
            if downloadSeq ~= m_downloadSeq then
                logic.debug.LogError('seq mismatch:'..downloadSeq..'<=>'..m_downloadSeq)
                return
            end
            GameHelper.SetBookCover(sprite,DefaultImg_sprite,BookIconImage);
        end)
    end

end


function GameHelper.SetBookCover(sprite,DefaultImg_sprite,BookIconImage)
    if(CS.XLuaHelper.is_Null(sprite)==false and DefaultImg_sprite)then
        sprite = DefaultImg_sprite;
    end

    if(BookIconImage==nil)then return; end
    logic.StoryEditorMgr:SetCover(BookIconImage,sprite,BookIconImage.transform.rect.size);
end


--endregion


--region【展示观看次数】
function GameHelper.ShowLookNumber(read_count,LookNumberText)
    if(LookNumberText==nil or read_count==nil or read_count<0)then return; end

    --【观看次数】【观看次数】【观看次数】
    local readcount=read_count;

    if(readcount >= 1000000)then
        local num=readcount/1000000;
        local _result = string.format("%.1f", num)
        --观看次数
        self.LookNumberText.text =tostring(_result)..'m';
    elseif (readcount > 1000)then
        local num=readcount/1000;
        local _result = string.format("%.1f", num)
        --观看次数
        LookNumberText.text =tostring(_result)..'k';
    else
        LookNumberText.text =tostring(readcount);
    end
end
--endregion


local oldSpriteList= {};
--【临时】【临时】
function GameHelper.ShowNetImage_Banner(_type,_url,picture)
    if(_url and picture and CS.XLuaHelper.is_Null(picture)==false)then
        local oldImg = table.trygetvalue(oldSpriteList,_type);
        if(oldImg)then
            picture.sprite = oldImg;
        else
            logic.cs.ABSystem.ui:DownloadImage(_url, function(url, refCount)
                if _url ~= url then
                    refCount:Release();
                    return;
                end
                local sprite = refCount:GetObject();
                picture.sprite = sprite;
                oldSpriteList[_type]=sprite;
            end)
        end
    end
end


--endregion