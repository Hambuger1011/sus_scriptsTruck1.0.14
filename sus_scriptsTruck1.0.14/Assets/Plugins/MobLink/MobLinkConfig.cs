using System;
using System.Collections;
using UnityEngine;

namespace com.moblink.unity3d
{
	[Serializable]
	public class MobLinkConfig
	{
		public string appKey;
		public string appSecret;

		public MobLinkConfig()
		{
			this.appKey = "31c0505717a48";
			this.appSecret = "034a7f8df49928d424ab589fdb856079";
		}
	}		

}
