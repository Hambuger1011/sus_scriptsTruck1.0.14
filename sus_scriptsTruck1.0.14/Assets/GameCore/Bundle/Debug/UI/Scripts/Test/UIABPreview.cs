//using Object= UnityEngine.Object;

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using Common;
//using Framework;
//using DrawIn;
//using UnityEngine;
//using UnityEngine.UI;
//using UGUI;

//public class UIABPreview : MonoBehaviour {

//    public Button btnClose;
//    public GameObject prefabButton;
//    public Transform rootTrans;

//    void Awake()
//    {
//        CUIManager.Instance.CloseForm(CUIID.Canvas_Test);
//        btnClose.onClick.AddListener(() =>
//        {
//            this.GetComponent<CUIFrame>().Close();
//        });
//        prefabButton.SetActiveEx(false);



//        int idx = 0;
//        CreateButton(ref idx, "Assets/Bundle/Prefabs/Effect/Effects/fx_BulletTimeBlack_01.prefab",(url)=>
//        {
//            LoadAbImme(url);
//            var mr = go.transform.Find("Quad").GetComponent<MeshRenderer>();
//            var mat = mr.materials[0];
//            LOG.Error(mat.shader + " " + mat.GetTexture("_MainTex")+" "+mat.shader.isSupported);
//        });
//        foreach (var asset in ABConfiguration.Instance.assetsMap)
//        {
//            if (asset.Value.name.StartsWith("assets/bundle/prefabs/character/"))
//            {
//                string url = asset.Value.name;
//                CreateButton(ref idx,url,LoadAbAsync);
//            }
//        }
//    }

//    void CreateButton(ref int idx, string url,Action<string> callback)
//    {
//        GameObject go = GameObject.Instantiate(prefabButton);
//        go.transform.SetParent(prefabButton.transform.parent, false);
//        go.SetActiveEx(true);
//        go.GetComponentInChildren<Text>().text = string.Format("{0}.{1}", ++idx, Path.GetFileNameWithoutExtension(url));
//        var toggle = go.GetComponent<Toggle>();
//        toggle.isOn = false;
//        toggle.onValueChanged.AddListener((bIsSelected) =>
//        {
//            if (bIsSelected)
//            {
//                callback(url);
//            }
//        });
//    }


//    void OnDestroy()
//    {
//        if(mAsset.handle != null)
//        {
//            mAsset.handle.Release(this);
//        }
//    }

//    PoolItemHandle<CAsset> mAsset;
    
//    GameObject go;
//    void LoadAbAsync(string strAssetName)
//    {
//        ABMgr.Instance.LoadSync(enResTag.eNone,enResType.eObject,strAssetName, (asset)=>
//        {
//            if (mAsset.handle != null)
//            {
//                mAsset.handle.Release(this);
//                Destroy(go);
//            }
//            mAsset = new PoolItemHandle<CAsset>(asset);
//            mAsset.handle.Retain(this);
//            var obj = asset.resObject;
//            if(obj is GameObject)
//            {
//                go = asset.Instantiate();
//                go.SetLayer("UI");
//                go.transform.SetParent(rootTrans, false);
//                go.transform.localPosition = Vector3.zero;
//                go.transform.localRotation = Quaternion.identity;
//                go.transform.localScale = Vector3.one;
//            }
//        });
//    }

//    void LoadAbImme(string strAssetName)
//    {
//        if (mAsset.handle != null)
//        {
//            mAsset.handle.Release(this);
//            Destroy(go);
//        }
//        mAsset = new PoolItemHandle<CAsset>(ABMgr.Instance.LoadImme(enResTag.eNone,enResType.eObject,strAssetName));
//        {
//            mAsset.handle.Retain(this);
//            var obj = mAsset.handle.resObject;
//            if (obj is GameObject)
//            {
//                go = mAsset.handle.Instantiate() as GameObject;
//                go.SetLayer("UI");
//                go.transform.SetParent(rootTrans, false);
//                go.transform.localPosition = Vector3.zero;
//                go.transform.localRotation = Quaternion.identity;
//                go.transform.localScale = Vector3.one;
//            }
//        }
//    }
//}
