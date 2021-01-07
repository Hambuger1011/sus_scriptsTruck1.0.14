---@class CommentItem
local UIBubbleBox = require('Logic/StoryEditor/UI/Editor/BubbleItem/UIBubbleBox')
local CommentItem = core.Class('CommentItem', UIBubbleBox)
local UI_Reply = require('Logic/UI/UI_BookReading/UI_Reply')

---@class CommentItem_Item
local Item = core.Class("CommentItem.Item")

function Item:__init(uiItem)
    self.transform = uiItem
    self.gameObject = uiItem.gameObject

    self.SetActive = function(self, isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end

--region-------CommentItem

---@param uiItem UIBubbleItem
function CommentItem:__init(uiItem)
    UIBubbleBox.__init(self, uiItem)
    local transform = uiItem.transform
    self.uiBinding = transform:GetComponent(typeof(logic.cs.UIBinding))
    self.itemButton = transform:GetComponent(typeof(logic.cs.UITweenButton))
    self.activeBox = Item.New(uiItem.transform)
    self.line = self.uiBinding:Get('Line').transform
    self.headImage = self.uiBinding:Get('HeadImage', typeof(logic.cs.Image))
    self.HeadFrame = self.uiBinding:Get('HeadFrame', typeof(logic.cs.Image))
    self.nameText = self.uiBinding:Get('Name', typeof(logic.cs.Text))
    self.contentText = self.uiBinding:Get('ContentText', typeof(logic.cs.Text))
    self.dateText = self.uiBinding:Get('Date', typeof(logic.cs.Text))
    self.likeText = self.uiBinding:Get('LikeText', typeof(logic.cs.Text))
    self.unLikeText = self.uiBinding:Get('UnLikeText', typeof(logic.cs.Text))
    self.replyText = self.uiBinding:Get('ReplyText', typeof(logic.cs.Text))
    self.likeBtn = self.uiBinding:Get('LikeBtn', typeof(logic.cs.UITweenButton))
    self.unLikeBtn = self.uiBinding:Get('UnLikeBtn', typeof(logic.cs.UITweenButton))
    self.replyBtn = self.uiBinding:Get('ReplyBtn', typeof(logic.cs.UITweenButton))
    self.LikeIcon =CS.DisplayUtil.GetChild(self.likeBtn.gameObject, "LikeIcon"):GetComponent("Image");
    self.UnLikeIcon =CS.DisplayUtil.GetChild(self.unLikeBtn.gameObject, "UnLikeIcon"):GetComponent("Image");



    self.SetActive = function(self, isOn)
        self.gameObject:SetActiveEx(isOn)
    end
end

function CommentItem:FormatNum(_num)
    _num = tonumber(_num)
    if _num >= 1000000 then
        _num = string.format("%0.1f", tonumber(_num) / 1000000) .. "m"
    elseif _num >= 1000 then
        _num = string.format("%0.1f", tonumber(_num) / 1000) .. "k"
    end
    return _num
end

function CommentItem:GetSize()
    return self.boxSize
end

function CommentItem:SetSize()
    --self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Horizontal, self.boxSize.x)
    self.activeBox.transform:SetSizeWithCurrentAnchors(logic.cs.RectTransform.Axis.Vertical, self.boxSize.y)
end

function CommentItem:SetData(Data)
    if Data.pageNum and Data.pageNum ~= 0 then
        Data.pageNum = 0
    elseif Data.pageNum and Data.pageNum == 0 and Data.GetNextPage then
        Data.GetNextPage()
        Data.GetNextPage = nil
    end
    --加载头像
    GameHelper.luaShowDressUpForm(Data.avatar, self.headImage, DressUp.Avatar, 1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(Data.avatar_frame, self.HeadFrame, DressUp.AvatarFrame, 2001);



    --self.headImage.sprite = CS.ResourceManager.Instance:GetUISprite("ProfileForm/img_renwu"..Data.face_icon);
    self.nameText.text = Data.nickname
    local contentTxt = Data.content
    if Data.is_self == 0 then
        contentTxt = logic.cs.PluginTools:ReplaceBannedWords(contentTxt)
    end
    self.contentText.text = contentTxt
    self.contentText.transform:GetComponent("ContentSizeFitter"):SetLayoutVertical()
    self.dateText.text = os.date("%m-%d %H:%M", Data.create_time)
    self.likeText.text = self:FormatNum(Data.agree_count)
    self.unLikeText.text = self:FormatNum(Data.disagree_count)
    self.replyText.text = Data.reply_count


    if(Data.agree)then
        self:SetAgreeState(Data.agree,Data.agree_count,Data.disagree_count);
    end

    self.itemButton.onClick:RemoveAllListeners()
    self.itemButton.onClick:AddListener(function()
        self:OnItemClick(Data)
    end)

    self.likeBtn.onClick:RemoveAllListeners()
    self.likeBtn.onClick:AddListener(function()
        logic.gameHttp:BookCommentSetAgree(tonumber(Data.id), function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.ThumbUpRecord, "", "", tostring(logic.bookReadingMgr.selectBookId))
                Data.agree_count = json.data.agree_count
                Data.disagree_count = json.data.disagree_count


                if(json.data.disagree==1)then
                    Data.agree=2;
                elseif(json.data.agree==1)then
                    Data.agree=json.data.agree;
                else
                    Data.agree=0;
                end
                if(Data.agree)then
                    self:SetAgreeState(Data.agree,Data.agree_count);
                end
            else
                logic.cs.UIAlertMgr:Show("TIPS", json.msg)
            end
        end)
    end)

    self.unLikeBtn.onClick:RemoveAllListeners()
    self.unLikeBtn.onClick:AddListener(function()
        logic.gameHttp:BookCommentSetDisagree(tonumber(Data.id), function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.CancelThumbUp, "", "", tostring(logic.bookReadingMgr.selectBookId))
                Data.agree_count = json.data.agree_count
                Data.disagree_count = json.data.disagree_count

                if(json.data.disagree==1)then
                    Data.agree=2;
                elseif(json.data.agree==1)then
                    Data.agree=json.data.agree;
                else
                    Data.agree=0;
                end
                if(Data.agree)then
                    self:SetAgreeState(Data.agree,Data.agree_count,Data.disagree_count);
                end

            else
                logic.cs.UIAlertMgr:Show("TIPS", json.msg)
            end
        end)
    end)

    self.replyBtn.onClick:RemoveAllListeners()
    self.replyBtn.onClick:AddListener(function()
        self:OnItemClick(Data)
    end)

    local rootSize = self.contentText.transform.rect.size
    local pos = self.contentText.transform.anchoredPosition
    local linePos = self.line.anchoredPosition
    local height = Mathf.Abs(rootSize.y) + Mathf.Abs(pos.y) + Mathf.Abs(linePos.y)
    self.boxSize = core.Vector2.New(750, height)
end


function CommentItem:SetAgreeState(agree,agree_count,disagree_count)
    if(self.LikeIcon==nil or self.UnLikeIcon==nil)then return; end
    self.LikeIcon.sprite = CS.ResourceManager.Instance:GetUISprite("Comment/btn__iconlike");
    self.UnLikeIcon.sprite = CS.ResourceManager.Instance:GetUISprite("Comment/btn_icon_dislike");
    if(agree)then
        if(agree==0)then --0中立
        elseif(agree==1)then  --1赞同
            self.LikeIcon.sprite = CS.ResourceManager.Instance:GetUISprite("Comment/btn__iconlike_1");
        elseif(agree==2)then  --2不赞同
            self.UnLikeIcon.sprite = CS.ResourceManager.Instance:GetUISprite("Comment/btn_icon_dislike_1");
        end
    end

    if(self.likeText and agree_count)then
        self.likeText.text = self:FormatNum(agree_count)

    end

    if(self.unLikeText and disagree_count)then
        self.unLikeText.text = self:FormatNum(disagree_count)
    end
end



--endregion
function CommentItem:OnItemClick(Data)
    UI_Reply:SetCommentData(Data,function(_Data) self:SetAgreeState(_Data.agree,_Data.agree_count,_Data.disagree_count)end)
    logic.UIMgr:Open(logic.uiid.Reply)
end

return CommentItem