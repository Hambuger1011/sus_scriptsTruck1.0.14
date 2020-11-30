using pb;
using System;
using System.Collections;
using System.Collections.Generic;
using UGUI;
using UnityEngine;

public class MyBooksDisINSTANCE
{

    private static MyBooksDisINSTANCE mInstance;
    public static MyBooksDisINSTANCE Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new MyBooksDisINSTANCE();
            }
            return mInstance;
        }
    }

    public bool TFTo(bool bo)
    {
        return tf = bo;
    }

    public static bool tf = false;
    public static bool addTF = false;
    public static bool Candestroy = false;
    public static bool nofirstgame = false;
    public static string DestroyBookName = "";
    public static bool MyBooksFirst = false;
    private float Alltime = 120;//分钟
    private int MyBooksFirstId = -1;
    private float paytotalKey = 0, paytotalDim=0;//这个是推荐付费党的价格

    private int endtime;
    private int UseTou = 0;
    private string UseName="Gust";
    private int GameOpenUItypeNum = 0;

    /// <summary>
    /// 本地记录新手礼包已经购买了
    /// </summary>
    public void FirstGigtHadBuyF()
    {
        UserDataManager.Instance.userInfo.data.userinfo.newpackage_status = 2;
    }
    /// <summary>
    /// 这个是游戏进行时，资源不足的时候，依次打开的不同界面
    /// 1,新手界面   2，广告  3，分档推荐
    /// </summary>
    /// <returns></returns>
    public int GameOpenUItype()
    {
        GameOpenUItypeNum++;
        if (UserDataManager.Instance.userInfo != null && UserDataManager.Instance.userInfo.data != null &&
                         UserDataManager.Instance.userInfo.data.userinfo.newpackage_status == 1)
        {
            //这个是新手界面没有购买过的时候
            if (GameOpenUItypeNum>2)
            {
                GameOpenUItypeNum = 1;
            }
        }
        else
        {
            if (GameOpenUItypeNum==1)
            {
                GameOpenUItypeNum++;
            }
            if (GameOpenUItypeNum>3)
            {
                GameOpenUItypeNum = 2;
            }
        }

        return  3/*GameOpenUItypeNum*/;
    }

    #region 看广告
    public void VideoUI(int vBuyType = 2)
    {

        CUIManager.Instance.OpenForm(UIFormName.PublicNotice);
        CUIManager.Instance.GetForm<PublicNotice>(UIFormName.PublicNotice).Inite(1,vBuyType);     
    }

    #endregion

    public string GetName()
    {
        return UseName;
    }
    public void SetName(string name)
    {
        UseName = name;
    }

    /// <summary>
    /// 获取当前使用的头像ID
    /// </summary>
    /// <returns></returns>
    public int GetUseTou()
    {
        return UseTou;
    }
    /// <summary>
    /// 保存当前使用的头像
    /// </summary>
    /// <param name="usetou"></param>
    public void SetUseTou(int usetou)
    {
        UseTou = usetou;
    }
    public bool returnto()
    {
        return tf;
    }

    public  Dictionary<string, string> BookDic;
    private Dictionary<string, string> HadNewPiceList;//保存已经下载好的的图片
    private bool returnBoool = false;
    private string newfullPath;//记录保存图片的路径
    private ArrayList newgoList;
    private ArrayList bookRecommendList;
    private int NoviceGuiBookDetailsId = 0, NoviceGuiBookDetailstype=0, NoviceGuiBookMax=0;

    private ArrayList TypeArrayList, TypeTotalArrayList;
    private Dictionary<int, int> NoviceGuideInfoChoceDic;

    private ArrayList type1List;
    private t_BookDetails BookDetailsMax;
    private ArrayList paytotalTypeList;
    private int chapterID = 0;//记录当前阅读是是第几个章节
    private bool isPlaying = false;

    /// <summary>
    /// 判断是否在游戏中
    /// </summary>
    /// <returns></returns>
    public bool GetIsPlaying()
    {
        return isPlaying;
    }
    public void SetIsPlaying(bool isPlaying)
    {
        this.isPlaying = isPlaying;
    }

    /// <summary>
    /// 获得当前阅读书本的章节
    /// </summary>
    /// <returns></returns>
    public int chapterIDGet()
    {
        //LOG.Info("chapterID的数值是：" + chapterID);
        return this.chapterID;
    }
    /// <summary>
    /// 储存当前阅读书本的章节
    /// </summary>
    /// <param name="chapterID"></param>
    public void chapterIDSet(int chapterID)
    {
        this.chapterID = chapterID;
    }
    /// <summary>
    /// 得到付费商品的列表
    /// </summary>
    /// <returns></returns>
    public ArrayList paytotalTypeListGet()
    {
        if (paytotalTypeList==null)
        {
            paytotalTypeList = new ArrayList();            
        }
        return paytotalTypeList;
    }
    /// <summary>
    /// 保存付费商品的价格
    /// </summary>
    /// <param name="price"></param>
    public void paytotalTypeListSet(float price)
    {
        if (paytotalTypeList==null)
        {
            paytotalTypeList = new ArrayList();
        }

        paytotalTypeList.Add(price);
    }

    /// <summary>
    /// 获得当前KEY付费档的价格
    /// </summary>
    /// <returns></returns>
    public float paytotalGet()
    {
       return this.paytotalKey;
    }
    //这个保存KEY付费档的价格
    public void paytotalSet(float paytotal)
    {
        this.paytotalKey = paytotal;
    }

    /// <summary>
    /// 获得当前钻石付费档的价格
    /// </summary>
    /// <returns></returns>
    public float paytotakDimGet()
    {
        return this.paytotalDim;
    }
    //这个保存钻石付费档的价格
    public void paytotakDimSet(float paytotal)
    {
        this.paytotalDim = paytotal;
    }
    /// <summary>
    /// 得到新手引导结束后需转到的书本
    /// </summary>
    public int NoviceGuideToBooke()
    {
        NoviceGuiBookDetailsId++;

        if (NoviceGuiBookDetailsId==1)
        {
            //得到最喜欢的书本类型
            NoviceGuiBookDetailstype= NoviceGuideLove();

            LOG.Info("分析得出最喜爱的书本类型是："+NoviceGuiBookDetailstype);
        }
        
        t_BookDetails mBookDetail = GameDataMgr.Instance.table.GetBookDetailsById(NoviceGuiBookDetailsId);

        //Debug.Log("mBookDetail:" + mBookDetail+ "--NoviceGuiBookDetailsId:"+ NoviceGuiBookDetailsId);
        if (mBookDetail == null)
        {
            LOG.Info("遍历表中的数据结束了,应该跳转的书本id是：" + BookDetailsMax.id+"--其中该类型的最大占比是："+ NoviceGuiBookMax);
            NoviceGuiBookDetailsId = 0;//bookDetails的id
            NoviceGuiBookDetailstype = 0;//得到最喜欢的书本类型
            NoviceGuiBookMax = 0;//这本书的typeTotal最大值
            NoviceGuideInfoChoceDic.Clear();//清除字典         
            return BookDetailsMax.id;
        }

        string Type = mBookDetail.Type1;
        //LOG.Info("mBookDetail.Type1:"+Type);
        if (Type.Length==1)
        {
            //只有一个类型
            int type1 = int.Parse(Type);

            if (type1== NoviceGuiBookDetailstype)
            {
                int TypeTotal = int.Parse(mBookDetail.TypeTotal);
                if (TypeTotal > NoviceGuiBookMax)
                {
                    NoviceGuiBookMax = TypeTotal;
                    BookDetailsMax = mBookDetail;
                    NoviceGuideToBooke();
                }else
                {
                    NoviceGuideToBooke();
                }
            }
            else
            {
                NoviceGuideToBooke();
            }
        }
        else if(Type.Length >1)
        {
            string[] type1 = Type.Split(',');

            if (type1List==null)
            {
                type1List = new ArrayList();
            }
            
            for (int i=0;i<type1.Length;i++)
            {
                int type1Va = int.Parse(type1[i]);

                if (type1Va== NoviceGuiBookDetailstype)
                {
                    //该行数据中有这个类型的数据
                    string[] TypeTotalsVa = mBookDetail.TypeTotal.Split(',');
                    int TypeTotalVa = int.Parse(TypeTotalsVa[i]);

                    if (TypeTotalVa> NoviceGuiBookMax)
                    {
                        NoviceGuiBookMax = TypeTotalVa;
                        BookDetailsMax = mBookDetail;
                       // NoviceGuideToBooke();
                    }
                }

                if (i==type1.Length-1)
                {
                    NoviceGuideToBooke();
                }
            }
        }
        return BookDetailsMax.id;
    }

    /// <summary>
    /// 获得新手引导中value最大值，和key.以此来判断他最喜欢的书本类型是什么,并且返回喜爱的书本类型是什么
    /// </summary>
    public int NoviceGuideLove()
    {
        int maxValue = 0;
        int keytoMax = 0;

        foreach (KeyValuePair<int, int> kvp in NoviceGuideInfoChoceDic)
        {
            if (kvp.Value>maxValue)
            {
                keytoMax = kvp.Key;
                maxValue = kvp.Value;              
            }

            LOG.Info("字典中存储的喜爱值分析：key--"+kvp.Key+"--value:"+kvp.Value);
        }

        return keytoMax;
    }

    /// <summary>
    /// 储存新手引导书本类型的兴趣选择
    /// </summary>
    public void NoviceGuideInfoChoce(string Type,string TypeTotal)
    {
        LOG.Info("新手兴趣类型是：" + Type + "--TypeTotal内容是：" + TypeTotal);
        
        if (string.IsNullOrEmpty(Type)|| string.IsNullOrEmpty(TypeTotal))
        {
            LOG.Info("少年！你给的值咋有空值捏");
            return;
        }

        if (TypeArrayList==null)
        {
            TypeArrayList = new ArrayList();
        }
        if (TypeTotalArrayList==null)
        {
            TypeTotalArrayList = new ArrayList();
        }
        if (NoviceGuideInfoChoceDic==null)
        {
            NoviceGuideInfoChoceDic = new Dictionary<int, int>();
        }

        TypeArrayList.Clear();
        TypeTotalArrayList.Clear();

        if (Type.Length==1)
        {
            int Typevalu = int.Parse(Type);
            int TypeTotalvalu = int.Parse(TypeTotal);
            if (Typevalu == 0)
            {
                LOG.Info("有无意义值");
                return;              
            }
            TypeArrayList.Add(Typevalu);
            TypeTotalArrayList.Add(TypeTotalvalu);

            if (NoviceGuideInfoChoceDic.ContainsKey((int)TypeArrayList[0]))
            {
                //这个key已经在字典里了，对值进行相加

                foreach (KeyValuePair<int, int> kvp in NoviceGuideInfoChoceDic)
                {
                    if (kvp.Key== (int)TypeArrayList[0])
                    {
                        //找到这个类型的字典所存储的数据

                        int Dicvalue = kvp.Value;//获得字典中现在存储的值
                        int newDicvalue = Dicvalue + (int)TypeTotalArrayList[0];
                        //储存相加好的值
                        NoviceGuideInfoChoceDic[kvp.Key] = newDicvalue;
                        break;
                    }
                }

            }
            else
            {
                //这个字典中还没有这个key，保存值和key
                NoviceGuideInfoChoceDic.Add(Typevalu, TypeTotalvalu);
            }
        }
        else
        {

            string[] TypeArrayListSt = Type.Split(',');
            string[] TypeTotalArrayListSt = TypeTotal.Split(',');

            //Debug.Log("TypeArrayListSt:"+ TypeArrayListSt.Length+ "__TypeTotalArrayListSt:"+ TypeTotalArrayListSt.Length);
            for (int i=0;i<TypeArrayListSt.Length;i++)
            {
                string TypeArrayValue = TypeArrayListSt[i];
               
                TypeArrayList.Add(int.Parse(TypeArrayValue));

                string TypeTotalArray = TypeTotalArrayListSt[i];
                TypeTotalArrayList.Add( int.Parse(TypeTotalArray));
            }

            for (int i=0;i< TypeArrayList.Count;i++)
            {
                if (NoviceGuideInfoChoceDic.ContainsKey((int)TypeArrayList[i]))
                {
                    //这个key已经在字典里了，对值进行相加
                    foreach (KeyValuePair<int, int> kvp in NoviceGuideInfoChoceDic)
                    {
                        if (kvp.Key == (int)TypeArrayList[i])
                        {
                            //找到这个类型的字典所存储的数据

                            int Newkey = kvp.Key;
                            int Dicvalue = kvp.Value;//获得字典中现在存储的值
                            int newDicvalue = Dicvalue + (int)TypeTotalArrayList[0];

                           
                            //储存相加好的值(修改)
                            NoviceGuideInfoChoceDic[Newkey]= newDicvalue;
                            break;
                        }
                    }
                }else
                {
                    //这个字典中还没有这个key，保存值和key
                    NoviceGuideInfoChoceDic.Add((int)TypeArrayList[i], (int)TypeTotalArrayList[i]);
                }
            }
        }             
    }



    public ArrayList returnbookRecommendList()
    {
        if (bookRecommendList==null)
        {
            bookRecommendList = new ArrayList();
        }
        return bookRecommendList;
    }

    /// <summary>
    /// 存储推荐的书本
    /// </summary>
    /// <param name="bookid"></param>
    public void bookRecommendListAdd(int bookid)
    {
        if (bookRecommendList==null)
        {
            bookRecommendList = new ArrayList();
        }

        //Debug.Log("baochun");
        bookRecommendList.Add(bookid);
    }

    public ArrayList returNewGoList()
    {
        if (newgoList == null)
        {
            newgoList = new ArrayList();
        }
        return newgoList;
    }
    public string SetnewfullPath(string ss)
    {
        //newfullPath = ss;
        PlayerPrefs.SetString("FilePath", ss);
        return newfullPath;
    }

    public string returnNewFullPath()
    {
        newfullPath = PlayerPrefs.GetString("FilePath");
        return newfullPath;
    }
    public Dictionary<string, string> returnHadNewPiceList()
    {
        if (HadNewPiceList == null)
        {
            HadNewPiceList = new Dictionary<string, string>();
        }
        return HadNewPiceList;
    }
    public bool returnBookDic(string bookName)
    {
        if (BookDic==null)
        {
            BookDic = new Dictionary<string, string>();
        }

        if (BookDic.ContainsValue(bookName))
        {
            returnBoool = false;
        }else
        {
            BookDic.Add(bookName, bookName);
            returnBoool = true;
        }
        
        return returnBoool;
    }

    public Dictionary<string, string> ReturnTheBookDic()
    {
        if (BookDic == null)
        {
            BookDic = new Dictionary<string, string>();
        }

        return BookDic;
    }
    public void InitBookDic()
    {
        if (BookDic == null)
        {
            BookDic = new Dictionary<string, string>();
        }
    }

    public void WriteBookeNameToDic(string name)
    {
        returnBookDic(name);
    }


    public void ChackOpenCuntDown()
    {
        if (UserDataManager.Instance.UserData.KeyNum <2)
        {
          
            if (PlayerPrefs.GetInt("alltimes") == 0)
            {              
                PlayerPrefs.SetInt("alltimes", 1);
                LOG.Info("执行");          
                EventDispatcher.Dispatch(EventEnum.CheckIsCanCountDown);
            }
        }    
    }


    public void GetFreeKeyApplyEndtime(string endtime)
    {
        this.endtime = int.Parse(endtime);        
    }
    public int FreeKeyApplyEndtimReturn()
    {      
        return this.endtime;
    }

    /// <summary>
    /// 记录当前阅读的书本id
    /// </summary>
    /// <param name="bookid"></param>
    public void setMyBooksFirstId(int bookid)
    {
        MyBooksFirstId = bookid;
    }

    /// <summary>
    /// 获得当前阅读书本的id
    /// </summary>
    /// <returns></returns>
    public int getMyBooksFirstId()
    {
        return MyBooksFirstId;
    }


#region 弹幕列表信息
    
    //保存所以实例化出来是物体
    private List<GameObject> PrefbPool;

    //保存真正活跃状态的物体
    private List<GameObject> UsePrefbPool;

    //保存所以实例化出来的物体
    private List<GameObject> AllGameObjectInite;
    /// <summary>
    /// 弹幕列表的初始
    /// </summary>
    public void BarrageInit()
    {
        if (PrefbPool == null)
        {
            PrefbPool = new List<GameObject>();
            PrefbPool.Clear();
            return;
        }
        PrefbPool.Clear();
    }
    /// <summary>
    /// 获得池中闲置的物体
    /// </summary>
    /// <returns></returns>
    public GameObject GetPrefb()
    {
        GameObject go = null;
        if (PrefbPool == null)
        {
            PrefbPool = new List<GameObject>();
        }

        if (UsePrefbPool==null)
        {
            UsePrefbPool = new List<GameObject>();
        }

        if (PrefbPool.Count > 0)
        {
            //池子中有闲置的预制体
            go = PrefbPool[0];
            PrefbPool.RemoveAt(0);        
        }
        else
        {
            //池子里没有足够的预制体，就创建一个
            go=ResourceManager.Instance.LoadAssetBundleUI(UIFormName.BarrageItem);

            if (AllGameObjectInite==null)
            {
                AllGameObjectInite = new List<GameObject>();
            }
            AllGameObjectInite.Add(go);
        }

        UsePrefbPool.Add(go);
        return go;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="go"></param>
    public void SetPool(GameObject go)
    {
        PrefbPool.Add(go);
        go.SetActive(false);

        if (UsePrefbPool==null)
        {
            UsePrefbPool = new List<GameObject>();
        }

        if (UsePrefbPool.Count<=0)
        {
            return;
        }

        UsePrefbPool.Remove(go);
    }

    /// <summary>
    /// 得到当前正在使用的弹幕物体
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetNowUseGame()
    {
        if (UsePrefbPool == null)
        {
            UsePrefbPool = new List<GameObject>();
        }

        return UsePrefbPool;
    }

    /// <summary>
    /// 得到所以实例化出来的弹幕
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetAllGameInite()
    {
        if (AllGameObjectInite == null)
        {
            AllGameObjectInite = new List<GameObject>();
        }

        return AllGameObjectInite;
    }
#endregion

#region 保存弹幕开启和禁止状态

    private bool isCloseBarrage = true;
    public bool GetisCloseBarrage()
    {
        return isCloseBarrage;
    }

    public bool SetisCloseBarrage()
    {
        return isCloseBarrage = !isCloseBarrage;
    }
#endregion

}
