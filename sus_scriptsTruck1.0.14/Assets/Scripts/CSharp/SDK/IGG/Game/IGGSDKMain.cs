using IGG.SDK.Utils.Common;
using UnityEngine;

namespace IGG.SDK
{
    /// <summary>
    /// 初始化全局预制件（USDK需要用到）。
    /// </summary>
    public class IGGSDKMain
    {
        public delegate void OnInitComplete();

        public static bool sHasInit = false;
        public static IGGSDKGameObject gameObject;
        public static OnInitComplete sInitDelegate;
        public static void Init(OnInitComplete initDelegate)
        {
            Debug.Log("IGGSDKMain init.");
            if (sHasInit)
            {
                Debug.Log("IGGSDKMain has inited.");
                initDelegate?.Invoke();
                return;
            }
            sInitDelegate = delegate ()
            {
                Debug.Log("IGGSDKMain InitDelegate");
                sHasInit = true;
                initDelegate?.Invoke();
            };
            // 实例化一个IGGSDKGameObject（IGGMonoBehaviour）
            GameObject go = new GameObject("IGGSDK-Global");
            gameObject = go.AddComponent<IGGSDKGameObject>();
            IGGLog.Debug("IGGSDKMain init finished.");
        }
    }
}