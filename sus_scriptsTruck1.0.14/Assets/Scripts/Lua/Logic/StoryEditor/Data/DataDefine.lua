
---book简略信息
---@class StoryEditor_BookDetial
local BookDetial = {
    id = 11,
    title = "100",
    cover_image = "https://www.baidu.com",
    tag = "2,5,6",
    writer_id = 10270,
    read_count = 0,
    status = 1
}

---book详细信息
---@class StoryEditor_BookDetials
local BookDetials = {
    tagArray = {1,2,3},
    id = 1,
    title = "The new title",    --书名
    cover_image = "http:--pic29.nipic.com/20130525/12162827_163453547130_2.jpg",   --封面图
    writer_name = "alan",   --作者
    writer_id = 1,
    description = "The book description",   
    tag = "1,2",               --分类标签 id
    total_chapter_count = 0,        --总章节数
    update_chapter_count = 0,    --已开放章节数
    status = 0,             --状态 0创作中 ,1已发布,2待审核状态,3审核失败,4已关闭
    read_count = 0,           --阅读量
    favorite_count = 0,      --收藏量
    word_count = 0,            --单词量
    fail_reason = "",          --审核或关闭理由,忽略
    create_time = "2020-02-24 15:28:25",
    update_time = "2020-02-24 15:28:25",
    publish_time = 0,         --发布时间
    role_list = '',            --roles list
    is_fav = 1,
    
    ---@type t_RoleTable
    roleTable = nil,
}


---@class StoryEditor_ChapterDetial
local ChapterDetial = {
    id = 12,
    title = "11222",
    writer_id = 10270,
    status = 1,
    book_id = 11,
    chapter_number = 1,
    description = "",
    dialog_count = 0,
    word_count = 0,
    checksum = "",
    update_version = 0,
    create_time = 1583824673,
    update_time = 1583824673,
    status = 1,
}

---@class StoryEditor_DialogData
local DialogData = {
    id = 0,
    dialog_number = 6,
    role = 1,
    title = "",
    type = 1,
    parent_dialog = 0,
    option_list = {
        [1]={
            option = '',
            jump = 4
        }
    },
    image = '',
    image_md5 = '',
}

---@class StoryEditor_Role
local RoleData = {
    id = 6,
    rolename = "",
    main = 1,
    sex = 1,
    icon = 1,
}


local class = core.Class

local DataDefine = {}

DataDefine.BookTags = {
    'Romance',
    'LGBTQ+',
    'Action',
    'Youth',
    'Adventure',
    'Drama',
    'Comedy',
    'Horror',
    '18+',
    'Fantasy',
    'Suspense',
    'Others'
}

---@class EBubbleType
DataDefine.EBubbleType=
{
    Narration = 0,  --旁白(Middle)
    SupportingRole = 1,    --配角(Right)
    LeadRole = 2,   --主角(Left)
    Count = 3,
}

---@class EBubbleBoxType
DataDefine.EBubbleBoxType=
{
    Text = 0,
    Image = 1,
    Voice = 2,
    Selection = 3,
}

---@class t_StoryTable
DataDefine.t_StoryTable = class("StoryEditor.t_StoryTable")
function DataDefine.t_StoryTable:__init()
    
    ---@type Dication<int,t_StoryItem>
    self.items = System.Collections.SortedDictionary.New(function(x,y)
        local a = x.Value
        local b = y.Value
        return a.id - b.id
    end)
    

    self.FromTable = function(self,list)
        if list == nil then
            return self
        end

        self.items:Clear()
        ---@param json StoryEditor_DialogData
        for i,json in pairs(list) do
            local item = DataDefine.t_StoryItem.New()
            item:FromTable(json)
            self.items:Add(item.id, item)
        end
        self.items:Foreach(function(i,kv)
            local item = kv.Value
            --debug
            for i,optionItem in pairs(item.selections) do
                --local name  = optionItem.option
                local jumpId = optionItem.jump
                local jumpItem = self.items:Get(jumpId)
                if jumpItem then
                    jumpItem.parent_dialog = 0
                end
            end
            ---@type t_StoryItem
            local parentItem = self.items:Get(item.parent_dialog)
            if parentItem and parentItem ~= item then
                parentItem.nextId = item.id
            end
        end)
        return self
    end

    self.ToTable = function(self,tb)
        ---@type StoryEditor_DialogData
        local list = {}
        self.items:Foreach(function(i,kv)
            local tb = kv.Value:ToTable()
            table.insert(list,tb)
        end)
        return list
    end

    self.ToJson  = function(self)
        local json = core.json.Serialize(self:ToTable())
        return json
    end
    
    self.AppendStoryItem = function(self, storyItem)
        local id = 1
        ---@type t_StoryItem
        local lastItem = self.items:GetByIndex(self.items:Count())
        if lastItem then
            id = lastItem.id + 1
        end
        storyItem.id = id
        self.items:Add(id,storyItem)
    end
end

---@class t_StoryItem
DataDefine.t_StoryItem = class("StoryEditor.t_StoryItem")
function DataDefine.t_StoryItem:__init()
    self.id = 0
    self.svrID = 0
    self.msgBoxType = DataDefine.EBubbleBoxType.Text
    self.text = ''
    self.roleID = 0
    ---@type {option='',jump=0}[]
    self.selections = {}
    self.parent_dialog = 0
    self.nextId = 0
    self.image = nil
    self.imageMd5 = nil

    self.ToTable = function(self)
        ---@type StoryEditor_DialogData
        local tb = {}
        tb.id = self.svrID
        tb.dialog_number = self.id
        tb.role = self.roleID
        tb.title = self.text
        tb.type = self.msgBoxType
        tb.parent_dialog = self.parent_dialog
        tb.option_list = self.selections
        tb.image = self.image
        tb.image_md5 = self.imageMd5
        return tb
    end
    self.FromTable = function(self,tb)
        if tb == nil then
            return self
        end
        self.id = tb.dialog_number
        self.svrID = tb.id or 0
        self.msgBoxType = tb.type
        self.text = tb.title
        self.roleID = tb.role
        self.parent_dialog = tb.parent_dialog
        self.image = tb.image
        self.imageMd5 = tb.image_md5
        
        if type(tb.option_list) == 'string' then
            local list = core.json.Derialize(tb.option_list) or {}
            for i,item in pairs(list) do
                local o = {}
                o.option = item.option
                o.jump = item.jump
                self.selections[i] = o
            end
        else
            self.selections = tb.option_list
        end
        return self
    end
end


---@class t_RoleTable
DataDefine.t_RoleTable = class("StoryEditor.t_RoleTable")
function DataDefine.t_RoleTable:__init()
    self.roleIdSeq = 0
    ---@type SortedDictionary<int,t_Role>
    self.roles = System.Collections.SortedDictionary.New(function(x,y)
        local a = x.Value
        local b = y.Value
        ---@type EBubbleType
        local aType = a.type
        ---@type EBubbleType
        local bType = b.type
        if aType ~= bType then
            --主角
            if aType == DataDefine.EBubbleType.LeadRole then
                return -1    --a在前面
            elseif bType == DataDefine.EBubbleType.LeadRole then
                return 1    --b在前面
            --旁白
            elseif aType == DataDefine.EBubbleType.Narration then
                return -1    --a在前面
            elseif bType == DataDefine.EBubbleType.Narration then
                return 1    --b在前面
            end
        end
        return a.id - b.id  --id升序
    end)

    self.FromTable = function(self,tb)
        self.roleIdSeq = 0
        self.roles:Clear()
        if tb then
            for k,v in pairs(tb) do
                local item = DataDefine.t_Role.New()
                item:FromTable(v)
                self.roleIdSeq = math.max(self.roleIdSeq, item.id)
                self.roles:Set(item.id,item)
            end
        end
        
        --自动添加Narration
        local roleData= logic.StoryEditorMgr.DataDefine.t_Role.New()
        roleData.id = 0
        roleData.icon = 0
        roleData.name = 'Narration'
        roleData.type = logic.StoryEditorMgr.DataDefine.EBubbleType.Narration
        self.roles:Set(0,roleData)
        self.roles:Foreach(function(i,kv)
            if kv.Value.type == DataDefine.EBubbleType.Narration and kv.Value.id ~= 0 then
                --detial.roles:Remove(kv.Key)
                kv.Value.type = DataDefine.EBubbleType.LeadRole
                if string.IsNullOrEmpty(kv.Value.name) then
                    kv.Value.name = 'error'
                end
            end
        end)
    end

    self.ToTable = function(self)
        local list = {}
        self.roles:Foreach(function(i,kv)
            ---@type t_Role
            local v = kv.Value
            ---@type StoryEditor_Role
            local item = v:ToTable()
            table.insert(list,item)
        end)
        return list
    end

    self.ToJson = function(self)
        local tb = self:ToTable()
        local json = core.json.Serialize(tb)
        return json
    end
    

    ---@param role t_Role
    self.AddRole = function(self,role)
        local newID = (self.roleIdSeq or 0) + 1
        self.roleIdSeq = newID
        role.id = newID
        self.roles:Set(newID, role)
    end

    self.RemoveRole = function(self,id)
        self.roles:Remvoe(id)
    end

    self.GetRole = function(self,id)
        return self.roles:Get(id)
    end
    
    ---@param self t_StoryItem
    ---@param detials t_StoryDetial
    self.GetDialogType = function(self, roleID)
        ---@type t_Role
        local role = self.roles:Get(roleID)
        if role then
            return role.type
        end
        logic.debug.LogError('Not Found: roleID ='..tostring(self.roleID))
        return DataDefine.EBubbleType.Narration
    end
end

---@class t_Role
DataDefine.t_Role = class("StoryEditor.t_Role")
function DataDefine.t_Role:__init()
    self.id = 0
    self.name = ''
    self.icon = 1
    self.type = DataDefine.EBubbleType.Narration
    self.isMale = false

    self.GetIconName = function(self)
        local headName = nil
        if self.isMale then
            headName = string.format('bg_avatar_male_%.2d',self.icon)
        else
            headName = string.format('bg_avatar_female_%.2d',self.icon)
        end
        return headName
    end

    self.ToTable = function(self)
        ---@type StoryEditor_Role
        local tb = {}
        tb.id = self.id
        tb.rolename = self.name
        tb.main = (self.type == DataDefine.EBubbleType.LeadRole and 1 or 0)
        tb.sex = (self.isMale and 1 or 0)
        tb.icon = self.icon
        return tb
    end

    ---@param tb StoryEditor_Role
    self.FromTable = function(self,tb)
        if tb == nil then
            return self
        end
        self.id = tb.id
        self.name = tb.rolename
        self.type = (tb.main == 1 and DataDefine.EBubbleType.LeadRole or DataDefine.EBubbleType.SupportingRole)
        self.isMale = (tb.sex == 1 and true or false)
        self.icon = tb.icon
        return self
    end
end


--故事编辑节点
---@class t_StoryNode
DataDefine.t_StoryNode = class("StoryEditor.t_StoryNode")
function DataDefine.t_StoryNode:__init()
    self.name = ''
    ---@type t_StoryNode[]
    self.children = {}
    ---@type t_StoryItem
    self.storyItem = nil
    self.refStoryTable = nil
    self.isRoot = false


    self.AddChild = function(self, storyTable, storyItem, pos)
        local child = DataDefine.t_StoryNode.New()
        child.refStoryTable = storyTable
        child.storyItem = storyItem
        if pos then
            table.insert(self.children, pos, child)
        else
            table.insert(self.children, child)
        end
        return child
    end
    self.CreateChild = function(self, storyTable, storyItem, pos)
        if storyItem then
            storyTable:AppendStoryItem(storyItem)
        end
        return self:AddChild(storyTable, storyItem, pos)
    end
    self.RemoveChild = function(self, index)
        table.remove(self.children, index)
        if self.storyItem then
            self.refStoryTable.items:Remove(self.storyItem.id)
        end
    end

    self.GetID = function(self)
        if self.storyItem then
            return self.storyItem.id
        end
        return 0
    end
    self.IsSelectionNode = function()
        if self.storyItem then
            return self.storyItem.msgBoxType == DataDefine.EBubbleBoxType.Selection
        end
        return false
    end
    
    ---@param storyItem t_StoryItem
    self.SetChildren = function(self, storyTable, storyItem)
        if self.hasChild then
            logic.debug.LogError('loop set child:'..(storyItem and storyItem.id or 'nil'))
            return
        end
        self.hasChild = true
        self.refStoryTable = storyTable
        while storyItem and storyItem ~= self.storyItem do
            local child = self:AddChild(storyTable, storyItem)
            for i,selectionItem in pairs(storyItem.selections) do
                local name  = selectionItem.option
                local jumpId = selectionItem.jump
                local subNode = child:AddEmptySelection()
                subNode.name = name
                local jumpItem = storyTable.items:Get(jumpId)
                subNode:SetChildren(storyTable, jumpItem)
            end
            storyItem = storyTable.items:Get(storyItem.nextId)
        end
    end

    self.ToStoryTable = function(self)
        if not self.isRoot then
            logic.debug.LogError("this node is not root")
            return nil
        end
        ---@type t_StoryTable
        local storyTable = self.refStoryTable
        storyTable.items:Clear()
        local dfs = nil

        --local nodeIdSeq = 1
        ---@param node t_StoryNode
        dfs = function(map,node,lastStoryItem)
            local hasMultiSubNode = false   --是否有子剧情(选项)
            ---@type t_StoryItem
            local storyItem = node.storyItem
            if storyItem then
                if lastStoryItem then
                    storyItem.parent_dialog = lastStoryItem.id
                end

                if not storyItem.id or storyItem.id == 0 then
                    logic.debug.LogError('jjjjj')
                end
                -- storyItem.id = nodeIdSeq
                -- nodeIdSeq = nodeIdSeq + 1
                map:Add(storyItem.id,node.storyItem)

                if storyItem.msgBoxType == DataDefine.EBubbleBoxType.Selection then
                    hasMultiSubNode = true
                    for i,child in pairs(node.children) do
                        dfs(map, child, storyItem)
                        
                        local jumpID = 0
                        if child.children[1] then
                            local jumpItem = child.children[1].storyItem
                            jumpID = jumpItem.id
                            jumpItem.parent_dialog = 0 --storyItem.id
                        end
                        local selectionItem = {}
                        selectionItem.option = child.name
                        selectionItem.jump = jumpID
                        storyItem.selections[i] = selectionItem
                    end
                end
            end
            if not hasMultiSubNode then
                local parent = node
                for i,child in pairs(node.children) do
                    dfs(map,child, parent.storyItem or lastStoryItem)
                    parent = child
                end
            end
        end
        dfs(storyTable.items,self,self.storyItem)
        return storyTable
    end
    self.ToJson  = function(self)
        local storyTable = self:ToStoryTable()
        local json = core.json.Serialize(storyTable:ToTable())
        return json
    end

    

    self.AddEmptySelection = function(self)
        local node = self:CreateChild(self.refStoryTable, nil)
        node.isRoot = true
        return node
    end
    

    self.InsertSelection = function(self,pos)
        local node = self:CreateChild(self.refStoryTable, nil, pos)
        node.isRoot = true
        return node
    end

    self.RemoveSelection = function(self,pos)
        self:RemoveChild(pos)
    end
end

---@param storyTable t_StoryTable
DataDefine.t_StoryNode.Create = function(storyTable)
    local storyNode = DataDefine.t_StoryNode.New()
    storyNode.isRoot = true
    storyNode:SetChildren(storyTable, storyTable.items:GetByIndex(1))
    return storyNode
end




---@class t_ReadingRecord
DataDefine.t_ReadingRecord = class("StoryEditor.t_ReadingRecord")
function DataDefine.t_ReadingRecord:__init()
    self.isReadEnd = false
    self.dialogID = 0
    self.selectionHistory = {}

    local CopyTo = function(src, dst)
        dst.isReadEnd = src.isReadEnd
        dst.dialogID = src.dialogID
        dst.selectionHistory = src.selectionHistory
    end

    self.ToTable = function(self)
        ---@type StoryEditor_Role
        local tb = {}
        CopyTo(self,tb)
        return tb
    end

    ---@param tb StoryEditor_Role
    self.FromTable = function(self,tb)
        if tb == nil then
            return self
        end
        CopyTo(tb,self)
        return self
    end
end

return DataDefine