using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace ThisisGame
{

    public class TestEmailUI : BaseUIForm
    {

        InfinityGridLayoutGroup infinityGridLayoutGroup;

        private  int amount = 20;//设定列表数据的总的数量
        public Transform Content;

        private bool isnofirst = false;
        // Use this for initialization
        public  void Init(int amounts)
        {
            ////初始化数据列表;
            LOG.Info("复列初始化了");
            LOG.Info("amounts:" + amounts);
            this.amount = amounts;
            infinityGridLayoutGroup = Content.GetComponent<InfinityGridLayoutGroup>();
            infinityGridLayoutGroup.SetAmount(amount);

            if (!isnofirst)
            {
                isnofirst = true;
                infinityGridLayoutGroup.updateChildrenCallback = UpdateChildrenCallback;
            }

        }

        void UpdateChildrenCallback(int index, Transform trans)
        {
            //LOG.Info("UpdateChildrenCallback: index=" + index + " name:" + trans.name);

            //Text text = trans.Find("Text").GetComponent<Text>();
            //text.text = index.ToString();
            
            ArrayList arr = MyBooksDisINSTANCE.Instance.returNewGoList();
            if (index<arr.Count)
            {
                if (arr[index] != null)
                {
                    trans.GetComponent<newItemSprite>().Init((NewInfoList)arr[index]);

                }
            }
            else
            {
                int ss = arr.Count;
                LOG.Info("新闻滑到了最后的一个数据，该加载新的新闻了,数组中的数据："+ss);
                transform.GetComponent<EmailForm>().NewPagUpdate();           
            }                       
        }      
    }

}
