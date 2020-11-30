using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 事件定义，方面统一管理和查找
/// </summary>
public class EventDefine
{

    public static readonly string MainPanleRequire1v1RoomMatch = "MainPanleRequire1v1RoomMatch";
    public static readonly string MainPanleReply1v1RoomMatch = "MainPanleReply1v1RoomMatch";
    public static readonly string BattlePanelBackEvent      = "BattlePanelBackEvent";

    public static readonly string CreateUserErrorEvent = "CreateUserErrorEvent";

    public static readonly string BattleSettlementEvent = "BattleSettlementEvent";
    public static readonly string ClearUIFSMStack_BackToMainUIPanel = "ClearUIFSMStack_BackToMainUIPanel";

    public static readonly string UpdateGameMoneyInfo = "UpdateGameMoneyInfo";      //更新游戏钱币信息
    public static readonly string UpdateBagItemInfo = "UpdateBagItemInfo";      //背包数据有更新

}
