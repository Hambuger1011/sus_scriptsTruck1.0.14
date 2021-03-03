local BaseClass = core.Class
local ImageWallLoadResources = BaseClass("ImageWallLoadResources")
local ImageWallResType = {
    Spine = 1
}

function ImageWallLoadResources:__init()
    self.useResMap = {}
    self.cacheResAsset = {}
    self.cacheWebImage = {}
end

function ImageWallLoadResources:StartLoadResources()
    local useResTable = {}

    useResTable["Assets/Bundle/ImageWall/male/body/body_SkeletonData.asset"] = ImageWallResType.Spine
    useResTable["Assets/Bundle/ImageWall/male/clothes/clothes1_SkeletonData.asset"] = ImageWallResType.Spine
    useResTable["Assets/Bundle/ImageWall/male/head/b/head1_SkeletonData.asset"] = ImageWallResType.Spine
    useResTable["Assets/Bundle/ImageWall/male/hair/hair1_SkeletonData.asset"] = ImageWallResType.Spine

    self:Release(useResTable)
    self:DownloadByList(useResTable)
    self:Begin()
end

function ImageWallLoadResources:Retain(assetName)
    if self.useResMap[assetName] then
        self.useResMap[assetName] = self.useResMap[assetName] + 1
    else
        self.useResMap[assetName] = 1
    end
end

function ImageWallLoadResources:Release(useResList)

    --计数重置
    for k,_ in pairs(self.useResMap) do
        self.useResMap[k] = 0
    end

    --使用计数
    if useResList then
        for k,resType in pairs(useResList) do
            self.useResMap[k] = resType
        end
    end

    --释放未使用
    for k,v in pairs(self.useResMap) do
        if v == 0 then
            local asset = self.cacheResAsset[k]
            if not asset then
                goto continue
            end
            self.cacheResAsset[k] = nil
            asset:Release(self.__cname)

            --销毁图片
            local refCount = self.cacheWebImage[k]
            if refCount then
                self.cacheWebImage[k] = nil
                refCount:Release()
            end
        end
        ::continue::
    end
    
    logic.ResMgr.ClearTag(logic.ResMgr.tag.ImageWall)
    logic.ResMgr.GC()
    local saveResCnt = table.count(self.cacheResAsset)
    logic.debug.LogError(string.format('[%s]保留资源:%d',self.__cname,saveResCnt))
    
end

local isALl=false;
local allsize=0;
function ImageWallLoadResources:GetAllSize()
    if(self.isBegin==nil or self.isBegin==false)then
        return 0
    end
    if(isALl==true)then
        return allsize;
    else
        return 0;
    end
    return allsize
end

function ImageWallLoadResources:DownloadByList(resTable)
    allsize = 0;
    local loadedNum = 0
    for url,resType in pairs(resTable) do
        local asset = self:Download(logic.ResMgr.type.Object,url,resType,function()
            loadedNum = loadedNum + 1
            if loadedNum == #resTable then
                logic.cs.UITipsMgr:ShowTips("形象墙资源预加载完毕");
            end
        end)
        if  asset ~= nil then
            allsize=allsize+asset:GetAllSize();
            asset:IsDone()
            logic.debug.LogWarning(url.."加载完成")
        else
            logic.debug.LogError(url.."加载失败")
        end
    end
    isALl=true
end

function ImageWallLoadResources:Download(abResType,assetName,resType,callback)
    local asset = nil
    asset = logic.ResMgr.LoadAsync(logic.ResMgr.tag.ImageWall,abResType,assetName, callback)
    if not asset then
        logic.debug.LogError(string.format('预加载失败:type=%s,filename=%s,type=%d',abResType,assetName,resType))
        return nil
    end
    asset:Retain(self.__cname)
    self.cacheResAsset[assetName] = asset
    self:Retain(assetName)
    return asset
end

function ImageWallLoadResources:Begin()
    self.isBegin = true
end

function ImageWallLoadResources:End()
    self.isBegin = false
end

--销毁
function ImageWallLoadResources:__delete()
end

return ImageWallLoadResources
