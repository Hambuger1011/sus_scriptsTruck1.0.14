#if UNITY_EDITOR && false
namespace GameCore
{
    using AB;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;

    public class ABDemo : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            GameDataMgr.CreateInstance();
            GameDataMgr.Instance.ResourceType = 0;
            ABSystem.ui.LoadBundleConfig((suc)=>
            {
                Debug.LogError("success");
            },(f)=>{ });
        }

        [ContextMenu("LoadFile")]
        public void LoadFile()
        {
            var cfg = ABSystem.ui.bundle.resConfig.GetConfigItemByAssetName("assets/bundle/ui/canvas_bookreading.prefab");
            List<AssetBundle> bundles = new List<AssetBundle>();
            var bundle = AssetBundle.LoadFromFile(Get(cfg.fileHashName));
            bundles.Add(bundle);
            foreach(var other in cfg.dependencyList)
            {
                cfg = ABSystem.ui.bundle.resConfig.GetConfigItemByAbName(other);
                var otherBundle = AssetBundle.LoadFromFile(Get(cfg.fileHashName));
                bundles.Add(otherBundle);
            }

            var pfb = bundle.LoadAsset<GameObject>("assets/bundle/ui/canvas_bookreading.prefab");
            GameObject.Instantiate(pfb);

            foreach(var b in bundles)
            {
                b.Unload(false);
            }
        }


        [ContextMenu("LoadWeb")]
        public void LoadWeb()
        {
            StartCoroutine(DoLoadWeb());
        }

        IEnumerator DoLoadWeb()
        {
            var cfg = ABSystem.ui.bundle.resConfig.GetConfigItemByAssetName("assets/bundle/ui/canvas_bookreading.prefab");
            List<AssetBundle> bundles = new List<AssetBundle>();

            var loadRequest = UnityWebRequestAssetBundle.GetAssetBundle(cfg.abFilePath, cfg.crc, 0);
            yield return loadRequest.SendWebRequest();
            var bundle = ((DownloadHandlerAssetBundle)loadRequest.downloadHandler).assetBundle;
            bundles.Add(bundle);
            foreach (var other in cfg.dependencyList)
            {
                cfg = ABSystem.ui.bundle.resConfig.GetConfigItemByAbName(other);

                loadRequest = UnityWebRequestAssetBundle.GetAssetBundle(cfg.abFilePath, cfg.crc, 0);
                yield return loadRequest.SendWebRequest();

                var otherBundle = ((DownloadHandlerAssetBundle)loadRequest.downloadHandler).assetBundle;
                bundles.Add(otherBundle);
            }

            var pfb = bundle.LoadAsset<GameObject>("assets/bundle/ui/canvas_bookreading.prefab");
            GameObject.Instantiate(pfb);

            foreach (var b in bundles)
            {
                b.Unload(false);
            }
        }

        static string Get(string name)
        {
            return string.Concat(AbUtility.abReadonlyPath,name);
        }

        
        static string Get2(string name)
        {
            return string.Concat(AbUtility.abReadonlyPath,name);
        }
    }
}
#endif