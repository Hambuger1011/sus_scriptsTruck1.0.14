using pb;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityItem : MonoBehaviour {

    public GameObject Item, ItemParen;

    private List<ActivityItemChile> ActivityItemChileList;

    public void ActivityItemInit()
    {
        ActivityItemChileList = new List<ActivityItemChile>();
        ActivityItemChileList.Clear();

        Item.SetActive(false);
        int number = int.Parse(gameObject.name);

        t_Activity data1 = GameDataMgr.Instance.table.ActivityStateListReturn(number * 2);
        
        if (data1==null)
        {
            return;
        }else
        {
            GameObject go1 = Instantiate(Item, ItemParen.transform);
            go1.SetActive(true);
            //go1.GetComponent<ActivityItemChile>().ActivityItemChilInit(data1);

            ActivityItemChile item = go1.GetComponent<ActivityItemChile>();
            item.ActivityItemChilInit(data1);
            ActivityItemChileList.Add(item);

            t_Activity data2 = GameDataMgr.Instance.table.ActivityStateListReturn(number * 2+1);
            if (data2 == null)
            {
                return;
            }else
            {
                GameObject go2 = Instantiate(Item, ItemParen.transform);
                go2.SetActive(true);
                //go2.GetComponent<ActivityItemChile>().ActivityItemChilInit(data2);

                ActivityItemChile item2 = go2.GetComponent<ActivityItemChile>();
                item2.ActivityItemChilInit(data2);
                ActivityItemChileList.Add(item2);
            }
        }
    }

    public void DisPoste()
    {
        if (ActivityItemChileList!=null)
        {
            for (int i=0;i< ActivityItemChileList.Count;i++)
            {
                ActivityItemChileList[i].DisPoste();
            }
        }
       

        Destroy(gameObject);
    }
}
