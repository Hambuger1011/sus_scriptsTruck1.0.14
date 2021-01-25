using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class LoginData 
{

}

public class HttpInfoReturn<T>
{
    public int code;
    public string msg;
    public T data;
}
public class HttpInfoWithoutMsg<T>
{
    public int code;
    public T data;
}

public class HttpInfoWithoutData
{
    public int code;
    public string msg;
    public string data;
}

public class Getimpinfo<T>
{
    public T userinfo;
}

public class Getboxlist
{
  public List<boxarr> boxarr;
}
public class boxarr
{
    public int box_id;
    public int star_count;//需要的星星数
    public int bkey_qty;
    public int diamond_qty;
}

public class Achieveallmsgprice
{
    public int bkey;
    public int diamond;
    public int ticket;
}
public class Getusertask
{
    public int box_status;//宝箱状态 1可以打开 0不能打开
    public List<taskarr> taskarr;
    public string userstar;//用户当前的星星总数/总数
    public List<int> boxarr;//满足条件的宝箱一维数组
    public List<int> open_boxarr;//已开启的宝箱一维数组
}

public class taskarr
{
    public int id;//任务id
    public string task_name;//任务名称
    public int bkey_qty;//完成后获得的钥匙数
    public int diamond_qty;//完成后获得的钻石数
    public int star_qty;//完成后获得的星星数
    public string remark;//描述
    public int gold;//目标数
    public int finished_times;//已完成数
    public int task_status;//任务状态
}

public class Achievetaskprice
{
    public int bkey;
    public int diamond;
    public int ticket;
}

public class Getvipcard<T>
{
    public int is_buy;
    public int day;
    public int is_receive;//是否已经领取
    public T vipinfo;
}
public class Getvipcardreceive
{
    public int bkey;
    public int diamond;
}

public class vipinfo
{
    public int get_bkey_qty;//立即获得钥匙的数量
    public int get_diamond_qty;
    public int day_bkey_qty;
    public int day_diamond_qty;
    public int mallid;//商城配置ID

    public string account;//实际需要支付的金额
    public string original_account;//原价
    public int add_rate;//商品的加成比例
    public int day;
}
public class GetBulletinBoard
{
    public int end_time;//这个是公告的时间
}
public class ActiveInfo
{
    //public int isfreeticket;  //抽奖状态 0可使用 1已使用
    //public int unreadnewscount; //未读新闻数
    public int shopadcount; //当天可看商店广告的次数
    public activeadcountInfo activeadcount;//
    public int countdown;//倒计时的结束时间戳
    //public int istaskprice;//任务是否可以领取

}

public class activeadcountInfo
{
    public OneInfo one;
    public TwoInfo two;
    public ThreeInfo three;
    public FourInfo four;
    public FiveInfo five;
}
public class activeadcountInfoBasse
{
    public int is_receive;
    public int bkey;
    public int diamond;
}
public class OneInfo: activeadcountInfoBasse
{
    //public int is_receive;
    //public int bkey;
    //public int diamond;
}
public class TwoInfo: activeadcountInfoBasse
{
    //public int is_receive;
    //public int bkey;
    //public int diamond;
}
public class ThreeInfo: activeadcountInfoBasse
{
    //public int is_receive;
    //public int bkey;
    //public int diamond;
}
public class FourInfo: activeadcountInfoBasse
{
    //public int is_receive;
    //public int bkey;
    //public int diamond;
}
public class FiveInfo: activeadcountInfoBasse
{
    //public int is_receive;
    //public int bkey;
    //public int diamond;
}



public class UserInfoCont
{
    [JsonProperty("user_info")]
    public UserInfo userinfo;

    [JsonProperty("ip_info")]
    public IpAdressInfo ipinfo;
}

public class ThirdPartyLoginInfo
{
    public int code;
    public string msg;
    public string token;
}

public class MoveCodeInfoCont
{
    public int uid;
    public string account_code;  // 如有会显示具体的迁移code
    public int bkey; //钥匙数
    public int diamond;//钻石数
    public string nickname; //昵称,
    public int source ;// 注册来源 0：google 1：facebook 2：huawei 3：iggaccount(igg通行证) 4：guest游客
}

public class UserInfo
{
    public int end_time;
    public int is_receive;      //是否可领取免费钥匙(1=可以申请领取3=不可以申请领取2=正在倒计时)
    public string current_time;

    [JsonProperty("new_package_status")]
    public int newpackage_status;   //购买新手礼包状态 1可以购买，2不能购买  3暂不开放

    [JsonProperty("pay_bkey_total")]
    public string paybkeytotal;//用户充值的最大钥匙金额

    [JsonProperty("pay_diamond_total")]
    public string paydiamondtotal;//用户充值的最大钻石金额
    public string phoneimei;//服务端返回的UUID
    public int diamond;
    public int bkey;
    public int status;  //状态 0正常 1锁定 2禁用
    public string language;
    [JsonProperty("last_login_time")]
    public string lastLoginTime;//上次登录时间
    public string reg_area;
    public string email;
    public string nickname;
    public string invite_code;

    [JsonProperty("is_old")]
    public int firstplay;//是否新用户 0是  1不是
    //public int system_type;
    public int writer_guide;//创作引导, 0未引导，1已引导
    [JsonProperty("source")]
    public int type;// 代表注册来源 0:google 1:facebook 2:huawei 3:apple 4:游客
   
    public float pay_total;           //该字段表示充了多少钱，是否付费需自行判断

    [JsonProperty("avatar")]
    public int avatar;//头像id
    [JsonProperty("avatar_frame")]
    public int avatar_frame;     //头像框
    [JsonProperty("comment_frame")]
    public int comment_frame;     //评论框
    [JsonProperty("barrage_frame")]
    public int barrage_frame;     //弹幕框


    [JsonProperty("activity_login")]
    public int activity_login;//缓存首次签到数据




    // [JsonProperty("face_background")]
    // public int background;     //背景框

    [JsonProperty("is_store_score")]
    public int is_store_score;     //是否已经有过评星    0：否   1：是
    public string attention_media_award;     //第三方登录奖励
    public string third_party_award;     //第三方登录奖励
    public string third_party_num;     //第三方登录奖励数量
    public string se_move_finish;     //迁移状态  0.未迁移 1.已迁移 2.已领迁移奖励





    public string uid;
    //public int firstbkey;//初次登陆2小时免费钥匙领取数


    //public int unreadmsgcount;  //未读消息数
    public int is_vip;
    //public List<int> booklist;      //已购买的书本列表
    //public int yardid;              //院子扩建数:1-5 默认0代表未开启宠物功能
    public int free_key;
}

public class ThirdPartyReturnInfo
{
    public string token;
    public int isfirst; //是否第一次登录 1是 0否
}


public class EmailGetAwardInfo
{
    public int diamond;
    public int bkey;
    public int ticket;
}

public class FreeKeyInfo
{
    public int bKey;    //用户的钥匙数
    public int diamond; //用户的钻石数
    public int end_time; //下一次领取2小时免费钥匙的目标时间
}

public class FreeKeyApply
{
    public int end_time;
}
public class DayLoginInfo
{
    public int bKey;    //用户的钥匙数
    public int diamond; //用户的钻石数
    public int rewardType; //获取奖励的类型 1钥匙 2钻石
    public int rewardAmount; //奖励数量
}

public class VersionCont<T>
{
    public T apparr;
}

public class GameVersionInfo
{
    public string versionid;    //最新版本号
    public int qdtype; //渠道类型 1腾讯应用宝，2百度  21google应用市场
    public string remark; //版本描述
    public string updtime; //更新时间
    public string app_rul; //下载地址
    public int isold;   //是否强制更新  0否 1是
}

//服务器审核的状态
public class AuditStatusInfo
{
    public string status;
}

//版本信息
public class VersionInfo
{
    //public string time;                              //从服务器获取的当前时间
    public string host;                             //当前请求对应的服务器
    public string resource_version;                          //版本信息
    public string database_version;                          //表格版本信息
    public string resource_url;                            //大于等于1.0.41时, api与资源请求地址
    public int log_status;                 //埋点开关
    public int is_test_pay;
    public string zip;                    //公共表的配置路径
}





// IGGAppconf信息
public class AppconfInfo
{
    //维护信息
    [JsonProperty("Update")]
    public UpdateInfo updateInfo;

    //游戏协议弹窗  //强提醒 //弱提醒
    [JsonProperty("InformedConsent")]
    public InformedConsentInfo informedconsentInfo;

    //配置杂项    //评星控制   "rating":"0" Disabled  
    [JsonProperty("Misc")]
    public MiscInfo miscInfo;

    //登录框  //提审版本 //提更版本  //强更版本
    [JsonProperty("LoginBox")]
    public LoginBoxInfo loginboxInfo;

    //内容配置
    [JsonProperty("Messages")]
    public MessagesInfo messagesInfo;

    //页面新闻列表
    [JsonProperty("PageLink")]
    public PageLinkInfo pagelinkInfo;

    //【服务器】配置
    [JsonProperty("LoginServer")]
    public List<LoginServerInfo> loginserverInfo;


    /// <summary>
    /// 如果AppConf 加载失败  创建默认数据
    /// </summary>
    public void DefaultInfo()
    {
        //维护信息
        updateInfo = new UpdateInfo();
        updateInfo.DefaultInfo();
        //游戏协议弹窗  //强提醒 //弱提醒
        informedconsentInfo = new InformedConsentInfo();
        informedconsentInfo.mode = "asap";
        //配置杂项    //评星控制   "rating":"0" Disabled  
        miscInfo = new MiscInfo();
        miscInfo.rating = 0;
        //登录框  //提审版本 //提更版本  //强更版本
        loginboxInfo = new LoginBoxInfo();
        loginboxInfo.DefaultInfo();
        //内容配置
        messagesInfo = new MessagesInfo();
        messagesInfo.DefaultInfo();
        //页面新闻列表
        pagelinkInfo = new PageLinkInfo();
        pagelinkInfo._event = "https://sus.igg.com/event/list.php?game_id={0}&sso_token={1}";
        pagelinkInfo.guide = "https://sus.igg.com/news/game_guild.php?game_id={0}";
        //【服务器】配置
        loginserverInfo = new List<LoginServerInfo>();
        LoginServerInfo serverinfo = new LoginServerInfo();
        serverinfo.host = "127.0.0.1";
        serverinfo.port = "8080";
        loginserverInfo.Add(serverinfo);
    }



    public class UpdateInfo
    {
        [JsonProperty("isMaintain")]
        public MaintainInfo maintainInfo;  //是否维护   

        [JsonProperty("maintainItemNum")]
        public int maintainItemNum;        //维护奖励数量 

        [JsonProperty("googleUrl")]
        public string googleUrl;          //Google更新网址 

        [JsonProperty("appleUrl")]
        public string appleUrl;           //IOS更新网址

        //是否维护   
        public class MaintainInfo
        {
            [JsonProperty("state")]       //0 不  1 是
            public int state;

            [JsonProperty("startAt")]
            public string startAt;       //维护开始时间

            [JsonProperty("endAt")] 
            public string endAt;         //维护结束时间
        }


        /// <summary>
        /// 如果AppConf 加载失败  创建默认数据
        /// </summary>
        public void DefaultInfo()
        {
            maintainInfo = new MaintainInfo();
            maintainInfo.state = 0;
            maintainInfo.startAt = "2019-08-08 05:00";
            maintainInfo.endAt = "2019-08-08 07:35";
            maintainItemNum = 1;
            googleUrl = "market://details?id=com.igg.android.scriptsuntoldsecrets";
            appleUrl = "itms-apps://itunes.apple.com/app/id1535571424";
        }
    }

    //游戏协议弹窗
    public class InformedConsentInfo
    {
        [JsonProperty("mode")]
        public string mode;            //强提醒asap  //弱提醒
    }

    //配置杂项
    public class MiscInfo
    {
        [JsonProperty("rating")]
        public int rating;             //评星控制 Disabled      
    }

    //登录框
    public class LoginBoxInfo
    {
        [JsonProperty("nextVersion")]
        public string nextVersion;           //提审版本 

        [JsonProperty("version")]
        public string version;               //提更版本

        [JsonProperty("forceVersion")]
        public string forceVersion;          //强更版本

        [JsonProperty("showLoginPop")]
        public int showLoginPop;          //是否显示登入广告

        [JsonProperty("startTime")]
        public string startTime;             //开始时间

        [JsonProperty("endTime")]
        public string endTime;               //结束时间

        [JsonProperty("maintenanceAwardNum")]
        public int maintenanceAwardNum;         //维护奖励数量

        [JsonProperty("whiteList")]
        public List<userListInfo> whiteList;        //维护白名单列表    

        public class userListInfo
        {
            [JsonProperty("userList")]
            public string userList;               //白名单用户校验值
        }


        public List<string> MyWhiteList = new List<string>();
        public void UpdateWhiteList()
        {
            MyWhiteList.Clear();
            if (whiteList != null && whiteList.Count > 0)
            {
                for (int i = 0; i < whiteList.Count; i++)
                {
                    MyWhiteList.Add(whiteList[i].userList);
                }
            }
        }

        /// <summary>
        /// 如果AppConf 加载失败  创建默认数据
        /// </summary>
        public void DefaultInfo()
        {
            nextVersion = "1.0.0";              //提审版本 
            version = "1.0.0";                  //提更版本
            forceVersion = "1.0.0";             //强更版本
            showLoginPop = 0;                   //是否显示登入广告
            startTime = "2019-08-01 17:00:00";             //开始时间
            endTime = "2019-08-01 17:02:00";               //结束时间
            maintenanceAwardNum = 1;             //维护奖励数量
            whiteList = new List<userListInfo>();    //维护白名单列表 
            userListInfo info = new userListInfo();
            info.userList = "061a5b416e2bbaa42dc4a40fd584a2f4";
            whiteList.Add(info);
        }

    }


    //内容配置
    public class MessagesInfo
    {
        [JsonProperty("content")]
        public ContentInfo content;             //内容配置

        public class ContentInfo
        {
            [JsonProperty("maintain")]
            public string maintain;              //【游戏维护】信息

            [JsonProperty("update")]
            public string update;              //【更新】展示信息

            [JsonProperty("forceUpdate")]
            public string forceUpdate;          //【强更】展示信息

            [JsonProperty("login")]
            public string login;              //【登录】异常提示内容
        }

        /// <summary>
        /// 如果AppConf 加载失败  创建默认数据
        /// </summary>
        public void DefaultInfo()
        {
            content = new ContentInfo();
            content.maintain = "【游戏维护】信息";
            content.update = "【更新】展示信息";
            content.forceUpdate = "【强更】信息";
            content.login = "【登录】异常提示内容";
        }

    }

    //页面新闻列表
    public class PageLinkInfo
    {
        [JsonProperty("event")]
        public string _event;           //活动新闻列表

        [JsonProperty("guide")]
        public string guide;           //新手指南页面
    }

    //【服务器】配置
    public class LoginServerInfo
    {
        [JsonProperty("host")]
        public string host;           //Login Server

        [JsonProperty("port")]
        public string port;           //端口
    }
}







//系统消息
public class SystemMessage
{
    public int id;
    public int type;               //类型 0普通公告 ,1维护公告  
    public string title;
    public string sub_title;  //子标题或用户对象
    public string content; 
    public string link;                //链接
    public string link_button;         //链接按钮名
    public int from;          //来源,0来自平台,1来自igg
    public int update_time;  
}

//返回socket连接的相关信息
public class SocketInfo
{
    //public List<HostItemInfo> host_list;
    public int uid;      //用户ID
}

public class HostItemInfo
{
    public string host_ip;      //主机ip
    public string host_name;    //主机名称
    public int host_port;       //主机端口
    public int is_online;       //在线状态1在线
    public int online_num;      //线路
}

public class ShareAwardInfo
{
    public int bkey;    //用户的钥匙数
    public int diamond; //用户的钻石数
    public int ticket; //用户的票券数
}

public class HwUserInfo
{
    public string displayName;
    public string gameAuthSign;
    public int isAuth;
    public string playerId;
    public int playerLevel;
    public string ts;
    public int type;    //来自0:三个登陆按钮的界面（hw初始化登陆界面） 1：侧边栏的快捷登陆，2：:设置界面
}

//用户地址信息
public class IpAdressInfo
{
    public string ip;
    public string country_code;
}

//返回的ip状态信息
 public class IpStateInfo
{
    public string query;
    public string status;
}

public class TouristLoginInfo
{
    public string token;
}

//游戏功能开关列表
public class GameFunStateList
{
    public List<GameFunStateInfo> disjunctor;
}

public class GameFunStateInfo
{
    public int id;  //功能id
    public int state;//状态 1开放 0关闭
}

//背包列表
public class PakageInfo
{
    public List<PakageItemData> prop_list; //背包列表
}
public class PakageItemData
{
    public int id;              // 背包记录id
    public string name;         // 道具名称
    public string resources;    // 道具资源名称
    public int prop_id;         // 道具id
    public int prop_num;        // 道具数量
    public int is_read;         // 道具是否已读 1：已读 0未读
    public int expire_time;     // 道具过期时间：秒数，-1为永久
    public string describe;     // 物品描述
    public int prop_type;    // 道具类型
    public string discount;	    // 折扣率，0.90代表九折
}

/// <summary>
/// 道具信息
/// </summary>
public class PropInfo
{
    [JsonProperty("discount_list")]
    public List<PropInfoItem> discount_list; //折扣列表
}
public class PropInfoItem
{
    [JsonProperty("prop_num")]
    public int prop_num;        //道具数量

    [JsonProperty("discount")]
    public string discount;     // 折扣比例

    [JsonProperty("discount_string")]
    public string discount_string; // 折扣比例百分比字符
}
public enum PropType
{
    Outfit_Discount = 1, //装扮(折扣券)
    Choice_Discount = 2, //选项(折扣券)
    Choice_Coupon = 3, //选项[抵扣券]
    Outfit_Coupon = 4, //装扮[抵扣券]
    Key = 5,    //钥匙[抵扣券]
    MessageBird = 6, //信鸽
}