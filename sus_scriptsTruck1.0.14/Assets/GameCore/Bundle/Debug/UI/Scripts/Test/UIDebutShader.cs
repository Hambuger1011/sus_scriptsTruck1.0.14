//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using Framework;
//using UniGameLib;
//using AB;
//using System.IO;

//public class UIDebutShader : MonoBehaviour
//{
//    class CData
//    {
//        public string strTitle;
//        public string strResPath;
//        public CData(string tile,string resPath)
//        {
//            this.strTitle = tile;
//            this.strResPath = resPath;
//        }
//    }

//    public Button btnQuit;
//    public CUITableView uiTableView;
//    public GameObject itemPfb;
//    List<CData> data = new List<CData>();

//    private void Awake()
//    {
//        UnitySceneMgr.CreateInstance();
//        itemPfb.SetActiveEx(false);
//    }

//    private void Start()
//    {
//        foreach(var itr in ABConfiguration.Instance.abConfItems)
//        {
//            string asset = itr.assetNames[0];
//            var ex = Path.GetExtension(asset);
//            if(ex != ".unity")
//            {
//                continue;
//            }
//            if (asset.StartsWith("Assets/Bundle/LearnDemo/", System.StringComparison.CurrentCultureIgnoreCase))
//            {
//                var name = Path.GetFileNameWithoutExtension(asset);
//                data.Add(new CData(name,asset));
//            }
//        }
//        data.Sort((x, y) => { return x.strTitle.CompareTo(y.strResPath); });

//        uiTableView.onInitItem = OnInitItem;
//        uiTableView.SetItemCount(data.Count);
//    }

//    private void OnInitItem(ref CTableCell cell, int index, int cellNum)
//    {
//        if (cell == null)
//        {
//            GameObject go = GameObject.Instantiate<GameObject>(itemPfb);
//            go.SetActiveEx(true);
//            RectTransform trans = go.GetComponent<RectTransform>();
//            trans.SetParent(uiTableView.transform, false);
//            cell = new CTableCell(index, trans);
//        }
//        var d = data[index];
//        cell.trans.Find("Text").GetComponent<Text>().text = d.strTitle;
//        var c = cell;
//        EventTriggerListener.Get(cell.trans.gameObject).onClick = (g) => { OnSelect(c, d); };
//    }

//    int m_resTag = 0;

//    void OnSelect(CTableCell cell,CData data)
//    {
//        ABMgrImpl.Instance.ClearAssetTag((enResTag)m_resTag);
//        m_resTag += 1;
//        //data.strResPath = "assets/bundle/learndemo/mylearn/learn_02/002_2d/rain01/2DRain.unity";
//        LOG.Error(data.strResPath);
//        UnitySceneMgr.Instance.LoadBundleSceneImme((enResTag)m_resTag, data.strResPath);
//    }
//}
