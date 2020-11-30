local BaseClass = core.Class
local BookFreeView = BaseClass("BookFreeView")

function BookFreeView:__init(gameObject)
    self.gameObject=gameObject;
    self.Titile =CS.DisplayUtil.GetChild(gameObject, "Titile"):GetComponent("Text");
    self.TimeLeftText =CS.DisplayUtil.GetChild(gameObject, "TimeLeftText"):GetComponent("Text");
    self.BookFreeBg =CS.DisplayUtil.GetChild(gameObject, "BookFreeBg");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.BookFreeBg.gameObject,function(data) self:BookFreeBgClick() end)
end

--点击打开书本
function BookFreeView:BookFreeBgClick()
    local activityForm = logic.UIMgr:GetView2(logic.uiid.UIActivityForm);
    if(activityForm==nil)then
        logic.UIMgr:Open(logic.uiid.UIActivityForm);
    end
end


function BookFreeView:SetInfo(info)

end



function BookFreeView:Countdown(_time)
    if(_time and _time>0)then
        local day =  math.modf( _time / 86400 )
        _time=math.fmod(_time, 86400);
        local hour =  math.modf( _time / 3600 )
        local minute = math.fmod( math.modf(_time / 60), 60 )
        --local second = math.fmod(_time, 60 )
        -- self.TimeLeftText.text = string.format("%02d:%02d", hour, minute)
        self.TimeLeftText.text =day.."d:"..hour.."h:"..minute.."m";
    end
end




function BookFreeView:__delete()
    self.gameObject = nil;
    self.Titile = nil;
    self.TimeLeftText = nil;
end

return BookFreeView
