local BaseClass = core.Class
local UIView = core.UIView
---@class UINewBookTipsForm
local UINewBookTipsForm = BaseClass("UINewBookTipsForm", UIView)
local uiid = logic.uiid
local LeftPos = Vector2.New(-632, 58)
local RightPos = Vector2.New(640, 58)
local MiddlePos = Vector2.New(4, 58)
local ItemList = {}
local Index = 1
local newBookList = {}

UINewBookTipsForm.config = {
    ID = uiid.UINewBookTipsForm,
    AssetName = 'UI/Resident/UI/UINewBookTipsForm'
}

function UINewBookTipsForm:OnInitView()
    UIView.OnInitView(self)
    ItemList = {}
    Index = 1
    local this = self.uiform
    self.LeftBtn = CS.DisplayUtil.GetChild(this.gameObject, "LeftBtn"):GetComponent(typeof(logic.cs.Button))
    self.RightBtn = CS.DisplayUtil.GetChild(this.gameObject, "RightBtn"):GetComponent(typeof(logic.cs.Button))
    self.Item = CS.DisplayUtil.GetChild(this.gameObject, "Item"):GetComponent(typeof(logic.cs.Button))
    self.Bg = CS.DisplayUtil.GetChild(this.gameObject, "Bg"):GetComponent(typeof(logic.cs.Button))
    self.ViewBtn = CS.DisplayUtil.GetChild(this.gameObject, "ViewBtn"):GetComponent(typeof(logic.cs.Button))
    self.Item.gameObject:SetActive(false)

    self.Bg.onClick:RemoveAllListeners()
    self.Bg.onClick:AddListener(function()
        self:OnExitClick()
    end)

    self.LeftBtn.onClick:RemoveAllListeners()
    self.LeftBtn.onClick:AddListener(function()
        self:ShowLastItem()
    end)

    self.RightBtn.onClick:RemoveAllListeners()
    self.RightBtn.onClick:AddListener(function()
        self:ShowNextItem()
    end)

    self.ViewBtn.onClick:RemoveAllListeners()
    self.ViewBtn.onClick:AddListener(function()
        self:ViewBook()
    end)

    self.Item.onClick:RemoveAllListeners()
    self.Item.onClick:AddListener(function()
        self:ViewBook()
    end)
end

function UINewBookTipsForm:SetData(data)
    newBookList = data
    self.LeftBtn.gameObject:SetActive( #newBookList > 1)
    self.RightBtn.gameObject:SetActive(#newBookList > 1)
    for k, v in pairs(newBookList) do
        local item = logic.cs.GameObject.Instantiate(self.Item.gameObject, self.Bg.transform, false)
        local Image = item:GetComponent(typeof(logic.cs.Image))
        local Button = item:GetComponent(typeof(logic.cs.Button))
        local Icon = CS.DisplayUtil.GetChild(item.gameObject, "Icon")
        logic.cs.ABSystem.ui:DownloadBookCover(v.book_id, function(id, refCount)
            if v.book_id ~= id then
                refCount:Release()
                return
            end
            local sprite = refCount:GetObject()
            if not IsNull(sprite) then
                Image.sprite = sprite
            end
        end)
        if tostring(v.tag) == "New" then
            Icon:SetActive(true)
        else
            Icon:SetActive(false)
        end
        if k == Index then
            item.transform.anchoredPosition = MiddlePos
        else
            item.transform.anchoredPosition = RightPos
        end

        Button.onClick:RemoveAllListeners()
        Button.onClick:AddListener(function()
            self:ViewBook()
        end)
        item:SetActive(true)
        table.insert(ItemList, item)
    end
end

function UINewBookTipsForm:ShowLastItem()
    ItemList[Index].transform:DOLocalMoveX(RightPos.x, 0.3):SetEase(core.tween.Ease.OutBack):Play()
    if Index == 1 then
        Index = #ItemList
    else
        Index = Index - 1
    end
    ItemList[Index].transform.anchoredPosition = LeftPos
    ItemList[Index].transform:DOLocalMoveX(MiddlePos.x, 0.3):SetDelay(0.15):SetEase(core.tween.Ease.OutBack):Play()
end

function UINewBookTipsForm:ShowNextItem()
    ItemList[Index].transform:DOLocalMoveX(LeftPos.x, 0.3):SetEase(core.tween.Ease.OutBack):Play()
    if Index == #ItemList then
        Index = 1
    else
        Index = Index + 1
    end
    ItemList[Index].transform.anchoredPosition = RightPos
    ItemList[Index].transform:DOLocalMoveX(MiddlePos.x, 0.3):SetDelay(0.15):SetEase(core.tween.Ease.OutBack):Play()
end

function UINewBookTipsForm:ViewBook()
    local bookinfo = {};
    bookinfo.book_id = newBookList[Index].book_id;
    GameHelper.BookClick(bookinfo);
    self:OnExitClick()
end

function UINewBookTipsForm:OnOpen()
    UIView.OnOpen(self)
end

function UINewBookTipsForm:OnClose()
    UIView.OnClose(self)
end

function UINewBookTipsForm:OnExitClick()
    UIView.__Close(self)
    if self.onClose then
        self.onClose()
    end
end

return UINewBookTipsForm