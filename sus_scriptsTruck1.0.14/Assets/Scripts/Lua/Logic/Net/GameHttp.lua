local socket = require "socket"
--socket.gettime() --ms
---@class GameHttp
local GameHttp = core.Class("GameHttp")
local _sendSeq = 0

function GameHttp:GetIP()
    local client = socket.connect("www.baidu.com", 80);
    local ip, port = client:getsockname();
    logic.debug.LogError(ip)
end

function GameHttp:GetLocalIP()
    local ip = socket.dns.toip(socket.dns.gethostname())
    logic.debug.LogError(ip)
end

function GameHttp:SetUrlHead()


    -- if logic.config.channel == Channel.Spain then
    --     if logic.config.isDebugMode then
    --         local svrType = logic.cs.GameDataMgr.ServiceType
    --         if svrType == 1 then    --开发
    --             logic.cs.GameHttpNet.GameUrlHead = "http://192.168.0.199"
    --         elseif svrType == 2 then    --spain测试
    --             logic.cs.GameHttpNet.GameUrlHead = "http://test.onyxgamespain.com"
    --         else    --spain正式
    --             logic.cs.GameHttpNet.GameUrlHead = "http://www.onyxgamespain.com"
    --         end
    --     else
    --         if logic.cs.GameHttpNet.AuditStatus == 1 then
    --             logic.cs.GameHttpNet.GameUrlHead = "http://www.onyxgamespain.com"
    --         else
    --             logic.cs.GameHttpNet.GameUrlHead = "http://test.onyxgamespain.com"
    --         end
    --     end
    -- else
    --     if logic.config.isDebugMode then
    --         local svrType = logic.cs.GameDataMgr.ServiceType
    --         if svrType == 1 then    --开发
    --             logic.cs.GameHttpNet.GameUrlHead = "http://192.168.0.199"
    --         elseif svrType == 2 then    --tencent
    --             logic.cs.GameHttpNet.GameUrlHead = "http://193.112.66.252"
    --         elseif svrType == 3 then    --spain测试
    --             logic.cs.GameHttpNet.GameUrlHead = "http://www.onyxgames1.com:30996"
    --         else    --正式
    --             logic.cs.GameHttpNet.GameUrlHead = "http://www.onyxgames1.com"
    --         end
    --     else
    --         if logic.cs.GameHttpNet.AuditStatus == 1 then
    --             logic.cs.GameHttpNet.GameUrlHead = "http://www.onyxgames1.com"
    --         else
    --             logic.cs.GameHttpNet.GameUrlHead = "http://www.onyxgames1.com:30996"
    --         end
    --     end
    -- end
    -- logic.debug.LogError(logic.cs.GameHttpNet.GameUrlHead)

    self.csHttp = CS.UniHttp.Instance

end


local moveWaitCallBack
local moveWait
local ShowMoveWaitPanel

---@param self GameHttp @GameHttp
function GameHttp:Post(self, apiName, param, callback, timeoutMS, tryCount, isRequired, isFullUrl, isShowLoadUI)
    timeoutMS = timeoutMS or 20
    tryCount = tryCount or 3
    isRequired = isRequired or false
    isFullUrl = isFullUrl or true
    if isShowLoadUI == null then
        isShowLoadUI = true
    end

    local url = logic.cs.GameHttpNet.GameUrlHead .. "/" .. apiName

    if param then
        for k, v in pairs(param) do
            if param[k] then
                param[k] = tostring(v)
            else
                param[k] = nil
            end
        end
    end

    local sendSeq = logic.cs.GameHttpNet:getSendSeq()
    local sendInfo = ''
    if param then
        sendInfo = string.format("<color=#009000>[lua][send]POST:[%d]%s phoneimei:%s token:%s %s</color>",
                sendSeq,
                url,
                logic.cs.GameHttpNet.UUID,
                logic.cs.GameHttpNet.TOKEN,
                core.json.Serialize(param)
        )
    else
        sendInfo = string.format("<color=#009000>[lua][send]POST:[%d]%s %s</color>", sendSeq, url, 'no param')
    end
    logic.debug.Log(sendInfo)
    --logic.cs.CFileManager.WriteFileString('d://'..sendSeq..'.txt',sendInfo)
    local timestamp = socket.gettime()

    self.csHttp:Post(url, param, function(obj, responseCode, result)
        logic.cs.UINetLoadingMgr:Close()

        timestamp = socket.gettime() - timestamp
        logic.debug.Log(string.format("<color=#ee00ee>[lua][recv]POST:耗时%.3fs[%d]%s,result:\n%s</color>", timestamp, sendSeq, url, result))

        if responseCode ~= 200 then
            --网络不通
            if isRequired then
                if responseCode ~= 0 then
                    param["result"] = result
                    logic.cs.TalkingDataManager:RecordProtocolError(apiName, param)
                end
                logic.cs.UIAlertMgr:ShowNetworkAlert(responseCode, function()
                    logic.debug.Log(sendInfo)
                    self.csHttp:DoWebPost(obj)
                end)
            end
            return
        end

        if (result == nil or result == "") then
            logic.cs.UIAlertMgr:Show("tips", "请求返回数据错误 解析Result失败！[Post]", logic.cs.AlertType.Sure,
                    function(isOK)
                        --自己调自己
                        self:Post(self, apiName, param, callback, timeoutMS, tryCount, isRequired, isFullUrl);
                    end)
            CS.XLuaHelper.DebugLog("请求返回数据result解析错误[Post] result：" .. result .. "apiName:" .. apiName);
            return ;
        end

        local JsonObject = core.json.Derialize(result)
        --print("responseCode:"..JsonObject.code)
        if JsonObject.code == 280 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        local LoginForm = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.LoginForm)
                        local go = LoginForm.transform:GetComponent(typeof(CS.LoginForm))
                        go:IsTimeOutOpenFanc()
                    end)
        elseif JsonObject.code == 281 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        logic.cs.IGGSDKMrg:AutoLogin();
                    end)
        elseif JsonObject.code == 282 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        local LoginForm = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.LoginForm)
                        local go = LoginForm.transform:GetComponent(typeof(CS.LoginForm))
                        go:IsTimeOutOpenFanc()
                    end)
        elseif JsonObject.code == 910 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        local LoginForm = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.LoginForm)
                        local go = LoginForm.transform:GetComponent(typeof(CS.LoginForm))
                        go:IsTimeOutOpenFanc()
                    end)
        elseif JsonObject.code == 277 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        logic.cs.PluginTools:KillRunningProcess();
                    end)
        elseif JsonObject.code == 377 then
            logic.cs.GameHttpNet:ShowMovePanel()
        elseif JsonObject.code == 378 then
            ShowMoveWaitPanel(self, apiName, param, callback, timeoutMS, tryCount, isRequired, isFullUrl);
        else
            callback(result)
        end
    end, timeoutMS, tryCount, isShowLoadUI)
end

moveWait = function(result)
    logic.UIMgr:Close(logic.uiid.MoveWait)
    moveWaitCallBack(result)
end

ShowMoveWaitPanel = function(_self, apiName, param, callback, timeoutMS, tryCount, isRequired, isFullUrl)
    param["is_move"] = "1"
    moveWaitCallBack = callback
    logic.UIMgr:Open(logic.uiid.MoveWait)
    GameHttp:Post(_self, apiName, param, moveWait, 120, tryCount, isRequired, isFullUrl, false)
end

---@param self GameHttp @GameHttp
function GameHttp:Get(self, apiName, param, callback, timeoutMS, tryCount, isRequired, isFullUrl)
    timeoutMS = timeoutMS or 20
    tryCount = tryCount or 3
    isRequired = isRequired or false
    isFullUrl = isFullUrl or true

    local urlParam = ''
    if param then
        local isFirst = true
        for k, v in pairs(param) do
            if not v then
                goto continue
            end
            if isFirst then
                isFirst = false
                urlParam = "?" .. k .. "=" .. (v and v or string.Empty)
            else
                urlParam = urlParam .. "&" .. k .. "=" .. (v and v or string.Empty)
            end
            :: continue ::
        end
    end

    local url = logic.cs.GameHttpNet.GameUrlHead .. "/" .. apiName .. urlParam
    --logic.debug.Log("<color=cyan>---GET:"..url.."</color>");

    local sendSeq = logic.cs.GameHttpNet:getSendSeq()
    local sendInfo = ''
    if param then
        sendInfo = string.format("<color=#009000>[lua][send]GET:[%d]%s/%s   phoneimei:%s  token:%s  %s</color>",
                sendSeq,
                logic.cs.GameHttpNet.GameUrlHead,
                apiName,
                logic.cs.GameHttpNet.UUID,
                logic.cs.GameHttpNet.TOKEN,
                core.json.Serialize(param))
    else
        sendInfo = string.format("<color=#009000>[lua][send]GET:[%d]%s/%s  %s</color>", sendSeq, logic.cs.GameHttpNet.GameUrlHead, apiName, 'no param')
    end
    logic.debug.Log(sendInfo)
    local timestamp = socket.gettime()
    self.csHttp:Get(url, function(obj, responseCode, result)
        logic.cs.UINetLoadingMgr:Close()

        timestamp = socket.gettime() - timestamp
        logic.debug.Log(string.format("<color=#ee00ee>[lua][recv]GET:耗时%.3fs[%d]%s,result:  %s</color>", timestamp, sendSeq, url, result))

        if responseCode ~= 200 then
            --网络不通
            if isRequired then
                if responseCode ~= 0 then
                    param["result"] = result
                    logic.cs.TalkingDataManager:RecordProtocolError(apiName, param)
                end
                logic.cs.UIAlertMgr:ShowNetworkAlert(responseCode, function()
                    logic.debug.Log(sendInfo)
                    self.csHttp:DoWebGet(obj)
                end)
            end
            return
        end

        if (result == nil or result == "") then
            logic.cs.UIAlertMgr:Show("tips", "请求返回数据错误 解析Result失败！[Get]", logic.cs.AlertType.Sure,
                    function(isOK)
                        --自己调自己
                        self:Get(self, apiName, param, callback, timeoutMS, tryCount, isRequired, isFullUrl);
                    end)
            CS.XLuaHelper.DebugLog("请求返回数据result解析错误[Get] result：" .. result .. "apiName:" .. apiName);
            return ;
        end

        local JsonObject = core.json.Derialize(result)
        --print("responseCode:"..JsonObject.code)

        if JsonObject.code == 280 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        local LoginForm = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.LoginForm)
                        local go = LoginForm.transform:GetComponent(typeof(CS.LoginForm))
                        go:IsTimeOutOpenFanc()
                    end)
        elseif JsonObject.code == 281 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        logic.cs.IGGSDKMrg:AutoLogin();
                    end)
        elseif JsonObject.code == 282 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        local LoginForm = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.LoginForm)
                        local go = LoginForm.transform:GetComponent(typeof(CS.LoginForm))
                        go:IsTimeOutOpenFanc()
                    end)
        elseif JsonObject.code == 910 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        local LoginForm = logic.cs.CUIManager:OpenForm(logic.cs.UIFormName.LoginForm)
                        local go = LoginForm.transform:GetComponent(typeof(CS.LoginForm))
                        go:IsTimeOutOpenFanc()
                    end)
        elseif JsonObject.code == 277 then
            logic.cs.UIAlertMgr:Show("TIPS", JsonObject.msg, logic.cs.AlertType.Sure,
                    function(isOK)
                        logic.cs.PluginTools:KillRunningProcess();
                    end)
        elseif JsonObject.code == 377 then
            logic.cs.GameHttpNet:ShowMovePanel()
        else
            callback(result)
        end
    end, timeoutMS, tryCount)
end

function GameHttp:Login(iggid, access_token, is_switch, callback)
    local param = {
        system_type = logic.cs.GameHttpNet.SYSTEMTYPE,
        iggid = iggid,
        access_token = access_token,
        is_switch = is_switch,
    }
    if(logic.cs.UserDataManager.InviteCode and #logic.cs.UserDataManager.InviteCode > 0)then
        param = {
            system_type = logic.cs.GameHttpNet.SYSTEMTYPE,
            iggid = iggid,
            access_token = access_token,
            is_switch = is_switch,
            invite_code = logic.cs.UserDataManager.InviteCode,
        }
        logic.cs.UserDataManager.InviteCode = "";
    end
    self:Post(self, "api_login", param, callback, 10, nil, true)
end

function GameHttp:GetUserInfo(callback)
    local param = {
    }
    self:Get(self, "api_getUserInfo", param, callback, nil, nil, true)
end

function GameHttp:GetSelfBookInfo(callback)
    local param = {
    }
    self:Get(self, "api_getIndexData", param, callback, nil, nil, true)
end

function GameHttp:GetActivityRewardContent(callback)
    local param = {
    }
    self:Get(self, "api_getActivityRewardContent", param, callback, nil, nil, true)
end

function GameHttp:GetActivityReceiveStatus(callback)
    local param = {
    }
    self:Get(self, "api_getActivityReceiveStatus", param, callback, nil, nil, true)
end

function GameHttp:GetInviteList(callback)
    local param = {
    }
    self:Get(self, "api_getInviteList", param, callback, nil, nil, true)
end

--获取任务列表
function GameHttp:GetMyTaskList(callback)
    local param = {
    }
    self:Get(self, "api_getMyTaskList", param, callback, nil, nil, true)
end

function GameHttp:GetShopList(callback)
    self:Get(self, "api_getmall", nil, callback, nil, nil, true)
end

--领取任务奖励
function GameHttp:ReceiveTaskPrize(task_id, callback)
    local param = {
        task_id = task_id,
    }
    self:Post(self, "api_receiveTaskPrize", param, callback, nil, nil, true)
end

function GameHttp:GetShopList(callback)
    self:Get(self, "api_getProductList", nil, callback, nil, nil, true)
end

--function GameHttp:TouristLogin(callback)
--    local param = {
--
--        phoneimei = logic.cs.GameHttpNet.UUID,
--    }
--    Post(self, "api_touristLogin", param, callback)
--end

function GameHttp:GetBookShelfInfo(callback)
    local param = {
    }
    self:Get(self, "api_getAllBook", param, callback, nil, nil, true)
end

--临时进度保存
function GameHttp:markStep(
        bookID,
        chapterID,
        dialogID,
        callback)

    local param = {
        bookid = bookID,
        chapterid = chapterID,
        dialogid = dialogID,
    }

    self:Post(self, "api_markStep", param, callback, nil, nil, true)
end

--领取每日登录奖励
function GameHttp:ReceiveDailyLoginAward(callback)
    local param = {
        phoneimei = logic.cs.GameHttpNet.UUID,
    }
    self:Post(self, "api_receiveDailyLoginAward", param, callback, nil, nil, true)
end

--领取活动广告奖励
function GameHttp:ReceiveDailyAdAward(callback)
    local param = {
        phoneimei = logic.cs.GameHttpNet.UUID,
    }
    self:Post(self, "api_receiveDailyAdAward", param, callback, nil, nil, true)
end

--领取登录宝箱奖励
function GameHttp:ReceiveActivityBoxAward(boxId, callback)
    local param = {
        id = boxId,
        phoneimei = logic.cs.GameHttpNet.UUID,
    }
    self:Post(self, "api_receiveActivityBoxAward", param, callback, nil, nil, true)
end

--领取彩蛋活动奖励
function GameHttp:ReceiveEggActivityAward(egg_id, book_id, dialog_id, callback)
    local param = {
        egg_id = egg_id,
        book_id = book_id,
        dialog_id = dialog_id,
        phoneimei = logic.cs.GameHttpNet.UUID,
    }
    self:Post(self, "api_receiveEggActivityAward", param, callback, nil, nil, true)
end

--获取邀请奖励
function GameHttp:ReceiveInvitePrize(number, callback)
    local param = {
        number = number,
        phoneimei = logic.cs.GameHttpNet.UUID,
    }
    self:Post(self, "api_receiveInvitePrize", param, callback, nil, nil, true)
end

--获取邀请奖励
function GameHttp:ReceiveFirstRechargeAward(callback)
    local param = {
        phoneimei = logic.cs.GameHttpNet.UUID,
    }
    self:Post(self, "api_receiveFirstRechargeAward", param, callback, nil, nil, true)
end

--领取第三方登录绑定的奖励
function GameHttp:ReceiveThirdPartyAward(callback)
    local param = {
    }
    self:Post(self, "api_receiveThirdPartyAward", param, callback, nil, nil, true)
end

--领取用户迁移的奖励
function GameHttp:ReceiveUserMoveAward(callback)
    local param = {
    }
    self:Post(self, "api_receiveUserMoveAward", param, callback, nil, nil, true)
end

--领取关注社媒奖励
function GameHttp:ReceiveAttentionMediaReward(callback)
    local param = {
    }
    self:Post(self, "api_receiveAttentionMediaReward", param, callback, nil, nil, true)
end

--更新社媒状态奖励为可领取
function GameHttp:UpdateAttentionMedia(callback)
    local param = {
        phoneimei = logic.cs.GameHttpNet.UUID,
    }
    self:Post(self, "api_updateAttentionMedia", param, callback, nil, nil, true)
end

--书本评论
function GameHttp:GetBookCommentList(book_id, page, sort_type, callback)
    local param = {
        book_id = book_id,
        page = page,
        sort_type = sort_type,
    }
    self:Get(self, "api_getBookCommentList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--获取通用奖励配置
function GameHttp:GetRewardConfig(callback)
    local param = {
    }
    self:Get(self, "api_getRewardConfig", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--获取用户的邮箱信息
function GameHttp:GetSystemMsg(page, callback)
    local param = {
        page = page,
    }
    self:Post(self, "api_getsystemmsg", param, callback, nil, nil, true)
end

--读取用户的邮件
function GameHttp:ReadSystemMsg(msgid, callback)
    local param = {
        msgid = msgid,
    }
    self:Post(self, "api_readsystemmsg", param, callback, nil, nil, true)
end

--获得邮件的奖励
function GameHttp:AchieveMsgPrice(msgid, callback)
    local param = {
        msgid = msgid,
    }
    self:Post(self, "api_achievemsgprice", param, callback, nil, nil, true)
end

--回复列表
function GameHttp:GetBookCommentReplyList(comment_id, page, callback)
    local param = {
        comment_id = comment_id,
        page = page,
    }
    self:Get(self, "api_getBookCommentReplyList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--创建书本评论
function GameHttp:CreateBookComment(book_id, content, callback)
    local param = {
        book_id = book_id,
        content = content,
    }
    self:Post(self, "api_createBookComment", param, callback, nil, nil, true)
end

--创建评论回复
function GameHttp:CreateBookCommentReply(comment_id, content, reply_id, callback)
    local param = {
        comment_id = comment_id,
        content = content,
        reply_id = reply_id,
    }
    self:Post(self, "api_createBookCommentReply", param, callback, nil, nil, true)
end

--点赞书本评论
function GameHttp:BookCommentSetAgree(comment_id, callback)
    local param = {
        comment_id = comment_id,
    }
    self:Post(self, "api_bookCommentSetAgree", param, callback, nil, nil, true)
end

--不赞同书本评论
function GameHttp:BookCommentSetDisagree(comment_id, callback)
    local param = {
        comment_id = comment_id,
    }
    self:Post(self, "api_bookCommentSetDisagree", param, callback, nil, nil, true)
end

function GameHttp:SendPlayerProgress(
        bookID,
        chapterID,
        dialogID,
        option,
        roleName,
        npcID,
        npcName,
        npcSex,
        character_id,
        outfit_id,
        hair_id,
        optionList,
        is_use_prop,--是否使用钥匙优惠价道具: 1.使用道具 0不使用（非必传，默认不使用）
        discount,--折扣率
        callback
)
    local param = {
        bookid = bookID,
        chapterid = chapterID,
        dialogid = dialogID,
        option = option,
        role_name = roleName,
        npc_id = npcID,
        npc_name = npcName,
        npc_sex = npcSex,
        character_id = character_id,
        outfit_id = outfit_id,
        hair_id = hair_id,
        option_list = optionList,
        is_use_prop = is_use_prop, --是否使用钥匙优惠价道具: 1.使用道具 0不使用（非必传，默认不使用）
        discount = discount, --折扣率
    }
    self:Post(self, "api_saveStep", param, callback)
end

function GameHttp:GetBookDetailInfo(bookid, callback)
    local param = {
        bookid = bookid,
    }
    self:Get(self, "api_getbookdetail", param, callback)
end

function GameHttp:GetOrderToSubmitForIos(vOrderId, vOrderToken, vProductid, vTransactionId, vIsSandbox, vCallBackHandler)
    local tokenResult = CS.UnityEngine.WWW.EscapeURL(vOrderToken, CS.System.Text.Encoding.UTF8);
    local param = {
        recharge_no = vOrderId,
        productid = vProductid,
        packagename = CS.SdkMgr.packageName,
        payment_type = 2,

        datasignature = tokenResult,
        ios_orderid = vTransactionId,
        is_sandbox = logic.config.isDebugMode and 1 or 0,
        secretword = logic.cs.GameHttpNet.PayFinishKey,
    }
    self:Post(self, "api_finishOrder", param, function(result)
        vCallBackHandler(vTransactionId, result)
    end, nil, nil, true)
end

function GameHttp:GetOrderToSubmitForAndroid(recharge_no, vOrderId, vOrderToken, vProductid, vPackagename, vDatasignature, vPurchasetime, vPurchaseState, vTestToken, vCallBackHandler)
    local tokenResult = CS.UnityEngine.WWW.EscapeURL(vOrderToken, CS.System.Text.Encoding.UTF8);
    local param = {
        recharge_no = recharge_no,
        productid = vProductid,
        packagename = CS.SdkMgr.packageName,
        payment_type = 1,

        datasignature = vDatasignature,
        purchasetime = vPurchasetime,
        purchaseState = vPurchaseState,
        google_orderid = vOrderId,
        order_token = vOrderToken,
        secretword = logic.cs.GameHttpNet.PayFinishKey,
        test_token = vTestToken,
    }
    self:Post(self, "api_finishOrder", param, function(result)
        vCallBackHandler(vOrderId, result)
    end, nil, nil, true)
end


--region StoryEditor

function GameHttp:StoryEditor_GetMyChapterList(bookID, callback)
    local param = {
        book_id = bookID
    }
    self:Get(self, "api_getMyWriterChapterList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_GetChapterList(bookID, callback)
    local param = {
        book_id = bookID
    }
    self:Get(self, "api_getWriterChapterList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_GetMyBookDetail(bookID, callback)
    local param = {
        book_id = bookID
    }
    self:Get(self, "api_getMyWriterBookDetail", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_GetBookDetail(bookID, callback)
    local param = {
        book_id = bookID
    }
    self:Get(self, "api_getWriterBookDetail", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_GetChapterDetail(book_id, chapter_id, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id
    }
    self:Get(self, "api_getWriterChapterDetail", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_NewBook(penName, title, description, tag, cover_image, callback)
    local param = {
        title = title,
        description = description,
        tag = string.join(tag, ','),
        cover_image = cover_image,
        writer_name = penName,
    }
    self:Post(self, "api_createWriterBook", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_DeleteBook(bookID, callback)
    local param = {
        book_id = bookID,
    }
    self:Post(self, "api_delWriterBook", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_SaveBook(penName, book_id, title, description, tag, cover_image, callback)
    local param = {
        book_id = book_id,
        title = title,
        description = description,
        tag = string.join(tag, ','),
        cover_image = cover_image,
        writer_name = penName,
    }
    self:Post(self, "api_saveWriterBook", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_NewChapter(book_id, chapter_id, title, description, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id,
        title = title,
        description = description
    }
    self:Post(self, "api_createWriterChapter", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_SaveChapter(book_id, chapter_id, title, description, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id,
        title = title,
        description = description
    }
    self:Post(self, "api_saveWriterChapter", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_GetDialogList(book_id, chapter_id, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id
    }
    self:Get(self, "api_getWriterDialogList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_GetMyDialogList(book_id, chapter_id, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id
    }
    self:Get(self, "api_getMyWriterDialogList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_SaveDialogList(book_id, chapter_id, dialog_content, callback)
    --logic.debug.LogError(dialog_content)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id,
        dialog_content = dialog_content
    }
    self:Post(self, "api_saveWriterDialog", param, function(result)
        callback(result)
    end, nil, nil, true)
end


-- function GameHttp:StoryEditor_GetRoleList(book_id, callback)
--     local param = {
--         book_id = book_id,
--     }
--     self:Get(self, "api_getWriterRole", param, function(result)
--         callback(result)
--     end)
-- end

function GameHttp:StoryEditor_SaveRoleList(book_id, json, callback)
    --logic.debug.LogError(dialog_content)
    local param = {
        book_id = book_id,
        role_list = json,
    }
    self:Post(self, "api_saveWriterRole", param, function(result)
        callback(result)
    end, nil, nil, true)
end



---@param pay_type --支付类型: 0只是保存进度 1钥匙支付 2观看广告 3倒计时
function GameHttp:StoryEditor_SaveReadingRecord(book_id, chapter_id, pay_type, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id,
        pay_type = pay_type,
    }
    self:Post(self, "api_saveReadingRecord", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_GetBookUploadToken(book_id, chapter_id, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id
    }
    self:Get(self, "api_getBookUploadToken", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_ModifyChapter(book_id, chapter_id, title, description, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id,
        title = title,
        description = description,
    }
    self:Post(self, "api_saveWriterChapter", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_GetReadingRecord(book_id, callback)
    local param = {
        book_id = book_id,
    }
    self:Get(self, "api_getReadingRecord", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_FinishReadingChapter(book_id, chapter_id, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_id,
    }
    self:Post(self, "api_finishReadingChapter", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_SaveFavorite(book_id, isFavorite, callback)
    local param = {
        book_id = book_id,
    }
    self:Post(self, "api_saveFavorite", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:SubmitChapter(book_id, chapter_number, callback)
    local param = {
        book_id = book_id,
        chapter_number = chapter_number,
    }
    self:Post(self, "api_submitChapter", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:StoryEditor_UpdateGuide(callback)
    local param = {
    }
    self:Post(self, "api_updateWriterGuide", param, callback, nil, nil, true)
end

function GameHttp:CheckAccessToken(callback)
    local param = {
        access_token = logic.cs.UserDataManager.Accesskey,
    }
    self:Post(self, "api_checkAccessToken", param, callback, nil, nil, true)
end
--endregion


--这里处理账号过期的逻辑-------------------------------------------------------

local FunctionList = {}
function GameHttp:AddFunList(fun)
    FunctionList = {}
    FunctionList[0] = fun
    --print("保存的类型："..type(fun))
end

function GameHttp:Dofun()
    local fun = FunctionList[0]

    --local num=22
    --print("类型："..type(fun).."--num:"..type(num))
    --print("--方法："..type(type()))
    fun()
end

function GameHttp:getNewTonken()
    --logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.LoginTouristStart)
    --logic.gameHttp:TouristLogin(  
    --    function(result)
    --        logic.debug.Log("----touristLogin---->" .. result)
    --        local json = core.json.Derialize(result)
    --        local code = tonumber(json.code)
    --        if code ==200 then
    --            logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.LoginTouristResultSucc)
    --            if  logic.cs.GameHttpNet.TOKEN == json.data.token then
    --                logic.debug.LogError("Lua获取新tonken失败,token未变化:"..json.data.token)
    --                --logic.cs.UINetLoadingMgr:Close()
    --            else
    --                logic.debug.Log("Lua获取新tonken成功")
    --                logic.cs.GameHttpNet.TOKEN = json.data.token
    logic.gameHttp:Dofun()
    --                --FunctionList={}
    --            end
    --        else
    --            logic.cs.talkingdata:OpenApp(logic.cs.EventEnum.LoginTouristResultFail)
    --        end
    --    end)

end

--end  --------------------------------------------------------------


function GameHttp:GoBackStep(bookid, chapterid, dialogid, callback)
    local param = {
        bookid = bookid,
        chapterid = chapterid,
        dialogid = dialogid,
    }
    self:Post(self, "api_goBackStep", param, function(result)
        callback(result)
    end, nil, nil, true)
end

function GameHttp:ReceiveGameScoreAward(callback)
    local param = {
        ProtocolError = "api_receiveGameScoreAward",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
    }
    self:Post(self, "api_receiveGameScoreAward", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--【领取平台评分奖励】
function GameHttp:ReceivePlatformAward(callback)
    local param = {
        ProtocolError = "api_receivePlatformAward",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
    }
    self:Post(self, "api_receivePlatformAward", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【 设置用户关闭评分】 设置类型3 是否已经提示去商店评分 (1是  0否) </param>
function GameHttp:SetUserInfo(callback)
    local param = {
        ProtocolError = "api_setUserInfo",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
        value = "1";
        change_type = "3";
    }
    self:Post(self, "api_setUserInfo", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【评分建议保存】
function GameHttp:ScoreSuggest(_content, callback)
    local param = {
        ProtocolError = "api_scoreSuggest",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
        content = _content;
    }
    self:Post(self, "api_scoreSuggest", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--获取周更书本
function GameHttp:GetbooksUpdatedWeekly(callback)
    local param = {
        ProtocolError = "api_booksUpdatedWeekly",
    }
    self:Get(self, "api_booksUpdatedWeekly", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【旧版排行榜】
function GameHttp:GetBookRankInfo(callback)
    local param = {
        ProtocolError = "api_bookRanking",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
    }
    self:Get(self, "api_bookRanking", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--【新版排行榜】
function GameHttp:GetbookRanking(callback)
    local param = {
        ProtocolError = "api_bookRankingNew",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
    }
    self:Get(self, "api_bookRankingNew", param, function(result)
        callback(result)
    end, nil, nil, true)
end



--进入主页 获取自己书架的书本
function GameHttp:GetSelfBookInfo(callback)
    local param = {
        ProtocolError = "api_getIndexData",
        UUID = logic.cs.GameHttpNet.UUID,
        TOKEN = logic.cs.GameHttpNet.TOKEN,
        jpushid = logic.cs.UserDataManager.UserData.JPushId,
    }
    self:Get(self, "api_getIndexData", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--获取首页书本列表
function GameHttp:GetIndexBookList(_type, callback)
    local param = {
        ProtocolError = "api_getIndexBookList",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
        type = _type;
    }
    self:Get(self, "api_getIndexBookList", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--获取搜索分类的书本
function GameHttp:GetSearchBookList(_type, callback)
    local param = {
        ProtocolError = "api_getSearchBookList",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
        type = _type;
    }
    self:Get(self, "api_getSearchBookList", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--获取红点状态
function GameHttp:GetRedDot(callback)
    local param = {
        ProtocolError = "api_redDot",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
    }
    self:Get(self, "api_redDot", param, function(result)
        callback(result)
    end, nil, nil, true)
end

-- 获更新每天首次登陆有任务未完成时红点状态
function GameHttp:FirstTaskNotice(callback)
    local param = {
        ProtocolError = "api_firstTaskNotice",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
    }
    self:Get(self, "api_firstTaskNotice", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--更新在线阅读时长
function GameHttp:updateReadingTaskTime(second, callback)
    local param = {
        second = second,
    }
    self:Post(self, "api_updateReadingTaskTime", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--获取在线阅读任务状态
function GameHttp:GetReadingTaskStatus(callback)
    local param = {
        ProtocolError = "api_getReadingTaskStatus",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
    }
    self:Get(self, "api_getReadingTaskStatus", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--领取在线阅读任务奖励
function GameHttp:ReceiveReadingTaskPrize(callback)
    local param = {
    }
    self:Post(self, "api_receiveReadingTaskPrize", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--获取用户头像及框列表
function GameHttp:GetUserFrameList(callback)
    local param = {
        ProtocolError = "api_getUserFrameList",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
    }
    self:Get(self, "api_getUserFrameList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【设置用户头像】
function GameHttp:SetUserAvatar(curAvatarID, curAvatarFrameID, curBarrageFrameID, curCommentFrameID, callback)
    local param = {
        ProtocolError = "api_setUserAvatar",
        TOKEN = logic.cs.GameHttpNet.TOKEN,
        avatar = curAvatarID;
        avatar_frame = curAvatarFrameID;
        comment_frame = curBarrageFrameID;
        barrage_frame = curCommentFrameID;
    }

    self:Post(self, "api_setUserAvatar", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【获取限时活动状态】
function GameHttp:GetLimitTimeActivityStatus(callback)
    local param = {

    }
    self:Get(self, "api_getLimitTimeActivityStatus", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【获取通用活动列表】
function GameHttp:GetActivityList(callback)
    local param = {

    }
    self:Get(self, "api_getActivityList", param, function(result)
        callback(result)
    end, nil, nil,true)
end

--【获取通用活动详情】
function GameHttp:GetActivityInfo(_activity,callback)
    local param = {
        activity_id=_activity;
    }
    self:Get(self, "api_getActivityInfo", param, function(result)
        callback(result)
    end, nil, nil,true)
end

--【获得的创作综合书本的信息】
function GameHttp:GetwriterIndex(callback)
    local param = {
    }
    self:Get(self, "api_writerIndex", param, function(result)
        callback(result)
    end, nil, nil,true)
end

--【获得创作更多界面里面的热门书本】
function GameHttp:GetWriterHotBookList(_page,callback)
    local param = {
        page=_page;
    }
    self:Get(self, "api_getWriterHotBookList", param, function(result)
        callback(result)
    end, nil, nil,true)
end

--【获得创作更多界面里面的热门书本】
function GameHttp:GetWriterNewBookList(_page,callback)
    local param = {
        page=_page;
    }
    self:Get(self, "api_getWriterNewBookList", param, function(result)
        callback(result)
    end, nil, nil,true)
end

--【我的写作书本列表】
function GameHttp:GetMyWriterBookList(callback)
    local param = {
    }
    self:Get(self, "api_getMyWriterBookList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【获取创作已读过的书本】
function GameHttp:GetReadingHistory(callback)
    local param = {
    }
    self:Get(self, "api_getReadingHistory", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【默认书本搜索列表(浏览量排序)】
function GameHttp:GetWriterBookList(_page,booktypeArr,title,callback)
    local param = {
        page=_page;
        tag=booktypeArr;
        title=title;
    }
    self:Get(self, "api_getWriterBookList", param, function(result)
        callback(result)
    end, nil, nil,true)
end


--【首页作者列表】
function GameHttp:GetHotWriter(callback)
    local param = {
    }
    self:Get(self, "api_getHotWriter", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--【获取作者详情】
function GameHttp:GetWriterInfo(_uid,callback)
    local param = {
        uid=_uid;
    }
    self:Get(self, "api_getWriterInfo", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【获取动态列表(分页)】
function GameHttp:GetActionLogPage(_uid,_page,callback)
    local param = {
        uid=_uid;
        page=_page;
    }
    self:Get(self, "api_getActionLogPage", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【获取作者首页书本列表】
function GameHttp:GetWriterHomeBookList(_uid,callback)
    local param = {
        uid=_uid;
    }
    self:Get(self, "api_getWriterHomeBookList", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--获取用户可用的背包道具
function GameHttp:GetBackpackProp( callback)
    local param = {
        iggid = logic.cs.UserDataManager.IGGid,
        access_token = logic.cs.UserDataManager.Accesskey,
        system_type = logic.cs.GameHttpNet.SYSTEMTYPE,
    }
    self:Get(self, "api_getBackpackProp", param, callback, 100, nil, true)
end
-- 设置背包道具为已读状态
---@param id 背包记录id 可单个，多个（英文逗号隔开），默认全部标记为已读
function GameHttp:SetPropRead(id, callback)
    local param = {
        iggid = logic.cs.UserDataManager.IGGid,
        access_token = logic.cs.UserDataManager.Accesskey,
        system_type = logic.cs.GameHttpNet.SYSTEMTYPE,
        id = id,
    }
    self:Post(self, "api_setPropRead", param, callback, 20, nil, true)
end
--通过类型获取可用道具(折扣列表)
---@param _type 道具类型 多个用英文逗号隔开，1.装扮(折扣券) 2.选项(折扣券) 3.选项[抵扣券] 4.装扮[抵扣券]  5.钥匙[抵扣券] 6.信鸽
function GameHttp:SetPropByType(_type,callback)
    local param = {
        iggid = logic.cs.UserDataManager.IGGid,
        access_token = logic.cs.UserDataManager.Accesskey,
        system_type = logic.cs.GameHttpNet.SYSTEMTYPE,
        type = _type,
    }
    self:Get(self, "api_getPropByType", param, callback, nil, nil, true)
end


--【关注作者】
function GameHttp:SetWriterFollow(_uid,callback)
    local param = {
        uid=_uid;
    }
    self:Post(self, "api_setWriterFollow", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【赞同作者】
function GameHttp:SetWriterAgree(_uid,callback)
    local param = {
        uid=_uid;
    }
    self:Post(self, "api_setWriterAgree", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--【获取信鸽可用次数】
function GameHttp:GetFreePrivateLetterCount(callback)
    local param = {
    }
    self:Get(self, "api_getFreePrivateLetterCount", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【发送信鸽】
function GameHttp:SendWriterLetter(_uid,_content,callback)
    local param = {
        uid=_uid;
        content=_content;
    }
    self:Post(self, "api_sendWriterLetter", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【修改签名】
function GameHttp:UpdateSign(_sign,callback)
    local param = {
        sign=_sign;
    }
    self:Post(self, "api_updateSign", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--【获取私信组(分页)】【跟哪些人对话的列表  boxlist】
function GameHttp:GetPrivateLetterTeamPage(_page,_limit,callback)
    local param = {
        page=_page;
        limit=_limit;
    }
    self:Get(self, "api_getPrivateLetterTeamPage", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--【获取私信对话列表(分页)】【跟某个人单独聊天的具体内容】
function GameHttp:GetPrivateLetterPage(_uid,_page,_limit,callback)
    local param = {
        uid=_uid;
        page=_page;
        limit=_limit;
    }
    self:Get(self, "api_getPrivateLetterPage", param, function(result)
        callback(result)
    end, nil, nil, true)
end


--【单个、批量删除邮件】
function GameHttp:DelMail(_msgid,callback)
    local param = {
        msgid=_msgid;
    }
    self:Post(self, "api_delMail", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【批量已读或领取奖励邮件】
function GameHttp:BatchReadReceiveMail(_msgid,callback)
    local param = {
        msgid=_msgid;
    }
    self:Post(self, "api_batchReadReceiveMail", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【单个、批量删除私信】
function GameHttp:DelPrivateLetterTeam(_ids,callback)
    local param = {
        ids=_ids;
    }
    self:Post(self, "api_delPrivateLetterTeam", param, function(result)
        callback(result)
    end, nil, nil, true)
end

--【阅读私信组】
function GameHttp:ReadPrivateLetterTeam(_ids,callback)
    local param = {
        ids=_ids;
    }
    self:Post(self, "api_readPrivateLetterTeam", param, function(result)
        callback(result)
    end, nil, nil, true)
end


return GameHttp.New()