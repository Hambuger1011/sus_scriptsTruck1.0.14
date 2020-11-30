using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatServerData
{

}

/// <summary>
/// 猫的引导步骤
/// </summary>
public enum CatGuidEnum
{
    CatButtonGuid = 1,  //引导点击个人中心
    PersonalCenterCatEnter,//引导点击个人中心猫的入口
    CatEnterTips,       //刚进入院子的时候提示文字
    CatGetFoodTips,     //刚进入院子的时候提示你要给食物的提示文字
    CatFoodOnGuid,      //引导点击食盒
    PlaceFoodYes,       //引导点击放置食物yes按钮
    ShopOnClicke,       //引导点击商店
    ShopBuyHuangyuandian, //引导购买商店的黄圆垫
    HuangyuandianYesOnclick,//引导点击确认购买还圆垫
    PlaceHuangyuandianYes,  //引导点击确认放置黄圆垫按钮

    //当购买了黄圆垫后，在放置步骤退出来，再次进来是时候引导去装饰物界面放置

    DecorationsButtonOn,  //点击装饰物按钮
    DecorationsOnclick,   //点击装饰物
    PlaceDecorationsYes,  //装饰物放置确定

    //end


    PlaceHuangyuandian,     //引导放置黄圆垫
    PlaceDecorationsTips,   //引导放置装饰物提示
    SpwanCat,               //引导生成猫
    FeedbackTips,           //引导产出回馈说明
    CatCountdown,           //引导猫的倒计时
    GetGiftGuid,            //引导点击礼物回赠
    GetGiftGuidYes,          //引导点击领取礼物回馈按钮



    CatGuidEnd,//引导结束
}

/// <summary>
/// 这是猫咪引导显示出的UI类型
/// </summary>
public enum CatGuidUiType
{
    ShowTipsUi=1, //显示出提示框的界面
    OnClickeUi,   //规定范围可点击的界面
    ClockUi,      //显示倒计时界面
    TextPromptUi, //显示文本框提示
}
/// <summary>
/// 养猫界面
/// </summary>
public enum CatFormEnum
{
    CAT_MAIN = 1,
    CAT_ANIMAL,
    CAT_SHOP,
    CAT_DECORATION,
    CAT_STORY,
    CAT_STORY_DETAIL,
    CAT_MY_CHART,
    CAT_DETAIL,
    CAT_FOODSET,
    CAT_GIFT_FROM_ANIM,
    CAT_PUBLIC,
    CAT_SET,
    CAT_COLLECT,
    CAT_WELCOMEBACK,
    CAT_DIAMONDEXCHANGE,
    CAT_ANIMALATTRIBUTE
}

/// <summary>
/// 获得宠物信息
/// </summary>
public class Getpetinfo
{
    public int petarr_count;// 宠物总数;
    public int pages_total;//最大分页数; 
    public List<petarr> petarr;

}
public class petarr
{
    public int id;
    public string pet_name;
    public string personality;
    public int level;
    public int diamond_qty;
    public int headres;
    public string remark;
    public int lockstatus;//是否解锁宠物 1是 0否
    public string lasttime;
    public int isfit;//是否满足收养条件 1是 0否
    public int nofitseason;
}

public class Getpetgiftinfo
{
    public int feedback_count;//宠物回馈数
    public int pages_total;//最大分页数
    public List<feedback> feedback;
    

}
public class feedback
{
    public int id;
    public string pet_name;
    public int love_qty;//爱心数
    public int diamond_qty;
    public string plan_time;
    public string remark;
    public int  pid;
    public int shop_id;
    public int isprice;//是否领取奖励 1是 0否
    public int decorations_id;
}
public class Getpetfoodinfo
{
    public int shoparr_count;//商城总数
    public int pages_total;//最大分页数
    public List<shoparr> shoparr;

}
public class shoparr
{
    public int id;
    public string product_name;
    public int pay_type;//购买类型 1爱心 2钻石
    public int love_qty;//爱心数
    public int diamond_qty;//钻石数
    public int product_type;//商品类型 1小号放置物  2大号放置物 3食物
    public string res;
    public string remark;//描述
}



public class BuyItemResult
{
    public int love;//爱心
    public int diamond;//钻石
    public int shop_id ;//商店id
}
public class BagInfo
{
    public List<FoodBagItem> foodpack;
    public List<DecorationBagItem> decorationspack;
    public List<CatRobotInfo> robot;        //养猫机器人
}
public class FoodBagItem
{
    public int id;
    public int link_id;//:食物ID;
    public string option_name;//食物名称
    public int count;//数量
    public int weight;
    public int isUsed;//是否使用 1是  0否
    public string remark;
    public int shop_id;
}

public class DecorationBagItem
{
    public int id;
    public int link_id;//:装饰物ID;
    public string option_name;//装饰物
    public int isUsed;//是否使用 1是  0否
    public string remark;
    public string place; //位置
    public int shop_id;
}

public class SceneInfo
{
   public List<packarr> packarr;
   public List<firstpetarr> firstpetarr;
   public firstgift firstgift;
   public usermoney usermoney;
   public List<adoptchange> adopt_change;
   public List<CatRobotInfo> robot;      //机器人    
}
public class adoptchange
{
    public int pid;//收养宠物的Id
    public int change_status;//11宠物丢失 2新故事 3领取收益  4饥饿的 5其他
    public int intimacy;//变化前的亲密度
    public int intimacy_new;//变化后的亲密度
    public int story_new;//开启的新故事iD
    public int love;//
    public int diamond;//

}
public class packarr
{
    public int id;//:背包ID;
    public int pay_type;// 类型 1食物 2装饰物
    public int link_id;//食物ID/装饰物ID
    public int shop_id;
    public int count;
    public string place; //位置
    public int weight;
    public string remark;
}

public class firstpetarr
{
    public int pid;//宠物ID;
    public int shop_id;
    public string place;
    public int decorations_id;//装饰物ID
    public int food_id;//食物ID
    public string create_time; //来访时间
    public string plan_time;//计算结束时间
}
public class firstgift
{
    public int id;//背包ID
    public int shop_id;//商城ID
    public int link_id;//食物ID/装饰物ID
    public int count;//数量
    public int weight; //重量
    public string place;//位置
    public string remark;//描述
}
public class usermoney
{
    public int love;
    public int diamond;
    public int otherfood;
    public int usergift_status;//是否可领取回馈 1是 0否
}

//养猫机器人的数据
public class CatRobotInfo
{
    public int shop_id;         // 商城ID(机器人ID)
    public int begintime;    // 机器人生效开始时间
    public int endtime;      // 机器人生效结束时间
    public int utime;        //下次更新时间
    public int time;          //系统时间
}

public class YardInfo
{
    public yard_data yard_data;
    public yard_next_data yard_next_data;
}

public class yard_data
{
    public int id;// 院子ID;
    public string yard_name;//:院子名称
    public int diamond_qty;//:钻石数
    public int max_pet;
    public string remark;// 描述
}

public class yard_next_data
{
    public int id;// 院子ID;
    public string yard_name;//:院子名称
    public int diamond_qty;//:钻石数
    public int space_qty; //增加空间数
    public int pet_qty;// 增加宠物数
    public string remark;// 描述
}

/// <summary>
/// 获取用户已收养的宠物
/// </summary>
public class Getadoptpet
{
    public int max_pet;//当前最大可收养数
    public int gold_pet;//目标总数
    public string level_next_str;//解锁需要的等级
    public List<Getadoptpetpetarr> petarr;
}

public class Getadoptpetpetarr
{
    public int id;//宠物id
    public string pet_name;//宠物名称
    public string personality;//个性
    public string appearance;//外貌
    public int level;//稀有度
    public int intimacy;//亲密度
    public int headres;//资源
    public string remark;//描述
    public int adopt_status;//倒计时状态
    public int end_time;//倒计时结束时间
    public int stay_time;//停留秒数
    public int fid;//回馈id
    public int love;//用户可以获得爱心的个数
    public int diamond;//用户可获得的钻石数
}
public class GoodsInfo
{
    public int diamond;
    public int love;
}

/// <summary>
/// 获得指定宠物的信息
/// </summary>
public class Getuserhomepet
{
    public homepetpetarr petarr;
}

public class homepetpetarr
{
    public int id;//宠物id
    public int adoptid;//收养id
    public string pet_name;//宠物名字
    public string personality;//个性
    public string appearance;//外貌
    public string petlimit;//性格要求（2#30）
    public int level;//稀有读
    public int diamond_qty;//钻石数
    public string headres;//资源
    public string remark;//描述
    public int dif_level;//收养难度
    public int isadopt;//是否已收养 1是 0否
    public int isfullpet;//院子状态 1未满   0 已满
    public int intimacy;//亲密度
    public int storys_isused;//故事按钮状态 1可用  0不可用
    public int storys;//故事数
    public int max_story;//最大可开启的故事数
    public int visits;//来访数
    public int visit_min;//收养需要的最小来访数
    public int isfit;//是否满足收养条件 1是 0否
    public int nofitseason;//不满足原因
}

/// <summary>
/// 获得宠物故事
/// </summary>
public class Getpetstory
{
    public List<storypetarr> storypetarr;
}

public class storypetarr
{
    public int id;//收养宠物Id
    public int pid;//宠物Id
    public string pet_name;//宠物名称
    public int storys_isused;//故事按钮状态 1可用 0不可用
    public int storys;//已开启的故事数
    public int max_story;//最大可开启的故事数

}
public class Petadopt
{
    public int love;
    public int diamond;
}

public class Getpetmsg
{
    public int ispetstory;//是否有故事 1是有 0是没有

}

public class AchieveguidePrice
{
    public int love;
    public int diamond;
}


