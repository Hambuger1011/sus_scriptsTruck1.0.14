local BaseClass = core.Class
local StoryItem2 = BaseClass("StoryItem2")

function StoryItem2:__init(gameObject)
    self.gameObject=gameObject;

    self.rectTransform=gameObject:GetComponent(typeof(logic.cs.RectTransform));
    self.DefaultImg =CS.DisplayUtil.GetChild(gameObject, "DefaultImg"):GetComponent("Image");
    self.BookIconImage =CS.DisplayUtil.GetChild(gameObject, "BookIconImage"):GetComponent("Image");
    self.BookName =CS.DisplayUtil.GetChild(gameObject, "BookName"):GetComponent("Text");
    self.Chapter =CS.DisplayUtil.GetChild(gameObject, "Chapter"):GetComponent("Text");
    self.Content =CS.DisplayUtil.GetChild(gameObject, "Content"):GetComponent("Text");
    self.LookNumberText =CS.DisplayUtil.GetChild(gameObject, "LookNumberText"):GetComponent("Text");
    self.AutorName =CS.DisplayUtil.GetChild(gameObject, "AutorName"):GetComponent("Text");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.gameObject,function(data) self:BookOnclicke() end)
    self.m_downloadSeq = 1;
    self.BookInfo=nil;
    self.mItemIndex = 0;
end

function StoryItem2:SetItemData(itemData,itemIndex)
    if(itemData==nil)then return; end
    self.mItemIndex = itemIndex;

    self.m_downloadSeq = self.m_downloadSeq + 1
    self.BookInfo = itemData;

    if(self.BookName)then
        self.BookName.text = itemData.title;
    end

    --if(self.gameObject)then
    --    self.gameObject.name=tostring(itemIndex);
    --end

    if (itemData.read_count > 1000)then
        local num=itemData.read_count/1000;
        self.LookNumberText.text = string.format("%.1f", num).."k";
    else
        self.LookNumberText.text = tostring(itemData.read_count);
    end

    if(self.Content)then
        self.Content.text = itemData.description;
    end

    if(self.Chapter)then
        self.Chapter.text = "Chapter:"..itemData.total_chapter_count;
    end

    if(self.AutorName)then
        self.AutorName.text = itemData.writer_name;
    end


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

--点击书本
function StoryItem2:BookOnclicke()
    --【点击书本】
    logic.StoryEditorMgr:ReadingOtherBook(self.BookInfo.id)
end


function StoryItem2:SetBookCover(sprite)
    if logic.IsNull(self.gameObject) then
        return;
    end
    if logic.IsNull(sprite) then
        sprite = self.DefaultImg.sprite;
    end
    logic.StoryEditorMgr:SetCover(self.BookIconImage,sprite,self.BookIconImage.transform.rect.size);
end

--销毁
function StoryItem2:__delete()
    if(self.gameObject)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.gameObject,function(data) self:BookOnclicke() end)
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


return StoryItem2
