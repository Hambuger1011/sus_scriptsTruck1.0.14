using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Reflection;
using UnityEngine.SceneManagement;
using AB;
using System.Diagnostics;
using System.Threading;
using System.Text;
using UnityEngine.Profiling;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;
using UGUI;

public class UIDumpRes : MonoBehaviour
{
    class CData
    {
        public string strTitle;
        public string strResPath;
        public Object obj;
        public CData(string tile, string resPath, Object o)
        {
            this.strTitle = tile;
            this.strResPath = resPath;
            this.obj = o;
        }
    }

    public Dropdown searchType;
    public Button btnClose;
    public Button btnSearch;
    public InputField inFilterRegex;
    public Image imgSeer;

    public CUITableView uiSearchResultTableView;
    public GameObject prefabButton;
    List<CData> data = new List<CData>();

    Dictionary<IntPtr, Sprite> m_sptDict = new Dictionary<IntPtr, Sprite>();

    delegate void OnSearchResRunc(enResType resType);
    OnSearchResRunc[] onSearchResFuncList;

    public class TestAttribute : Attribute
    {
        public string name;

        public TestAttribute(string strName)
        {
            this.name = strName;
        }
    }

    ScreenOrientation m_currentOrientation;
    void Awake()
    {
        m_currentOrientation = Screen.orientation;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        btnClose.onClick.AddListener(() =>
        {
            this.GetComponent<CUIForm>().Close();
        });
        prefabButton.SetActiveEx(false);
        btnSearch.onClick.AddListener(OnSearchClick);
        imgSeer.GetComponent<Button>().onClick.AddListener(HidTexture2DSeer);
        //CreateButton("刷新", DumpAb);
        //DumpAb();
        List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        for(int i=1,iMax = (int)enResType.eMax; i < iMax; ++ i)
        {
            var data = new Dropdown.OptionData();
            data.text = ((enResType)i).ToString();
            list.Add(data);
        }
        searchType.options = list;
        onSearchResFuncList = new OnSearchResRunc[(int)enResType.eMax];
        for (int i = 1, iMax = (int)enResType.eMax; i < iMax; ++i)
        {
            switch((enResType)i)
            {
                default:
                    onSearchResFuncList[i] = OnSearchResCommon;
                    break;
            }
        }
        uiSearchResultTableView.onInitItem = OnInitItem;
        //uiSearchResultTableView.SetItemCount(data.Count);
    }

    void OnDestroy()
    {
        foreach (var itr in m_sptDict)
        {
            Object.Destroy(itr.Value);
        }
        m_sptDict.Clear();
        Resources.UnloadUnusedAssets();
        Screen.orientation = m_currentOrientation;
    }
    void OnSearchClick()
    {

        int k = searchType.value + 1;
        onSearchResFuncList[k]((enResType)k);
    }

    #region Result
    private void OnInitItem(ref CTableCell cell, int index, int cellNum)
    {
        if (cell == null)
        {
            GameObject go = GameObject.Instantiate<GameObject>(prefabButton);
            go.SetActiveEx(true);
            RectTransform trans = go.GetComponent<RectTransform>();
            trans.SetParent(uiSearchResultTableView.transform, false);
            cell = new CTableCell(index, trans);
        }
        var d = data[index];
        cell.trans.Find("Text").GetComponent<Text>().text =index+"."+ d.strTitle+" "+d.strResPath;
        var c = cell;
        EventTriggerListener.Get(cell.trans.gameObject).onClick = (g) => { OnSelect(c, d); };
    }
    

    void OnSelect(CTableCell cell, CData data)
    {
    }
    #endregion

    private void OnSearchResCommon(enResType resType)
    {
        Type t = typeof(Object);
        switch (resType)
        {
            case enResType.ePrefab:
                t = typeof(GameObject);
                break;
            case enResType.eAudio:
                t = typeof(AudioClip);
                break;
            case enResType.eText:
                t = typeof(TextAsset);
                break;
            case enResType.eSprite:
                t = typeof(Sprite);
                break;
            case enResType.eTexture2D:
                t = typeof(Texture2D);
                break;
            case enResType.eFont:
                t = typeof(Font);
                break;
            case enResType.eScriptableObject:
                t = typeof(ScriptableObject);
                break;
            case enResType.eShader:
                t = typeof(Shader);
                break;
            default:
                break;
        }
        Clear();
        Regex filterRegex = null;
        if (!string.IsNullOrEmpty(inFilterRegex.text))
        {
            filterRegex = new Regex(inFilterRegex.text);
        }

        long totalSize = 0;
        var objs = Resources.FindObjectsOfTypeAll(t);
        int cnt = 0;
        foreach (var obj in objs)
        {
#if UNITY_EDITOR
            if (obj.hideFlags == HideFlags.HideAndDontSave)
            {
                //continue;
            }
#endif

            var s = Profiler.GetRuntimeMemorySizeLong(obj);

            if (filterRegex != null && !filterRegex.IsMatch(obj.name))
            {
                continue;
            }

            totalSize += s;
            if (s < 1024 * 1024 * 0.2)
            {
                //continue;
            }
            CData d = new CData(obj.name, GameUtility.FormatBytes(s) + "," + obj.hideFlags, obj);
            data.Add(d);
        }
        uiSearchResultTableView.SetItemCount(data.Count);
        (inFilterRegex.placeholder as Text).text = string.Format("总数:{0}${1}", cnt, GameUtility.FormatBytes(totalSize));
    }

    GameObject CreateButton(string strText, UnityAction func = null)
    {
        GameObject go;
        if (mObjPool.Count > 0)
        {
            go = mObjPool.Dequeue();
        }
        else
        {
            go = GameObject.Instantiate(prefabButton);
            go.transform.SetParent(prefabButton.transform.parent, false);
        }
        go.transform.SetAsLastSibling();
        go.SetActiveEx(true);
        go.GetComponentInChildren<Text>().text = strText;
        if (func != null)
        {
            go.GetComponent<Button>().onClick.RemoveAllListeners();
            go.GetComponent<Button>().onClick.AddListener(func);
        }
        this.mObjList.Add(go);
        return go;
    }

    #region all dump
    public void DumpAb()
    {
        //onSearchFunc = () =>
        //{
        //    Clear();
        //    Regex filterRegex = null;
        //    if (!string.IsNullOrEmpty(inFilterRegex.text))
        //    {
        //        filterRegex = new Regex(inFilterRegex.text);
        //    }

        //    int cnt = 0;
        //    var dic = ABMgr.Instance.bundleCache;
        //    foreach (var itr in dic)
        //    {
        //        var ab = itr.Value;
        //        if (filterRegex != null && !filterRegex.IsMatch(ab.abConfigItem.name))
        //        {
        //            continue;
        //        }
        //        ++cnt;
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(string.Format("[<color=green>{0}</color>]{1}\n引用：", ab.abConfigItem.name, GameUtility.FormatBytes(Profiler.GetRuntimeMemorySize(ab.assetbundle))));
        //        sb.AppendLine();
        //        int idx = 0;
        //        foreach (var itr2 in ab.GetReferenceSet())
        //        {
        //            ++idx;
        //            string strColor = "Black";
        //            if (idx % 2 == 0)
        //            {
        //                strColor = "Blue";
        //            }
        //            sb.Append(string.Format("<color={0}>{1}.{2}</color>", strColor, idx, itr2 != null ? itr2 : "null"));
        //            sb.AppendLine();
        //        }
        //        CreateButton(sb.ToString());
        //    }
        //    (inFilterRegex.placeholder as Text).text = "总数:" + cnt;
        //};
    }


    public void DumpAsset()
    {
        //onSearchFunc = () =>
        //{
        //    Clear();
        //    Regex filterRegex = null;
        //    if (!string.IsNullOrEmpty(inFilterRegex.text))
        //    {
        //        filterRegex = new Regex(inFilterRegex.text);
        //    }

        //    int cnt = 0;
        //    var dic = ABMgr.Instance.assetCache;
        //    foreach (var itr in dic)
        //    {
        //        var ab = itr.Value;

        //        if (filterRegex != null && !filterRegex.IsMatch(ab.assetName))
        //        {
        //            continue;
        //        }

        //        ++cnt;
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(string.Format("[<color=green>{0}</color>]{1}\n引用：", ab.assetName, GameUtility.FormatBytes(Profiler.GetRuntimeMemorySize(ab.resObject))));
        //        sb.AppendLine();
        //        int idx = 0;
        //        foreach (var itr2 in ab.GetReferenceSet())
        //        {
        //            ++idx;
        //            string strColor = "Black";
        //            if (idx % 2 == 0)
        //            {
        //                strColor = "Blue";
        //            }
        //            sb.Append(string.Format("<color={0}>{1}.{2}</color>", strColor, idx, itr2 != null ? itr2 : "null"));
        //            sb.AppendLine();
        //        }
        //        CreateButton(sb.ToString());
        //    }
        //    (inFilterRegex.placeholder as Text).text = "总数:" + cnt;
        //};
    }
    #endregion


    #region 图集
    public void DumpAtlasAB()
    {
        //onSearchFunc = () =>
        //{
        //    Clear();
        //    Regex filterRegex = null;
        //    if (!string.IsNullOrEmpty(inFilterRegex.text))
        //    {
        //        filterRegex = new Regex(inFilterRegex.text);
        //    }

        //    int cnt = 0;
        //    var dic = ABMgr.Instance.bundleCache;
        //    foreach (var itr in dic)
        //    {
        //        var ab = itr.Value;
        //        if (ab.abConfigItem.abType != enResType.eAtlas)
        //        {
        //            continue;
        //        }

        //        if (filterRegex != null && !filterRegex.IsMatch(ab.abConfigItem.name))
        //        {
        //            continue;
        //        }

        //        ++cnt;
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(string.Format("[<color=green>{0}</color>]{1}\n引用：", ab.abConfigItem.name, GameUtility.FormatBytes(Profiler.GetRuntimeMemorySize(ab.assetbundle))));
        //        sb.AppendLine();
        //        int idx = 0;
        //        foreach (var itr2 in ab.GetReferenceSet())
        //        {
        //            ++idx;
        //            string strColor = "Black";
        //            if (idx % 2 == 0)
        //            {
        //                strColor = "Blue";
        //            }
        //            sb.Append(string.Format("<color={0}>{1}.{2}</color>", strColor, idx, itr2 != null ? itr2 : "null"));
        //            sb.AppendLine();
        //        }
        //        CreateButton(sb.ToString());
        //    }
        //    (inFilterRegex.placeholder as Text).text = "总数:" + cnt;
        //};
    }



    public void DumpAtlasAsset()
    {
        //onSearchFunc = () =>
        //{
        //    Clear();
        //    Regex filterRegex = null;
        //    if (!string.IsNullOrEmpty(inFilterRegex.text))
        //    {
        //        filterRegex = new Regex(inFilterRegex.text);
        //    }

        //    int cnt = 0;
        //    var dic = ABSystem.Instance.;
        //    foreach (var itr in dic)
        //    {
        //        var ab = itr.Value;
        //        if (ab.abConfigItem.abType != enResType.eAtlas)
        //        {
        //            continue;
        //        }

        //        if (filterRegex != null && !filterRegex.IsMatch(ab.assetName))
        //        {
        //            continue;
        //        }

        //        ++cnt;
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append(string.Format("[<color=green>{0}</color>]{1}\n引用：", ab.assetName, GameUtility.FormatBytes(Profiler.GetRuntimeMemorySize(ab.resObject))));
        //        sb.AppendLine();
        //        int idx = 0;
        //        foreach (var itr2 in ab.GetReferenceSet())
        //        {
        //            ++idx;
        //            string strColor = "Black";
        //            if (idx % 2 == 0)
        //            {
        //                strColor = "Blue";
        //            }
        //            sb.Append(string.Format("<color={0}>{1}.{2}</color>", strColor, idx, itr2 != null ? itr2 : "null"));
        //            sb.AppendLine();
        //        }
        //        CreateButton(sb.ToString());
        //    }
        //    (inFilterRegex.placeholder as Text).text = "总数:" + cnt;
        //};
    }


    public void DumpTexture2D()
    {
//        onSearchFunc = () =>
//        {
//            Clear();
//            Regex filterRegex = null;
//            if (!string.IsNullOrEmpty(inFilterRegex.text))
//            {
//                filterRegex = new Regex(inFilterRegex.text);
//            }

//            int totalSize = 0;
//            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
//            int cnt = 0;
//            SortedList<int, Texture2D> list = new SortedList<int, Texture2D>(new SizeCmp());
//            foreach (var itr in textures)
//            {
//#if UNITY_EDITOR
//                if (itr.hideFlags == HideFlags.HideAndDontSave)
//                {
//                    continue;
//                }
//#endif

//                var s = Profiler.GetRuntimeMemorySize(itr);

//                if (filterRegex != null && !filterRegex.IsMatch(itr.name))
//                {
//                    continue;
//                }

//                totalSize += s;
//                if (s < 1024 * 1024 * 0.2)
//                {
//                    continue;
//                }

//                list.Add(s, itr);
//            }

//            foreach (var i in list)
//            {
//                var s = i.Key;
//                var itr = i.Value;
//                ++cnt;
//                Sprite spt;
//                if (!m_sptDict.TryGetValue(itr.GetNativeTexturePtr(), out spt))
//                {
//                    spt = Sprite.Create(itr, new Rect(0, 0, itr.width, itr.height), Vector2.zero);
//                    m_sptDict.Add(itr.GetNativeTexturePtr(), spt);
//                }
//                StringBuilder sb = new StringBuilder();
//                sb.Append(string.Format("[<color=green>{0}</color>]{1}({2}*{3}${4})", itr.name, itr.format, itr.width, itr.height, GameUtility.FormatBytes(s)));
//                var go = CreateButton(sb.ToString(), () =>
//                {
//                    ShowTexture2DSeer(spt);
//                });
//                //go.GetComponent<VerticalLayoutGroup>().enabled = false;
//                var texTrans = go.transform.Find("Tex");
//                if (texTrans == null)
//                {
//                    texTrans = new GameObject("Tex").transform;
//                    texTrans.SetParent(go.transform, false);
//                    var ui = texTrans.gameObject.AddComponent<Image>();
//                    ui.rectTransform.anchorMin = new Vector2(1, 0);
//                    ui.rectTransform.anchorMax = new Vector2(1, 1);
//                    ui.rectTransform.offsetMin = new Vector2(0, 0);
//                    ui.rectTransform.anchorMax = new Vector2(200, 0);
//                    texTrans = ui.rectTransform;
//                }
//                var img = texTrans.GetComponent<Image>();
//                img.sprite = spt;
//            }
//            (inFilterRegex.placeholder as Text).text = string.Format("总数:{0}${1}", cnt, GameUtility.FormatBytes(totalSize));
//        };
    }

    public void DumpShader()
    {
        //onSearchFunc = () =>
        {
            Clear();
            Regex filterRegex = null;
            if (!string.IsNullOrEmpty(inFilterRegex.text))
            {
                filterRegex = new Regex(inFilterRegex.text);
            }

            int totalSize = 0;
            var shaders = Resources.FindObjectsOfTypeAll<Shader>();
            int cnt = 0;
            SortedList<int, Shader> list = new SortedList<int, Shader>(new SizeCmp());
            foreach (var itr in shaders)
            {
#if UNITY_EDITOR
                if (itr.hideFlags == HideFlags.HideAndDontSave)
                {
                    continue;
                }
#endif

                var s = Profiler.GetRuntimeMemorySize(itr);

                if (filterRegex != null && !filterRegex.IsMatch(itr.name))
                {
                    continue;
                }

                totalSize += s;
                //if (s < 1024 * 1024 * 0.2)
                //{
                //    continue;
                //}

                list.Add(s, itr);
            }

            foreach (var i in list)
            {
                var s = i.Key;
                var itr = i.Value;
                ++cnt;

                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("[<color=green>{0}</color>]{1}\nisSupported:{2} renderQueue:{3}", itr.name, GameUtility.FormatBytes(s), itr.isSupported, itr.renderQueue));
                var go = CreateButton(sb.ToString());
            }
            (inFilterRegex.placeholder as Text).text = string.Format("总数:{0}${1}", cnt, GameUtility.FormatBytes(totalSize));
        };
    }

    class SizeCmp : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            if (x == y)
            {
                return 1;
            }
            return y - x;
        }
    }
    #endregion



    Queue<GameObject> mObjPool = new Queue<GameObject>();
    List<GameObject> mObjList = new List<GameObject>();

    void Clear()
    {
        data.Clear();
        for (int i = 0; i < mObjList.Count; ++i)
        {
            var go = mObjList[i];
            go.SetActiveEx(false);
            //go.transform.SetParent(this.transform, false);
            mObjPool.Enqueue(go);
        }
        mObjList.Clear();
    }

    

    void ShowTexture2DSeer(Sprite spt)
    {
        imgSeer.gameObject.SetActiveEx(true);
        imgSeer.sprite = spt;
        imgSeer.SetNativeSize();
    }

    void HidTexture2DSeer()
    {
        imgSeer.gameObject.SetActiveEx(false);
        imgSeer.sprite = null;
    }
}

