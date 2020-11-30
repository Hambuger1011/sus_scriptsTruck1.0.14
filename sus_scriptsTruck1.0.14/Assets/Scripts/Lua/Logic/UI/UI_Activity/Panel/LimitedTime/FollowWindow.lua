local BaseClass = core.Class
local FollowWindow = BaseClass("FollowWindow")



--region【init】
function FollowWindow:__init(gameObject)

    self.gameObject=gameObject;
    self.Back = CS.DisplayUtil.GetChild(gameObject, "Back");
    self.Facebook = CS.DisplayUtil.GetChild(gameObject, "Facebook");
    self.Twitter = CS.DisplayUtil.GetChild(gameObject, "Twitter");
    self.Instagram = CS.DisplayUtil.GetChild(gameObject, "Instagram");
    self.Google = CS.DisplayUtil.GetChild(gameObject, "Google");
    self.Youtube = CS.DisplayUtil.GetChild(gameObject, "Youtube");
    self.Twitter = CS.DisplayUtil.GetChild(gameObject, "Twitter");
    self.Google.gameObject:SetActiveEx(false);
    self.Facebook.transform.anchoredPosition = core.Vector2.New(0,380)
    self.Bg = CS.DisplayUtil.GetChild(gameObject, "Bg").transform;
    self.Bg.sizeDelta =  {x=610,y=680}


    logic.cs.UIEventListener.AddOnClickListener(self.Back,function(data) self.gameObject:SetActiveEx(false); end)
    logic.cs.UIEventListener.AddOnClickListener(self.Instagram,function(data) self:GoToMedia("https://www.instagram.com/Scripts_Untold_Secrets/") end)
    logic.cs.UIEventListener.AddOnClickListener(self.Twitter,function(data) self:GoToMedia("https://twitter.com/ScriptsUntold") end)
    logic.cs.UIEventListener.AddOnClickListener(self.Facebook,function(data) self:GoToMedia("https://www.facebook.com/Scripts-Untold-Secrets-107729237761206/") end)
    logic.cs.UIEventListener.AddOnClickListener(self.Google,function(data) self:GoToMedia("https://mail.google.com/mail/u/2/#inbox") end)
    logic.cs.UIEventListener.AddOnClickListener(self.Youtube,function(data) self:GoToMedia("https://www.youtube.com/channel/UCZuXtAPaiEPTlSJAMx-otOg?view_as=subscriber") end)

end
--endregion


--region 【更新社媒状态奖励为可领取】

function FollowWindow:GoToMedia(url)
   GameController.ActivityControl:UpdateAttentionMediaRequest(url);
end
--endregion


--region【销毁】
function FollowWindow:__delete()

    if(self.WatchBtn)then
        logic.cs.UIEventListener.RemoveOnClickListener(self.WatchBtn,function(data) self:WatchBtnClick() end);
    end

    self.TimeText = nil;
    self.WatchBtn = nil;
    self.WatchBtnText = nil;
    self.WatchBtnMask = nil;
    self.WatchBtnMaskText = nil;
    self.ads_number = nil;

    self.gameObject=nil;
end
--endregion


return FollowWindow