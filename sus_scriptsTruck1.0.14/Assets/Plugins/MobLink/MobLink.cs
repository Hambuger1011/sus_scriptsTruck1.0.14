using UnityEngine;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace com.moblink.unity3d
{
	public class MobLink : MonoBehaviour {

        // 第一步：定义委托
		public delegate void GetMobIdHandler(string mobId, string errorInfo);
		public delegate void RestoreSceneHandler(MobLinkScene scene);

        // 第二步：创建委托对象
		private static event GetMobIdHandler onGetMobId;
		private static event RestoreSceneHandler onRestoreScene;

		private static bool isInit;
		private static MobLink _instance;
		private static MobLinkImpl moblinkUtils;
		private String mobId;
		public static string InviteCode = string.Empty;

		void Awake()
		{
			if (!isInit) 
			{
				#if UNITY_ANDROID
				moblinkUtils = new AndroidMobLinkImpl();
				#elif UNITY_IPHONE
				moblinkUtils = new iOSMobLinkImpl();
				#endif
				isInit = true;
			}

			if (_instance != null) 
			{
				DestroyObject (_instance.gameObject);
			}
			_instance = this;

			DontDestroyOnLoad(this.gameObject);
		}
		
		void Start () {
			setRestoreSceneListener (OnRestoreScene);
			Hashtable custom = new Hashtable ();
			String pathString = "LaunchScene";
			custom.Add("TestKey","TailTest");
			// MobLinkScene scene = new MobLinkScene (pathString, source, custom);
			MobLinkScene scene = new MobLinkScene(pathString, custom);
			MobLink.getMobId (scene, mobIdHandler);
		}
		
		// 创造符合委托格式的函数
		void mobIdHandler (string mobid, string errorInfo)
		{
			mobId = mobid;
			Debug.Log("[moblink-unity]Received MobId: " + mobid + ", errorInfo: " + errorInfo);
		}
		

		/*
		 * 全局的场景还原监听函数 
		 */
		protected static void OnRestoreScene(MobLinkScene scene)
		{
			Hashtable customParams = scene.customParams;

			if (customParams != null)
			{
				Debug.Log("[moblink-unity]OnRestoreScene(). path: " + scene.path +
				          ", params: " + customParams.toJson());
				string url = customParams["startPage"].ToString();
				Debug.Log("[moblink-unity]ReceiveSharingUrl=:" + url);
				ParseUrl(url, out _,out var nvc);
				string inviteCode = nvc.Get("invite_code");
				Debug.Log("[moblink-unity]inviteCode=:" + url);
				InviteCode = inviteCode;
				
			} else
			{
				Debug.Log("[moblink-unity]OnRestoreScene(). path: " + scene.path +
				          ", params: " + null);
			}
        
		}

		/// <summary>        
		/// 分析 url 字符串中的参数信息        
		/// </summary>        
		/// <param name="url">输入的 URL</param>        
		/// <param name="baseUrl">输出 URL 的基础部分</param>        
		/// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>        
		public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
		{
			if (url == null)
				throw new ArgumentNullException("url");
 
			nvc = new NameValueCollection();
			baseUrl = "";
 
			if (url == "")
				return;
 
			int questionMarkIndex = url.IndexOf('?');
 
			if (questionMarkIndex == -1)
			{
				baseUrl = url;
				return;
			}
			baseUrl = url.Substring(0, questionMarkIndex);
			if (questionMarkIndex == url.Length - 1)
				return;
			string ps = url.Substring(questionMarkIndex + 1);
 
			// 开始分析参数对
			Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
			MatchCollection mc = re.Matches(ps);
 
			foreach (Match m in mc)
			{
				nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
			}
		}

		public static void setRestoreSceneListener (com.moblink.unity3d.MobLink.RestoreSceneHandler sceneHander) {
			moblinkUtils.setRestoreSceneListener ();
			onRestoreScene += sceneHander;
		}

		public static void getMobId(MobLinkScene scene, GetMobIdHandler modIdHandler)
		{
            // 第三步：将函数名赋值给委托（函数名在Demo.cs中创建）
			onGetMobId += modIdHandler;
			moblinkUtils.GetMobId(scene);
		}

		private void _MobIdCallback (string data)
		{
            Debug.Log("[moblink-unity]_MobIdCallback: " + data);
            /** 
             * 解析出mobId
             * data为json格式，定义在UnityBridge中：
             * onResult时：{"mobid":"weR2v", "result":1, "errorMsg": ""}
             * onError时：{"mobid":"", "result":0, "errorMsg": "xxx"}
             */
            Hashtable json = (Hashtable) MiniJSON.jsonDecode(data);
			if (json ["mobid"] != null) {
				string mobId = json ["mobid"].ToString ();
				// 第四步：调用委托实例，执行方法
				onGetMobId (mobId, null);

			} else if (json ["errorMsg"] != null) {
				string errorInfo = json["errorMsg"].ToString();
				// 第四步：调用委托实例，执行方法
				onGetMobId (null, errorInfo);
			}
			onGetMobId = null;  
		}
			
		private void _RestoreCallBack (string data)
		{
            Debug.Log ("[moblink-unity]_RestoreCallBack: " + data);
			Hashtable res = (Hashtable) MiniJSON.jsonDecode(data);
			if (res == null || res.Count <= 0) 
			{
				return;
			}

			string path = res ["path"].ToString();
			Hashtable customParams = (Hashtable)res ["params"];

			MobLinkScene scene = new MobLinkScene (path, customParams);
			onRestoreScene (scene);
		}
	}

}


