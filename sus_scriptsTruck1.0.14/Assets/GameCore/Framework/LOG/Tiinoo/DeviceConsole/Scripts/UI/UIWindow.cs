using UnityEngine;
using System.Collections;
//using Framework;

namespace Tiinoo.DeviceConsole
{
	public class UIWindow
	{
		public enum Id
		{
			Invalid = 0,
			Detector = 1,
			Console = 2,
			Commands = 3,
			Dashboard = 4,
		}
		
		[System.Serializable]
		public class Cfg
		{
			public UIWindow.Id id;
			public GameObject go;
            //public CUIFrame uiForm;
		}
	}
}


