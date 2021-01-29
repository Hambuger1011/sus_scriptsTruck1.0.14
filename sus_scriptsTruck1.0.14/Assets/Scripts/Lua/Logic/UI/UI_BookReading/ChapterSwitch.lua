local Class = core.Class
local ChapterSwitch = Class("ChapterSwitch")

function ChapterSwitch:__init(root)
    self.ui = root:GetComponent(typeof(CS.UIChapterSwitch))

    self.DayPassBg = CS.DisplayUtil.GetChild(self.ui.btnContinue.gameObject, "DayPassBg");
    self.DayPassBg2 = CS.DisplayUtil.GetChild(self.ui.btnRestart.gameObject, "DayPassBg");

    --【请求获取限时活动状态】
    GameController.ActivityControl:GetActivityInfoRequest(EnumActivity.FreeKey);
end

function ChapterSwitch:__delete()
end

function ChapterSwitch:Show()
    logic.bookReadingMgr.isReading = false
    local component = logic.bookReadingMgr.playingComponent
    local bookData = logic.bookReadingMgr.bookData
    self.ui:SetData(bookData.BookID,bookData.ChapterID,component.cfg.dialogID,component.cfg.next)
    self.ui:Show()
    logic.cs.UINetLoadingMgr:Close()
    if logic.config.channel == Channel.Spain then
        if logic.cs.UserDataManager.userInfo and logic.cs.UserDataManager.lotteryDrawInfo.data.bookadcount > 0 then
            logic.UIMgr:Open(logic.uiid.GetADiamonds)
        else
            logic.debug.Log('可看广告次数:0')
        end
    end
end


function ChapterSwitch:Limit_time_Free()
    if(self.ui.BookFree1)then
        GameHelper.Limit_time_Free(self.ui.BookFree1)
    end

    if(self.ui.BookFree2)then
        GameHelper.Limit_time_Free(self.ui.BookFree2)
    end
end

function ChapterSwitch:DayPass(bookId);
    local bookData = logic.bookReadingMgr.bookData
    if(bookData)then
        if(bookData.BookID==bookId)then
            self.DayPassBg:SetActive(true);
            self.DayPassBg2:SetActive(true);
            self.ui.restartKeyIcon:SetActive(false);
            self.ui.continueKeyIcon:SetActive(false);
        else
            self.DayPassBg:SetActive(false);
            self.DayPassBg2:SetActive(false);
        end
    end
end


function ChapterSwitch:Hide()
    self.ui:Hide()
end

return ChapterSwitch