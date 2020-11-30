namespace AB
{
#if UNITY_EDITOR
    using System.Linq;
#endif
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.IO;
    using UnityEngine.Profiling;
    using Framework;
    using UnityEngine.UI;

    public class AbAtlasMgr : CSingleton<AbAtlasMgr>
    {

        Material _defaultGreyMat = null;
        public Material defaultGreyMat
        {
            get
            {
                if (_defaultGreyMat == null)
                {
                    _defaultGreyMat = new Material(Shader.Find("UI/Default_Grey"));
                }
                return _defaultGreyMat;
            }
        }

        public Dictionary<Texture2D, AbAtlas> atlasDict = new Dictionary<Texture2D, AbAtlas>();
        public void Add(Texture2D tex, AbAtlas data)
        {
            if (atlasDict.ContainsKey(tex))
            {
                atlasDict[tex] = data;
            }
            else
            {
                atlasDict.Add(tex, data);
            }
        }

        public void Remove(Texture2D tex)
        {
            atlasDict.Remove(tex);
        }

        public AbAtlas Get(Texture2D tex)
        {
            AbAtlas data;
            atlasDict.TryGetValue(tex, out data);
            return data;
        }

        public AbAtlas GetUIAtlas(string atlasName)
        {
            var data = ABSystem.Instance.LoadImme(AbTag.Debug, enResType.ePrefab, "assets/bundle/atlas/" + atlasName + ".prefab");
            return data.Get<AbAtlas>();
        }
        protected override void Init()
        {
            base.Init();
        }


        Dictionary<enAtlas, string> m_atlasNameMap = new Dictionary<enAtlas, string>();
        Dictionary<enAtlas, AbAtlas> m_atlasLoadedMap = new Dictionary<enAtlas, AbAtlas>();
        //public void RegisterAtlas()
        //{
        //    for (int i = 0, iMax = (int)enAtlas.Count; i < iMax; ++i)
        //    {
        //        var key = enAtlas.Share + i;
        //        var assetName = "assets/bundle/atlas/" + key + ".prefab";
        //        m_atlasNameMap.Add(key, assetName);
        //        LoadAtlas(key, assetName);
        //    }
        //}

        public void LoadAtlas(enAtlas key, string assetName, Action<AbAtlas> callback = null)
        {
            ABSystem.Instance.LoadAsync(AbTag.Atlas, enResType.ePrefab, assetName, (_) =>
            {
                var data = _.Get<AbAtlas>();
                if (!m_atlasLoadedMap.ContainsKey(key))
                {
                    m_atlasLoadedMap.Add(key, data);
                }
                if (callback != null) callback(data);
            });
        }

        public AbAtlas Get(enAtlas key)
        {
            AbAtlas data;
            if (!m_atlasLoadedMap.TryGetValue(key, out data))
            {
                var assetName = m_atlasNameMap[key];
                var asset = ABSystem.Instance.LoadImme(AbTag.Atlas, enResType.ePrefab, assetName);
                data = asset.Get<AbAtlas>();
                m_atlasLoadedMap.Add(key, data);
            }
            return data;
        }
        public void SetSprite(Image img, enAtlas key, string sptName, bool applyMaterial = true, bool nativeSize = false)
        {
            AbAtlas data;
            if (m_atlasLoadedMap.TryGetValue(key, out data))
            {
                data.SetSprite(img, sptName, applyMaterial, nativeSize);
            }
            else
            {
                var assetName = m_atlasNameMap[key];
                LoadAtlas(key, assetName, (d) =>
                {
                    d.SetSprite(img, sptName, applyMaterial, nativeSize);
                });
            }
        }

        public void SetGrey(GameObject go, bool isGrey)
        {
            var graphics = go.GetComponentsInChildren<Graphic>(true);
            CUIGrey[] uiGrey = new CUIGrey[graphics.Length];
            for (int i = 0; i < graphics.Length; ++i)
            {
                var obj = graphics[i].gameObject;
                uiGrey[i] = obj.GetComponent<CUIGrey>();
                if (uiGrey[i] == null)
                {
                    uiGrey[i] = obj.AddComponent<CUIGrey>();
                    uiGrey[i].Init(graphics[i]);
                }

                uiGrey[i].SetGrey(isGrey);
            }
        }

    }



    public enum enAtlas
    {
        Share,
        Item,
        Singin,
        Main,
        Setting,
        Shop,
        Map,
        GameLoading,
        Game,
        Game2,
        Count,
    }
}