local BaseClass = core.Class
local StoryItem = BaseClass("StoryItem")

function StoryItem:__init(gameObject)
    self.gameObject=gameObject;

    self.rectTransform=gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.DefaultImg =CS.DisplayUtil.GetChild(gameObject, "DefaultImg"):GetComponent("Image");
    self.BookIconImage =CS.DisplayUtil.GetChild(gameObject, "BookIconImage"):GetComponent("Image");
    self.BookName =CS.DisplayUtil.GetChild(gameObject, "BookName"):GetComponent("Text");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.BookIconImage.gameObject,function(data) self:BookOnclicke() end)
    self.m_downloadSeq = 1;
    self.BookInfo=nil;
    self.mItemIndex = 0;
    self._type=EStoryList.None;
end

function StoryItem:SetItemData(itemData,itemIndex)
    self.mItemIndex = itemIndex;

    self.m_downloadSeq = self.m_downloadSeq + 1
    self.BookInfo = itemData;

    if(self.BookName)then
       self.BookName.text = itemData.title;
    end

    --if(self.gameObject)then
    --    self.gameObject.name=tostring(itemIndex);
    --end

    self:SetBookCover(nil)
    local cover_image = itemData.cover_image
    if not string.IsNullOrEmpty(cover_image) then
        local url,md5 = logic.StoryEditorMgr:ParseImageUrl(cover_image)
        local filename = logic.config.WritablePath .. '/cache/story_image/'..md5

        local downloadSeq = self.m_downloadSeq
        logic.StoryEditorMgr.data:LoadSprite(filename, md5, url,function(sprite)
            if downloadSeq ~= self.m_downloadSeq then
                logic.debug.LogError('seq mismatch:'..downloadSeq..'<=>'..self.m_downloadSeq)
                return
            end
            self:SetBookCover(sprite)
        end)
    end

end


function StoryItem:SetType(_type)
    self._type=_type;
end




--点击书本
function StoryItem:BookOnclicke()
    --【点击书本】
    if(self._type==EStoryList.MyWriterList)then
        logic.StoryEditorMgr:EnterBookDetials(self.BookInfo.id, function(storyDetial)
            local uiView = logic.UIMgr:Open(logic.uiid.Story_Detials); if(uiView)then uiView:SetData(storyDetial); end
        end)
    elseif(self._type==EStoryList.HistoryList)then
        logic.StoryEditorMgr:ReadingOtherChapter(self.BookInfo.book_id, self.BookInfo.chapter_number,function()
            logic.StoryEditorMgr:BackToMainClick()
        end)
    else
        logic.StoryEditorMgr:ReadingOtherBook(self.BookInfo.id)
    end
end


function StoryItem:SetBookCover(sprite)
    if logic.IsNull(self.gameObject) then
        return;
    end
    if logic.IsNull(sprite) then
        sprite = self.DefaultImg.sprite;
    end
    logic.StoryEditorMgr:SetCover(self.BookIconImage,sprite,self.BookIconImage.transform.rect.size);
end

--销毁
function StoryItem:__delete()
    if(self.BookIconImage)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.BookIconImage.gameObject,function(data) self:BookOnclicke() end)
    end
    self.rectTransform=nil;
    self.BookIconImage =nil;
    self.BookName=nil;
    self.m_downloadSeqnil=nil;
    self.BookInfo=nil;
    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return StoryItem
