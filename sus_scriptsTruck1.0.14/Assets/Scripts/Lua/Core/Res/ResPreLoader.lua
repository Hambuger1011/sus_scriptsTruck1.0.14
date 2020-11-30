local class = Core.oop.class;
local ResPreLoader = class("ResPreLoader")
local M = ResPreLoader

function M:ctor(resTag)
    self.preLoadTasks = {}
    self.isPreLoad = false
    self.resTag = resTag
end

function M:Begin()
    self.isPreLoad = true
end

function M:End()
    self.isPreLoad = false
end

function M:Progress()
    if not self.isPreLoad then
        return 1
    end
    local p = 0
    for k,v in pairs(self.preLoadTasks) do
        p = p + v:Progress()
    end
    p = p/#self.preLoadTasks
    return p
end

function M:IsDone()
    if not self.isPreLoad then
        return true
    end
    local isDone = true
    for k,v in pairs(self.preLoadTasks) do
        if not v:IsDone() then
            isDone = false
            break
        end
        v:DoneCallback()
    end
    return isDone
end

function M:Push(resType, name, callback)
    local task = Logic.res.LoadAsync(self.resTag, resType, name, callback)
    if not task then
        Logic.log.Error("预加载失败:type=" .. resType .. "," .. name)
    end
    task:Retain(self.resTag)
    table.insert(self.preLoadTasks, task)
    return task
end

return M