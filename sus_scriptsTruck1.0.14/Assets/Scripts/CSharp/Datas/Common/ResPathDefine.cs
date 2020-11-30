using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏Prefab对应指向的路径
/// </summary>
public class ResPathDefine 
{

    //------------------------------------sceneMap---------------------------------------
    public static readonly string Loading = "loading";

    public static readonly string UIRoot = "Prefabs/UI/UIRoot";
    public static readonly string UIScrollGroupItem = "Prefabs/UIPrefabs/Modules/Base/UIScrollGroupItem";//UI滚动组

    public static readonly string LoadingUIPanel = "Prefabs/UIPrefabs/Modules/LoadingUIPanel/LoadingUIPanel";       //游戏主界面UI
    public static readonly string MatchTypeUIPanel = "Prefabs/UIPrefabs/Modules/LoadingUIPanel/MatchTypeUIPanel";     //匹配类型界面
    public static readonly string MatchingUIPanel = "Prefabs/UIPrefabs/Modules/LoadingUIPanel/MatchingUIPanel";     //匹配等待界面
    public static readonly string RegisterUIPanel = "Prefabs/UIPrefabs/Modules/LoadingUIPanel/RegisterUIPanel";     //账号注册，登陆界面
    public static readonly string CreateUserPanel = "Prefabs/UIPrefabs/Modules/CreateUserPanel/CreateUserPanel";    //创建角色界面
    public static readonly string MainUIPanel = "Prefabs/UIPrefabs/Modules/MainUIPanel/MainUIPanel";       //游戏主界面UI
    public static readonly string GameMenuUIPanel = "Prefabs/UIPrefabs/Modules/MainUIPanel/GameMenuUIPanel";       //游戏主菜单
    public static readonly string HeroControlPanel = "Prefabs/UIPrefabs/Modules/HeroControlPanel/HeroControlPanel";//战斗内英雄控制界面
    public static readonly string HeroDisplayPrefab = "Prefabs/UIPrefabs/Modules/HeroDisplay/HeroDisplayGameObject";//战斗内英雄控制界面
    public static readonly string HeroSelectUIPanel = "Prefabs/UIPrefabs/Modules/HeroSelectUIPanel/HeroSelectUIPanel";//匹配前英雄选择界面
    public static readonly string HeroCollectUIPanel = "Prefabs/UIPrefabs/Modules/HeroCollectUIPanel/HeroCollectUIPanel";//英雄收集界面
    public static readonly string HeroDetailUIPanel = "Prefabs/UIPrefabs/Modules/HeroCollectUIPanel/HeroDetailUIPanel";//英雄收集界面
    public static readonly string BattleSettlementUIPanel = "Prefabs/UIPrefabs/Modules/BattleSettlementUIPanel/BattleSettlementUIPanel";
    public static readonly string MallUIPanel = "Prefabs/UIPrefabs/Modules/MallUIPanel/MallUIPanel";   //商场界面

    public static readonly string HeroHPBar = "Prefabs/UIPrefabs/Modules/HeroHPBar/HeroHPBar";//战斗内英雄控制界面



    public static readonly string GameTipsUIPanel = "Prefabs/UIPrefabs/Modules/Base/GameTipsUIPanel";   //弹出提示界面
    public static readonly string AlertUIPanel = "Prefabs/UIPrefabs/Modules/Base/AlertUIPanel";         //二次确认框
    //------------------------------------Texture-------------------------------------------
    public static readonly string SkillIconPath = "Textures/UITexture/Battle/Skill/";//技能图标路径
    public static readonly string HeroHeadTexture = "Textures/UITextures/RoleHead/";//英雄头像
    public static readonly string MallItemTexture = "Textures/UITextures/MallUIPanel/";//商场物品图标

}
