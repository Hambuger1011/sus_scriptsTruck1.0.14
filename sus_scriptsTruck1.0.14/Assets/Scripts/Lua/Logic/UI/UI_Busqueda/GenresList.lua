local BaseClass = core.Class
local GenresList = BaseClass("GenresList")

function GenresList:__init(gameObject)
    self.gameObject=gameObject;

    self.Romance = CS.DisplayUtil.GetChild(self.gameObject, "Romance"):GetComponent("UIToggle");
    self.LGBT = CS.DisplayUtil.GetChild(self.gameObject, "LGBT"):GetComponent("UIToggle");
    self.Action = CS.DisplayUtil.GetChild(self.gameObject, "Action"):GetComponent("UIToggle");
    self.Youth = CS.DisplayUtil.GetChild(self.gameObject, "Youth"):GetComponent("UIToggle");
    self.Adventure =CS.DisplayUtil.GetChild(self.gameObject, "Adventure"):GetComponent("UIToggle");
    self.Drama =CS.DisplayUtil.GetChild(self.gameObject, "Drama"):GetComponent("UIToggle");
    self.Comedy =CS.DisplayUtil.GetChild(self.gameObject, "Comedy"):GetComponent("UIToggle");
    self.Horror =CS.DisplayUtil.GetChild(self.gameObject, "Horror"):GetComponent("UIToggle");
    self.Eighteen =CS.DisplayUtil.GetChild(self.gameObject, "Eighteen"):GetComponent("UIToggle");
    self.Fantasy =CS.DisplayUtil.GetChild(self.gameObject, "Fantasy"):GetComponent("UIToggle");
    self.Suspense =CS.DisplayUtil.GetChild(self.gameObject, "Suspense"):GetComponent("UIToggle");
    self.Others =CS.DisplayUtil.GetChild(self.gameObject, "Others"):GetComponent("UIToggle");

    --按钮监听
    logic.cs.UIEventListener.AddOnClickListener(self.Romance.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.LGBT.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Action.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Youth.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Adventure.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Drama.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Comedy.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Horror.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Eighteen.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Fantasy.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Suspense.gameObject,function(data) self:UIToggleClick(data) end);
    logic.cs.UIEventListener.AddOnClickListener(self.Others.gameObject,function(data) self:UIToggleClick(data) end);


    --【总共最多三个数字】【总共最多三个数字】【总共最多三个数字】
    self.ToggleArr={};
end

function GenresList:UIToggleClick(data)
    if(self.ToggleArr==nil)then return; end
    local index=0;
    if(data and CS.XLuaHelper.is_Null(data)==false)then

        if(data.selectedObject and CS.XLuaHelper.is_Null(data.selectedObject)==false)then

            if(data.selectedObject.name=="Romance")then
                if(self.Romance.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Romance);
                else
                    index=BookType.Romance;
                end
            elseif(data.selectedObject.name=="LGBT")then
                if(self.LGBT.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.LGBT);
                else
                    index=BookType.LGBT;
                end
            elseif(data.selectedObject.name=="Action")then
                if(self.Action.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Action);
                else
                    index=BookType.Action;
                end
            elseif(data.selectedObject.name=="Youth")then
                if(self.Youth.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Youth);
                else
                    index=BookType.Youth;
                end
            elseif(data.selectedObject.name=="Adventure")then
                if(self.Adventure.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Adventure);
                else
                    index=BookType.Adventure;
                end
            elseif(data.selectedObject.name=="Drama")then
                if(self.Drama.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Drama);
                else
                    index=BookType.Drama;
                end
            elseif(data.selectedObject.name=="Comedy")then
                if(self.Comedy.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Comedy);
                else
                    index=BookType.Comedy;
                end
            elseif(data.selectedObject.name=="Horror")then

                if(self.Horror.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Horror);
                else
                    index=BookType.Horror;
                end
            elseif(data.selectedObject.name=="Eighteen")then

                if(self.Eighteen.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Eighteen);
                else
                    index=BookType.Eighteen;
                end
            elseif(data.selectedObject.name=="Fantasy")then
                if(self.Fantasy.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Fantasy);
                else
                    index=BookType.Fantasy;
                end
            elseif(data.selectedObject.name=="Suspense")then
                if(self.Suspense.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Suspense);
                else
                    index=BookType.Suspense;
                end
            elseif(data.selectedObject.name=="Others")then
                if(self.Others.isOn==false)then
                    table.removebyvalue(self.ToggleArr,BookType.Others);
                else
                    index=BookType.Others;
                end
            end

            local len=table.length(self.ToggleArr);

            if(index>0 and len<3)then
                local key= table.keyof(self.ToggleArr,index);
                if(key==nil)then
                    table.insert(self.ToggleArr,index);
                end
            end
        end
    end

    local len=table.length(self.ToggleArr);
    if(len>=3)then
        self:Interactable(false);
    else
        self:Interactable(true);
    end



    local str="";
    if(len>0)then
        for i = 1, len do
            if(i > 1)then
                str=str..","..self.ToggleArr[i];
            else
                str=str..self.ToggleArr[i];
            end
        end
    end
    GameController.BusquedaControl.m_curPage=0;
    GameController.BusquedaControl:RequestBook(1,str,nil);

end

function GenresList:Interactable(isOpen)
    if(isOpen==nil)then return; end
    self.Romance.interactable=isOpen;
    self.LGBT.interactable=isOpen;
    self.Action.interactable=isOpen;
    self.Youth.interactable=isOpen;
    self.Adventure.interactable=isOpen;
    self.Drama.interactable=isOpen;
    self.Comedy.interactable=isOpen;
    self.Horror.interactable=isOpen;
    self.Eighteen.interactable=isOpen;
    self.Fantasy.interactable=isOpen;
    self.Suspense.interactable=isOpen;
    self.Others.interactable=isOpen;

    if(isOpen==true)then return; end
    if(self.ToggleArr==nil)then return; end
    local len=table.length(self.ToggleArr);
    if(len>=3)then
        for i = 1, 3 do
            if(self.ToggleArr[i]==BookType.Romance)then
                self.Romance.interactable=true;
            elseif(self.ToggleArr[i]==BookType.LGBT)then
                self.LGBT.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Action)then
                self.Action.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Youth)then
                self.Youth.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Adventure)then
                self.Adventure.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Drama)then
                self.Drama.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Comedy)then
                self.Comedy.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Horror)then
                self.Horror.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Eighteen)then
                self.Eighteen.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Fantasy)then
                self.Fantasy.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Suspense)then
                self.Suspense.interactable=true;
            elseif(self.ToggleArr[i]==BookType.Others)then
                self.Others.interactable=true;
            end
        end
    end
end

--销毁
function GenresList:__delete()
    --if(self.BookIconImage)then
    --    logic.cs.UIEventListener.RemoveOnClickListener(self.BookIconImage.gameObject,function(data) self:BookOnclicke() end)
    --end

    if(CS.XLuaHelper.is_Null(self.gameObject)==false)then
        logic.cs.GameObject.Destroy(self.gameObject)
    end
    self.gameObject=nil;
end


return GenresList
