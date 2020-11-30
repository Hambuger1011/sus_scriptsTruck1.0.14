using Framework;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableViewDemo : MonoBehaviour {
    public CUITableView table;
    public int count;
    public GameObject itemPfb;

	// Use this for initialization
	void Start () 
    {
        itemPfb.SetActiveEx(false);
        table.onInitItem += OnInitItem;
        table.SetItemCount(count);
        //table.DoLayout();
    }

    void OnInitItem(ref CTableCell cell, int index, int cellNum)
    {
        if(cell == null)
        {
            GameObject go = GameObject.Instantiate<GameObject>(itemPfb, table.rectTransform);
            go.SetActiveEx(true);
            cell = new CTableCell(index, go.GetComponent<RectTransform>());
        }
        var trans = cell.trans;
        trans.name = string.Concat("Item_",index);
        trans.Find("Text").GetComponent<Text>().text = index.ToString();
    }
}
