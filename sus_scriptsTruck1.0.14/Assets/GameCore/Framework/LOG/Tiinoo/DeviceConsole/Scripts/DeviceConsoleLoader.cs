using UnityEngine;
using System.Collections;
using Framework;
using UGUI;

namespace Tiinoo.DeviceConsole
{
	public class DeviceConsoleLoader
	{

        static LogListener logListener;
        public static void Load()
		{
			string pluginName = DCConst.PLUGIN_NAME;
			string prefabPath = DCConst.DC_PREFAB_PATH_IN_RESOURCES;

            // UberLogger.Logger.Initialize();

            //GameObject go = GameObject.Find(pluginName);
            //if(go == null)
            //{
            //    GameObject prefab = Resources.Load<GameObject>(prefabPath);
            //    if (prefab == null)
            //    {
            //        LOG.Error(string.Format("[{0}] error: {1} doesn't exist!", pluginName, prefabPath));
            //        return;
            //    }
            //    go = Object.Instantiate(prefab) as GameObject;
            //    go.name = pluginName;
            //    UberLogger.Logger.AddLogger(new LogListener());
            //}
            CUIManager.Instance.OpenForm(prefabPath,true, false, true);
            UIWindowMgr.Instance.Init(DCSettings.Instance);
            UIWindowMgr.Instance.PopUpWindow(UIWindow.Id.Detector, true);
            UIWindowMgr.Instance.CloseWindow(UIWindow.Id.Console);
            if (logListener == null)
            {
                logListener = new LogListener();
                UberLogger.Logger.AddLogger(logListener);
            }
        }

//#if ENABLE_DEBUG

//        [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//        public static void LoadDebuger()
//        {
//            Tiinoo.DeviceConsole.DeviceConsoleLoader.Load();
//            Debug.Log("Debuger Start!");
//        }
//#endif
	}
}

