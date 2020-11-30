using System.Collections;
using System.Collections.Generic;
using UnityEngine;



#region  全局枚举

public enum EnumUIStyle
{
    //空
    Null =-1,
    //不处理
    None = 0,
    //普通UI
    CommonUI = 1,
    //二级UI
    TwoUI = 2,
    //弹窗UI
    PopWindowUI = 3,
    //剧情中
    StoryUI = 4,
    //特殊界面
    Special = 5,
}

//适配
public enum EnumAdaptation
{
    //顶部和底部
    TopAndBottom,
    //顶部
    Top,
    //底部
    Bottom,
}


//重新登录 方式
public enum EnumReLogin
{
    //空
    None,
    //踢人维护
    KickMaintain,
    //切换账号
    SwitchAccount,
    //登录界面
    LoginForm,
}






#endregion















public class Defines : MonoBehaviour
{
   
}
