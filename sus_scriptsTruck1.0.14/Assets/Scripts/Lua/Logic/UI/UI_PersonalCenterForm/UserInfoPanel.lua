local BaseClass = core.Class
local UserInfoPanel = BaseClass("UserInfoPanel")
local this = UserInfoPanel

function UserInfoPanel:__init(gameObject)
    self.gameObject=gameObject;

    self.HeadIcon =CS.DisplayUtil.GetChild(gameObject, "HeadIcon"):GetComponent(typeof(logic.cs.Image));
    self.HeadFrame =CS.DisplayUtil.GetChild(gameObject, "HeadFrame"):GetComponent(typeof(logic.cs.Image));
    this.UserNameInputField =CS.DisplayUtil.GetChild(gameObject, "UserNameInputField"):GetComponent(typeof(logic.cs.InputField));
    this.UserName =CS.DisplayUtil.GetChild(gameObject, "UserName"):GetComponent(typeof(logic.cs.Text));
    self.PersonalStatus =CS.DisplayUtil.GetChild(gameObject, "PersonalStatus"):GetComponent(typeof(logic.cs.Text));
    self.BookNumsText =CS.DisplayUtil.GetChild(gameObject, "BookNumsText"):GetComponent(typeof(logic.cs.Text));
    self.FansNumsText =CS.DisplayUtil.GetChild(gameObject, "FansNumsText"):GetComponent(typeof(logic.cs.Text));
    self.ThumbUpToogle =CS.DisplayUtil.GetChild(gameObject, "ThumbUpToogle"):GetComponent(typeof(logic.cs.UIToggle));
    self.ThumbUpText =CS.DisplayUtil.GetChild(self.ThumbUpToogle.gameObject, "ThumbUpText"):GetComponent(typeof(logic.cs.Text));
    self.IGGIDText =CS.DisplayUtil.GetChild(gameObject, "IGGIDText"):GetComponent(typeof(logic.cs.Text));
    self.EditBtn =CS.DisplayUtil.GetChild(gameObject, "EditBtn");
    self.MessageBtn =CS.DisplayUtil.GetChild(gameObject, "MessageBtn");
    self.PackageBtn =CS.DisplayUtil.GetChild(gameObject, "PackageBtn");
    self.SettingBtn =CS.DisplayUtil.GetChild(gameObject, "SettingBtn");
    self.BottleBtn =CS.DisplayUtil.GetChild(gameObject, "BottleBtn");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.MessageBtn,self.MessageBtnClick)
    logic.cs.UIEventListener.AddOnClickListener(self.PackageBtn,self.PackageBtnClick)
    logic.cs.UIEventListener.AddOnClickListener(self.SettingBtn,self.SettingBtnClick)
    logic.cs.UIEventListener.AddOnClickListener(self.BottleBtn,self.BottleBtnClick)
    logic.cs.UIEventListener.AddOnClickListener(self.EditBtn,self.EditBtnClick)
    logic.cs.UIEventListener.AddOnClickListener(self.ThumbUpToogle.gameObject,self.ThumbUpToogleClick)

    this.UserNameInputField.onValueChanged:AddListener(self.OnValueChanged)
    this.UserNameInputField.onEndEdit:AddListener(self.OnEndEditUserName);
end

function UserInfoPanel:UpdateInfo()
    --显示头像
    GameHelper.luaShowDressUpForm(Cache.DressUpCache.avatar,self.HeadIcon,DressUp.Avatar,1001);
    --加载头像框
    GameHelper.luaShowDressUpForm(Cache.DressUpCache.avatar_frame,self.HeadFrame,DressUp.AvatarFrame,2001);
    --作者名字
    this.UserNameInputField.text=logic.cs.UserDataManager.userInfo.data.userinfo.nickname;
    this.UserName.text=logic.cs.UserDataManager.userInfo.data.userinfo.nickname;
    --个性签名
    --self.PersonalStatus.text=Cache.ComuniadaCache.WriterInfo.writer_sign;
    --书本数量
    self.BookNumsText.text=logic.cs.UserDataManager.selfBookInfo.data.read_book_count;
    --IGG ID
    self.IGGIDText.text="IGG ID:"..logic.cs.UserDataManager.IGGid;
    --粉丝数量
    --self.FansNumsText.text=tostring(Cache.ComuniadaCache.WriterInfo.fans_count);
    --【刷新 点赞状态】
    self:UpdateWriterAgree();
    
end

--region【刷新 点赞状态】

function UserInfoPanel:UpdateWriterAgree()

    if(Cache.ComuniadaCache.WriterInfo and Cache.ComuniadaCache.WriterInfo.is_agree)then
        if(Cache.ComuniadaCache.WriterInfo.is_agree==0)then
            self.ThumbUpToogle.isOn=false;
        elseif(Cache.ComuniadaCache.WriterInfo.is_agree==1)then
            self.ThumbUpToogle.isOn=true;
        end

        --self.ThumbUpText.text=Cache.ComuniadaCache.WriterInfo.agree_count;
    end

end

--endregion

--region【按钮点击】【信鸽按钮】
function UserInfoPanel:MessageBtnClick(data)
    logic.UIMgr:Open(logic.uiid.UIEmailForm)
end
--endregion

--region【按钮点击】【背包按钮】
function UserInfoPanel:PackageBtnClick(data)
    logic.UIMgr:Open(logic.uiid.UIPakageForm)
end
--endregion

--region【按钮点击】【设置按钮】
function UserInfoPanel:SettingBtnClick(data)
    
end
--endregion

--region【按钮点击】【漂流瓶按钮】
function UserInfoPanel:BottleBtnClick(data)
    
end
--endregion

--region【按钮点击】【改名按钮】
function UserInfoPanel:EditBtnClick(data)
    logic.cs.EventSystem.current:SetSelectedGameObject(this.UserNameInputField.gameObject);
end
--endregion

--region【按钮点击】【改名按钮】
function UserInfoPanel.OnEndEditUserName(value)
    if logic.cs.UserDataManager.userInfo.data.userinfo.nickname == value then
        return;
    else
        if (#value < 1 or #value > 20) then
            this.UserNameInputField.text = logic.cs.UserDataManager.userInfo.data.userinfo.nickname
            return;
        end
        logic.cs.GameHttpNet:SetUserLanguage(value, 2, function(result)
            local json = core.json.Derialize(result)
            local code = tonumber(json.code)
            if code == 200 then
                logic.cs.UserDataManager.userInfo.data.userinfo.nickname = value
            else
                this.UserNameInputField.text = logic.cs.UserDataManager.userInfo.data.userinfo.nickname
                logic.cs.UIAlertMgr:Show("TIPS",json.msg);
            end
        end);
    end
end
--endregion

--region【按钮点击】【改名按钮】
function UserInfoPanel.OnValueChanged(value)
    this.UserName.text = value
end
--endregion

--region【按钮点击】【点赞作者】

function UserInfoPanel:ThumbUpToogleClick(data)
    --if(Cache.ComuniadaCache.WriterInfo.uid)then
    --    GameController.CommunityControl:SetWriterAgreeRequest(Cache.ComuniadaCache.WriterInfo.uid);
    --end
end

--endregion


--销毁
function UserInfoPanel:__delete()
    --按钮监听
    logic.cs.UIEventListener.RemoveOnClickListener(self.MessageBtn,self.MessageBtnClick)
    logic.cs.UIEventListener.RemoveOnClickListener(self.PackageBtn,self.PackageBtnClick)
    logic.cs.UIEventListener.RemoveOnClickListener(self.SettingBtn,self.SettingBtnClick)
    logic.cs.UIEventListener.RemoveOnClickListener(self.BottleBtn,self.BottleBtnClick)
    logic.cs.UIEventListener.RemoveOnClickListener(self.EditBtn,self.EditBtnClick)
    logic.cs.UIEventListener.RemoveOnClickListener(self.ThumbUpToogle.gameObject,self.ThumbUpToogleClick)
    this.UserNameInputField.onEndEdit:RemoveListener(self.OnEndEditUserName);
    this.UserNameInputField.onValueChanged:RemoveListener(self.OnValueChanged);
end

return UserInfoPanel
