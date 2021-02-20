using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// 书本内存储的信息
/// </summary>
public class BookServerData 
{

	
}



public class BookList<T>
{
    public List<T> bookarr;
}




public class BookDetailCont<T>
{
    public T bookarr;
}

public class BookChapterCont<T>
{
    public T chapterarr;
}
public class BookChapterInfo
{
    public int bookid;
    public int chapter;
    public string chapterName;
    public string dsc;
    public int open;
    public int chapterPay;
    public int payType;
    public int payAmount;
    public int rewardType;
    public int rewardAmount;
}

public class SelfBookInfo
{
    public int firstbkey;//初次免费领取钥匙的数量
    public int loginday;//可领取的奖励的第几天的
    public int dayprice_status;//7天奖励  1 已领取完 2今日已领取 3今日未领取
    public int is_luckroller;
    public int is_receive;//今天是否领取Vip
    public int read_chapter_count;//用户阅读过的章节数
    public int read_book_count;//用户阅读过的书本数
    public int unreadmsgcount;  //未读消息数
    public int unreceivemsgcount;//未领取奖励的邮件数
    public int bookadcount; //当天可看书本广告的次数
    public int activity_box_switch; //宝箱是否开启
    public FinalBookInfo final_book_info;////最后阅读数本的节点
    public List<SelfBookShelfItemInfo> favorite_book;//mybook 里面的书（即自己阅读过的书）
    public int first_recharge_switch; //首冲礼包的开关 1：开，0：关
}

public class SwitchStatus
{
    public int ad_activity_status;//活动广告  1开 ,0关
    public int ad_finish_chapter_status;//完成章节广告 1开 0关
    public int ad_mall_status;//商城广告 1开 0关
}

public class FinalBookInfo
{
    public int book_id;
    public int chapter_id;
    public int bookicon;
}

//自己书架上的书本信息
public class SelfBookShelfItemInfo
{
    public int bookid;
    public int chapterid;
    public string BookName;
    public int bookicon;
    public int ChapterCount;    //书本章节数
    public int dialogid;        //书本上次对话ID
    public int type;            //书本类型
    public int isfav;
}


//自己书架上的书本信息
public class UserBookFavInfo
{
    public int isfav;
}
//书架上的书本信息
public class BookItemsInfo
{
    public List<BookItemInfo> book_list;     //npc信息
}

public class BookItemInfo
{
    public int id;
    public int chaptercount;
    public int chapteropen;
    public string version;
    public string tag;
    public int chapter_update_num;
}

// 游戏进度保存的结果信息的结果信息
public class SaveStep
{
    public int diamond_amount;         //钻石增减量
    public int diamond_price;		   //选择消费钻石
    public int key_amount;				//钥匙增减量
    public int user_diamond;			//当前用户剩余钻石数
    public int user_key;					//当前用户剩余钥匙数
    //public personalit user_personalit;			//增加的性格指数,结点如Emotion,Temper,Charm,Passion,Loyalty
    public int chapter_end;				//本章节是否结束了
    public int show_ad;
}

public class personalit
{

}

// 用户选择某选项后的结果信息
public class UserOptionCostResultInfo
{
    public int bkey;
    public int diamond;
}


//已经付费过的对话选项列表
public class BookOptionCostCont<T>
{
    public List<T> costarr;
}

//已经付费过的对话选项
public class BookOptionCostItemInfo
{
    public int bookid;
    public int chapterid;
    public int dialogid;
    public int option;  //选项
}

//
public class HiddenEggInfo
{
    public int bkey;    //用户的钥匙数
    public int diamond; //用户的钻石数
}

public class NewUserEggState
{
    public int is_end; //1=已经领取完 0 还可领取
}

public class NewUserEggInfo
{
    public int diamond; //用户的钻石数
    public int bkey;    //用户的钥匙数
    public int add;     //新增的钻石数量
    public int is_end; //1=已经领取完 0 还可领取
}

// 用户服装或者角色扣费
public class ChoicesClothResultInfo
{
    public int user_key;
    public int user_diamond;
}



public class BookDetailInfo
{
    //public bookinfo book_info;
    public int current_chapter;        //当前在阅读的章节数
    public int finish_max_chapter;   //最大完成章节
    public int cost_max_chapter;	 //已购买的最大章节
    public int read_count;        //书本阅读量
    public int isfav;               //是否收藏
    public int book_comment_count;  //书本评论数量 ,有缓存,10分钟更新新一次
    public int book_barrage_status; //书本弹幕开放情况 ,0关闭,1开放中
    public string publish;


    //public List<BookBuyClothItemInfo> clothearr;
    //public BookUserLogInfo userlog;
    //public BookDetailItemInfo bookarr;
    //public string readcount;
}

public class bookinfo
{
    public int id;
    public string bookname;
    public int bookicon;
    public int type;
    public int chaptercount;
    public int chapteropen;
    public int opentime;
    public int endtime;
    public string chapterprice;
    public int dialogframeheight;
    public string releaseday;

    public int book_tips_status; //弹窗开关
    public string book_tips_title;  //弹窗标题
    public string book_tips_content;  //弹窗内容
}

public class BookBuyClothItemInfo
{
    public int bookid;
    public int link_id; //服装id
}

public class BookBuyRoleItemInfo
{
    public int bookid;
    public int link_id; //角色id
}

public class BookUserLogInfo
{
    public int bookid;
    public int roleid;
    public int clothid;
    public string role_name;
    public int phonesceneid;    //电话场景 1已进入 0未进入
    public int phoneroleid;     //打电话的对象
    public int chapterid;       //用户上次读取章节ID 
    public int dialogid;        //用户上次读取对话ID
    public int option;          //用户上次读取的选项
    public int isfav;           //0未收藏 1已收藏
    public List<BookNpcInfo> npc_detail;     //npc信息
}

public class BookNpcInfo
{
    public int npc_id;
    public string npc_name;     //名称
    public int npc_sex;         //
}


public class BookDetailItemInfo
{
    public int bookid;
    public string bookname;
    public int bookicon;
    public int booktype;
    public string chapterdiscription;
    public int chaptercount;
    public int isopen; // 是否开放 0开放 1不开放 2限时开放
    public string opentime;             //开放开始时间  0代表永久
    public string endtime;              //开放结束时间
    public string chapterprice;         //消耗费
    public string descriptioncolor;     //描述颜色
    public string dialogcolor;          //对话颜色
    public string enternamecolor;       //输入颜色
    public string selectioncolor;       //选项颜色
    public int DialogFrameHeight;       //对话框高度

}


public class BookBarrageCountList
{
    public List<BookBarrageCountItem> data_list;
}

public class BookBarrageCountItem
{
    public int dialog_id;//对话id
    public int barrage_count;//弹幕数量
}

public class BookBarrageInfoList
{
    public int total;       //总记录数
    public int per_page;    //每页数量
    public int current_page; //当前页
    public int page_count;   //总页数
    public List<BookBarrageItemInfo> data;
}

public class BookBarrageItemInfo
{
    public int id;//对话id
    public int create_time;//创作时间戳
    public string content;   //内容
    public string nickname;  //昵称
    public int avatar;    //头像id
    public int avatar_frame;  //外框id
    public int barrage_frame;    //弹幕框id
    public int comment_frame;  //评论框id
}

// 重置章节或书本
public class ResetBookOrChapterResultInfo
{
    public int bkey;
    public int diamond;
}


// 设置玩家进度后的回调信息
public class SetProgressResultInfo
{
    public int bkey;
    public int diamond;
    public int rewardtype;  //获得奖励的类型 1钥匙 2钻石
    public int rewardamount; //奖励的数量
}


// 获取用户指定书本已扣费的章节
public class BookCostChapterListCont<T>
{
    public List<T> costarr;
}

public class BookCostChapterItemInfo
{
    public int bookid;
    public int chapterid;
}

//评论回复列表
public class Getcommentreplay
{
    public int count;//评论总数
    public int pages_total;//最大页数
    public List<commentlists> commentlist;
}
public class commentlists
{
    public int replyid;//评论回复id
    public int commentid;//评论id
    public int uid;//回复用户id
    public string username;//回复用户昵称
    public string face;//回复用户头像
    public int to_uid;//被回复用户id
    public string to_username;//被回复用户昵称
    public string to_face;//被回复用户头像
    public string content;//评论回复内容
    public int bestests;//点赞总数
    public string ctime;//评论时间
    public int is_praise;//是否已点赞
}
//新闻评论列表
public class Getcomment
{
    public int count;//评论总数
    public int pages_total;//最大页数
    public List<commentlist> commentlist;
}

//新闻评论列表详情
public class commentlist
{
    public int commentid;//评论id
    public int uid;//用户id
    public string username;//用户昵称
    public string face;//用户头像
    public string content;//评论内容
    public int bestests;//点赞总数
    public int replies;//回复总数
    public string ctime;//评论时间
    public int is_praise;//是否已点赞  1是已点赞
    public int is_vip;//是否是vip 1是
    public replay replay;
}

//新闻评论列表回复信息
public class replay
{
    public int replyid;//评论回复id
    public int commentid;//评论id
    public int uid;//回复用户id
    public string username;//回复用户昵称
    public string face;//回复用户头像
    public int to_uid;//被回复用户id
    public string to_username;//被回复用户昵称
    public string to_face;//被回复用户头像
    //public string content;//评论回复内容
    public List<content> content;
}

public class content
{
    public string detail;
}

//获取商城免费钻石状态
public class MallAwardStatus
{
    public int finish; //是否已完成所有奖励,0未完成,1已完成
    public int diamond; //当前奖励钻石数量
    public int key; //当前奖励钥匙数量
    public int last_receive_time;
    public int countdown; //倒计时
}

//商城免费钻石奖励列表
public class MallAward
{
    public int finish; //是否已完成所有奖励,0未完成,1已完成
    public int diamond; //当前奖励钻石数量
    public int key; //当前奖励钥匙数量
    public int last_receive_time;
    public int countdown; //倒计时
    public ReceiveMallAwardResultInfo user_info; //用户信息
}

public class ReceiveMallAwardResultInfo
{
    public int bkey;
    public int diamond;
}

//商店信息列表
public class ShopListCont
{
    //钻石商品列表
    public List<ShopItemInfo> diamond_list;
    //钥匙商品列表
    public List<ShopItemInfo> key_list;
    //礼包商品列表
    public List<ShopItemInfo> package_list;
    // public List<ShopItemInfo> ticketarr;


    public ShopItemInfo GetTypeByProduct_id(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        if (diamond_list != null && diamond_list.Count>0 )
        {
            for (int i = 0; i < diamond_list.Count; i++)
            {
                if (diamond_list[i].product_id == id)
                {
                    return diamond_list[i];
                }
            }
        }

        if (key_list != null && key_list.Count > 0)
        {
            for (int i = 0; i < key_list.Count; i++)
            {
                if (key_list[i].product_id == id)
                {
                    return key_list[i];
                }
            }
        }

        if (package_list != null && package_list.Count > 0)
        {
            for (int i = 0; i < package_list.Count; i++)
            {
                if (package_list[i].product_id == id)
                {
                    return package_list[i];
                }
            }
        }
        return null;
    }



}

public class ShopItemInfo
{
    public int id;//购买过的商品
    public int type; //类型 1钥匙 2钻石 3票券 4组合包  
    public string price;
    public int key_count; //钥匙数量
    public int diamond_count; //钻石数量 
    public int total_count; //总数量(钻石+钥匙)
    // public int pricetype;   // 获得类型 0购买 1奖励
    public string product_id;  //商品名称
    public string product_name;    // 游戏产品名称
    public string discount_desc;     //打折名称
    public int icon_id;              //图标ID
    public int status;              //类型
    public int create_time;              //创建时间
    public int update_time;              //刷新时间
    public int af_code;       //AF事件编号

    public int is_recommend;//是否推荐商品 1是 0否
}

public class PackageItemInfo
{
    public int id;
    public string price;
    public int bkey_qty;    //钥匙数量
    public int diamond_qty; //钻石数量
    public int ticket_qty;  //票券数量
    public int pricetype;   // 获得类型 0购买 1奖励
    public string productName;  //商品名称
    public string GamePriceName;    // 游戏产品名称
    public string DiscountName;     //打折名称
    public int IconID;              //图标ID
}

public class EmailListCont
{
    public int sysarr_count;
    public int pages_total;
    public int unreadcount;
    public List<EmailItemInfo> sysarr;
}

public class EmailItemInfo
{
    public int msgid;
    public string title;
    public string content;
    public string createtime;
    public int status;
    public int msg_type;
    public int isprice;
    public int price_bkey;
    public int price_diamond;
    public int price_ticket;
    public int price_status;
    public string email_pic;
    public string email_url;
    public string sign_url;
}

public class Getuserpackage
{
    public List<package> package;
}
public class package
{
    public int id;
    public string price;
    public string old_price;//新手礼包原价
    public int bkey_qty;
    public int diamond_qty;
    public int tickey_qty;
    public int pricetype;
    public string productName;
    public string GamePriceName;
    public string DiscountName;
    public int IconID;
    public string activity_start;
    public string activity_end;
}

public class Getuserpaymallid
{
    public List<int> ids;
}


public class GetBookGiftBag
{
    public int count;
    public int pages_total;
    public List<giftlist> giftlist;
}

public class giftlist
{
    public int giftid;
    public string title;
    public string thumb;
    public string content;
    public int original_bkey;
    public int original_diamond;
    public int bkey_qty;
    public int diamond_qty;
    public int creamsort;
    public int begintime;
    public int endtime;
    public List<booklist> booklist;
}
public class booklist
{
    public int id;
    public string bookname;
}

public class NewInfoCont
{
    public int count;
    public int unread;
    public int pages_total;
    public List<NewInfoList> newlist;
}

public class NewInfoList
{
    public int newid;//新闻id
    public string title;
    public int send_type;//1 新闻 2.公告
    public string thumb;
    public string thumb_details;
    public string content;
    public string author;
    public string createtime;
    public int story_type;
    public string story_value;
    public int articletype;
    public int is_read;
    public int is_bestests;
    public string sign_name;//签名信息
    public string sign_url;//签名跳转
    public string button_name;//按钮名称
    public int com_total;//评论总数
}
public class ReadEmaillListCont<T>
{
    public T sysarr;
}

public class ReadEmaillList
{
    public int msgid;
    public string title;
    public string content;
    public string createtime;
    public int status;
    public int msg_type;
    public int isprice;
    public int price_bkey;
    public int price_diamond;
    public int price_ticket;
    public int price_status;
    public string email_pic;
    public string email_url;
    public string sign_name;//签名信息
    public string sign_url;//签名跳转
    public string button_name;//按钮名称
}

public class RecommendABookListCont
{
    public List<RecommendABookList> book_list;
}

public class RecommendABookList
{
    public int id;
    public string bookname;
    public string booktypename;  
}
/// <summary>
/// 生成用户充值订单
/// </summary>
public class OrderFormInfo
{
    public int paytype;     //购买类型 1钥匙 2钻石
    public string account;     //购买价格
    public int paycount;    //购买数量
    public string payment_name;     //充值方式 如google 等
    public string recharge_no;     //充值订单号
    public string notify_url;     //回调地址
    public string sign;           //华为支付签名
}


// 充值订单完成
public class OrderFormSubmitResultInfo
{
    public int bkey;
    public int diamond;
    public int ticket;
    public string google_orderid;
}

// 充值订单完成
public class GetAdsRewardResultInfo
{
    public int bkey;
    public int diamond;
}


// 清理数据后的结果
public class ClearRecordResultInfo
{
    public int bkey;
    public int diamond;
}

/// <summary>
/// 购买章节后的结果
/// </summary>
public class BuyChapterResultInfo
{
    public int bkey;
    public int diamond;
    public stepinfo step_info;
    public string pay_clothes;
    public string pay_options;
    public string pay_character;
    public string pay_hair;
    public string pay_outfit;
    public List<BookNpcInfo> npc_detail;
    public List<BookPropertyItemInfo> other_trait;
    public ReadingTaskInfo reading_task_info;
    public List<EggDialogList> egg_dialog_list;
}

public class EggDialogList
{
    //彩蛋id
    public int id;
    //活动id
    public int activity_id;
    //对话列表
    public string dialog_id;
    //资源模板(风格)
    public string resource;
}

public class ReadingTaskInfo
{
    //已阅读秒数
    public int online_time;
    //已完成级别, 0~5
    public int finish_level;
    //已领取奖品级别
    public int receive_level;
}

public class stepinfo
{
    public int id;
    public int bookid;
    public int roleid;
    public string role_name;
    public int clothid;
    public int chapterid;
    public int dialogid;
    public int isfav;
    public string dialogid_step_option;

    public int character_id;//model选择形象id ,需要选择时必填
    public int outfit_id;//model选择服装id,需要选择时必填
    public int hair_id;//model选择头发id,需要选择时必填
}


/// <summary>
/// 华为购买后的结果
/// </summary>
public class HwBuyResultInfo
{
    public int bkey;
    public int diamond;
}

public class Getbookgrade
{
    public int isgrade;
    public int gradecount;
}

// 查看用户章节留言
public class ChapterCommentCont<T>
{
    public List<T> msgarr;
    public int msgarr_count;
    public int pages_total;
}

/// <summary>
/// 章节里单个留言信息
/// </summary>
public class ChapterCommentItemInfo
{
    public int discussid;       //评论ID
    public string nickname;     //评论用户
    public string createtime;   //评论时间
    public string content;      //评论内容
    public int bestests;        //点赞数
    public int noagree;         //不认同数
    public int discusstype;     //用户的点赞状态 1已点赞 0不认同 -1 未操作
    public List<replyarrInfo> replyarr;
}
public class GetNewRecommandbook
{
    public List<RecommandbookInfo> bookarr;
}
public class RecommandbookInfo
{
    public int bookid;
    public string bookname;
    public int bookshelfid;
    public int bookicon;
    public int booktype;
    public int isopen;
    public string opentime;
    public string endtime;
}
/// <summary>
/// 评论回复信息
/// </summary>
public class CommentBackInfo
{
    public int replyid;
    public string nickname;
    public string createtime;
    public string content;
    public int bestests;
    public int noagree;
    public int discusstype;
}

public class replyarrInfo
{
    public int replyid;
    public int discussid;
    public string content;
    public string createtime;
}
// 用户章节留言返回的结果信息
public class SendCommentResultCont
{
    public ChapterCommentItemInfo msgarr;
}

/// <summary>
/// 点赞后的结果信息
/// </summary>
public class DiscussCommentResultInfo
{
    public int discussid;       //评论ID
}


public class GetUserTickInfo
{
    public int isfreeticket;    //用户当天免费抽奖状态 0未使用 1已使用
    public int user_ticket;     //用户的票券数
}

public class LuckyDrawInfo
{
    public int option;          //抽中的选项数字
    public int price_type;      //奖项类型 1钥匙 2钻石 3票券 0未中奖
    public int price_count;     //中奖数量
    public int bkey;            //用户的钥匙数
    public int diamond;         //用户的钻石数
    public int ticket;          //用户的票券数
}

/// <summary>
/// 表情互动列表数据
/// </summary>
public class EmojiMsgListInfo
{
    public int bookid;
    public int dialogid;
    public int phiz1;
    public int phiz2;
    public int phiz3;
    public int phiz4;
}

// 抽奖选项列表
public class TicketCont<T>
{
    public List<T> ticketarr;
}

//抽奖的单项数据
public class TicketItemInfo
{
    public int pid;         //配置序号
    public string pname;    //奖项名称
    public int price_type;  //中奖类型 1钥匙 2钻石3票券 0未中奖 
    public int price_count; //数量
}

//获得邀请奖励列表
public class InviteListInfo
{
    public InviteUserInfo user_info;
    public List<InviteItemInfo> inviteList;
}

public class InviteUserInfo
{
    public string codes;        //当前用户邀请码
    public int exchange_invite;  // 互换邀请数量
    public int newuser_invite;   //新用户邀请数量
}

public class InviteItemInfo
{
    public int invite_id;        //奖励ID
    public int type;            //类型1新玩家2互换
    public int rewarddiamonds;  //钻石数量
    public int rewardkey;      //钥匙数量
    public int rewardweek;      //奖励周卡 1是
    public int param;           //参数(需要满足的数量才可领取)
    public int icon;            //奖励图标
    public int is_receive;       //是否已领取当前奖励 1是
    public string taskddescribe;    //任务描述
    public int sort;                //排序

}

/// <summary>
/// 互换邀请码
/// </summary>
public class ReceiveInviteResult
{
    public int bkey;
    public int diamond;
    public int bookid;
}

//用户信息
public class ProfileData
{
    public int type;        // 类型
    public ProfileBaseInfo select;
    public ProfileDetailInfo info;
    public int guide_id;// 新手引导阶段 0:未开启 1-7:新手引导步骤N 8:结束
}

//用户性格，基础信息
public class ProfileBaseInfo
{
    public int Select1;     // 开启性格图表
    public int Select2;     // 开启性格图表
    public string RefreshInterpretation;    // 刷新请求钥匙数
}

//用户性格，详细信息
public class ProfileDetailInfo
{
    public int option_num;      //已选择次数
    public int mature;          //性格1
    public int rigorous;        //性格2
    public int reasonable;      //性格3
    public int curious;         //性格4
    public int tsundere;        //性格5
    public string trait_type;      //选项类型
    public string trait_value;     //选项值
    public int refresh_count;    //刷新需要的钥匙
}

//性格刷新后的结果
public class ProfileRefreshData
{
    public int option_num;      //已选择次数
    public int mature;          //性格1
    public int rigorous;        //性格2
    public int reasonable;      //性格3
    public int curious;         //性格4
    public int tsundere;        //性格5
    public string trait_type;      //选项类型
    public string trait_value;     //选项值
    public int refresh_count;    //刷新需要的钥匙
    public int diamond;             //钻石
    public int bkey;                // 钥匙
}

public class ProfileConfigInfo
{
    public List<ProfileItemInfo> ornamentarr;
}

//个人中心头像信息
public class ProfileItemInfo
{
    public int id;      //配置ID
    public int type;    //1头像 2头像框 3背景
    public int res;     //资源
    public int param;   //参数
    public int sort;    //排序
    public string remark;   //描述
}


//书本选项选择的结果
public class BookOptionSelectInfo
{
    public List<string> options_arr;
}

public class Getrecommandmall
{
    public List<mallarrInfo> data_list;
}
public class mallarrInfo
{
    public int id;//商品推荐二维数组
    public int type;//类型 1钥匙 2钻石 3票券 4组合包 
    public string price;//购买价格
    public int key_count;//钥匙数量
    public int diamond_count;//钻石数量
    public int total_count;  //总数量 (钻石+钥匙)
    public string product_id; //对外商店 的商品id
    public string discount_desc;     //折扣描述
    public string icon_id;
    public int count;//实际购买获得的数量
    public int is_double;// 1普通 2 双倍
}

//书本属性列表
public class BookPropertyMap
{
    public List<BookPropertyItemInfo> trait2_arr;
}

//书本属性项
public class BookPropertyItemInfo
{
    public int stamp_key;//属性 key
    public int stamp_val;   //属性的 value
   
}


/// <summary>
/// 每周更新的书本
/// </summary>
public class BooksUpdatedWeekly
{
    public int type;//类型0：展示跟主界面书架一样,类型1：每周更新数据展示
    public List<UpdatedWeeklyList> book_list;
}

public class UpdatedWeeklyList
{
    public int book_id;
    public int bookicon;
    public int start_chapter;
    public int end_chapter;
    public string update_time;
    public bool IsHead;
    public int type;//类型0：展示跟主界面书架一样,类型1：每周更新数据展示
}

public class BookNotUser
{
    public List<BookNotUserInfo> book_list;
}

public class BookNotUserInfo
{
    public int id;
    public string bookname;
    public int bookicon;
    public int chaptercount;
    public int chapteropen;
    public int read_count;
    public string search_type;
    public int final_dialog;
    public int current_dialog;


    long _tagFlags = -1;
    public long tagFlags
    {
        get
        {
            if (_tagFlags == -1)
            {
                _tagFlags = 0;
                string[] arr = search_type.Split(',');
                for (int i = 0; i < arr.Length; ++i)
                {
                    int b = 0;
                    if (int.TryParse(arr[i], out b))
                    {
                        LOG.Assert(b < 63, "tag数量过多");
                        _tagFlags = BitUtils.SetBit64Mask(_tagFlags, b, true);
                    }
                    else
                    {
                        //Debug.LogError("not int :" + arr[i]);
                    }
                }
            }
            return _tagFlags;
        }
    }
}

public class WriterIndexInfo
{
    public List<newBookInfo> newList;
    public List<HotBookInfo> hotList;
    //public List<WriterUserInfo> userInfo;
}
public class WriterIndexBass
{
    public int id;
    public string title;
    public string cover_image;
    public string tag;
    public string writer_id;
    public string read_count;
}
public class newBookInfo: WriterIndexBass
{
   
}
public class HotBookInfo: WriterIndexBass
{
   
}
public class WriterUserInfo
{
    public int diamond;
    public int bkey;
}

public class GetWriterHotBookList
{
    public int total;
    public int per_page;
    public int current_page;
    public List<WriterBookList> data;
}

public class WriterBookList
{
    public int id;
    public string title;
    public string cover_image;
    public string tag;
    public int writer_id;
}

public class BusquedaBookData
{
    public int total;
    public int per_page;
    public int current_page;
    public List<BusquedaBookInfo> data;
}

public class BusquedaBookInfo
{
    public int id;
    public string title;
    public string cover_image;
    public string writer_name;
    public string writer_id;
    public string description;
    public string tag;
    public int total_chapter_count;
    public int update_chapter_count;
    public int status;
    public int read_count;
    public int favorite_count;
    public int word_count;
    public string fail_reason;
    public string create_time;
    public string update_time;
    public int publish_time;

    long _tagFlags = -1;
    public long tagFlags
    {
        get
        {
            if(_tagFlags == -1)
            {
                _tagFlags = 0;
                string[] arr = tag.Split(',');
                for(int i = 0; i < arr.Length; ++i)
                {
                    int b = 0;
                    if(int.TryParse(arr[i], out b))
                    {
                        LOG.Assert(b < 63, "tag数量过多");
                        _tagFlags = BitUtils.SetBit64Mask(_tagFlags, b, true);
                    }
                    else
                    {
                        //Debug.LogError("not int :" + arr[i]);
                    }
                }
            }
            return _tagFlags;
        }
    }
}


public class JsonDataTable
{
    public List<JDT_Version> version;//各个部分的版本信息
    public List<JDT_Book> book;//书本当前开放章节，总章节等
    public List<JDT_Chapter> chapter;// 某书本每个章节的起始和结束ID，章节介绍等信息
    public List<JDT_Skin> skin;//    肤色描述表
    public List<JDT_ClothesPrice> clothes_price; //服装价格表
    public List<JDT_ModelPrice> model_price; //模型：头发、衣服定价表
    public List<JDT_RoleModel> role_model;//书本模型配置表
}

public class BookJDTFormSever
{
    public JDT_Version info;// 当前书本的各部分，版本信息
    public JDT_Book book_version;// 书本版本信息
    public List<JDT_Chapter> chapter_version;    //当前书本的每个章节的起始和结束ID，章节介绍等信息
    public JDT_DialogVersioin dialog_version;      //书本对话的版本
    public List<JDT_Skin> skin_version;    //皮肤描述表
    public List<JDT_ClothesPrice> clothes_price_version; //服装价格表
    public List<JDT_ModelPrice> model_price_version; //模型：头发、衣服定价表
    public List<JDT_RoleModel> role_model_version;//书本模型配置表
}

#region JDT data
public class JDT_Version
{
    public int book_id = 0;
    public int book_version = 0;
    public int chapter_version = 0;
    public int dialog_version = 0;
    public int skin_version = 0;
    public int clothes_price_version = 0;
    public int model_price_version = 0;
    public int role_model_version = 0;
    public int create_time = 0;
    public int update_time = 0;
}

public class JDT_Book
{
    public int id = 0;
    public string bookname = "";
    public string booktypename = "";
    public string type1 = "";
    public int chaptercount = 0;
    public int chapteropen = 0;
    public string bookcharactername = "";
    public string version = "";
    public int chapterrelease = 0;
    public int rolescale = 0;
    public int isNew = 0;
    public int releaseday = 0;
    
    public int book_tips_status; //弹窗开关
    public string book_tips_title;  //弹窗标题
    public string book_tips_content;  //弹窗内容

    public List<string> strCharacterNames;

    public List<string> Type1Array;

    public void Init()
    {
        if (strCharacterNames == null)
        {
            strCharacterNames = new List<string>();
            if (!string.IsNullOrEmpty(bookcharactername))
            {
                //string[] tempArr = JsonHelper.JsonToObject<string[]>(bookcharactername);
                string[] tempArr = bookcharactername.Split(',');
                for (int i = 0; i < tempArr.Length; i++)
                {
                    strCharacterNames.Add(tempArr[i]);
                }
            }
        }
        
        if (Type1Array == null )
        {
            Type1Array = new List<string>();
            if (!string.IsNullOrEmpty(type1))
            {
                string[] tempArr = type1.Split(',');
                for (int i = 0; i < tempArr.Length; i++)
                {
                    Type1Array.Add(tempArr[i]);
                }
            }
        }
    }
}

public class JDT_Chapter
{
    public int bookid = 0;
    public int chapter = 0;
    public int chapterstart = 0;
    public int chapterfinish = 0;
    public string chaptername ="";
    public string dsc ="";
    public int payamount = 0;
    public int rewardamount = 0;
}

public class JDT_DialogVersioin
{
    public string key;
    public int version;
    public string zip;
    public string path;
}

public class JDT_ChapterDialogList
{
    public int bookId;
    public int chapterId;
    public List<JDT_Dialog> dialogList;
}

public class JDT_Dialog
{
    public int bookid = 0;
    public int chapterid = 0;
    public int dialogid = 0;
    public int role_id = 0;
    public int dialog_type = 0;
    public int trigger = 0;
    public int next = 0;
    public int selection_num = 0;
    public int modelid = 0;
    public int requirement1 = 0;
    public int requirement2 = 0;
    public int requirement3 = 0;
    public int requirement4 = 0;
    public int next_1 = 0;
    public int next_2 = 0;
    public int next_3 = 0;
    public int next_4 = 0;
    public string selection_1 ="";
    public string selection_2 ="";
    public string selection_3 ="";
    public string selection_4 ="";
    public string sceneid ="";
    public int scenealpha = 0;
    public int icon =0;
    public int phiz_id =0;
    public int icon_bg =0;
    public int is_tingle =0;
    public int orientation =0;
    public int phonecall =0;
    public string tips ="";
    public int bgmid =0;
    public string sfx ="";
    public string sceneparticals ="";
    public int scenes_x = 0;
    public int barrage = 0;
    public string dialog ="";
    public int consequenceid = 0;

    private List<string> _sceneparticalsArray = new List<string>();
    public List<string> sceneparticalsArray
    {
        get
        {
            return _sceneparticalsArray;
        }
    }
    

    public void Init()
    {
        _sceneparticalsArray.Clear();
        if(!string.IsNullOrEmpty(sceneparticals))
        {
            string[] tempStr = sceneparticals.Split(',');
            int len = tempStr.Length;
            for (int i = 0; i < len; i++)
            {
                _sceneparticalsArray.Add(tempStr[i]);
            }
        }
    }
}

public class JDT_Skin
{
    public int id = 0;
    public int book_id = 0;
    public int icon_id = 0;
    public string dec ="";
}

public class JDT_ClothesPrice
{
    public int id = 0;
    public int bookid = 0;
    public int clotheid = 0;
    public int clotheprice = 0;
    public int pricetype = 0;
}

public class JDT_ModelPrice
{
    public int id = 0;
    public int book_id = 0;
    public int type = 0;
    public int item_id = 0;
    public int price = 0;
    public string description ="";
}

public class JDT_RoleModel
{
    public int id = 0;
    public int book_id = 0;
    public string character_list ="";
    public string hair_list ="";
    public string outfit_list ="";
    public string remark ="";
    
    public List<string> character_type1;
    public List<string> hair_type2;
    public List<string> outfit_type3;

    public void Init()
    {
        if (character_type1 == null)
        {
            character_type1 = new List<string>();
            if (!string.IsNullOrEmpty(character_list))
            {
                string[] tempArr = character_list.Split(',');
                for (int i = 0; i < tempArr.Length; i++)
                {
                    character_type1.Add(tempArr[i]);
                }
            }
        }
        
        if (hair_type2 == null)
        {
            hair_type2 = new List<string>();
            if (!string.IsNullOrEmpty(hair_list))
            {
                string[] tempArr =hair_list.Split(',');
                for (int i = 0; i < tempArr.Length; i++)
                {
                    hair_type2.Add(tempArr[i]);
                }
            }
        }
        
        if (outfit_type3 == null)
        {
            outfit_type3 = new List<string>();
            if (!string.IsNullOrEmpty(outfit_list))
            {
                string[] tempArr =  outfit_list.Split(',');
                for (int i = 0; i < tempArr.Length; i++)
                {
                    outfit_type3.Add(tempArr[i]);
                }
            }
        }
    }
}

/// <summary>
/// 服务端返回的书本版本-书本详情
/// </summary>
public class VersionBookDetailInfo
{
    public string key ="";
    public int version = 0;
    public JDT_Book info;
}

/// <summary>
/// 服务端返回的书本版本-章节信息
/// </summary>
public class VersionChapterInfo
{
    public string key ="";
    public int version = 0;
    public JDT_Chapter info;
}
    
/// <summary>
/// 服务端返回的书本版本-章节列表
/// </summary>
public class VersionChapterList
{
    public string key ="";
    public int version = 0;
    public List<JDT_Chapter> list;
}
    
/// <summary>
/// 服务端返回的书本版本-对话列表
/// </summary>
public class VersionDialogList
{
    public string key ="";
    public int version = 0;
    public List<JDT_Dialog> list;
}
    
/// <summary>
/// 服务端返回的书本版本-皮肤列表
/// </summary>
public class VersionSkinList
{
    public string key ="";
    public int version = 0;
    public List<JDT_Skin> list;
}
    
/// <summary>
/// 服务端返回的书本版本-服装价格列表
/// </summary>
public class VersionClothesPriceList
{
    public string key ="";
    public int version = 0;
    public List<JDT_ClothesPrice> list;
}
    
/// <summary>
/// 服务端返回的书本版本-角色形象配置列表信息
/// </summary>
public class VersionRoleModelList
{
    public string key ="";
    public int version = 0;
    public List<JDT_RoleModel> list;
}
    
/// <summary>
/// 服务端返回的书本版本-形象价格说明列表信息
/// </summary>
public class VersionModelPriceList
{
    public string key ="";
    public int version = 0;
    public List<JDT_ModelPrice> list;
}

#endregion 





