using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;

public class CatGuidModle  {

}

#region 这里是猫的引导模块

public abstract class State
{
    public abstract void CatGuidReturn(ModuleTransfer ModuleTransfer);
    public abstract int GetUiType();
    public abstract Vector3 GetVector3();
    public abstract int SetTouchAreaW();
    public abstract int SetTouchAreaH();
    public abstract string ShowConter();
}

public class ModuleTransfer
{
   private State content;

    private int _CatGuidEnum;
    public int CatGuidEnum
    {
        get
        {
           return _CatGuidEnum;
        }
        set
        {
            _CatGuidEnum = value;
        }
    }

   
   public ModuleTransfer()
   {
        content = new CatButtonGuid();
   }

    public void SetContent(State State)
    {
        content = State;       
    }

    public void GoToModule()
    {      
        content.CatGuidReturn(this);
    }

    /// <summary>
    /// 获取UI显示的类型  1 文本  2 空间可点击指引
    /// </summary>
    /// <returns></returns>
    public int GetUiType()
    {
        return content.GetUiType();
    }

    /// <summary>
    /// 返回设置好的中心图片位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetVector3()
    {
        return content.GetVector3();
    }

    /// <summary>
    /// 返回能点击的中间图片的宽度
    /// </summary>
    /// <returns></returns>
    public int GetTouchAreaW()
    {
        return content.SetTouchAreaW();
    }

    /// <summary>
    /// 返回能点击的中间图片的高度
    /// </summary>
    /// <returns></returns>
    public int GetTouchAreaH()
    {
        return content.SetTouchAreaH();
    }

    public string GetContern()
    {
        return content.ShowConter();
    }
}


#region 这个是引导点击个人中心按钮
/// <summary>
/// 这个是引导解开猫的按钮
/// </summary>
public class CatButtonGuid : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;

    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.CatButtonGuid)
        {
            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x /*+ 379*/, UserDataManager.Instance.GuidPos.y /*+ 189*/, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(43,16,0);

            TouchAreaW = 150;
            TouchAreaH = 150;
            Content = GameDataMgr.Instance.table.GetLocalizationById(252) /*"恭喜，你解锁了猫咪院子的功能，让我们来体验一下吧！"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new PersonalCenterCatButtonGuid());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion


#region 这个是引导点击个人中心里面猫入口的按钮
/// <summary>
/// 这个是引导解开猫的按钮
/// </summary>
public class PersonalCenterCatButtonGuid : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;

    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.PersonalCenterCatEnter)
        {
            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x /*+ 379*/, UserDataManager.Instance.GuidPos.y /*+ 189*/, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(43, 16, 0);

            TouchAreaW = 250;
            TouchAreaH = 100;
            Content = GameDataMgr.Instance.table.GetLocalizationById(252) /*"恭喜，你解锁了猫咪院子的功能，让我们来体验一下吧！"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new CatTipsEnter());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion


#region 当进入猫咪院子时，弹出对话框

/// <summary>
/// 当进入猫咪院子时，弹出对话框
/// </summary>
public class CatTipsEnter : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.CatEnterTips)
        {
            //Debug.Log("13");
            UiType = (int)CatGuidUiType.ShowTipsUi;
            vect = new Vector3(2, 2, 2);
            TouchAreaW = 200;
            TouchAreaH = 200;

            Content = GameDataMgr.Instance.table.GetLocalizationById(253) /*"喵，喵，喵，亲爱的主人，欢迎来到我们的院子，这里是我们猫咪娱乐的天堂，快来陪我们一起玩耍吧！"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new CatFoodOnGuid());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion

#region 引导点击食盒

/// <summary>
/// 引导点击食盒
/// </summary>
public class CatFoodOnGuid : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.CatFoodOnGuid)
        {

            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(75, -24, 0);

            TouchAreaW = 150;
            TouchAreaH = 150;
            Content = GameDataMgr.Instance.table.GetLocalizationById(254) /*"主人、点击我的食盒，给我放一些食物吧，我饿了！"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new PlaceFoodYes());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion

#region 引导点击放置食盒 Yes 按钮

/// <summary>
/// 引导点击放置食盒 Yes 按钮
/// </summary>
public class PlaceFoodYes : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.PlaceFoodYes)
        {

            #region 中断后修补的步骤

            if (CUIManager.Instance.GetForm<CatFoodSetForm>(UIFormName.CatFoodSetForm)==null)
            {
                EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            }

            #endregion

            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置

            UserDataManager.Instance.GuidFingerPos = new Vector3(75,14, 0);

            UserDataManager.Instance.GuidFingerEFPos = new Vector3(0,38,0);

            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y-5, 1);
            TouchAreaW = 221;
            TouchAreaH = 114;
            Content = GameDataMgr.Instance.table.GetLocalizationById(255) /*"点击确实放置食物"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new ShopOnclick());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion

#region  引导点击商店按钮
public class ShopOnclick : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.ShopOnClicke)
        {

            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(75, 16, 0);


            TouchAreaW = 172;
            TouchAreaH = 190;
            Content = GameDataMgr.Instance.table.GetLocalizationById(256)/* "主人、这个是商店，里面有很多有趣的东西，我们进去看看吧！"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new BuyHuangyuandian());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}
#endregion

#region 引导购买黄圆垫

public class BuyHuangyuandian : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.ShopBuyHuangyuandian)
        {

            #region 中断后修补的步骤

            if (CUIManager.Instance.GetForm<CatShopForm>(UIFormName.CatShop) == null)
            {
                EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            }

            #endregion


            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y+109, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(218, -30, 0);

            UserDataManager.Instance.GuidFingerEFPos = new Vector3(0, -58, 0);


            TouchAreaW = 284;
            TouchAreaH = 346;
            Content = GameDataMgr.Instance.table.GetLocalizationById(257) /*"主人、我好喜欢这个，给我买一个吧！"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new BuyHuangyuandianYes());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 引导点击确认购买还圆垫

public class BuyHuangyuandianYes : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.HuangyuandianYesOnclick)
        {

            #region 中断后修补的步骤

            if (CUIManager.Instance.GetForm<CatShopForm>(UIFormName.CatShop) == null)
            {
                EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            }

            if (CUIManager.Instance.GetForm<CatPublicForm>(UIFormName.CatPublicForm) == null)
            {
                UserDataManager.Instance.HuangyuandianYesOnclickRepair = true;
                //EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            }

            #endregion


            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(70, 14, 0);
            UserDataManager.Instance.GuidFingerEFPos = new Vector3(0, 38, 0);

            TouchAreaW = 221;
            TouchAreaH = 114;
            Content = GameDataMgr.Instance.table.GetLocalizationById(258) /*"点击确认购买"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new PlaceHuangyuandianYes());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 引导点击确认放置黄圆垫

public class PlaceHuangyuandianYes : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.PlaceHuangyuandianYes)
        {

            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);
            TouchAreaW = 221;
            TouchAreaH = 114;
            Content = GameDataMgr.Instance.table.GetLocalizationById(259) /*"点击确认放置"*/;
        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new PlaceHuangyuandian());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 引导放置黄圆垫

public class PlaceHuangyuandian : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.PlaceHuangyuandian)
        {
            #region 中断后修补的步骤

            if (CUIManager.Instance.GetForm<CatShopForm>(UIFormName.CatShop) == null)
            {
                EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            }
         
            #endregion


            UiType = (int)CatGuidUiType.OnClickeUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(200,2, 0);

            TouchAreaW = 404;
            TouchAreaH = 200;
            Content = GameDataMgr.Instance.table.GetLocalizationById(260)/* "主人，你把我放在这里吧！我喜欢在这里玩耍。"*/;
        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new PlaceDecorationsTips());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 引导放置装饰物提示

public class PlaceDecorationsTips : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.PlaceDecorationsTips)
        {

            #region 中断后修补的步骤

            EventDispatcher.Dispatch(EventEnum.CatGuidRepair);

            #endregion


            UiType = (int)CatGuidUiType.TextPromptUi;
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置

            UserDataManager.Instance.GuidFingerPos= new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y + 150, 1);

            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y+150, 1);
            TouchAreaW = 300;
            TouchAreaH = 200;
            Content = GameDataMgr.Instance.table.GetLocalizationById(261) /*"主人，你稍等片刻，我要闪亮登场了！"*/;
        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new SpwanCat());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 引导生成猫

public class SpwanCat : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.SpwanCat)
        {
            #region 中断后修补的步骤

            EventDispatcher.Dispatch(EventEnum.CatGuidRepair);

            #endregion

            UiType = (int)CatGuidUiType.TextPromptUi;
            EventDispatcher.Dispatch(EventEnum.GuidSpwanCat);//生成引导默认的猫
         
            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y + 150, 1);

            TouchAreaW = 300;
            TouchAreaH = 200;
            Content = GameDataMgr.Instance.table.GetLocalizationById(262) /*"谢谢主人给我食物和舒适的毯子，我好喜欢你，我要给你一些礼物作为回馈"*/;
        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new CatCountdown());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 引导猫的倒计时

public class CatCountdown : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;
    private int TimeAll = 10;
    private int mTimeQue = 0;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.CatCountdown)
        {

            #region 中断后修补的步骤

            EventDispatcher.Dispatch(EventEnum.CatGuidRepair);

            #endregion

            UiType = (int)CatGuidUiType.ClockUi;

            TimeAll = 10;
            EventDispatcher.Dispatch(EventEnum.ClockTime, TimeAll);//获得需要点击的位置

            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置
            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);

            UserDataManager.Instance.GuidFingerPos = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y + 150, 1);


            TouchAreaW = 300;
            TouchAreaH = 200;
            Content = "";

            mTimeQue = CTimerManager.Instance.AddTimer(1000, -1, CountDown);
        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new GetGiftGuid());
            ModuleTransfer.GoToModule();
        }
    }

    private void CountDown(int vQue)
    {
        TimeAll--;
        EventDispatcher.Dispatch(EventEnum.ClockTime, TimeAll);//获得需要点击的位置

        if (TimeAll<=0)
        {
            CTimerManager.Instance.RemoveTimer(mTimeQue);
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 引导点击礼物回赠

public class GetGiftGuid : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;
    private int TimeAll = 10;
    private int mTimeQue = 0;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.GetGiftGuid)
        {
            #region 中断后修补的步骤

            EventDispatcher.Dispatch(EventEnum.CatGuidRepair);

            #endregion

            UiType = (int)CatGuidUiType.OnClickeUi;

            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置

            EventDispatcher.Dispatch(EventEnum.DestroyGuidCat);//销毁临时生成的猫


            UserDataManager.Instance.GuidFingerPos = new Vector3(-71.5f, -297.5f,0);
            UserDataManager.Instance.GuidFingerEFPos = new Vector3(0, 38, 0);

            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);
            TouchAreaW = 150;
            TouchAreaH = 150;
            Content = GameDataMgr.Instance.table.GetLocalizationById(263) /*"主人、我已经把礼物放在这里了，你查看并接收一下吧！"*/;

           
        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new GetGiftGuidYes());
            ModuleTransfer.GoToModule();
        }
    }

    private void CountDown(int vQue)
    {
        TimeAll--;
        EventDispatcher.Dispatch(EventEnum.ClockTime, TimeAll);//获得需要点击的位置

        if (TimeAll <= 0)
        {
            CTimerManager.Instance.RemoveTimer(mTimeQue);
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 引导点击领取礼物回馈按钮

public class GetGiftGuidYes : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;
    private int TimeAll = 10;
    private int mTimeQue = 0;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.GetGiftGuidYes)
        {

            #region 中断后修补的步骤

            if (CUIManager.Instance.GetForm<CatGiftFromAnimalFrom>(UIFormName.CatGiftFromAnimalForm) == null)
            {
                CUIManager.Instance.OpenForm(UIFormName.CatGiftFromAnimalForm);

                EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            }

            #endregion

            UiType = (int)CatGuidUiType.OnClickeUi;

            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置

          
            UserDataManager.Instance.GuidFingerPos = new Vector3(128, 18, 0);

            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);
            TouchAreaW = 257;
            TouchAreaH = 100;
            Content = GameDataMgr.Instance.table.GetLocalizationById(264)/* "主人领取礼物吧！"*/;


        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new DecorationsButtonOn());
            ModuleTransfer.GoToModule();
        }
    }

    private void CountDown(int vQue)
    {
        TimeAll--;
        EventDispatcher.Dispatch(EventEnum.ClockTime, TimeAll);//获得需要点击的位置

        if (TimeAll <= 0)
        {
            CTimerManager.Instance.RemoveTimer(mTimeQue);
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion 

#region 当购买了黄圆垫后，在放置步骤退出来，再次进来是时候引导去装饰物界面放置

#region 引导点击装饰物按钮
public class DecorationsButtonOn : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;
    private int TimeAll = 10;
    private int mTimeQue = 0;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.DecorationsButtonOn)
        {

            #region 中断后修补的步骤

            //if (CUIManager.Instance.GetForm<CatGiftFromAnimalFrom>(UIFormName.CatGiftFromAnimalForm) == null)
            //{
            //    CUIManager.Instance.OpenForm(UIFormName.CatGiftFromAnimalForm);

            //    EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            //}

            #endregion


            UiType = (int)CatGuidUiType.OnClickeUi;

            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置


            UserDataManager.Instance.GuidFingerPos = new Vector3(1, -3, 0);

            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);
            TouchAreaW = 150;
            TouchAreaH = 150;
            Content = GameDataMgr.Instance.table.GetLocalizationById(265) /*"主人进去放置装饰物吧！"*/;
            
        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new DecorationsOnclick());
            ModuleTransfer.GoToModule();
        }
    }

    private void CountDown(int vQue)
    {
        TimeAll--;
        EventDispatcher.Dispatch(EventEnum.ClockTime, TimeAll);//获得需要点击的位置

        if (TimeAll <= 0)
        {
            CTimerManager.Instance.RemoveTimer(mTimeQue);
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}
#endregion

#region 点击装饰物
public class DecorationsOnclick : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;
    private int TimeAll = 10;
    private int mTimeQue = 0;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.DecorationsOnclick)
        {

          
            #region 中断后修补的步骤

            if (CUIManager.Instance.GetForm<CatDecorationForm>(UIFormName.CatDecorations) == null)
            {
                CUIManager.Instance.OpenForm(UIFormName.CatDecorations);

                //EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            }

            #endregion


            UiType = (int)CatGuidUiType.OnClickeUi;

            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置


            UserDataManager.Instance.GuidFingerPos = new Vector3(166,239, 0);

            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);
            TouchAreaW = 335;
            TouchAreaH = 113;
            Content = GameDataMgr.Instance.table.GetLocalizationById(266) /*"主人去放置装饰物吧！"*/;

        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new PlaceDecorationsYes());
            ModuleTransfer.GoToModule();
        }
    }

    private void CountDown(int vQue)
    {
        TimeAll--;
        EventDispatcher.Dispatch(EventEnum.ClockTime, TimeAll);//获得需要点击的位置

        if (TimeAll <= 0)
        {
            CTimerManager.Instance.RemoveTimer(mTimeQue);
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}
#endregion

#region 装饰物放置确定
public class PlaceDecorationsYes : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;
    private int TimeAll = 10;
    private int mTimeQue = 0;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.PlaceDecorationsYes)
        {


            #region 中断后修补的步骤

            if (CUIManager.Instance.GetForm<CatDecorationForm>(UIFormName.CatDecorations) == null)
            {
                CUIManager.Instance.OpenForm(UIFormName.CatDecorations);
                //EventDispatcher.Dispatch(EventEnum.CatGuidRepair);
            }

            #endregion


            UiType = (int)CatGuidUiType.OnClickeUi;

            EventDispatcher.Dispatch(EventEnum.SetGuidPos);//获得需要点击的位置


            UserDataManager.Instance.GuidFingerPos = new Vector3(61,1, 0);

            vect = new Vector3(UserDataManager.Instance.GuidPos.x, UserDataManager.Instance.GuidPos.y, 1);
            TouchAreaW = 221;
            TouchAreaH = 114;
            Content = GameDataMgr.Instance.table.GetLocalizationById(267)/* "确认放置装饰物！"*/;

        }
        else
        {
            //上面的类 一定要  New 了这个类

            ModuleTransfer.SetContent(new CatGetFoodTips());
            ModuleTransfer.GoToModule();
        }
    }

    private void CountDown(int vQue)
    {
        TimeAll--;
        EventDispatcher.Dispatch(EventEnum.ClockTime, TimeAll);//获得需要点击的位置

        if (TimeAll <= 0)
        {
            CTimerManager.Instance.RemoveTimer(mTimeQue);
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}
#endregion


#endregion

#region 刚进入院子的时候提示你要给食物的提示文字

/// <summary>
/// 当进入猫咪院子时，弹出对话框
/// </summary>
public class CatGetFoodTips : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.CatGetFoodTips)
        {
            //Debug.Log("13");
            UiType = (int)CatGuidUiType.ShowTipsUi;
            vect = new Vector3(2, 2, 2);
            TouchAreaW = 200;
            TouchAreaH = 200;

            Content = GameDataMgr.Instance.table.GetLocalizationById(268) /*"主人，你想和我们一起玩耍吗？那需要给我们带点食物和礼物，这样我们才会更喜欢你！"*/;
        }
        else
        {
            ModuleTransfer.SetContent(new FeedbackTips());
            ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion


#region 猫的礼物回馈获得说明

/// <summary>
/// 
/// </summary>
public class FeedbackTips : State
{
    private int UiType;
    private Vector3 vect;
    private int TouchAreaW, TouchAreaH;
    private string Content;


    public override void CatGuidReturn(ModuleTransfer ModuleTransfer)
    {
        if (ModuleTransfer.CatGuidEnum == (int)CatGuidEnum.FeedbackTips)
        {

            EventDispatcher.Dispatch(EventEnum.GuidSpwanCat);//生成引导默认的猫

            //Debug.Log("13");
            UiType = (int)CatGuidUiType.ShowTipsUi;
            vect = new Vector3(2, 2, 2);
            TouchAreaW = 200;
            TouchAreaH = 200;

            Content = GameDataMgr.Instance.table.GetLocalizationById(270) /*"喵，喵，喵，亲爱的主人，欢迎来到我们的院子，这里是我们猫咪娱乐的天堂，快来陪我们一起玩耍吧！"*/;
        }
        else
        {
            //ModuleTransfer.SetContent(new CatFoodOnGuid());
            //ModuleTransfer.GoToModule();
        }
    }

    //这里返回界面的类型  1 文本  2 空间可点击指引
    public override int GetUiType()
    {
        return UiType;
    }

    //返回空间指引的坐标
    public override Vector3 GetVector3()
    {
        return vect;
    }

    public override int SetTouchAreaH()
    {
        return TouchAreaH;
    }

    public override int SetTouchAreaW()
    {
        return TouchAreaW;
    }

    public override string ShowConter()
    {
        return Content;

    }
}

#endregion




#endregion
