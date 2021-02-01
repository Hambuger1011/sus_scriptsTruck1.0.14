local BaseClass = core.Class
local UIView = core.UIView
---@class UIGuide
local UIGuide = BaseClass("UIGuide", UIView)
local base = UIView

local uiid = logic.uiid

UIGuide.config = {
	ID = uiid.Guide,
	AssetName = 'UI/Resident/UI/Canvas_Guide'
}


function UIGuide:OnInitView()
    UIView.OnInitView(self)
    self.bookItemList = {}
    self.bookNameList = {}
    self.bookBtnList = {}
    self.bookImageList = {}
    self.playImageList = {}
    self.selectIndex = 0
    local root = self.uiform.transform
    self.uiBinding = root:GetComponent(typeof(CS.UIBinding))
    self.Page1 = self.uiBinding:Get('Page1').gameObject
    self.Page2 = self.uiBinding:Get('Page2').gameObject
    for i = 1 , #self.bookData do
        self.bookItemList[i] = self.uiBinding:Get('bookItem'..i)
        self.bookNameList[i] = self.bookItemList[i].transform:Find("Image/Image/Text"):GetComponent(typeof(logic.cs.Text))
        self.bookBtnList[i] = self.bookItemList[i].transform:Find("Button"):GetComponent(typeof(logic.cs.Button))
        self.bookImageList[i] = self.bookItemList[i].transform:Find("BookImage"):GetComponent(typeof(logic.cs.Image))
        self.playImageList[i] = self.bookItemList[i].transform:Find("PlayImage"):GetComponent(typeof(logic.cs.Image))
        self.bookItemList[i].gameObject:SetActiveEx(false)
        self.playImageList[i].gameObject:SetActiveEx(false)
    end
    
    self.closeBtn = self.uiBinding:Get('closeBtn' , typeof(logic.cs.Button))
    self.closeBtn.onClick:RemoveAllListeners()
    self.closeBtn.onClick:AddListener(function()
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NewerOut)
        self:__Close()
        logic.cs.talkingdata:OpenApp("OpenMainForm")
        --logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.MainForm)
        logic.UIMgr:Open(logic.uiid.UIMainForm);
    end)
    
    self.switchBtn = self.uiBinding:Get('switchBtn' , typeof(logic.cs.Button))
    self.switchBtn.gameObject:SetActiveEx(false)
    self.switchBtn.transform:Find("Text"):GetComponent(typeof(logic.cs.Text)).text = "SWAP"
    self.switchBtn.onClick:RemoveAllListeners()
    self.switchBtn.onClick:AddListener(function()
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NewerNextBook)
        if self.Page1.activeInHierarchy then --先显示后隐藏  避免切换白屏
            self.Page2:SetActiveEx(true)
            self.Page1:SetActiveEx(false)
        else
            self.Page1:SetActiveEx(true)
            self.Page2:SetActiveEx(false)
        end
    end)
    
    self:UpdateBookInfo()
end


function UIGuide:OnOpen()
    UIView.OnOpen(self)
end

function UIGuide:OnClose()
    UIView.OnClose(self)
end

function UIGuide:GetRecommandBook(callBack)
    logic.cs.GameHttpNet:GetRecommendABook(os.time,function(result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            self.bookData = json.data.book_list
            local size = #self.bookData
            if size > 3 then
                size = 3
            end
            self.bookImageData = {}
            local downloadNum = 0
            logic.cs.ABSystem.ui:BannerLoadListClear()
            for i = 1 , size do
                logic.cs.ABSystem.ui:AddBannerLoadList(self.bookData[i].id
                ,function(id,refCount)
                    if self.bookData[i].id ~= id then
                        refCount:Release()
                        return
                    end
                    local sprite = refCount:GetObject()
                    self.bookImageData[i] = sprite
                    downloadNum = downloadNum + 1
                    if downloadNum == size then
                        callBack()
                    end
                end
                )
            end
        elseif code == 277 then
            logic.cs.UIAlertMgr:Show("TIPS",json.msg)
        end
    end)
end

function UIGuide:UpdateBookInfo()
    for i = 1 , #self.bookData do
        self.bookItemList[i].gameObject:SetActiveEx(true)
        self.bookNameList[i].text = self.bookData[i].booktypename
        self.bookImageList[i].sprite = self. bookImageData[i]
        
        self.bookBtnList[i].onClick:RemoveAllListeners()
        self.bookBtnList[i].onClick:AddListener(function()
            self:BookOnClick(i)
        end)
    end
end

function UIGuide:BookOnClick(index)
    if true then
        local bookID = self.bookData[index].id
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NewerReadBook,"","",tostring(bookID))
        if bookID then
            logic.cs.UserDataManager.UserData.CurSelectBookID = bookID
        else
            logic.cs.UserDataManager.UserData.CurSelectBookID = 1
        end
        logic.cs.GameHttpNet:BuyChapter(tonumber(bookID),1,function(result)
            if result.code == 200 then
                logic.cs.UserDataManager:SetBuyChapterResultInfo(bookID,result)
                logic.cs.UserDataManager.UserData.IsSelectFirstBook = 1
                logic.cs.TalkingDataManager:SelectBooksInEnter(bookID)
                logic.cs.GameDataMgr.userData:AddMyBookId(bookID)
                
                logic.cs.BookReadingWrapper:ChangeBookDialogPath(bookID,1)
                local chapterid= tonumber(result.data.step_info.chapterid)
                local dialogid = tonumber(result.data.step_info.dialogid)
                
                local chapterInfo = logic.cs.JsonDTManager:GetJDTChapterInfo(bookID,chapterid);
                local beginDialogID = 1;
                local endDialogID = beginDialogID
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
                        1,
                        dialogid,
                        beginDialogID,
                        endDialogID
                )
                
                logic.cs.GameHttpNet:GetBookVersionInfo(tonumber(bookID),1,function(result)
                    local bookVersionJson = core.json.Derialize(result)
                    local code = tonumber(bookVersionJson.code)
                    if code == 200 then
                        
                        logic.cs.UserDataManager.bookJDTFormSever = bookVersionJson

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
                        logic.cs.GameHttpNet:GetBookBarrageCountList(tonumber(bookID),1,function(result)
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
                        
                    end
                end )
                
                
            elseif code == 277 then
                logic.cs.UIAlertMgr:Show("TIPS",json.msg)
            end
        end)
        return
    end
    logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.NewerSelectBook,"","",tostring(self.bookData[index].id))
    if self.selectIndex ~= 0 then
        self.playImageList[self.selectIndex].gameObject:SetActiveEx(false)
        self.bookItemList[self.selectIndex].transform:DOScale(core.Vector3.New(1,1,1),0.5)
    end
    self.selectIndex = index
    self.playImageList[index].gameObject:SetActiveEx(true)
    self.bookItemList[self.selectIndex].transform:DOScale(core.Vector3.New(1.07,1.07,1),0.5):SetEase(core.tween.Ease.OutBack)
end

return UIGuide