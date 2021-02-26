local Class = core.Class
local Eggs = Class("Eggs")
local this = Eggs
local spinAnim = {
    baozha = "baozha",
    chuxian = "chuxian",
    daiji = "daiji",
    xingxing = "xingxing",
}
local eggObjs = {}
local co = nil
local cfg = nil
local receivedEggs = {}
local receivedNum = 0
local AutoPlayPause = false
local canReceived = false
local maskBtn = nil
local eggId = nil

function Eggs:Show(_cfg,_eggId)
    this:Clean()
    cfg = _cfg
    eggId = _eggId
    if cfg == nil or eggId == nil then
        return
    end
    for k, v in pairs(receivedEggs) do
        if tonumber(eggId) == v then
            logic.debug.Log("彩蛋已领取或者已过期Id==" .. tostring(eggId))
            return
        end
    end
    if logic.cs.GameDataMgr.InAutoPlay then
        AutoPlayPause = true
        logic.cs.GameDataMgr.InAutoPlay = false
    end
    logic.debug.Log("创建彩蛋Id==" .. tostring(eggId))
    logic.bookReadingMgr.view.eggTransComponent.gameObject:SetActiveEx(true)
    if maskBtn == nil then
        maskBtn = logic.bookReadingMgr.view.eggTransComponent.gameObject:GetComponent(typeof(logic.cs.Button))
        maskBtn.onClick:RemoveAllListeners()
        maskBtn.onClick:AddListener(function()
            this:EggClick()
        end)
    end
    local prefab = logic.bookReadingMgr.Res:GetPrefab("Assets/Bundle/UI/BookReading/Eggs.prefab")

    eggObjs.gameObject = logic.cs.GameObject.Instantiate(prefab, logic.bookReadingMgr.view.eggTransComponent, false)
    eggObjs.gameObject.name = "egg" .. tostring(eggId)
    eggObjs.Egg = CS.DisplayUtil.GetChild(eggObjs.gameObject, "Egg"):GetComponent(typeof(logic.cs.UITweenButton))
    eggObjs.eggSpine = logic.SpineEgg.New(eggObjs.gameObject)
    eggObjs.Egg.onClick:RemoveAllListeners()
    eggObjs.Egg.onClick:AddListener(function()
        this:EggClick()
    end)
    local localScale = Vector3.New(1, 1, 1)
    local anchoredPosition = Vector2.New(0, 0)
    eggObjs.eggSpine:SetScale(localScale)
    eggObjs.eggSpine:SetPosition(anchoredPosition)
    eggObjs.eggSpine:SetData(spinAnim.chuxian)
    co = coroutine.create(function()
        coroutine.wait(2)
        if eggObjs and eggObjs.eggSpine then
            eggObjs.eggSpine:SetData(spinAnim.daiji, true)
            canReceived = true
        end
    end)
    eggObjs.gameObject:SetActiveEx(true)
    coroutine.resume(co)
end

function Eggs:EggClick()
    if canReceived then
        canReceived = false
        coroutine.stop(co)
        co = coroutine.create(function()
            eggObjs.eggSpine:SetData(spinAnim.baozha)
            canReceived = false
            coroutine.wait(2.1)
            logic.cs.GameObject.Destroy(eggObjs.gameObject)
            logic.gameHttp:ReceiveEggActivityAward(eggId, logic.bookReadingMgr.bookData.BookID, cfg.dialogid, function(result)
                local json = core.json.Derialize(result)
                local code = tonumber(json.code)
                if code == 200 then
                    table.insert(receivedEggs, eggId)
                    logic.cs.UserDataManager:ResetMoney(1, tonumber(json.data.bkey))
                    logic.cs.UserDataManager:ResetMoney(2, tonumber(json.data.diamond))
                    local cc = coroutine.create(function()
                        coroutine.wait(2)
                        logic.bookReadingMgr.view.eggTransComponent.gameObject:SetActiveEx(false)
                    end)
                    coroutine.resume(cc)
                else
                    table.insert(receivedEggs, eggId)
                    logic.bookReadingMgr.view.eggTransComponent.gameObject:SetActiveEx(false)
                end
            end)
        end)
        coroutine.resume(co)
    end
end

function Eggs:Clean()
    logic.bookReadingMgr.view.eggTransComponent.gameObject:SetActiveEx(false)
    if AutoPlayPause then
        AutoPlayPause = false
        logic.cs.GameDataMgr.InAutoPlay = true
    end
    eggObjs = {}
    co = nil
    receivedNum = 0
    cfg = nil
    maskBtn = nil
    canReceived = false
end

function Eggs:CleanReceivedEggs()
    receivedEggs = {}
end

return Eggs