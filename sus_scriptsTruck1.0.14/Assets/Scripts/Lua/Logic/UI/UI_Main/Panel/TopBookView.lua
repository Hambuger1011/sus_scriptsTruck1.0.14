local BaseClass = core.Class
local TopBookView = BaseClass("TopBookView")

function TopBookView:__init(gameObject)
    self.gameObject=gameObject;
    self.TopBookBg =CS.DisplayUtil.GetChild(gameObject, "TopBookBg"):GetComponent("Image");
    self.TopBookItemObj =CS.DisplayUtil.GetChild(gameObject, "TopBookItemObj");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.TopBookBg.gameObject,function(data) self:OnPlayClick() end)
    --事件监听
    logic.cs.EventDispatcher.AddMessageListener(logic.cs.EventEnum.GotoRead,function() self:OpenRead() end)

    self.infos=nil;
end



--点击打开书本
function TopBookView:OpenRead()
    self:OnPlayClick();
end

--点击打开书本
function TopBookView:OnPlayClick()
    if(self.infos)then
        GameHelper.BookClick(self.infos);
       --埋点*继续阅读模块
        logic.cs.GamePointManager:BuriedPoint(logic.cs.EventEnum.SelectBook,"home_1", "1" ,tostring(self.infos.book_id));
    end

end

function TopBookView:ShowTopBook(info)
    self.infos=info;
    if(info)then
        local BookId = info.book_id;
        GameHelper.CurBookId=BookId;
        self.TopBookBg.sprite = CS.ResourceManager.Instance:GetUISprite("BookDisplayForm/bg_picture");

        logic.cs.ABSystem.ui:DownloadBanner(BookId,function(id,refCount)

            if(self.TopBookBg==nil)then
                refCount:Release();
                return;
            end

            if (BookId ~=id)then
                refCount:Release();
                return;
            end
            self.TopBookBg:DOFade(0, 0):SetEase(core.tween.Ease.Flash):Play();
            self.TopBookBg.sprite = refCount:GetObject();
            self.TopBookBg:DOFade(1, 0.2):SetEase(core.tween.Ease.Flash):Play();
        end)
    else
        logic.debug.LogError("数据里没有final_book_info");
    end

end

function TopBookView:__delete()

    local On_PlayClick = function(data)
        self:OnPlayClick()
    end

    logic.cs.UIEventListener.RemoveOnClickListener(self.TopBookBg.gameObject,On_PlayClick);

    logic.cs.EventDispatcher.RemoveMessageListener(logic.cs.EventEnum.GotoRead,function() self:OpenRead() end);

    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end

    self.gameObject = nil;
    self.button = nil;
    self.TopBookBg = nil;
    self.TopBookItemObj = nil;

end

return TopBookView
