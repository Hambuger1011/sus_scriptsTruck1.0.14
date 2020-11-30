local Class = core.Class
local target = CS.PurchaseRecordManager
local PurchaseRecordManager = Class('PurchaseRecordManager')

xlua.private_accessible(target) --开启私有成员访问权限

function PurchaseRecordManager:Register()
	xlua.hotfix(target, "AddPurchaseRecord", PurchaseRecordManager.AddPurchaseRecord)
	xlua.hotfix(target, "SendRestoredToServer", PurchaseRecordManager.SendRestoredToServer)
end

function PurchaseRecordManager:Unregister()
	xlua.hotfix(target, "AddPurchaseRecord", nil)
	xlua.hotfix(target, "SendRestoredToServer", nil)
end


function PurchaseRecordManager.AddPurchaseRecord(
    self,
    recharge_no,  
    vOrderId,  
    vOrderToken, 
    vProductid, 
    vPackagename, 
    vDatasignature, 
    vPurchasetime, 
    vPurchaseState, 
    vPaymentType
    )
    
    local PurchaseManager = CS.PurchaseManager.Instance
    if string.IsNullOrEmpty(recharge_no) then
        --logic.debug.LogError("[*]********* order:"..vOrderId)
        --恢复订单，如果在这个接口返回，则也通知服务端，进行补单
        local hasBuyItem = CS.HasBuyProductItem()
        hasBuyItem.myOrderId = recharge_no
        hasBuyItem.transactionId = vOrderId
        hasBuyItem.token = vOrderToken
        hasBuyItem.productId = vProductid
        self:DoTransactionRestored(hasBuyItem)
        return
    end
    local vPrice = PurchaseManager:CheckPriceByProduct(vProductid)
    local result = recharge_no .. "," .. vOrderId .. "," .. vOrderToken .. "," .. vProductid .. "," .. vPackagename .. "," .. vDatasignature .. "," .. vPurchasetime .. "," .. vPurchaseState .. "," .. vPrice .. "," .. vPaymentType
    if not self.mPurchaseRecordDic:ContainsKey(vOrderId) then
        self.mPurchaseRecordDic:Add(vOrderId, result)
        logic.cs.PlayerPrefs.SetString(vOrderId, result)
        self:SavePurchaseRecordDic()
        --logic.debug.LogError("save order:"..vOrderId)
    else
        logic.debug.LogError("had order:"..vOrderId)
    end
end


function PurchaseRecordManager.SendRestoredToServer(self,vTransactionId, vSignatureToken)
    --logic.cs.UINetLoadingMgr:Show()
    logic.cs.GameHttpNet:Recoverorder(vTransactionId,vSignatureToken,function(arg)
        local needRemove = false
        local result = tostring(arg)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 200 then
            needRemove = true
        elseif code == 201 then
            logic.cs.UIAlertMgr:Show("Error", json.msg)
        elseif code == 202 then
            needRemove = true
            logic.cs.UIAlertMgr:Show("Error", json.msg)
        end
        if needRemove and self.mRestoredRecoredDic:ContainsKey(vTransactionId) then
            self.mRestoredRecoredDic:Remove(vTransactionId)
            self:SaveRestoredRecoredDic()
        end
    end)
end

return PurchaseRecordManager