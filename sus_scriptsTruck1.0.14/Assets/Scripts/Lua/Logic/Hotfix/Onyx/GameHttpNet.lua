local Class = core.Class
local target = CS.GameHttpNet
local GameHttpNet = Class('GameHttpNet')

function GameHttpNet:Register()
	xlua.hotfix(target, "GetOrderToSubmitForIos", GameHttpNet.GetOrderToSubmitForIos)
	xlua.hotfix(target, "GetOrderToSubmitForAndroid", GameHttpNet.GetOrderToSubmitForAndroid)
	--xlua.hotfix(target, "get_TOKEN", GameHttpNet.GetTOKEN)
    --xlua.hotfix(target, "set_TOKEN", GameHttpNet.SetTOKEN)
 
end

function GameHttpNet:Unregister()
	xlua.hotfix(target, "GetOrderToSubmitForIos", nil)
	xlua.hotfix(target, "GetOrderToSubmitForAndroid", nil)
end


function GameHttpNet.GetOrderToSubmitForIos(self,vOrderId, vOrderToken, vProductid, vTransactionId, vIsSandbox, vCallBackHandler)
    if string.IsNullOrEmpty(vOrderId) then
        return
    end
    logic.gameHttp:GetOrderToSubmitForIos(vOrderId, vOrderToken, vProductid, vTransactionId, vIsSandbox, function(orderID,result)
        --logic.debug.Log("----GetOrderToSubmitForIos---->" .. result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 201 or code == 202 then
            logic.cs.UIAlertMgr:Show("Error", json.msg)
        end
        vCallBackHandler(orderID,result)
    end)
end

function GameHttpNet.GetOrderToSubmitForAndroid(self,recharge_no, vOrderId, vOrderToken, vProductid, vPackagename, vDatasignature, vPurchasetime, vPurchaseState, vTestToken, vCallBackHandler)
    if string.IsNullOrEmpty(recharge_no) then
        return
    end
    logic.gameHttp:GetOrderToSubmitForAndroid(recharge_no, vOrderId, vOrderToken, vProductid, vPackagename, vDatasignature, vPurchasetime, vPurchaseState, vTestToken, function(orderID,result)
        --logic.debug.Log("----GetOrderToSubmitForIos---->" .. result)
        local json = core.json.Derialize(result)
        local code = tonumber(json.code)
        if code == 201 or code == 202 then
            logic.cs.UIAlertMgr:Show("Error", json.msg)
        end
        vCallBackHandler(orderID,result)
    end)
end

function GameHttpNet.GetTOKEN(self)
    --logic.debug.LogError('get m_TOKEN:'..self.m_TOKEN)
    return self.m_TOKEN
end


function GameHttpNet.SetTOKEN(self,value)
    --logic.debug.LogError('set m_TOKEN:'..value)
    self.m_TOKEN = value
end

return GameHttpNet