﻿using UnityEngine;
using System;
using System.Collections;

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
		
		public void ReceiveSharingInfo(string msg)
		{
			Debug.LogError("ReceiveSharingInfo:" + msg);
		}
	}

}


