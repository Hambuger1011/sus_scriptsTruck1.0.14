local BaseClass = core.Class
local NewsPanel = BaseClass("NewsPanel")

function NewsPanel:__init(gameObject,Component)
    self.gameObject=gameObject;
    self.Component = Component;
end

function NewsPanel:ShowWebView()
    local gameId = logic.cs.IGGSDKMrg:GetGameId()
    local SSOToken = logic.cs.IGGSDKMrg:GetSSOToken()

    self.viewGameObject = logic.cs.GameObject.Instantiate(self.gameObject, self.Component)
    self.viewGameObject:SetActiveEx(true)
    if self.UniWebView then
        self:DestroyWebView()
    end
    self.UniWebView=self.viewGameObject:GetComponent("UniWebView");

    self.UniWebView:ShowWebView(CS.System.String.Format(logic.cs.UserDataManager.appconfinfo.pagelinkInfo._event,gameId,SSOToken));
end

function NewsPanel:HideWebView()
    if self.UniWebView then
        self.UniWebView:HideWebView();
    end
end

function NewsPanel:DestroyWebView()
    if self.UniWebView then
        self.UniWebView:DestroyWebView();
    end
    if self.viewGameObject then
        logic.cs.GameObject.Destroy(self.viewGameObject)
    end
    self.UniWebView=nil;
    self.viewGameObject = nil;
end

return NewsPanel
