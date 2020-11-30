--[[
    面板-章节信息修改
]]
local BaseClass = core.Class
local UIChapterEditorView = BaseClass("UIChapterEditorView")


function UIChapterEditorView:__init(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
    self.uiBinding = gameObject:GetComponent(typeof(CS.UIBinding))

    
    ---chapter new
    self.chapter_btnOK = self.uiBinding:Get('chapter_btnOK', typeof(logic.cs.UITweenButton))
    self.chapter_btnClose = self.uiBinding:Get('chapter_btnClose', typeof(logic.cs.UITweenButton))
    --self.chapter_inName = self.uiBinding:Get('chapter_inName', typeof(logic.cs.InputField))
    self.chapter_inDesc = self.uiBinding:Get('chapter_inDesc', typeof(logic.cs.InputField))
    self.chapter_btnClose.onClick:AddListener(function()
        self:Hide()
    end)

    
    self.lbBookDescNumber = self.uiBinding:Get('lbBookDescNumber', typeof(logic.cs.Text))
    self.chapter_inDesc.onValueChanged:AddListener(function(text)
        local value = string.trim(text)
        local len = string.GetUtf8Len(value)
        self.lbBookDescNumber.text = string.format("%d/150",len)
    end)

    self.chapter_btnOK.onClick:AddListener(function()
        local name = ''--self.chapter_inName.text
        local desc = string.trim(self.chapter_inDesc.text)
        local len = string.GetUtf8Len(desc)
        if len < 10 or len > 150 then
            logic.debug.LogError(desc)
            logic.cs.UITipsMgr:PopupTips("Enter a 10-150 words description.", false)
            return
        end
        self:Hide()
        self.callback(desc)
    end)
    
end

function UIChapterEditorView:Show()
    self.gameObject:SetActiveEx(true)
end
function UIChapterEditorView:Hide()
    self.gameObject:SetActiveEx(false)
end

function UIChapterEditorView:SetData(strDesc,chapterID,callback)
    self.isNewChapter = (chapterID == 0)
    self.chapter_inDesc.text = strDesc
    self.callback = callback
end

return UIChapterEditorView